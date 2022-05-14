using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(CampaignDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CampaignDefinitionExtensions
    {
        public static T AddAutoGameplayRoles<T>(this T entity, params CampaignDefinition.GameplayRoleFilter[] value)
            where T : CampaignDefinition
        {
            AddAutoGameplayRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAutoGameplayRoles<T>(this T entity, IEnumerable<CampaignDefinition.GameplayRoleFilter> value)
            where T : CampaignDefinition
        {
            entity.AutoGameplayRoles.AddRange(value);
            return entity;
        }

        public static T AddInitialBestiaryContent<T>(this T entity, params MonsterKnowledgeDescription[] value)
            where T : CampaignDefinition
        {
            AddInitialBestiaryContent(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddInitialBestiaryContent<T>(this T entity, IEnumerable<MonsterKnowledgeDescription> value)
            where T : CampaignDefinition
        {
            entity.InitialBestiaryContent.AddRange(value);
            return entity;
        }

        public static T AddIntroductionCaptions<T>(this T entity, params System.String[] value)
            where T : CampaignDefinition
        {
            AddIntroductionCaptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddIntroductionCaptions<T>(this T entity, IEnumerable<System.String> value)
            where T : CampaignDefinition
        {
            entity.IntroductionCaptions.AddRange(value);
            return entity;
        }

        public static T AddKnownRecipes<T>(this T entity, params RecipeDefinition[] value)
            where T : CampaignDefinition
        {
            AddKnownRecipes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownRecipes<T>(this T entity, IEnumerable<RecipeDefinition> value)
            where T : CampaignDefinition
        {
            entity.KnownRecipes.AddRange(value);
            return entity;
        }

        public static T AddPredefinedParty<T>(this T entity, params System.String[] value)
            where T : CampaignDefinition
        {
            AddPredefinedParty(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPredefinedParty<T>(this T entity, IEnumerable<System.String> value)
            where T : CampaignDefinition
        {
            entity.PredefinedParty.AddRange(value);
            return entity;
        }

        public static T AddRegisteredFactions<T>(this T entity, params CampaignDefinition.FactionRegistration[] value)
            where T : CampaignDefinition
        {
            AddRegisteredFactions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRegisteredFactions<T>(this T entity, IEnumerable<CampaignDefinition.FactionRegistration> value)
            where T : CampaignDefinition
        {
            entity.RegisteredFactions.AddRange(value);
            return entity;
        }

        public static T AddRegisteredVariables<T>(this T entity, params VariableRegistrationDescription[] value)
            where T : CampaignDefinition
        {
            AddRegisteredVariables(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRegisteredVariables<T>(this T entity, IEnumerable<VariableRegistrationDescription> value)
            where T : CampaignDefinition
        {
            entity.RegisteredVariables.AddRange(value);
            return entity;
        }

        public static T AddSkipIntroRegisteredVariables<T>(this T entity, params VariableRegistrationDescription[] value)
            where T : CampaignDefinition
        {
            AddSkipIntroRegisteredVariables(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSkipIntroRegisteredVariables<T>(this T entity, IEnumerable<VariableRegistrationDescription> value)
            where T : CampaignDefinition
        {
            entity.SkipIntroRegisteredVariables.AddRange(value);
            return entity;
        }

        public static T AddStartingQuests<T>(this T entity, params QuestTreeDefinition[] value)
            where T : CampaignDefinition
        {
            AddStartingQuests(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStartingQuests<T>(this T entity, IEnumerable<QuestTreeDefinition> value)
            where T : CampaignDefinition
        {
            entity.StartingQuests.AddRange(value);
            return entity;
        }

        public static T ClearAutoGameplayRoles<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.AutoGameplayRoles.Clear();
            return entity;
        }

        public static T ClearInitialBestiaryContent<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.InitialBestiaryContent.Clear();
            return entity;
        }

        public static T ClearIntroductionCaptions<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.IntroductionCaptions.Clear();
            return entity;
        }

        public static T ClearKnownRecipes<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.KnownRecipes.Clear();
            return entity;
        }

        public static T ClearPredefinedParty<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.PredefinedParty.Clear();
            return entity;
        }

        public static T ClearRegisteredFactions<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.RegisteredFactions.Clear();
            return entity;
        }

        public static T ClearRegisteredVariables<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.RegisteredVariables.Clear();
            return entity;
        }

        public static T ClearSkipIntroRegisteredVariables<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.SkipIntroRegisteredVariables.Clear();
            return entity;
        }

        public static T ClearStartingQuests<T>(this T entity)
            where T : CampaignDefinition
        {
            entity.StartingQuests.Clear();
            return entity;
        }

        public static T SetAutoGameplayRoles<T>(this T entity, params CampaignDefinition.GameplayRoleFilter[] value)
            where T : CampaignDefinition
        {
            SetAutoGameplayRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAutoGameplayRoles<T>(this T entity, IEnumerable<CampaignDefinition.GameplayRoleFilter> value)
            where T : CampaignDefinition
        {
            entity.AutoGameplayRoles.SetRange(value);
            return entity;
        }

        public static T SetCalendar<T>(this T entity, CalendarDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("calendar", value);
            return entity;
        }

        public static T SetCanSkipIntro<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("canSkipIntro", value);
            return entity;
        }

        public static T SetConclusionMovieDefinition<T>(this T entity, MoviePlaybackDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("conclusionMovieDefinition", value);
            return entity;
        }

        public static T SetEditorOnly<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("editorOnly", value);
            return entity;
        }

        public static T SetFastTimeScaleDuringTravel<T>(this T entity, System.Single value)
            where T : CampaignDefinition
        {
            entity.SetField("fastTimeScaleDuringTravel", value);
            return entity;
        }

        public static T SetGraphicsCampaignMapReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : CampaignDefinition
        {
            entity.SetField("graphicsCampaignMapReference", value);
            return entity;
        }

        public static T SetInitialBestiaryContent<T>(this T entity, params MonsterKnowledgeDescription[] value)
            where T : CampaignDefinition
        {
            SetInitialBestiaryContent(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetInitialBestiaryContent<T>(this T entity, IEnumerable<MonsterKnowledgeDescription> value)
            where T : CampaignDefinition
        {
            entity.InitialBestiaryContent.SetRange(value);
            return entity;
        }

        public static T SetInitialPartyPosition<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("initialPartyPosition", value);
            return entity;
        }

        public static T SetInsideLocation<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("insideLocation", value);
            return entity;
        }

        public static T SetIntroductionCaptions<T>(this T entity, params System.String[] value)
            where T : CampaignDefinition
        {
            SetIntroductionCaptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetIntroductionCaptions<T>(this T entity, IEnumerable<System.String> value)
            where T : CampaignDefinition
        {
            entity.IntroductionCaptions.SetRange(value);
            return entity;
        }

        public static T SetIntroMovieDefinition<T>(this T entity, MoviePlaybackDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("introMovieDefinition", value);
            return entity;
        }

        public static T SetIsUserCampaign<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("isUserCampaign", value);
            return entity;
        }

        public static T SetJournalStart<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("journalStart", value);
            return entity;
        }

        public static T SetKnownRecipes<T>(this T entity, params RecipeDefinition[] value)
            where T : CampaignDefinition
        {
            SetKnownRecipes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownRecipes<T>(this T entity, IEnumerable<RecipeDefinition> value)
            where T : CampaignDefinition
        {
            entity.KnownRecipes.SetRange(value);
            return entity;
        }

        public static T SetLevelCap<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("levelCap", value);
            return entity;
        }

        public static T SetLocationProfile<T>(this T entity, UnityEngine.Rendering.PostProcessing.PostProcessProfile value)
            where T : CampaignDefinition
        {
            entity.SetField("locationProfile", value);
            return entity;
        }

        public static T SetMaxStartLevel<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("maxStartLevel", value);
            return entity;
        }

        public static T SetMinStartLevel<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("minStartLevel", value);
            return entity;
        }

        public static T SetMultiplayer<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("multiplayer", value);
            return entity;
        }

        public static T SetNormalTimeScaleDuringTravel<T>(this T entity, System.Single value)
            where T : CampaignDefinition
        {
            entity.SetField("normalTimeScaleDuringTravel", value);
            return entity;
        }

        public static T SetPartySize<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("partySize", value);
            return entity;
        }

        public static T SetPredefinedParty<T>(this T entity, params System.String[] value)
            where T : CampaignDefinition
        {
            SetPredefinedParty(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPredefinedParty<T>(this T entity, IEnumerable<System.String> value)
            where T : CampaignDefinition
        {
            entity.PredefinedParty.SetRange(value);
            return entity;
        }

        public static T SetRegisteredFactions<T>(this T entity, params CampaignDefinition.FactionRegistration[] value)
            where T : CampaignDefinition
        {
            SetRegisteredFactions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRegisteredFactions<T>(this T entity, IEnumerable<CampaignDefinition.FactionRegistration> value)
            where T : CampaignDefinition
        {
            entity.RegisteredFactions.SetRange(value);
            return entity;
        }

        public static T SetRegisteredVariables<T>(this T entity, params VariableRegistrationDescription[] value)
            where T : CampaignDefinition
        {
            SetRegisteredVariables(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRegisteredVariables<T>(this T entity, IEnumerable<VariableRegistrationDescription> value)
            where T : CampaignDefinition
        {
            entity.RegisteredVariables.SetRange(value);
            return entity;
        }

        public static T SetRenderSettingsSceneProfile<T>(this T entity, RenderSettingsSceneProfile value)
            where T : CampaignDefinition
        {
            entity.SetField("renderSettingsSceneProfile", value);
            return entity;
        }

        public static T SetSceneFilePath<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("sceneFilePath", value);
            return entity;
        }

        public static T SetSceneReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : CampaignDefinition
        {
            entity.SetField("sceneReference", value);
            return entity;
        }

        public static T SetSkipIntroEntrance<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("skipIntroEntrance", value);
            return entity;
        }

        public static T SetSkipIntroLocation<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("skipIntroLocation", value);
            return entity;
        }

        public static T SetSkipIntroRegisteredVariables<T>(this T entity, params VariableRegistrationDescription[] value)
            where T : CampaignDefinition
        {
            SetSkipIntroRegisteredVariables(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSkipIntroRegisteredVariables<T>(this T entity, IEnumerable<VariableRegistrationDescription> value)
            where T : CampaignDefinition
        {
            entity.SkipIntroRegisteredVariables.SetRange(value);
            return entity;
        }

        public static T SetStartDay<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startDay", value);
            return entity;
        }

        public static T SetStartHour<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startHour", value);
            return entity;
        }

        public static T SetStartingQuests<T>(this T entity, params QuestTreeDefinition[] value)
            where T : CampaignDefinition
        {
            SetStartingQuests(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStartingQuests<T>(this T entity, IEnumerable<QuestTreeDefinition> value)
            where T : CampaignDefinition
        {
            entity.StartingQuests.SetRange(value);
            return entity;
        }

        public static T SetStartMonth<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startMonth", value);
            return entity;
        }

        public static T SetStartYear<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startYear", value);
            return entity;
        }
    }
}