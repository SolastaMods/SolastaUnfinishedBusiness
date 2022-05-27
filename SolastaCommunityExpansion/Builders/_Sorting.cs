using static FeatureDefinitionAbilityCheckAffinity;
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaCommunityExpansion.Builders
{
    internal static class Sorting
    {
        internal static int Compare(BaseDefinition x, BaseDefinition y)
        {
            return x.Name.CompareTo(y.Name);
        }

        internal static int CompareTitle(BaseDefinition x, BaseDefinition y)
        {
            return x.FormatTitle().CompareTo(y.FormatTitle());
        }

        internal static int Compare(SavingThrowAffinityGroup x, SavingThrowAffinityGroup y)
        {
            var result = x.abilityScoreName.CompareTo(y.abilityScoreName);
            if (result == 0) { return x.affinity.CompareTo(y.affinity); }

            return result;
        }

        internal static int Compare(AbilityCheckAffinityGroup x, AbilityCheckAffinityGroup y)
        {
            var result = x.abilityScoreName.CompareTo(y.abilityScoreName);
            if (result == 0) { return x.proficiencyName.CompareTo(y.proficiencyName); }

            return result;
        }

        internal static int Compare(FeatureUnlockByLevel x, FeatureUnlockByLevel y)
        {
            var result = x.Level.CompareTo(y.Level);
            if (result == 0) { return x.FeatureDefinition.Name.CompareTo(y.FeatureDefinition.Name); }

            return result;
        }

        internal static int Compare(EffectForm x, EffectForm y)
        {
            return x.FormType.CompareTo(y.FormType); // then by?
        }

        internal static int Compare(MonsterSavingThrowProficiency x, MonsterSavingThrowProficiency y)
        {
            return x.AbilityScoreName.CompareTo(y.AbilityScoreName); // then by bonus?
        }

        internal static int Compare(MonsterSkillProficiency x, MonsterSkillProficiency y)
        {
            return x.SkillName.CompareTo(y.SkillName); // then by bonus?
        }

        internal static int Compare(MonsterAttackIteration x, MonsterAttackIteration y)
        {
            return x.MonsterAttackDefinition.Name.CompareTo(y.MonsterAttackDefinition.Name);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static int Compare(LegendaryActionDescription x, LegendaryActionDescription y)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return 0; // TODO:
        }
    }
}
