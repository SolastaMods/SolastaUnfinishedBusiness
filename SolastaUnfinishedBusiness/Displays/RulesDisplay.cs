using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class RulesDisplay
{
    private static readonly string[] CriticalHitOptions = ["0", "1", "2", "3"];
    private static readonly string[] SenseNormalVisionOptions = ["12", "24", "48"];

    internal static void DisplayRules()
    {
        UI.Label();
        UI.Label();

        var toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
            Main.Settings.UseOfficialFlankingRulesAlsoForRanged = false;
        }

        toggle = Main.Settings.UseOfficialFoodRationsWeight;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFoodRationsWeight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialFoodRationsWeight = toggle;
            SrdAndHouseRulesContext.SwitchOfficialFoodRationsWeight();
        }

        toggle = Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialSmallRacesDisWithHeavyWeapons"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons = toggle;
        }

        toggle = Main.Settings.UseAlternateSpellPointsSystem;
        if (UI.Toggle(Gui.Localize("ModUi/&UseAlternateSpellPointsSystem"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseAlternateSpellPointsSystem = toggle;
            SpellPointsContext.SwitchFeatureDefinitionCastSpellSlots();
        }

        if (Main.Settings.UseAlternateSpellPointsSystem)
        {
            UI.Label(Gui.Localize("ModUi/&UseAlternateSpellPointsSystemHelp"));
        }

        UI.Label();

        toggle = Main.Settings.UseOfficialFlankingRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFlankingRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialFlankingRules = toggle;
        }

        if (Main.Settings.UseOfficialFlankingRules)
        {
            toggle = Main.Settings.UseMathFlankingRules;
            if (UI.Toggle(Gui.Localize("ModUi/&UseMathFlankingRules"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.UseMathFlankingRules = toggle;
            }

            if (Main.Settings.UseMathFlankingRules)
            {
                Main.Settings.UseOfficialFlankingRulesAlsoForReach = false;
                Main.Settings.UseOfficialFlankingRulesAlsoForRanged = false;
            }

            toggle = Main.Settings.UseOfficialFlankingRulesAlsoForReach;
            if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFlankingRulesAlsoForReach"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.UseOfficialFlankingRulesAlsoForReach = toggle;
            }

            if (Main.Settings.UseOfficialFlankingRulesAlsoForReach)
            {
                Main.Settings.UseMathFlankingRules = false;
            }

            toggle = Main.Settings.UseOfficialFlankingRulesAlsoForRanged;
            if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFlankingRulesAlsoForRanged"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.UseOfficialFlankingRulesAlsoForRanged = toggle;
            }

            if (Main.Settings.UseOfficialFlankingRulesAlsoForRanged)
            {
                Main.Settings.UseMathFlankingRules = false;
            }

            toggle = Main.Settings.UseOfficialFlankingRulesButAddAttackModifier;
            if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFlankingRulesButAddAttackModifier"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.UseOfficialFlankingRulesButAddAttackModifier = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.UseOfficialLightingObscurementAndVisionRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialObscurementRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialLightingObscurementAndVisionRules = toggle;
            Main.Settings.OfficialObscurementRulesInvisibleCreaturesCanBeTarget = toggle;
            Main.Settings.OfficialObscurementRulesCancelAdvDisPairs = toggle;
            Main.Settings.OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker = false;
            Main.Settings.OfficialObscurementRulesMagicalDarknessAsProjectileBlocker = false;
            Main.Settings.OfficialObscurementRulesTweakMonsters = toggle;
            LightingAndObscurementContext.SwitchOfficialObscurementRules();
        }

        if (Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            UI.Label(Gui.Localize("ModUi/&UseOfficialObscurementRulesHelp"));

            toggle = Main.Settings.OfficialObscurementRulesInvisibleCreaturesCanBeTarget;
            if (UI.Toggle(Gui.Localize("ModUi/&OfficialObscurementRulesInvisibleCreaturesCanBeTarget"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OfficialObscurementRulesInvisibleCreaturesCanBeTarget = toggle;
            }

            toggle = Main.Settings.OfficialObscurementRulesCancelAdvDisPairs;
            if (UI.Toggle(Gui.Localize("ModUi/&OfficialObscurementRulesCancelAdvDisPairs"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OfficialObscurementRulesCancelAdvDisPairs = toggle;
            }

            toggle = Main.Settings.OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker;
            if (UI.Toggle(Gui.Localize("ModUi/&OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker = toggle;
                LightingAndObscurementContext.SwitchHeavilyObscuredOnObscurementRules();
            }

            toggle = Main.Settings.OfficialObscurementRulesMagicalDarknessAsProjectileBlocker;
            if (UI.Toggle(Gui.Localize("ModUi/&OfficialObscurementRulesMagicalDarknessAsProjectileBlocker"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OfficialObscurementRulesMagicalDarknessAsProjectileBlocker = toggle;
                LightingAndObscurementContext.SwitchMagicalDarknessOnObscurementRules();
            }

            toggle = Main.Settings.OfficialObscurementRulesTweakMonsters;
            if (UI.Toggle(Gui.Localize("ModUi/&OfficialObscurementRulesTweakMonsters"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.OfficialObscurementRulesTweakMonsters = toggle;
                LightingAndObscurementContext.SwitchMonstersOnObscurementRules();
            }

            if (Main.Settings.OfficialObscurementRulesTweakMonsters)
            {
                UI.Label(Gui.Localize("ModUi/&OfficialObscurementRulesTweakMonstersHelp"));
            }
        }

        UI.Label();

        toggle = Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&KeepStealthOnHeroIfPerceivedDuringSurpriseAttack"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack = toggle;
        }

        toggle = Main.Settings.StealthDoesNotBreakWithSubtle;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthDoesNotBreakWithSubtle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthDoesNotBreakWithSubtle = toggle;
        }

        UI.Label();

        toggle = Main.Settings.StealthBreaksWhenAttackHits;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenAttackHits"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenAttackHits = toggle;
        }

        toggle = Main.Settings.StealthBreaksWhenAttackMisses;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenAttackMisses"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenAttackMisses = toggle;
        }

        UI.Label();

        toggle = Main.Settings.StealthBreaksWhenCastingMaterial;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenCastingMaterial"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenCastingMaterial = toggle;
        }

        toggle = Main.Settings.StealthBreaksWhenCastingVerbose;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenCastingVerbose"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenCastingVerbose = toggle;
        }

        toggle = Main.Settings.StealthBreaksWhenCastingSomatic;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenCastingSomatic"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenCastingSomatic = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AddDexModifierToEnemiesInitiativeRoll;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDexModifierToEnemiesInitiativeRoll"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddDexModifierToEnemiesInitiativeRoll = toggle;
            Main.Settings.EnemiesAlwaysRollInitiative = toggle;
        }

        if (Main.Settings.AddDexModifierToEnemiesInitiativeRoll)
        {
            toggle = Main.Settings.EnemiesAlwaysRollInitiative;
            if (UI.Toggle(Gui.Localize("ModUi/&EnemiesAlwaysRollInitiative"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnemiesAlwaysRollInitiative = toggle;
            }
        }

        toggle = Main.Settings.DontEndTurnAfterReady;
        if (UI.Toggle(Gui.Localize("ModUi/&DontEndTurnAfterReady"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DontEndTurnAfterReady = toggle;
        }

        UI.Label();

        toggle = Main.Settings.KeepInvisibilityWhenUsingItems;
        if (UI.Toggle(Gui.Localize("ModUi/&KeepInvisibilityWhenUsingItems"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.KeepInvisibilityWhenUsingItems = toggle;
        }

        toggle = Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange;
        if (UI.Toggle(Gui.Localize("ModUi/&IllusionSpellsAutomaticallyFailAgainstTrueSightInRange"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange = toggle;
        }

        toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
        if (UI.Toggle(Gui.Localize("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
            SrdAndHouseRulesContext.SwitchConditionBlindedShouldNotAllowOpportunityAttack();
        }

        UI.Label();

        var intValue = Main.Settings.TotalFeatsGrantedFirstLevel;
        if (UI.Slider(Gui.Localize("ModUi/&TotalFeatsGrantedFirstLevel"), ref intValue,
                CharacterContext.MinInitialFeats, CharacterContext.MaxInitialFeats, 0, "",
                UI.AutoWidth()))
        {
            Main.Settings.TotalFeatsGrantedFirstLevel = intValue;
            CharacterContext.SwitchFirstLevelTotalFeats();
        }

        UI.Label();

        toggle = Main.Settings.EnablesAsiAndFeat;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablesAsiAndFeat"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablesAsiAndFeat = toggle;
            CharacterContext.SwitchAsiAndFeat();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevels;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevels"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevels = toggle;
            CharacterContext.SwitchEveryFourLevelsFeats();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevelsMiddle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevelsMiddle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevelsMiddle = toggle;
            CharacterContext.SwitchEveryFourLevelsFeats(true);
        }

        UI.Label();

        toggle = Main.Settings.AccountForAllDiceOnFollowUpStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&AccountForAllDiceOnFollowUpStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AccountForAllDiceOnFollowUpStrike = toggle;
        }

        toggle = Main.Settings.AccountForAllDiceOnSavageAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&AccountForAllDiceOnSavageAttack"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AccountForAllDiceOnSavageAttack = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AllowFlightSuspend;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowFlightSuspend"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowFlightSuspend = toggle;
        }

        if (Main.Settings.AllowFlightSuspend)
        {
            toggle = Main.Settings.FlightSuspendWingedBoots;
            if (UI.Toggle(Gui.Localize("ModUi/&FlightSuspendWingedBoots"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FlightSuspendWingedBoots = toggle;
            }
        }

        toggle = Main.Settings.EnablePullPushOnVerticalDirection;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePullPushOnVerticalDirection"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePullPushOnVerticalDirection = toggle;
            if (!toggle)
            {
                Main.Settings.ModifyGravitySlam = false;
                SrdAndHouseRulesContext.ToggleGravitySlamModification();
            }
        }

        toggle = Main.Settings.FullyControlConjurations;
        if (UI.Toggle(Gui.Localize("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FullyControlConjurations = toggle;
            SrdAndHouseRulesContext.SwitchFullyControlConjurations();
        }

        UI.Label();

        toggle = Main.Settings.EnableHigherGroundRules;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHigherGroundRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHigherGroundRules = toggle;
        }

        toggle = Main.Settings.EnableTeleportToRemoveRestrained;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTeleportToRemoveRestrained"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableTeleportToRemoveRestrained = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableCharactersOnFireToEmitLight;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCharactersOnFireToEmitLight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharactersOnFireToEmitLight = toggle;
            SrdAndHouseRulesContext.SwitchMagicStaffFoci();
        }

        toggle = Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition;
        if (UI.Toggle(Gui.Localize("ModUi/&ColdResistanceAlsoGrantsImmunityToChilledCondition"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition = toggle;
            SrdAndHouseRulesContext.SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        }

        toggle = Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition;
        if (UI.Toggle(Gui.Localize("ModUi/&ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition = toggle;
            SrdAndHouseRulesContext.SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        }

        UI.Label();

        intValue = Main.Settings.SenseNormalVisionRangeMultiplier;

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&SenseNormalVisionRangeMultiplier"), UI.Width(275f));

            if (UI.SelectionGrid(ref intValue, SenseNormalVisionOptions, SenseNormalVisionOptions.Length, 3,
                    UI.Width(165f)))
            {
                Main.Settings.SenseNormalVisionRangeMultiplier = intValue;
            }
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Critical"));
        UI.Label();

        UI.Label(Gui.Localize("ModUi/&CriticalOption0"));
        UI.Label(Gui.Localize("ModUi/&CriticalOption1"));
        UI.Label(Gui.Localize("ModUi/&CriticalOption2"));
        UI.Label(Gui.Localize("ModUi/&CriticalOption3"));
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("Caption/&TargetFilteringAllyCreature"), UI.Width(100f));

            intValue = Main.Settings.CriticalHitModeAllies;
            if (UI.SelectionGrid(ref intValue, CriticalHitOptions, CriticalHitOptions.Length, 4, UI.Width(220f)))
            {
                Main.Settings.CriticalHitModeAllies = intValue;
            }
        }

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("Caption/&TargetFilteringEnemyCreature"), UI.Width(100f));

            intValue = Main.Settings.CriticalHitModeEnemies;
            if (UI.SelectionGrid(ref intValue, CriticalHitOptions, CriticalHitOptions.Length, 4, UI.Width(220f)))
            {
                Main.Settings.CriticalHitModeEnemies = intValue;
            }
        }

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("Action/&NeutralCreatureTitle"), UI.Width(100f));

            intValue = Main.Settings.CriticalHitModeNeutral;
            if (UI.SelectionGrid(ref intValue, CriticalHitOptions, CriticalHitOptions.Length, 4, UI.Width(220f)))
            {
                Main.Settings.CriticalHitModeNeutral = intValue;
            }
        }

        UI.Label();
    }
}
