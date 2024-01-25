using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

public class ForceConcentrationCheck
{
    private ForceConcentrationCheck()
    {
    }

    public static ForceConcentrationCheck Mark { get; } = new();

    internal static void ProcessConcentratedEffects(
        GameLocationCharacter locCharacter,
        int damage,
        string damageType,
        bool stillConscious)
    {
        var rulesetCharacter = locCharacter.RulesetCharacter;

        if (rulesetCharacter is not (RulesetCharacterHero or RulesetCharacterMonster))
        {
            return;
        }

        var service = ServiceRepository.GetService<IGameSettingsService>();

        if (service is { NeverLoseConcentrationOnSpells: true } && rulesetCharacter.Side == RuleDefinitions.Side.Ally)
        {
            return;
        }

        var effects = rulesetCharacter.EnumerateActiveEffectsActivatedByMe()
            .Where(e => e.SourceDefinition.HasSubFeatureOfType<ForceConcentrationCheck>())
            .ToList();

        if (effects.Count == 0)
        {
            return;
        }

        var handler = rulesetCharacter.ConcentrationCheckRolled;

        effects.ForEach(effect => RollConcentration(effect, rulesetCharacter, damage, damageType, stillConscious));
        rulesetCharacter.ConcentrationCheckRolled = handler;
    }


    private static void RollConcentration(
        RulesetEffect effect,
        RulesetCharacter rulesetCharacter,
        int damage,
        string damageType,
        bool stillConscious)
    {
        var result = RuleDefinitions.RollOutcome.Failure;

        if (stillConscious)
        {
            if (!rulesetCharacter.IsAutomaticallyFailingSavingThrow(AttributeDefinitions.Constitution))
            {
                rulesetCharacter.ConcentrationCheckRolled =
                    (character, outcome, roll, rawRoll, modifier, dc, trends, advantageTrends) =>
                    {
                        character.LogConcentrationCheckRoll(
                            effect, outcome, roll, rawRoll, modifier, dc, trends, advantageTrends);
                    };

                rulesetCharacter.RollConcentrationCheckFromDamage(damage, damageType, out result);
            }
        }

        if (result is not RuleDefinitions.RollOutcome.Failure and not RuleDefinitions.RollOutcome.CriticalFailure)
        {
            return;
        }

        effect.DoTerminate(rulesetCharacter);
    }
}
