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
    [TargetType(typeof(CharacterHeroBuildingData)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterHeroBuildingDataExtensions
    {
        public static T AddAllActiveFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            AddAllActiveFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAllActiveFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.AllActiveFeatures.AddRange(value);
            return entity;
        }

        public static T AddGrantedItemsAfterRandom<T>(this T entity,  params  ItemDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            AddGrantedItemsAfterRandom(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddGrantedItemsAfterRandom<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.GrantedItemsAfterRandom.AddRange(value);
            return entity;
        }

        public static T AddMatchingFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            AddMatchingFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMatchingFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.MatchingFeatures.AddRange(value);
            return entity;
        }

        public static T AddStringCache<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterHeroBuildingData
        {
            AddStringCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStringCache<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterHeroBuildingData
        {
            entity.StringCache.AddRange(value);
            return entity;
        }

        public static T ClearAllActiveFeatures<T>(this T entity)
            where T : CharacterHeroBuildingData
        {
            entity.AllActiveFeatures.Clear();
            return entity;
        }

        public static T ClearGrantedItemsAfterRandom<T>(this T entity)
            where T : CharacterHeroBuildingData
        {
            entity.GrantedItemsAfterRandom.Clear();
            return entity;
        }

        public static T ClearMatchingFeatures<T>(this T entity)
            where T : CharacterHeroBuildingData
        {
            entity.MatchingFeatures.Clear();
            return entity;
        }

        public static T ClearStringCache<T>(this T entity)
            where T : CharacterHeroBuildingData
        {
            entity.StringCache.Clear();
            return entity;
        }

        public static T SetAllActiveFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            SetAllActiveFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAllActiveFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.AllActiveFeatures.SetRange(value);
            return entity;
        }

        public static T SetGrantedContainer<T>(this T entity, ItemDefinition value)
            where T : CharacterHeroBuildingData
        {
            entity.GrantedContainer = value;
            return entity;
        }

        public static T SetGrantedItemsAfterRandom<T>(this T entity,  params  ItemDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            SetGrantedItemsAfterRandom(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetGrantedItemsAfterRandom<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.GrantedItemsAfterRandom.SetRange(value);
            return entity;
        }

        public static T SetHeroCharacter<T>(this T entity, RulesetCharacterHero value)
            where T : CharacterHeroBuildingData
        {
            entity.SetField("heroCharacter", value);
            return entity;
        }

        public static T SetLevelingUp<T>(this T entity, System.Boolean value)
            where T : CharacterHeroBuildingData
        {
            entity.LevelingUp = value;
            return entity;
        }

        public static T SetMatchingFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterHeroBuildingData
        {
            SetMatchingFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMatchingFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterHeroBuildingData
        {
            entity.MatchingFeatures.SetRange(value);
            return entity;
        }

        public static T SetSpellComparer<T>(this T entity, SpellLearnPriorityComparer value)
            where T : CharacterHeroBuildingData
        {
            entity.SetField("spellComparer", value);
            return entity;
        }

        public static T SetStartingStylesCount<T>(this T entity, System.Int32 value)
            where T : CharacterHeroBuildingData
        {
            entity.StartingStylesCount = value;
            return entity;
        }

        public static T SetStringCache<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterHeroBuildingData
        {
            SetStringCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStringCache<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterHeroBuildingData
        {
            entity.StringCache.SetRange(value);
            return entity;
        }

        public static T SetTempAcquiredCantripsNumber<T>(this T entity, System.Int32 value)
            where T : CharacterHeroBuildingData
        {
            entity.TempAcquiredCantripsNumber = value;
            return entity;
        }

        public static T SetTempAcquiredSpellsNumber<T>(this T entity, System.Int32 value)
            where T : CharacterHeroBuildingData
        {
            entity.TempAcquiredSpellsNumber = value;
            return entity;
        }

        public static T SetTempUnlearnedSpellsNumber<T>(this T entity, System.Int32 value)
            where T : CharacterHeroBuildingData
        {
            entity.TempUnlearnedSpellsNumber = value;
            return entity;
        }
    }
}