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
        }

        private static void SpikeGrowthDoesNotAffectFlyingCreatures()
        {
            if (!Main.Settings.SpikeGrowthDoesNotAffectFlyingCreatures)
            {
                return;
            }

            var spikeGrowthEffect = SpikeGrowth.EffectDescription;
            spikeGrowthEffect.EffectForms
                .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                .ToList()
                .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(false));

            spikeGrowthEffect.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            spikeGrowthEffect.SetTargetParameter2(1);

            // Entangle is more difficult because it's a cube, and what we need is a Square Cylinder height 1.
            var entangleEffect = Entangle.EffectDescription;
            entangleEffect.EffectForms
                .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                .ToList()
                .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(false));
            entangleEffect.SetTargetParameter2(1);
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
