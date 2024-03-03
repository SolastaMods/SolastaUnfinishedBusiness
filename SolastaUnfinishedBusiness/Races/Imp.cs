using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceImpBuilder
{
    private const string Name = "Imp";
    private const ActionDefinitions.Id ImpishWrathToggle = (ActionDefinitions.Id)ExtraActionId.ImpishWrathToggle;

    internal static CharacterRaceDefinition RaceImp { get; } = BuildImp();

    [NotNull]
    private static CharacterRaceDefinition BuildImp()
    {
        var raceImp = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Tiefling, $"Race{Name}")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne)
            .AddToDB();

        RacesContext.RaceScaleMap[raceImp] = 7f / 9.4f;

        raceImp.subRaces = [BuildImpInfernal(raceImp), BuildImpForest(raceImp)];
        return raceImp;
    }

    #region Infernal Imp

    private static CharacterRaceDefinition BuildImpInfernal(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpInfernal";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpInfernal, 1024, 512);

        var featureSetImpInfernalFiendishResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}FiendishResistance")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance)
            .AddToDB();

        var featureSetImpInfernalAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseCha)
            .AddToDB();

        var castSpellImpInfernal = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create($"SpellList{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                    .SetSpellsAtLevel(1, SpellDefinitions.Invisibility)
                    .FinalizeSpells(true, 1)
                    .AddToDB())
            .AddToDB();

        var raceImpInfernal = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetBaseHeight(42)
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpInfernalAbilityScoreIncrease,
                featureSetImpInfernalFiendishResistance,
                castSpellImpInfernal)
            .AddToDB();

        raceImpInfernal.racePresentation.preferedSkinColors = new RangedInt(15, 19);

        return raceImpInfernal;
    }

    #endregion

    #region Forest Imp

    private static CharacterRaceDefinition BuildImpForest(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpForest";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpForest, 1024, 512);

        var featureSetImpForestAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierHalfOrcAbilityScoreIncreaseCon)
            .AddToDB();

        var actionAffinityImpForestInnateCunning = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}InnateCunning")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(
                ActionDefinitions.Id.DisengageBonus,
                ActionDefinitions.Id.HideBonus)
            .AddToDB();

        var additionalDamageImpishWrath = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}ImpishWrath")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ImpishWrath")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .AddToDB();

        var conditionImpishWrath = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ImpishWrath")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageImpishWrath)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var powerImpForestImpishWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        powerImpForestImpishWrath.AddCustomSubFeatures(
            new CustomBehaviorImpishWrath(powerImpForestImpishWrath, conditionImpishWrath));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "ImpishWrathToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ImpishWrathToggle)
            .SetActivatedPower(powerImpForestImpishWrath)
            .AddToDB();

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityImpishWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ImpishWrathToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAvailablePowerUsage(powerImpForestImpishWrath)))
            .AddToDB();

        var featureSetImpForestImpishWrath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}ImpishWrath")
            .SetGuiPresentation($"Power{NAME}ImpishWrath", Category.Feature)
            .SetFeatureSet(powerImpForestImpishWrath, actionAffinityImpishWrathToggle)
            .AddToDB();

        var raceImpForest = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpForestAbilityScoreIncrease,
                actionAffinityImpForestInnateCunning,
                featureSetImpForestImpishWrath,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry)
            .AddToDB();

        raceImpForest.racePresentation.preferedSkinColors = new RangedInt(28, 37);

        return raceImpForest;
    }

    private sealed class CustomBehaviorImpishWrath(
        FeatureDefinitionPower powerImpishWrath,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionImpishWrath)
        : IAttackBeforeHitConfirmedOnEnemy, IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode != null &&
                actualEffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                yield return HandleImpishWrath(attacker);
            }
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (actualEffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                yield return HandleImpishWrath(attacker);
            }
        }

        private IEnumerator HandleImpishWrath(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.RulesetCharacter.IsToggleEnabled(ImpishWrathToggle) ||
                rulesetAttacker.GetRemainingPowerUses(powerImpishWrath) == 0)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerImpishWrath, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.InflictCondition(
                conditionImpishWrath.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionImpishWrath.Name,
                0,
                0,
                0);
        }
    }

    #endregion
}
