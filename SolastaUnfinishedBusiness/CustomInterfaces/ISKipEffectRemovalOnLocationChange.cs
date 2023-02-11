using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ISKipEffectRemovalOnLocationChange
{
    [UsedImplicitly]
    public bool Skip(bool willEnterChainedLocation);
}
