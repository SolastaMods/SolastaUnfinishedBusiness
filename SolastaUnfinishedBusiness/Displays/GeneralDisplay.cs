using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using UnityExplorer;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ToolsDisplay
{
    private static string ExportFileName { get; set; } =
        ServiceRepository.GetService<INetworkingService>().GetUserName();

    private static bool IsUnityExplorerEnabled { get; set; }

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

    private static void DisplayGeneral()
    {
        UI.Label();

        var width = UI.Width(IsUnityExplorerEnabled ? 195f : 145f);

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&Update"), () => UpdateContext.UpdateMod(), width);
            UI.ActionButton(Gui.Localize("ModUi/&Rollback"), UpdateContext.DisplayRollbackMessage, width);
            UI.ActionButton(Gui.Localize("ModUi/&Changelog"), UpdateContext.OpenChangeLog, width);

            if (!IsUnityExplorerEnabled)
            {
                UI.ActionButton(Gui.Localize("ModUi/&UnityExplorer"), EnableUnityExplorerUi, UI.Width(145f));
            }
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

            UI.ActionTextField(ref text, string.Empty, s => { ExportFileName = s; }, null, UI.Width(144f));
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
            Tabletop2024Context.SwitchOneDndSurprisedEnforceDisadvantage();
        }

        toggle = Main.Settings.OneDndHealingPotionBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&OneDndHealingPotionBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.OneDndHealingPotionBonusAction = toggle;
            Tabletop2024Context.SwitchOneDndHealingPotionBonusAction();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.ChangeBardicInspirationDurationToOneHour;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeBardicInspirationDurationToOneHour"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeBardicInspirationDurationToOneHour = toggle;
            Tabletop2024Context.SwitchOneDndChangeBardicInspirationDurationToOneHour();
        }

        toggle = Main.Settings.EnableBardCounterCharmAsReactionAtLevel7;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardCounterCharmAsReactionAtLevel7"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardCounterCharmAsReactionAtLevel7 = toggle;
            Tabletop2024Context.SwitchOneDndEnableBardCounterCharmAsReactionAtLevel7();
        }

        toggle = Main.Settings.EnableBardExpertiseOneLevelBefore;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardExpertiseOneLevelBefore"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardExpertiseOneLevelBefore = toggle;
            Tabletop2024Context.SwitchOneDndEnableBardExpertiseOneLevelBefore();
        }

        toggle = Main.Settings.EnableBardSuperiorInspirationAtLevel18;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardSuperiorInspirationAtLevel18"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardSuperiorInspirationAtLevel18 = toggle;
            Tabletop2024Context.SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        }

        toggle = Main.Settings.EnableBardWordsOfCreationAtLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardWordsOfCreationAtLevel20"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardWordsOfCreationAtLevel20 = toggle;
            Tabletop2024Context.SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        }

        toggle = Main.Settings.RemoveBardMagicalSecretAt14And18;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardMagicalSecretAt14And18"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardMagicalSecretAt14And18 = toggle;
            Tabletop2024Context.SwitchOneDndRemoveBardMagicalSecretAt14And18();
        }

        toggle = Main.Settings.RemoveBardSongOfRestAt2;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardSongOfRestAt2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardSongOfRestAt2 = toggle;
            Tabletop2024Context.SwitchOneDndRemoveBardSongOfRestAt2();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BarbarianTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableBarbarianBrutalStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianBrutalStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianBrutalStrike = toggle;
            Main.Settings.DisableBarbarianBrutalCritical = toggle;
            Tabletop2024Context.SwitchBarbarianBrutalStrike();
            Tabletop2024Context.SwitchBarbarianBrutalCritical();
        }

        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            toggle = Main.Settings.DisableBarbarianBrutalCritical;
            if (UI.Toggle(Gui.Localize("ModUi/&DisableBarbarianBrutalCritical"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableBarbarianBrutalCritical = toggle;
                Tabletop2024Context.SwitchBarbarianBrutalCritical();
            }
        }

        toggle = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRecklessSameBuffDebuffDuration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration = toggle;
            Tabletop2024Context.SwitchBarbarianRecklessSameBuffDebuffDuration();
        }

        toggle = Main.Settings.EnableBarbarianRegainOneRageAtShortRest;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRegainOneRageAtShortRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRegainOneRageAtShortRest = toggle;
            Tabletop2024Context.SwitchBarbarianRegainOneRageAtShortRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&DruidTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableDruidUseMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidUseMetalArmor = toggle;
            Tabletop2024Context.SwitchOneDnDEnableDruidUseMetalArmor();
        }

        toggle = Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDruidPrimalOrderAndRemoveMediumArmorProficiency"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency = toggle;
            Tabletop2024Context.SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency();
        }

        toggle = Main.Settings.SwapDruidToUseOneDndWeaponProficiency;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapDruidToUseOneDndWeaponProficiency"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapDruidToUseOneDndWeaponProficiency = toggle;
            Tabletop2024Context.SwitchDruidWeaponProficiencyToUseOneDnd();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&FighterTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AddFighterLevelToIndomitableSavingReroll;
        if (UI.Toggle(Gui.Localize("ModUi/&AddFighterLevelToIndomitableSavingReroll"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddFighterLevelToIndomitableSavingReroll = toggle;
            Tabletop2024Context.SwitchFighterLevelToIndomitableSavingReroll();
        }

        toggle = Main.Settings.AddPersuasionToFighterSkillOptions;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPersuasionToFighterSkillOptions"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPersuasionToFighterSkillOptions = toggle;
            Tabletop2024Context.SwitchPersuasionToFighterSkillOptions();
        }

        toggle = Main.Settings.EnableSecondWindToUseOneDndUsagesProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSecondWindToUseOneDndUsagesProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSecondWindToUseOneDndUsagesProgression = toggle;
            Tabletop2024Context.SwitchSecondWindToUseOneDndUsagesProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&MonkTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMindToReplacePerfectSelf"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf = toggle;
            Tabletop2024Context.SwitchMonkBodyAndMindToReplacePerfectSelf();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry = toggle;
            Tabletop2024Context.SwitchMonkDoNotRequireAttackActionForFlurry();
        }

        toggle = Main.Settings.EnableMonkHeightenedMetabolism;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedMetabolism"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedMetabolism = toggle;
            Tabletop2024Context.SwitchMonkHeightenedMetabolism();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack = toggle;
            Tabletop2024Context.SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        }

        toggle = Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSuperiorDefenseToReplaceEmptyBody"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody = toggle;
            Tabletop2024Context.SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        }

        toggle = Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapMonkToUseOneDndUnarmedDieTypeProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression = toggle;
            Tabletop2024Context.SwitchOneDndMonkUnarmedDieTypeProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&PaladinTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnablePaladinLayOnHandsAsBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinLayOnHandsAsBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinLayOnHandsAsBonusAction = toggle;
            Tabletop2024Context.SwitchOneDndPaladinLayOnHandAsBonusAction();
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
            Tabletop2024Context.SwitchOneDndPaladinLearnSpellCastingAtOne();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&RangerTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableRangerNatureShroudAt14;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroudAt14"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroudAt14 = toggle;
            Tabletop2024Context.SwitchRangerNatureShroud();
        }

        toggle = Main.Settings.EnableRangerSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerSpellCastingAtLevel1 = toggle;
            Tabletop2024Context.SwitchOneDndRangerLearnSpellCastingAtOne();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&RogueTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableRogueCunningStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueCunningStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueCunningStrike = toggle;
            Tabletop2024Context.SwitchRogueCunningStrike();
        }

        toggle = Main.Settings.EnableRogueSteadyAim;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSteadyAim"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSteadyAim = toggle;
            Tabletop2024Context.SwitchRogueSteadyAim();
        }

        toggle = Main.Settings.RemoveRogueBlindSense;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRogueBlindSense"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRogueBlindSense = toggle;
            Tabletop2024Context.SwitchRogueBlindSense();
        }

        toggle = Main.Settings.EnableRogueSlipperyMind;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSlipperyMind"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSlipperyMind = toggle;
            Tabletop2024Context.SwitchRogueSlipperyMind();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&SorcererTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableSorcererInnateSorceryAt1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererInnateSorceryAt1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererInnateSorceryAt1 = toggle;
            Tabletop2024Context.SwitchSorcererInnateSorcery();
        }

        toggle = Main.Settings.EnableSorcerousRestorationAtLevel5;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcerousRestorationAtLevel5"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcerousRestorationAtLevel5 = toggle;
            Tabletop2024Context.SwitchSorcerousRestorationAtLevel5();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WarlockTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWarlockToUseOneDndInvocationProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapWarlockToUseOneDndInvocationProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockToUseOneDndInvocationProgression = toggle;
            Tabletop2024Context.SwitchOneDndWarlockInvocationsProgression();
        }

        toggle = Main.Settings.EnableWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20"),
                ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20 = toggle;
            Tabletop2024Context.SwitchWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20();
        }

        toggle = Main.Settings.EnableWarlockToLearnPatronAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockToLearnPatronAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockToLearnPatronAtLevel3 = toggle;
            Tabletop2024Context.SwitchOneDndWarlockPatronLearningLevel();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WizardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWizardToLearnScholarAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnScholarAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnScholarAtLevel2 = toggle;
            Tabletop2024Context.SwitchOneDndWizardScholar();
        }

        toggle = Main.Settings.EnableWizardToLearnSchoolAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnSchoolAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnSchoolAtLevel3 = toggle;
            Tabletop2024Context.SwitchOneDndWizardSchoolOfMagicLearningLevel();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("ModUi/&DocsRaces") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableAlternateHuman;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAlternateHuman"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAlternateHuman = toggle;
            FeatsContext.SwitchFirstLevelTotalFeats();
        }

        toggle = Main.Settings.RaceLightSensitivityApplyOutdoorsOnly;
        if (UI.Toggle(Gui.Localize("ModUi/&RaceLightSensitivityApplyOutdoorsOnly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RaceLightSensitivityApplyOutdoorsOnly = toggle;
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("ModUi/&DocsSpells") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableOneDnDPreparedSpellsTables;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDnDPreparedSpellsTables"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDnDPreparedSpellsTables = toggle;
            Tabletop2024Context.SwitchOneDndPreparedSpellsTables();
        }

        toggle = Main.Settings.EnableRitualOnAllCasters;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRitualOnAllCasters"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRitualOnAllCasters = toggle;
            Tabletop2024Context.SwitchSpellRitualOnAllCasters();
        }

        UI.Label();

        toggle = Main.Settings.EnableOneDndBarkskinSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndBarkskinSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndBarkskinSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellBarkskin();
        }

        toggle = Main.Settings.EnableOneDndDamagingSpellsUpgrade;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndDamagingSpellsUpgrade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndDamagingSpellsUpgrade = toggle;
            Tabletop2024Context.SwitchOneDndDamagingSpellsUpgrade();
        }

        toggle = Main.Settings.EnableOneDndHealingSpellsUpgrade;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHealingSpellsUpgrade"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHealingSpellsUpgrade = toggle;
            Tabletop2024Context.SwitchOneDndHealingSpellsUpgrade();
        }

        toggle = Main.Settings.EnableOneDndDivineFavorSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndDivineFavorSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndDivineFavorSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellDivineFavor();
        }

        toggle = Main.Settings.EnableOneDndGuidanceSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndGuidanceSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndGuidanceSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellGuidance();
        }

        toggle = Main.Settings.EnableOneDndHideousLaughterSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHideousLaughterSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHideousLaughterSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellHideousLaughter();
        }

        toggle = Main.Settings.EnableOneDndHuntersMarkSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHuntersMarkSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHuntersMarkSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellHuntersMark();
        }

        toggle = Main.Settings.EnableOneDndLesserRestorationSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndLesserRestorationSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndLesserRestorationSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellLesserRestoration();
        }

        toggle = Main.Settings.EnableOneDndMagicWeaponSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndMagicWeaponSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndMagicWeaponSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellMagicWeapon();
        }

        toggle = Main.Settings.EnableOneDndPowerWordStunSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndPowerWordStunSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndPowerWordStunSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellPowerWordStun();
        }

        toggle = Main.Settings.EnableOneDndSpareTheDyingSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndSpareTheDyingSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndSpareTheDyingSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellSpareTheDying();
        }

        toggle = Main.Settings.EnableOneDndSpiderClimbSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndSpiderClimbSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndSpiderClimbSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellSpiderClimb();
        }

        toggle = Main.Settings.EnableOneDndStoneSkinSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndStoneSkinSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndStoneSkinSpell = toggle;
            Tabletop2024Context.SwitchOneDndSpellStoneSkin();
        }

        toggle = Main.Settings.EnableOneDndTrueStrikeCantrip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndTrueStrikeCantrip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndTrueStrikeCantrip = toggle;
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

        toggle = Main.Settings.UseOfficialFoodRationsWeight;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialFoodRationsWeight"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialFoodRationsWeight = toggle;
            Tabletop2014Context.SwitchOfficialFoodRationsWeight();
        }

        toggle = Main.Settings.FixRingOfRegenerationHealRate;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&FixRingOfRegenerationHealRate"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixRingOfRegenerationHealRate = toggle;
            Tabletop2014Context.SwitchRingOfRegenerationHealRate();
        }

        UI.Label();

        toggle = Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialSmallRacesDisWithHeavyWeapons"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableSorcererMagicalGuidance;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererMagicalGuidance"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererMagicalGuidance = toggle;
            ClassesContext.SwitchSorcererMagicalGuidance();
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
            SpellsContext.SwitchRecurringEffectOnEntangle();
        }

        toggle = Main.Settings.FixEldritchBlastRange;
        if (UI.Toggle(Gui.Localize("ModUi/&FixEldritchBlastRange"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixEldritchBlastRange = toggle;
            Tabletop2014Context.SwitchEldritchBlastRange();
        }

        UI.Label();

        toggle = Main.Settings.EnableBardHealingBalladOnLongRest;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardHealingBalladOnLongRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardHealingBalladOnLongRest = toggle;
            Tabletop2014Context.SwitchBardHealingBalladOnLongRest();
        }
    }
}
