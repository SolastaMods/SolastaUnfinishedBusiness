using System;
using static FeatureDefinitionAbilityCheckAffinity;
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaUnfinishedBusiness.Builders;

internal static class Sorting
{
    internal static int Compare(BaseDefinition x, BaseDefinition y)
    {
        return String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
    }

    internal static int CompareTitle(BaseDefinition x, BaseDefinition y)
    {
        return String.Compare(x.FormatTitle(), y.FormatTitle(), StringComparison.CurrentCultureIgnoreCase);
    }

    internal static int Compare(SavingThrowAffinityGroup x, SavingThrowAffinityGroup y)
    {
        var result = String.Compare(x.abilityScoreName, y.abilityScoreName, StringComparison.CurrentCultureIgnoreCase);

        return result == 0 ? x.affinity.CompareTo(y.affinity) : result;
    }

    internal static int Compare(AbilityCheckAffinityGroup x, AbilityCheckAffinityGroup y)
    {
        var result = String.Compare(x.abilityScoreName, y.abilityScoreName, StringComparison.CurrentCultureIgnoreCase);

        return result == 0
            ? String.Compare(x.proficiencyName, y.proficiencyName, StringComparison.CurrentCultureIgnoreCase)
            : result;
    }

    internal static int Compare(FeatureUnlockByLevel x, FeatureUnlockByLevel y)
    {
        var result = x.Level.CompareTo(y.Level);

        return result == 0
            ? String.Compare(x.FeatureDefinition.Name, y.FeatureDefinition.Name,
                StringComparison.CurrentCultureIgnoreCase)
            : result;
    }

    internal static int Compare(EffectForm x, EffectForm y)
    {
        return x.FormType.CompareTo(y.FormType); // then by?
    }

    internal static int Compare(MonsterSavingThrowProficiency x, MonsterSavingThrowProficiency y)
    {
        return String.Compare(x.AbilityScoreName, y.AbilityScoreName,
            StringComparison.CurrentCultureIgnoreCase); // then by bonus?
    }

    internal static int Compare(MonsterSkillProficiency x, MonsterSkillProficiency y)
    {
        return String.Compare(x.SkillName, y.SkillName, StringComparison.CurrentCultureIgnoreCase); // then by bonus?
    }

    internal static int Compare(MonsterAttackIteration x, MonsterAttackIteration y)
    {
        return String.Compare(x.MonsterAttackDefinition.Name, y.MonsterAttackDefinition.Name,
            StringComparison.CurrentCultureIgnoreCase);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    internal static int Compare(LegendaryActionDescription x, LegendaryActionDescription y)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return 0; // TODO:
    }
}
