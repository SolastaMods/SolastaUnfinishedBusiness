using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    private const string FeatSavageAttackerName = "FeatSavageAttacker";
    internal const string FeatShieldExpertName = "FeatShieldExpert";

    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        // Savage Attacker
        var featSavageAttacker = FeatDefinitionBuilder
            .Create(FeatSavageAttackerName)
            .SetFeatures(
                BuildFeatSavageAttackerDieRollModifier("DieRollModifierFeatSavageAttacker",
                    AttackDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */),
                BuildFeatSavageAttackerDieRollModifier("DieRollModifierFeatSavageMagicAttacker",
                    MagicDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */))
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatImprovedCritical")
                    .SetGuiPresentation("FeatImprovedCritical", Category.Feat)
                    .SetModifier(AttributeModifierOperation.Set, AttributeDefinitions.CriticalThreshold, 19)
                    .AddToDB())
            .AddToDB();

        // Tough
        var featTough = FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentation("FeatTough", Category.Feat)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Shield Expert
        var featShieldExpert = FeatDefinitionBuilder
            .Create(FeatShieldExpertName)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                    .Create("FeatShieldExpertBonusShieldAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new AddBonusShieldAttack())
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityFeatShieldExpertShove")
                    .SetGuiPresentationNoContent(true)
                    .SetDefaultAllowedActonTypes()
                    // Shove as bonus action seems too OP for this feat
                    // .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
                    .SetActionExecutionModifiers(
                        new ActionDefinitions.ActionExecutionModifier
                        {
                            actionId = ActionDefinitions.Id.Shove,
                            advantageType = RuleDefinitions.AdvantageType.Advantage,
                            equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                        },
                        new ActionDefinitions.ActionExecutionModifier
                        {
                            actionId = ActionDefinitions.Id.ShoveBonus,
                            advantageType = RuleDefinitions.AdvantageType.Advantage,
                            equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                        }
                    )
                    .AddToDB())
            .SetArmorProficiencyPrerequisite(DatabaseHelper.ArmorCategoryDefinitions.ShieldCategory)
            .AddToDB();

        feats.AddRange(featSavageAttacker, featTough, featImprovedCritical, featShieldExpert);
    }

    private static FeatureDefinitionDieRollModifier BuildFeatSavageAttackerDieRollModifier(
        string name, RuleDefinitions.RollContext context, int rerollCount, int minRerollValue)
    {
        return FeatureDefinitionDieRollModifierBuilder
            .Create(name)
            .SetModifiers(context, rerollCount, minRerollValue, minRerollValue, "Feat/&FeatSavageAttackerReroll")
            .SetGuiPresentation(FeatSavageAttackerName, Category.Feat)
            .AddToDB();
    }
}
