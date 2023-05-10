using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region HELPERS

    private static class CustomSpellEffectLevel
    {
        internal static readonly ICustomSpellEffectLevel ByCasterLevel = new SpellEffectLevelFromCasterLevel();
    }

    private sealed class SpellEffectLevelFromCasterLevel : ICustomSpellEffectLevel
    {
        public int GetEffectLevel([NotNull] RulesetActor caster, RulesetEffectSpell rulesetEffectSpell)
        {
            return caster.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
        }
    }

    private sealed class ChainSpellEffectOnAttackHit : IChainMagicEffect
    {
        private readonly string _notificationTag;
        private readonly SpellDefinition _spell;

        internal ChainSpellEffectOnAttackHit(SpellDefinition spell, [CanBeNull] string notificationTag = null)
        {
            _spell = spell;
            _notificationTag = notificationTag;
        }

        [CanBeNull]
        public CharacterActionMagicEffect GetNextMagicEffect(
            [CanBeNull] CharacterActionMagicEffect baseEffect,
            CharacterActionAttack triggeredAttack,
            RollOutcome attackOutcome)
        {
            if (baseEffect == null)
            {
                return null;
            }

            var spellEffect = baseEffect as CharacterActionCastSpell;
            var repertoire = spellEffect?.ActiveSpell.SpellRepertoire;
            var actionParams = baseEffect.actionParams;

            if (actionParams == null)
            {
                return null;
            }

            if (baseEffect.Countered || baseEffect.ExecutionFailed)
            {
                return null;
            }

            if (attackOutcome != RollOutcome.Success
                && attackOutcome != RollOutcome.CriticalSuccess)
            {
                return null;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;

            if (caster == null || targets.Count < 2)
            {
                return null;
            }

            var rulesetCaster = caster.RulesetCharacter;
            var rules = ServiceRepository.GetService<IRulesetImplementationService>();
            var bonusLevelProvider = _spell.GetFirstSubFeatureOfType<IBonusSlotLevels>();
            var slotLevel = _spell.SpellLevel;

            if (bonusLevelProvider != null)
            {
                slotLevel += bonusLevelProvider.GetBonusSlotLevels(rulesetCaster);
            }

            var effectSpell = rules.InstantiateEffectSpell(rulesetCaster, repertoire, _spell, slotLevel, false);

            for (var i = 1; i < targets.Count; i++)
            {
                var rulesetTarget = targets[i].RulesetCharacter;

                if (!string.IsNullOrEmpty(_notificationTag))
                {
                    GameConsoleHelper.LogCharacterAffectsTarget(rulesetCaster, rulesetTarget, _notificationTag, true);
                }

                effectSpell.ApplyEffectOnCharacter(rulesetTarget, true, targets[i].LocationPosition);
            }

            effectSpell.Terminate(true);

            return null;
        }
    }

    private sealed class BonusSlotLevelsByClassLevel : IBonusSlotLevels
    {
        public int GetBonusSlotLevels([NotNull] RulesetCharacter caster)
        {
            var level = caster.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            return SpellAdvancementByCasterLevel[level - 1];
        }
    }

    private sealed class UpgradeRangeBasedOnWeaponReach : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return effectDescription;
            }

            var weapon = hero.GetMainWeapon();

            if (weapon == null || !weapon.itemDefinition.IsWeapon)
            {
                return effectDescription;
            }

            var reach = weapon.itemDefinition.WeaponDescription.ReachRange;

            if (reach <= 1)
            {
                return effectDescription;
            }

            effectDescription.rangeParameter = reach;
            return effectDescription;
        }
    }

    private sealed class ConditionUsesPowerOnTarget : ICustomConditionFeature
    {
        private readonly FeatureDefinitionPower power;
        private readonly bool removeCondition;

        public ConditionUsesPowerOnTarget(FeatureDefinitionPower power, bool removeCondition = true)
        {
            this.power = power;
            this.removeCondition = removeCondition;
        }

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var defender = GameLocationCharacter.GetFromActor(target);
            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetAttacker == null || defender == null)
            {
                return;
            }

            var usablePower = UsablePowersProvider.Get(power, rulesetAttacker);
            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);

            effectPower.ApplyEffectOnCharacter(target, true, defender.LocationPosition);

            if (removeCondition)
            {
                target.RemoveCondition(rulesetCondition);
            }
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class OnAttackHitEffectBanishingSmite : IAttackEffectAfterDamage
    {
        private readonly ConditionDefinition _conditionDefinition;

        public OnAttackHitEffectBanishingSmite(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure ||
                rulesetAttacker == null ||
                rulesetDefender == null ||
                rulesetDefender.IsDeadOrDying ||
                rulesetDefender.CurrentHitPoints > 50)
            {
                return;
            }

            //TODO: ideally we need to banish extra planar creatures forever (kill them?)
            rulesetDefender.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyMagicEffectSkinOfRetribution : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character, RulesetEffect rulesetEffect)
        {
            var rulesetCondition =
                character.AllConditions.FirstOrDefault(x =>
                    x.EffectDefinitionName != null && x.EffectDefinitionName.Contains("SkinOfRetribution"));

            if (rulesetCondition == null || !effectDescription.HasDamageForm())
            {
                return effectDescription;
            }

            var effectLevel = rulesetCondition.EffectLevel;
            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.bonusDamage *= effectLevel;

            MaybeRemoveSkinOfRetribution(character);

            return effectDescription;
        }

        private static void MaybeRemoveSkinOfRetribution(RulesetCharacter target)
        {
            if (target.temporaryHitPoints > 0)
            {
                return;
            }

            foreach (var condition in target.AllConditions
                         .FindAll(x =>
                             x.EffectDefinitionName != null && x.EffectDefinitionName.Contains("SkinOfRetribution")))
            {
                target.RemoveCondition(condition);
            }
        }
    }

    private sealed class SanctuaryBeforeAttackHitPossible : IAttackHitPossible
    {
        private readonly ConditionDefinition _conditionSanctuaryBuff;

        internal SanctuaryBeforeAttackHitPossible(ConditionDefinition conditionSanctuaryBuff)
        {
            _conditionSanctuaryBuff = conditionSanctuaryBuff;
        }

        public IEnumerator DefenderAttackHitPossible(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            ActionModifier attackModifier,
            int attackRoll)
        {
            if (battle.Battle == null)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender == null)
            {
                yield break;
            }

            var modifierTrend = attacker.RulesetCharacter.actionModifier.savingThrowModifierTrends;
            var advantageTrends = attacker.RulesetCharacter.actionModifier.savingThrowAdvantageTrends;
            var attackerWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter
                .TryGetAttributeValue(AttributeDefinitions.Wisdom));
            var profBonus = AttributeDefinitions.ComputeProficiencyBonus(rulesetDefender
                .TryGetAttributeValue(AttributeDefinitions.CharacterLevel));
            var defenderWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(rulesetDefender
                .TryGetAttributeValue(AttributeDefinitions.Wisdom));

            attacker.RulesetCharacter.RollSavingThrow(0, AttributeDefinitions.Wisdom, null, modifierTrend,
                advantageTrends, attackerWisModifier, 8 + profBonus + defenderWisModifier, false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionSanctuaryBuff.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class SanctuaryBeforeHitConfirmed : IPhysicalAttackBeforeHitConfirmed
    {
        private readonly ConditionDefinition _conditionSanctuaryBuff;

        internal SanctuaryBeforeHitConfirmed(ConditionDefinition conditionSanctuaryBuff)
        {
            _conditionSanctuaryBuff = conditionSanctuaryBuff;
        }


        public IEnumerator OnAttackBeforeHitConfirmed(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            if (battle.Battle == null)
            {
                yield break;
            }

            if (criticalHit == false)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender?.InflictCondition(
                _conditionSanctuaryBuff.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    #endregion
}
