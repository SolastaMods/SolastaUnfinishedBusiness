using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSavingThrowAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionSavingThrowAffinity, FeatureDefinitionSavingThrowAffinityBuilder>
{
    internal FeatureDefinitionSavingThrowAffinityBuilder SetAffinities(
        RuleDefinitions.CharacterSavingThrowAffinity affinityType,
        bool againstMagic,
        params string[] abilityScores)
    {
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

    internal FeatureDefinitionSavingThrowAffinityBuilder SetModifiers(
        ModifierType modifierType,
        RuleDefinitions.DieType modifierDieType,
        int modifierDieNumber,
        bool againstMagic,
        params string[] abilityScores)
    {
        foreach (var ability in abilityScores)
        {
            var group = new SavingThrowAffinityGroup
            {
                abilityScoreName = ability,
                affinity = RuleDefinitions.CharacterSavingThrowAffinity.None,
                savingThrowModifierType = modifierType,
                savingThrowModifierDieType = modifierDieType,
                savingThrowModifierDiceNumber = modifierDieNumber
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

        Definition.AffinityGroups.Sort(Sorting.Compare);
        return this;
    }

#if false
    internal FeatureDefinitionSavingThrowAffinityBuilder UseControllerSavingThrows(bool value = true)
    {
        Definition.useControllerSavingThrows = value;
        return this;
    }
#endif

    #region Constructors

    protected FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSavingThrowAffinityBuilder(FeatureDefinitionSavingThrowAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
