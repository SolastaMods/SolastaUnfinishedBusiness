using System.Linq;
using SolastaCommunityExpansion.Builders;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class HouseSpellTweaks
    {
        public static void Register()
        {
            AddBleedingToRestoration();
            AddFrightenedCharmedImmunityToCalmEmotions();
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

        public static void AddFrightenedCharmedImmunityToCalmEmotions()
        {
            var effectForms = CalmEmotionsOnAlly.EffectDescription.EffectForms;

            var effectForm = effectForms
                .Where(ef => ef.FormType == EffectForm.EffectFormType.Condition)
                .SingleOrDefault(ef => ef.ConditionForm.ConditionDefinition.Name == "CECalmEmotionsImmunityEffectForm");

            if (Main.Settings.AddImmunitiesToCalmEmotions)
            {
                if (effectForm == null)
                {
                    effectForms.Add(CECalmEmotionsImmunityEffectForm);
                }
            }
            else if (effectForm != null)
            {
                effectForms.Remove(effectForm);
            }
        }

        private static readonly EffectForm CECalmEmotionsImmunityEffectForm = BuildCECalmEmotionsImmunityEffectForm();

        private static EffectForm BuildCECalmEmotionsImmunityEffectForm()
        {
            // TODO: create new condition
            return EffectFormBuilder
                .Create()
                .SetConditionForm(ConditionProtectedFromEvil, ConditionForm.ConditionOperation.Add, false, false)
                .Build();
        }
    }
}
