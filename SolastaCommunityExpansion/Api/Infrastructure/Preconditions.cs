using System;
using System.Diagnostics;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Diagnostics;

namespace SolastaCommunityExpansion.Api.Infrastructure;

// TODO: rename IsNotNull -> ArgumentIsNotNull etc
// TODO: remove Assert.IsNotNull which duplicates Preconditions.IsNotNull
public static class Preconditions
{
    [ContractAnnotation("halt <= paramValue : null")]
    [AssertionMethod]
    public static void IsNotNull<T>([NotNull] [NoEnumeration] T paramValue, [InvokerParameterName] string paramName)
        where T : class
    {
        if (paramValue == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    [ContractAnnotation("halt <= paramValue : null")]
    [AssertionMethod]
    public static void IsNotNullOrEmpty([NotNull] string paramValue, [InvokerParameterName] string paramName)
    {
        if (string.IsNullOrEmpty(paramValue))
        {
            throw new ArgumentException("The parameter must not be null or an empty string.", paramName);
        }
    }

    [ContractAnnotation("halt <= paramValue : null")]
    [AssertionMethod]
    public static void IsNotNullOrWhiteSpace([NotNull] string paramValue, [InvokerParameterName] string paramName)
    {
        if (string.IsNullOrWhiteSpace(paramValue))
        {
            throw new ArgumentException("The parameter must not be null or whitespace.", paramName);
        }
    }

    [Conditional("DEBUG")]
    [AssertionMethod]
    public static void IsValidDuration(RuleDefinitions.DurationType type, int duration)
    {
        if (RuleDefinitions.IsVariableDuration(type))
        {
            if (duration == 0)
            {
                throw new ArgumentException($"A duration value is required for duration type {type}.",
                    nameof(duration));
            }
        }
        else
        {
            if (duration != 0)
            {
                throw new ArgumentException(
                    $"Duration={duration}. A duration value is not expected for duration type {type}.",
                    nameof(duration));
            }
        }
    }

    [Conditional("DEBUG")]
    [AssertionMethod]
    internal static void AreEqual<T>([NotNull] T left, T right, string message)
    {
        if (!left.Equals(right))
        {
            throw new SolastaCommunityExpansionException(message);
        }
    }
}
