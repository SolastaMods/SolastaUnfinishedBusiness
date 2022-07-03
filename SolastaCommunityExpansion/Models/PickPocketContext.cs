using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static FeatureDefinitionAbilityCheckAffinity;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.LootPackDefinitions;

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
        if (!Main.Settings.AddPickpocketableLoot || initialized)
        {
            return;
        }

        initialized = true;

        var sleightOfHand = DatabaseHelper.SkillDefinitions.SleightOfHand;
        sleightOfHand.GuiPresentation.unusedInSolastaCOTM = false;

        var treasureCopper = new TreasureOption
        {
            odds = 30, itemDefinition = DatabaseHelper.ItemDefinitions._1D6_Copper_Coins, amount = 3
        };

        var treasure_silver = new TreasureOption();
        treasure_silver.odds = 12;
        treasure_silver.itemDefinition = DatabaseHelper.ItemDefinitions._1D6_Silver_Coins;
        treasure_silver.amount = 2;

        var treasure_gold = new TreasureOption();
        treasure_gold.odds = 8;
        treasure_gold.itemDefinition = DatabaseHelper.ItemDefinitions._1D6_Gold_Coins;
        treasure_gold.amount = 1;

        var treasure_abyss_moss = new TreasureOption();
        treasure_abyss_moss.odds = 20;
        treasure_abyss_moss.itemDefinition = DatabaseHelper.ItemDefinitions.Ingredient_AbyssMoss;
        treasure_abyss_moss.amount = 1;

        var treasure_deeproot_lichen = new TreasureOption();
        treasure_deeproot_lichen.odds = 20;
        treasure_deeproot_lichen.itemDefinition = DatabaseHelper.ItemDefinitions.Ingredient_DeepRootLichen;
        treasure_deeproot_lichen.amount = 1;

        var treasure_goblinhair_fungus = new TreasureOption();
        treasure_goblinhair_fungus.odds = 20;
        treasure_goblinhair_fungus.itemDefinition = DatabaseHelper.ItemDefinitions.Ingredient_GoblinHairFungus;
        treasure_goblinhair_fungus.amount = 1;

        var treasure_scrollbless = new TreasureOption();
        treasure_scrollbless.odds = 1;
        treasure_scrollbless.itemDefinition = DatabaseHelper.ItemDefinitions.ScrollBless;
        treasure_scrollbless.amount = 1;

        var treasure_scrollcurewounds = new TreasureOption();
        treasure_scrollcurewounds.odds = 1;
        treasure_scrollcurewounds.itemDefinition = DatabaseHelper.ItemDefinitions.ScrollCureWounds;
        treasure_scrollcurewounds.amount = 1;

        var treasure_scrolldetectmagic = new TreasureOption();
        treasure_scrolldetectmagic.odds = 1;
        treasure_scrolldetectmagic.itemDefinition = DatabaseHelper.ItemDefinitions.ScrollDetectMagic;
        treasure_scrolldetectmagic.amount = 1;

        var treasure_scrollidentify = new TreasureOption();
        treasure_scrollidentify.odds = 1;
        treasure_scrollidentify.itemDefinition = DatabaseHelper.ItemDefinitions.ScrollIdentify;
        treasure_scrollidentify.amount = 1;

        var treasure_poisonbasic = new TreasureOption();
        treasure_poisonbasic.odds = 1;
        treasure_poisonbasic.itemDefinition = DatabaseHelper.ItemDefinitions.Poison_Basic;
        treasure_poisonbasic.amount = 1;

        var treasure_potionremedy = new TreasureOption();
        treasure_potionremedy.odds = 1;
        treasure_potionremedy.itemDefinition = DatabaseHelper.ItemDefinitions.PotionRemedy;
        treasure_potionremedy.amount = 1;

        var treasure_dagger = new TreasureOption();
        treasure_dagger.odds = 4;
        treasure_dagger.itemDefinition = DatabaseHelper.ItemDefinitions.Dagger;
        treasure_dagger.amount = 1;

        var treasure_refinedoil = new TreasureOption();
        treasure_refinedoil.odds = 1;
        treasure_refinedoil.itemDefinition = DatabaseHelper.ItemDefinitions.Ingredient_RefinedOil;
        treasure_refinedoil.amount = 1;

        var treasure_acid = new TreasureOption();
        treasure_acid.odds = 1;
        treasure_acid.itemDefinition = DatabaseHelper.ItemDefinitions.Ingredient_Acid;
        treasure_acid.amount = 1;

        var treasure_amethyst = new TreasureOption();
        treasure_amethyst.odds = 1;
        treasure_amethyst.itemDefinition = DatabaseHelper.ItemDefinitions._20_GP_Amethyst;
        treasure_amethyst.amount = 1;

        var pick_pocket_table = TreasureTableDefinitionBuilder
            .Create(DatabaseHelper.TreasureTableDefinitions.RandomTreasureTableG_25_GP_Art_Items, "PickPocketTable",
                "79cac3e5-0f00-4062-b263-adbc854223d7")
            .SetGuiPresentationNoContent()
            .AddToDB();

        pick_pocket_table.TreasureOptions.Add(treasureCopper);
        pick_pocket_table.TreasureOptions.Add(treasure_silver);
        pick_pocket_table.TreasureOptions.Add(treasure_gold);
        pick_pocket_table.TreasureOptions.Add(treasure_scrollbless);
        pick_pocket_table.TreasureOptions.Add(treasure_scrollcurewounds);
        pick_pocket_table.TreasureOptions.Add(treasure_scrolldetectmagic);
        pick_pocket_table.TreasureOptions.Add(treasure_scrollidentify);
        pick_pocket_table.TreasureOptions.Add(treasure_poisonbasic);
        pick_pocket_table.TreasureOptions.Add(treasure_potionremedy);
        pick_pocket_table.TreasureOptions.Add(treasure_dagger);
        pick_pocket_table.TreasureOptions.Add(treasure_refinedoil);
        pick_pocket_table.TreasureOptions.Add(treasure_acid);
        pick_pocket_table.TreasureOptions.Add(treasure_amethyst);

        var pick_pocket_table_undead = TreasureTableDefinitionBuilder
            .Create(DatabaseHelper.TreasureTableDefinitions.RandomTreasureTableG_25_GP_Art_Items,
                "PickPocketTableC", "f1bbd8e5-3e05-48da-9c70-2db676a280b4")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(treasureCopper, treasure_abyss_moss, treasure_deeproot_lichen,
                treasure_goblinhair_fungus)
            .AddToDB();

        var loot_pickpocket_table = new ItemOccurence(Zombie_loot_drop.ItemOccurencesList[0]);
        loot_pickpocket_table.itemMode = ItemOccurence.SelectionMode.TreasureTable;
        loot_pickpocket_table.treasureTableDefinition = pick_pocket_table;
        loot_pickpocket_table.diceNumber = 1;

        var loot_pickpocket_undead = new ItemOccurence(Zombie_loot_drop.ItemOccurencesList[0]);
        loot_pickpocket_undead.itemMode = ItemOccurence.SelectionMode.TreasureTable;
        loot_pickpocket_undead.treasureTableDefinition = pick_pocket_table_undead;
        loot_pickpocket_undead.diceNumber = 1;

        var pick_pocket_loot = LootPackDefinitionBuilder.CreateCopyFrom(Tutorial_04_Loot_Stealable,
                "PickPocketLoot", "30c308db-1ad7-4f93-9431-43ce32358493")
            .SetGuiPresentationNoContent()
            .AddToDB();

        pick_pocket_loot.lootChallengeMode = LootPackDefinition.LootChallenge.ByPartyLevel;
        pick_pocket_loot.ItemOccurencesList.Clear();
        pick_pocket_loot.ItemOccurencesList.Add(loot_pickpocket_table);

        var pick_pocket_undead = LootPackDefinitionBuilder.CreateCopyFrom(Tutorial_04_Loot_Stealable,
                "PickPocketUndead", "af2eb8e0-6a5a-40e2-8a62-160f80e2453e")
            .SetGuiPresentationNoContent()
            .AddToDB();

        pick_pocket_undead.lootChallengeMode = LootPackDefinition.LootChallenge.ByPartyLevel;
        pick_pocket_undead.ItemOccurencesList.Clear();
        pick_pocket_undead.ItemOccurencesList.Add(loot_pickpocket_undead);

        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            if (monster.CharacterFamily == "Humanoid" &&
                monster.DefaultFaction == "HostileMonsters")
            {
                monster.stealableLootDefinition = pick_pocket_loot;
            }
        }

        var adam_12 = DatabaseHelper.MonsterDefinitions.Adam_The_Twelth;
        adam_12.stealableLootDefinition = pick_pocket_loot;

        var brood_of_blood = DatabaseHelper.MonsterDefinitions.Brood_of_blood;
        brood_of_blood.stealableLootDefinition = pick_pocket_loot;

        var brood_of_dread = DatabaseHelper.MonsterDefinitions.Brood_of_dread;
        brood_of_dread.stealableLootDefinition = pick_pocket_loot;

        var brood_of_flesh = DatabaseHelper.MonsterDefinitions.Brood_of_flesh;
        brood_of_flesh.stealableLootDefinition = pick_pocket_loot;

        var skeleton_basic = DatabaseHelper.MonsterDefinitions.Skeleton;
        skeleton_basic.stealableLootDefinition = pick_pocket_undead;

        var skeleton_archer = DatabaseHelper.MonsterDefinitions.Skeleton_Archer;
        skeleton_archer.stealableLootDefinition = pick_pocket_undead;

        var skeleton_enforcer = DatabaseHelper.MonsterDefinitions.Skeleton_Enforcer;
        skeleton_enforcer.stealableLootDefinition = pick_pocket_undead;

        var skeleton_knight = DatabaseHelper.MonsterDefinitions.Skeleton_Knight;
        skeleton_knight.stealableLootDefinition = pick_pocket_undead;

        var skeleton_marksman = DatabaseHelper.MonsterDefinitions.Skeleton_Marksman;
        skeleton_marksman.stealableLootDefinition = pick_pocket_undead;

        var skeleton_sorcerer = DatabaseHelper.MonsterDefinitions.Skeleton_Sorcerer;
        skeleton_sorcerer.stealableLootDefinition = pick_pocket_undead;
    }
}
