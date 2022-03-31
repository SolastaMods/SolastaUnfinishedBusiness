using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaMulticlass.Models.LevelDownContext;

namespace SolastaMulticlass.CustomDefinitions
{
    internal sealed class RestActivityLevelDownBuilder : BaseDefinitionBuilder<RestActivityDefinition>
    {
        private const string LevelDownName = "LevelDown";
        private const string LevelDownGuid = "fdb4d86eaef942d1a22dbf1fb5a7299f";

        private const RestActivityDefinition.ActivityCondition ActivityConditionCanLevelDown = (RestActivityDefinition.ActivityCondition)(-1002);

        [Obsolete]
        private RestActivityLevelDownBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "RestActivity/&LevelDownTitle";
            Definition.GuiPresentation.Description = "RestActivity/&LevelDownDescription";
            Definition.SetCondition(ActivityConditionCanLevelDown);
            Definition.SetFunctor(LevelDownName);
            ServiceRepository.GetService<IFunctorService>().RegisterFunctor(LevelDownName, new FunctorLevelDown());
        }

        [Obsolete]
        private static RestActivityDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RestActivityLevelDownBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        internal static readonly RestActivityDefinition RestActivityLevelDown = CreateAndAddToDB(LevelDownName, LevelDownGuid);
    }
}
