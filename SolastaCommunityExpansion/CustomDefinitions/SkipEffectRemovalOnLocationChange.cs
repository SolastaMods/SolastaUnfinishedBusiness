namespace SolastaCommunityExpansion.CustomDefinitions;

public static class SkipEffectRemovalOnLocationChange
{
    public static readonly ISKipEffectRemovalOnLocationChange Always = new AlwaysSkip();
    public static readonly ISKipEffectRemovalOnLocationChange OnChained = new SkipOnChained();

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

public interface ISKipEffectRemovalOnLocationChange
{
    public bool Skip(bool willEnterChainedLocation);
}
