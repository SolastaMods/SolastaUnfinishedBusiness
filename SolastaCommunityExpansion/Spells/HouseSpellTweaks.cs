using System.Linq;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class HouseSpellTweaks
    {
        public static void Register()
        {
            AddBleedingToRestoration();
            SpikeGrowthDoesNotAffectFlyingCreatures();
            SquareAreaOfEffectSpellsDoNotAffectFlyingCreatures();
        }

        internal static void SpikeGrowthDoesNotAffectFlyingCreatures()
        {
            var spikeGrowthEffect = SpikeGrowth.EffectDescription;
            spikeGrowthEffect.SetTargetParameter(4);

            if (Main.Settings.SpikeGrowthDoesNotAffectFlyingCreatures)
            {
                // Set to Cylinder radius 4, height 1
                spikeGrowthEffect.EffectForms
                    .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                    .ToList()
                    .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(false));

                spikeGrowthEffect.SetTargetType(RuleDefinitions.TargetType.Cylinder);
                spikeGrowthEffect.SetTargetParameter2(1);
            }
            else
            {
                // Restore default of Sphere radius 4
                spikeGrowthEffect.EffectForms
                    .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                    .ToList()
                    .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(true));

                spikeGrowthEffect.SetTargetType(RuleDefinitions.TargetType.Sphere);
                spikeGrowthEffect.SetTargetParameter2(0);
            }
        }

        internal static void SquareAreaOfEffectSpellsDoNotAffectFlyingCreatures()
        {
            // always applicable
            ClearTargetParameter2ForTargetTypeCube();

            // Spells with TargetType.Cube and defaults values of (tp, tp2)
            // Note that tp2 should be 0 for Cube and is ignored in game.
            // BlackTentacles: (4, 2)
            // Entangle: (4, 1)
            // FaerieFire: (4, 2)
            // FlamingSphere: (3, 2) <- a flaming sphere is a cube?
            // Grease: (2, 2)
            // HypnoticPattern: (6, 2)
            // PetalStorm: (3, 2)
            // Slow: (8, 2)

            if (!Main.Settings.SquareAreaOfEffectSpellsDoNotAffectFlyingCreatures)
            {
                RestoreDefinition(BlackTentacles);
                RestoreDefinition(Entangle);
                RestoreDefinition(Grease);
                return;
            }

            SetHeight(BlackTentacles);
            SetHeight(Entangle);
            SetHeight(Grease);

            static void SetHeight(SpellDefinition sd, int height = 1)
            {
                var effect = sd.EffectDescription;

                effect.EffectForms
                    .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                    .ToList()
                    .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(false));

                if (Main.Settings.EnableTargetTypeSquareCylinder)
                {
                    Main.Log($"Changing {sd.Name} to target type=Cube");
                    effect.SetTargetType(RuleDefinitions.TargetType.Cube);
                }
                else
                {
                    Main.Log($"Changing {sd.Name} to target type=CylinderWithDiameter");
                    effect.SetTargetType(RuleDefinitions.TargetType.CylinderWithDiameter);
                }

                effect.SetTargetParameter2(height);
            }

            static void RestoreDefinition(SpellDefinition sd)
            {
                var effect = sd.EffectDescription;

                // Topology forms have ImpactsFlyingCharacters = true as default
                effect.EffectForms
                    .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                    .ToList()
                    .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(true));

                Main.Log($"Restoring {sd.Name} to target type=Cube");
                effect.SetTargetType(RuleDefinitions.TargetType.Cube);
                effect.SetTargetParameter2(0);
            }

            static void ClearTargetParameter2ForTargetTypeCube()
            {
                foreach (var sd in DatabaseRepository
                    .GetDatabase<SpellDefinition>()
                    .Where(sd => sd.EffectDescription.TargetType == RuleDefinitions.TargetType.Cube))
                {
                    // TargetParameter2 is not used by TargetType.Cube but has random values assigned.
                    // We are going to use it to create a square cylinder with height so set to zero for all spells with TargetType.Cube.
                    sd.EffectDescription.SetTargetParameter2(0);
                }
            }
        }

        public static void AddBleedingToRestoration()
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
}
