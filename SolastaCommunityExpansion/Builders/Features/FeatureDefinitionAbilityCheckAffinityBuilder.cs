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
        protected FeatureDefinitionAbilityCheckAffinityBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
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
        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion

        public static FeatureDefinitionAbilityCheckAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionAbilityCheckAffinityBuilder Create(
            FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(original, name, guid);
        }

        // Add other standard Create methods and constructors as requi
    }
}
