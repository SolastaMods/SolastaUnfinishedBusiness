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
    [TargetType(typeof(TutorialTableDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TutorialTableDefinitionExtensions
    {
        public static T AddSections<T>(this T entity,  params  TutorialSectionDefinition [ ]  value)
            where T : TutorialTableDefinition
        {
            AddSections(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSections<T>(this T entity, IEnumerable<TutorialSectionDefinition> value)
            where T : TutorialTableDefinition
        {
            entity.Sections.AddRange(value);
            return entity;
        }

        public static T ClearSections<T>(this T entity)
            where T : TutorialTableDefinition
        {
            entity.Sections.Clear();
            return entity;
        }

        public static System.Collections.Generic.List<TutorialStyleDuplet> GetStyleDuplets<T>(this T entity)
            where T : TutorialTableDefinition
        {
            return entity.GetField<System.Collections.Generic.List<TutorialStyleDuplet>>("styleDuplets");
        }

        public static T SetSectionLineHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("sectionLineHeight", value);
            return entity;
        }

        public static T SetSections<T>(this T entity,  params  TutorialSectionDefinition [ ]  value)
            where T : TutorialTableDefinition
        {
            SetSections(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSections<T>(this T entity, IEnumerable<TutorialSectionDefinition> value)
            where T : TutorialTableDefinition
        {
            entity.Sections.SetRange(value);
            return entity;
        }

        public static T SetStepHeaderHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepHeaderHeight", value);
            return entity;
        }

        public static T SetStepLineHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepLineHeight", value);
            return entity;
        }

        public static T SetStepParagraphSpacing<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepParagraphSpacing", value);
            return entity;
        }

        public static T SetStepTitleHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepTitleHeight", value);
            return entity;
        }

        public static T SetStepTralingHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepTralingHeight", value);
            return entity;
        }

        public static T SetStepWordSpacing<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("stepWordSpacing", value);
            return entity;
        }

        public static T SetSubsectionIndentWidth<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("subsectionIndentWidth", value);
            return entity;
        }

        public static T SetSubsectionLineHeight<T>(this T entity, System.Single value)
            where T : TutorialTableDefinition
        {
            entity.SetField("subsectionLineHeight", value);
            return entity;
        }
    }
}