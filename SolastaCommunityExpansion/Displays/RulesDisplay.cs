using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Spells;

namespace SolastaCommunityExpansion.Displays;

internal static class RulesDisplay
{
    internal static void DisplayRules()
    {
        bool toggle;
        int intValue;

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&SRD"));
        UI.Label("");

        toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AddBleedingToLesserRestoration;
        if (UI.Toggle(Gui.Localize("ModUi/&AddBleedingToLesserRestoration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddBleedingToLesserRestoration = toggle;
            HouseSpellTweaks.AddBleedingToRestoration();
        }

        toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
        if (UI.Toggle(Gui.Localize("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
            SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        }

        UI.Label("");

        toggle = Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowTargetingSelectionWhenCastingChainLightningSpell"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell = toggle;
            SrdAndHouseRulesContext.AllowTargetingSelectionWhenCastingChainLightningSpell();
        }

        toggle = Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove;
        if (UI.Toggle(Gui.Localize("ModUi/&BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove = toggle;
        }

        toggle = Main.Settings.EnableUpcastConjureElementalAndFey;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableUpcastConjureElementalAndFey"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableUpcastConjureElementalAndFey = toggle;
            Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = false;
            ConjurationsContext.Load();
        }

        if (Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            toggle = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey;
            if (UI.Toggle(Gui.Localize("ModUi/&OnlyShowMostPowerfulUpcastConjuredElementalOrFey"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = toggle;
            }
        }

        toggle = Main.Settings.FixSorcererTwinnedLogic;
        if (UI.Toggle(Gui.Localize("ModUi/&FixSorcererTwinnedLogic"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixSorcererTwinnedLogic = toggle;
        }

        toggle = Main.Settings.FullyControlConjurations;
        if (UI.Toggle(Gui.Localize("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FullyControlConjurations = toggle;
            ConjurationsContext.Load();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&House"));
        UI.Label("");

        toggle = Main.Settings.UseHeightOneCylinderEffect;
        if (UI.Toggle(Gui.Localize("ModUi/&UseHeightOneCylinderEffect"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseHeightOneCylinderEffect = toggle;
            HouseSpellTweaks.UseHeightOneCylinderEffect();
        }

        toggle = Main.Settings.RemoveConcentrationRequirementsFromAnySpell;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveConcentrationRequirementsFromAnySpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveConcentrationRequirementsFromAnySpell = toggle;
        }

        toggle = Main.Settings.RemoveHumanoidFilterOnHideousLaughter;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveHumanoidFilterOnHideousLaughter"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveHumanoidFilterOnHideousLaughter = toggle;
        }

        toggle = Main.Settings.RemoveRecurringEffectOnEntangle;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRecurringEffectOnEntangle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRecurringEffectOnEntangle = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AllowAnyClassToWearSylvanArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowAnyClassToWearSylvanArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToWearSylvanArmor = toggle;
            ItemOptionsContext.SwitchUniversalSylvanArmorAndLightbringer();
        }

        toggle = Main.Settings.AllowDruidToWearMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowDruidToWearMetalArmor = toggle;
            ItemOptionsContext.SwitchDruidAllowMetalArmor();
        }

        toggle = Main.Settings.DisableAutoEquip;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableAutoEquip = toggle;
        }

        toggle = Main.Settings.MakeAllMagicStaveArcaneFoci;
        if (UI.Toggle(Gui.Localize("ModUi/&MakeAllMagicStaveArcaneFoci"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.MakeAllMagicStaveArcaneFoci = toggle;
            ItemOptionsContext.SwitchMagicStaffFoci();
        }

        UI.Label("");

        intValue = Main.Settings.IncreaseSenseNormalVision;
        UI.Label(Gui.Localize("ModUi/&IncreaseSenseNormalVision"));
        if (UI.Slider(Gui.Localize("ModUi/&IncreaseSenseNormalVisionHelp"), ref intValue,
                HouseFeatureContext.DEFAULT_VISION_RANGE, HouseFeatureContext.MAX_VISION_RANGE,
                HouseFeatureContext.DEFAULT_VISION_RANGE, "", UI.AutoWidth()))
        {
            Main.Settings.IncreaseSenseNormalVision = intValue;
        }

        UI.Label("");

        toggle = Main.Settings.QuickCastLightCantripOnWornItemsFirst;
        if (UI.Toggle(Gui.Localize("ModUi/&QuickCastLightCantripOnWornItemsFirst"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.QuickCastLightCantripOnWornItemsFirst = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AddPickpocketableLoot;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPickpocketableLoot"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPickpocketableLoot = toggle;
            if (toggle)
            {
                PickPocketContext.Load();
            }
        }

        toggle = Main.Settings.AllowStackedMaterialComponent;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowStackedMaterialComponent"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowStackedMaterialComponent = toggle;
        }

        toggle = Main.Settings.ScaleMerchantPricesCorrectly;
        if (UI.Toggle(Gui.Localize("ModUi/&ScaleMerchantPricesCorrectly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ScaleMerchantPricesCorrectly = toggle;
        }

        UI.Label("");
    }
}
