using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class ConditionPersistentRageBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string ConditionPersistentRagingName = "ZSConditionPersistentRaging";
        private const string ConditionPersistentRagingGuid = "08e1a91049414d2cb3a247f8dae14b66";

        protected ConditionPersistentRageBuilder(string name, string guid) : base(ConditionRaging, name, guid)
        {
            Definition.SetDurationType(RuleDefinitions.DurationType.UntilAnyRest);
            Definition.SetGuiPresentation(new GuiPresentationBuilder("Condition/&ZSConditionPersistentRagingDescription", "Condition/&ZSConditionPersistentRagingTitle").Build());
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new ConditionPersistentRageBuilder(name, guid).AddToDB();

        internal static readonly ConditionDefinition ConditionPersistentRage =
            CreateAndAddToDB(ConditionPersistentRagingName, ConditionPersistentRagingGuid);
    }
}
