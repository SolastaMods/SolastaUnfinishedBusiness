using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Builders.DefinitionBuilder;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class DmProEditorContext
{
    private const string BackupFolder = "../UserContentBackup";
    internal static readonly List<string> OutdoorRooms = [];

    internal static void Load()
    {
        UpdateAvailableDungeonSizes();
        UpdateCategories();
        LoadFlatRooms();
        UnleashGadgetsOnAllEnvironments();
        UnleashPropsOnAllEnvironments();
        UnleashRoomsOnAllEnvironments();
    }

    internal static void BackupAndDelete([NotNull] string path, [NotNull] UserContent userContent)
    {
        const int MaxBackupFilesPerLocationCampaign = 20;

        var backupDirectory = Path.Combine(Main.ModFolder, BackupFolder);

        Directory.CreateDirectory(backupDirectory);

        var title = userContent.Title;
        var compliantTitle = IOHelper.GetOsCompliantFilename(title);
        var destinationPath =
            Path.Combine(backupDirectory, compliantTitle) + "." + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        var backupFiles = Directory
            .EnumerateFiles(backupDirectory, compliantTitle + "*")
            .OrderBy(f => f)
            .ToList();

        for (var i = 0; i <= backupFiles.Count - MaxBackupFilesPerLocationCampaign; i++)
        {
            File.Delete(backupFiles[i]);
        }

        File.Copy(path, destinationPath);
        File.Delete(path);
    }

    internal static void AddCustomDungeonSizes(UserLocationSettingsModal __instance)
    {
        for (var size = ExtendedDungeonSize.Huge; size <= ExtendedDungeonSize.Gargantuan; size++)
        {
            var sizeString = UserLocationDefinitions.CellsBySize[(UserLocationDefinitions.Size)size].ToString();

            __instance.optionsListSize.Add(new GuiDropdown.OptionDataAdvanced
            {
                text = Gui.FormatLocationSize((UserLocationDefinitions.Size)size).Khaki() + " " +
                       Gui.Format("{0} x {1}", sizeString, sizeString),
                TooltipContent = string.Empty
            });
        }
    }

    internal static int Compare([NotNull] BaseBlueprint left, [NotNull] BaseBlueprint right)
    {
        var leftCategory = DatabaseHelper.GetDefinition<BlueprintCategory>(left.Category);
        var rightCategory = DatabaseHelper.GetDefinition<BlueprintCategory>(right.Category);
        var result = String.Compare(leftCategory.FormatTitle(), rightCategory.FormatTitle(),
            StringComparison.CurrentCultureIgnoreCase);

        if (result != 0)
        {
            return result;
        }

        result = String.Compare(left.name, right.name, StringComparison.CurrentCultureIgnoreCase);

        if (result == 0)
        {
            result = left.GuiPresentation.SortOrder - right.GuiPresentation.SortOrder;
        }

        return result;
    }

    private static void UpdateAvailableDungeonSizes()
    {
        var customDungeonSizes = new Dictionary<UserLocationDefinitions.Size, int>
        {
            { (UserLocationDefinitions.Size)ExtendedDungeonSize.Huge, 150 },
            { (UserLocationDefinitions.Size)ExtendedDungeonSize.Gargantuan, 200 }
        };

        UserLocationDefinitions.CellsBySize[UserLocationDefinitions.Size.Medium] = 75;

        foreach (var kvp in customDungeonSizes)
        {
            UserLocationDefinitions.CellsBySize.Add(kvp.Key, kvp.Value);
        }
    }

    private static void UpdateCategories()
    {
        var categories = new List<BlueprintCategory>();
        var dbBlueprintCategory = DatabaseRepository.GetDatabase<BlueprintCategory>();
        var dbEnvironmentDefinition = DatabaseRepository.GetDatabase<EnvironmentDefinition>();
        var emptyRoomsCategory = dbBlueprintCategory.GetElement("EmptyRooms");
        var flatRoomsCategory = Object.Instantiate(emptyRoomsCategory);

        flatRoomsCategory.name = "FlatRooms";
        flatRoomsCategory.guid = GuidHelper.Create(CeNamespaceGuid, flatRoomsCategory.name).ToString();
        flatRoomsCategory.GuiPresentation.Title =
            Gui.Localize($"BlueprintCategory/&{flatRoomsCategory.name}Title").Khaki();
        dbBlueprintCategory.Add(flatRoomsCategory);

        foreach (var blueprintCategory in dbBlueprintCategory)
        {
            foreach (var environmentDefinition in dbEnvironmentDefinition)
            {
                var newBlueprintCategory = Object.Instantiate(blueprintCategory);
                var environmentName = environmentDefinition.Name;
                var categoryName = blueprintCategory.Name + "~" + environmentName + "~MOD";

                newBlueprintCategory.name = categoryName;
                newBlueprintCategory.guid = GuidHelper.Create(CeNamespaceGuid, newBlueprintCategory.name).ToString();
                newBlueprintCategory.GuiPresentation.Title = Gui.Localize(blueprintCategory.GuiPresentation.Title) +
                                                             " " + Gui.Localize(environmentDefinition.GuiPresentation
                                                                 .Title) + " [MODDED]".Khaki();
                categories.Add(newBlueprintCategory);
            }
        }

        categories.ForEach(x => dbBlueprintCategory.Add(x));
    }

    private static void LoadFlatRooms()
    {
        if (!Main.Settings.EnableDungeonMakerModdedContent)
        {
            return;
        }

        CreateFlatRooms(12); // from 12x12 to 144x144
        CreateSpecialFlatRoom("Drain_Big_24C_A", 12 + 1);
        CreateSpecialFlatRoom("Drain_Big_24C_B", 12 + 2);
    }

    private static void CreateFlatRooms(int maxMultiplier)
    {
        const string TEMPLATE = "Crossroad_12C";
        var dbRoomBlueprint = DatabaseRepository.GetDatabase<RoomBlueprint>();

        for (var multiplier = 1; multiplier <= maxMultiplier; multiplier++)
        {
            var flatRoom = Object.Instantiate(dbRoomBlueprint.GetElement(TEMPLATE));

            flatRoom.name = $"Flat{multiplier:D2}Room";
            flatRoom.guid = GuidHelper.Create(CeNamespaceGuid, flatRoom.name).ToString();
            flatRoom.GuiPresentation.title = "Flat".Khaki() + " Room";
            flatRoom.GuiPresentation.sortOrder = multiplier;
            flatRoom.GuiPresentation.hidden = true;
            flatRoom.category = "FlatRooms";
            flatRoom.dimensions = new Vector2Int(
                flatRoom.Dimensions.x * multiplier, flatRoom.Dimensions.y * multiplier);
            flatRoom.cellInfos = new int[flatRoom.Dimensions.x * flatRoom.Dimensions.y];
            flatRoom.wallSpriteReference = new AssetReferenceSprite("");
            flatRoom.wallAndOpeningSpriteReference = new AssetReferenceSprite("");

            for (var i = 0; i < flatRoom.CellInfos.Length; i++)
            {
                flatRoom.CellInfos[i] = 1;
            }

            dbRoomBlueprint.Add(flatRoom);
        }
    }

    private static void CreateSpecialFlatRoom(string template, int sortOrder)
    {
        var dbRoomBlueprint = DatabaseRepository.GetDatabase<RoomBlueprint>();
        var flatRoom = Object.Instantiate(dbRoomBlueprint.GetElement(template));

        flatRoom.name = "Flat" + template;
        flatRoom.guid = GuidHelper.Create(CeNamespaceGuid, flatRoom.name).ToString();
        flatRoom.GuiPresentation.title = "Flat".Khaki() + " " + Gui.Localize(flatRoom.GuiPresentation.Title);
        flatRoom.GuiPresentation.sortOrder = sortOrder;
        flatRoom.GuiPresentation.hidden = true;
        flatRoom.category = "FlatRooms";

        for (var i = 0; i < flatRoom.CellInfos.Length; i++)
        {
            if (flatRoom.CellInfos[i] == 2 || flatRoom.CellInfos[i] == 3)
            {
                flatRoom.CellInfos[i] = 1;
            }
        }

        dbRoomBlueprint.Add(flatRoom);
    }

    private static void UnleashGadgetsOnAllEnvironments()
    {
        var dbGadgetBlueprint = DatabaseRepository.GetDatabase<GadgetBlueprint>();
        var dbEnvironmentDefinition = DatabaseRepository.GetDatabase<EnvironmentDefinition>();
        var newGadgets = new List<GadgetBlueprint>();

        foreach (var gadgetBlueprint in dbGadgetBlueprint)
        {
            foreach (var prefabByEnvironment in gadgetBlueprint.PrefabsByEnvironment.Where(x =>
                         x.Environment != string.Empty))
            {
                var environmentName = prefabByEnvironment.Environment;
                var prefabEnvironmentDefinition = dbEnvironmentDefinition.GetElement(environmentName);
                var newGadgetBlueprint = Object.Instantiate(gadgetBlueprint);
                var categoryName = gadgetBlueprint.Category + "~" + environmentName + "~MOD";

                newGadgetBlueprint.name = gadgetBlueprint.Name + "~" + environmentName + "~MOD";
                newGadgetBlueprint.guid = GuidHelper.Create(CeNamespaceGuid, newGadgetBlueprint.name).ToString();
                newGadgetBlueprint.GuiPresentation.Title = Gui.Localize(gadgetBlueprint.GuiPresentation.Title) + " " +
                                                           Gui.Localize(prefabEnvironmentDefinition.GuiPresentation
                                                               .Title).Khaki();
                newGadgetBlueprint.category = categoryName;
                newGadgetBlueprint.PrefabsByEnvironment.Clear();

                foreach (var environmentDefinition in dbEnvironmentDefinition)
                {
                    var myPrefabByEnvironment = new BaseBlueprint.PrefabByEnvironmentDescription
                    {
                        environment = environmentDefinition.name,
                        prefabReference = prefabByEnvironment.PrefabReference
                    };

                    newGadgetBlueprint.PrefabsByEnvironment.Add(myPrefabByEnvironment);
                }

                newGadgets.TryAdd(newGadgetBlueprint);
            }
        }

        newGadgets.ForEach(x => dbGadgetBlueprint.Add(x));
        SetModdedGadgetsHiddenStatus();
    }

    private static void UnleashPropsOnAllEnvironments()
    {
        var dbPropBlueprint = DatabaseRepository.GetDatabase<PropBlueprint>();
        var dbEnvironmentDefinition = DatabaseRepository.GetDatabase<EnvironmentDefinition>();
        var newProps = new List<PropBlueprint>();

        foreach (var propBlueprint in dbPropBlueprint)
        {
            foreach (var prefabByEnvironment in propBlueprint.PrefabsByEnvironment.Where(x =>
                         x.Environment != string.Empty))
            {
                var environmentName = prefabByEnvironment.Environment;
                var prefabEnvironmentDefinition = dbEnvironmentDefinition.GetElement(environmentName);
                var newPropBlueprint = Object.Instantiate(propBlueprint);
                var categoryName = propBlueprint.Category + "~" + environmentName + "~MOD";

                newPropBlueprint.name = propBlueprint.Name + "~" + environmentName + "~MOD";
                newPropBlueprint.guid = GuidHelper.Create(CeNamespaceGuid, newPropBlueprint.name).ToString();
                newPropBlueprint.GuiPresentation.Title = Gui.Localize(propBlueprint.GuiPresentation.Title) + " " +
                                                         Gui.Localize(prefabEnvironmentDefinition.GuiPresentation
                                                             .Title).Khaki();
                newPropBlueprint.category = categoryName;
                newPropBlueprint.PrefabsByEnvironment.Clear();

                foreach (var environmentDefinition in dbEnvironmentDefinition)
                {
                    var myPrefabByEnvironment = new BaseBlueprint.PrefabByEnvironmentDescription
                    {
                        environment = environmentDefinition.name,
                        prefabReference = prefabByEnvironment.PrefabReference
                    };

                    newPropBlueprint.PrefabsByEnvironment.Add(myPrefabByEnvironment);
                }

                newProps.TryAdd(newPropBlueprint);
            }
        }

        newProps.ForEach(x => dbPropBlueprint.Add(x));
        SetModdedPropsHiddenStatus();
    }

    private static void UnleashRoomsOnAllEnvironments()
    {
        var dbRoomBlueprint = DatabaseRepository.GetDatabase<RoomBlueprint>();
        var dbEnvironmentDefinition = DatabaseRepository.GetDatabase<EnvironmentDefinition>();
        var newRooms = new List<RoomBlueprint>();

        foreach (var roomBlueprint in dbRoomBlueprint
                     .Where(x => x.Category is "EmptyRooms" or "FlatRooms"))
        {
            foreach (var prefabByEnvironment in roomBlueprint.PrefabsByEnvironment
                         .Where(x => x.Environment != string.Empty))
            {
                var environmentName = prefabByEnvironment.Environment;
                var prefabEnvironmentDefinition = dbEnvironmentDefinition.GetElement(environmentName);
                var newRoomBlueprint = Object.Instantiate(roomBlueprint);
                var categoryName = roomBlueprint.Category + "~" + environmentName + "~MOD";

                newRoomBlueprint.name = roomBlueprint.Name + "~" + environmentName + "~MOD";
                newRoomBlueprint.guid = GuidHelper.Create(CeNamespaceGuid, newRoomBlueprint.name).ToString();
                newRoomBlueprint.GuiPresentation.Title = Gui.Localize(roomBlueprint.GuiPresentation.Title) + " " +
                                                         Gui.Localize(prefabEnvironmentDefinition.GuiPresentation
                                                             .Title).Khaki();
                newRoomBlueprint.category = categoryName;
                newRoomBlueprint.GuiPresentation.hidden = false;
                newRoomBlueprint.PrefabsByEnvironment.Clear();

                foreach (var environmentDefinition in dbEnvironmentDefinition
                             .Where(x => !prefabEnvironmentDefinition.Outdoor || x.Outdoor))
                {
                    var myPrefabByEnvironment = new BaseBlueprint.PrefabByEnvironmentDescription
                    {
                        environment = environmentDefinition.name,
                        prefabReference = prefabByEnvironment.PrefabReference
                    };

                    newRoomBlueprint.PrefabsByEnvironment.Add(myPrefabByEnvironment);
                }

                newRooms.Add(newRoomBlueprint);

                if (!prefabEnvironmentDefinition.Outdoor)
                {
                    continue;
                }

                OutdoorRooms.Add(newRoomBlueprint.Name);

                if (!OutdoorRooms.Contains(roomBlueprint.Name))
                {
                    OutdoorRooms.Add(roomBlueprint.Name);
                }
            }
        }

        newRooms.ForEach(x => dbRoomBlueprint.Add(x));
        SetModdedRoomsHiddenStatus();
    }

    private static void SetModdedGadgetsHiddenStatus()
    {
        var dbGadgetBlueprint = DatabaseRepository.GetDatabase<GadgetBlueprint>();

        dbGadgetBlueprint
            .Where(x => x.Name.EndsWith("MOD")).ToList()
            .ForEach(y => y.GuiPresentation.hidden = !Main.Settings.EnableDungeonMakerModdedContent);
    }

    private static void SetModdedPropsHiddenStatus()
    {
        var dbPropBlueprint = DatabaseRepository.GetDatabase<PropBlueprint>();

        dbPropBlueprint
            .Where(x => x.Name.EndsWith("MOD")).ToList()
            .ForEach(y => y.GuiPresentation.hidden = !Main.Settings.EnableDungeonMakerModdedContent);
    }

    private static void SetModdedRoomsHiddenStatus()
    {
        var dbRoomBlueprint = DatabaseRepository.GetDatabase<RoomBlueprint>();

        dbRoomBlueprint
            .Where(x => x.Name.EndsWith("MOD")).ToList()
            .ForEach(y => y.GuiPresentation.hidden = !Main.Settings.EnableDungeonMakerModdedContent);

        dbRoomBlueprint
            .Where(x => x.Name.StartsWith("Flat")).ToList()
            .ForEach(y => y.GuiPresentation.hidden = !Main.Settings.EnableDungeonMakerModdedContent);
    }

    private enum ExtendedDungeonSize
    {
        Huge = UserLocationDefinitions.Size.Large + 1,
        Gargantuan
    }
}
