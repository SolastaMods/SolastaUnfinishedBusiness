using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(RulesetCharacterHero))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class RulesetCharacterHeroExtensions
    {
        public static T AddActiveFightingStyles<T>(this T entity, params FightingStyleDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddActiveFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddActiveFightingStyles<T>(this T entity, IEnumerable<FightingStyleDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.ActiveFightingStyles.AddRange(value);
            return entity;
        }

        public static T AddAfterRestActions<T>(this T entity, params RestActivityDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddAfterRestActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAfterRestActions<T>(this T entity, IEnumerable<RestActivityDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.AfterRestActions.AddRange(value);
            return entity;
        }

        public static T AddAlignmentOptionaPersonalityFlags<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddAlignmentOptionaPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAlignmentOptionaPersonalityFlags<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.AlignmentOptionaPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T AddArmorCategoryProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddArmorCategoryProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddArmorCategoryProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ArmorCategoryProficiencies.AddRange(value);
            return entity;
        }

        public static T AddArmorTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddArmorTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddArmorTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ArmorTypeProficiencies.AddRange(value);
            return entity;
        }

        public static T AddBackgroundOptionalPersonalityFlags<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddBackgroundOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddBackgroundOptionalPersonalityFlags<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.BackgroundOptionalPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T AddClassesHistory<T>(this T entity, params CharacterClassDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddClassesHistory(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddClassesHistory<T>(this T entity, IEnumerable<CharacterClassDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.ClassesHistory.AddRange(value);
            return entity;
        }

        public static T AddExpertiseProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddExpertiseProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddExpertiseProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ExpertiseProficiencies.AddRange(value);
            return entity;
        }

        public static T AddFeatProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddFeatProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.FeatProficiencies.AddRange(value);
            return entity;
        }

        public static T AddLanguageProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddLanguageProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLanguageProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.LanguageProficiencies.AddRange(value);
            return entity;
        }

        public static T AddMetamagicProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddMetamagicProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMetamagicProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.MetamagicProficiencies.AddRange(value);
            return entity;
        }

        public static T AddSkillProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddSkillProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSkillProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.SkillProficiencies.AddRange(value);
            return entity;
        }

        public static T AddToolTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddToolTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddToolTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ToolTypeProficiencies.AddRange(value);
            return entity;
        }

        public static T AddTrainedExpertises<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedExpertises(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedExpertises<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedExpertises.AddRange(value);
            return entity;
        }

        public static T AddTrainedFeats<T>(this T entity, params FeatDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedFeats(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedFeats<T>(this T entity, IEnumerable<FeatDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedFeats.AddRange(value);
            return entity;
        }

        public static T AddTrainedFightingStyles<T>(this T entity, params FightingStyleDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedFightingStyles<T>(this T entity, IEnumerable<FightingStyleDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedFightingStyles.AddRange(value);
            return entity;
        }

        public static T AddTrainedLanguages<T>(this T entity, params LanguageDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedLanguages<T>(this T entity, IEnumerable<LanguageDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedLanguages.AddRange(value);
            return entity;
        }

        public static T AddTrainedMetamagicOptions<T>(this T entity, params MetamagicOptionDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedMetamagicOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedMetamagicOptions<T>(this T entity, IEnumerable<MetamagicOptionDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedMetamagicOptions.AddRange(value);
            return entity;
        }

        public static T AddTrainedSkills<T>(this T entity, params SkillDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedSkills(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedSkills<T>(this T entity, IEnumerable<SkillDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedSkills.AddRange(value);
            return entity;
        }

        public static T AddTrainedToolTypes<T>(this T entity, params ToolTypeDefinition[] value)
            where T : RulesetCharacterHero
        {
            AddTrainedToolTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrainedToolTypes<T>(this T entity, IEnumerable<ToolTypeDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedToolTypes.AddRange(value);
            return entity;
        }

        public static T AddWeaponCategoryProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddWeaponCategoryProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWeaponCategoryProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.WeaponCategoryProficiencies.AddRange(value);
            return entity;
        }

        public static T AddWeaponTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            AddWeaponTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWeaponTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.WeaponTypeProficiencies.AddRange(value);
            return entity;
        }

        public static T ClearActiveFightingStyles<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ActiveFightingStyles.Clear();
            return entity;
        }

        public static T ClearAfterRestActions<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.AfterRestActions.Clear();
            return entity;
        }

        public static T ClearAlignmentOptionaPersonalityFlags<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.AlignmentOptionaPersonalityFlags.Clear();
            return entity;
        }

        public static T ClearArmorCategoryProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ArmorCategoryProficiencies.Clear();
            return entity;
        }

        public static T ClearArmorTypeProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ArmorTypeProficiencies.Clear();
            return entity;
        }

        public static T ClearBackgroundOptionalPersonalityFlags<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.BackgroundOptionalPersonalityFlags.Clear();
            return entity;
        }

        public static T ClearClassesHistory<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ClassesHistory.Clear();
            return entity;
        }

        public static T ClearExpertiseProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ExpertiseProficiencies.Clear();
            return entity;
        }

        public static T ClearFeatProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.FeatProficiencies.Clear();
            return entity;
        }

        public static T ClearLanguageProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.LanguageProficiencies.Clear();
            return entity;
        }

        public static T ClearMetamagicProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.MetamagicProficiencies.Clear();
            return entity;
        }

        public static T ClearSkillProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.SkillProficiencies.Clear();
            return entity;
        }

        public static T ClearToolTypeProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.ToolTypeProficiencies.Clear();
            return entity;
        }

        public static T ClearTrainedExpertises<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedExpertises.Clear();
            return entity;
        }

        public static T ClearTrainedFeats<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedFeats.Clear();
            return entity;
        }

        public static T ClearTrainedFightingStyles<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedFightingStyles.Clear();
            return entity;
        }

        public static T ClearTrainedLanguages<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedLanguages.Clear();
            return entity;
        }

        public static T ClearTrainedMetamagicOptions<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedMetamagicOptions.Clear();
            return entity;
        }

        public static T ClearTrainedSkills<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedSkills.Clear();
            return entity;
        }

        public static T ClearTrainedToolTypes<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.TrainedToolTypes.Clear();
            return entity;
        }

        public static T ClearWeaponCategoryProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.WeaponCategoryProficiencies.Clear();
            return entity;
        }

        public static T ClearWeaponTypeProficiencies<T>(this T entity)
            where T : RulesetCharacterHero
        {
            entity.WeaponTypeProficiencies.Clear();
            return entity;
        }

        public static Dictionary<ItemDefinition, List<FeatureDefinition>> GetActiveItemFeatures<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<Dictionary<ItemDefinition, List<FeatureDefinition>>>("activeItemFeatures");
        }

        public static List<String> GetAlignmentOptionalPersonalityFlags<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<String>>("alignmentOptionalPersonalityFlags");
        }

        public static List<IAttackModificationProvider> GetAttackModifiers<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<IAttackModificationProvider>>("attackModifiers");
        }

        public static Dictionary<ItemDefinition, Int32> GetBonusByItem<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<Dictionary<ItemDefinition, Int32>>("bonusByItem");
        }

        public static List<RulesetItem> GetCarriedItems<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<RulesetItem>>("carriedItems");
        }

        public static Dictionary<CharacterClassDefinition, CharacterSubclassDefinition> GetClassesAndSublasses<T>(
            this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<Dictionary<CharacterClassDefinition, CharacterSubclassDefinition>>(
                "classesAndSublasses");
        }

        public static List<RulesetItem> GetDestroyedItems<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<RulesetItem>>("destroyedItems");
        }

        public static List<FeatureDefinition> GetEquipmentBrowseList<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<FeatureDefinition>>("equipmentBrowseList");
        }

        public static List<String> GetForbiddenArmorTags<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<String>>("forbiddenArmorTags");
        }

        public static List<Int32> GetHitPointsGainHistory<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<Int32>>("hitPointsGainHistory");
        }

        public static List<RulesetAttributeModifier> GetModifiers<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<RulesetAttributeModifier>>("modifiers");
        }

        public static Dictionary<DieType, Int32> GetSpentHitDice<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<Dictionary<DieType, Int32>>("spentHitDice");
        }

        public static Dictionary<String, TagsDefinitions.Criticity> GetTagsMap<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<Dictionary<String, TagsDefinitions.Criticity>>("tagsMap");
        }

        public static List<FeatureDefinition> GetWeaponModifiers<T>(this T entity)
            where T : RulesetCharacterHero
        {
            return entity.GetField<List<FeatureDefinition>>("weaponModifiers");
        }

        public static T SetActiveFightingStyles<T>(this T entity, params FightingStyleDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetActiveFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetActiveFightingStyles<T>(this T entity, IEnumerable<FightingStyleDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.ActiveFightingStyles.SetRange(value);
            return entity;
        }

        public static T SetAdditionalBackstory<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.AdditionalBackstory = value;
            return entity;
        }

        public static T SetAfterRestActions<T>(this T entity, params RestActivityDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetAfterRestActions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAfterRestActions<T>(this T entity, IEnumerable<RestActivityDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.AfterRestActions.SetRange(value);
            return entity;
        }

        public static T SetAlignmentDefinition<T>(this T entity, AlignmentDefinition value)
            where T : RulesetCharacterHero
        {
            entity.SetField("alignmentDefinition", value);
            return entity;
        }

        public static T SetAlignmentOptionaPersonalityFlags<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetAlignmentOptionaPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAlignmentOptionaPersonalityFlags<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.AlignmentOptionaPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetArmorCategoryProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetArmorCategoryProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetArmorCategoryProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ArmorCategoryProficiencies.SetRange(value);
            return entity;
        }

        public static T SetArmorTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetArmorTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetArmorTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ArmorTypeProficiencies.SetRange(value);
            return entity;
        }

        public static T SetAttunementCapital<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("attunementCapital", value);
            return entity;
        }

        public static T SetBackgroundDefinition<T>(this T entity, CharacterBackgroundDefinition value)
            where T : RulesetCharacterHero
        {
            entity.BackgroundDefinition = value;
            return entity;
        }

        public static T SetBackgroundOptionalPersonalityFlags<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetBackgroundOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetBackgroundOptionalPersonalityFlags<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.BackgroundOptionalPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetBackgroundSubType<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.BackgroundSubType = value;
            return entity;
        }

        public static T SetBodyAssetPrefix<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SetField("bodyAssetPrefix", value);
            return entity;
        }

        public static T SetBuildingData<T>(this T entity, CharacterHeroBuildingData value)
            where T : RulesetCharacterHero
        {
            entity.SetField("buildingData", value);
            return entity;
        }

        public static T SetBuiltIn<T>(this T entity, Boolean value)
            where T : RulesetCharacterHero
        {
            entity.BuiltIn = value;
            return entity;
        }

        public static T SetCharacterClassChanged<T>(this T entity,
            RulesetCharacterHero.CharacterClassChangedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<CharacterClassChanged>k__BackingField", value);
            return entity;
        }

        public static T SetCharacterInventory<T>(this T entity, RulesetInventory value)
            where T : RulesetCharacterHero
        {
            entity.CharacterInventory = value;
            return entity;
        }

        public static T SetClassesHistory<T>(this T entity, params CharacterClassDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetClassesHistory(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetClassesHistory<T>(this T entity, IEnumerable<CharacterClassDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.ClassesHistory.SetRange(value);
            return entity;
        }

        public static T SetContextParams<T>(this T entity,
            RulesetImplementationDefinitions.SituationalContextParams value)
            where T : RulesetCharacterHero
        {
            entity.SetField("contextParams", value);
            return entity;
        }

        public static T SetCriticalFailures<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("criticalFailures", value);
            return entity;
        }

        public static T SetCriticalHits<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("criticalHits", value);
            return entity;
        }

        public static T SetDeityDefinition<T>(this T entity, DeityDefinition value)
            where T : RulesetCharacterHero
        {
            entity.DeityDefinition = value;
            return entity;
        }

        public static T SetDeviceAutoIdentified<T>(this T entity,
            RulesetCharacterHero.DeviceAutoIdentifiedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<DeviceAutoIdentified>k__BackingField", value);
            return entity;
        }

        public static T SetDeviceLastChargeUsed<T>(this T entity,
            RulesetCharacterHero.DeviceLastChargeUsedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<DeviceLastChargeUsed>k__BackingField", value);
            return entity;
        }

        public static T SetDisplayBackstory<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SetField("displayBackstory", value);
            return entity;
        }

        public static T SetDisplayName<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SetField("displayName", value);
            return entity;
        }

        public static T SetDisplaySurName<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SetField("displaySurName", value);
            return entity;
        }

        public static T SetEarnedXP<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("earnedXP", value);
            return entity;
        }

        public static T SetEditorOnly<T>(this T entity, Boolean value)
            where T : RulesetCharacterHero
        {
            entity.EditorOnly = value;
            return entity;
        }

        public static T SetExperienceGained<T>(this T entity, RulesetCharacterHero.ExperienceGainedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<ExperienceGained>k__BackingField", value);
            return entity;
        }

        public static T SetExpertiseProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetExpertiseProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetExpertiseProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ExpertiseProficiencies.SetRange(value);
            return entity;
        }

        public static T SetFailedAttacks<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("failedAttacks", value);
            return entity;
        }

        public static T SetFeatProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetFeatProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.FeatProficiencies.SetRange(value);
            return entity;
        }

        public static T SetIgnoreEquipmentOnAbilityScores<T>(this T entity, Boolean value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<IgnoreEquipmentOnAbilityScores>k__BackingField", value);
            return entity;
        }

        public static T SetInflictedDamage<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("inflictedDamage", value);
            return entity;
        }

        public static T SetItemEquipedCallback<T>(this T entity, RulesetCharacterHero.ItemEquipedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<ItemEquipedCallback>k__BackingField", value);
            return entity;
        }

        public static T SetKnockOuts<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("knockOuts", value);
            return entity;
        }

        public static T SetLanguageProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetLanguageProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLanguageProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.LanguageProficiencies.SetRange(value);
            return entity;
        }

        public static T SetLastLongRestDay<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("lastLongRestDay", value);
            return entity;
        }

        public static T SetMetamagicProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetMetamagicProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMetamagicProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.MetamagicProficiencies.SetRange(value);
            return entity;
        }

        public static T SetMorphotypeAssetPrefix<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SetField("morphotypeAssetPrefix", value);
            return entity;
        }

        public static T SetMorphotypeElementAdditionalValueChanged<T>(this T entity,
            RulesetCharacterHero.MorphotypeElementAdditionalValueChangedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<MorphotypeElementAdditionalValueChanged>k__BackingField", value);
            return entity;
        }

        public static T SetMorphotypeElementChanged<T>(this T entity,
            RulesetCharacterHero.MorphotypeElementChangedHandler value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<MorphotypeElementChanged>k__BackingField", value);
            return entity;
        }

        public static T SetName<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.Name = value;
            return entity;
        }

        public static T SetRaceDefinition<T>(this T entity, CharacterRaceDefinition value)
            where T : RulesetCharacterHero
        {
            entity.RaceDefinition = value;
            return entity;
        }

        public static T SetRefreshing<T>(this T entity, Boolean value)
            where T : RulesetCharacterHero
        {
            entity.SetField("refreshing", value);
            return entity;
        }

        public static T SetRemainingSleepHours<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.RemainingSleepHours = value;
            return entity;
        }

        public static T SetRemainingSleepTime<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("remainingSleepTime", value);
            return entity;
        }

        public static T SetRestoredHealth<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("restoredHealth", value);
            return entity;
        }

        public static T SetSkillProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetSkillProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSkillProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.SkillProficiencies.SetRange(value);
            return entity;
        }

        public static T SetSlainEnemies<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("slainEnemies", value);
            return entity;
        }

        public static T SetSubRaceDefinition<T>(this T entity, CharacterRaceDefinition value)
            where T : RulesetCharacterHero
        {
            entity.SubRaceDefinition = value;
            return entity;
        }

        public static T SetSuccessfulAttacks<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("successfulAttacks", value);
            return entity;
        }

        public static T SetSurName<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.SurName = value;
            return entity;
        }

        public static T SetSustainedInjuries<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("sustainedInjuries", value);
            return entity;
        }

        public static T SetTemplateDefinition<T>(this T entity, CharacterTemplateDefinition value)
            where T : RulesetCharacterHero
        {
            entity.TemplateDefinition = value;
            return entity;
        }

        public static T SetToolTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetToolTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetToolTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.ToolTypeProficiencies.SetRange(value);
            return entity;
        }

        public static T SetTrainedExpertises<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedExpertises(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedExpertises<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedExpertises.SetRange(value);
            return entity;
        }

        public static T SetTrainedFeats<T>(this T entity, params FeatDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedFeats(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedFeats<T>(this T entity, IEnumerable<FeatDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedFeats.SetRange(value);
            return entity;
        }

        public static T SetTrainedFightingStyles<T>(this T entity, params FightingStyleDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedFightingStyles<T>(this T entity, IEnumerable<FightingStyleDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedFightingStyles.SetRange(value);
            return entity;
        }

        public static T SetTrainedLanguages<T>(this T entity, params LanguageDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedLanguages<T>(this T entity, IEnumerable<LanguageDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedLanguages.SetRange(value);
            return entity;
        }

        public static T SetTrainedMetamagicOptions<T>(this T entity, params MetamagicOptionDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedMetamagicOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedMetamagicOptions<T>(this T entity, IEnumerable<MetamagicOptionDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedMetamagicOptions.SetRange(value);
            return entity;
        }

        public static T SetTrainedSkills<T>(this T entity, params SkillDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedSkills(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedSkills<T>(this T entity, IEnumerable<SkillDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedSkills.SetRange(value);
            return entity;
        }

        public static T SetTrainedToolTypes<T>(this T entity, params ToolTypeDefinition[] value)
            where T : RulesetCharacterHero
        {
            SetTrainedToolTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrainedToolTypes<T>(this T entity, IEnumerable<ToolTypeDefinition> value)
            where T : RulesetCharacterHero
        {
            entity.TrainedToolTypes.SetRange(value);
            return entity;
        }

        public static T SetTravelledCells<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("travelledCells", value);
            return entity;
        }

        public static T SetTreasury<T>(this T entity, RulesetTreasury value)
            where T : RulesetCharacterHero
        {
            entity.SetField("treasury", value);
            return entity;
        }

        public static T SetUsableDeviceFromMenu<T>(this T entity, RulesetItemDevice value)
            where T : RulesetCharacterHero
        {
            entity.SetField("<UsableDeviceFromMenu>k__BackingField", value);
            return entity;
        }

        public static T SetUsedMagicAndPowers<T>(this T entity, Int32 value)
            where T : RulesetCharacterHero
        {
            entity.SetField("usedMagicAndPowers", value);
            return entity;
        }

        public static T SetVoiceID<T>(this T entity, String value)
            where T : RulesetCharacterHero
        {
            entity.VoiceID = value;
            return entity;
        }

        public static T SetWeaponCategoryProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetWeaponCategoryProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWeaponCategoryProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.WeaponCategoryProficiencies.SetRange(value);
            return entity;
        }

        public static T SetWeaponTypeProficiencies<T>(this T entity, params String[] value)
            where T : RulesetCharacterHero
        {
            SetWeaponTypeProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWeaponTypeProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : RulesetCharacterHero
        {
            entity.WeaponTypeProficiencies.SetRange(value);
            return entity;
        }
    }
}
