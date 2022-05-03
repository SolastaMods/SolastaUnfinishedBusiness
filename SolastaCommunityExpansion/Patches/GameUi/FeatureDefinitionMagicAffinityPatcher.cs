using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi
{
    internal static class FeatureDefinitionDescriptionPatcher
    {
        private static string FormatSpellLevel(int level)
        {
            return Gui.Colorize((level == 0 ? "0" : Gui.ToRoman(level)), Gui.ColorHighEmphasis);
        }

        [HarmonyPatch(typeof(FeatureDefinitionAutoPreparedSpells), "FormatDescription")]
        internal static class FeatureDefinitionAutoPreparedSpells_FormatDescription
        {
            internal static bool Prefix(FeatureDefinitionAutoPreparedSpells __instance, ref string __result)
            {
                string empty1 = string.Empty;
                foreach (var group in __instance.AutoPreparedSpellsGroups)
                {
                    var spells = string.Join(", ", group.SpellsList.Select(s => s.FormatTitle()));
                    string str = $"{FormatSpellLevel(group.ClassLevel)}\t{spells}";
                    if (!string.IsNullOrEmpty(empty1))
                        empty1 += "\n";
                    empty1 += str;
                }

                var description = __instance.GuiPresentation.Description;
                description = description == Gui.NoLocalization ? string.Empty : Gui.Localize(description);

                __result = Gui.Format(description, empty1);
                return false;
            }
        }

        [HarmonyPatch(typeof(FeatureDefinitionMagicAffinity), "FormatDescription")]
        internal static class FeatureDefinitionMagicAffinity_FormatDescription
        {
            /**Formats spell list into list with spell levels, instead of 1 line of all spells like default does*/
            public static string FormatSpellList(string format, string _, FeatureDefinitionMagicAffinity instance)
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

                foreach (var level in levels)
                {
                    if (spells.ContainsKey(level) && spells[level].Empty())
                    {
                        spells.Remove(level);
                    }
                }

                var spellList = "\n" + string.Join("\n",
                    spells.Select(e =>
                        $"{FormatSpellLevel(e.Key)}\t{string.Join(", ", e.Value.Select(s => s.FormatTitle()))}"));

                return Gui.Format(Gui.Localize(description), spellList);
            }

           

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var formatMethod = typeof(Gui).GetMethod("Format", BindingFlags.Static | BindingFlags.Public);
                var myFormatMethod = typeof(FeatureDefinitionMagicAffinity_FormatDescription).GetMethod("FormatSpellList");
                var found = 0;

                foreach (var instruction in instructions)
                {
                    //Find first call to Gui.Format and replace it with oour custom method
                    if (instruction.Calls(formatMethod) && ++found == 1)
                    {
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
}
