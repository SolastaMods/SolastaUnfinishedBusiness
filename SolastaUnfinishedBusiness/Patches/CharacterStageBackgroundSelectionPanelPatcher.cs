using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageBackgroundSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageBackgroundSelectionPanel),
        nameof(CharacterStageBackgroundSelectionPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterStageBackgroundSelectionPanel __instance)
        {
            //PATCH: avoids a restart when enabling / disabling backgrounds on the Mod UI panel
            var compatibleBackgrounds = DatabaseRepository.GetDatabase<CharacterBackgroundDefinition>()
                .Where(x => !x.GuiPresentation.Hidden)
                .OrderBy(x => x.FormatTitle());

            __instance.compatibleBackgrounds.SetRange(compatibleBackgrounds);
        }
    }
}
