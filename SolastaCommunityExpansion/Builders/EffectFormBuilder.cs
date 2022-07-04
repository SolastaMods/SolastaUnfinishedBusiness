using System.Collections.Generic;
using System.Linq;
using TA.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Builders;

public class EffectFormBuilder
{
    private readonly EffectForm effectForm;

    public EffectFormBuilder()
    {
        effectForm = new EffectForm();
    }

    public EffectFormBuilder(EffectForm reference)
    {
        effectForm = new EffectForm();

        effectForm.Copy(reference);
    }

    public EffectFormBuilder HasSavingThrow(EffectSavingThrowType savingThrowAffinity)
    {
        effectForm.HasSavingThrow = true;
        effectForm.SavingThrowAffinity = savingThrowAffinity;
        return this;
    }

    public EffectFormBuilder CanSaveToCancel(TurnOccurenceType saveOccurence)
    {
        effectForm.CanSaveToCancel = true;
        effectForm.SaveOccurence = saveOccurence;
        return this;
    }

    public EffectFormBuilder CreatedByCharacter()
    {
        effectForm.createdByCharacter = true;
        return this;
    }

    public EffectFormBuilder CreatedByCondition()
    {
        effectForm.createdByCondition = true;
        return this;
    }

    public EffectFormBuilder SetBonusMode(AddBonusMode bonusMode)
    {
        effectForm.AddBonusMode = bonusMode;
        return this;
    }

    public EffectFormBuilder SetLevelAdvancement(EffectForm.LevelApplianceType applyLevel,
        LevelSourceType levelType, int levelMultiplier = 1)
    {
        effectForm.applyLevel = applyLevel;
        effectForm.levelType = levelType;
        effectForm.levelMultiplier = levelMultiplier;
        return this;
    }

    public EffectFormBuilder SetAlterationForm(AlterationForm.Type alterationType)
    {
        var alterationForm = new AlterationForm {alterationType = alterationType};

        effectForm.alterationForm = alterationForm;
        effectForm.FormType = EffectForm.EffectFormType.Alteration;
        return this;
    }

    public EffectFormBuilder SetAlterationAbilityScore(string abilityScore, int valueIncrease, int maximumIncrease)
    {
        var alterationForm = new AlterationForm
        {
            alterationType = AlterationForm.Type.AbilityScoreIncrease,
            abilityScore = abilityScore,
            valueIncrease = valueIncrease,
            maximumIncrease = maximumIncrease
        };

        effectForm.alterationForm = alterationForm;
        effectForm.FormType = EffectForm.EffectFormType.Alteration;
        return this;
    }

    public EffectFormBuilder SetConditionForm(ConditionDefinition condition,
        ConditionForm.ConditionOperation operation)
    {
        return SetConditionForm(condition, operation, false, false, condition);
    }

    public EffectFormBuilder SetConditionForm(ConditionDefinition condition,
        ConditionForm.ConditionOperation operation, bool applyToSelf, bool forceOnSelf,
        params ConditionDefinition[] detrimentalConditions)
    {
        return SetConditionForm(condition, operation, applyToSelf, forceOnSelf,
            detrimentalConditions.AsEnumerable());
    }

    public EffectFormBuilder SetConditionForm(ConditionDefinition condition,
        ConditionForm.ConditionOperation operation, bool applyToSelf, bool forceOnSelf,
        IEnumerable<ConditionDefinition> detrimentalConditions)
    {
        effectForm.FormType = EffectForm.EffectFormType.Condition;
        var conditionForm = new ConditionForm
        {
            Operation = operation,
            ConditionDefinition = condition,
            applyToSelf = applyToSelf,
            forceOnSelf = forceOnSelf,
            conditionsList = detrimentalConditions.ToList()
        };

        if (condition != null)
        {
            conditionForm.conditionDefinitionName = condition.Name;
        }

        effectForm.ConditionForm = conditionForm;
        return this;
    }

    public EffectFormBuilder SetCounterForm(CounterForm.CounterType type, int automaticSpellLevel, int checkBaseDC,
        bool addSpellCastingAbility, bool addProficiencyBonus)
    {
        var counterForm = new CounterForm
        {
            type = type,
            automaticSpellLevel = automaticSpellLevel,
            checkBaseDC = checkBaseDC,
            addSpellCastingAbility = addSpellCastingAbility,
            addProficiencyBonus = addProficiencyBonus
        };
        effectForm.counterForm = counterForm;
        effectForm.FormType = EffectForm.EffectFormType.Counter;
        return this;
    }

    public EffectFormBuilder SetDamageForm(bool versatile = false, DieType versatileDieType = DieType.D1,
        string damageType = DamageTypeBludgeoning, int bonusDamage = 0,
        DieType dieType = DieType.D1, int diceNumber = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        params TrendInfo[] damageBonusTrends)
    {
        return SetDamageForm(versatile, versatileDieType, damageType, bonusDamage, dieType,
            diceNumber, healFromInflictedDamage, damageBonusTrends.AsEnumerable());
    }

    public EffectFormBuilder SetDamageForm(bool versatile, DieType versatileDieType, string damageType,
        int bonusDamage,
        DieType dieType, int diceNumber, HealFromInflictedDamage healFromInflictedDamage,
        IEnumerable<TrendInfo> damageBonusTrends)
    {
        var damageForm = new DamageForm
        {
            versatile = versatile,
            VersatileDieType = versatileDieType,
            BonusDamage = bonusDamage,
            DamageType = damageType,
            DiceNumber = diceNumber,
            DieType = dieType,
            healFromInflictedDamage = healFromInflictedDamage,
            damageBonusTrends = damageBonusTrends.ToList()
        };

        effectForm.FormType = EffectForm.EffectFormType.Damage;
        effectForm.DamageForm = damageForm;
        return this;
    }

    public EffectFormBuilder SetKillForm(KillCondition condition, float challengeRating = 0, int hitPoints = 0)
    {
        var killForm = new KillForm
        {
            killCondition = condition, challengeRating = challengeRating, hitPoints = hitPoints
        };

        effectForm.killForm = killForm;
        effectForm.FormType = EffectForm.EffectFormType.Kill;
        return this;
    }

    public EffectFormBuilder SetDivinationForm(DivinationForm.Type divinationType,
        IEnumerable<CharacterFamilyDefinition> creatureFamilies,
        IEnumerable<string> revealedTags, int rangeCells)
    {
        var divinationForm = new DivinationForm
        {
            divinationType = divinationType,
            creatureFamilies = creatureFamilies.ToList(),
            revealedTags = revealedTags.ToList(),
            rangeCells = rangeCells
        };

        effectForm.divinationForm = divinationForm;
        effectForm.FormType = EffectForm.EffectFormType.Divination;
        return this;
    }

    public EffectFormBuilder SetHealingForm(HealingComputation healingComputation,
        int bonusHitPoints, DieType dieType, int diceNumber, bool variablePool,
        HealingCap healingCap)
    {
        var healingForm = new HealingForm
        {
            HealingComputation = healingComputation,
            BonusHealing = bonusHitPoints,
            DieType = dieType,
            DiceNumber = diceNumber,
            VariablePool = variablePool,
            HealingCap = healingCap
        };

        effectForm.healingForm = healingForm;
        effectForm.FormType = EffectForm.EffectFormType.Healing;
        return this;
    }

    public EffectFormBuilder SetItemPropertyForm(ItemPropertyUsage usageLimitation, int useAmount,
        params FeatureUnlockByLevel[] featureBySlotLevel)
    {
        return SetItemPropertyForm(featureBySlotLevel.AsEnumerable(), usageLimitation, useAmount);
    }

    public EffectFormBuilder SetItemPropertyForm(IEnumerable<FeatureUnlockByLevel> featureBySlotLevel,
        ItemPropertyUsage usageLimitation, int useAmount)
    {
        var itemForm = new ItemPropertyForm
        {
            usageLimitation = usageLimitation,
            useAmount = useAmount,
            featureBySlotLevel = featureBySlotLevel.ToList()
        };

        effectForm.itemPropertyForm = itemForm;
        effectForm.FormType = EffectForm.EffectFormType.ItemProperty;
        return this;
    }

    public EffectFormBuilder SetLightSourceForm(LightSourceType lightSourceType, int brightRange,
        int dimAdditionalRange,
        Color color, AssetReference graphicsPrefabReference)
    {
        var lightSourceForm = new LightSourceForm
        {
            lightSourceType = lightSourceType,
            brightRange = brightRange,
            dimAdditionalRange = dimAdditionalRange,
            color = color,
            graphicsPrefabReference = graphicsPrefabReference
        };

        effectForm.lightSourceForm = lightSourceForm;
        effectForm.FormType = EffectForm.EffectFormType.LightSource;
        return this;
    }

    public EffectFormBuilder SetMotionForm(MotionForm.MotionType motionType, int motionDistance)
    {
        var motionForm = new MotionForm {type = motionType, distance = motionDistance};

        effectForm.motionForm = motionForm;
        effectForm.FormType = EffectForm.EffectFormType.Motion;
        return this;
    }

    public EffectFormBuilder SetReviveForm(int secondsSinceDeath, ReviveHitPoints reviveHitPoints,
        params ConditionDefinition[] removedConditions)
    {
        return SetReviveForm(secondsSinceDeath, reviveHitPoints, removedConditions.AsEnumerable());
    }

    public EffectFormBuilder SetReviveForm(int secondsSinceDeath, ReviveHitPoints reviveHitPoints,
        IEnumerable<ConditionDefinition> removedConditions)
    {
        var reviveForm = new ReviveForm
        {
            maxSecondsSinceDeath = secondsSinceDeath,
            reviveHitPoints = reviveHitPoints,
            removedConditions = removedConditions.ToList()
        };

        effectForm.reviveForm = reviveForm;
        effectForm.FormType = EffectForm.EffectFormType.Revive;
        return this;
    }

    public EffectFormBuilder SetSpellForm(int maxSlotLevel)
    {
        var spellSlotsForm = new SpellSlotsForm
        {
            type = SpellSlotsForm.EffectType.RecoverHalfLevelUp, maxSlotLevel = maxSlotLevel
        };

        effectForm.spellSlotsForm = spellSlotsForm;
        effectForm.FormType = EffectForm.EffectFormType.SpellSlots;
        return this;
    }

    public EffectFormBuilder SetSummonForm(SummonForm.Type summonType, ItemDefinition item, int number,
        string monsterDefinitionName, ConditionDefinition conditionDefinition,
        bool persistOnConcentrationLoss, DecisionPackageDefinition decisionPackage,
        EffectProxyDefinition effectProxyDefinition)
    {
        var summonForm = new SummonForm
        {
            summonType = summonType,
            itemDefinition = item,
            number = number,
            monsterDefinitionName = monsterDefinitionName,
            conditionDefinition = conditionDefinition,
            persistOnConcentrationLoss = persistOnConcentrationLoss,
            decisionPackage = decisionPackage,
            effectProxyDefinitionName = effectProxyDefinition.Name
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    public EffectFormBuilder SetSummonCreatureForm(int number, string monsterDefinitionName,
        bool persistOnConcentrationLoss = false, ConditionDefinition condition = null,
        DecisionPackageDefinition decisionPackage = null)
    {
        var summonForm = new SummonForm
        {
            summonType = SummonForm.Type.Creature,
            itemDefinition = null,
            number = number,
            monsterDefinitionName = monsterDefinitionName,
            conditionDefinition = condition,
            persistOnConcentrationLoss = persistOnConcentrationLoss,
            decisionPackage = decisionPackage,
            effectProxyDefinitionName = null
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    public EffectFormBuilder SetSummonItemForm(ItemDefinition item, int number)
    {
        var summonForm = new SummonForm
        {
            summonType = SummonForm.Type.InventoryItem,
            itemDefinition = item,
            number = number,
            monsterDefinitionName = "", //do we even need this?
            conditionDefinition = null,
            persistOnConcentrationLoss = true,
            decisionPackage = null,
            effectProxyDefinitionName = null
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    public EffectFormBuilder SetTempHPForm(int bonusHitPoints, DieType dieType, int diceNumber)
    {
        var tempHpForm = new TemporaryHitPointsForm
        {
            BonusHitPoints = bonusHitPoints, DieType = dieType, DiceNumber = diceNumber
        };
        effectForm.temporaryHitPointsForm = tempHpForm;
        effectForm.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
        return this;
    }

    public EffectFormBuilder SetTopologyForm(TopologyForm.Type changeType, bool impactsFlyingCharacters)
    {
        var topologyForm =
            new TopologyForm {changeType = changeType, impactsFlyingCharacters = impactsFlyingCharacters};

        effectForm.topologyForm = topologyForm;
        effectForm.FormType = EffectForm.EffectFormType.Topology;
        return this;
    }

    public EffectForm Build()
    {
        return effectForm;
    }

    public static EffectFormBuilder Create() { return new EffectFormBuilder(); }
}
