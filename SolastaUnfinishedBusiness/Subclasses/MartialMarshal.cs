using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialMarshal : AbstractSubclass
{
    private const string FeatureSetMarshalKnowYourEnemyName = "FeatureSetMarshalKnowYourEnemy";

    private const string MarshalCoordinatedAttackName = "ReactToAttackFinishedMarshalCoordinatedAttack";

    private const string EternalComradeName = "MarshalEternalComrade";

    private static readonly ConditionDefinition ConditionEncourage = BuildEncourageCondition();

    private static readonly FeatureDefinitionPower PowerMarshalEncouragement = BuildEncourage();

    private static readonly FeatureDefinitionPower PowerMarshalImproveEncouragement = BuildImprovedEncourage();

    internal MartialMarshal()
    {
        BuildEternalComradeMonster();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialMarshal")
            .SetGuiPresentation(Category.Subclass, OathOfJugement)
            .AddFeaturesAtLevel(3,
                BuildMarshalCoordinatedAttack(),
                BuildFeatureSetMarshalKnowYourEnemyFeatureSet(),
                BuildStudyEnemyPower())
            .AddFeaturesAtLevel(7,
                BuildFeatureSetMarshalEternalComrade())
            .AddFeaturesAtLevel(10,
                BuildFeatureSetMarshalFearlessCommander(),
                PowerMarshalEncouragement)
            .AddFeaturesAtLevel(15,
                PowerMarshalImproveEncouragement)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    private static int GetKnowledgeLevelOfEnemy(RulesetCharacter enemy)
    {
        return ServiceRepository.GetService<IGameLoreService>().Bestiary.TryGetBestiaryEntry(enemy, out var entry)
            ? entry.KnowledgeLevelDefinition.Level
            : 0;
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalKnowYourEnemyFeatureSet()
    {
        var onComputeAttackModifierMarshalKnowYourEnemy = FeatureDefinitionBuilder
            .Create("OnComputeAttackModifierMarshalKnowYourEnemy")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new OnComputeAttackModifierMarshalKnowYourEnemy())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetMarshalKnowYourEnemyName)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                onComputeAttackModifierMarshalKnowYourEnemy,
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
                CommonBuilders.AdditionalDamageMarshalFavoredEnemyHumanoid
            )
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildStudyEnemyPower()
    {
        var sprite = Sprites.GetSprite("PowerStudyYourEnemy", Resources.PowerStudyYourEnemy, 128);

        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalStudyYourEnemy")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(IdentifyCreatures.EffectDescription)
                .SetDurationData(DurationType.Instantaneous)
                .ClearRestrictedCreatureFamilies()
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionMarshalStudyYourEnemy")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new StudyYourEnemy())
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildMarshalCoordinatedAttack()
    {
        return FeatureDefinitionBuilder
            .Create(MarshalCoordinatedAttackName)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ReactToAttackFinishedMarshalCoordinatedAttack())
            .AddToDB();
    }

    private static void BuildEternalComradeMonster()
    {
        _ = MonsterDefinitionBuilder
            .Create(SuperEgo_Servant_Hostile, EternalComradeName)
            .SetOrUpdateGuiPresentation(Category.Monster)
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
            .SetAttackIterations(new MonsterAttackIteration(
                MonsterAttackDefinitionBuilder
                    .Create(MonsterAttackDefinitions.Attack_Generic_Guard_Longsword,
                        "MonsterAttackMarshalEternalComrade")
                    .SetToHitBonus(5)
                    .SetEffectDescription(EffectDescriptionBuilder
                        .Create()
                        .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Count)
                        .SetEffectForms(EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeSlashing, 2, DieType.D6, 2)
                            .Build())
                        .Build())
                    .SetMagical()
                    .AddToDB(),
                1))
            .SetArmorClass(16)
            .SetAlignment(MonsterDefinitionBuilder.NeutralAlignment)
            .SetCharacterFamily(CharacterFamilyDefinitions.Undead)
            .SetCreatureTags(EternalComradeName)
            .SetDefaultBattleDecisionPackage(DefaultMeleeWithBackupRangeDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalEternalComrade()
    {
        var sprite = Sprites.GetSprite("PowerMarshalSummonEternalComrade", Resources.PowerMarshalSummonEternalComrade,
            128);

        //TODO: make this use concentration and reduce the duration to may be 3 rounds
        //TODO: increase the number of use to 2 and recharge per long rest
        var powerMarshalSummonEternalComrade = FeatureDefinitionPowerBuilder
            .Create("PowerMarshalSummonEternalComrade")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(ConjureAnimalsOneBeast.EffectDescription)
                .SetDurationData(DurationType.Round, 10)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetSummonCreatureForm(1, EternalComradeName)
                        .Build())
                .Build())
            .AddToDB();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, powerMarshalSummonEternalComrade);

        var conditionMarshalEternalComradeAc = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondAC, "ConditionMarshalEternalComradeAC")
            .SetGuiPresentationNoContent(true)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeSavingThrow = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondSavingThrows, "ConditionMarshalEternalComradeSavingThrow")
            .SetGuiPresentationNoContent(true)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeDamage = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionMarshalEternalComradeDamage")
            .SetGuiPresentationNoContent(true)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHit = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionMarshalEternalComradeHit")
            .SetGuiPresentationNoContent(true)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHp = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondHP, "ConditionMarshalEternalComradeHP")
            .SetGuiPresentationNoContent(true)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel)
            .AllowMultipleInstances()
            .SetAdditionalDamageType(FighterClass)
            .AddToDB();

        var summoningAffinityMarshalEternalComrade = FeatureDefinitionSummoningAffinityBuilder
            .Create(SummoningAffinityKindredSpiritBond, "SummoningAffinityMarshalEternalComrade")
            .ClearEffectForms()
            .SetRequiredMonsterTag(EternalComradeName)
            .SetAddedConditions(
                conditionMarshalEternalComradeAc,
                conditionMarshalEternalComradeSavingThrow,
                conditionMarshalEternalComradeDamage,
                conditionMarshalEternalComradeHit,
                conditionMarshalEternalComradeHp,
                conditionMarshalEternalComradeHp)
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMarshalEternalComrade")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(summoningAffinityMarshalEternalComrade, powerMarshalSummonEternalComrade)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalFearlessCommander()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMarshalFearlessCommander")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(ConditionAffinityFrightenedImmunity)
            .AddToDB();
    }

    private static ConditionDefinition BuildEncourageCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionMarshalEncouraged")
            .SetGuiPresentation("PowerMarshalEncouragement", Category.Feature, ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionCombatAffinitys.CombatAffinityBlessed,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBlessed)
            .SetSpecialDuration(DurationType.Permanent, 0, TurnOccurenceType.StartOfTurn)
            .AddConditionTags("Buff")
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEncourage()
    {
        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(ConditionEncourage);

        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalEncouragement")
            .SetGuiPresentation(Category.Feature, Bless)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 5)
                .SetDurationData(DurationType.Permanent)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionEncourage, ConditionForm.ConditionOperation.Add, false,
                            false)
                        .Build())
                .Build())
            .SetShowCasting(false)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEncourage()
    {
        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(ConditionEncourage);

        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalImprovedEncouragement")
            .SetOverriddenPower(PowerMarshalEncouragement)
            .SetGuiPresentation(Category.Feature, Bless)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 13)
                .SetDurationData(DurationType.Permanent)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionEncourage, ConditionForm.ConditionOperation.Add, false,
                            false)
                        .Build())
                .Build())
            .SetShowCasting(false)
            .AddToDB();
    }

    private sealed class ReactToAttackFinishedMarshalCoordinatedAttack : IReactToMyAttackFinished
    {
        public IEnumerator HandleReactToMyAttackFinished(
            GameLocationCharacter me,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            [NotNull] RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            // melee only
            if (attackMode.ranged || outcome is RollOutcome.CriticalFailure or RollOutcome.Failure ||
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
                    || !rulesetCharacterMonster.MonsterDefinition.CreatureTags.Contains(EternalComradeName))
                {
                    continue;
                }

                if (!rulesetCharacterMonster.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
                        RuleDefinitions.ConditionConjuredCreature,
                        out var activeCondition)
                    || activeCondition.SourceGuid != me.Guid)
                {
                    continue;
                }

                var gameLocationMonster = GameLocationCharacter.GetFromActor(rulesetCharacterMonster);

                if (gameLocationMonster.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) ==
                    ActionDefinitions.ActionStatus.Available
                    && !gameLocationMonster.Prone)
                {
                    allies.Add(gameLocationMonster);
                }
            }

            allies.AddRange(characterService.PartyCharacters
                .Where(partyCharacter =>
                    partyCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) == 0 &&
                    !partyCharacter.Prone)
                .Where(partyCharacter => partyCharacter != me));

            var reactions = new List<CharacterActionParams>();

            foreach (var partyCharacter in allies)
            {
                var allAttackMode = partyCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
                var attackParams = default(BattleDefinitions.AttackEvaluationParams);
                var actionModifierBefore = new ActionModifier();
                var canAttack = true;

                if (allAttackMode == null)
                {
                    continue;
                }

                if (allAttackMode.Ranged)
                {
                    attackParams.FillForPhysicalRangeAttack(partyCharacter, partyCharacter.LocationPosition,
                        allAttackMode,
                        defender, defender.LocationPosition, actionModifierBefore);
                }
                else
                {
                    attackParams.FillForPhysicalReachAttack(partyCharacter, partyCharacter.LocationPosition,
                        allAttackMode,
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
                    StringParameter2 = MarshalCoordinatedAttackName, BoolParameter4 = !canAttack
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

            yield return battleManager.WaitForReactions(me, actionService, count);
        }
    }

    private sealed class OnComputeAttackModifierMarshalKnowYourEnemy : IOnComputeAttackModifier
    {
        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            // no spell attack
            if (attackMode == null || defender == null)
            {
                return;
            }

            var knowledgeLevelOfEnemy = GetKnowledgeLevelOfEnemy(defender);

            attackModifier.attackRollModifier += knowledgeLevelOfEnemy;
            attackModifier.attackToHitTrends.Add(new TrendInfo(knowledgeLevelOfEnemy,
                FeatureSourceType.CharacterFeature, "Feature/&FeatureSetMarshalKnowYourEnemyTitle", null));
        }
    }

    private sealed class StudyYourEnemy : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var manager = ServiceRepository.GetService<IGameLoreService>();
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(target);

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster creature)
            {
                return;
            }

            var sourceCharacter = EffectHelpers.GetCharacterByGuid(rulesetCondition.sourceGuid);

            if (sourceCharacter == null)
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

            if (entry == null)
            {
                return;
            }

            var checkModifier = new ActionModifier();
            var roller = GameLocationCharacter.GetFromActor(sourceCharacter);

            roller.RollAbilityCheck(AttributeDefinitions.Wisdom, SkillDefinitions.Survival,
                10 + Mathf.FloorToInt(entry.MonsterDefinition.ChallengeRating), AdvantageType.None, checkModifier,
                false, -1, out var outcome, out _, true);

            const int MAX_KNOWLEDGE_LEVEL = 4;

            var level = entry.KnowledgeLevelDefinition.Level;

            // rollback one level to at least get a nice message on UI and not get a null after we call Invoke
            if (level == MAX_KNOWLEDGE_LEVEL)
            {
                level--;
                entry.KnowledgeLevelDefinition.level--;
            }

            var newLevel = level;

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                var learned = outcome == RollOutcome.Success ? 1 : 2;

                newLevel = Mathf.Min(entry.KnowledgeLevelDefinition.Level + learned, 4);

                manager.LearnMonsterKnowledge(entry.MonsterDefinition,
                    manager.Bestiary.SortedKnowledgeLevels[newLevel]);
            }

            gameLocationCharacter.RulesetCharacter.MonsterIdentificationRolled?.Invoke(
                gameLocationCharacter.RulesetCharacter, entry.MonsterDefinition, outcome, level, newLevel);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }
}
