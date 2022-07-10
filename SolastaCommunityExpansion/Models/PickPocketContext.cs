using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static FeatureDefinitionAbilityCheckAffinity;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.LootPackDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.TreasureTableDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class PickPocketContext
{
    private static bool _initialized;

    internal static void CreateFeats([NotNull] ICollection<FeatDefinition> feats)
    {
        var pickpocketCheckAffinity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityFeatLockbreaker,
                "AbilityCheckAffinityFeatPickPocket", "30b1492a-053f-412e-b247-798fbc255038")
            .SetGuiPresentation("PickPocketFeat", Category.Feat)
            .AddToDB();

        var pickpocketAbilityCheckAffinityGroup = new AbilityCheckAffinityGroup
        {
            abilityScoreName = AttributeDefinitions.Dexterity,
            proficiencyName = SkillDefinitions.SleightOfHand,
            affinity = CharacterAbilityCheckAffinity.Advantage
        };

        pickpocketCheckAffinity.AffinityGroups.SetRange(pickpocketAbilityCheckAffinityGroup);

        var pickpocketProficiency = FeatureDefinitionProficiencyBuilder
            .Create(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker,
                "ProficiencyFeatPickPocket", "d8046b0c-2f93-4b47-b2dd-110234a4a848")
            .SetGuiPresentation("PickPocketFeat", Category.Feat)
            .AddToDB();

        pickpocketProficiency.proficiencyType = ProficiencyType.SkillOrExpertise;
        pickpocketProficiency.Proficiencies.Clear();
        pickpocketProficiency.Proficiencies.Add(SkillDefinitions.SleightOfHand);

        var pickPocketFeat = FeatDefinitionBuilder
            .Create(DatabaseHelper.FeatDefinitions.Lockbreaker, "PickPocketFeat",
                "947a31fc-4990-45a5-bcfd-6c478b4dff8a")
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        pickPocketFeat.Features.SetRange(pickpocketCheckAffinity, pickpocketProficiency);

        feats.Add(pickPocketFeat);
    }

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
            .Create(RandomTreasureTableE2_Mundane_Ingredients, "PickPocketTableLow",
                "79cac3e5-0f00-4062-b263-adbc854223d7")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddToDB();

        var pickpocketTableMed = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE_Ingredients, "PickPocketTableMed",
                "79cac3e5-0f00-4062-b263-adbc854223d8")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddTreasureOptions(RandomTreasureTableA_Gem.TreasureOptions)
            .SetGuiPresentationNoContent()
            .AddToDB();

        var pickpocketTableUndead = TreasureTableDefinitionBuilder
            .Create("PickPocketTableUndead",
                "79cac3e5-0f00-4062-b263-adbc854223d9")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[3])
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[9])
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[16])
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

        var pickPocketableLootA = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_A", "edb9b436-1d94-4d11-bd37-4027c4dc7640")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootB = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_B", "edb9b436-1d94-4d11-bd37-4027c4dc7641")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootC = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_C", "edb9b436-1d94-4d11-bd37-4027c4dc7642")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootD = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_D", "edb9b436-1d94-4d11-bd37-4027c4dc7643")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow, lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootUndead = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_Undead",
                "edb9b436-1d94-4d11-bd37-4027c4dc7644")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableUndead)
            .AddToDB();

        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            switch (monster.CharacterFamily)
            {
                case "Humanoid" when monster.DefaultFaction == "HostileMonsters" &&
                                     monster.StealableLootDefinition == null:
                {
                    if (monster.ChallengeRating < 1.0)
                    {
                        monster.stealableLootDefinition = Pickpocket_generic_loot_LowMoney;
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
                                   monster.StealableLootDefinition == null && !monster.Name.Contains("Ghost") &&
                                   !monster.Name.Contains("Spectral") && !monster.Name.Contains("Servant"):
                    monster.stealableLootDefinition = pickPocketableLootUndead;
                    break;
            }
        }
    }
}
