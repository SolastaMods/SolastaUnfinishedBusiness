using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    internal static class CharacterReactionItemPatcher
    {
        [HarmonyPatch(typeof(CharacterReactionItem), "Bind")]
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

                if (bindIndex > 0)
                {
                    codes[bindIndex] = new CodeInstruction(OpCodes.Call, customBindMethod);
                    codes.Insert(bindIndex, new CodeInstruction(OpCodes.Ldarg_1));
                }

                return codes.AsEnumerable();
            }

            private static void CustomBind(CharacterReactionSubitem instance,
                RulesetSpellRepertoire spellRepertoire,
                int slotLevel,
                string text,
                bool interactable,
                CharacterReactionSubitem.SubitemSelectedHandler subitemSelected, ReactionRequest reactionRequest)
            {
                Main.Log($"CustomBind ", true);
                if (reactionRequest is ReactionRequestWarcaster warcasterRequest)
                {
                    instance.BindWarcaster(warcasterRequest, slotLevel, interactable, subitemSelected);
                }
                else
                {
                    instance.Bind(spellRepertoire, slotLevel, text, interactable, subitemSelected);
                }
            }
        }
    }
}
