using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

public class
    GuiTooltipClassDefinitionBuilder : DefinitionBuilder<GuiTooltipClassDefinition, GuiTooltipClassDefinitionBuilder>
{
    #region Constructors

    internal GuiTooltipClassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal GuiTooltipClassDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal GuiTooltipClassDefinitionBuilder(GuiTooltipClassDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    internal GuiTooltipClassDefinitionBuilder(GuiTooltipClassDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion

    public GuiTooltipClassDefinitionBuilder SetTooltipFeatures(IEnumerable<TooltipDefinitions.FeatureInfo> features)
    {
        Definition.tooltipFeatures.SetRange(features);
        return this;
    }

    public GuiTooltipClassDefinitionBuilder AddTooltipFeature(TooltipDefinitions.FeatureInfo feature)
    {
        Definition.tooltipFeatures.Add(feature);
        return this;
    }


    public GuiTooltipClassDefinitionBuilder SetPanelWidth(float width)
    {
        Definition.panelWidth = width;
        return this;
    }
}
