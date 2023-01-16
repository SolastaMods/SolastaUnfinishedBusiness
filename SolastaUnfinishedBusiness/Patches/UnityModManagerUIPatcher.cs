using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UnityModManagerUIPatcher
{
    [HarmonyPatch(typeof(UnityModManager.UI), nameof(UnityModManager.UI.ToggleWindow), typeof(bool))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ToggleWindow_Patch
    {
        //PATCH: prevents game from receiving input if Mod UI is open
        [UsedImplicitly]
        public static async void Postfix(bool open)
        {
            await ModManagerUI.Set(open);
        }
    }

    public static class ModManagerUI
    {
        public static bool IsOpen { get; private set; }

        public static async Task Set(bool open)
        {
            if (open)
            {
                IsOpen = true;
            }
            else
            {
                // wait 100ms to allow UI to close before setting IsOpen to false
                await Task.Delay(100);

                // but use actual state of UI in case someone quickly opens it again
                IsOpen = UnityModManager.UI.Instance.Opened;
            }
        }
    }
}
