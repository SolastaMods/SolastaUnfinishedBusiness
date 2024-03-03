using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerWildMaster : AbstractSubclass
{
    private const string Name = "RangerWildMaster";
    private const string BeastCompanionTag = "BeastCompanion";
    private const string PowerSummonBeastCompanionPrefix = $"Power{Name}SummonBeastCompanion";

    private static readonly FeatureDefinitionAttributeModifier HpBonus = FeatureDefinitionAttributeModifierBuilder
        .Create($"AttributeModifier{Name}HitPoints")
        .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
        .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
        .AddToDB();

    public RangerWildMaster()
    {
        var conditionAdvancedTraining = ConditionDefinitionBuilder
            .Create($"Condition{Name}AdvancedTraining")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPassWithoutTrace)
            .SetPossessive()
            .AddToDB();

        // Beast Companion

        var acBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ArmorClass")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}AttackRoll")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}DamageRoll")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var summoningAffinityBeastCompanion = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(BeastCompanionTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionArmorClass")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionArmorClassAttackRoll")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toHit)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionDamageRoll")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionHitPoints")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(HpBonus, HpBonus, HpBonus, HpBonus, HpBonus)
                    .AddToDB())
            .AddToDB();

        var actionAffinityBeastCompanion = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.DashMain, Id.DashBonus, Id.DisengageMain, Id.DisengageBonus)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasNoneOfConditions(conditionAdvancedTraining.Name)))
            .AddToDB();

        var conditionAffinityBeastCompanion = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(ConditionDefinitions.ConditionSurprised)
            .AddCustomSubFeatures(ForceInitiativeToSummoner.Mark)
            .AddToDB();

        var powerWildMasterSummonBeastCompanionPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeastCompanionPool")
            .SetGuiPresentation(Category.Feature, MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var powerBeastCompanionBear = BuildBeastCompanionBear(
            powerWildMasterSummonBeastCompanionPool, actionAffinityBeastCompanion, conditionAffinityBeastCompanion);

        var powerBeastCompanionEagle = BuildBeastCompanionEagle(
            powerWildMasterSummonBeastCompanionPool, actionAffinityBeastCompanion, conditionAffinityBeastCompanion);

        var powerBeastCompanionWolf = BuildBeastCompanionWolf(
            powerWildMasterSummonBeastCompanionPool, actionAffinityBeastCompanion, conditionAffinityBeastCompanion,
            FeatureDefinitionCombatAffinityBuilder
                .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics, $"CombatAffinity{Name}WolfTactics")
                .SetGuiPresentationNoContent(true)
                .SetSituationalContext(ExtraSituationalContext.SummonerIsNextToBeast)
                .AddToDB());

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonBeastCompanionPool, true,
            powerBeastCompanionBear, powerBeastCompanionEagle, powerBeastCompanionWolf);

        var featureSetWildMasterBeastCompanion = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BeastCompanion")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionHealingModifiers.HealingModifierKindredSpiritBond,
                summoningAffinityBeastCompanion,
                powerWildMasterSummonBeastCompanionPool,
                powerBeastCompanionBear,
                powerBeastCompanionEagle,
                powerBeastCompanionWolf)
            .AddToDB();

        // Advanced Training

        var summoningAffinityAdvancedTraining = FeatureDefinitionSummoningAffinityBuilder
            .Create(SummoningAffinityKindredSpiritMagicalSpirit, $"SummoningAffinity{Name}AdvancedTraining")
            .SetRequiredMonsterTag(BeastCompanionTag)
            .AddToDB();

        var powerAdvancedTraining = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AdvancedTraining")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerPatronTimekeeperAccelerate)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAdvancedTraining))
                    .Build())
            .AddCustomSubFeatures(
                new FilterTargetingCharacterAdvancedTraining(),
                new ValidatorsValidatePowerUse(c =>
                    Gui.Battle != null &&
                    c.PowersUsedByMe.Any(x => x.Name.StartsWith(PowerSummonBeastCompanionPrefix))))
            .AddToDB();

        var featureSetWildMasterAdvancedTraining = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AdvancedTraining")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(summoningAffinityAdvancedTraining, powerAdvancedTraining)
            .AddToDB();

        // True Expertise

        var summoningAffinityTrueExpertise = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}TrueExpertise")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredMonsterTag(BeastCompanionTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}TrueExpertise")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create($"SavingThrowAffinity{Name}TrueExpertise")
                            .SetGuiPresentationNoContent(true)
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

        // Kill Command

        var conditionKillCommand = ConditionDefinitionBuilder
            .Create($"Condition{Name}KillCommand")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        conditionKillCommand.AddCustomSubFeatures(new PhysicalAttackInitiatedOnMeKillCommand(conditionKillCommand));

        var powerKillCommand = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}KillCommand")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionKillCommand))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(c =>
                    Gui.Battle != null &&
                    c.PowersUsedByMe.Any(x => x.Name.StartsWith(PowerSummonBeastCompanionPrefix))))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerWildMaster, 256))
            .AddFeaturesAtLevel(3, featureSetWildMasterBeastCompanion)
            .AddFeaturesAtLevel(7, featureSetWildMasterAdvancedTraining)
            .AddFeaturesAtLevel(11, summoningAffinityTrueExpertise)
            .AddFeaturesAtLevel(15, powerKillCommand)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Beast Companion
    //

    private sealed class ModifyEffectDescriptionSummonBeastCompanion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerSummonBeastCompanion,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        MonsterDefinition beastCompanion03,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        MonsterDefinition beastCompanion11) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerSummonBeastCompanion;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var level = character.GetClassLevel(CharacterClassDefinitions.Ranger);
            var summonForm =
                effectDescription.EffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Summon);

            if (summonForm == null)
            {
                return effectDescription;
            }

            summonForm.SummonForm.monsterDefinitionName = level switch
            {
                >= 11 => beastCompanion11.Name,
                _ => beastCompanion03.Name
            };

            return effectDescription;
        }
    }

    //
    // Beast Companion Bear
    //

    private sealed class MagicEffectInitiatedByMeBeastCompanionBear(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionBearHitPoints) : IMagicEffectInitiatedByMe
    {
        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionBearHitPoints.Name,
                DurationType.UntilLongRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionBearHitPoints.Name,
                0,
                0,
                0);

            yield break;
        }
    }

    //
    // Advanced Training
    //

    private sealed class FilterTargetingCharacterAdvancedTraining : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var isValid = target.RulesetCharacter.GetMySummoner()?.Guid == __instance.ActionParams.ActingCharacter.Guid;

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeBeastCompanion");
            }

            return isValid;
        }
    }

    //
    // Kill Command
    //

    private sealed class PhysicalAttackInitiatedOnMeKillCommand(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionKillCommand) : IPhysicalAttackInitiatedOnMe
    {
        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var summoner = rulesetAttacker.GetMySummoner();

            if (summoner == null)
            {
                yield break;
            }

            var pb = summoner.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var rulesetDefender = defender.RulesetCharacter;

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionKillCommand.Name, out var activeCondition) ||
                activeCondition.SourceGuid != summoner.Guid)
            {
                yield break;
            }

            attackMode.ToHitBonus = pb;
            attackMode.ToHitBonusTrends.Add(
                new TrendInfo(pb, FeatureSourceType.Condition, conditionKillCommand.Name, conditionKillCommand));

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                yield break;
            }

            damage.BonusDamage += pb;
            damage.DamageBonusTrends.Add(
                new TrendInfo(pb, FeatureSourceType.Condition, conditionKillCommand.Name, conditionKillCommand));
        }
    }

    # region Beast Companion

    private static FeatureDefinitionPowerSharedPool BuildBeastCompanionBear(
        FeatureDefinitionPower sharedPoolPower, params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var monsterDefinition = MonsterDefinitions.KindredSpiritBear;
        var beastCompanion03 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "03")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(false)
                .AddToDB();

        var beastCompanion11 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "11")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(true)
                .AddToDB();

        var summoningAffinityBearHitPoints = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}BearHitPoints")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(BeastCompanionTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionBearHitPoints")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(HpBonus, HpBonus, HpBonus)
                    .AddToDB())
            .AddToDB();

        var conditionBearHitPoints = ConditionDefinitionBuilder
            .Create($"Condition{Name}BearHitPoints")
            .SetGuiPresentationNoContent(true)
            //.SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(summoningAffinityBearHitPoints)
            .AddToDB();

        var name = PowerSummonBeastCompanionPrefix + monsterDefinition.name;
        var title =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionTitle", beastCompanion03.FormatTitle());
        var description =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionDescription", beastCompanion03.FormatTitle());

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(title, description, monsterDefinition, true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 1, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, beastCompanion03.Name)
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalAir)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            SkipEffectRemovalOnLocationChange.Always,
            ValidatorsValidatePowerUse.NotInCombat,
            new ModifyEffectDescriptionSummonBeastCompanion(power, beastCompanion03, beastCompanion11),
            new MagicEffectInitiatedByMeBeastCompanionBear(conditionBearHitPoints));

        return power;
    }

    private static FeatureDefinitionPowerSharedPool BuildBeastCompanionEagle(
        FeatureDefinitionPower sharedPoolPower, params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var monsterDefinition = MonsterDefinitions.KindredSpiritEagle;
        var beastCompanion03 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "03")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(false)
                .AddToDB();

        var beastCompanion11 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "11")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(true)
                .AddToDB();

        var name = PowerSummonBeastCompanionPrefix + monsterDefinition.name;
        var title =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionTitle", beastCompanion03.FormatTitle());
        var description =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionDescription", beastCompanion03.FormatTitle());

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(title, description, monsterDefinition, true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 1, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, beastCompanion03.Name)
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalAir)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        power.AddCustomSubFeatures(
            SkipEffectRemovalOnLocationChange.Always,
            ValidatorsValidatePowerUse.NotInCombat,
            new ModifyEffectDescriptionSummonBeastCompanion(power, beastCompanion03, beastCompanion11));

        return power;
    }

    private static FeatureDefinitionPowerSharedPool BuildBeastCompanionWolf(
        FeatureDefinitionPower sharedPoolPower, params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var monsterDefinition = MonsterDefinitions.KindredSpiritWolf;
        var beastCompanion03 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "03")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(false)
                .AddToDB();

        var beastCompanion11 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "11")
                .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetStandardHitPoints(5)
                .SetGroupAttacks(true)
                .AddToDB();

        // give wolf a second bite attack
        beastCompanion11.AttackIterations.Add(beastCompanion11.AttackIterations[0]);

        var name = PowerSummonBeastCompanionPrefix + monsterDefinition.name;
        var title =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionTitle", beastCompanion03.FormatTitle());
        var description =
            Gui.Format($"Feature/&Power{Name}SummonBeastCompanionDescription", beastCompanion03.FormatTitle());

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(title, description, monsterDefinition, true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 1, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, beastCompanion03.Name)
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalAir)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        power.AddCustomSubFeatures(
            SkipEffectRemovalOnLocationChange.Always,
            ValidatorsValidatePowerUse.NotInCombat,
            new ModifyEffectDescriptionSummonBeastCompanion(power, beastCompanion03, beastCompanion11));

        return power;
    }

    #endregion
}
