using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(NarrativeStateDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class NarrativeStateDescriptionExtensions
    {
        public static T SetAbilityCheckDescription<T>(this T entity, AbilityCheckDescription value)
            where T : NarrativeStateDescription
        {
            entity.AbilityCheckDescription = value;
            return entity;
        }

        public static T SetAudioEvent<T>(this T entity, AK.Wwise.Event value)
            where T : NarrativeStateDescription
        {
            entity.AudioEvent = value;
            return entity;
        }

        public static T SetAudioTarget<T>(this T entity, System.String value)
            where T : NarrativeStateDescription
        {
            entity.AudioTarget = value;
            return entity;
        }

        public static T SetAutomaticExitLine<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.AutomaticExitLine = value;
            return entity;
        }

        public static T SetBodyAnimationType<T>(this T entity, AnimationDefinitions.SpeakType value)
            where T : NarrativeStateDescription
        {
            entity.BodyAnimationType = value;
            return entity;
        }

        public static T SetBodyAnimationVariation<T>(this T entity, System.Int32 value)
            where T : NarrativeStateDescription
        {
            entity.BodyAnimationVariation = value;
            return entity;
        }

        public static T SetCampaignDefinition<T>(this T entity, CampaignDefinition value)
            where T : NarrativeStateDescription
        {
            entity.CampaignDefinition = value;
            return entity;
        }

        public static T SetChildrenNarrativeStates<T>(this T entity, NarrativeStateDescription[] value)
            where T : NarrativeStateDescription
        {
            entity.ChildrenNarrativeStates = value;
            return entity;
        }

        public static T SetClearPreviousRoles<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.ClearPreviousRoles = value;
            return entity;
        }

        public static T SetCutAnimation<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.CutAnimation = value;
            return entity;
        }

        public static T SetDialogHeadMovement<T>(this T entity, AnimationDefinitions.DialogHeadMovement value)
            where T : NarrativeStateDescription
        {
            entity.DialogHeadMovement = value;
            return entity;
        }

        public static T SetFacialExpression<T>(this T entity, AnimationDefinitions.FacialExpression value)
            where T : NarrativeStateDescription
        {
            entity.FacialExpression = value;
            return entity;
        }

        public static T SetFactionTest<T>(this T entity, FactionTestDescription value)
            where T : NarrativeStateDescription
        {
            entity.FactionTest = value;
            return entity;
        }

        public static T SetFadeDuration<T>(this T entity, System.Single value)
            where T : NarrativeStateDescription
        {
            entity.FadeDuration = value;
            return entity;
        }

        public static T SetFadeToBlack<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.FadeToBlack = value;
            return entity;
        }

        public static T SetFixedDuration<T>(this T entity, System.Single value)
            where T : NarrativeStateDescription
        {
            entity.FixedDuration = value;
            return entity;
        }

        public static T SetFunctorParametersListDefinition<T>(this T entity, FunctorParametersListDefinition value)
            where T : NarrativeStateDescription
        {
            entity.FunctorParametersListDefinition = value;
            return entity;
        }

        public static T SetItemGainDescription<T>(this T entity, ItemGainDescription value)
            where T : NarrativeStateDescription
        {
            entity.SetField("itemGainDescription", value);
            return entity;
        }

        public static T SetItemGainOperation<T>(this T entity, ItemGainDescription value)
            where T : NarrativeStateDescription
        {
            entity.ItemGainOperation = value;
            return entity;
        }

        public static T SetLocationDefinition<T>(this T entity, LocationDefinition value)
            where T : NarrativeStateDescription
        {
            entity.LocationDefinition = value;
            return entity;
        }

        public static T SetLocationKnowLedgeLevel<T>(this T entity, GameCampaignDefinitions.NodeKnowledge value)
            where T : NarrativeStateDescription
        {
            entity.LocationKnowLedgeLevel = value;
            return entity;
        }

        public static T SetLookAtTarget<T>(this T entity, System.String value)
            where T : NarrativeStateDescription
        {
            entity.LookAtTarget = value;
            return entity;
        }

        public static T SetLookAtType<T>(this T entity, NarrativeDefinitions.LookAtType value)
            where T : NarrativeStateDescription
        {
            entity.LookAtType = value;
            return entity;
        }

        public static T SetLoopAnimation<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.LoopAnimation = value;
            return entity;
        }

        public static T SetLoreType<T>(this T entity, RuleDefinitions.LoreType value)
            where T : NarrativeStateDescription
        {
            entity.LoreType = value;
            return entity;
        }

        public static T SetMerchantDefinition<T>(this T entity, MerchantDefinition value)
            where T : NarrativeStateDescription
        {
            entity.MerchantDefinition = value;
            return entity;
        }

        public static T SetMerchantLookup<T>(this T entity, NarrativeDefinitions.MerchantLookup value)
            where T : NarrativeStateDescription
        {
            entity.MerchantLookup = value;
            return entity;
        }

        public static T SetMonsterDefinition<T>(this T entity, MonsterDefinition value)
            where T : NarrativeStateDescription
        {
            entity.MonsterDefinition = value;
            return entity;
        }

        public static T SetMonsterKnowledgeLevel<T>(this T entity, KnowledgeLevelDefinition value)
            where T : NarrativeStateDescription
        {
            entity.MonsterKnowledgeLevel = value;
            return entity;
        }

        public static T SetNeedsAudioTarget<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.NeedsAudioTarget = value;
            return entity;
        }

        public static T SetNpcGroupIndex<T>(this T entity, System.Int32 value)
            where T : NarrativeStateDescription
        {
            entity.NpcGroupIndex = value;
            return entity;
        }

        public static T SetNumberOfStepsToRewind<T>(this T entity, System.Int32 value)
            where T : NarrativeStateDescription
        {
            entity.NumberOfStepsToRewind = value;
            return entity;
        }

        public static T SetPlayerGroupIndex<T>(this T entity, System.Int32 value)
            where T : NarrativeStateDescription
        {
            entity.PlayerGroupIndex = value;
            return entity;
        }

        public static T SetPostAction<T>(this T entity, NarrativeDefinitions.PostAction value)
            where T : NarrativeStateDescription
        {
            entity.PostAction = value;
            return entity;
        }

        public static T SetQuestOperation<T>(this T entity, QuestOperationDescription value)
            where T : NarrativeStateDescription
        {
            entity.QuestOperation = value;
            return entity;
        }

        public static T SetRecipeDefinition<T>(this T entity, RecipeDefinition value)
            where T : NarrativeStateDescription
        {
            entity.RecipeDefinition = value;
            return entity;
        }

        public static T SetRole<T>(this T entity, System.String value)
            where T : NarrativeStateDescription
        {
            entity.Role = value;
            return entity;
        }

        public static T SetSelectActorsWithoutARole<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.SelectActorsWithoutARole = value;
            return entity;
        }

        public static T SetSide<T>(this T entity, RuleDefinitions.Side value)
            where T : NarrativeStateDescription
        {
            entity.Side = value;
            return entity;
        }

        public static T SetStagingDescription<T>(this T entity, DialogStagingDescription value)
            where T : NarrativeStateDescription
        {
            entity.StagingDescription = value;
            return entity;
        }

        public static T SetStartOffset<T>(this T entity, System.Single value)
            where T : NarrativeStateDescription
        {
            entity.StartOffset = value;
            return entity;
        }

        public static T SetStashOperation<T>(this T entity, NarrativeDefinitions.StashOperation value)
            where T : NarrativeStateDescription
        {
            entity.StashOperation = value;
            return entity;
        }

        public static T SetTimeline<T>(this T entity, UnityEngine.Timeline.TimelineAsset value)
            where T : NarrativeStateDescription
        {
            entity.Timeline = value;
            return entity;
        }

        public static T SetTreasuryAmount<T>(this T entity, System.Int32 value)
            where T : NarrativeStateDescription
        {
            entity.TreasuryAmount = value;
            return entity;
        }

        public static T SetType<T>(this T entity, System.String value)
            where T : NarrativeStateDescription
        {
            entity.Type = value;
            return entity;
        }

        public static T SetUseExitLine<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.UseExitLine = value;
            return entity;
        }

        public static T SetUserDialog<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.SetField("<UserDialog>k__BackingField", value);
            return entity;
        }

        public static T SetVariableName<T>(this T entity, System.String value)
            where T : NarrativeStateDescription
        {
            entity.VariableName = value;
            return entity;
        }

        public static T SetVariableOperation<T>(this T entity, VariableOperationDescription value)
            where T : NarrativeStateDescription
        {
            entity.VariableOperation = value;
            return entity;
        }

        public static T SetVariableTest<T>(this T entity, VariableTestDescription value)
            where T : NarrativeStateDescription
        {
            entity.VariableTest = value;
            return entity;
        }

        public static T SetVariableType<T>(this T entity, GameVariableDefinitions.Type value)
            where T : NarrativeStateDescription
        {
            entity.VariableType = value;
            return entity;
        }

        public static T SetVisibleExitLine<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.VisibleExitLine = value;
            return entity;
        }

        public static T SetWaitForTimelineToEnd<T>(this T entity, System.Boolean value)
            where T : NarrativeStateDescription
        {
            entity.WaitForTimelineToEnd = value;
            return entity;
        }

        public static T SetZoneTag<T>(this T entity, GraphicsDefinitions.ZoneTag value)
            where T : NarrativeStateDescription
        {
            entity.ZoneTag = value;
            return entity;
        }
    }
}