using Microsoft.CodeAnalysis.Diagnostics;

namespace Demo.AssemblyAnnotations.Analyzers;

/// <summary>
/// MsBuild Property Helper Extension Methods
/// </summary>
internal static class MsBuildPropertyHelpers
{
    /// <summary>
    /// Gets the Required MsBuild Property
    /// </summary>
    /// <param name="optionsProvider">The <see cref="AnalyzerConfigOptionsProvider"/>.</param>
    /// <param name="name">The property name</param>
    /// <param name="defaultValue">The default value if not present</param>
    /// <returns>The MsBuild Property value.</returns>
    public static string? GetMsBuildProperty(
        this AnalyzerConfigOptionsProvider optionsProvider,
        string name,
        string? defaultValue = null)
    {
        optionsProvider.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
        return value ?? defaultValue;
    }
}
