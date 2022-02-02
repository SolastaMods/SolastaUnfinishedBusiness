using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid, List<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
        GuiPresentation guiPresentation) : base(name, guid)
        {
            foreach ((string abilityScoreName, string proficiencyName) in abilityProficiencyPairs)
            {
                var group = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
                {
                    abilityScoreName = abilityScoreName
                };

                if (!string.IsNullOrEmpty(proficiencyName))
                {
                    group.proficiencyName = proficiencyName;
                }

                group.affinity = affinityType;
                group.abilityCheckModifierDiceNumber = diceNumber;
                group.abilityCheckModifierDieType = dieType;
                Definition.AffinityGroups.Add(group);
            }
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
