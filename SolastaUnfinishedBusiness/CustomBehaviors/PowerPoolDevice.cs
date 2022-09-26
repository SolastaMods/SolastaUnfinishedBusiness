using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

//TODO: add validation to check if this should apply at all
/**
 * Adds fake usable device to hero's devices that has its charges set to uses of provided power pool.
 */
public class PowerPoolDevice
{
    private static readonly Dictionary<RulesetCharacterHero, Dictionary<string, RulesetItemDevice>> DeviceCache = new();

    private readonly ItemDefinition baseItem;

    public PowerPoolDevice(ItemDefinition baseItem, FeatureDefinitionPower pool)
    {
        this.baseItem = baseItem;
        Pool = pool;
    }

    public FeatureDefinitionPower Pool { get; }

    public static void Clear(RulesetCharacterHero hero)
    {
        if (!DeviceCache.TryGetValue(hero, out var devices))
        {
            return;
        }

        DeviceCache.Remove(hero);
        foreach (var device in devices.Values)
        {
            device.Unregister();
        }
    }

    public static PowerPoolDevice GetFromRulesetItem(RulesetCharacterHero hero, RulesetItemDevice device)
    {
        return hero.GetSubFeaturesByType<PowerPoolDevice>()
            .FirstOrDefault(p => p.baseItem.Name == device.ItemDefinition.Name);
    }

    public RulesetItemDevice GetDevice(RulesetCharacterHero hero)
    {
        if (!DeviceCache.TryGetValue(hero, out var devices))
        {
            devices = new Dictionary<string, RulesetItemDevice>();
            DeviceCache.Add(hero, devices);
        }

        if (!devices.TryGetValue(baseItem.Name, out var device))
        {
            var item = ServiceRepository
                .GetService<IRulesetItemFactoryService>()
                .CreateStandardItem(baseItem);

            device = item as RulesetItemDevice;
            if (device == null)
            {
                item.Unregister();
                throw new ArgumentException($"Can't create RulesetItemDevice from '{baseItem.Name}'!");
            }

            devices.Add(baseItem.Name, device);
        }

        //Update charges based on current state of the pool
        var charges = device.Attributes["ItemCharges"];
        var deviceDescription = device.UsableDeviceDescription;
        var maxUsesForPool = hero.GetMaxUsesForPool(Pool);
        var remainingPowerCharges = hero.GetRemainingPowerCharges(Pool);

        deviceDescription.rechargeRate = Pool.RechargeRate;
        deviceDescription.chargesCapitalNumber = maxUsesForPool;
        deviceDescription.rechargeNumber = maxUsesForPool;
        charges.MaxValue = maxUsesForPool;
        charges.BaseValue = remainingPowerCharges;
        charges.Refresh();

        return device;
    }
}
