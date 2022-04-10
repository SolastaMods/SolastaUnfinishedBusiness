using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    // allows prepared spell casters to take metamagic feats and have a working UI [otherwise sorcery points get off screen]
    [HarmonyPatch(typeof(SpellRepertoirePanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellRepertoirePanel_OnBeginShow
    {
        internal static void Postfix(SpellRepertoirePanel __instance, RectTransform ___sorceryPointsBox)
        {
            if (!Main.Settings.EnableMoveSorceryPointsBox || __instance.SpellRepertoire.MaxPreparedSpell == 0)
            {
                return;
            }

            ___sorceryPointsBox.localPosition = new Vector3(-900, 38, 0);
        }
    }
}
