using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerArcanist : AbstractSubclass
{
    private const string ArcanistMarkTag = "ArcanistMark";

    internal RangerArcanist()
    {
        var conditionMarkedByArcanist = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionMarkedByBrandingSmite, "ConditionMarkedByArcanist")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetAllowMultipleInstances(false)
            .SetDuration(RuleDefinitions.DurationType.Permanent, 1, false)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetPossessive(true)
            .SetSpecialDuration(true)
            .AddToDB();

        //
        // LEVEL 03
        //

        var autoPreparedSpellsArcanist = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsArcanist")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Ranger)
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
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistMark")
            .SetGuiPresentation(Category.Feature)
            .SetSpecificDamageType(RuleDefinitions.DamageTypeForce)
            .SetDamageDice(RuleDefinitions.DieType.D6, 0)
            .SetNotificationTag(ArcanistMarkTag)
            .SetTargetCondition(conditionMarkedByArcanist,
                RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
            .SetNoSave()
            .SetNoAdvancement()
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = conditionMarkedByArcanist,
                    Operation = ConditionOperationDescription.ConditionOperation.Add
                }
            )
            .AddToDB();

        //
        // LEVEL 07
        //

        var assetReference = new AssetReference();

        assetReference.SetField("m_AssetGUID", "9f1fe10e6ef8c9c43b6b2ef91b2ad38a");

        var additionalDamageArcanistArcaneDetonation = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistArcaneDetonation")
            .SetGuiPresentation(Category.Feature)
            .SetSpecificDamageType(RuleDefinitions.DamageTypeForce)
            .SetDamageDice(RuleDefinitions.DieType.D6, 1)
            .SetNotificationTag(ArcanistMarkTag)
            .SetTargetCondition(conditionMarkedByArcanist,
                RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
            .SetNoSave()
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = conditionMarkedByArcanist,
                    Operation = ConditionOperationDescription.ConditionOperation.Remove
                }
            )
            .SetAdvancement(
                RuleDefinitions.AdditionalDamageAdvancement.ClassLevel,
                (1, 1),
                (2, 1),
                (3, 1),
                (4, 1),
                (5, 1),
                (6, 1),
                (7, 1),
                (8, 1),
                (9, 1),
                (10, 1),
                (11, 2),
                (12, 2),
                (13, 2),
                (14, 2),
                (15, 2),
                (16, 2),
                (17, 2),
                (18, 2),
                (19, 2),
                (20, 2))
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .SetImpactParticleReference(assetReference)
            .AddToDB();

        //
        // LEVEL 07
        //

        var arcanistMarkedEffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };

        arcanistMarkedEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        arcanistMarkedEffect.ConditionForm.ConditionDefinition = conditionMarkedByArcanist;

        var arcanistDamageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = RuleDefinitions.DamageTypeForce,
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 4,
                healFromInflictedDamage = RuleDefinitions.HealFromInflictedDamage.Never
            },
            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        var powerArcanistArcanePulse = CreatePowerArcanistArcanePulse(
            "PowerArcanistArcanePulse",
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

        var arcanistDamageUpgradeEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = RuleDefinitions.DamageTypeForce,
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 8,
                healFromInflictedDamage = RuleDefinitions.HealFromInflictedDamage.Never
            },
            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        var powerArcanistArcanePulseUpgrade = CreatePowerArcanistArcanePulse(
            "PowerArcanistArcanePulseUpgrade",
            arcanistMarkedEffect,
            arcanistDamageUpgradeEffect,
            powerArcanistArcanePulse);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RangerArcanist")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
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

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    private static FeatureDefinitionPower CreatePowerArcanistArcanePulse(
        string name,
        EffectForm markedEffect,
        EffectForm damageEffect,
        FeatureDefinitionPower overriddenPower = null)
    {
        var pulseDescription = new EffectDescription()
            .Create(MagicMissile.EffectDescription)
            .SetCreatedByCharacter(true)
            .SetTargetSide(RuleDefinitions.Side.Enemy)
            .SetTargetType(RuleDefinitions.TargetType.Sphere)
            .SetTargetParameter(3)
            .SetRangeType(RuleDefinitions.RangeType.Distance)
            .SetRangeParameter(30)
            .SetEffectForms(damageEffect, markedEffect);

        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation("PowerArcanistArcanePulse", Category.Feature,
                PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference)
            .SetUsesAbility(0, AttributeDefinitions.Wisdom)
            .SetShowCasting(true)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetEffectDescription(pulseDescription)
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetOverriddenPower(overriddenPower)
            .AddToDB();
    }
}
