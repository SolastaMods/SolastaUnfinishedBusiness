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
    [TargetType(typeof(GadgetBlueprint)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GadgetBlueprintExtensions
    {
        public static T AddInteractionNodes<T>(this T entity,  params  InteractionNodeDescription [ ]  value)
            where T : GadgetBlueprint
        {
            AddInteractionNodes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddInteractionNodes<T>(this T entity, IEnumerable<InteractionNodeDescription> value)
            where T : GadgetBlueprint
        {
            entity.InteractionNodes.AddRange(value);
            return entity;
        }

        public static T AddParameters<T>(this T entity,  params  GadgetParameterDescription [ ]  value)
            where T : GadgetBlueprint
        {
            AddParameters(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddParameters<T>(this T entity, IEnumerable<GadgetParameterDescription> value)
            where T : GadgetBlueprint
        {
            entity.Parameters.AddRange(value);
            return entity;
        }

        public static T ClearInteractionNodes<T>(this T entity)
            where T : GadgetBlueprint
        {
            entity.InteractionNodes.Clear();
            return entity;
        }

        public static T ClearParameters<T>(this T entity)
            where T : GadgetBlueprint
        {
            entity.Parameters.Clear();
            return entity;
        }

        public static T SetCampaignOnly<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("campaignOnly", value);
            return entity;
        }

        public static T SetCanBeActivated<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("canBeActivated", value);
            return entity;
        }

        public static T SetCustomizableDimensions<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("customizableDimensions", value);
            return entity;
        }

        public static T SetInteractionNodes<T>(this T entity,  params  InteractionNodeDescription [ ]  value)
            where T : GadgetBlueprint
        {
            SetInteractionNodes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetInteractionNodes<T>(this T entity, IEnumerable<InteractionNodeDescription> value)
            where T : GadgetBlueprint
        {
            entity.InteractionNodes.SetRange(value);
            return entity;
        }

        public static T SetMaxCustomizableDimensions<T>(this T entity, UnityEngine.Vector2Int value)
            where T : GadgetBlueprint
        {
            entity.SetField("maxCustomizableDimensions", value);
            return entity;
        }

        public static T SetMinCustomizableDimensions<T>(this T entity, UnityEngine.Vector2Int value)
            where T : GadgetBlueprint
        {
            entity.SetField("minCustomizableDimensions", value);
            return entity;
        }

        public static T SetParameters<T>(this T entity,  params  GadgetParameterDescription [ ]  value)
            where T : GadgetBlueprint
        {
            SetParameters(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetParameters<T>(this T entity, IEnumerable<GadgetParameterDescription> value)
            where T : GadgetBlueprint
        {
            entity.Parameters.SetRange(value);
            return entity;
        }
    }
}