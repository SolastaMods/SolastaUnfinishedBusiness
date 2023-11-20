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

        int day = DateTime.Now.Date.Day;

        if (shouldUpdate && !Main.Settings.DisableUpdateMessage)
        {
            DisplayUpdateMessage();
        }
        else if (Main.Settings.DisplayModMessage != day)
        {
            Main.Settings.DisplayModMessage = day;
            DisplayWelcomeMessage();
        }
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
        var minor = Int32.Parse(a1[3]);

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
            var v1 = a1[0] + a1[1] + a1[2] + Int32.Parse(a1[3]).ToString("D3");
            var v2 = a2[0] + a2[1] + a2[2] + Int32.Parse(a2[3]).ToString("D3");

            shouldUpdate = String.Compare(v2, v1, StringComparison.Ordinal) > 0;
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

        var version = toLatest ? LatestVersion : PreviousVersion;
        var destFiles = new[] { "Info.json", "SolastaUnfinishedBusiness.dll" };

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
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

            foreach (var destFile in destFiles)
            {
                var fullDestFile = Path.Combine(Main.ModFolder, destFile);

                File.Delete(fullDestFile);
                File.Move(
                    Path.Combine(fullZipFolder, destFile),
                    fullDestFile);
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
            "Donate",
            "Message/&MessageOkTitle",
            OpenDonatePayPal,
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
            "Donate",
            "Message/&MessageOkTitle",
            OpenDonatePayPal,
            () => { });
    }

    internal static void OpenDonateGithub()
    {
        OpenUrl("https://github.com/sponsors/ThyWoof");
    }

    internal static void OpenDonatePatreon()
    {
        OpenUrl("https://patreon.com/SolastaMods");
    }

    internal static void OpenDonatePayPal()
    {
        OpenUrl("https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Unfinished+Business");
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

    internal static void OpenSubjectiveGuideToSolasta()
    {
        OpenUrl("https://docs.google.com/presentation/d/1iqXc3JzT_uKUcnmFoB3Tyddj3Jzj-AXfNtf3vWcWqc0");
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
