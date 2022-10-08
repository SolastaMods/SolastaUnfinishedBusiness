using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static FeatureDefinitionAbilityCheckAffinity;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAbilityCheckAffinityBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionAbilityCheckAffinity, FeatureDefinitionAbilityCheckAffinityBuilder>
{
    internal FeatureDefinitionAbilityCheckAffinityBuilder BuildAndSetAffinityGroups(
        CharacterAbilityCheckAffinity affinityType,
        DieType dieType,
        int diceNumber,
        params (string abilityScoreName, string proficiencyName)[] abilityProficiencyPairs)
    {
        Definition.AffinityGroups.SetRange(
            abilityProficiencyPairs.Select(pair => new AbilityCheckAffinityGroup
            {
                abilityScoreName = pair.abilityScoreName,
                proficiencyName = (pair.proficiencyName ?? string.Empty).Trim(),
                affinity = affinityType,
                abilityCheckModifierDiceNumber = diceNumber,
                abilityCheckModifierDieType = dieType
            }));
        Definition.AffinityGroups.Sort(Sorting.Compare);
        return this;
    }

    internal FeatureDefinitionAbilityCheckAffinityBuilder SetAffinityGroups(params AbilityCheckAffinityGroup[] affinityGroups)
    {
        Definition.AffinityGroups.SetRange(affinityGroups);
        return this;
    }
    
    #region Constructors

    protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original,
        string name, string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
