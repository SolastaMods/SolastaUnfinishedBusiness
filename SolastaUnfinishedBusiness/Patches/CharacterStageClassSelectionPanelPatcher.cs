using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageClassSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), nameof(CharacterStageClassSelectionPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterStageClassSelectionPanel __instance)
        {
            //PATCH: avoids a restart when enabling / disabling classes on the Mod UI panel
            var visibleClasses = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                .Where(x => !x.GuiPresentation.Hidden)
                .OrderBy(x => x.FormatTitle());

            __instance.compatibleClasses.SetRange(visibleClasses);

            if (!LevelUpContext.IsLevelingUp(__instance.currentHero))
            {
                return;
            }

            //PATCH: mark we started selecting classes (MULTICLASS)
            LevelUpContext.SetIsClassSelectionStage(__instance.currentHero, true);

            //PATCH: apply in/out logic (MULTICLASS)
            MulticlassInOutRulesContext.EnumerateHeroAllowedClassDefinitions(
                __instance.currentHero,
                __instance.compatibleClasses,
                out __instance.selectedClass);

            //PATCH: refresh the panel (MULTICLASS)
            var commonData = __instance.CommonData;

            // NOTE: don't use AttackModesPanel?. which bypasses Unity object lifetime check
            if (commonData.AttackModesPanel)
            {
                commonData.AttackModesPanel.RefreshNow();
            }

            // NOTE: don't use PersonalityMapPanel?. which bypasses Unity object lifetime check
            if (commonData.PersonalityMapPanel)
            {
                commonData.PersonalityMapPanel.RefreshNow();
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel),
        nameof(CharacterStageClassSelectionPanel.FillClassFeatures))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FillClassFeatures_Patch
    {
        //PATCH: hides the features list for already acquired classes (MULTICLASS)
        private static int Level(
            [NotNull] FeatureUnlockByLevel featureUnlockByLevel,
            [NotNull] RulesetCharacterHero hero)
        {
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var selectedClass = LevelUpContext.GetSelectedClass(hero);

            if (!isLevelingUp)
            {
                return featureUnlockByLevel.Level;
            }

            var levels = 0;

            if (selectedClass
                && hero.ClassesAndLevels.TryGetValue(selectedClass, out levels)
                && featureUnlockByLevel.Level != levels + 1)
            {
                return int.MaxValue;
            }

            if (levels == 0)
            {
                return featureUnlockByLevel.Level;
            }

            return featureUnlockByLevel.Level - 1;
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var levelMethod = typeof(FeatureUnlockByLevel).GetMethod("get_Level");
            var myLevelMethod = new Func<FeatureUnlockByLevel, RulesetCharacterHero, int>(Level).Method;
            var currentHeroField =
                typeof(CharacterStageClassSelectionPanel).GetField("currentHero",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            return instructions.ReplaceCalls(levelMethod, "CharacterStageClassSelectionPanel.FillClassFeatures",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, currentHeroField),
                new CodeInstruction(OpCodes.Call, myLevelMethod));
        }
    }

    //PATCH: hides the equipment panel group (MULTICLASS)
    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), nameof(CharacterStageClassSelectionPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        private static bool SetActive([NotNull] RulesetCharacterHero currentHero)
        {
            return !LevelUpContext.IsLevelingUp(currentHero);
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
            var mySetActiveMethod = new Func<RulesetCharacterHero, bool>(SetActive).Method;
            var currentHeroField =
                typeof(CharacterStageClassSelectionPanel).GetField("currentHero",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            return instructions.ReplaceCall(setActiveMethod,
                4, "CharacterStageClassSelectionPanel.Refresh",
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, currentHeroField),
                new CodeInstruction(OpCodes.Call, mySetActiveMethod),
                new CodeInstruction(OpCodes.Call, setActiveMethod)); // checked for Call vs CallVirtual
        }
    }
}
