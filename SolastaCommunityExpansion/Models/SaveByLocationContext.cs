using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Models;

internal static class SaveByLocationContext
{
    private const string CotmCampaign = "CrownOfTheMagister";
    private const string VotpCampaign = "DLC1_ValleyOfThePast_Campaign";
    internal const string UserCampaign = "UserCampaign";

    private const string LocationSaveFolder = @"CE\Location";
    private const string CampaignSaveFolder = @"CE\Campaign";

    internal static readonly string DefaultSaveGameDirectory =
        Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");

    private static readonly string LocationSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, LocationSaveFolder);

    private static readonly string CampaignSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, CampaignSaveFolder);

    internal static void LateLoad()
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
                LastWriteTime =
                    Directory.EnumerateFiles(d, "*.sav").Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                LocationType = LocationType.UserLocation
            })
            .Concat(
                Directory.EnumerateDirectories(CampaignSaveGameDirectory)
                    .Select(d => new
                    {
                        Path = d,
                        LastWriteTime =
                            Directory.EnumerateFiles(d, "*.sav")
                                .Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                        LocationType = LocationType.CustomCampaign
                    })
            )
            .Concat(
                Enumerable.Repeat(
                    new
                    {
                        Path = DefaultSaveGameDirectory,
                        LastWriteTime =
                            Directory.EnumerateFiles(DefaultSaveGameDirectory, "*.sav")
                                .Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                        LocationType = LocationType.StandardCampaign
                    }
                    , 1)
            )
            .Where(d => d.LastWriteTime.HasValue)
            .OrderByDescending(d => d.LastWriteTime)
            .FirstOrDefault();

        var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

        if (mostRecent == null)
        {
            return;
        }

        Main.Log($"Most recent folder={mostRecent.Path}");

        switch (mostRecent.LocationType)
        {
            default:
                selectedCampaignService.SetStandardCampaignLocation();
                break;
            case LocationType.UserLocation:
                selectedCampaignService.SetCampaignLocation(UserCampaign, Path.GetFileName(mostRecent.Path));
                break;
            case LocationType.CustomCampaign:
                selectedCampaignService.SetCampaignLocation(Path.GetFileName(mostRecent.Path), string.Empty);
                break;
        }
    }

    internal static int SaveFileCount(LocationType locationType, string folder)
    {
        switch (locationType)
        {
            case LocationType.UserLocation:
            {
                var saveFolder = Path.Combine(LocationSaveGameDirectory, folder);

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            case LocationType.CustomCampaign:
            {
                var saveFolder = Path.Combine(CampaignSaveGameDirectory, folder);

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            case LocationType.StandardCampaign:
            {
                var saveFolder = DefaultSaveGameDirectory;

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            default:
                Main.Error($"Unknown LocationType: {locationType}");
                break;
        }

        return 0;
    }

    internal static class ServiceRepositoryEx
    {
        [NotNull]
        public static T GetOrCreateService<T>() where T : class, IService, new()
        {
            var repo = ServiceRepository.GetService<T>();

            if (repo != null)
            {
                return repo;
            }

            repo = new T();
            ServiceRepository.AddService(repo);

            return repo;
        }
    }

    private interface ISelectedCampaignService : IService
    {
        // string CampaignOrLocationName { get; }
        LocationType LocationType { get; }
        // string SaveGameDirectory { get; }
        // void SetCampaignLocation(string campaign, string location);
    }

    internal enum LocationType
    {
        StandardCampaign,
        UserLocation,
        CustomCampaign
    }

    internal sealed class SelectedCampaignService : ISelectedCampaignService
    {
        public string CampaignOrLocationName { get; private set; }
        public string SaveGameDirectory { get; private set; }
        public LocationType LocationType { get; private set; }

        public void SetCampaignLocation([CanBeNull] string campaign, [CanBeNull] string location)
        {
            Main.Log($"SetCampaignLocation: Campaign='{campaign}', Location='{location}'");

            var camp = campaign?.Trim() ?? string.Empty;
            var loc = location?.Trim() ?? string.Empty;

            if ((camp == UserCampaign || string.IsNullOrWhiteSpace(camp)) && !string.IsNullOrWhiteSpace(loc))
            {
                // User individual location
                SaveGameDirectory = Path.Combine(LocationSaveGameDirectory, loc);
                LocationType = LocationType.UserLocation;
                CampaignOrLocationName = location;
            }
            // this check not really needed, could just be !string.IsNullOrWhiteSpace(camp)
            else if (camp != CotmCampaign && camp != VotpCampaign && !string.IsNullOrWhiteSpace(camp))
            {
                // User campaign
                SaveGameDirectory = Path.Combine(CampaignSaveGameDirectory, camp);
                LocationType = LocationType.CustomCampaign;
                CampaignOrLocationName = campaign;
            }
            else
            {
                // Crown of the Magister or Lost Valley
                SaveGameDirectory = DefaultSaveGameDirectory;
                LocationType = LocationType.StandardCampaign;
                CampaignOrLocationName = string.Empty;
            }

            Main.Log($"SelectedCampaignService: Campaign='{camp}', Location='{loc}', Folder='{SaveGameDirectory}'");
        }

        public void SetStandardCampaignLocation()
        {
            SetCampaignLocation(string.Empty, string.Empty);
        }
    }
}
