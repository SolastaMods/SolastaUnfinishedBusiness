using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Feats
{
    internal static class ZappaFeats
    {
        public static readonly Guid OtherFeatNamespace = new("655e8588-4d6e-42f3-9564-69e7345d5620");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Fighting Surge (Dexterity)
            var fightingSurgeDexterity = FeatDefinitionBuilder
                .Create("FeatFightingSurgeDexterity", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Fighting Surge (Strength)
            var fightingSurgeStrength = FeatDefinitionBuilder
                .Create("FeatFightingSurgeStrength", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal Defense (Constitution)
            var primalDefenseConstitution = FeatDefinitionBuilder
                .Create("FeatPrimalDefenseConstitution", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    AttributeModifierBarbarianUnarmoredDefense  
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal Defense (Strength)
            var primalDefenseStrength = FeatDefinitionBuilder
                .Create("FeatPrimalDefenseStrength", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    AttributeModifierBarbarianUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal Rage (Constitution)
            var primalRageConstitution = FeatDefinitionBuilder
                .Create("FeatPrimalRageConstitution", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal Rage (Strength)
            var primalRageStrength = FeatDefinitionBuilder
                .Create("FeatPrimalRageStrength", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Shady
            var shady = FeatDefinitionBuilder
                .Create("FeatShady", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack", OtherFeatNamespace)
                        .SetGuiPresentation("AdditionalDamageFeatShadySneakAttack", Category.Feature)
                        .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                        .SetDiceByRank(
                            (1, 1),
                            (2, 1),
                            (3, 1),
                            (4, 1),
                            (5, 1),
                            (6, 1),
                            (7, 2),
                            (8, 2),
                            (9, 2),
                            (10, 2),
                            (11, 2),
                            (12, 2),
                            (13, 3),
                            (14, 3),
                            (15, 3),
                            (16, 3),
                            (17, 3),
                            (18, 3),
                            (19, 4),
                            (20, 4)
                        )
                        .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.OncePerTurn)
                        .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
                        .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // set feats to be registered in mod settings
            //

            feats.AddRange(
                fightingSurgeDexterity,
                fightingSurgeStrength,
                primalRageConstitution,
                primalRageStrength,
                primalDefenseConstitution,
                primalDefenseStrength,
                shady);
        }
    }
}
