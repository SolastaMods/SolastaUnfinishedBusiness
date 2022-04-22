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
    [TargetType(typeof(GuiCharacterAction)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GuiCharacterActionExtensions
    {
        public static System.Collections.Generic.List<RulesetItemDevice> GetAvailableDevices<T>(this T entity)
            where T : GuiCharacterAction
        {
            return entity.GetField<System.Collections.Generic.List<RulesetItemDevice>>("availableDevices");
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetFeatures<T>(this T entity)
            where T : GuiCharacterAction
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("features");
        }

        public static T SetActingCharacter<T>(this T entity, GameLocationCharacter value)
            where T : GuiCharacterAction
        {
            entity.SetField("actingCharacter", value);
            return entity;
        }

        public static T SetActionDefinition<T>(this T entity, ActionDefinition value)
            where T : GuiCharacterAction
        {
            entity.SetField("actionDefinition", value);
            return entity;
        }

        public static T SetActionId<T>(this T entity, ActionDefinitions.Id value)
            where T : GuiCharacterAction
        {
            entity.SetField("actionId", value);
            return entity;
        }

        public static T SetActionScope<T>(this T entity, ActionDefinitions.ActionScope value)
            where T : GuiCharacterAction
        {
            entity.SetField("actionScope", value);
            return entity;
        }

        public static T SetActionTypeDefinition<T>(this T entity, ActionTypeDefinition value)
            where T : GuiCharacterAction
        {
            entity.SetField("actionTypeDefinition", value);
            return entity;
        }

        public static T SetBaseDescription<T>(this T entity, System.String value)
            where T : GuiCharacterAction
        {
            entity.SetField("baseDescription", value);
            return entity;
        }

        public static T SetBaseTitle<T>(this T entity, System.String value)
            where T : GuiCharacterAction
        {
            entity.SetField("baseTitle", value);
            return entity;
        }

        public static T SetLastActionStatus<T>(this T entity, ActionDefinitions.ActionStatus value)
            where T : GuiCharacterAction
        {
            entity.SetField("lastActionStatus", value);
            return entity;
        }

        public static T SetLastAttackMode<T>(this T entity, RulesetAttackMode value)
            where T : GuiCharacterAction
        {
            entity.SetField("lastAttackMode", value);
            return entity;
        }

        public static T SetLastEffectFormCount<T>(this T entity, System.Int32 value)
            where T : GuiCharacterAction
        {
            entity.SetField("lastEffectFormCount", value);
            return entity;
        }

        public static T SetLastFailureString<T>(this T entity, System.String value)
            where T : GuiCharacterAction
        {
            entity.SetField("lastFailureString", value);
            return entity;
        }

        public static T SetLastTooltip<T>(this T entity, System.String value)
            where T : GuiCharacterAction
        {
            entity.SetField("lastTooltip", value);
            return entity;
        }
    }
}