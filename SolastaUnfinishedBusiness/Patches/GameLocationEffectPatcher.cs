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
            if (versionProvider.GetVersionNumber() >= 3U)
            {
                __instance.effectSourceName =
                    serializer.SerializeAttribute("EffectSourceName", __instance.effectSourceName);
            }

            ulong num = 0;

            //PATCH: original code doesn't check for null rulesetEffect
            if (__instance.rulesetEffect != null)
            {
                if (serializer.Mode == Serializer.SerializationMode.Write)
                {
                    num = __instance.rulesetEffect.Guid;
                }
            }
            else
            {
                //Main.Info("SERIALIZATION: null rulesetEffect");
            }

            var guid = serializer.SerializeAttribute("RulesetEffectGuid", num);

            if (RulesetEntity.TryGetEntity(guid, out __instance.rulesetEffect))
            {
                //Main.Info($"SERIALIZATION: {__instance.rulesetEffect.Name}");
                __instance.rulesetEffect.EntityImplementation = __instance;
            }
            else
            {
                //Main.Info($"Cannot reconcile RulesetEffect {__instance.effectSourceName} of id {guid}");
            }

            __instance.position = serializer.SerializeAttribute("Position", __instance.position);
            __instance.position2 = serializer.SerializeAttribute("Position2", __instance.position2);

            if (versionProvider.GetVersionNumber() < 2U)
            {
                serializer.SerializeAttribute("Direction", new Vector3());
            }

            if (versionProvider.GetVersionNumber() >= 1U)
            {
                __instance.sourceOriginalPosition =
                    serializer.SerializeAttribute("SourceOriginalPosition", __instance.sourceOriginalPosition);
            }
            else if (serializer.Mode == Serializer.SerializationMode.Read)
            {
                __instance.sourceOriginalPosition = __instance.position;

                if (__instance.rulesetEffect != null
                    && RulesetEntity.TryGetEntity(__instance.rulesetEffect.SourceGuid, out RulesetCharacter entity))
                {
                    var fromActor = GameLocationCharacter.GetFromActor(entity);

                    if (fromActor != null)
                    {
                        __instance.sourceOriginalPosition = fromActor.LocationPosition;
                    }
                }
            }

            if (versionProvider.GetVersionNumber() >= 4U)
            {
                __instance.hasMagneticTargeting =
                    serializer.SerializeAttribute("HasMagneticTargeting", __instance.hasMagneticTargeting);
            }

            if (versionProvider.GetVersionNumber() >= 5U)
            {
                __instance.sourceGuid = serializer.SerializeAttribute("SourceGuid", __instance.sourceGuid);
            }

            return false;
        }
    }
}
