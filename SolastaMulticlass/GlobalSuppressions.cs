// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Fighting analyzers", Scope = "namespaceanddescendants", Target = "~N:SolastaMulticlass")]
[assembly: SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Reflection is required", Scope = "namespaceanddescendants", Target = "~N:SolastaMulticlass")]
