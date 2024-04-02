using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;
using static FeatureDefinitionAbilityCheckAffinity;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAbilityCheckAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionAbilityCheckAffinity, FeatureDefinitionAbilityCheckAffinityBuilder>
{
    internal FeatureDefinitionAbilityCheckAffinityBuilder BuildAndSetAffinityGroups(
        CharacterAbilityCheckAffinity affinityType = CharacterAbilityCheckAffinity.None,
        DieType dieType = DieType.D1,
        int diceNumber = 0,
        params (string abilityScoreName, string proficiencyName)[] abilityProficiencyPairs)
    {
        Definition.AffinityGroups.SetRange(
            abilityProficiencyPairs
                .Select(pair => new AbilityCheckAffinityGroup
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

    internal FeatureDefinitionAbilityCheckAffinityBuilder BuildAndSetAffinityGroups(
        CharacterAbilityCheckAffinity affinityType = CharacterAbilityCheckAffinity.None,
        DieType dieType = DieType.D1,
        int diceNumber = 0,
        params (string abilityScoreName, string proficiencyName, AbilityCheckContext abilityCheckContext)[]
            abilityProficiencyPairs)
    {
        Definition.AffinityGroups.SetRange(
            abilityProficiencyPairs
                .Select(tuple => new AbilityCheckAffinityGroup
                {
                    abilityScoreName = tuple.abilityScoreName,
                    proficiencyName = (tuple.proficiencyName ?? string.Empty).Trim(),
                    affinity = affinityType,
                    abilityCheckModifierDiceNumber = diceNumber,
                    abilityCheckModifierDieType = dieType,
                    abilityCheckContext = tuple.abilityCheckContext
                }));
        Definition.AffinityGroups.Sort(Sorting.Compare);
        return this;
    }

    internal FeatureDefinitionAbilityCheckAffinityBuilder BuildAndAddAffinityGroups(
        CharacterAbilityCheckAffinity affinityType = CharacterAbilityCheckAffinity.None,
        DieType dieType = DieType.D1,
        int diceNumber = 0,
        params (string abilityScoreName, string proficiencyName)[] abilityProficiencyPairs)
    {
        Definition.AffinityGroups.AddRange(
            abilityProficiencyPairs
                .Select(pair => new AbilityCheckAffinityGroup
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

    private FeatureDefinitionAbilityCheckAffinityBuilder BuildAndSetAffinityGroups(
        CharacterAbilityCheckAffinity affinityType = CharacterAbilityCheckAffinity.None,
        DieType dieType = DieType.D1,
        int diceNumber = 0,
        params string[] abilityScores)
    {
        return BuildAndSetAffinityGroups(affinityType, dieType, diceNumber,
            abilityScores.Select(a => (a, string.Empty)).ToArray());
    }

    internal FeatureDefinitionAbilityCheckAffinityBuilder BuildAndSetAffinityGroups(
        CharacterAbilityCheckAffinity affinityType,
        params string[] abilityScores)
    {
        return BuildAndSetAffinityGroups(affinityType, DieType.D1, 0, abilityScores);
    }

#if false
    internal FeatureDefinitionAbilityCheckAffinityBuilder UseControllerAbilityChecks()
    {
        Definition.useControllerAbilityChecks = true;
        return this;
    }
#endif

    #region Constructors

    protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
