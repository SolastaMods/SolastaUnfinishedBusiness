namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ISKipEffectRemovalOnLocationChange
{
    public bool Skip(bool willEnterChainedLocation);
}
