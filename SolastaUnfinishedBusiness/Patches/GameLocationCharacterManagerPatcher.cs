using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationCharacterManagerPatcher
{
    //BUGFIX: fix demonic influence getting all enemies agro on a custom map
    [HarmonyPatch(typeof(GameLocationCharacterManager), nameof(GameLocationCharacterManager.CreateCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpawnParty_Patch
    {
        [UsedImplicitly]
        public static void Prefix(Side side, ref GameLocationBehaviourPackage behaviourPackage)
        {
            if (side == Side.Ally)
            {
                behaviourPackage ??= new GameLocationBehaviourPackage
                {
                    BattleStartBehavior = GameLocationBehaviourPackage.BattleStartBehaviorType.DoNotRaiseAlarm
                };
            }
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
            SrdAndHouseRulesContext.AddLightSourceIfNeeded(character);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacterManager),
        nameof(GameLocationCharacterManager.CreateAndBindEffectProxy))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateAndBindEffectProxy_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationCharacterManager __instance,
            RulesetActor rulesetEntity,
            RulesetEffect rulesetEffect,
            int3 position,
            EffectProxyDefinition effectProxyDefinition)
        {
            CreateAndBindEffectProxy(__instance, rulesetEntity, rulesetEffect, position, effectProxyDefinition);

            return false;
        }

        // vanilla code except for BEGIN / END PATCH block to support Circle of the Wildfire use case
        private static void CreateAndBindEffectProxy(
            GameLocationCharacterManager __instance,
            RulesetActor rulesetEntity,
            RulesetEffect rulesetEffect,
            int3 position,
            EffectProxyDefinition effectProxyDefinition)
        {
            if (!effectProxyDefinition)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null effectProxyDefinition in CreateAndBindEffectProxy."));
            }
            else if (rulesetEffect == null)
            {
                Trace.LogError("Null rulesetEffect for proxy {0}", effectProxyDefinition.Name);
                Trace.LogException(
                    new Exception("[TACTICAL INVISIBLE FOR PLAYERS] null rulesetEffect in CreateAndBindEffectProxy."));
            }
            else if (!rulesetEffect.SourceDefinition)
            {
                Trace.LogError("Null rulesetEffect.SourceDefinition for proxy {0}", effectProxyDefinition.Name);
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null rulesetEffect.SourceDefinition in CreateAndBindEffectProxy."));
            }
            else if (rulesetEffect.EffectDescription == null)
            {
                Trace.LogError("Null rulesetEffect.EffectDescription for proxy {0}", effectProxyDefinition.Name);
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null rulesetEffect.EffectDescription in CreateAndBindEffectProxy."));
            }
            else if (rulesetEntity == null)
            {
                Trace.LogError("Null rulesetEntity for proxy {0} and rulesetEffect {1}", effectProxyDefinition.Name,
                    rulesetEffect.SourceDefinition.Name);
                Trace.LogException(
                    new Exception("[TACTICAL INVISIBLE FOR PLAYERS] null rulesetEntity in CreateAndBindEffectProxy."));
            }
            else if (rulesetEntity.EntityImplementation == null)
            {
                Trace.LogError("Null rulesetEntity for proxy {0} and rulesetEffect {1} and rulesetEntity {2}",
                    effectProxyDefinition.Name, rulesetEffect.SourceDefinition.Name, rulesetEntity.Name);
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null rulesetEntity.EntityImplementation in CreateAndBindEffectProxy."));
            }
            else if (rulesetEntity.EntityImplementation is not GameLocationCharacter entityImplementation1)
            {
                Trace.LogError("Null controller for proxy {0} and rulesetEffect {1} and rulesetEntity {2}",
                    effectProxyDefinition.Name, rulesetEffect.SourceDefinition.Name, rulesetEntity.Name);
                Trace.LogException(
                    new Exception("[TACTICAL INVISIBLE FOR PLAYERS] null controller in CreateAndBindEffectProxy."));
            }
            else if (entityImplementation1.RulesetCharacter == null)
            {
                Trace.LogError(
                    "Null controller.RulesetCharacter for proxy {0} and rulesetEffect {1} and rulesetEntity {2}",
                    effectProxyDefinition.Name, rulesetEffect.SourceDefinition.Name, rulesetEntity.Name);
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null controller.RulesetCharacter in CreateAndBindEffectProxy."));
            }
            else
            {
                RulesetEffect entity = null;

                foreach (var allProxyCharacter in __instance.allProxyCharacters)
                {
                    if (allProxyCharacter.RulesetCharacter is not RulesetCharacterEffectProxy
                            rulesetCharacterEffectProxy ||
                        rulesetCharacterEffectProxy.ControllerGuid != entityImplementation1.Guid ||
                        rulesetCharacterEffectProxy.EffectDefinitionName != rulesetEffect.SourceDefinition.Name ||
                        rulesetCharacterEffectProxy.EffectProxyDefinition != effectProxyDefinition ||
                        // BEGIN PATCH
                        // supports Circle of the Wildfire 
                        rulesetCharacterEffectProxy.EffectDefinitionName ==
                        CircleOfTheWildfire.PowerSummonCauterizingFlamesName)
                        // END PATCH
                    {
                        continue;
                    }

                    if (RulesetEntity.TryGetEntity(rulesetCharacterEffectProxy.EffectGuid, out entity))
                    {
                        break;
                    }
                }

                entity?.Terminate(true);

                var characterEffectProxy = new RulesetCharacterEffectProxy(
                    effectProxyDefinition,
                    entityImplementation1.Guid,
                    rulesetEffect.Guid,
                    rulesetEffect.SourceDefinition.Name,
                    rulesetEffect.SourceAbility,
                    entityImplementation1.RulesetCharacter.TryGetAttributeValue("ProficiencyBonus"),
                    rulesetEffect.ComputeSourceAbilityBonus(entityImplementation1.RulesetCharacter),
                    rulesetEffect.SaveDC,
                    entityImplementation1.RulesetCharacter.Side);

                entityImplementation1.RulesetCharacter.BindEffectProxy(characterEffectProxy);
                characterEffectProxy.Register(true);

                var character = new GameLocationCharacter(entityImplementation1.ControllerId);

                character.SetRuleset(characterEffectProxy);
                character.ChangeSide(entityImplementation1.Side);
                characterEffectProxy.RefreshAll();
                character.RefreshActionPerformances();
                rulesetEffect.TrackEffectProxy(characterEffectProxy);

                var entityImplementation = rulesetEffect.EntityImplementation as GameLocationEffect;

                character.LocationPosition =
                    entityImplementation == null ||
                    rulesetEffect.EffectDescription.TargetType != TargetType.WallLine
                        ? position
                        : (entityImplementation.Position + entityImplementation.Position2) / 2;

                if (effectProxyDefinition.AddLightSource)
                {
                    var service = ServiceRepository.GetService<IGameLocationVisibilityService>();
                    var lightSourceForm = effectProxyDefinition.LightSourceForm;

                    if (lightSourceForm != null)
                    {
                        float brightRange = lightSourceForm.BrightRange;
                        var dimRangeCells = brightRange + lightSourceForm.DimAdditionalRange;

                        if (rulesetEffect.EffectDescription.TargetType == TargetType.WallLine &&
                            entityImplementation != null)
                        {
                            var num2 = (int)Mathf.Ceil(int3.Distance(
                                entityImplementation.Position, entityImplementation.Position2) / 2);

                            for (var index = 1; index <= num2; ++index)
                            {
                                var t = index / (float)num2;
                                var vector3 = Vector3.Lerp((Vector3)entityImplementation.Position,
                                    (Vector3)entityImplementation.Position2, t);
                                var specificLocationPosition = new int3((int)vector3.x, (int)vector3.y, (int)vector3.z);
                                var rulesetLightSource = new RulesetLightSource(lightSourceForm.Color, brightRange,
                                    dimRangeCells, lightSourceForm.GraphicsPrefabAssetGUID,
                                    lightSourceForm.LightSourceType, effectProxyDefinition.Name,
                                    characterEffectProxy.Guid, useSpecificLocationPosition: true,
                                    specificLocationPosition: specificLocationPosition, effectGuid: rulesetEffect.Guid);

                                characterEffectProxy.AddAdditionalPersonalLightSource(rulesetLightSource);
                                rulesetLightSource.Register(true);
                                service.AddCharacterLightSource(character, rulesetLightSource);
                                rulesetEffect.TrackLightSource(
                                    characterEffectProxy, character.Guid, string.Empty, rulesetLightSource);
                            }
                        }
                        else
                        {
                            characterEffectProxy.PersonalLightSource = new RulesetLightSource(
                                lightSourceForm.Color,
                                brightRange, dimRangeCells,
                                lightSourceForm.GraphicsPrefabAssetGUID,
                                lightSourceForm.LightSourceType,
                                effectProxyDefinition.Name,
                                characterEffectProxy.Guid,
                                effectGuid: rulesetEffect.Guid);
                            characterEffectProxy.PersonalLightSource.Register(true);
                            service.AddCharacterLightSource(character, characterEffectProxy.PersonalLightSource);
                            rulesetEffect.TrackLightSource(
                                characterEffectProxy, character.Guid,
                                string.Empty,
                                characterEffectProxy.PersonalLightSource);
                        }
                    }
                }

                __instance.allProxyCharacters.Add(character);
                character.Visible = true;

                var characterProxyRevealed = __instance.CharacterProxyRevealed;

                characterProxyRevealed?.Invoke(character);

                if (!character.ShouldSelfRegisterInOccupants)
                {
                    return;
                }

                ServiceRepository.GetService<IGameLocationPositioningService>()
                    .PlaceCharacter(character, character.LocationPosition, character.Orientation);
            }
        }
    }

    //PATCH: recalculates additional party members positions (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationCharacterManager),
        nameof(GameLocationCharacterManager.UnlockCharactersForLoading))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnlockCharactersForLoading_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] GameLocationCharacterManager __instance)
        {
            var partyCharacters = __instance.PartyCharacters;

            for (var idx = ToolsContext.GamePartySize; idx < partyCharacters.Count; idx++)
            {
                var position = partyCharacters[idx % ToolsContext.GamePartySize].LocationPosition;

                partyCharacters[idx].LocationPosition = new int3(position.x, position.y, position.z);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacterManager), nameof(GameLocationCharacterManager.LoseWildShapeAndRefund))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LoseWildShapeAndRefund_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] GameLocationCharacterManager __instance, ref bool __result,
            GameLocationCharacter character)
        {
            //PATCH: fixes crashes on characters affected by Shapechange (True Polymorph) or similar
            var rulesetCharacter = character.RulesetCharacter;

            if (!rulesetCharacter.HasConditionOfType(ConditionWildShapeSubstituteForm))
            {
                //not shape shifted - use default method
                return true;
            }

            var power = rulesetCharacter.OriginalFormCharacter
                ?.GetPowerFromDefinition(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape);
            if (power != null)
            {
                //has wildshape power - use default method
                return true;
            }

            //no wildshape power - kill shape shifted form and skip default method
            __instance.KillCharacter(character, false, true, true, true, false);
            __result = true;

            return false;
        }
    }
}
