using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionAbilityCheckAffinityBuilder : DefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        private FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionAbilityCheckAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionAbilityCheckAffinityBuilder Create(
            FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(original, name, guid);
        }

        // TODO: is this a method good name?
        public FeatureDefinitionAbilityCheckAffinityBuilder SetAbilityAffinities(
            IEnumerable<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType)
        {
            Definition.AffinityGroups.SetRange(
                abilityProficiencyPairs.Select(pair => new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
                {
                    abilityScoreName = pair.abilityScoreName,
                    proficiencyName = (pair.proficiencyName ?? string.Empty).Trim(),
                    affinity = affinityType,
                    abilityCheckModifierDiceNumber = diceNumber,
                    abilityCheckModifierDieType = dieType
                }));

            return this;
        }
    }
}
