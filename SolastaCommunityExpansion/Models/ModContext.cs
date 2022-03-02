using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    public static class ModContext
    {
        //
        // delegates that get called before / after these enumerator calls
        // we can use them to get a context on specific character actions or battle scenarios and add custom logic for more complex features
        //

        public delegate void CharacterActionExecute(
            ref CharacterAction characterAction,
            bool isPrefix);

        public delegate void HandleCharacterAttack(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackerAttackMode,
            bool isPrefix);

        public delegate void HandleCharacterAttackDamage(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackMode,
            ref bool rangedAttack,
            ref RuleDefinitions.AdvantageType advantageType,
            ref List<EffectForm> actualEffectForms,
            ref RulesetEffect rulesetEffect,
            ref bool criticalHit,
            ref bool firstTarget,
            bool isPrefix);

        public delegate void HandleCharacterAttackFinished(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref RulesetAttackMode attackerAttackMode,
            bool isPrefix);

        public delegate void HandleCharacterAttackHit(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref int attackRoll,
            ref int successDelta,
            ref bool ranged,
            bool isPrefix);

        public delegate void HandleCharacterMagicalAttackDamage(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier magicModifier,
            ref RulesetEffect activeEffect,
            ref List<EffectForm> actualEffectForms,
            ref bool firstTarget,
            bool isPrefix);

        public delegate void HandleCharacterMoveEnd(
            ref GameLocationCharacter mover,
            bool isPrefix);

        public delegate void HandleCharacterMoveStart(
            ref GameLocationCharacter mover,
            ref TA.int3 destination,
            bool isPrefix);

        public delegate void HandleCharacterSurprise(
            ref GameLocationCharacter character,
            bool isPrefix);

        public delegate void HandleFailedSavingThrowAgainstEffect(
            ref CharacterActionMagicEffect action,
            ref GameLocationCharacter caster,
            ref GameLocationCharacter defender,
            ref RulesetEffect rulesetEffect,
            ref ActionModifier saveModifier,
            ref bool hasHitVisual,
            bool isPrefix);

        public delegate void HandleRangeAttack(
            ref GameLocationCharacter launcher,
            ref GameLocationCharacter target,
            ref RulesetAttackMode attackMode,
            ref Vector3 sourcePoint,
            ref Vector3 impactPoint,
            ref float projectileFlightDuration,
            bool isPrefix);

        public delegate void HandleReactionToDamageShare(
            ref GameLocationCharacter damagedCharacter,
            ref int damageAmount,
            bool isPrefix);

        public delegate void HandleReactionToRageStart(
            ref GameLocationCharacter rager,
            bool isPrefix);

        public delegate void HandleSpellCast(
            ref GameLocationCharacter caster,
            ref CharacterActionCastSpell castAction,
            ref RulesetSpellRepertoire selectedRepertoire,
            ref SpellDefinition selectedSpellDefinition,
            bool isPrefix);

        public delegate void HandleSpellTargeted(
            ref GameLocationCharacter caster,
            ref RulesetEffectSpell hostileSpell,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            bool isPrefix);

        public static CharacterActionExecute CharacterActionExecuteHandler { get; set; }
        public static HandleCharacterAttack HandleCharacterAttackHandler { get; set; }
        public static HandleCharacterAttackDamage HandleCharacterAttackDamageHandler { get; set; }
        public static HandleCharacterAttackFinished HandleCharacterAttackFinishedHandler { get; set; }
        public static HandleCharacterAttackHit HandleCharacterAttackHitHandler { get; set; }
        public static HandleCharacterMagicalAttackDamage HandleCharacterMagicalAttackDamageHandler { get; set; }
        public static HandleCharacterMoveEnd HandleCharacterMoveEndHandler { get; set; }
        public static HandleCharacterMoveStart HandleCharacterMoveStartHandler { get; set; }
        public static HandleCharacterSurprise HandleCharacterSurpriseHandler { get; set; }
        public static HandleFailedSavingThrowAgainstEffect HandleFailedSavingThrowAgainstEffectHandler { get; set; }
        public static HandleRangeAttack HandleRangeAttackHandler { get; set; }
        public static HandleReactionToDamageShare HandleReactionToDamageShareHandler { get; set; }
        public static HandleReactionToRageStart HandleReactionToRageStartHandler { get; set; }
        public static HandleSpellCast HandleSpellCastHandler { get; set; }
        public static HandleSpellTargeted HandleSpellTargetedHandler { get; set; }

    }
}
