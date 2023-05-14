using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationPositioningManagerPatcher
{
    //PATCH: avoids a trace message when party greater than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationPositioningManager), nameof(GameLocationPositioningManager.CharacterMoved),
        typeof(GameLocationCharacter),
        typeof(int3), typeof(int3), typeof(RulesetActor.SizeParameters), typeof(RulesetActor.SizeParameters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CharacterMoved_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(string) }, null);

            return instructions.ReplaceCalls(logErrorMethod, "GameLocationPositioningManager.CharacterMoved",
                new CodeInstruction(OpCodes.Pop));
        }
    }

    //PATH: Fire monsters should emit light
    [HarmonyPatch(typeof(GameLocationCharacterManager), nameof(GameLocationCharacterManager.RevealCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RevealCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter character)
        {
            if (!Main.Settings.EnableCharactersOnFireToEmitLight)
            {
                return;
            }

            var rulesetCharacter = character.RulesetCharacter;

            if (rulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
            {
                return;
            }

            var monstersThatEmitLight = new List<MonsterDefinition>
            {
                DatabaseHelper.MonsterDefinitions.CubeOfLight,
                DatabaseHelper.MonsterDefinitions.Fire_Elemental,
                DatabaseHelper.MonsterDefinitions.Fire_Jester,
                DatabaseHelper.MonsterDefinitions.Fire_Osprey,
                DatabaseHelper.MonsterDefinitions.Fire_Spider
            };

            if (!monstersThatEmitLight.Contains(rulesetCharacterMonster.MonsterDefinition))
            {
                return;
            }

            rulesetCharacterMonster.PersonalLightSource = new RulesetLightSource(
                AdditionalDamageBrandingSmite.LightSourceForm.Color,
                2,
                4,
                AdditionalDamageBrandingSmite.LightSourceForm.GraphicsPrefabAssetGUID,
                RuleDefinitions.LightSourceType.Basic,
                rulesetCharacterMonster.MonsterDefinition.Name,
                rulesetCharacterMonster.Guid);

            rulesetCharacterMonster.PersonalLightSource.Register(true);

            ServiceRepository.GetService<IGameLocationVisibilityService>()?
                .AddCharacterLightSource(character, rulesetCharacterMonster.PersonalLightSource);
        }
    }
}
