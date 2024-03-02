using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerWildMaster : AbstractSubclass
{
    private const string Name = "RangerWildMaster";
    private const string BeastCompanionTag = "BeastCompanion";
    private const string ConditionCommandBeastCompanionName = $"Condition{Name}CommandBeastCompanion";
    private const string PowerSummonBeastCompanionName = $"Power{Name}SummonBeastCompanion";

    private static readonly FeatureDefinitionActionAffinity ActionAffinityBeastCompanion =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(
                Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.AttackOpportunity,
                Id.Ready, Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
            .AddCustomSubFeatures(new CustomBehaviorBeastCompanion())
            .AddToDB();

    private static readonly FeatureDefinitionConditionAffinity ConditionAffinityWildMasterBeastCompanion =
        FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(ConditionDefinitions.ConditionSurprised)
            .AddCustomSubFeatures(ForceInitiativeToSummoner.Mark)
            .AddToDB();

    public RangerWildMaster()
    {
        // Beast Companion

        var powerWildMasterSummonBeastCompanionPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeastCompanionPool")
            .SetGuiPresentation(Category.Feature, MonsterDefinitions.KindredSpiritWolf)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var powerKindredSpiritEagle03 = BuildBeastCompanionPower(
            powerWildMasterSummonBeastCompanionPool, MonsterDefinitions.KindredSpiritEagle);

        var powerKindredSpiritBear03 = BuildBeastCompanionPower(
            powerWildMasterSummonBeastCompanionPool, MonsterDefinitions.KindredSpiritBear);

        var combatAffinityWolfTactics = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics, $"CombatAffinity{Name}WolfTactics")
            .SetGuiPresentationNoContent(true)
            .SetSituationalContext(ExtraSituationalContext.SummonerIsNextToBeast)
            .AddToDB();

        var powerKindredSpiritWolf03 = BuildBeastCompanionPower(
            powerWildMasterSummonBeastCompanionPool, MonsterDefinitions.KindredSpiritWolf, combatAffinityWolfTactics);

        PowerBundle.RegisterPowerBundle(powerWildMasterSummonBeastCompanionPool, true,
            powerKindredSpiritBear03,
            powerKindredSpiritEagle03,
            powerKindredSpiritWolf03);

        var featureSetWildMaster03 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}03")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildCommandBeastCompanion(),
                BuildBeastCompanionAffinityLevel03(),
                FeatureDefinitionHealingModifiers.HealingModifierKindredSpiritBond,
                powerWildMasterSummonBeastCompanionPool,
                powerKindredSpiritBear03,
                powerKindredSpiritEagle03,
                powerKindredSpiritWolf03)
            .AddToDB();

        var featureSetWildMaster07 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}07")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionSummoningAffinityBuilder
                    .Create(FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritMagicalSpirit,
                        $"SummoningAffinity{Name}SummonBeastCompanion07")
                    .SetRequiredMonsterTag(BeastCompanionTag)
                    .AddToDB())
            .AddToDB();

        var featureSetWildMaster11 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}11")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionSummoningAffinityBuilder
                    .Create($"SummoningAffinity{Name}SummonBeastCompanion11")
                    .SetGuiPresentationNoContent(true)
                    .SetRequiredMonsterTag(BeastCompanionTag)
                    .SetAddedConditions(
                        ConditionDefinitionBuilder
                            .Create($"Condition{Name}SummonBeastCompanionSaving")
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetFeatures(
                                FeatureDefinitionSavingThrowAffinityBuilder
                                    .Create($"SavingThrowAffinity{Name}SummonBeastCompanion")
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
                    .AddToDB())
            .AddToDB();

        var featureSetWildMaster15 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}15")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet()
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerWildMaster, 256))
            .AddFeaturesAtLevel(3, featureSetWildMaster03)
            .AddFeaturesAtLevel(7, featureSetWildMaster07)
            .AddFeaturesAtLevel(11, featureSetWildMaster11)
            .AddFeaturesAtLevel(15, featureSetWildMaster15)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPowerSharedPool BuildBeastCompanionPower(
        FeatureDefinitionPower sharedPoolPower,
        MonsterDefinition monsterDefinition,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var beastCompanion03 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "03")
                .AddFeatures(
                    CharacterContext.FeatureDefinitionPowerHelpAction,
                    ActionAffinityBeastCompanion,
                    ConditionAffinityWildMasterBeastCompanion)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetHitPointsBonus(5)
                .SetGroupAttacks(false)
                .AddToDB();

        var beastCompanion11 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "11")
                .AddFeatures(
                    CharacterContext.FeatureDefinitionPowerHelpAction,
                    ActionAffinityBeastCompanion,
                    ConditionAffinityWildMasterBeastCompanion)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetHitPointsBonus(5)
                .SetGroupAttacks(true)
                .AddToDB();

        var beastCompanion15 =
            MonsterDefinitionBuilder
                .Create(monsterDefinition, Name + monsterDefinition.name + "15")
                .AddFeatures(
                    CharacterContext.FeatureDefinitionPowerHelpAction,
                    ActionAffinityBeastCompanion,
                    ConditionAffinityWildMasterBeastCompanion)
                .AddFeatures(monsterAdditionalFeatures)
                .SetCreatureTags(BeastCompanionTag)
                .SetChallengeRating(0)
                .SetFullyControlledWhenAllied(true)
                .NoExperienceGain()
                .SetHitPointsBonus(5)
                .SetGroupAttacks(true)
                .AddToDB();

        var name = PowerSummonBeastCompanionName + monsterDefinition.name;
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
                    .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
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
            new ModifyEffectDescriptionSummonBeastCompanion(
                power, beastCompanion03, beastCompanion11, beastCompanion15));

        return power;
    }

    private static FeatureDefinitionSummoningAffinity BuildBeastCompanionAffinityLevel03()
    {
        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SummonBeastCompanionHP")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
            .AddToDB();

        var acBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SummonBeastCompanionAC")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}SummonBeastCompanionToHit")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}SummonBeastCompanionDamage")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        return FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}BeastCompanion03")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(BeastCompanionTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}SummonBeastCompanionAcBonus")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}SummonBeastCompanionSourceProficiencyBonusToHit")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toHit)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}SummonBeastCompanionProficiencyBonusToDamage")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}SummonBeastCompanionLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildCommandBeastCompanion()
    {
        var conditionCommandBeastCompanion = ConditionDefinitionBuilder
            .Create(ConditionCommandBeastCompanionName)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerCommandBeastCompanion = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CommandBeastCompanion")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCommandBeastCompanion))
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
                Gui.Battle != null &&
                c.PowersUsedByMe.Any(p => p.SourceDefinition.Name.StartsWith(PowerSummonBeastCompanionName))))
            .AddToDB();

        powerCommandBeastCompanion.AddCustomSubFeatures(
            new CharacterTurnEndListenerCommandBeastCompanion(
                conditionCommandBeastCompanion, powerCommandBeastCompanion));

        return powerCommandBeastCompanion;
    }

    private sealed class CustomBehaviorBeastCompanion : IValidateDefinitionApplication, ICharacterTurnStartListener
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
                ServiceRepository.GetService<ICommandService>()?
                    .ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
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

            return summoner.IsUnconscious ||
                   summoner.HasConditionOfType(ConditionCommandBeastCompanionName);
        }
    }

    private sealed class CharacterTurnEndListenerCommandBeastCompanion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionCommandBeastCompanion,
        FeatureDefinitionPower powerCommandBeastCompanion) : ICharacterTurnEndListener
    {
        public void OnCharacterTurnEnded(GameLocationCharacter gameLocationCharacter)
        {
            var status = gameLocationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            rulesetCharacter.LogCharacterUsedPower(powerCommandBeastCompanion);
            rulesetCharacter.InflictCondition(
                conditionCommandBeastCompanion.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionCommandBeastCompanion.Name,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyEffectDescriptionSummonBeastCompanion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerSummonBeastCompanion,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        MonsterDefinition beastCompanion03,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        MonsterDefinition beastCompanion11,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        MonsterDefinition beastCompanion15) : IModifyEffectDescription
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

            effectDescription.EffectForms[0].SummonForm.monsterDefinitionName = level switch
            {
                >= 15 => beastCompanion15.Name,
                >= 11 => beastCompanion11.Name,
                _ => beastCompanion03.Name
            };

            return effectDescription;
        }
    }
}
