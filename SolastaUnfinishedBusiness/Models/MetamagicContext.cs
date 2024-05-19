using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Subclasses.Builders.MetamagicBuilders;

namespace SolastaUnfinishedBusiness.Models;

internal static class MetamagicContext
{
    internal static HashSet<MetamagicOptionDefinition> Metamagic { get; private set; } = [];

    internal static void LateLoad()
    {
        LoadMetamagic(BuildMetamagicAltruisticSpell());
        LoadMetamagic(BuildMetamagicFocusedSpell());
        LoadMetamagic(BuildMetamagicPowerfulSpell());
        LoadMetamagic(BuildMetamagicSeekingSpell());
        LoadMetamagic(BuildMetamagicTransmutedSpell());
        LoadMetamagic(BuildMetamagicWidenedSpell());

        // sorting
        Metamagic = Metamagic.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.MetamagicEnabled
                     .Where(name => Metamagic.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.MetamagicEnabled.Remove(name);
        }
    }

    private static void LoadMetamagic([NotNull] MetamagicOptionDefinition metamagicDefinition)
    {
        Metamagic.Add(metamagicDefinition);
        UpdateMetamagicVisibility(metamagicDefinition);
    }

    private static void UpdateMetamagicVisibility([NotNull] BaseDefinition metamagicDefinition)
    {
        metamagicDefinition.GuiPresentation.hidden =
            !Main.Settings.MetamagicEnabled.Contains(metamagicDefinition.Name);
    }

    internal static void SwitchMetamagic(MetamagicOptionDefinition metamagicDefinition, bool active)
    {
        if (!Metamagic.Contains(metamagicDefinition))
        {
            return;
        }

        var name = metamagicDefinition.Name;

        if (active)
        {
            Main.Settings.MetamagicEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.MetamagicEnabled.Remove(name);
        }

        UpdateMetamagicVisibility(metamagicDefinition);
    }

    internal static int CompareMetamagic(MetamagicOptionDefinition a, MetamagicOptionDefinition b)
    {
        var compare = Math.Max(a.SorceryPointsCost, 1) - Math.Max(b.SorceryPointsCost, 1);

        return compare == 0
            ? string.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase)
            : compare;
    }
}
