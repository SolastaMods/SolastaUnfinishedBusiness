using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.GadgetBlueprints;

namespace SolastaCommunityExpansion.Patches.GameUi.ScreenMap
{
    [HarmonyPatch(typeof(GameLocationManager), "ReadyLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationManager_ReadyLocation
    {
        internal static void SetTeleporterGadgetActiveAnimation(WorldGadget worldGadget, bool visibility = false)
        {
            if (!Main.Settings.HideExitAndTeleporterGizmosIfNotDiscovered || worldGadget?.UserGadget == null)
            {
                return;
            }

            if (worldGadget.UserGadget.GadgetBlueprint == TeleporterIndividual)
            {
                var visualEffect = worldGadget.transform.FindChildRecursive("Vfx_Teleporter_Individual_Idle_01");

                visualEffect?.gameObject.SetActive(visibility);
            }
            else if (worldGadget.UserGadget.GadgetBlueprint == TeleporterParty)
            {
                var visualEffect = worldGadget.transform.FindChildRecursive("Vfx_Teleporter_Party_Idle_01");

                visualEffect?.gameObject.SetActive(visibility);
            }
        }

        internal static void Postfix(GameLocationManager __instance)
        {
            if (Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var worldGadgets = __instance.WorldLocation.WorldSectors.SelectMany(x => x.WorldGadgets);

            foreach (var worldGadget in worldGadgets)
            {
                SetTeleporterGadgetActiveAnimation(worldGadget);
            }
        }
    }
}
