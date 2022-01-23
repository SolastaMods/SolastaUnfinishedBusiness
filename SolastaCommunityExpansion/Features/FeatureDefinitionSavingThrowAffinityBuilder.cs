using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionSavingThrowAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSavingThrowAffinity>
    {
        public FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid, List<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, GuiPresentation guiPresentation) : base(name, guid)
        {
            foreach (string ability in abilityScores)
            {
                FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup group = new FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup
                {
                    abilityScoreName = ability,
                    affinity = affinityType
                };

                if (againstMagic)
                {
                    group.restrictedSchools.AddRange(
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

            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
