using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class GroupFeats
{
    internal const string Slasher = "Slasher";
    internal const string Piercer = "Piercer";
    internal const string Crusher = "Crusher";
    private static readonly List<FeatDefinition> Groups = new();

    internal static FeatDefinition FeatGroupAgilityCombat { get; } = MakeGroup("FeatGroupAgilityCombat", null,
        FeatDefinitions.EagerForBattle,
        FeatDefinitions.ForestRunner,
        FeatDefinitions.ReadyOrNot,
        FeatDefinitions.RushToBattle);

    internal static FeatDefinition FeatGroupDefenseCombat { get; } = MakeGroup("FeatGroupDefenseCombat", null,
        FeatDefinitions.CloakAndDagger,
        FeatDefinitions.RaiseShield,
        FeatDefinitions.TwinBlade);

    internal static FeatDefinition FeatGroupElementalTouch { get; } = MakeGroup("FeatGroupElementalTouch",
        "Touch",
        FeatDefinitions.BurningTouch,
        FeatDefinitions.ToxicTouch,
        FeatDefinitions.ElectrifyingTouch,
        FeatDefinitions.IcyTouch,
        FeatDefinitions.MeltingTouch);

    internal static FeatDefinition FeatGroupPiercer { get; } = MakeGroup("FeatGroupPiercer", Piercer);

    internal static FeatDefinition FeatGroupSpellCombat { get; } = MakeGroup("FeatGroupSpellCombat", null,
        FeatDefinitions.FlawlessConcentration,
        FeatDefinitions.PowerfulCantrip);

    internal static FeatDefinition FeatGroupSupportCombat { get; } = MakeGroup("FeatGroupSupportCombat", null,
        FeatDefinitions.Mender);

    internal static FeatDefinition FeatGroupTwoHandedCombat { get; } = MakeGroup("FeatGroupTwoHandedCombat", null,
        FeatDefinitions.MightyBlow,
        FeatDefinitions.ForestallingStrength,
        FeatDefinitions.FollowUpStrike);

    internal static FeatDefinition FeatGroupUnarmoredCombat { get; } = MakeGroup("FeatGroupUnarmoredCombat", null,
        FeatGroupElementalTouch);

    private static void ApplyDynamicDescription(FeatDefinition groupDefinition)
    {
        var groupedFeat = groupDefinition.GetFirstSubFeatureOfType<GroupedFeat>();

        if (groupedFeat == null)
        {
            return;
        }

        var titles = groupedFeat.GetSubFeats(true)
            .Select(x => x.FormatTitle())
            .OrderBy(x => x)
            .ToArray();
        var title = string.Join(", ", titles);

        groupDefinition.guiPresentation.description = Gui.Format(groupDefinition.guiPresentation.description, title);
    }

    internal static void Load(Action<FeatDefinition> loader)
    {
        MakeGroup("FeatGroupCreed", null,
            FeatDefinitions.Creed_Of_Arun,
            FeatDefinitions.Creed_Of_Einar,
            FeatDefinitions.Creed_Of_Maraike,
            FeatDefinitions.Creed_Of_Misaye,
            FeatDefinitions.Creed_Of_Pakri,
            FeatDefinitions.Creed_Of_Solasta);

        Groups.ForEach(ApplyDynamicDescription);
        Groups.ForEach(loader);
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

    internal static FeatDefinition MakeGroupWithPreRequisite(
        string name,
        string family,
        Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)> validator,
        params FeatDefinition[] feats)
    {
        var group = FeatDefinitionWithPrerequisitesBuilder
            .Create(name)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(feats))
            .SetFeatFamily(family)
            .SetFeatures()
            .SetValidators(validator)
            .AddToDB();

        Groups.Add(group);

        return group;
    }

    internal static void AddFeats(this FeatDefinition groupDefinition, params FeatDefinition[] feats)
    {
        var groupedFeat = groupDefinition.GetFirstSubFeatureOfType<GroupedFeat>();

        groupedFeat?.AddFeats(feats);
    }
}
