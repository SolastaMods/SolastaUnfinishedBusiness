using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

public static class ElvenPrecision
{
    internal static bool Active;

    internal static void PhysicalAttackRollPrefix(RulesetCharacter character, RulesetAttackMode attackMode)
    {
        Active = false;

        if (attackMode == null)
        {
            return;
        }

        if (!HasPrecision(character))
        {
            return;
        }

        Active = ValidAbility(attackMode.abilityScore);
    }

    public static void MagicAttackRollPrefix(RulesetCharacter character, RulesetEffect activeEffect)
    {
        Active = false;

        if (!HasPrecision(character))
        {
            return;
        }

        if (activeEffect.EffectDescription.rangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
        {
            return;
        }

        switch (activeEffect)
        {
            case RulesetEffectSpell spell:
                var repertoire = spell.spellRepertoire;

                //Spell from a scroll or wand
                if (repertoire == null)
                {
                    return;
                }

                Active = ValidAbility(repertoire.SpellCastingAbility);
                break;
            case RulesetEffectPower power:
                var definition = power.PowerDefinition;
                var computation = definition.AttackHitComputation;
                if ((computation != PowerAttackHitComputation.AbilityScore
                     || !definition.AbilityScoreBonusToAttack)
                    && computation != (PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
                {
                    return;
                }

                Active = ValidAbility(character.GetAbilityScoreOfPower(definition));
                break;
        }
    }

    private static bool ValidAbility(string ability)
    {
        return ability != AttributeDefinitions.Strength &&
               ability != AttributeDefinitions.Constitution;
    }

    private static bool HasPrecision(RulesetActor character)
    {
        return character.HasSubFeatureOfType<ElvenPrecisionContext>();
    }

    internal sealed class ElvenPrecisionContext
    {
        private ElvenPrecisionContext()
        {
        }

        public static ElvenPrecisionContext Mark { get; } = new();
    }
}
