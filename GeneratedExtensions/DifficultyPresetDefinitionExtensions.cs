using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
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
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DifficultyPresetDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DifficultyPresetDefinitionExtensions
    {
        public static T SetAbilityCheckAllyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("abilityCheckAllyModifier", value);
            return entity;
        }

        public static T SetAbilityCheckEnemyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("abilityCheckEnemyModifier", value);
            return entity;
        }

        public static T SetAiTargetsHelplessCharacters<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("aiTargetsHelplessCharacters", value);
            return entity;
        }

        public static T SetAiUsesPowerfulMovesMoreOften<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("aiUsesPowerfulMovesMoreOften", value);
            return entity;
        }

        public static T SetAllowAttuningUnknownItems<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("allowAttuningUnknownItems", value);
            return entity;
        }

        public static T SetAlwaysDisplayDialogChances<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("alwaysDisplayDialogChances", value);
            return entity;
        }

        public static T SetAttackRollAllyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("attackRollAllyModifier", value);
            return entity;
        }

        public static T SetAttackRollEnemyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("attackRollEnemyModifier", value);
            return entity;
        }

        public static T SetAuthorizeRetryOnGadgets<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("authorizeRetryOnGadgets", value);
            return entity;
        }

        public static T SetAutoDetectTraps<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("autoDetectTraps", value);
            return entity;
        }

        public static T SetAutorevive<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("autorevive", value);
            return entity;
        }

        public static T SetCompanionsRest<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("companionsRest", value);
            return entity;
        }

        public static T SetDamageTakenAllyMultiplier<T>(this T entity, System.Single value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("damageTakenAllyMultiplier", value);
            return entity;
        }

        public static T SetDisableEnemyCrits<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("disableEnemyCrits", value);
            return entity;
        }

        public static T SetDisableRandomEncounters<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("disableRandomEncounters", value);
            return entity;
        }

        public static T SetEncumbranceRuleType<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("encumbranceRuleType", value);
            return entity;
        }

        public static T SetEnemyHpMultiplier<T>(this T entity, System.Single value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("enemyHpMultiplier", value);
            return entity;
        }

        public static T SetForceCraftingRollSuccess<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("forceCraftingRollSuccess", value);
            return entity;
        }

        public static T SetForceCritEveryFewRolls<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("forceCritEveryFewRolls", value);
            return entity;
        }

        public static T SetForceSuccessOnDialogRolls<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("forceSuccessOnDialogRolls", value);
            return entity;
        }

        public static T SetIsDefaultPreset<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("isDefaultPreset", value);
            return entity;
        }

        public static T SetMaterialComponent<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("materialComponent", value);
            return entity;
        }

        public static T SetMaxHpOnHitDice<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("maxHpOnHitDice", value);
            return entity;
        }

        public static T SetMaxHpOnLevelUp<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("maxHpOnLevelUp", value);
            return entity;
        }

        public static T SetNeverLoseConcentrationOnSpells<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("neverLoseConcentrationOnSpells", value);
            return entity;
        }

        public static T SetNoFoodNeeded<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("noFoodNeeded", value);
            return entity;
        }

        public static T SetNoToolRequirement<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("noToolRequirement", value);
            return entity;
        }

        public static T SetPresetName<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("presetName", value);
            return entity;
        }

        public static T SetRandomnessMode<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("randomnessMode", value);
            return entity;
        }

        public static T SetSavingThrowAllyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("savingThrowAllyModifier", value);
            return entity;
        }

        public static T SetSavingThrowEnemyModifier<T>(this T entity, System.Int32 value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("savingThrowEnemyModifier", value);
            return entity;
        }

        public static T SetScrollsCanBeUsedByAnyCharacter<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("scrollsCanBeUsedByAnyCharacter", value);
            return entity;
        }

        public static T SetSomaticComponent<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("somaticComponent", value);
            return entity;
        }

        public static T SetSpellbookPagesConstraint<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("spellbookPagesConstraint", value);
            return entity;
        }

        public static T SetUnlockAllBestiaryContent<T>(this T entity, System.Boolean value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("unlockAllBestiaryContent", value);
            return entity;
        }

        public static T SetVerbalComponent<T>(this T entity, System.String value)
            where T : DifficultyPresetDefinition
        {
            entity.SetField("verbalComponent", value);
            return entity;
        }
    }
}