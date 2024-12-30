using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class PrecisionFocusedFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        const string PrecisionFocused = "PrecisionFocused";

        // Arcane Precision

        // kept name for backward compatibility
        var attackModifierArcanePrecision = FeatureDefinitionBuilder
            .Create("AttackModifierArcanePrecision")
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Intelligence),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var powerArcanePrecision = FeatureDefinitionPowerBuilder
            .Create("PowerArcanePrecision")
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierArcanePrecision, 0))
                            .Build())
                    .Build())
            .AddToDB();

        var featArcanePrecision = FeatDefinitionBuilder
            .Create("FeatArcanePrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerArcanePrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddCustomSubFeatures(FeatsContext.HideFromFeats.Marker)
            .AddToDB();

        // Charismatic Precision

        // kept name for backward compatibility
        var attackModifierCharismaticPrecision = FeatureDefinitionBuilder
            .Create("AttackModifierCharismaticPrecision")
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var powerCharismaticPrecision = FeatureDefinitionPowerBuilder
            .Create("PowerCharismaticPrecision")
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierCharismaticPrecision, 0))
                            .Build())
                    .Build())
            .AddToDB();

        var featCharismaticPrecision = FeatDefinitionBuilder
            .Create("FeatCharismaticPrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                powerCharismaticPrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddCustomSubFeatures(FeatsContext.HideFromFeats.Marker)
            .AddToDB();

        // Wise Precision

        // kept name for backward compatibility
        var attackModifierWisePrecision = FeatureDefinitionBuilder
            .Create("AttackModifierWisePrecision")
            .SetGuiPresentation("FeatWisePrecision", Category.Feat)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Wisdom),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var powerWisePrecision = FeatureDefinitionPowerBuilder
            .Create("PowerWisePrecision")
            .SetGuiPresentation("FeatWisePrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierWisePrecision, 0))
                            .Build())
                    .Build())
            .AddToDB();

        var featWisePrecision = FeatDefinitionBuilder
            .Create("FeatWisePrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                powerWisePrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddCustomSubFeatures(FeatsContext.HideFromFeats.Marker)
            .AddToDB();

        feats.AddRange(featArcanePrecision, featCharismaticPrecision, featWisePrecision);

        var featGroup = GroupFeats.MakeGroup("FeatGroupPrecisionFocused", PrecisionFocused,
            featArcanePrecision,
            featCharismaticPrecision,
            featWisePrecision);

        GroupFeats.FeatGroupSupportCombat.AddFeats(featGroup);
    }
}
