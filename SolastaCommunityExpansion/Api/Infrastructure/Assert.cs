using System;
using JetBrains.Annotations;

namespace SolastaModApi.Infrastructure;

public static class Assert
{
    public static void IsNotNull<T>([NotNull] [NoEnumeration] T instance, string message = null) where T : class
    {
        if (instance == null)
        {
            throw new ArgumentNullException(message ?? string.Empty);
        }
    }
}
