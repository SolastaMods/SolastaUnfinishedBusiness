using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    public static class DungeonMakerContext
    {
        internal const int GAME_PARTY_SIZE = 4;

        internal const int MIN_PARTY_SIZE = 1;
        internal const int MAX_PARTY_SIZE = 6;

        internal const float ADVENTURE_PANEL_DEFAULT_SCALE = 0.75f;
        internal const float REST_PANEL_DEFAULT_SCALE = 0.8f;
        internal const float PARTY_CONTROL_PANEL_DEFAULT_SCALE = 0.95f;
        internal const float VICTORY_MODAL_DEFAULT_SCALE = 0.85f;
        internal const float REVIVE_PARTY_CONTROL_PANEL_DEFAULT_SCALE = 0.85f;

        internal const int DUNGEON_MIN_LEVEL = 1;
        internal const int DUNGEON_MAX_LEVEL = 20;

        private const string BACKUP_FOLDER = "DungeonMakerBackups";

        public static readonly List<MonsterDefinition> ModdedMonsters = new();

        internal static void BackupAndDelete(string path, UserContent userContent)
        {
            var backupDirectory = Path.Combine(Main.MOD_FOLDER, BACKUP_FOLDER);

            Directory.CreateDirectory(backupDirectory);

            var title = userContent.Title;
            var compliantTitle = IOHelper.GetOsCompliantFilename(title);
            var destinationPath = Path.Combine(backupDirectory, compliantTitle) + "." + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var backupFiles = Directory.EnumerateFiles(backupDirectory, compliantTitle + "*").OrderBy(f => f).ToList();

            for (int i = 0; i <= backupFiles.Count - Main.Settings.MaxBackupFilesPerLocationCampaign; i++)
            {
                File.Delete(backupFiles[i]);
            }

            File.Copy(path, destinationPath);
            File.Delete(path);
        }
    }
}
