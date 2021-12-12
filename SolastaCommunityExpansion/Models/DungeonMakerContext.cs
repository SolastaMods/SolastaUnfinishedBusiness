using System;
using System.IO;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class DungeonMakerContext
    {
        private const string BACKUP_FOLDER = "DungeonMakerBackups";

        internal static void Load()
        {
            UpdateGadgetsPlacement();
            UpdatePropsPlacement();
        }

        internal static void BackupAndDelete(string path, UserContent userContent)
        {
            var backupDirectory = Path.Combine(Main.MOD_FOLDER, BACKUP_FOLDER);

            Directory.CreateDirectory(backupDirectory);

            var title = userContent.Title;
            var compliantTitle = IOHelper.GetOsCompliantFilename(title);
            var destinationPath = Path.Combine(backupDirectory, compliantTitle) + "." + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var backupFiles = Directory.EnumerateFiles(backupDirectory, compliantTitle + "*").ToList();

            backupFiles.Sort(); // consider changing to OrderBy before ToList

            for (int i = 0; i <= backupFiles.Count - Main.Settings.maxBackupFiles; i++)
            {
                File.Delete(backupFiles[i]);
            }

            File.Copy(path, destinationPath);
            File.Delete(path);
        }

        internal static void UpdateGadgetsPlacement()
        {
            if (Main.Settings.FlexibleGadgetsPlacement)
            {
                var dbGadgetBlueprint = DatabaseRepository.GetDatabase<GadgetBlueprint>();

                foreach (GadgetBlueprint gadgetBlueprint in dbGadgetBlueprint)
                {
                    if (gadgetBlueprint.GroundPlacement || gadgetBlueprint.GroundLowPlacement || gadgetBlueprint.GroundHighPlacement ||
                        gadgetBlueprint.OpeningPlacement || gadgetBlueprint.OpeningLowPlacement || gadgetBlueprint.OpeningHighPlacement)
                    {
                        gadgetBlueprint.SetGroundPlacement(true);
                        gadgetBlueprint.SetGroundLowPlacement(true);
                        gadgetBlueprint.SetGroundHighPlacement(true);
                        gadgetBlueprint.SetOpeningPlacement(true);
                        gadgetBlueprint.SetOpeningLowPlacement(true);
                        gadgetBlueprint.SetOpeningHighPlacement(true);
                    }
                }
            }
        }

        internal static void UpdatePropsPlacement()
        {
            if (Main.Settings.FlexiblePropsPlacement)
            {
                var dbPropBlueprint = DatabaseRepository.GetDatabase<PropBlueprint>();

                foreach (PropBlueprint propBlueprint in dbPropBlueprint)
                {
                    if (propBlueprint.GroundPlacement || propBlueprint.GroundLowPlacement || propBlueprint.GroundHighPlacement ||
                        propBlueprint.OpeningPlacement || propBlueprint.OpeningLowPlacement || propBlueprint.OpeningHighPlacement)
                    {
                        propBlueprint.SetGroundPlacement(true);
                        propBlueprint.SetGroundLowPlacement(true);
                        propBlueprint.SetGroundHighPlacement(true);
                        propBlueprint.SetOpeningPlacement(true);
                        propBlueprint.SetOpeningLowPlacement(true);
                        propBlueprint.SetOpeningHighPlacement(true);
                    }
                }
            }
        }
    }
}
