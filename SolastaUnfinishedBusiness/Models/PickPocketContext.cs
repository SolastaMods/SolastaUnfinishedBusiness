using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.TreasureTableDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class PickPocketContext
{
    private static bool _initialized;

    internal static void Load()
    {
        if (!Main.Settings.AddPickPocketableLoot || _initialized)
        {
            return;
        }

        _initialized = true;

        var pickPocketableLootTrivial = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_Trivial")
            .SetGuiPresentationNoContent()
            .AddExplicitItem(_1D6_Silver_Coins)
            .AddExplicitItem(_1D6_Copper_Coins)
            .AddToDB();

        var pickPocketableLootA = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_A")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = RandomTreasureTableE2_Mundane_Ingredients,
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_1D6_Silver_Coins)
            .AddExplicitItem(_1D6_Silver_Coins)
            .AddExplicitItem(_6D6_Copper_Coins)
            .AddToDB();

        var pickPocketableLootB = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_B")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = TreasureTableDefinitionBuilder
                        .Create(RandomTreasureTableE2_Mundane_Ingredients, "PickPocketTableB")
                        .SetGuiPresentationNoContent()
                        .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
                        .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions)
                        .AddToDB(),
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_1D6_Gold_Coins)
            .AddExplicitItem(_10D6_Copper_Coins)
            .AddToDB();

        var pickPocketableLootC = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_C")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = TreasureTableDefinitionBuilder
                        .Create(RandomTreasureTableE_Ingredients, "PickPocketTableC")
                        .SetGuiPresentationNoContent()
                        .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
                        .AddTreasureOptions(RandomTreasureTableA_Gem.TreasureOptions)
                        .AddToDB(),
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_5D6_Silver_Coins)
            .AddExplicitItem(_1D6_Gold_Coins)
            .AddToDB();

        var pickPocketableLootD = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_D")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = TreasureTableDefinitionBuilder
                        .Create(RandomTreasureTableE_Ingredients, "PickPocketTableD")
                        .SetGuiPresentationNoContent()
                        .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
                        .AddTreasureOptions(RandomTreasureTableA_Gem.TreasureOptions)
                        .AddToDB(),
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_5D6_Gold_Coins)
            .AddExplicitItem(_20D6_Silver_Coins)
            .AddToDB();

        var pickPocketableLootE = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_E")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = TreasureTableDefinitionBuilder
                        .Create(RandomTreasureTableA_Gem, "PickPocketTableE")
                        .SetGuiPresentationNoContent()
                        .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
                        .AddTreasureOptions(DLC3_RandomTreasureTableJ_IngredientsEnchanted.TreasureOptions)
                        .AddToDB(),
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_5D6_Gold_Coins)
            .AddExplicitItem(_20D6_Silver_Coins)
            .AddToDB();

        var pickPocketableLootUndead = LootPackDefinitionBuilder
            .Create("CE_PickpocketableLoot_Undead")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(new
                ItemOccurence
                {
                    itemMode = ItemOccurence.SelectionMode.TreasureTable,
                    treasureTableDefinition = TreasureTableDefinitionBuilder
                        .Create("PickPocketTableUndead")
                        .SetGuiPresentationNoContent()
                        .AddTreasureOptions(new
                            List<TreasureOption>
                            {
                                RandomTreasureTableE_Ingredients.TreasureOptions[3],
                                RandomTreasureTableE_Ingredients.TreasureOptions[9],
                                RandomTreasureTableE_Ingredients.TreasureOptions[16]
                            })
                        .AddToDB(),
                    diceNumber = 1,
                    diceType = DieType.D1,
                    additiveModifier = 0
                })
            .AddExplicitItem(_1D6_Copper_Coins)
            .AddToDB();

        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            switch (monster.CharacterFamily)
            {
                case "Humanoid" when monster.DefaultFaction == "HostileMonsters" &&
                                     !monster.StealableLootDefinition:
                {
                    if (monster.ChallengeRating < 1.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootTrivial;
                    }

                    if (monster.ChallengeRating > 0.9 &&
                        monster.ChallengeRating < 2.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootA;
                    }

                    if (monster.ChallengeRating > 1.9 &&
                        monster.ChallengeRating < 3.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootB;
                    }

                    if (monster.ChallengeRating > 2.9 &&
                        monster.ChallengeRating < 5.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootC;
                    }

                    if (monster.ChallengeRating > 4.9 &&
                        monster.ChallengeRating < 7.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootD;
                    }

                    if (monster.ChallengeRating > 6.9)
                    {
                        monster.stealableLootDefinition = pickPocketableLootE;
                    }

                    break;
                }
                case "Undead" when monster.DefaultFaction.Contains("HostileMonsters") &&
                                   !monster.StealableLootDefinition && !monster.Name.Contains("Ghost") &&
                                   !monster.Name.Contains("Spectral") && !monster.Name.Contains("Servant"):
                    monster.stealableLootDefinition = pickPocketableLootUndead;
                    break;
            }
        }
    }
}
