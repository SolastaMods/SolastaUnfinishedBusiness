using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionAbilityCheckAffinityBuilder CreateCopyFrom(
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
