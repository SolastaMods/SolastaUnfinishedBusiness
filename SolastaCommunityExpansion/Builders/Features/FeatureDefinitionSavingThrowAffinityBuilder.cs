using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSavingThrowAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionSavingThrowAffinity, FeatureDefinitionSavingThrowAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
        #endregion

        #region Factory methods
        public static FeatureDefinitionSavingThrowAffinityBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(name, guid);
        }

        public static FeatureDefinitionSavingThrowAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionSavingThrowAffinityBuilder Create(FeatureDefinitionSavingThrowAffinity original, string name, string guid)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(original, name, guid);
        }

        public static FeatureDefinitionSavingThrowAffinityBuilder Create(FeatureDefinitionSavingThrowAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(original, name, namespaceGuid);
        }
        #endregion

        public FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, params string[] abilityScores)
        {
            return SetAffinities(affinityType, againstMagic, abilityScores.AsEnumerable());
        }

        public FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, IEnumerable<string> abilityScores)
        {
            foreach (string ability in abilityScores)
            {
                var group = new SavingThrowAffinityGroup
                {
                    abilityScoreName = ability,
                    affinity = affinityType
                };

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

            return this;
        }
    }
}
