using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionMagicAffinityPatcher
{
    private static string FormatSpellLevel(int level)
    {
        return Gui.Colorize(level == 0 ? "0" : Gui.ToRoman(level), Gui.ColorHighEmphasis);
    }

    //PATCH: formats spell list into list with spell levels, instead of 1 line of all spells like default does
    [HarmonyPatch(typeof(FeatureDefinitionMagicAffinity), nameof(FeatureDefinitionMagicAffinity.FormatDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatDescription_Patch
    {
        private static string FormatSpellList(FeatureDefinitionMagicAffinity instance)
        {
            var description = instance.GuiPresentation.Description;
            var spells = new Dictionary<int, List<SpellDefinition>>();
            var levels = new HashSet<int>();

            foreach (var duplet in instance.ExtendedSpellList.SpellsByLevel)
            {
                if (!spells.TryGetValue(duplet.Level, out var value))
                {
                    value = [];
                    spells.Add(duplet.Level, value);
                    levels.Add(duplet.Level);
                }

                value.AddRange(duplet.Spells);
            }

            foreach (var level in levels
                         .Where(level => spells.ContainsKey(level) && spells[level].Empty()))
            {
                spells.Remove(level);
            }

            var spellList = string.Join("\n",
                spells.Select(e =>
                    $"{FormatSpellLevel(e.Key)}\t{string.Join(", ", e.Value.Select(s => s.FormatTitle()))}"));

            return Gui.Format(Gui.Localize(description), spellList);
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var formatMethod = typeof(Gui).GetMethod("Format", BindingFlags.Static | BindingFlags.Public);
            var myFormatMethod = new Func<FeatureDefinitionMagicAffinity, string>(FormatSpellList).Method;

            return instructions.ReplaceCall(formatMethod,
                1, "FeatureDefinitionMagicAffinity.FormatDescription",
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myFormatMethod));
        }
    }
}
