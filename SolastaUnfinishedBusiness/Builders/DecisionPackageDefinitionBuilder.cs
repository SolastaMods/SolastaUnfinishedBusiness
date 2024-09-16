using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TA.AI;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class DecisionPackageDefinitionBuilder
    : DefinitionBuilder<DecisionPackageDefinition, DecisionPackageDefinitionBuilder>
{
#if false
    internal DecisionPackageDefinitionBuilder AddWeightedDecisions(
        params WeightedDecisionDescription[] weightedDecisionDescription)
    {
        Definition.Package.WeightedDecisions.AddRange(weightedDecisionDescription);

        return this;
    }
#endif

    internal DecisionPackageDefinitionBuilder SetWeightedDecisions(
        params WeightedDecisionDescription[] weightedDecisionDescription)
    {
        Definition.Package.WeightedDecisions.SetRange(weightedDecisionDescription);

        return this;
    }

#if false
    internal DecisionPackageDefinitionBuilder CopyWeightedDecisions(
        DecisionPackageDefinition decisionPackageDefinition, params int[] indexes)
    {
        var weightedDecisions = decisionPackageDefinition.Package.WeightedDecisions;

        if (indexes.Length == 0)
        {
            Definition.Package.WeightedDecisions.SetRange(weightedDecisions
                .Select(d => d.DeepCopy()));
        }
        else
        {
            foreach (var index in indexes)
            {
                if (index < weightedDecisions.Count)
                {
                    Definition.Package.WeightedDecisions.Add(weightedDecisions[index]);
                }
            }
        }

        return this;
    }
#endif

    #region Constructors

    protected DecisionPackageDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected DecisionPackageDefinitionBuilder(DecisionPackageDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
