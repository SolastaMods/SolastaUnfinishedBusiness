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
    internal const string FeatShieldExpertName = "FeatShieldExpert";

    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        // Savage Attacker
        var savageAttacker = FeatDefinitionBuilder
            .Create("FeatSavageAttacker")
            .SetFeatures(
                BuildDieRollModifier("DieRollModifierFeatSavageAttacker",
                    AttackDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */),
                BuildDieRollModifier("DieRollModifierFeatSavageMagicAttacker",
                    MagicDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */))
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Improved Critical
        var improvedCritical = FeatDefinitionBuilder
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
        var tough = FeatDefinitionBuilder
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
        var shieldExpert = FeatDefinitionBuilder
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

        feats.AddRange(savageAttacker, tough, improvedCritical, shieldExpert);
    }

    private static FeatureDefinitionDieRollModifier BuildDieRollModifier(string name,
        RuleDefinitions.RollContext context, int rerollCount, int minRerollValue)
    {
        return FeatureDefinitionDieRollModifierBuilder
            .Create(name)
            .SetModifiers(context, rerollCount, minRerollValue, "Feat/&FeatSavageAttackerReroll")
            .SetGuiPresentation("FeatSavageAttacker", Category.Feat)
            .AddToDB();
    }
}
