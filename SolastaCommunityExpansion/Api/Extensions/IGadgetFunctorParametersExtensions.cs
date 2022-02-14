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
    [TargetType(typeof(IGadgetFunctorParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class IGadgetFunctorParametersExtensions
    {
        public static T AddActingCharacters<T>(this T entity,  params  GameLocationCharacter [ ]  value)
            where T : IGadgetFunctorParameters
        {
            AddActingCharacters(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddActingCharacters<T>(this T entity, IEnumerable<GameLocationCharacter> value)
            where T : IGadgetFunctorParameters
        {
            entity.ActingCharacters.AddRange(value);
            return entity;
        }

        public static T AddFailedCharacters<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : IGadgetFunctorParameters
        {
            AddFailedCharacters(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFailedCharacters<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharacters.AddRange(value);
            return entity;
        }

        public static T AddFailedCharactersAlternate<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : IGadgetFunctorParameters
        {
            AddFailedCharactersAlternate(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFailedCharactersAlternate<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharactersAlternate.AddRange(value);
            return entity;
        }

        public static T AddMonsterSpawnInfos<T>(this T entity,  params  GadgetDefinitions . MonsterSpawnInfo [ ]  value)
            where T : IGadgetFunctorParameters
        {
            AddMonsterSpawnInfos(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMonsterSpawnInfos<T>(this T entity, IEnumerable<GadgetDefinitions.MonsterSpawnInfo> value)
            where T : IGadgetFunctorParameters
        {
            entity.MonsterSpawnInfos.AddRange(value);
            return entity;
        }

        public static T ClearActingCharacters<T>(this T entity)
            where T : IGadgetFunctorParameters
        {
            entity.ActingCharacters.Clear();
            return entity;
        }

        public static T ClearFailedCharacters<T>(this T entity)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharacters.Clear();
            return entity;
        }

        public static T ClearFailedCharactersAlternate<T>(this T entity)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharactersAlternate.Clear();
            return entity;
        }

        public static T ClearMonsterSpawnInfos<T>(this T entity)
            where T : IGadgetFunctorParameters
        {
            entity.MonsterSpawnInfos.Clear();
            return entity;
        }

        public static T SetActingCharacters<T>(this T entity,  params  GameLocationCharacter [ ]  value)
            where T : IGadgetFunctorParameters
        {
            SetActingCharacters(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetActingCharacters<T>(this T entity, IEnumerable<GameLocationCharacter> value)
            where T : IGadgetFunctorParameters
        {
            entity.ActingCharacters.SetRange(value);
            return entity;
        }

        public static T SetFailedCharacters<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : IGadgetFunctorParameters
        {
            SetFailedCharacters(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFailedCharacters<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharacters.SetRange(value);
            return entity;
        }

        public static T SetFailedCharactersAlternate<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : IGadgetFunctorParameters
        {
            SetFailedCharactersAlternate(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFailedCharactersAlternate<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : IGadgetFunctorParameters
        {
            entity.FailedCharactersAlternate.SetRange(value);
            return entity;
        }

        public static T SetFlowId<T>(this T entity, System.String value)
            where T : IGadgetFunctorParameters
        {
            entity.FlowId = value;
            return entity;
        }

        public static T SetFunctorIndex<T>(this T entity, System.Int32 value)
            where T : IGadgetFunctorParameters
        {
            entity.FunctorIndex = value;
            return entity;
        }

        public static T SetListenerIndex<T>(this T entity, System.Int32 value)
            where T : IGadgetFunctorParameters
        {
            entity.ListenerIndex = value;
            return entity;
        }

        public static T SetMonsterSpawnInfos<T>(this T entity,  params  GadgetDefinitions . MonsterSpawnInfo [ ]  value)
            where T : IGadgetFunctorParameters
        {
            SetMonsterSpawnInfos(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMonsterSpawnInfos<T>(this T entity, IEnumerable<GadgetDefinitions.MonsterSpawnInfo> value)
            where T : IGadgetFunctorParameters
        {
            entity.MonsterSpawnInfos.SetRange(value);
            return entity;
        }
    }
}