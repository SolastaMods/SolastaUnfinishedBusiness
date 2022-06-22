using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Level20.Features;

internal sealed class RangerVanishActionBuilder : FeatureDefinitionActionAffinityBuilder
{
    private const string RangerVanishActionName = "ZSAdditionalActionVanish";
    private const string RangerVanishActionGuid = "83711ec64d8c47bfa91053a00a1d0a83";

    internal static readonly FeatureDefinitionActionAffinity RangerVanishAction
        = CreateAndAddToDB(RangerVanishActionName, RangerVanishActionGuid);

    private RangerVanishActionBuilder(string name, string guid) : base(ActionAffinityRogueCunningAction, name, guid)
    {
        Definition.GuiPresentation.Title = "Feature/&RangerVanishActionTitle";
        Definition.GuiPresentation.Description = "Feature/&RangerVanishActionDescription";

        Definition.AuthorizedActions.Clear();
        Definition.AuthorizedActions.Add(ActionDefinitions.Id.HideBonus);
    }

    private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
    {
        return new RangerVanishActionBuilder(name, guid).AddToDB();
    }
}
