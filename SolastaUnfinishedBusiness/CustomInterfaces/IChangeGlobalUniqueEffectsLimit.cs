using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeGlobalUniqueEffectsLimit
{
    public GlobalUniqueEffects.Group GroupKey { get; }
    public int Limit { get; }
}
