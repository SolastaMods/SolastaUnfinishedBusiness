using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSavingThrowAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionSavingThrowAffinity,
        FeatureDefinitionSavingThrowAffinityBuilder>
{
    internal FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(
        RuleDefinitions.CharacterSavingThrowAffinity affinityType,
        bool againstMagic,
        params string[] abilityScores)
    {
        // TODO: this isn't a set, it's an Add

        foreach (var ability in abilityScores)
        {
            var group = new SavingThrowAffinityGroup { abilityScoreName = ability, affinity = affinityType };

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

    internal FeatureDefinitionSavingThrowAffinityBuilder UseControllerSavingThrows(bool value = true)
    {
        Definition.useControllerSavingThrows = value;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original,
        string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
