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
    [TargetType(typeof(CharacterSubclassDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterSubclassDefinitionExtensions
    {
        public static T AddFeatureUnlocks<T>(this T entity,  params  FeatureUnlockByLevel [ ]  value)
            where T : CharacterSubclassDefinition
        {
            AddFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterSubclassDefinition
        {
            entity.FeatureUnlocks.AddRange(value);
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterSubclassDefinition
        {
            AddPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterSubclassDefinition
        {
            entity.PersonalityFlagOccurences.AddRange(value);
            return entity;
        }

        public static T ClearFeatureUnlocks<T>(this T entity)
            where T : CharacterSubclassDefinition
        {
            entity.FeatureUnlocks.Clear();
            return entity;
        }

        public static T ClearPersonalityFlagOccurences<T>(this T entity)
            where T : CharacterSubclassDefinition
        {
            entity.PersonalityFlagOccurences.Clear();
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity,  params  FeatureUnlockByLevel [ ]  value)
            where T : CharacterSubclassDefinition
        {
            SetFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterSubclassDefinition
        {
            entity.FeatureUnlocks.SetRange(value);
            return entity;
        }

        public static T SetMorphotypeSubclassFilterTag<T>(this T entity, GraphicsDefinitions.MorphotypeSubclassFilterTag value)
            where T : CharacterSubclassDefinition
        {
            entity.SetField("morphotypeSubclassFilterTag", value);
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterSubclassDefinition
        {
            SetPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterSubclassDefinition
        {
            entity.PersonalityFlagOccurences.SetRange(value);
            return entity;
        }
    }
}