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
    [TargetType(typeof(TooltipFeatureFactionRelicParameters))]
    public static partial class TooltipFeatureFactionRelicParametersExtensions
    {
        public static System.Collections.Generic.List<GuiFactionDefinition> GetInterestedFactions<T>(this T entity)
            where T : TooltipFeatureFactionRelicParameters
        {
            return entity.GetField<System.Collections.Generic.List<GuiFactionDefinition>>("interestedFactions");
        }

        public static T SetFactionItemPrefab<T>(this T entity, UnityEngine.GameObject value)
            where T : TooltipFeatureFactionRelicParameters
        {
            entity.SetField("factionItemPrefab", value);
            return entity;
        }

        public static T SetFactionItemsTable<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureFactionRelicParameters
        {
            entity.SetField("factionItemsTable", value);
            return entity;
        }

        public static T SetVecticalLayoutGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureFactionRelicParameters
        {
            entity.SetField("vecticalLayoutGroup", value);
            return entity;
        }
    }
}