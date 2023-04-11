using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeGlobalUniqueEffectsLimit
{
    public GlobalUniqueEffects.Group GroupKey { get; set; }
    public int Limit { get; set; }
}
