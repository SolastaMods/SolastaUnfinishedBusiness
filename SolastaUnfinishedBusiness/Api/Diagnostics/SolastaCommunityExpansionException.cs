using System;

namespace SolastaUnfinishedBusiness.Api.Diagnostics;

[Serializable]
internal class SolastaUnfinishedBusinessException : Exception
{
    internal SolastaUnfinishedBusinessException(string message) : base(message)
    {
    }
}
