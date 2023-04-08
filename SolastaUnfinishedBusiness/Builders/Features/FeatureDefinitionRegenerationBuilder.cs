using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionRegenerationBuilder
    : DefinitionBuilder<FeatureDefinitionRegeneration, FeatureDefinitionRegenerationBuilder>
{
    internal FeatureDefinitionRegenerationBuilder SetDuration(RuleDefinitions.DurationType durationType, int duration)
    {
        Definition.tickNumber = duration;
        Definition.tickType = durationType;
        return this;
    }

    internal FeatureDefinitionRegenerationBuilder SetRegenerationDice(
        RuleDefinitions.DieType dieType, int diceNumber, int bonus)
    {
        Definition.dieType = dieType;
        Definition.diceNumber = diceNumber;
        Definition.bonus = bonus;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionRegenerationBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionRegenerationBuilder(FeatureDefinitionRegeneration original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
