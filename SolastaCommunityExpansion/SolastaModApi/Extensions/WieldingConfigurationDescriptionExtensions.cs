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
    [TargetType(typeof(WieldingConfigurationDescription))]
    public static partial class WieldingConfigurationDescriptionExtensions
    {
        public static T SetAnimationTag<T>(this T entity, System.String value)
            where T : WieldingConfigurationDescription
        {
            entity.SetField("animationTag", value);
            return entity;
        }

        public static T SetSecondaryAnimationTag<T>(this T entity, System.String value)
            where T : WieldingConfigurationDescription
        {
            entity.SetField("secondaryAnimationTag", value);
            return entity;
        }

        public static T SetSoundEffectDescription<T>(this T entity, SoundEffectDescription value)
            where T : WieldingConfigurationDescription
        {
            entity.SetField("soundEffectDescription", value);
            return entity;
        }

        public static T SetWeaponCategory<T>(this T entity, System.String value)
            where T : WieldingConfigurationDescription
        {
            entity.SetField("weaponCategory", value);
            return entity;
        }

        public static T SetWeaponProximity<T>(this T entity, RuleDefinitions.AttackProximity value)
            where T : WieldingConfigurationDescription
        {
            entity.SetField("weaponProximity", value);
            return entity;
        }
    }
}