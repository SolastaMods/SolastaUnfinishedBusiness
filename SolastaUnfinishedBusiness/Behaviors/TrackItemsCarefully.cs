using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Behaviors;

internal class TrackItemsCarefully
{
    internal static readonly TrackItemsCarefully Marker = new();

    private TrackItemsCarefully() { }

    internal static void Process(RulesetEffect activeEffect)
    {
        if (!activeEffect.SourceDefinition.HasSubFeatureOfType<TrackItemsCarefully>())
        {
            return;
        }

        var entityService = ServiceRepository.GetService<IRulesetEntityService>();
        var allEntities = entityService?.RulesetEntities.Values;

        ProcessSummonedItems(activeEffect, allEntities);
        ProcessItemProperties(activeEffect, allEntities);
    }

    internal static void FixDynamicPropertiesWithoutEffect()
    {
        var properties = ServiceRepository
            .GetService<IRulesetEntityService>()
            .RulesetEntities.Values.OfType<RulesetItemProperty>()
            .Where(IsOrphaned)
            .ToList();

        foreach (var property in properties)
        {
            var item = EffectHelpers.GetItemByGuid(property.TargetItemGuid);

            if (item == null)
            {
                continue;
            }

            RemoveItemProperty(property, item);
        }
    }

    private static bool IsOrphaned(RulesetItemProperty p)
    {
        return
            p.FeatureDefinition.HasSubFeatureOfType<TrackItemsCarefully>() &&
            p.SourceEffectGuid > 0 &&
            EffectHelpers.GetEffectByGuid(p.SourceEffectGuid) == null;
    }

    private static void ProcessSummonedItems(RulesetEffect activeEffect,
        [CanBeNull] Dictionary<ulong, RulesetEntity>.ValueCollection allEntities)
    {
        var characters = allEntities?
            .OfType<RulesetCharacter>()
            .ToList();

        var containers = allEntities?
            .OfType<RulesetContainer>()
            .ToList();

        var itemService = ServiceRepository.GetService<IGameLocationItemService>();

        if (activeEffect.TrackedSummonedItemGuids.Count == 0)
        {
            return;
        }

        foreach (var guid in activeEffect.TrackedSummonedItemGuids)
        {
            if (!RulesetEntity.TryGetEntity<RulesetItem>(guid, out var trackedItem))
            {
                continue;
            }

            if (characters != null)
            {
                foreach (var character in characters)
                {
                    character.LoseItem(trackedItem);
                }
            }

            if (containers != null)
            {
                foreach (var slot in containers.Select(container => container.FindSlotHoldingItem(trackedItem)))
                {
                    slot?.UnequipItem(true, true);
                }
            }

            itemService?.LootItem(trackedItem);
            trackedItem.ItemDestroyed -= activeEffect.ItemDestroyed;
            trackedItem.Unregister();
        }

        activeEffect.TrackedSummonedItemGuids.Clear();
    }

    private static void ProcessItemProperties(RulesetEffect activeEffect, [CanBeNull] IEnumerable<RulesetEntity> allEntities)
    {
        if (allEntities == null || activeEffect.TrackedItemPropertyGuids.Count == 0)
        {
            return;
        }

        var items = allEntities.OfType<RulesetItem>().ToList();

        foreach (var item in items)
        {
            var propertyGuids = activeEffect.trackedItemPropertyGuids;
            var properties = item.DynamicItemProperties
                .Where(p => propertyGuids.Contains(p.guid))
                .ToList();

            foreach (var itemProperty in properties)
            {
                RemoveItemProperty(itemProperty, item);
            }

            item.ItemPropertyRemoved -= activeEffect.ItemPropertyRemoved;
        }
    }

    private static void RemoveItemProperty(RulesetItemProperty itemProperty, RulesetItem item)
    {
        if (itemProperty.Guid > 0)
        {
            itemProperty.Unregister();
        }

        item.dynamicItemProperties.Remove(itemProperty);
    }
}
