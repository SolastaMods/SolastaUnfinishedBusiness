using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerArcanist : AbstractSubclass
{
    private const string ArcanistMarkTag = "ArcanistMark";

    public RangerArcanist()
    {
        var conditionMarkedByArcanist = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionMarkedByBrandingSmite, "ConditionMarkedByArcanist")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Permanent)
            .SetPossessive()
            .AddToDB();

        //
        // LEVEL 03
        //

        var autoPreparedSpellsArcanist = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsArcanist")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, Haste),
                BuildSpellGroup(13, DimensionDoor),
                BuildSpellGroup(17, HoldMonster))
            .AddToDB();

        var magicAffinityRangerArcanist = FeatureDefinitionMagicAffinityBuilder
            .Create(MagicAffinityBattleMagic, "MagicAffinityRangerArcanist")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var additionalDamageArcanistMark = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageArcanistMark")
            .SetGuiPresentationNoContent(true)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D6, 0)
            .SetTargetCondition(conditionMarkedByArcanist, AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionMarkedByArcanist)
            .AddToDB();

        var additionalDamageArcanistArcaneDetonation = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistArcaneDetonation")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(ArcanistMarkTag)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 10)
            .SetTargetCondition(
                conditionMarkedByArcanist,
                AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Remove, conditionMarkedByArcanist)
            .SetImpactParticleReference(MagicMissile.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        //
        // LEVEL 07
        //

        var arcanistMarkedEffect =
            EffectFormBuilder
                .Create()
                .SetConditionForm(conditionMarkedByArcanist, ConditionForm.ConditionOperation.Add)
                .Build();

        var arcanistDamageEffect =
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeForce, 4, DieType.D8)
                .Build();

        var powerArcanistArcanePulse = CreatePowerArcanistArcanePulse(
            "PowerArcanistArcanePulse",
            null,
            arcanistMarkedEffect,
            arcanistDamageEffect);

        //
        // LEVEL 11
        //

        var additionalDamageArcanistArcaneDetonationUpgrade = FeatureDefinitionBuilder
            .Create("AdditionalDamageArcanistArcaneDetonationUpgrade")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        //
        // LEVEL 15
        //

        var arcanistDamageUpgradeEffect =
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeForce, 8, DieType.D8)
                .Build();

        var powerArcanistArcanePulseUpgrade = CreatePowerArcanistArcanePulse(
            "PowerArcanistArcanePulseUpgrade",
            powerArcanistArcanePulse,
            arcanistMarkedEffect,
            arcanistDamageUpgradeEffect
        );

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RangerArcanist")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("RangerArcanist", Resources.RangerArcanist, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsArcanist,
                magicAffinityRangerArcanist,
                additionalDamageArcanistMark,
                additionalDamageArcanistArcaneDetonation)
            .AddFeaturesAtLevel(7,
                powerArcanistArcanePulse)
            .AddFeaturesAtLevel(11,
                additionalDamageArcanistArcaneDetonationUpgrade)
            .AddFeaturesAtLevel(15,
                powerArcanistArcanePulseUpgrade)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower CreatePowerArcanistArcanePulse(
        string name,
        FeatureDefinitionPower overriddenPower = null,
        params EffectForm[] effectForms)
    {
        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation("PowerArcanistArcanePulse", Category.Feature,
                PowerDomainElementalHeraldOfTheElementsThunder)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MagicMissile.EffectDescription)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Sphere, 3)
                    .SetEffectForms(effectForms)
                    .Build())
            .SetOverriddenPower(overriddenPower)
            .AddToDB();
    }
}
