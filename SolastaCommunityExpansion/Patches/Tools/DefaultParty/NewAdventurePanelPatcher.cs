using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(NewAdventurePanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NewAdventurePanel_Refresh
    {
        internal static void Postfix(RectTransform ___characterSessionPlatesTable)
        {
            if (!Main.Settings.EnableTogglesToOverwriteDefaultTestParty || Models.Global.IsMultiplayer)
            {
                return;
            }

            var max = System.Math.Min(Main.Settings.DefaultPartyHeroes.Count, ___characterSessionPlatesTable.childCount);

            for (var i = 0; i < max; i++)
            {
                var characterPlateSession = ___characterSessionPlatesTable.GetChild(i).GetComponent<CharacterPlateSession>();

                if (characterPlateSession.gameObject.activeSelf)
                {
                    characterPlateSession.BindCharacter(Main.Settings.DefaultPartyHeroes[i], false);
                }
            }
        }
    }
}
