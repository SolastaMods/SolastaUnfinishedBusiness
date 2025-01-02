using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ClassesDisplay
{
    internal static void DisplayClasses()
    {
        UI.Label();

        UI.ActionButton(Gui.Localize("ModUi/&DocsClasses").Bold().Khaki(),
            () => UpdateContext.OpenDocumentation("Classes.md"), UI.Width(189f));

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BardTitle") + ":</color>");
        UI.Label();

        var toggle = Main.Settings.EnableBardCounterCharm2024;
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

        toggle = Main.Settings.EnableBardScimitarSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableScimitarSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardScimitarSpecialization = toggle;
            ClassesContext.SwitchBardScimitarSpecialization();
        }

        toggle = Main.Settings.RemoveBardSongOfRest2024;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardSongOfRest2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardSongOfRest2024 = toggle;
            Tabletop2024Context.SwitchBardSongOfRest();
        }

        toggle = Main.Settings.EnableBardicInspiration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardicInspiration2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardicInspiration2024 = toggle;
            Tabletop2024Context.SwitchBardBardicInspiration();
        }

        toggle = Main.Settings.EnableBardMagicalSecrets2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardMagicalSecrets2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardMagicalSecrets2024 = toggle;
            Tabletop2024Context.SwitchBardBardMagicalSecrets();
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

        toggle = Main.Settings.EnableBarbarianFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianFightingStyle = toggle;
            ClassesContext.SwitchBarbarianFightingStyle();
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

        toggle = Main.Settings.EnableBarbarianPersistentRage2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianPersistentRage2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianPersistentRage2024 = toggle;
            Tabletop2024Context.SwitchBarbarianPersistentRage();
        }

        toggle = Main.Settings.EnableBarbarianRage2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRage2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRage2024 = toggle;
            Tabletop2024Context.SwitchBarbarianRage();
        }

        toggle = Main.Settings.EnableBarbarianReckless2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianReckless2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianReckless2024 = toggle;
            Tabletop2024Context.SwitchBarbarianReckless();
        }

        toggle = Main.Settings.EnableBarbarianRelentlessRage2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRelentlessRage2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRelentlessRage2024 = toggle;
            Tabletop2024Context.SwitchBarbarianRelentlessRage();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&ClericTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableClericBlessedStrikes2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericBlessedStrikes2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericBlessedStrikes2024 = toggle;
            Tabletop2024Context.SwitchClericBlessedStrikes();
        }

        toggle = Main.Settings.EnableClericDivineOrder2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericDivineOrder2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericDivineOrder2024 = toggle;
            Tabletop2024Context.SwitchClericDivineOrder();
        }

        toggle = Main.Settings.EnableClericToLearnDomainAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericToLearnDomainAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericToLearnDomainAtLevel3 = toggle;
            Tabletop2024Context.SwitchClericDomainLearningLevel();
        }

        toggle = Main.Settings.EnableClericSearUndead2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericSearUndead2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericSearUndead2024 = toggle;
            Tabletop2024Context.SwitchClericSearUndead();
        }

        toggle = Main.Settings.EnableClericChannelDivinity2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericChannelDivinity2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericChannelDivinity2024 = toggle;
            Tabletop2024Context.SwitchClericChannelDivinity();
        }

#if false
        toggle = Main.Settings.EnableClericDivineIntervention2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableClericDivineIntervention2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableClericDivineIntervention2024 = toggle;
            Tabletop2024Context.SwitchClericDivineIntervention();
        }
#endif

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&DruidTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableDruidToLearnCircleAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidToLearnCircleAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidToLearnCircleAtLevel3 = toggle;
            Tabletop2024Context.SwitchDruidCircleLearningLevel();
        }

        toggle = Main.Settings.EnableDruidWeaponProficiency2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidWeaponProficiency2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidWeaponProficiency2024 = toggle;
            Tabletop2024Context.SwitchDruidWeaponProficiency();
        }

        toggle = Main.Settings.EnableDruidWildResurgence2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidWildResurgence2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidWildResurgence2024 = toggle;
            Tabletop2024Context.SwitchDruidWildResurgence();
        }

        toggle = Main.Settings.EnableDruidMetalArmor2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidMetalArmor2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidMetalArmor2024 = toggle;
            Tabletop2024Context.SwitchDruidMetalArmor();
        }

        toggle = Main.Settings.EnableDruidArchDruid2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidArchDruid2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidArchDruid2024 = toggle;
            Tabletop2024Context.SwitchDruidArchDruid();
        }

        toggle = Main.Settings.EnableDruidWildshape2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDruidWildshape2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidWildshape2024 = toggle;
            Tabletop2024Context.SwitchDruidWildshape();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&FighterTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableFighterSkillOptions2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterSkillOptions2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterSkillOptions2024 = toggle;
            Tabletop2024Context.SwitchFighterSkillOptions();
        }

#if false
        toggle = Main.Settings.EnableFighterWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterWeaponSpecialization = toggle;
            ClassesContext.SwitchFighterWeaponSpecialization();
        }
#endif

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

        toggle = Main.Settings.EnableFighterIndomitableSaving2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterIndomitableSaving2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterIndomitableSaving2024 = toggle;
            Tabletop2024Context.SwitchFighterIndomitableSaving();
        }

        toggle = Main.Settings.EnableFighterSecondWind2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterSecondWind2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterSecondWind2024 = toggle;
            Tabletop2024Context.SwitchFighterSecondWind();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&MonkTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableMonkAbundantKi;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkAbundantKi"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkAbundantKi = toggle;
            ClassesContext.SwitchMonkAbundantKi();
        }

        toggle = Main.Settings.EnableMonkBodyAndMind2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMind2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMind2024 = toggle;
            Tabletop2024Context.SwitchMonkBodyAndMind();
        }

        toggle = Main.Settings.EnableMonkDeflectAttacks2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDeflectAttacks2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDeflectAttacks2024 = toggle;
            Tabletop2024Context.SwitchMonkDeflectAttacks();
        }

        UI.Label();

        toggle = Main.Settings.EnableMonkFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkFightingStyle = toggle;
            ClassesContext.SwitchMonkFightingStyle();
        }

        toggle = Main.Settings.EnableMonkHandwrapsOnGauntletSlot;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableMonkHandwrapsOnGauntletSlot")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHandwrapsOnGauntletSlot = toggle;
            ClassesContext.SwitchMonkHandwrapsGauntletSlot();
        }

        toggle = Main.Settings.EnableMonkHeightenedFocus2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedFocus2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedFocus2024 = toggle;
            Tabletop2024Context.SwitchMonkHeightenedFocus();
        }

        UI.Label();

        toggle = Main.Settings.EnableMonkImprovedUnarmoredMovement;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkImprovedUnarmoredMovement"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkImprovedUnarmoredMovement = toggle;
            ClassesContext.SwitchMonkImprovedUnarmoredMovement();
        }

        toggle = Main.Settings.EnableMonkSelfRestoration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSelfRestoration2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSelfRestoration2024 = toggle;
            Tabletop2024Context.SwitchMonkSelfRestoration();
        }

        toggle = Main.Settings.EnableMonkSuperiorDefense2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSuperiorDefense2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSuperiorDefense2024 = toggle;
            Tabletop2024Context.SwitchMonkSuperiorDefense();
        }

        toggle = Main.Settings.EnableMonkUncannyMetabolism2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkUncannyMetabolism2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkUncannyMetabolism2024 = toggle;
            Tabletop2024Context.SwitchMonkUncannyMetabolism();
        }

        toggle = Main.Settings.EnableMonkKatanaSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkKatanaSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkKatanaSpecialization = toggle;
            ClassesContext.SwitchMonkKatanaSpecialization();
        }

        toggle = Main.Settings.EnableMonkFocus2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkFocus2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkFocus2024 = toggle;
            Tabletop2024Context.SwitchMonkFocus();
        }

        toggle = Main.Settings.EnableMonkMartialArts2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkMartialArts2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkMartialArts2024 = toggle;
            Tabletop2024Context.SwitchMonkMartialArts();
        }

        toggle = Main.Settings.EnableMonkStunningStrike2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkStunningStrike2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkStunningStrike2024 = toggle;
            Tabletop2024Context.SwitchMonkStunningStrike();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&PaladinTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AddPaladinSmiteToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPaladinSmiteToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPaladinSmiteToggle = toggle;
        }

        toggle = Main.Settings.EnablePaladinAbjureFoes2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinAbjureFoes2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinAbjureFoes2024 = toggle;
            Tabletop2024Context.SwitchPaladinAbjureFoes();
        }

        toggle = Main.Settings.EnablePaladinRestoringTouch2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinRestoringTouch2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinRestoringTouch2024 = toggle;
            Tabletop2024Context.SwitchPaladinRestoringTouch();
        }

        toggle = Main.Settings.EnablePaladinSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinSpellCastingAtLevel1 = toggle;
            Tabletop2024Context.SwitchPaladinSpellCastingAtOne();
        }

        toggle = Main.Settings.ShowChannelDivinityOnPortrait;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowChannelDivinityOnPortrait"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowChannelDivinityOnPortrait = toggle;
        }

        toggle = Main.Settings.EnablePaladinChannelDivinity2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinChannelDivinity2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinChannelDivinity2024 = toggle;
            Tabletop2024Context.SwitchPaladinChannelDivinity();
        }

        toggle = Main.Settings.EnablePaladinSmite2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinSmite2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinSmite2024 = toggle;
        }

        toggle = Main.Settings.EnablePaladinLayOnHands2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePaladinLayOnHands2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePaladinLayOnHands2024 = toggle;
            Tabletop2024Context.SwitchPaladinLayOnHand();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&RangerTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AddHumanoidFavoredEnemyToRanger;
        if (UI.Toggle(Gui.Localize("ModUi/&AddHumanoidFavoredEnemyToRanger"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddHumanoidFavoredEnemyToRanger = toggle;
            ClassesContext.SwitchRangerHumanoidFavoredEnemy();
        }

        toggle = Main.Settings.EnableRangerDeftExplorer2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerDeftExplorer2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerDeftExplorer2024 = toggle;
            Tabletop2024Context.SwitchRangerDeftExplorer();
        }

        toggle = Main.Settings.EnableRangerExpertise2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerExpertise2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerExpertise2024 = toggle;
            Tabletop2024Context.SwitchRangerExpertise();
        }

        toggle = Main.Settings.EnableRangerNatureShroud2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroud2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroud2024 = toggle;
            Tabletop2024Context.SwitchRangerNatureShroud();
        }

        toggle = Main.Settings.EnableRangerPreciseHunter2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerPreciseHunter2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerPreciseHunter2024 = toggle;
            Tabletop2024Context.SwitchRangerPreciseHunter();
        }

        toggle = Main.Settings.EnableRangerRelentlessHunter2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerRelentlessHunter2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerRelentlessHunter2024 = toggle;
            Tabletop2024Context.SwitchRangerRelentlessHunter();

            if (toggle)
            {
                Main.Settings.EnableRangerRelentlessHunter2024AsNoConcentration = false;
            }
        }

        if (Main.Settings.EnableRangerRelentlessHunter2024)
        {
            toggle = Main.Settings.EnableRangerRelentlessHunter2024AsNoConcentration;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerRelentlessHunter2024AsNoConcentration"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.EnableRangerRelentlessHunter2024AsNoConcentration = toggle;
            }
        }

        toggle = Main.Settings.EnableRangerRoving2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerRoving2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerRoving2024 = toggle;
            Tabletop2024Context.SwitchRangerRoving();
        }

        toggle = Main.Settings.EnableRangerSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerSpellCastingAtLevel1 = toggle;
            Tabletop2024Context.SwitchRangerSpellCastingAtOne();
        }

        toggle = Main.Settings.EnableRangerTireless2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerTireless2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerTireless2024 = toggle;
            Tabletop2024Context.SwitchRangerTireless();
        }

        toggle = Main.Settings.RemoveRangerPrimevalAwareness2024;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRangerPrimevalAwareness2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRangerPrimevalAwareness2024 = toggle;
            Tabletop2024Context.SwitchRangerPrimevalAwareness();
        }

        toggle = Main.Settings.EnableRangerFavoredEnemy2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerFavoredEnemy2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerFavoredEnemy2024 = toggle;
            Tabletop2024Context.SwitchRangerFavoredEnemy();
        }

        toggle = Main.Settings.EnableRangerFeralSenses2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerFeralSenses2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerFeralSenses2024 = toggle;
            Tabletop2024Context.SwitchRangerFeralSenses();
        }

        toggle = Main.Settings.EnableRangerFoeSlayers2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerFoeSlayers2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerFoeSlayers2024 = toggle;
            Tabletop2024Context.SwitchRangerFoeSlayers();
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

        toggle = Main.Settings.EnableRogueFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueFightingStyle = toggle;
            ClassesContext.SwitchRogueFightingStyle();
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

        toggle = Main.Settings.EnableRogueScimitarSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableScimitarSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueScimitarSpecialization = toggle;
            ClassesContext.SwitchRogueScimitarSpecialization();
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

        toggle = Main.Settings.EnableSorcererInnateSorcery2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererInnateSorcery2024"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererInnateSorcery2024 = toggle;
            Tabletop2024Context.SwitchSorcererInnateSorcery();
        }

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

        toggle = Main.Settings.EnableSorcererMetamagic2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererMetamagic2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererMetamagic2024 = toggle;
            Tabletop2024Context.SwitchSorcererMetamagic();
        }

        toggle = Main.Settings.EnableSorcererOrigin2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererOrigin2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererOrigin2024 = toggle;
            Tabletop2024Context.SwitchSorcererOriginLearningLevel();
        }

        toggle = Main.Settings.EnableSorcererSorcerousRestoration2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererSorcerousRestoration2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererSorcerousRestoration2024 = toggle;
            Tabletop2024Context.SwitchSorcererSorcerousRestorationAtLevel5();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WarlockTitle") + ":</color>");
        UI.Label();

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

        toggle = Main.Settings.EnableWarlockInvocationProgression2024;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockInvocationProgression2024"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockInvocationProgression2024 = toggle;
            Tabletop2024Context.SwitchWarlockInvocationsProgression();
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

        toggle = Main.Settings.EnableSignatureSpellsRelearn;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSignatureSpellsRelearn"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSignatureSpellsRelearn = toggle;
        }

        UI.Label();
    }
}
