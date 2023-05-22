using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition, LootPackDefinitionBuilder>
{
    internal LootPackDefinitionBuilder SetItemOccurrencesList([NotNull] params ItemOccurence[] occurrences)
    {
        Definition.ItemOccurencesList.SetRange(occurrences);
        return this;
    }

    internal LootPackDefinitionBuilder AddExplicitItem(ItemDefinition item, int diceNumber = 1,
        RuleDefinitions.DieType diceType = RuleDefinitions.DieType.D1, int bonus = 0)
    {
        Definition.ItemOccurencesList.Add(new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.Explicit,
            itemDefinition = item,
            diceNumber = diceNumber,
            diceType = diceType,
            additiveModifier = bonus
        });
        return this;
    }

    #region Constructors

    protected LootPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected LootPackDefinitionBuilder(LootPackDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
