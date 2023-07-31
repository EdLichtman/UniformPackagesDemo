using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using static NuGet.Packaging.PackagingConstants;
// using Nuke.Common.ProjectModel;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>();

    [Parameter("The solution file to sync this proj structure within.")]
    readonly AbsolutePath? solution;

    public Target Sync => _ => _
        .Executes(() =>
        {
            var solutionFile = this.FindSolution();

            var solutionPath = solutionFile.Parent;

            var solutionRepresentation = solutionFile.Exists()
                ? SolutionModelTasks.ParseSolution(solutionFile)
                : SolutionModelTasks.CreateSolution(solutionFile, folderNameProvider: solution => solution.FileName);

            this.ClearSolution(solutionRepresentation);
            this.AddProjects(solutionRepresentation);
            this.AddSolutionFiles(solutionRepresentation);
            solutionRepresentation.Save();
        });

    private AbsolutePath FindSolution()
    {
        AbsolutePath? foundSolution = null;
        var workingDirectory = RootDirectory;
        while (workingDirectory.Parent != null && workingDirectory.Parent != workingDirectory)
        {
            var solutions = workingDirectory.GlobFiles("*.sln");
            if (solutions.Any())
            {
                if (solutions.Count > 1)
                {
                    Console.WriteLine("Solutions:");
                    foreach (var solution in solutions)
                    {
                        Console.WriteLine(solution);
                    }
                    Assert.Fail("Found more than one Solution in directory. Could not determine Solution to add project to.");
                }

                foundSolution = solutions.First();
            }

            workingDirectory = workingDirectory.Parent;
        }

        var solutionFile = solution ?? foundSolution;

        if (solutionFile == null)
        {
            Assert.Fail("No valid Solution found and none provided. Try again while including the 'solution' parameter");
        }

        if (solutionFile?.Extension != ".sln")
        {
            Assert.Fail("'solution' provided was not a valid .sln file.");
        }

        return solutionFile!;
    }

    private void ClearSolution(Solution solution)
    {
        foreach (var project in solution.AllProjects)
        {
            solution.RemoveProject(project);
        }

        foreach (var folder in solution.AllSolutionFolders)
        {
            solution.RemoveSolutionFolder(folder);
        }
    }

    private void AddProjects(Solution solution)
    {
        var projects = solution.Path.Parent.GlobFiles("**/*.csproj");
        foreach (var project in projects)
        {
            this.AddProjects(solution, project);
        }
    }

    private void AddProjects(Solution solution, AbsolutePath projectPath)
    {
        var relativePath = solution.Path.Parent
            .GetRelativePathTo(projectPath)
            .ToUnixRelativePath();

        var project = relativePath.ToString().Replace(".csproj", string.Empty);
        var subPaths = project.Split("/");
        if (subPaths.Contains("tests", StringComparer.OrdinalIgnoreCase))
        {
            var folder = solution.GetSolutionFolder("tests") ?? solution.AddSolutionFolder("tests");
            solution.AddProject(project, Guid.NewGuid(), projectPath, solutionFolder: folder);
        }
        else
        {
            solution.AddProject(project, Guid.NewGuid(), projectPath);
        }
    }

    private void AddSolutionFiles(Solution solution)
    {
        var items = solution.Path.Parent.GlobFiles("*").Where(file => file.Name != solution.Path.Name).Select(x => x.Name).ToList();
        var solutionFolder = solution.AddSolutionFolder("Solution Files");
        foreach (var item in items)
        {
            solutionFolder.Items.Add(item, item);
        }

        var nukeFiles = solution.Path.Parent.GlobFiles(".nuke/*").Select(x => x.Name);
        var nukeSolutionFolder = solution.AddSolutionFolder(".nuke", solutionFolder: solutionFolder);
        foreach (var nukeFile in nukeFiles.Select(file => $".nuke/{file}"))
        {
            nukeSolutionFolder.Items.Add(nukeFile, nukeFile);
        }
    }
}
