using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using static EffectForm;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class UpcastSummonsContext
    {
        private static Dictionary<SummonForm, UpcastSummonInfo> UpcastInfo { get; } = new Dictionary<SummonForm, UpcastSummonInfo>();

        private static readonly Guid Namespace = new Guid("de4539b8e0194684b1d0585100dd94e5");

        private const string FireElementalCR6Name = "FireElementalCE_CR6";
        private const string FireElementalCR7Name = "FireElementalCE_CR7";

        private const string AirElementalCR6Name = "AirElementalCE_CR6";
        private const string AirElementalCR7Name = "AirElementalCE_CR7";

        private const string EarthElementalCR6Name = "EarthElementalCE_CR6";
        private const string EarthElementalCR7Name = "EarthElementalCE_CR7";

        public static void Load()
        {
            if (!Main.Settings.EnableUpcastConjureElemental)
            {
                ResetAdvancement(ConjureElemental);
                UpcastInfo.Keys.ToList().ForEach(RestoreStandardSummon);
                UpcastInfo.Clear();
                return;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Conjure Elemental
            ConfigureAdvancement(ConjureElemental);

            CreateHigherLevelSummons();

            AddUpcastSummons(ConjureElementalAir, AirElementalCR6Name, AirElementalCR7Name);
            AddUpcastSummons(ConjureElementalEarth, EarthElementalCR6Name, EarthElementalCR7Name);
            AddUpcastSummons(ConjureElementalFire, FireElementalCR6Name, FireElementalCR7Name);

            // TODO: same for Fey

            // Set advancement at spell level,not sub-spell
            void ConfigureAdvancement(SpellDefinition definition)
            {
                var advancement = definition.EffectDescription.EffectAdvancement;
                advancement.SetEffectIncrementMethod(EffectIncrementMethod.PerAdditionalSlotLevel);
                advancement.SetAdditionalSpellLevelPerIncrement(1);
            }

            void ResetAdvancement(SpellDefinition definition)
            {
                // NOTE: this assumes that default spell doesn't support advancement
                var advancement = definition.EffectDescription.EffectAdvancement;
                advancement.SetEffectIncrementMethod(EffectIncrementMethod.None);
                advancement.SetAdditionalSpellLevelPerIncrement(0);
            }

            void AddUpcastSummons(SpellDefinition definition, params string[] upcastMonsterDefinitionNames)
            {
                var description = definition.EffectDescription;

                var effectForm = description.EffectForms[0];

                var summonForm = effectForm.SummonForm;

                if (!UpcastInfo.ContainsKey(summonForm))
                {
                    if (effectForm.FormType != EffectFormType.Summon)
                    {
                        Main.Log($"{effectForm.FormType} is not supported.");
                        throw new SolastaModApiException($"UpcastSummonsContext: {effectForm.FormType} is not supported.");
                    }

                    if (upcastMonsterDefinitionNames.Length == 0)
                    {
                        Main.Log($"At least one higher level monster definition name required.");
                        throw new SolastaModApiException($"UpcastSummonsContext: At least one higher level monster definition name required.");
                    }

                    UpcastInfo[summonForm] = new UpcastSummonInfo(summonForm, definition.SpellLevel, upcastMonsterDefinitionNames);
                }
            }
        }

        internal static void CreateHigherLevelSummons()
        {
            // Quick and dirty :)
            // Not in DM, not in bestiary.  Purely for summons purposes.

            // TODO: localization, a description, refine (increase attack bonus, more attacks etc)

            // Fire

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(FireElementalCR6Name, out var _))
            {
                var builder = GetBuilder(FireElementalCR6Name,
                    "Large Fire Elemental", "description", MonsterDefinitions.Fire_Elemental);

                builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(42)
                    .SetStandardHitPoints(77 + 42)
                    .SetAbilityScores(12, 17, 16, 6, 10, 7)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(FireElementalCR7Name, out var _))
            {
                var builder = GetBuilder(FireElementalCR7Name,
                    "Huge Fire Elemental", "description", MonsterDefinitions.Fire_Elemental);

                // TODO: 3 attacks?

                builder
                    .SetHitDiceNumber(16)
                    .SetHitPointsBonus(48)
                    .SetStandardHitPoints(88 + 48)
                    .SetAbilityScores(14, 17, 16, 6, 10, 7)
                    .SetModelScale(0.9f)
                    .SetChallengeRating(7)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            // Air

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(AirElementalCR6Name, out var _))
            {
                var builder = GetBuilder(AirElementalCR6Name,
                    "Large Air Elemental", "description", MonsterDefinitions.Air_Elemental);

                builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(28)
                    .SetStandardHitPoints(77 + 28)
                    .SetAbilityScores(16, 20, 14, 6, 10, 6)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(AirElementalCR7Name, out var _))
            {
                var builder = GetBuilder(AirElementalCR7Name,
                    "Huge Air Elemental", "description", MonsterDefinitions.Air_Elemental);

                // TODO: 3 attacks?

                builder
                    .SetHitDiceNumber(16)
                    .SetHitPointsBonus(32)
                    .SetStandardHitPoints(88 + 32)
                    .SetAbilityScores(18, 20, 14, 6, 10, 6)
                    .SetModelScale(0.9f)
                    .SetChallengeRating(7)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            // Earth

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(EarthElementalCR6Name, out var _))
            {
                var builder = GetBuilder(EarthElementalCR6Name,
                    "Large Earth Elemental", "description", MonsterDefinitions.Earth_Elemental);

                builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(60)
                    .SetStandardHitPoints(77 + 60)
                    .SetAbilityScores(22, 8, 20, 5, 10, 5)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(EarthElementalCR7Name, out var _))
            {
                var builder = GetBuilder(EarthElementalCR7Name,
                    "Huge Earth Elemental", "description", MonsterDefinitions.Earth_Elemental);

                // TODO: 3 attacks?

                builder
                    .SetHitDiceNumber(16)
                    .SetHitPointsBonus(60)
                    .SetStandardHitPoints(88 + 60)
                    .SetAbilityScores(24, 8, 20, 5, 10, 5)
                    .SetModelScale(0.9f)
                    .SetChallengeRating(7)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();
            }

            // Helpers

            MonsterBuilder GetBuilder(string name, string title, string description, MonsterDefinition baseMonster)
            {
                return new MonsterBuilder(name, CreateGuid(name), title, description, baseMonster);
            }

            string CreateGuid(string name) => GuidHelper.Create(Namespace, name).ToString("N");
        }

        internal static void ApplyUpcastSummon(EffectForm effectForm, int effectiveLevel)
        {
            if (!Main.Settings.EnableUpcastConjureElemental
                || effectForm.FormType != EffectFormType.Summon)
            {
                return;
            }

            var summonForm = effectForm.SummonForm;

            if (UpcastInfo.TryGetValue(summonForm, out var upcastSummonInfo))
            {
                #region Preconditions
                if (string.IsNullOrEmpty(upcastSummonInfo.OriginalMonsterDefinitionName))
                {
                    Main.Log($"UpcastSummon-ApplySpellLevel: not initialized - ignoring");
                    return;
                }

                if (effectiveLevel <= upcastSummonInfo.OriginalSpellLevel)
                {
                    Main.Log($"UpcastSummon-ApplySpellLevel: {effectiveLevel} <= {upcastSummonInfo.OriginalSpellLevel} - ignoring");
                    return;
                }

                if (effectiveLevel - upcastSummonInfo.OriginalSpellLevel > upcastSummonInfo.UpcastMonsterDefinitionNames.Length)
                {
                    Main.Log($"UpcastSummon-ApplySpellLevel: {effectiveLevel}, no suitable upcast - ignoring");
                    return;
                }
                #endregion

                var upcastMonsterName = upcastSummonInfo.UpcastMonsterDefinitionNames[effectiveLevel - upcastSummonInfo.OriginalSpellLevel - 1];

                if (DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(upcastMonsterName, out var _))
                {
                    summonForm.SetMonsterDefinitionName(upcastMonsterName);
                }
                else
                {
                    Main.Log($"UpcastSummon-ApplySpellLevel: {upcastMonsterName}, not found - ignoring");
                }
            }
        }

        internal static void RestoreStandardSummon(EffectForm effectForm)
        {
            if (!Main.Settings.EnableUpcastConjureElemental
                || effectForm.FormType != EffectFormType.Summon)
            {
                return;
            }

            RestoreStandardSummon(effectForm.SummonForm);
        }

        private static void RestoreStandardSummon(SummonForm summonForm)
        {
            if (UpcastInfo.TryGetValue(summonForm, out var upcastSummonInfo)
                && !string.IsNullOrEmpty(upcastSummonInfo.OriginalMonsterDefinitionName))
            {
                summonForm.SetMonsterDefinitionName(upcastSummonInfo.OriginalMonsterDefinitionName);
            }
        }

        private sealed class UpcastSummonInfo
        {
            public string OriginalMonsterDefinitionName { get; }
            public int OriginalSpellLevel { get; }
            public string[] UpcastMonsterDefinitionNames { get; }

            internal UpcastSummonInfo(SummonForm summonForm, int originalSpellLevel, params string[] upcastMonsterDefinitionNames)
            {
                OriginalMonsterDefinitionName = summonForm.MonsterDefinitionName;
                OriginalSpellLevel = originalSpellLevel;
                UpcastMonsterDefinitionNames = upcastMonsterDefinitionNames;
            }
        }
    }
}
