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
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersAberration.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Beasts".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersBeast.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Celestials".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersCelestial.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Constructs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersConstruct.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Dragons".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersDragon.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Elementals".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersElemental.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Fey".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersFey.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Fiend".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersFiend.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Giants".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersGiant.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Humanoids".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersHumanoid.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Monstrosities".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersMonstrosity.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Undead".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersUndead.md"), UI.Width(200f));
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Advanced"));
        UI.Label();

        UI.Label(Gui.Localize("ModUi/&AdvancedHelp"));
        UI.Label();
        
        toggle = Main.Settings.AddNewWeaponsAndRecipesToEditor;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableAdditionalItemsInDungeonMaker")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AddNewWeaponsAndRecipesToEditor = toggle;
        }

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
