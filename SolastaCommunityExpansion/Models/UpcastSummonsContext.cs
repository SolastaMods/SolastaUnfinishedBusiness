using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class UpcastSummonsContext
    {
        public static void Load()
        {
            if (!Main.Settings.EnableUpcastConjureElemental)
            {
                return;
            }

            // TODO: Add CR 6/7 versions of Earth/Fire/Air elementals
            // e.g. "Fire_Elemental_CE_Large", "Fire_Elemental_CE_Huge"

            AddUpcastSummons(ConjureElementalAir, "Giant_Frost", "Young_GreenDragon");
            AddUpcastSummons(ConjureElementalEarth, "Giant_Hill", "Giant_Stone");
            AddUpcastSummons(ConjureElementalFire, "Giant_Fire", "Young_BlackDragon");

            void AddUpcastSummons(SpellDefinition definition, params string[] monsterDefinitionNames)
            {
                var description = definition.EffectDescription;
                var advancement = description.EffectAdvancement;

                advancement.SetEffectIncrementMethod(EffectIncrementMethod.PerAdditionalSlotLevel);
                advancement.SetAdditionalSpellLevelPerIncrement(1);

                var effectForm = description.EffectForms[0];
                description.EffectForms[0] = UpcastSummonEffectForm.Build(effectForm, definition.SpellLevel, monsterDefinitionNames);
            }
        }
    }

    /// <summary>
    /// TODO: not sure where to put this class
    /// </summary>
    internal sealed class UpcastSummonEffectForm : EffectForm
    {
        public string OriginalMonsterDefinitionName { get; }
        public int OriginalSpellLevel { get; }
        public string[] UpcastMonsterDefinitionNames { get; }

        private UpcastSummonEffectForm(EffectForm effectForm, int originalSpellLevel, params string[] upcastMonsterDefinitionNames)
        {
            effectForm.Copy(this);

            if (effectForm.FormType != EffectFormType.Summon)
            {
                // throw or log?
                Main.Log($"{effectForm.FormType} is not supported");
                return;
            }

            if (upcastMonsterDefinitionNames.Length == 0)
            {
                // throw or log?
                Main.Log($"At least one higher level monster definition name required");
                return;
            }

            OriginalMonsterDefinitionName = effectForm.SummonForm.MonsterDefinitionName;
            OriginalSpellLevel = originalSpellLevel;
            UpcastMonsterDefinitionNames = upcastMonsterDefinitionNames;
        }

        public static EffectForm Build(EffectForm effectForm, int effectiveSpellLevel, params string[] upcastMonsterDefinitionNames)
        {
            return new UpcastSummonEffectForm(effectForm, effectiveSpellLevel, upcastMonsterDefinitionNames);
        }

        public void ApplySpellLevel(int effectiveLevel)
        {
            if (string.IsNullOrEmpty(OriginalMonsterDefinitionName))
            {
                Main.Log($"UpcastSummon-ApplySpellLevel: not initialized - ignoring");

                return;
            }

            if (effectiveLevel <= OriginalSpellLevel)
            {
                Main.Log($"UpcastSummon-ApplySpellLevel: {effectiveLevel} <= {OriginalSpellLevel} - ignoring");

                return;
            }

            if (effectiveLevel - OriginalSpellLevel > UpcastMonsterDefinitionNames.Length)
            {
                Main.Log($"UpcastSummon-ApplySpellLevel: {effectiveLevel}, no suitable upcast - ignoring");

                return;
            }

            var upcastMonsterName = UpcastMonsterDefinitionNames[effectiveLevel - OriginalSpellLevel - 1];

            if (DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(upcastMonsterName, out var _))
            {
                SummonForm.SetMonsterDefinitionName(upcastMonsterName);
            }
            else
            {
                Main.Log($"UpcastSummon-ApplySpellLevel: {upcastMonsterName}, not found - ignoring");
            }
        }

        public void Restore()
        {
            if (!string.IsNullOrEmpty(OriginalMonsterDefinitionName))
            {
                SummonForm.SetMonsterDefinitionName(OriginalMonsterDefinitionName);
            }
        }
    }
}
