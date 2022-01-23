using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.AdventureLog
{
    [HarmonyPatch(typeof(PosterScreen), "Show")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PosterScreen_Show
    {
        internal static void Postfix(List<string> captions)
        {
            if (Main.Settings.EnableAdventureLogLore)
            {
                var builder = new StringBuilder();

                captions.ForEach(x => builder.Append(x));
                Models.AdventureLogContext.LogEntry("Lore", builder.ToString());
            }
        }
    }
}
