using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Displays;

internal static class RulesDisplay
{
    private static readonly string[] CriticalHitOptions = ["0", "1", "2", "3"];
    private static readonly string[] SenseNormalVisionOptions = ["12", "24", "48"];

    internal static void DisplayRules()
    {
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&SRD"));
        UI.Label();

        var toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
            Main.Settings.UseOfficialFlankingRulesAlsoForRanged = false;
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

        toggle = Main.Settings.UseOfficialFoodRationsWeight;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFoodRationsWeight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialFoodRationsWeight = toggle;
            SrdAndHouseRulesContext.SwitchOfficialFoodRationsWeight();
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

        toggle = Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&KeepStealthOnHeroIfPerceivedDuringSurpriseAttack"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack = toggle;
        }

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

        toggle = Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowTargetingSelectionWhenCastingChainLightningSpell"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell = toggle;
            SrdAndHouseRulesContext.SwitchAllowTargetingSelectionWhenCastingChainLightningSpell();
        }

        toggle = Main.Settings.RemoveHumanoidFilterOnHideousLaughter;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveHumanoidFilterOnHideousLaughter"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveHumanoidFilterOnHideousLaughter = toggle;
            SrdAndHouseRulesContext.SwitchFilterOnHideousLaughter();
        }

        toggle = Main.Settings.AddBleedingToLesserRestoration;
        if (UI.Toggle(Gui.Localize("ModUi/&AddBleedingToLesserRestoration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddBleedingToLesserRestoration = toggle;
            SrdAndHouseRulesContext.SwitchAddBleedingToLesserRestoration();
        }

        toggle = Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove;
        if (UI.Toggle(Gui.Localize("ModUi/&BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove = toggle;
        }

        toggle = Main.Settings.RemoveRecurringEffectOnEntangle;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRecurringEffectOnEntangle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRecurringEffectOnEntangle = toggle;
            SrdAndHouseRulesContext.SwitchRecurringEffectOnEntangle();
        }

        toggle = Main.Settings.EnableUpcastConjureElementalAndFey;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableUpcastConjureElementalAndFey"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableUpcastConjureElementalAndFey = toggle;
            Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = false;
            SrdAndHouseRulesContext.SwitchEnableUpcastConjureElementalAndFey();
        }

        if (Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            toggle = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey;
            if (UI.Toggle(Gui.Localize("ModUi/&OnlyShowMostPowerfulUpcastConjuredElementalOrFey"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.ChangeSleetStormToCube;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeSleetStormToCube"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeSleetStormToCube = toggle;
            SrdAndHouseRulesContext.SwitchChangeSleetStormToCube();
        }

        toggle = Main.Settings.UseHeightOneCylinderEffect;
        if (UI.Toggle(Gui.Localize("ModUi/&UseHeightOneCylinderEffect"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseHeightOneCylinderEffect = toggle;
            SrdAndHouseRulesContext.SwitchUseHeightOneCylinderEffect();
        }

        toggle = Main.Settings.FixEldritchBlastRange;
        if (UI.Toggle(Gui.Localize("ModUi/&FixEldritchBlastRange"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixEldritchBlastRange = toggle;
            SrdAndHouseRulesContext.SwitchEldritchBlastRange();
        }

        UI.Label();

        toggle = Main.Settings.EnableOneDndHealingSpellsBuf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHealingSpellsBuf"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHealingSpellsBuf = toggle;
            SrdAndHouseRulesContext.SwitchOneDndHealingSpellsBuf();
        }

        toggle = Main.Settings.FixRingOfRegenerationHealRate;
        if (UI.Toggle(Gui.Localize("ModUi/&FixRingOfRegenerationHealRate"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixRingOfRegenerationHealRate = toggle;
            SrdAndHouseRulesContext.SwitchRingOfRegenerationHealRate();
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&House"));
        UI.Label();

        toggle = Main.Settings.AllowAnyClassToUseArcaneShieldstaff;
        if (UI.Toggle(Gui.Localize("ModUi/&ArcaneShieldstaffOptions"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToUseArcaneShieldstaff = toggle;
            ItemCraftingMerchantContext.SwitchAttuneArcaneShieldstaff();
        }

        toggle = Main.Settings.IdentifyAfterRest;
        if (UI.Toggle(Gui.Localize("ModUi/&IdentifyAfterRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IdentifyAfterRest = toggle;
        }

        toggle = Main.Settings.IncreaseMaxAttunedItems;
        if (UI.Toggle(Gui.Localize("ModUi/&IncreaseMaxAttunedItems"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IncreaseMaxAttunedItems = toggle;
        }

        toggle = Main.Settings.RemoveAttunementRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveAttunementRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveAttunementRequirements = toggle;
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

        toggle = Main.Settings.StealthDoesNotBreakWithSubtle;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthDoesNotBreakWithSubtle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthDoesNotBreakWithSubtle = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AllowHasteCasting;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowHasteCasting"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowHasteCasting = toggle;
            SrdAndHouseRulesContext.SwitchHastedCasing();
        }

        toggle = Main.Settings.AllowStackedMaterialComponent;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowStackedMaterialComponent"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowStackedMaterialComponent = toggle;
        }

        toggle = Main.Settings.EnableCantripsTriggeringOnWarMagic;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCantripsTriggeringOnWarMagic"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCantripsTriggeringOnWarMagic = toggle;
        }

        toggle = Main.Settings.RemoveSchoolRestrictionsFromShadowCaster;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveSchoolRestrictionsFromShadowCaster"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveSchoolRestrictionsFromShadowCaster = toggle;
            SrdAndHouseRulesContext.SwitchSchoolRestrictionsFromShadowCaster();
        }

        toggle = Main.Settings.RemoveSchoolRestrictionsFromSpellBlade;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveSchoolRestrictionsFromSpellBlade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveSchoolRestrictionsFromSpellBlade = toggle;
            SrdAndHouseRulesContext.SwitchSchoolRestrictionsFromSpellBlade();
        }

        UI.Label();

        toggle = Main.Settings.AllowAnyClassToWearSylvanArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowAnyClassToWearSylvanArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToWearSylvanArmor = toggle;
            SrdAndHouseRulesContext.SwitchUniversalSylvanArmorAndLightbringer();
        }

        toggle = Main.Settings.AllowDruidToWearMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowDruidToWearMetalArmor = toggle;
            SrdAndHouseRulesContext.SwitchDruidAllowMetalArmor();
        }

        toggle = Main.Settings.AllowClubsToBeThrown;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowClubsToBeThrown"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowClubsToBeThrown = toggle;
            SrdAndHouseRulesContext.SwitchAllowClubsToBeThrown();
        }

        toggle = Main.Settings.IgnoreHandXbowFreeHandRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&IgnoreHandXbowFreeHandRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IgnoreHandXbowFreeHandRequirements = toggle;
        }

        toggle = Main.Settings.MakeAllMagicStaveArcaneFoci;
        if (UI.Toggle(Gui.Localize("ModUi/&MakeAllMagicStaveArcaneFoci"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.MakeAllMagicStaveArcaneFoci = toggle;
            SrdAndHouseRulesContext.SwitchMagicStaffFoci();
        }

        UI.Label();

        var intValue = Main.Settings.WildSurgeDieRollThreshold;
        if (UI.Slider(Gui.Localize("ModUi/&WildSurgeDieRollThreshold"), ref intValue, 1, 20,
                2, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.WildSurgeDieRollThreshold = intValue;
            SorcerousWildMagic.SwitchWildSurgeChanceDieThreshold();
        }

        UI.Label();

        toggle = Main.Settings.ChangeDragonbornElementalBreathUsages;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeDragonbornElementalBreathUsages"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeDragonbornElementalBreathUsages = toggle;
            CharacterContext.SwitchDragonbornElementalBreathUsages();
        }

        toggle = Main.Settings.EnableSignatureSpellsRelearn;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSignatureSpellsRelearn"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSignatureSpellsRelearn = toggle;
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

        toggle = Main.Settings.EnableCharactersOnFireToEmitLight;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCharactersOnFireToEmitLight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharactersOnFireToEmitLight = toggle;
            SrdAndHouseRulesContext.SwitchMagicStaffFoci();
        }

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

        toggle = Main.Settings.FullyControlConjurations;
        if (UI.Toggle(Gui.Localize("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FullyControlConjurations = toggle;
            SrdAndHouseRulesContext.SwitchFullyControlConjurations();
        }

        UI.Label();

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

        toggle = Main.Settings.QuickCastLightCantripOnWornItemsFirst;
        if (UI.Toggle(Gui.Localize("ModUi/&QuickCastLightCantripOnWornItemsFirst"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.QuickCastLightCantripOnWornItemsFirst = toggle;
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
