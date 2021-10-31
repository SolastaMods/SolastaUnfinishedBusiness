using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionMagicAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
    {

        public FeatureDefinitionMagicAffinityBuilder(string name, string guid, RuleDefinitions.ConcentrationAffinity concentrationAffinity, int attackModifier,
            GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetConcentrationAffinity(concentrationAffinity);
            Definition.SetGuiPresentation(guiPresentation);

            Definition.SetSomaticWithWeaponOrShield(true);
            Definition.SetRangeSpellNoProximityPenalty(true);
            Definition.SetSpellAttackModifier(attackModifier);
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, string guid, List<string> spellNames,
            int levelBonus, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetUsesWarList(true);
            Definition.SetWarListSlotBonus(levelBonus);
            foreach (string spell in spellNames)
            {
                Definition.WarListSpells.Add(spell);
            }
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, string guid, int attackModifier,
            int dcModifier, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetSpellAttackModifier(attackModifier);
            Definition.SetSaveDCModifier(dcModifier);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
