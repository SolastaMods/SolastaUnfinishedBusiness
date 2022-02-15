using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class HouseSpellTweaks
    {
        public static void Register()
        {
            AddBleedingToRestoration();
            BugFixCalmEmotionsOnAlly();
            CertainsSpellDoNotAffectFlyingCreatures();
        }

        private static void CertainsSpellDoNotAffectFlyingCreatures()
        {
            if (!Main.Settings.CertainSpellsDoNotAffectFlyingCreatures)
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

            var entangleEffect = Entangle.EffectDescription;
            entangleEffect.EffectForms
                .Where(ef => ef.FormType == EffectForm.EffectFormType.Topology)
                .ToList()
                .ForEach(ef => ef.TopologyForm.SetImpactsFlyingCharacters(false));

            //entangleEffect.SetTargetType(RuleDefinitions.TargetType.CubeWithOffset);
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
        public static void BugFixCalmEmotionsOnAlly()
        {
            if (!Main.Settings.BugFixCalmEmotionsOnAlly)
            {
                return;
            }

            var invalidForm = CalmEmotionsOnAlly.EffectDescription.EffectForms
                .Where(ef => ef.FormType == EffectForm.EffectFormType.Condition)
                .Where(ef => ef.ConditionForm.Operation == ConditionForm.ConditionOperation.Add)
                .Where(ef => ef.ConditionForm.ConditionsList.Contains(ConditionCharmed))
                .SingleOrDefault();

            if (invalidForm != null)
            {
                Main.Log("BugFixCalmEmotionsOnAlly: Fixing invalid form.");

                invalidForm.ConditionForm.ConditionsList.Clear();

                if (ConditionCalmedByCalmEmotionsAlly.ConditionType == RuleDefinitions.ConditionType.Detrimental)
                {
                    ConditionCalmedByCalmEmotionsAlly.SetConditionType(RuleDefinitions.ConditionType.Beneficial);

                    // Note: Features is null and needs to be set with SetField
                    ConditionCalmedByCalmEmotionsAlly.SetField("features",
                        new List<FeatureDefinition> {
                            ConditionAffinityFrightenedImmunity,
                            ConditionAffinityCharmImmunity
                        });
                }
            }
        }
    }
}
