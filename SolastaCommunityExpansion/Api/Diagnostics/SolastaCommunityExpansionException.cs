using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Diagnostics;

[Serializable]
public class SolastaCommunityExpansionException : Exception
{
    public SolastaCommunityExpansionException()
    {
    }

    public SolastaCommunityExpansionException(string message) : base(message)
    {
    }

    public SolastaCommunityExpansionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SolastaCommunityExpansionException([NotNull] SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
