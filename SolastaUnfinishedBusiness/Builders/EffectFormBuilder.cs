using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class EffectFormBuilder
{
    private readonly EffectForm effectForm;

    private EffectFormBuilder()
    {
        effectForm = new EffectForm();
    }

    internal EffectForm Build()
    {
        return effectForm;
    }

    internal static EffectFormBuilder Create()
    {
        return new EffectFormBuilder();
    }

    internal EffectFormBuilder HasSavingThrow(
        EffectSavingThrowType savingThrowAffinity,
        TurnOccurenceType saveOccurence = TurnOccurenceType.EndOfTurn,
        bool canSaveToCancel = false)
    {
        effectForm.HasSavingThrow = true;
        effectForm.SavingThrowAffinity = savingThrowAffinity;
        effectForm.saveOccurence = saveOccurence;
        effectForm.canSaveToCancel = canSaveToCancel;
        return this;
    }

    internal EffectFormBuilder SetBonusMode(AddBonusMode bonusMode)
    {
        effectForm.AddBonusMode = bonusMode;
        return this;
    }

    internal EffectFormBuilder SetLevelAdvancement(
        EffectForm.LevelApplianceType applyLevel,
        LevelSourceType levelType,
        int levelMultiplier = 1)
    {
        effectForm.applyLevel = applyLevel;
        effectForm.levelType = levelType;
        effectForm.levelMultiplier = levelMultiplier;
        return this;
    }

    internal EffectFormBuilder SetDiceAdvancement(
        LevelSourceType levelType,
        int start = 0,
        int increment = 1,
        int step = 1,
        int begin = 1)
    {
        effectForm.levelType = levelType;
        effectForm.applyLevel = EffectForm.LevelApplianceType.DiceNumberByLevelTable;
        effectForm.DiceByLevelTable.SetRange(DiceByRankBuilder.BuildDiceByRankTable(start, increment, step, begin));
        return this;
    }

    internal EffectFormBuilder SetAlterationForm(AlterationForm.Type alterationType)
    {
        var alterationForm = new AlterationForm { alterationType = alterationType };

        effectForm.alterationForm = alterationForm;
        effectForm.FormType = EffectForm.EffectFormType.Alteration;
        return this;
    }

#if false
    internal EffectFormBuilder SetAlterationAbilityScore(string abilityScore, int valueIncrease, int maximumIncrease)
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
#endif

    internal EffectFormBuilder SetConditionForm(
        ConditionDefinition condition,
        ConditionForm.ConditionOperation operation,
        bool applyToSelf = false,
        bool forceOnSelf = false,
        params ConditionDefinition[] conditionsList)
    {
        effectForm.FormType = EffectForm.EffectFormType.Condition;

        var conditionForm = new ConditionForm
        {
            Operation = operation,
            ConditionDefinition = condition,
            applyToSelf = applyToSelf,
            forceOnSelf = forceOnSelf,
            conditionsList = conditionsList.ToList()
        };

        if (condition != null)
        {
            conditionForm.conditionDefinitionName = condition.Name;
        }

        effectForm.ConditionForm = conditionForm;
        return this;
    }

    internal static EffectForm ConditionForm(
        ConditionDefinition condition,
        ConditionForm.ConditionOperation operation = global::ConditionForm.ConditionOperation.Add,
        bool applyToSelf = false,
        bool forceOnSelf = false,
        params ConditionDefinition[] conditionsList)
    {
        return Create()
            .SetConditionForm(condition, operation, applyToSelf, forceOnSelf, conditionsList)
            .Build();
    }

    internal EffectFormBuilder OverrideSavingThrowInfo(
        string savingThrowAbility,
        int savingThrowDc,
        string sourceDefinitionName = "",
        FeatureSourceType featureSourceType = FeatureSourceType.Base)
    {
        effectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(savingThrowAbility, savingThrowDc,
            sourceDefinitionName, featureSourceType);
        return this;
    }

    internal EffectFormBuilder SetCounterForm(
        CounterForm.CounterType type,
        int automaticSpellLevel,
        int checkBaseDc,
        bool addSpellCastingAbility,
        bool addProficiencyBonus)
    {
        var counterForm = new CounterForm
        {
            type = type,
            automaticSpellLevel = automaticSpellLevel,
            checkBaseDC = checkBaseDc,
            addSpellCastingAbility = addSpellCastingAbility,
            addProficiencyBonus = addProficiencyBonus
        };
        effectForm.counterForm = counterForm;
        effectForm.FormType = EffectForm.EffectFormType.Counter;
        return this;
    }

    internal static EffectForm DamageForm(string damageType = DamageTypeBludgeoning,
        int diceNumber = 0,
        DieType dieType = DieType.D1,
        int bonusDamage = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        bool overrideWithBardicInspirationDie = false)
    {
        return CreateDamageForm(
            damageType,
            diceNumber,
            dieType,
            bonusDamage,
            healFromInflictedDamage,
            overrideWithBardicInspirationDie
        ).Build();
    }

    internal static EffectFormBuilder CreateDamageForm(string damageType = DamageTypeBludgeoning,
        int diceNumber = 0,
        DieType dieType = DieType.D1,
        int bonusDamage = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        bool overrideWithBardicInspirationDie = false)
    {
        return Create().SetDamageForm(
            damageType,
            diceNumber,
            dieType,
            bonusDamage,
            healFromInflictedDamage,
            overrideWithBardicInspirationDie
        );
    }


    internal EffectFormBuilder SetDamageForm(
        string damageType = DamageTypeBludgeoning,
        int diceNumber = 0,
        DieType dieType = DieType.D1,
        int bonusDamage = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        bool overrideWithBardicInspirationDie = false)
    {
        var damageForm = new DamageForm
        {
            versatile = false,
            VersatileDieType = DieType.D1,
            BonusDamage = bonusDamage,
            DamageType = damageType,
            DiceNumber = diceNumber,
            DieType = dieType,
            healFromInflictedDamage = healFromInflictedDamage,
            damageBonusTrends = new List<TrendInfo>(),
            OverrideWithBardicInspirationDie = overrideWithBardicInspirationDie
        };

        effectForm.damageForm = damageForm;
        effectForm.FormType = EffectForm.EffectFormType.Damage;
        return this;
    }

    internal EffectFormBuilder SetKillForm(KillCondition condition, float challengeRating = 0, int hitPoints = 0)
    {
        var killForm = new KillForm
        {
            killCondition = condition, challengeRating = challengeRating, hitPoints = hitPoints
        };

        effectForm.killForm = killForm;
        effectForm.FormType = EffectForm.EffectFormType.Kill;
        return this;
    }

#if false
    internal EffectFormBuilder SetDivinationForm(
        DivinationForm.Type divinationType,
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
#endif

    internal EffectFormBuilder SetHealingForm(
        HealingComputation healingComputation,
        int bonusHitPoints,
        DieType dieType,
        int diceNumber,
        bool variablePool,
        HealingCap healingCap,
        EffectForm.LevelApplianceType levelApplianceType = EffectForm.LevelApplianceType.No)
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

        effectForm.applyLevel = levelApplianceType;
        effectForm.healingForm = healingForm;
        effectForm.FormType = EffectForm.EffectFormType.Healing;
        return this;
    }

    internal EffectFormBuilder SetItemPropertyForm(
        ItemPropertyUsage usageLimitation,
        int useAmount,
        params FeatureUnlockByLevel[] featureBySlotLevel)
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

    internal static EffectForm LightSourceForm(LightSourceType lightSourceType,
        int brightRange,
        int dimAdditionalRange,
        Color color,
        AssetReference graphicsPrefabReference = null)
    {
        return Create()
            .SetLightSourceForm(lightSourceType, brightRange, dimAdditionalRange, color, graphicsPrefabReference)
            .Build();
    }

    internal EffectFormBuilder SetLightSourceForm(
        LightSourceType lightSourceType,
        int brightRange,
        int dimAdditionalRange,
        Color color,
        AssetReference graphicsPrefabReference)
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

    internal EffectFormBuilder SetMotionForm(MotionForm.MotionType motionType, int motionDistance = 0)
    {
        var motionForm = new MotionForm { type = motionType, distance = motionDistance };

        effectForm.motionForm = motionForm;
        effectForm.FormType = EffectForm.EffectFormType.Motion;
        return this;
    }

    internal EffectFormBuilder SetMotionForm(ExtraMotionType motionType, int motionDistance = 0)
    {
        return SetMotionForm((MotionForm.MotionType)motionType, motionDistance);
    }

    internal EffectFormBuilder SetReviveForm(
        int secondsSinceDeath,
        ReviveHitPoints reviveHitPoints,
        params ConditionDefinition[] removedConditions)
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

    internal EffectFormBuilder SetSpellForm(int maxSlotLevel)
    {
        var spellSlotsForm = new SpellSlotsForm
        {
            type = SpellSlotsForm.EffectType.RecoverHalfLevelUp, maxSlotLevel = maxSlotLevel
        };

        effectForm.spellSlotsForm = spellSlotsForm;
        effectForm.FormType = EffectForm.EffectFormType.SpellSlots;
        return this;
    }

    internal EffectFormBuilder SetShapeChangeForm(
        ShapeChangeForm.Type shapeChangeType,
        bool keepMentalAbilityScores,
        ConditionDefinition specialSubstituteCondition,
        List<ShapeOptionDescription> shapeOptions)
    {
        var shapeChangeForm = new ShapeChangeForm
        {
            shapeChangeType = shapeChangeType,
            keepMentalAbilityScores = keepMentalAbilityScores,
            specialSubstituteCondition = specialSubstituteCondition,
            shapeOptions = shapeOptions
        };

        effectForm.shapeChangeForm = shapeChangeForm;
        effectForm.FormType = EffectForm.EffectFormType.ShapeChange;

        return this;
    }

    internal EffectFormBuilder SetSummonCreatureForm(
        int number,
        string monsterDefinitionName)
    {
        var summonForm = new SummonForm
        {
            summonType = SummonForm.Type.Creature,
            itemDefinition = null,
            number = number,
            monsterDefinitionName = monsterDefinitionName,
            conditionDefinition = null,
            persistOnConcentrationLoss = false,
            decisionPackage = null,
            effectProxyDefinitionName = null
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    internal EffectFormBuilder SetSummonItemForm(ItemDefinition item, int number, bool trackItem = false)
    {
        var summonForm = new SummonForm
        {
            summonType = SummonForm.Type.InventoryItem,
            itemDefinition = item,
            number = number,
            trackItem = trackItem,
            monsterDefinitionName = "",
            conditionDefinition = null,
            persistOnConcentrationLoss = true,
            decisionPackage = null,
            effectProxyDefinitionName = null
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    internal EffectFormBuilder SetSummonEffectProxyForm(EffectProxyDefinition effectProxyDefinition)
    {
        var summonForm = new SummonForm
        {
            summonType = SummonForm.Type.EffectProxy,
            itemDefinition = null,
            number = 0,
            trackItem = false,
            monsterDefinitionName = "",
            conditionDefinition = null,
            persistOnConcentrationLoss = true,
            decisionPackage = null,
            effectProxyDefinitionName = effectProxyDefinition.Name
        };

        effectForm.summonForm = summonForm;
        effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    internal EffectFormBuilder SetTempHpForm(
        int bonusHitPoints = 0,
        DieType dieType = DieType.D1,
        int diceNumber = 0,
        bool applyToSelf = false)
    {
        var tempHpForm = new TemporaryHitPointsForm
        {
            BonusHitPoints = bonusHitPoints, DieType = dieType, DiceNumber = diceNumber, ApplyToSelf = applyToSelf
        };
        effectForm.temporaryHitPointsForm = tempHpForm;
        effectForm.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
        return this;
    }

    internal static EffectForm TopologyForm(TopologyForm.Type changeType, bool impactsFlyingCharacters)
    {
        return CreateTopologyForm(changeType, impactsFlyingCharacters).Build();
    }

    internal static EffectFormBuilder CreateTopologyForm(TopologyForm.Type changeType, bool impactsFlyingCharacters)
    {
        return Create().SetTopologyForm(changeType, impactsFlyingCharacters);
    }

    internal EffectFormBuilder SetTopologyForm(TopologyForm.Type changeType, bool impactsFlyingCharacters)
    {
        var topologyForm =
            new TopologyForm { changeType = changeType, impactsFlyingCharacters = impactsFlyingCharacters };

        effectForm.topologyForm = topologyForm;
        effectForm.FormType = EffectForm.EffectFormType.Topology;
        return this;
    }
}
