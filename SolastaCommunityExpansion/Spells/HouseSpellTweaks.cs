using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Patches.Bugfix;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Spells;

internal static class HouseSpellTweaks
{
    internal static void Register()
    {
        AddBleedingToRestoration();
        UseCubeOnSleetStorm();
        UseHeightOneCylinderEffect();
        MinorFixes();
        RemoveConcentrationRequirementsFromAnySpell();
        RemoveHumanoidFilterOnHideousLaughter();
        RemoveRecurringEffectOnEntangle();
    }

    private static void RemoveConcentrationRequirementsFromAnySpell()
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

    private static void RemoveHumanoidFilterOnHideousLaughter()
    {
        if (!Main.Settings.RemoveHumanoidFilterOnHideousLaughter)
        {
            return;
        }

        // Remove Humanoid only filter on Hideous Laughter (as per SRD, any creature can be targeted)
        HideousLaughter.effectDescription.restrictedCreatureFamilies.Clear();
    }

    private static void RemoveRecurringEffectOnEntangle()
    {
        if (!Main.Settings.RemoveRecurringEffectOnEntangle)
        {
            return;
        }

        // Remove recurring effect on Entangle (as per SRD, any creature is only affected at cast time)
        Entangle.effectDescription.recurrentEffect = RuleDefinitions.RecurrentEffect.OnActivation;
    }


    private static void MinorFixes()
    {
        // Shows Concentration tag in UI
        BladeBarrier.requiresConcentration = true;

        //
        // BUGFIX: spells durations
        //

        // Use our logic to calculate duration for DominatePerson/Beast/Monster
        DominateBeast.EffectDescription.EffectAdvancement.alteredDuration =
            (RuleDefinitions.AdvancementDuration)AdvancementDurationEx.DominateBeast;
        DominatePerson.EffectDescription.EffectAdvancement.alteredDuration =
            (RuleDefinitions.AdvancementDuration)AdvancementDurationEx.DominatePerson;

        // Stops upcasting assigning non-SRD durations
        ClearAlteredDuration(ProtectionFromEnergy);
        ClearAlteredDuration(ProtectionFromEnergyAcid);
        ClearAlteredDuration(ProtectionFromEnergyCold);
        ClearAlteredDuration(ProtectionFromEnergyFire);
        ClearAlteredDuration(ProtectionFromEnergyLightning);
        ClearAlteredDuration(ProtectionFromEnergyThunder);
        ClearAlteredDuration(ProtectionFromPoison);

        static void ClearAlteredDuration(SpellDefinition spell)
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

        static void SetHeight(SpellDefinition sd, int height)
        {
            sd.EffectDescription.targetParameter2 = height;
        }

        static void ClearTargetParameter2ForTargetTypeCube()
        {
            foreach (var sd in DatabaseRepository
                         .GetDatabase<SpellDefinition>()
                         .Where(sd =>
                             sd.EffectDescription.TargetType == RuleDefinitions.TargetType.Cube
                             || sd.EffectDescription.TargetType == RuleDefinitions.TargetType.CubeWithOffset))
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
