using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
/*

DOCUMENTATION:

There are some official game delegates I recreated here for easier reference. They can be used to get an action context or 
even have more control on magical effects.

As I'm not sure they might be enough I also revamped the original IOnEffectInterface and we now have 3 interfaces handling
the sequences below...

HOW-TO USE THESE INTERFACES:

You can have control over attack sequences and create a Feature that implements more than one interface for an advanced logic

A. weapon attack sequence:

    1. HandleCharacterAttack
    2. HandleCharacterAttackHit (only if hit)
    3. HandleCharacterAttackDamage (only if hit)
    4. HandleCharacterAttackFinished

B. spell attack sequence (area effect spells):

    1. HandleCharacterMagicalAttackDamage

C. spell attack sequence (targeting spells):

    1. HandleCharacterAttackHit (only if hit)
    2. HandleCharacterMagicalAttackDamage (only if hit)
    3. HandleCharacterAttackDamage (only if hit)

*/
    internal static class CharacterActionContext
    {
        public static Dictionary<ActionDefinitions.Id, ActionDefinition> AllActionDefinitions =>
            ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions;

        public static Stack<ReactionRequestGroup> PendingReactionRequestGroups =>
            ServiceRepository.GetService<IGameLocationActionService>().PendingReactionRequestGroups;

        public static ReactionRequest.ReactionTriggeredHandler ReactionTriggered { get; set; }

        public static ReactionRequest.ReactionRequestProcessedHandler ReactionRequestProcessed { get; set; }

        public static ActionDefinitions.ActionStartedHandler ActionStarted { get; set; }

        public static ActionDefinitions.ActionChainStartedHandler ActionChainStarted { get; set; }

        public static ActionDefinitions.ActionChainFinishedHandler ActionChainFinished { get; set; }

        public static ActionDefinitions.MagicEffectPreparingHandler MagicEffectPreparing { get; set; }

        public static ActionDefinitions.MagicEffectPreparingOnTargetHandler MagicEffectPreparingOnTarget { get; set; }

        public static ActionDefinitions.MagicEffectLaunchHandler MagicEffectLaunch { get; set; }

        public static ActionDefinitions.MagicEffectCastOnTargetHandler MagicEffectCastOnTarget { get; set; }

        public static ActionDefinitions.MagicEffectCastOnZoneHandler MagicEffectCastOnZone { get; set; }

        public static ActionDefinitions.MagicEffectBeforeHitTargetHandler MagicEffectBeforeHitTarget { get; set; }

        public static ActionDefinitions.MagicEffectHitTargetHandler MagicEffectHitTarget { get; set; }

        public static ActionDefinitions.SpellCastHandler SpellCast { get; set; }

        public static ActionDefinitions.ItemUsedHandler ItemUsed { get; set; }

        public static ActionDefinitions.ActionUsedHandler ActionUsed { get; set; }

        public static ActionDefinitions.ShoveActionUsedHandler ShoveActionUsed { get; set; }

        public static bool IsAttackAction(ActionDefinitions.Id actionId) => actionId == ActionDefinitions.Id.AttackMain || actionId == ActionDefinitions.Id.AttackOff || actionId == ActionDefinitions.Id.AttackReadied || actionId == ActionDefinitions.Id.AttackOpportunity;

        public static bool IsSpellAction(ActionDefinitions.Id actionId) => actionId == ActionDefinitions.Id.CastMain || actionId == ActionDefinitions.Id.CastBonus || actionId == ActionDefinitions.Id.CastReaction || actionId == ActionDefinitions.Id.CastReadied || actionId == ActionDefinitions.Id.CastRitual || actionId == ActionDefinitions.Id.CastNoCost;

        public static bool IsPowerAction(ActionDefinitions.Id actionId) => actionId == ActionDefinitions.Id.PowerMain || actionId == ActionDefinitions.Id.PowerBonus || actionId == ActionDefinitions.Id.PowerNoCost || actionId == ActionDefinitions.Id.PowerReaction || actionId == ActionDefinitions.Id.VampiricTouch || actionId == ActionDefinitions.Id.BreakEnchantment || actionId == ActionDefinitions.Id.Dismissal;

        public static bool IsProxyAction(ActionDefinitions.Id actionId, bool includeFree = true) => actionId == ActionDefinitions.Id.ProxySpiritualWeapon || actionId == ActionDefinitions.Id.ProxySpiritualWeaponFree & includeFree || actionId == ActionDefinitions.Id.ProxyFlamingSphere || actionId == ActionDefinitions.Id.ProxyDancingLights || actionId == ActionDefinitions.Id.ProxyMoonBeam || actionId == ActionDefinitions.Id.ProxyCallLightning || actionId == ActionDefinitions.Id.ProxyVengefulSpirits || actionId == ActionDefinitions.Id.ProxyCallLightningFree & includeFree;

        public static bool IsProxyFreeAction(ActionDefinitions.Id actionId) => actionId == ActionDefinitions.Id.ProxySpiritualWeaponFree || actionId == ActionDefinitions.Id.ProxyCallLightningFree;

        public static bool IsTargetingAction(ActionDefinitions.Id actionId) => actionId == ActionDefinitions.Id.CoordinatedDefense;

        internal static void Load()
        {
            var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();

            gameLocationActionService.ReactionTriggered += ReactionTriggered;
            gameLocationActionService.ReactionRequestProcessed += ReactionRequestProcessed;

            gameLocationActionService.ActionStarted += ActionStarted;

            gameLocationActionService.ActionChainStarted += ActionChainStarted;
            gameLocationActionService.ActionChainFinished += ActionChainFinished;

            gameLocationActionService.MagicEffectPreparing += MagicEffectPreparing;
            gameLocationActionService.MagicEffectPreparingOnTarget += MagicEffectPreparingOnTarget;
            gameLocationActionService.MagicEffectCastOnTarget += MagicEffectCastOnTarget;
            gameLocationActionService.MagicEffectCastOnZone += MagicEffectCastOnZone;
            gameLocationActionService.MagicEffectBeforeHitTarget += MagicEffectBeforeHitTarget;
            gameLocationActionService.MagicEffectHitTarget += MagicEffectHitTarget;

            gameLocationActionService.SpellCast += SpellCast;
            gameLocationActionService.ItemUsed += ItemUsed;
            gameLocationActionService.ActionUsed += ActionUsed;
            gameLocationActionService.ShoveActionUsed += ShoveActionUsed;
        }
    }
}
