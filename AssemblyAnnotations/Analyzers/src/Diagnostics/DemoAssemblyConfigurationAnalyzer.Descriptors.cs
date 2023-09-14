using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Demo.AssemblyAnnotations.Abstractions;

namespace Demo.AssemblyAnnotations.Analyzers.Diagnostics;

public partial class DemoAssemblyConfigurationAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            MissingDemoAssemblyPropertyDiagnosticDescriptor,
            InvalidDemoAssemblyTypeDiagnosticDescriptor,
            MissingDemoPackageAssemblyPropertyDiagnosticDescriptor);

    private static Diagnostic CreateMissingAssemblyPropertyDiagnostic(string propertyName)
    {
        return Diagnostic.Create(MissingDemoAssemblyPropertyDiagnosticDescriptor, null, propertyName);
    }

    private static Diagnostic CreateInvalidAssemblyTypeDiagnostic(string assemblyType)
    {
        return Diagnostic.Create(InvalidDemoAssemblyTypeDiagnosticDescriptor, null, assemblyType);
    }

    private static Diagnostic CreateMissingPackageAssemblyPropertyDiagnostic(string propertyName)
    {
        return Diagnostic.Create(MissingDemoPackageAssemblyPropertyDiagnosticDescriptor, null, propertyName);
    }

    /// <summary>
    /// Gets the Missing CompilerVisibleProperty Diagnostic Descriptor
    /// </summary>
    private static readonly DiagnosticDescriptor MissingDemoAssemblyPropertyDiagnosticDescriptor = new(
        "DemoA1000",
        "Missing DemoAssembly Property",
        "No '{0}' Property was configured, or it was configured but is not visible to the compiler. " +
            "Be sure to include <{0}></{0}> in a <PropertyGroup> and <CompilerVisibleProperty Include=\"{0}\"/> in an <ItemGroup>.",
        "Configuration",
        DiagnosticSeverity.Error,
        true,
        null,
        null,
        "CompilationEnd");

    /// <summary>
    /// Gets the Missing CompilerVisibleProperty Diagnostic Descriptor
    /// </summary>
    private static readonly DiagnosticDescriptor InvalidDemoAssemblyTypeDiagnosticDescriptor = new(
        "DemoA1101",
        "Incorrect DemoAssemblyType",
        "Incorrect AssemblyType was configured. AssemblyType was: \"{0}\". Be sure to configure your application with one of the following " +
        $"DemoAssemblyType Property assignments: {string.Join(", ", Enum.GetNames(typeof(DemoAssemblyType)))}.",
        "Configuration",
        DiagnosticSeverity.Error,
        true,
        null,
        null,
        "CompilationEnd");

    /// <summary>
    /// Gets the Missing CompilerVisibleProperty Diagnostic Descriptor
    /// </summary>
    private static readonly DiagnosticDescriptor MissingDemoPackageAssemblyPropertyDiagnosticDescriptor = new(
        "DemoA1102",
        "Missing Demo PackageAssembly Property",
        $"No '{{0}}' Property was configured, or it was configured but is not visible to the compiler. When building a <DemoAssemblyType>{DemoAssemblyType.PackageAssembly}<DemoAssemblyType>" +
        "Be sure to include <{0}></{0}> in a <PropertyGroup> and <CompilerVisibleProperty Include=\"{0}\"/> in an <ItemGroup>.",
        "Configuration",
        DiagnosticSeverity.Error,
        true,
        null,
        null,
        "CompilationEnd");
}
