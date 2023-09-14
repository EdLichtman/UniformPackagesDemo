namespace Demo.AssemblyAnnotations.Abstractions;

/// <summary>
/// The Allowed DemoAssemblyTypes
/// </summary>
public enum DemoAssemblyType
{
    /// <summary>
    /// Represents a Packable Class Library
    /// </summary>
    PackageAssembly,

    /// <summary>
    /// Represents a Test Library
    /// </summary>
    TestAssembly,

    /// <summary>
    /// Represents a Packable DotNet Tool
    /// </summary>
    DotNetToolAssembly,

    /// <summary>
    /// Represents a WebHost 
    /// </summary>
    WebHostAssembly,

    /// <summary>
    /// Represents a non-packable Satellite assembly
    /// </summary>
    SatelliteAssembly
}
