using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HarmonyLib;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Patches.GameUi.Location;

[HarmonyPatch(typeof(UnityModManager.UI), "ToggleWindow", typeof(bool))]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UnityModManager_UI_ToggleWindow
{
#pragma warning disable S3168 // "async" methods should not return "void"
    internal static async void Postfix(bool open)
#pragma warning restore S3168 // "async" methods should not return "void"
    {
        await ModManagerUI.Set(open);
    }
}

internal static class ModManagerUI
{
    public static bool IsOpen { get; private set; }

    public static async Task Set(bool open)
    {
        Main.Log($"UnityModManagerUIToggleWindowPatch: open={open}");

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

        Main.Log($"UnityModManagerUIToggleWindowPatch: IsOpen={IsOpen}");
    }
}
