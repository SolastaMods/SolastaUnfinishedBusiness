using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class PrimalChampionConstitutionBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        private const string PrimalChampionConstitutionName = "ZSPrimalChampionConstitution";
        private const string PrimalChampionConstitutionGuid = "8fd61e6f5ded4dfea2d8783b16c7ca1c";

        protected PrimalChampionConstitutionBuilder(string name, string guid) : base(AttributeModifierTomeOfAllThings_CON, name, guid)
        {
            Definition.SetModifierValue(4);
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.GuiPresentation.Description = "Feature/&PrimalChampionConstitutionDescription";
            Definition.GuiPresentation.Title = "Feature/&PrimalChampionConstitutionTitle";
        }

        private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            => new PrimalChampionConstitutionBuilder(name, guid).AddToDB();

        internal static readonly FeatureDefinitionAttributeModifier PrimalChampionConstitution =
            CreateAndAddToDB(PrimalChampionConstitutionName, PrimalChampionConstitutionGuid);
    }
}
