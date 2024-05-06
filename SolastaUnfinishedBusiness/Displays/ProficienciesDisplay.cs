using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ProficienciesDisplay
{
    private static bool _displayTabletop;

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
            ref sliderPos,
            headerRendering: FeatsHeader);
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
            ref sliderPos,
            headerRendering: FightingStylesHeader);
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
            ref sliderPos,
            headerRendering: InvocationsHeader);
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
            ref sliderPos,
            headerRendering: MetamagicHeader);
        Main.Settings.DisplayMetamagicToggle = displayToggle;
        Main.Settings.MetamagicSliderPosition = sliderPos;

        _displayTabletop = isFeatTabletop && isFightingStyleTabletop && isInvocationTabletop && isMetamagicTabletop;

        UI.Label();
    }

    private static void FeatsHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Feats docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessFeats.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Feats docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaFeats.md"), UI.Width(200f));
        }

        UI.Label();
    }

    private static void FightingStylesHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Fighting Styles docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessFightingStyles.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Fighting Styles docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaFightingStyles.md"), UI.Width(200f));
        }

        UI.Label();
    }

    private static void InvocationsHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Invocations docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessInvocations.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Invocations docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaInvocations.md"), UI.Width(200f));
        }

        UI.Label();
    }

    private static void MetamagicHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Metamagic docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessMetamagic.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Metamagic docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMetamagic.md"), UI.Width(200f));
        }

        UI.Label();
    }

    private static void OtherHeaders()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("Arcane Shots docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessArcaneShots.md"), UI.Width(200f));
            2.Space();
            UI.ActionButton("Maneuvers docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessGambits.md"), UI.Width(200f));
            2.Space();
            UI.ActionButton("Infusions docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessInfusions.md"), UI.Width(200f));
        }

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
