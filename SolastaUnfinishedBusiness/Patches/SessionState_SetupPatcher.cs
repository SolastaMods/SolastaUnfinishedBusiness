using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SessionState_SetupPatcher
{
    [HarmonyPatch(typeof(SessionState_Setup), nameof(SessionState_Setup.Begin))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Begin_Patch
    {
        //PATCH: assign selected heroes to predefined party used to test dungeons (DEFAULT_PARTY)
        private static void AssignCharacterToPlayer(
            Session session,
            int playerIndex,
            int slotIndex,
            string filename,
            bool notify)
        {
            if (Main.Settings.EnableTogglesToOverwriteDefaultTestParty &&
                slotIndex < Main.Settings.DefaultPartyHeroes.Count)
            {
                var name = Main.Settings.DefaultPartyHeroes.ElementAt(slotIndex);
                var isBuiltIn = ToolsContext.IsBuiltIn(name);

                filename = Path.Combine(
                    !isBuiltIn
                        ? TacticalAdventuresApplication.GameCharactersDirectory
                        : TacticalAdventuresApplication.GameBuiltInCharactersDirectory, name) + ".chr";
            }

            session.AssignCharacterToPlayer(playerIndex, slotIndex, filename, notify);
        }

        //PATCH: assign selected heroes to predefined party used to test dungeons (DEFAULT_PARTY)
        private static List<string> PredefinedParty(CampaignDefinition campaignDefinition)
        {
            if (Global.IsMultiplayer
                || campaignDefinition.PredefinedParty == null
                || campaignDefinition.PredefinedParty.Count == 0)
            {
                return campaignDefinition.PredefinedParty;
            }

            var max = Main.Settings.OverridePartySize;
            var result = campaignDefinition.PredefinedParty.ToList();

            while (result.Count > max)
            {
                result.RemoveAt(0);
            }

            while (result.Count < max)
            {
                result.Add(result[result.Count % 4]);
            }

            return result;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var assignCharacterToPlayerMethod = typeof(Session).GetMethod("AssignCharacterToPlayer");
            var myAssignCharacterToPlayerMethod =
                new Action<Session, int, int, string, bool>(AssignCharacterToPlayer).Method;
            var predefinedPartyMethod = typeof(CampaignDefinition).GetMethod("get_PredefinedParty");
            var myPredefinedPartyMethod = new Func<CampaignDefinition, List<string>>(PredefinedParty).Method;

            return instructions
                .ReplaceCalls(assignCharacterToPlayerMethod, "SessionState_Setup.Begin.1",
                    new CodeInstruction(OpCodes.Call, myAssignCharacterToPlayerMethod))
                .ReplaceCalls(predefinedPartyMethod, "SessionState_Setup.Begin.2",
                    new CodeInstruction(OpCodes.Call, myPredefinedPartyMethod));
        }
    }
}
