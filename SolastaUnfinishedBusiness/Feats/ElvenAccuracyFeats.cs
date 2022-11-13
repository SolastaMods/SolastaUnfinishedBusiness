using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ElvenAccuracyFeats
{
    internal const string ElvenAccuracyTag = "ElvenAccuracy";

    internal static void CheckElvenPrecisionContext(
        bool result,
        RulesetCharacter character,
        RulesetAttackMode attackMode)
    {
        if (!result || character is not RulesetCharacterHero hero || attackMode == null)
        {
            return;
        }

        foreach (var feat in hero.TrainedFeats
                     .Where(x => x.Name.Contains(ElvenAccuracyTag)))
        {
            var elvenPrecisionContext = feat.GetFirstSubFeatureOfType<ElvenPrecisionContext>();

            if (elvenPrecisionContext != null)
            {
                elvenPrecisionContext.Qualified =
                    attackMode.abilityScore is not AttributeDefinitions.Strength or AttributeDefinitions.Constitution;
            }
        }
    }

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        const string ElvenPrecision = "ElvenPrecision";

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma);

        GroupFeats.MakeGroup("FeatGroupElvenAccuracy", ElvenPrecision,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);
    }
}

internal sealed class ElvenPrecisionContext
{
    internal bool Qualified { get; set; }
}
