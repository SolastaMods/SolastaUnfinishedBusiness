using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class GroupFeats
{
    private static readonly List<FeatDefinition> Groups = new();

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.AddRange(BuildAlchemistEnchanterAndTools());
        feats.Add(BuildBodyResilience());
        feats.Add(BuildElementalTouchGroup());
        feats.Add(BuildCreedGroup());
        feats.Add(BuildRangedCombat());
        feats.Add(BuildSpellCombat());
        feats.Add(BuildTwoHandedCombat());
        feats.Add(BuildTwoWeaponCombat());
        feats.AddRange(Groups);
    }


    internal static FeatDefinition MakeGroup(string name, string family, params FeatDefinition[] feats)
    {
        return MakeGroup(name, family, feats.AsEnumerable());
    }

    internal static FeatDefinition MakeGroup(string name, string family, IEnumerable<FeatDefinition> feats)
    {
        var group = FeatDefinitionBuilder
            .Create(name)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(feats))
            .SetFeatFamily(family)
            .SetFeatures()
            .AddToDB();

        Groups.Add(group);

        return group;
    }


    private static IEnumerable<FeatDefinition> BuildAlchemistEnchanterAndTools()
    {
        var featGroupAlchemist = FeatDefinitionBuilder
            .Create("FeatGroupAlchemist")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.InitiateAlchemist,
                FeatDefinitions.MasterAlchemist))
            .SetFeatures()
            .AddToDB();

        var featGroupEnchanter = FeatDefinitionBuilder
            .Create("FeatGroupEnchanter")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.InitiateEnchanter,
                FeatDefinitions.MasterEnchanter))
            .SetFeatures()
            .AddToDB();

        var featGroupTools = FeatDefinitionBuilder
            .Create("FeatGroupTools")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                CraftyFeats.FeatGroupApothecary,
                CraftyFeats.FeatGroupToxicologist,
                CraftyFeats.FeatCraftyFletcher,
                CraftyFeats.FeatCraftyScriber,
                featGroupAlchemist,
                featGroupEnchanter))
            .SetFeatures()
            .AddToDB();

        return new[] { featGroupAlchemist, featGroupEnchanter, featGroupTools };
    }

    private static FeatDefinition BuildBodyResilience()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupBodyResilience")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.BadlandsMarauder,
                FeatDefinitions.Enduring_Body,
                FeatDefinitions.FocusedSleeper,
                FeatDefinitions.HardToKill,
                FeatDefinitions.Hauler,
                FeatDefinitions.Robust,
                OtherFeats.FeatTough))
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildElementalTouchGroup()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupElementalTouch")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.BurningTouch,
                FeatDefinitions.ToxicTouch,
                FeatDefinitions.ElectrifyingTouch,
                FeatDefinitions.IcyTouch,
                FeatDefinitions.MeltingTouch))
            .SetFeatFamily(FeatDefinitions.BurningTouch.FamilyTag)
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildCreedGroup()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupCreed")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.Creed_Of_Arun,
                FeatDefinitions.Creed_Of_Einar,
                FeatDefinitions.Creed_Of_Maraike,
                FeatDefinitions.Creed_Of_Misaye,
                FeatDefinitions.Creed_Of_Pakri,
                FeatDefinitions.Creed_Of_Solasta))
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildRangedCombat()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupRangedCombat")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.TakeAim,
                FeatDefinitions.UncannyAccuracy,
                CraftyFeats.FeatCraftyFletcher,
                OtherFeats.FeatRangedExpert,
                OtherFeats.FeatDeadEye,
                OtherFeats.FeatMarksman))
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildSpellCombat()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupSpellCombat")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.FlawlessConcentration,
                FeatDefinitions.PowerfulCantrip,
                OtherFeats.FeatWarCaster))
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildTwoHandedCombat()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupTwoHandedCombat")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.MightyBlow,
                FeatDefinitions.ForestallingStrength,
                FeatDefinitions.FollowUpStrike))
            .SetFeatures()
            .AddToDB();
    }

    private static FeatDefinition BuildTwoWeaponCombat()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupTwoWeaponCombat")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.Ambidextrous,
                OtherFeats.FeatDualWeaponDefense,
                OtherFeats.FeatDualFlurry,
                FeatDefinitions.TwinBlade))
            .SetFeatures()
            .AddToDB();
    }
}
