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
    private const string SculptSpellsSavedTag = $"{Name}SculptSpellsSaved";

    //
    // these lists contain all evocation spells that do damage in a non-vanilla way so they also get bonus
    //

    private static readonly string[] CantripsAdditionalDamages =
    [
        "AdditionalDamageBoomingBlade",
        "AdditionalDamageResonatingStrike", // Green-Flame Blade
        "AdditionalDamageSunlightBlade"
    ];

    private static readonly string[] SpellsAdditionalDamages =
    [
        "AdditionalDamageBanishingSmite",
        "AdditionalDamageBlindingSmite",
        "AdditionalDamageHolyWeapon",
        "AdditionalDamageSearingSmite",
        "AdditionalDamageStaggeringSmite",
        "AdditionalDamageWrathfulSmite"
    ];

    private static readonly string[] SpellsPowerDamages =
    [
        "PowerCrownOfStars",
        "PowerHolyWeapon",
        "PowerThunderousSmite"
    ];

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

        conditionSculptSpells.AddCustomSubFeatures(
            new CustomBehaviorSculptSpells(conditionSculptSpells, featureSculptSpells));

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
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AllowMultipleInstances()
            .AddToDB();

        var conditionMaxDamage = ConditionDefinitionBuilder
            .Create($"Condition{Name}OverChannelMaxDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new ForceMaxDamageTypeDependentOverChannel())
            .AddToDB();

        var actionAffinityOverChannelToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityOverChannelToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.OverChannelToggle)
            .AddToDB();

        actionAffinityOverChannelToggle.AddCustomSubFeatures(
            new CustomBehaviorOverChannel(actionAffinityOverChannelToggle, conditionOverChannel, conditionMaxDamage));

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
                target.UsedSpecialFeatures.Remove(SculptSpellsSavedTag);

                var rulesetTarget = target.RulesetCharacter;
                var rulesetSource = attacker.RulesetCharacter;

                rulesetTarget.InflictCondition(
                    conditionSculptPocket.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetSource.guid,
                    rulesetSource.CurrentFaction.Name,
                    1,
                    conditionSculptPocket.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class CustomBehaviorSculptSpells(
        ConditionDefinition conditionSculptSpells,
        FeatureDefinition featureSculptSpells) : IMagicEffectBeforeHitConfirmedOnMe, IRollSavingThrowFinished
    {
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

            if (removed == 0 ||
                !defender.RulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSculptSpells.Name, out var actionCondition))
            {
                yield break;
            }

            var caster = EffectHelpers.GetCharacterByGuid(actionCondition.SourceGuid);

            caster.LogCharacterUsedFeature(featureSculptSpells);
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

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyPotentCantrips
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage
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
            var isSpell = rulesetEffect.SourceDefinition is SpellDefinition;

            switch (isSpell)
            {
                case true when rulesetEffect.EffectDescription.RangeType
                    is not (RangeType.MeleeHit or RangeType.RangeHit):
                case true when !firstTarget &&
                               rulesetEffect.EffectDescription.TargetType
                                   is TargetType.Individuals
                                   or TargetType.IndividualsUnique:
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

        // handle special blade cantrips use cases
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (!CantripsAdditionalDamages.Contains(featureDefinitionAdditionalDamage.Name))
            {
                return;
            }

            var pb = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            damageForm.BonusDamage += pb;
        }
    }

    //
    // Empowered Evocation
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage
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
            var isSpell = rulesetEffect.SourceDefinition is SpellDefinition;

            switch (isSpell)
            {
                case false when !SpellsPowerDamages.Contains(rulesetEffect.SourceDefinition.Name):
                case true when rulesetEffect.SchoolOfMagic != SchoolEvocation:
                case true when !firstTarget &&
                               rulesetEffect.EffectDescription.TargetType
                                   is TargetType.Individuals
                                   or TargetType.IndividualsUnique:
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

        // handle special blade cantrips use cases
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            var featureName = featureDefinitionAdditionalDamage.Name;

            if (!CantripsAdditionalDamages.Contains(featureName) && !SpellsAdditionalDamages.Contains(featureName))
            {
                return;
            }

            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence));

            damageForm.BonusDamage += Math.Max(1, intelligenceModifier);
        }
    }

    //
    // Over Channel
    //

    private sealed class ForceMaxDamageTypeDependentOverChannel : IForceMaxDamageTypeDependent
    {
        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            return true;
        }
    }

    private sealed class CustomBehaviorOverChannel(
        FeatureDefinition featureOverChannel,
        ConditionDefinition conditionOverChannel,
        ConditionDefinition conditionOverChannelMaxDamage)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IMagicEffectFinishedByMe
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
            var rulesetAttacker = attacker.RulesetCharacter;

            // only spells between 1st and 5th levels
            if (!firstTarget ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OverChannelToggle) ||
                rulesetEffect.SourceDefinition is not SpellDefinition spellDefinition ||
                spellDefinition.SpellLevel == 0 ||
                spellDefinition.SpellLevel > 5)
            {
                yield break;
            }

            // allow max spell damage on this attack
            EffectHelpers.StartVisualEffect(
                attacker, attacker, FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury , EffectHelpers.EffectType.Caster);
            rulesetAttacker.LogCharacterUsedFeature(featureOverChannel);
            rulesetAttacker.InflictCondition(
                conditionOverChannelMaxDamage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionOverChannelMaxDamage.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionOverChannelMaxDamage.Name, out var actionCondition))
            {
                yield break;
            }

            rulesetAttacker.RemoveCondition(actionCondition);

            // add one instance of over channel
            rulesetAttacker.InflictCondition(
                conditionOverChannel.Name,
                DurationType.UntilLongRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionOverChannel.Name,
                0,
                0,
                0);

            var overChannelInstancesCount =
                rulesetAttacker.AllConditions.Count(x => x.ConditionDefinition == conditionOverChannel);

            // first time used so no self damage
            if (overChannelInstancesCount <= 1)
            {
                yield break;
            }

            const DieType DIE_TYPE = DieType.D12;

            var rulesetEffect = action.ActionParams.RulesetEffect;
            var diceNumber = overChannelInstancesCount * rulesetEffect.EffectLevel;
            var rolls = new List<int>();
            var damage = rulesetAttacker.RollDiceAndSum(DIE_TYPE, RollContext.None, diceNumber, rolls, false);

            EffectHelpers.StartVisualEffect(
                attacker, attacker, FeatureDefinitionPowers.PowerPatronFiendDarkOnesOwnLuck, EffectHelpers.EffectType.Effect);
            rulesetAttacker.SustainDamage(
                damage, DamageTypeNecrotic, false, rulesetAttacker.Guid,
                new RollInfo(DIE_TYPE, rolls, 0), out _);
        }
    }
}
