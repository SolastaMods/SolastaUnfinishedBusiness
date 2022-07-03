using System;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class Assert
{
    public static void IsNotNull<T>([NotNull] [NoEnumeration] T instance, [CanBeNull] string message = null)
        where T : class
    {
        if (instance == null)
        {
            throw new ArgumentNullException(message ?? string.Empty);
        }
    }
}
