using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(MonsterDefinition))]
    public static partial class MonsterDefinitionExtensions
    {
        public static T SetAbilityScores<T>(this T entity, System.Int32[] value)
            where T : MonsterDefinition
        {
            entity.SetField("abilityScores", value);
            return entity;
        }

        public static T SetAlignment<T>(this T entity, System.String value)
            where T : MonsterDefinition
        {
            entity.SetField("alignment", value);
            return entity;
        }

        public static T SetAlwaysHideStats<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("alwaysHideStats", value);
            return entity;
        }

        public static T SetArmor<T>(this T entity, System.String value)
            where T : MonsterDefinition
        {
            entity.SetField("armor", value);
            return entity;
        }

        public static T SetArmorClass<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.ArmorClass = value;
            return entity;
        }

        public static T SetAudioRaceRTPCValue<T>(this T entity, System.Single value)
            where T : MonsterDefinition
        {
            entity.SetField("audioRaceRTPCValue", value);
            return entity;
        }

        public static T SetBestiaryCameraOffset<T>(this T entity, UnityEngine.Vector3 value)
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

        public static T SetBestiaryReference<T>(this T entity, MonsterDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiaryReference", value);
            return entity;
        }

        public static T SetBestiarySpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : MonsterDefinition
        {
            entity.SetField("bestiarySpriteReference", value);
            return entity;
        }

        public static T SetChallengeRating<T>(this T entity, System.Single value)
            where T : MonsterDefinition
        {
            entity.ChallengeRating = value;
            return entity;
        }

        public static T SetCharacterFamily<T>(this T entity, System.String value)
            where T : MonsterDefinition
        {
            entity.SetField("characterFamily", value);
            return entity;
        }

        public static T SetDefaultBattleDecisionPackage<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("defaultBattleDecisionPackage", value);
            return entity;
        }

        public static T SetDefaultFaction<T>(this T entity, System.String value)
            where T : MonsterDefinition
        {
            entity.SetField("defaultFaction", value);
            return entity;
        }

        public static T SetDifferentActionEachTurn<T>(this T entity, System.Boolean value)
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

        public static T SetDualSex<T>(this T entity, System.Boolean value)
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

        public static T SetFollowFloorAngle<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("followFloorAngle", value);
            return entity;
        }

        public static T SetForceCombatStartsAnimation<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forceCombatStartsAnimation", value);
            return entity;
        }

        public static T SetForceNoFlyAnimation<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forceNoFlyAnimation", value);
            return entity;
        }

        public static T SetForcePersistentBody<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("forcePersistentBody", value);
            return entity;
        }

        public static T SetFullyControlledWhenAllied<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("fullyControlledWhenAllied", value);
            return entity;
        }

        public static T SetGroupAttacks<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("groupAttacks", value);
            return entity;
        }

        public static T SetHasLookAt<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("hasLookAt", value);
            return entity;
        }

        public static T SetHeight<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("height", value);
            return entity;
        }

        public static T SetHitDice<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("hitDice", value);
            return entity;
        }

        public static T SetHitDiceType<T>(this T entity, RuleDefinitions.DieType value)
            where T : MonsterDefinition
        {
            entity.SetField("hitDiceType", value);
            return entity;
        }

        public static T SetHitPointsBonus<T>(this T entity, System.Int32 value)
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

        public static T SetIsHusk<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("isHusk", value);
            return entity;
        }

        public static T SetIsPet<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("isPet", value);
            return entity;
        }

        public static T SetIsUnique<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("isUnique", value);
            return entity;
        }

        public static T SetLegendaryCreature<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("legendaryCreature", value);
            return entity;
        }

        public static T SetMaximalAge<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maximalAge", value);
            return entity;
        }

        public static T SetMaxLegendaryActionPoints<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maxLegendaryActionPoints", value);
            return entity;
        }

        public static T SetMaxLegendaryResistances<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("maxLegendaryResistances", value);
            return entity;
        }

        public static T SetMinimalAge<T>(this T entity, System.Int32 value)
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

        public static T SetNoExperienceGain<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("noExperienceGain", value);
            return entity;
        }

        public static T SetPatrolStance<T>(this T entity, ActionDefinitions.MoveStance value)
            where T : MonsterDefinition
        {
            entity.SetField("patrolStance", value);
            return entity;
        }

        public static T SetSizeDefinition<T>(this T entity, CharacterSizeDefinition value)
            where T : MonsterDefinition
        {
            entity.SizeDefinition = value;
            return entity;
        }

        public static T SetSneakStance<T>(this T entity, ActionDefinitions.MoveStance value)
            where T : MonsterDefinition
        {
            entity.SetField("sneakStance", value);
            return entity;
        }

        public static T SetStandardHitPoints<T>(this T entity, System.Int32 value)
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

        public static T SetThreatEvaluatorDefinition<T>(this T entity, TA.AI.ThreatEvaluatorDefinition value)
            where T : MonsterDefinition
        {
            entity.SetField("threatEvaluatorDefinition", value);
            return entity;
        }

        public static T SetUniqueNameId<T>(this T entity, System.String value)
            where T : MonsterDefinition
        {
            entity.SetField("uniqueNameId", value);
            return entity;
        }

        public static T SetUserMonster<T>(this T entity, System.Boolean value)
            where T : MonsterDefinition
        {
            entity.SetField("<UserMonster>k__BackingField", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, System.Int32 value)
            where T : MonsterDefinition
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}