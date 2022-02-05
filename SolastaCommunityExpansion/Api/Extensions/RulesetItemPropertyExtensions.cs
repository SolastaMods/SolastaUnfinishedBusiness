using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
    [TargetType(typeof(RulesetItemProperty)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetItemPropertyExtensions
    {
        public static T SetFeatureDefinition<T>(this T entity, FeatureDefinition value)
            where T : RulesetItemProperty
        {
            entity.SetField("featureDefinition", value);
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : RulesetItemProperty
        {
            entity.Name = value;
            return entity;
        }

        public static T SetRemainingRounds<T>(this T entity, System.Int32 value)
            where T : RulesetItemProperty
        {
            entity.SetField("<RemainingRounds>k__BackingField", value);
            return entity;
        }

        public static T SetSourceEffectGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetItemProperty
        {
            entity.SourceEffectGuid = value;
            return entity;
        }

        public static T SetTargetItemGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetItemProperty
        {
            entity.SetField("targetItemGuid", value);
            return entity;
        }

        public static T SetUnicityTag<T>(this T entity, System.String value)
            where T : RulesetItemProperty
        {
            entity.SetField("unicityTag", value);
            return entity;
        }

        public static T SetUsageLimitation<T>(this T entity, RuleDefinitions.ItemPropertyUsage value)
            where T : RulesetItemProperty
        {
            entity.SetField("usageLimitation", value);
            return entity;
        }
    }
}