using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using TA.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class EffectFormBuilder
    {
        private readonly EffectForm effectForm;

        public EffectFormBuilder()
        {
            effectForm = new EffectForm();
        }

        public EffectFormBuilder HasSavingThrow(RuleDefinitions.EffectSavingThrowType savingThrowAffinity)
        {
            effectForm.HasSavingThrow = true;
            effectForm.SavingThrowAffinity = savingThrowAffinity;
            return this;
        }

        public EffectFormBuilder CanSaveToCancel(RuleDefinitions.TurnOccurenceType saveOccurence)
        {
            effectForm.CanSaveToCancel = true;
            effectForm.SaveOccurence = saveOccurence;
            return this;
        }

        public EffectFormBuilder CreatedByCharacter()
        {
            effectForm.SetCreatedByCharacter(true);
            return this;
        }

        public EffectFormBuilder CreatedByCondition()
        {
            effectForm.SetCreatedByCondition(true);
            return this;
        }

        [Obsolete("Use SetBonusMode")]
        public EffectFormBuilder ApplyAbilityBonus()
        {
            this.SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);
            return this;
        }

        public EffectFormBuilder SetBonusMode(RuleDefinitions.AddBonusMode bonusMode)
        {
            effectForm.AddBonusMode = bonusMode;
            return this;
        }

        public EffectFormBuilder SetLevelAdvancement(EffectForm.LevelApplianceType applyLevel,
            RuleDefinitions.LevelSourceType levelType, int levelMultiplier)
        {
            effectForm.SetApplyLevel(applyLevel);
            effectForm.SetLevelType(levelType);
            effectForm.SetLevelMultiplier(levelMultiplier);
            return this;
        }

        public EffectFormBuilder SetAlterationForm(AlterationForm.Type alterationType)
        {
            effectForm.FormType = EffectForm.EffectFormType.Alteration;
            AlterationForm alterationForm = new AlterationForm();
            alterationForm.SetAlterationType(alterationType);
            effectForm.SetAlterationForm(alterationForm);
            return this;
        }

        public EffectFormBuilder SetAlterationAbilityScore(string abilityScore, int valueIncrease, int maximumIncrease)
        {
            effectForm.FormType = EffectForm.EffectFormType.Alteration;
            AlterationForm alterationForm = new AlterationForm();
            alterationForm.SetAlterationType(AlterationForm.Type.AbilityScoreIncrease);
            alterationForm.SetAbilityScore(abilityScore);
            alterationForm.SetValueIncrease(valueIncrease);
            alterationForm.SetMaximumIncrease(maximumIncrease);
            effectForm.SetAlterationForm(alterationForm);
            return this;
        }

        public EffectFormBuilder SetConditionForm(ConditionDefinition condition, ConditionForm.ConditionOperation operation, bool applyToSelf, bool forceOnSelf, IEnumerable<ConditionDefinition> detrimentalConditions)
        {
            effectForm.FormType = EffectForm.EffectFormType.Condition;
            ConditionForm conditionForm = new ConditionForm
            {
                Operation = operation,
                ConditionDefinition = condition
            };
            conditionForm.SetConditionDefinitionName(condition.Name);
            conditionForm.SetApplyToSelf(applyToSelf);
            conditionForm.SetForceOnSelf(forceOnSelf);
            conditionForm.ConditionsList.SetRange(detrimentalConditions.ToList());
            effectForm.ConditionForm = conditionForm;
            return this;
        }

        public EffectFormBuilder SetCounterForm(CounterForm.CounterType type, int automaticSpellLevel, int checkBaseDC, bool addSpellCastingAbility, bool addProficiencyBonus)
        {
            effectForm.FormType = EffectForm.EffectFormType.Counter;
            CounterForm counterForm = new CounterForm();
            counterForm.SetType(type);
            counterForm.SetAutomaticSpellLevel(automaticSpellLevel);
            counterForm.SetCheckBaseDC(checkBaseDC);
            counterForm.SetAddSpellCastingAbility(addSpellCastingAbility);
            counterForm.SetAddProficiencyBonus(addProficiencyBonus);
            effectForm.SetCounterForm(counterForm);
            return this;
        }

        public EffectFormBuilder SetDamageForm(bool versatile, RuleDefinitions.DieType versatileDieType, string damageType, int bonusDamage,
            RuleDefinitions.DieType dieType, int diceNumber, RuleDefinitions.HealFromInflictedDamage healFromInflictedDamage,
            params RuleDefinitions.TrendInfo[] damageBonusTrends)
        {
            return SetDamageForm(versatile, versatileDieType, damageType, bonusDamage, dieType,
                diceNumber, healFromInflictedDamage, damageBonusTrends.AsEnumerable());
        }

        public EffectFormBuilder SetDamageForm(bool versatile, RuleDefinitions.DieType versatileDieType, string damageType, int bonusDamage,
            RuleDefinitions.DieType dieType, int diceNumber, RuleDefinitions.HealFromInflictedDamage healFromInflictedDamage,
            IEnumerable<RuleDefinitions.TrendInfo> damageBonusTrends)
        {
            effectForm.FormType = EffectForm.EffectFormType.Damage;
            DamageForm damageForm = new DamageForm();
            damageForm.SetVersatile(versatile);
            damageForm.VersatileDieType = versatileDieType;
            damageForm.BonusDamage = bonusDamage;
            damageForm.DamageType = damageType;
            damageForm.DiceNumber = diceNumber;
            damageForm.DieType = dieType;
            damageForm.SetHealFromInflictedDamage(healFromInflictedDamage);
            damageForm.SetField("damageBonusTrends", damageBonusTrends.ToList());
            effectForm.DamageForm = damageForm;
            return this;
        }

        public EffectFormBuilder SetDivinationForm(DivinationForm.Type divinationType, IEnumerable<CharacterFamilyDefinition> creatureFamilies,
            IEnumerable<string> revealedTags, int rangeCells)
        {
            effectForm.FormType = EffectForm.EffectFormType.Divination;
            DivinationForm divinationForm = new DivinationForm();
            divinationForm.SetDivinationType(divinationType);
            divinationForm.SetField("creatureFamilies", creatureFamilies.ToList());
            divinationForm.SetField("revealedTags", revealedTags.ToList());
            divinationForm.SetRangeCells(rangeCells);
            effectForm.SetDivinationForm(divinationForm);
            return this;
        }

        public EffectFormBuilder SetHealingForm(RuleDefinitions.HealingComputation healingComputation,
            int bonusHitPoints, RuleDefinitions.DieType dieType, int diceNumber, bool variablePool,
            RuleDefinitions.HealingCap healingCap)
        {
            effectForm.FormType = EffectForm.EffectFormType.Healing;

            HealingForm healingForm = new HealingForm
            {
                HealingComputation = healingComputation,
                BonusHealing = bonusHitPoints,
                DieType = dieType,
                DiceNumber = diceNumber,
                VariablePool = variablePool,
                HealingCap = healingCap
            };
            effectForm.SetHealingForm(healingForm);
            return this;
        }

        public EffectFormBuilder SetItemPropertyForm(IEnumerable<FeatureUnlockByLevel> featureBySlotLevel, RuleDefinitions.ItemPropertyUsage usageLimitation, int useAmount)
        {
            effectForm.FormType = EffectForm.EffectFormType.ItemProperty;
            ItemPropertyForm itemForm = new ItemPropertyForm();
            itemForm.SetUsageLimitation(usageLimitation);
            itemForm.SetUseAmount(useAmount);
            itemForm.SetField("featureBySlotLevel", featureBySlotLevel);
            effectForm.SetItemPropertyForm(itemForm);
            return this;
        }

        public EffectFormBuilder SetLightSourceForm(RuleDefinitions.LightSourceType lightSourceType, int brightRange, int dimAdditionalRange,
            Color color, AssetReference graphicsPrefabReference)
        {
            effectForm.FormType = EffectForm.EffectFormType.LightSource;
            LightSourceForm lightSourceForm = new LightSourceForm();
            lightSourceForm.SetLightSourceType(lightSourceType);
            lightSourceForm.SetBrightRange(brightRange);
            lightSourceForm.SetDimAdditionalRange(dimAdditionalRange);
            lightSourceForm.SetColor(color);
            lightSourceForm.SetGraphicsPrefabReference(graphicsPrefabReference);
            effectForm.SetLightSourceForm(lightSourceForm);
            return this;
        }

        public EffectFormBuilder SetMotionForm(MotionForm.MotionType motionType, int motionDistance)
        {
            effectForm.FormType = EffectForm.EffectFormType.Motion;
            MotionForm motionForm = new MotionForm();
            motionForm.SetType(motionType);
            motionForm.SetDistance(motionDistance);
            effectForm.SetMotionForm(motionForm);
            return this;
        }

        public EffectFormBuilder SetReviveForm(int secondsSinceDeath, RuleDefinitions.ReviveHitPoints reviveHitPoints,
            params ConditionDefinition[] removedConditions)
        {
            return SetReviveForm(secondsSinceDeath, reviveHitPoints, removedConditions.AsEnumerable());
        }

        public EffectFormBuilder SetReviveForm(int secondsSinceDeath, RuleDefinitions.ReviveHitPoints reviveHitPoints,
            IEnumerable<ConditionDefinition> removedConditions)
        {
            effectForm.FormType = EffectForm.EffectFormType.Revive;
            ReviveForm reviveForm = new ReviveForm();
            reviveForm.SetMaxSecondsSinceDeath(secondsSinceDeath);
            reviveForm.SetReviveHitPoints(reviveHitPoints);
            reviveForm.SetField("removedConditions", removedConditions.ToList());

            effectForm.SetReviveForm(reviveForm);
            return this;
        }

        public EffectFormBuilder SetSpellForm(int maxSlotLevel)
        {
            effectForm.FormType = EffectForm.EffectFormType.SpellSlots;
            SpellSlotsForm spellSlotsForm = new SpellSlotsForm();
            spellSlotsForm.SetType(SpellSlotsForm.EffectType.RecoverHalfLevelUp);
            spellSlotsForm.SetMaxSlotLevel(maxSlotLevel);
            effectForm.SetSpellSlotsForm(spellSlotsForm);
            return this;
        }

        public EffectFormBuilder SetSummonForm(SummonForm.Type summonType, ItemDefinition item, int number, string monsterDefinitionName, ConditionDefinition conditionDefinition,
            bool persistOnConcentrationLoss, DecisionPackageDefinition decisionPackage, EffectProxyDefinition effectProxyDefinition)
        {
            effectForm.FormType = EffectForm.EffectFormType.Summon;
            SummonForm summonForm = new SummonForm();
            summonForm.SetSummonType(summonType);
            summonForm.SetItemDefinition(item);
            summonForm.SetNumber(number);
            summonForm.SetMonsterDefinitionName(monsterDefinitionName);
            summonForm.SetConditionDefinition(conditionDefinition);
            summonForm.SetPersistOnConcentrationLoss(persistOnConcentrationLoss);
            summonForm.SetDecisionPackage(decisionPackage);
            summonForm.SetEffectProxyDefinitionName(effectProxyDefinition.Name);
            effectForm.SetSummonForm(summonForm);
            return this;
        }

        public EffectFormBuilder SetTempHPForm(int bonusHitPoints, RuleDefinitions.DieType dieType, int diceNumber)
        {
            effectForm.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
            TemporaryHitPointsForm tempHPForm = new TemporaryHitPointsForm
            {
                BonusHitPoints = bonusHitPoints,
                DieType = dieType,
                DiceNumber = diceNumber
            };
            effectForm.SetTemporaryHitPointsForm(tempHPForm);
            return this;
        }

        public EffectFormBuilder SetTopologyForm(TopologyForm.Type changeType, bool impactsFlyingCharacters)
        {
            effectForm.FormType = EffectForm.EffectFormType.Topology;
            TopologyForm topologyForm = new TopologyForm();
            topologyForm.SetChangeType(changeType);
            topologyForm.SetImpactsFlyingCharacters(impactsFlyingCharacters);
            effectForm.SetTopologyForm(topologyForm);
            return this;
        }

        public EffectForm Build()
        {
            return effectForm;
        }
    }
}
