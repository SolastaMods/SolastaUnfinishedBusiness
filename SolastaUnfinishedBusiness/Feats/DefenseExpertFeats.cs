using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class DefenseExpertFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        const string DefenseExpert = "DefenseExpert";

        // Arcane Defense
        var featArcaneDefense = FeatDefinitionBuilder
            .Create("FeatArcaneDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatArcaneDefenseAdd")
                    .SetGuiPresentationNoContent()
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmorOrShield)
                    .SetDexPlusAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Intelligence)
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        // Charismatic Defense
        var featCharismaticDefense = FeatDefinitionBuilder
            .Create("FeatCharismaticDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatCharismaticDefenseAdd")
                    .SetGuiPresentationNoContent()
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmorOrShield)
                    .SetDexPlusAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Charisma)
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        // Wise Defense
        var featWiseDefense = FeatDefinitionBuilder
            .Create("FeatWiseDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                AttributeModifierMonkUnarmoredDefense)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(featArcaneDefense, featCharismaticDefense, featWiseDefense);

        var featGroupDefenseExpert = GroupFeats.MakeGroup("FeatGroupDefenseExpert", DefenseExpert,
            featArcaneDefense,
            featCharismaticDefense,
            featWiseDefense);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featGroupDefenseExpert);

        GroupFeats.FeatGroupUnarmoredCombat.AddFeats(
            featGroupDefenseExpert);
    }
}
