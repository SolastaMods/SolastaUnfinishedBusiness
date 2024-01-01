using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

/*
    Courtesy to https://stackoverflow.com/questions/4035719/getmethod-for-generic-method
    A powerful GetMethod that handles matching one of the overloads including generic type params
 */

internal static class TypeExtensions
{
    /// <summary>
    ///     Search for a method by name and parameter types.
    ///     Unlike GetMethod(), does 'loose' matching on generic
    ///     parameter types, and searches base interfaces.
    /// </summary>
    /// <exception cref="AmbiguousMatchException" />
    internal static MethodInfo GetMethodExt(this Type thisType,
        string name,
        params Type[] parameterTypes)
    {
        return GetMethodExt(thisType,
            name,
            BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.FlattenHierarchy,
            parameterTypes);
    }

    /// <summary>
    ///     Search for a method by name, parameter types, and binding flags.
    ///     Unlike GetMethod(), does 'loose' matching on generic
    ///     parameter types, and searches base interfaces.
    /// </summary>
    /// <exception cref="AmbiguousMatchException" />
    [UsedImplicitly]
    internal static MethodInfo GetMethodExt(this Type thisType,
        string name,
        BindingFlags bindingFlags,
        params Type[] parameterTypes)
    {
        MethodInfo matchingMethod = null;

        // Check all methods with the specified name, including in base classes
        GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

        // If we're searching an interface, we have to manually search base interfaces
        if (matchingMethod != null || !thisType.IsInterface)
        {
            return matchingMethod;
        }

        foreach (var interfaceType in thisType.GetInterfaces())
        {
            GetMethodExt(ref matchingMethod,
                interfaceType,
                name,
                bindingFlags,
                parameterTypes);
        }

        return matchingMethod;
    }

    private static void GetMethodExt(ref MethodInfo matchingMethod,
        Type type,
        string name,
        BindingFlags bindingFlags,
        params Type[] parameterTypes)
    {
        // Check all methods with the specified name, including in base classes
        foreach (var memberInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
        {
            var methodInfo = (MethodInfo)memberInfo;

            // Check that the parameter counts and types match, 
            // with 'loose' matching on generic parameters
            var parameterInfos = methodInfo.GetParameters();

            if (parameterInfos.Length != parameterTypes.Length)
            {
                continue;
            }

            var i = 0;

            for (; i < parameterInfos.Length; ++i)
            {
                if (!parameterInfos[i].ParameterType.IsSimilarType(parameterTypes[i]))
                {
                    break;
                }
            }

            if (i != parameterInfos.Length)
            {
                continue;
            }

            if (matchingMethod == null)
            {
                matchingMethod = methodInfo;
            }
            else
            {
                throw new AmbiguousMatchException(
                    "More than one matching method found!");
            }
        }
    }

    /// <summary>
    ///     Determines if the two types are either identical, or are both generic
    ///     parameters or generic types with generic parameters in the same
    ///     locations (generic parameters match any other generic parameter,
    ///     but NOT concrete types).
    /// </summary>
    [UsedImplicitly]
    internal static bool IsSimilarType(this Type thisType, Type type)
    {
        while (true)
        {
            // Ignore any 'ref' types
            if (thisType is { IsByRef: true })
            {
                thisType = thisType.GetElementType();
            }

            if (type is { IsByRef: true })
            {
                type = type.GetElementType();
            }

            // Handle array types
            if (type != null && thisType is { IsArray: true } && type.IsArray)
            {
                thisType = thisType.GetElementType();
                type = type.GetElementType();

                continue;
            }

            // If the types are identical, or they're both generic parameters 
            // or the special 'T' type, treat as a match
            if (type != null &&
                thisType != null &&
                (thisType == type ||
                 ((thisType.IsGenericParameter ||
                   thisType == typeof(T)) &&
                  (type.IsGenericParameter ||
                   type == typeof(T)))))
            {
                return true;
            }

            // Handle any generic arguments
            if (type == null || thisType is not { IsGenericType: true } || !type.IsGenericType)
            {
                return false;
            }

            var thisArguments = thisType.GetGenericArguments();
            var arguments = type.GetGenericArguments();

            if (thisArguments.Length != arguments.Length)
            {
                return false;
            }

            return !thisArguments.Where((t, i) => !t.IsSimilarType(arguments[i])).Any();
        }
    }

    /// <summary>
    ///     Special type used to match any generic parameter type in GetMethodExt().
    /// </summary>
    [UsedImplicitly]
    internal class T;
}
