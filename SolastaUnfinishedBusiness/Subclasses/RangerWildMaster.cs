using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerWildMaster : AbstractSubclass
{
    private const string SpiritBeastTag = "SpiritBeast";
    private const string CommandSpiritBeastCondition = "ConditionWildMasterSpiritBeastCommand";
    private const string SummonSpiritBeastPower = "PowerWildMasterSummonSpiritBeast";


    internal RangerWildMaster()
    {
        var actionAffinitySpiritBeast03 =
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinitySpiritBeast03")
                .SetGuiPresentationNoContent()
                .SetDefaultAllowedActionTypes()
                .SetForbiddenActions(Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.AttackOpportunity,
                    Id.DisengageBonus,
                    Id.DisengageMain, Id.Ready, Id.Shove, Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
                .SetCustomSubFeatures(new SummonerHasConditionOrKOd())
                .AddToDB();

        var actionAffinitySpiritBeast07 =
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinitySpiritBeast07")
                .SetGuiPresentationNoContent()
                .SetDefaultAllowedActionTypes()
                .SetForbiddenActions(Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                    Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
                .SetCustomSubFeatures(new SummonerHasConditionOrKOd())
                .AddToDB();

        var conditionAffinityWildMasterSpiritBeastInitiative =
            FeatureDefinitionConditionAffinityBuilder
                .Create("ConditionAffinityWildMasterSpiritBeastInitiative")
                .SetGuiPresentationNoContent()
                .SetConditionAffinityType(ConditionAffinityType.Immunity)
                .SetConditionType(ConditionDefinitions.ConditionSurprised)
                .SetCustomSubFeatures(ForceInitiativeToSummoner.Mark)
                .AddToDB();

        var perceptionAffinitySpiritBeast =
            FeatureDefinitionPerceptionAffinityBuilder
                .Create("PerceptionAffinitySpiritBeast")
                .SetGuiPresentationNoContent()
                .CannotBeSurprised()
                .AddToDB();

        var powerWildMasterSummonSpiritBeastPool = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSummonSpiritBeastPool")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var powerWildMasterSpiritBeastMender = FeatureDefinitionPowerBuilder
            .Create("PowerWildMasterSpiritBeastMender")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 3)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetHealingForm(HealingComputation.Dice, 4, DieType.D8, 2, false,
                        HealingCap.MaximumHitPoints)
                    .Build())
                .Build())
            .AddToDB();

        var powerKindredSpiritBear03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritBear, 3,
            actionAffinitySpiritBeast03,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var powerKindredSpiritEagle03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritEagle, 3,
            actionAffinitySpiritBeast03,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var powerKindredSpiritTiger03 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            null, MonsterDefinitions.KindredSpiritTiger, 3,
            actionAffinitySpiritBeast03,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var powerKindredSpiritBear07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritBear03, MonsterDefinitions.KindredSpiritBear, 7,
            actionAffinitySpiritBeast07,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var powerKindredSpiritEagle07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritEagle03, MonsterDefinitions.KindredSpiritEagle, 7,
            actionAffinitySpiritBeast07,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var powerKindredSpiritTiger07 = BuildSpiritBeastPower(powerWildMasterSummonSpiritBeastPool,
            powerKindredSpiritTiger03, MonsterDefinitions.KindredSpiritTiger, 7,
            actionAffinitySpiritBeast07,
            conditionAffinityWildMasterSpiritBeastInitiative,
            perceptionAffinitySpiritBeast);

        var featureSetWildMaster03 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster03")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildCommandSpiritBeast(),
                BuildSpiritBeastAffinityLevel03(),
                powerWildMasterSummonSpiritBeastPool,
                powerWildMasterSpiritBeastMender,
                powerKindredSpiritBear03,
                powerKindredSpiritEagle03,
                powerKindredSpiritTiger03)
            .AddToDB();

        var featureSetWildMaster07 = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWildMaster07")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSpiritBeastAffinityLevel07(),
                powerKindredSpiritBear07,
                powerKindredSpiritEagle07,
                powerKindredSpiritTiger07)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RangerWildMaster")
            .SetGuiPresentation(Category.Subclass, PatronFiend)
            .AddFeaturesAtLevel(3, featureSetWildMaster03)
            .AddFeaturesAtLevel(7, featureSetWildMaster07)
            .AddFeaturesAtLevel(11)
            .AddFeaturesAtLevel(15)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    private static RulesetCharacter GetSpiritBeast(RulesetCharacter character)
    {
        var bladeEffect = character.powersUsedByMe.Find(p => p.sourceDefinition.Name.Contains(SummonSpiritBeastPower));
        var summons = EffectHelpers.GetSummonedCreatures(bladeEffect);

        return summons.Empty() ? null : summons[0];
    }

    private static FeatureDefinitionPower BuildSpiritBeastPower(
        FeatureDefinitionPower featureDefinitionPower,
        FeatureDefinitionPower featureDefinitionPowerToReplace,
        MonsterDefinition monsterDefinition,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var spiritBeastMonster = BuildSpiritBeastMonster(monsterDefinition, level, monsterAdditionalFeatures);
        var name = SummonSpiritBeastPower + monsterDefinition.name + level;

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetGuiPresentation(
                Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastTitle",
                    spiritBeastMonster.FormatTitle()),
                Gui.Format("Feature/&PowerWildMasterSummonSpiritBeastDescription",
                    spiritBeastMonster.FormatDescription()),
                monsterDefinition)
            .SetSharedPool(ActivationTime.Action, featureDefinitionPower)
            .SetOverriddenPower(featureDefinitionPowerToReplace)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetSummonCreatureForm(1, spiritBeastMonster.Name)
                    .Build())
                .SetParticleEffectParameters(ConjureElementalAir)
                .Build())
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always, ValidatorsPowerUse.NotInCombat)
            .AddToDB();
    }

    private static MonsterDefinition BuildSpiritBeastMonster(
        MonsterDefinition monsterDefinition,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return MonsterDefinitionBuilder
            .Create(monsterDefinition, "WildMasterSpiritBeast" + monsterDefinition.name + level)
            .HideFromDungeonEditor()
            .AddFeatures(monsterAdditionalFeatures)
            .SetCreatureTags(SpiritBeastTag)
            .SetFullyControlledWhenAllied(true)
            .NoExperienceGain()
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
                //Bonuses from Ranger's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastAcBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                //Bonuses from Ranger's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastToHitBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(toHit)
                    .AddToDB(),
                //Bonuses from Ranger's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastDamageBonus")
                    .SetGuiPresentation("Condition/&ConditionWildMasterSummonSpiritBeastBonusTitle", Gui.NoLocalization)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                //Bonuses from Ranger's level
                ConditionDefinitionBuilder
                    .Create("ConditionWildMasterSummonSpiritBeastLevel")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel)
                    //Set damage type to class name so `ExtraOriginOfAmount.SourceClassLevel` would know what class to use
                    .SetAdditionalDamageWhenHit(damageType: RangerClass, active: false)
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

    private static FeatureDefinition BuildCommandSpiritBeast()
    {
        var condition = ConditionDefinitionBuilder
            .Create(CommandSpiritBeastCondition)
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Round, 1)
            .SetSpecialDuration()
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create("PowerRangerWildBeastSpiritBeastCommand")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetCustomSubFeatures(new ShowInCombatWhenHasSpiritBeast())
            .AddToDB();

        power.AddCustomSubFeatures(new ApplyOnTurnEnd(condition, power));

        return power;
    }

    private class SummonerHasConditionOrKOd : IDefinitionApplicationValidator, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // force dodge action if not at level 7 yet
            if (locationCharacter.RulesetCharacter.GetMySummoner()?.RulesetCharacter is RulesetCharacterHero hero
                && hero.ClassesAndLevels[CharacterClassDefinitions.Ranger] < 7)
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
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower power;

        public ApplyOnTurnEnd(ConditionDefinition condition, FeatureDefinitionPower power)
        {
            this.condition = condition;
            this.power = power;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var status = locationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available)
            {
                return;
            }

            var character = locationCharacter.RulesetCharacter;
            var newCondition = RulesetCondition.CreateActiveCondition(character.Guid, condition, DurationType.Round, 1,
                TurnOccurenceType.StartOfTurn, locationCharacter.Guid, character.CurrentFaction.Name);

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, newCondition);
            GameConsoleHelper.LogCharacterUsedPower(character, power);
        }
    }

    private class ShowInCombatWhenHasSpiritBeast : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress &&
                   character.powersUsedByMe.Any(p => p.sourceDefinition.Name.StartsWith(SummonSpiritBeastPower));
        }
    }

    private class TargetSpiritBeast : IRetargetCustomRestPower
    {
        public GameLocationCharacter GetTarget(RulesetCharacter summoner)
        {
            var spiritBeast = GetSpiritBeast(summoner);

            return spiritBeast == null ? null : GameLocationCharacter.GetFromActor(spiritBeast);
        }
    }
}
