using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ProficienciesDisplay
{
    private static bool _displayTabletop;

    private static void DisplayProficienciesGeneral()
    {
        var toggle = Main.Settings.DisplayProficienciesGeneralToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&General"), ref toggle, 200))
        {
            Main.Settings.DisplayProficienciesGeneralToggle = toggle;
        }

        if (!Main.Settings.DisplayProficienciesGeneralToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.DisableLevelPrerequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableClassPrerequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableLevelPrerequisitesOnModFeats = toggle;
        }

        toggle = Main.Settings.DisableRacePrerequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableRacePrerequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableRacePrerequisitesOnModFeats = toggle;
        }

        toggle = Main.Settings.DisableCastSpellPreRequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableCastSpellPreRequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableCastSpellPreRequisitesOnModFeats = toggle;
        }

        UI.Label();


        var intValue = Main.Settings.TotalFeatsGrantedFirstLevel;
        if (UI.Slider(Gui.Localize("ModUi/&TotalFeatsGrantedFirstLevel"), ref intValue,
                FeatsContext.MinInitialFeats, FeatsContext.MaxInitialFeats, 0, "",
                UI.AutoWidth()))
        {
            Main.Settings.TotalFeatsGrantedFirstLevel = intValue;
            FeatsContext.SwitchFirstLevelTotalFeats();
        }

        UI.Label();

        toggle = Main.Settings.EnablesAsiAndFeat;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablesAsiAndFeat"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablesAsiAndFeat = toggle;
            FeatsContext.SwitchAsiAndFeat();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevels;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevels"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevels = toggle;
            FeatsContext.SwitchEveryFourLevelsFeats();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevelsMiddle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevelsMiddle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevelsMiddle = toggle;
            FeatsContext.SwitchEveryFourLevelsFeats(true);
        }

        UI.Label();

        toggle = Main.Settings.AccountForAllDiceOnFollowUpStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&AccountForAllDiceOnFollowUpStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AccountForAllDiceOnFollowUpStrike = toggle;
        }

        toggle = Main.Settings.AllowCantripsTriggeringOnWarMagic;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCantripsTriggeringOnWarMagic"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowCantripsTriggeringOnWarMagic = toggle;
        }
    }

    internal static void DisplayProficiencies()
    {
        UI.Label();

        OtherHeaders();

        var displayToggle = Main.Settings.DisplayFeatGroupsToggle;
        var sliderPos = Main.Settings.FeatGroupSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&FeatGroups"),
            FeatsContext.SwitchFeatGroup,
            FeatsContext.FeatGroups,
            Main.Settings.FeatGroupEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: () =>
            {
                UI.Label(Gui.Localize("ModUi/&FeatGroupsHelp"));
                UI.Label();
            },
            displaySelectTabletop: false);
        Main.Settings.DisplayFeatGroupsToggle = displayToggle;
        Main.Settings.FeatGroupSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayFeatsToggle;
        sliderPos = Main.Settings.FeatSliderPosition;
        var isFeatTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Feats"),
            FeatsContext.SwitchFeat,
            FeatsContext.Feats,
            Main.Settings.FeatEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayFeatsToggle = displayToggle;
        Main.Settings.FeatSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayFightingStylesToggle;
        sliderPos = Main.Settings.FightingStyleSliderPosition;
        var isFightingStyleTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&FightingStyles"),
            FightingStyleContext.Switch,
            FightingStyleContext.FightingStyles,
            Main.Settings.FightingStyleEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayFightingStylesToggle = displayToggle;
        Main.Settings.FightingStyleSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayInvocationsToggle;
        sliderPos = Main.Settings.InvocationSliderPosition;
        var isInvocationTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Invocations"),
            InvocationsContext.SwitchInvocation,
            InvocationsContext.Invocations,
            Main.Settings.InvocationEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayInvocationsToggle = displayToggle;
        Main.Settings.InvocationSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayMetamagicToggle;
        sliderPos = Main.Settings.MetamagicSliderPosition;
        var isMetamagicTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Metamagic"),
            MetamagicContext.SwitchMetamagic,
            MetamagicContext.Metamagic,
            Main.Settings.MetamagicEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayMetamagicToggle = displayToggle;
        Main.Settings.MetamagicSliderPosition = sliderPos;

        _displayTabletop = isFeatTabletop && isFightingStyleTabletop && isInvocationTabletop && isMetamagicTabletop;

        UI.Label();
    }

    private static void OtherHeaders()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&DocsFeats").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Feats.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsFightingStyles").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("FightingStyles.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsInvocations").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Invocations.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsMetamagic").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Metamagic.md"), UI.Width(189f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&DocsArcaneShots").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("ArcaneShots.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsInfusions").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Infusions.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsManeuvers").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Maneuvers.md"), UI.Width(189f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsVersatilities").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Versatilities.md"), UI.Width(189f));
        }

        UI.Label();

        DisplayProficienciesGeneral();

        UI.Label();

        using (UI.HorizontalScope())
        {
            var toggle =
                Main.Settings.DisplayFeatsToggle &&
                Main.Settings.DisplayFeatGroupsToggle &&
                Main.Settings.DisplayFightingStylesToggle &&
                Main.Settings.DisplayInvocationsToggle &&
                Main.Settings.DisplayMetamagicToggle;

            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                Main.Settings.DisplayFeatsToggle = toggle;
                Main.Settings.DisplayFeatGroupsToggle = toggle;
                Main.Settings.DisplayFightingStylesToggle = toggle;
                Main.Settings.DisplayInvocationsToggle = toggle;
                Main.Settings.DisplayMetamagicToggle = toggle;
            }

            toggle =
                FeatsContext.Feats.Count == Main.Settings.FeatEnabled.Count &&
                FeatsContext.FeatGroups.Count == Main.Settings.FeatGroupEnabled.Count &&
                FightingStyleContext.FightingStyles.Count == Main.Settings.FightingStyleEnabled.Count &&
                InvocationsContext.Invocations.Count == Main.Settings.InvocationEnabled.Count &&
                MetamagicContext.Metamagic.Count == Main.Settings.MetamagicEnabled.Count;

            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var feat in FeatsContext.Feats)
                {
                    FeatsContext.SwitchFeat(feat, toggle);
                }

                foreach (var featGroup in FeatsContext.FeatGroups)
                {
                    FeatsContext.SwitchFeatGroup(featGroup, toggle);
                }

                foreach (var fightingStyles in FightingStyleContext.FightingStyles)
                {
                    FightingStyleContext.Switch(fightingStyles, toggle);
                }

                foreach (var invocation in InvocationsContext.Invocations)
                {
                    InvocationsContext.SwitchInvocation(invocation, toggle);
                }

                foreach (var metamagic in MetamagicContext.Metamagic)
                {
                    MetamagicContext.SwitchMetamagic(metamagic, toggle);
                }
            }

            toggle = _displayTabletop;
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var feat in FeatsContext.Feats)
                {
                    FeatsContext.SwitchFeat(feat,
                        toggle && ModUi.TabletopDefinitions.Contains(feat));
                }

                foreach (var featGroup in FeatsContext.FeatGroups)
                {
                    FeatsContext.SwitchFeatGroup(featGroup, toggle);
                }

                foreach (var fightingStyles in FightingStyleContext.FightingStyles)
                {
                    FightingStyleContext.Switch(fightingStyles,
                        toggle && ModUi.TabletopDefinitions.Contains(fightingStyles));
                }

                foreach (var invocation in InvocationsContext.Invocations)
                {
                    InvocationsContext.SwitchInvocation(invocation,
                        toggle && ModUi.TabletopDefinitions.Contains(invocation));
                }

                foreach (var metamagic in MetamagicContext.Metamagic)
                {
                    MetamagicContext.SwitchMetamagic(metamagic,
                        toggle && ModUi.TabletopDefinitions.Contains(metamagic));
                }
            }
        }

        UI.Div();
    }
}
