using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Subclasses;
internal sealed class PathOfTheSpirits : AbstractSubclass
{
    private const string SubclassName = "PathOfTheSpirits";

    internal PathOfTheSpirits()
    {
        #region 3rd LEVEL FEATURES

        // Spirit Seeker
        // Yours is a path that seeks attunement with the natural world, giving you a kinship with beasts.
        // At 3rd level when you adopt this path, you gain the ability to cast the AnimalFriendship and FindTraps spells at will.
        var featureSetPathOfTheSpiritsSpiritSeeker = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheSpiritsSpiritSeeker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritSeekerSpell(SpellDefinitions.AnimalFriendship),
                BuildSpiritSeekerSpell(SpellDefinitions.FindTraps)
            )
            .AddToDB();


        // Animal Spirit
        // At 3rd level, when you adopt this path, you choose an animal spirit as a guide and gain its feature.
        var featureSetPathOfTheSpiritsAnimalSpirit = FeatureDefinitionFeatureSetBuilder
        .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice, "PathOfTheSpiritsAnimalSpiritChoices")
        .SetGuiPresentation(Category.Feature)
        .ClearFeatureSet()
        .AddFeatureSet(
            // Bear: While raging, you have resistance to all damage except psychic damage. The spirit of the bear makes you tough enough to stand up to any punishment.
            BuildAnimalSpiritChoice("Bear", PowerBearResistance()),
            // Eagle: (rogue's cunning action) The spirit of the eagle makes you into a nimble predator who can weave through the fray with ease. You can take the Dash, Disengage, or Hide action as a Bonus Action.
            BuildAnimalSpiritChoice("Eagle", ActionAffinityRogueCunningAction),
            // Wolf: The spirit of the wolf makes you a leader of hunters. While you're raging, your friends have advantage on melee attack rolls against any creature within 5 feet of you that is hostile to you.
            BuildAnimalSpiritChoice("Wolf", CombatAffinityPackTactics) // Needs to be implemented, right now gives advantage to self instead of friends
        )
        .AddToDB();

        #endregion


        #region 6th LEVEL FEATURES

        // Animal Aspect
        // At 6th level, you gain a magical aspect (benefit) based on the spirit animal of your choice. You can choose the same animal you selected at 3rd level or a different one.
        var featureSetPathOfTheSpiritsAnimalAspect = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice, "PathOfTheSpiritsAnimalAspectChoices")
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSet(
                //Bear: You gain the might of a bear. Your HP increases by 2 points for every level you take in this class and have advantage on Strenght Checks.
                BuildAnimalAspectChoice("Bear", PowerBearDurability(), PowerBearMight()),
                //Eagle: You gain the eyesight of an eagle. You gain superior darkvision and Keen Sight.
                BuildAnimalAspectChoice("Eagle", SenseSuperiorDarkvision, AbilityCheckAffinityKeenSight),
                //Wolf: You gain the hunting sensibilities of a wolf. You can track other creatures by smell and hearing (Keen Smell and Keen Hearing). You also gain the ability of casting the IdentifyCreatures spell at will.
                BuildAnimalAspectChoice("Wolf", PowerWolfKnowledge(), AbilityCheckAffinityKeenSmell, AbilityCheckAffinityKeenHearing)
            )
            .AddToDB();

        #endregion

        #region 10th LEVEL FEATURES

        // Spirit Walker
        // At 10th level, as a Bonus Action, you can summon the protection of your Animal Spirit to guide and protect you in a 15ft sphere area around you for 10 minutes.
        // Enemy creatures in this sphere are affected by this feature. An affected creature's speed is halved in the area, and when the creature enters
        // the area for the first time on a turn or starts its turn there, it must make a Wisdom saving throw.
        // On a failed save, the creature takes 3d8 radiant damage (if you are good or neutral) or 3d8 necrotic damage (if you are evil).
        // On a successful save, the creature takes half as much damage.
        // You can use this feature 3 times per long rest.
        var featureSetPathOfTheSpiritsSpiritWalker = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheSpiritsSpiritWalker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(PowerSpiritGuardian())
            .AddToDB();

        #endregion



        Subclass = CharacterSubclassDefinitionBuilder
        .Create(SubclassName)
        .SetGuiPresentation(Category.Subclass, MartialMountaineer)
        .AddFeaturesAtLevel(3,
            featureSetPathOfTheSpiritsSpiritSeeker,
            featureSetPathOfTheSpiritsAnimalSpirit)
        .AddFeaturesAtLevel(6,
            featureSetPathOfTheSpiritsAnimalAspect)
        .AddFeaturesAtLevel(10,
            featureSetPathOfTheSpiritsSpiritWalker)
        .AddToDB();
    }
    private static FeatureDefinition BuildSpiritSeekerSpell(SpellDefinition spellDefinition)
    {
        return FeatureDefinitionPowerBuilder
            .Create($"PathOfTheSpirits{spellDefinition.name}")
            .SetGuiPresentation(spellDefinition.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.AtWill)
            .SetEffectDescription(spellDefinition.EffectDescription)
            .AddToDB();
    }
    private static FeatureDefinitionFeatureSet BuildAnimalSpiritChoice(string name, params FeatureDefinition[] featureDefinitions)
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create($"PathOfTheSpiritsAnimalSpiritChoice{name}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureDefinitions)
            .AddToDB();
    }
    private static FeatureDefinitionFeatureSet BuildAnimalAspectChoice(string name, params FeatureDefinition[] featureDefinitions)
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create($"PathOfTheSpiritsAnimalAspectChoice{name}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureDefinitions)
            .AddToDB();
    }

    private static FeatureDefinition PowerBearResistance()
    {
        return FeatureDefinitionPowerBuilder
            .Create($"PowerPathOfTheSpiritsBearResistance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"ConditionPathOfTheSpiritsBearResistance")
                        .SetGuiPresentationNoContent(true)
                        .SetConditionType(ConditionType.Beneficial)
                        .SetDuration(DurationType.Permanent)
                        .SetTerminateWhenRemoved()
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialInterruptions(ConditionInterruption.RageStop)
                        .SetFeatures(
                            DamageAffinityPoisonResistance,
                            DamageAffinityAcidResistance,
                            DamageAffinityColdResistance,
                            DamageAffinityFireResistance,
                            DamageAffinityThunderResistance,
                            DamageAffinityLightningResistance,
                            DamageAffinityNecroticResistance)
                        .SetAllowMultipleInstances(false)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition PowerBearDurability()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("PathOfTheSpiritsBearDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
            .AddToDB();
    }
#if false
    private static FeatureDefinition PowerWolfLeadership()
    {
        var wolfLeadershipCondition = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheSpiritsWolfLeadershipAura")
            .SetGuiPresentationNoContent(true)
            .SetConditionType(ConditionType.Beneficial)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(CombatAffinityReckless)
            .AddToDB();

        var wolfLeadershipEffect = EffectFormBuilder
            .Create()
            .SetBonusMode(AddBonusMode.None)
            .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
            .CreatedByCharacter()
            .SetConditionForm(wolfLeadershipCondition, ConditionForm.ConditionOperation.Add)
            .Build();
            

        return FeatureDefinitionPowerBuilder
            .Create("PathOfTheSpiritsWolfLeadershipAura")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent, 0, TurnOccurenceType.EndOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Self, 2, TargetType.Cube, 5, 2)
                .SetTargetProximityData(true, 10)
                .ExcludeCaster()
                .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetEffectForms(wolfLeadershipEffect)
                .Build())
            .AddToDB();
    }
#endif
    private static FeatureDefinition PowerBearMight()
    {
        return FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("PathOfTheSpiritsBearMight")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0, (AttributeDefinitions.Strength, string.Empty))
            .AddToDB();
    }

    private static FeatureDefinition PowerWolfKnowledge()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheSpiritsWolfKnowledge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(BuildSpiritSeekerSpell(SpellDefinitions.IdentifyCreatures))
            .AddToDB();
    }

    private static FeatureDefinition PowerSpiritGuardian()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PathOfTheSpiritsSpiritGuardians")
            .SetGuiPresentation(SpellDefinitions.SpiritGuardians.guiPresentation)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 3)
            .SetEffectDescription(SpellDefinitions.SpiritGuardians.EffectDescription)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
}
