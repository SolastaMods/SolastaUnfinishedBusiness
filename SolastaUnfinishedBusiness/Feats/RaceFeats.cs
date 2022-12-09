using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RaceFeats
{
    private const string ElvenPrecision = "ElvenPrecision";
    private const string FadeAway = "FadeAway";

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
        var powerFeatFadeAwayInvisible = FeatureDefinitionPowerBuilder
            .Create("PowerFeatFadeAwayInvisible")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Invisibility.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .Build())
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .AddToDB();

        // Fade Away (Dexterity)
        var featFadeAwayDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Fade Away (Intelligence)
        var featFadeAwayInt = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featFadeAwayDex,
            featFadeAwayInt,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma);

        var featGroupsElvenAccuracy = GroupFeats.MakeGroup("FeatGroupElvenAccuracy", ElvenPrecision,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        var featGroupsFadeAway = GroupFeats.MakeGroup("FeatGroupFadeAway", FadeAway,
            featFadeAwayDex,
            featFadeAwayInt);

        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featGroupsElvenAccuracy,
            featGroupsFadeAway);
    }

    internal sealed class ElvenPrecisionContext
    {
        internal bool Qualified { get; set; }
    }
}
