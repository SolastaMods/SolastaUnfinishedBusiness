using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPreventEffectRemovalOnLocationChange
{
    [UsedImplicitly]
    public bool Skip(bool willEnterChainedLocation);
}
