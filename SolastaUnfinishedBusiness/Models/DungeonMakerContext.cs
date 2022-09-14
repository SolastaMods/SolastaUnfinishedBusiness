using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Models;

public static class DungeonMakerContext
{
    internal const int GamePartySize = 4;

    internal const int MinPartySize = 1;
    internal const int MaxPartySize = 6;

    internal const float AdventurePanelDefaultScale = 0.75f;
    internal const float VictoryModalDefaultScale = 0.85f;
    internal const float RevivePartyControlPanelDefaultScale = 0.85f;

    internal const int DungeonMinLevel = 1;
    internal const int DungeonMaxLevel = 20;

    // private const string BackupFolder = "DungeonMakerBackups";

    internal static float GetPartyControlScale()
    {
        return (float)GamePartySize / Gui.GameCampaign.Party.CharactersList.Count;
    }

    // [NotNull]
    // internal static string ReplaceVariable(string line)
    // {
    //     var service = ServiceRepository.GetService<IGameVariableService>();
    //     const string PATTERN = @"\{[a-zA-Z_][a-zA-Z0-9_]*\}";
    //
    //     foreach (Match match in Regex.Matches(line, PATTERN))
    //     {
    //         var variableName = match.Value.Substring(1, match.Value.Length - 2);
    //
    //         if (service.TryFindVariable(variableName, out var gameVariable))
    //         {
    //             line = line.Replace(match.Value, gameVariable.StringValue);
    //         }
    //     }
    //
    //     return line;
    // }

    // must be public because of transpiler
    // ReSharper disable once UnusedMember.Global
    // public static void BackupAndDelete([NotNull] string path, [NotNull] UserContent userContent)
    // {
    //     var backupDirectory = Path.Combine(Main.ModFolder, BackupFolder);
    //
    //     Directory.CreateDirectory(backupDirectory);
    //
    //     var title = userContent.Title;
    //     var compliantTitle = IOHelper.GetOsCompliantFilename(title);
    //     var destinationPath = Path.Combine(backupDirectory, compliantTitle) + "." +
    //                           DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
    //     var backupFiles = Directory.EnumerateFiles(backupDirectory, compliantTitle + "*").OrderBy(f => f).ToList();
    //
    //     for (var i = 0; i <= backupFiles.Count - Main.Settings.MaxBackupFilesPerLocationCampaign; i++)
    //     {
    //         File.Delete(backupFiles[i]);
    //     }
    //
    //     File.Copy(path, destinationPath);
    //     File.Delete(path);
    // }
}
