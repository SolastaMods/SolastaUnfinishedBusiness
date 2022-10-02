using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal static class FeatureDefinitionSkipEffectRemovalOnLocationChange
{
    internal static readonly ISKipEffectRemovalOnLocationChange Always = new AlwaysSkip();
    internal static readonly ISKipEffectRemovalOnLocationChange OnChained = new SkipOnChained();

    private sealed class AlwaysSkip : ISKipEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return true;
        }
    }

    private sealed class SkipOnChained : ISKipEffectRemovalOnLocationChange
    {
        public bool Skip(bool willEnterChainedLocation)
        {
            return willEnterChainedLocation;
        }
    }
}
