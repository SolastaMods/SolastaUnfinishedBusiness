using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;

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

            // Shield Expert
            var shieldExpert = FeatDefinitionBuilder
                .Create("FeatShieldExpert", OtherFeatNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(FeatureDefinitionBuilder
                        .Create("ShieldExpertAttack", OtherFeatNamespace)
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new AddBonusShieldAttack())
                        .AddToDB(),
                    FeatureDefinitionActionAffinityBuilder
                        .Create("ShieldExpertShove", OtherFeatNamespace)
                        .SetGuiPresentationNoContent(true)
                        .SetDefaultAllowedActonTypes()
                        // Shove as bonus action seems too OP for this feat
                        // .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
                        .SetActionExecutionModifiers(
                            new ActionDefinitions.ActionExecutionModifier()
                            {
                                actionId = ActionDefinitions.Id.Shove,
                                advantageType = RuleDefinitions.AdvantageType.Advantage,
                                equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                            },
                            new ActionDefinitions.ActionExecutionModifier()
                            {
                                actionId = ActionDefinitions.Id.ShoveBonus,
                                advantageType = RuleDefinitions.AdvantageType.Advantage,
                                equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                            }
                        )
                        .AddToDB())
                .SetArmorProficiencyPrerequisite(DatabaseHelper.ArmorCategoryDefinitions.ShieldCategory)
                .AddToDB();

            // War Caster
            var warCaster = FeatDefinitionBuilder
                .Create("FeatWarCaster", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionMagicAffinityBuilder
                        .Create("MagicAffinityWarCasterFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatWarCaster", Category.Feat)
                        .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.FlatValue, 0, RuleDefinitions.SpellParamsModifierType.None, false, false, false)
                        .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                        .SetHandsFullCastingModifiers(true, true, true)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .SetMustCastSpellsPrerequisite()
                .AddToDB();

            feats.AddRange(savageAttacker, tough, warCaster, improvedCritical, shieldExpert);
            ReactionRequestWarcaster.Initialize();
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