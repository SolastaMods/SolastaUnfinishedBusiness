using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class HouseSpellTweaks
    {
        public static void Register()
        {
            AddBleedingToLesserConjuration();
        }

        public static void AddBleedingToLesserConjuration()
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
        }
    }
}
