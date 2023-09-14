using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Demo.AssemblyAnnotations.Analyzers.Diagnostics;

/// <summary>
/// The Analyzer for the DemoAssemblyType
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public partial class DemoAssemblyConfigurationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(EnsureDemoAssemblyIsConfigured);
        context.RegisterCompilationAction(EnsureDemoAssemblyTypeIsValid);
        context.RegisterCompilationAction(EnsureDemoPackageAssemblyConfigurationIsConfigured);

    }

    private static void EnsureDemoAssemblyIsConfigured(CompilationAnalysisContext context)
    {
        EnsurePropertyIsConfigured(
            context,
            CreateMissingAssemblyPropertyDiagnostic,
            "RootNamespace",
            "DemoAssemblyType");
    }

    /// <summary>
    /// Ensures AppSettings has an additional File allocation
    /// </summary>
    /// <param name="context">The <see cref="AdditionalFileAnalysisContext"/>.</param>
    private static void EnsureDemoAssemblyTypeIsValid(CompilationAnalysisContext context)
    {
        if (TryGetAssemblyTypeStringAnnotation(context.Options.AnalyzerConfigOptionsProvider, out var assemblyType)
            && !TryGetAssemblyTypeAnnotation(context.Options.AnalyzerConfigOptionsProvider, out _))
        {
            context.ReportDiagnostic(CreateInvalidAssemblyTypeDiagnostic(assemblyType));
        }
    }

    private static void EnsureDemoPackageAssemblyConfigurationIsConfigured(CompilationAnalysisContext context)
    {
        if (!context.Options.AnalyzerConfigOptionsProvider.IsPackageAssembly())
        {
            return;
        }

        EnsurePropertyIsConfigured(
            context,
            CreateMissingPackageAssemblyPropertyDiagnostic,
            "Version",
            "Description");
    }

    private static void EnsurePropertyIsConfigured(
        CompilationAnalysisContext context,
        Func<string, Diagnostic> getDiagnostic,
        params string[] requiredAssemblyConfigurations)
    {
        foreach (var configuration in requiredAssemblyConfigurations)
        {
            if (string.IsNullOrWhiteSpace(context.Options.AnalyzerConfigOptionsProvider.GetMsBuildProperty(configuration)))
            {
                context.ReportDiagnostic(getDiagnostic(configuration));
            }
        }
    }
}
