#if DEBUG
using System;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

/// <summary>
///     Attribute to support testing
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class TargetTypeAttribute : Attribute
{
    internal TargetTypeAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    internal Type TargetType { get; }
}
#endif
