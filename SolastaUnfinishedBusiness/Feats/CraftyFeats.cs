using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
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
        // skill

        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyArcana")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyAnimalHandling")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.AnimalHandling)
            .AddToDB();

        var proficiencyCraftyMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyMedicine")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Medecine)
            .AddToDB();

        var proficiencyCraftyNature = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyNature")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Nature)
            .AddToDB();

        var proficiencyCraftyReligion = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyReligion")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Religion)
            .AddToDB();

        var proficiencyCraftyHerbalismKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyHerbalismKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Tool, ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var proficiencyCraftyPoisonersKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyPoisonersKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Tool, ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var proficiencyCraftyScrollKit = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyScrollKit")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Tool, ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        var proficiencyCraftyArcanaExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyArcanaExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, SkillDefinitions.Arcana)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyAnimalHandlingExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, SkillDefinitions.AnimalHandling)
            .AddToDB();

        var proficiencyCraftyMedicineExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyMedicineExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, SkillDefinitions.Medecine)
            .AddToDB();

        var proficiencyCraftyNatureExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyNatureExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, SkillDefinitions.Nature)
            .AddToDB();

        var proficiencyCraftyReligionExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyReligionExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, SkillDefinitions.Religion)
            .AddToDB();

        var proficiencyCraftyHerbalismKitExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyHerbalismKitExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var proficiencyCraftyPoisonersKitExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyPoisonersKitExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var proficiencyCraftyScrollKitExpertise = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyScrollKitExpertise")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Expertise, ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        //
        // Apothecary
        //

        var featApothecaryInt = FeatDefinitionBuilder
            .Create("FeatApothecaryInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.HerbalismKitType,
                    proficiencyCraftyHerbalismKit, proficiencyCraftyHerbalismKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Medecine,
                    proficiencyCraftyMedicine, proficiencyCraftyMedicineExpertise))
            .SetFeatFamily("Apothecary")
            .AddToDB();

        var featApothecaryWis = FeatDefinitionBuilder
            .Create("FeatApothecaryWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.HerbalismKitType,
                    proficiencyCraftyHerbalismKit, proficiencyCraftyHerbalismKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Medecine,
                    proficiencyCraftyMedicine, proficiencyCraftyMedicineExpertise))
            .SetFeatFamily("Apothecary")
            .AddToDB();

        var featApothecaryCha = FeatDefinitionBuilder
            .Create("FeatApothecaryCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.HerbalismKitType,
                    proficiencyCraftyHerbalismKit, proficiencyCraftyHerbalismKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Medecine,
                    proficiencyCraftyMedicine, proficiencyCraftyMedicineExpertise))
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
            .SetFeatures(AttributeModifierCreed_Of_Pakri)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.PoisonersKitType,
                    proficiencyCraftyPoisonersKit, proficiencyCraftyPoisonersKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Nature,
                    proficiencyCraftyNature, proficiencyCraftyNatureExpertise))
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featToxicologistWis = FeatDefinitionBuilder
            .Create("FeatToxicologistWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.PoisonersKitType,
                    proficiencyCraftyPoisonersKit, proficiencyCraftyPoisonersKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Nature,
                    proficiencyCraftyNature, proficiencyCraftyNatureExpertise))
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featToxicologistCha = FeatDefinitionBuilder
            .Create("FeatToxicologistCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.PoisonersKitType,
                    proficiencyCraftyPoisonersKit, proficiencyCraftyPoisonersKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Nature,
                    proficiencyCraftyNature, proficiencyCraftyNatureExpertise))
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
            .SetFeatures(AttributeModifierCreed_Of_Pakri)
            .AddCustomSubFeatures(
                new FeatHelpers.ToolOrExpertise(ToolTypeDefinitions.ScrollKitType,
                    proficiencyCraftyScrollKit, proficiencyCraftyScrollKitExpertise),
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Arcana,
                    proficiencyCraftyArcana, proficiencyCraftyArcanaExpertise))
            .AddToDB();

        //
        // Arcanist
        //

        var featArcanist = FeatDefinitionBuilder
            .Create("FeatArcanist")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatArcanist")
                    .SetGuiPresentation(SpellDefinitions.DetectMagic.GuiPresentation)
                    .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create(SpellDefinitions.DetectMagic)
                            .Build())
                    .AddToDB())
            .AddCustomSubFeatures(
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Arcana,
                    proficiencyCraftyArcana, proficiencyCraftyArcanaExpertise))
            .AddToDB();

        //
        // Theologian
        //

        var featTheologian = FeatDefinitionBuilder
            .Create("FeatTheologian")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatTheologian")
                    .SetGuiPresentation(SpellDefinitions.DetectEvilAndGood.GuiPresentation)
                    .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create(SpellDefinitions.DetectEvilAndGood)
                            .Build())
                    .AddToDB())
            .AddCustomSubFeatures(
                new FeatHelpers.SkillOrExpertise(DatabaseHelper.SkillDefinitions.Religion,
                    proficiencyCraftyReligion, proficiencyCraftyReligionExpertise))
            .AddToDB();

        //
        // MAIN
        //

        feats.AddRange(
            featArcanist,
            featTheologian,
            featApothecaryInt,
            featApothecaryWis,
            featApothecaryCha,
            featToxicologistInt,
            featToxicologistWis,
            featToxicologistCha,
            featCraftyScriber);

        GroupFeats.FeatGroupSkills.AddFeats(
            featArcanist,
            featTheologian,
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
