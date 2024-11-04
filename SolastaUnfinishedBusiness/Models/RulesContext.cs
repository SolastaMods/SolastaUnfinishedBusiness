using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Races;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MorphotypeElementDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Models;

internal static class RulesContext
{
    internal const int GameMaxAttribute = 15;
    internal const int GameBuyPoints = 27;
    internal const int ModMaxAttribute = 17;
    internal const int ModBuyPoints = 35;

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolPathClawDraconicChoice =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolPathClawDraconicChoice")
            .SetGuiPresentation(FeatureSetPathClawDragonAncestry.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.PathClawDraconicChoice)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolPathOfTheElementsElementalFuryChoice =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolPathOfTheElementsElementalFuryChoice")
            .SetGuiPresentation(PathOfTheElements.FeatureSetElementalFury.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.PathOfTheElementsElementalFuryChoiceChoice)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolSorcererDraconicChoice =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolSorcererDraconicChoice")
            .SetGuiPresentation(FeatureSetSorcererDraconicChoice.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.SorcererDraconicChoice)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolKindredSpiritChoice =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolKindredSpiritChoice")
            .SetGuiPresentation(FeatureSetKindredSpiritChoice.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.KindredSpiritChoice)
            .AddToDB();

    internal static readonly FeatureDefinitionCustomInvocationPool InvocationPoolRangerTerrainType =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolRangerTerrainType")
            .SetGuiPresentation(TerrainTypeAffinityRangerNaturalExplorerChoice.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.RangerTerrainTypeAffinity)
            .AddToDB();

    internal static readonly FeatureDefinitionCustomInvocationPool InvocationPoolRangerPreferredEnemy =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolRangerPreferredEnemy")
            .SetGuiPresentation(AdditionalDamageRangerFavoredEnemyChoice.GuiPresentation)
            .Setup(InvocationPoolTypeCustom.Pools.RangerPreferredEnemy)
            .AddToDB();

    internal static readonly FeatureDefinitionPower PowerTeleportSummon = FeatureDefinitionPowerBuilder
        .Create("PowerTeleportSummon")
        .SetGuiPresentation(Category.Feature, DimensionDoor)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                        .Build())
                .UseQuickAnimations()
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.NotInCombat, new FilterTargetingPositionPowerTeleportSummon())
        .AddToDB();

    internal static readonly FeatureDefinitionPower PowerVanishSummon = FeatureDefinitionPowerBuilder
        .Create("PowerVanishSummon")
        .SetGuiPresentation(Category.Feature, PowerSorcererCreateSpellSlot)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetKillForm(KillCondition.Always)
                        .Build())
                .UseQuickAnimations()
                .Build())
        .AddToDB();

    private static readonly List<MonsterDefinition> MonstersThatEmitLight =
    [
        CubeOfLight,
        Fire_Elemental,
        Fire_Jester,
        Fire_Osprey,
        Fire_Spider
    ];

    internal static void LateLoad()
    {
        LoadAdditionalNames();
        LoadEpicArray();
        LoadSenseNormalVisionRangeMultiplier();
        LoadVisuals();
        LoadSorcererQuickened();
        SwitchDruidKindredBeastToUseCustomInvocationPools();
        SwitchPathOfTheElementsElementalFuryToUseCustomInvocationPools();
        SwitchRangerToUseCustomInvocationPools();
        SwitchSubclassAncestriesToUseCustomInvocationPools(
            "PathClaw", PathClaw,
            FeatureSetPathClawDragonAncestry, InvocationPoolPathClawDraconicChoice,
            InvocationPoolTypeCustom.Pools.PathClawDraconicChoice);
        SwitchSubclassAncestriesToUseCustomInvocationPools(
            "Sorcerer", SorcerousDraconicBloodline,
            FeatureSetSorcererDraconicChoice, InvocationPoolSorcererDraconicChoice,
            InvocationPoolTypeCustom.Pools.SorcererDraconicChoice);
    }

    private static void LoadSenseNormalVisionRangeMultiplier()
    {
        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision24")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision24")
                .SetSense(SenseMode.Type.NormalVision, 24)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision48")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision48")
                .SetSense(SenseMode.Type.NormalVision, 48)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();
    }

    private static void AddNameToRace(CharacterRaceDefinition raceDefinition, string gender, string name)
    {
        var racePresentation = raceDefinition.RacePresentation;

        switch (gender)
        {
            case "Male":
                racePresentation.MaleNameOptions.Add(name);
                break;

            case "Female":
                racePresentation.FemaleNameOptions.Add(name);
                break;

            case "Sur":
                racePresentation.SurNameOptions.Add(name);
                break;
        }
    }

    private static void LoadAdditionalNames()
    {
        char[] separator = ['\t'];

        if (!SettingsContext.GuiModManagerInstance.UnlockAdditionalLoreFriendlyNames)
        {
            return;
        }

        var payload = Resources.Names;
        var lines = new List<string>(payload.Split([Environment.NewLine], StringSplitOptions.None));

        foreach (var line in lines)
        {
            var columns = line.Split(separator, 3);

            if (columns.Length != 3)
            {
                Main.Error($"additional names cannot load: {line}.");

                continue;
            }

            var raceName = columns[0];
            var gender = columns[1];
            var name = columns[2];

            if (DatabaseRepository.GetDatabase<CharacterRaceDefinition>().TryGetElement(raceName, out var race))
            {
                if (race.subRaces.Count == 0)
                {
                    AddNameToRace(race, gender, name);
                }
                else
                {
                    foreach (var subRace in race.SubRaces)
                    {
                        AddNameToRace(subRace, gender, name);
                    }
                }
            }
            else
            {
                Main.Error($"additional names cannot load: {line}.");
            }
        }
    }

    private static void LoadEpicArray()
    {
        AttributeDefinitions.PredeterminedRollScores = Main.Settings.EnableEpicPointsAndArray
            ? [17, 15, 13, 12, 10, 8]
            : [15, 14, 13, 12, 10, 8];
    }


    private static void LoadSorcererQuickened()
    {
        _ = ActionDefinitionBuilder
            .Create(CastBonus, "CastQuickened")
            .SetGuiPresentation(
                "Rules/&MetamagicOptionQuickenedSpellTitle", "Action/&CastQuickenedDescription", CastMain)
            .SetSortOrder(CastBonus)
            .SetActionId(ExtraActionId.CastQuickened)
            .AddToDB();

        //leaving for compatibility?
        //needed for characters who saved while affected by this
        _ = ConditionDefinitionBuilder
            .Create("ConditionSorcererQuickenedCastMain")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();
    }


    private static void LoadVisuals()
    {
        var dbMorphotypeElementDefinition = DatabaseRepository.GetDatabase<MorphotypeElementDefinition>();

        if (SettingsContext.GuiModManagerInstance.UnlockSkinColors)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(
                         x => x.Category == MorphotypeElementDefinition.ElementCategory.Skin &&
                              x != FaceAndSkin_12 &&
                              x != FaceAndSkin_13 &&
                              x != FaceAndSkin_14 &&
                              x != FaceAndSkin_15 &&
                              x != FaceAndSkin_16 &&
                              x != FaceAndSkin_17 &&
                              x != FaceAndSkin_18))
            {
                morphotype.playerSelectable = true;
                morphotype.originAllowed = EyeColor_001.OriginAllowed;
                if (morphotype.Name.Contains("Dragonborn"))
                {
                    morphotype.GuiPresentation.sortOrder = morphotype.GuiPresentation.SortOrder + 54;
                }
            }
        }

        if (SettingsContext.GuiModManagerInstance.UnlockGlowingColorsForAllMarksAndTattoos)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(
                         x => x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecorationColor &&
                              x.SubclassFilterMask == GraphicsDefinitions.MorphotypeSubclassFilterTag
                                  .SorcererManaPainter))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        var brightEyes = new List<MorphotypeElementDefinition>();

        Morphotypes.CreateBrightEyes(brightEyes);

        if (!SettingsContext.GuiModManagerInstance.UnlockNewBrightEyeColors)
        {
            foreach (var morphotype in brightEyes)
            {
                morphotype.playerSelectable = false;
            }
        }

        var glowingEyes = new List<MorphotypeElementDefinition>();

        Morphotypes.CreateGlowingEyes(glowingEyes);

        if (!SettingsContext.GuiModManagerInstance.UnlockGlowingEyeColors)
        {
            foreach (var morphotype in glowingEyes)
            {
                morphotype.playerSelectable = false;
            }
        }

        if (SettingsContext.GuiModManagerInstance.UnlockEyeStyles)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.Eye))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
                morphotype.originAllowed = EyeColor_001.OriginAllowed;
            }

            var races = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();

            foreach (var race in races)
            {
                var presentation = race.racePresentation;

                if (presentation.IsAvailable(MorphotypeElementDefinition.ElementCategory.Eye))
                {
                    continue;
                }

                var all = new List<MorphotypeElementDefinition.ElementCategory>(
                    presentation.availableMorphotypeCategories) { MorphotypeElementDefinition.ElementCategory.Eye };

                presentation.availableMorphotypeCategories = all.ToArray();
            }
        }

        if (SettingsContext.GuiModManagerInstance.UnlockAllNpcFaces)
        {
            HalfElf.RacePresentation.FemaleFaceShapeOptions.Add("FaceShape_NPC_Princess");
            HalfElf.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_HalfElf_NPC_Bartender");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TavernGuy");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TomWorker");

            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.FaceShape &&
                         x != FaceShape_NPC_Aksha))
            {
                morphotype.playerSelectable = true;
            }
        }

        if (SettingsContext.GuiModManagerInstance.UnlockBeardlessDwarves)
        {
            Dwarf.RacePresentation.needBeard = false;
            DwarfHill.RacePresentation.needBeard = false;
            DwarfSnow.RacePresentation.needBeard = false;
            SubraceGrayDwarfBuilder.SubraceGrayDwarf.RacePresentation.needBeard = false;
            SubraceIronbornDwarfBuilder.SubraceIronbornDwarf.RacePresentation.needBeard = false;
            SubraceObsidianDwarfBuilder.SubraceObsidianDwarf.RacePresentation.needBeard = false;
            Dwarf.RacePresentation.MaleBeardShapeOptions.Add(BeardShape_None.Name);
        }

        if (SettingsContext.GuiModManagerInstance.UnlockMarkAndTattoosForAllCharacters)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecoration))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        if (!SettingsContext.GuiModManagerInstance.UnlockUnmarkedSorcerers)
        {
            return;
        }

        SorcerousDraconicBloodline.morphotypeSubclassFilterTag = GraphicsDefinitions.MorphotypeSubclassFilterTag
            .Default;
        SorcerousManaPainter.morphotypeSubclassFilterTag = GraphicsDefinitions.MorphotypeSubclassFilterTag
            .Default;
        SorcerousChildRift.morphotypeSubclassFilterTag = GraphicsDefinitions.MorphotypeSubclassFilterTag
            .Default;
        SorcerousHauntedSoul.morphotypeSubclassFilterTag = GraphicsDefinitions.MorphotypeSubclassFilterTag
            .Default;
    }

    private static void SwitchDruidKindredBeastToUseCustomInvocationPools()
    {
        var kindredSpirits = FeatureSetKindredSpiritChoice.FeatureSet;

        var kindredSpiritsSprites = new Dictionary<string, byte[]>
        {
            { "PowerKindredSpiritApe", Resources.SpiritApe },
            { "PowerKindredSpiritBear", Resources.SpiritBear },
            { "PowerKindredSpiritEagle", Resources.SpiritEagle },
            { "PowerKindredSpiritSpider", Resources.SpiritSpider },
            { "PowerKindredSpiritViper", Resources.SpiritViper },
            { "PowerKindredSpiritWolf", Resources.SpiritWolf }
        };

        foreach (var featureDefinitionPower in kindredSpirits.OfType<FeatureDefinitionPower>())
        {
            var monsterName = featureDefinitionPower.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName;
            var monsterDefinition = GetDefinition<MonsterDefinition>(monsterName);
            var guiPresentation = monsterDefinition.GuiPresentation;
            var powerName = featureDefinitionPower.Name;
            var sprite = kindredSpiritsSprites.TryGetValue(powerName, out var resource)
                ? Sprites.GetSprite(powerName, resource, 128)
                : monsterDefinition.GuiPresentation.SpriteReference;

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{monsterName}")
                .SetGuiPresentation(guiPresentation.Title, guiPresentation.Description, sprite)
                .SetPoolType(InvocationPoolTypeCustom.Pools.KindredSpiritChoice)
                .SetGrantedFeature(featureDefinitionPower)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        // replace the original features with custom invocation pools
        if (!Main.Settings.EnableLevelUpFeaturesSelection)
        {
            return;
        }

        var replacedFeatures = CircleKindred.FeatureUnlocks
            .Select(x => x.FeatureDefinition == FeatureSetKindredSpiritChoice
                ? new FeatureUnlockByLevel(InvocationPoolKindredSpiritChoice, x.Level)
                : x)
            .ToArray();

        CircleKindred.FeatureUnlocks.SetRange(replacedFeatures);
    }

    private static void SwitchPathOfTheElementsElementalFuryToUseCustomInvocationPools()
    {
        var elementalFuries = PathOfTheElements.FeatureSetElementalFury.FeatureSet;

        var elementalFuriesSprites = new Dictionary<string, BaseDefinition>
        {
            { "Storm", PowerDomainElementalLightningBlade },
            { "Blizzard", PowerDomainElementalIceLance },
            { "Wildfire", PowerDomainElementalFireBurst }
        };

        foreach (var featureDefinitionAncestry in elementalFuries.OfType<FeatureDefinitionAncestry>())
        {
            var name = featureDefinitionAncestry.Name.Replace("AncestryPathOfTheElements", string.Empty);
            var guiPresentation = featureDefinitionAncestry.guiPresentation;

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocationPathOfTheElements{name}")
                .SetGuiPresentation(guiPresentation.Title, guiPresentation.Description, elementalFuriesSprites[name])
                .SetPoolType(InvocationPoolTypeCustom.Pools.PathOfTheElementsElementalFuryChoiceChoice)
                .SetGrantedFeature(featureDefinitionAncestry)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        // replace the original features with custom invocation pools
        if (!Main.Settings.EnableLevelUpFeaturesSelection)
        {
            return;
        }

        var subclass = GetDefinition<CharacterSubclassDefinition>(PathOfTheElements.Name);
        var replacedFeatures = subclass.FeatureUnlocks
            .Select(x => x.FeatureDefinition == PathOfTheElements.FeatureSetElementalFury
                ? new FeatureUnlockByLevel(InvocationPoolPathOfTheElementsElementalFuryChoice, x.Level)
                : x)
            .ToArray();

        subclass.FeatureUnlocks.SetRange(replacedFeatures);
    }

    private static void SwitchRangerToUseCustomInvocationPools()
    {
        const string Name = "Ranger";

        //
        // Terrain Type Affinity
        //

        var dbFeatureDefinitionTerrainTypeAffinity =
            DatabaseRepository.GetDatabase<FeatureDefinitionTerrainTypeAffinity>()
                .Where(x => x.ContentPack != CeContentPackContext.CeContentPack);

        var terrainAffinitySprites = new Dictionary<string, byte[]>
        {
            { "Arctic", Resources.TerrainAffinityArctic },
            { "Coast", Resources.TerrainAffinityCoast },
            { "Desert", Resources.TerrainAffinityDesert },
            { "Forest", Resources.TerrainAffinityForest },
            { "Grassland", Resources.TerrainAffinityGrassland },
            { "Mountain", Resources.TerrainAffinityMountain },
            { "Swamp", Resources.TerrainAffinitySwamp }
        };

        foreach (var featureDefinitionTerrainTypeAffinity in dbFeatureDefinitionTerrainTypeAffinity)
        {
            var terrainTypeName = featureDefinitionTerrainTypeAffinity.TerrainType;
            var terrainType = GetDefinition<TerrainTypeDefinition>(terrainTypeName);
            var guiPresentation = terrainType.GuiPresentation;
            var sprite = Sprites.GetSprite(terrainTypeName, terrainAffinitySprites[terrainTypeName], 128);
            var terrainTitle = Gui.Localize($"Environment/&{terrainTypeName}Title");

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}TerrainType{terrainTypeName}")
                .SetGuiPresentation(
                    Gui.Format(guiPresentation.Title, terrainTitle),
                    Gui.Format(guiPresentation.Description, terrainTitle),
                    sprite)
                .SetPoolType(InvocationPoolTypeCustom.Pools.RangerTerrainTypeAffinity)
                .SetGrantedFeature(featureDefinitionTerrainTypeAffinity)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        //
        // Preferred Enemy
        //

        var preferredEnemies = AdditionalDamageRangerFavoredEnemyChoice.FeatureSet;

        var preferredEnemySprites = new Dictionary<string, byte[]>
        {
            { "Aberration", Resources.PreferredEnemyAberration },
            { "Beast", Resources.PreferredEnemyBeast },
            { "Celestial", Resources.PreferredEnemyCelestial },
            { "Construct", Resources.PreferredEnemyConstruct },
            { "Dragon", Resources.PreferredEnemyDragon },
            { "Elemental", Resources.PreferredEnemyElemental },
            { "Fey", Resources.PreferredEnemyFey },
            { "Fiend", Resources.PreferredEnemyFiend },
            { "Giant", Resources.PreferredEnemyGiant },
            { "Humanoid", Resources.PreferredEnemyHumanoid },
            { "Monstrosity", Resources.PreferredEnemyMonstrosity },
            { "Ooze", Resources.PreferredEnemyOoze },
            { "Plant", Resources.PreferredEnemyPlant },
            { "Undead", Resources.PreferredEnemyUndead }
        };

        foreach (var featureDefinitionPreferredEnemy in preferredEnemies.OfType<FeatureDefinitionAdditionalDamage>())
        {
            var preferredEnemyName = featureDefinitionPreferredEnemy.RequiredCharacterFamily.Name;
            var guiPresentation = featureDefinitionPreferredEnemy.RequiredCharacterFamily.GuiPresentation;
            var sprite = Sprites.GetSprite(preferredEnemyName, preferredEnemySprites[preferredEnemyName], 128);
            var enemyTitle = Gui.Localize($"CharacterFamily/&{preferredEnemyName}Title");

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}PreferredEnemy{preferredEnemyName}")
                .SetGuiPresentation(
                    Gui.Format(guiPresentation.Title, enemyTitle),
                    Gui.Format(guiPresentation.Description, enemyTitle),
                    sprite)
                .SetPoolType(InvocationPoolTypeCustom.Pools.RangerPreferredEnemy)
                .SetGrantedFeature(featureDefinitionPreferredEnemy)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        // replace the original features with custom invocation pools
        if (!Main.Settings.EnableLevelUpFeaturesSelection)
        {
            return;
        }

        // Ranger

        var replacedFeatures = Ranger.FeatureUnlocks
            .Select(x =>
                x.FeatureDefinition == TerrainTypeAffinityRangerNaturalExplorerChoice
                    ? new FeatureUnlockByLevel(InvocationPoolRangerTerrainType, x.Level)
                    : x.FeatureDefinition == AdditionalDamageRangerFavoredEnemyChoice
                        ? new FeatureUnlockByLevel(InvocationPoolRangerPreferredEnemy, x.Level)
                        : x)
            .ToArray();

        Ranger.FeatureUnlocks.SetRange(replacedFeatures);

        // Ranger Survivalist

        var rangerSurvivalist = GetDefinition<CharacterSubclassDefinition>("RangerSurvivalist");

        replacedFeatures = rangerSurvivalist.FeatureUnlocks
            .Select(x =>
                x.FeatureDefinition == AdditionalDamageRangerFavoredEnemyChoice
                    ? new FeatureUnlockByLevel(InvocationPoolRangerPreferredEnemy, x.Level)
                    : x)
            .ToArray();

        rangerSurvivalist.FeatureUnlocks.SetRange(replacedFeatures);
    }

    private static void SwitchSubclassAncestriesToUseCustomInvocationPools(
        string name,
        CharacterSubclassDefinition characterSubclassDefinition,
        FeatureDefinitionFeatureSet featureDefinitionFeatureSet,
        FeatureDefinition featureDefinitionCustomInvocationPool,
        InvocationPoolTypeCustom invocationPoolTypeCustom)
    {
        var draconicAncestries = featureDefinitionFeatureSet.FeatureSet;

        var draconicAncestriesSprites = new Dictionary<string, byte[]>
        {
            { $"Ancestry{name}DraconicBlack", Resources.BlackDragon },
            { $"Ancestry{name}DraconicBlue", Resources.BlueDragon },
            { $"Ancestry{name}DraconicGold", Resources.GoldDragon },
            { $"Ancestry{name}DraconicGreen", Resources.GreenDragon },
            { $"Ancestry{name}DraconicSilver", Resources.SilverDragon }
        };

        foreach (var featureDefinitionAncestry in draconicAncestries.OfType<FeatureDefinitionAncestry>())
        {
            var ancestryName = featureDefinitionAncestry.Name;
            var sprite = Sprites.GetSprite(ancestryName, draconicAncestriesSprites[$"{ancestryName}"], 128);

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{ancestryName}")
                .SetGuiPresentation(
                    featureDefinitionAncestry.GuiPresentation.Title,
                    Gui.Format("Feature/&AncestryLevelUpDraconicDescription",
                        Gui.Localize($"Rules/&{featureDefinitionAncestry.damageType}Title")),
                    sprite)
                .SetPoolType(invocationPoolTypeCustom)
                .SetGrantedFeature(featureDefinitionAncestry)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        // replace the original features with custom invocation pools
        if (!Main.Settings.EnableLevelUpFeaturesSelection)
        {
            return;
        }

        var replacedFeatures = characterSubclassDefinition.FeatureUnlocks
            .Select(x => x.FeatureDefinition == featureDefinitionFeatureSet
                ? new FeatureUnlockByLevel(featureDefinitionCustomInvocationPool, x.Level)
                : x)
            .ToArray();

        characterSubclassDefinition.FeatureUnlocks.SetRange(replacedFeatures);
    }

    internal static void AddLightSourceIfNeeded(GameLocationCharacter gameLocationCharacter)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
        {
            return;
        }

        if (!MonstersThatEmitLight.Contains(rulesetCharacterMonster.MonsterDefinition))
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacterMonster, "ShouldEmitLightFromMonster");
    }

    internal static void AddLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (rulesetCondition == null || !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter)
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacter, "ShouldEmitLightFromCondition");
    }

    private static void AddLightSource(
        GameLocationCharacter gameLocationCharacter,
        RulesetCharacter rulesetCharacter,
        string name)
    {
        var lightSourceForm = Shine.EffectDescription.EffectForms[0].LightSourceForm;

        rulesetCharacter.PersonalLightSource?.Unregister();
        rulesetCharacter.PersonalLightSource = new RulesetLightSource(
            lightSourceForm.Color,
            2,
            4,
            lightSourceForm.GraphicsPrefabAssetGUID,
            LightSourceType.Basic,
            name,
            rulesetCharacter.Guid);

        rulesetCharacter.PersonalLightSource.Register(true);

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .AddCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);
    }

    internal static void RemoveLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (rulesetCondition == null ||
            !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter ||
            rulesetCharacter.PersonalLightSource == null) // if using extinguish fire light source will come null here
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .RemoveCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);

        rulesetCharacter.PersonalLightSource.Unregister();
        rulesetCharacter.PersonalLightSource = null;
    }

    private sealed class FilterTargetingPositionPowerTeleportSummon : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var source = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var summoner = source.RulesetCharacter.GetMySummoner();
            var boxInt = new BoxInt(summoner.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(1, 0, 1);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (positioningService.CanPlaceCharacter(
                        source, position, CellHelpers.PlacementMode.Station) &&
                    positioningService.CanCharacterStayAtPosition_Floor(
                        source, position, onlyCheckCellsWithRealGround: true))
                {
                    cursorLocationSelectPosition.validPositionsCache.Add(position);
                }

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }
    }
}
