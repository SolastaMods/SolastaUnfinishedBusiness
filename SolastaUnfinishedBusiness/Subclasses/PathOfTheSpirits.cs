using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheSpirits : AbstractSubclass
{
    private const string Name = "PathOfTheSpirits";

    public PathOfTheSpirits()
    {
        #region 3rd LEVEL FEATURES

        // Spirit Seeker
        // Yours is a path that seeks attunement with the natural world, giving you a kinship with beasts.
        // At 3rd level when you adopt this path, you gain the ability to cast the AnimalFriendship and FindTraps spells at will.
        var featureSetPathOfTheSpiritsSpiritSeeker = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheSpiritsSpiritSeeker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritSeekerSpell(SpellDefinitions.AnimalFriendship),
                BuildSpiritSeekerSpell(SpellDefinitions.FindTraps))
            .AddToDB();

        // Animal Spirit
        // At 3rd level, when you adopt this path, you choose an animal spirit as a guide and gain its feature.
        var featureSetPathOfTheSpiritsAnimalSpirit = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheSpiritsAnimalSpiritChoices")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                // Bear: While raging, you have resistance to all damage except psychic damage. The spirit of the bear makes you tough enough to stand up to any punishment.
                PowerPathOfTheSpiritsBearResistance(),
                // Eagle: The spirit of the eagle makes you into a nimble predator who can weave through the fray with ease. You can take the Dash, Disengage, or Hide action as a Bonus Action.
                FeatureDefinitionActionAffinityBuilder
                    .Create(ActionAffinityRogueCunningAction, "ActionAffinityPathOfTheSpiritsCunningAction")
                    .SetOrUpdateGuiPresentation(Category.Feature)
                    .AddToDB(),
                // Wolf: The spirit of the wolf makes you a leader of hunters. While you're raging, your friends have advantage on melee attack rolls against any creature within 5 feet of you that is hostile to you.
                PowerPathOfTheSpiritsWolfLeadership())
            .AddToDB();

        #endregion

        #region 6th LEVEL FEATURES

        // Animal Aspect
        // At 6th level, you gain a magical aspect (benefit) based on the spirit animal of your choice. You can choose the same animal you selected at 3rd level or a different one.
        var featureSetPathOfTheSpiritsAnimalAspect = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheSpiritsAnimalAspectChoices")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                //Bear: You gain the might of a bear. Your HP increases by 2 points for every level you take in this class and have advantage on Strength Checks.
                BuildAnimalAspectChoice("Bear",
                    AttributeModifierBearDurability(),
                    AbilityCheckAffinityPathOfTheSpiritsBearMight()),
                //Eagle: You gain the eyesight of an eagle. You gain superior darkvision and Keen Sight.
                BuildAnimalAspectChoice("Eagle",
                    SenseSuperiorDarkvision,
                    AbilityCheckAffinityKeenSight),
                //Wolf: You gain the hunting sensibilities of a wolf. You can track other creatures by smell and hearing (Keen Smell and Keen Hearing). You also gain the ability of casting the IdentifyCreatures spell at will.
                BuildAnimalAspectChoice("Wolf",
                    BuildSpiritSeekerSpell(SpellDefinitions.IdentifyCreatures),
                    AbilityCheckAffinityKeenSmell,
                    AbilityCheckAffinityKeenHearing))
            .AddToDB();

        #endregion

        #region 10th LEVEL FEATURES

        // Spirit Walker
        // At 10th level, as a Bonus Action, you can summon the protection of your Animal Spirit to guide and protect you in a 15ft sphere area around you for 10 minutes.
        // Enemy creatures in this sphere are affected by this feature. An affected creature's speed is halved in the area, and when the creature enters
        // the area for the first time on a turn or starts its turn there, it must make a Wisdom saving throw.
        // On a failed save, the creature takes 3d8 radiant damage (if you are good or neutral) or 3d8 necrotic damage (if you are evil).
        // On a successful save, the creature takes half as much damage.
        // You can use this feature a number of times equal to your proficiency modifier.
        var featureSetPathOfTheSpiritsSpiritWalker = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheSpiritsSpiritWalker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(PowerSpiritGuardian())
            .AddToDB();

        #endregion

        #region 14th LEVEL FEATURES

        // Animal Aspect
        // At 14th level, you gain a magical aspect (benefit) based on the spirit animal of your choice. You can choose the same animal you selected at previous levels or a different one.
        var featureSetPathOfTheSpiritsHonedAnimalAspects = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheSpiritsHonedAnimalAspectChoices")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                //Bear: You hone your bear aspect. Gain an aura while raging applying disadvantages on enemies while attacking those other than you.
                BuildAnimalAspectChoice("HonedBear",
                    PowerPathOfTheSpiritsHonedBear()),
                //Eagle: You hone your eagle aspect. Gain the ability to fly without extra movement while raging.
                BuildAnimalAspectChoice("HonedEagle",
                    PowerPathOfTheSpiritsHonedEagle()),
                //Wolf: You hone your wolf aspect. Gain the ability to shove as a bonus action while raging.
                BuildAnimalAspectChoice("HonedWolf",
                    PowerPathOfTheSpiritsHonedWolf()))
            .AddToDB();

        #endregion

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("PathOfTheSpirits", Resources.PathOfTheSpirits, 256))
            .AddFeaturesAtLevel(3,
                featureSetPathOfTheSpiritsSpiritSeeker,
                featureSetPathOfTheSpiritsAnimalSpirit)
            .AddFeaturesAtLevel(6,
                featureSetPathOfTheSpiritsAnimalAspect)
            .AddFeaturesAtLevel(10,
                featureSetPathOfTheSpiritsSpiritWalker)
            .AddFeaturesAtLevel(14,
                featureSetPathOfTheSpiritsHonedAnimalAspects)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinition BuildSpiritSeekerSpell(SpellDefinition spellDefinition)
    {
        var effectDescription = EffectDescriptionBuilder
            .Create(spellDefinition.EffectDescription)
            .Build();

        // hack as Barbs don't have repertoires to get DC from spell casting feature (easier than recreate effect)
        effectDescription.difficultyClassComputation = EffectDifficultyClassComputation.AbilityScoreAndProficiency;

        return FeatureDefinitionPowerBuilder
            .Create($"PowerPathOfTheSpirits{spellDefinition.name}")
            .SetGuiPresentation(spellDefinition.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(effectDescription)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildAnimalAspectChoice(
        string name,
        params FeatureDefinition[] featureDefinitions)
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSetPathOfTheSpiritsAnimalAspectChoice{name}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureDefinitions)
            .AddToDB();
    }

    private static FeatureDefinition PowerPathOfTheSpiritsBearResistance()
    {
        var conditionPathOfTheSpiritsBearResistance = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheSpiritsBearResistance")
            .SetGuiPresentation("PowerPathOfTheSpiritsBearResistance", Category.Feature,
                ConditionDefinitions.ConditionBarkskin)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.RageStop, ConditionInterruption.BattleEnd)
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .AddToDB();

        return FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsBearResistance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Dispelled)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPathOfTheSpiritsBearResistance,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();
    }

    private static FeatureDefinition AttributeModifierBearDurability()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierPathOfTheSpiritsBearDurability")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
            .AddToDB();
    }

    private static FeatureDefinition PowerPathOfTheSpiritsWolfLeadership()
    {
        var conditionPathOfTheSpiritsWolfLeadershipPack = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheSpiritsWolfLeadershipPack")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityPathOfTheSpiritsWolfLeadershipPack")
                    .SetGuiPresentation("ConditionPathOfTheSpiritsWolfLeadershipPack", Category.Condition,
                        Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        var powerPathOfTheSpiritsWolfLeadership = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsWolfLeadership")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPathOfTheSpiritsWolfLeadershipPack,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var customBehaviorWolfLeadership = new CustomRagingAura(
            powerPathOfTheSpiritsWolfLeadership, conditionPathOfTheSpiritsWolfLeadershipPack, true);

        conditionPathOfTheSpiritsWolfLeadershipPack.AddCustomSubFeatures(customBehaviorWolfLeadership);
        powerPathOfTheSpiritsWolfLeadership.AddCustomSubFeatures(customBehaviorWolfLeadership);

        return powerPathOfTheSpiritsWolfLeadership;
    }

    private static FeatureDefinition AbilityCheckAffinityPathOfTheSpiritsBearMight()
    {
        return FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityPathOfTheSpiritsBearMight")
            .SetGuiPresentationNoContent(true)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Strength, string.Empty))
            .AddToDB();
    }

    private static FeatureDefinition PowerSpiritGuardian()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsSpiritGuardians")
            .SetGuiPresentation(SpellDefinitions.SpiritGuardians.guiPresentation)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.SpiritGuardians.EffectDescription)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .Build())
            .AddToDB();
    }

    private static FeatureDefinition PowerPathOfTheSpiritsHonedBear()
    {
        var conditionHonedAnimalAspectsBear = ConditionDefinitionBuilder
            .Create($"Condition{Name}HonedAnimalAspectsBear")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityPowerHonedAnimalAspectsBear")
                    .SetGuiPresentation($"Condition{Name}HonedAnimalAspectsBear", Category.Condition,
                        Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .SetSituationalContext(ExtraSituationalContext.TargetIsNotEffectSource)
                    .AddToDB())
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerHonedAnimalAspectsBear = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsBear")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .ExcludeCaster()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionHonedAnimalAspectsBear,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var customBehaviorHonedBear = new CustomRagingAura(
            powerHonedAnimalAspectsBear, conditionHonedAnimalAspectsBear, false);

        conditionHonedAnimalAspectsBear.AddCustomSubFeatures(customBehaviorHonedBear);
        powerHonedAnimalAspectsBear.AddCustomSubFeatures(customBehaviorHonedBear);

        return powerHonedAnimalAspectsBear;
    }

    private static FeatureDefinition PowerPathOfTheSpiritsHonedEagle()
    {
        var conditionHonedAnimalAspectsEagle = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlying12, $"Condition{Name}HonedAnimalAspectsEagle")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFlying12)
            .AddSpecialInterruptions(ConditionInterruption.RageStop)
            .SetFeatures(FeatureDefinitionMoveModes.MoveModeFly8)
            .AddToDB();

        var powerHonedAnimalAspectsEagle = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsEagle")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionHonedAnimalAspectsEagle, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        return powerHonedAnimalAspectsEagle;
    }

    private static FeatureDefinition PowerPathOfTheSpiritsHonedWolf()
    {
        var actionAffinityHonedAnimalAspectsWolf = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinityMountaineerShieldCharge, "ActionAffinityHonedAnimalAspectsWolf")
            .AddToDB();

        var conditionHonedAnimalAspectsWolf = ConditionDefinitionBuilder
            .Create($"Condition{Name}HonedAnimalAspectsWolf")
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(actionAffinityHonedAnimalAspectsWolf)
            .AddSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerHonedAnimalAspectsWolf = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsWolf")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionHonedAnimalAspectsWolf, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        return powerHonedAnimalAspectsWolf;
    }
}
