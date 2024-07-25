using System;
using static FeatureDefinitionAbilityCheckAffinity;
using static FeatureDefinitionSavingThrowAffinity;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Builders;

public enum Category
{
    Action = 1,
    Background,
    Class,
    Condition,
    ContentPack,
    Feat,
    Feature,
    FightingStyle,
    Invocation,
    Item,
    Monster,
    MonsterFamily,
    Proxy,
    Race,
    Reaction,
    RestActivity,
    Spell,
    Subclass,
    Tooltip,
    Tutorial,
    UI,
    Rules
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

        if (!a.FeatureDefinition || !b.FeatureDefinition)
        {
            return 0;
        }

        return CompareTitle(a.FeatureDefinition, b.FeatureDefinition);
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

    internal static int Compare(MonsterSkillProficiency x, MonsterSkillProficiency y)
    {
        return String.Compare(x.SkillName, y.SkillName, StringComparison.CurrentCultureIgnoreCase); // then by bonus?
    }
}
