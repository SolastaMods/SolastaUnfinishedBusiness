using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public static class InvocationPoolTypes
{
    public static readonly CustomInvocationPoolType Infusion =
        CustomInvocationPoolType.Register("Infusion", DatabaseHelper.SpellDefinitions.Fly);
}

public class CustomInvocationPoolType
{
    private static readonly List<CustomInvocationPoolType> pools = new();

    public string Name { get; private set; }

    /**Are level requirements in character levels or class levels?*/
    public bool RequireClassLevels { get; private set; }

    public AssetReferenceSprite Sprite { get; private set; }

    [NotNull] public List<int> AllLevels { get; } = new();
    public List<CustomInvocationDefinition> AllFeatures { get; } = new();

    private readonly Dictionary<int, List<CustomInvocationDefinition>> featuresByLevel = new();

    private CustomInvocationPoolType()
    {
    }

    public static CustomInvocationPoolType Register(string name, BaseDefinition sprite, bool requireClassLevel = true)
    {
        return Register(name, sprite.GuiPresentation.SpriteReference, requireClassLevel);
    }

    public static CustomInvocationPoolType Register(string name, AssetReferenceSprite sprite,
        bool requireClassLevel = true)
    {
        var pool = new CustomInvocationPoolType()
        {
            Name = name, Sprite = sprite, RequireClassLevels = requireClassLevel
        };
        pools.Add(pool);
        return pool;
    }

    public static void RefreshAll()
    {
        var invocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .GetAllElements()
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
        AllFeatures.SetRange(invocations.Where(d => d.PoolType == Name));

        featuresByLevel.Clear();
        AllFeatures.ForEach(f => GetOrMakeLevelFeatures(f.requiredLevel).Add(f));
        AllLevels.SetRange(featuresByLevel.Select(e => e.Key));
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
}
