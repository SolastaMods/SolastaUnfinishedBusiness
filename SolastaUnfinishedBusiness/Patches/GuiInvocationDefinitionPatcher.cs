using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

public static class GuiInvocationDefinitionPatcher
{
    //NOTE: There is a typo on official method name
    [HarmonyPatch(typeof(GuiInvocationDefinition), "IsInvocationMacthingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsInvocationMacthingPrerequisites_Patch
    {
        //PATCH: return class level instead of char level on invocations selection (MULTICLASS)
        private static int TryGetAttributeValue(
            RulesetCharacterHero hero,
            string attributeName)
        {
            hero.ClassesAndLevels.TryGetValue(DatabaseHelper.CharacterClassDefinitions.Warlock, out var levels);

            return levels;
        }

        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .ReplaceCalls(typeof(RulesetEntity).GetMethod("TryGetAttributeValue"),
                    "GuiInvocationDefinition.IsInvocationMacthingPrerequisites",
                    new CodeInstruction(OpCodes.Call,
                        new Func<RulesetCharacterHero, string, int>(TryGetAttributeValue).Method));
        }
    }
}
