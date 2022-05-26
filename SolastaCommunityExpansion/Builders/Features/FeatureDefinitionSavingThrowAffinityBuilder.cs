using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSavingThrowAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionSavingThrowAffinity,
            FeatureDefinitionSavingThrowAffinityBuilder>
    {
        public FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(
            RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, params string[] abilityScores)
        {
            return SetAffinities(affinityType, againstMagic, abilityScores.AsEnumerable());
        }

        public FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(
            RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic,
            IEnumerable<string> abilityScores)
        {
            // TODO: this isn't a set, it's an Add

            foreach (var ability in abilityScores)
            {
                var group = new SavingThrowAffinityGroup {abilityScoreName = ability, affinity = affinityType};

                if (againstMagic)
                {
                    group.restrictedSchools.SetRange(
                        SchoolAbjuration.Name,
                        SchoolConjuration.Name,
                        SchoolDivination.Name,
                        SchoolEnchantment.Name,
                        SchoolEvocation.Name,
                        SchoolIllusion.Name,
                        SchoolNecromancy.Name,
                        SchoolTransmutation.Name);
                }

                Definition.AffinityGroups.Add(group);
            }

            Definition.AffinityGroups.Sort(Sorting.Compare);
            return this;
        }

        #region Constructors

        protected FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original,
            string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original,
            string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        #endregion
    }
}
