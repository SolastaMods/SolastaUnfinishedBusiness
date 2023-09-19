using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationEffectPatcher
{
    //PATCH: this is mostly vanilla game code so I can get a better sense where code is failing
    [HarmonyPatch(typeof(GameLocationEffect), nameof(GameLocationEffect.SerializeAttributes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RevealCharacter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationEffect __instance,
            IAttributesSerializer serializer,
            IVersionProvider versionProvider)
        {
            var step = 0;

            Main.Info($"SERIALIZATION: step {step++}");

            if (versionProvider.GetVersionNumber() >= 3U)
            {
                __instance.effectSourceName =
                    serializer.SerializeAttribute("EffectSourceName", __instance.effectSourceName);
            }

            Main.Info($"SERIALIZATION: step {step++}");

            ulong num = 0;

            if (serializer.Mode == Serializer.SerializationMode.Write)
            {
                num = __instance.rulesetEffect.Guid;
            }

            Main.Info($"SERIALIZATION: step {step++}");

            var guid = serializer.SerializeAttribute("RulesetEffectGuid", num);

            if (RulesetEntity.TryGetEntity(guid, out __instance.rulesetEffect))
            {
                __instance.rulesetEffect.EntityImplementation = __instance;
            }
            //BEGIN PATCH: avoid trace messages
            // else
            // {
            //     Trace.LogError("Cannot reconcile RulesetEffect {0} of id {1}", __instance.effectSourceName,
            //         guid.ToString());
            //     Trace.LogException(new Exception("[Tactical - Invisible for players] Cannot reconcile RulesetEffect " +
            //                                      __instance.effectSourceName + "."));
            // }

            Main.Info($"SERIALIZATION: step {step++}");

            __instance.position = serializer.SerializeAttribute("Position", __instance.position);
            __instance.position2 = serializer.SerializeAttribute("Position2", __instance.position2);

            Main.Info($"SERIALIZATION: step {step++}");

            if (versionProvider.GetVersionNumber() < 2U)
            {
                serializer.SerializeAttribute("Direction", new Vector3());
            }

            Main.Info($"SERIALIZATION: step {step++}");

            if (versionProvider.GetVersionNumber() >= 1U)
            {
                Main.Info($"SERIALIZATION: step {step++}a");

                __instance.sourceOriginalPosition =
                    serializer.SerializeAttribute("SourceOriginalPosition", __instance.sourceOriginalPosition);
            }
            else if (serializer.Mode == Serializer.SerializationMode.Read)
            {
                Main.Info($"SERIALIZATION: step {step++}b");

                __instance.sourceOriginalPosition = __instance.position;

                if (__instance.rulesetEffect != null
                    && __instance.rulesetEffect.SourceGuid != 0 //PATCH: check if source guid is zero
                    && RulesetEntity.TryGetEntity(__instance.rulesetEffect.SourceGuid, out RulesetCharacter entity)
                    && entity != null) //PATCH: check if entity is null
                {
                    var fromActor = GameLocationCharacter.GetFromActor(entity);

                    if (fromActor != null)
                    {
                        __instance.sourceOriginalPosition = fromActor.LocationPosition;
                    }
                }
            }

            Main.Info($"SERIALIZATION: step {step++}");

            if (versionProvider.GetVersionNumber() >= 4U)
            {
                __instance.hasMagneticTargeting =
                    serializer.SerializeAttribute("HasMagneticTargeting", __instance.hasMagneticTargeting);
            }

            Main.Info($"SERIALIZATION: step {step}");

            if (versionProvider.GetVersionNumber() >= 5U)
            {
                __instance.sourceGuid = serializer.SerializeAttribute("SourceGuid", __instance.sourceGuid);
            }

            return false;
        }
    }
}
