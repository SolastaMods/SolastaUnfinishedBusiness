using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using SolastaMonsters.Models;
//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters
{
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
            BuildNewDominateMonster_Spell();  //  soolasta cant handle monsters that cause the party to change sides
            BuildNewFingerOfDeath_Spell();
            BuildNew_PowerWordKill_Spell();
            BuildNew_PowerWordStun_Spell();
            BuildNewTimeStop_Spell();
        }

        public static void BuildNewReverseGravity_Spell()
        {
            string text = "ReverseGravity_Spell";

            ConditionDefinition ReverseGravity_Condition = BuildNewCondition(
                     "DH_Custom_" + text + "condition",
                     DatabaseHelper.ConditionDefinitions.ConditionLevitate,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text + "condition").ToString(),
                    "Condition/&DH_" + text + "_Title",
                     "Condition/&DH_" + text + "_Description"
                      );


            ReverseGravity_Condition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);

            MotionForm motionForm = new MotionForm();
            motionForm.SetDistance(10);
            motionForm.SetType(MotionForm.MotionType.Levitate);

            EffectForm FallingEffect = new EffectForm();
            FallingEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            FallingEffect.SetLevelMultiplier(1);
            FallingEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            FallingEffect.SetCreatedByCondition(true);
            FallingEffect.FormType = EffectForm.EffectFormType.Motion;
            FallingEffect.SetMotionForm(motionForm);
            FallingEffect.SetHasSavingThrow(false);
            FallingEffect.SetCanSaveToCancel(false);
            FallingEffect.SetSaveOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);

            ReverseGravity_Condition.RecurrentEffectForms.Add(FallingEffect);


            ReverseGravity_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.Levitate,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );

            ReverseGravity_Spell.SetSpellLevel(7);



            DamageForm damageForm = new DamageForm();
            damageForm.SetDiceNumber(6);
            damageForm.SetDieType(RuleDefinitions.DieType.D6);
            damageForm.SetBonusDamage(0);
            damageForm.SetDamageType(RuleDefinitions.DamageTypeForce);

            EffectForm damageEffect = new EffectForm();
            damageEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            damageEffect.SetLevelMultiplier(1);
            damageEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            damageEffect.SetCreatedByCharacter(true);
            damageEffect.FormType = EffectForm.EffectFormType.Damage;
            damageEffect.SetDamageForm(damageForm);
            damageEffect.SetHasSavingThrow(false);
            damageEffect.SetCanSaveToCancel(false);

            ReverseGravity_Spell.EffectDescription.EffectForms.Add(damageEffect);

            ReverseGravity_Spell.EffectDescription.EffectForms[0].ConditionForm.SetApplyToSelf(false);
            ReverseGravity_Spell.EffectDescription.EffectForms[0].ConditionForm.SetForceOnSelf(false);
            ReverseGravity_Spell.EffectDescription.EffectForms[1].MotionForm.SetDistance(10);
            ReverseGravity_Spell.EffectDescription.EffectForms.RemoveAt(2);

            ReverseGravity_Spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            ReverseGravity_Spell.EffectDescription.SetTargetParameter(10);
            ReverseGravity_Spell.EffectDescription.SetTargetParameter2(10);


            ReverseGravity_Spell.EffectDescription.RestrictedCharacterSizes.Clear();
            ReverseGravity_Spell.EffectDescription.SetTargetExcludeCaster(true);
            ReverseGravity_Spell.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            ReverseGravity_Spell.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            ReverseGravity_Spell.EffectDescription.SetFixedSavingThrowDifficultyClass(20);

            ReverseGravity_Spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
            ReverseGravity_Spell.EffectDescription.SetDurationParameter(1);

        }

        public static void BuildNewDominateMonster_Spell()
        {
            // control Powers aren't used by monsters in solasta
            string text = "DominateMonster_Spell";


            DominateMonster_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.DominatePerson,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );


            DominateMonster_Spell.SetSpellLevel(8);

            DominateMonster_Spell.EffectDescription.RestrictedCreatureFamilies.Clear();
            DominateMonster_Spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);


        }

        public static void BuildNewFingerOfDeath_Spell()
        {

            string text = "FingerOfDeath_Spell";


            FingerOfDeath_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.Disintegrate,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );

            FingerOfDeath_Spell.SetSpellLevel(7);

            FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(7);
            FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D8);
            FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(30);
            FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.SetDamageType(DatabaseHelper.DamageDefinitions.DamageNecrotic.Name);

            FingerOfDeath_Spell.EffectDescription.EffectForms[0].DamageForm.SetSpecialDeathCondition(DatabaseHelper.ConditionDefinitions.ConditionMindControlledByCaster);

            FingerOfDeath_Spell.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement.GuiPresentation.SpriteReference);

        }

        public static void BuildNew_PowerWordKill_Spell()
        {

            string text = "PowerWordKill_Spell";


            PowerWordKill_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.Harm,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );

            PowerWordKill_Spell.SetSpellLevel(9);


            PowerWordKill_Spell.SetSomaticComponent(false);
            PowerWordKill_Spell.EffectDescription.SetHasSavingThrow(false);
            PowerWordKill_Spell.EffectDescription.EffectForms.Clear();

            KillForm killForm = new KillForm();
            killForm.SetKillCondition(RuleDefinitions.KillCondition.UnderHitPoints);
            killForm.SetHitPoints(100);

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Kill;
            effectForm.SetKillForm(killForm);


            PowerWordKill_Spell.EffectDescription.EffectForms.Add(effectForm);

            PowerWordKill_Spell.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Banishment.GuiPresentation.SpriteReference);


        }
        public static void BuildNew_PowerWordStun_Spell()
        {

            string text = "PowerWordStun_Spell";


            PowerWordStun_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.Harm,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );

            PowerWordStun_Spell.SetSpellLevel(8);

            PowerWordStun_Spell.SetSomaticComponent(false);
            PowerWordStun_Spell.EffectDescription.SetHasSavingThrow(false);
            PowerWordStun_Spell.EffectDescription.EffectForms.Clear();

            ConditionForm conditionForm = new ConditionForm();
            conditionForm.SetApplyToSelf(false);
            conditionForm.SetForceOnSelf(false);
            conditionForm.Operation = ConditionForm.ConditionOperation.Add;
            conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionStunned.Name);
            conditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionStunned;



            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Condition;
            effectForm.SetConditionForm(conditionForm);
            effectForm.SetCanSaveToCancel(true);
            effectForm.SetSaveOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            PowerWordStun_Spell.EffectDescription.EffectForms.Add(effectForm);
            PowerWordStun_Spell.EffectDescription.SetTargetFilteringMethod(RuleDefinitions.TargetFilteringMethod.CharacterIncreasingHitPointsFromPool);
            PowerWordStun_Spell.EffectDescription.SetPoolFilterDiceNumber(150);
            PowerWordStun_Spell.EffectDescription.SetPoolFilterDieType(RuleDefinitions.DieType.D1);

            PowerWordStun_Spell.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.name);
            PowerWordStun_Spell.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            PowerWordStun_Spell.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
            PowerWordStun_Spell.EffectDescription.SetHasSavingThrow(true);


            PowerWordStun_Spell.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Contagion.GuiPresentation.SpriteReference);

        }


        public static void BuildNewTimeStop_Spell()
        {

            string text = "TimeStop_Spell";

            ConditionDefinition TimeStopped_Condition = BuildNewCondition(
                 "DH_Custom_" + text + "condition",
                 DatabaseHelper.ConditionDefinitions.ConditionIncapacitated,
                 GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text + "condition").ToString(),
                "Condition/&DH_" + text + "_Title",
                 "Condition/&DH_" + text + "_Description"
                  );

            TimeStopped_Condition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Attacked);
            TimeStopped_Condition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);
            TimeStopped_Condition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacked);
            TimeStopped_Condition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Damaged);
            TimeStopped_Condition.SetInterruptionDamageThreshold(1);
            TimeStopped_Condition.SetInterruptionRequiresSavingThrow(false);


            TimeStop_Spell = BuildNewSpell(
                     "DH_Custom_" + text,
                     DatabaseHelper.SpellDefinitions.Disintegrate,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                    "Spell/&DH_" + text + "_Title",
                     "Spell/&DH_" + text + "_Description"
                      );

            TimeStop_Spell.SetSpellLevel(9);

            ConditionForm conditionForm = new ConditionForm();
            conditionForm.SetApplyToSelf(false);
            conditionForm.SetForceOnSelf(false);
            conditionForm.Operation = ConditionForm.ConditionOperation.Add;
            conditionForm.SetConditionDefinitionName(TimeStopped_Condition.Name);
            conditionForm.ConditionDefinition = TimeStopped_Condition;



            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Condition;
            effectForm.SetConditionForm(conditionForm);
            effectForm.SetCanSaveToCancel(false);

            TimeStop_Spell.EffectDescription.SetHasSavingThrow(false);
            TimeStop_Spell.EffectDescription.EffectForms.Clear();
            TimeStop_Spell.EffectDescription.EffectForms.Add(effectForm);
            TimeStop_Spell.EffectDescription.SetTargetFilteringMethod(RuleDefinitions.TargetFilteringMethod.CharacterOnly);
            TimeStop_Spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            TimeStop_Spell.EffectDescription.SetDurationParameter(3);
            TimeStop_Spell.EffectDescription.EffectForms.Add(effectForm);
            TimeStop_Spell.SetRequiresConcentration(false);
            TimeStop_Spell.SetRitual(false);
            TimeStop_Spell.EffectDescription.SetTargetExcludeCaster(true);
            TimeStop_Spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            TimeStop_Spell.EffectDescription.SetTargetSide(RuleDefinitions.Side.All);
            TimeStop_Spell.EffectDescription.SetTargetParameter(20);
            TimeStop_Spell.EffectDescription.SetTargetParameter2(10);
            TimeStop_Spell.EffectDescription.SetRangeParameter(0);
            TimeStop_Spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);

            TimeStop_Spell.GuiPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference);

        }
        public static SpellDefinition BuildNewSpell(string name, SpellDefinition baseSpell, string guid, string title, string description)
        {
            return SpellDefinitionBuilder
                .Create(baseSpell, name, guid)
                .SetOrUpdateGuiPresentation(title, description)
                .AddToDB();
        }

        public static ConditionDefinition BuildNewCondition(string name, ConditionDefinition baseCondition, string guid, string title, string description)
        {
            return ConditionDefinitionBuilder
                .Create(baseCondition, name, guid)
                .SetOrUpdateGuiPresentation(title, description)
                .AddToDB();
        }

    }

}
