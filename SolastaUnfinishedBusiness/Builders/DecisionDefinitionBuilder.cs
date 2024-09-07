using System;
using JetBrains.Annotations;
using TA.AI;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class DecisionDefinitionBuilder
    : DefinitionBuilder<DecisionDefinition, DecisionDefinitionBuilder>
{
    internal DecisionDefinitionBuilder SetDecisionDescription(
        string description,
        string activityType,
        ActivityScorerDefinition scorer,
        string stringParameter = "",
        string stringSecParameter = "",
        bool boolParameter = false,
        bool boolSecParameter = false,
        float floatParameter = 0,
        int enumParameter = 0)
    {
        Definition.decision = new DecisionDescription
        {
            description = description,
            activityType = activityType,
            scorer = scorer,
            stringParameter = stringParameter,
            stringSecParameter = stringSecParameter,
            boolParameter = boolParameter,
            boolSecParameter = boolSecParameter,
            floatParameter = floatParameter,
            enumParameter = enumParameter
        };

        return this;
    }

    #region Constructors

    protected DecisionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected DecisionDefinitionBuilder(DecisionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
