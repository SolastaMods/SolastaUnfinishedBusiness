using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        // TODO: remove/refactor this ctor
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid, IEnumerable<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType, GuiPresentation guiPresentation)
            : base(name, guid)
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
