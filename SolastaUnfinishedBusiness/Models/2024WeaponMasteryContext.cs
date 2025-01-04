using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static readonly FeatureDefinitionCustomInvocationPool InvocationPoolWeaponMasteryLearn1 =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolWeaponMasteryLearn1")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolWeaponMasteryLearn2 =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolWeaponMasteryLearn2")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization, 2)
            .AddToDB();

    private static readonly FeatureDefinitionCustomInvocationPool InvocationPoolWeaponMasteryLearn3 =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolWeaponMasteryLearn3")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization, 3)
            .AddToDB();

    private static readonly Dictionary<WeaponTypeDefinition, MasteryProperty> WeaponMasteryTable = new()
    {
        { CustomWeaponsContext.HalberdWeaponType, MasteryProperty.Cleave },
        { CustomWeaponsContext.PikeWeaponType, MasteryProperty.Push },
        { WeaponTypeDefinitions.BattleaxeType, MasteryProperty.Cleave },
        { WeaponTypeDefinitions.ClubType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.DaggerType, MasteryProperty.Nick },
        { WeaponTypeDefinitions.DartType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.GreataxeType, MasteryProperty.Cleave },
        { WeaponTypeDefinitions.GreatswordType, MasteryProperty.Graze },
        { WeaponTypeDefinitions.HandaxeType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.HeavyCrossbowType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.JavelinType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.LightCrossbowType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.LongbowType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.LongswordType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.MaceType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.MaulType, MasteryProperty.Topple },
        { WeaponTypeDefinitions.MorningstarType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.QuarterstaffType, MasteryProperty.Topple },
        { WeaponTypeDefinitions.RapierType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.ScimitarType, MasteryProperty.Nick },
        { WeaponTypeDefinitions.ShortbowType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.ShortswordType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.SpearType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.WarhammerType, MasteryProperty.Push }
    };

    private static void LoadWeaponMastery()
    {
        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryCleave")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorCleave())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryGraze")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorGraze())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryNick")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorNick())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryPush")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorPush())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasterySap")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorSap())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryTopple")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorTopple())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasterySlow")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorSlow())
            .AddToDB();

        _ = FeatureDefinitionBuilder
            .Create("FeatureWeaponMasteryVex")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorVex())
            .AddToDB();

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityWeaponMasteryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryToggle)
            .AddToDB();

        foreach (var kvp in WeaponMasteryTable)
        {
            var weaponTypeDefinition = kvp.Key;
            var weaponTypeName = weaponTypeDefinition.Name;
            var masteryProperty = kvp.Value;
            var featureSpecialization = GetDefinition<FeatureDefinition>($"FeatureWeaponMastery{masteryProperty}");
            var featureSet = FeatureDefinitionFeatureSetBuilder
                .Create($"FeatureSetWeaponMastery{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetFeatureSet(actionAffinityToggle, featureSpecialization)
                .AddToDB();

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocationWeaponMastery{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    featureSpecialization.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization)
                .SetGrantedFeature(featureSet)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }
    }

    internal static void SwitchWeaponMastery()
    {
        var klasses = new[] { Barbarian, Fighter, Paladin, Ranger, Rogue };

        foreach (var klass in klasses)
        {
            klass.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == InvocationPoolWeaponMasteryLearn1 ||
                x.FeatureDefinition == InvocationPoolWeaponMasteryLearn2 ||
                x.FeatureDefinition == InvocationPoolWeaponMasteryLearn3);
        }

        if (!Main.Settings.UseWeaponMasterySystem)
        {
            return;
        }

        Barbarian.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn2, 1),
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn1, 4),
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn1, 10));
        Fighter.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn3, 1),
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn1, 4),
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn1, 10),
            new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn1, 16));
        Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn2, 1));
        Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn2, 1));
        Rogue.FeatureUnlocks.Add(new FeatureUnlockByLevel(InvocationPoolWeaponMasteryLearn2, 1));
    }

    private enum MasteryProperty
    {
        Cleave,
        Graze,
        Nick,
        Push,
        Sap,
        Slow,
        Topple,
        Vex
    }

    private sealed class CustomBehaviorCleave
    {
    }

    private sealed class CustomBehaviorGraze
    {
    }

    private sealed class CustomBehaviorNick
    {
    }

    private sealed class CustomBehaviorPush
    {
    }

    private sealed class CustomBehaviorSap
    {
    }

    private sealed class CustomBehaviorSlow
    {
    }

    private sealed class CustomBehaviorTopple
    {
    }

    private sealed class CustomBehaviorVex
    {
    }
}
