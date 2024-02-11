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
            "maintenance, mod UI, infrastructure, gameplay, rules, quality of life, backgrounds, " +
            "feats, fighting styles, invocations, metamagic, spells, Fairy, Half-elf, Tiefling, " +
            "Roguish Acrobat, Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Slayer, Roguish Umbral Stalker, " +
            "College of Audacity, College of Guts, College of Life, College of Thespian, College of Valiance, " +
            "Circle of the Ancient Forest, Circle of the Eternal Grove, Wizard Bladedancer, Wizard Deadmaster, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Sorr-Akkath, Oath of Dread, Oath of Thunder, " +
            "Path of the Elements, Path of the Reaver, Path of the Savagery, Path of the Yeoman, " +
            "Ranger Hellwalker, Ranger Lightbearer, Ranger Sky Warrior, Ranger Survivalist, Ranger Wildmaster, " +
            "Martial Arcane Archer, Martial Guardian, Martial Royal Knight, Martial Warlord, Martial Weapon Master, " +
            "Patron Moonlit, Way of the Discordance, Way of the Silhouette, Way of the Tempest, Way of Weal and Woe, Way of Zen Archery, " +
            "Innovation Artillerist, Innovation Vitriolist, Innovation Vivisectionist, Patron Celestial, Patron Mountain, " +
            "Lighting and Obscurement, Level 20, Multiclass"),

        ("TPABOBAP",
            "custom behaviors, game UI, infrastructure, gameplay, rules, quality of life, " +
            "feats, fighting styles, invocations, metamagic, spells, quality of life, " +
            "Innovation Armor, Innovation Grenadier, Innovation Weapon, " +
            "Patron Elementalist, Patron Moonlit, Patron Riftwalker, Patron Soulblade, " +
            "Martial Tactician, Inventor, Multiclass"),

        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("ChrisJohnDigital",
            "builders, gameplay, feats, fighting styles, Wizard Arcane Fighter, Wizard Spellmaster, Martial Spell Shield"),

        ("HiddenHax",
            "QA, sprites [spells, powers], homebrew design [feats, spells, Circle of the Eternal Grove, " +
            "College of Audacity, College of Thespian, College of Valiance, Oath of Dread, " +
            "Path of the Elemental Fury, Path of the Reaver, Path of the Savagery, Patron Moonlit, " +
            "Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Slayer, Roguish Umbral Stalker, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Psion, Sorcerous Sorr-Akkath, " +
            "Martial Guardian, Martial Warlord, Martial Weapon Master, " +
            "Way of the Discordance, Way of Dragon, Way of Tempest, Way of Zen Archery]"),

        ("DemonicDuck",
            "QA, homebrew design [rules, feats, fighting styles, spells, Innovation Vivisectionist, Oath of the Hammer, Sorcerous Sorr-Akkath, Way of Weal and Woe]"),

        ("Earandil",
            "homebrew design [Patron Mountain, Path of the Savagery, Path of the Yeoman, Ranger Sky Warrior, Ranger Survivalist]"),

        ("Nd", "College of Wardancer, Roguish Opportunist, Roguish Raven"),
        ("Haxermn", "spells, Domain Defiler, Domain Smith, Oath of Ancient, Oath of Hatred, Way of Dragon"),
        ("Otearaisu", "Battleborn, Malakh, Oligath, Oni, Wendigo, Wildling, Wyrmkin"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Draconic Kobold, Grey Dwarf, Sorcerous Divine Heart"),
        ("Narria", "modKit, mod UI improvements, Party Editor"),
        ("tivie", "Circle of the Night, Path of the Spirits"),
        ("Remunos", "Ironborn Dwarf, Obsidian Dwarf"),
        ("ElAntonius", "feats, fighting styles, Ranger Arcanist"),
        ("Kloranger", "feats, spells, Wizard Graviturgist"),
        ("magicskysword", "chinese translations, asian languages support, Oath of Demon Hunter"),
        ("HIEROT", "fixes, Patron Eldritch Surge"),
        ("DreadMaker", "Circle of the Forest Guardian"),
        ("RedOrca", "Path of the Light"),
        ("Kiloku", "quality of life"),
        ("Andargor", "rules, quality of life"),
        ("TheRev", "quality of life"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Stuffies12", "homebrew design [Ranger Hellwalker, Ranger Lightbearer]"),
        ("Vess", "QA, homebrew design [Innovation Vitriolist]"),
        ("Holic75", "spells, Bolgrif"),
        ("Artyoan", "pre-gen heroes and sample portraits"),
        ("Thaladar", "sample portraits"),
        ("Taco",
            "sprites [fighting styles, powers, spells, subclasses], homebrew design [feats, Roguish Acrobat, Defiler Domain, Oath of Altruism]"),
        ("DubhHerder", "quality of life, spells, homebrew design [Patron Elementalist, Patron Riftwalker]"),
        ("team-waldo", "official korean font and translations"),
        ("akintos", "korean translations"),
        ("Dovel", "russian and non-official russian translations"),
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
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width((float)150));
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
                "<b><color=#D89555>SPECIAL THANKS:</color></b> Tactical Adventures / JetBrains <i><color=#F0DAA0>[development licenses]</color></i> / DemonicDuck, Gwizzz, Vess <i><color=#F0DAA0>[hardware acquisition]</color></i> / Balmz <i><color=#F0DAA0>[coffee eh!]</color></i>");

            UI.Label();

            // credits
            foreach (var (author, content) in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(author.Orange(), UI.Width((float)150));
                    UI.Label(content, UI.Width((float)740));
                }
            }
        }

        UI.Label();
    }
}
