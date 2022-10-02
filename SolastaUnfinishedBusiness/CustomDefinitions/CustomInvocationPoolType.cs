using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes.Inventor;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public class CustomInvocationPoolType
{
    private static readonly List<CustomInvocationPoolType> pools = new();

    private readonly Dictionary<int, List<CustomInvocationDefinition>> featuresByLevel = new();

    private CustomInvocationPoolType()
    {
    }

    public string Name { get; private set; }

    /**Are level requirements in character levels or class levels?*/
    public string RequireClassLevels { get; private set; }

    public AssetReferenceSprite Sprite { get; private set; }

    [NotNull] public List<int> AllLevels { get; } = new();
    public List<CustomInvocationDefinition> AllFeatures { get; } = new();

    public string PanelTitle => $"Screen/&InvocationPool{Name}Header";

    public static CustomInvocationPoolType Register(string name, BaseDefinition sprite, string requireClassLevel = null)
    {
        return Register(name, sprite.GuiPresentation.SpriteReference, requireClassLevel);
    }

    public static CustomInvocationPoolType Register(string name, AssetReferenceSprite sprite = null,
        string requireClassLevel = null)
    {
        var pool = new CustomInvocationPoolType
        {
            Name = name, Sprite = sprite, RequireClassLevels = requireClassLevel
        };
        pools.Add(pool);
        return pool;
    }

    public static void RefreshAll()
    {
        var invocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .OfType<CustomInvocationDefinition>()
            .ToList();

        foreach (var pool in pools)
        {
            pool.Refresh(invocations);
        }
    }

    private string GuiName(bool unlearn)
    {
        return $"InvocationPool{Name}{(unlearn ? "Unlearn" : "Learn")}";
    }

    public string FormatDescription(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateDescriptionKey(GuiName(unlearn), Category.Feature));
    }

    public string FormatTitle(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateTitleKey(GuiName(unlearn), Category.Feature));
    }

    [NotNull]
    public List<CustomInvocationDefinition> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return (featuresByLevel.TryGetValue(level, out var result) ? result : null)
               ?? new List<CustomInvocationDefinition>();
    }

    private void Refresh(List<CustomInvocationDefinition> invocations)
    {
        AllFeatures.SetRange(invocations.Where(d => d.PoolType == this));

        featuresByLevel.Clear();
        AllFeatures.ForEach(f => GetOrMakeLevelFeatures(f.requiredLevel).Add(f));
        AllLevels.SetRange(featuresByLevel.Select(e => e.Key));
        AllLevels.Sort();
    }

    private List<CustomInvocationDefinition> GetOrMakeLevelFeatures(int level)
    {
        List<CustomInvocationDefinition> levelFeatures;
        if (!featuresByLevel.ContainsKey(level))
        {
            levelFeatures = new List<CustomInvocationDefinition>();
            featuresByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = featuresByLevel[level];
        }

        return levelFeatures;
    }

    public static class Pools
    {
        public static readonly CustomInvocationPoolType Infusion =
            Register("Infusion", DatabaseHelper.SpellDefinitions.Fly, InventorClass.ClassName);

        public static List<CustomInvocationPoolType> All => pools;
    }
}
