using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishDuelist : AbstractSubclass
{
    private const string Name = "RoguishDuelist";

    internal RoguishDuelist()
    {
        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, $"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("SneakAttack")
            .SetDamageDice(RuleDefinitions.DieType.D6, 1)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .AddToDB();

        var additionalDamageSureFooted = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}SureFooted")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("SureFooted")
            .SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetCustomSubFeatures(ValidatorsCharacter.HasFreeOffHand)
            .AddToDB();

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SureFooted")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext((RuleDefinitions.SituationalContext)
                ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
            .AddToDB();

        var featureSetSureFooted = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SureFooted")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle,
                additionalDamageSureFooted,
                attributeModifierSureFooted)
            .AddToDB();

        var actionAffinitySwiftReprisal = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwiftReprisal")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.SwirlingDance)
            .AddToDB();

        var actionAffinityGracefulTakeDown = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}GracefulTakeDown")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, RangerSwiftBlade)
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, featureSetSureFooted)
            .AddFeaturesAtLevel(9, actionAffinitySwiftReprisal)
            .AddFeaturesAtLevel(13, actionAffinityGracefulTakeDown)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
}
