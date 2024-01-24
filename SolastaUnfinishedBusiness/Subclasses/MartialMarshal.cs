using System;
using System.Collections;
using System.Collections.Generic;
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
using UnityEngine;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialMarshal : AbstractSubclass
{
    private const string FeatureSetMarshalKnowYourEnemyName = "FeatureSetMarshalKnowYourEnemy";

    private const string MarshalCoordinatedAttackName = "ReactToAttackFinishedMarshalCoordinatedAttack";

    private const string EternalComradeName = "MarshalEternalComrade";

    private const string ConditionMarshalKnowledgeableDefenseACName = "ConditionMarshalKnowledgeableDefenseAC";

    public MartialMarshal()
    {
        BuildEternalComradeMonster();

        var conditionEncourage = BuildEncourageCondition();
        var powerMarshalEncourage = BuildEncourage(conditionEncourage);
        var powerMarshalImprovedEncourage = BuildImprovedEncourage(conditionEncourage, powerMarshalEncourage);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialMarshal")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("MartialMarshal", Resources.MartialMarshal, 256))
            .AddFeaturesAtLevel(3,
                BuildMarshalCoordinatedAttack(),
                BuildFeatureSetMarshalKnowYourEnemyFeatureSet(),
                BuildStudyEnemyPower())
            .AddFeaturesAtLevel(7,
                BuildFeatureSetMarshalEternalComrade())
            .AddFeaturesAtLevel(10,
                BuildFeatureSetMarshalFearlessCommander(),
                powerMarshalEncourage)
            .AddFeaturesAtLevel(15,
                powerMarshalImprovedEncourage)
            .AddFeaturesAtLevel(18,
                BuildKnowledgeableDefense())
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static int GetKnowledgeLevelOfEnemy(RulesetCharacter enemy)
    {
        return ServiceRepository.GetService<IGameLoreService>().Bestiary.TryGetBestiaryEntry(enemy, out var entry)
            ? entry.KnowledgeLevelDefinition.Level
            : 0;
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalKnowYourEnemyFeatureSet()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetMarshalKnowYourEnemyName)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageMarshalKnowYourEnemy")
                    .SetGuiPresentationNoContent()
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
                    .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                    .SetNotificationTag("KnowYourEnemy")
                    .AddCustomSubFeatures(new ModifyAttackActionModifierMarshalKnowYourEnemy())
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildStudyEnemyPower()
    {
        var sprite = Sprites.GetSprite("PowerStudyYourEnemy", Resources.PowerStudyYourEnemy, 256, 128);

        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalStudyYourEnemy")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(IdentifyCreatures)
                    .SetDurationData(DurationType.Instantaneous)
                    .ClearRestrictedCreatureFamilies()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionMarshalStudyYourEnemy")
                                    .SetGuiPresentationNoContent(true)
                                    .AddCustomSubFeatures(new OnConditionAddedOrRemovedStudyYourEnemy())
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildMarshalCoordinatedAttack()
    {
        return FeatureDefinitionBuilder
            .Create(MarshalCoordinatedAttackName)
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ReactToAttackFinishedByMeMarshalCoordinatedAttack())
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
                ConditionAffinityHinderedByFrostImmunity)
            .SetAttackIterations(new MonsterAttackIteration(
                MonsterAttackDefinitionBuilder
                    .Create(MonsterAttackDefinitions.Attack_Generic_Guard_Longsword,
                        "MonsterAttackMarshalEternalComrade")
                    .SetToHitBonus(5)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Count)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetDamageForm(DamageTypeSlashing, 2, DieType.D6, 2)
                                    .SetCreatedBy(false, false)
                                    .Build())
                            .Build())
                    .SetMagical()
                    .AddToDB(),
                1))
            .SetArmorClass(16)
            .SetAlignment(MonsterDefinitionBuilder.NeutralAlignment)
            .SetCharacterFamily(CharacterFamilyDefinitions.Undead.Name)
            .SetCreatureTags(EternalComradeName)
            .SetDefaultBattleDecisionPackage(DefaultMeleeWithBackupRangeDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalEternalComrade()
    {
        var sprite = Sprites.GetSprite("PowerMarshalSummonEternalComrade",
            Resources.PowerMarshalSummonEternalComrade, 256, 128);

        var powerMarshalSummonEternalComrade = FeatureDefinitionPowerBuilder
            .Create("PowerMarshalSummonEternalComrade")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ConjureAnimalsOneBeast.EffectDescription)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, EternalComradeName)
                            .Build())
                    .Build())
            .AddToDB();

        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.Familiar, powerMarshalSummonEternalComrade);

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMarshalEternalComradeHP")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.AddConditionAmount,
                AttributeDefinitions.HitPoints)
            .AddToDB();

        var conditionMarshalEternalComradeAc = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondAC, "ConditionMarshalEternalComradeAC")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeSavingThrow = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondSavingThrows, "ConditionMarshalEternalComradeSavingThrow")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeDamage = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionMarshalEternalComradeDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHit = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionMarshalEternalComradeHit")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHp = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondHP, "ConditionMarshalEternalComradeHP")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel, FighterClass)
            .SetFeatures(hpBonus, hpBonus) // 2 HP per level
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
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEncourage(ConditionDefinition conditionEncourage)
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalEncouragement")
            .SetGuiPresentation(Category.Feature, Bless)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 5)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEncourage, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(false)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEncourage(
        ConditionDefinition conditionEncourage, FeatureDefinitionPower overridenPower)
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalImprovedEncouragement")
            .SetGuiPresentation(Category.Feature, Bless)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 13)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEncourage, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(overridenPower)
            .SetShowCasting(false)
            .AddToDB();
    }

    private static FeatureDefinition BuildKnowledgeableDefense()
    {
        for (var i = 1; i <= 3; i++)
        {
            _ = ConditionDefinitionBuilder
                .Create($"{ConditionMarshalKnowledgeableDefenseACName}{i}")
                .SetGuiPresentation("FeatureMarshalKnowledgeableDefense", Category.Feature)
                .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .SetSilent(Silent.WhenRemoved)
                .SetPossessive()
                .AddFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create($"AttributeModifierKnowledgeableDefenseAC{i}")
                        .SetGuiPresentation("Feature/&FeatureMarshalKnowledgeableDefenseTitle",
                            $"Feature/&AttributeModifierACPlus{i}Description")
                        .SetModifier(AttributeModifierOperation.Additive,
                            AttributeDefinitions.ArmorClass, i)
                        .AddToDB())
                .AddToDB();
        }

        var featureMarshalKnowledgeableDefense = FeatureDefinitionBuilder
            .Create("FeatureMarshalKnowledgeableDefense")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new PhysicalAttackInitiatedOnMeKnowledgeableDefense())
            .AddToDB();

        return featureMarshalKnowledgeableDefense;
    }

    private sealed class ReactToAttackFinishedByMeMarshalCoordinatedAttack : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var actionParams = action.actionParams;

            // non-reaction melee hits only
            if (attackMode.ranged || attackRollOutcome is RollOutcome.CriticalFailure or RollOutcome.Failure ||
                actionParams.actionDefinition.Id == ActionDefinitions.Id.AttackOpportunity)
            {
                yield break;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var allies = new List<GameLocationCharacter>();

            foreach (var guestCharacter in characterService.GuestCharacters.ToList())
            {
                if (guestCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster
                    || !rulesetCharacterMonster.MonsterDefinition.CreatureTags.Contains(EternalComradeName))
                {
                    continue;
                }

                if (!rulesetCharacterMonster.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
                        RuleDefinitions.ConditionConjuredCreature,
                        out var activeCondition)
                    || activeCondition.SourceGuid != attacker.Guid)
                {
                    continue;
                }

                if (guestCharacter.CanReact())
                {
                    allies.Add(guestCharacter);
                }
            }

            allies.AddRange(characterService.PartyCharacters
                .Where(partyCharacter => partyCharacter.CanReact() && partyCharacter != attacker));

            var reactions = new List<CharacterActionParams>();

            foreach (var partyCharacter in allies)
            {
                RulesetAttackMode mode;
                ActionModifier modifier;

                //prefer melee if main hand is melee or if enemy is close
                var preferMelee =
                    ValidatorsWeapon.IsMelee(partyCharacter.RulesetCharacter.GetMainWeapon()) ||
                    partyCharacter.IsWithinRange(defender, 1);

                var (meleeMode, meleeModifier) = partyCharacter.GetFirstMeleeModeThatCanAttack(defender);
                var (rangedMode, rangedModifier) = partyCharacter.GetFirstRangedModeThatCanAttack(defender);

                if (preferMelee)
                {
                    mode = meleeMode ?? rangedMode;
                    modifier = meleeModifier ?? rangedModifier;
                }
                else
                {
                    mode = rangedMode ?? meleeMode;
                    modifier = rangedModifier ?? meleeModifier;
                }

                if (mode == null)
                {
                    var cantrips = ReactionRequestWarcaster.GetValidCantrips(battleManager, partyCharacter, defender);

                    if (cantrips == null || cantrips.Empty())
                    {
                        continue;
                    }
                }

                var reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.AttackOpportunity)
                {
                    StringParameter2 = MarshalCoordinatedAttackName,
                    BoolParameter4 = mode == null //true means no attack
                };
                reactionParams.targetCharacters.Add(defender);
                reactionParams.actionModifiers.Add(modifier ?? new ActionModifier());
                if (mode != null)
                {
                    reactionParams.attackMode = RulesetAttackMode.AttackModesPool.Get();
                    reactionParams.attackMode.Copy(mode);
                }

                reactions.Add(reactionParams);
            }

            if (reactions.Empty() || battleManager is not { IsBattleInProgress: true })
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
    }

    private sealed class ModifyAttackActionModifierMarshalKnowYourEnemy : IModifyAttackActionModifier
    {
        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackProximity != BattleDefinitions.AttackProximity.PhysicalRange &&
                attackProximity != BattleDefinitions.AttackProximity.PhysicalReach)
            {
                return;
            }

            var knowledgeLevelOfEnemy = GetKnowledgeLevelOfEnemy(defender);

            attackModifier.attackRollModifier += knowledgeLevelOfEnemy;
            attackModifier.attackToHitTrends.Add(new TrendInfo(knowledgeLevelOfEnemy,
                FeatureSourceType.CharacterFeature, "Feature/&FeatureSetMarshalKnowYourEnemyTitle", null));
        }
    }

    private sealed class OnConditionAddedOrRemovedStudyYourEnemy : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var gameLoreService = ServiceRepository.GetService<IGameLoreService>();
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(target);

            if (gameLocationCharacter is not { RulesetCharacter: RulesetCharacterMonster creature })
            {
                return;
            }

            var sourceCharacter = EffectHelpers.GetCharacterByGuid(rulesetCondition.sourceGuid);

            if (sourceCharacter == null)
            {
                return;
            }

            if (!gameLoreService.Bestiary.TryGetBestiaryEntry(creature, out var entry)
                && (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Full
                    || (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Reference &&
                        creature.MonsterDefinition.BestiaryReference.BestiaryEntry ==
                        BestiaryDefinitions.BestiaryEntry.Full)))
            {
                entry = gameLoreService.Bestiary.AddNewMonsterEntry(creature.MonsterDefinition);
                gameLoreService.MonsterKnowledgeChanged?.Invoke(creature.MonsterDefinition,
                    entry.KnowledgeLevelDefinition);
            }

            if (entry == null)
            {
                return;
            }

            var checkModifier = new ActionModifier();
            var roller = GameLocationCharacter.GetFromActor(sourceCharacter);

            if (roller == null)
            {
                return;
            }

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

                gameLoreService.LearnMonsterKnowledge(entry.MonsterDefinition,
                    gameLoreService.Bestiary.SortedKnowledgeLevels[newLevel]);
            }

            gameLocationCharacter.RulesetCharacter.MonsterIdentificationRolled?.Invoke(
                gameLocationCharacter.RulesetCharacter, entry.MonsterDefinition, outcome, level, newLevel);
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private class PhysicalAttackInitiatedOnMeKnowledgeableDefense : IPhysicalAttackInitiatedOnMe
    {
        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var gameLoreService = ServiceRepository.GetService<IGameLoreService>();

            if (gameLoreService == null)
            {
                yield break;
            }

            var rulesetMe = defender.RulesetCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetMe is not { IsDeadOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!gameLoreService.Bestiary.TryGetBestiaryEntry(rulesetAttacker, out var gameBestiaryEntry) ||
                gameBestiaryEntry.KnowledgeLevelDefinition == null)
            {
                yield break;
            }

            var level = Math.Min(gameBestiaryEntry.KnowledgeLevelDefinition.Level, 3);

            rulesetMe.InflictCondition(
                $"{ConditionMarshalKnowledgeableDefenseACName}{level}",
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetMe.guid,
                rulesetMe.CurrentFaction.Name,
                1,
                $"{ConditionMarshalKnowledgeableDefenseACName}{level}",
                0,
                0,
                0);
        }
    }
}
