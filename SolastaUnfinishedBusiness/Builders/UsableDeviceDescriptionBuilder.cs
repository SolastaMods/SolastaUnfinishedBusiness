using UnityEngine.AddressableAssets;
using static EquipmentDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class UsableDeviceDescriptionBuilder
{
    private readonly UsableDeviceDescription _description;

    internal UsableDeviceDescriptionBuilder()
    {
        _description = new UsableDeviceDescription
        {
            usage = ItemUsage.ByFunction,
            chargesCapital = ItemChargesCapital.Fixed,
            chargesCapitalNumber = 1,
            chargesCapitalDie = RuleDefinitions.DieType.D1,
            chargesCapitalBonus = 0,
            rechargeRate = RuleDefinitions.RechargeRate.Dawn,
            rechargeNumber = 1,
            rechargeDie = RuleDefinitions.DieType.D1,
            rechargeBonus = 0,
            outOfChargesConsequence = ItemOutOfCharges.Persist,
            magicAttackBonus = 0,
            saveDC = 10,
            onUseParticle = new AssetReference()
        };
    }

    internal UsableDeviceDescriptionBuilder SetSaveDc(int dc)
    {
        _description.saveDC = dc;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetMagicAttackBonus(int bonus)
    {
        _description.magicAttackBonus = bonus;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetOutOfChargesConsequence(ItemOutOfCharges consequence)
    {
        _description.outOfChargesConsequence = consequence;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetUsage(ItemUsage usage)
    {
        _description.usage = usage;
        return this;
    }

    internal UsableDeviceDescriptionBuilder AddFunctions(params DeviceFunctionDescription[] functions)
    {
        _description.DeviceFunctions.AddRange(functions);
        return this;
    }

    internal UsableDeviceDescription Build()
    {
        return _description;
    }

    #region Charge

    internal UsableDeviceDescriptionBuilder SetChargesCapitalNumber(int number)
    {
        _description.chargesCapitalNumber = number;
        return this;
    }

#if false
    internal UsableDeviceDescriptionBuilder SetCharges(
        ItemChargesCapital capital = ItemChargesCapital.Fixed,
        int number = 1,
        RuleDefinitions.DieType dieType = RuleDefinitions.DieType.D1,
        int bonus = 0)
    {
        description.chargesCapital = capital;
        description.chargesCapitalNumber = number;
        description.chargesCapitalDie = dieType;
        description.chargesCapitalBonus = bonus;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetChargesCapital(ItemChargesCapital capital)
    {
        description.chargesCapital = capital;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetChargesCapitalDie(RuleDefinitions.DieType dieType)
    {
        description.chargesCapitalDie = dieType;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetChargesCapitalBonus(int bonus)
    {
        description.chargesCapitalBonus = bonus;
        return this;
    }
#endif

    #endregion

    #region Recharge

    internal UsableDeviceDescriptionBuilder SetRecharge(
        RuleDefinitions.RechargeRate rate = RuleDefinitions.RechargeRate.Dawn,
        int number = 1,
        RuleDefinitions.DieType dieType = RuleDefinitions.DieType.D1,
        int bonus = 0)
    {
        _description.rechargeRate = rate;
        _description.rechargeNumber = number;
        _description.rechargeDie = dieType;
        _description.rechargeBonus = bonus;
        return this;
    }

#if false
    internal UsableDeviceDescriptionBuilder SetRechargeRate(RuleDefinitions.RechargeRate rate)
    {
        description.rechargeRate = rate;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetRechargeNumber(int number)
    {
        description.rechargeNumber = number;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetRechargeDie(RuleDefinitions.DieType dieType)
    {
        description.rechargeDie = dieType;
        return this;
    }

    internal UsableDeviceDescriptionBuilder SetRechargeBonus(int bonus)
    {
        description.rechargeBonus = bonus;
        return this;
    }
#endif

    #endregion
}
