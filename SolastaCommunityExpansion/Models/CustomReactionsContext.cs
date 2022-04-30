using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    public static class CustomReactionsContext
    {
        private static IDamagedReactionSpell _alwayseact;
        public static IDamagedReactionSpell AlwaysReactToDamaged => _alwayseact ??= new AlwaysReactToDamagedImpl();

        public interface IDamagedReactionSpell
        {
            bool CanReact(GameLocationCharacter attacker, GameLocationCharacter defender, ActionModifier attackModifier,
                RulesetAttackMode attackMode, bool rangedAttack, RuleDefinitions.AdvantageType advantageType,
                List<EffectForm> actualEffectForms, RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget);
        }

        private class AlwaysReactToDamagedImpl : IDamagedReactionSpell
        {
            public bool CanReact(GameLocationCharacter attacker, GameLocationCharacter defender,
                ActionModifier attackModifier,
                RulesetAttackMode attackMode, bool rangedAttack, RuleDefinitions.AdvantageType advantageType,
                List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
            {
                return true;
            }
        }

        public static IEnumerator TryReactingToDamageWithSpell(GameLocationCharacter attacker,
            GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
            bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
        {
            var ruleCaster = attacker.RulesetCharacter;

            // Wildshape heroes shouldn't be able to cast react spells
            if (ruleCaster.IsSubstitute)
            {
                yield break;
            }

            var ruleDefender = defender.RulesetCharacter;

            if (ruleDefender == null)
            {
                yield break;
            }

            ruleDefender.InvokeMethod("EnumerateUsableSpells");

            var spells = ruleDefender.UsableSpells
                .OfType<SpellWithCustomFeatures>()
                .Where(s => s.GetTypedFeatures<IDamagedReactionSpell>().Any())
                .ToList();

            foreach (var spell in spells)
            {
                var reactions = spell.GetTypedFeatures<IDamagedReactionSpell>();

                if (reactions.Any(r => r.CanReact(attacker, defender, attackModifier, attackMode, rangedAttack,
                        advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget)))
                {
                    yield return ReactWithSpell(spell, defender, attacker);
                }
            }
        }

        public static IEnumerator ReactWithSpell(SpellDefinition spell, GameLocationCharacter caster,
            GameLocationCharacter target)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager != null)
            {
                var ruleCaster = caster.RulesetCharacter;
                var spellSlot = ruleCaster.GetLowestSlotLevelAndRepertoireToCastSpell(spell, out var spellBook);

                if (spellBook != null)
                {
                    var ruleset = ServiceRepository.GetService<IRulesetImplementationService>();
                    var reactionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastReaction)
                    {
                        IntParameter = 0,
                        RulesetEffect =
                            ruleset.InstantiateEffectSpell(ruleCaster, spellBook, spell, spellSlot, false),
                        IsReactionEffect = true
                    };

                    reactionParams.TargetCharacters.Add(target);
                    reactionParams.ActionModifiers.Add(new ActionModifier());

                    var reactions = actionManager.PendingReactionRequestGroups.Count;
                    var reaction = new ReactionRequestCastDamageSpell(reactionParams, target, spellSlot == 0);

                    actionManager.InvokeMethod("AddInterruptRequest", reaction);

                    yield return WaitForReactions(actionManager, reactions);
                }
            }
        }

        private static IEnumerator WaitForReactions(IGameLocationActionService actionService, int previousReactionCount)
        {
            while (actionService?.PendingReactionRequestGroups != null &&
                   previousReactionCount < actionService.PendingReactionRequestGroups.Count)
            {
                yield return null;
            }
        }
    }
}
