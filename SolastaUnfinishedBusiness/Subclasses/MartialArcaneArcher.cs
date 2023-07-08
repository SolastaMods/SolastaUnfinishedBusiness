using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialArcaneArcher : AbstractSubclass
{
    private const string Name = "MartialArcaneArcher";

    private const ActionDefinitions.Id ArcaneArcherToggle = (ActionDefinitions.Id)ExtraActionId.ArcaneArcherToggle;

    internal MartialArcaneArcher()
    {
        // LEVEL 03

        // Arcane Lore

        var proficiencyArcana = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Arcana")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        var proficiencyNature = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Nature")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Nature)
            .AddToDB();

        var featureSetArcaneLore = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneLore")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyArcana, proficiencyNature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddToDB();

        // Arcane Magic

        var spellListArcaneMagic = SpellListDefinitionBuilder
            .Create($"SpellList{Name}ArcaneMagic")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use druid spell list, so custom cantrips selected for druid will show here 
        spellListArcaneMagic.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells;

        var castSpellArcaneMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, $"CastSpell{Name}ArcaneMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListArcaneMagic)
            .AddToDB();

        // Arcane Shot

        var actionAffinityAudaciousWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityArcaneArcherToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.ArcaneArcherToggle)
            .AddToDB();

        var arcaneShotPowers = BuildArcaneShotPowers();

        var powerArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneShot")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((_, character, _) =>
                    character.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerArcaneShot, true, arcaneShotPowers);

        var arcaneShotPowersWithoutAttackRoll = BuildArcaneShotPowersWithoutAttackRoll(powerArcaneShot);

        CreateArcaneArcherChoices(arcaneShotPowers.Union(arcaneShotPowersWithoutAttackRoll));

        var invocationPoolArcaneArcherChoice1 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneArcherChoice1")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .AddToDB();

        var invocationPoolArcaneArcherChoice2 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneArcherChoice2")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice, 2)
                .AddToDB();

        var featureSetArcaneShot = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneShot")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                actionAffinityAudaciousWhirlToggle,
                invocationPoolArcaneArcherChoice2,
                powerArcaneShot)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerSwiftBlade)
            .AddFeaturesAtLevel(3, featureSetArcaneLore, castSpellArcaneMagic, featureSetArcaneShot)
            .AddFeaturesAtLevel(7, invocationPoolArcaneArcherChoice1)
            .AddFeaturesAtLevel(10, invocationPoolArcaneArcherChoice1)
            .AddFeaturesAtLevel(15, invocationPoolArcaneArcherChoice1)
            .AddFeaturesAtLevel(18, invocationPoolArcaneArcherChoice1)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static List<FeatureDefinitionPower> BuildArcaneShotPowers()
    {
        // Banishing Arrow

        var powerBanishingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BanishingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Beguiling Arrow

        var powerBeguilingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeguilingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Enfeebling Arrow

        var powerEnfeeblingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EnfeeblingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Bursting Arrow

        var powerBurstingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BurstingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Grasping Arrow

        var powerGraspingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GraspingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Shadow Arrow

        var powerShadowArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        return new List<FeatureDefinitionPower>
        {
            powerBanishingArrow,
            powerBeguilingArrow,
            powerBurstingArrow,
            powerEnfeeblingArrow,
            powerGraspingArrow,
            powerShadowArrow
        };
    }

    private static IEnumerable<FeatureDefinitionPower> BuildArcaneShotPowersWithoutAttackRoll(
        FeatureDefinitionPower powerPool)
    {
        // Piercing Arrow

        var powerPiercingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PiercingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .AddToDB();

        // Seeking Arrow

        var powerSeekingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SeekingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .AddToDB();

        return new List<FeatureDefinitionPower> { powerPiercingArrow, powerSeekingArrow };
    }

    private static void CreateArcaneArcherChoices(IEnumerable<FeatureDefinitionPower> powers)
    {
        foreach (var power in powers)
        {
            var name = power.Name.Replace("Power", string.Empty);
            var guiPresentation = power.guiPresentation;

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{name}")
                .SetGuiPresentation(guiPresentation)
                .SetPoolType(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .SetGrantedFeature(power)
                .SetCustomSubFeatures(HiddenInvocation.Marker)
                .AddToDB();
        }
    }
}
