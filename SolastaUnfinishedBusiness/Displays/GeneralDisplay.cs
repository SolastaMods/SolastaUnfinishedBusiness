using System;
using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ToolsDisplay
{
    private static string ExportFileName { get; set; } =
        ServiceRepository.GetService<INetworkingService>().GetUserName();

    internal static void DisplayGameplay()
    {
        DisplayGeneral();
        UI.Label();
        DisplayMultiplayer();

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&TableTopHelp1"));
        UI.Label(Gui.Localize("ModUi/&TableTopHelp2"));
        UI.Label();
        UI.ActionButton(Gui.Localize("ModUi/&TableTopButton"), SelectTabletopSet, UI.AutoWidth());
        UI.Label();
        DisplayTabletop();
        UI.Label();
        DisplayOneDnd();
        UI.Label();
    }

    private static void SelectTabletopSet()
    {
        foreach (var background in BackgroundsContext.Backgrounds)
        {
            BackgroundsContext.Switch(background, ModUi.TabletopDefinitions.Contains(background));
        }

        foreach (var race in RacesContext.Races)
        {
            RacesContext.Switch(race, ModUi.TabletopDefinitions.Contains(race));
        }

        foreach (var subrace in RacesContext.Subraces)
        {
            RacesContext.SwitchSubrace(subrace, ModUi.TabletopDefinitions.Contains(subrace));
        }

        foreach (var feat in FeatsContext.Feats)
        {
            FeatsContext.SwitchFeat(feat, ModUi.TabletopDefinitions.Contains(feat));
        }

        foreach (var featGroup in FeatsContext.FeatGroups)
        {
            FeatsContext.SwitchFeatGroup(featGroup, true);
        }

        foreach (var fightingStyles in FightingStyleContext.FightingStyles)
        {
            FightingStyleContext.Switch(fightingStyles, ModUi.TabletopDefinitions.Contains(fightingStyles));
        }

        foreach (var invocation in InvocationsContext.Invocations)
        {
            InvocationsContext.SwitchInvocation(invocation, ModUi.TabletopDefinitions.Contains(invocation));
        }

        foreach (var metamagic in MetamagicContext.Metamagic)
        {
            MetamagicContext.SwitchMetamagic(metamagic, ModUi.TabletopDefinitions.Contains(metamagic));
        }

        SpellsContext.SelectTabletopSet(true);
        SubclassesContext.SelectTabletopSet(true);
    }

    private static void DisplayGeneral()
    {
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&Update"), () => UpdateContext.UpdateMod(),
                UI.Width(195f));
            UI.ActionButton(Gui.Localize("ModUi/&Rollback"), UpdateContext.DisplayRollbackMessage,
                UI.Width(195f));
            UI.ActionButton(Gui.Localize("ModUi/&Changelog"), UpdateContext.OpenChangeLog,
                UI.Width(195f));
        }

        UI.Label();

        var toggle = Main.Settings.EnablePcgRandom;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePcgRandom"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePcgRandom = toggle;
        }

        toggle = Main.Settings.EnableCustomPortraits;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCustomPortraits"), ref toggle))
        {
            Main.Settings.EnableCustomPortraits = toggle;
        }

        if (Main.Settings.EnableCustomPortraits)
        {
            UI.Label();

            UI.ActionButton(Gui.Localize("ModUi/&PortraitsOpenFolder"), () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = PortraitsContext.PortraitsFolder, UseShellExecute = true, Verb = "open"
                });
            }, UI.Width(292f));

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&EnableCustomPortraitsHelp"));
        }

        UI.Label();

        toggle = Main.Settings.DisableMultilineSpellOffering;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableMultilineSpellOffering"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableMultilineSpellOffering = toggle;
        }

        toggle = Main.Settings.DisableUnofficialTranslations;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableUnofficialTranslations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableUnofficialTranslations = toggle;
        }

        if (!Gui.GameCampaign)
        {
            return;
        }

        UI.Label();

        var gameCampaign = Gui.GameCampaign;

        if (!gameCampaign)
        {
            return;
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&IncreaseGameTimeBy"), UI.Width(300f));
            UI.ActionButton("1 hour", () => gameCampaign.UpdateTime(60 * 60), UI.Width(100f));
            UI.ActionButton("6 hours", () => gameCampaign.UpdateTime(60 * 60 * 6), UI.Width(100f));
            UI.ActionButton("12 hours", () => gameCampaign.UpdateTime(60 * 60 * 12), UI.Width(100f));
            UI.ActionButton("24 hours", () => gameCampaign.UpdateTime(60 * 60 * 24), UI.Width(100f));
        }
    }

    private static void DisplayMultiplayer()
    {
        var toggle = Main.Settings.DisplayMultiplayerToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Multiplayer"), ref toggle, 200))
        {
            Main.Settings.DisplayMultiplayerToggle = toggle;
        }

        if (!Main.Settings.DisplayMultiplayerToggle)
        {
            return;
        }

        UI.Label(Gui.Localize("ModUi/&SettingsHelp"));
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&SettingsExport"), () =>
            {
                Main.SaveSettings(ExportFileName);
            }, UI.Width(144f));

            UI.ActionButton(Gui.Localize("ModUi/&SettingsRemove"), () =>
            {
                Main.RemoveSettings(ExportFileName);
            }, UI.Width(144f));

            var text = ExportFileName;

            UI.ActionTextField(ref text, String.Empty, s => { ExportFileName = s; }, null, UI.Width(144f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&SettingsRefresh"), Main.LoadSettingFilenames, UI.Width(144f));
            UI.ActionButton(Gui.Localize("ModUi/&SettingsOpenFolder"), () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Main.SettingsFolder, UseShellExecute = true, Verb = "open"
                });
            }, UI.Width(292f));
        }

        if (Main.SettingsFiles.Length == 0)
        {
            return;
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&SettingsLoad"));
        UI.Label();

        var intValue = -1;
        if (UI.SelectionGrid(ref intValue, Main.SettingsFiles, Main.SettingsFiles.Length, 4, UI.Width(440f)))
        {
            Main.LoadSettings(Main.SettingsFiles[intValue]);
        }
    }

    private static void DisplayOneDnd()
    {
        var toggle = Main.Settings.DisplayOneDndToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&OneDnd"), ref toggle, 200))
        {
            Main.Settings.DisplayOneDndToggle = toggle;
        }

        if (!Main.Settings.DisplayOneDndToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.EnableSurprisedToEnforceDisadvantage;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSurprisedToEnforceDisadvantage"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSurprisedToEnforceDisadvantage = toggle;
            SrdAndHouseRulesContext.SwitchOneDndSurprisedEnforceDisadvantage();
        }

        toggle = Main.Settings.OneDndHealingPotionBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&OneDndHealingPotionBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.OneDndHealingPotionBonusAction = toggle;
            SrdAndHouseRulesContext.SwitchOneDndHealingPotionBonusAction();
        }


        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.ChangeBardicInspirationDurationToOneHour;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeBardicInspirationDurationToOneHour"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeBardicInspirationDurationToOneHour = toggle;
            SrdAndHouseRulesContext.SwitchOneDndBardicInspirationDurationToOneHour();
        }

        toggle = Main.Settings.EnableBardExpertiseOneLevelBefore;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardExpertiseOneLevelBefore"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardExpertiseOneLevelBefore = toggle;
            SrdAndHouseRulesContext.SwitchOneDndBardExpertiseOneLevelBefore();
        }

        toggle = Main.Settings.RemoveBardSongOfRest;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardSongOfRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardSongOfRest = toggle;
            SrdAndHouseRulesContext.SwitchOneDndRemoveBardSongOfRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BarbarianTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableBarbarianBrutalStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianBrutalStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianBrutalStrike = toggle;
            Main.Settings.DisableBarbarianBrutalCritical = toggle;
            CharacterContext.SwitchBarbarianBrutalStrike();
            CharacterContext.SwitchBarbarianBrutalCritical();
        }

        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            toggle = Main.Settings.DisableBarbarianBrutalCritical;
            if (UI.Toggle(Gui.Localize("ModUi/&DisableBarbarianBrutalCritical"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableBarbarianBrutalCritical = toggle;
                CharacterContext.SwitchBarbarianBrutalCritical();
            }
        }

        toggle = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRecklessSameBuffDebuffDuration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration = toggle;
            CharacterContext.SwitchBarbarianRecklessSameBuffDebuffDuration();
        }

        toggle = Main.Settings.EnableBarbarianRegainOneRageAtShortRest;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRegainOneRageAtShortRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRegainOneRageAtShortRest = toggle;
            CharacterContext.SwitchBarbarianRegainOneRageAtShortRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&DruidTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AllowDruidToWearMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowDruidToWearMetalArmor = toggle;
            SrdAndHouseRulesContext.SwitchDruidAllowMetalArmor();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&FighterTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AddFighterLevelToIndomitableSavingReroll;
        if (UI.Toggle(Gui.Localize("ModUi/&AddFighterLevelToIndomitableSavingReroll"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddFighterLevelToIndomitableSavingReroll = toggle;
            CharacterContext.SwitchFighterLevelToIndomitableSavingReroll();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&MonkTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMindToReplacePerfectSelf"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf = toggle;
            CharacterContext.SwitchMonkBodyAndMindToReplacePerfectSelf();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry = toggle;
            CharacterContext.SwitchMonkDoNotRequireAttackActionForFlurry();
        }

        toggle = Main.Settings.EnableMonkHeightenedMetabolism;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedMetabolism"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedMetabolism = toggle;
            CharacterContext.SwitchMonkHeightenedMetabolism();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack = toggle;
            CharacterContext.SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        }

        toggle = Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSuperiorDefenseToReplaceEmptyBody"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody = toggle;
            CharacterContext.SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&PaladinTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnablePaladinLayOnHandsAsBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinLayOnHandsAsBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinLayOnHandsAsBonusAction = toggle;
            SrdAndHouseRulesContext.SwitchOneDndPaladinLayOnHandAsBonusAction();
        }

        toggle = Main.Settings.EnablePaladinSmiteAsBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinSmiteAsBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinSmiteAsBonusAction = toggle;
        }

        toggle = Main.Settings.EnablePaladinSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinSpellCastingAtLevel1 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndPaladinLearnSpellCastingAtOne();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&RangerTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableRangerSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerSpellCastingAtLevel1 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndRangerLearnSpellCastingAtOne();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&RogueTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableRogueCunningStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueCunningStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueCunningStrike = toggle;
            CharacterContext.SwitchRogueCunningStrike();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WarlockTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWarlockMagicalCunningAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockMagicalCunningAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockMagicalCunningAtLevel2 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndWarlockMagicalCunningAtLevel2();
        }

        toggle = Main.Settings.SwapWarlockToUseOneDndInvocationProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapWarlockToUseOneDndInvocationProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapWarlockToUseOneDndInvocationProgression = toggle;
            SrdAndHouseRulesContext.SwitchOneDndWarlockInvocationsProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WizardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWizardToLearnScholarAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnScholarAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnScholarAtLevel2 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndWizardScholar();
        }

        toggle = Main.Settings.EnableWizardToLearnSchoolAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnSchoolAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnSchoolAtLevel3 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndWizardSchoolOfMagicLearningLevel();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("ModUi/&DocsSpells") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableOneDnDPreparedSpellsTables;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDnDPreparedSpellsTables"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDnDPreparedSpellsTables = toggle;
            SrdAndHouseRulesContext.SwitchOneDndPreparedSpellsTables();
        }

        toggle = Main.Settings.EnableOneDndHealingSpellsBuf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHealingSpellsBuf"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHealingSpellsBuf = toggle;
            SrdAndHouseRulesContext.SwitchOneDndHealingSpellsBuf();
        }

        toggle = Main.Settings.EnableRitualOnAllCasters;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRitualOnAllCasters"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRitualOnAllCasters = toggle;
            SrdAndHouseRulesContext.SwitchEnableRitualOnAllCasters();
        }

        toggle = Main.Settings.SwapOneDndBarkskinSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapOneDndBarkskinSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapOneDndBarkskinSpell = toggle;
            SrdAndHouseRulesContext.SwapOneDndBarkskinSpell();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("ModUi/&DocsSubclasses") + ":</color>");
        UI.Label();

        toggle = Main.Settings.SwapAbjurationSavant;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapAbjurationSavant"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapAbjurationSavant = toggle;
            WizardAbjuration.SwapSavantAndSavant2024();
        }

        toggle = Main.Settings.SwapEvocationSavant;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&SwapEvocationSavant"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapEvocationSavant = toggle;
            WizardEvocation.SwapSavantAndSavant2024();
        }

        toggle = Main.Settings.SwapEvocationPotentCantripAndSculptSpell;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&SwapEvocationPotentCantripAndSculptSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapEvocationPotentCantripAndSculptSpell = toggle;
            WizardEvocation.SwapEvocationPotentCantripAndSculptSpell();
        }
    }

    private static void DisplayTabletop()
    {
        var toggle = Main.Settings.DisplayTabletopToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Tabletop"), ref toggle, 200))
        {
            Main.Settings.DisplayTabletopToggle = toggle;
        }

        if (!Main.Settings.DisplayTabletopToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.EnableProneAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableProneAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableProneAction = toggle;
            CharacterContext.SwitchProneAction();
        }

        toggle = Main.Settings.EnableGrappleAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableGrappleAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableGrappleAction = toggle;
            GrappleContext.SwitchGrappleAction();
        }

        toggle = Main.Settings.EnableHelpAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHelpAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHelpAction = toggle;
            CharacterContext.SwitchHelpPower();
        }

        toggle = Main.Settings.EnableUnarmedMainAttackAction;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableUnarmedMainAttackAction")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableUnarmedMainAttackAction = toggle;
        }

        UI.Label();

        toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
        if (UI.Toggle(Gui.Localize("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
            SrdAndHouseRulesContext.SwitchConditionBlindedShouldNotAllowOpportunityAttack();
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

        toggle = Main.Settings.StealthBreaksWhenCastingVerbose;
        if (UI.Toggle(Gui.Localize("ModUi/&StealthBreaksWhenCastingVerbose"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StealthBreaksWhenCastingVerbose = toggle;
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

        toggle = Main.Settings.EnableTeleportToRemoveRestrained;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTeleportToRemoveRestrained"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableTeleportToRemoveRestrained = toggle;
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

        toggle = Main.Settings.UseOfficialFoodRationsWeight;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFoodRationsWeight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialFoodRationsWeight = toggle;
            SrdAndHouseRulesContext.SwitchOfficialFoodRationsWeight();
        }

        toggle = Main.Settings.FixRingOfRegenerationHealRate;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&FixRingOfRegenerationHealRate"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixRingOfRegenerationHealRate = toggle;
            SrdAndHouseRulesContext.SwitchRingOfRegenerationHealRate();
        }

        UI.Label();

        toggle = Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialSmallRacesDisWithHeavyWeapons"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableRangerNatureShroudAt10;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroudAt10"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroudAt10 = toggle;
            CharacterContext.SwitchRangerNatureShroud();
        }

        toggle = Main.Settings.EnableRogueSteadyAim;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSteadyAim"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSteadyAim = toggle;
            CharacterContext.SwitchRogueSteadyAim();
        }

        toggle = Main.Settings.EnableSorcererMagicalGuidance;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererMagicalGuidance"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererMagicalGuidance = toggle;
            CharacterContext.SwitchSorcererMagicalGuidance();
        }

        toggle = Main.Settings.EnableSorcererQuickenedAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererQuickenedAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererQuickenedAction = toggle;
        }

        if (Main.Settings.EnableSorcererQuickenedAction)
        {
            toggle = Main.Settings.HideQuickenedActionWhenMetamagicOff;
            if (UI.Toggle(Gui.Localize("ModUi/&HideQuickenedActionWhenMetamagicOff"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.HideQuickenedActionWhenMetamagicOff = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.AccountForAllDiceOnFollowUpStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&AccountForAllDiceOnFollowUpStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AccountForAllDiceOnFollowUpStrike = toggle;
        }

        UI.Label();

        toggle = Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange;
        if (UI.Toggle(Gui.Localize("ModUi/&IllusionSpellsAutomaticallyFailAgainstTrueSightInRange"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange = toggle;
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

        toggle = Main.Settings.FixEldritchBlastRange;
        if (UI.Toggle(Gui.Localize("ModUi/&FixEldritchBlastRange"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixEldritchBlastRange = toggle;
            SrdAndHouseRulesContext.SwitchEldritchBlastRange();
        }

        UI.Label();

        toggle = Main.Settings.EnableBardHealingBalladOnLongRest;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardHealingBalladOnLongRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardHealingBalladOnLongRest = toggle;
            CharacterContext.SwitchBardHealingBalladOnLongRest();
        }
    }
}
