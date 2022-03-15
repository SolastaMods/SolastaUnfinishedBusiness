using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Feats
{
    internal static class OtherFeats
    {
        public static readonly Guid OtherFeatNamespace = new("655e8588-4d6e-42f3-9564-69e7345d5620");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Savage Attacker
            var savageAttacker = FeatDefinitionBuilder
                .Create("FeatSavageAttacker", OtherFeatNamespace)
                .SetFeatures(
                    BuildDieRollModifier("DieRollModifierFeatSavageAttacker",
                        AttackDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ),
                    BuildDieRollModifier("DieRollModifierFeatSavageMagicAttacker",
                        MagicDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ))
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Improved critical
            var improvedCritical = FeatDefinitionBuilder
                .Create("FeatImprovedCritical", OtherFeatNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierImprovedCriticalFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatImprovedCritical", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Set, AttributeDefinitions.CriticalThreshold, 19)
                        .AddToDB())
                .AddToDB();

            // Tough
            var tough = FeatDefinitionBuilder
                .Create("FeatTough", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierToughFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatTough", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // War Caster
            var warCaster = FeatDefinitionBuilder
                .Create("FeatWarCaster", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionMagicAffinityBuilder
                        .Create("MagicAffinityWarCasterFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatWarCaster", Category.Feat)
                        .SetCastingModifiers(2, 0, true, false, false)
                        .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                        .SetHandsFullCastingModifiers(true, true, true)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // Zappa Feats
            //

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

            // Shady
            var shady = FeatDefinitionBuilder
                .Create("FeatShady", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                     FeatureDefinitionAdditionalDamageBuilder
                        .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack", OtherFeatNamespace)
                        .SetGuiPresentation("AdditionalDamageFeatShadySneakAttack", Category.Feature)
                        .SetDiceByRank((1,1), (5,2))
                        .AddToDB(),
                     FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolFeatShadyExpertise")
                        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                        .AddToDB()

                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            feats.AddRange(savageAttacker, tough, warCaster, improvedCritical, fightingSurgeDexterity, fightingSurgeStrength, shady);
        }

        private static FeatureDefinitionDieRollModifier BuildDieRollModifier(string name,
            RuleDefinitions.RollContext context, int rerollCount, int minRerollValue)
        {
            return FeatureDefinitionDieRollModifierBuilder
                .Create(name, OtherFeatNamespace)
                .SetModifiers(context, rerollCount, minRerollValue, "Feat/&FeatSavageAttackerReroll")
                .SetGuiPresentation("FeatSavageAttacker", Category.Feat)
                .AddToDB();
        }
    }
}
