using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        internal static void Postfix()

        {
            InspectionPanelContext.Load();
            LevelDownContext.Load();
            PatchingContext.Load();

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                CacheSpellsContext.Load();
                IntegrationContext.Load();
                SharedSpellsContext.Load();
            };
        }
    }
}
