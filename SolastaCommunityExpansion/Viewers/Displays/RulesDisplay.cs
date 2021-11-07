using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class RulesDisplay
    {
        private static readonly string reqRestart = "[requires restart]".italic().red();

        internal static void DisplayRules()
        {
            bool toggle;

            UI.Label("");
            UI.Label("SRD:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableSRDAdvantageRules;
            if (UI.Toggle("Uses official advantage / disadvantage rules", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDAdvantageRules = toggle;
            }

            toggle = Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack;
            if (UI.Toggle("Blinded".orange() + " condition doesn't allow attack of opportunity", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack = toggle;
                SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            }

            UI.Label("");
            UI.Label("House:".yellow());
            UI.Label("");

            toggle = Main.Settings.PickPocketEnabled;
            if (UI.Toggle("Adds pickpocketable loot", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.PickPocketEnabled = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle("Disables auto-equip of items in inventory", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Superior Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.ExactMerchantCostScaling;
            if (UI.Toggle("Scales merchant prices correctly / exactly", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.ExactMerchantCostScaling = toggle;
            }
        }
    }
}
