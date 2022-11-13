using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Roslynator", "ROS0003")]

[assembly: SuppressMessage("Major Code Smell",
    "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
    Justification = "Reflection is required", Scope = "namespaceanddescendants",
    Target = "~N:SolastaUnfinishedBusiness")]
