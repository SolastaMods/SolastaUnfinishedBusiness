using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
#if DEBUG
using UnityExplorer;
#endif

namespace SolastaUnfinishedBusiness.Displays;

internal static class ToolsDisplay
{
    private static string ExportFileName { get; set; } =
        ServiceRepository.GetService<INetworkingService>().GetUserName();

#if DEBUG
    private static bool IsUnityExplorerEnabled { get; set; }
#endif

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
        DisplayTabletop2014();
        UI.Label();
        DisplayTabletop2024();
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

#if DEBUG
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
#endif

    private static void DisplayGeneral()
    {
        UI.Label();

#if DEBUG
        var size = IsUnityExplorerEnabled ? 195f : 145f;
#else
        // ReSharper disable once ConvertToConstant.Local
        var size = 195f;
#endif

        var width = UI.Width(size);

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&Update"), () => UpdateContext.UpdateMod(), width);
            UI.ActionButton(Gui.Localize("ModUi/&Rollback"), UpdateContext.DisplayRollbackMessage, width);
            UI.ActionButton(Gui.Localize("ModUi/&Changelog"), UpdateContext.OpenChangeLog, width);

#if DEBUG
            if (!IsUnityExplorerEnabled)
            {
                UI.ActionButton(Gui.Localize("ModUi/&UnityExplorer"), EnableUnityExplorerUi, UI.Width(145f));
            }
#endif
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

    private static void DisplayTabletop2024()
    {
        var toggle = Main.Settings.DisplayTabletop2024;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&OneDnd"), ref toggle, 200))
        {
            Main.Settings.DisplayTabletop2024 = toggle;
        }

        if (!Main.Settings.DisplayTabletop2024)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.EnableSurprisedToEnforceDisadvantage;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSurprisedToEnforceDisadvantage"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSurprisedToEnforceDisadvantage = toggle;
            Tabletop2024Context.SwitchSurprisedEnforceDisadvantage();
        }

        toggle = Main.Settings.EnablePotionsBonusAction2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePotionsBonusAction2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePotionsBonusAction2024 = toggle;
            Tabletop2024Context.SwitchPotionsBonusAction();
        }

        toggle = Main.Settings.EnablePoisonsBonusAction2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePoisonsBonusAction2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePoisonsBonusAction2024 = toggle;
            Tabletop2024Context.SwitchPoisonsBonusAction();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableBardicInspiration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardicInspiration2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardicInspiration2024 = toggle;
            Tabletop2024Context.SwitchBardBardicInspiration();
        }

        toggle = Main.Settings.EnableBardCounterCharm2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardCounterCharm2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardCounterCharm2024 = toggle;
            Tabletop2024Context.SwitchBardCounterCharm();
        }

        toggle = Main.Settings.EnableBardExpertiseOneLevelBefore2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardExpertiseOneLevelBefore2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardExpertiseOneLevelBefore2024 = toggle;
            Tabletop2024Context.SwitchBardExpertiseOneLevelBefore();
        }

        toggle = Main.Settings.EnableBardSuperiorInspiration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardSuperiorInspiration2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardSuperiorInspiration2024 = toggle;
            Tabletop2024Context.SwitchBardSuperiorInspiration();
        }

        toggle = Main.Settings.EnableBardWordsOfCreation2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardWordsOfCreation2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardWordsOfCreation2024 = toggle;
            Tabletop2024Context.SwitchBardWordsOfCreation();
        }

        toggle = Main.Settings.RemoveBardMagicalSecret2024;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardMagicalSecret2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardMagicalSecret2024 = toggle;
            Tabletop2024Context.SwitchBardBardMagicalSecrets();
        }

        toggle = Main.Settings.RemoveBardSongOfRest2024;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardSongOfRest2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardSongOfRest2024 = toggle;
            Tabletop2024Context.SwitchBardSongOfRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BarbarianTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableBarbarianBrutalStrike2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianBrutalStrike2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianBrutalStrike2024 = toggle;
            Tabletop2024Context.SwitchBarbarianBrutalStrike();
        }

        toggle = Main.Settings.EnableBarbarianInstinctivePounce2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianInstinctivePounce2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianInstinctivePounce2024 = toggle;
            Tabletop2024Context.SwitchBarbarianInstinctivePounce();
        }

        toggle = Main.Settings.EnableBarbarianPrimalKnowledge2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianPrimalKnowledge2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianPrimalKnowledge2024 = toggle;
            Tabletop2024Context.SwitchBarbarianPrimalKnowledge();
        }

        toggle = Main.Settings.EnableBarbarianReckless2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianReckless2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianReckless2024 = toggle;
            Tabletop2024Context.SwitchBarbarianReckless();
        }

        toggle = Main.Settings.EnableBarbarianRegainOneRageAtShortRest;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRegainOneRageAtShortRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRegainOneRageAtShortRest = toggle;
            Tabletop2024Context.SwitchBarbarianRegainOneRageAtShortRest();
        }

        toggle = Main.Settings.EnableBarbarianPersistentRage2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianPersistentRage2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianPersistentRage2024 = toggle;
            Tabletop2024Context.SwitchBarbarianPersistentRage();
        }

        toggle = Main.Settings.EnableBarbarianRelentlessRage2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRelentlessRage2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRelentlessRage2024 = toggle;
            Tabletop2024Context.SwitchBarbarianRelentlessRage();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&DruidTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableDruidPrimalOrder2024;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDruidPrimalOrderAndRemoveMediumArmorProficiency"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableDruidPrimalOrder2024 = toggle;
            Tabletop2024Context.SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency();
        }

        toggle = Main.Settings.EnableDruidWeaponProficiency2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidWeaponProficiency2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidWeaponProficiency2024 = toggle;
            Tabletop2024Context.SwitchDruidWeaponProficiency();
        }

        toggle = Main.Settings.EnableDruidMetalArmor2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidMetalArmor2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidMetalArmor2024 = toggle;
            Tabletop2024Context.SwitchDruidMetalArmor();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&FighterTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableFighterIndomitableSaving2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterIndomitableSaving2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterIndomitableSaving2024 = toggle;
            Tabletop2024Context.SwitchFighterIndomitableSaving();
        }

        toggle = Main.Settings.EnableFighterSkillOptions2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterSkillOptions2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterSkillOptions2024 = toggle;
            Tabletop2024Context.SwitchFighterSkillOptions();
        }

        toggle = Main.Settings.EnableFighterSecondWind2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterSecondWind2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterSecondWind2024 = toggle;
            Tabletop2024Context.SwitchFighterSecondWind();
        }

        toggle = Main.Settings.EnableFighterStudiedAttacks2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterStudiedAttacks2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterStudiedAttacks2024 = toggle;
            Tabletop2024Context.SwitchFighterStudiedAttacks();
        }

        toggle = Main.Settings.EnableFighterTacticalProgression2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterTacticalProgression2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterTacticalProgression2024 = toggle;
            Tabletop2024Context.SwitchFighterTacticalProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&MonkTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableMonkBodyAndMind2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMind2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMind2024 = toggle;
            Tabletop2024Context.SwitchMonkBodyAndMind();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry2024 = toggle;
            Tabletop2024Context.SwitchMonkDoNotRequireAttackActionForFlurry();
        }

        toggle = Main.Settings.EnableMonkHeightenedMetabolism2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedMetabolism2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedMetabolism2024 = toggle;
            Tabletop2024Context.SwitchMonkHeightenedMetabolism();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack2024 = toggle;
            Tabletop2024Context.SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        }

        toggle = Main.Settings.EnableMonkSuperiorDefense2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSuperiorDefense2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSuperiorDefense2024 = toggle;
            Tabletop2024Context.SwitchMonkSuperiorDefense();
        }

        toggle = Main.Settings.EnableMonkUnarmoredDieTypeProgression2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkUnarmoredDieTypeProgression2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkUnarmoredDieTypeProgression2024 = toggle;
            Tabletop2024Context.SwitchOneDndMonkUnarmedDieTypeProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&PaladinTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnablePaladinLayOnHandsAsBonusAction2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinLayOnHandsAsBonusAction2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinLayOnHandsAsBonusAction2024 = toggle;
            Tabletop2024Context.SwitchOneDndPaladinLayOnHandAsBonusAction();
        }

        toggle = Main.Settings.EnablePaladinSmiteAsBonusAction2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinSmiteAsBonusAction2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinSmiteAsBonusAction2024 = toggle;
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

        toggle = Main.Settings.EnableRangerNatureShroud2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroud2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroud2024 = toggle;
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

        toggle = Main.Settings.EnableRogueCunningStrike2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueCunningStrike2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueCunningStrike2024 = toggle;
            Tabletop2024Context.SwitchRogueCunningStrike();
        }

        toggle = Main.Settings.EnableRogueSteadyAim2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSteadyAim2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSteadyAim2024 = toggle;
            Tabletop2024Context.SwitchRogueSteadyAim();
        }

        toggle = Main.Settings.EnableRogueReliableTalent2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueReliableTalent2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueReliableTalent2024 = toggle;
            Tabletop2024Context.SwitchRogueReliableTalent();
        }

        toggle = Main.Settings.RemoveRogueBlindSense2024;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRogueBlindSense2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRogueBlindSense2024 = toggle;
            Tabletop2024Context.SwitchRogueBlindSense();
        }

        toggle = Main.Settings.EnableRogueSlipperyMind2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSlipperyMind2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSlipperyMind2024 = toggle;
            Tabletop2024Context.SwitchRogueSlipperyMind();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&SorcererTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableSorcererArcaneApotheosis2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererArcaneApotheosis2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererArcaneApotheosis2024 = toggle;
            Tabletop2024Context.SwitchSorcererArcaneApotheosis();
        }

        toggle = Main.Settings.EnableSorcererInnateSorceryAndSorceryIncarnate2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererInnateSorceryAndSorceryIncarnate2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererInnateSorceryAndSorceryIncarnate2024 = toggle;
            Tabletop2024Context.SwitchSorcererInnateSorcery();
        }

        toggle = Main.Settings.EnableSorcererSorcerousRestoration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererSorcerousRestoration2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererSorcerousRestoration2024 = toggle;
            Tabletop2024Context.SwitchSorcerousRestorationAtLevel5();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WarlockTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWarlockInvocationProgression2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockInvocationProgression2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockInvocationProgression2024 = toggle;
            Tabletop2024Context.SwitchWarlockInvocationsProgression();
        }

        toggle = Main.Settings.EnableWarlockMagicalCunningAndImprovedEldritchMaster2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockMagicalCunningAndImprovedEldritchMaster2024"),
                ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockMagicalCunningAndImprovedEldritchMaster2024 = toggle;
            Tabletop2024Context.SwitchWarlockMagicalCunningAndImprovedEldritchMaster();
        }

        toggle = Main.Settings.EnableWarlockToLearnPatronAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockToLearnPatronAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockToLearnPatronAtLevel3 = toggle;
            Tabletop2024Context.SwitchWarlockPatronLearningLevel();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WizardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWizardMemorizeSpell2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardMemorizeSpell2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardMemorizeSpell2024 = toggle;
            Tabletop2024Context.SwitchWizardMemorizeSpell();
        }

        toggle = Main.Settings.EnableWizardToLearnScholarAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnScholarAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnScholarAtLevel2 = toggle;
            Tabletop2024Context.SwitchWizardScholar();
        }

        toggle = Main.Settings.EnableWizardToLearnSchoolAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnSchoolAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnSchoolAtLevel3 = toggle;
            Tabletop2024Context.SwitchWizardSchoolOfMagicLearningLevel();
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

        toggle = Main.Settings.EnablePreparedSpellsTables2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePreparedSpellsTables2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePreparedSpellsTables2024 = toggle;
            Tabletop2024Context.SwitchOneDndPreparedSpellsTables();
        }

        toggle = Main.Settings.EnableRitualOnAllCasters2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRitualOnAllCasters2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRitualOnAllCasters2024 = toggle;
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

    private static void DisplayTabletop2014()
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

        toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
            Main.Settings.UseOfficialFlankingRulesAlsoForRanged = false;
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
