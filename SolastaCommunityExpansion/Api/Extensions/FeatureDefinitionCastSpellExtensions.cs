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
    [TargetType(typeof(FeatureDefinitionCastSpell))]
    public static partial class FeatureDefinitionCastSpellExtensions
    {
        public static T SetFocusType<T>(this T entity, EquipmentDefinitions.FocusType value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("focusType", value);
            return entity;
        }

        public static T SetSlotsRecharge<T>(this T entity, RuleDefinitions.RechargeRate value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("slotsRecharge", value);
            return entity;
        }

        public static T SetSpellcastingAbility<T>(this T entity, System.String value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellcastingAbility", value);
            return entity;
        }

        public static T SetSpellCastingLevel<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellCastingLevel", value);
            return entity;
        }

        public static T SetSpellCastingOrigin<T>(this T entity, FeatureDefinitionCastSpell.CastingOrigin value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellCastingOrigin", value);
            return entity;
        }

        public static T SetSpellcastingParametersComputation<T>(this T entity, RuleDefinitions.SpellcastingParametersComputation value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellcastingParametersComputation", value);
            return entity;
        }

        public static T SetSpellKnowledge<T>(this T entity, RuleDefinitions.SpellKnowledge value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellKnowledge", value);
            return entity;
        }

        public static T SetSpellListDefinition<T>(this T entity, SpellListDefinition value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellListDefinition", value);
            return entity;
        }

        public static T SetSpellPreparationCount<T>(this T entity, RuleDefinitions.SpellPreparationCount value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellPreparationCount", value);
            return entity;
        }

        public static T SetSpellReadyness<T>(this T entity, RuleDefinitions.SpellReadyness value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellReadyness", value);
            return entity;
        }

        public static T SetStaticDCValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("staticDCValue", value);
            return entity;
        }

        public static T SetStaticToHitValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("staticToHitValue", value);
            return entity;
        }
    }
}