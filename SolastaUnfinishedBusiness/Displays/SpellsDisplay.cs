using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class SpellsDisplay
{
    private const int ShowAll = -1;

    internal static int SpellLevelFilter { get; private set; } = ShowAll;

    private static void DisplaySpellsGeneral()
    {
        var toggle = Main.Settings.DisplaySpellsGeneralToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&General"), ref toggle, 200))
        {
            Main.Settings.DisplaySpellsGeneralToggle = toggle;
        }

        if (!Main.Settings.DisplaySpellsGeneralToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.AllowBladeCantripsToUseReach;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowBladeCantripsToUseReach"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowBladeCantripsToUseReach = toggle;
            SpellsContext.SwitchAllowBladeCantripsToUseReach();
        }

        toggle = Main.Settings.QuickCastLightCantripOnWornItemsFirst;
        if (UI.Toggle(Gui.Localize("ModUi/&QuickCastLightCantripOnWornItemsFirst"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.QuickCastLightCantripOnWornItemsFirst = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AddBleedingToLesserRestoration;
        if (UI.Toggle(Gui.Localize("ModUi/&AddBleedingToLesserRestoration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddBleedingToLesserRestoration = toggle;
            SpellsContext.SwitchAddBleedingToLesserRestoration();
        }

        toggle = Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowTargetingSelectionWhenCastingChainLightningSpell"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell = toggle;
            SpellsContext.SwitchAllowTargetingSelectionWhenCastingChainLightningSpell();
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
            SpellsContext.SwitchEnableUpcastConjureElementalAndFey();
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

        toggle = Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange;
        if (UI.Toggle(Gui.Localize("ModUi/&IllusionSpellsAutomaticallyFailAgainstTrueSightInRange"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange = toggle;
        }

        toggle = Main.Settings.RemoveRecurringEffectOnEntangle;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRecurringEffectOnEntangle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRecurringEffectOnEntangle = toggle;
            SpellsContext.SwitchRecurringEffectOnEntangle();
        }

        toggle = Main.Settings.RemoveHumanoidFilterOnHideousLaughter;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveHumanoidFilterOnHideousLaughter"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveHumanoidFilterOnHideousLaughter = toggle;
            SpellsContext.SwitchFilterOnHideousLaughter();
        }

        UI.Label();

        toggle = Main.Settings.ChangeSleetStormToCube;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeSleetStormToCube"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeSleetStormToCube = toggle;
            SpellsContext.SwitchChangeSleetStormToCube();
        }

        toggle = Main.Settings.UseHeightOneCylinderEffect;
        if (UI.Toggle(Gui.Localize("ModUi/&UseHeightOneCylinderEffect"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseHeightOneCylinderEffect = toggle;
            SpellsContext.SwitchUseHeightOneCylinderEffect();
        }

        toggle = Main.Settings.FixEldritchBlastRange;
        if (UI.Toggle(Gui.Localize("ModUi/&FixEldritchBlastRange"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixEldritchBlastRange = toggle;
            Tabletop2014Context.SwitchEldritchBlastRange();
        }

        toggle = Main.Settings.ModifyGravitySlam;
        if (UI.Toggle(Gui.Localize("ModUi/&ModifyGravitySlam"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ModifyGravitySlam = toggle && Main.Settings.EnablePullPushOnVerticalDirection;
            Tabletop2014Context.SwitchGravitySlam();
        }

        UI.Label();
        UI.Label();

        toggle = Main.Settings.EnablePreparedSpellsTables2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePreparedSpellsTables2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePreparedSpellsTables2024 = toggle;
            Tabletop2024Context.SwitchOneDndPreparedSpellsTables();
        }

        toggle = Main.Settings.EnableRitualOnAllCasters2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRitualOnAllCasters2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRitualOnAllCasters2024 = toggle;
            Tabletop2024Context.SwitchOneDndSpellRitualOnAllCasters();
        }

        UI.Label();

        toggle = Main.Settings.EnableOneDndBarkskinSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndBarkskinSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndBarkskinSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellBarkskin();
        }

        toggle = Main.Settings.EnableOneDndDamagingSpellsUpgrade;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndDamagingSpellsUpgrade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndDamagingSpellsUpgrade = toggle;
            Tabletop2024Context.SwitchOneDndDamagingSpellsUpgrade();
        }

        toggle = Main.Settings.EnableOneDndHealingSpellsUpgrade;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHealingSpellsUpgrade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHealingSpellsUpgrade = toggle;
            Tabletop2024Context.SwitchOneDndHealingSpellsUpgrade();
        }

        toggle = Main.Settings.EnableOneDndDivineFavorSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndDivineFavorSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndDivineFavorSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellDivineFavor();
        }

        toggle = Main.Settings.EnableOneDndGuidanceSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndGuidanceSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndGuidanceSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellGuidance();
        }

        toggle = Main.Settings.EnableOneDndHideousLaughterSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHideousLaughterSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHideousLaughterSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellHideousLaughter();
        }

        toggle = Main.Settings.EnableOneDndHuntersMarkSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHuntersMarkSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHuntersMarkSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellHuntersMark();
        }

        toggle = Main.Settings.EnableOneDndLesserRestorationSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndLesserRestorationSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndLesserRestorationSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellLesserRestoration();
        }

        toggle = Main.Settings.EnableOneDndMagicWeaponSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndMagicWeaponSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndMagicWeaponSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellMagicWeapon();
        }

        toggle = Main.Settings.EnableOneDndPowerWordStunSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndPowerWordStunSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndPowerWordStunSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellPowerWordStun();
        }

        toggle = Main.Settings.EnableOneDndSpareTheDyingSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndSpareTheDyingSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndSpareTheDyingSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellSpareTheDying();
        }

        toggle = Main.Settings.EnableOneDndSpiderClimbSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndSpiderClimbSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndSpiderClimbSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellSpiderClimb();
        }

        toggle = Main.Settings.EnableOneDndStoneSkinSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndStoneSkinSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndStoneSkinSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellStoneSkin();
        }

        toggle = Main.Settings.EnableOneDndTrueStrikeCantrip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndTrueStrikeCantrip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndTrueStrikeCantrip = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AllowHasteCasting;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowHasteCasting"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowHasteCasting = toggle;
            SpellsContext.SwitchHastedCasing();
        }

        toggle = Main.Settings.AllowStackedMaterialComponent;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowStackedMaterialComponent"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowStackedMaterialComponent = toggle;
        }

        toggle = Main.Settings.EnableRelearnSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRelearnSpells"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRelearnSpells = toggle;
        }
    }

    internal static void DisplaySpells()
    {
        UI.Label();

        UI.ActionButton(Gui.Localize("ModUi/&DocsSpells").Bold().Khaki(),
            () => UpdateContext.OpenDocumentation("Spells.md"), UI.Width(189f));

        UI.Label();

        DisplaySpellsGeneral();

        UI.Label();

        var intValue = SpellLevelFilter;
        // ReSharper disable once InvertIf
        if (UI.Slider(Gui.Localize("ModUi/&SpellLevelFilter"), ref intValue, ShowAll, 9, ShowAll))
        {
            SpellLevelFilter = intValue;
            SpellsContext.RecalculateDisplayedSpells();
        }

        UI.Label();

        var toggle = Main.Settings.AllowDisplayingOfficialSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDisplayingOfficialSpells"), ref toggle,
                UI.Width(ModUi.PixelsPerColumn)))
        {
            Main.Settings.AllowDisplayingOfficialSpells = toggle;
            SpellsContext.RecalculateDisplayedSpells();
        }

        toggle = Main.Settings.AllowDisplayingNonSuggestedSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDisplayingNonSuggestedSpells"), ref toggle,
                UI.Width(ModUi.PixelsPerColumn)))
        {
            Main.Settings.AllowDisplayingNonSuggestedSpells = toggle;
            SpellsContext.RecalculateDisplayedSpells();
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            var displaySpellListsToggle = Main.Settings.DisplaySpellListsToggle.All(x => x.Value);

            toggle = displaySpellListsToggle;
            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var key in Main.Settings.DisplaySpellListsToggle.Keys.ToHashSet())
                {
                    Main.Settings.DisplaySpellListsToggle[key] = toggle;
                }
            }

            toggle = SpellsContext.IsSuggestedSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectSuggested"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SpellsContext.SelectSuggestedSet(toggle);
            }

            toggle = SpellsContext.IsTabletopSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SpellsContext.SelectTabletopSet(toggle);
            }

            if (displaySpellListsToggle)
            {
                toggle = SpellsContext.IsAllSetSelected();
                if (UI.Toggle(Gui.Localize("ModUi/&SelectDisplayed"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    SpellsContext.SelectAllSet(toggle);
                }
            }
        }

        UI.Div();

        foreach (var kvp in SpellsContext.SpellLists)
        {
            var spellListDefinition = kvp.Value;
            var spellListContext = SpellsContext.SpellListContextTab[spellListDefinition];
            var name = spellListDefinition.name;
            var displayToggle = Main.Settings.DisplaySpellListsToggle[name];
            var sliderPos = Main.Settings.SpellListSliderPosition[name];
            var spellEnabled = Main.Settings.SpellListSpellEnabled[name];
            var allowedSpells = spellListContext.DisplayedSpells;

            ModUi.DisplayDefinitions(
                kvp.Key.Khaki(),
                spellListContext.Switch,
                allowedSpells,
                spellEnabled,
                ref displayToggle,
                ref sliderPos,
                additionalRendering: AdditionalRendering);

            Main.Settings.DisplaySpellListsToggle[name] = displayToggle;
            Main.Settings.SpellListSliderPosition[name] = sliderPos;

            continue;

            void AdditionalRendering()
            {
                toggle = spellListContext.IsSuggestedSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectSuggested"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectSuggestedSetInternal(toggle);
                }

                toggle = spellListContext.IsTabletopSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectTabletopSetInternal(toggle);
                }

                toggle = spellListContext.IsAllSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectDisplayed"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectAllSetInternal(toggle);
                }
            }
        }

        UI.Label();
    }
}
