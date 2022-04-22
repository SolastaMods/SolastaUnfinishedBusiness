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
    [TargetType(typeof(QuestOutcomeDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class QuestOutcomeDescriptionExtensions
    {
        public static T AddChildrenQuestSteps<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestOutcomeDescription
        {
            AddChildrenQuestSteps(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddChildrenQuestSteps<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestOutcomeDescription
        {
            entity.ChildrenQuestSteps.AddRange(value);
            return entity;
        }

        public static T ClearChildrenQuestSteps<T>(this T entity)
            where T : QuestOutcomeDescription
        {
            entity.ChildrenQuestSteps.Clear();
            return entity;
        }

        public static T SetChildrenQuestSteps<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestOutcomeDescription
        {
            SetChildrenQuestSteps(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetChildrenQuestSteps<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestOutcomeDescription
        {
            entity.ChildrenQuestSteps.SetRange(value);
            return entity;
        }

        public static T SetDescriptionText<T>(this T entity, System.String value)
            where T : QuestOutcomeDescription
        {
            entity.DescriptionText = value;
            return entity;
        }

        public static T SetOnCompleteFunctors<T>(this T entity, FunctorParametersListDefinition value)
            where T : QuestOutcomeDescription
        {
            entity.OnCompleteFunctors = value;
            return entity;
        }

        public static T SetOutcomeType<T>(this T entity, QuestDefinitions.QuestOutcomeType value)
            where T : QuestOutcomeDescription
        {
            entity.OutcomeType = value;
            return entity;
        }

        public static T SetValidatorDescription<T>(this T entity, QuestValidatorDescription value)
            where T : QuestOutcomeDescription
        {
            entity.ValidatorDescription = value;
            return entity;
        }
    }
}