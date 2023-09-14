using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Demo.AssemblyAnnotations.Abstractions;

namespace Demo.AssemblyAnnotations.CodeAnalysis.Analyzers.Diagnostics;

/// <summary>
/// The Analyzer for the AppSettings
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AppSettingsAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the Missing CompilerVisibleProperty Diagnostic Descriptor
    /// </summary>
    private static readonly DiagnosticDescriptor MissingAppSettingsDiagnosticDescriptor = new(
        "DemoA1201",
        "Missing App Settings",
        "WebHost assembly requires AppSettings however no AppSettings were included as Additional Files. Be sure to include appSettings.json as AdditionalFiles.",
        "Configuration",
        DiagnosticSeverity.Error,
        true,
        null,
        null,
        "CompilationEnd");

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            MissingAppSettingsDiagnosticDescriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterAdditionalFileAction(EnsureAppSettingsAdditionalFileExists);
    }

    /// <summary>
    /// Ensures AppSettings has an additional File allocation
    /// </summary>
    /// <param name="context">The <see cref="AdditionalFileAnalysisContext"/>.</param>
    private static void EnsureAppSettingsAdditionalFileExists(AdditionalFileAnalysisContext context)
    {
        if (
            !TryGetAssemblyTypeAnnotation(context.Options.AnalyzerConfigOptionsProvider, out var assemblyType)
            || assemblyType != DemoAssemblyType.WebHostAssembly)
        {
            return;
        }

        var appSettingsExists = context.Options.AdditionalFiles.Any(file =>
            file.Path.EndsWith("appSettings.json", StringComparison.OrdinalIgnoreCase));

        if (!appSettingsExists)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                MissingAppSettingsDiagnosticDescriptor,
                null));
        }
    }
}
