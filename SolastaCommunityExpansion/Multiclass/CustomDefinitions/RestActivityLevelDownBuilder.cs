using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.RestActivityDefinitions;

namespace SolastaCommunityExpansion.Multiclass.CustomDefinitions
{
    internal class RestActivityLevelDownBuilder : BaseDefinitionBuilder<RestActivityDefinition>
    {
        private const string LevelDownName = "ZSLevelDown";
        private const string LevelDownGuid = "fdb4d86eaef942d1a22dbf1fb5a7299f";

        private const RestActivityDefinition.ActivityCondition ActivityConditionCanLevelDown = (RestActivityDefinition.ActivityCondition)(-1002);

        protected RestActivityLevelDownBuilder(string name, string guid) : base(LevelUp, name, guid)
        {
            Definition.GuiPresentation.Title = "RestActivity/&ZSLevelDownTitle";
            Definition.GuiPresentation.Description = "RestActivity/&ZSLevelDownDescription";
            Definition.SetCondition(ActivityConditionCanLevelDown);
            Definition.SetFunctor(LevelDownName);
            ServiceRepository.GetService<IFunctorService>().RegisterFunctor(LevelDownName, new Models.LevelDownContext.FunctorLevelDown());
        }

        private static RestActivityDefinition CreateAndAddToDB(string name, string guid) => new RestActivityLevelDownBuilder(name, guid).AddToDB();

        internal static readonly RestActivityDefinition RestActivityLevelDown = CreateAndAddToDB(LevelDownName, LevelDownGuid);
    }
}
