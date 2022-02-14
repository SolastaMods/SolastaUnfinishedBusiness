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
    [TargetType(typeof(NarrativeTreeDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class NarrativeTreeDefinitionExtensions
    {
        public static T AddAllNarrativeStateDescriptions<T>(this T entity,  params  NarrativeStateDescription [ ]  value)
            where T : NarrativeTreeDefinition
        {
            AddAllNarrativeStateDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAllNarrativeStateDescriptions<T>(this T entity, IEnumerable<NarrativeStateDescription> value)
            where T : NarrativeTreeDefinition
        {
            entity.AllNarrativeStateDescriptions.AddRange(value);
            return entity;
        }

        public static T AddStaticNpcRoles<T>(this T entity,  params  System . String [ ]  value)
            where T : NarrativeTreeDefinition
        {
            AddStaticNpcRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStaticNpcRoles<T>(this T entity, IEnumerable<System.String> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRoles.AddRange(value);
            return entity;
        }

        public static T AddStaticNpcRolesMandatoryStatus<T>(this T entity,  params  System . Boolean [ ]  value)
            where T : NarrativeTreeDefinition
        {
            AddStaticNpcRolesMandatoryStatus(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStaticNpcRolesMandatoryStatus<T>(this T entity, IEnumerable<System.Boolean> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRolesMandatoryStatus.AddRange(value);
            return entity;
        }

        public static T AddStaticPlayerRoles<T>(this T entity,  params  System . String [ ]  value)
            where T : NarrativeTreeDefinition
        {
            AddStaticPlayerRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStaticPlayerRoles<T>(this T entity, IEnumerable<System.String> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticPlayerRoles.AddRange(value);
            return entity;
        }

        public static T ClearAllNarrativeStateDescriptions<T>(this T entity)
            where T : NarrativeTreeDefinition
        {
            entity.AllNarrativeStateDescriptions.Clear();
            return entity;
        }

        public static T ClearStaticNpcRoles<T>(this T entity)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRoles.Clear();
            return entity;
        }

        public static T ClearStaticNpcRolesMandatoryStatus<T>(this T entity)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRolesMandatoryStatus.Clear();
            return entity;
        }

        public static T ClearStaticPlayerRoles<T>(this T entity)
            where T : NarrativeTreeDefinition
        {
            entity.StaticPlayerRoles.Clear();
            return entity;
        }

        public static T SetAllNarrativeStateDescriptions<T>(this T entity,  params  NarrativeStateDescription [ ]  value)
            where T : NarrativeTreeDefinition
        {
            SetAllNarrativeStateDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAllNarrativeStateDescriptions<T>(this T entity, IEnumerable<NarrativeStateDescription> value)
            where T : NarrativeTreeDefinition
        {
            entity.AllNarrativeStateDescriptions.SetRange(value);
            return entity;
        }

        public static T SetAvailableCameraShotNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.AvailableCameraShotNames = value;
            return entity;
        }

        public static T SetAvailableCameraTargetNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.AvailableCameraTargetNames = value;
            return entity;
        }

        public static T SetGeneratedCameraShotNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.GeneratedCameraShotNames = value;
            return entity;
        }

        public static T SetHasSpecialCutsceneLightingForCharacters<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.HasSpecialCutsceneLightingForCharacters = value;
            return entity;
        }

        public static T SetIsUserDialog<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.SetField("<IsUserDialog>k__BackingField", value);
            return entity;
        }

        public static T SetNarrativeCameraSetupGUID<T>(this T entity, System.String value)
            where T : NarrativeTreeDefinition
        {
            entity.NarrativeCameraSetupGUID = value;
            return entity;
        }

        public static T SetSerializeVersion<T>(this T entity, System.Int32 value)
            where T : NarrativeTreeDefinition
        {
            entity.SerializeVersion = value;
            return entity;
        }

        public static T SetSkippable<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.Skippable = value;
            return entity;
        }

        public static T SetStaticNpcRoles<T>(this T entity,  params  System . String [ ]  value)
            where T : NarrativeTreeDefinition
        {
            SetStaticNpcRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStaticNpcRoles<T>(this T entity, IEnumerable<System.String> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRoles.SetRange(value);
            return entity;
        }

        public static T SetStaticNpcRolesMandatoryStatus<T>(this T entity,  params  System . Boolean [ ]  value)
            where T : NarrativeTreeDefinition
        {
            SetStaticNpcRolesMandatoryStatus(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStaticNpcRolesMandatoryStatus<T>(this T entity, IEnumerable<System.Boolean> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticNpcRolesMandatoryStatus.SetRange(value);
            return entity;
        }

        public static T SetStaticPlayerRoles<T>(this T entity,  params  System . String [ ]  value)
            where T : NarrativeTreeDefinition
        {
            SetStaticPlayerRoles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStaticPlayerRoles<T>(this T entity, IEnumerable<System.String> value)
            where T : NarrativeTreeDefinition
        {
            entity.StaticPlayerRoles.SetRange(value);
            return entity;
        }

        public static T SetUnequipWieldedItems<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.UnequipWieldedItems = value;
            return entity;
        }
    }
}