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
    [TargetType(typeof(QuestTreeDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class QuestTreeDefinitionExtensions
    {
        public static T AddAllQuestStepDescriptions<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestTreeDefinition
        {
            AddAllQuestStepDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAllQuestStepDescriptions<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestTreeDefinition
        {
            entity.AllQuestStepDescriptions.AddRange(value);
            return entity;
        }

        public static T ClearAllQuestStepDescriptions<T>(this T entity)
            where T : QuestTreeDefinition
        {
            entity.AllQuestStepDescriptions.Clear();
            return entity;
        }

        public static T SetAllQuestStepDescriptions<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestTreeDefinition
        {
            SetAllQuestStepDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAllQuestStepDescriptions<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestTreeDefinition
        {
            entity.AllQuestStepDescriptions.SetRange(value);
            return entity;
        }

        public static T SetCategory<T>(this T entity, QuestDefinitions.QuestCategory value)
            where T : QuestTreeDefinition
        {
            entity.Category = value;
            return entity;
        }

        public static T SetFactionDefinition<T>(this T entity, FactionDefinition value)
            where T : QuestTreeDefinition
        {
            entity.FactionDefinition = value;
            return entity;
        }

        public static T SetIsUserQuest<T>(this T entity, System.Boolean value)
            where T : QuestTreeDefinition
        {
            entity.SetField("<IsUserQuest>k__BackingField", value);
            return entity;
        }

        public static T SetSerializeVersion<T>(this T entity, System.Int32 value)
            where T : QuestTreeDefinition
        {
            entity.SetField("serializeVersion", value);
            return entity;
        }
    }
}