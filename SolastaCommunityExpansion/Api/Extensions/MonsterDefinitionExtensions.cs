using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using AK.Wwise;
using SolastaModApi.Infrastructure;
using TA.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(MonsterDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class MonsterDefinitionExtensions
    {
        public static T AddAttackIterations<T>(this T entity, params MonsterAttackIteration[] value)
            where T : MonsterDefinition
        {
            AddAttackIterations(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAttackIterations<T>(this T entity, IEnumerable<MonsterAttackIteration> value)
            where T : MonsterDefinition
        {
            entity.AttackIterations.AddRange(value);
            return entity;
        }

        public static T AddAudioSwitches<T>(this T entity, params Switch[] value)
            where T : MonsterDefinition
        {
            AddAudioSwitches(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAudioSwitches<T>(this T entity, IEnumerable<Switch> value)
            where T : MonsterDefinition
        {
            entity.AudioSwitches.AddRange(value);
            return entity;
        }

        public static T AddAudioSwitchesOnHands<T>(this T entity, params Switch[] value)
            where T : MonsterDefinition
        {
            AddAudioSwitchesOnHands(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAudioSwitchesOnHands<T>(this T entity, IEnumerable<Switch> value)
            where T : MonsterDefinition
        {
            entity.AudioSwitchesOnHands.AddRange(value);
            return entity;
        }

        public static T AddBestiaryLootOptions<T>(this T entity, params ItemDefinition[] value)
            where T : MonsterDefinition
        {
            AddBestiaryLootOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddBestiaryLootOptions<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : MonsterDefinition
        {
            entity.BestiaryLootOptions.AddRange(value);
            return entity;
        }

        public static T AddCreatureTags<T>(this T entity, params String[] value)
            where T : MonsterDefinition
        {
            AddCreatureTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddCreatureTags<T>(this T entity, IEnumerable<String> value)
            where T : MonsterDefinition
        {
            entity.CreatureTags.AddRange(value);
            return entity;
        }

        public static T AddFeatures<T>(this T entity, params FeatureDefinition[] value)
            where T : MonsterDefinition
        {
            AddFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : MonsterDefinition
        {
            entity.Features.AddRange(value);
            return entity;
        }

        public static T AddLanguages<T>(this T entity, params String[] value)
            where T : MonsterDefinition
        {
            AddLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLanguages<T>(this T entity, IEnumerable<String> value)
            where T : MonsterDefinition
        {
            entity.Languages.AddRange(value);
            return entity;
        }

        public static T AddLegendaryActionOptions<T>(this T entity, params LegendaryActionDescription[] value)
            where T : MonsterDefinition
        {
            AddLegendaryActionOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLegendaryActionOptions<T>(this T entity, IEnumerable<LegendaryActionDescription> value)
            where T : MonsterDefinition
        {
            entity.LegendaryActionOptions.AddRange(value);
            return entity;
        }

        public static T AddSavingThrowScores<T>(this T entity, params MonsterSavingThrowProficiency[] value)
            where T : MonsterDefinition
        {
            AddSavingThrowScores(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSavingThrowScores<T>(this T entity, IEnumerable<MonsterSavingThrowProficiency> value)
            where T : MonsterDefinition
        {
            entity.SavingThrowScores.AddRange(value);
            return entity;
        }

        public static T AddSkillScores<T>(this T entity, params MonsterSkillProficiency[] value)
            where T : MonsterDefinition
        {
            AddSkillScores(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSkillScores<T>(this T entity, IEnumerable<MonsterSkillProficiency> value)
            where T : MonsterDefinition
        {
            entity.SkillScores.AddRange(value);
            return entity;
        }

        public static T ClearAttackIterations<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.AttackIterations.Clear();
            return entity;
        }

        public static T ClearAudioSwitches<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.AudioSwitches.Clear();
            return entity;
        }

        public static T ClearAudioSwitchesOnHands<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.AudioSwitchesOnHands.Clear();
            return entity;
        }

        public static T ClearBestiaryLootOptions<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.BestiaryLootOptions.Clear();
            return entity;
        }

        public static T ClearCreatureTags<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.CreatureTags.Clear();
            return entity;
        }

        public static T ClearFeatures<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.Features.Clear();
            return entity;
        }

        public static T ClearLanguages<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.Languages.Clear();
            return entity;
        }

        public static T ClearLegendaryActionOptions<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.LegendaryActionOptions.Clear();
            return entity;
        }

        public static T ClearSavingThrowScores<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.SavingThrowScores.Clear();
            return entity;
        }

        public static T ClearSkillScores<T>(this T entity)
            where T : MonsterDefinition
        {
            entity.SkillScores.Clear();
            return entity;
        }

        public static MonsterDefinition Copy(this MonsterDefinition entity)
        {
            var copy = new MonsterDefinition();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAbilityScores<T>(this T entity, Int32[] value)
            where T : MonsterDefinition
        {
            entity.SetField("abilityScores", value);
            return entity;
        }

        public static T SetAlignment<T>(this T entity, String value)
            where T : MonsterDefinition
        {
            entity.SetField("alignment", value);
            return entity;
        }

        public static T SetAlwaysHideStats<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("alwaysHideStats", value);
            return entity;
        }

        public static T SetArmor<T>(this T entity, String value)
            where T : MonsterDefinition
        {
            entity.SetField("armor", value);
            return entity;
        }

        public static T SetArmorClass<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.ArmorClass = value;
            return entity;
        }

        public static T SetAttackIterations<T>(this T entity, params MonsterAttackIteration[] value)
            where T : MonsterDefinition
        {
            SetAttackIterations(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAttackIterations<T>(this T entity, IEnumerable<MonsterAttackIteration> value)
            where T : MonsterDefinition
        {
            entity.AttackIterations.SetRange(value);
            return entity;
        }

        public static T SetAudioRaceRTPCValue<T>(this T entity, Single value)
            where T : MonsterDefinition
        {
            entity.SetField("audioRaceRTPCValue", value);
            return entity;
        }

        public static T SetAudioSwitches<T>(this T entity, params Switch[] value)
            where T : MonsterDefinition
        {
            SetAudioSwitches(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAudioSwitches<T>(this T entity, IEnumerable<Switch> value)
            where T : MonsterDefinition
        {
            entity.AudioSwitches.SetRange(value);
            return entity;
        }

        public static T SetAudioSwitchesOnHands<T>(this T entity, params Switch[] value)
            where T : MonsterDefinition
        {
            SetAudioSwitchesOnHands(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAudioSwitchesOnHands<T>(this T entity, IEnumerable<Switch> value)
            where T : MonsterDefinition
        {
            entity.AudioSwitchesOnHands.SetRange(value);
            return entity;
        }

        public static T SetBestiaryCameraOffset<T>(this T entity, Vector3 value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiaryCameraOffset", value);
            return entity;
        }

        public static T SetBestiaryEntry<T>(this T entity, BestiaryDefinitions.BestiaryEntry value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiaryEntry", value);
            return entity;
        }

        public static T SetBestiaryLootOptions<T>(this T entity, params ItemDefinition[] value)
            where T : MonsterDefinition
        {
            SetBestiaryLootOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetBestiaryLootOptions<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : MonsterDefinition
        {
            entity.BestiaryLootOptions.SetRange(value);
            return entity;
        }

        public static T SetBestiaryReference<T>(this T entity, MonsterDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiaryReference", value);
            return entity;
        }

        public static T SetBestiarySpriteReference<T>(this T entity, AssetReferenceSprite value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiarySpriteReference", value);
            return entity;
        }

        public static T SetChallengeRating<T>(this T entity, Single value)
            where T : MonsterDefinition
        {
            entity.ChallengeRating = value;
            return entity;
        }

        public static T SetCharacterFamily<T>(this T entity, String value)
            where T : MonsterDefinition
        {
            entity.SetField("characterFamily", value);
            return entity;
        }

        public static T SetCreatureTags<T>(this T entity, params String[] value)
            where T : MonsterDefinition
        {
            SetCreatureTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetCreatureTags<T>(this T entity, IEnumerable<String> value)
            where T : MonsterDefinition
        {
            entity.CreatureTags.SetRange(value);
            return entity;
        }

        public static T SetDefaultBattleDecisionPackage<T>(this T entity, DecisionPackageDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("defaultBattleDecisionPackage", value);
            return entity;
        }

        public static T SetDefaultFaction<T>(this T entity, String value)
            where T : MonsterDefinition
        {
            entity.SetField("defaultFaction", value);
            return entity;
        }

        public static T SetDifferentActionEachTurn<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("differentActionEachTurn", value);
            return entity;
        }

        public static T SetDroppedLootDefinition<T>(this T entity, LootPackDefinition value)
            where T : MonsterDefinition
        {
            entity.DroppedLootDefinition = value;
            return entity;
        }

        public static T SetDualSex<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("dualSex", value);
            return entity;
        }

        public static T SetDungeonMakerPresence<T>(this T entity, MonsterDefinition.DungeonMaker value)
            where T : MonsterDefinition
        {
            entity.SetField("dungeonMakerPresence", value);
            return entity;
        }

        public static T SetFeatures<T>(this T entity, params FeatureDefinition[] value)
            where T : MonsterDefinition
        {
            SetFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : MonsterDefinition
        {
            entity.Features.SetRange(value);
            return entity;
        }

        public static T SetFollowFloorAngle<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("followFloorAngle", value);
            return entity;
        }

        public static T SetForbidFastTravel<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forbidFastTravel", value);
            return entity;
        }

        public static T SetForceCombatStartsAnimation<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forceCombatStartsAnimation", value);
            return entity;
        }

        public static T SetForceHasComplexActions<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forceHasComplexActions", value);
            return entity;
        }

        public static T SetForceNoFlyAnimation<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forceNoFlyAnimation", value);
            return entity;
        }

        public static T SetForcePersistentBody<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forcePersistentBody", value);
            return entity;
        }

        public static T SetFullyControlledWhenAllied<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("fullyControlledWhenAllied", value);
            return entity;
        }

        public static T SetGroupAttacks<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("groupAttacks", value);
            return entity;
        }

        public static T SetHasLookAt<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("hasLookAt", value);
            return entity;
        }

        public static T SetHeight<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("height", value);
            return entity;
        }

        public static T SetHitDice<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("hitDice", value);
            return entity;
        }

        public static T SetHitDiceType<T>(this T entity, DieType value)
            where T : MonsterDefinition
        {
            entity.SetField("hitDiceType", value);
            return entity;
        }

        public static T SetHitPointsBonus<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("hitPointsBonus", value);
            return entity;
        }

        public static T SetInterceptStance<T>(this T entity, ActionDefinitions.MoveStance value)
            where T : MonsterDefinition
        {
            entity.SetField("interceptStance", value);
            return entity;
        }

        public static T SetIsHusk<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("isHusk", value);
            return entity;
        }

        public static T SetIsPet<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("isPet", value);
            return entity;
        }

        public static T SetIsUnique<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.IsUnique = value;
            return entity;
        }

        public static T SetLanguages<T>(this T entity, params String[] value)
            where T : MonsterDefinition
        {
            SetLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLanguages<T>(this T entity, IEnumerable<String> value)
            where T : MonsterDefinition
        {
            entity.Languages.SetRange(value);
            return entity;
        }

        public static T SetLegendaryActionOptions<T>(this T entity, params LegendaryActionDescription[] value)
            where T : MonsterDefinition
        {
            SetLegendaryActionOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLegendaryActionOptions<T>(this T entity, IEnumerable<LegendaryActionDescription> value)
            where T : MonsterDefinition
        {
            entity.LegendaryActionOptions.SetRange(value);
            return entity;
        }

        public static T SetLegendaryCreature<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("legendaryCreature", value);
            return entity;
        }

        public static T SetMaximalAge<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maximalAge", value);
            return entity;
        }

        public static T SetMaxLegendaryActionPoints<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maxLegendaryActionPoints", value);
            return entity;
        }

        public static T SetMaxLegendaryResistances<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maxLegendaryResistances", value);
            return entity;
        }

        public static T SetMinimalAge<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("minimalAge", value);
            return entity;
        }

        public static T SetMonsterPresentation<T>(this T entity, MonsterPresentation value)
            where T : MonsterDefinition
        {
            entity.SetField("monsterPresentation", value);
            return entity;
        }

        public static T SetNoExperienceGain<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("noExperienceGain", value);
            return entity;
        }

        public static T SetOverrideSpawnDecision<T>(this T entity, DecisionPackageDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("overrideSpawnDecision", value);
            return entity;
        }

        public static T SetPatrolStance<T>(this T entity, ActionDefinitions.MoveStance value)
            where T : MonsterDefinition
        {
            entity.SetField("patrolStance", value);
            return entity;
        }

        public static T SetSavingThrowScores<T>(this T entity, params MonsterSavingThrowProficiency[] value)
            where T : MonsterDefinition
        {
            SetSavingThrowScores(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSavingThrowScores<T>(this T entity, IEnumerable<MonsterSavingThrowProficiency> value)
            where T : MonsterDefinition
        {
            entity.SavingThrowScores.SetRange(value);
            return entity;
        }

        public static T SetSizeDefinition<T>(this T entity, CharacterSizeDefinition value)
            where T : MonsterDefinition
        {
            entity.SizeDefinition = value;
            return entity;
        }

        public static T SetSkillScores<T>(this T entity, params MonsterSkillProficiency[] value)
            where T : MonsterDefinition
        {
            SetSkillScores(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSkillScores<T>(this T entity, IEnumerable<MonsterSkillProficiency> value)
            where T : MonsterDefinition
        {
            entity.SkillScores.SetRange(value);
            return entity;
        }

        public static T SetSneakStance<T>(this T entity, ActionDefinitions.MoveStance value)
            where T : MonsterDefinition
        {
            entity.SetField("sneakStance", value);
            return entity;
        }

        public static T SetStandardHitPoints<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.StandardHitPoints = value;
            return entity;
        }

        public static T SetStealableLootDefinition<T>(this T entity, LootPackDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("stealableLootDefinition", value);
            return entity;
        }

        public static T SetThreatEvaluatorDefinition<T>(this T entity, ThreatEvaluatorDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("threatEvaluatorDefinition", value);
            return entity;
        }

        public static T SetUniqueNameId<T>(this T entity, String value)
            where T : MonsterDefinition
        {
            entity.SetField("uniqueNameId", value);
            return entity;
        }

        public static T SetUserMonster<T>(this T entity, Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("<UserMonster>k__BackingField", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}
