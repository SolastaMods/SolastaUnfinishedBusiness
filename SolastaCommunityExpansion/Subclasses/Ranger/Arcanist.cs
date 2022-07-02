using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Ranger;

internal sealed class Arcanist : AbstractSubclass
{
    private const string RangerArcanistRangerSubclassGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";

    private static readonly Guid RaBaseGuid = new(RangerArcanistRangerSubclassGuid);

    private static ConditionDefinition _markedByArcanist;
    private CharacterSubclassDefinition Subclass;

    private static ConditionDefinition MarkedByArcanist => _markedByArcanist ??= ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionMarkedByBrandingSmite, "ConditionMarkedByArcanist", RaBaseGuid)
        .SetGuiPresentation(Category.Condition,
            ConditionDefinitions.ConditionMarkedByBrandingSmite.GuiPresentation.SpriteReference)
        .SetAllowMultipleInstances(false)
        .SetDuration(RuleDefinitions.DurationType.Permanent, 1, false)
        .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
        .SetPossessive(true)
        .SetSpecialDuration(true)
        .AddToDB();

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass ??= BuildAndAddSubclass();
    }

    private static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        var rangerArcanistMagic = CreateRangerArcanistMagic();
        var arcanistMark = CreateArcanistMark();
        var arcaneDetonation = CreateArcaneDetonation();
        var arcaneDetonationUpgrade = CreateArcaneDetonationUpgrade();
        var (arcanePulseAction, arcanePulseUpgradeAction) = CreateArcanePulsePowers();

        return CharacterSubclassDefinitionBuilder
            .Create("RangerArcanistRangerSubclass", RangerArcanistRangerSubclassGuid)
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, rangerArcanistMagic)
            .AddFeatureAtLevel(arcanistMark, 3)
            .AddFeatureAtLevel(arcaneDetonation, 3)
            .AddFeatureAtLevel(arcanePulseAction, 7)
            .AddFeatureAtLevel(arcaneDetonationUpgrade, 11)
            .AddFeatureAtLevel(arcanePulseUpgradeAction, 15)
            .AddToDB();
    }

    [NotNull]
    private static FeatureDefinition[] CreateRangerArcanistMagic()
    {
        var preparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("ArcanistAutoPreparedSpells", RaBaseGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, Haste),
                BuildSpellGroup(13, DimensionDoor),
                BuildSpellGroup(17, HoldMonster))
            .AddToDB();

        var arcanistAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create(MagicAffinityBattleMagic, "MagicAffinityRangerArcanist", RaBaseGuid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        //Not actually used anywhere, but leaving this just in case some old character would need it
        FeatureDefinitionFeatureSetBuilder
            .Create("RangerArcanistMagic",
                GuidHelper.Create(RaBaseGuid, "RangerArcanistManaTouchedGuardian")
                    .ToString()) // Oops, will have to live with this name being off)
            .SetGuiPresentationNoContent(true)
            .SetFeatureSet(preparedSpells, arcanistAffinity)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();

        return new FeatureDefinition[] {preparedSpells, arcanistAffinity};
    }

    private static FeatureDefinitionAdditionalDamage CreateArcanistMark()
    {
        return FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistMark", RaBaseGuid)
            .SetGuiPresentation("ArcanistMark", Category.Feature)
            .SetSpecificDamageType("DamageForce")
            .SetDamageDice(RuleDefinitions.DieType.D6, 0)
            .SetNotificationTag("ArcanistMark")
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetNoSave()
            .SetNoAdvancement()
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = MarkedByArcanist,
                    Operation = ConditionOperationDescription.ConditionOperation.Add
                }
            )
            .AddToDB();
    }

    private static FeatureDefinitionAdditionalDamage CreateArcaneDetonation()
    {
        var assetReference = new AssetReference();
        assetReference.SetField("m_AssetGUID", "9f1fe10e6ef8c9c43b6b2ef91b2ad38a");

        return FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcaneDetonation", RaBaseGuid)
            .SetGuiPresentation("ArcaneDetonation", Category.Feature)
            .SetSpecificDamageType("DamageForce")
            .SetDamageDice(RuleDefinitions.DieType.D6, 1)
            .SetNotificationTag("ArcanistMark")
            .SetTargetCondition(MarkedByArcanist,
                RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
            .SetNoSave()
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = MarkedByArcanist,
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
    }

    private static FeatureDefinition CreateArcaneDetonationUpgrade()
    {
        // This is a blank feature. It does nothing except create a description for what happens at level 11.
        return FeatureDefinitionBuilder.Create("AdditionalDamageArcaneDetonationUpgrade", RaBaseGuid)
            .SetGuiPresentation("ArcaneDetonationUpgrade", Category.Feature)
            .AddToDB();
    }

    private static (FeatureDefinitionPower arcane_pulse_action, FeatureDefinitionPower arcane_pulse_upgrade_action)
        CreateArcanePulsePowers()
    {
        var markedEffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        markedEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        markedEffect.ConditionForm.ConditionDefinition = MarkedByArcanist;

        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = "DamageForce",
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 4,
                healFromInflictedDamage = RuleDefinitions.HealFromInflictedDamage.Never
            },
            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        var damageUpgradeEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = "DamageForce",
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 8,
                healFromInflictedDamage = RuleDefinitions.HealFromInflictedDamage.Never
            },
            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        var arcanePulseAction = CreateArcanePulse("ArcanePulse", "Feature/&ArcanePulseTitle",
            "Feature/&ArcanePulseDescription", markedEffect, damageEffect);

        var arcanePulseUpgradeAction = CreateArcanePulse("ArcanePulseUpgrade", "Feature/&ArcanePulseTitle",
            "Feature/&ArcanePulseDescription", markedEffect, damageUpgradeEffect);
        arcanePulseUpgradeAction.overriddenPower = arcanePulseAction;

        return (arcanePulseAction, arcanePulseUpgradeAction);
    }

    private static FeatureDefinitionPower CreateArcanePulse(string name, string title, string description,
        EffectForm markedEffect, EffectForm damageEffect)
    {
        var pulseDescription = new EffectDescription();
        pulseDescription.Copy(MagicMissile.EffectDescription);
        pulseDescription.SetCreatedByCharacter(true);
        pulseDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        pulseDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        pulseDescription.SetTargetParameter(3);
        pulseDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        pulseDescription.SetRangeParameter(30);
        pulseDescription.EffectForms.SetRange(damageEffect, markedEffect);

        return FeatureDefinitionPowerBuilder
            .Create(name, RaBaseGuid)
            .SetGuiPresentation(title, description,
                PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference)
            .SetUsesAbility(0, AttributeDefinitions.Wisdom)
            .SetShowCasting(true)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
            .SetEffectDescription(pulseDescription)
            .SetAbility(AttributeDefinitions.Wisdom)
            .SetShortTitle("Arcane Pulse")
            .AddToDB();
    }
}
