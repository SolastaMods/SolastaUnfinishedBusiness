using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    GuiTooltipClassDefinitionBuilder : DefinitionBuilder<GuiTooltipClassDefinition, GuiTooltipClassDefinitionBuilder>
{
    internal GuiTooltipClassDefinitionBuilder AddTooltipFeature(TooltipDefinitions.FeatureInfo feature)
    {
        Definition.tooltipFeatures.Add(feature);
        return this;
    }

#if false
    internal GuiTooltipClassDefinitionBuilder SetTooltipFeatures(IEnumerable<TooltipDefinitions.FeatureInfo> features)
    {
        Definition.tooltipFeatures.SetRange(features);
        return this;
    }

    internal GuiTooltipClassDefinitionBuilder SetPanelWidth(float width)
    {
        Definition.panelWidth = width;
        return this;
    }
#endif

    #region Constructors

    internal GuiTooltipClassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal GuiTooltipClassDefinitionBuilder(GuiTooltipClassDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
