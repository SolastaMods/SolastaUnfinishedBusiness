using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class SkipEffectRemovalOnLocationChange
{
    internal static readonly ISKipEffectRemovalOnLocationChange Always = new AlwaysSkip();
    // internal static readonly ISKipEffectRemovalOnLocationChange OnChained = new SkipOnChained();

    private sealed class AlwaysSkip : ISKipEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return true;
        }
    }

#if false
    private sealed class SkipOnChained : ISKipEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return willEnterChainedLocation;
        }
    }
#endif
}
