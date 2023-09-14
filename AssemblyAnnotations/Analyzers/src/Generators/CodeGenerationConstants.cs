﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Demo.AssemblyAnnotations.Analyzers.Generators;

/// <summary>
/// The Constants to be used in code generation.
/// </summary>
public static class CodeGenerationConstants
{
    /// <summary>
    /// The Auto-Generated Header
    /// </summary>
    public const string AutoGeneratedHeader = @"
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
";

    /// <summary>
    /// The Standard Using Statements
    /// </summary>
    public const string StandardUsingStatements = @"
using System;
";

    /// <summary>
    /// The Pragma Warning Disable
    /// </summary>
    public const string PragmaWarningDisable = @"
#pragma warning disable
";

    /// <summary>
    /// The Pragma Warning Enable
    /// </summary>
    public const string PragmaWarningEnable = @"
#pragma warning enable
";

    /// <summary>
    /// The Namespace Declaration
    /// </summary>
    /// <param name="namespace">The namespace</param>
    /// <returns>The Namespace Declaration</returns>
    public static string GetNamespaceDeclaration(string @namespace)
    {
        return $@"
namespace {@namespace};
";
    }

    /// <summary>
    /// Gets a Standard Wrapped File
    /// </summary>
    /// <param name="namespace">The Namespace</param>
    /// <param name="contents">The File Contents</param>
    /// <param name="usingNamespaces">The namespaces to use.</param>
    /// <returns></returns>
    public static string GetStandardGeneratedFile(string @namespace, string contents, params string[] usingNamespaces)
    {
        return (AutoGeneratedHeader
               + PragmaWarningDisable
               + StandardUsingStatements
               + string.Join("\r\n", usingNamespaces.Select(@using => $"using {@using};"))
               + GetNamespaceDeclaration(@namespace)
               + contents
               + PragmaWarningEnable).ToNormalizedCSharp();
    }

    /// <summary>
    /// Turns the code into normalized CSharp with better spacing
    /// </summary>
    /// <param name="value">The value to transform</param>
    /// <returns>CSharp in a normalized fashion.</returns>
    public static string ToNormalizedCSharp(this string value)
    {
        return CSharpSyntaxTree.ParseText(value).GetRoot().NormalizeWhitespace().ToFullString();
    }
}
