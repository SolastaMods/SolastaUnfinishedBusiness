using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

//TODO: add validation to check if this should apply at all
/**
 * Adds fake usable device to hero's devices that has its charges set to uses of provided power pool.
 */
internal class PowerPoolDevice
{
    private static readonly Dictionary<RulesetCharacter, Dictionary<string, RulesetItemDevice>> DeviceCache = new();

    private readonly ItemDefinition baseItem;

    internal PowerPoolDevice(ItemDefinition baseItem, FeatureDefinitionPower pool)
    {
        this.baseItem = baseItem;
        Pool = pool;
    }

    internal FeatureDefinitionPower Pool { get; }

    internal static void Clear(RulesetCharacter hero)
    {
        DeviceCache.Remove(hero);
    }

    internal static PowerPoolDevice GetFromRulesetItem(RulesetCharacter hero, RulesetItemDevice device)
    {
        return hero.GetSubFeaturesByType<PowerPoolDevice>()
            .FirstOrDefault(p => p.baseItem.Name == device.ItemDefinition.Name);
    }

    internal RulesetItemDevice GetDevice(RulesetCharacter hero)
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
                .CreateStandardItem(baseItem, false);

            device = item as RulesetItemDevice;

            if (device == null)
            {
                throw new ArgumentException($"Can't create RulesetItemDevice from '{baseItem.Name}'!");
            }

            //mark fake device item as unidentified, so that if base item marked as need identification, it wont list all functions in the tooltip
            device.Identified = false;
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
