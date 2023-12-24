using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ProficienciesDisplay
{
    internal static void DisplayProficiencies()
    {
        UI.Label();
        
        OtherHeaders();

        var displayToggle = Main.Settings.DisplayFeatsToggle;
        var sliderPos = Main.Settings.FeatSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Feats"),
            FeatsContext.SwitchFeat,
            FeatsContext.Feats,
            Main.Settings.FeatEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: FeatsHeader);
        Main.Settings.DisplayFeatsToggle = displayToggle;
        Main.Settings.FeatSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayFeatGroupsToggle;
        sliderPos = Main.Settings.FeatGroupSliderPosition;
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
            });
        Main.Settings.DisplayFeatGroupsToggle = displayToggle;
        Main.Settings.FeatGroupSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayFightingStylesToggle;
        sliderPos = Main.Settings.FightingStyleSliderPosition;
        ModUi.DisplayDefinitions(
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
        ModUi.DisplayDefinitions(
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
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Metamagic"),
            MetamagicContext.SwitchMetamagic,
            MetamagicContext.Metamagic,
            Main.Settings.MetamagicEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: MetamagicHeader);
        Main.Settings.DisplayMetamagicToggle = displayToggle;
        Main.Settings.MetamagicSliderPosition = sliderPos;

        UI.Label();
    }

    private static void FeatsHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Feats docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessFeats.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Feats docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaFeats.md"), UI.Width((float)200));
        }

        UI.Label();
    }

    private static void FightingStylesHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Fighting Styles docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessFightingStyles.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Fighting Styles docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaFightingStyles.md"), UI.Width((float)200));
        }

        UI.Label();
    }

    private static void InvocationsHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Invocations docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessInvocations.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Invocations docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaInvocations.md"), UI.Width((float)200));
        }

        UI.Label();
    }

    private static void MetamagicHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Metamagic docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessMetamagic.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Metamagic docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMetamagic.md"), UI.Width((float)200));
        }

        UI.Label();
    }
    
    private static void OtherHeaders()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("Arcane Shots docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessArcaneShots.md"), UI.Width((float)200));
            2.Space();
            UI.ActionButton("Gambits docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessGambits.md"), UI.Width((float)200));
            2.Space();
            UI.ActionButton("Infusions docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessInfusions.md"), UI.Width((float)200));
        }
    }
}
