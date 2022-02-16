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
    [TargetType(typeof(LegendaryActionDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class LegendaryActionDescriptionExtensions
    {
        public static LegendaryActionDescription Copy(this LegendaryActionDescription entity)
        {
            return new LegendaryActionDescription(entity);
        }

        public static T SetCanMove<T>(this T entity, System.Boolean value)
            where T : LegendaryActionDescription
        {
            entity.SetField("canMove", value);
            return entity;
        }

        public static T SetCost<T>(this T entity, System.Int32 value)
            where T : LegendaryActionDescription
        {
            entity.SetField("cost", value);
            return entity;
        }

        public static T SetDecisionPackage<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : LegendaryActionDescription
        {
            entity.SetField("decisionPackage", value);
            return entity;
        }

        public static T SetFeatureDefinitionPower<T>(this T entity, FeatureDefinitionPower value)
            where T : LegendaryActionDescription
        {
            entity.SetField("featureDefinitionPower", value);
            return entity;
        }

        public static T SetMagicAbilityBonus<T>(this T entity, System.Int32 value)
            where T : LegendaryActionDescription
        {
            entity.SetField("magicAbilityBonus", value);
            return entity;
        }

        public static T SetMagicAttackBonus<T>(this T entity, System.Int32 value)
            where T : LegendaryActionDescription
        {
            entity.SetField("magicAttackBonus", value);
            return entity;
        }

        public static T SetMonsterAttackDefinition<T>(this T entity, MonsterAttackDefinition value)
            where T : LegendaryActionDescription
        {
            entity.SetField("monsterAttackDefinition", value);
            return entity;
        }

        public static T SetMoveMode<T>(this T entity, FeatureDefinitionMoveMode value)
            where T : LegendaryActionDescription
        {
            entity.SetField("moveMode", value);
            return entity;
        }

        public static T SetNoOpportunityAttack<T>(this T entity, System.Boolean value)
            where T : LegendaryActionDescription
        {
            entity.SetField("noOpportunityAttack", value);
            return entity;
        }

        public static T SetSaveDC<T>(this T entity, System.Int32 value)
            where T : LegendaryActionDescription
        {
            entity.SetField("saveDC", value);
            return entity;
        }

        public static T SetSpellDefinition<T>(this T entity, SpellDefinition value)
            where T : LegendaryActionDescription
        {
            entity.SetField("spellDefinition", value);
            return entity;
        }

        public static T SetSubaction<T>(this T entity, LegendaryActionDescription.SubactionType value)
            where T : LegendaryActionDescription
        {
            entity.SetField("subaction", value);
            return entity;
        }
    }
}