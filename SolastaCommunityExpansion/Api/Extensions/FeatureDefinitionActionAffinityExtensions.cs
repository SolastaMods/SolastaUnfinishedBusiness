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
    [TargetType(typeof(FeatureDefinitionActionAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionActionAffinityExtensions
    {
        public static T AddActionExecutionModifiers<T>(this T entity,  params  ActionDefinitions . ActionExecutionModifier [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            AddActionExecutionModifiers(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddActionExecutionModifiers<T>(this T entity, IEnumerable<ActionDefinitions.ActionExecutionModifier> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ActionExecutionModifiers.AddRange(value);
            return entity;
        }

        public static T AddAuthorizedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            AddAuthorizedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAuthorizedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.AuthorizedActions.AddRange(value);
            return entity;
        }

        public static T AddForbiddenActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            AddForbiddenActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddForbiddenActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ForbiddenActions.AddRange(value);
            return entity;
        }

        public static T AddRandomBehaviourOptions<T>(this T entity,  params  BehaviorModeDescription [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            AddRandomBehaviourOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRandomBehaviourOptions<T>(this T entity, IEnumerable<BehaviorModeDescription> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RandomBehaviourOptions.AddRange(value);
            return entity;
        }

        public static T AddRestrictedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            AddRestrictedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRestrictedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RestrictedActions.AddRange(value);
            return entity;
        }

        public static T ClearActionExecutionModifiers<T>(this T entity)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ActionExecutionModifiers.Clear();
            return entity;
        }

        public static T ClearAuthorizedActions<T>(this T entity)
            where T : FeatureDefinitionActionAffinity
        {
            entity.AuthorizedActions.Clear();
            return entity;
        }

        public static T ClearForbiddenActions<T>(this T entity)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ForbiddenActions.Clear();
            return entity;
        }

        public static T ClearRandomBehaviourOptions<T>(this T entity)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RandomBehaviourOptions.Clear();
            return entity;
        }

        public static T ClearRestrictedActions<T>(this T entity)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RestrictedActions.Clear();
            return entity;
        }

        public static T SetActionExecutionModifiers<T>(this T entity,  params  ActionDefinitions . ActionExecutionModifier [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            SetActionExecutionModifiers(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetActionExecutionModifiers<T>(this T entity, IEnumerable<ActionDefinitions.ActionExecutionModifier> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ActionExecutionModifiers.SetRange(value);
            return entity;
        }

        public static T SetAllowedActionTypes<T>(this T entity, System.Boolean[] value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.AllowedActionTypes = value;
            return entity;
        }

        public static T SetAuthorizedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            SetAuthorizedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAuthorizedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.AuthorizedActions.SetRange(value);
            return entity;
        }

        public static T SetEitherMainOrBonus<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.SetField("eitherMainOrBonus", value);
            return entity;
        }

        public static T SetForbiddenActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            SetForbiddenActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetForbiddenActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.ForbiddenActions.SetRange(value);
            return entity;
        }

        public static T SetMaxAttacksNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.SetField("maxAttacksNumber", value);
            return entity;
        }

        public static T SetRandomBehaviorDie<T>(this T entity, RuleDefinitions.DieType value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.SetField("randomBehaviorDie", value);
            return entity;
        }

        public static T SetRandomBehaviourOptions<T>(this T entity,  params  BehaviorModeDescription [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            SetRandomBehaviourOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRandomBehaviourOptions<T>(this T entity, IEnumerable<BehaviorModeDescription> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RandomBehaviourOptions.SetRange(value);
            return entity;
        }

        public static T SetRestrictedActions<T>(this T entity,  params  ActionDefinitions . Id [ ]  value)
            where T : FeatureDefinitionActionAffinity
        {
            SetRestrictedActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRestrictedActions<T>(this T entity, IEnumerable<ActionDefinitions.Id> value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.RestrictedActions.SetRange(value);
            return entity;
        }

        public static T SetSpecialBehaviour<T>(this T entity, RuleDefinitions.SpecialBehaviour value)
            where T : FeatureDefinitionActionAffinity
        {
            entity.SetField("specialBehaviour", value);
            return entity;
        }
    }
}