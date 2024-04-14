using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageIdentityDefinitionPanelPatcher
{
    //PATCH: avoids issues during RESPEC if a hero with same name is in the pool
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel),
        nameof(CharacterStageIdentityDefinitionPanel.CanProceedToNextStage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanProceedToNextStage_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterStageIdentityDefinitionPanel __instance, ref bool __result)
        {
            var name = __instance.currentHero.Name;

            if (!Main.Settings.EnableRespec || !ToolsContext.FunctorRespec.IsRespecing ||
                string.IsNullOrEmpty(name))
            {
                return;
            }

            __result = !Gui.GameCampaign || ToolsContext.FunctorRespec.OldHeroName == name ||
                       !Gui.GameCampaign.Party.CharactersList.Exists(x => x.Name == name);
        }
    }
}
