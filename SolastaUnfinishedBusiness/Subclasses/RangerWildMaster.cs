using System.Collections;
using System.Collections.Generic;
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
        // Advanced Training

        var actionAffinityAdvancedTraining = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}AdvancedTraining")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(Id.DashBonus, Id.DisengageBonus)
            .AddToDB();

        var conditionAdvancedTraining = ConditionDefinitionBuilder
            .Create($"Condition{Name}AdvancedTraining")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPassWithoutTrace)
            .SetPossessive()
            .SetFeatures(actionAffinityAdvancedTraining)
            .AddToDB();

        //
        // LEVEL 03
        //

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
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionAttackRoll")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(toHit)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionDamageRoll")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionHitPoints")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(HpBonus, HpBonus, HpBonus, HpBonus, HpBonus)
                    .AddToDB())
            .AddToDB();

        var actionAffinityBeastCompanionDashMain = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}BeastCompanionDashMain")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.DashMain)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasNoneOfConditions(conditionAdvancedTraining.Name)))
            .AddToDB();

        var actionAffinityBeastCompanionDisengageMain = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}BeastCompanionDisengageMain")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.DisengageMain)
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
            powerWildMasterSummonBeastCompanionPool,
            actionAffinityBeastCompanionDashMain, actionAffinityBeastCompanionDisengageMain,
            conditionAffinityBeastCompanion);

        var powerBeastCompanionEagle = BuildBeastCompanionEagle(
            powerWildMasterSummonBeastCompanionPool,
            actionAffinityBeastCompanionDashMain, actionAffinityBeastCompanionDisengageMain,
            conditionAffinityBeastCompanion);

        var powerBeastCompanionWolf = BuildBeastCompanionWolf(
            powerWildMasterSummonBeastCompanionPool,
            actionAffinityBeastCompanionDashMain, actionAffinityBeastCompanionDisengageMain,
            conditionAffinityBeastCompanion,
            FeatureDefinitionCombatAffinityBuilder
                .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics, $"CombatAffinity{Name}WolfTactics")
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

        //
        // LEVEL 07
        //

        // Advanced Training

        var summoningAffinityAdvancedTraining = FeatureDefinitionSummoningAffinityBuilder
            .Create(SummoningAffinityKindredSpiritMagicalSpirit, $"SummoningAffinity{Name}AdvancedTraining")
            .SetRequiredMonsterTag(BeastCompanionTag)
            .AddToDB();

        var powerAdvancedTraining = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AdvancedTraining")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerPatronTimekeeperAccelerate)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAdvancedTraining))
                    .SetCasterEffectParameters(FeatureDefinitionPowers.PowerMarksmanRecycler)
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

        //
        // LEVEL 11
        //

        // True Expertise

        var summoningAffinityTrueExpertise = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}TrueExpertise")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredMonsterTag(BeastCompanionTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}TrueExpertise")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create($"SavingThrowAffinity{Name}TrueExpertise")
                            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
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

        //
        // LEVEL 15
        //

        // Kill Command

        var conditionKillCommand = ConditionDefinitionBuilder
            .Create($"Condition{Name}KillCommand")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
            .SetConditionParticleReference(ConditionDefinitions.ConditionPainful.conditionParticleReference)
            .AddToDB();

        conditionKillCommand.AddCustomSubFeatures(new CustomBehaviorKillCommand(conditionKillCommand));

        var powerKillCommand = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}KillCommand")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionKillCommand))
                    .SetCasterEffectParameters(FeatureDefinitionPowers.PowerPactChainImp)
                    .Build())
            .AddCustomSubFeatures(
                new MagicEffectFinishedByMeKillCommand(),
                new ValidatorsValidatePowerUse(c =>
                    Gui.Battle != null &&
                    GameLocationCharacter.GetFromActor(c)?.OnceInMyTurnIsValid($"Power{Name}KillCommand") == true &&
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

    private sealed class CustomBehaviorKillCommand(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionKillCommand) : IPhysicalAttackInitiatedOnMe, IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            foreach (var enemy in Gui.Battle.GetMyContenders(target.Side)
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     x.RulesetCharacter != target))
            {
                if (enemy.RulesetCharacter.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionKillCommand.Name, out var activeCondition))
                {
                    enemy.RulesetCharacter.RemoveCondition(activeCondition);
                }
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

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

            attackMode.ToHitBonus += pb;
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

    private sealed class MagicEffectFinishedByMeKillCommand : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.UsedSpecialFeatures.TryAdd($"Power{Name}KillCommand", 1);

            yield break;
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
                .SetArmorClass(12)
                .SetAbilityScores(14, 14, 16, 10, 12, 8)
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
                .SetArmorClass(12)
                .SetAbilityScores(14, 14, 16, 10, 12, 8)
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
            .SetSilent(Silent.WhenAddedOrRemoved)
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
                .SetArmorClass(13)
                .SetAbilityScores(10, 16, 12, 14, 8, 14)
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
                .SetArmorClass(13)
                .SetAbilityScores(10, 16, 12, 14, 8, 14)
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
                .SetArmorClass(13)
                .SetAbilityScores(12, 16, 14, 14, 8, 10)
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
                .SetArmorClass(13)
                .SetAbilityScores(12, 16, 14, 14, 8, 10)
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
