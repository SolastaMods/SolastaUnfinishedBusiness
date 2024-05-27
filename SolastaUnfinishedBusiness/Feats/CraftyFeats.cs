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
        // skill

        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyArcana")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Arcana)
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

        var proficiencyCraftyReligion = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCraftyReligion")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Religion)
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
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyHerbalismKit, proficiencyCraftyMedicine)
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
            .SetFeatures(AttributeModifierCreed_Of_Maraike, proficiencyCraftyPoisonersKit, proficiencyCraftyNature)
            .SetFeatFamily("Toxicologist")
            .AddToDB();

        var featToxicologistCha = FeatDefinitionBuilder
            .Create("FeatToxicologistCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, proficiencyCraftyPoisonersKit, proficiencyCraftyNature)
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
        // Arcanist
        //

        var featArcanist = FeatDefinitionBuilder
            .Create("FeatArcanist")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                proficiencyCraftyArcana,
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatArcanist")
                    .SetGuiPresentation(SpellDefinitions.DetectMagic.GuiPresentation)
                    .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create(SpellDefinitions.DetectMagic)
                            .Build())
                    .AddToDB())
            .AddToDB();

        //
        // Theologian
        //

        var featTheologian = FeatDefinitionBuilder
            .Create("FeatTheologian")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                proficiencyCraftyReligion,
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatTheologian")
                    .SetGuiPresentation(SpellDefinitions.DetectEvilAndGood.GuiPresentation)
                    .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create(SpellDefinitions.DetectEvilAndGood)
                            .Build())
                    .AddToDB())
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
