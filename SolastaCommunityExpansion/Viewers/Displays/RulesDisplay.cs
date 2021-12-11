using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class RulesDisplay
    {
        internal static void DisplayRules()
        {
            bool toggle;

            UI.Label("");
            UI.Label("SRD:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableSRDAdvantageRules;
            if (UI.Toggle("Uses official advantage / disadvantage rules", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDAdvantageRules = toggle;
            }

            toggle = Main.Settings.EnableSRDCombatSurpriseRules;
            if (UI.Toggle("Uses official combat surprise rules", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDCombatSurpriseRules = toggle;
                Main.Settings.EnableSRDCombatSurpriseRulesManyRolls = toggle; // makes many rolls default
            }

            if (Main.Settings.EnableSRDCombatSurpriseRules)
            {
                toggle = Main.Settings.EnableSRDCombatSurpriseRulesManyRolls;
                if (UI.Toggle("Rolls different " + "Stealth".orange() + " checks for each surprised / surprising character pairs", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableSRDCombatSurpriseRulesManyRolls = toggle;
                }
            }

            UI.Label("");

            toggle = Main.Settings.FullyControlAlliedConjurations;
            if (UI.Toggle("Fully controls conjurations " + "[animals, elementals, etc]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FullyControlAlliedConjurations = toggle;
                ConjurationsContext.Load();
            }

            toggle = Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack;
            if (UI.Toggle("Blinded".orange() + " condition doesn't allow attack of opportunity", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack = toggle;
                SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            }

            UI.Label("");
            UI.Label("House:".yellow());
            UI.Label("");

            toggle = Main.Settings.PickPocketEnabled;
            if (UI.Toggle("Adds pickpocketable loot [suggested if " + "Pickpocket".orange() + " feat is enabled]", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.PickPocketEnabled = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            toggle = Main.Settings.EnableUniversalSylvanArmor;
            if (UI.Toggle("Allows any class to wear sylvan armor", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableUniversalSylvanArmor = toggle;
                ItemOptionsContext.SwitchUniversalSylvanArmor();
            }

            toggle = Main.Settings.DruidNoMetalRestriction;
            if (UI.Toggle("Allows Druids to wear metal armor", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DruidNoMetalRestriction = toggle;
                DruidArmorContext.Switch(toggle);
            }

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle("Disables auto-equip of items in inventory", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            toggle = Main.Settings.EnableMagicStaffFoci;
            if (UI.Toggle("Makes all magic staves arcane foci " + "[except for Staff of Healing which is Universal]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMagicStaffFoci = toggle;
                ItemOptionsContext.SwitchMagicStaffFoci();
            }

            toggle = Main.Settings.ExactMerchantCostScaling;
            if (UI.Toggle("Scales merchant prices correctly / exactly", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.ExactMerchantCostScaling = toggle;
            }
        }
    }
}
