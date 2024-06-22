namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyTeleportEffectBehavior
{
    bool AllyOnly { get; }

    bool TeleportSelf { get; }

    int MaxTargets { get; }
}
