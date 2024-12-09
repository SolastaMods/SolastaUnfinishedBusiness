using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class RulesDisplay
{
    private static readonly string[] CriticalHitOptions = ["0", "1", "2", "3"];
    private static readonly string[] SenseNormalVisionOptions = ["12", "24", "48"];

    internal static void DisplayRules()
    {
        int intValue;

        UI.Label();
        UI.Label();

        var toggle = Main.Settings.EnableEpicPointsAndArray;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableEpicPointsAndArray"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableEpicPointsAndArray = toggle;
        }

        toggle = Main.Settings.EnableLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLevel20"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLevel20 = toggle;
        }

        toggle = Main.Settings.EnableMulticlass;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMulticlass"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMulticlass = toggle;
            Main.Settings.MaxAllowedClasses = MulticlassContext.DefaultClasses;
            Main.Settings.EnableMinInOutAttributes = true;
            Main.Settings.DisplayAllKnownSpellsDuringLevelUp = true;
            Main.Settings.DisplayPactSlotsOnSpellSelectionPanel = true;
        }

        if (Main.Settings.EnableMulticlass)
        {
            UI.Label();

            intValue = Main.Settings.MaxAllowedClasses;
            if (UI.Slider(Gui.Localize("ModUi/&MaxAllowedClasses"), ref intValue,
                    2, MulticlassContext.MaxClasses, MulticlassContext.DefaultClasses, "", UI.AutoWidth()))
            {
                Main.Settings.MaxAllowedClasses = intValue;
            }

            UI.Label();

            toggle = Main.Settings.DisplayAllKnownSpellsDuringLevelUp;
            if (UI.Toggle(Gui.Localize("ModUi/&DisplayAllKnownSpellsDuringLevelUp"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisplayAllKnownSpellsDuringLevelUp = toggle;
            }

            toggle = Main.Settings.DisplayPactSlotsOnSpellSelectionPanel;
            if (UI.Toggle(Gui.Localize("ModUi/&DisplayPactSlotsOnSpellSelectionPanel"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisplayPactSlotsOnSpellSelectionPanel = toggle;
            }

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableMinInOutAttributes"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&MulticlassKeyHelp"));
        }

        UI.Label();

        toggle = Main.Settings.EnableActionSwitching;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableActionSwitching"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableActionSwitching = toggle;
        }

        toggle = Main.Settings.DontEndTurnAfterReady;
        if (UI.Toggle(Gui.Localize("ModUi/&DontEndTurnAfterReady"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DontEndTurnAfterReady = toggle;
        }

        toggle = Main.Settings.EnableUnlimitedInventoryActions;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableUnlimitedInventoryActions"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableUnlimitedInventoryActions = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableProneAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableProneAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableProneAction = toggle;
            Tabletop2014Context.SwitchProneAction();
        }

        toggle = Main.Settings.EnableGrappleAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableGrappleAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableGrappleAction = toggle;
            Tabletop2014Context.SwitchGrappleAction();
        }

        toggle = Main.Settings.EnableHelpAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHelpAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHelpAction = toggle;
            Tabletop2014Context.SwitchHelpPower();
        }

        toggle = Main.Settings.EnableRespecAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRespecAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRespecAction = toggle;
            ToolsContext.SwitchRespec();
        }

        toggle = Main.Settings.EnableUnarmedMainAttackAction;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableUnarmedMainAttackAction")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableUnarmedMainAttackAction = toggle;
        }

        UI.Label();

        toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
            Main.Settings.UseOfficialFlankingRulesAlsoForRanged = false;
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

        toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
        if (UI.Toggle(Gui.Localize("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
            Tabletop2014Context.SwitchConditionBlindedShouldNotAllowOpportunityAttack();
        }

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

        toggle = Main.Settings.AccountForAllDiceOnSavageAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&AccountForAllDiceOnSavageAttack"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AccountForAllDiceOnSavageAttack = toggle;
        }

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
                Tabletop2014Context.SwitchGravitySlam();
            }
        }

        toggle = Main.Settings.FullyControlConjurations;
        if (UI.Toggle(Gui.Localize("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FullyControlConjurations = toggle;
            Tabletop2014Context.SwitchFullyControlConjurations();
        }

        UI.Label();

        toggle = Main.Settings.EnableHigherGroundRules;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHigherGroundRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHigherGroundRules = toggle;
        }

        toggle = Main.Settings.EnableSurprisedToEnforceDisadvantage;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSurprisedToEnforceDisadvantage"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSurprisedToEnforceDisadvantage = toggle;
            Tabletop2024Context.SwitchSurprisedEnforceDisadvantage();
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
        }

        toggle = Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition;
        if (UI.Toggle(Gui.Localize("ModUi/&ColdResistanceAlsoGrantsImmunityToChilledCondition"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition = toggle;
            Tabletop2014Context.SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        }

        toggle = Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition;
        if (UI.Toggle(Gui.Localize("ModUi/&ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition = toggle;
            Tabletop2014Context.SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
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
