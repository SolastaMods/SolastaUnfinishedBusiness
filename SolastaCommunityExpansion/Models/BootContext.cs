using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Models;

internal static class BootContext
{
    internal static void Load()
    {
        if (ShouldUpdate(out var version, out var changeLog))
        {
            DisplayUpdateMessage(version, changeLog);
        }
        else if (Main.Settings.DisplayWelcomeMessage)
        {
            DisplayWelcomeMessage();

            Main.Settings.DisplayWelcomeMessage = false;
        }
    }

    private static string GetInstalledVersion()
    {
        var infoPayload = File.ReadAllText(Path.Combine(Main.MOD_FOLDER, "Info.json"));
        var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

        return infoJson["Version"].Value<string>();
    }

    private static bool ShouldUpdate(out string version, out string changeLog)
    {
        const string BASE_URL =
            "https://raw.githubusercontent.com/SolastaMods/SolastaCommunityExpansion/master/SolastaCommunityExpansion";

        var hasUpdate = false;

        version = "";
        changeLog = "";

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        try
        {
            var infoPayload = wc.DownloadString($"{BASE_URL}/Info.json");
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            version = infoJson["Version"].Value<string>();
            hasUpdate = version.CompareTo(GetInstalledVersion()) > 0;

            changeLog = wc.DownloadString($"{BASE_URL}/Changelog.txt");
        }
        catch
        {
            Main.Logger.Log("cannot fetch update data.");
        }

        return hasUpdate;
    }

    private static void UpdateMod(string version)
    {
        const string BASE_URL = "https://github.com/SolastaMods/SolastaCommunityExpansion";

        var destFiles = new[] {"Info.json", "SolastaCommunityExpansion.dll"};

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
        var zipFile = $"SolastaCommunityExpansion-{version}.zip";
        var fullZipFile = Path.Combine(Main.MOD_FOLDER, zipFile);
        var fullZipFolder = Path.Combine(Main.MOD_FOLDER, "SolastaCommunityExpansion");
        var url = $"{BASE_URL}/releases/download/{version}/{zipFile}";

        try
        {
            wc.DownloadFile(url, fullZipFile);

            if (Directory.Exists(fullZipFolder))
            {
                Directory.Delete(fullZipFolder, true);
            }

            ZipFile.ExtractToDirectory(fullZipFile, Main.MOD_FOLDER);
            File.Delete(fullZipFile);

            foreach (var destFile in destFiles)
            {
                var fullDestFile = Path.Combine(Main.MOD_FOLDER, destFile);

                File.Delete(fullDestFile);
                File.Move(
                    Path.Combine(fullZipFolder, destFile),
                    fullDestFile);
            }

            Directory.Delete(fullZipFolder, true);

            message = "Update successful. Please restart.";
        }
        catch
        {
            message = $"Cannot fetch update payload. Try again or download from:\n{url}.";
        }

        Main.Logger.Log(message);
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Informative1,
            "Message/&MessageModWelcomeTitle",
            message,
            "Message/&MessageOkTitle",
            string.Empty,
            null,
            null);
    }

    private static void DisplayUpdateMessage(string version, string changeLog)
    {
        if (changeLog == string.Empty)
        {
            changeLog = "- no changelog found";
        }

        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            $"Version {version} is now available.\n\n{changeLog}\n\nWould you like to update?",
            "Message/&MessageOkTitle",
            "Message/&MessageCancelTitle",
            () => UpdateMod(version),
            null);
    }

    internal static void DisplayWelcomeMessage()
    {
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Informative1,
            "Message/&MessageModWelcomeTitle",
            "Message/&MessageModWelcomeDescription",
            "Message/&MessageOkTitle",
            string.Empty,
            () => UnityModManager.UI.Instance.ToggleWindow(),
            null);
    }
}
