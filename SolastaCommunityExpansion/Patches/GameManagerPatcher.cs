using HarmonyLib;
using SolastaCommunityExpansion.Models;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameManager_BindPostDatabase
    {
        internal static void Postfix()
        {
            BugFixContext.Load();

            AdditionalNamesContext.Load();
            AsiAndFeatContext.Load();
            InitialChoicesContext.Load();
            ItemCraftingContext.Load();
            GameUiContext.Load();
            // Fighting Styles should be loaded before feats in
            // order to generate feats of new fighting styles.
            FightingStyleContext.Load();
            FeatsContext.Load();
            SubclassesContext.Load();
            FlexibleBackgroundsContext.Load();
            FlexibleRacesContext.Load();
            VisionContext.Load();
            PickPocketContext.Load();
            EpicArrayContext.Load();
            RespecContext.Load();
            RemoveIdentificationContext.Load();
            Level20Context.Load();
            DruidArmorContext.Load();
            CharacterExportContext.Load();
            InventoryManagementContext.Load();
            RemoveBugVisualModelsContext.Load();
            FaceUnlockContext.Load();
            ConjurationsContext.Load();
            ItemOptionsContext.Load();
            DungeonMakerContext.Load();
            TelemaCampaignContext.Load();
            EncountersSpawnContext.Load();
            SrdAndHouseRulesContext.Load();

            Main.Enabled = true;
        }
    }
    //[HarmonyPatch(typeof(GameLocationScreenMap), "BindGadgets")]
    //[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    //internal static class Test
    //{
    //    internal static void Prefix()
    //    {
    //        foreach (GameSector gameSector in Gui.GameLocation.GameSectors)
    //        {
    //            foreach (GameGadget gameGadget in gameSector.GameGadgets)
    //            {
    //                Main.Log($"{gameGadget.UniqueNameId}, {gameGadget.Revealed}");

    //                if (gameGadget.Revealed && gameGadget.CheckIsEnabled())
    //                {


    //                    MapGadgetItem.ItemType itemType;
    //                    if (gameGadget.CheckIsLocked())
    //                    {
    //                        itemType = MapGadgetItem.ItemType.Lock;
    //                    }
    //                    else if (gameGadget.CheckHasActiveDetectedTrap())
    //                    {
    //                        itemType = MapGadgetItem.ItemType.Trap;
    //                    }
    //                    else if (gameGadget.ItemContainer != null)
    //                    {
    //                        itemType = MapGadgetItem.ItemType.Container;
    //                    }
    //                    else
    //                    {
    //                        continue;
    //                    }

    //                    //++this.activeMapGadgetItems;
    //                    //for (int index = this.mapGadgetItems.Count - 1; index < this.activeMapGadgetItems; ++index)
    //                    //{
    //                    //    GameObject gameObject = Object.Instantiate<GameObject>(this.mapGadgetItemPrefab, this.mapItemsTransform);
    //                    //    MapGadgetItem mapGadgetItem = (MapGadgetItem)null;
    //                    //    ref MapGadgetItem local = ref mapGadgetItem;
    //                    //    gameObject.TryGetComponent<MapGadgetItem>(out local);
    //                    //    mapGadgetItem.Unbind();
    //                    //    this.mapGadgetItems.Add(mapGadgetItem);
    //                    //}
    //                    //this.mapGadgetItems[this.activeMapGadgetItems - 1].Bind(gameGadget, itemType);
    //                }
    //            }
    //        }
    //        //for (int index = 0; index < this.activeMapGadgetItems; ++index)
    //        //    this.sortedItems.Add((MapBaseItem)this.mapGadgetItems[index]);

    //    }
    //}
}
