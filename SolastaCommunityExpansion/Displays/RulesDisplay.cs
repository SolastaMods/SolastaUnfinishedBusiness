using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Spells;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class RulesDisplay
    {
        internal static void DisplayRules()
        {
            bool toggle;

            UI.Label("");
            UI.Label("SRD:".yellow());
            UI.Label("");

            toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
            if (UI.Toggle(Gui.Format("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AddBleedingToLesserRestoration;
            if (UI.Toggle(Gui.Format("ModUi/&AddBleedingToLesserRestoration"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AddBleedingToLesserRestoration = toggle;
                HouseSpellTweaks.AddBleedingToRestoration();
            }

            toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
            if (UI.Toggle(Gui.Format("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
                SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            }

            UI.Label("");

            toggle = Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell;
            if (UI.Toggle(Gui.Format("ModUi/&AllowTargetingSelectionWhenCastingChainLightningSpell"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell = toggle;
                SrdAndHouseRulesContext.AllowTargetingSelectionWhenCastingChainLightningSpell();
            }

            toggle = Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove;
            if (UI.Toggle(Gui.Format("ModUi/&BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove = toggle;
            }

            toggle = Main.Settings.EnableUpcastConjureElementalAndFey;
            if (UI.Toggle(Gui.Format("ModUi/&EnableUpcastConjureElementalAndFey"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableUpcastConjureElementalAndFey = toggle;
                Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = false;
                ConjurationsContext.Load();
            }

            if (Main.Settings.EnableUpcastConjureElementalAndFey)
            {
                toggle = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey;
                if (UI.Toggle(Gui.Format("ModUi/&OnlyShowMostPowerfulUpcastConjuredElementalOrFey"), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = toggle;
                }
            }

            toggle = Main.Settings.FixSorcererTwinnedLogic;
            if (UI.Toggle(Gui.Format("ModUi/&FixSorcererTwinnedLogic"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FixSorcererTwinnedLogic = toggle;
            }

            toggle = Main.Settings.FullyControlConjurations;
            if (UI.Toggle(Gui.Format("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FullyControlConjurations = toggle;
                ConjurationsContext.Load();
            }

            UI.Label("");
            UI.Label("House:".yellow());
            UI.Label("");

            toggle = Main.Settings.AllowAnyClassToWearSylvanArmor;
            if (UI.Toggle(Gui.Format("ModUi/&AllowAnyClassToWearSylvanArmor"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowAnyClassToWearSylvanArmor = toggle;
                ItemOptionsContext.SwitchUniversalSylvanArmor();
            }

            toggle = Main.Settings.AllowDruidToWearMetalArmor;
            if (UI.Toggle(Gui.Format("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowDruidToWearMetalArmor = toggle;
                ItemOptionsContext.SwitchDruidAllowMetalArmor();
            }

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle(Gui.Format("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            toggle = Main.Settings.MakeAllMagicStaveArcaneFoci;
            if (UI.Toggle(Gui.Format("ModUi/&MakeAllMagicStaveArcaneFoci"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.MakeAllMagicStaveArcaneFoci = toggle;
                ItemOptionsContext.SwitchMagicStaffFoci();
            }

            UI.Label("");

            toggle = Main.Settings.IncreaseSenseNormalVision;
            if (UI.Toggle(Gui.Format("ModUi/&IncreaseSenseNormalVision"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.IncreaseSenseNormalVision = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.QuickCastLightCantripOnWornItemsFirst;
            if (UI.Toggle(Gui.Format("ModUi/&QuickCastLightCantripOnWornItemsFirst"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.QuickCastLightCantripOnWornItemsFirst = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AddPickpocketableLoot;
            if (UI.Toggle(Gui.Format("ModUi/&AddPickpocketableLoot"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AddPickpocketableLoot = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            toggle = Main.Settings.AllowStackedMaterialComponent;
            if (UI.Toggle(Gui.Format("ModUi/&AllowStackedMaterialComponent"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.AllowStackedMaterialComponent = toggle;
            }

            toggle = Main.Settings.ScaleMerchantPricesCorrectly;
            if (UI.Toggle(Gui.Format("ModUi/&ScaleMerchantPricesCorrectly"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.ScaleMerchantPricesCorrectly = toggle;
            }

            UI.Label("");
        }
    }
}
