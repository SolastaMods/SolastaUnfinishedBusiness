using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionAutoPreparedSpellsPatcher
{
    private static string FormatSpellLevel(int level)
    {
        return Gui.Colorize(level == 0 ? "0" : Gui.ToRoman(level), Gui.ColorHighEmphasis);
    }

    [HarmonyPatch(typeof(FeatureDefinitionAutoPreparedSpells),
        nameof(FeatureDefinitionAutoPreparedSpells.FormatDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatDescription_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(FeatureDefinitionAutoPreparedSpells __instance, out string __result)
        {
            //PATCH: formats spell list into list with spell levels, instead of 1 line of all spells like default does
            var spells = new Dictionary<int, List<SpellDefinition>>();

            foreach (var group in __instance.AutoPreparedSpellsGroups)
            {
                foreach (var spell in group.SpellsList)
                {
                    var spellLevel = spell.SpellLevel;

                    if (!spells.TryGetValue(spellLevel, out var value))
                    {
                        value = new List<SpellDefinition>();
                        spells.Add(spellLevel, value);
                    }

                    value.Add(spell);
                }
            }

            var result = string.Join("\n", spells.Select(e =>
                $"{FormatSpellLevel(e.Key)}\t{string.Join(", ", e.Value.Select(s => s.FormatTitle()))}"));

            var description = __instance.GuiPresentation.Description;

            description = description == GuiPresentationBuilder.EmptyString ? string.Empty : Gui.Localize(description);

            __result = Gui.Format(description, result);

            return false;
        }
    }
}
