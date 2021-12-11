using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion
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

            var root = TacticalAdventuresApplication.SaveGameDirectory;

            var mostRecent = Directory.EnumerateDirectories(root)
                .Select(d => new
                {
                    Path = d,
                    LastWriteTime = Directory.EnumerateFiles(d).Max(f => (DateTime?)File.GetLastWriteTimeUtc(f))
                })
                .Concat(
                    Enumerable.Repeat(
                        new
                        {
                            Path = root,
                            LastWriteTime = Directory.EnumerateFiles(root).Max(f => (DateTime?)File.GetLastWriteTimeUtc(f))
                        }, 
                        1)
                )
                .Where(d => d.LastWriteTime.HasValue)
                .OrderByDescending(d => d.LastWriteTime)
                .FirstOrDefault();

            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            if (mostRecent != null && mostRecent.Path != root && selectedCampaignService != null)
            {
                selectedCampaignService.Location = Path.GetFileName(mostRecent.Path);
                selectedCampaignService.Campaign = USER_CAMPAIGN;
            }
        }
    }
}
