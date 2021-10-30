using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCJDExtraContent.Models.Features
{
    internal class PointPoolBonusFeatsBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        public PointPoolBonusFeatsBuilder(
            string name,
            string guid,
            HeroDefinitions.PointsPoolType poolType,
            int pointPoolSize,
            GuiPresentation guiPresentation)
            : base(name, guid)
        {
            FeatureDefinitionPointPoolExtensions.SetPoolAmount<FeatureDefinitionPointPool>(this.Definition, pointPoolSize);
            FeatureDefinitionPointPoolExtensions.SetPoolType<FeatureDefinitionPointPool>(this.Definition, poolType);
            BaseDefinitionExtensions.SetGuiPresentation<FeatureDefinitionPointPool>(this.Definition, guiPresentation);
        }
    }
}