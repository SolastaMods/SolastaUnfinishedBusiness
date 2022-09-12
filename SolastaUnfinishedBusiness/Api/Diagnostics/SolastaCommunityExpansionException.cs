using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Diagnostics;

[Serializable]
public class SolastaUnfinishedBusinessException : Exception
{
    public SolastaUnfinishedBusinessException()
    {
    }

    public SolastaUnfinishedBusinessException(string message) : base(message)
    {
    }

    public SolastaUnfinishedBusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SolastaUnfinishedBusinessException([NotNull] SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
