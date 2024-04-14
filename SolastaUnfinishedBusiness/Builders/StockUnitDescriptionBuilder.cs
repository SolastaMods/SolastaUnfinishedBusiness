using System;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.Builders;

internal class StockUnitDescriptionBuilder
{
    private string _factionStatus = DatabaseHelper.FactionStatusDefinitions.Alliance.Name;
    private int _initialAmount = 1;
    private ItemDefinition _itemDefinition;
    private int _maxAmount = 1;
    private int _minAmount;
    private int _reassortAmount;
    private RuleDefinitions.DurationType _reassortRateType = RuleDefinitions.DurationType.Day;
    private int _reassortRateValue = 1;
    private string _requiredFaction = string.Empty;
    private int _stackCount = 1;

    internal StockUnitDescriptionBuilder SetItem(ItemDefinition itemDefinition)
    {
        _itemDefinition = itemDefinition;
        return this;
    }

    internal StockUnitDescriptionBuilder SetStock(
        int stackCount = 1,
        int initialAmount = 0,
        int minAmount = 0,
        int maxAmount = 1)
    {
        _stackCount = stackCount;
        _initialAmount = initialAmount;
        _minAmount = minAmount;
        _maxAmount = maxAmount;
        return this;
    }

    internal StockUnitDescriptionBuilder SetRestock(
        int reassortAmount = 0,
        RuleDefinitions.DurationType reassortRateType = RuleDefinitions.DurationType.Day,
        int reassortRateValue = 1)
    {
        _reassortAmount = reassortAmount;
        _reassortRateType = reassortRateType;
        _reassortRateValue = reassortRateValue;
        return this;
    }

    internal StockUnitDescriptionBuilder SetFaction(string faction, string status)
    {
        _requiredFaction = faction;
        _factionStatus = status;
        return this;
    }

    internal StockUnitDescription Build()
    {
        if (!_itemDefinition)
        {
            throw new ArgumentException("StockUnitDescriptionBuilder: trying to build with empty item!");
        }

        var stock = new StockUnitDescription
        {
            initialized = true,
            ItemDefinition = _itemDefinition,
            StackCount = _stackCount,
            InitialAmount = _initialAmount,
            MinAmount = _minAmount,
            MaxAmount = _maxAmount,
            ReassortAmount = _reassortAmount,
            ReassortRateType = _reassortRateType,
            ReassortRateValue = _reassortRateValue,
            RequiredFaction = _requiredFaction,
            factionStatus = _factionStatus
        };

        return stock;
    }
}
