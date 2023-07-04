using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class SkipEffectRemovalOnLocationChange
{
    internal static readonly IPreventEffectRemovalOnLocationChange Always = new AlwaysPrevent();
    // internal static readonly IPreventEffectRemovalOnLocationChange OnChained = new SkipOnChained();

    private sealed class AlwaysPrevent : IPreventEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return true;
        }
    }

#if false
    private sealed class SkipOnChained : IPreventEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return willEnterChainedLocation;
        }
    }
#endif
}
