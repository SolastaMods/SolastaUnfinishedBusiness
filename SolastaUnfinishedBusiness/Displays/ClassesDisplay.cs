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

        var toggle = Main.Settings.GrantScimitarSpecializationToBardRogue;
        if (UI.Toggle(Gui.Localize("ModUi/&GrantScimitarSpecializationToBarkMonkRogue"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.GrantScimitarSpecializationToBardRogue = toggle;
            ClassesContext.SwitchScimitarWeaponSpecialization();
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

        toggle = Main.Settings.EnableBarbarianFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianFightingStyle = toggle;
            ClassesContext.SwitchBarbarianFightingStyle();
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

        toggle = Main.Settings.EnableFighterWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterWeaponSpecialization = toggle;
            ClassesContext.SwitchFighterWeaponSpecialization();
        }

        toggle = Main.Settings.EnableSecondWindToUseOneDndUsagesProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSecondWindToUseOneDndUsagesProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSecondWindToUseOneDndUsagesProgression = toggle;
            Tabletop2024Context.SwitchSecondWindToUseOneDndUsagesProgression();
        }

        toggle = Main.Settings.EnableFighterStudiedAttacks;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterStudiedAttacks"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterStudiedAttacks = toggle;
            Tabletop2024Context.SwitchFighterStudiedAttacks();
        }

        toggle = Main.Settings.EnableFighterTacticalProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterTacticalProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterTacticalProgression = toggle;
            Tabletop2024Context.SwitchFighterTacticalProgression();
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

        toggle = Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMindToReplacePerfectSelf"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf = toggle;
            Tabletop2024Context.SwitchMonkBodyAndMindToReplacePerfectSelf();
        }

        toggle = Main.Settings.EnableMonkFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkFightingStyle = toggle;
            ClassesContext.SwitchMonkFightingStyle();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry = toggle;
            Tabletop2024Context.SwitchMonkDoNotRequireAttackActionForFlurry();
        }

        toggle = Main.Settings.EnableMonkHandwrapsUseGauntletSlot;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableMonkHandwrapsUseGauntletSlot")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHandwrapsUseGauntletSlot = toggle;
            ClassesContext.UpdateHandWrapsUseGauntletSlot();
        }

        toggle = Main.Settings.EnableMonkHeightenedMetabolism;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedMetabolism"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedMetabolism = toggle;
            Tabletop2024Context.SwitchMonkHeightenedMetabolism();
        }

        toggle = Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkImprovedUnarmoredMovementToMoveOnTheWall"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall = toggle;
            ClassesContext.SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall();
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

        toggle = Main.Settings.EnableMonkWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkWeaponSpecialization = toggle;
            ClassesContext.SwitchMonkWeaponSpecialization();
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

        toggle = Main.Settings.AddPaladinSmiteToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPaladinSmiteToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPaladinSmiteToggle = toggle;
        }

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

        toggle = Main.Settings.ShowChannelDivinityOnPortrait;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowChannelDivinityOnPortrait"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowChannelDivinityOnPortrait = toggle;
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

        toggle = Main.Settings.EnableRogueFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueFightingStyle = toggle;
            ClassesContext.SwitchRogueFightingStyle();
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

        toggle = Main.Settings.EnableRogueReliableTalentAt7;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueReliableTalentAt7"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueReliableTalentAt7 = toggle;
            Tabletop2024Context.SwitchRogueReliableTalent();
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

        toggle = Main.Settings.EnableSorcererArcaneApotheosis;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererArcaneApotheosis"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererArcaneApotheosis = toggle;
            Tabletop2024Context.SwitchSorcererArcaneApotheosis();
        }

        toggle = Main.Settings.EnableSorcererInnateSorceryAndSorceryIncarnate;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererInnateSorceryAndSorceryIncarnate"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererInnateSorceryAndSorceryIncarnate = toggle;
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

        toggle = Main.Settings.EnableSorcererSorcerousRestoration;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererSorcerousRestoration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererSorcerousRestoration = toggle;
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

        toggle = Main.Settings.EnableWizardMemorizeSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardMemorizeSpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardMemorizeSpell = toggle;
            Tabletop2024Context.SwitchOneDndWizardMemorizeSpell();
        }

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

        toggle = Main.Settings.EnableSignatureSpellsRelearn;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSignatureSpellsRelearn"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSignatureSpellsRelearn = toggle;
        }

        UI.Label();
    }
}
