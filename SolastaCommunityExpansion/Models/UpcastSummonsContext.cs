using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using static EffectForm;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class UpcastSummonsContext
    {
        private static Dictionary<SummonForm, UpcastSummonInfo> UpcastInfo { get; } = new Dictionary<SummonForm, UpcastSummonInfo>();

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
            // TODO: ConfigureAdvancement(ConjureFey);

            // TODO: Add CR 6/7 versions of Earth/Fire/Air elementals
            // e.g. "Fire_Elemental_CE_Large", "Fire_Elemental_CE_Huge"
            // TODO: once we have those, make them FullyControlledWhenAllied 

            AddUpcastSummons(ConjureElementalAir, "Giant_Frost", "Young_GreenDragon");
            AddUpcastSummons(ConjureElementalEarth, "Giant_Hill", "Giant_Stone");
            AddUpcastSummons(ConjureElementalFire, "Giant_Fire", "Young_BlackDragon");

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
