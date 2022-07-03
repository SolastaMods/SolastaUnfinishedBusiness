using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models;

public class GlobalUniqueEffects
{
    public enum Group { Familiar, Tinkerer }

    private static readonly Dictionary<Group, (List<FeatureDefinitionPower>, List<SpellDefinition>)>
        _groups = new();

    private static (List<FeatureDefinitionPower>, List<SpellDefinition>) GetGroup(Group group)
    {
        if (_groups.ContainsKey(group))
        {
            return _groups[group];
        }

        var newGroup = new ValueTuple<List<FeatureDefinitionPower>, List<SpellDefinition>>
        {
            Item1 = new List<FeatureDefinitionPower>(), Item2 = new List<SpellDefinition>()
        };

        _groups.Add(group, newGroup);

        return newGroup;
    }

    /**Returns copies*/
    public static (HashSet<FeatureDefinitionPower>, HashSet<SpellDefinition>) GetSameGroupItems(
        FeatureDefinitionPower power)
    {
        var powers = new HashSet<FeatureDefinitionPower>();
        var spells = new HashSet<SpellDefinition>();

        foreach (var group in _groups.Where(e => e.Value.Item1.Contains(power)))
        {
            foreach (var p in group.Value.Item1)
            {
                powers.Add(p);
            }

            foreach (var s in group.Value.Item2)
            {
                spells.Add(s);
            }
        }

        return (powers, spells);
    }

    public static (HashSet<FeatureDefinitionPower>, HashSet<SpellDefinition>) GetSameGroupItems(
        SpellDefinition spell)
    {
        var powers = new HashSet<FeatureDefinitionPower>();
        var spells = new HashSet<SpellDefinition>();
        foreach (var group in _groups.Where(e => e.Value.Item2.Contains(spell)))
        {
            foreach (var p in group.Value.Item1)
            {
                powers.Add(p);
            }

            foreach (var s in group.Value.Item2)
            {
                spells.Add(s);
            }
        }

        return (powers, spells);
    }

    public static void AddToGroup(Group group, params FeatureDefinitionPower[] powers)
    {
        GetGroup(group).Item1.AddRange(powers);
    }

    public static void AddToGroup(Group group, params SpellDefinition[] spells)
    {
        GetGroup(group).Item2.AddRange(spells);
    }
}
