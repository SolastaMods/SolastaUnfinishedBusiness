using System;
using static FeatureDefinitionAbilityCheckAffinity;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Builders;

internal enum Category
{
    None = 0,
    Action,
    Background,
    Class,
    Condition,
    ContentPack,
    Feat,
    Feature,
    FightingStyle,
    Item,
    Monster,
    Race,
    Reaction,
    RestActivity,
    Spell,
    Subclass
}

internal static class Sorting
{
    internal static int CompareFeatureUnlock(FeatureUnlockByLevel a, FeatureUnlockByLevel b)
    {
        var result = a.Level - b.Level;

        if (result != 0)
        {
            return result;
        }

        // hack as TA code requires this FEATURE to be last on list (DRAGONBORN only)
        if (a.FeatureDefinition == FeatureSetDragonbornBreathWeapon)
        {
            return 1;
        }

        if (b.FeatureDefinition == FeatureSetDragonbornBreathWeapon)
        {
            return -1;
        }

        // CharacterBuildingManager.BrowseGrantedFeaturesHierarchically expects CastSpell to be last
        if (a.FeatureDefinition is FeatureDefinitionCastSpell)
        {
            return 1;
        }

        if (b.FeatureDefinition is FeatureDefinitionCastSpell)
        {
            return -1;
        }

        return String.Compare(a.FeatureDefinition.FormatTitle(), b.FeatureDefinition.FormatTitle(),
            StringComparison.CurrentCulture);
    }

    internal static int CompareTitle(BaseDefinition x, BaseDefinition y)
    {
        return String.Compare(x.FormatTitle(), y.FormatTitle(), StringComparison.CurrentCultureIgnoreCase);
    }

    internal static int Compare(BaseDefinition x, BaseDefinition y)
    {
        return String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
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

    internal static int Compare(MonsterSkillProficiency x, MonsterSkillProficiency y)
    {
        return String.Compare(x.SkillName, y.SkillName, StringComparison.CurrentCultureIgnoreCase); // then by bonus?
    }

#if false
    internal static int Compare(EffectForm x, EffectForm y)
    {
        return x.FormType.CompareTo(y.FormType); // then by?
    }

    internal static int Compare(MonsterSavingThrowProficiency x, MonsterSavingThrowProficiency y)
    {
        return String.Compare(x.AbilityScoreName, y.AbilityScoreName,
            StringComparison.CurrentCultureIgnoreCase); // then by bonus?
    }
    
    internal static int Compare(MonsterAttackIteration x, MonsterAttackIteration y)
    {
        return String.Compare(x.MonsterAttackDefinition.Name, y.MonsterAttackDefinition.Name,
            StringComparison.CurrentCultureIgnoreCase);
    }
#endif
}
