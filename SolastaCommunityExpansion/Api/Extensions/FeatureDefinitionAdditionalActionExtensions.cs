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
    [TargetType(typeof(FeatureDefinitionAdditionalAction)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionAdditionalActionExtensions
    {
        public static T AddAuthorizedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            AddAuthorizedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAuthorizedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.AuthorizedActions.AddRange(value);
            return entity;
        }

        public static T AddForbiddenActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            AddForbiddenActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddForbiddenActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.ForbiddenActions.AddRange(value);
            return entity;
        }

        public static T AddRestrictedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            AddRestrictedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRestrictedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.RestrictedActions.AddRange(value);
            return entity;
        }

        public static T ClearAuthorizedActions<T>(this T entity)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.AuthorizedActions.Clear();
            return entity;
        }

        public static T ClearForbiddenActions<T>(this T entity)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.ForbiddenActions.Clear();
            return entity;
        }

        public static T ClearRestrictedActions<T>(this T entity)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.RestrictedActions.Clear();
            return entity;
        }

        public static T SetActionType<T>(this T entity, ActionDefinitions.ActionType value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("actionType", value);
            return entity;
        }

        public static T SetAuthorizedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            SetAuthorizedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAuthorizedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.AuthorizedActions.SetRange(value);
            return entity;
        }

        public static T SetForbiddenActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            SetForbiddenActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetForbiddenActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.ForbiddenActions.SetRange(value);
            return entity;
        }

        public static T SetMaxAttacksNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("maxAttacksNumber", value);
            return entity;
        }

        public static T SetRestrictedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionAdditionalAction
        {
            SetRestrictedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRestrictedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.RestrictedActions.SetRange(value);
            return entity;
        }

        public static T SetTriggerCondition<T>(this T entity, RuleDefinitions.AdditionalActionTriggerCondition value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("triggerCondition", value);
            return entity;
        }
    }
}