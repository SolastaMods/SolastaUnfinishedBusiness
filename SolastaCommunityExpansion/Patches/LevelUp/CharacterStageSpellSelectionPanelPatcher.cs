using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

// caches allowed spells offered on this stage
[HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "EnterStage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageSpellSelectionPanel_EnterStage
{
    public static void Prefix(CharacterStageSpellSelectionPanel __instance)
    {
        LevelUpContext.CacheSpells(__instance.currentHero);
    }
}
