using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class MetamagicFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var metaMagicFeats = BuildMetamagic();

        GroupFeats.MakeGroup("FeatGroupMetamagic", null, metaMagicFeats);

        feats.AddRange(metaMagicFeats);
    }

    private static List<FeatDefinition> BuildMetamagic()
    {
        // Metamagic
        var attributeModifierSorcererSorceryPointsBonus2 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.SorceryPoints, 3)
            .AddToDB();

        var metaMagicFeats = new List<FeatDefinition>();
        var dbMetamagicOptionDefinition = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();

        metaMagicFeats.SetRange(dbMetamagicOptionDefinition
            .Select(metamagicOptionDefinition => FeatDefinitionBuilder
                .Create($"FeatAdept{metamagicOptionDefinition.Name}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatAdeptMetamagicTitle", metamagicOptionDefinition.FormatTitle()),
                    Gui.Format("Feat/&FeatAdeptMetamagicDescription", metamagicOptionDefinition.FormatTitle()))
                .SetFeatures(
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2,
                    FeatureDefinitionBuilder
                        .Create($"CustomCodeFeatAdept{metamagicOptionDefinition.Name}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new CustomCodeFeatMetamagicAdept(metamagicOptionDefinition))
                        .AddToDB())
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
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
