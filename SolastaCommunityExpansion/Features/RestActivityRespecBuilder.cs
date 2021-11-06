using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.RestActivityDefinitions;

namespace SolastaCommunityExpansion.Features
{
    public class RestActivityRespecBuilder : BaseDefinitionBuilder<RestActivityDefinition>
    {
        private const string RespecName = "ZSRespec";
        private const string RespecGuid = "40824029eb224fb581f0d4e5989b6735";

        protected RestActivityRespecBuilder(string name, string guid) : base(LevelUp, name, guid)
        {
            Definition.GuiPresentation.Title = "RestActivity/&ZSRespecTitle";
            Definition.GuiPresentation.Description = "RestActivity/&ZSRespecDescription";
            Definition.SetCondition<RestActivityDefinition>(Settings.ActivityConditionCanRespec);
            Definition.SetFunctor<RestActivityDefinition>(RespecName);
            ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new Functors.FunctorRespec());
        }

        private static RestActivityDefinition CreateAndAddToDB(string name, string guid)
            => new RestActivityRespecBuilder(name, guid).AddToDB();

        public static readonly RestActivityDefinition RestActivityRespec
            = CreateAndAddToDB(RespecName, RespecGuid);
    }
}