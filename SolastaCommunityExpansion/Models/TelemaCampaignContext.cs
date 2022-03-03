using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.CampaignDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterTemplateDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class TelemaCampaignContext
    {
        private sealed class TelemaCampaignUnleashedBuilder : CampaignDefinitionBuilder
        {
            private const string TelemaDemoUnleashedName = "TelemaDemoUnleashed";
            private const string TelemaDemoUnleashedGuid = "397df3dcfcd444f09df11d05034ec52e";

            private TelemaCampaignUnleashedBuilder(string name, string guid) : base(TelemaDemo, name, guid)
            {
                Definition.GuiPresentation.Title += " Unleashed";
                Definition.PredefinedParty.Clear();
            }

            private static CampaignDefinition CreateAndAddToDB(string name, string guid)
            {
                return new TelemaCampaignUnleashedBuilder(name, guid).AddToDB();
            }

            public static readonly CampaignDefinition TelemaDemoUnleashed = CreateAndAddToDB(TelemaDemoUnleashedName, TelemaDemoUnleashedGuid);
        }

        internal static void SwitchTelemaCampaign()
        {
            TelemaDemo.GuiPresentation.SetHidden(!Main.Settings.EnableTelemaCampaign);
            TelemaCampaignUnleashedBuilder.TelemaDemoUnleashed.GuiPresentation.SetHidden(!Main.Settings.EnableTelemaCampaign);
        }

        internal static void Load()
        {
            Garrad.SetEditorOnly(false);
            Rhuad.SetEditorOnly(false);
            Vigdis.SetEditorOnly(false);
            Violet.SetEditorOnly(false);

            TelemaDemo.SetEditorOnly(false);

            SwitchTelemaCampaign();
        }
    }
}
