using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays;

public static class EncountersDisplay
{
    private static bool showStats;

    private static bool showAttributes;

    private static readonly Dictionary<MonsterDefinition, bool> currentFeaturesMonster = new();

    private static readonly Dictionary<MonsterDefinition, bool> currentAttacksMonster = new();

    private static readonly Dictionary<RulesetCharacterHero, bool> currentItemsHeroes = new();

    private static string SplitCamelCase(string str)
    {
        return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }

    private static void DisplayHeroStats(RulesetCharacterHero hero, string actionText, Action action)
    {
        var flip = false;
        var inventory = hero.CharacterInventory.EnumerateAllSlots(false, true);

        using (UI.HorizontalScope())
        {
            UI.ActionButton(actionText.bold().red(), action, UI.Width(30));
            UI.Label($"{hero.Name} {hero.SurName}".orange().bold(), UI.Width(240));
            UI.Label($"{hero.RaceDefinition.FormatTitle()} {hero.ClassesHistory[0].FormatTitle()}".white(),
                UI.Width(120));

            var attributesLabel = showAttributes ? "" : "Attributes";

            UI.DisclosureToggle(attributesLabel, ref showAttributes, attributesLabel.Length * 12);

            if (showAttributes)
            {
                UI.Label($"Str: {hero.GetAttribute(AttributeDefinitions.Strength).CurrentValue:0#}".white(),
                    UI.Width(48));
                UI.Label($"Con: {hero.GetAttribute(AttributeDefinitions.Constitution).CurrentValue:0#}".yellow(),
                    UI.Width(48));
                UI.Label($"Dex: {hero.GetAttribute(AttributeDefinitions.Dexterity).CurrentValue:0#}".white(),
                    UI.Width(48));
                UI.Label($"Int: {hero.GetAttribute(AttributeDefinitions.Intelligence).CurrentValue:0#}".yellow(),
                    UI.Width(48));
                UI.Label($"Wis: {hero.GetAttribute(AttributeDefinitions.Wisdom).CurrentValue:0#}".white(),
                    UI.Width(48));
                UI.Label($"Cha: {hero.GetAttribute(AttributeDefinitions.Charisma).CurrentValue:0#}".yellow(),
                    UI.Width(48));
            }

            var statsLabel = showStats ? "" : "Stats";

            UI.DisclosureToggle(statsLabel, ref showStats, statsLabel.Length * 12);

            if (showStats)
            {
                UI.Label($"AC: {hero.GetAttribute(AttributeDefinitions.ArmorClass).CurrentValue:0#}".white(),
                    UI.Width(48));
                UI.Label($"HD: {hero.MaxHitDiceCount():0#}{hero.MainHitDie}".yellow(), UI.Width(72));
                UI.Label($"XP: {hero.GetAttribute(AttributeDefinitions.Experience).CurrentValue}".white(),
                    UI.Width(72));
                UI.Label($"LV: {hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue:0#}".white(),
                    UI.Width(48));
            }

            currentItemsHeroes.TryGetValue(hero, out flip);

            if (UI.DisclosureToggle("Inventory", ref flip, 132))
            {
                currentItemsHeroes.AddOrReplace(hero, flip);
            }
        }

        currentItemsHeroes.TryGetValue(hero, out flip);

        if (flip)
        {
            using (UI.VerticalScope())
            {
                using (UI.HorizontalScope())
                {
                    UI.Space(30);
                    UI.Label("Inventory".bold().cyan());
                }

                foreach (var slot in inventory)
                {
                    if (slot.EquipedItem != null)
                    {
                        using (UI.HorizontalScope())
                        {
                            UI.Space(60);
                            UI.Label(slot.EquipedItem.ItemDefinition.FormatTitle(), UI.Width(192));
                        }
                    }
                }
            }
        }
    }

    private static void DisplayMonsterStats(MonsterDefinition monsterDefinition, string actionText,
        Action action)
    {
        var flip = false;

        using (UI.HorizontalScope())
        {
            UI.ActionButton(actionText.bold().red(), action, UI.Width(30));
            UI.Label($"{monsterDefinition.FormatTitle()}".orange().bold(), UI.Width(240));
            UI.Label($"{SplitCamelCase(monsterDefinition.Alignment)}".white(), UI.Width(120));

            var attributesLabel = showAttributes ? "" : "Attributes";

            UI.DisclosureToggle(attributesLabel, ref showAttributes, attributesLabel.Length * 12);

            if (showAttributes)
            {
                UI.Label($"Str: {monsterDefinition.AbilityScores[0]:0#}".white(), UI.Width(48));
                UI.Label($"Con: {monsterDefinition.AbilityScores[1]:0#}".yellow(), UI.Width(48));
                UI.Label($"Dex: {monsterDefinition.AbilityScores[2]:0#}".white(), UI.Width(48));
                UI.Label($"Int: {monsterDefinition.AbilityScores[3]:0#}".yellow(), UI.Width(48));
                UI.Label($"Wis: {monsterDefinition.AbilityScores[4]:0#}".white(), UI.Width(48));
                UI.Label($"Cha: {monsterDefinition.AbilityScores[5]:0#}".yellow(), UI.Width(48));
            }

            var statsLabel = showStats ? "" : "Stats";

            UI.DisclosureToggle(statsLabel, ref showStats, statsLabel.Length * 12);

            if (showStats)
            {
                UI.Label($"AC: {monsterDefinition.ArmorClass}".white(), UI.Width(48));
                UI.Label($"HD: {monsterDefinition.HitDice:0#}{monsterDefinition.HitDiceType}".yellow(),
                    UI.Width(72));
                UI.Label($"CR: {monsterDefinition.ChallengeRating}".yellow(), UI.Width(72));
            }

            currentAttacksMonster.TryGetValue(monsterDefinition, out flip);

            if (UI.DisclosureToggle($"Attacks ({monsterDefinition.AttackIterations.Count:0#})", ref flip, 132))
            {
                currentAttacksMonster.AddOrReplace(monsterDefinition, flip);
            }

            currentFeaturesMonster.TryGetValue(monsterDefinition, out flip);

            if (UI.DisclosureToggle($"Features ({monsterDefinition.Features.Count:0#})", ref flip, 144))
            {
                currentFeaturesMonster.AddOrReplace(monsterDefinition, flip);
            }
        }

        currentFeaturesMonster.TryGetValue(monsterDefinition, out flip);
        if (flip)
        {
            using (UI.VerticalScope())
            {
                using (UI.HorizontalScope())
                {
                    UI.Space(30);
                    UI.Label("Features".bold().cyan());
                }

                foreach (var feature in monsterDefinition.Features)
                {
                    using (UI.HorizontalScope())
                    {
                        var title = feature.FormatTitle();

                        if (title == "None")
                        {
                            title = SplitCamelCase(feature.Name);
                        }

                        UI.Space(60);
                        UI.Label(title, UI.Width(192));
                    }
                }
            }
        }

        currentAttacksMonster.TryGetValue(monsterDefinition, out flip);
        if (flip)
        {
            using (UI.VerticalScope())
            {
                using (UI.HorizontalScope())
                {
                    UI.Space(30);
                    UI.Label("Attacks".bold().cyan());
                }

                foreach (var attackIteration in monsterDefinition.AttackIterations)
                {
                    using (UI.HorizontalScope())
                    {
                        var title = attackIteration.MonsterAttackDefinition.FormatTitle();

                        if (title == "None")
                        {
                            title = SplitCamelCase(attackIteration.MonsterAttackDefinition.name);
                        }

                        UI.Space(60);
                        UI.Label(title, UI.Width(192));
                        UI.Label($"action type: {attackIteration.MonsterAttackDefinition.ActionType}".green(),
                            UI.Width(120));
                        UI.Label($"reach: {attackIteration.MonsterAttackDefinition.ReachRange}".green(),
                            UI.Width(108));
                        UI.Label($"hit bonus: {attackIteration.MonsterAttackDefinition.ToHitBonus}".green(),
                            UI.Width(108));
                        if (attackIteration.MonsterAttackDefinition.MaxUses < 0)
                        {
                            UI.Label("max uses: inf".green(), UI.Width(108));
                        }
                        else
                        {
                            UI.Label($"max uses: {attackIteration.MonsterAttackDefinition.MaxUses}".green(),
                                UI.Width(108));
                        }

                        if (attackIteration.MonsterAttackDefinition.Magical)
                        {
                            UI.Label(TagsDefinitions.Magical.green(), UI.Width(108));
                        }
                    }
                }
            }
        }
    }

    internal static void DisplayEncountersGeneral()
    {
        bool toggle;

        UI.Label("");
        UI.Label("Controllers:".yellow());
        UI.Label("");

        UI.Label(". Note the encounters feature won't work in a Multiplayer session. Yet...");
        UI.Label("");

        toggle = Main.Settings.EnableEnemiesControlledByPlayer;
        if (UI.Toggle("Enable enemies controlled by players", ref toggle))
        {
            Main.Settings.EnableEnemiesControlledByPlayer = toggle;
        }

        toggle = Main.Settings.EnableHeroesControlledByComputer;
        if (UI.Toggle("Enable heroes controlled by computer", ref toggle))
        {
            Main.Settings.EnableHeroesControlledByComputer = toggle;

            if (toggle)
            {
                PlayerControllerContext.RefreshGuiState();
            }
        }

        if (Main.Settings.EnableHeroesControlledByComputer)
        {
            UI.Label("");

            if (Global.IsOffGame)
            {
                UI.Label("Load a game to modify heroes AI...".bold().red(), UI.AutoWidth());
            }
            else if (Global.IsMultiplayer)
            {
                UI.Label("You can only change controllers in a local session...".bold().red(), UI.AutoWidth());
            }
            else
            {
                var controllers = PlayerControllerContext.Controllers;
                var playerCharacters = PlayerControllerContext.PlayerCharacters;
                var playerCharactersChoices = PlayerControllerContext.PlayerCharactersChoices;

                for (var i = 0; i < playerCharacters.Count; i++)
                {
                    // Prevent captured closure 
                    var index = i;

                    UI.HStack(playerCharacters[index].Name, 1, () =>
                    {
                        if (UI.SelectionGrid(ref playerCharactersChoices[index], controllers, controllers.Length,
                                UI.Width(300)))
                        {
                            PlayerControllerContext.PlayerCharactersChoices = playerCharactersChoices;
                        }
                    });
                }
            }
        }

        UI.Label("");
        UI.Label("Encounters:".yellow());
        UI.Label("");

        UI.Label(". encounters only work in custom campaigns or locations");
        UI.Label(". use the Bestiary tab to add monsters to the challenge", UI.AutoWidth());
        UI.Label(". use the Characters Pool tab to add heroes as enemies to the challenge", UI.AutoWidth());
        UI.Label(". click " + "minus".italic() + " to remove participants from the group");
        UI.Label(
            ". pan the camera to the desired encounter location and press " + "ctrl-shift-(S)".cyan() +
            "pawn to place the enemies", UI.AutoWidth());
        UI.Label("");

        if (EncountersSpawnContext.EncounterCharacters.Count == 0)
        {
            UI.Label("Encounter table is empty...".red().bold());
        }
        else
        {
            for (var index = 0; index < EncountersSpawnContext.EncounterCharacters.Count; index++)
            {
                // Prevent captured closure 
                var index2 = index;

                if (EncountersSpawnContext.EncounterCharacters[index2] is RulesetCharacterMonster
                    rulesetCharacterMonster)
                {
                    DisplayMonsterStats(rulesetCharacterMonster.MonsterDefinition, "-",
                        () => EncountersSpawnContext.RemoveFromEncounter(index2));
                }
                else if (EncountersSpawnContext.EncounterCharacters[index2] is RulesetCharacterHero
                         rulesetCharacterHero)
                {
                    DisplayHeroStats(rulesetCharacterHero, "-",
                        () => EncountersSpawnContext.RemoveFromEncounter(index2));
                }
            }
        }

        UI.Label("");
    }

    internal static void DisplayBestiary()
    {
        UI.Label("");
        UI.Label(
            $". Click + to add up to {EncountersSpawnContext.MAX_ENCOUNTER_CHARACTERS} characters to the encounter list");
        UI.Label("");

        foreach (var monsterDefinition in EncountersSpawnContext.GetMonsters())
        {
            DisplayMonsterStats(monsterDefinition, "+",
                () => EncountersSpawnContext.AddToEncounter(monsterDefinition));
        }
    }

    internal static void DisplayNPCs()
    {
        using (UI.VerticalScope(UI.AutoWidth(), UI.AutoHeight()))
        {
            UI.Label("");
            UI.Label(
                $". Click + to add up to {EncountersSpawnContext.MAX_ENCOUNTER_CHARACTERS} characters to the encounter list");
            UI.Label("");

            foreach (var hero in EncountersSpawnContext.GetHeroes())
            {
                DisplayHeroStats(hero, "+", () => EncountersSpawnContext.AddToEncounter(hero));
            }
        }
    }
}
