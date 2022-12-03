using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
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
    private const string PathOfTheSpiritsWolfLeadershipPackName = "ConditionPathOfTheSpiritsWolfLeadershipPack";
    private const string PathOfTheSpiritsWolfLeadershipLeaderName = "ConditionPathOfTheSpiritsWolfLeadershipLeader";

    internal PathOfTheSpirits()
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
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice,
                "FeatureSetPathOfTheSpiritsAnimalSpiritChoices")
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
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
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice,
                "FeatureSetPathOfTheSpiritsAnimalAspectChoices")
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    private static FeatureDefinition BuildSpiritSeekerSpell(SpellDefinition spellDefinition)
    {
        return FeatureDefinitionPowerBuilder
            .Create($"PowerPathOfTheSpirits{spellDefinition.name}")
            .SetGuiPresentation(spellDefinition.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(spellDefinition.EffectDescription)
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
            .SetGuiPresentation("PowerPathOfTheSpiritsBearResistance", Category.Feature)
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
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionPathOfTheSpiritsBearResistance);

        return FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsBearResistance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionPathOfTheSpiritsBearResistance, ConditionForm.ConditionOperation.Add)
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
        const string NAME = "FeatureSetPathOfTheSpiritsWolfLeadership";

        var conditionPathOfTheSpiritsWolfLeadershipLeader = ConditionDefinitionBuilder
            .Create(PathOfTheSpiritsWolfLeadershipLeaderName)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            // notify all allies to remove wolf spirit condition
            .SetCustomSubFeatures(new NotifyRemoval())
            .AddToDB();

        var conditionPathOfTheSpiritsWolfLeadershipPack = ConditionDefinitionBuilder
            .Create(PathOfTheSpiritsWolfLeadershipPackName)
            .SetGuiPresentation(NAME, Category.Feature)
            .SetAllowMultipleInstances(true)
            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                .Create(CombatAffinityRousingShout, "CombatAffinityPathOfTheSpiritsWolfLeadership")
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        ValidatorsCharacter.HasAnyOfConditions(ConditionDefinitions.ConditionRaging)))
                .AddToDB())
            .AddToDB();

        var powerPathOfTheSpiritsWolfLeadershipLeader = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsWolfLeadershipLeader")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionPathOfTheSpiritsWolfLeadershipLeader,
                        ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        var powerPathOfTheSpiritsWolfLeadershipPack = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheSpiritsWolfLeadershipPack")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                // only during the round Barbarian started raging
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionPathOfTheSpiritsWolfLeadershipPack,
                        ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                // ExcludeCaster() doesn't work on Auras so need to make Barb immune to advantage
                FeatureDefinitionConditionAffinityBuilder
                    .Create("ConditionAffinityPathOfTheSpiritsWolfLeadershipPack")
                    .SetGuiPresentationNoContent(true)
                    .SetConditionType(conditionPathOfTheSpiritsWolfLeadershipPack)
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .AddToDB(),
                powerPathOfTheSpiritsWolfLeadershipLeader,
                powerPathOfTheSpiritsWolfLeadershipPack)
            .AddToDB();
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
            .SetEffectDescription(SpellDefinitions.SpiritGuardians.EffectDescription)
            .AddToDB();
    }

    private class NotifyRemoval : INotifyConditionRemoval
    {
        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            RemovePackCondition(removedFrom, rulesetCondition);
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            RemovePackCondition(rulesetActor, rulesetCondition);
        }

        private static void RemovePackCondition(RulesetEntity rulesetActor, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.conditionDefinition.Name != PathOfTheSpiritsWolfLeadershipLeaderName)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (gameLocationCharacterService == null)
            {
                return;
            }

            foreach (var rulesetCharacter in gameLocationCharacterService.ValidCharacters
                         .Select(x => x.RulesetCharacter)
                         .Where(x => x.CurrentFaction == FactionDefinitions.Party))
            {
                if (rulesetCharacter.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                        PathOfTheSpiritsWolfLeadershipPackName, out var condition) &&
                    condition.sourceGuid == rulesetActor.guid)
                {
                    rulesetCharacter.RemoveCondition(condition);
                }
            }
        }
    }
}
