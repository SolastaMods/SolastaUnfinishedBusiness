using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ClassFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featCallForCharge = BuildCallForCharge();
        var featCunningEscape = BuildCunningEscape();
        var featPotentSpellcaster = BuildPotentSpellcaster();

        feats.AddRange(
            featCallForCharge,
            featCunningEscape,
            featPotentSpellcaster);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(
            featCunningEscape);

        GroupFeats.FeatGroupSpellCombat.AddFeats(
            featPotentSpellcaster);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featCallForCharge);

        GroupFeats.MakeGroup("FeatGroupClassBound", null,
            featCallForCharge,
            featCunningEscape,
            featPotentSpellcaster);
    }

    #region Call for Charge

    private static FeatDefinition BuildCallForCharge()
    {
        const string NAME = "FeatCallForCharge";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCallForCharge")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionPowerBuilder
                .Create($"Power{NAME}")
                .SetGuiPresentation(Category.Feature, PowerOathOfTirmarGoldenSpeech)
                .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                        .SetDurationData(DurationType.Round, 1)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}")
                                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                                        .SetSpecialInterruptions(ConditionInterruption.Attacked)
                                        .SetPossessive()
                                        .SetFeatures(
                                            FeatureDefinitionMovementAffinityBuilder
                                                .Create($"MovementAffinity{NAME}")
                                                .SetGuiPresentation($"Condition{NAME}", Category.Condition)
                                                .SetBaseSpeedAdditiveModifier(3)
                                                .AddToDB(),
                                            FeatureDefinitionCombatAffinityBuilder
                                                .Create($"CombatAffinity{NAME}")
                                                .SetGuiPresentation($"Condition{NAME}", Category.Condition)
                                                .SetMyAttackAdvantage(AdvantageType.Advantage)
                                                .AddToDB())
                                        .AddToDB(),
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                        .Build())
                .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetValidators(ValidatorsFeat.IsPaladinLevel1)
            .AddToDB();
    }

    #endregion

    #region Cunning Escape

    private static FeatDefinition BuildCunningEscape()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCunningEscape")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new OnAfterActionFeatureFeatCunningEscape())
            .SetValidators(ValidatorsFeat.IsRogueLevel3)
            .AddToDB();
    }

    private class OnAfterActionFeatureFeatCunningEscape : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action.ActionDefinition != DatabaseHelper.ActionDefinitions.DashBonus)
            {
                return;
            }

            var actingCharacter = action.ActingCharacter.RulesetCharacter;

            var condition = RulesetCondition.CreateActiveCondition(
                actingCharacter.Guid, ConditionDefinitions.ConditionDisengaging, DurationType.Round, 0,
                TurnOccurenceType.EndOfTurn, actingCharacter.Guid, actingCharacter.CurrentFaction.Name);

            actingCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, condition);
        }
    }

    #endregion

    #region Potent Spellcaster

    private static FeatDefinition BuildPotentSpellcaster()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatPotentSpellcaster")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new ModifyMagicEffectFeatPotentSpellcaster())
            .SetValidators(ValidatorsFeat.IsWizardLevel6)
            .AddToDB();
    }

    private sealed class ModifyMagicEffectFeatPotentSpellcaster : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter character)
        {
            if (definition is not SpellDefinition spellDefinition ||
                !SpellListDefinitions.SpellListWizard.SpellsByLevel.Any(x =>
                    x.Level == 0 && x.Spells.Contains(spellDefinition)))
            {
                return effect;
            }

            var damage = effect.FindFirstDamageForm();

            if (damage == null)
            {
                return effect;
            }

            damage.BonusDamage += AttributeDefinitions.ComputeAbilityScoreModifier(
                character.GetAttribute(AttributeDefinitions.Intelligence).CurrentValue);
            damage.DamageBonusTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, "Feat/&FeatPotentSpellcasterTitle", null));

            return effect;
        }
    }

    #endregion
}
