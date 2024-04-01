using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CraftyFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyArcana")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Arcana)
            .AddToDB();

        var proficiencyCraftyAnimalHandling = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyAnimalHandling")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.AnimalHandling)
            .AddToDB();

        var proficiencyCraftyMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyMedicine")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
            .AddToDB();

        var proficiencyCraftyNature = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyNature")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Nature)
            .AddToDB();

        var proficiencyCraftyHerbalismKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyHerbalismKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var proficiencyCraftyPoisonersKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyPoisonersKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var proficiencyCraftyScrollKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyScrollKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        //
        // Apothecary
        //

        var featApothecaryInt = FeatDefinitionBuilder
            .Create("FeatApothecaryInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyHerbalismKit, proficiencyCraftyArcana)
            .SetFeatFamily("Apothecary")
            .AddToDB();

        var featApothecaryWis = FeatDefinitionBuilder
            .Create("FeatApothecaryWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, proficiencyCraftyHerbalismKit, proficiencyCraftyMedicine)
            .SetFeatFamily("Apothecary")
            .AddToDB();

        var featApothecaryCha = FeatDefinitionBuilder
            .Create("FeatApothecaryCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, proficiencyCraftyHerbalismKit, proficiencyCraftyMedicine)
            .SetFeatFamily("Apothecary")
            .AddToDB();

        var featGroupApothecary = GroupFeats.MakeGroup("FeatGroupApothecary", "Apothecary",
            featApothecaryInt,
            featApothecaryWis,
            featApothecaryCha);

        //
        // Toxicologist
        //

        var featToxicologistInt = FeatDefinitionBuilder
            .Create("FeatToxicologistInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyPoisonersKit, proficiencyCraftyNature)
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featToxicologistWis = FeatDefinitionBuilder
            .Create("FeatToxicologistWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, proficiencyCraftyPoisonersKit, proficiencyCraftyMedicine)
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featToxicologistCha = FeatDefinitionBuilder
            .Create("FeatToxicologistCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, proficiencyCraftyPoisonersKit,
                proficiencyCraftyAnimalHandling)
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featGroupToxicologist = GroupFeats.MakeGroup("FeatGroupToxicologist", "Toxicologist",
            featToxicologistInt,
            featToxicologistWis,
            featToxicologistCha);

        //
        // Scriber
        //

        var featCraftyScriber = FeatDefinitionBuilder
            .Create("FeatCraftyScriber")
            .SetGuiPresentation(Category.Feat)
            .SetMustCastSpellsPrerequisite()
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyScrollKit, proficiencyCraftyArcana)
            .AddToDB();

        //
        // MAIN
        //

        feats.AddRange(
            featApothecaryInt,
            featApothecaryWis,
            featApothecaryCha,
            featToxicologistInt,
            featToxicologistWis,
            featToxicologistCha,
            featCraftyScriber);

        GroupFeats.FeatGroupSkills.AddFeats(
            featGroupApothecary,
            featGroupToxicologist,
            featCraftyScriber);

        GroupFeats.FeatGroupTools.AddFeats(
            featGroupToxicologist,
            featCraftyScriber,
            FeatDefinitions.InitiateAlchemist,
            FeatDefinitions.MasterAlchemist,
            FeatDefinitions.InitiateEnchanter,
            FeatDefinitions.MasterEnchanter);
    }
}
