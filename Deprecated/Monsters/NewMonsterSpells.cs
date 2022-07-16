using System;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaMonsters.Models;
using UnityEngine;

//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters;

public class NewMonsterSpells
{
    public static SpellDefinition DominateMonster_Spell = ScriptableObject.CreateInstance<SpellDefinition>();
    public static SpellDefinition PowerWordKill_Spell = ScriptableObject.CreateInstance<SpellDefinition>();
    public static SpellDefinition PowerWordStun_Spell = ScriptableObject.CreateInstance<SpellDefinition>();
    public static SpellDefinition FingerOfDeath_Spell = ScriptableObject.CreateInstance<SpellDefinition>();
    public static SpellDefinition ReverseGravity_Spell = ScriptableObject.CreateInstance<SpellDefinition>();
    public static SpellDefinition TimeStop_Spell = ScriptableObject.CreateInstance<SpellDefinition>();

    internal static void Create()
    {
        BuildNewReverseGravity_Spell();
        BuildNewDominateMonster_Spell(); //  soolasta cant handle monsters that cause the party to change sides
        BuildNewFingerOfDeath_Spell();
        BuildNew_PowerWordKill_Spell();
        BuildNew_PowerWordStun_Spell();
        BuildNewTimeStop_Spell();
    }

    public static void BuildNewReverseGravity_Spell()
    {
        var text = "ReverseGravity_Spell";

        var ReverseGravity_Condition = BuildNewCondition(
            "DH_Custom_" + text + "condition",
            DatabaseHelper.ConditionDefinitions.ConditionLevitate,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text + "condition").ToString(),
            "Condition/&DH_" + text + "_Title",
            "Condition/&DH_" + text + "_Description"
        );


        ReverseGravity_Condition.turnOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn;

        MotionForm motionForm = new();
        motionForm.distance = 10;
        motionForm.type = MotionForm.MotionType.Levitate;

        EffectForm FallingEffect = new();
        FallingEffect.applyLevel = EffectForm.LevelApplianceType.No;
        FallingEffect.levelMultiplier = 1;
        FallingEffect.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        FallingEffect.createdByCondition = true;
        FallingEffect.FormType = EffectForm.EffectFormType.Motion;
        FallingEffect.motionForm = motionForm;
        FallingEffect.hasSavingThrow = false;
        FallingEffect.canSaveToCancel = false;
        FallingEffect.saveOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn;

        ReverseGravity_Condition.RecurrentEffectForms.Add(FallingEffect);


        ReverseGravity_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.Levitate,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );

        ReverseGravity_Spell.spellLevel = 7;


        DamageForm damageForm = new();
        damageForm.diceNumber = 6;
        damageForm.dieType = RuleDefinitions.DieType.D6;
        damageForm.bonusDamage = 0;
        damageForm.damageType = RuleDefinitions.DamageTypeForce;

        EffectForm damageEffect = new();
        damageEffect.applyLevel = EffectForm.LevelApplianceType.No;
        damageEffect.levelMultiplier = 1;
        damageEffect.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        damageEffect.createdByCharacter = true;
        damageEffect.FormType = EffectForm.EffectFormType.Damage;
        damageEffect.damageForm = damageForm;
        damageEffect.hasSavingThrow = false;
        damageEffect.canSaveToCancel = false;

        ReverseGravity_Spell.EffectDescription.EffectForms.Add(damageEffect);

        ReverseGravity_Spell.EffectDescription.EffectForms[0].ConditionForm.applyToSelf = false;
        ReverseGravity_Spell.EffectDescription.EffectForms[0].ConditionForm.forceOnSelf = false;
        ReverseGravity_Spell.EffectDescription.EffectForms[1].MotionForm.distance = 10;
        ReverseGravity_Spell.EffectDescription.EffectForms.RemoveAt(2);

        ReverseGravity_Spell.EffectDescription.targetType = RuleDefinitions.TargetType.Cylinder;
        ReverseGravity_Spell.EffectDescription.targetParameter = 10;
        ReverseGravity_Spell.EffectDescription.targetParameter2 = 10;


        ReverseGravity_Spell.EffectDescription.RestrictedCharacterSizes.Clear();
        ReverseGravity_Spell.EffectDescription.targetExcludeCaster = true;
        ReverseGravity_Spell.EffectDescription.savingThrowAbility = DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name;
        ReverseGravity_Spell.EffectDescription.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.FixedValue;
        ReverseGravity_Spell.EffectDescription.fixedSavingThrowDifficultyClass = 20;

        ReverseGravity_Spell.EffectDescription.durationType = RuleDefinitions.DurationType.Minute;
        ReverseGravity_Spell.EffectDescription.durationParameter = 1;
    }

    public static void BuildNewDominateMonster_Spell()
    {
        // control Powers aren't used by monsters in solasta
        var text = "DominateMonster_Spell";


        DominateMonster_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.DominatePerson,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );


        DominateMonster_Spell.spellLevel = 8;

        DominateMonster_Spell.EffectDescription.RestrictedCreatureFamilies.Clear();
        DominateMonster_Spell.EffectDescription.durationType = RuleDefinitions.DurationType.Hour;
    }

    public static void BuildNewFingerOfDeath_Spell()
    {
        var text = "FingerOfDeath_Spell";


        FingerOfDeath_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.Disintegrate,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );

        FingerOfDeath_Spell.spellLevel = 7;

        FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.diceNumber = 7;
        FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D8;
        FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.bonusDamage = 30;
        FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm
            .damageType = DatabaseHelper.DamageDefinitions.DamageNecrotic.Name;

        FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm
            .specialDeathCondition = DatabaseHelper.ConditionDefinitions.ConditionMindControlledByCaster;

        FingerOfDeath_Spell.GuiPresentation.spriteReference = DatabaseHelper.SpellDefinitions.RayOfEnfeeblement
            .GuiPresentation.SpriteReference;
    }

    public static void BuildNew_PowerWordKill_Spell()
    {
        var text = "PowerWordKill_Spell";


        PowerWordKill_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.Harm,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );

        PowerWordKill_Spell.spellLevel = 9;


        PowerWordKill_Spell.somaticComponent = false;
        PowerWordKill_Spell.EffectDescription.hasSavingThrow = false;
        PowerWordKill_Spell.EffectDescription.EffectForms.Clear();

        KillForm killForm = new();
        killForm.killCondition = RuleDefinitions.KillCondition.UnderHitPoints;
        killForm.hitPoints = 100;

        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Kill;
        effectForm.killForm = killForm;


        PowerWordKill_Spell.EffectDescription.EffectForms.Add(effectForm);

        PowerWordKill_Spell.GuiPresentation.spriteReference = DatabaseHelper.SpellDefinitions.Banishment
            .GuiPresentation.SpriteReference;
    }

    public static void BuildNew_PowerWordStun_Spell()
    {
        var text = "PowerWordStun_Spell";


        PowerWordStun_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.Harm,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );

        PowerWordStun_Spell.spellLevel = 8;

        PowerWordStun_Spell.somaticComponent = false;
        PowerWordStun_Spell.EffectDescription.hasSavingThrow = false;
        PowerWordStun_Spell.EffectDescription.EffectForms.Clear();

        ConditionForm conditionForm = new();
        conditionForm.applyToSelf = false;
        conditionForm.forceOnSelf = false;
        conditionForm.Operation = ConditionForm.ConditionOperation.Add;
        conditionForm.conditionDefinitionName = DatabaseHelper.ConditionDefinitions.ConditionStunned.Name;
        conditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionStunned;


        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Condition;
        effectForm.conditionForm = conditionForm;
        effectForm.canSaveToCancel = true;
        effectForm.saveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;

        PowerWordStun_Spell.EffectDescription.EffectForms.Add(effectForm);
        PowerWordStun_Spell.EffectDescription.targetFilteringMethod = RuleDefinitions.TargetFilteringMethod
            .CharacterIncreasingHitPointsFromPool;
        PowerWordStun_Spell.EffectDescription.poolFilterDiceNumber = 150;
        PowerWordStun_Spell.EffectDescription.poolFilterDieType = RuleDefinitions.DieType.D1;

        PowerWordStun_Spell.EffectDescription.savingThrowAbility = DatabaseHelper.SmartAttributeDefinitions
            .Constitution.name;
        PowerWordStun_Spell.EffectDescription.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.FixedValue;
        PowerWordStun_Spell.EffectDescription.fixedSavingThrowDifficultyClass = 20;
        PowerWordStun_Spell.EffectDescription.hasSavingThrow = true;


        PowerWordStun_Spell.GuiPresentation.spriteReference = DatabaseHelper.SpellDefinitions.Contagion
            .GuiPresentation.SpriteReference;
    }


    public static void BuildNewTimeStop_Spell()
    {
        var text = "TimeStop_Spell";

        var TimeStopped_Condition = BuildNewCondition(
            "DH_Custom_" + text + "condition",
            DatabaseHelper.ConditionDefinitions.ConditionIncapacitated,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text + "condition").ToString(),
            "Condition/&DH_" + text + "_Title",
            "Condition/&DH_" + text + "_Description"
        );

        TimeStopped_Condition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Attacked);
        TimeStopped_Condition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);
        TimeStopped_Condition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacked);
        TimeStopped_Condition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Damaged);
        TimeStopped_Condition.interruptionDamageThreshold = 1;
        TimeStopped_Condition.interruptionRequiresSavingThrow = false;


        TimeStop_Spell = BuildNewSpell(
            "DH_Custom_" + text,
            DatabaseHelper.SpellDefinitions.Disintegrate,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "Spell/&DH_" + text + "_Title",
            "Spell/&DH_" + text + "_Description"
        );

        TimeStop_Spell.spellLevel = 9;

        ConditionForm conditionForm = new();
        conditionForm.applyToSelf = false;
        conditionForm.forceOnSelf = false;
        conditionForm.Operation = ConditionForm.ConditionOperation.Add;
        conditionForm.conditionDefinitionName = TimeStopped_Condition.Name;
        conditionForm.ConditionDefinition = TimeStopped_Condition;


        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Condition;
        effectForm.conditionForm = conditionForm;
        effectForm.canSaveToCancel = false;

        TimeStop_Spell.EffectDescription.hasSavingThrow = false;
        TimeStop_Spell.EffectDescription.EffectForms.Clear();
        TimeStop_Spell.EffectDescription.EffectForms.Add(effectForm);
        TimeStop_Spell.EffectDescription.targetFilteringMethod = RuleDefinitions.TargetFilteringMethod
            .CharacterOnly;
        TimeStop_Spell.EffectDescription.durationType = RuleDefinitions.DurationType.Round;
        TimeStop_Spell.EffectDescription.durationParameter = 3;
        TimeStop_Spell.EffectDescription.EffectForms.Add(effectForm);
        TimeStop_Spell.requiresConcentration = false;
        TimeStop_Spell.ritual = false;
        TimeStop_Spell.EffectDescription.targetExcludeCaster = true;
        TimeStop_Spell.EffectDescription.targetType = RuleDefinitions.TargetType.Cylinder;
        TimeStop_Spell.EffectDescription.targetSide = RuleDefinitions.Side.All;
        TimeStop_Spell.EffectDescription.targetParameter = 20;
        TimeStop_Spell.EffectDescription.targetParameter2 = 10;
        TimeStop_Spell.EffectDescription.rangeParameter = 0;
        TimeStop_Spell.EffectDescription.rangeType = RuleDefinitions.RangeType.Self;

        TimeStop_Spell.GuiPresentation.spriteReference = DatabaseHelper.FeatureDefinitionPowers
            .PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference;
    }

    public static SpellDefinition BuildNewSpell(string name, SpellDefinition baseSpell, string guid, string title,
        string description)
    {
        return SpellDefinitionBuilder
            .Create(baseSpell, name, guid)
            .SetOrUpdateGuiPresentation(title, description)
            .AddToDB();
    }

    public static ConditionDefinition BuildNewCondition(string name, ConditionDefinition baseCondition, string guid,
        string title, string description)
    {
        return ConditionDefinitionBuilder
            .Create(baseCondition, name, guid)
            .SetOrUpdateGuiPresentation(title, description)
            .AddToDB();
    }
}
