using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Models;

internal static class Level20PatchingContext
{
    private const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

    [NotNull]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> Level20Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (Main.Settings.EnableLevel20)
        {
            code
                .FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GameMaxLevel)
                .ForEach(x => x.operand = ModMaxLevel);
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
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"),
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeAttributes"),
            // TODO: The fist 2 - 12 shouldn't really be patched as they are version numbers but doesn't seem to cause issues in logic there...
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"),
            typeof(RulesetEntity).GetMethod("SerializeElements")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(Level20PatchingContext).GetMethod("Level20Transpiler");

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error("cannot fully patch Level 20");
            }
        }
    }
}
