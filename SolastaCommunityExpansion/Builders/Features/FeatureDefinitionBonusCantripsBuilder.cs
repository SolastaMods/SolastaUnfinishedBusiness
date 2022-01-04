using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
    {
        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionBonusCantripsBuilder ClearBonusCantrips()
        {
            Definition.BonusCantrips.Clear();
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder AddBonusCantrip(SpellDefinition spellDefinition)
        {

            Definition.BonusCantrips.Add(spellDefinition);
            return this;
        }

    }

}
