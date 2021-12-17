using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

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
            if (UI.Toggle("Use official advantage / disadvantage rules", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDAdvantageRules = toggle;
            }

            toggle = Main.Settings.EnableSRDCombatSurpriseRules;
            if (UI.Toggle("Use official combat surprise rules", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDCombatSurpriseRules = toggle;
                Main.Settings.EnableSRDCombatSurpriseRulesManyRolls = toggle; // makes many rolls default
            }

            if (Main.Settings.EnableSRDCombatSurpriseRules)
            {
                toggle = Main.Settings.EnableSRDCombatSurpriseRulesManyRolls;
                if (UI.Toggle("Roll different " + "Stealth".orange() + " checks for each surprised / surprising character pairs", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableSRDCombatSurpriseRulesManyRolls = toggle;
                }
            }

            UI.Label("");

            toggle = Main.Settings.FullyControlAlliedConjurations;
            if (UI.Toggle("Fully control conjurations " + "[animals, elementals, etc]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FullyControlAlliedConjurations = toggle;
                ConjurationsContext.Load();
            }

            if (Main.Settings.FullyControlAlliedConjurations)
            {
                toggle = Main.Settings.DismissControlledConjurationsWhenDeliberatelyDropConcentration;
                if (UI.Toggle("+ Dismiss fully controlled conjurations when deliberately dropping concentration".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.DismissControlledConjurationsWhenDeliberatelyDropConcentration = toggle;
                }
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

            toggle = Main.Settings.EnableUniversalSylvanArmor;
            if (UI.Toggle("Allow any class to wear sylvan armor", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableUniversalSylvanArmor = toggle;
                ItemOptionsContext.SwitchUniversalSylvanArmor();
            }

            toggle = Main.Settings.DruidNoMetalRestriction;
            if (UI.Toggle("Allow " + "Druid".orange() + " to wear metal armor", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DruidNoMetalRestriction = toggle;
                DruidArmorContext.Switch(toggle);
            }

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle("Disable auto-equip of items in inventory", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            toggle = Main.Settings.EnableMagicStaffFoci;
            if (UI.Toggle("Make all magic staves arcane foci " + "[except for Staff of Healing which is Universal]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMagicStaffFoci = toggle;
                ItemOptionsContext.SwitchMagicStaffFoci();
            }

            UI.Label("");

            toggle = Main.Settings.IncreaseNormalVisionSenseRange;
            if (UI.Toggle("Increase " + "Sense Normal Vision".orange() + " range to enable long range attacks " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.IncreaseNormalVisionSenseRange = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.PickPocketEnabled;
            if (UI.Toggle("Add pickpocketable loot [suggested if " + "Pickpocket".orange() + " feat is enabled]", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.PickPocketEnabled = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            toggle = Main.Settings.ExactMerchantCostScaling;
            if (UI.Toggle("Scale merchant prices correctly / exactly", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.ExactMerchantCostScaling = toggle;
            }

            toggle = Main.Settings.AllowStackedMaterialComponent;
            if (UI.Toggle("Allow stacked material component " + "[e.g. 2x500gp diamond is equivalent to 1000gp diamond]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowStackedMaterialComponent = toggle;
            }

            UI.Label("");
        }
    }
}
