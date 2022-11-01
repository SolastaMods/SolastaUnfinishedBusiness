using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes.Inventor;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class InvocationPoolTypeCustom
{
    private static readonly List<InvocationPoolTypeCustom> PrivatePools = new();

    private readonly Dictionary<int, List<InvocationDefinitionCustom>> PrivateFeaturesByLevel = new();

    private InvocationPoolTypeCustom()
    {
    }

    internal string Name { get; private set; }

    /**Are level requirements in character levels or class levels?*/
    internal string RequireClassLevels { get; private set; }

    internal AssetReferenceSprite Sprite { get; private set; }

    internal List<int> AllLevels { get; } = new();
    private List<InvocationDefinitionCustom> AllFeatures { get; } = new();

    internal string PanelTitle => $"Screen/&InvocationPool{Name}Header";

    private static InvocationPoolTypeCustom Register(string name, BaseDefinition sprite,
        string requireClassLevel = null)
    {
        return Register(name, sprite.GuiPresentation.SpriteReference, requireClassLevel);
    }

    private static InvocationPoolTypeCustom Register(string name, AssetReferenceSprite sprite = null,
        string requireClassLevel = null)
    {
        var pool = new InvocationPoolTypeCustom
        {
            Name = name, Sprite = sprite, RequireClassLevels = requireClassLevel
        };
        PrivatePools.Add(pool);
        return pool;
    }

    internal static void RefreshAll()
    {
        var invocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .OfType<InvocationDefinitionCustom>()
            .ToList();

        foreach (var pool in PrivatePools)
        {
            pool.Refresh(invocations);
        }
    }

    private string GuiName(bool unlearn)
    {
        return $"InvocationPool{Name}{(unlearn ? "Unlearn" : "Learn")}";
    }

    internal string FormatDescription(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateDescriptionKey(GuiName(unlearn), Category.Feature));
    }

    internal string FormatTitle(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateTitleKey(GuiName(unlearn), Category.Feature));
    }

    internal List<InvocationDefinitionCustom> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return (PrivateFeaturesByLevel.TryGetValue(level, out var result) ? result : null)
               ?? new List<InvocationDefinitionCustom>();
    }

    private void Refresh(IEnumerable<InvocationDefinitionCustom> invocations)
    {
        PrivateFeaturesByLevel.Clear();
        AllFeatures.SetRange(invocations.Where(d => d.PoolType == this));
        AllFeatures.ForEach(f => GetOrMakeLevelFeatures(f.requiredLevel).Add(f));
        AllLevels.SetRange(PrivateFeaturesByLevel.Select(e => e.Key));
        AllLevels.Sort();
    }

    private List<InvocationDefinitionCustom> GetOrMakeLevelFeatures(int level)
    {
        List<InvocationDefinitionCustom> levelFeatures;
        if (!PrivateFeaturesByLevel.ContainsKey(level))
        {
            levelFeatures = new List<InvocationDefinitionCustom>();
            PrivateFeaturesByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = PrivateFeaturesByLevel[level];
        }

        return levelFeatures;
    }

    internal static class Pools
    {
        internal static readonly InvocationPoolTypeCustom Infusion =
            Register("Infusion", DatabaseHelper.SpellDefinitions.Fly, InventorClass.ClassName);

        internal static readonly InvocationPoolTypeCustom Alchemy =
            Register("Alchemy", DatabaseHelper.ItemDefinitions.AlchemistFire, InventorClass.ClassName);

        internal static List<InvocationPoolTypeCustom> All => PrivatePools;
    }
}
