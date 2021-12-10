namespace SolastaCommunityExpansion.Models
{
    internal static class SaveByLocationContext
    {
        internal const string MAIN_CAMPAIGN = "CrownOfTheMagister";
        internal const string USER_CAMPAIGN = "UserCampaign";

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
        }
    }
}
