using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    [HarmonyPatch(typeof(GameLocationCharacter), "IsCriticalCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacterManager_IsCriticalCharacter
    {
        internal static bool Prefix(ref bool __result)
        {
            if (Models.EncountersSpawnContext.HasStagedHeroes || (Gui.GameLocation.UserLocation != null && Main.Settings.AllowDeathInCustomDungeons))
            {
                __result = false;

                return false;
            }

            return true;
        }
    }
}
