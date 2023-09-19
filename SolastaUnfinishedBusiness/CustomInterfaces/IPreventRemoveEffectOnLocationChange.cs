using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPreventRemoveEffectOnLocationChange
{
    [UsedImplicitly]
    public bool Skip(bool willEnterChainedLocation);
}
