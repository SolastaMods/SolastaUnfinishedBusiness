using System;
using System.Linq;
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationSystem;
using AwesomeTechnologies.VegetationSystem.Biomes;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SolastaCommunityExpansion.Models;

internal static class DmProRendererContext
{
    private const int Margin = 25;
    private const int FlatRoomSize = 12;
    private const string FlatRoomTag = "Flat";

    private static VegetationMaskArea TemplateVegetationMaskArea { get; set; }

    private static bool IsFlatRoom([NotNull] UserRoom userRoom)
    {
        return userRoom.RoomBlueprint.name.StartsWith(FlatRoomTag);
    }

    private static bool IsDynamicFlatRoom([NotNull] UserRoom userRoom)
    {
        return IsFlatRoom(userRoom) &&
               int.TryParse(userRoom.RoomBlueprint.name.Substring(FlatRoomTag.Length, 2), out _);
    }

    // ReSharper disable once UnusedMember.Global
    public static void GetTemplateVegetationMaskArea([NotNull] WorldLocation worldLocation)
    {
        var prefabByReference =
            worldLocation.prefabByReference;

        foreach (var prefab in prefabByReference.Values)
        {
            TemplateVegetationMaskArea = prefab.GetComponentInChildren<VegetationMaskArea>();

            if (TemplateVegetationMaskArea != null)
            {
                break;
            }
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static void SetupLocationTerrain([NotNull] WorldLocation worldLocation, UserLocation userLocation)
    {
        var masterTerrain = worldLocation.gameObject.GetComponentInChildren<Terrain>();

        if (masterTerrain == null)
        {
            return;
        }

        // calculates inverted heights in map coordinates
        var locationSize = UserLocationDefinitions.CellsBySize[userLocation.Size];
        var mapHeights = new int[locationSize + (Margin * 2), locationSize + (Margin * 2)];

        foreach (var userRoom in userLocation.UserRooms)
        {
            var isIndoor = !DmProEditorContext.OutdoorRooms.Contains(userRoom.RoomBlueprint.name);
            const int BORDER = 2;
            var px = userRoom.Position.x;
            var py = userRoom.Position.y;
            var oh = userRoom.OrientedHeight;
            var ow = userRoom.OrientedWidth;

            for (var x = 0; x < ow; x++)
            {
                for (var y = 0; y < oh; y++)
                {
                    var lowGround = isIndoor &&
                                    ((x >= BORDER && x <= ow - BORDER && y >= BORDER && y <= oh - BORDER) ||
                                     userRoom.GetCellType(x, y) == RoomBlueprint.CellType.GroundLow);

                    mapHeights[Margin + px + x, Margin + py + y] = lowGround ? 1 : 0;
                }
            }
        }

        // calculates heights in terrain data coordinates
        var resolution = masterTerrain.terrainData.heightmapResolution;
        var heights = new float[resolution, resolution];

        for (var y = 0; y < resolution; y++)
        {
            for (var x = 0; x < resolution; x++)
            {
                var sx = (int)Math.Round(x * (locationSize + (Margin * 2f) - 1f) / (resolution - 1f));
                var sy = (int)Math.Round(y * (locationSize + (Margin * 2f) - 1f) / (resolution - 1f));

                heights[y, x] = 1 - mapHeights[sx, sy];
            }
        }

        // adjusts terrain to new settings
        masterTerrain.terrainData = TerrainDataCloner.Clone(masterTerrain.terrainData);

        var transformPosition = masterTerrain.transform.position;
        var terrainData = masterTerrain.terrainData;

        terrainData.size =
            new Vector3(locationSize + (Margin * 2f), 5f, locationSize + (Margin * 2f));
        terrainData.SetHeights(0, 0, heights);
        masterTerrain.transform.position = new Vector3(transformPosition.x, -5.01f, transformPosition.z);

        worldLocation.duplicatedTerrainData
            .Add(masterTerrain.terrainData);

        // updates the biome to cover the entire location
        var biomeMaskArea = worldLocation.gameObject.GetComponentInChildren<BiomeMaskArea>();

        if (biomeMaskArea == null)
        {
            return;
        }

        biomeMaskArea.ClearNodes();
        biomeMaskArea.AddNode(
            biomeMaskArea.transform.InverseTransformDirection(new Vector3(-Margin, 0, locationSize + Margin)));
        biomeMaskArea.AddNode(biomeMaskArea.transform.InverseTransformDirection(new Vector3(-Margin, 0, -Margin)));
        biomeMaskArea.AddNode(
            biomeMaskArea.transform.InverseTransformDirection(new Vector3(locationSize + Margin, 0, -Margin)));
        biomeMaskArea.AddNode(
            biomeMaskArea.transform.InverseTransformDirection(new Vector3(locationSize + Margin, 0,
                locationSize + Margin)));
        biomeMaskArea.UpdateBiomeMask();
        worldLocation.gameObject.GetComponentInChildren<VegetationSystemPro>()?.CalculateVegetationSystemBounds();
    }

    // ReSharper disable once UnusedMember.Global
    public static void SetupFlatRooms(Transform roomTransform, [NotNull] UserRoom userRoom)
    {
        static void DisableWalls([NotNull] Transform transform)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                DisableWalls(transform.GetChild(i));
            }

            var name = transform.gameObject.name;

            if ((!name.Contains("Wall") || name.Contains("Drain")) && !name.Contains("Column") &&
                !name.Contains("DM_Dirt_Pack"))
            {
                return;
            }

            // need to keep parents around otherwise pure flat locations don't render correctly
            if (transform.childCount > 0)
            {
                transform.position = new Vector3(-1f, 0f, -1f);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        if (!IsFlatRoom(userRoom))
        {
            return;
        }

        DisableWalls(roomTransform);

        if (!int.TryParse(userRoom.RoomBlueprint.name.Substring(FlatRoomTag.Length, 2), out var multiplier))
        {
            return;
        }

        var rnd = new Random();
        var position = roomTransform.position;
        var moveBy = (multiplier - 1) * FlatRoomSize / 2;
        var newPosition = new Vector3(position.x - moveBy, 0, position.z - moveBy);

        roomTransform.position = newPosition;

        for (var x = 0; x < multiplier; x++)
        {
            for (var z = 0; z < multiplier; z++)
            {
                if (x <= 0 && z <= 0)
                {
                    continue;
                }

                // placing textures using a random angle to remove the repetition feeling a bit
                var angle = LocationDefinitions.OrientationToAngle(
                    (LocationDefinitions.Orientation)rnd.Next(0, 3));
                var newRoom = Object.Instantiate(roomTransform.gameObject,
                    new Vector3(newPosition.x + (FlatRoomSize * x), 0,
                        newPosition.z + (FlatRoomSize * z)), Quaternion.identity,
                    roomTransform.parent);

                newRoom.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        // adds a hint to fix the reflection probe later on
        roomTransform.name = FlatRoomTag + roomTransform.name;
    }

    // ReSharper disable once UnusedMember.Global
    public static void AddVegetationMaskArea(Transform roomTransform, UserRoom userRoom)
    {
        if (TemplateVegetationMaskArea == null ||
            DmProEditorContext.OutdoorRooms.Contains(userRoom.RoomBlueprint.name))
        {
            return;
        }

        var vegetationMaskArea = Object.Instantiate(TemplateVegetationMaskArea, roomTransform);
        var sizex = userRoom.OrientedWidth;
        var sizey = userRoom.OrientedHeight;

        // exceptional case here as the mask must be a constant size in this case as the vegetation mask object will get re-instantiated later
        if (IsDynamicFlatRoom(userRoom))
        {
            sizex = FlatRoomSize;
            sizey = FlatRoomSize;
        }

        vegetationMaskArea.transform.position = new Vector3(userRoom.Position.x + (sizex / 2f), 0,
            userRoom.Position.y + (sizey / 2f));
        vegetationMaskArea.AdditionalGrassPerimiter = 0;
        vegetationMaskArea.RemoveGrass = true;
        vegetationMaskArea.RemoveLargeObjects = true;
        vegetationMaskArea.RemoveObjects = true;
        vegetationMaskArea.RemovePlants = true;
        vegetationMaskArea.RemoveTrees = true;
        vegetationMaskArea.ClearNodes();
        vegetationMaskArea.AddNode(new Vector3(-sizex / 2f, 0, -sizey / 2f));
        vegetationMaskArea.AddNode(new Vector3(-sizex / 2f, 0, +sizey / 2f));
        vegetationMaskArea.AddNode(new Vector3(+sizex / 2f, 0, +sizey / 2f));
        vegetationMaskArea.AddNode(new Vector3(+sizex / 2f, 0, -sizey / 2f));
        vegetationMaskArea.UpdateVegetationMask();
    }

    // ReSharper disable once UnusedMember.Global
    public static void FixFlatRoomReflectionProbe([NotNull] WorldLocation worldLocation)
    {
        var reflectionProbes = worldLocation.GetComponentsInChildren<ReflectionProbe>();

        foreach (var reflectionProbe in reflectionProbes.Where(x =>
                     x.transform.parent.name.StartsWith(FlatRoomTag)))
        {
            var size = reflectionProbe.size;

            reflectionProbe.transform.position = new Vector3(size.x / 2f, size.y, size.z / 2f);
        }
    }
}
