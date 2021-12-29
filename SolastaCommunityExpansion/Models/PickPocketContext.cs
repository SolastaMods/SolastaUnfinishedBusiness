
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.LootPackDefinitions;

namespace SolastaCommunityExpansion.Models
{
    [TargetType(typeof(TreasureOption))]
    public static partial class TreasureOptionsExtension
    {
        public static T SetOdds<T>(this T entity, int value)
           where T : TreasureOption
        {
            entity.SetField("odds", value);
            return entity;
        }
        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : TreasureOption
        {
            entity.SetField("itemDefinition", value);
            return entity;
        }
        public static T SetAmount<T>(this T entity, int value)
            where T : TreasureOption
        {
            entity.SetField("amount", value);
            return entity;
        }
    }

    public static class PickPocketContext
    {
        internal static void CreateFeats(List<FeatDefinition> feats)
        {
            FeatureDefinitionAbilityCheckAffinity pickpocket_check_affinity = PickPocketAbilityCheckAffinityBuilder.CreateCopyFrom(
                "AbilityCheckAffinityFeatPickPocket", "30b1492a-053f-412e-b247-798fbc255038", "Feat/&PickPocketFeatTitle", "Feat/&PickPocketFeatDescription",
                DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityFeatLockbreaker);

            // TODO make the set field calls type safe by using extensions annd/or builders
            FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup pickpocketAbilityCheckAffinityGroup = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup();

            pickpocketAbilityCheckAffinityGroup.SetField("abilityScoreName", "Dexterity");
            pickpocketAbilityCheckAffinityGroup.SetField("proficiencyName", "SleightOfHand");
            pickpocketAbilityCheckAffinityGroup.SetField("affinity", CharacterAbilityCheckAffinity.Advantage);
            pickpocket_check_affinity.AffinityGroups.Clear();
            pickpocket_check_affinity.AffinityGroups.Add(pickpocketAbilityCheckAffinityGroup);

            FeatureDefinitionProficiency pickpocket_proficiency = PickPocketProficiencyBuilder.CreateCopyFrom(
                "ProficiencyFeatPickPocket", "d8046b0c-2f93-4b47-b2dd-110234a4a848", "Feat/&PickPocketFeatTitle", "Feat/&PickPocketFeatDescription",
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker);

            pickpocket_proficiency.SetProficiencyType(ProficiencyType.SkillOrExpertise);
            pickpocket_proficiency.Proficiencies.Clear();
            pickpocket_proficiency.Proficiencies.Add("SleightOfHand");

            FeatDefinition PickPocketFeat = PickPocketFeatBuilder.CreateCopyFrom(
                "PickPocketFeat", "947a31fc-4990-45a5-bcfd-6c478b4dff8a", "Feat/&PickPocketFeatTitle", "Feat/&PickPocketFeatDescription",
                DatabaseHelper.FeatDefinitions.Lockbreaker);

            PickPocketFeat.Features.Clear();
            PickPocketFeat.Features.Add(pickpocket_check_affinity);
            PickPocketFeat.Features.Add(pickpocket_proficiency);

            feats.Add(PickPocketFeat);
        }

        private static bool initialized = false;

        internal static void Load()
        {
            if (!Main.Settings.AddPickpocketableLoot || initialized)
            {
                return;
            }
            initialized = true;

            SkillDefinition sleight_of_hand = DatabaseHelper.SkillDefinitions.SleightOfHand;
            sleight_of_hand.GuiPresentation.SetUnusedInSolastaCOTM(false);

            TreasureOption treasure_copper = new TreasureOption();
            treasure_copper.SetOdds(30);
            treasure_copper.SetItemDefinition(DatabaseHelper.ItemDefinitions._1D6_Copper_Coins);
            treasure_copper.SetAmount(3);

            TreasureOption treasure_silver = new TreasureOption();
            treasure_silver.SetOdds(12);
            treasure_silver.SetItemDefinition(DatabaseHelper.ItemDefinitions._1D6_Silver_Coins);
            treasure_silver.SetAmount(2);

            TreasureOption treasure_gold = new TreasureOption();
            treasure_gold.SetOdds(8);
            treasure_gold.SetItemDefinition(DatabaseHelper.ItemDefinitions._1D6_Gold_Coins);
            treasure_gold.SetAmount(1);

            TreasureOption treasure_abyss_moss = new TreasureOption();
            treasure_abyss_moss.SetOdds(20);
            treasure_abyss_moss.SetItemDefinition(DatabaseHelper.ItemDefinitions.Ingredient_AbyssMoss);
            treasure_abyss_moss.SetAmount(1);

            TreasureOption treasure_deeproot_lichen = new TreasureOption();
            treasure_deeproot_lichen.SetOdds(20);
            treasure_deeproot_lichen.SetItemDefinition(DatabaseHelper.ItemDefinitions.Ingredient_DeepRootLichen);
            treasure_deeproot_lichen.SetAmount(1);

            TreasureOption treasure_goblinhair_fungus = new TreasureOption();
            treasure_goblinhair_fungus.SetOdds(20);
            treasure_goblinhair_fungus.SetItemDefinition(DatabaseHelper.ItemDefinitions.Ingredient_GoblinHairFungus);
            treasure_goblinhair_fungus.SetAmount(1);

            TreasureOption treasure_scrollbless = new TreasureOption();
            treasure_scrollbless.SetOdds(1);
            treasure_scrollbless.SetItemDefinition(DatabaseHelper.ItemDefinitions.ScrollBless);
            treasure_scrollbless.SetAmount(1);

            TreasureOption treasure_scrollcurewounds = new TreasureOption();
            treasure_scrollcurewounds.SetOdds(1);
            treasure_scrollcurewounds.SetItemDefinition(DatabaseHelper.ItemDefinitions.ScrollCureWounds);
            treasure_scrollcurewounds.SetAmount(1);

            TreasureOption treasure_scrolldetectmagic = new TreasureOption();
            treasure_scrolldetectmagic.SetOdds(1);
            treasure_scrolldetectmagic.SetItemDefinition(DatabaseHelper.ItemDefinitions.ScrollDetectMagic);
            treasure_scrolldetectmagic.SetAmount(1);

            TreasureOption treasure_scrollidentify = new TreasureOption();
            treasure_scrollidentify.SetOdds(1);
            treasure_scrollidentify.SetItemDefinition(DatabaseHelper.ItemDefinitions.ScrollIdentify);
            treasure_scrollidentify.SetAmount(1);

            TreasureOption treasure_poisonbasic = new TreasureOption();
            treasure_poisonbasic.SetOdds(1);
            treasure_poisonbasic.SetItemDefinition(DatabaseHelper.ItemDefinitions.Poison_Basic);
            treasure_poisonbasic.SetAmount(1);

            TreasureOption treasure_potionremedy = new TreasureOption();
            treasure_potionremedy.SetOdds(1);
            treasure_potionremedy.SetItemDefinition(DatabaseHelper.ItemDefinitions.PotionRemedy);
            treasure_potionremedy.SetAmount(1);

            TreasureOption treasure_dagger = new TreasureOption();
            treasure_dagger.SetOdds(4);
            treasure_dagger.SetItemDefinition(DatabaseHelper.ItemDefinitions.Dagger);
            treasure_dagger.SetAmount(1);

            TreasureOption treasure_refinedoil = new TreasureOption();
            treasure_refinedoil.SetOdds(1);
            treasure_refinedoil.SetItemDefinition(DatabaseHelper.ItemDefinitions.Ingredient_RefinedOil);
            treasure_refinedoil.SetAmount(1);

            TreasureOption treasure_acid = new TreasureOption();
            treasure_acid.SetOdds(1);
            treasure_acid.SetItemDefinition(DatabaseHelper.ItemDefinitions.Ingredient_Acid);
            treasure_acid.SetAmount(1);

            TreasureOption treasure_amethyst = new TreasureOption();
            treasure_amethyst.SetOdds(1);
            treasure_amethyst.SetItemDefinition(DatabaseHelper.ItemDefinitions._20_GP_Amethyst);
            treasure_amethyst.SetAmount(1);

            TreasureTableDefinition pick_pocket_table = TreasureTableDefinitionBuilder.createCopyFrom("PickPocketTable", "79cac3e5-0f00-4062-b263-adbc854223d7", "", "", DatabaseHelper.TreasureTableDefinitions.RandomTreasureTableG_25_GP_Art_Items);
            pick_pocket_table.TreasureOptions.Add(treasure_copper);
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

            TreasureTableDefinition pick_pocket_table_undead = TreasureTableDefinitionBuilder.createCopyFrom("PickPocketTableC", "f1bbd8e5-3e05-48da-9c70-2db676a280b4", "", "", DatabaseHelper.TreasureTableDefinitions.RandomTreasureTableG_25_GP_Art_Items);
            pick_pocket_table_undead.TreasureOptions.Add(treasure_copper);
            pick_pocket_table_undead.TreasureOptions.Add(treasure_abyss_moss);
            pick_pocket_table_undead.TreasureOptions.Add(treasure_deeproot_lichen);
            pick_pocket_table_undead.TreasureOptions.Add(treasure_goblinhair_fungus);

            ItemOccurence loot_pickpocket_table = new ItemOccurence(Zombie_loot_drop.ItemOccurencesList[0]);
            loot_pickpocket_table.SetItemMode(ItemOccurence.SelectionMode.TreasureTable);
            loot_pickpocket_table.SetTreasureTableDefinition(pick_pocket_table);
            loot_pickpocket_table.SetDiceNumber(1);

            ItemOccurence loot_pickpocket_undead = new ItemOccurence(Zombie_loot_drop.ItemOccurencesList[0]);
            loot_pickpocket_undead.SetItemMode(ItemOccurence.SelectionMode.TreasureTable);
            loot_pickpocket_undead.SetTreasureTableDefinition(pick_pocket_table_undead);
            loot_pickpocket_undead.SetDiceNumber(1);

            LootPackDefinition pick_pocket_loot = LootPackDefinitionBuilder.createCopyFrom("PickPocketLoot", "30c308db-1ad7-4f93-9431-43ce32358493", "", "", Tutorial_04_Loot_Stealable);
            pick_pocket_loot.SetLootChallengeMode(LootPackDefinition.LootChallenge.ByPartyLevel);
            pick_pocket_loot.ItemOccurencesList.Clear();
            pick_pocket_loot.ItemOccurencesList.Add(loot_pickpocket_table);

            LootPackDefinition pick_pocket_undead = LootPackDefinitionBuilder.createCopyFrom("PickPocketUndead", "af2eb8e0-6a5a-40e2-8a62-160f80e2453e", "", "", Tutorial_04_Loot_Stealable);
            pick_pocket_undead.SetLootChallengeMode(LootPackDefinition.LootChallenge.ByPartyLevel);
            pick_pocket_undead.ItemOccurencesList.Clear();
            pick_pocket_undead.ItemOccurencesList.Add(loot_pickpocket_undead);

            foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
            {
                if (monster.CharacterFamily == "Humanoid" &&
                    monster.DefaultFaction == "HostileMonsters")
                {
                    monster.SetStealableLootDefinition(pick_pocket_loot);
                }
            }

            MonsterDefinition adam_12 = DatabaseHelper.MonsterDefinitions.Adam_The_Twelth;
            adam_12.SetStealableLootDefinition(pick_pocket_loot);

            MonsterDefinition brood_of_blood = DatabaseHelper.MonsterDefinitions.Brood_of_blood;
            brood_of_blood.SetStealableLootDefinition(pick_pocket_loot);

            MonsterDefinition brood_of_dread = DatabaseHelper.MonsterDefinitions.Brood_of_dread;
            brood_of_dread.SetStealableLootDefinition(pick_pocket_loot);

            MonsterDefinition brood_of_flesh = DatabaseHelper.MonsterDefinitions.Brood_of_flesh;
            brood_of_flesh.SetStealableLootDefinition(pick_pocket_loot);

            MonsterDefinition skeleton_basic = DatabaseHelper.MonsterDefinitions.Skeleton;
            skeleton_basic.SetStealableLootDefinition(pick_pocket_undead);

            MonsterDefinition skeleton_archer = DatabaseHelper.MonsterDefinitions.Skeleton_Archer;
            skeleton_archer.SetStealableLootDefinition(pick_pocket_undead);

            MonsterDefinition skeleton_enforcer = DatabaseHelper.MonsterDefinitions.Skeleton_Enforcer;
            skeleton_enforcer.SetStealableLootDefinition(pick_pocket_undead);

            MonsterDefinition skeleton_knight = DatabaseHelper.MonsterDefinitions.Skeleton_Knight;
            skeleton_knight.SetStealableLootDefinition(pick_pocket_undead);

            MonsterDefinition skeleton_marksman = DatabaseHelper.MonsterDefinitions.Skeleton_Marksman;
            skeleton_marksman.SetStealableLootDefinition(pick_pocket_undead);

            MonsterDefinition skeleton_sorcerer = DatabaseHelper.MonsterDefinitions.Skeleton_Sorcerer;
            skeleton_sorcerer.SetStealableLootDefinition(pick_pocket_undead);
        }
    }

    // TODO move complete builders to ModAPI, move reusable builders to the Features folder so they can be shared.
    public class LootPackDefinitionBuilder : BaseDefinitionBuilder<LootPackDefinition>
    {
        protected LootPackDefinitionBuilder(string name, string guid, string title_string, string description_string, LootPackDefinition base_loot) : base(base_loot, name, guid)
        {
            // ?? would these be better as !string.IsNullOr...
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }
            if (description_string != "")
            {
                Definition.GuiPresentation.Description = description_string;
            }
        }

        // TODO: should be capitalized - breaking change?
        public static LootPackDefinition createCopyFrom(string name, string guid, string new_title_string, string new_description_string, LootPackDefinition base_loot)
        {
            return new LootPackDefinitionBuilder(name, guid, new_title_string, new_description_string, base_loot).AddToDB();
        }
    }

    public class TreasureTableDefinitionBuilder : BaseDefinitionBuilder<TreasureTableDefinition>
    {
        protected TreasureTableDefinitionBuilder(string name, string guid, string title_string, string description_string, TreasureTableDefinition base_table) : base(base_table, name, guid)
        {
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }
            if (description_string != "")
            {
                Definition.GuiPresentation.Description = description_string;
            }
        }

        public static TreasureTableDefinition createCopyFrom(string name, string guid, string new_title_string, string new_description_string, TreasureTableDefinition base_table)
        {
            return new TreasureTableDefinitionBuilder(name, guid, new_title_string, new_description_string, base_table).AddToDB();
        }
    }

    public class PickPocketAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        protected PickPocketAbilityCheckAffinityBuilder(string name, string guid, string title_string, string description_string, FeatureDefinitionAbilityCheckAffinity base_check_affinity) : base(base_check_affinity, name, guid)
        {
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }
            if (description_string != "")
            {
                Definition.GuiPresentation.Description = description_string;
            }
        }
        public static FeatureDefinitionAbilityCheckAffinity CreateCopyFrom(string name, string guid, string new_title_string, string new_description_string, FeatureDefinitionAbilityCheckAffinity base_check_affinity)
        {
            return new PickPocketAbilityCheckAffinityBuilder(name, guid, new_title_string, new_description_string, base_check_affinity).AddToDB();
        }
    }

    public class PickPocketProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        protected PickPocketProficiencyBuilder(string name, string guid, string title_string, string description_string, FeatureDefinitionProficiency base_proficiency) : base(base_proficiency, name, guid)
        {
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }
            if (description_string != "")
            {
                Definition.GuiPresentation.Description = description_string;
            }
        }
        public static FeatureDefinitionProficiency CreateCopyFrom(string name, string guid, string new_title_string, string new_description_string, FeatureDefinitionProficiency base_proficiency)
        {
            return new PickPocketProficiencyBuilder(name, guid, new_title_string, new_description_string, base_proficiency).AddToDB();
        }
    }

    public class PickPocketFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        protected PickPocketFeatBuilder(string name, string guid, string title_string, string description_string, FeatDefinition base_Feat) : base(base_Feat, name, guid)
        {
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }
            if (description_string != "")
            {
                Definition.GuiPresentation.Description = description_string;
            }
        }
        public static FeatDefinition CreateCopyFrom(string name, string guid, string new_title_string, string new_description_string, FeatDefinition base_Feat)
        {
            return new PickPocketFeatBuilder(name, guid, new_title_string, new_description_string, base_Feat).AddToDB();
        }
    }
}
