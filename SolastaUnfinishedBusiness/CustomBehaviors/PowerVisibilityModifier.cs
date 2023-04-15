using SolastaUnfinishedBusiness.Api.GameExtensions;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsPowerVisibleHandler(
    RulesetCharacter character,
    FeatureDefinitionPower power,
    ActionType actionType);

internal class PowerVisibilityModifier
{
    internal static readonly PowerVisibilityModifier Default = new((_, power, actionType) =>
    {
        if (Gui.Battle == null)
        {
            return true;
        }

        var powerActivationTime = power.activationTime;

        CastingTimeToActionDefinition.TryGetValue(powerActivationTime, out var powerActionType);

        return powerActionType == actionType
               || (actionType == ActionType.Main && (powerActivationTime == ActivationTime.Reaction
                                                     || (powerActivationTime != ActivationTime.NoCost &&
                                                         powerActionType == ActionType.NoCost)));
    });

    internal static readonly PowerVisibilityModifier Hidden = new((_, _, _) => false);
    internal static readonly PowerVisibilityModifier Visible = new((_, _, _) => true);

    private readonly IsPowerVisibleHandler handler;

    protected PowerVisibilityModifier(IsPowerVisibleHandler handler)
    {
        this.handler = handler;
    }

    internal bool IsVisible(RulesetCharacter character, FeatureDefinitionPower power, ActionType actionType)
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
