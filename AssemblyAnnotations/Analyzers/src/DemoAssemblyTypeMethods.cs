using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Demo.AssemblyAnnotations.Abstractions;

namespace Demo.AssemblyAnnotations.Analyzers;

/// <summary>
/// Extension methods for operating with <see cref="DemoAssemblyType"/>
/// </summary>
internal static class DemoAssemblyTypeMethods
{
    public static bool TryGetAssemblyTypeStringAnnotation(
        AnalyzerConfigOptionsProvider optionsProvider,
        out string assemblyType)
    {
        var assemblyTypeString = optionsProvider.GetMsBuildProperty("DemoAssemblyType");
        if (string.IsNullOrWhiteSpace(assemblyTypeString))
        {
            assemblyType = string.Empty;
            return false;
        }

        assemblyType = assemblyTypeString!;
        return true;
    }

    public static bool TryGetAssemblyTypeAnnotation(
        AnalyzerConfigOptionsProvider optionsProvider,
        out DemoAssemblyType? assemblyType)
    {
        if (!TryGetAssemblyTypeStringAnnotation(optionsProvider, out var assemblyTypeString))
        {
            assemblyType = null;
            return false;
        }

        if (!Enum.TryParse<DemoAssemblyType>(assemblyTypeString, out var DemoAssemblyType))
        {
            assemblyType = null;
            return false;
        }

        assemblyType = DemoAssemblyType;
        return true;
    }

    public static bool IsPackageAssembly(this AnalyzerConfigOptionsProvider optionsProvider)
    {
        return TryGetAssemblyTypeAnnotation(
                   optionsProvider,
                   out var DemoAssemblyType)
               && DemoAssemblyType == DemoAssemblyType.PackageAssembly;
    }
}
