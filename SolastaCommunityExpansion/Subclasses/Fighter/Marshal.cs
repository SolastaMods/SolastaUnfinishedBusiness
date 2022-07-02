using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterAttackDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Fighter;

internal sealed class Marshal : AbstractSubclass
{
    private CharacterSubclassDefinition Subclass;

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass ??= MarshalFighterSubclassBuilder.BuildAndAddSubclass();
    }
}

internal static class KnowYourEnemyBuilder
{
    private static int GetKnowledgeLevelOfEnemy(RulesetCharacter enemy)
    {
        return ServiceRepository.GetService<IGameLoreService>().Bestiary.TryGetBestiaryEntry(enemy, out var entry)
            ? entry.KnowledgeLevelDefinition.Level
            : 0;
    }

    private static void KnowYourEnemyOnAttackDelegate(GameLocationCharacter attacker,
        GameLocationCharacter defender,
        [CanBeNull] ActionModifier attackModifier,
        [CanBeNull] RulesetAttackMode attackerAttackMode)
    {
        // no spell attack
        if (attackerAttackMode == null || attackModifier == null)
        {
            return;
        }

        var knowledgeLevelOfEnemy = GetKnowledgeLevelOfEnemy(defender.RulesetCharacter);
        attackerAttackMode.toHitBonus += knowledgeLevelOfEnemy;
        attackModifier.AttacktoHitTrends.Add(new TrendInfo(knowledgeLevelOfEnemy,
            FeatureSourceType.CharacterFeature, "KnowYourEnemy", null));
    }

    internal static FeatureDefinitionFeatureSet BuildKnowYourEnemyFeatureSet()
    {
        var knowYourEnemiesAttackHitModifier = FeatureDefinitionOnAttackEffectBuilder
            .Create("KnowYourEnemyAttackHitModifier", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("Subclass/&FighterMarshalKnowYourEnemyFeatureSetTitle",
                "Subclass/&FighterMarshalKnowYourEnemyFeatureSetDescription")
            .SetOnAttackDelegates(KnowYourEnemyOnAttackDelegate, null)
            .AddToDB();

        var additionalDamageRangerFavoredEnemyHumanoid = FeatureDefinitionAdditionalDamageBuilder
            .Create("MarshalAdditionalDamageFavoredEnemyHumanoid",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetNotificationTag("FavoredEnemy")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpecificCharacterFamily)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .AddToDB();

        additionalDamageRangerFavoredEnemyHumanoid.requiredCharacterFamily = CharacterFamilyDefinitions.Humanoid;

        return FeatureDefinitionFeatureSetBuilder
            .Create("KnowYourEnemy", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshalKnowYourEnemyFeatureSet", Category.Subclass)
            .AddFeatureSet(
                knowYourEnemiesAttackHitModifier,
                AdditionalDamageRangerFavoredEnemyAberration,
                AdditionalDamageRangerFavoredEnemyBeast,
                AdditionalDamageRangerFavoredEnemyCelestial,
                AdditionalDamageRangerFavoredEnemyConstruct,
                AdditionalDamageRangerFavoredEnemyDragon,
                AdditionalDamageRangerFavoredEnemyElemental,
                AdditionalDamageRangerFavoredEnemyFey,
                AdditionalDamageRangerFavoredEnemyFiend,
                AdditionalDamageRangerFavoredEnemyGiant,
                AdditionalDamageRangerFavoredEnemyMonstrosity,
                AdditionalDamageRangerFavoredEnemyOoze,
                AdditionalDamageRangerFavoredEnemyPlant,
                AdditionalDamageRangerFavoredEnemyUndead,
                additionalDamageRangerFavoredEnemyHumanoid
            )
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }
}

internal static class StudyYourEnemyBuilder
{
    public static FeatureDefinitionPower BuildStudyEnemyPower()
    {
        var effectDescription = IdentifyCreatures.EffectDescription
            .Copy()
            .SetDuration(DurationType.Instantaneous)
            .SetHasSavingThrow(false)
            .SetRange(RangeType.Distance, 12)
            .SetTargetType(TargetType.Individuals)
            .SetTargetSide(Side.Enemy)
            .SetTargetParameter(1)
            .ClearRestrictedCreatureFamilies()
            .SetEffectForms(new StudyEnemyEffectDescription());

        return FeatureDefinitionPowerBuilder
            .Create("StudyYourEnemy", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshalStudyYourEnemyPower", Category.Subclass,
                IdentifyCreatures.GuiPresentation.SpriteReference)
            .SetFixedUsesPerRecharge(2)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetActivation(ActivationTime.BonusAction, 0)
            .SetEffectDescription(effectDescription)
            .AddToDB();
    }

    private sealed class StudyEnemyEffectDescription : CustomEffectForm
    {
        public override void ApplyForm(
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            EffectApplication effectApplication = EffectApplication.All,
            [CanBeNull] List<EffectFormFilter> filters = null)
        {
            var manager = ServiceRepository.GetService<IGameLoreService>();

            var gameLocationCharacter = GameLocationCharacter.GetFromActor(formsParams.targetCharacter);

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster creature)
            {
                return;
            }

            if (!manager.Bestiary.TryGetBestiaryEntry(creature, out var entry)
                && (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Full
                    || (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Reference &&
                        creature.MonsterDefinition.BestiaryReference.BestiaryEntry ==
                        BestiaryDefinitions.BestiaryEntry.Full)))
            {
                entry = manager.Bestiary.AddNewMonsterEntry(creature.MonsterDefinition);
                manager.MonsterKnowledgeChanged?.Invoke(creature.MonsterDefinition, entry.KnowledgeLevelDefinition);
            }

            var checkModifier = new ActionModifier();
            var roller = GameLocationCharacter.GetFromActor(formsParams.sourceCharacter);

            roller.RollAbilityCheck(AttributeDefinitions.Wisdom, SkillDefinitions.Survival,
                10 + Mathf.FloorToInt(entry.MonsterDefinition.ChallengeRating), AdvantageType.None, checkModifier,
                false, -1, out var outcome, true);

            var level = entry.KnowledgeLevelDefinition.Level;
            var num = level;

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                var num2 = outcome == RollOutcome.Success ? 1 : 2;
                num = Mathf.Min(entry.KnowledgeLevelDefinition.Level + num2, 4);
                manager.LearnMonsterKnowledge(entry.MonsterDefinition, manager.Bestiary.SortedKnowledgeLevels[num]);
            }

            gameLocationCharacter.RulesetCharacter.MonsterIdentificationRolled?.Invoke(
                gameLocationCharacter.RulesetCharacter, entry.MonsterDefinition, outcome, level, num);
        }

        public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
        }
    }
}

internal static class CoordinatedAttackBuilder
{
    private static IEnumerator CoordinatedAttackOnAttackHitDelegate(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RollOutcome outcome,
        CharacterActionParams actionParams,
        [NotNull] RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        // melee only
        if (attackMode.ranged || outcome == RollOutcome.CriticalFailure || outcome == RollOutcome.Failure ||
            actionParams.actionDefinition.Id == ActionDefinitions.Id.AttackOpportunity)
        {
            yield break;
        }

        var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var battleManager = gameLocationBattleService as GameLocationBattleManager;

        var allies = new List<GameLocationCharacter>();
        foreach (var guestCharacter in characterService.GuestCharacters)
        {
            if (guestCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster
                || !rulesetCharacterMonster.MonsterDefinition.CreatureTags.Contains("MarshalEternalComrade"))
            {
                continue;
            }

            if (!rulesetCharacterMonster.TryGetConditionOfCategoryAndType("17TagConjure", "ConditionConjuredCreature",
                    out var activeCondition)
                || activeCondition.SourceGuid != attacker.Guid)
            {
                continue;
            }

            var gameLocationMonster = GameLocationCharacter.GetFromActor(rulesetCharacterMonster);

            allies.Add(gameLocationMonster);
        }

        foreach (var partyCharacter in characterService.PartyCharacters)
        {
            if (partyCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) != 0)
            {
                continue;
            }

            if (partyCharacter == attacker)
            {
                continue;
            }

            allies.Add(partyCharacter);
        }

        var reactions = new List<CharacterActionParams>();
        foreach (var partyCharacter in allies)
        {
            var allAttackMode = partyCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            var attackParams = default(BattleDefinitions.AttackEvaluationParams);
            var actionModifierBefore = new ActionModifier();
            var canAttack = true;
            if (allAttackMode.Ranged)
            {
                attackParams.FillForPhysicalRangeAttack(partyCharacter, partyCharacter.LocationPosition, allAttackMode,
                    defender, defender.LocationPosition, actionModifierBefore);
            }
            else
            {
                attackParams.FillForPhysicalReachAttack(partyCharacter, partyCharacter.LocationPosition, allAttackMode,
                    defender, defender.LocationPosition, actionModifierBefore);
            }

            if (!gameLocationBattleService.CanAttack(attackParams))
            {
                canAttack = false;
                var cantrips = ReactionRequestWarcaster.GetValidCantrips(battleManager, partyCharacter, defender);
                if (cantrips == null || cantrips.Empty())
                {
                    continue;
                }
            }

            var reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.AttackOpportunity,
                allAttackMode, defender, actionModifierBefore)
            {
                StringParameter2 = "CoordinatedAttack", BoolParameter4 = !canAttack
            };
            reactions.Add(reactionParams);
        }

        if (reactions.Empty() || battleManager == null)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        foreach (var reaction in reactions)
        {
            actionService.ReactForOpportunityAttack(reaction);
        }

        yield return battleManager.WaitForReactions(attacker, actionService, count);
    }

    internal static FeatureDefinition BuildCoordinatedAttack()
    {
        return FeatureDefinitionBuilder
            .Create("CoordinatedAttack", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshalCoordinatedAttack", Category.Subclass)
            .SetCustomSubFeatures(new ReactToAttackFinished(CoordinatedAttackOnAttackHitDelegate))
            .AddToDB();
    }
}

internal static class EternalComradeBuilder
{
    private static readonly MonsterDefinition EternalComrade = BuildEternalComradeMonster();

    private static MonsterDefinition BuildEternalComradeMonster()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectForms(
                new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Damage,
                    DamageForm = new DamageForm
                    {
                        BonusDamage = 2, DiceNumber = 2, DieType = DieType.D6, DamageType = DamageTypeSlashing
                    }
                }
            )
            .Build();
        effectDescription.SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Count);

        var eternalComradeAttack = MonsterAttackDefinitionBuilder
            .Create(Attack_Generic_Guard_Longsword, "AttackEternalComrade",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        eternalComradeAttack.magical = true;

        var marshalEternalComradeAttackInteraction = new MonsterAttackIteration(eternalComradeAttack, 1);

        return MonsterDefinitionBuilder
            .Create(SuperEgo_Servant_Hostile, "EternalComrade",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("MarshalEternalComrade", Category.Subclass,
                SuperEgo_Servant_Hostile.GuiPresentation.SpriteReference)
            .ClearFeatures()
            .SetFeatures(
                SenseNormalVision,
                SenseDarkvision24,
                MoveModeMove10,
                MoveModeFly10,
                ActionAffinityFightingStyleProtection,
                ConditionAffinityCharmImmunity,
                ConditionAffinityExhaustionImmunity,
                ConditionAffinityFrightenedImmunity,
                ConditionAffinityGrappledImmunity,
                ConditionAffinityParalyzedmmunity,
                ConditionAffinityPetrifiedImmunity,
                ConditionAffinityPoisonImmunity,
                ConditionAffinityProneImmunity,
                ConditionAffinityRestrainedmmunity,
                DamageAffinityColdImmunity,
                DamageAffinityNecroticImmunity,
                DamageAffinityFireResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityFireResistance,
                DamageAffinityAcidResistance,
                DamageAffinityLightningResistance,
                DamageAffinityThunderResistance,
                ConditionAffinityHinderedByFrostImmunity
            )
            .SetAttackIterations(marshalEternalComradeAttackInteraction)
            .SetArmorClass(16)
            .SetAlignment(AlignmentDefinitions.Neutral.Name)
            .SetCharacterFamily(CharacterFamilyDefinitions.Undead.name)
            .SetCreatureTags("MarshalEternalComrade")
            .SetDefaultBattleDecisionPackage(DefaultMeleeWithBackupRangeDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddToDB();
    }

    internal static FeatureDefinitionFeatureSet BuildEternalComradeFeatureSet()
    {
        var summonForm = new SummonForm {monsterDefinitionName = EternalComrade.Name};

        var effectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Summon, createdByCharacter = true, summonForm = summonForm
        };

        // TODO: make this use concentration and reduce the duration to may be 3 rounds
        // TODO: increase the number of use to 2 and recharge per long rest
        var summonEternalComradePower = FeatureDefinitionPowerBuilder
            .Create("SummonEternalComrade", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshalSummonEternalComradePower", Category.Subclass,
                Bane.GuiPresentation.SpriteReference)
            .SetCostPerUse(1)
            .SetUsesFixed(1)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ConjureAnimalsOneBeast.EffectDescription.Copy())
                    .SetDurationData(DurationType.Round, 10)
                    .ClearEffectForms()
                    .AddEffectForm(effectForm)
                    .Build()
            )
            .AddToDB();
        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, summonEternalComradePower);

        var acConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondAC, "ConditionMarshalEternarlComradeAC",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var stConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondSavingThrows, "ConditionMarshalEternarlComradeST",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var damageConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionMarshalEternarlComradeDamage",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var hitConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionMarshalEternarlComradeHit",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var hpConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondHP, "ConditionMarshalEternarlComradeHP",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
            .SetAllowMultipleInstances(true)
            .AddToDB();

        // Find a better place to put this in?
        hpConditionDefinition.additionalDamageType = "Fighter";

        var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
            .Create(SummoningAffinityKindredSpiritBond, "SummoningAffinityMarshalEternalComrade",
                MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .ClearEffectForms()
            .SetRequiredMonsterTag("MarshalEternalComrade")
            .SetAddedConditions(
                acConditionDefinition, stConditionDefinition, damageConditionDefinition,
                hitConditionDefinition, hpConditionDefinition, hpConditionDefinition)
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("EternalComradeFeatureSet", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshalEternalComradeFeatureSet", Category.Subclass)
            .SetFeatureSet(summoningAffinity, summonEternalComradePower)
            .AddToDB();
    }
}

internal static class FearlessCommanderBuilder
{
    internal static FeatureDefinitionFeatureSet BuildFearlessCommander()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FearlessCommander", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetFeatureSet(ConditionAffinityFrightenedImmunity)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetGuiPresentation("FighterMarshalFearlessCommanderFeatureSet", Category.Subclass)
            .AddToDB();
    }
}

internal static class EncourageBuilder
{
    internal static FeatureDefinitionPower BuildEncourage()
    {
        var conditionEncouraged = ConditionDefinitionBuilder
            .Create("ConditionEncouraged", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .SetGuiPresentation(
                "Subclass/&FighterMarshalEncouragementPowerTitle",
                "Subclass/&FighterMarshalEncouragementPowerDescription",
                ConditionBlessed.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionCombatAffinitys.CombatAffinityBlessed,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBlessed)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        conditionEncouraged.ConditionTags.Add("Buff");

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionEncouraged);

        var effect = EffectDescriptionBuilder
            .Create()
            .SetCreatedByCharacter()
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 5, 2)
            .SetDurationData(DurationType.Permanent)
            .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .CreatedByCharacter()
                    .SetConditionForm(conditionEncouraged, ConditionForm.ConditionOperation.Add, false, false)
                    .Build()
            ).Build();

        effect.SetCanBePlacedOnCharacter(true);

        return FeatureDefinitionPowerBuilder
            .Create("Encouragement", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
            .Configure(-1, UsesDetermination.Fixed, AttributeDefinitions.Charisma,
                ActivationTime.PermanentUnlessIncapacitated, 1,
                RechargeRate.AtWill, false, false, AttributeDefinitions.Charisma, effect)
            .SetShowCasting(false)
            .SetGuiPresentation("FighterMarshalEncouragementPower", Category.Subclass,
                Bless.GuiPresentation.SpriteReference)
            .AddToDB();
    }
}

internal static class MarshalFighterSubclassBuilder
{
    private const string MarshalFighterSubclassName = "MarshalFighter";
    internal static readonly Guid MarshalFighterSubclassNameGuid = new("79608b4e-8293-452e-bd1a-9cf0d0e9d077");

    private static readonly FeatureDefinition CoordinatedAttack =
        CoordinatedAttackBuilder.BuildCoordinatedAttack();

    private static readonly FeatureDefinitionFeatureSet KnowYourEnemies =
        KnowYourEnemyBuilder.BuildKnowYourEnemyFeatureSet();

    private static readonly FeatureDefinitionPower StudyYourEnemy = StudyYourEnemyBuilder.BuildStudyEnemyPower();

    private static readonly FeatureDefinitionFeatureSet FearlessCommander =
        FearlessCommanderBuilder.BuildFearlessCommander();

    private static readonly FeatureDefinitionPower Encourage = EncourageBuilder.BuildEncourage();

    private static readonly FeatureDefinitionFeatureSet EternalComrade =
        EternalComradeBuilder.BuildEternalComradeFeatureSet();

    internal static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        return CharacterSubclassDefinitionBuilder
            .Create(MarshalFighterSubclassName, MarshalFighterSubclassNameGuid)
            .SetGuiPresentation("FighterMarshal", Category.Subclass, OathOfJugement.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(CoordinatedAttack, 3)
            .AddFeatureAtLevel(KnowYourEnemies, 3)
            .AddFeatureAtLevel(StudyYourEnemy, 3)
            .AddFeatureAtLevel(EternalComrade, 7)
            .AddFeatureAtLevel(FearlessCommander, 10)
            .AddFeatureAtLevel(Encourage, 10)
            .AddToDB();
    }
}
