using SolastaModApi;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid, List<Tuple<string, string>> abilityProficiencyPairs,
        int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
        GuiPresentation guiPresentation) : base(name, guid)
        {
            foreach (Tuple<string, string> abilityProficiency in abilityProficiencyPairs)
            {
                FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup group = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup();
                group.abilityScoreName = abilityProficiency.Item1;
                if (!String.IsNullOrEmpty(abilityProficiency.Item2))
                {
                    group.proficiencyName = abilityProficiency.Item2;
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
