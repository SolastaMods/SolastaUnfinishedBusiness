using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        private FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        private FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public static FeatureDefinitionAbilityCheckAffinityBuilder Create(
            string name, Guid namespaceGuid, Category category = Category.None)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name, namespaceGuid, category);
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
