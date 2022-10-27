using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

internal static class Assert
{
    internal static void IsNotNull<T>(
        [NotNull] [NoEnumeration] T instance,
        [CanBeNull] string message = null)
        where T : class
    {
        if (instance == null)
        {
            throw new ArgumentNullException(message ?? string.Empty);
        }
    }
}
