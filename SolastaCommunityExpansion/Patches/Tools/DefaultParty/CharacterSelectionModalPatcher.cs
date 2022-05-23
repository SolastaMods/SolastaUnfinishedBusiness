using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterSelectionModal_EnumeratePlates
    {
        internal static void Postfix(RectTransform ___charactersTable)
        {
            for (var i = 0; i < ___charactersTable.childCount; i++)
            {
                var character = ___charactersTable.GetChild(i);
                var checkBoxToggle = character.GetComponentInChildren<Toggle>();

                if (checkBoxToggle)
                {
                    checkBoxToggle.gameObject.SetActive(false);
                } 
            }
        }
    }
}
