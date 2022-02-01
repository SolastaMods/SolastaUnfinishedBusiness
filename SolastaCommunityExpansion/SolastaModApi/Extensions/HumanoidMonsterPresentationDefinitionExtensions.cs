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
    [TargetType(typeof(HumanoidMonsterPresentationDefinition))]
    public static partial class HumanoidMonsterPresentationDefinitionExtensions
    {
        public static System.Collections.Generic.Dictionary<MorphotypeElementDefinition.ElementCategory, System.String> GetMorphotypeElements<T>(this T entity)
            where T : HumanoidMonsterPresentationDefinition
        {
            return entity.GetField<System.Collections.Generic.Dictionary<MorphotypeElementDefinition.ElementCategory, System.String>>("morphotypeElements");
        }

        public static System.Collections.Generic.Dictionary<MorphotypeElementDefinition.ElementCategory, System.Single> GetMorphotypeElementsAdditionalValues<T>(this T entity)
            where T : HumanoidMonsterPresentationDefinition
        {
            return entity.GetField<System.Collections.Generic.Dictionary<MorphotypeElementDefinition.ElementCategory, System.Single>>("morphotypeElementsAdditionalValues");
        }

        public static T SetAgeMorphotypeValue<T>(this T entity, System.Single value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("ageMorphotypeValue", value);
            return entity;
        }

        public static T SetArmorDefinition<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("armorDefinition", value);
            return entity;
        }

        public static T SetBeardShapeMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("beardShapeMorphotype", value);
            return entity;
        }

        public static T SetBodyDecorationColorMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("bodyDecorationColorMorphotype", value);
            return entity;
        }

        public static T SetBodyDecorationMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("bodyDecorationMorphotype", value);
            return entity;
        }

        public static T SetEyeColorMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("eyeColorMorphotype", value);
            return entity;
        }

        public static T SetEyeMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("eyeMorphotype", value);
            return entity;
        }

        public static T SetFaceShapeMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("faceShapeMorphotype", value);
            return entity;
        }

        public static T SetFirstColor<T>(this T entity, UnityEngine.Color value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("firstColor", value);
            return entity;
        }

        public static T SetFourthColor<T>(this T entity, UnityEngine.Color value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("fourthColor", value);
            return entity;
        }

        public static T SetHairColorMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("hairColorMorphotype", value);
            return entity;
        }

        public static T SetHairShapeMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("hairShapeMorphotype", value);
            return entity;
        }

        public static T SetHelmetDefinition<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("helmetDefinition", value);
            return entity;
        }

        public static T SetItemDefinitionMainHand<T>(this T entity, ItemDefinition value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("itemDefinitionMainHand", value);
            return entity;
        }

        public static T SetItemDefinitionOffHand<T>(this T entity, ItemDefinition value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("itemDefinitionOffHand", value);
            return entity;
        }

        public static T SetMusculatureMorphotypeValue<T>(this T entity, System.Single value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("musculatureMorphotypeValue", value);
            return entity;
        }

        public static T SetOriginMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("originMorphotype", value);
            return entity;
        }

        public static T SetOverrideWieldedItems<T>(this T entity, System.Boolean value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("overrideWieldedItems", value);
            return entity;
        }

        public static T SetRaceDefinition<T>(this T entity, CharacterRaceDefinition value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("raceDefinition", value);
            return entity;
        }

        public static T SetScarsMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("scarsMorphotype", value);
            return entity;
        }

        public static T SetSecondColor<T>(this T entity, UnityEngine.Color value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("secondColor", value);
            return entity;
        }

        public static T SetSex<T>(this T entity, RuleDefinitions.CreatureSex value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("sex", value);
            return entity;
        }

        public static T SetShowHelmet<T>(this T entity, System.Boolean value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("showHelmet", value);
            return entity;
        }

        public static T SetSkinMorphotype<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("skinMorphotype", value);
            return entity;
        }

        public static T SetSubRaceDefinition<T>(this T entity, CharacterRaceDefinition value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("subRaceDefinition", value);
            return entity;
        }

        public static T SetTabardDefinition<T>(this T entity, System.String value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("tabardDefinition", value);
            return entity;
        }

        public static T SetThirdColor<T>(this T entity, UnityEngine.Color value)
            where T : HumanoidMonsterPresentationDefinition
        {
            entity.SetField("thirdColor", value);
            return entity;
        }
    }
}