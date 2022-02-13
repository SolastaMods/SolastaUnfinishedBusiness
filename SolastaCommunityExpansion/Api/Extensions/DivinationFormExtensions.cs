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
    [TargetType(typeof(DivinationForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DivinationFormExtensions
    {
        public static T AddCreatureFamilies<T>(this T entity,  params  CharacterFamilyDefinition [ ]  value)
            where T : DivinationForm
        {
            AddCreatureFamilies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddCreatureFamilies<T>(this T entity, IEnumerable<CharacterFamilyDefinition> value)
            where T : DivinationForm
        {
            entity.CreatureFamilies.AddRange(value);
            return entity;
        }

        public static T AddRevealedTags<T>(this T entity,  params  System . String [ ]  value)
            where T : DivinationForm
        {
            AddRevealedTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRevealedTags<T>(this T entity, IEnumerable<System.String> value)
            where T : DivinationForm
        {
            entity.RevealedTags.AddRange(value);
            return entity;
        }

        public static T ClearCreatureFamilies<T>(this T entity)
            where T : DivinationForm
        {
            entity.CreatureFamilies.Clear();
            return entity;
        }

        public static T ClearRevealedTags<T>(this T entity)
            where T : DivinationForm
        {
            entity.RevealedTags.Clear();
            return entity;
        }

        public static DivinationForm Copy(this DivinationForm entity)
        {
            var copy = new DivinationForm();
            copy.Copy(entity);
            return entity;
        }

        public static T SetCreatureFamilies<T>(this T entity,  params  CharacterFamilyDefinition [ ]  value)
            where T : DivinationForm
        {
            SetCreatureFamilies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetCreatureFamilies<T>(this T entity, IEnumerable<CharacterFamilyDefinition> value)
            where T : DivinationForm
        {
            entity.CreatureFamilies.SetRange(value);
            return entity;
        }

        public static T SetDivinationType<T>(this T entity, DivinationForm.Type value)
            where T : DivinationForm
        {
            entity.SetField("divinationType", value);
            return entity;
        }

        public static T SetRangeCells<T>(this T entity, System.Int32 value)
            where T : DivinationForm
        {
            entity.SetField("rangeCells", value);
            return entity;
        }

        public static T SetRevealedTags<T>(this T entity,  params  System . String [ ]  value)
            where T : DivinationForm
        {
            SetRevealedTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRevealedTags<T>(this T entity, IEnumerable<System.String> value)
            where T : DivinationForm
        {
            entity.RevealedTags.SetRange(value);
            return entity;
        }
    }
}