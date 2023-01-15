using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationActionManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendSpellSlot")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactToSpendSpellSlot_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendSpellSlot` reaction with custom one
            __instance.AddInterruptRequest(new ReactionRequestSpendSpellSlotExtended(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactForOpportunityAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactForOpportunityAttack_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `OpportunityAttack` reaction with warcaster one
            __instance.AddInterruptRequest(new ReactionRequestWarcaster(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactToSpendPower_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendPower` reaction for bundled powers or customized one for other powers
            if (reactionParams.RulesetEffect is not RulesetEffectPower powerEffect)
            {
                return true;
            }

            if (powerEffect.PowerDefinition.IsBundlePower())
            {
                __instance.AddInterruptRequest(new ReactionRequestSpendBundlePower(reactionParams));
            }
            else
            {
                __instance.AddInterruptRequest(new ReactionRequestSpendPowerCustom(reactionParams));
            }

            return false;
        }
    }
}
