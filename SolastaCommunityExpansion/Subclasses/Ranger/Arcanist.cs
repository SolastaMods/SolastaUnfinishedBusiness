using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Ranger;

internal class Arcanist : AbstractSubclass
{
    private const string RangerArcanistRangerSubclassGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";

    public static readonly Guid RA_BASE_GUID = new(RangerArcanistRangerSubclassGuid);

    private static ConditionDefinition markedByArcanist;
    private CharacterSubclassDefinition Subclass;

    private static ConditionDefinition MarkedByArcanist => markedByArcanist ??= ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionMarkedByBrandingSmite, "ConditionMarkedByArcanist", RA_BASE_GUID)
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

    public static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        var ranger_arcanist_magic = CreateRangerArcanistMagic();
        var arcanist_mark = CreateArcanistMark();
        var arcane_detonation = CreateArcaneDetonation();
        var arcane_detonation_upgrade = CreateArcaneDetonationUpgrade();
        var (arcane_pulse_action, arcane_pulse_upgrade_action) = CreateArcanePulsePowers();

        return CharacterSubclassDefinitionBuilder
            .Create("RangerArcanistRangerSubclass", RangerArcanistRangerSubclassGuid)
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, ranger_arcanist_magic)
            .AddFeatureAtLevel(arcanist_mark, 3)
            .AddFeatureAtLevel(arcane_detonation, 3)
            .AddFeatureAtLevel(arcane_pulse_action, 7)
            .AddFeatureAtLevel(arcane_detonation_upgrade, 11)
            .AddFeatureAtLevel(arcane_pulse_upgrade_action, 15)
            .AddToDB();
    }

    private static FeatureDefinition[] CreateRangerArcanistMagic()
    {
        var preparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("ArcanistAutoPreparedSpells", RA_BASE_GUID)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, Haste),
                BuildSpellGroup(13, DimensionDoor),
                BuildSpellGroup(17, HoldMonster))
            .AddToDB();

        var arcanist_affinity = FeatureDefinitionMagicAffinityBuilder
            .Create(MagicAffinityBattleMagic, "MagicAffinityRangerArcanist", RA_BASE_GUID)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        //Not actually used anywhere, but leaving this just in case some old character would need it
        FeatureDefinitionFeatureSetBuilder
            .Create("RangerArcanistMagic",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistManaTouchedGuardian")
                    .ToString()) // Oops, will have to live with this name being off)
            .SetGuiPresentationNoContent(true)
            .SetFeatureSet(preparedSpells, arcanist_affinity)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();

        return new FeatureDefinition[] {preparedSpells, arcanist_affinity};
    }

    private static FeatureDefinitionAdditionalDamage CreateArcanistMark()
    {
        return FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistMark", RA_BASE_GUID)
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
        var asset_reference = new AssetReference();
        asset_reference.SetField("m_AssetGUID", "9f1fe10e6ef8c9c43b6b2ef91b2ad38a");

        return FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcaneDetonation", RA_BASE_GUID)
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
            .SetImpactParticleReference(asset_reference)
            .AddToDB();
    }

    private static FeatureDefinition CreateArcaneDetonationUpgrade()
    {
        // This is a blank feature. It does nothing except create a description for what happens at level 11.
        return FeatureDefinitionBuilder.Create("AdditionalDamageArcaneDetonationUpgrade", RA_BASE_GUID)
            .SetGuiPresentation("ArcaneDetonationUpgrade", Category.Feature)
            .AddToDB();
    }

    private static (FeatureDefinitionPower arcane_pulse_action, FeatureDefinitionPower arcane_pulse_upgrade_action)
        CreateArcanePulsePowers()
    {
        var marked_effect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        marked_effect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        marked_effect.ConditionForm.ConditionDefinition = MarkedByArcanist;

        var damage_effect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = "DamageForce", DieType = RuleDefinitions.DieType.D8, DiceNumber = 4
            }
        };
        damage_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
        damage_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

        var damage_upgrade_effect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DamageType = "DamageForce", DieType = RuleDefinitions.DieType.D8, DiceNumber = 8
            }
        };
        damage_upgrade_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
        damage_upgrade_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

        var arcane_pulse_action = CreateArcanePulse("ArcanePulse", "Feature/&ArcanePulseTitle",
            "Feature/&ArcanePulseDescription", marked_effect, damage_effect);

        var arcane_pulse_upgrade_action = CreateArcanePulse("ArcanePulseUpgrade", "Feature/&ArcanePulseTitle",
            "Feature/&ArcanePulseDescription", marked_effect, damage_upgrade_effect);
        arcane_pulse_upgrade_action.SetOverriddenPower(arcane_pulse_action);

        return (arcane_pulse_action, arcane_pulse_upgrade_action);
    }

    private static FeatureDefinitionPower CreateArcanePulse(string name, string title, string description,
        EffectForm marked_effect, EffectForm damage_effect)
    {
        var pulse_description = new EffectDescription();
        pulse_description.Copy(MagicMissile.EffectDescription);
        pulse_description.SetCreatedByCharacter(true);
        pulse_description.SetTargetSide(RuleDefinitions.Side.Enemy);
        pulse_description.SetTargetType(RuleDefinitions.TargetType.Sphere);
        pulse_description.SetTargetParameter(3);
        pulse_description.SetRangeType(RuleDefinitions.RangeType.Distance);
        pulse_description.SetRangeParameter(30);
        pulse_description.EffectForms.SetRange(damage_effect, marked_effect);

        return FeatureDefinitionPowerBuilder
            .Create(name, RA_BASE_GUID)
            .SetGuiPresentation(title, description,
                PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference)
            .SetUsesAbility(0, AttributeDefinitions.Wisdom)
            .SetShowCasting(true)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
            .SetEffectDescription(pulse_description)
            .SetAbility(AttributeDefinitions.Wisdom)
            .SetShortTitle("Arcane Pulse")
            .AddToDB();
    }
}
