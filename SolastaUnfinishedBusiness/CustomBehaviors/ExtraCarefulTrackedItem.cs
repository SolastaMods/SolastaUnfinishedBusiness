using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal class ExtraCarefulTrackedItem
{
    internal static readonly ExtraCarefulTrackedItem Marker = new();

    private ExtraCarefulTrackedItem() { }

    internal static void Process(RulesetEffect activeEffect)
    {
        if (!activeEffect.SourceDefinition.HasSubFeatureOfType<ExtraCarefulTrackedItem>())
        {
            return;
        }

        ProcessSummonedItems(activeEffect);
        ProcessItemProperties(activeEffect);
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

            //TODO: do we need to log this?
            RemoveItemProperty(property, item);
        }
    }

    private static bool IsOrphaned(RulesetItemProperty p)
    {
        return p.FeatureDefinition.HasSubFeatureOfType<ExtraCarefulTrackedItem>()
               && p.SourceEffectGuid > 0
               && EffectHelpers.GetEffectByGuid(p.SourceEffectGuid) == null;
    }

    private static void ProcessSummonedItems(RulesetEffect activeEffect)
    {
        var allEntities = ServiceRepository
            .GetService<IRulesetEntityService>()
            .RulesetEntities.Values;

        var characters = allEntities
            .Select(e => e as RulesetCharacter)
            .Where(e => e != null)
            .ToList();

        var containers = allEntities
            .Select(e => e as RulesetContainer)
            .Where(e => e != null)
            .ToList();

        var itemService = ServiceRepository.GetService<IGameLocationItemService>();

        if (activeEffect.TrackedSummonedItemGuids.Empty())
        {
            return;
        }

        foreach (var guid in activeEffect.TrackedSummonedItemGuids)
        {
            if (!RulesetEntity.TryGetEntity<RulesetItem>(guid, out var trackedItem))
            {
                continue;
            }

            foreach (var character in characters)
            {
                character.LoseItem(trackedItem);
            }

            foreach (var slot in containers.Select(container => container.FindSlotHoldingItem(trackedItem)))
            {
                slot?.UnequipItem(true, true);
            }

            //TODO: check if it works with merchants

            itemService?.LootItem(trackedItem);
            trackedItem.ItemDestroyed -= activeEffect.ItemDestroyed;
            trackedItem.Unregister();
        }

        activeEffect.TrackedSummonedItemGuids.Clear();
    }

    private static void ProcessItemProperties(RulesetEffect activeEffect)
    {
        if (activeEffect.TrackedItemPropertyGuids.Empty())
        {
            return;
        }

        var items = ServiceRepository
            .GetService<IRulesetEntityService>()
            .RulesetEntities.Values.OfType<RulesetItem>().ToList();

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
