using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

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
            .SetSpellcastingClass(Druid)
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
            .SetCustomSubFeatures(new ModifyEffectDescriptionVerdancy(conditionVerdancy, conditionVerdancy14))
            .AddToDB();

        // Seed of Life

        var conditionSeedOfLife = ConditionDefinitionBuilder
            .Create(ConditionSeedOfLife)
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetPossessive()
            .CopyParticleReferences(ConditionGuided)
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
            .SetFeatures(DamageAffinityNecroticResistance, SavingThrowAffinityDwarvenPlate)
            .AddToDB();

        var featureRevitalizingBoom = FeatureDefinitionBuilder
            .Create($"Feature{Name}RevitalizingBoon")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyEffectDescriptionRevitalizingBoon(conditionRevitalizingBoom, powerSeedOfLife))
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
        foreach (var spellDefinition in SpellListAllSpells
                     .SpellsByLevel
                     .SelectMany(x => x.Spells)
                     .Where(x => x.EffectDescription.EffectForms
                         .Any(y => y.FormType == EffectForm.EffectFormType.Healing)))
        {
            if (spellDefinition.SpellsBundle)
            {
                foreach (var spellInBundle in spellDefinition.SubspellsList)
                {
                    MagicAffinityHarmoniousBloom.WarListSpells.Add(spellInBundle.Name);
                }
            }
            else
            {
                MagicAffinityHarmoniousBloom.WarListSpells.Add(spellDefinition.Name);
            }
        }
    }

    private static int GetDruidLevel(ulong guid)
    {
        var caster = EffectHelpers.GetCharacterByGuid(guid);
        var hero = caster.GetOriginalHero();

        return hero?.GetClassLevel(DruidClass) ?? 0;
    }

    private static bool IsAuthorizedSpell(EffectDescription effectDescription, BaseDefinition baseDefinition)
    {
        if (baseDefinition is not SpellDefinition spellDefinition)
        {
            return false;
        }

        var hasHealingForm =
            effectDescription.EffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Healing);

        return hasHealingForm || spellDefinition == LesserRestoration || spellDefinition == GreaterRestoration;
    }

    private static void RemoveRevitalizingBoonIfRequired(RulesetActor removedFrom)
    {
        var hasVerdancy =
            removedFrom.HasAnyConditionOfType(ConditionSeedOfLife, ConditionVerdancy, ConditionVerdancy14);

        if (!hasVerdancy)
        {
            removedFrom.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionRevitalizingBoon);
        }
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
            RemoveRevitalizingBoonIfRequired(removedFrom);
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class ModifyEffectDescriptionVerdancy : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionVerdancy;
        private readonly ConditionDefinition _conditionVerdancy14;

        public ModifyEffectDescriptionVerdancy(
            ConditionDefinition conditionVerdancy,
            ConditionDefinition conditionVerdancy14)
        {
            _conditionVerdancy = conditionVerdancy;
            _conditionVerdancy14 = conditionVerdancy14;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return IsAuthorizedSpell(effectDescription, definition);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(Druid);
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
            RemoveRevitalizingBoonIfRequired(removedFrom);

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

    private sealed class ModifyEffectDescriptionRevitalizingBoon : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionRevitalizingBoon;
        private readonly FeatureDefinitionPower _powerSeedOfLife;

        public ModifyEffectDescriptionRevitalizingBoon(
            ConditionDefinition conditionRevitalizingBoon,
            FeatureDefinitionPower powerSeedOfLife)
        {
            _conditionRevitalizingBoon = conditionRevitalizingBoon;
            _powerSeedOfLife = powerSeedOfLife;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == _powerSeedOfLife || IsAuthorizedSpell(effectDescription, definition);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms.Add(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(_conditionRevitalizingBoon, ConditionForm.ConditionOperation.Add)
                    .Build());

            return effectDescription;
        }
    }
}
