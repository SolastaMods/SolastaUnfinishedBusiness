using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        [System.Obsolete]
        internal static void Postfix()

        {
            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                // always load custom definitions even when multiclass is disabled to avoid errors parsing MC heroes during startup
                LevelUpContext.Load();

                if (!(Main.Settings.EnableMulticlass || Main.Settings.EnableLevelDown))
                {
                    return;
                }

                CacheSpellsContext.Load();

                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                IntegrationContext.Load();
                InspectionPanelContext.Load();
                LevelDownContext.Load();
                SharedSpellsContext.Load();

                // tells CE that MC is in the building and will handle pact magic from now on
                SolastaCommunityExpansion.Main.IsMulticlassInstalled = Main.Settings.EnableMulticlass;

                Main.Enabled = true;
                Main.Logger.Log("Enabled.");
            };
        }
    }
}
