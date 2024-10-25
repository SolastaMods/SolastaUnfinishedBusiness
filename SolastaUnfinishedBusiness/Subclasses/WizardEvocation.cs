using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardEvocation : AbstractSubclass
{
    private const string Name = "WizardEvocation";

    public WizardEvocation()
    {
        // LEVEL 02

        // Evocation Savant

        var magicAffinitySavant = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Savant")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(
                0.5f, 0.5f, 0, AdvantageType.None, PreparedSpellsModifier.None)
            .AddCustomSubFeatures(new ModifyScribeCostAndDurationEvocationSavant())
            .AddToDB();

        // Sculpt Spells

        var conditionSculptSpells = ConditionDefinitionBuilder
            .Create($"Condition{Name}SculptSpells")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var featureSculptSpells = FeatureDefinitionBuilder
            .Create($"Feature{Name}SculptSpells")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectInitiatedByMeSculptSpells(conditionSculptSpells))
            .AddToDB();

        conditionSculptSpells.AddCustomSubFeatures(new CustomBehaviorSculptSpells(featureSculptSpells));

        // LEVEL 06

        // Potent Cantrip

        var magicAffinityPotentCantrip = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}PotentCantrip")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyPotentCantrips())
            .AddToDB();

        magicAffinityPotentCantrip.forceHalfDamageOnCantrips = true;

        // LEVEL 10

        // Empowered Evocation

        var featureEmpoweredEvocation = FeatureDefinitionBuilder
            .Create($"Feature{Name}EmpoweredEvocation")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation())
            .AddToDB();

        // LEVEL 14

        // Over Channel

        var conditionOverChannel = ConditionDefinitionBuilder
            .Create($"Condition{Name}OverChannel")
            .SetGuiPresentationNoContent(true)
            .AllowMultipleInstances()
            .AddToDB();
            
        var actionAffinityOverChannelToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityOverChannelToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.OverChannelToggle)
            .AddToDB();

        var featureSetOverChannel = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}OverChannel")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(actionAffinityOverChannelToggle)
            .AddToDB();

        //
        // Main
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardEvocation, 256))
            .AddFeaturesAtLevel(2, magicAffinitySavant, featureSculptSpells)
            .AddFeaturesAtLevel(6, magicAffinityPotentCantrip)
            .AddFeaturesAtLevel(10, featureEmpoweredEvocation)
            .AddFeaturesAtLevel(14, featureSetOverChannel)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Sculpt Spells
    //

    private sealed class MagicEffectInitiatedByMeSculptSpells(ConditionDefinition conditionSculptPocket)
        : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetEffect = action.ActionParams.RulesetEffect;

            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation })
            {
                yield break;
            }

            foreach (var target in targets.Where(x => !x.IsOppositeSide(attacker.Side)))
            {
                var rulesetTarget = target.RulesetCharacter;

                if (rulesetTarget.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionSculptPocket.Name, out var actionCondition))
                {
                    rulesetTarget.RemoveCondition(actionCondition);
                }
            }
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect rulesetEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation })
            {
                yield break;
            }

            foreach (var target in targets.Where(x => !x.IsOppositeSide(attacker.Side)))
            {
                var rulesetTarget = target.RulesetCharacter;

                rulesetTarget.InflictCondition(
                    conditionSculptPocket.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetTarget.guid,
                    rulesetTarget.CurrentFaction.Name,
                    1,
                    conditionSculptPocket.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class CustomBehaviorSculptSpells(FeatureDefinition featureSculptSpells)
        : IMagicEffectBeforeHitConfirmedOnMe, IRollSavingThrowFinished
    {
        private const string SculptSpellsSavedTag = "SculptSpellsSaved";

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (!defender.UsedSpecialFeatures.Remove(SculptSpellsSavedTag))
            {
                yield break;
            }

            var removed = actualEffectForms.RemoveAll(x =>
                x.HasSavingThrow &&
                x.FormType == EffectForm.EffectFormType.Damage &&
                x.SavingThrowAffinity == EffectSavingThrowType.HalfDamage);

            if (removed > 0)
            {
                defender.RulesetCharacter.LogCharacterUsedFeature(featureSculptSpells);
            }
        }

        public void OnSavingThrowFinished(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (outcome == RollOutcome.Success)
            {
                GameLocationCharacter.GetFromActor(rulesetActorDefender)
                    .SetSpecialFeatureUses(SculptSpellsSavedTag, 0);
            }
        }
    }

    //
    // Potent Cantrips
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyPotentCantrips : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            if (rulesetEffect.EffectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique &&
                !firstTarget)
            {
                yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (effectForm == null)
            {
                yield break;
            }

            var pb = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            effectForm.DamageForm.BonusDamage += pb;
        }
    }

    //
    // Evocation Savant
    //

    private sealed class ModifyScribeCostAndDurationEvocationSavant : IModifyScribeCostAndDuration
    {
        public void ModifyScribeCostMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                costMultiplier = 1;
            }
        }

        public void ModifyScribeDurationMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                durationMultiplier = 1;
            }
        }
    }

    //
    // Empowered Evocation
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation })
            {
                yield break;
            }

            if (rulesetEffect.EffectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique &&
                !firstTarget)
            {
                yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (effectForm == null)
            {
                yield break;
            }

            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence));

            effectForm.DamageForm.BonusDamage += Math.Max(1, intelligenceModifier);
        }
    }
    
    //
    // Over Channel
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyOverChannel : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender, 
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms, 
            bool firstTarget, 
            bool criticalHit)
        {
            yield break;
        }
    }
    
}
