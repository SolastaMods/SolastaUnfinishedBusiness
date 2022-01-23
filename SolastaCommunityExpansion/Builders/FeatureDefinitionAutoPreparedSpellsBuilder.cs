using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class FeatureDefinitionAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(int classLevel, List<SpellDefinition> spellnames)
        {
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellgroup = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = classLevel,
                SpellsList = new List<SpellDefinition>()
            };
            spellgroup.SpellsList.AddRange(spellnames);
            return spellgroup;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid, List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetField("autoPreparedSpellsGroups",
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>(autospelllists));

            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid) : base(name, guid)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name, string guid) : base(original, name, guid)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(params FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[] autospelllists)
        {
            return SetPreparedSpellGroups(autospelllists.AsEnumerable());
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists)
        {
            Definition.SetField("autoPreparedSpellsGroups",
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>(autospelllists));

            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetGuiPresentation(GuiPresentation guiPresentation)
        {
            Definition.SetGuiPresentation(guiPresentation);
            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetCharacterClass(CharacterClassDefinition castingClass)
        {
            Definition.SetSpellcastingClass(castingClass);
            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetAffinityRace(CharacterRaceDefinition castingRace)
        {
            Definition.SetAffinityRace(castingRace);
            return this;
        }

        /**
         * This tag is used to create a tooltip:
         * this.autoPreparedTitle.Text = string.Format("Screen/&{0}SpellTitle", autoPreparedTag);
	     * this.autoPreparedTooltip.Content = string.Format("Screen/&{0}SpellDescription", autoPreparedTag);
         */
        public FeatureDefinitionAutoPreparedSpellsBuilder SetAutoTag(string tag)
        {
            Definition.SetAutopreparedTag(tag);
            return this;
        }
    }
}
