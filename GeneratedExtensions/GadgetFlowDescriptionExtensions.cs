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
    [TargetType(typeof(GadgetFlowDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GadgetFlowDescriptionExtensions
    {
        public static T AddBoxColliders<T>(this T entity,  params  UnityEngine . BoxCollider [ ]  value)
            where T : GadgetFlowDescription
        {
            AddBoxColliders(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddBoxColliders<T>(this T entity, IEnumerable<UnityEngine.BoxCollider> value)
            where T : GadgetFlowDescription
        {
            entity.BoxColliders.AddRange(value);
            return entity;
        }

        public static T AddDamageTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : GadgetFlowDescription
        {
            AddDamageTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDamageTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : GadgetFlowDescription
        {
            entity.DamageTypes.AddRange(value);
            return entity;
        }

        public static T AddExpectedConditionStates<T>(this T entity,  params  GadgetDefinitions . ConditionConstrainedState [ ]  value)
            where T : GadgetFlowDescription
        {
            AddExpectedConditionStates(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddExpectedConditionStates<T>(this T entity, IEnumerable<GadgetDefinitions.ConditionConstrainedState> value)
            where T : GadgetFlowDescription
        {
            entity.ExpectedConditionStates.AddRange(value);
            return entity;
        }

        public static T AddFunctorDescriptions<T>(this T entity,  params  FunctorParametersDescription [ ]  value)
            where T : GadgetFlowDescription
        {
            AddFunctorDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFunctorDescriptions<T>(this T entity, IEnumerable<FunctorParametersDescription> value)
            where T : GadgetFlowDescription
        {
            entity.FunctorDescriptions.AddRange(value);
            return entity;
        }

        public static T AddNodes<T>(this T entity,  params  WorldNode [ ]  value)
            where T : GadgetFlowDescription
        {
            AddNodes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddNodes<T>(this T entity, IEnumerable<WorldNode> value)
            where T : GadgetFlowDescription
        {
            entity.Nodes.AddRange(value);
            return entity;
        }

        public static T ClearBoxColliders<T>(this T entity)
            where T : GadgetFlowDescription
        {
            entity.BoxColliders.Clear();
            return entity;
        }

        public static T ClearDamageTypes<T>(this T entity)
            where T : GadgetFlowDescription
        {
            entity.DamageTypes.Clear();
            return entity;
        }

        public static T ClearExpectedConditionStates<T>(this T entity)
            where T : GadgetFlowDescription
        {
            entity.ExpectedConditionStates.Clear();
            return entity;
        }

        public static T ClearFunctorDescriptions<T>(this T entity)
            where T : GadgetFlowDescription
        {
            entity.FunctorDescriptions.Clear();
            return entity;
        }

        public static T ClearNodes<T>(this T entity)
            where T : GadgetFlowDescription
        {
            entity.Nodes.Clear();
            return entity;
        }

        public static T SetAlterationType<T>(this T entity, AlterationForm.Type value)
            where T : GadgetFlowDescription
        {
            entity.SetField("alterationType", value);
            return entity;
        }

        public static T SetBattleEventCounter<T>(this T entity, System.Int32 value)
            where T : GadgetFlowDescription
        {
            entity.SetField("battleEventCounter", value);
            return entity;
        }

        public static T SetBattleEventMaxOccurences<T>(this T entity, System.Int32 value)
            where T : GadgetFlowDescription
        {
            entity.SetField("battleEventMaxOccurences", value);
            return entity;
        }

        public static T SetBoolParameter1<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("boolParameter1", value);
            return entity;
        }

        public static T SetBoxColliders<T>(this T entity,  params  UnityEngine . BoxCollider [ ]  value)
            where T : GadgetFlowDescription
        {
            SetBoxColliders(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetBoxColliders<T>(this T entity, IEnumerable<UnityEngine.BoxCollider> value)
            where T : GadgetFlowDescription
        {
            entity.BoxColliders.SetRange(value);
            return entity;
        }

        public static T SetCharacterInteractionDefinition<T>(this T entity, CharacterInteractionDefinition value)
            where T : GadgetFlowDescription
        {
            entity.SetField("characterInteractionDefinition", value);
            return entity;
        }

        public static T SetDamageType<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SetField("damageType", value);
            return entity;
        }

        public static T SetDamageTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : GadgetFlowDescription
        {
            SetDamageTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDamageTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : GadgetFlowDescription
        {
            entity.DamageTypes.SetRange(value);
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SetField("description", value);
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : GadgetFlowDescription
        {
            entity.DurationType = value;
            return entity;
        }

        public static T SetExpectedConditionStates<T>(this T entity,  params  GadgetDefinitions . ConditionConstrainedState [ ]  value)
            where T : GadgetFlowDescription
        {
            SetExpectedConditionStates(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetExpectedConditionStates<T>(this T entity, IEnumerable<GadgetDefinitions.ConditionConstrainedState> value)
            where T : GadgetFlowDescription
        {
            entity.ExpectedConditionStates.SetRange(value);
            return entity;
        }

        public static T SetFunctorDescriptions<T>(this T entity,  params  FunctorParametersDescription [ ]  value)
            where T : GadgetFlowDescription
        {
            SetFunctorDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFunctorDescriptions<T>(this T entity, IEnumerable<FunctorParametersDescription> value)
            where T : GadgetFlowDescription
        {
            entity.FunctorDescriptions.SetRange(value);
            return entity;
        }

        public static T SetId<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SetField("id", value);
            return entity;
        }

        public static T SetInteractionHighlight<T>(this T entity, UnityEngine.GameObject value)
            where T : GadgetFlowDescription
        {
            entity.SetField("interactionHighlight", value);
            return entity;
        }

        public static T SetInteractor<T>(this T entity, WorldManipulationInteractor value)
            where T : GadgetFlowDescription
        {
            entity.Interactor = value;
            return entity;
        }

        public static T SetInteractWithBoundCharacters<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("interactWithBoundCharacters", value);
            return entity;
        }

        public static T SetIntParameter1<T>(this T entity, System.Int32 value)
            where T : GadgetFlowDescription
        {
            entity.IntParameter1 = value;
            return entity;
        }

        public static T SetListenerType<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SetField("listenerType", value);
            return entity;
        }

        public static T SetMinPerceptionRange<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("minPerceptionRange", value);
            return entity;
        }

        public static T SetNodes<T>(this T entity,  params  WorldNode [ ]  value)
            where T : GadgetFlowDescription
        {
            SetNodes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetNodes<T>(this T entity, IEnumerable<WorldNode> value)
            where T : GadgetFlowDescription
        {
            entity.Nodes.SetRange(value);
            return entity;
        }

        public static T SetOrientation<T>(this T entity, LocationDefinitions.Orientation value)
            where T : GadgetFlowDescription
        {
            entity.SetField("orientation", value);
            return entity;
        }

        public static T SetOutcomeNumber<T>(this T entity, System.Int32 value)
            where T : GadgetFlowDescription
        {
            entity.OutcomeNumber = value;
            return entity;
        }

        public static T SetPosition<T>(this T entity, TA.int3 value)
            where T : GadgetFlowDescription
        {
            entity.SetField("position", value);
            return entity;
        }

        public static T SetQuestName<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.QuestName = value;
            return entity;
        }

        public static T SetRadius<T>(this T entity, System.Single value)
            where T : GadgetFlowDescription
        {
            entity.SetField("radius", value);
            return entity;
        }

        public static T SetReactToLocationEntered<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("reactToLocationEntered", value);
            return entity;
        }

        public static T SetRevealedTag<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SetField("revealedTag", value);
            return entity;
        }

        public static T SetSortAbilityScoreName<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SortAbilityScoreName = value;
            return entity;
        }

        public static T SetSortByAbilityScore<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("sortByAbilityScore", value);
            return entity;
        }

        public static T SetSortProficiencyName<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.SortProficiencyName = value;
            return entity;
        }

        public static T SetSpecificEntrance<T>(this T entity, System.Boolean value)
            where T : GadgetFlowDescription
        {
            entity.SetField("specificEntrance", value);
            return entity;
        }

        public static T SetStepName<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.StepName = value;
            return entity;
        }

        public static T SetToggledCondition<T>(this T entity, GadgetDefinitions.ConditionConstrainedState value)
            where T : GadgetFlowDescription
        {
            entity.SetField("toggledCondition", value);
            return entity;
        }

        public static T SetVariableInteractionDefinitionParameter1<T>(this T entity, GadgetDefinitions.GadgetVariableScriptableObjectInteractionDefinition value)
            where T : GadgetFlowDescription
        {
            entity.SetField("variableInteractionDefinitionParameter1", value);
            return entity;
        }

        public static T SetVariableIntParameter1<T>(this T entity, GadgetDefinitions.GadgetVariableInt value)
            where T : GadgetFlowDescription
        {
            entity.SetField("variableIntParameter1", value);
            return entity;
        }

        public static T SetVariableName<T>(this T entity, System.String value)
            where T : GadgetFlowDescription
        {
            entity.VariableName = value;
            return entity;
        }

        public static T SetVariableTestDescription<T>(this T entity, VariableTestDescription value)
            where T : GadgetFlowDescription
        {
            entity.VariableTestDescription = value;
            return entity;
        }

        public static T SetVariableType<T>(this T entity, GameVariableDefinitions.Type value)
            where T : GadgetFlowDescription
        {
            entity.VariableType = value;
            return entity;
        }

        public static T SetVolumetricTriggerMode<T>(this T entity, GadgetDefinitions.VolumetricTriggerMode value)
            where T : GadgetFlowDescription
        {
            entity.SetField("volumetricTriggerMode", value);
            return entity;
        }

        public static T SetWantedBattleEvent<T>(this T entity, GadgetDefinitions.BattleEvent value)
            where T : GadgetFlowDescription
        {
            entity.SetField("wantedBattleEvent", value);
            return entity;
        }

        public static T SetWantedContentPack<T>(this T entity, GamingPlatformDefinitions.ContentPack value)
            where T : GadgetFlowDescription
        {
            entity.SetField("wantedContentPack", value);
            return entity;
        }

        public static T SetWantedEncounterName<T>(this T entity, GadgetDefinitions.GadgetVariableString value)
            where T : GadgetFlowDescription
        {
            entity.SetField("wantedEncounterName", value);
            return entity;
        }

        public static T SetWantedEntranceIndex<T>(this T entity, System.Int32 value)
            where T : GadgetFlowDescription
        {
            entity.WantedEntranceIndex = value;
            return entity;
        }

        public static T SetWantedPresence<T>(this T entity, GadgetDefinitions.WantedPresence value)
            where T : GadgetFlowDescription
        {
            entity.SetField("wantedPresence", value);
            return entity;
        }

        public static T SetWantedPressure<T>(this T entity, GadgetDefinitions.WantedPressure value)
            where T : GadgetFlowDescription
        {
            entity.SetField("wantedPressure", value);
            return entity;
        }

        public static T SetWantedQuestUpdate<T>(this T entity, GadgetDefinitions.QuestUpdateType value)
            where T : GadgetFlowDescription
        {
            entity.WantedQuestUpdate = value;
            return entity;
        }
    }
}