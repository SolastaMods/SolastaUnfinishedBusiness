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
    [TargetType(typeof(CharacterTemplateDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class CharacterTemplateDefinitionExtensions
    {
        public static T AddEquipment<T>(this T entity, params ItemDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddEquipment(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEquipment<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.Equipment.AddRange(value);
            return entity;
        }

        public static T AddExpertisesOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddExpertisesOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddExpertisesOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.ExpertisesOverride.AddRange(value);
            return entity;
        }

        public static T AddFeatsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddFeatsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.FeatsOverride.AddRange(value);
            return entity;
        }

        public static T AddKnownClassCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownClassCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownClassCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassCantrips.AddRange(value);
            return entity;
        }

        public static T AddKnownClassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownClassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownClassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassSpells.AddRange(value);
            return entity;
        }

        public static T AddKnownRaceCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownRaceCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownRaceCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceCantrips.AddRange(value);
            return entity;
        }

        public static T AddKnownRaceSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownRaceSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownRaceSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceSpells.AddRange(value);
            return entity;
        }

        public static T AddKnownSubclassCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownSubclassCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownSubclassCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassCantrips.AddRange(value);
            return entity;
        }

        public static T AddKnownSubclassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddKnownSubclassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownSubclassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassSpells.AddRange(value);
            return entity;
        }

        public static T AddLanguagesOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddLanguagesOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLanguagesOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.LanguagesOverride.AddRange(value);
            return entity;
        }

        public static T AddMetamagicOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddMetamagicOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMetamagicOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.MetamagicOverride.AddRange(value);
            return entity;
        }

        public static T AddPreparedClassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddPreparedClassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPreparedClassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedClassSpells.AddRange(value);
            return entity;
        }

        public static T AddPreparedSubclassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            AddPreparedSubclassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPreparedSubclassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedSubclassSpells.AddRange(value);
            return entity;
        }

        public static T AddSkillsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddSkillsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSkillsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.SkillsOverride.AddRange(value);
            return entity;
        }

        public static T AddToolsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            AddToolsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddToolsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.ToolsOverride.AddRange(value);
            return entity;
        }

        public static T AddWieldedItemsConfigurations<T>(this T entity, params WieldedItemsConfiguration[] value)
            where T : CharacterTemplateDefinition
        {
            AddWieldedItemsConfigurations(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWieldedItemsConfigurations<T>(this T entity, IEnumerable<WieldedItemsConfiguration> value)
            where T : CharacterTemplateDefinition
        {
            entity.WieldedItemsConfigurations.AddRange(value);
            return entity;
        }

        public static T ClearEquipment<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.Equipment.Clear();
            return entity;
        }

        public static T ClearExpertisesOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.ExpertisesOverride.Clear();
            return entity;
        }

        public static T ClearFeatsOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.FeatsOverride.Clear();
            return entity;
        }

        public static T ClearKnownClassCantrips<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassCantrips.Clear();
            return entity;
        }

        public static T ClearKnownClassSpells<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassSpells.Clear();
            return entity;
        }

        public static T ClearKnownRaceCantrips<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceCantrips.Clear();
            return entity;
        }

        public static T ClearKnownRaceSpells<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceSpells.Clear();
            return entity;
        }

        public static T ClearKnownSubclassCantrips<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassCantrips.Clear();
            return entity;
        }

        public static T ClearKnownSubclassSpells<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassSpells.Clear();
            return entity;
        }

        public static T ClearLanguagesOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.LanguagesOverride.Clear();
            return entity;
        }

        public static T ClearMetamagicOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.MetamagicOverride.Clear();
            return entity;
        }

        public static T ClearPreparedClassSpells<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedClassSpells.Clear();
            return entity;
        }

        public static T ClearPreparedSubclassSpells<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedSubclassSpells.Clear();
            return entity;
        }

        public static T ClearSkillsOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.SkillsOverride.Clear();
            return entity;
        }

        public static T ClearToolsOverride<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.ToolsOverride.Clear();
            return entity;
        }

        public static T ClearWieldedItemsConfigurations<T>(this T entity)
            where T : CharacterTemplateDefinition
        {
            entity.WieldedItemsConfigurations.Clear();
            return entity;
        }

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

        public static T SetEquipment<T>(this T entity, params ItemDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetEquipment(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEquipment<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.Equipment.SetRange(value);
            return entity;
        }

        public static T SetExpertisesOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetExpertisesOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetExpertisesOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.ExpertisesOverride.SetRange(value);
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

        public static T SetFeatsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetFeatsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.FeatsOverride.SetRange(value);
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

        public static T SetKnownClassCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownClassCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownClassCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassCantrips.SetRange(value);
            return entity;
        }

        public static T SetKnownClassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownClassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownClassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownClassSpells.SetRange(value);
            return entity;
        }

        public static T SetKnownRaceCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownRaceCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownRaceCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceCantrips.SetRange(value);
            return entity;
        }

        public static T SetKnownRaceSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownRaceSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownRaceSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownRaceSpells.SetRange(value);
            return entity;
        }

        public static T SetKnownSubclassCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownSubclassCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownSubclassCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassCantrips.SetRange(value);
            return entity;
        }

        public static T SetKnownSubclassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetKnownSubclassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownSubclassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.KnownSubclassSpells.SetRange(value);
            return entity;
        }

        public static T SetLanguagesOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetLanguagesOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLanguagesOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.LanguagesOverride.SetRange(value);
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

        public static T SetMetamagicOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetMetamagicOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMetamagicOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.MetamagicOverride.SetRange(value);
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

        public static T SetPreparedClassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetPreparedClassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPreparedClassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedClassSpells.SetRange(value);
            return entity;
        }

        public static T SetPreparedSubclassSpells<T>(this T entity, params SpellDefinition[] value)
            where T : CharacterTemplateDefinition
        {
            SetPreparedSubclassSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPreparedSubclassSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : CharacterTemplateDefinition
        {
            entity.PreparedSubclassSpells.SetRange(value);
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

        public static T SetSex<T>(this T entity, CreatureSex value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("sex", value);
            return entity;
        }

        public static T SetSkillsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetSkillsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSkillsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.SkillsOverride.SetRange(value);
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

        public static T SetToolsOverride<T>(this T entity, params System.String[] value)
            where T : CharacterTemplateDefinition
        {
            SetToolsOverride(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetToolsOverride<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterTemplateDefinition
        {
            entity.ToolsOverride.SetRange(value);
            return entity;
        }

        public static T SetVoiceId<T>(this T entity, System.String value)
            where T : CharacterTemplateDefinition
        {
            entity.SetField("voiceId", value);
            return entity;
        }

        public static T SetWieldedItemsConfigurations<T>(this T entity, params WieldedItemsConfiguration[] value)
            where T : CharacterTemplateDefinition
        {
            SetWieldedItemsConfigurations(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWieldedItemsConfigurations<T>(this T entity, IEnumerable<WieldedItemsConfiguration> value)
            where T : CharacterTemplateDefinition
        {
            entity.WieldedItemsConfigurations.SetRange(value);
            return entity;
        }
    }
}
