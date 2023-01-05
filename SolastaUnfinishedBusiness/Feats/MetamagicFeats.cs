using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class MetamagicFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var metaMagicFeats = BuildMetamagic();

        var group = GroupFeats.MakeGroup("FeatGroupMetamagic", "Metamagic", metaMagicFeats);

        group.mustCastSpellsPrerequisite = true;
        group.minimalAbilityScorePrerequisite = true;
        group.minimalAbilityScoreName = AttributeDefinitions.Charisma;
        group.minimalAbilityScoreValue = 13;

        feats.AddRange(metaMagicFeats);
    }

    private static List<FeatDefinition> BuildMetamagic()
    {
        // Metamagic
        var attributeModifierSorcererSorceryPointsBonus3 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus3")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                AttributeDefinitions.SorceryPoints)
            .AddToDB();

        var metaMagicFeats = new List<FeatDefinition>();
        var dbMetamagicOptionDefinition = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();

        metaMagicFeats.SetRange(dbMetamagicOptionDefinition
            .Select(metamagicOptionDefinition => FeatDefinitionWithPrerequisitesBuilder
                .Create($"FeatAdept{metamagicOptionDefinition.Name}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatAdeptMetamagicTitle", metamagicOptionDefinition.FormatTitle()),
                    Gui.Format("Feat/&FeatAdeptMetamagicDescription",
                        metamagicOptionDefinition.FormatTitle(),
                        metamagicOptionDefinition.FormatDescription()))
                .SetFeatures(
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus3,
                    FeatureDefinitionBuilder
                        .Create($"CustomCodeFeatAdept{metamagicOptionDefinition.Name}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new CustomCodeFeatMetamagicAdept(metamagicOptionDefinition))
                        .AddToDB())
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetValidators(ValidatorsFeat.ValidateNotMetamagic(metamagicOptionDefinition))
                .AddToDB()));

        return metaMagicFeats;
    }

    //
    // HELPERS
    //

    private sealed class CustomCodeFeatMetamagicAdept : IFeatureDefinitionCustomCode
    {
        public CustomCodeFeatMetamagicAdept(MetamagicOptionDefinition metamagicOption)
        {
            MetamagicOption = metamagicOption;
        }

        private MetamagicOptionDefinition MetamagicOption { get; }

        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            if (hero.MetamagicFeatures.ContainsKey(MetamagicOption))
            {
                return;
            }

            hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition> { MetamagicOption });
        }
    }
}
