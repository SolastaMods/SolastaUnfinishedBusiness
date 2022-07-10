using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class GlobalUniqueEffects
{
    public enum Group { Familiar, Tinkerer }

    private static readonly Dictionary<Group, (List<FeatureDefinitionPower>, List<SpellDefinition>)>
        Groups = new();

    private static (List<FeatureDefinitionPower>, List<SpellDefinition>) GetGroup(Group group)
    {
        if (Groups.ContainsKey(group))
        {
            return Groups[group];
        }

        var newGroup = new ValueTuple<List<FeatureDefinitionPower>, List<SpellDefinition>>
        {
            Item1 = new List<FeatureDefinitionPower>(), Item2 = new List<SpellDefinition>()
        };

        Groups.Add(group, newGroup);

        return newGroup;
    }

    /**Returns copies*/
    public static (HashSet<FeatureDefinitionPower>, HashSet<SpellDefinition>) GetSameGroupItems(
        FeatureDefinitionPower power)
    {
        var powers = new HashSet<FeatureDefinitionPower>();
        var spells = new HashSet<SpellDefinition>();

        foreach (var group in Groups.Where(e => e.Value.Item1.Contains(power)))
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
        foreach (var group in Groups.Where(e => e.Value.Item2.Contains(spell)))
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

    public static void AddToGroup(Group group, [NotNull] params FeatureDefinitionPower[] powers)
    {
        GetGroup(group).Item1.AddRange(powers);
    }

    public static void AddToGroup(Group group, [NotNull] params SpellDefinition[] spells)
    {
        GetGroup(group).Item2.AddRange(spells);
    }
}
