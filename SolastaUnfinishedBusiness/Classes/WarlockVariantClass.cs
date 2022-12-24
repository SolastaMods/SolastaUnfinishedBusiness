using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Classes;

internal static class WarlockVariantClass
{
    public const string ClassName = "WarlockVariant";

    internal static CharacterClassDefinition Build()
    {
        var castSpellWarlockVariant = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellWarlock, "CastSpellWarlockVariant")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .AddToDB();

        var pointPoolWarlockVariantSkillPoints = FeatureDefinitionPointPoolBuilder
            .Create(PointPoolWarlockSkillPoints, "PointPoolWarlockVariantSkillPoints")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyWarlockVariantArmor = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyWarlockArmor, "ProficiencyWarlockVariantArmor")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyWarlockVariantSavingThrow =
            FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyWarlockSavingThrow, "ProficiencyWarlockVariantSavingThrow")
                .SetProficiencies(ProficiencyType.SavingThrow,
                    AttributeDefinitions.Intelligence, AttributeDefinitions.Wisdom)
                .AddToDB();

        var proficiencyWarlockVariantWeapon = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyWarlockWeapon, "ProficiencyWarlockVariantWeapon")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var warlockVariant = CharacterClassDefinitionBuilder
            .Create(Warlock, ClassName)
            .SetOrUpdateGuiPresentation(Category.Class)
            .AddFeaturesAtLevel(1,
                castSpellWarlockVariant,
                pointPoolWarlockVariantSkillPoints,
                proficiencyWarlockVariantArmor,
                proficiencyWarlockVariantSavingThrow,
                proficiencyWarlockVariantWeapon)
            .SetAbilityScorePriorities(
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Strength)
            .AddToDB();

        warlockVariant.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellWarlock ||
            x.FeatureDefinition == PointPoolWarlockSkillPoints ||
            x.FeatureDefinition == ProficiencyWarlockArmor ||
            x.FeatureDefinition == ProficiencyWarlockSavingThrow ||
            x.FeatureDefinition == ProficiencyWarlockWeapon);

        return warlockVariant;
    }
}
