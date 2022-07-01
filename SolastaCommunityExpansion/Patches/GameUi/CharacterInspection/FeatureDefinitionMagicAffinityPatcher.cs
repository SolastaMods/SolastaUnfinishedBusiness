using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

internal static class FeatureDefinitionDescriptionPatcher
{
    private static string FormatSpellLevel(int level)
    {
        return Gui.Colorize(level == 0 ? "0" : Gui.ToRoman(level), Gui.ColorHighEmphasis);
    }

    [HarmonyPatch(typeof(FeatureDefinitionAutoPreparedSpells), "FormatDescription")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDefinitionAutoPreparedSpells_FormatDescription
    {
        internal static bool Prefix(FeatureDefinitionAutoPreparedSpells __instance, ref string __result)
        {
            if (!Main.Settings.EnableEnhancedCharacterInspection)
            {
                return true;
            }

            var spells = new Dictionary<int, List<SpellDefinition>>();

            foreach (var group in __instance.AutoPreparedSpellsGroups)
            {
                foreach (var spell in group.SpellsList)
                {
                    var spellLevel = spell.SpellLevel;
                    if (!spells.ContainsKey(spellLevel))
                    {
                        spells.Add(spellLevel, new List<SpellDefinition>());
                    }

                    spells[spellLevel].Add(spell);
                }
            }

            var result = string.Join("\n", spells.Select(e =>
                $"{FormatSpellLevel(e.Key)}\t{string.Join(", ", e.Value.Select(s => s.FormatTitle()))}"));

            var description = __instance.GuiPresentation.Description;

            description = description == Gui.NoLocalization ? string.Empty : Gui.Localize(description);

            __result = Gui.Format(description, result);

            return false;
        }
    }

    // formats spell list into list with spell levels, instead of 1 line of all spells like default does
    [HarmonyPatch(typeof(FeatureDefinitionMagicAffinity), "FormatDescription")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDefinitionMagicAffinity_FormatDescription
    {
        public static string FormatSpellList(FeatureDefinitionMagicAffinity instance)
        {
            var description = instance.GuiPresentation.Description;
            var spells = new Dictionary<int, List<SpellDefinition>>();
            var levels = new HashSet<int>();

            foreach (var duplet in instance.ExtendedSpellList.SpellsByLevel)
            {
                if (!spells.ContainsKey(duplet.Level))
                {
                    spells.Add(duplet.Level, new List<SpellDefinition>());
                    levels.Add(duplet.Level);
                }

                spells[duplet.Level].AddRange(duplet.Spells);
            }

            foreach (var level in levels
                         .Where(level => spells.ContainsKey(level) && spells[level].Empty()))
            {
                spells.Remove(level);
            }

            var spellList = "\n" + string.Join("\n",
                spells.Select(e =>
                    $"{FormatSpellLevel(e.Key)}\t{string.Join(", ", e.Value.Select(s => s.FormatTitle()))}"));

            return Gui.Format(Gui.Localize(description), spellList);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableEnhancedCharacterInspection)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                }

                yield break;
            }

            var formatMethod = typeof(Gui).GetMethod("Format", BindingFlags.Static | BindingFlags.Public);
            var myFormatMethod =
                typeof(FeatureDefinitionMagicAffinity_FormatDescription).GetMethod("FormatSpellList");
            var found = 0;

            foreach (var instruction in instructions)
            {
                // find first call to Gui.Format and replace it with oour custom method
                if (instruction.Calls(formatMethod) && ++found == 1)
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, myFormatMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
