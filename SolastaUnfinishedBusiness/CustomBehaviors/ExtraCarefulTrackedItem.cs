using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ExtraCarefulTrackedItem
{
    public static readonly ExtraCarefulTrackedItem Marker = new();
    private ExtraCarefulTrackedItem() { }

    public static void Process(RulesetImplementationManager rules, RulesetEffect activeEffect)
    {
        if (!activeEffect.SourceDefinition.HasSubFeatureOfType<ExtraCarefulTrackedItem>())
        {
            return;
        }

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

        if (activeEffect.TrackedSummonedItemGuids.Count <= 0)
            return;
        rules.summonedItemGuidsToProcess.AddRange(activeEffect.TrackedSummonedItemGuids);
        foreach (ulong guid in rules.summonedItemGuidsToProcess)
        {
            if (RulesetEntity.TryGetEntity<RulesetItem>(guid, out var entity))
                entity.ItemDestroyed -= activeEffect.ItemDestroyed;
        }

        foreach (ulong guid in rules.summonedItemGuidsToProcess)
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

            trackedItem.Unregister();
        }

        activeEffect.TrackedSummonedItemGuids.Clear();
        rules.summonedItemGuidsToProcess.Clear();
    }
}
