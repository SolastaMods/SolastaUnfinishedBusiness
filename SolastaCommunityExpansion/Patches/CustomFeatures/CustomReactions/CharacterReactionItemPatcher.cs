using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomUI;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

[HarmonyPatch(typeof(CharacterReactionItem), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterReactionItem_Bind
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var customBindMethod =
            new Action<CharacterReactionSubitem, RulesetSpellRepertoire, int, string, bool,
                CharacterReactionSubitem.SubitemSelectedHandler, ReactionRequest>(CustomBind).Method;

        var bind = typeof(CharacterReactionSubitem).GetMethod("Bind",
            BindingFlags.Public | BindingFlags.Instance);

        var bindIndex = codes.FindIndex(x => x.Calls(bind));

        if (bindIndex <= 0)
        {
            return codes.AsEnumerable();
        }

        codes[bindIndex] = new CodeInstruction(OpCodes.Call, customBindMethod);
        codes.Insert(bindIndex, new CodeInstruction(OpCodes.Ldarg_1));

        return codes.AsEnumerable();
    }

    internal static void Postfix(CharacterReactionItem __instance)
    {
        var request = __instance.ReactionRequest;
        var size = request is ReactionRequestWarcaster || request is ReactionRequestSpendBundlePower
            ? 400
            : 290;

        __instance.GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
    }

    private static void CustomBind(CharacterReactionSubitem instance,
        RulesetSpellRepertoire spellRepertoire,
        int slotLevel,
        string text,
        bool interactable,
        CharacterReactionSubitem.SubitemSelectedHandler subitemSelected, ReactionRequest reactionRequest)
    {
        if (reactionRequest is ReactionRequestWarcaster warcasterRequest)
        {
            instance.BindWarcaster(warcasterRequest, slotLevel, interactable, subitemSelected);
        }
        else if (reactionRequest is ReactionRequestSpendBundlePower bundlePoweRequest)
        {
            instance.BindPowerBundle(bundlePoweRequest, slotLevel, interactable, subitemSelected);
        }
        else
        {
            instance.Bind(spellRepertoire, slotLevel, text, interactable, subitemSelected);
        }
    }
}
