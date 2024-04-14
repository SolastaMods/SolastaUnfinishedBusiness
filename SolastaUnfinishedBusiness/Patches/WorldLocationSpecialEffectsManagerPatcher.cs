using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterAttackDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class WorldLocationSpecialEffectsManagerPatcher
{
    [HarmonyPatch(typeof(WorldLocationSpecialEffectsManager), nameof(WorldLocationSpecialEffectsManager.ChargeStarted))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ChargeStarted_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            WorldLocationSpecialEffectsManager __instance,
            GameLocationCharacter character,
            CharacterActionParams actionParams)
        {
            GameObject prefabStart;
            GameObject prefabLoop;

            if (actionParams.AttackMode?.SourceDefinition is MonsterAttackDefinition sourceDefinition)
            {
                prefabStart = sourceDefinition.ChargeStartParticle;
                prefabLoop = sourceDefinition.ChargeLoopParticle;
            }
            //PATCH: supports heroes using Action Charge
            else
            {
                prefabStart = Attack_Minotaur_Elite_Charged_Gore.ChargeStartParticle;
                prefabLoop = Attack_Minotaur_Elite_Charged_Gore.ChargeLoopParticle;
            }

            if (prefabStart == null)
            {
                return false;
            }

            var sentParameters = new ParticleSentParameters(character, character, string.Empty);

            WorldLocationPoolManager.GetElement(prefabStart, true).GetComponent<ParticleSetup>().Setup(sentParameters);

            if (prefabLoop == null)
            {
                return false;
            }

            sentParameters = new ParticleSentParameters(character, character, string.Empty);

            var component = WorldLocationPoolManager.GetElement(prefabLoop, true).GetComponent<ParticleSetup>();

            component.Setup(sentParameters);
            __instance.chargeLoopParticleDictionary.TryAdd(character, component);

            return false;
        }
    }

    [HarmonyPatch(typeof(WorldLocationSpecialEffectsManager), nameof(WorldLocationSpecialEffectsManager.ChargeEnded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ChargeEnded_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            WorldLocationSpecialEffectsManager __instance,
            GameLocationCharacter character,
            CharacterActionParams actionParams)
        {
            GameObject prefabEnd;

            if (actionParams.AttackMode?.SourceDefinition is MonsterAttackDefinition sourceDefinition)
            {
                prefabEnd = sourceDefinition.ChargeEndParticle;
            }
            //PATCH: supports heroes using Action Charge
            else
            {
                prefabEnd = Attack_Minotaur_Elite_Charged_Gore.ChargeEndParticle;
            }

            ParticleSetup particleSetup;

            if (prefabEnd != null)
            {
                var sentParameters = new ParticleSentParameters(character, character, string.Empty);

                particleSetup = WorldLocationPoolManager.GetElement(prefabEnd, true).GetComponent<ParticleSetup>();
                particleSetup.Setup(sentParameters);
            }

            if (!__instance.chargeLoopParticleDictionary.TryGetValue(character, out particleSetup))
            {
                return false;
            }

            particleSetup.StopAndReturnToPool();
            __instance.chargeLoopParticleDictionary.Remove(character);

            return false;
        }
    }

    [HarmonyPatch(typeof(WorldLocationSpecialEffectsManager),
        nameof(WorldLocationSpecialEffectsManager.PrepareChargeStarted))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PrepareChargeStarted_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationCharacter character,
            CharacterActionParams actionParams)
        {
            GameObject prefabPrepare;

            if (actionParams.AttackMode?.SourceDefinition is MonsterAttackDefinition sourceDefinition)
            {
                prefabPrepare = sourceDefinition.ChargePrepareParticle;
            }
            //PATCH: supports heroes using Action Charge
            else
            {
                prefabPrepare = Attack_Minotaur_Elite_Charged_Gore.ChargePrepareParticle;
            }

            if (prefabPrepare == null)
            {
                return false;
            }

            var sentParameters = new ParticleSentParameters(character, character, string.Empty);

            WorldLocationPoolManager.GetElement(prefabPrepare, true).GetComponent<ParticleSetup>()
                .Setup(sentParameters);

            return false;
        }
    }
}
