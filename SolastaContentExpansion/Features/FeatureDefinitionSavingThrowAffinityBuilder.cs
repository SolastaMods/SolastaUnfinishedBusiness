using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionSavingThrowAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSavingThrowAffinity>
    {
        public FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid, List<string> abilityScores,
        RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, GuiPresentation guiPresentation) : base(name, guid)
        {
            foreach (string ability in abilityScores)
            {
                FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup group = new FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup();
                group.abilityScoreName = ability;
                group.affinity = affinityType;
                if (againstMagic)
                {
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolAbjuration.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolDivination.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEnchantment.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolIllusion.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolNecromancy.Name);
                    group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation.Name);
                }
                Definition.AffinityGroups.Add(group);
            }

            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
