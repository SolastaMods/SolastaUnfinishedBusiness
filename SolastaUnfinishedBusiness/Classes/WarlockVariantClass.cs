using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Classes;

internal static class WarlockVariantClass
{
    public const string ClassName = "WarlockVariant";

    internal static CharacterClassDefinition Build()
    {
        var castSpellWarlockVariant = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellWarlock, "CastSpellWarlockVariant")
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .AddToDB();

        var proficiencyWarlockSavingThrowIntelligence =
            FeatureDefinitionProficiencyBuilder
                .Create(FeatureDefinitionProficiencys.ProficiencyWarlockSavingThrow,
                    "ProficiencyWarlockVariantSavingThrow")
                .SetProficiencies(ProficiencyType.SavingThrow,
                    AttributeDefinitions.Intelligence, AttributeDefinitions.Wisdom)
                .AddToDB();

        var warlockVariant = CharacterClassDefinitionBuilder
            .Create(Warlock, ClassName)
            .SetOrUpdateGuiPresentation(Category.Class)
            .AddFeaturesAtLevel(1, castSpellWarlockVariant, proficiencyWarlockSavingThrowIntelligence)
            .AddToDB();

        warlockVariant.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellWarlock ||
            x.FeatureDefinition == FeatureDefinitionProficiencys.ProficiencyWarlockSavingThrow);

        return warlockVariant;
    }
}
