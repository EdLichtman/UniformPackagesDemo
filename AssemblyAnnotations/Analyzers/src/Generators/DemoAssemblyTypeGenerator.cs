using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Demo.AssemblyAnnotations.Abstractions;

namespace Demo.AssemblyAnnotations.Analyzers.Generators;

/// <summary>
/// The AssemblyType Incremental Generator
/// </summary>
[Generator]
public class DemoAssemblyTypeGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.AnalyzerConfigOptionsProvider, Generate);
    }

    private static void Generate(SourceProductionContext context, AnalyzerConfigOptionsProvider options)
    {
        context.AddSource("Demo.Test.g.cs", "// this is a test");
        var rootNamespace = options.GetMsBuildProperty("RootNamespace");
        if (
            rootNamespace != null && TryGetAssemblyTypeAnnotation(options, out var assemblyType))
        {
            context.AddSource("DemoAssemblyType.g.cs",
                GetStandardGeneratedFile(rootNamespace, GetDemoAssemblyTypeAssemblyDeclarationCode(assemblyType!.Value)));
        }
    }

    private static string GetDemoAssemblyTypeAssemblyDeclarationCode(DemoAssemblyType DemoAssemblyType)
    {
        return $@"
public enum DemoAssemblyType {{
    {string.Join(",", Enum.GetNames(typeof(DemoAssemblyType)))}
}}
public partial class DemoAssemblyAnnotations {{
    public const {nameof(DemoAssemblyType)} AssemblyType = {nameof(DemoAssemblyType)}.{DemoAssemblyType};
}}
";
    }
}
