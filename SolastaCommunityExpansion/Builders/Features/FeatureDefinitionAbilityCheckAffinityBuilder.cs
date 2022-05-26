using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using static FeatureDefinitionAbilityCheckAffinity;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionAbilityCheckAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionAffinityBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionAbilityCheckAffinity
        where TBuilder : FeatureDefinitionAbilityCheckAffinityBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity affinityType,
            DieType dieType, int diceNumber, params (string abilityScoreName, string proficiencyName)[] abilityProficiencyPairs)
        {
            return BuildAndSetAffinityGroups(affinityType, dieType, diceNumber, abilityProficiencyPairs.AsEnumerable());
        }

        public TBuilder BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity affinityType,
            DieType dieType, int diceNumber, IEnumerable<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs)
        {
            SetAffinityGroups(
                abilityProficiencyPairs.Select(pair => new AbilityCheckAffinityGroup
                {
                    abilityScoreName = pair.abilityScoreName,
                    proficiencyName = (pair.proficiencyName ?? string.Empty).Trim(),
                    affinity = affinityType,
                    abilityCheckModifierDiceNumber = diceNumber,
                    abilityCheckModifierDieType = dieType
                }));

            return This();
        }

        public TBuilder SetAffinityGroups(IEnumerable<AbilityCheckAffinityGroup> affinityGroups)
        {
            Definition.SetAffinityGroups(affinityGroups);
            Definition.AffinityGroups.Sort(Sorting.Compare);
            return This();
        }

        public TBuilder SetAffinityGroups(params AbilityCheckAffinityGroup[] affinityGroups)
        {
            return SetAffinityGroups(affinityGroups.AsEnumerable());
        }
    }

    public class FeatureDefinitionAbilityCheckAffinityBuilder : FeatureDefinitionAbilityCheckAffinityBuilder<FeatureDefinitionAbilityCheckAffinity, FeatureDefinitionAbilityCheckAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
