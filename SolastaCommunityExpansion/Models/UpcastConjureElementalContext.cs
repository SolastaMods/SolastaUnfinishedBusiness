using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static EffectForm;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class UpcastConjureElementalContext
    {
        private static Dictionary<SummonForm, UpcastSummonInfo> UpcastInfo { get; } = new Dictionary<SummonForm, UpcastSummonInfo>();

        private static readonly Guid Namespace = new Guid("de4539b8e0194684b1d0585100dd94e5");

        private const string FireElementalCR6Name = "FireElementalCE_CR6";
        private const string AirElementalCR6Name = "AirElementalCE_CR6";
        private const string EarthElementalCR6Name = "EarthElementalCE_CR6";

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

            AddUpcastSummons(ConjureElementalAir, AirElementalCR6Name);
            AddUpcastSummons(ConjureElementalEarth, EarthElementalCR6Name);
            AddUpcastSummons(ConjureElementalFire, FireElementalCR6Name);

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
            // Not in DM, not in bestiary.  Purely for summons purposes.

            // Fire
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(FireElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(FireElementalCR6Name,
                    "Fire Elemental (CR6)", MonsterDefinitions.Fire_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(42)
                    .SetStandardHitPoints(77 + 42)
                    .SetAbilityScores(12, 17, 16, 6, 10, 7)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }

            // Air
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(AirElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(AirElementalCR6Name,
                    "Air Elemental (CR6)", MonsterDefinitions.Air_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(28)
                    .SetStandardHitPoints(77 + 28)
                    .SetAbilityScores(16, 20, 14, 6, 10, 6)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }

            // Earth
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(EarthElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(EarthElementalCR6Name,
                    "Earth Elemental (CR6)", MonsterDefinitions.Earth_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(60)
                    .SetStandardHitPoints(77 + 60)
                    .SetAbilityScores(22, 8, 20, 5, 10, 5)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }

            // Helpers

            MonsterBuilder GetMonsterBuilder(string name, string title, MonsterDefinition baseMonster)
            {
                return new MonsterBuilder(name, CreateGuid(name), title, baseMonster.GuiPresentation.Description, baseMonster);
            }

            string CreateGuid(string name) => GuidHelper.Create(Namespace, name).ToString("N");

            MonsterAttackIteration CreateAttackIteration(MonsterAttackIteration attackIteration, string namePrefix, int attacks = 2)
            {
                // copy existing attack iteration and bump up ToHitBonus and DamageBonus by 1
                var attackDefinition = CreateAttackDefinition(attackIteration.MonsterAttackDefinition, namePrefix);

                return new MonsterAttackIteration(attackDefinition, attacks);
            }

            MonsterAttackDefinition CreateAttackDefinition(MonsterAttackDefinition attackDefinition, string namePrefix)
            {
                var name = $"{namePrefix}_{attackDefinition.Name}";

                var builder = new MonsterAttackDefinitionBuilder(name, CreateGuid(name), attackDefinition);

                builder.SetDamageBonus(4);
                builder.SetToHitBonus(7);

                return builder.AddToDB();
            }
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
                #endregion

                var upcastMonsterName = upcastSummonInfo.OriginalMonsterDefinitionName;

                if (effectiveLevel > upcastSummonInfo.OriginalSpellLevel)
                {
                    if (effectiveLevel - upcastSummonInfo.OriginalSpellLevel > upcastSummonInfo.UpcastMonsterDefinitionNames.Length)
                    {
                        Main.Log($"UpcastSummon-ApplySpellLevel: {effectiveLevel} no suitable monster - using highest available.");
                        upcastMonsterName = upcastSummonInfo.UpcastMonsterDefinitionNames.Last();
                    }
                    else
                    {
                        upcastMonsterName = upcastSummonInfo.UpcastMonsterDefinitionNames[effectiveLevel - upcastSummonInfo.OriginalSpellLevel - 1];
                    }
                }

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
