using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class PrecisionFocusedFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        const string PrecisionFocused = "PrecisionFocused";

        // Arcane Precision
        var attackModifierArcanePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierArcanePrecision")
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat, AttackModifierMagicWeapon)
            .AddCustomSubFeatures(new CanUseAttribute(AttributeDefinitions.Intelligence))
            .SetMagicalWeapon()
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
            .AddToDB();

        // Charismatic Precision
        var attackModifierCharismaticPrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierCharismaticPrecision")
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, AttackModifierMagicWeapon)
            .AddCustomSubFeatures(new CanUseAttribute(AttributeDefinitions.Charisma))
            .SetMagicalWeapon()
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
            .AddToDB();

        // Wise Precision
        var attackModifierWisePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWisePrecision")
            .SetGuiPresentation("FeatWisePrecision", Category.Feat, AttackModifierMagicWeapon)
            .AddCustomSubFeatures(new CanUseAttribute(AttributeDefinitions.Wisdom))
            .SetMagicalWeapon()
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
            .AddToDB();

        feats.AddRange(featArcanePrecision, featCharismaticPrecision, featWisePrecision);

        GroupFeats.MakeGroup("FeatGroupPrecisionFocused", PrecisionFocused,
            featArcanePrecision,
            featCharismaticPrecision,
            featWisePrecision);
    }
}
