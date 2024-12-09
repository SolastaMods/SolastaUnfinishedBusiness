using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
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

        var featureSetPathOfTheSpiritsSpiritSeeker = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SpiritSeeker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritSeekerSpell(SpellDefinitions.AnimalFriendship, RechargeRate.ShortRest),
                BuildSpiritSeekerSpell(SpellDefinitions.FindTraps, RechargeRate.ShortRest))
            .AddToDB();

        // Animal Spirit

        var featureSetPathOfTheSpiritsAnimalSpirit = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AnimalSpiritChoices")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                PowerPathOfTheSpiritsBearResistance(),
                FeatureDefinitionActionAffinityBuilder
                    .Create(ActionAffinityRogueCunningAction, $"ActionAffinity{Name}CunningAction")
                    .SetOrUpdateGuiPresentation(Category.Feature)
                    .AddToDB())
            .AddFeatureSet(PowerPathOfTheSpiritsWolfLeadership())
            .AddToDB();

        #endregion

        #region 6th LEVEL FEATURES

        // Animal Aspect

        var featureSetPathOfTheSpiritsAnimalAspect = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AnimalAspectChoices")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                BuildAnimalAspectChoice("Bear",
                    FeatureDefinitionAttributeModifierBuilder
                        .Create($"AttributeModifier{Name}BearDurability")
                        .SetGuiPresentationNoContent(true)
                        .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                        .AddToDB(),
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create($"AbilityCheckAffinity{Name}BearMight")
                        .SetGuiPresentationNoContent(true)
                        .BuildAndSetAffinityGroups(
                            CharacterAbilityCheckAffinity.Advantage,
                            abilityProficiencyPairs: (AttributeDefinitions.Strength, string.Empty))
                        .AddToDB()),
                BuildAnimalAspectChoice("Eagle",
                    SenseSuperiorDarkvision,
                    FeatureDefinitionProficiencyBuilder
                        .Create($"Proficiency{Name}Eagle")
                        .SetGuiPresentation("FeatureSetPathOfTheSpiritsAnimalAspectChoiceEagle", Category.Feature)
                        .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Perception)
                        .AddToDB(),
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create($"AbilityCheckAffinity{Name}Eagle")
                        .SetGuiPresentation("FeatureSetPathOfTheSpiritsAnimalAspectChoiceEagle", Category.Feature)
                        .BuildAndSetAffinityGroups(
                            CharacterAbilityCheckAffinity.Advantage,
                            abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
                        .AddToDB()),
                BuildAnimalAspectChoice("Wolf",
                    FeatureDefinitionProficiencyBuilder
                        .Create($"Proficiency{Name}Wolf")
                        .SetGuiPresentation("FeatureSetPathOfTheSpiritsAnimalAspectChoiceWolf", Category.Feature)
                        .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Survival)
                        .AddToDB(),
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create($"AbilityCheckAffinity{Name}Wolf")
                        .SetGuiPresentation("FeatureSetPathOfTheSpiritsAnimalAspectChoiceWolf", Category.Feature)
                        .BuildAndSetAffinityGroups(
                            CharacterAbilityCheckAffinity.Advantage,
                            abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Survival))
                        .AddToDB(),
                    BuildSpiritSeekerSpell(SpellDefinitions.IdentifyCreatures, RechargeRate.LongRest)))
            .AddToDB();

        #endregion

        #region 10th LEVEL FEATURES

        PowerSpiritGuardians.effectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
            ConditionSpiritGuardians;
        PowerSpiritGuardians.effectDescription.EffectForms[2].ConditionForm.ConditionDefinition =
            ConditionSpiritGuardiansSelf;
        PowerSpiritGuardians.AddCustomSubFeatures(
            new ModifyEffectDescriptionSpiritGuardians(PowerSpiritGuardians));

        PowerSpiritGuardiansRageCost.effectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
            ConditionSpiritGuardians;
        PowerSpiritGuardiansRageCost.effectDescription.EffectForms[2].ConditionForm.ConditionDefinition =
            ConditionSpiritGuardiansSelf;
        PowerSpiritGuardiansRageCost.AddCustomSubFeatures(
            new ModifyEffectDescriptionSpiritGuardians(PowerSpiritGuardiansRageCost));

        #endregion

        #region 14th LEVEL FEATURES

        // Animal Aspect
        // At 14th level, you gain a magical aspect (benefit) based on the spirit animal of your choice. You can choose the same animal you selected at previous levels or a different one.
        var featureSetPathOfTheSpiritsHonedAnimalAspects = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}HonedAnimalAspectChoices")
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
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheSpirits, 256))
            .AddFeaturesAtLevel(3,
                featureSetPathOfTheSpiritsSpiritSeeker,
                featureSetPathOfTheSpiritsAnimalSpirit)
            .AddFeaturesAtLevel(6,
                featureSetPathOfTheSpiritsAnimalAspect)
            .AddFeaturesAtLevel(10,
                FeatureSetPathOfTheSpiritsSpiritWalker)
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

    private static FeatureDefinitionFeatureSet BuildAnimalAspectChoice(
        string name,
        params FeatureDefinition[] featureDefinitions)
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AnimalAspectChoice{name}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureDefinitions)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildSpiritSeekerSpell(
        SpellDefinition spellDefinition,
        RechargeRate rechargeRate)
    {
        var effectDescription = EffectDescriptionBuilder
            .Create(spellDefinition.EffectDescription)
            .Build();

        effectDescription.difficultyClassComputation = EffectDifficultyClassComputation.AbilityScoreAndProficiency;

        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{spellDefinition.name}")
            .SetGuiPresentation(spellDefinition.GuiPresentation)
            .SetUsesFixed(ActivationTime.BonusAction, rechargeRate)
            .SetEffectDescription(effectDescription)
            .AddToDB();
    }

    private static FeatureDefinitionPower PowerPathOfTheSpiritsBearResistance()
    {
        var conditionPathOfTheSpiritsBearResistance = ConditionDefinitionBuilder
            .Create($"Condition{Name}BearResistance")
            .SetGuiPresentation($"Power{Name}BearResistance", Category.Feature,
                ConditionDefinitions.ConditionBarkskin)
            .SetPossessive()
            // don't use vanilla RageStop with permanent conditions
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistanceTrue,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistanceTrue,
                DamageAffinityPoisonResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistanceTrue,
                DamageAffinityThunderResistance)
            .AddToDB();

        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BearResistance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
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

    private static FeatureDefinitionFeatureSet PowerPathOfTheSpiritsWolfLeadership()
    {
        var combatAffinityWolfLeadershipPack = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}WolfLeadershipPack")
            .SetGuiPresentation($"Condition{Name}WolfLeadershipPack", Category.Condition,
                Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(ExtraSituationalContext.IsNotConditionSourceNotRanged)
            .AddToDB();

        var conditionPathOfTheSpiritsWolfLeadershipPack = ConditionDefinitionBuilder
            .Create($"Condition{Name}WolfLeadershipPack")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(combatAffinityWolfLeadershipPack)
            .AddToDB();

        combatAffinityWolfLeadershipPack.requiredCondition = conditionPathOfTheSpiritsWolfLeadershipPack;

        var powerPathOfTheSpiritsWolfLeadership = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}WolfLeadership")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionPathOfTheSpiritsWolfLeadershipPack))
                    .Build())
            .AddToDB();

        var featureSetWolfLeadership = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}WolfLeadership")
            .SetGuiPresentation($"Power{Name}WolfLeadership", Category.Feature)
            .AddFeatureSet(powerPathOfTheSpiritsWolfLeadership)
            .AddToDB();

        return featureSetWolfLeadership;
    }

    private static FeatureDefinitionPower PowerPathOfTheSpiritsHonedBear()
    {
        var combatAffinityHonedAnimalAspectsBear = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}HonedAnimalAspectsBear")
            .SetGuiPresentation($"Condition{Name}HonedAnimalAspectsBear", Category.Condition,
                Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .SetSituationalContext(ExtraSituationalContext.IsNotConditionSource)
            .AddToDB();

        var conditionHonedAnimalAspectsBear = ConditionDefinitionBuilder
            .Create($"Condition{Name}HonedAnimalAspectsBear")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(combatAffinityHonedAnimalAspectsBear)
            .AddToDB();

        combatAffinityHonedAnimalAspectsBear.requiredCondition = conditionHonedAnimalAspectsBear;

        var powerHonedAnimalAspectsBear = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsBear")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHonedAnimalAspectsBear))
                    .Build())
            .AddToDB();

        return powerHonedAnimalAspectsBear;
    }

    private static FeatureDefinitionPower PowerPathOfTheSpiritsHonedEagle()
    {
        var conditionHonedAnimalAspectsEagle = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}HonedAnimalAspectsEagle")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            // don't use vanilla RageStop with permanent conditions
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .AddToDB();

        var powerHonedAnimalAspectsEagle = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsEagle")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHonedAnimalAspectsEagle))
                    .Build())
            .AddToDB();

        return powerHonedAnimalAspectsEagle;
    }

    private static FeatureDefinitionPower PowerPathOfTheSpiritsHonedWolf()
    {
        var conditionHonedAnimalAspectsWolf = ConditionDefinitionBuilder
            .Create($"Condition{Name}HonedAnimalAspectsWolf")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            // don't use vanilla RageStop with permanent conditions
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .AddFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create(ActionAffinityMountaineerShieldCharge, $"ActionAffinity{Name}HonedAnimalAspectsWolf")
                    .AddToDB())
            .AddToDB();

        var powerHonedAnimalAspectsWolf = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HonedAnimalAspectsWolf")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHonedAnimalAspectsWolf))
                    .Build())
            .AddToDB();

        return powerHonedAnimalAspectsWolf;
    }

    // Supports integration with persistent rage 2024
    private sealed class ModifyEffectDescriptionSpiritGuardians(FeatureDefinitionPower power) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == power;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.EnableBarbarianPersistentRage2024)
            {
                return effectDescription;
            }

            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Barbarian);

            // persistent rage is only granted at 15
            if (classLevel < 15)
            {
                return effectDescription;
            }

            effectDescription.durationParameter = 10;

            return effectDescription;
        }
    }

    private sealed class MagicEffectFinishedByMeSpiritWalker : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                (characterActionUsePower.activePower.PowerDefinition != PowerBarbarianRageStart &&
                 characterActionUsePower.activePower.PowerDefinition.OverriddenPower != PowerBarbarianRageStart))
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var power = rulesetCharacter.GetRemainingPowerUses(PowerSpiritGuardians) > 0
                ? PowerSpiritGuardians
                : rulesetCharacter.GetRemainingPowerUses(PowerSpiritGuardiansRageCost) > 0
                    ? PowerSpiritGuardiansRageCost
                    : null;

            if (!power)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetCharacter);

            yield return attacker.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                [attacker],
                attacker,
                "SpiritWalker",
                Main.Settings.EnableBarbarianRelentlessRage2024
                    ? "Reaction/&UseSpiritWalkerExtendedDescription"
                    : "Reaction/&UseSpiritWalkerDescription",
                ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                if (power == PowerSpiritGuardiansRageCost)
                {
                    rulesetCharacter.SpendRagePoint();
                }
            }
        }
    }

    #region 10th LEVEL FEATURES

    // Spirit Walker

    internal static readonly ConditionDefinition ConditionSpiritGuardians = ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionSpiritGuardians, $"Condition{Name}SpiritGuardians")
        .AddToDB();

    internal static readonly ConditionDefinition ConditionSpiritGuardiansSelf = ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionSpiritGuardiansSelf, $"Condition{Name}SpiritGuardiansSelf")
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerSpiritGuardians = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}SpiritGuardians")
        .SetGuiPresentation(SpellDefinitions.SpiritGuardians.guiPresentation)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(SpellDefinitions.SpiritGuardians.EffectDescription)
                .SetDurationData(DurationType.Minute, 1)
                .SetSavingThrowData(
                    false, AttributeDefinitions.Wisdom, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerSpiritGuardiansRageCost = FeatureDefinitionPowerBuilder
        .Create(PowerSpiritGuardians, $"Power{Name}SpiritGuardiansRageCost")
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.RagePoints)
        .AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new MagicEffectFinishedByMeSpiritWalker())
        .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetPathOfTheSpiritsSpiritWalker =
        FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SpiritWalker")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(PowerSpiritGuardians, PowerSpiritGuardiansRageCost)
            .AddToDB();

    #endregion
}
