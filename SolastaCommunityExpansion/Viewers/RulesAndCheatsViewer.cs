using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion.Viewers
{
    public class RulesAndCheatsViewer : IMenuSelectablePage
    {
        public string Name => "Rules & Cheats";

        public int Priority => 5;

        private static readonly string reqRestart = "[requires restart to disable]".italic().red();

        public void DisplaySrdRules()
        {
            bool toggle;

            UI.Label("");
            UI.Label("SRD rules:".yellow());

            UI.Label("");

            toggle = Main.Settings.EnableSRDAdvantageRules;
            if (UI.Toggle("Uses official advantage/disadvantage rules", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDAdvantageRules = toggle;
            }

            toggle = Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack;
            if (UI.Toggle("Blinded".orange() + " condition doesn't allow attack of opportunity", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack = toggle;
                Models.SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            }
        }

        public void DisplayHouseRules()
        {
            bool toggle;

            UI.Label("");
            UI.Label("House rules:".yellow());

            UI.Label("");

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle("Disables auto-equip of items in inventory", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            toggle = Main.Settings.ExactMerchantCostScaling;
            if (UI.Toggle("Scales merchant prices correctly / exactly", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.ExactMerchantCostScaling = toggle;
            }

            //toggle = Main.Settings.EnableSRDAdvantageRules;
            //if (UI.Toggle("SRD: uses official advantage/disadvantage rules", ref toggle, 0, UI.AutoWidth()))
            //{
            //    Main.Settings.EnableSRDAdvantageRules = toggle;
            //}

            //toggle = Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack;
            //if (UI.Toggle("SRD: " + "Blinded".orange() + " condition doesn't allow attack of opportunity", ref toggle, 0, UI.AutoWidth()))
            //{
            //    Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack = toggle;
            //    Models.SrdRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            //}
        }

        public void DisplayCheats()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Cheats:".yellow());

            UI.Label("");

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required on level up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplaySrdRules();
            DisplayHouseRules();
            DisplayCheats();
        }
    }
}