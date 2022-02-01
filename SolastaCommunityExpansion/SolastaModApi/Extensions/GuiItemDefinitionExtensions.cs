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
    [TargetType(typeof(GuiItemDefinition))]
    public static partial class GuiItemDefinitionExtensions
    {
        public static System.Collections.Generic.Dictionary<System.String, TagsDefinitions.Criticity> GetItemTags<T>(this T entity)
            where T : GuiItemDefinition
        {
            return entity.GetField<System.Collections.Generic.Dictionary<System.String, TagsDefinitions.Criticity>>("itemTags");
        }

        public static System.Collections.Generic.List<ItemPropertyDescription> GetProperties<T>(this T entity)
            where T : GuiItemDefinition
        {
            return entity.GetField<System.Collections.Generic.List<ItemPropertyDescription>>("properties");
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : GuiItemDefinition
        {
            entity.SetField("<ItemDefinition>k__BackingField", value);
            return entity;
        }
    }
}