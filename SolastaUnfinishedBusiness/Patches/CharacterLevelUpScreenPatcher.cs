using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterLevelUpScreenPatcher
{
    [HarmonyPatch(typeof(CharacterLevelUpScreen), nameof(CharacterLevelUpScreen.InitiateLevelUpInGame))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InitiateLevelUpInGame_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref bool canUnlearn)
        {
            //PATCH: TA made it so that if you levelup more than 1 level without closing levelup screen
            //(like when starting PoI with low level party) then you can't unlearn spells or invocations.
            //This change removes that limitation.
            if (Main.Settings.DisableStreamlinedMultiLevelUp)
            {
                canUnlearn = true;
            }
        }
    }
}
