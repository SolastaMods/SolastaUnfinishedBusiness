using SolastaUnfinishedBusiness.Api.Extensions;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate bool IsPowerVisibleHandler(RulesetCharacter character, FeatureDefinitionPower power,
    ActionType actionType);

internal class PowerVisibilityModifier
{
    internal static PowerVisibilityModifier Default = new PowerVisibilityModifier((_, power, actionType) =>
    {
        if (Gui.Battle != null)
        {
            var powerActivationTime = power.activationTime;
            CastingTimeToActionDefinition.TryGetValue(powerActivationTime, out var powerActionType);
            return powerActionType == actionType
                   || actionType == ActionType.Main && (powerActivationTime == ActivationTime.Reaction
                                                        || (powerActivationTime != ActivationTime.NoCost &&
                                                            powerActionType == ActionType.NoCost));
        }
        else
        {
            return true;
        }
    });

    internal static PowerVisibilityModifier Hidden = new PowerVisibilityModifier((_, _, _) => false);

    private readonly IsPowerVisibleHandler handler;

    public PowerVisibilityModifier(IsPowerVisibleHandler handler)
    {
        this.handler = handler;
    }

    virtual internal bool IsVisible(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
    {
        return handler(character, power, actionType);
    }

    internal static bool IsPowerHidden(RulesetCharacter character, RulesetUsablePower power, ActionType actionType)
    {
        var validator = power.PowerDefinition.GetFirstSubFeatureOfType<PowerVisibilityModifier>();
        return validator != null && !validator.IsVisible(character, power.powerDefinition, actionType);
    }
}
