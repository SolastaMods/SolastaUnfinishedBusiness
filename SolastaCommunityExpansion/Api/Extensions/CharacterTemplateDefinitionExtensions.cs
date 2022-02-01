using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
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
    [TargetType(typeof(CharacterTemplateDefinition))]
    public static partial class CharacterTemplateDefinitionExtensions
    {
        public static T SetAbilityScores<T>(this T entity, System.Int32[] value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("abilityScores", value);
            return entity;
        }

        public static T SetAge<T>(this T entity, System.Int32 value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("age", value);
            return entity;
        }

        public static T SetAgeMorphotypeValue<T>(this T entity, System.Single value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("ageMorphotypeValue", value);
            return entity;
        }

        public static T SetAlignment<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("alignment", value);
            return entity;
        }

        public static T SetAlignmentPersonalityFlag1<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("alignmentPersonalityFlag1", value);
            return entity;
        }

        public static T SetAlignmentPersonalityFlag2<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("alignmentPersonalityFlag2", value);
            return entity;
        }

        public static T SetAutomateAbilityScoreIncreases<T>(this T entity, System.Boolean value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("automateAbilityScoreIncreases", value);
            return entity;
        }

        public static T SetBackground<T>(this T entity, CharacterBackgroundDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("background", value);
            return entity;
        }

        public static T SetBackgroundPersonalityFlag1<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("backgroundPersonalityFlag1", value);
            return entity;
        }

        public static T SetBackgroundPersonalityFlag2<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("backgroundPersonalityFlag2", value);
            return entity;
        }

        public static T SetBeardShapeMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("beardShapeMorphotype", value);
            return entity;
        }

        public static T SetBodyDecorationColorMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("bodyDecorationColorMorphotype", value);
            return entity;
        }

        public static T SetBodyDecorationMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("bodyDecorationMorphotype", value);
            return entity;
        }

        public static T SetCharacterLevel<T>(this T entity, System.Int32 value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("characterLevel", value);
            return entity;
        }

        public static T SetDeity<T>(this T entity, DeityDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("deity", value);
            return entity;
        }

        public static T SetEditorOnly<T>(this T entity, System.Boolean value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("editorOnly", value);
            return entity;
        }

        public static T SetEyeColorMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("eyeColorMorphotype", value);
            return entity;
        }

        public static T SetEyeMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("eyeMorphotype", value);
            return entity;
        }

        public static T SetFacePath<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("facePath", value);
            return entity;
        }

        public static T SetFaceShapeMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("faceShapeMorphotype", value);
            return entity;
        }

        public static T SetFightingStyle<T>(this T entity, FightingStyleDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("fightingStyle", value);
            return entity;
        }

        public static T SetFirstName<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("firstName", value);
            return entity;
        }

        public static T SetHairColorMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("hairColorMorphotype", value);
            return entity;
        }

        public static T SetHairShapeMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("hairShapeMorphotype", value);
            return entity;
        }

        public static T SetMainClass<T>(this T entity, CharacterClassDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("mainClass", value);
            return entity;
        }

        public static T SetMainRace<T>(this T entity, CharacterRaceDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("mainRace", value);
            return entity;
        }

        public static T SetMusculatureMorphotypeValue<T>(this T entity, System.Single value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("musculatureMorphotypeValue", value);
            return entity;
        }

        public static T SetOriginMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("originMorphotype", value);
            return entity;
        }

        public static T SetPronoun<T>(this T entity, Gui.LocalizationSpeakerGender value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("pronoun", value);
            return entity;
        }

        public static T SetScarsMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("scarsMorphotype", value);
            return entity;
        }

        public static T SetSex<T>(this T entity, RuleDefinitions.CreatureSex value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("sex", value);
            return entity;
        }

        public static T SetSkinMorphotype<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("skinMorphotype", value);
            return entity;
        }

        public static T SetStartingMoney<T>(this T entity, System.Int32[] value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("startingMoney", value);
            return entity;
        }

        public static T SetSubClass<T>(this T entity, CharacterSubclassDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("subClass", value);
            return entity;
        }

        public static T SetSubRace<T>(this T entity, CharacterRaceDefinition value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("subRace", value);
            return entity;
        }

        public static T SetSurName<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("surName", value);
            return entity;
        }

        public static T SetVoiceId<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("voiceId", value);
            return entity;
        }
    }
}