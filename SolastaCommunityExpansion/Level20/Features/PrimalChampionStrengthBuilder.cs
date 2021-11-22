using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class PrimalChampionStrengthBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        private const string PrimalChampionStrengthName = "ZSPrimalChampionStrength";
        private const string PrimalChampionStrengthGuid = "cfa0404178014d638d3ebd254a7ad77f";

        protected PrimalChampionStrengthBuilder(string name, string guid) : base(AttributeModifierTomeOfAllThings_STR, name, guid)
        {
            Definition.SetModifierValue(4);
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.GuiPresentation.Description = "Feature/&PrimalChampionStrengthDescription";
            Definition.GuiPresentation.Title = "Feature/&PrimalChampionStrengthTitle";
        }

        private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            => new PrimalChampionStrengthBuilder(name, guid).AddToDB();

        internal static readonly FeatureDefinitionAttributeModifier PrimalChampionStrength =
            CreateAndAddToDB(PrimalChampionStrengthName, PrimalChampionStrengthGuid);
    }
}
