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
    [TargetType(typeof(EntityDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EntityDescriptionExtensions
    {
        public static T AddAdvantageTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : EntityDescription
        {
            AddAdvantageTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAdvantageTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : EntityDescription
        {
            entity.AdvantageTrends.AddRange(value);
            return entity;
        }

        public static T ClearAdvantageTrends<T>(this T entity)
            where T : EntityDescription
        {
            entity.AdvantageTrends.Clear();
            return entity;
        }

        public static EntityDescription Copy(this EntityDescription entity)
        {
            var copy = new EntityDescription();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAction<T>(this T entity, EntityDescription.DescriptionAction value)
            where T : EntityDescription
        {
            entity.Action = value;
            return entity;
        }

        public static T SetAdvantageTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : EntityDescription
        {
            SetAdvantageTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAdvantageTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : EntityDescription
        {
            entity.AdvantageTrends.SetRange(value);
            return entity;
        }

        public static T SetCover<T>(this T entity, RuleDefinitions.CoverType value)
            where T : EntityDescription
        {
            entity.Cover = value;
            return entity;
        }

        public static T SetFailure<T>(this T entity, System.String value)
            where T : EntityDescription
        {
            entity.Failure = value;
            return entity;
        }

        public static T SetHeader<T>(this T entity, System.String value)
            where T : EntityDescription
        {
            entity.Header = value;
            return entity;
        }
    }
}