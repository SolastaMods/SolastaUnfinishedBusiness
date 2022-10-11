using SolastaUnfinishedBusiness.Api.Extensions;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate bool IsPowerVisibleHandler(RulesetCharacter character, FeatureDefinitionPower power,
    ActionType actionType);

internal class PowerVisibilityModifier
{
    internal static PowerVisibilityModifier Default = new((_, power, actionType) =>
    {
        if (Gui.Battle != null)
        {
            var powerActivationTime = power.activationTime;
            CastingTimeToActionDefinition.TryGetValue(powerActivationTime, out var powerActionType);
            return powerActionType == actionType
                   || (actionType == ActionType.Main && (powerActivationTime == ActivationTime.Reaction
                                                         || (powerActivationTime != ActivationTime.NoCost &&
                                                             powerActionType == ActionType.NoCost)));
        }

        return true;
    });

    internal static PowerVisibilityModifier Hidden = new((_, _, _) => false);
    internal static PowerVisibilityModifier Visible = new((_, _, _) => true);

    private readonly IsPowerVisibleHandler handler;

    public PowerVisibilityModifier(IsPowerVisibleHandler handler)
    {
        this.handler = handler;
    }

    internal virtual bool IsVisible(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
    {
        return handler(character, power, actionType);
    }

    internal static bool IsPowerHidden(RulesetCharacter character, RulesetUsablePower power, ActionType actionType)
    {
        return IsPowerHidden(character, power.PowerDefinition, actionType);
    }

    internal static bool IsPowerHidden(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
    {
        var validator = power.GetFirstSubFeatureOfType<PowerVisibilityModifier>();
        return validator != null && !validator.IsVisible(character, power, actionType);
    }
}
