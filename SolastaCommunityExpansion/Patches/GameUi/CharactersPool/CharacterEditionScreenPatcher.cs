using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool
{
    [HarmonyPatch(typeof(CharacterEditionScreen), "OnFinishCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterEditionScreen_OnFinishCb
    {
        internal static string HeroName { get; set; }

        internal static void Prefix()
        {
            if (Gui.Game == null && Main.Settings.KeepCharactersPanelOpenAndHeroSelectedOnLevelUp)
            {
                HeroName = ServiceRepository.GetService<ICharacterBuildingService>().HeroCharacter.Name;
            }
            else
            {
                HeroName = null;
            }
        }
    }
}
