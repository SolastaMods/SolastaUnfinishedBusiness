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
    [TargetType(typeof(QuestStepDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class QuestStepDescriptionExtensions
    {
        public static T AddOutcomesTable<T>(this T entity,  params  QuestOutcomeDescription [ ]  value)
            where T : QuestStepDescription
        {
            AddOutcomesTable(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOutcomesTable<T>(this T entity, IEnumerable<QuestOutcomeDescription> value)
            where T : QuestStepDescription
        {
            entity.OutcomesTable.AddRange(value);
            return entity;
        }

        public static T AddParentQuestSteps<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestStepDescription
        {
            AddParentQuestSteps(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddParentQuestSteps<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestStepDescription
        {
            entity.ParentQuestSteps.AddRange(value);
            return entity;
        }

        public static T ClearOutcomesTable<T>(this T entity)
            where T : QuestStepDescription
        {
            entity.OutcomesTable.Clear();
            return entity;
        }

        public static T ClearParentQuestSteps<T>(this T entity)
            where T : QuestStepDescription
        {
            entity.ParentQuestSteps.Clear();
            return entity;
        }

        public static T SetGuiPresentation<T>(this T entity, GuiPresentation value)
            where T : QuestStepDescription
        {
            entity.SetField("guiPresentation", value);
            return entity;
        }

        public static T SetIsChoice<T>(this T entity, System.Boolean value)
            where T : QuestStepDescription
        {
            entity.IsChoice = value;
            return entity;
        }

        public static T SetIsObsolete<T>(this T entity, System.Boolean value)
            where T : QuestStepDescription
        {
            entity.IsObsolete = value;
            return entity;
        }

        public static T SetIsOptional<T>(this T entity, System.Boolean value)
            where T : QuestStepDescription
        {
            entity.IsOptional = value;
            return entity;
        }

        public static T SetLocationDefinition<T>(this T entity, LocationDefinition value)
            where T : QuestStepDescription
        {
            entity.LocationDefinition = value;
            return entity;
        }

        public static T SetMonsterDefinition<T>(this T entity, MonsterDefinition value)
            where T : QuestStepDescription
        {
            entity.MonsterDefinition = value;
            return entity;
        }

        public static T SetNeededParentStepsToStart<T>(this T entity, QuestDefinitions.QuestStepsNeed value)
            where T : QuestStepDescription
        {
            entity.NeededParentStepsToStart = value;
            return entity;
        }

        public static T SetOnStartFunctors<T>(this T entity, FunctorParametersListDefinition value)
            where T : QuestStepDescription
        {
            entity.OnStartFunctors = value;
            return entity;
        }

        public static T SetOptionalEntranceId<T>(this T entity, System.Int32 value)
            where T : QuestStepDescription
        {
            entity.OptionalEntranceId = value;
            return entity;
        }

        public static T SetOutcomesTable<T>(this T entity,  params  QuestOutcomeDescription [ ]  value)
            where T : QuestStepDescription
        {
            SetOutcomesTable(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOutcomesTable<T>(this T entity, IEnumerable<QuestOutcomeDescription> value)
            where T : QuestStepDescription
        {
            entity.OutcomesTable.SetRange(value);
            return entity;
        }

        public static T SetParentQuestSteps<T>(this T entity,  params  QuestStepDescription [ ]  value)
            where T : QuestStepDescription
        {
            SetParentQuestSteps(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetParentQuestSteps<T>(this T entity, IEnumerable<QuestStepDescription> value)
            where T : QuestStepDescription
        {
            entity.ParentQuestSteps.SetRange(value);
            return entity;
        }

        public static T SetQuestOutcome<T>(this T entity, QuestDefinitions.QuestOutcomeType value)
            where T : QuestStepDescription
        {
            entity.QuestOutcome = value;
            return entity;
        }

        public static T SetStepName<T>(this T entity, System.String value)
            where T : QuestStepDescription
        {
            entity.StepName = value;
            return entity;
        }

        public static T SetType<T>(this T entity, System.String value)
            where T : QuestStepDescription
        {
            entity.Type = value;
            return entity;
        }

        public static T SetUseEntranceId<T>(this T entity, System.Boolean value)
            where T : QuestStepDescription
        {
            entity.UseEntranceId = value;
            return entity;
        }
    }
}