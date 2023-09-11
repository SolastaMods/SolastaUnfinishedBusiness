using System;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionDieRollModifierBuilder
    : DefinitionBuilder<FeatureDefinitionDieRollModifier, FeatureDefinitionDieRollModifierBuilder>
{
    internal FeatureDefinitionDieRollModifierBuilder SetModifiers(
        RollContext context,
        int rerollCount,
        int minRollValue,
        int minReRollValue,
        string consoleLocalizationKey)
    {
        Definition.validityContext = context;
        Definition.rerollLocalizationKey = consoleLocalizationKey;
        Definition.rerollCount = rerollCount;
        Definition.minRollValue = minRollValue;
        Definition.minRerollValue = minReRollValue;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionDieRollModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDieRollModifierBuilder(FeatureDefinitionDieRollModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
