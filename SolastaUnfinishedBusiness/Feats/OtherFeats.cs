using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

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
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB(),
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageMagicAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(MagicDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierMartialChampionImprovedCritical)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(Category.Feat)
            .SetKnownFeatsPrerequisite(featImprovedCritical.Name)
            .SetFeatures(AttributeModifierMartialChampionSuperiorCritical)
            .AddToDB();

        // Tough
        var featTough = FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Shield Expert
        var featShieldExpert = FeatDefinitionBuilder
            .Create(FeatShieldExpertName)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("AddExtraAttackFeatShieldExpert")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new AddBonusShieldAttack())
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityFeatShieldExpertShove")
                    .SetGuiPresentationNoContent(true)
                    .SetDefaultAllowedActionTypes()
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

        GroupFeats.MakeGroup("FeatGroupCriticalVirtuoso", null,
            featImprovedCritical,
            featSuperiorCritical);

        feats.AddRange(featSavageAttacker, featTough, featImprovedCritical, featSuperiorCritical, featShieldExpert);
    }
}
