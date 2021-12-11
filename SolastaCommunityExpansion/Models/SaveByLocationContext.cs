using System.IO;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class SaveByLocationContext
    {
        internal const string MAIN_CAMPAIGN = "CrownOfTheMagister";
        internal const string USER_CAMPAIGN = "UserCampaign";

        internal static readonly string BaseSaveFolder = Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");

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

        interface ISelectedCampaignService : IService
        {
            string Campaign { get; set; }
            string Location { get; set; }
        }

        internal class SelectedCampaignService : ISelectedCampaignService
        {
            public string Campaign { get; set; }
            public string Location { get; set; }

            public string GetFolderName()
            {
                string folder = string.Empty;

                if ((Campaign == USER_CAMPAIGN || string.IsNullOrWhiteSpace(Campaign)) && !string.IsNullOrWhiteSpace(Location))
                {
                    folder = $@"CE\Location\{Location.Trim()}";
                }
                else if (Campaign != MAIN_CAMPAIGN && !string.IsNullOrWhiteSpace(Campaign))
                {
                    folder = $@"CE\Campaign\{Campaign.Trim()}";
                }

                Main.Log($"Campaign='{Campaign}', Location='{Location}', Folder='{folder}'");

                return folder;
            }
        }

        public static bool HasSaves(bool? isLocation, string folder)
        {
            switch (isLocation)
            {
                case true:
                    {
                        var saveFolder = Path.Combine(BaseSaveFolder, $@"CE\Location\{folder}");

                        var hasSaves = Directory.Exists(saveFolder) && Directory.EnumerateFiles(saveFolder, "*.sav").Any();

                        Main.Log($"Scanning {saveFolder}, hasSaves={hasSaves}");

                        return hasSaves;
                    }
                case false:
                    {
                        var saveFolder = Path.Combine(BaseSaveFolder, $@"CE\Campaign\{folder}");

                        var hasSaves = Directory.Exists(saveFolder) && Directory.EnumerateFiles(saveFolder, "*.sav").Any();

                        Main.Log($"Scanning {saveFolder}, hasSaves={hasSaves}");

                        return hasSaves;
                    }
            }

            return true;
        }
    }
}
