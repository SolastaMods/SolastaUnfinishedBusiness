using System;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Builders;

public class StockUnitDescriptionBuilder
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

#if false
    public StockUnitDescriptionBuilder(StockUnitDescription reference)
    {
        _itemDefinition = reference.ItemDefinition;
        _stackCount = reference.StackCount;
        _initialAmount = reference.InitialAmount;
        _minAmount = reference.MinAmount;
        _maxAmount = reference.MaxAmount;
        _reassortAmount = reference.ReassortAmount;
        _reassortRateType = reference.ReassortRateType;
        _reassortRateValue = reference.ReassortRateValue;
        _requiredFaction = reference.RequiredFaction;
        _factionStatus = reference.Factionstatus;
    }
#endif

    public StockUnitDescriptionBuilder SetItem(ItemDefinition itemDefinition)
    {
        _itemDefinition = itemDefinition;
        return this;
    }

    public StockUnitDescriptionBuilder SetStock(int stackCount = 1, int initialAmount = 0,
        int minAmount = 0, int maxAmount = 1)
    {
        _stackCount = stackCount;
        _initialAmount = initialAmount;
        _minAmount = minAmount;
        _maxAmount = maxAmount;
        return this;
    }

    public StockUnitDescriptionBuilder SetRestock(int reassortAmount = 0,
        RuleDefinitions.DurationType reassortRateType = RuleDefinitions.DurationType.Day, int reassortRateValue = 1)
    {
        _reassortAmount = reassortAmount;
        _reassortRateType = reassortRateType;
        _reassortRateValue = reassortRateValue;
        return this;
    }

    public StockUnitDescriptionBuilder SetFaction(FactionDefinition faction, FactionStatusDefinition status)
    {
        _requiredFaction = faction.Name;
        _factionStatus = status.Name;
        return this;
    }


    public StockUnitDescriptionBuilder SetFaction(string faction, string status)
    {
        _requiredFaction = faction;
        _factionStatus = status;
        return this;
    }

    public StockUnitDescription Build()
    {
        if (_itemDefinition == null)
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
