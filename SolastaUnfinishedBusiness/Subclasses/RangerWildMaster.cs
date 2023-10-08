using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerWildMaster : AbstractSubclass
{
    private const string SpiritBeastTag = "SpiritBeast";
    private const string CommandSpiritBeastCondition = "ConditionWildMasterSpiritBeastCommand";
    private const string SummonSpiritBeastPower = "PowerWildMasterSummonSpiritBeast";

    public RangerWildMaster()
    {
        #region COMMON

        //
        // required for a better UI presentation on level 15
        //

        FeatureDefinitionPowers.PowerEyebiteAsleep.guiPresentation.spriteReference =
            Eyebite.guiPresentation.spriteReference;

        FeatureDefinitionPowers.PowerEyebitePanicked.guiPresentation.spriteReference =
            Eyebite.guiPresentation.spriteReference;

        FeatureDefinitionPowers.PowerEyebiteSickened.guiPresentation.spriteReference =
            Eyebite.guiPresentation.spriteReference;

        var actionAffinitySpiritBeast =
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityWildMasterSpiritBeast")
                .SetGuiPresentationNoContent()
                .SetForbiddenActions(Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                    Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
                .AddCustomSubFeatures(new SummonerHasConditionOrKOd())
                .AddToDB();

        var combatAffinityWildMasterSummonerIsNextToBeast = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics,
                "CombatAffinityWildMasterSummonerIsNextToBeast")
            .SetSituationalContext(ExtraSituationalContext.SummonerIsNextToBeast)
            .AddToDB();

        var conditionAffinityWildMasterSpiritBeastInitiative =
            FeatureDefinitionConditionAffinityBuilder
                .Create("ConditionAffinityWildMasterSpiritBeastInitiative")
                .SetGuiPresentationNoContent()
                .SetConditionAffinityType(ConditionAffinityType.Immunity)
                .SetConditionType(ConditionDefinitions.ConditionSurprised)
                .AddCustomSubFeatures(ForceInitiativeToSummoner.Mark)
                .AddToDB();

        var perceptionAffinitySpiritBeast =
            FeatureDefinitionPerceptionAffinityBuilder
                .Create("PerceptionAffinityWildMasterSpiritBeast")
                .SetGuiPresentationNoContent()
                .CannotBeSurprised()
                .AddToDB();

        var powerWildMasterSummonSpiritBeastPool03 = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool03")
            .SetGuiPresentation("PowerWildMasterSummonSpiritBeastPool", Category.Feature,
                MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var powerWildMasterSummonSpiritBeastPool07 = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool07")
            .SetGuiPresentation("PowerWildMasterSummonSpiritBeastPool", Category.Feature,
                MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetOverriddenPower(powerWildMasterSummonSpiritBeastPool03)
            .AddToDB();

        var powerWildMasterSummonSpiritBeastPool11 = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool11")
            .SetGuiPresentation("PowerWildMasterSummonSpiritBeastPool", Category.Feature,
                MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetOverriddenPower(powerWildMasterSummonSpiritBeastPool07)
            .AddToDB();

        var powerWildMasterSummonSpiritBeastPool15 = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool15")
            .SetGuiPresentation("PowerWildMasterSummonSpiritBeastPool", Category.Feature,
                MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetOverriddenPower(powerWildMasterSummonSpiritBeastPool11)
            .AddToDB();

        var powerWildMasterInvisibility = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerFunctionPotionOfInvisibility, "PowerWildMasterInvisibility")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 2)
            .AddToDB();

        #endregion

        #region EAGLE

        var powerSpiritBeastEyebiteAsleep = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebiteAsleep, "PowerSpiritBeastEyebiteAsleep")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EyebiteAsleep)
                    .Build())
            .AddToDB();

        powerSpiritBeastEyebiteAsleep.EffectDescription.difficultyClassComputation =
            EffectDifficultyClassComputation.FixedValue;
        powerSpiritBeastEyebiteAsleep.EffectDescription.fixedSavingThrowDifficultyClass = 15;

        var powerSpiritBeastBreathWeaponBlue = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponBlue, "PowerSpiritBeastBreathWeaponBlue")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        powerSpiritBeastBreathWeaponBlue.EffectDescription.EffectForms[0].diceByLevelTable = DiceByRankBuilder
            .BuildDiceByRankTable(0, 1, 4);

        var powerKindredSpiritEagle03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool03,
            MonsterDefinitions.KindredSpiritEagle, 3, false,
            powerSpiritBeastBreathWeaponBlue,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool07,
            MonsterDefinitions.KindredSpiritEagle, 7, false,
            powerSpiritBeastBreathWeaponBlue,
            FeatureDefinitionPowers.PowerFiendishResilienceLightning,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool11,
            MonsterDefinitions.KindredSpiritEagle, 11, true,
            powerSpiritBeastBreathWeaponBlue,
            FeatureDefinitionPowers.PowerFiendishResilienceLightning,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritEagle15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool15,
            MonsterDefinitions.KindredSpiritEagle, 15, true,
            powerSpiritBeastBreathWeaponBlue,
            FeatureDefinitionPowers.PowerFiendishResilienceLightning,
            powerWildMasterInvisibility,
            powerSpiritBeastEyebiteAsleep,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        #region BEAR

        var powerSpiritBeastEyebitePanicked = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebitePanicked, "PowerSpiritBeastEyebitePanicked")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EyebitePanicked)
                    .Build())
            .AddToDB();

        powerSpiritBeastEyebitePanicked.EffectDescription.difficultyClassComputation =
            EffectDifficultyClassComputation.FixedValue;
        powerSpiritBeastEyebitePanicked.EffectDescription.fixedSavingThrowDifficultyClass = 15;

        var powerSpiritBeastBreathWeaponGold = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponGold, "PowerSpiritBeastBreathWeaponGold")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        powerSpiritBeastBreathWeaponGold.EffectDescription.EffectForms[0].diceByLevelTable = DiceByRankBuilder
            .BuildDiceByRankTable(0, 1, 4);

        var powerKindredSpiritBear03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool03,
            MonsterDefinitions.KindredSpiritBear, 3, false,
            powerSpiritBeastBreathWeaponGold,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool07,
            MonsterDefinitions.KindredSpiritBear, 7, false,
            powerSpiritBeastBreathWeaponGold,
            FeatureDefinitionPowers.PowerFiendishResilienceFire,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool11,
            MonsterDefinitions.KindredSpiritBear, 11, true,
            powerSpiritBeastBreathWeaponGold,
            FeatureDefinitionPowers.PowerFiendishResilienceFire,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritBear15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool15,
            MonsterDefinitions.KindredSpiritBear, 15, true,
            powerSpiritBeastBreathWeaponGold,
            FeatureDefinitionPowers.PowerFiendishResilienceFire,
            powerWildMasterInvisibility,
            powerSpiritBeastEyebitePanicked,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        #region WOLF

        var powerSpiritBeastEyebiteSickened = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerEyebiteSickened, "PowerSpiritBeastEyebiteSickened")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EyebiteSickened)
                    .Build())
            .AddToDB();

        powerSpiritBeastEyebiteSickened.EffectDescription.difficultyClassComputation =
            EffectDifficultyClassComputation.FixedValue;
        powerSpiritBeastEyebiteSickened.EffectDescription.fixedSavingThrowDifficultyClass = 15;

        var powerSpiritBeastBreathWeaponSilver = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDragonbornBreathWeaponSilver, "PowerSpiritBeastBreathWeaponSilver")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        powerSpiritBeastBreathWeaponSilver.EffectDescription.EffectForms[0].diceByLevelTable = DiceByRankBuilder
            .BuildDiceByRankTable(0, 1, 4);

        var powerKindredSpiritWolf03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool03,
            MonsterDefinitions.KindredSpiritWolf, 3, false,
            powerSpiritBeastBreathWeaponSilver,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool07,
            MonsterDefinitions.KindredSpiritWolf, 7, false,
            powerSpiritBeastBreathWeaponSilver,
            FeatureDefinitionPowers.PowerFiendishResilienceCold,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf11 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool11,
            MonsterDefinitions.KindredSpiritWolf, 11, true,
            powerSpiritBeastBreathWeaponSilver,
            FeatureDefinitionPowers.PowerFiendishResilienceCold,
            powerWildMasterInvisibility,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            actionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        var powerKindredSpiritWolf15 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool15,
            MonsterDefinitions.KindredSpiritWolf, 15, true,
            powerSpiritBeastBreathWeaponSilver,
            FeatureDefinitionPowers.PowerFiendishResilienceCold,
            powerWildMasterInvisibility,
            powerSpiritBeastEyebiteSickened,
            CharacterContext.FeatureDefinitionPowerHelpAction,
            perceptionAffinitySpiritBeast,
            combatAffinityWildMasterSummonerIsNextToBeast,
            conditionAffinityWildMasterSpiritBeastInitiative);

        #endregion

        #region SUBCLASS

        var featureSetWildMasterBeastIsNextToSummoner = FeatureDefinitionBuilder
            .Create("FeatureWildMasterBeastIsNextToSummoner")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureSetWildMaster03 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster03")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildCommandSpiritBeast(),
                FeatureDefinitionHealingModifiers.HealingModifierKindredSpiritBond,
                BuildSpiritBeastAffinityLevel03(),
                powerWildMasterSummonSpiritBeastPool03,
                powerKindredSpiritBear03,
                powerKindredSpiritEagle03,
                powerKindredSpiritWolf03)
            .AddToDB();

        var featureSetWildMaster07 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster07")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritBeastAffinityLevel07(),
                powerWildMasterSummonSpiritBeastPool07,
                powerKindredSpiritBear07,
                powerKindredSpiritEagle07,
                powerKindredSpiritWolf07)
            .AddToDB();

        var featureSetWildMaster11 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster11")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritBeastAffinityLevel11(),
                powerWildMasterSummonSpiritBeastPool11,
                powerKindredSpiritBear11,
                powerKindredSpiritEagle11,
                powerKindredSpiritWolf11)
            .AddToDB();

        var featureSetWildMaster15 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster15")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerWildMasterSummonSpiritBeastPool15,
                powerKindredSpiritBear15,
                powerKindredSpiritEagle15,
                powerKindredSpiritWolf15)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RangerWildMaster")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RangerWildMaster", Resources.RangerWildMaster, 256))
            .AddFeaturesAtLevel(3,
                featureSetWildMaster03,
                featureSetWildMasterBeastIsNextToSummoner)
            .AddFeaturesAtLevel(7,
                featureSetWildMaster07)
            .AddFeaturesAtLevel(11,
                featureSetWildMaster11)
            .AddFeaturesAtLevel(15,
                featureSetWildMaster15)
            .AddToDB();

        #endregion

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonSpiritBeastPool03, true,
            powerKindredSpiritBear03,
            powerKindredSpiritEagle03,
            powerKindredSpiritWolf03);

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonSpiritBeastPool07, true,
            powerKindredSpiritBear07,
            powerKindredSpiritEagle07,
            powerKindredSpiritWolf07);

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonSpiritBeastPool11, true,
            powerKindredSpiritBear11,
            powerKindredSpiritEagle11,
            powerKindredSpiritWolf11);

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonSpiritBeastPool15, true,
            powerKindredSpiritBear15,
            powerKindredSpiritEagle15,
            powerKindredSpiritWolf15);

        // required to avoid beast duplicates when they get upgraded from 6 to 7, 10 to 11, 14 to 15
        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.WildMasterBeast,
            powerKindredSpiritBear03,
            powerKindredSpiritEagle03,
            powerKindredSpiritWolf03,
            powerKindredSpiritBear07,
            powerKindredSpiritEagle07,
            powerKindredSpiritWolf07,
            powerKindredSpiritBear11,
            powerKindredSpiritEagle11,
            powerKindredSpiritWolf11,
            powerKindredSpiritBear15,
            powerKindredSpiritEagle15,
            powerKindredSpiritWolf15);
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower BuildSpiritBeastPower(
        FeatureDefinitionPower sharedPoolPower,
        MonsterDefinition monsterDefinition,
        int level,
        bool groupAttacks,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var name = SummonSpiritBeastPower + monsterDefinition.name + level;
        var spiritBeast = BuildSpiritBeastMonster(monsterDefinition, level, groupAttacks, monsterAdditionalFeatures);

        var title =
            Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastTitle", spiritBeast.FormatTitle());
        var description =
            Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastDescription", spiritBeast.FormatTitle());

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(title, description, monsterDefinition, true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, spiritBeast.Name)
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalAir)
                    .Build())
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                SkipEffectRemovalOnLocationChange.Always,
                ValidatorsValidatePowerUse.NotInCombat)
            .AddToDB();
    }

    private static MonsterDefinition BuildSpiritBeastMonster(
        MonsterDefinition monsterDefinition,
        int level,
        bool groupAttacks,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return MonsterDefinitionBuilder
            .Create(monsterDefinition, "WildMasterSpiritBeast" + monsterDefinition.name + level)
            .AddFeatures(monsterAdditionalFeatures)
            .SetCreatureTags(SpiritBeastTag)
            .SetChallengeRating(0)
            .SetFullyControlledWhenAllied(true)
            .NoExperienceGain()
            .SetGroupAttacks(groupAttacks)
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel03()
    {
        var acBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierWildMasterSummonSpiritBeastAC")
            .SetGuiPresentationNoContent()
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
            .AddToDB();

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierWildMasterSummonSpiritBeastHP")
            .SetGuiPresentationNoContent()
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWildMasterSummonSpiritBeastToHit")
            .SetGuiPresentationNoContent()
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWildMasterSummonSpiritBeastDamage")
            .SetGuiPresentationNoContent()
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        return FeatureDefinitionSummoningAffinityBuilder
            .Create("SummoningAffinityWildMasterSummonSpiritBeast03")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(SpiritBeastTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastAcBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle",
                        Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastSourceProficiencyBonusToHit")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle",
                        Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(toHit)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastProficiencyBonusToDamage")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle",
                        Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastCopyCharacterLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCopyAttributeFromSummoner,
                        AttributeDefinitions.CharacterLevel)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastCopyProficiencyBonus")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCopyAttributeFromSummoner,
                        AttributeDefinitions.ProficiencyBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus) // 4 HP per level
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel07()
    {
        return FeatureDefinitionSummoningAffinityBuilder
            .Create(FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritMagicalSpirit,
                "SummoningAffinityWildMasterSummonSpiritBeast07")
            .SetRequiredMonsterTag(SpiritBeastTag)
            .AddToDB();
    }

    private static FeatureDefinition BuildSpiritBeastAffinityLevel11()
    {
        return FeatureDefinitionSummoningAffinityBuilder
            .Create("SummoningAffinityWildMasterSummonSpiritBeast11")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(SpiritBeastTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastSaving")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellAttack)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create("SavingThrowAffinityWildMasterSummonSpiritBeast")
                            .SetGuiPresentationNoContent()
                            .AddCustomSubFeatures(new AddPBToSummonCheck(1,
                                AttributeDefinitions.Strength,
                                AttributeDefinitions.Dexterity,
                                AttributeDefinitions.Constitution,
                                AttributeDefinitions.Intelligence,
                                AttributeDefinitions.Wisdom,
                                AttributeDefinitions.Charisma))
                            .AddToDB())
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition BuildCommandSpiritBeast()
    {
        var condition = ConditionDefinitionBuilder
            .Create(CommandSpiritBeastCondition)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var powerWildMasterSpiritBeastCommand = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSpiritBeastCommand")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ShowInCombatWhenHasSpiritBeast())
            .AddToDB();

        powerWildMasterSpiritBeastCommand.AddCustomSubFeatures(
            new ApplyOnTurnEnd(condition, powerWildMasterSpiritBeastCommand));

        return powerWildMasterSpiritBeastCommand;
    }

    private class SummonerHasConditionOrKOd : IValidateDefinitionApplication, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // force dodge action if not at level 7 yet
            if (locationCharacter.RulesetCharacter
                    .GetMySummoner()?.RulesetCharacter
                    .GetClassLevel(CharacterClassDefinitions.Ranger) < 7)
            {
                ServiceRepository.GetService<ICommandService>()
                    ?.ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
            }
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //Apply limits if not commanded
            return !IsCommanded(character);
        }

        private static bool IsCommanded(RulesetCharacter character)
        {
            //can act freely outside of battle
            if (Gui.Battle == null)
            {
                return true;
            }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but consider being commanded in this case
            if (summoner == null)
            {
                return true;
            }

            //can act if summoner is KO
            return summoner.IsUnconscious ||
                   //can act if summoner commanded
                   summoner.HasConditionOfType(CommandSpiritBeastCondition);
        }
    }

    private class ApplyOnTurnEnd : ICharacterTurnEndListener
    {
        private readonly ConditionDefinition _condition;
        private readonly FeatureDefinitionPower _power;

        public ApplyOnTurnEnd(ConditionDefinition condition, FeatureDefinitionPower power)
        {
            _condition = condition;
            _power = power;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter gameLocationCharacter)
        {
            var status = gameLocationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            rulesetCharacter.LogCharacterUsedPower(_power);
            rulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private class ShowInCombatWhenHasSpiritBeast : IValidatePowerUse
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return Gui.Battle != null &&
                   character.powersUsedByMe.Any(p => p.sourceDefinition.Name.StartsWith(SummonSpiritBeastPower));
        }
    }
}
