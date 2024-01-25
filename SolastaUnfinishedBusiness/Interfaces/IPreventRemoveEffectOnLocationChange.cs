using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPreventRemoveEffectOnLocationChange
{
    [UsedImplicitly]
    public bool Skip(bool willEnterChainedLocation);
}
