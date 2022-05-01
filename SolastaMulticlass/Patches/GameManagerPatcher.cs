using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        internal static void Postfix()

        {
            InspectionPanelContext.Load(); // no dependencies
            LevelDownContext.Load(); // no dependencies

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                CacheSpellsContext.Load(); // depends on all CE blueprints in databases
                IntegrationContext.Load(); // depends on all CE blueprints in databases
                PatchingContext.Load(); // depends on IntegrationContext
                SharedSpellsContext.Load(); // depends on IntegrationContext
            };
        }
    }
}
