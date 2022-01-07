using System.IO;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class SaveByLocationContext
    {
        internal const string MAIN_CAMPAIGN = "CrownOfTheMagister";
        internal const string USER_CAMPAIGN = "UserCampaign";

        internal static readonly string DefaultSaveGameDirectory = Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");

        internal const string LocationSaveFolder = @"CE\Location";
        internal const string CampaignSaveFolder = @"CE\Campaign";

        internal static readonly string LocationSaveGameDirectory = Path.Combine(DefaultSaveGameDirectory, LocationSaveFolder);
        internal static readonly string CampaignSaveGameDirectory = Path.Combine(DefaultSaveGameDirectory, CampaignSaveFolder);

        internal static class ServiceRepositoryEx
        {
            public static T GetOrCreateService<T>() where T : class, IService, new()
            {
                var repo = ServiceRepository.GetService<T>();

                if (repo == null)
                {
                    repo = new T();
                    ServiceRepository.AddService(repo);
                }

                return repo;
            }
        }

        internal interface ISelectedCampaignService : IService
        {
            string CampaignOrLocationName { get; }
            void SetCampaignLocation(string campaign, string location);
            LocationType LocationType { get; }
            string SaveGameDirectory { get; }
        }

        internal enum LocationType
        {
            MainCampaign,
            UserLocation,
            CustomCampaign
        }

        internal class SelectedCampaignService : ISelectedCampaignService
        {
            public string CampaignOrLocationName { get; private set; }
            public LocationType LocationType { get; private set; }
            public string SaveGameDirectory { get; private set; }

            public void SetCampaignLocation(string campaign, string location)
            {
                var camp = campaign?.Trim() ?? string.Empty;
                var loc = location?.Trim() ?? string.Empty;

                if ((camp == USER_CAMPAIGN || string.IsNullOrWhiteSpace(camp)) && !string.IsNullOrWhiteSpace(loc))
                {
                    // User individual location
                    SaveGameDirectory = Path.Combine(LocationSaveGameDirectory, loc);
                    LocationType = LocationType.UserLocation;
                    CampaignOrLocationName = location;
                }
                else if (camp != MAIN_CAMPAIGN && !string.IsNullOrWhiteSpace(camp))
                {
                    // User campaign
                    SaveGameDirectory = Path.Combine(CampaignSaveGameDirectory, camp);
                    LocationType = LocationType.CustomCampaign;
                    CampaignOrLocationName = campaign;
                }
                else
                {
                    // Crown of the magister
                    SaveGameDirectory = DefaultSaveGameDirectory;
                    LocationType = LocationType.MainCampaign;
                    CampaignOrLocationName = string.Empty;
                }

                Main.Log($"Campaign='{camp}', Location='{loc}', Folder='{SaveGameDirectory}'");
            }
        }

        public static int SaveFileCount(LocationType locationType, string folder)
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
                case LocationType.MainCampaign:
                    {
                        var saveFolder = DefaultSaveGameDirectory;

                        return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
                    }
            }

            return 0;
        }
    }
}
