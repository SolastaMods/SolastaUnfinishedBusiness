using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionDieRollModifierBuilder
    : FeatureDefinitionAffinityBuilder<FeatureDefinitionDieRollModifier, FeatureDefinitionDieRollModifierBuilder>
{
    public FeatureDefinitionDieRollModifierBuilder SetModifiers(
        RuleDefinitions.RollContext context, int rerollCount, int minRerollValue, string consoleLocalizationKey)
    {
        Definition.validityContext = context;
        Definition.rerollLocalizationKey = consoleLocalizationKey;
        Definition.rerollCount = rerollCount;
        Definition.minRerollValue = minRerollValue;
        Definition.minRollValue = minRerollValue;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionDieRollModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDieRollModifierBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionDieRollModifierBuilder(FeatureDefinitionDieRollModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDieRollModifierBuilder(FeatureDefinitionDieRollModifier original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
