using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
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
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MorphotypeElementDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class CharacterContext
{
    internal const int MinInitialFeats = 0;
    internal const int MaxInitialFeats = 4; // don't increase this value to avoid issue reports on crazy scenarios

    internal const int GameMaxAttribute = 15;
    internal const int GameBuyPoints = 27;

    internal const int ModMaxAttribute = 17;
    internal const int ModBuyPoints = 35;

    internal static readonly ConditionDefinition ConditionIndomitableSaving = ConditionDefinitionBuilder
        .Create("ConditionIndomitableSaving")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .AddCustomSubFeatures(new RollSavingThrowInitiatedIndomitableSaving(
            ConditionDefinitionBuilder
                .Create("ConditionIndomitableSavingSavingThrow")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(
                    FeatureDefinitionSavingThrowAffinityBuilder
                        .Create("SavingThrowAffinityIndomitableSaving")
                        .SetGuiPresentation("Feature/&IndomitableResistanceTitle", Gui.NoLocalization)
                        .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.SourceAbility, DieType.D1, 1,
                            false,
                            AttributeDefinitions.Strength,
                            AttributeDefinitions.Dexterity,
                            AttributeDefinitions.Constitution,
                            AttributeDefinitions.Intelligence,
                            AttributeDefinitions.Wisdom,
                            AttributeDefinitions.Charisma)
                        .AddToDB())
                .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
                .AddToDB()))
        .AddToDB();

    private static readonly FeatureDefinitionAttributeModifier AttributeModifierMonkAbundantKi =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMonkAbundantKi")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.KiPoints, 1)
            .SetSituationalContext(SituationalContext.NotWearingArmorOrShield)
            .AddToDB();

    private static readonly FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityDarknessPerceptive =
        FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarknessPerceptive")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage,
                abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddCustomSubFeatures(ValidatorsCharacter.IsUnlitOrDarkness)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolMonkWeaponSpecialization =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolMonkWeaponSpecialization")
            .SetGuiPresentation("InvocationPoolMonkWeaponSpecializationLearn", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization)
            .AddToDB();

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

    internal static readonly FeatureDefinitionPower FeatureDefinitionPowerHelpAction = FeatureDefinitionPowerBuilder
        .Create("PowerHelp")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerHelp", Resources.PowerHelp, 256, 128))
        .SetUsesFixed(ActivationTime.Action)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                .SetEffectForms(EffectFormBuilder.ConditionForm(CustomConditionsContext.Distracted))
                .Build())
        .SetUniqueInstance()
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

    private static readonly FeatureDefinitionPower FeatureDefinitionPowerNatureShroud = FeatureDefinitionPowerBuilder
        .Create("PowerRangerNatureShroud")
        .SetGuiPresentation(Category.Feature, Invisibility)
        .SetUsesProficiencyBonus(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible))
                .SetParticleEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                .Build())
        .AddToDB();

    private static int PreviousTotalFeatsGrantedFirstLevel { get; set; } = -1;
    private static bool PreviousAlternateHuman { get; set; }

    internal static void LateLoad()
    {
        FlexibleBackgroundsContext.Load();
        FlexibleBackgroundsContext.SwitchFlexibleBackgrounds();
        FlexibleRacesContext.SwitchFlexibleRaces();
        LoadAdditionalNames();
        LoadEpicArray();
        LoadFeatsPointPools();
        LoadMonkWeaponSpecialization();
        LoadVision();
        LoadVisuals();
        BuildBarbarianBrutalStrike();
        BuildRogueCunningStrike();
        SwitchAsiAndFeat();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianBrutalCritical();
        SwitchBarbarianRecklessSameBuffDebuffDuration();
        SwitchBarbarianRegainOneRageAtShortRest();
        SwitchBarbarianFightingStyle();
        SwitchDarknessPerceptive();
        SwitchDragonbornElementalBreathUsages();
        SwitchDruidKindredBeastToUseCustomInvocationPools();
        SwitchEveryFourLevelsFeats();
        SwitchEveryFourLevelsFeats(true);
        SwitchFighterLevelToIndomitableSavingReroll();
        SwitchFighterWeaponSpecialization();
        SwitchFirstLevelTotalFeats();
        SwitchHelpPower();
        SwitchMonkAbundantKi();
        SwitchMonkFightingStyle();
        SwitchMonkDoNotRequireAttackActionForFlurry();
        SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall();
        SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        SwitchMonkWeaponSpecialization();
        SwitchPathOfTheElementsElementalFuryToUseCustomInvocationPools();
        SwitchRangerHumanoidFavoredEnemy();
        SwitchRangerNatureShroud();
        SwitchRangerToUseCustomInvocationPools();
        SwitchRogueCunningStrike();
        SwitchRogueFightingStyle();
        SwitchRogueSteadyAim();
        SwitchRogueStrSaving();
        SwitchScimitarWeaponSpecialization();
        SwitchSubclassAncestriesToUseCustomInvocationPools(
            "PathClaw", PathClaw,
            FeatureSetPathClawDragonAncestry, InvocationPoolPathClawDraconicChoice,
            InvocationPoolTypeCustom.Pools.PathClawDraconicChoice);
        SwitchSubclassAncestriesToUseCustomInvocationPools(
            "Sorcerer", SorcerousDraconicBloodline,
            FeatureSetSorcererDraconicChoice, InvocationPoolSorcererDraconicChoice,
            InvocationPoolTypeCustom.Pools.SorcererDraconicChoice);
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
        if (!Main.Settings.OfferAdditionalLoreFriendlyNames)
        {
            return;
        }

        var payload = Resources.Names;
        var lines = new List<string>(payload.Split([Environment.NewLine], StringSplitOptions.None));

        foreach (var line in lines)
        {
            var columns = line.Split(Separator, 3);

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

    private static void LoadFeatsPointPools()
    {
        // create feats point pools
        // +1 here as need to count the Alternate Human Feat
        for (var i = 1; i <= MaxInitialFeats + 1; i++)
        {
            var s = i.ToString();

            _ = FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{i}BonusFeats")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PointPoolSelectBonusFeatsTitle", s),
                    Gui.Format("Feature/&PointPoolSelectBonusFeatsDescription", s))
                .SetPool(HeroDefinitions.PointsPoolType.Feat, i)
                .AddToDB();
        }
    }

    private static void LoadVision()
    {
        if (Main.Settings.DisableSenseDarkVisionFromAllRaces)
        {
            foreach (var featureUnlocks in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                         .Select(crd => crd.FeatureUnlocks))
            {
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseDarkvision);
                // Half-orcs have a different darkvision.
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseDarkvision12);
            }
        }

        // ReSharper disable once InvertIf
        if (Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces)
        {
            foreach (var featureUnlocks in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                         .Select(crd => crd.FeatureUnlocks))
            {
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseSuperiorDarkvision);
            }
        }
    }

    private static void LoadVisuals()
    {
        var dbMorphotypeElementDefinition = DatabaseRepository.GetDatabase<MorphotypeElementDefinition>();

        if (Main.Settings.UnlockSkinColors)
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

        if (Main.Settings.UnlockGlowingColorsForAllMarksAndTattoos)
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

        if (!Main.Settings.AddNewBrightEyeColors)
        {
            foreach (var morphotype in brightEyes)
            {
                morphotype.playerSelectable = false;
            }
        }

        var glowingEyes = new List<MorphotypeElementDefinition>();

        Morphotypes.CreateGlowingEyes(glowingEyes);

        if (!Main.Settings.UnlockGlowingEyeColors)
        {
            foreach (var morphotype in glowingEyes)
            {
                morphotype.playerSelectable = false;
            }
        }

        if (Main.Settings.UnlockEyeStyles)
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

        if (Main.Settings.UnlockAllNpcFaces)
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

        if (Main.Settings.AllowBeardlessDwarves)
        {
            Dwarf.RacePresentation.needBeard = false;
            DwarfHill.RacePresentation.needBeard = false;
            DwarfSnow.RacePresentation.needBeard = false;
            SubraceGrayDwarfBuilder.SubraceGrayDwarf.RacePresentation.needBeard = false;
            SubraceIronbornDwarfBuilder.SubraceIronbornDwarf.RacePresentation.needBeard = false;
            SubraceObsidianDwarfBuilder.SubraceObsidianDwarf.RacePresentation.needBeard = false;
            Dwarf.RacePresentation.MaleBeardShapeOptions.Add(BeardShape_None.Name);
        }

        if (Main.Settings.UnlockMarkAndTattoosForAllCharacters)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecoration))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        if (!Main.Settings.AllowUnmarkedSorcerers)
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

    internal static void SwitchAsiAndFeat()
    {
        FeatureSetAbilityScoreChoice.mode = Main.Settings.EnablesAsiAndFeat
            ? FeatureDefinitionFeatureSet.FeatureSetMode.Union
            : FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion;
    }

    internal static void SwitchDragonbornElementalBreathUsages()
    {
        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .Where(x =>
                x.Name.StartsWith("PowerDragonbornBreathWeapon") ||
                x.Name == "PowerFeatDragonFear");

        foreach (var power in powers)
        {
            if (Main.Settings.ChangeDragonbornElementalBreathUsages)
            {
                power.usesAbilityScoreName = AttributeDefinitions.Constitution;
                power.usesDetermination = UsesDetermination.AbilityBonusPlusFixed;
                power.fixedUsesPerRecharge = 0;
            }
            else
            {
                power.usesAbilityScoreName = AttributeDefinitions.Charisma;
                power.usesDetermination = UsesDetermination.Fixed;
                power.fixedUsesPerRecharge = 1;
            }
        }
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

        if (!Main.Settings.ImproveLevelUpFeaturesSelection)
        {
            return;
        }

        var replacedFeatures = CircleKindred.FeatureUnlocks
            .Select(x => x.FeatureDefinition == FeatureSetKindredSpiritChoice
                ? new FeatureUnlockByLevel(InvocationPoolKindredSpiritChoice, x.Level)
                : x)
            .ToList();

        CircleKindred.FeatureUnlocks.SetRange(replacedFeatures);
    }

    internal static void SwitchEveryFourLevelsFeats(bool isMiddle = false)
    {
        var levels = isMiddle ? new[] { 6, 14 } : [2, 10, 18];
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var pointPool1BonusFeats = GetDefinition<FeatureDefinitionPointPool>("PointPool1BonusFeats");
        var pointPool2BonusFeats = GetDefinition<FeatureDefinitionPointPool>("PointPool2BonusFeats");
        var enable = isMiddle
            ? Main.Settings.EnableFeatsAtEveryFourLevelsMiddle
            : Main.Settings.EnableFeatsAtEveryFourLevels;

        foreach (var characterClassDefinition in dbCharacterClassDefinition)
        {
            foreach (var level in levels)
            {
                var featureUnlockPointPool1 = new FeatureUnlockByLevel(pointPool1BonusFeats, level);
                var featureUnlockPointPool2 = new FeatureUnlockByLevel(pointPool2BonusFeats, level);

                if (enable)
                {
                    characterClassDefinition.FeatureUnlocks.Add(ShouldBe2Points()
                        ? featureUnlockPointPool2
                        : featureUnlockPointPool1);
                }
                else
                {
                    if (ShouldBe2Points())
                    {
                        characterClassDefinition.FeatureUnlocks.RemoveAll(x =>
                            x.FeatureDefinition == pointPool2BonusFeats && x.level == level);
                    }
                    else
                    {
                        characterClassDefinition.FeatureUnlocks.RemoveAll(x =>
                            x.FeatureDefinition == pointPool1BonusFeats && x.level == level);
                    }
                }

                continue;

                bool ShouldBe2Points()
                {
                    return (characterClassDefinition == Rogue && level is 10 && !isMiddle) ||
                           (characterClassDefinition == Fighter && level is 6 or 14 && isMiddle);
                }
            }

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                characterClassDefinition.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
            }
        }
    }

    internal static void SwitchFighterLevelToIndomitableSavingReroll()
    {
        DatabaseHelper.ActionDefinitions.UseIndomitableResistance.GuiPresentation.description =
            Main.Settings.AddFighterLevelToIndomitableSavingReroll
                ? "Feature/&EnhancedIndomitableResistanceDescription"
                : "Feature/&IndomitableResistanceDescription";
    }

    internal static void SwitchFighterWeaponSpecialization()
    {
        var levels = new[] { 8, 16 };

        if (Main.Settings.EnableFighterWeaponSpecialization)
        {
            foreach (var level in levels)
            {
                Fighter.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(MartialWeaponMaster.InvocationPoolSpecialization, level));
            }
        }
        else
        {
            foreach (var level in levels)
            {
                Fighter.FeatureUnlocks
                    .RemoveAll(x => x.level == level &&
                                    x.FeatureDefinition == MartialWeaponMaster.InvocationPoolSpecialization);
            }
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchFirstLevelTotalFeats()
    {
        if (PreviousTotalFeatsGrantedFirstLevel > -1)
        {
            UnloadRacesLevel1Feats(PreviousTotalFeatsGrantedFirstLevel, PreviousAlternateHuman);
        }

        PreviousTotalFeatsGrantedFirstLevel = Main.Settings.TotalFeatsGrantedFirstLevel;
        PreviousAlternateHuman = Main.Settings.EnableAlternateHuman;
        LoadRacesLevel1Feats(Main.Settings.TotalFeatsGrantedFirstLevel, Main.Settings.EnableAlternateHuman);
    }

    internal static void SwitchHelpPower()
    {
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var subRaces = dbCharacterRaceDefinition
            .SelectMany(x => x.SubRaces);
        var races = dbCharacterRaceDefinition
            .Where(x => !subRaces.Contains(x));

        if (Main.Settings.AddHelpActionToAllRaces)
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => !a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.Add(
                    new FeatureUnlockByLevel(FeatureDefinitionPowerHelpAction, 1));
            }
        }
        else
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.RemoveAll(x =>
                    x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction);
            }
        }
    }

    internal static void SwitchDarknessPerceptive()
    {
        var races = new List<CharacterRaceDefinition>
        {
            RaceKoboldBuilder.SubraceDarkKobold,
            SubraceDarkelfBuilder.SubraceDarkelf,
            SubraceGrayDwarfBuilder.SubraceGrayDwarf
        };

        if (Main.Settings.AddDarknessPerceptiveToDarkRaces)
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => !a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive)))
            {
                characterRaceDefinition.FeatureUnlocks.Add(
                    new FeatureUnlockByLevel(AbilityCheckAffinityDarknessPerceptive, 1));
            }
        }
        else
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive)))
            {
                characterRaceDefinition.FeatureUnlocks.RemoveAll(x =>
                    x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive);
            }
        }
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
        if (!Main.Settings.ImproveLevelUpFeaturesSelection)
        {
            return;
        }

        var subclass = GetDefinition<CharacterSubclassDefinition>(PathOfTheElements.Name);
        var replacedFeatures = subclass.FeatureUnlocks
            .Select(x => x.FeatureDefinition == PathOfTheElements.FeatureSetElementalFury
                ? new FeatureUnlockByLevel(InvocationPoolPathOfTheElementsElementalFuryChoice, x.Level)
                : x)
            .ToList();

        subclass.FeatureUnlocks.SetRange(replacedFeatures);
    }

    private static void SwitchRangerToUseCustomInvocationPools()
    {
        const string Name = "Ranger";

        //
        // Terrain Type Affinity
        //

        var dbFeatureDefinitionTerrainTypeAffinity =
            DatabaseRepository.GetDatabase<FeatureDefinitionTerrainTypeAffinity>();

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

        if (!Main.Settings.ImproveLevelUpFeaturesSelection)
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
            .ToList();

        Ranger.FeatureUnlocks.SetRange(replacedFeatures);

        // Ranger Survivalist

        var rangerSurvivalist = GetDefinition<CharacterSubclassDefinition>("RangerSurvivalist");

        replacedFeatures = rangerSurvivalist.FeatureUnlocks
            .Select(x =>
                x.FeatureDefinition == AdditionalDamageRangerFavoredEnemyChoice
                    ? new FeatureUnlockByLevel(InvocationPoolRangerPreferredEnemy, x.Level)
                    : x)
            .ToList();

        rangerSurvivalist.FeatureUnlocks.SetRange(replacedFeatures);
    }

    internal static void SwitchScimitarWeaponSpecialization()
    {
        var proficiencies = new List<FeatureDefinitionProficiency> { ProficiencyBardWeapon, ProficiencyRogueWeapon };

        foreach (var proficiency in proficiencies)
        {
            if (Main.Settings.GrantScimitarSpecializationToBardRogue)
            {
                proficiency.Proficiencies.TryAdd(WeaponTypeDefinitions.ScimitarType.Name);
            }
            else
            {
                proficiency.Proficiencies.Remove(WeaponTypeDefinitions.ScimitarType.Name);
            }
        }
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

        if (!Main.Settings.ImproveLevelUpFeaturesSelection)
        {
            return;
        }

        var replacedFeatures = characterSubclassDefinition.FeatureUnlocks
            .Select(x => x.FeatureDefinition == featureDefinitionFeatureSet
                ? new FeatureUnlockByLevel(featureDefinitionCustomInvocationPool, x.Level)
                : x)
            .ToList();

        characterSubclassDefinition.FeatureUnlocks.SetRange(replacedFeatures);
    }

    private static void BuildFeatureUnlocks(
        int initialFeats,
        bool alternateHuman,
        [CanBeNull] out FeatureUnlockByLevel featureUnlockByLevelNonHuman,
        [CanBeNull] out FeatureUnlockByLevel featureUnlockByLevelHuman)
    {
        string name;

        featureUnlockByLevelNonHuman = null;
        featureUnlockByLevelHuman = null;

        switch (initialFeats)
        {
            case 0:
            {
                if (alternateHuman)
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(PointPoolBonusFeat, 1);
                }

                break;
            }
            case 1:
            {
                featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(PointPoolBonusFeat, 1);

                name = "PointPool2BonusFeats";
                if (alternateHuman && TryGetDefinition<FeatureDefinitionPointPool>(name, out var pointPool2BonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPool2BonusFeats, 1);
                }

                break;
            }
            case > 1:
            {
                name = $"PointPool{initialFeats}BonusFeats";
                if (TryGetDefinition<FeatureDefinitionPointPool>(name, out var featureDefinitionPointPool))
                {
                    featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(featureDefinitionPointPool, 1);
                }

                name = $"PointPool{initialFeats + 1}BonusFeats";
                if (alternateHuman && TryGetDefinition<FeatureDefinitionPointPool>(name, out var pointPoolXBonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPoolXBonusFeats, 1);
                }

                break;
            }
        }
    }

    private static void LoadRacesLevel1Feats(int initialFeats, bool alternateHuman)
    {
        var human = Human;

        BuildFeatureUnlocks(initialFeats, alternateHuman, out var featureUnlockByLevelNonHuman,
            out var featureUnlockByLevelHuman);

        foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
        {
            if (IsSubRace(characterRaceDefinition))
            {
                continue;
            }

            if (alternateHuman && characterRaceDefinition == human)
            {
                if (featureUnlockByLevelHuman != null)
                {
                    human.FeatureUnlocks.Add(featureUnlockByLevelHuman);
                }

                var pointPoolAbilityScoreImprovement =
                    new FeatureUnlockByLevel(PointPoolAbilityScoreImprovement, 1);
                human.FeatureUnlocks.Add(pointPoolAbilityScoreImprovement);

                var pointPoolHumanSkillPool = new FeatureUnlockByLevel(PointPoolHumanSkillPool, 1);
                human.FeatureUnlocks.Add(pointPoolHumanSkillPool);

                Remove(human,
                    FeatureDefinitionAttributeModifiers
                        .AttributeModifierHumanAbilityScoreIncrease);
            }
            else
            {
                if (featureUnlockByLevelNonHuman != null)
                {
                    characterRaceDefinition.FeatureUnlocks.Add(featureUnlockByLevelNonHuman);
                }
            }
        }
    }

    private static void UnloadRacesLevel1Feats(int initialFeats, bool alternateHuman)
    {
        var human = Human;

        BuildFeatureUnlocks(initialFeats, alternateHuman,
            out var featureUnlockByLevelNonHuman,
            out var featureUnlockByLevelHuman);

        foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
        {
            if (IsSubRace(characterRaceDefinition))
            {
                continue;
            }

            if (alternateHuman && characterRaceDefinition == human)
            {
                if (featureUnlockByLevelHuman != null)
                {
                    Remove(human, featureUnlockByLevelHuman);
                }

                Remove(human, PointPoolAbilityScoreImprovement);
                Remove(human, PointPoolHumanSkillPool);

                var humanAttributeIncrease = new FeatureUnlockByLevel(
                    FeatureDefinitionAttributeModifiers.AttributeModifierHumanAbilityScoreIncrease, 1);

                human.FeatureUnlocks.Add(humanAttributeIncrease);
            }
            else
            {
                if (featureUnlockByLevelNonHuman != null)
                {
                    Remove(characterRaceDefinition, featureUnlockByLevelNonHuman);
                }
            }
        }
    }

    private static void Remove(
        [NotNull] CharacterRaceDefinition characterRaceDefinition,
        BaseDefinition toRemove)
    {
        var ndx = -1;

        for (var i = 0; i < characterRaceDefinition.FeatureUnlocks.Count; i++)
        {
            if (characterRaceDefinition.FeatureUnlocks[i].Level == 1 &&
                characterRaceDefinition.FeatureUnlocks[i].FeatureDefinition == toRemove)
            {
                ndx = i;
            }
        }

        if (ndx >= 0)
        {
            characterRaceDefinition.FeatureUnlocks.RemoveAt(ndx);
        }
    }

    private static void Remove(
        [NotNull] CharacterRaceDefinition characterRaceDefinition,
        [NotNull] FeatureUnlockByLevel featureUnlockByLevel)
    {
        Remove(characterRaceDefinition, featureUnlockByLevel.FeatureDefinition);
    }

    private static bool IsSubRace(CharacterRaceDefinition raceDefinition)
    {
        return DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .Any(crd => crd.SubRaces.Contains(raceDefinition));
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

    private sealed class RollSavingThrowInitiatedIndomitableSaving(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionIndomitableSaving) : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
            List<EffectForm> effectForms)
        {
            var classLevel = defender.GetClassLevel(Fighter);

            defender.InflictCondition(
                conditionIndomitableSaving.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagStatus,
                caster.Guid,
                caster.CurrentFaction.Name,
                1,
                conditionIndomitableSaving.Name,
                0,
                classLevel,
                0);
        }
    }
}
