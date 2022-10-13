using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

/*
Note this is based on FeatureDefinitionPower so that you can take advantage of power usage calculations
like proficiency or ability score usage. However in order to do that the game needs to add a power to
the hero and only one power for a given name+guid is added. Which means if you want to add a +1 modifier
at 4 different character levels you need to create 4 different FeatureDefinitionPowerPoolModifier.
*/
[UsedImplicitly]
internal class FeatureDefinitionPowerPoolModifierBuilder
    : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerPoolModifier, FeatureDefinitionPowerPoolModifierBuilder>
{
    internal override void Validate()
    {
        base.Validate();

        Preconditions.ArgumentIsNotNull(Definition.PoolPower,
            $"{GetType().Name}[{Definition.Name}].PoolPower is null.");
        Preconditions.AreEqual(Definition.CostPerUse, 1,
            $"{GetType().Name}[{Definition.Name}].CostPerUse must be set to 1.");
    }

    internal FeatureDefinitionPowerPoolModifierBuilder SetPoolModifier(
        FeatureDefinitionPower poolPower,
        int powerPoolModifier)
    {
        Preconditions.ArgumentIsNotNull(poolPower, $"{GetType().Name}[{Definition.Name}] poolPower is null.");

        Definition.fixedUsesPerRecharge = powerPoolModifier;
        Definition.overriddenPower = Definition;
        Definition.PoolPower = poolPower;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionPowerPoolModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerPoolModifierBuilder(FeatureDefinitionPowerPoolModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
