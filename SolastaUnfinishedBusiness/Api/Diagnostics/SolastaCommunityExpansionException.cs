using System;

namespace SolastaUnfinishedBusiness.Api.Diagnostics;

[Serializable]
public class SolastaUnfinishedBusinessException : Exception
{
    public SolastaUnfinishedBusinessException(string message) : base(message)
    {
    }
}
