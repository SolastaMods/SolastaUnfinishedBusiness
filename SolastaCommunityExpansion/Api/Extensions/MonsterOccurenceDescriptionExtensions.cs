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
    [TargetType(typeof(MonsterOccurenceDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MonsterOccurenceDescriptionExtensions
    {
        public static T SetCreatureSex<T>(this T entity, GadgetDefinitions.CreatureSex value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("creatureSex", value);
            return entity;
        }

        public static T SetEncounterPlacementDecision<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("encounterPlacementDecision", value);
            return entity;
        }

        public static T SetFactionName<T>(this T entity, System.String value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("factionName", value);
            return entity;
        }

        public static T SetMonsterDefinition<T>(this T entity, MonsterDefinition value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("monsterDefinition", value);
            return entity;
        }

        public static T SetNumber<T>(this T entity, System.Int32 value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("number", value);
            return entity;
        }

        public static T SetOverrideFaction<T>(this T entity, System.Boolean value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("overrideFaction", value);
            return entity;
        }

        public static T SetPresentationDefinitionIndex<T>(this T entity, System.Int32 value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("presentationDefinitionIndex", value);
            return entity;
        }

        public static T SetRandomHumanoidPresentation<T>(this T entity, System.Boolean value)
            where T : MonsterOccurenceDescription
        {
            entity.SetField("randomHumanoidPresentation", value);
            return entity;
        }
    }
}