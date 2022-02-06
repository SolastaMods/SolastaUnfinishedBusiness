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
    [TargetType(typeof(AdventureLogDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class AdventureLogDefinitionExtensions
    {
        public static System.Collections.Generic.List<AdventureStyleDuplet> GetStyleDuplets<T>(this T entity)
            where T : AdventureLogDefinition
        {
            return entity.GetField<System.Collections.Generic.List<AdventureStyleDuplet>>("styleDuplets");
        }

        public static T SetBattleHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("battleHeight", value);
            return entity;
        }

        public static T SetCampaignStartHeaderHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("campaignStartHeaderHeight", value);
            return entity;
        }

        public static T SetCampaignStartLineHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("campaignStartLineHeight", value);
            return entity;
        }

        public static T SetCampaignStartTrailingHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("campaignStartTrailingHeight", value);
            return entity;
        }

        public static T SetCampaignStartWordSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("campaignStartWordSpacing", value);
            return entity;
        }

        public static T SetConversationHeaderHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationHeaderHeight", value);
            return entity;
        }

        public static T SetConversationIndentWidth<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationIndentWidth", value);
            return entity;
        }

        public static T SetConversationLineHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationLineHeight", value);
            return entity;
        }

        public static T SetConversationParagraphSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationParagraphSpacing", value);
            return entity;
        }

        public static T SetConversationTrailingHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationTrailingHeight", value);
            return entity;
        }

        public static T SetConversationWordSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("conversationWordSpacing", value);
            return entity;
        }

        public static T SetLoreTextLineHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("loreTextLineHeight", value);
            return entity;
        }

        public static T SetLoreTextTrailingHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("loreTextTrailingHeight", value);
            return entity;
        }

        public static T SetLoreTextWordSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("loreTextWordSpacing", value);
            return entity;
        }

        public static T SetNewDayHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("newDayHeight", value);
            return entity;
        }

        public static T SetQuestHeaderHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("questHeaderHeight", value);
            return entity;
        }

        public static T SetQuestLineHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("questLineHeight", value);
            return entity;
        }

        public static T SetQuestParagraphSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("questParagraphSpacing", value);
            return entity;
        }

        public static T SetQuestTrailingHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("questTrailingHeight", value);
            return entity;
        }

        public static T SetQuestWordSpacing<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("questWordSpacing", value);
            return entity;
        }

        public static T SetRelationHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("relationHeight", value);
            return entity;
        }

        public static T SetSectionHeaderHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("sectionHeaderHeight", value);
            return entity;
        }

        public static T SetTravelFooterHeight<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("travelFooterHeight", value);
            return entity;
        }

        public static T SetTravelHeaderHeightDestination<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("travelHeaderHeightDestination", value);
            return entity;
        }

        public static T SetTravelHeaderHeightNoDestination<T>(this T entity, System.Single value)
            where T : AdventureLogDefinition
        {
            entity.SetField("travelHeaderHeightNoDestination", value);
            return entity;
        }
    }
}