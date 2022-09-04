using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty;

//PATCH: EnableTogglesToOverwriteDefaultTestParty
[HarmonyPatch(typeof(SessionState_Setup), "Begin")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SessionState_Setup_Begin
{
    // ReSharper disable once UnusedMember.Global
    public static void AssignCharacterToPlayer(
        [NotNull] Session session,
        int playerIndex,
        int slotIndex,
        string filename,
        bool notify)
    {
        if (!Global.IsMultiplayer
            && Main.Settings.EnableTogglesToOverwriteDefaultTestParty
            && slotIndex < Main.Settings.defaultPartyHeroes.Count)
        {
            var characterPoolService = ServiceRepository.GetService<ICharacterPoolService>();

            filename = characterPoolService.BuildCharacterFilename(
                Main.Settings.defaultPartyHeroes.ElementAt(slotIndex));
        }

        session.AssignCharacterToPlayer(playerIndex, slotIndex, filename, notify);
    }

    [CanBeNull]
    // ReSharper disable once UnusedMember.Global
    public static List<string> PredefinedParty([NotNull] CampaignDefinition campaignDefinition)
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

    [ItemNotNull]
    // ReSharper disable once UnusedMember.Global
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var assignCharacterToPlayerMethod = typeof(Session).GetMethod("AssignCharacterToPlayer");
        var myAssignCharacterToPlayerMethod = typeof(SessionState_Setup_Begin).GetMethod("AssignCharacterToPlayer");
        var predefinedPartyMethod = typeof(CampaignDefinition).GetMethod("get_PredefinedParty");
        var myPredefinedPartyMethod = typeof(SessionState_Setup_Begin).GetMethod("PredefinedParty");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(assignCharacterToPlayerMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, myAssignCharacterToPlayerMethod);
            }
            else if (instruction.Calls(predefinedPartyMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, myPredefinedPartyMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
