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
    [TargetType(typeof(CharacterClassDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class CharacterClassDefinitionExtensions
    {
        public static T AddAbilityScoresPriority<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddAbilityScoresPriority(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAbilityScoresPriority<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.AbilityScoresPriority.AddRange(value);
            return entity;
        }

        public static T AddEquipmentRows<T>(this T entity, params CharacterClassDefinition.HeroEquipmentRow[] value)
            where T : CharacterClassDefinition
        {
            AddEquipmentRows(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEquipmentRows<T>(this T entity, IEnumerable<CharacterClassDefinition.HeroEquipmentRow> value)
            where T : CharacterClassDefinition
        {
            entity.EquipmentRows.AddRange(value);
            return entity;
        }

        public static T AddExpertiseAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddExpertiseAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddExpertiseAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.ExpertiseAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T AddFeatAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddFeatAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.FeatAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T AddFeatureUnlocks<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : CharacterClassDefinition
        {
            AddFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterClassDefinition
        {
            entity.FeatureUnlocks.AddRange(value);
            return entity;
        }

        public static T AddMetamagicAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddMetamagicAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMetamagicAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.MetamagicAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, params PersonalityFlagOccurence[] value)
            where T : CharacterClassDefinition
        {
            AddPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterClassDefinition
        {
            entity.PersonalityFlagOccurences.AddRange(value);
            return entity;
        }

        public static T AddSkillAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddSkillAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSkillAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.SkillAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T AddToolAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            AddToolAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddToolAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.ToolAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T ClearAbilityScoresPriority<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.AbilityScoresPriority.Clear();
            return entity;
        }

        public static T ClearEquipmentRows<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.EquipmentRows.Clear();
            return entity;
        }

        public static T ClearExpertiseAutolearnPreference<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.ExpertiseAutolearnPreference.Clear();
            return entity;
        }

        public static T ClearFeatAutolearnPreference<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.FeatAutolearnPreference.Clear();
            return entity;
        }

        public static T ClearFeatureUnlocks<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.FeatureUnlocks.Clear();
            return entity;
        }

        public static T ClearMetamagicAutolearnPreference<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.MetamagicAutolearnPreference.Clear();
            return entity;
        }

        public static T ClearPersonalityFlagOccurences<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.PersonalityFlagOccurences.Clear();
            return entity;
        }

        public static T ClearSkillAutolearnPreference<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.SkillAutolearnPreference.Clear();
            return entity;
        }

        public static T ClearToolAutolearnPreference<T>(this T entity)
            where T : CharacterClassDefinition
        {
            entity.ToolAutolearnPreference.Clear();
            return entity;
        }

        public static T SetAbilityScoresPriority<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetAbilityScoresPriority(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAbilityScoresPriority<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.AbilityScoresPriority.SetRange(value);
            return entity;
        }

        public static T SetClassAnimationId<T>(this T entity, AnimationDefinitions.ClassAnimationId value)
            where T : CharacterClassDefinition
        {
            entity.SetField("classAnimationId", value);
            return entity;
        }

        public static T SetClassPictogramReference<T>(this T entity,
            UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : CharacterClassDefinition
        {
            entity.SetField("classPictogramReference", value);
            return entity;
        }

        public static T SetDefaultBattleDecisions<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : CharacterClassDefinition
        {
            entity.SetField("defaultBattleDecisions", value);
            return entity;
        }

        public static T SetEquipmentRows<T>(this T entity, params CharacterClassDefinition.HeroEquipmentRow[] value)
            where T : CharacterClassDefinition
        {
            SetEquipmentRows(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEquipmentRows<T>(this T entity, IEnumerable<CharacterClassDefinition.HeroEquipmentRow> value)
            where T : CharacterClassDefinition
        {
            entity.EquipmentRows.SetRange(value);
            return entity;
        }

        public static T SetExpertiseAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetExpertiseAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetExpertiseAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.ExpertiseAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetFeatAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetFeatAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.FeatAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : CharacterClassDefinition
        {
            SetFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterClassDefinition
        {
            entity.FeatureUnlocks.SetRange(value);
            return entity;
        }

        public static T SetHitDice<T>(this T entity, DieType value)
            where T : CharacterClassDefinition
        {
            entity.SetField("hitDice", value);
            return entity;
        }

        public static T SetIngredientGatheringOdds<T>(this T entity, System.Int32 value)
            where T : CharacterClassDefinition
        {
            entity.SetField("ingredientGatheringOdds", value);
            return entity;
        }

        public static T SetMetamagicAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetMetamagicAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMetamagicAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.MetamagicAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, params PersonalityFlagOccurence[] value)
            where T : CharacterClassDefinition
        {
            SetPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterClassDefinition
        {
            entity.PersonalityFlagOccurences.SetRange(value);
            return entity;
        }

        public static T SetRequiresDeity<T>(this T entity, System.Boolean value)
            where T : CharacterClassDefinition
        {
            entity.SetField("requiresDeity", value);
            return entity;
        }

        public static T SetSkillAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetSkillAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSkillAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.SkillAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetToolAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterClassDefinition
        {
            SetToolAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetToolAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterClassDefinition
        {
            entity.ToolAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetVocalSpellSemeClass<T>(this T entity, VocalSpellSemeClass value)
            where T : CharacterClassDefinition
        {
            entity.SetField("vocalSpellSemeClass", value);
            return entity;
        }
    }
}
