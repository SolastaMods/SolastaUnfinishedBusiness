using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using static RuleDefinitions;
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

        var sleightOfHand = DatabaseHelper.SkillDefinitions.SleightOfHand;
        sleightOfHand.GuiPresentation.unusedInSolastaCOTM = false;

        var pickpocketTableLow = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE2_Mundane_Ingredients, "PickPocketTableLow")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddToDB();

        var pickpocketTableMed = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE_Ingredients, "PickPocketTableMed")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddTreasureOptions(RandomTreasureTableA_Gem.TreasureOptions)
            .SetGuiPresentationNoContent()
            .AddToDB();

        var pickpocketTableUndead = TreasureTableDefinitionBuilder
            .Create("PickPocketTableUndead")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(new List<TreasureOption>
            {
                RandomTreasureTableE_Ingredients.TreasureOptions[3],
                RandomTreasureTableE_Ingredients.TreasureOptions[9],
                RandomTreasureTableE_Ingredients.TreasureOptions[16]
            })
            .AddToDB();

        var lootPickpocketTableLow = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableLow,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var lootPickpocketTableMed = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableMed,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var lootPickpocketTableUndead = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableUndead,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var pickpocketGenericLootLowMoney = DatabaseHelper.LootPackDefinitions.Pickpocket_generic_loot_LowMoney;
        var pickpocketGenericLootMedMoney = DatabaseHelper.LootPackDefinitions.Pickpocket_generic_loot_MedMoney;

        var pickPocketableLootA = LootPackDefinitionBuilder
            .Create(pickpocketGenericLootLowMoney, "CE_PickpocketableLoot_A")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootB = LootPackDefinitionBuilder
            .Create(pickpocketGenericLootMedMoney, "CE_PickpocketableLoot_B")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootC = LootPackDefinitionBuilder
            .Create(pickpocketGenericLootMedMoney, "CE_PickpocketableLoot_C")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootD = LootPackDefinitionBuilder
            .Create(pickpocketGenericLootMedMoney, "CE_PickpocketableLoot_D")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow, lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootUndead = LootPackDefinitionBuilder
            .Create(pickpocketGenericLootLowMoney, "CE_PickpocketableLoot_Undead")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableUndead)
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
                        monster.stealableLootDefinition = pickpocketGenericLootLowMoney;
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

                    if (monster.ChallengeRating > 4.9)
                    {
                        monster.stealableLootDefinition = pickPocketableLootD;
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
