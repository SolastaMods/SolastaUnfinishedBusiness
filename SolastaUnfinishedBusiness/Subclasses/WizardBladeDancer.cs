using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardBladeDancer : AbstractSubclass
{
    private const string Name = "BladeDancer";
    private const string BladeDanceTitle = $"Feature/&FeatureSet{Name}BladeDanceTitle";

    public WizardBladeDancer()
    {
        // LEVEL 02

        // Caster Fighting

        var proficiencyBladeDancerLightArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}LightArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory)
            .AddToDB();

        var proficiencyBladeDancerMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}MartialWeapon")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var featureSetCasterBladeDancerFighting = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSetCaster{Name}Fighting")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyBladeDancerLightArmor, proficiencyBladeDancerMartialWeapon)
            .AddToDB();

        // Blade Dance

        ConditionBladeDancerBladeDance = ConditionDefinitionBuilder
            .Create($"Condition{Name}BladeDance")
            .SetGuiPresentation(BladeDanceTitle, Gui.NoLocalization, ConditionHeroism)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}BladeDance")
                    .SetGuiPresentation($"Condition{Name}BladeDance", Category.Condition, Gui.NoLocalization)
                    .SetModifierAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Intelligence)
                    .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(AbilityCheckAffinityIslandHalflingAcrobatics,
                        $"AbilityCheckAffinity{Name}BladeDanceAcrobatics")
                    .SetGuiPresentation($"Condition{Name}BladeDance", Category.Condition, Gui.NoLocalization)
                    .AddToDB(),
                // keep name for compatibility reasons
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}BladeDanceConstitution")
                    .SetGuiPresentation($"Condition{Name}BladeDance", Category.Condition, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Constitution)
                    .AddToDB())
            .AddCustomSubFeatures(new CheckDanceValidity())
            .AddToDB();

        var powerBladeDancerBladeDance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BladeDance")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerClericDivineInterventionWizard)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionBladeDancerBladeDance, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(IsBladeDanceValid))
            .AddToDB();

        // LEVEL 10

        // Dance of Defense

        ConditionBladeDancerDanceOfDefense = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerBladeDance, $"Condition{Name}DanceOfDefense")
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create($"ReduceDamage{Name}DanceOfDefense")
                    .SetNotificationTag("DanceOfDefense")
                    .SetGuiPresentation(Category.Feature)
                    .SetConsumeSpellSlotsReducedDamage(CharacterClassDefinitions.Wizard, (_, _) => 5)
                    .AddToDB())
            .AddCustomSubFeatures(new CheckDanceValidity())
            .AddToDB();

        var powerBladeDancerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, $"Power{Name}DanceOfDefense")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionBladeDancerDanceOfDefense, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerBladeDancerBladeDance)
            .AddToDB();

        // LEVEL 14

        // Dance of Victory

        ConditionBladeDancerDanceOfVictory = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerDanceOfDefense, $"Condition{Name}DanceOfVictory")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{Name}DanceOfVictory")
                    .SetGuiPresentation($"Condition{Name}DanceOfVictory", Category.Condition, Gui.NoLocalization)
                    .SetDamageRollModifier(5)
                    .AddToDB())
            .AddCustomSubFeatures(new CheckDanceValidity())
            .AddToDB();

        var powerBladeDancerDanceOfVictory = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, $"Power{Name}DanceOfVictory")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionBladeDancerDanceOfVictory, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerBladeDancerDanceOfDefense)
            .AddToDB();

        //
        // use sets for better descriptions on level up
        //

        var featureSetBladeDancerBladeDance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BladeDance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerBladeDancerBladeDance)
            .AddToDB();

        var featureSetBladeDancerDanceOfDefense = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DanceOfDefense")
            .SetGuiPresentation($"ReduceDamage{Name}DanceOfDefense", Category.Feature)
            .AddFeatureSet(powerBladeDancerDanceOfDefense)
            .AddToDB();

        var featureSetBladeDancerDanceOfVictory = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DanceOfVictory")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerBladeDancerDanceOfVictory)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Wizard{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.WizardBladeDancer, 256))
            .AddFeaturesAtLevel(2,
                featureSetCasterBladeDancerFighting,
                featureSetBladeDancerBladeDance)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                featureSetBladeDancerDanceOfDefense)
            .AddFeaturesAtLevel(14,
                featureSetBladeDancerDanceOfVictory)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    private static ConditionDefinition ConditionBladeDancerBladeDance { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfDefense { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfVictory { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool IsBladeDanceValid(RulesetCharacter hero)
    {
        var gameLocationCharacter = GameLocationCharacter.GetFromActor(hero);

        return gameLocationCharacter != null &&
               gameLocationCharacter.CanAct() &&
               !hero.IsWearingMediumArmor() &&
               !hero.IsWearingHeavyArmor() &&
               !hero.IsWearingShield() &&
               !hero.IsWieldingTwoHandedWeapon();
    }

    private sealed class CheckDanceValidity : IOnItemEquipped
    {
        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (IsBladeDanceValid(hero))
            {
                return;
            }

            foreach (var rulesetCondition in hero.allConditions
                         .Where(x => x.ConditionDefinition == ConditionBladeDancerBladeDance ||
                                     x.ConditionDefinition == ConditionBladeDancerDanceOfDefense ||
                                     x.ConditionDefinition == ConditionBladeDancerDanceOfVictory)
                         .ToList())
            {
                hero.RemoveCondition(rulesetCondition);
            }
        }
    }
}
