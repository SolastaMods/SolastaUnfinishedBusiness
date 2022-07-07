using System.Collections.Generic;
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
    private static bool initialized;

    internal static void CreateFeats(ICollection<FeatDefinition> feats)
    {
        var pickpocket_check_affinity = FeatureDefinitionAbilityCheckAffinityBuilder
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

        pickpocket_check_affinity.AffinityGroups.SetRange(pickpocketAbilityCheckAffinityGroup);

        var pickpocket_proficiency = FeatureDefinitionProficiencyBuilder
            .Create(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker,
                "ProficiencyFeatPickPocket", "d8046b0c-2f93-4b47-b2dd-110234a4a848")
            .SetGuiPresentation("PickPocketFeat", Category.Feat)
            .AddToDB();

        pickpocket_proficiency.proficiencyType = ProficiencyType.SkillOrExpertise;
        pickpocket_proficiency.Proficiencies.Clear();
        pickpocket_proficiency.Proficiencies.Add(SkillDefinitions.SleightOfHand);

        var pickPocketFeat = FeatDefinitionBuilder
            .Create(DatabaseHelper.FeatDefinitions.Lockbreaker, "PickPocketFeat",
                "947a31fc-4990-45a5-bcfd-6c478b4dff8a")
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        pickPocketFeat.Features.SetRange(pickpocket_check_affinity, pickpocket_proficiency);

        feats.Add(pickPocketFeat);
    }

    internal static void Load()
    {
        if (!Main.Settings.AddPickPocketableLoot || initialized)
        {
            return;
        }

        initialized = true;

        var sleightOfHand = DatabaseHelper.SkillDefinitions.SleightOfHand;
        sleightOfHand.GuiPresentation.unusedInSolastaCOTM = false;

        var pickpocket_table_low = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE2_Mundane_Ingredients, "PickPocketTableLow",
                "79cac3e5-0f00-4062-b263-adbc854223d7")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocket_table_low.TreasureOptions.AddRange(RandomTreasureTableB_Consumables.TreasureOptions);

        var pickpocket_table_med = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE_Ingredients, "PickPocketTableMed",
                "79cac3e5-0f00-4062-b263-adbc854223d8")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocket_table_med.TreasureOptions.AddRange(RandomTreasureTableB_Consumables.TreasureOptions);
        pickpocket_table_med.TreasureOptions.AddRange(RandomTreasureTableA_Gem.TreasureOptions);

        var pickpocket_table_undead = TreasureTableDefinitionBuilder
            .Create("PickPocketTableUndead",
                "79cac3e5-0f00-4062-b263-adbc854223d9")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocket_table_undead.TreasureOptions.Add(RandomTreasureTableE_Ingredients.TreasureOptions[3]);
        pickpocket_table_undead.TreasureOptions.Add(RandomTreasureTableE_Ingredients.TreasureOptions[9]);
        pickpocket_table_undead.TreasureOptions.Add(RandomTreasureTableE_Ingredients.TreasureOptions[16]);

        var loot_pickpocket_table_low = new ItemOccurence();
        loot_pickpocket_table_low.itemMode = ItemOccurence.SelectionMode.TreasureTable;
        loot_pickpocket_table_low.treasureTableDefinition = pickpocket_table_low;
        loot_pickpocket_table_low.diceNumber = 1;
        loot_pickpocket_table_low.diceType = DieType.D1;
        loot_pickpocket_table_low.additiveModifier = 0;

        var loot_pickpocket_table_med = new ItemOccurence();
        loot_pickpocket_table_med.itemMode = ItemOccurence.SelectionMode.TreasureTable;
        loot_pickpocket_table_med.treasureTableDefinition = pickpocket_table_med;
        loot_pickpocket_table_med.diceNumber = 1;
        loot_pickpocket_table_med.diceType = DieType.D1;
        loot_pickpocket_table_med.additiveModifier = 0;

        var loot_pickpocket_table_undead = new ItemOccurence();
        loot_pickpocket_table_undead.itemMode = ItemOccurence.SelectionMode.TreasureTable;
        loot_pickpocket_table_undead.treasureTableDefinition = pickpocket_table_undead;
        loot_pickpocket_table_undead.diceNumber = 1;
        loot_pickpocket_table_undead.diceType = DieType.D1;
        loot_pickpocket_table_undead.additiveModifier = 0;

        var pickpocketableLootA = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_A", "edb9b436-1d94-4d11-bd37-4027c4dc7640")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocketableLootA.ItemOccurencesList.Add(loot_pickpocket_table_low);

        var pickpocketableLootB = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_B", "edb9b436-1d94-4d11-bd37-4027c4dc7641")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocketableLootB.ItemOccurencesList.Add(loot_pickpocket_table_low);

        var pickpocketableLootC = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_C", "edb9b436-1d94-4d11-bd37-4027c4dc7642")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocketableLootC.ItemOccurencesList.Add(loot_pickpocket_table_med);

        var pickpocketableLootD = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_D", "edb9b436-1d94-4d11-bd37-4027c4dc7643")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocketableLootD.ItemOccurencesList.Add(loot_pickpocket_table_low);
        pickpocketableLootD.ItemOccurencesList.Add(loot_pickpocket_table_med);

        var pickpocketableLootUndead = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_Undead", "edb9b436-1d94-4d11-bd37-4027c4dc7644")
            .SetGuiPresentationNoContent()
            .AddToDB();
        pickpocketableLootUndead.ItemOccurencesList.Add(loot_pickpocket_table_undead);

        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            if (monster.CharacterFamily == "Humanoid" &&
                monster.DefaultFaction == "HostileMonsters" &&
                monster.StealableLootDefinition == null
                )
            {
                if (monster.ChallengeRating < 1.0)
                {
                    monster.stealableLootDefinition = Pickpocket_generic_loot_LowMoney;
                }

                if (monster.ChallengeRating > 0.9 &&
                    monster.ChallengeRating < 2.0)
                {
                    monster.stealableLootDefinition = pickpocketableLootA;
                }

                if (monster.ChallengeRating > 1.9 &&
                    monster.ChallengeRating < 3.0)
                {
                    monster.stealableLootDefinition = pickpocketableLootB;
                }

                if (monster.ChallengeRating > 2.9 &&
                    monster.ChallengeRating < 5.0)
                {
                    monster.stealableLootDefinition = pickpocketableLootC;
                }

                if (monster.ChallengeRating > 4.9)
                {
                    monster.stealableLootDefinition = pickpocketableLootD;
                }
            }

            if (monster.CharacterFamily == "Undead" &&
                monster.DefaultFaction.Contains("HostileMonsters") &&
                monster.StealableLootDefinition == null &&
                !monster.Name.Contains("Ghost") &&
                !monster.Name.Contains("Spectral") &&
                !monster.Name.Contains("Servant")
                )
            { 
                monster.stealableLootDefinition = pickpocketableLootUndead;
            }
        }

    }
}
