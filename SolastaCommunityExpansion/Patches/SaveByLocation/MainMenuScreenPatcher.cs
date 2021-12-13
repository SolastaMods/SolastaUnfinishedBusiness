using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(MainMenuScreen), "OnEndShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class MainMenuScreen_OnEndShow
    {
        internal static void Postfix()
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            // Ensure folders exist
            Directory.CreateDirectory(LocationSaveGameDirectory);
            Directory.CreateDirectory(CampaignSaveGameDirectory);

            // Find the most recently touched save file and select the correct location/campaign for that save
            var mostRecent = Directory.EnumerateDirectories(LocationSaveGameDirectory)
                .Select(d => new
                {
                    Path = d,
                    LastWriteTime = Directory.EnumerateFiles(d, "*.sav").Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                    LocationType = LocationType.UserLocation
                })
                .Concat(
                    Directory.EnumerateDirectories(CampaignSaveGameDirectory)
                        .Select(d => new
                        {
                            Path = d,
                            LastWriteTime = Directory.EnumerateFiles(d, "*.sav").Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                            LocationType = LocationType.CustomCampaign
                        })
                )
                .Concat(
                    Enumerable.Repeat(
                        new
                        {
                            Path = DefaultSaveGameDirectory,
                            LastWriteTime = Directory.EnumerateFiles(DefaultSaveGameDirectory, "*.sav").Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                            LocationType = LocationType.MainCampaign
                        }
                    , 1)
                )
                .Where(d => d.LastWriteTime.HasValue)
                .OrderByDescending(d => d.LastWriteTime)
                .FirstOrDefault();

            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            if (selectedCampaignService != null && mostRecent != null)
            {
                Main.Log($"Most recent folder={mostRecent.Path}");

                switch(mostRecent.LocationType)
                {
                    default:
                        selectedCampaignService.SetCampaignLocation(MAIN_CAMPAIGN, string.Empty);
                        break;
                    case LocationType.UserLocation:
                        selectedCampaignService.SetCampaignLocation(USER_CAMPAIGN, Path.GetFileName(mostRecent.Path));
                        break;
                    case LocationType.CustomCampaign:
                        selectedCampaignService.SetCampaignLocation(Path.GetFileName(mostRecent.Path), string.Empty);
                        break;
                }
            }
        }
    }
}
