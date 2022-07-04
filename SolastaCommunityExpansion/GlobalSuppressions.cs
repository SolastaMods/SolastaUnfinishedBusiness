using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Roslynator", "ROS0003")]
[assembly:
    SuppressMessage("Roslynator", "RCS1093:Remove file with no code.", Justification = "keeping code for reference")]

[assembly:
    SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Fighting analyzers",
        Scope = "namespaceanddescendants", Target = "~N:SolastaCommunityExpansion")]

[assembly:
    SuppressMessage("", "IDE0130", Justification = "3rd party source", Scope = "namespaceanddescendants",
        Target = "~N:ModKit")]
[assembly:
    SuppressMessage("", "IDE0130", Justification = "3rd party source", Scope = "namespaceanddescendants",
        Target = "~N:SolastaMonsters")]

[assembly: SuppressMessage("Major Code Smell",
    "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
    Justification = "Reflection is required", Scope = "namespaceanddescendants",
    Target = "~N:SolastaCommunityExpansion")]
