using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class DungeonMakerDisplay
{
    internal static void DisplayDungeonMaker()
    {
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Basic"));
        UI.Label();

        UI.Label(Gui.Localize("ModUi/&DungeonMakerBasicHelp"));
        UI.Label();

        var toggle = Main.Settings.EnableLoggingInvalidReferencesInUserCampaigns;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLoggingInvalidReferencesInUserCampaigns"), ref toggle))
        {
            Main.Settings.EnableLoggingInvalidReferencesInUserCampaigns = toggle;
        }

        toggle = Main.Settings.EnableSortingDungeonMakerAssets;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSortingDungeonMakerAssets"), ref toggle))
        {
            Main.Settings.EnableSortingDungeonMakerAssets = toggle;
        }

        toggle = Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowGadgetsAndPropsToBePlacedAnywhere"), ref toggle))
        {
            Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere = toggle;
        }

        toggle = Main.Settings.UnleashEnemyAsNpc;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashEnemyAsNpc"), ref toggle))
        {
            Main.Settings.UnleashEnemyAsNpc = toggle;
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Aberrations".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersAberration.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Beasts".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersBeast.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Celestials".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersCelestial.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Constructs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersConstruct.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Dragons".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersDragon.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Elementals".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersElemental.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Fey".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersFey.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Fiend".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersFiend.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Giants".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersGiant.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Humanoids".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersHumanoid.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Monstrosities".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersMonstrosity.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Undead".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaMonstersUndead.md"), UI.Width(200f));
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Advanced"));
        UI.Label();

        UI.Label(Gui.Localize("ModUi/&AdvancedHelp"));
        UI.Label();

        toggle = Main.Settings.UnleashNpcAsEnemy;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashNpcAsEnemy"), ref toggle))
        {
            Main.Settings.UnleashNpcAsEnemy = toggle;
        }

        toggle = Main.Settings.EnableDungeonMakerModdedContent;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDungeonMakerModdedContent"), ref toggle))
        {
            Main.Settings.EnableDungeonMakerModdedContent = toggle;
        }

        UI.Label();
        UI.Label();
    }
}
