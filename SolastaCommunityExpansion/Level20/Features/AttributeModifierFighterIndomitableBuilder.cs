using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class AttributeModifierFighterIndomitableBuilder : FeatureDefinitionAttributeModifierBuilder
    {
        private const string AttributeModifierFighterIndomitableAddName = "ZSAttributeModifierFighterIndomitableAdd";
        private const string AttributeModifierFighterIndomitableAddGuid = "8a2f09cafd7b47d886cb0ce098c4f477";

        private AttributeModifierFighterIndomitableBuilder(string name, string guid) : base(AttributeModifierFighterIndomitable, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&IndomitableResistanceAddTitle";
            Definition.GuiPresentation.Description = "Feature/&IndomitableResistanceAddDescription";
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
        }

        private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
        {
            return new AttributeModifierFighterIndomitableBuilder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionAttributeModifier AttributeModifierFighterIndomitableAdd =
            CreateAndAddToDB(AttributeModifierFighterIndomitableAddName, AttributeModifierFighterIndomitableAddGuid);
    }
}
