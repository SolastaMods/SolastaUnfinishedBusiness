using SolastaModApi;
using static SolastaModApi.DatabaseHelper.ActionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class ActionAffinityRangerVanishActionBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
    {
        private const string ActionAffinityRangerVanishActionName = "ZSAdditionalActionVanish";
        private const string ActionAffinityRangerVanishActionGuid = "83711ec64d8c47bfa91053a00a1d0a83";

        protected ActionAffinityRangerVanishActionBuilder(string name, string guid) : base(ActionAffinityRogueCunningAction, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RangerVanishActionTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerVanishActionDescription";

            Definition.AuthorizedActions.Clear();
            Definition.AuthorizedActions.Add(HideBonus.Id);
        }

        private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
            => new ActionAffinityRangerVanishActionBuilder(name, guid).AddToDB();

        internal static readonly FeatureDefinitionActionAffinity ActionAffinityRangerVanishAction
            = CreateAndAddToDB(ActionAffinityRangerVanishActionName, ActionAffinityRangerVanishActionGuid);
    }
}
