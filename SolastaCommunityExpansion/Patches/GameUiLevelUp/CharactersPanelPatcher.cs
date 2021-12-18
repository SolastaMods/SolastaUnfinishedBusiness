using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(CharactersPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharactersPanel_OnBeginShow
    {
        internal static void Postfix(CharactersPanel __instance)
        {
            if (CharacterEditionScreen_OnFinishCb.HeroName != null)
            {
                var mainMenuScreen = __instance.OriginScreen as MainMenuScreen;
                var charactersPanel = AccessTools.Field(mainMenuScreen.GetType(), "charactersPanel").GetValue(mainMenuScreen) as CharactersPanel;
                var characterPlates = AccessTools.Field(charactersPanel.GetType(), "characterPlates").GetValue(charactersPanel) as List<CharacterPlateToggle>;
                var characterPlate = characterPlates.Find(x => x.GuiCharacter.Name == CharacterEditionScreen_OnFinishCb.HeroName);

                CharacterEditionScreen_OnFinishCb.HeroName = null;

                charactersPanel.OnSelectPlate(characterPlate);
            }
        }
    }
}
