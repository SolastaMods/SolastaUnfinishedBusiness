using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(SessionState_Setup), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SessionState_Setup_Begin
    {
        public static void AssignCharacterToPlayer(
            Session session,
            int playerIndex,
            int slotIndex,
            string filename,
            bool notify)
        {
            if (Main.Settings.EnableTogglesToOverwriteDefaultTestParty && slotIndex < Main.Settings.TestPartyHeroes.Count)
            {
                session.AssignCharacterToPlayer(playerIndex, slotIndex, Main.Settings.TestPartyHeroes.ElementAt(slotIndex), notify);
            }
            else
            {
                session.AssignCharacterToPlayer(playerIndex, slotIndex, filename, notify);
            }
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var assignCharacterToPlayerMethod = typeof(Session).GetMethod("AssignCharacterToPlayer");
            var myAssignCharacterToPlayerMethod = typeof(SessionState_Setup_Begin).GetMethod("AssignCharacterToPlayer");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(assignCharacterToPlayerMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, myAssignCharacterToPlayerMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
