using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using UnityExplorer;
#if DEBUG
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;
#endif

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
#if DEBUG
    private static bool _displayPatches;
#endif

    // ReSharper disable once MemberCanBePrivate.Global
    internal static readonly List<(string, string)> CreditsTable =
    [
        ("Zappastuff",
            "gameplay, infrastructure, maintenance, mod UI, quality of life, rules, " +
            "backgrounds, feats, fighting styles, maneuvers, invocations, metamagic, spells, " +
            "Fairy, Half-elf, Tiefling, " +
            "Circle of the Ancient Forest, Circle of the Eternal Grove, Circle of the Stars, " +
            "College of Eloquence, College of Life, College of Swords, College of Thespian, College of Valor, " +
            "Innovation Artillerist, Innovation Vitriolist, Innovation Vivisectionist, " +
            "Martial Arcane Archer, Martial Banneret, Martial Guardian, Martial Psi Warrior, Martial Warlord, Martial Weapon Master, " +
            "Oath of Altruism, Oath of Dread, Oath of Thunder, " +
            "Path of the Elements, Path of the Reaver, Path of the Savagery, Path of the Yeoman, " +
            "Patron Celestial, Patron Moonlit, Patron Mountain, Patron Riftwalker, " +
            "Ranger Gloom Stalker, Ranger Hellwalker, Ranger Lightbearer, Ranger Sky Warrior, Ranger Survivalist, Ranger Wildmaster, " +
            "Roguish Acrobat, Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Raven, Roguish Slayer, Roguish Umbral Stalker, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Scion, Sorcerous Sorr-Akkath, Sorcerous Spellblade, " +
            "Way of the Discordance, Way of the Shadow, Way of the Tempest, Way of Weal and Woe, Way of Zen Archery, " +
            "Wizard Bladesinger, Wizard Deadmaster, " +
            "Level 20, Lighting and Obscurement, Multiclass"),

        ("HiddenHax",
            "QA master, SFX, sprites, homebrew design [" +
            "Circle of the Eternal Grove, " +
            "College of Swords, College of Thespian, College of Eloquence, " +
            "Martial Arcane Archer, Martial Guardian, Martial Psi Warrior, Martial Warlord, Martial Weapon Master, " +
            "Oath of Dread, " +
            "Path of the Elements, Path of the Reaver, Path of the Savagery, " +
            "Patron Moonlit, " +
            "Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Raven, Roguish Slayer, Roguish Umbral Stalker, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Psion, Sorcerous Sorr-Akkath, Sorcerous Spellblade, " +
            "Way of the Discordance, Way of the Shadow, Way of the Tempest, Way of Dragon, Way of Tempest, Way of Zen Archery]"),

        ("TPABOBAP",
            "behaviors, game UI, infrastructure, gameplay, quality of life, rules, " +
            "feats, fighting styles, maneuvers, invocations, metamagic, spells, " +
            "Innovation Armor, Innovation Grenadier, Innovation Weapon, Martial Tactician, Patron Elementalist, Patron Soulblade, Artificer"),

        ("ImpPhil", "api, builders, infrastructure, gameplay, quality of life, rules"),
        ("ChrisJohnDigital",
            "feats, fighting styles, items & crafting, Martial Eldritch Knight, Wizard Arcane Fighter, Wizard Spellmaster"),

        ("Haxermn", "spells, Domain Defiler, Domain Forge, Oath of Ancients, Oath of Hatred, Way of Dragon"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Grey Dwarf, Kobold, Sorcerous Divine Soul"),
        ("Nd", "College of Wardancer, Roguish Opportunist"),
        ("tivie", "Circle of the Moon, Path of the Totem Warrior"),
        ("ElAntonius", "feats, fighting styles, Ranger Arcanist"),
        ("Kloranger", "feats, spells, Wizard Graviturgist"),
        ("magicskysword", "chinese translations, asian languages support, Oath of Demon Hunter"),
        ("DreadMaker", "Circle of the Forest Guardian"),
        ("RedOrca", "Path of the Light"),
        ("HIEROT", "Patron Eldritch Surge"),

        ("Otearaisu", "Aasimar, Battleborn, Goliath, Oni, Wendigo, Wildling, Wyrmkin"),
        ("Remunos", "Ironborn Dwarf, Obsidian Dwarf"),
        ("Holic75", "Firbolg"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Andargor", "rules, quality of life"),
        ("TheRev", "quality of life"),
        ("Kiloku", "quality of life"),

        ("Earandil",
            "homebrew design [Patron Mountain, Path of the Savagery, Path of the Yeoman, Ranger Sky Warrior, Ranger Survivalist]"),
        ("DemonicDuck",
            "homebrew design [Innovation Vivisectionist, Oath of Thunder, Sorcerous Sorr-Akkath, Way of Weal and Woe]"),
        ("Taco", "sprites, homebrew design [Roguish Acrobat, Domain Defiler, Oath of Altruism]"),
        ("DubhHerder", "quality of life, homebrew design [Patron Elementalist, Patron Riftwalker]"),
        ("Stuffies12", "homebrew design [Ranger Hellwalker, Ranger Lightbearer]"),
        ("Vess", "homebrew design [Innovation Vitriolist]"),

        ("Narria", "modKit, Party Editor"),

        ("Artyoan", "pre-gen heroes portraits, sample portraits"),
        ("Thaladar", "sample portraits"),

        ("team-waldo", "korean translations"),
        ("Dovel", "russian translations"),
        ("Ermite_Crabe", "french translations")
    ];

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    private static void EnableUnityExplorerUi()
    {
        IsUnityExplorerEnabled = true;

        try
        {
            ExplorerStandalone.CreateInstance();
        }
        catch
        {
            // ignored
        }
    }

    internal static void DisplayCredits()
    {
        UI.Label();

        if (IsUnityExplorerInstalled && !IsUnityExplorerEnabled)
        {
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width(150f));
            UI.Label();
        }

#if DEBUG
        DiagnosticsDisplay.DisplayDiagnostics();

        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref _displayPatches, 200);
        UI.Label();

        if (_displayPatches)
        {
            DisplayPatches();
        }
        else
#endif
        {
            UI.Label(
                "<b><color=#D89555>SPECIAL THANKS:</color></b><i><color>Tactical Adventures / JetBrains / WoTC</color></i>");
            UI.Label();

            // credits
            foreach (var (author, content) in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(author.Orange(), UI.Width(150f));
                    UI.Label(content, UI.Width(740f));
                }
            }
        }

        UI.Label();
    }
}
