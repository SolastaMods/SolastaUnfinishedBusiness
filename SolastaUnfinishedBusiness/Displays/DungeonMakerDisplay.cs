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

        var toggle = Main.Settings.EnableSortingDungeonMakerAssets;
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
        UI.ActionButton("Solasta Monsters docs".Bold().Khaki(),
            () => BootContext.OpenDocumentation("SolastaMonsters.md"), UI.Width((float)200));

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
