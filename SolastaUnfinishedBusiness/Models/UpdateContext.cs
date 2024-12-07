using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Models;

internal static class UpdateContext
{
    private const string BaseURL = "https://github.com/SolastaMods/SolastaUnfinishedBusiness/releases/latest/download";

    private static string InstalledVersion { get; } = GetInstalledVersion();
    private static string LatestVersion { get; set; }
    private static string PreviousVersion { get; } = GetPreviousVersion();

    internal static void Load()
    {
        LatestVersion = GetLatestVersion(out var shouldUpdate);

        if (shouldUpdate)
        {
            DisplayUpdateMessage();
        }
        else if (Main.Settings.DisplayModMessage == 0)
        {
            DisplayWelcomeMessage();
        }

        // display mod message every 30 launches
        Main.Settings.DisplayModMessage = (Main.Settings.DisplayModMessage + 1) % 30;
    }

    private static string GetInstalledVersion()
    {
        var infoPayload = File.ReadAllText(Path.Combine(Main.ModFolder, "Info.json"));
        var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

        // ReSharper disable once AssignNullToNotNullAttribute
        return infoJson["Version"].Value<string>();
    }

    private static string GetPreviousVersion()
    {
        var a1 = InstalledVersion.Split('.');
        var minor = int.Parse(a1[3]);

        a1[3] = (--minor).ToString();

        // ReSharper disable once AssignNullToNotNullAttribute
        return string.Join(".", a1);
    }

    private static string GetLatestVersion(out bool shouldUpdate)
    {
        var version = "";

        shouldUpdate = false;

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        try
        {
            var infoPayload = wc.DownloadString($"{BaseURL}/Info.json");
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            // ReSharper disable once AssignNullToNotNullAttribute
            version = infoJson["Version"].Value<string>();

            var a1 = InstalledVersion.Split('.');
            var a2 = version.Split('.');
            var v1 = a1[0] + a1[1] + a1[2] + int.Parse(a1[3]).ToString("D3");
            var v2 = a2[0] + a2[1] + a2[2] + int.Parse(a2[3]).ToString("D3");

            shouldUpdate = string.Compare(v2, v1, StringComparison.Ordinal) > 0;
        }
        catch
        {
            Main.Error("cannot fetch update data.");
        }

        return version;
    }

    internal static void UpdateMod(bool toLatest = true)
    {
        UnityModManager.UI.Instance.ToggleWindow(false);

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
        var version = toLatest ? LatestVersion : PreviousVersion;
        var zipFile = $"SolastaUnfinishedBusiness-{version}.zip";
        var fullZipFile = Path.Combine(Main.ModFolder, zipFile);
        var fullZipFolder = Path.Combine(Main.ModFolder, "SolastaUnfinishedBusiness");
        var baseUrlByVersion = BaseURL.Replace("latest/download", $"download/{version}");
        var url = $"{baseUrlByVersion}/{zipFile}";

        try
        {
            wc.DownloadFile(url, fullZipFile);

            if (Directory.Exists(fullZipFolder))
            {
                Directory.Delete(fullZipFolder, true);
            }

            ZipFile.ExtractToDirectory(fullZipFile, Main.ModFolder);
            File.Delete(fullZipFile);

            foreach (var sourceFile in Directory.GetFiles(fullZipFolder))
            {
                var fileName = Path.GetFileName(sourceFile);
                var destFile = Path.Combine(Main.ModFolder, fileName);

                File.Delete(destFile);
                File.Move(sourceFile, destFile);
            }

            Directory.Delete(fullZipFolder, true);

            message = "Mod version change successful. Please restart.";
        }
        catch
        {
            message = $"Cannot fetch update payload. Try again or download from:\r\n{url}.";
        }

        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            message,
            "ChangeLog",
            "Message/&MessageOkTitle",
            OpenChangeLog,
            () => { });
    }

    internal static void DisplayRollbackMessage()
    {
        UnityModManager.UI.Instance.ToggleWindow(false);

        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            $"Would you like to rollback to {PreviousVersion}?",
            "Message/&MessageOkTitle",
            "Message/&MessageCancelTitle",
            () => UpdateMod(false),
            () => { });
    }

    private static void DisplayUpdateMessage()
    {
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            $"Version {LatestVersion} is now available. Open Mod UI > Gameplay > Tools to update.",
            "Changelog",
            "Message/&MessageOkTitle",
            OpenChangeLog,
            () => { });
    }

    private static void DisplayWelcomeMessage()
    {
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            "Message/&MessageModWelcomeDescription",
            "ChangeLog",
            "Message/&MessageOkTitle",
            OpenChangeLog,
            () => { });
    }

    internal static void OpenChangeLog()
    {
        OpenUrl(
            "https://raw.githubusercontent.com/SolastaMods/SolastaUnfinishedBusiness/master/SolastaUnfinishedBusiness/ChangelogHistory.txt");
    }

    internal static void OpenDocumentation(string filename)
    {
        OpenUrl($"file://{Main.ModFolder}/Documentation/{filename}");
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
