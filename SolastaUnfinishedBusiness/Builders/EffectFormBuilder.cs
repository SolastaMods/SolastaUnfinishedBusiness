using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class EffectFormBuilder
{
    private readonly EffectForm _effectForm;

    private EffectFormBuilder()
    {
        _effectForm = new EffectForm { createdByCharacter = true };
    }

    internal EffectForm Build()
    {
        return _effectForm;
    }

    internal static EffectFormBuilder Create()
    {
        return new EffectFormBuilder();
    }

    internal static EffectFormBuilder WithSavingThrow(
        EffectSavingThrowType savingThrowAffinity,
        TurnOccurenceType saveOccurence = TurnOccurenceType.EndOfTurn,
        bool canSaveToCancel = false)
    {
        return Create().HasSavingThrow(
            savingThrowAffinity,
            saveOccurence,
            canSaveToCancel
        );
    }

    internal EffectFormBuilder HasSavingThrow(
        EffectSavingThrowType savingThrowAffinity,
        TurnOccurenceType saveOccurence = TurnOccurenceType.EndOfTurn,
        bool canSaveToCancel = false)
    {
        _effectForm.HasSavingThrow = true;
        _effectForm.SavingThrowAffinity = savingThrowAffinity;
        _effectForm.saveOccurence = saveOccurence;
        _effectForm.canSaveToCancel = canSaveToCancel;
        return this;
    }

    internal EffectFormBuilder SetBonusMode(AddBonusMode bonusMode)
    {
        _effectForm.AddBonusMode = bonusMode;
        return this;
    }

    internal EffectFormBuilder SetCreatedBy(bool createdByCharacter = false, bool createdByCondition = true)
    {
        _effectForm.createdByCharacter = createdByCharacter;
        _effectForm.createdByCondition = createdByCondition;
        return this;
    }

    internal EffectFormBuilder SetLevelAdvancement(
        EffectForm.LevelApplianceType applyLevel,
        LevelSourceType levelType,
        int levelMultiplier = 1)
    {
        _effectForm.applyLevel = applyLevel;
        _effectForm.levelType = levelType;
        _effectForm.levelMultiplier = levelMultiplier;
        return this;
    }

    internal EffectFormBuilder SetDiceAdvancement(
        LevelSourceType levelType,
        int start = 0,
        int increment = 1,
        int step = 1,
        int begin = 1)
    {
        _effectForm.levelType = levelType;
        _effectForm.applyLevel = EffectForm.LevelApplianceType.DiceNumberByLevelTable;
        _effectForm.DiceByLevelTable.SetRange(DiceByRankBuilder.BuildDiceByRankTable(start, increment, step, begin));
        return this;
    }

    internal EffectFormBuilder SetDiceAdvancement(
        LevelSourceType levelType,
        int minRank = 0,
        int maxRank = 20,
        params (int level, int dice)[] steps)
    {
        _effectForm.levelType = levelType;
        _effectForm.applyLevel = EffectForm.LevelApplianceType.DiceNumberByLevelTable;
        _effectForm.DiceByLevelTable.SetRange(DiceByRankBuilder.InterpolateDiceByRankTable(minRank, maxRank, steps));
        return this;
    }

    internal EffectFormBuilder SetAlterationForm(AlterationForm.Type alterationType)
    {
        var alterationForm = new AlterationForm { alterationType = alterationType };

        _effectForm.alterationForm = alterationForm;
        _effectForm.FormType = EffectForm.EffectFormType.Alteration;
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
        _effectForm.FormType = EffectForm.EffectFormType.Condition;

        var conditionForm = new ConditionForm
        {
            Operation = operation,
            ConditionDefinition = condition,
            applyToSelf = applyToSelf,
            forceOnSelf = forceOnSelf,
            conditionsList = [.. conditionsList]
        };

        if (condition)
        {
            conditionForm.conditionDefinitionName = condition.Name;
        }

        _effectForm.ConditionForm = conditionForm;
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

    internal static EffectForm AddConditionForm(
        ConditionDefinition condition,
        bool applyToSelf = false,
        bool forceOnSelf = false,
        params ConditionDefinition[] conditionsList)
    {
        return ConditionForm(condition, global::ConditionForm.ConditionOperation.Add, applyToSelf, forceOnSelf,
            conditionsList);
    }

#if false
    internal EffectFormBuilder OverrideSavingThrowInfo(
        string savingThrowAbility,
        int savingThrowDc,
        string sourceDefinitionName = "",
        FeatureSourceType featureSourceType = FeatureSourceType.Base)
    {
        _effectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(savingThrowAbility, savingThrowDc,
            sourceDefinitionName, featureSourceType);
        return this;
    }
#endif

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
        _effectForm.counterForm = counterForm;
        _effectForm.FormType = EffectForm.EffectFormType.Counter;
        return this;
    }

    internal EffectForm DamageForm(string damageType = DamageTypeBludgeoning,
        int diceNumber = 0,
        DieType dieType = DieType.D1,
        int bonusDamage = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        bool overrideWithBardicInspirationDie = false)
    {
        return DamageForm(damageType,
            diceNumber,
            dieType,
            bonusDamage,
            healFromInflictedDamage,
            overrideWithBardicInspirationDie,
            this);
    }

    internal static EffectForm DamageForm(string damageType = DamageTypeBludgeoning,
        int diceNumber = 0,
        DieType dieType = DieType.D1,
        int bonusDamage = 0,
        HealFromInflictedDamage healFromInflictedDamage = HealFromInflictedDamage.Never,
        bool overrideWithBardicInspirationDie = false,
        // ReSharper disable once MethodOverloadWithOptionalParameter
        EffectFormBuilder builder = null)
    {
        return (builder ?? Create())
            .SetDamageForm(
                damageType,
                diceNumber,
                dieType,
                bonusDamage,
                healFromInflictedDamage,
                overrideWithBardicInspirationDie
            ).Build();
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
            damageBonusTrends = [],
            OverrideWithBardicInspirationDie = overrideWithBardicInspirationDie
        };

        _effectForm.damageForm = damageForm;
        _effectForm.FormType = EffectForm.EffectFormType.Damage;
        return this;
    }

    internal EffectFormBuilder SetKillForm(KillCondition condition, float challengeRating = 0, int hitPoints = 0)
    {
        var killForm = new KillForm
        {
            killCondition = condition, challengeRating = challengeRating, hitPoints = hitPoints
        };

        _effectForm.killForm = killForm;
        _effectForm.FormType = EffectForm.EffectFormType.Kill;
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

        _effectForm.applyLevel = levelApplianceType;
        _effectForm.healingForm = healingForm;
        _effectForm.FormType = EffectForm.EffectFormType.Healing;
        return this;
    }

    // Unlocks al features at level 0
    internal static EffectForm ItemPropertyForm(
        ItemPropertyUsage usageLimitation,
        int useAmount,
        params FeatureDefinition[] features)
    {
        return ItemPropertyForm(
            usageLimitation,
            useAmount,
            features.Select(f => new FeatureUnlockByLevel(f, 0)).ToArray()
        );
    }

    internal static EffectForm ItemPropertyForm(
        ItemPropertyUsage usageLimitation,
        int useAmount,
        params (FeatureDefinition feature, int level)[] featureBySlotLevel)
    {
        return ItemPropertyForm(
            usageLimitation,
            useAmount,
            featureBySlotLevel.Select(f => new FeatureUnlockByLevel(f.feature, f.level)).ToArray()
        );
    }

    internal static EffectForm ItemPropertyForm(
        ItemPropertyUsage usageLimitation,
        int useAmount,
        params FeatureUnlockByLevel[] featureBySlotLevel)
    {
        return Create().SetItemPropertyForm(usageLimitation, useAmount, featureBySlotLevel).Build();
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

        _effectForm.itemPropertyForm = itemForm;
        _effectForm.FormType = EffectForm.EffectFormType.ItemProperty;
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
        AssetReference graphicsPrefabReference,
        bool applyToSelf = false,
        bool forceOnSelf = false)
    {
        var lightSourceForm = new LightSourceForm
        {
            lightSourceType = lightSourceType,
            brightRange = brightRange,
            dimAdditionalRange = dimAdditionalRange,
            color = color,
            graphicsPrefabReference = graphicsPrefabReference,
            applyToSelf = applyToSelf,
            forceOnSelf = forceOnSelf
        };

        _effectForm.lightSourceForm = lightSourceForm;
        _effectForm.FormType = EffectForm.EffectFormType.LightSource;
        return this;
    }

    [UsedImplicitly]
    internal static EffectForm MotionForm(MotionForm.MotionType motionType, int motionDistance = 0)
    {
        return Create()
            .SetMotionForm(motionType, motionDistance)
            .Build();
    }

    [UsedImplicitly]
    internal static EffectForm MotionForm(ExtraMotionType motionType, int motionDistance = 0)
    {
        return Create()
            .SetMotionForm(motionType, motionDistance)
            .Build();
    }

    internal EffectFormBuilder SetMotionForm(MotionForm.MotionType motionType, int motionDistance = 0)
    {
        var motionForm = new MotionForm { type = motionType, distance = motionDistance };

        _effectForm.motionForm = motionForm;
        _effectForm.FormType = EffectForm.EffectFormType.Motion;
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

        _effectForm.reviveForm = reviveForm;
        _effectForm.FormType = EffectForm.EffectFormType.Revive;
        return this;
    }

    internal EffectFormBuilder SetSpellForm(int maxSlotLevel)
    {
        var spellSlotsForm = new SpellSlotsForm
        {
            type = SpellSlotsForm.EffectType.RecoverHalfLevelUp, maxSlotLevel = maxSlotLevel
        };

        _effectForm.spellSlotsForm = spellSlotsForm;
        _effectForm.FormType = EffectForm.EffectFormType.SpellSlots;
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

        _effectForm.shapeChangeForm = shapeChangeForm;
        _effectForm.FormType = EffectForm.EffectFormType.ShapeChange;

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
            conditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionMindControlledByCaster,
            persistOnConcentrationLoss = false,
            decisionPackage = null,
            effectProxyDefinitionName = null
        };

        _effectForm.summonForm = summonForm;
        _effectForm.FormType = EffectForm.EffectFormType.Summon;
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

        _effectForm.summonForm = summonForm;
        _effectForm.FormType = EffectForm.EffectFormType.Summon;
        return this;
    }

    internal static EffectForm SummonEffectProxyForm(EffectProxyDefinition effectProxyDefinition)
    {
        return Create().SetSummonEffectProxyForm(effectProxyDefinition).Build();
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

        _effectForm.summonForm = summonForm;
        _effectForm.FormType = EffectForm.EffectFormType.Summon;
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
        _effectForm.temporaryHitPointsForm = tempHpForm;
        _effectForm.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
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

        _effectForm.topologyForm = topologyForm;
        _effectForm.FormType = EffectForm.EffectFormType.Topology;
        return this;
    }
}
