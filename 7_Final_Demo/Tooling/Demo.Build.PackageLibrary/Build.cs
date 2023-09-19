using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Demo.Build.PackageLibrary;

/// <summary>
/// The main build script.
/// </summary>
public class Build : NukeBuild
{
    /// <summary>
    /// The <see cref="AbsolutePath"/> to the solution. 
    /// </summary>
    [Parameter("The solution file to sync this proj structure within.")]
    private readonly AbsolutePath? solution;

    /// <summary>
    /// Gets the Target that Syncs the solution
    /// </summary>
    public Target SyncSolution => _ => _
        .Executes(() =>
        {
            var solutionFile = FindSolution(this.solution);

            var solutionRepresentation = solutionFile.Exists()
                ? SolutionModelTasks.ParseSolution(solutionFile)
                : SolutionModelTasks.CreateSolution(solutionFile, folderNameProvider: sol => sol.FileName);

            ClearSolution(solutionRepresentation);
            AddProjects(solutionRepresentation);
            AddSolutionFiles(solutionRepresentation);
            solutionRepresentation.Save();
        });

    /// <summary>
    /// The Main Execute method
    /// </summary>
    /// <returns>The exit code</returns>
    public static int Main()
    {
        return Execute<Build>();
    }

    /// <summary>
    /// Finds the Solution from root.
    /// </summary>
    /// <param name="solutionFile">The proposed solution file path.</param>
    /// <returns>The <see cref="AbsolutePath"/> of the solution.</returns>
    private static AbsolutePath FindSolution(AbsolutePath? solutionFile)
    {
        AbsolutePath? foundSolution = solutionFile;
        var workingDirectory = RootDirectory;
        while (foundSolution == null && workingDirectory.Parent != null && workingDirectory.Parent != workingDirectory)
        {
            var solutions = workingDirectory.GlobFiles("*.sln");
            if (solutions.Any())
            {
                if (solutions.Count > 1)
                {
                    Console.WriteLine("Solutions:");
                    foreach (var target in solutions)
                    {
                        Console.WriteLine(target);
                    }

                    Assert.Fail("Found more than one Solution in directory. Could not determine Solution to add project to.");
                }

                foundSolution = solutions.First();
            }

            workingDirectory = workingDirectory.Parent;
        }

        if (foundSolution == null)
        {
            Assert.Fail("No valid Solution found and none provided. Try again while including the 'solution' parameter");
        }

        if (foundSolution?.Extension != ".sln")
        {
            Assert.Fail("'solution' provided was not a valid .sln file.");
        }

        return foundSolution!;
    }

    /// <summary>
    /// Clears the solution
    /// </summary>
    /// <param name="target">The <see cref="Solution"/> to clear</param>
    private static void ClearSolution(Solution target)
    {
        foreach (var project in target.AllProjects)
        {
            target.RemoveProject(project);
        }

        foreach (var folder in target.AllSolutionFolders)
        {
            target.RemoveSolutionFolder(folder);
        }
    }

    /// <summary>
    /// Adds Projects to the <see cref="Solution"/>
    /// </summary>
    /// <param name="target">The target <see cref="Solution"/>.</param>
    private static void AddProjects(Solution target)
    {
        var projects = target.Path!.Parent.GlobFiles("**/*.csproj");
        foreach (var project in projects)
        {
            AddProjects(target, project);
        }
    }

    /// <summary>
    /// Adds Projects to the <see cref="Solution"/>
    /// </summary>
    /// <param name="target">The target <see cref="Solution"/>.</param>
    /// <param name="projectPath">The Project Path</param>
    private static void AddProjects(Solution target, AbsolutePath projectPath)
    {
        var relativePath = target.Path!.Parent
            .GetRelativePathTo(projectPath)
            .ToUnixRelativePath();

        var project = relativePath.ToString().Replace(".csproj", string.Empty);
        var subPaths = project.Split("/");
        var projectName = subPaths.Last();
        var typeId = ProjectType.CSharpProject.FirstGuid;
        if (subPaths.Contains("tests", StringComparer.OrdinalIgnoreCase))
        {
            var folder = target.GetSolutionFolder("tests") ?? target.AddSolutionFolder("tests");
            AddBuildConfiguration(target.AddProject(projectName, typeId, projectPath, solutionFolder: folder));
        }
        else
        {
            AddBuildConfiguration(target.AddProject(projectName, typeId, projectPath));
        }
    }

    /// <summary>
    /// Adds Solution Files to the <see cref="Solution"/>
    /// </summary>
    /// <param name="target">The target <see cref="Solution"/>.</param>
    private static void AddSolutionFiles(Solution target)
    {
        var items = target.Path!.Parent.GlobFiles("*").Where(file => file.Name != target.Path.Name).Select(x => x.Name)
            .ToList();
        var solutionFolder = target.AddSolutionFolder("Solution Files");
        foreach (var item in items)
        {
            solutionFolder.Items.Add(item, item);
        }

        var nukeFiles = target.Path.Parent.GlobFiles(".nuke/*").Select(x => x.Name);
        var nukeSolutionFolder = target.AddSolutionFolder(".nuke", solutionFolder: solutionFolder);
        foreach (var nukeFile in nukeFiles.Select(file => $".nuke/{file}"))
        {
            nukeSolutionFolder.Items.Add(nukeFile, nukeFile);
        }
    }

    /// <summary>
    /// Adds the Build Configuration for a project
    /// </summary>
    /// <param name="project">The <see cref="Project"/>.</param>
    private static void AddBuildConfiguration(Project project)
    {
        List<string> knownConfigurations = new()
        {
            "Develop",
            "Debug",
            "Release"
        };

        foreach (var configuration in knownConfigurations)
        {
            project.Configurations.Add($"{configuration}|Any CPU.ActiveCfg", $"{configuration}|Any CPU");
            project.Configurations.Add($"{configuration}|Any CPU.Build.0", $"{configuration}|Any CPU");
        }
    }
}
