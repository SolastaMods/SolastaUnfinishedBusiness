using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.FightingStyles;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class TwoWeaponCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDualFlurry = BuildDualFlurry();
        var featDualWeaponDefense = BuildDualWeaponDefense();

        feats.AddRange(featDualFlurry, featDualWeaponDefense);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featDualWeaponDefense);

        GroupFeats.MakeGroup("FeatGroupTwoWeaponCombat", null,
            FeatDefinitions.Ambidextrous,
            FeatDefinitions.TwinBlade,
            featDualFlurry,
            featDualWeaponDefense);
    }

    private static FeatDefinition BuildDualFlurry()
    {
        var conditionDualFlurryApply = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryApply")
            .SetGuiPresentationNoContent(true)
            .SetSpecialDuration()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionDualFlurryGrant = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryGrant")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSpecialDuration()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create("AdditionalActionDualFlurry")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionDefinitions.ActionType.Bonus)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatDualFlurry")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("OnAttackDamageEffectFeatDualFlurry")
                    .SetGuiPresentation("FeatDualFlurry", Category.Feat)
                    .SetCustomSubFeatures(
                        new OnAttackHitEffectFeatDualFlurry(conditionDualFlurryGrant, conditionDualFlurryApply))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDualWeaponDefense()
    {
        const string NAME = "FeatDualWeaponDefense";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierFeatDualWeaponDefense")
                .SetGuiPresentation(NAME, Category.Feat)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass, 1)
                .SetSituationalContext(SituationalContext.DualWieldingMeleeWeapons)
                .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private sealed class OnAttackHitEffectFeatDualFlurry : IAttackEffectAfterDamage
    {
        private readonly ConditionDefinition _conditionDualFlurryApply;
        private readonly ConditionDefinition _conditionDualFlurryGrant;

        internal OnAttackHitEffectFeatDualFlurry(
            ConditionDefinition conditionDualFlurryGrant,
            ConditionDefinition conditionDualFlurryApply)
        {
            _conditionDualFlurryGrant = conditionDualFlurryGrant;
            _conditionDualFlurryApply = conditionDualFlurryApply;
        }

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attacker.RulesetCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            var hasWeaponInMainHand = ValidatorsCharacter.HasMeleeWeaponInMainHand(hero);
            var hasWeaponInOffHand = ValidatorsCharacter.HasMeleeWeaponInOffHand(hero);
            var hasShield = ValidatorsCharacter.HasShield(hero);
            var hasShieldExpert =
                hero.TrainedFeats.Any(x => x.Name.Contains(ShieldExpert.ShieldExpertName)) ||
                hero.TrainedFightingStyles.Any(x => x.Name.Contains(ShieldExpert.ShieldExpertName));

            var isValid = hasWeaponInMainHand && ((hasShield && hasShieldExpert) || hasWeaponInOffHand);

            if (attackMode == null || !isValid)
            {
                return;
            }

            if (attacker.UsedSpecialFeatures.ContainsKey(_conditionDualFlurryGrant.Name))
            {
                return;
            }

            var condition = _conditionDualFlurryApply;

            if (hero.HasConditionOfType(condition))
            {
                attacker.UsedSpecialFeatures.Add(_conditionDualFlurryGrant.Name, 1);
                condition = _conditionDualFlurryGrant;
            }

            hero.InflictCondition(
                condition.Name,
                condition.DurationType,
                condition.DurationParameter,
                condition.TurnOccurence,
                AttributeDefinitions.TagCombat,
                hero.guid,
                hero.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }
}
