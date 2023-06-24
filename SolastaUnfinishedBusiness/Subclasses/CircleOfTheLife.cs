using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheLife : AbstractSubclass
{
    private const string Name = "CircleOfTheLife";
    private const string ConditionRevitalizingBoon = $"Condition{Name}RevitalizingBoon";
    private const string ConditionSeedOfLife = $"Condition{Name}SeedOfLife";
    private const string ConditionVerdancy = $"Condition{Name}Verdancy";
    private const string ConditionVerdancy14 = $"Condition{Name}Verdancy14";

    private static readonly FeatureDefinitionMagicAffinity MagicAffinityHarmoniousBloom =
        FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}HarmoniousBloom")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1) // spells are added on late load to contemplate mod spells
            .AddToDB();

    internal CircleOfTheLife()
    {
        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, CureWounds, Goodberry),
                BuildSpellGroup(3, LesserRestoration, PrayerOfHealing),
                BuildSpellGroup(5, BeaconOfHope, MassHealingWord),
                BuildSpellGroup(7, FreedomOfMovement, Stoneskin),
                BuildSpellGroup(9, GreaterRestoration, MassCureWounds))
            .SetSpellcastingClass(DatabaseHelper.CharacterClassDefinitions.Druid)
            .AddToDB();

        // Verdancy

        var conditionVerdancy = ConditionDefinitionBuilder
            .Create(ConditionVerdancy)
            .SetGuiPresentation(Category.Condition, ConditionChildOfDarkness_DimLight)
            // uses 2 but it will trigger 3 times as required because of the time we add it
            .SetSpecialDuration(DurationType.Round, 2, TurnOccurenceType.EndOfSourceTurn)
            .SetPossessive()
            .CopyParticleReferences(ConditionAided)
            .AllowMultipleInstances()
            .SetCancellingConditions(DatabaseHelper.ConditionDefinitions.ConditionDying)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(HealingComputation.Dice, 1, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .Build())
            .SetCustomSubFeatures(new CustomBehaviorConditionVerdancy())
            .AddToDB();

        var conditionVerdancy14 = ConditionDefinitionBuilder
            .Create(ConditionVerdancy14)
            .SetGuiPresentation(ConditionVerdancy, Category.Condition, ConditionChildOfDarkness_DimLight)
            // uses 4 but it will trigger 5 times as required because of the time we add it
            .SetSpecialDuration(DurationType.Round, 4, TurnOccurenceType.EndOfSourceTurn)
            .SetPossessive()
            .CopyParticleReferences(ConditionAided)
            .AllowMultipleInstances()
            .SetCancellingConditions(DatabaseHelper.ConditionDefinitions.ConditionDying)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(HealingComputation.Dice, 1, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .Build())
            .SetCustomSubFeatures(new CustomBehaviorConditionVerdancy())
            .AddToDB();

        var featureVerdancy = FeatureDefinitionBuilder
            .Create($"Feature{Name}Verdancy")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyMagicEffectVerdancy(conditionVerdancy, conditionVerdancy14))
            .AddToDB();

        // Seed of Life

        var conditionSeedOfLife = ConditionDefinitionBuilder
            .Create(ConditionSeedOfLife)
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetPossessive()
            .CopyParticleReferences(ConditionGuided)
            .SetCancellingConditions(DatabaseHelper.ConditionDefinitions.ConditionDying)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.Proficiency)
                    .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .Build())
            .AddToDB();

        conditionSeedOfLife.SetCustomSubFeatures(new NotifyConditionRemovalSeedOfLife());

        var powerSeedOfLife = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SeedOfLife")
            .SetGuiPresentation(Category.Feature, CureWounds)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2)
            .SetShowCasting(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(HealingWord)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionSeedOfLife, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // Revitalizing Boon

        var conditionRevitalizingBoom = ConditionDefinitionBuilder
            .Create(ConditionRevitalizingBoon)
            .SetGuiPresentation(Category.Condition, ConditionBrandingSmite)
            .SetSpecialDuration(DurationType.Dispelled)
            .SetPossessive()
            .CopyParticleReferences(ConditionAided)
            .SetCancellingConditions(DatabaseHelper.ConditionDefinitions.ConditionDying)
            .SetFeatures(DamageAffinityNecroticResistance, SavingThrowAffinityDwarvenPlate)
            .AddToDB();

        var featureRevitalizingBoom = FeatureDefinitionBuilder
            .Create($"Feature{Name}RevitalizingBoon")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyMagicEffectRevitalizingBoon(conditionRevitalizingBoom))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CircleOfTheLife, 256))
            .AddFeaturesAtLevel(2, autoPreparedSpells, featureVerdancy)
            .AddFeaturesAtLevel(6, powerSeedOfLife)
            .AddFeaturesAtLevel(10, featureRevitalizingBoom)
            .AddFeaturesAtLevel(14, MagicAffinityHarmoniousBloom)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        MagicAffinityHarmoniousBloom.WarListSpells.SetRange(DatabaseHelper.SpellListDefinitions
            .SpellListAllSpells
            .SpellsByLevel
            .SelectMany(x => x.Spells)
            .Where(x => x.EffectDescription.EffectForms
                .Any(y => y.FormType == EffectForm.EffectFormType.Healing))
            .Select(x => x.Name));
    }

    private static int GetDruidLevel(ulong guid)
    {
        var caster = EffectHelpers.GetCharacterByGuid(guid);
        var hero = caster.GetOriginalHero();

        return hero?.GetClassLevel(DruidClass) ?? 0;
    }

    private sealed class CustomBehaviorConditionVerdancy : IModifyMagicEffectRecurrent, INotifyConditionRemoval
    {
        public void ModifyEffect(RulesetCondition rulesetCondition, EffectForm effectForm, RulesetActor rulesetActor)
        {
            if (effectForm.FormType != EffectForm.EffectFormType.Healing)
            {
                return;
            }

            var druidLevel = GetDruidLevel(rulesetCondition.sourceGuid);
            var bonus = rulesetCondition.EffectLevel;

            effectForm.HealingForm.bonusHealing = bonus + (druidLevel >= 14 ? 1 : 0);
        }

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            var hasVerdancy = removedFrom.HasAnyConditionOfType(ConditionSeedOfLife, ConditionVerdancy) ||
                              removedFrom.HasAnyConditionOfType(ConditionSeedOfLife, ConditionVerdancy14);

            if (!hasVerdancy)
            {
                removedFrom.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                    ConditionRevitalizingBoon);
            }
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class ModifyMagicEffectVerdancy : IModifyMagicEffect
    {
        private readonly ConditionDefinition _conditionVerdancy;
        private readonly ConditionDefinition _conditionVerdancy14;

        public ModifyMagicEffectVerdancy(ConditionDefinition conditionVerdancy, ConditionDefinition conditionVerdancy14)
        {
            _conditionVerdancy = conditionVerdancy;
            _conditionVerdancy14 = conditionVerdancy14;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition is not SpellDefinition spell)
            {
                return effectDescription;
            }

            var hasHealingForm =
                effectDescription.EffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Healing);

            if (!hasHealingForm && spell != LesserRestoration && spell != GreaterRestoration)
            {
                return effectDescription;
            }

            var levels = character.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Druid, Name);
            var condition = levels >= 14 ? _conditionVerdancy14 : _conditionVerdancy;

            effectDescription.EffectForms.Add(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                    .Build());

            return effectDescription;
        }
    }

    private sealed class NotifyConditionRemovalSeedOfLife : INotifyConditionRemoval
    {
        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            var hasVerdancy = removedFrom.HasAnyConditionOfType(ConditionSeedOfLife, ConditionVerdancy) ||
                              removedFrom.HasAnyConditionOfType(ConditionSeedOfLife, ConditionVerdancy14);

            if (!hasVerdancy)
            {
                removedFrom.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionRevitalizingBoon);
            }

            var druidLevel = GetDruidLevel(rulesetCondition.sourceGuid);

            if (druidLevel > 0 && removedFrom.CurrentHitPoints > 0)
            {
                removedFrom.ReceiveHealing(druidLevel * 2, true, rulesetCondition.guid);
            }
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            // Empty
        }
    }

    private sealed class ModifyMagicEffectRevitalizingBoon : IModifyMagicEffect
    {
        private readonly ConditionDefinition _conditionRevitalizingBoon;

        public ModifyMagicEffectRevitalizingBoon(ConditionDefinition conditionRevitalizingBoon)
        {
            _conditionRevitalizingBoon = conditionRevitalizingBoon;
        }

        public EffectDescription ModifyEffect(BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character, RulesetEffect rulesetEffect)
        {
            if (definition is FeatureDefinitionPower { Name: $"Power{Name}SeedOfLife" })
            {
                effectDescription.EffectForms.Add(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(_conditionRevitalizingBoon, ConditionForm.ConditionOperation.Add)
                        .Build());

                return effectDescription;
            }

            if (definition is not SpellDefinition spell)
            {
                return effectDescription;
            }

            var hasHealingForm =
                effectDescription.EffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Healing);

            if (!hasHealingForm && spell != LesserRestoration && spell != GreaterRestoration)
            {
                return effectDescription;
            }

            effectDescription.EffectForms.Add(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(_conditionRevitalizingBoon, ConditionForm.ConditionOperation.Add)
                    .Build());

            return effectDescription;
        }
    }
}
