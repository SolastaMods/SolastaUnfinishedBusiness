using System;
using System.Runtime.Serialization;

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

    protected SolastaCommunityExpansionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
