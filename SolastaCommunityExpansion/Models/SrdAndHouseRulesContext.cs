using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models;

internal static class SrdAndHouseRulesContext
{
    internal static void Load()
    {
        AllowTargetingSelectionWhenCastingChainLightningSpell();
        ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        ApplyAcNonStackingRules();
        ApplySrdWeightToFoodRations();
    }

    internal static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
    {
        // Use the shocked condition affinity which has the desired effect
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            if (!ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionBlinded.Features.Add(ActionAffinityConditionShocked);
            }
        }
        else
        {
            if (ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionBlinded.Features.Remove(ActionAffinityConditionShocked);
            }
        }
    }

    /// <summary>
    ///     Allow the user to select targets when using 'Chain Lightning'.
    /// </summary>
    internal static void AllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        var spell = ChainLightning.EffectDescription;

        if (Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell)
        {
            // This is half bug-fix, half houses rules since it's not completely SRD but better than implemented.
            // Spell should arc from target (range 150ft) onto upto 3 extra selectable targets (range 30ft from first).
            // Fix by allowing 4 selectable targets.
            spell.TargetType = RuleDefinitions.TargetType.IndividualsUnique;
            spell.SetTargetParameter(4);
            spell.effectAdvancement.additionalTargetsPerIncrement = 1;

            // TODO: may need to tweak range parameters but it works as is.
        }
        else
        {
            spell.TargetType = RuleDefinitions.TargetType.ArcFromIndividual;
            spell.SetTargetParameter(3);
            spell.effectAdvancement.additionalTargetsPerIncrement = 0;
        }
    }

    private static void ApplyAcNonStackingRules()
    {
        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierBarbarianUnarmoredDefense
            .SetCustomSubFeatures(ExclusiveArmorClassBonus.Marker);
        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierSorcererDraconicResilienceAC
            .SetCustomSubFeatures(ExclusiveArmorClassBonus.Marker);
    }

    internal static void ApplySrdWeightToFoodRations()
    {
        var foodSrdWeight = DatabaseHelper.ItemDefinitions.Food_Ration;
        var foodForagedSrdWeight = DatabaseHelper.ItemDefinitions.Food_Ration_Foraged;

        if (Main.Settings.ApplySrdWeightToFoodRations)
        {
            foodSrdWeight.weight = 2.0f;
            foodForagedSrdWeight.weight = 2.0f;
        }
        else
        {
            foodSrdWeight.weight = 3.0f;
            foodForagedSrdWeight.weight = 3.0f;
        }
    }

    //
    //
    //

    internal static void RemoveConcentrationRequirementsFromAnySpell()
    {
        if (!Main.Settings.RemoveConcentrationRequirementsFromAnySpell)
        {
            return;
        }

        foreach (var spell in DatabaseRepository.GetDatabase<SpellDefinition>())
        {
            spell.requiresConcentration = false;
        }
    }

    internal static void RemoveHumanoidFilterOnHideousLaughter()
    {
        if (!Main.Settings.RemoveHumanoidFilterOnHideousLaughter)
        {
            return;
        }

        // Remove Humanoid only filter on Hideous Laughter (as per SRD, any creature can be targeted)
        HideousLaughter.effectDescription.restrictedCreatureFamilies.Clear();
    }

    internal static void RemoveRecurringEffectOnEntangle()
    {
        if (!Main.Settings.RemoveRecurringEffectOnEntangle)
        {
            return;
        }

        // Remove recurring effect on Entangle (as per SRD, any creature is only affected at cast time)
        Entangle.effectDescription.recurrentEffect = RuleDefinitions.RecurrentEffect.OnActivation;
    }


    internal static void MinorFixes()
    {
        // Shows Concentration tag in UI
        BladeBarrier.requiresConcentration = true;

        //
        // BUGFIX: spells durations
        //

        // Use our logic to calculate duration for DominatePerson/Beast/Monster
        DominateBeast.EffectDescription.EffectAdvancement.alteredDuration =
            (RuleDefinitions.AdvancementDuration)ExtraAdvancementDuration.DominateBeast;
        DominatePerson.EffectDescription.EffectAdvancement.alteredDuration =
            (RuleDefinitions.AdvancementDuration)ExtraAdvancementDuration.DominatePerson;

        // Stops upcasting assigning non-SRD durations
        ClearAlteredDuration(ProtectionFromEnergy);
        ClearAlteredDuration(ProtectionFromEnergyAcid);
        ClearAlteredDuration(ProtectionFromEnergyCold);
        ClearAlteredDuration(ProtectionFromEnergyFire);
        ClearAlteredDuration(ProtectionFromEnergyLightning);
        ClearAlteredDuration(ProtectionFromEnergyThunder);
        ClearAlteredDuration(ProtectionFromPoison);

        static void ClearAlteredDuration([NotNull] IMagicEffect spell)
        {
            spell.EffectDescription.EffectAdvancement.alteredDuration = RuleDefinitions.AdvancementDuration.None;
        }
    }

    internal static void UseCubeOnSleetStorm()
    {
        var sleetStormEffect = SleetStorm.EffectDescription;

        if (Main.Settings.ChangeSleetStormToCube)
        {
            // Set to Cube side 8, default height
            sleetStormEffect.targetType = RuleDefinitions.TargetType.Cube;
            sleetStormEffect.targetParameter = 8;
            sleetStormEffect.targetParameter2 = 0;
        }
        else
        {
            // Restore to cylinder radius 4, height 3
            sleetStormEffect.targetType = RuleDefinitions.TargetType.Cylinder;
            sleetStormEffect.targetParameter = 4;
            sleetStormEffect.targetParameter2 = 3;
        }
    }

    internal static void UseHeightOneCylinderEffect()
    {
        // always applicable
        ClearTargetParameter2ForTargetTypeCube();

        ///////////////////////////////////////////////////////////
        // Change SpikeGrowth to be height 1 round cylinder/sphere
        var spikeGrowthEffect = SpikeGrowth.EffectDescription;
        spikeGrowthEffect.targetParameter = 4;

        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Set to Cylinder radius 4, height 1
            spikeGrowthEffect.targetType = RuleDefinitions.TargetType.Cylinder;
            spikeGrowthEffect.targetParameter2 = 1;
        }
        else
        {
            // Restore default of Sphere radius 4
            spikeGrowthEffect.targetType = RuleDefinitions.TargetType.Sphere;
            spikeGrowthEffect.targetParameter2 = 0;
        }

        // Spells with TargetType.Cube and defaults values of (tp, tp2)
        // Note that tp2 should be 0 for Cube and is ignored in game.
        // BlackTentacles: (4, 2)
        // Entangle: (4, 1)
        // FaerieFire: (4, 2)
        // FlamingSphere: (3, 2) <- a flaming sphere is a cube?
        // Grease: (2, 2)
        // HypnoticPattern: (6, 2)
        // Slow: (8, 2)
        // Thunderwave: (3, 2)


        ///////////////////////////////////////////////////////////
        // Change Black Tentacles, Entangle, Grease to be height 1 square cylinder/cube
        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Setting height switches to square cylinder (if originally cube)
            SetHeight(BlackTentacles, 1);
            SetHeight(Entangle, 1);
            SetHeight(Grease, 1);
        }
        else
        {
            // Setting height to 0 restores original behaviour
            SetHeight(BlackTentacles, 0);
            SetHeight(Entangle, 0);
            SetHeight(Grease, 0);
        }

        static void SetHeight([NotNull] IMagicEffect spellDefinition, int height)
        {
            spellDefinition.EffectDescription.targetParameter2 = height;
        }

        static void ClearTargetParameter2ForTargetTypeCube()
        {
            foreach (var sd in DatabaseRepository
                         .GetDatabase<SpellDefinition>()
                         .Where(sd =>
                             sd.EffectDescription.TargetType is RuleDefinitions.TargetType.Cube
                                 or RuleDefinitions.TargetType.CubeWithOffset))
            {
                // TargetParameter2 is not used by TargetType.Cube but has random values assigned.
                // We are going to use it to create a square cylinder with height so set to zero for all spells with TargetType.Cube.
                sd.EffectDescription.SetTargetParameter2(0);
            }
        }
    }

    internal static void AddBleedingToRestoration()
    {
        var cf = LesserRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cf != null)
        {
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cf.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cf.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }
        else
        {
            Main.Error("Unable to find form of type Condition in LesserRestoration");
        }

        var cfg = GreaterRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cfg != null)
        {
            // NOTE: using the same setting as for Lesser Restoration for compatibility
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cfg.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cfg.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }
        else
        {
            Main.Error("Unable to find form of type Condition in GreaterRestoration");
        }
    }
}
