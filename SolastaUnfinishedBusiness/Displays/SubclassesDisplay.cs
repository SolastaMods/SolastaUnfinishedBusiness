using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Displays;

internal static class SubclassesDisplay
{
    private static void DisplaySubclassesGeneral()
    {
        var toggle = Main.Settings.DisplaySubClassesGeneralToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&General"), ref toggle, 200))
        {
            Main.Settings.DisplaySubClassesGeneralToggle = toggle;
        }

        if (!Main.Settings.DisplaySubClassesGeneralToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.AllowAlliesToPerceiveRangerGloomStalkerInNaturalDarkness;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowAlliesToPerceiveRangerGloomStalkerInNaturalDarkness"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowAlliesToPerceiveRangerGloomStalkerInNaturalDarkness = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableBg3AbjurationArcaneWard;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBG3AbjurationArcaneWard"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBg3AbjurationArcaneWard = toggle;
            WizardAbjuration.SwapAbjurationBaldurGate3Mode();
        }

        if (Main.Settings.EnableBg3AbjurationArcaneWard)
        {
            UI.Label(Gui.Localize("ModUi/&EnableBG3AbjurationArcaneWardHelp"));
            UI.Label();
        }

        toggle = Main.Settings.EnableBardHealingBalladOnLongRest;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardHealingBalladOnLongRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardHealingBalladOnLongRest = toggle;
            Tabletop2014Context.SwitchBardHealingBalladOnLongRest();
        }

        toggle = Main.Settings.EnableRogueStrSaving;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueStrSaving"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueStrSaving = toggle;
        }

        UI.Label();

        toggle = Main.Settings.SwapAbjurationSavant;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapAbjurationSavant"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapAbjurationSavant = toggle;
            WizardAbjuration.SwapSavantAndSavant2024();
        }

        toggle = Main.Settings.SwapEvocationSavant;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapEvocationSavant"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapEvocationSavant = toggle;
            WizardEvocation.SwapSavantAndSavant2024();
        }

        toggle = Main.Settings.SwapEvocationPotentCantripAndSculptSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapEvocationPotentCantripAndSculptSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapEvocationPotentCantripAndSculptSpell = toggle;
            WizardEvocation.SwapEvocationPotentCantripAndSculptSpell();
        }

        toggle = Main.Settings.EnableMartialChampion2024;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapMartialChampion"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMartialChampion2024 = toggle;
            Tabletop2024Context.SwitchMartialChampion();
        }

        UI.Label();

        toggle = Main.Settings.RemoveSchoolRestrictionsFromShadowCaster;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveSchoolRestrictionsFromShadowCaster"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveSchoolRestrictionsFromShadowCaster = toggle;
            SubclassesContext.SwitchSchoolRestrictionsFromShadowCaster();
        }

        toggle = Main.Settings.RemoveSchoolRestrictionsFromSpellBlade;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveSchoolRestrictionsFromSpellBlade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveSchoolRestrictionsFromSpellBlade = toggle;
            SubclassesContext.SwitchSchoolRestrictionsFromSpellBlade();
        }

        UI.Label();

        var intValue = Main.Settings.WildSurgeDieRollThreshold;
        if (UI.Slider(Gui.Localize("ModUi/&WildSurgeDieRollThreshold"), ref intValue, 1, 20,
                2, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.WildSurgeDieRollThreshold = intValue;
            SorcerousWildMagic.SwitchWildSurgeChanceDieThreshold();
        }

        UI.Label();
    }

    internal static void DisplaySubclasses()
    {
        UI.Label();

        UI.ActionButton(Gui.Localize("ModUi/&DocsSubclasses").Bold().Khaki(),
            () => UpdateContext.OpenDocumentation("SubClasses.md"), UI.Width(189f));

        UI.Label();

        DisplaySubclassesGeneral();

        UI.Label();

        using (UI.HorizontalScope())
        {
            var toggle = Main.Settings.DisplayKlassToggle.All(x => x.Value);
            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var key in Main.Settings.DisplayKlassToggle.Keys.ToHashSet())
                {
                    Main.Settings.DisplayKlassToggle[key] = toggle;
                }
            }

            toggle = SubclassesContext.IsAllSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SubclassesContext.SelectAllSet(toggle);
            }

            toggle = SubclassesContext.IsTabletopSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SubclassesContext.SelectTabletopSet(toggle);
            }
        }

        UI.Div();

        foreach (var kvp in SubclassesContext.Klasses)
        {
            var displayName = kvp.Key;
            var klassName = kvp.Value.Item1;
            var klassDefinition = kvp.Value.Item2;
            var subclassListContext = SubclassesContext.KlassListContextTab[klassDefinition];
            var displayToggle = Main.Settings.DisplayKlassToggle[klassName];
            var sliderPos = Main.Settings.KlassListSliderPosition[klassName];
            var subclassEnabled = Main.Settings.KlassListSubclassEnabled[klassName];

            ModUi.DisplayDefinitions(
                displayName.Khaki(),
                subclassListContext.Switch,
                subclassListContext.AllSubClasses,
                subclassEnabled,
                ref displayToggle,
                ref sliderPos);

            Main.Settings.DisplayKlassToggle[klassName] = displayToggle;
            Main.Settings.KlassListSliderPosition[klassName] = sliderPos;
        }

        UI.Label();
    }
}
