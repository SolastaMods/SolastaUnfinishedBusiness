using System;

namespace SolastaModApi.Infrastructure
{
    /// <summary>
    /// Attribute to support testing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TargetTypeAttribute : Attribute
    {
        public TargetTypeAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}
