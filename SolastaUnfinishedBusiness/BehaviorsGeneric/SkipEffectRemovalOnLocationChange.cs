using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

internal static class SkipEffectRemovalOnLocationChange
{
    internal static readonly IPreventRemoveEffectOnLocationChange Always = new AlwaysPreventRemove();
    // internal static readonly IPreventRemoveEffectOnLocationChange OnChained = new SkipOnChained();

    private sealed class AlwaysPreventRemove : IPreventRemoveEffectOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return true;
        }
    }

#if false
    private sealed class SkipOnChained : IPreventRemoveEffectOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return willEnterChainedLocation;
        }
    }
#endif
}
