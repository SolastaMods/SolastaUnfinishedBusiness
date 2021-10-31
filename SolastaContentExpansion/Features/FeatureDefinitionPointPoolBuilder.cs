using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionPointPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        public FeatureDefinitionPointPoolBuilder(string name, string guid, HeroDefinitions.PointsPoolType poolType,
            int pointPoolSize, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetPoolAmount(pointPoolSize);
            Definition.SetPoolType(poolType);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
