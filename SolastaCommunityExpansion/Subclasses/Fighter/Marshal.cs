using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.MonsterAttackDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaModApi.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static RuleDefinitions;
using UnityEngine;

namespace SolastaCommunityExpansion.Subclasses.Fighter
{
    internal class Marshal : AbstractSubclass
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
        private static int getKnowledgeLevelOfEnemy(RulesetCharacter enemy)
        {
            GameBestiaryEntry entry = null;
            if (ServiceRepository.GetService<IGameLoreService>().Bestiary.TryGetBestiaryEntry(enemy, out entry))
            {
                return entry.KnowledgeLevelDefinition.Level;
            }

            return 0;
        }

        private static void KnowYoureEnemyOnAttackDelegate(GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            // no spell attack
            if (attackerAttackMode == null || attackModifier == null)
            {
                return;
            }
            attackModifier.AttacktoHitTrends.Add(new TrendInfo(getKnowledgeLevelOfEnemy(defender.RulesetCharacter), FeatureSourceType.CharacterFeature, "KnowYourEnemies", null));
        }

        public static FeatureDefinitionFeatureSet BuildKnowYourEnemyFeatureSet()
        {
            var knowYourEnemiesAttackHitModifier = FeatureDefinitionOnAttackEffectBuilder
                .Create("KnowYourEnemyAttackHitModifier", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentation("FigherMarshalKnowYourEnemyAttakHitModifer",Category.Subclass)
                .SetOnAttackDelegates(KnowYoureEnemyOnAttackDelegate, null)
                .AddToDB();

            var AdditionalDamageRangerFavoredEnemyHumanonid = FeatureDefinitionAdditionalDamageBuilder
                .Create("MarshalAdditionalDamageFavoredEnemyHumanoid", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetNotificationTag("FavoredEnemy")
                .SetTriggerCondition(AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .AddToDB();
            AdditionalDamageRangerFavoredEnemyHumanonid.SetRequiredCharacterFamily(CharacterFamilyDefinitions.Humanoid);

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
                    AdditionalDamageRangerFavoredEnemyHumanonid
                 )
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .AddToDB();
        }
    }

    internal static class StudyYourEnemyBuilder
    { 
        internal sealed class StudyEnemyEffectDescrption: CustomEffectForm
        {
            public override void ApplyForm(
                RulesetImplementationDefinitions.ApplyFormsParams formsParams,
                bool retargeting,
                bool proxyOnly,
                bool forceSelfConditionOnly,
                EffectApplication effectApplication = EffectApplication.All,
                List<EffectFormFilter> filters = null)
            {
                GameLoreManager manager = ServiceRepository.GetService<IGameLoreService>() as GameLoreManager;
           
                var gameLocationCharacter = GameLocationCharacter.GetFromActor(formsParams.targetCharacter);
                var creature = gameLocationCharacter.RulesetCharacter;

                GameBestiaryEntry entry = null;
                if (!manager.Bestiary.TryGetBestiaryEntry(creature, out entry) && creature is RulesetCharacterMonster && (creature as RulesetCharacterMonster).MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Full)
                {
                    entry = manager.Bestiary.AddNewMonsterEntry((creature as RulesetCharacterMonster).MonsterDefinition);
                    manager.MonsterKnowledgeChanged?.Invoke((creature as RulesetCharacterMonster).MonsterDefinition, entry.KnowledgeLevelDefinition);
                }

                ActionModifier checkModifier = new ActionModifier();
                RollOutcome outcome = RollOutcome.Neutral;
                var roller = GameLocationCharacter.GetFromActor(formsParams.sourceCharacter);
                roller.RollAbilityCheck("Wisdom", "Survival", 10 + Mathf.FloorToInt(entry.MonsterDefinition.ChallengeRating), RuleDefinitions.AdvantageType.None, checkModifier, false, -1, out outcome, true);

                int level = entry.KnowledgeLevelDefinition.Level;
                int num = level;
                if (outcome == RollOutcome.Success || outcome == RollOutcome.CriticalSuccess)
                {
                    int num2 = ((outcome == RollOutcome.Success) ? 1 : 2);
                    num = Mathf.Min(entry.KnowledgeLevelDefinition.Level + num2, 4);
                    manager.LearnMonsterKnowledge(entry.MonsterDefinition, manager.Bestiary.SortedKnowledgeLevels[num]);
                }
                gameLocationCharacter?.RulesetCharacter.MonsterIdentificationRolled?.Invoke(gameLocationCharacter.RulesetCharacter, entry.MonsterDefinition, outcome, level, num);
            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {

            }
        }
        public static FeatureDefinitionPower BuildStudyEnemyPower()
        {
            var effectDescription = IdentifyCreatures.EffectDescription
                .Copy()
                .SetDuration(DurationType.Instantaneous)
                .SetHasSavingThrow(false)
                .SetRange(RangeType.Distance, 12)
                .SetTargetType(TargetType.Individuals)
                .SetTargetParameter(1)
                .ClearRestrictedCreatureFamilies()
                .SetEffectForms(new StudyEnemyEffectDescrption());

            return FeatureDefinitionPowerBuilder
                .Create("StudyYourEnemy", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentation("FighterMarshalStudyYourEnemyPower",Category.Subclass)
                .SetRechargeRate(RechargeRate.AtWill)
                .SetActivation(ActivationTime.BonusAction, 0)
                .SetEffectDescription(effectDescription)
                .AddToDB();
        }
    }

    internal static class CoordinatedAttackBuilder
    {
        private static void CoordinatedAttackOnAttackHitDelegate(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged)
        {
            // melee only
            if (ranged)
            {
                return;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            GameLocationBattleManager battlelocationSerivce = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // collect allied that can attack for 
            Main.Log(string.Format("total party member {0}", characterService.PartyCharacters.Count), true);

            var allies = new List<GameLocationCharacter>();
            foreach (GameLocationCharacter guestCharacter in characterService.GuestCharacters)
            {
                if (guestCharacter.RulesetCharacter is RulesetCharacterMonster && (guestCharacter.RulesetCharacter as RulesetCharacterMonster).MonsterDefinition.CreatureTags.Contains("MarshalEternalComrade"))
                {
                    RulesetCharacterMonster rulesetCharacterMonster = guestCharacter.RulesetCharacter as RulesetCharacterMonster;
                    RulesetCondition activeCondition = null;
                    if (rulesetCharacterMonster.TryGetConditionOfCategoryAndType("17TagConjure", "ConditionConjuredCreature", out activeCondition) && activeCondition.SourceGuid ==attacker.Guid)
                    {
                        var gameLocationMonster = GameLocationCharacter.GetFromActor(rulesetCharacterMonster);
                        allies.Add(gameLocationMonster);
                    }
                }
            }


            foreach (GameLocationCharacter partyCharacter in characterService.PartyCharacters)
            {
                if (partyCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) != 0)
                {
                    Main.Log(string.Format("party member name {0} doesn't have reaction available", partyCharacter.Name), true);
                    continue;
                }

                if (partyCharacter == attacker)
                {
                    Main.Log(string.Format("party member name {0} is attacker", partyCharacter.Name), true);
                    continue;
                }

                allies.Add(partyCharacter);
            }

            foreach (GameLocationCharacter partyCharacter in allies)
            { 
                var attackMode = partyCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
                BattleDefinitions.AttackEvaluationParams attackParams = default(BattleDefinitions.AttackEvaluationParams);
                var actionModifierBefore = new ActionModifier();
                if (attackMode.Ranged)
                {
                    Main.Log(string.Format("party member {0} can use ranged attack", partyCharacter.Name), true);
                    attackParams.FillForPhysicalRangeAttack(partyCharacter, partyCharacter.LocationPosition, attackMode, defender, defender.LocationPosition, actionModifierBefore);
                } else
                {
                    Main.Log(string.Format("party member {0} can use melee attack", partyCharacter.Name), true);
                    attackParams.FillForPhysicalReachAttack(partyCharacter, partyCharacter.LocationPosition, attackMode, defender, defender.LocationPosition, actionModifierBefore);
                }

                if (!battlelocationSerivce.CanAttack(attackParams))
                {
                    Main.Log(string.Format("party member name {0} cannot attack", partyCharacter.Name), true);
                    continue;
                }

                CharacterActionParams reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.AttackOpportunity, attackMode, defender, actionModifierBefore);
                actionService.ReactForOpportunityAttack(reactionParams);
            }
        }

        public static FeatureDefinitionOnAttackHitEffect BuildCoordinatedAttack()
        {
            return FeatureDefinitionOnAttackHitEffectBuilder
                .Create("CoordinatedAttack", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentation("FighterMarshalCoordinatedAttack", Category.Subclass)
                .SetOnAttackHitDelegates(null, CoordinatedAttackOnAttackHitDelegate)
                .AddToDB();
        }
    }

    public static class EternalComradebuilder
    {
        public static MonsterDefinition EternalComrade = BuildEternalComradeMonster();

        private static MonsterDefinition BuildEternalComradeMonster()
        {
            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectForms(
                    new EffectForm()
                    {
                        FormType = EffectForm.EffectFormType.Damage,
                        DamageForm = new DamageForm()
                        {
                            BonusDamage = 2,
                            DiceNumber = 2,
                            DieType = DieType.D6,
                            DamageType = DamageTypeSlashing
                        }
                    }
                 )
                .Build();
            effectDescription.SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Count);

            var eternalComradeAttack = MonsterAttackDefinitionBuilder
                .Create(Attack_Generic_Guard_Longsword, "AttackEternalComrade", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetEffectDescription(effectDescription)
                .AddToDB();
            eternalComradeAttack.SetMagical(true);

            var marshalEternalComradeAttackInteration = new MonsterAttackIteration(eternalComradeAttack, 1);

            return  MonsterDefinitionBuilder
                .Create(SuperEgo_Servant_Hostile, "EternalComrade", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentation("MarshalEternalComrade", Category.Monster, SuperEgo_Servant_Hostile.GuiPresentation.SpriteReference)
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
                .SetAttackIterations(marshalEternalComradeAttackInteration)
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
        public static FeatureDefinitionFeatureSet BuildEternalComradeFeatureSet()
        {
            var summonForm = new SummonForm();
            summonForm.SetMonsterDefinitionName(EternalComrade.Name);

            var effectForm = new EffectForm();
            effectForm.SetFormType(EffectForm.EffectFormType.Summon);
            effectForm.SetCreatedByCharacter(true);
            effectForm.SetSummonForm(summonForm);

            // TODO: make this use concentration and reduce the duration to may be 3 rounds
            // TODO: increase the number of use to 2 and recharge per long rest
            var summonEternalComradePower = FeatureDefinitionPowerBuilder
               .Create("SummonEternalComrade", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
               .SetGuiPresentation("FighterMarshalSummonEternalComradePower", Category.Subclass, Bane.GuiPresentation.SpriteReference)
               .SetCostPerUse(1)
               .SetUsesFixed(1)
               .SetFixedUsesPerRecharge(1)
               .SetRechargeRate(RechargeRate.ShortRest)
               .SetActivationTime(ActivationTime.BonusAction)
               .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create(ConjureAnimalsTwoBeasts.EffectDescription.Copy())
                        .SetDurationData(DurationType.Round, 10)
                        .ClearEffectForms()
                        .AddEffectForm(effectForm)
                        .Build()
                )
               .AddToDB();
            GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, summonEternalComradePower);

            var acConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondAC, "ConditionMarshalEternarlComradeAC", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var stConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondSavingThrows, "ConditionMarshalEternarlComradeST", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var damageConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionMarshalEternarlComradeDamage", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var hitConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionMarshalEternarlComradeHit", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var hpConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondHP, "ConditionMarshalEternarlComradeHP", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
                .SetAllowMultipleInstances(true)
                .AddToDB();

            // Find a better place to put this in?
            hpConditionDefinition.SetAdditionalDamageType("Fighter");

            var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
                .Create(SummoningAffinityKindredSpiritBond, "SummoningAffinityMarshalEternalComrade", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
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

    public static class FearlessCommanderBuilder
    {
        public static FeatureDefinitionFeatureSet BuildFearlessCommander ()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create("FearlessCommander", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .SetFeatureSet(ConditionAffinityFrightenedImmunity)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetGuiPresentation("FighterMarshalFearlessCommanderFeatureSet", Category.Subclass)
                .AddToDB();
        }
    }

    public static class EncourageBuilder
    {
        public static FeatureDefinitionPower BuildEncourage()
        {
            var effect = EffectDescriptionBuilder
                .Create(Bless.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique, 6, 6)
                .SetDurationData(DurationType.UntilLongRest)
                .Build();

            return FeatureDefinitionPowerBuilder
                .Create("Encouragement", MarshalFighterSubclassBuilder.MarshalFighterSubclassNameGuid)
                .Configure(1, UsesDetermination.Fixed, AttributeDefinitions.Charisma, ActivationTime.Hours1, 1, RechargeRate.LongRest, false, false, AttributeDefinitions.Charisma, effect)
                .SetGuiPresentation("FighterMarshalEncouragementPower", Category.Subclass, Bless.GuiPresentation.SpriteReference)
                .AddToDB();
        }
    }

    public static class MarshalFighterSubclassBuilder
    {
        public static readonly System.Guid MarshalFighterSubclassNameGuid  = new("79608b4e-8293-452e-bd1a-9cf0d0e9d077");
        public const string MarshalFighterSubclassName = "MarshalFighter";

        public static CharacterSubclassDefinition BuildAndAddSubclass()
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

        public static readonly FeatureDefinitionOnAttackHitEffect CoordinatedAttack = CoordinatedAttackBuilder.BuildCoordinatedAttack();
        public static readonly FeatureDefinitionFeatureSet KnowYourEnemies = KnowYourEnemyBuilder.BuildKnowYourEnemyFeatureSet();
        public static readonly FeatureDefinitionPower StudyYourEnemy = StudyYourEnemyBuilder.BuildStudyEnemyPower();
        public static readonly FeatureDefinitionFeatureSet FearlessCommander = FearlessCommanderBuilder.BuildFearlessCommander();
        public static readonly FeatureDefinitionPower Encourage = EncourageBuilder.BuildEncourage();
        public static readonly FeatureDefinitionFeatureSet EternalComrade = EternalComradebuilder.BuildEternalComradeFeatureSet();
    }
}
