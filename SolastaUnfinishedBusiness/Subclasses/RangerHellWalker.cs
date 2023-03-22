using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerHellWalker : AbstractSubclass
{
    private const string Name = "RangerHellWalker";

    internal RangerHellWalker()
    {
        // LEVEL 03

        // Hell Walker Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, HellishRebuke),
                BuildSpellGroup(5, Invisibility),
                BuildSpellGroup(9, BestowCurse),
                BuildSpellGroup(13, WallOfFire),
                BuildSpellGroup(17, SpellsContext.FarStep))
            .AddToDB();

        var powerFirebolt = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Firebolt")
            .SetGuiPresentation(FireBolt.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(FireBolt.EffectDescription)
            .AddToDB();

        var featureSetFirebolt = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Firebolt")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerFirebolt)
            .AddToDB();

        // Damming Strike

        var conditionDammingStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}DammingStrike")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionOnFire)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 1, DieType.D6)
                    .Build())
            .AddToDB();

        var additionalDamageDammingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DammingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetAttackOnly()
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetSavingThrowData()
            .SetConditionOperations(new ConditionOperationDescription
            {
                canSaveToCancel = true,
                hasSavingThrow = true,
                saveOccurence = TurnOccurenceType.StartOfTurn,
                conditionDefinition = conditionDammingStrike,
                operation = ConditionOperationDescription.ConditionOperation.Add,
                saveAffinity = EffectSavingThrowType.Negates
            })
            .AddToDB();

        // Cursed Tongue

        var proficiencyCursedTongue = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}CursedTongue")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Abyssal", "LanguageInfernal")
            .AddToDB();

        // LEVEL 07

        // Burning Constitution

        var featureSetBurningConstitution = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BurningConstitution")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance)
            .AddToDB();

        // LEVEL 11

        // Mark of the Dammed

        var conditionMarkOfTheDammed = ConditionDefinitionBuilder
            .Create($"Condition{Name}MarkOfTheDammed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDispellingEvilAndGood)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire)
            .SetFeatures(ConditionDefinitions.ConditionFrightened.Features)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                    .Build())
            .AddToDB();

        var powerMarkOfTheDammed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MarkOfTheDammed")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("MarkOfTheDammed", Resources.PowerMarkOfTheDammed, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .SetConditionForm(conditionMarkOfTheDammed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        conditionDammingStrike.cancellingConditions = new List<ConditionDefinition> { conditionMarkOfTheDammed };

        powerMarkOfTheDammed.SetCustomSubFeatures(
            new CustomBehaviorMarkOfTheDammed(powerMarkOfTheDammed, conditionMarkOfTheDammed));

        // LEVEL 15

        // Fiendish Spawn

        // You can cast summon fiend as a 6th level spell without expending a spell slot. When cast this way the spell duration becomes 1 min. You can use this feature once per long rest.

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerHellWalker, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetFirebolt,
                additionalDamageDammingStrike,
                proficiencyCursedTongue)
            .AddFeaturesAtLevel(7,
                featureSetBurningConstitution)
            .AddFeaturesAtLevel(11,
                powerMarkOfTheDammed)
            .AddFeaturesAtLevel(15)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Mark of the Dammed
    //

    private sealed class CustomBehaviorMarkOfTheDammed : IIgnoreDamageAffinity, IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public CustomBehaviorMarkOfTheDammed(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
        }

        public bool CanIgnoreDamageAffinity(
            IDamageAffinityProvider provider, RulesetActor rulesetActor, string damageType)
        {
            return !rulesetActor.HasConditionOfType(_conditionDefinition.Name) ||
                   (provider.DamageAffinityType == DamageAffinityType.Resistance && damageType == DamageTypeFire);
        }

        public void OnAfterAction(CharacterAction action)
        {
            var battle = Gui.Battle;

            if (battle == null || action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _featureDefinitionPower)
            {
                return;
            }

            var gameLocationDefender = action.actionParams.targetCharacters[0];

            // defender saved so no need to remove condition from other enemies
            if (!gameLocationDefender.RulesetCharacter.HasConditionOfType(_conditionDefinition.Name))
            {
                return;
            }

            // remove this condition from any other enemy
            foreach (var gameLocationCharacter in battle.EnemyContenders
                         .Where(x => x != gameLocationDefender))
            {
                var rulesetDefender = gameLocationCharacter.RulesetCharacter;
                var rulesetCondition = rulesetDefender.AllConditions
                    .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

                if (rulesetCondition != null)
                {
                    rulesetDefender.RemoveCondition(rulesetCondition);
                }
            }
        }
    }
}
