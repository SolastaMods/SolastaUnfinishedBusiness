using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

internal static class Level20PatchingContext
{
    private const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

    public static IEnumerable<CodeInstruction> Level20Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (Main.Settings.EnableLevel20)
        {
            code
                .FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL)
                .ForEach(x => x.operand = MOD_MAX_LEVEL);
        }

        return code;
    }

    internal static void Load()
    {
        if (!Main.Settings.EnableLevel20)
        {
            return;
        }

        var methods = new[]
        {
            typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding),
            typeof(CharacterBuildingManager).GetMethod("CreateCharacterFromTemplate"),
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding),
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"),
            typeof(GameCampaignParty).GetMethod("UpdateLevelCaps"),
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"),
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"), // TODO: The fist 2 - 12 shouldn't really be patched as they are version numbers but doesn't seem to cause issues in logic there...
            typeof(RulesetEntity).GetMethod("SerializeElements")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(Level20PatchingContext).GetMethod("Level20Transpiler");

        foreach (var method in methods)
        {
            Main.Log(method.Name);
            harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
        }
    }
}
