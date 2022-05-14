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
    [TargetType(typeof(CharacterRaceDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterRaceDefinitionExtensions
    {
        public static T AddAudioSwitches<T>(this T entity, params AK.Wwise.Switch[] value)
            where T : CharacterRaceDefinition
        {
            AddAudioSwitches(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAudioSwitches<T>(this T entity, IEnumerable<AK.Wwise.Switch> value)
            where T : CharacterRaceDefinition
        {
            entity.AudioSwitches.AddRange(value);
            return entity;
        }

        public static T AddFeatureUnlocks<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : CharacterRaceDefinition
        {
            AddFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterRaceDefinition
        {
            entity.FeatureUnlocks.AddRange(value);
            return entity;
        }

        public static T AddLanguageAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterRaceDefinition
        {
            AddLanguageAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLanguageAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterRaceDefinition
        {
            entity.LanguageAutolearnPreference.AddRange(value);
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, params PersonalityFlagOccurence[] value)
            where T : CharacterRaceDefinition
        {
            AddPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterRaceDefinition
        {
            entity.PersonalityFlagOccurences.AddRange(value);
            return entity;
        }

        public static T AddSubRaces<T>(this T entity, params CharacterRaceDefinition[] value)
            where T : CharacterRaceDefinition
        {
            AddSubRaces(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSubRaces<T>(this T entity, IEnumerable<CharacterRaceDefinition> value)
            where T : CharacterRaceDefinition
        {
            entity.SubRaces.AddRange(value);
            return entity;
        }

        public static T ClearAudioSwitches<T>(this T entity)
            where T : CharacterRaceDefinition
        {
            entity.AudioSwitches.Clear();
            return entity;
        }

        public static T ClearFeatureUnlocks<T>(this T entity)
            where T : CharacterRaceDefinition
        {
            entity.FeatureUnlocks.Clear();
            return entity;
        }

        public static T ClearLanguageAutolearnPreference<T>(this T entity)
            where T : CharacterRaceDefinition
        {
            entity.LanguageAutolearnPreference.Clear();
            return entity;
        }

        public static T ClearPersonalityFlagOccurences<T>(this T entity)
            where T : CharacterRaceDefinition
        {
            entity.PersonalityFlagOccurences.Clear();
            return entity;
        }

        public static T ClearSubRaces<T>(this T entity)
            where T : CharacterRaceDefinition
        {
            entity.SubRaces.Clear();
            return entity;
        }

        public static T SetAudioRaceRTPCValue<T>(this T entity, System.Single value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("audioRaceRTPCValue", value);
            return entity;
        }

        public static T SetAudioSwitches<T>(this T entity, params AK.Wwise.Switch[] value)
            where T : CharacterRaceDefinition
        {
            SetAudioSwitches(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAudioSwitches<T>(this T entity, IEnumerable<AK.Wwise.Switch> value)
            where T : CharacterRaceDefinition
        {
            entity.AudioSwitches.SetRange(value);
            return entity;
        }

        public static T SetBaseHeight<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("baseHeight", value);
            return entity;
        }

        public static T SetBaseWeight<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("baseWeight", value);
            return entity;
        }

        public static T SetDefaultAlignement<T>(this T entity, System.String value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("defaultAlignement", value);
            return entity;
        }

        public static T SetDualSex<T>(this T entity, System.Boolean value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("dualSex", value);
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : CharacterRaceDefinition
        {
            SetFeatureUnlocks(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureUnlocks<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : CharacterRaceDefinition
        {
            entity.FeatureUnlocks.SetRange(value);
            return entity;
        }

        public static T SetInventoryDefinition<T>(this T entity, InventoryDefinition value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("inventoryDefinition", value);
            return entity;
        }

        public static T SetLanguageAutolearnPreference<T>(this T entity, params System.String[] value)
            where T : CharacterRaceDefinition
        {
            SetLanguageAutolearnPreference(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLanguageAutolearnPreference<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterRaceDefinition
        {
            entity.LanguageAutolearnPreference.SetRange(value);
            return entity;
        }

        public static T SetMaximalAge<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("maximalAge", value);
            return entity;
        }

        public static T SetMinimalAge<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("minimalAge", value);
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, params PersonalityFlagOccurence[] value)
            where T : CharacterRaceDefinition
        {
            SetPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterRaceDefinition
        {
            entity.PersonalityFlagOccurences.SetRange(value);
            return entity;
        }

        public static T SetRacePresentation<T>(this T entity, RacePresentation value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("racePresentation", value);
            return entity;
        }

        public static T SetSizeDefinition<T>(this T entity, CharacterSizeDefinition value)
            where T : CharacterRaceDefinition
        {
            entity.SizeDefinition = value;
            return entity;
        }

        public static T SetSubRaces<T>(this T entity, params CharacterRaceDefinition[] value)
            where T : CharacterRaceDefinition
        {
            SetSubRaces(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSubRaces<T>(this T entity, IEnumerable<CharacterRaceDefinition> value)
            where T : CharacterRaceDefinition
        {
            entity.SubRaces.SetRange(value);
            return entity;
        }
    }
}