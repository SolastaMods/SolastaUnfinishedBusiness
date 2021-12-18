using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(UserEffectFormItem), "Initialize")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UserEffectFormItem_Initialize
    {
        /// <summary>
        /// Fixes bug with DM custom monsters form when adding 'User effect form'.  
        /// The 'save to cancel' toggle switch change handler isn't connected 
        /// which results in 'save to cancel' not being persisted.
        /// </summary>
        public static void Postfix(UserEffectFormItem __instance, Toggle ___canSaveToCancelToggle)
        {
            if(!Main.Settings.BugFixOnCanSaveToggleChanged)
            {
                return;
            }

            ___canSaveToCancelToggle.onValueChanged.AddListener(delegate
            {
                __instance.OnCanSaveToggleChanedCb(___canSaveToCancelToggle);
                __instance.ContentChanged?.Invoke();
            });
        }
    }
}
