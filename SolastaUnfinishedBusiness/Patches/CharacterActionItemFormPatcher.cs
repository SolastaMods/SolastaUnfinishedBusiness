using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterActionItemFormPatcher
{
    [HarmonyPatch(typeof(CharacterActionItemForm), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: Get dynamic properties from forced attack
            if (__instance.guiCharacterAction.forcedAttackMode == null)
            {
                return;
            }

            __instance.dynamicItemPropertiesEnumerator.Unbind();
            __instance.dynamicItemPropertiesEnumerator.Bind(
                __instance.guiCharacterAction.forcedAttackMode.sourceObject as RulesetItem);
        }
    }
}
