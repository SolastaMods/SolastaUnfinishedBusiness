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
            CharacterContext.SwitchScimitarWeaponSpecialization();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.ChangeBardicInspirationDurationToOneHour;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeBardicInspirationDurationToOneHour"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeBardicInspirationDurationToOneHour = toggle;
            OneDndContext.SwitchOneDndChangeBardicInspirationDurationToOneHour();
        }

        toggle = Main.Settings.EnableBardExpertiseOneLevelBefore;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardExpertiseOneLevelBefore"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardExpertiseOneLevelBefore = toggle;
            OneDndContext.SwitchOneDndEnableBardExpertiseOneLevelBefore();
        }

        toggle = Main.Settings.EnableBardSuperiorInspirationAtLevel18;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardSuperiorInspirationAtLevel18"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardSuperiorInspirationAtLevel18 = toggle;
            OneDndContext.SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        }

        toggle = Main.Settings.EnableBardWordsOfCreationAtLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBardWordsOfCreationAtLevel20"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBardWordsOfCreationAtLevel20 = toggle;
            OneDndContext.SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        }

        toggle = Main.Settings.RemoveBardMagicalSecretAt14And18;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardMagicalSecretAt14And18"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardMagicalSecretAt14And18 = toggle;
            OneDndContext.SwitchOneDndRemoveBardMagicalSecretAt14And18();
        }

        toggle = Main.Settings.RemoveBardSongOfRest;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBardSongOfRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBardSongOfRest = toggle;
            OneDndContext.SwitchOneDndRemoveBardSongOfRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&BarbarianTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableBarbarianBrutalStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianBrutalStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianBrutalStrike = toggle;
            Main.Settings.DisableBarbarianBrutalCritical = toggle;
            OneDndContext.SwitchBarbarianBrutalStrike();
            OneDndContext.SwitchBarbarianBrutalCritical();
        }

        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            toggle = Main.Settings.DisableBarbarianBrutalCritical;
            if (UI.Toggle(Gui.Localize("ModUi/&DisableBarbarianBrutalCritical"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableBarbarianBrutalCritical = toggle;
                OneDndContext.SwitchBarbarianBrutalCritical();
            }
        }

        toggle = Main.Settings.EnableBarbarianFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianFightingStyle = toggle;
            CharacterContext.SwitchBarbarianFightingStyle();
        }

        toggle = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRecklessSameBuffDebuffDuration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration = toggle;
            OneDndContext.SwitchBarbarianRecklessSameBuffDebuffDuration();
        }

        toggle = Main.Settings.EnableBarbarianRegainOneRageAtShortRest;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianRegainOneRageAtShortRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianRegainOneRageAtShortRest = toggle;
            OneDndContext.SwitchBarbarianRegainOneRageAtShortRest();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&DruidTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableDruidToUseMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableDruidToUseMetalArmor = toggle;
            OneDndContext.SwitchOneDnDEnableDruidToUseMetalArmor();
        }

        toggle = Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDruidPrimalOrderAndRemoveMediumArmorProficiency"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency = toggle;
            OneDndContext.SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency();
        }

        toggle = Main.Settings.SwapDruidWeaponProficiencyToUseOneDnd;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapDruidWeaponProficiencyToUseOneDnd"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapDruidWeaponProficiencyToUseOneDnd = toggle;
            OneDndContext.SwitchDruidWeaponProficiencyToUseOneDnd();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&FighterTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.AddFighterLevelToIndomitableSavingReroll;
        if (UI.Toggle(Gui.Localize("ModUi/&AddFighterLevelToIndomitableSavingReroll"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddFighterLevelToIndomitableSavingReroll = toggle;
            OneDndContext.SwitchFighterLevelToIndomitableSavingReroll();
        }

        toggle = Main.Settings.AddPersuasionToFighterSkillOptions;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPersuasionToFighterSkillOptions"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPersuasionToFighterSkillOptions = toggle;
            OneDndContext.SwitchPersuasionToFighterSkillOptions();
        }

        toggle = Main.Settings.EnableFighterWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterWeaponSpecialization = toggle;
            CharacterContext.SwitchFighterWeaponSpecialization();
        }

        toggle = Main.Settings.SwapSecondWindToUseOneDndUsagesProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapSecondWindToUseOneDndUsagesProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapSecondWindToUseOneDndUsagesProgression = toggle;
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&MonkTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableMonkAbundantKi;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkAbundantKi"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkAbundantKi = toggle;
            CharacterContext.SwitchMonkAbundantKi();
        }

        toggle = Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkBodyAndMindToReplacePerfectSelf"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf = toggle;
            OneDndContext.SwitchMonkBodyAndMindToReplacePerfectSelf();
        }

        toggle = Main.Settings.EnableMonkFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkFightingStyle = toggle;
            CharacterContext.SwitchMonkFightingStyle();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry = toggle;
            OneDndContext.SwitchMonkDoNotRequireAttackActionForFlurry();
        }

        toggle = Main.Settings.EnableMonkHandwrapsUseGauntletSlot;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableMonkHandwrapsUseGauntletSlot")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHandwrapsUseGauntletSlot = toggle;
            CustomWeaponsContext.UpdateHandWrapsUseGauntletSlot();
        }

        toggle = Main.Settings.EnableMonkHeightenedMetabolism;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkHeightenedMetabolism"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkHeightenedMetabolism = toggle;
            OneDndContext.SwitchMonkHeightenedMetabolism();
        }

        toggle = Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkImprovedUnarmoredMovementToMoveOnTheWall"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall = toggle;
            CharacterContext.SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack = toggle;
            OneDndContext.SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        }

        toggle = Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkSuperiorDefenseToReplaceEmptyBody"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody = toggle;
            OneDndContext.SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        }

        toggle = Main.Settings.EnableMonkWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkWeaponSpecialization = toggle;
            CharacterContext.SwitchMonkWeaponSpecialization();
        }

        toggle = Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapMonkToUseOneDndUnarmedDieTypeProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression = toggle;
            OneDndContext.SwitchOneDndMonkUnarmedDieTypeProgression();
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
            OneDndContext.SwitchOneDndPaladinLayOnHandAsBonusAction();
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
            OneDndContext.SwitchOneDndPaladinLearnSpellCastingAtOne();
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
            CharacterContext.SwitchRangerHumanoidFavoredEnemy();
        }

        toggle = Main.Settings.EnableRangerNatureShroudAt14;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroudAt14"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroudAt14 = toggle;
            OneDndContext.SwitchRangerNatureShroud();
        }

        toggle = Main.Settings.EnableRangerSpellCastingAtLevel1;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerSpellCastingAtLevel1"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerSpellCastingAtLevel1 = toggle;
            OneDndContext.SwitchOneDndRangerLearnSpellCastingAtOne();
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

        toggle = Main.Settings.EnableRogueFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueFightingStyle = toggle;
            CharacterContext.SwitchRogueFightingStyle();
        }

        toggle = Main.Settings.EnableRogueSteadyAim;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSteadyAim"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSteadyAim = toggle;
            CharacterContext.SwitchRogueSteadyAim();
        }

        toggle = Main.Settings.RemoveRogueBlindSense;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRogueBlindSense"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRogueBlindSense = toggle;
            CharacterContext.SwitchRogueBlindSense();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&SorcererTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableSorcererInnateSorcery;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSorcererInnateSorcery"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSorcererInnateSorcery = toggle;
            OneDndContext.SwitchSorcererInnateSorcery();
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
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WarlockTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWarlockMagicalCunningAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWarlockMagicalCunningAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWarlockMagicalCunningAtLevel2 = toggle;
            OneDndContext.SwitchOneDndWarlockMagicalCunningAtLevel2();
        }

        toggle = Main.Settings.SwapWarlockToUseOneDndInvocationProgression;
        if (UI.Toggle(Gui.Localize("ModUi/&SwapWarlockToUseOneDndInvocationProgression"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.SwapWarlockToUseOneDndInvocationProgression = toggle;
            OneDndContext.SwitchOneDndWarlockInvocationsProgression();
        }

        UI.Label();
        UI.Label("<color=#F0DAA0>" + Gui.Localize("Class/&WizardTitle") + ":</color>");
        UI.Label();

        toggle = Main.Settings.EnableWizardToLearnScholarAtLevel2;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnScholarAtLevel2"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnScholarAtLevel2 = toggle;
            OneDndContext.SwitchOneDndWizardScholar();
        }

        toggle = Main.Settings.EnableWizardToLearnSchoolAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnSchoolAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnSchoolAtLevel3 = toggle;
            OneDndContext.SwitchOneDndWizardSchoolOfMagicLearningLevel();
        }

        toggle = Main.Settings.EnableSignatureSpellsRelearn;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSignatureSpellsRelearn"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSignatureSpellsRelearn = toggle;
        }

        UI.Label();
    }
}
