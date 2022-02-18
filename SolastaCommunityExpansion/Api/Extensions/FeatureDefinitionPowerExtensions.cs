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
    [TargetType(typeof(FeatureDefinitionPower)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionPowerExtensions
    {
        public static T SetAbilityScore<T>(this T entity, System.String value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("abilityScore", value);
            return entity;
        }

        public static T SetAbilityScoreBonusToAttack<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("abilityScoreBonusToAttack", value);
            return entity;
        }

        public static T SetAbilityScoreDetermination<T>(this T entity, RuleDefinitions.AbilityScoreDetermination value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("abilityScoreDetermination", value);
            return entity;
        }

        public static T SetActivationTime<T>(this T entity, RuleDefinitions.ActivationTime value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("activationTime", value);
            return entity;
        }

        public static T SetAttackHitComputation<T>(this T entity, RuleDefinitions.PowerAttackHitComputation value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("attackHitComputation", value);
            return entity;
        }

        public static T SetCanUseInDialog<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("canUseInDialog", value);
            return entity;
        }

        public static T SetCastingSuccessComputation<T>(this T entity, RuleDefinitions.CastingSuccessComputation value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("castingSuccessComputation", value);
            return entity;
        }

        public static T SetCostPerUse<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("costPerUse", value);
            return entity;
        }

        public static T SetDelegatedToAction<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("delegatedToAction", value);
            return entity;
        }

        public static T SetDisableIfConditionIsOwned<T>(this T entity, ConditionDefinition value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("disableIfConditionIsOwned", value);
            return entity;
        }

        public static T SetEffectDescription<T>(this T entity, EffectDescription value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("effectDescription", value);
            return entity;
        }

        public static T SetFixedAttackHit<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("fixedAttackHit", value);
            return entity;
        }

        public static T SetFixedUsesPerRecharge<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("fixedUsesPerRecharge", value);
            return entity;
        }

        public static T SetHasCastingFailure<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("hasCastingFailure", value);
            return entity;
        }

        public static T SetIncludeBaseDescription<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("includeBaseDescription", value);
            return entity;
        }

        public static T SetMagical<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("magical", value);
            return entity;
        }

        public static T SetOverriddenPower<T>(this T entity, FeatureDefinitionPower value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("overriddenPower", value);
            return entity;
        }

        public static T SetProficiencyBonusToAttack<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("proficiencyBonusToAttack", value);
            return entity;
        }

        public static T SetReactionContext<T>(this T entity, RuleDefinitions.ReactionTriggerContext value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("reactionContext", value);
            return entity;
        }

        public static T SetReactionName<T>(this T entity, System.String value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("reactionName", value);
            return entity;
        }

        public static T SetRechargeRate<T>(this T entity, RuleDefinitions.RechargeRate value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("rechargeRate", value);
            return entity;
        }

        public static T SetShortTitleOverride<T>(this T entity, System.String value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("shortTitleOverride", value);
            return entity;
        }

        public static T SetShowCasting<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("showCasting", value);
            return entity;
        }

        public static T SetSpellcastingFeature<T>(this T entity, FeatureDefinitionCastSpell value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("spellcastingFeature", value);
            return entity;
        }

        public static T SetSurrogateToSpell<T>(this T entity, SpellDefinition value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("surrogateToSpell", value);
            return entity;
        }

        public static T SetTriggeredBySpecialMove<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("triggeredBySpecialMove", value);
            return entity;
        }

        public static T SetUniqueInstance<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("uniqueInstance", value);
            return entity;
        }

        public static T SetUsesAbilityScoreName<T>(this T entity, System.String value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("usesAbilityScoreName", value);
            return entity;
        }

        public static T SetUsesDetermination<T>(this T entity, RuleDefinitions.UsesDetermination value)
            where T : FeatureDefinitionPower
        {
            entity.SetField("usesDetermination", value);
            return entity;
        }
    }
}