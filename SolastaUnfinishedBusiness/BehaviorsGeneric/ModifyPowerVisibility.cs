using SolastaUnfinishedBusiness.Api.GameExtensions;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

internal delegate bool IsPowerVisibleHandler(
    RulesetCharacter character,
    FeatureDefinitionPower power,
    ActionType actionType);

internal class ModifyPowerVisibility
{
    internal static readonly ModifyPowerVisibility Default = new((_, power, actionType) =>
    {
        if (Gui.Battle == null)
        {
            return true;
        }

        var powerActivationTime = power.activationTime;

        CastingTimeToActionDefinition.TryGetValue(powerActivationTime, out var powerActionType);

        return powerActionType == actionType
               || (actionType == ActionType.Main &&
                   (powerActivationTime == ActivationTime.Reaction ||
                    (powerActivationTime != ActivationTime.NoCost &&
                     powerActionType == ActionType.NoCost)));
    });

    internal static readonly ModifyPowerVisibility Hidden = new((_, _, _) => false);
    internal static readonly ModifyPowerVisibility Visible = new((_, _, _) => true);
    internal static readonly ModifyPowerVisibility NotInCombat = new((_, _, _) => Gui.Battle == null);

    private readonly IsPowerVisibleHandler _handler;

    protected ModifyPowerVisibility(IsPowerVisibleHandler handler)
    {
        _handler = handler;
    }

    internal bool IsVisible(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
    {
        return _handler(character, power, actionType);
    }

    internal static bool IsPowerHidden(RulesetCharacter character, RulesetUsablePower power, ActionType actionType)
    {
        return IsPowerHidden(character, power.PowerDefinition, actionType);
    }

    internal static bool IsPowerHidden(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
    {
        var validator = power.GetFirstSubFeatureOfType<ModifyPowerVisibility>();

        return validator != null && !validator.IsVisible(character, power, actionType);
    }
}
