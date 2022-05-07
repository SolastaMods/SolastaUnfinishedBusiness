using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders.Features;
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
    public class NewMonsterPowers
    {
        public static FeatureDefinitionPower Generic_Lightning_Attack_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower Balor_FireAura_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower IncreasedGravityZone_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower Lich_DisruptLife_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower FireTitan_Aura_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AirTitan_Lightning_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AirTitan_Gale_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AirTitan_LightningStorm_Attack_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower EarthTitan_Earthquake_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower IlluminatingCrystals_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower DisintegratingBeam_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AtWillSelfBuff_Invisibility_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AtWillAOE_Fireball_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower LimitedPerDayTargetDebuff_HoldMonster_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower LimitedPerDayAOE_WallOfFire_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SummonCreature_Erinyes_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower ErinyesParry_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SummonCreature_Nalfeshnee_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SearingBurst_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower BlindingGaze_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SummonCreature_Elemental_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SummonCreature_Wolves_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower SummonCreature_LesserConstruct_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower AncientDragon_Wing_Power = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower TarrasqueSwallowPower = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        public static FeatureDefinitionPower VampireCharmPower = ScriptableObject.CreateInstance<FeatureDefinitionPower>();

        public static Dictionary<string, FeatureDefinitionPower> DictionaryOfAncientDragonBreaths = new Dictionary<string, FeatureDefinitionPower>();
        public static Dictionary<string, FeatureDefinitionPower> DictionaryOfGenericBreathsWithExtraDamage = new Dictionary<string, FeatureDefinitionPower>();
        public static Dictionary<string, FeatureDefinitionPower> Dictionaryof_SummoningElementals = new Dictionary<string, FeatureDefinitionPower>();


        internal static void Create()
        {
            BuildNewGeneric_Lightning_Attack();
            BuildNew_AncientDragon_Breath_Power();
            BuildNewBalor_FireAura_Power();
            BuildNewLich_DisruptLife_Power();
            BuildNewErinyesParry_Power();
            BuildNewSearingBurst_Power();
            BuildNewBlindingGaze_Power();
            BuildNewSummonCreature_Elemental_Power();
            BuildNewAtWillSelfBuff_Invisibility_Power();
            BuildNewAtWillAOE_Fireball_Power();
            BuildNewLimitedPerDayTargetDebuff_HoldMonster_Power();
            BuildNewLimitedPerDayAOE_WallOfFire_Power();
            BuildNewSummonCreature_Erinyes_Power();
            BuildNewSummonCreature_Nalfeshnee_Power();
            BuildNewSummonCreature_Wolves_Power();
            BuildNewAncientDragon_Wing_Power();
            BuildNewAirTitan_Gale_Power();
            BuildNewAirTitan_Lightning_Power();
            BuildNewAirTitan_LightningStorm_Attack();
            BuildNewFireTitan_Aura_Power();
            BuildNewEarthTitan_Earthquake_Power();
            BuildNewIlluminatingCrystals_Power();
            BuildNewDisintegratingBeam_Power();
            BuildNewSummonCreature_LesserConstruct_Power();
            BuildNewIncreasedGravityZone_Attack();
            BuildNewTarrasqueSwallowPower();
            BuildNewVampireCharmPower();
        }
        public static void BuildNewTarrasqueSwallowPower()
        {

            string text = "TarrasqueSwallowPower";


            ConditionDefinition TarrasqueSwallowingCondition = NewMonsterAttacks.BuildNewCondition(
                         "DH_Custom_TarrasqueSwallowingcondition",
                         DatabaseHelper.ConditionDefinitions.ConditionSwallowingRemorhaz,
                         GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_TarrasqueSwallowingcondition").ToString(),
                        "MonsterAttack/&DH_Custom_TarrasqueSwallowingcondition_Title",
                        "MonsterAttack/&DH_Custom_TarrasqueSwallowingcondition_Description"
           );

            TarrasqueSwallowingCondition.SetInterruptionDamageThreshold(60);
            TarrasqueSwallowingCondition.SetInterruptionRequiresSavingThrow(false);




            ConditionDefinition TarrasqueSwallowedCondition = NewMonsterAttacks.BuildNewCondition(
                         "DH_Custom_TarrasqueSwallowedcondition",
                         DatabaseHelper.ConditionDefinitions.ConditionSwallowedRemorhaz,
                         GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_TarrasqueSwallowedcondition").ToString(),
                        "MonsterAttack/&DH_Custom_TarrasqueSwallowedcondition_Title",
                        "MonsterAttack/&DH_Custom_TarrasqueSwallowedcondition_Description"
           );

            TarrasqueSwallowedCondition.RecurrentEffectForms[0].DamageForm.SetDiceNumber(16);


            TarrasqueSwallowPower = BuildNewPower(
                    text + "DH_Custom" ,
                   DatabaseHelper.FeatureDefinitionPowers.PowerRemorhazSwallow,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            TarrasqueSwallowPower.EffectDescription.SetTargetConditionAsset(NewMonsterAttacks.TarrasqueGrappledRestrainedCondition);

            TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(4);
            TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D12);
            TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(10);
            TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);

            TarrasqueSwallowPower.EffectDescription.EffectForms[2].ConditionForm.SetConditionDefinition(TarrasqueSwallowedCondition);

            TarrasqueSwallowPower.EffectDescription.EffectForms[3].ConditionForm.SetConditionDefinition(TarrasqueSwallowingCondition);

            TarrasqueSwallowPower.EffectDescription.EffectForms.RemoveAt(1);
        }
        public static void BuildNewErinyesParry_Power()
        {

            string text = "ErinyesParry_Power";


            ErinyesParry_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerFeatTwinBlade,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            ErinyesParry_Power.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);
            ErinyesParry_Power.EffectDescription.EffectForms[0].ConditionForm.SetApplyToSelf(true);
            ErinyesParry_Power.EffectDescription.EffectForms[0].ConditionForm.SetForceOnSelf(true);
        }
        public static void BuildNewVampireCharmPower()
        {

            string text = "VampireCharmPower";


            VampireCharmPower = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerLaetharParalyzingGaze,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                   "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.HypnoticPattern.EffectDescription);
            effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            effectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            effectDescription.SetTargetParameter(1);
            effectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            effectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Wisdom.Name);
            effectDescription.SetFixedSavingThrowDifficultyClass(17);
            VampireCharmPower.SetEffectDescription(effectDescription);
        }
        public static void BuildNewGeneric_Lightning_Attack()
        {

            // for storm giant
            string text = "Generic_Lightning_Attack";

            /*
                     //   FeatureDefinitionPower Generic_Lightning_Attack_Power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>
                               .createPower(
                               text + "DH_Custom_Power",
                               GuidHelper.Create(new System.Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
                               "Feature/&DH_" + text + "_Custom_Power_Title",
                               "Feature/&DH_" + text + "_Custom_Power_Description",
                               DatabaseHelper.SpellDefinitions.LightningBolt.GuiPresentation.SpriteReference,
                               DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.EffectDescription,
                               RuleDefinitions.ActivationTime.Action,
                               1,
                               RuleDefinitions.UsesDetermination.Fixed,
                               RuleDefinitions.RechargeRate.D6_56,
                               "Charisma",
                               "Charisma",
                               1,
                               true
                               );
                        */

            Generic_Lightning_Attack_Power = BuildNewPower(
                       text + "DH_Custom_Power",
                       DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
                       GuidHelper.Create(new System.Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
                       "Feature/&DH_" + text + "_Custom_Power_Title",
                       "Feature/&DH_" + text + "_Custom_Power_Description"
                        );


            Generic_Lightning_Attack_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Generic_Lightning_Attack_Power.SetFixedUsesPerRecharge(1);
            Generic_Lightning_Attack_Power.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            Generic_Lightning_Attack_Power.SetRechargeRate(RuleDefinitions.RechargeRate.D6_56);
            Generic_Lightning_Attack_Power.SetUsesAbilityScoreName("Charisma");
            Generic_Lightning_Attack_Power.SetAbilityScore("Charisma");
            Generic_Lightning_Attack_Power.SetCostPerUse(1);
            Generic_Lightning_Attack_Power.SetShowCasting(true);




            Generic_Lightning_Attack_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            Generic_Lightning_Attack_Power.EffectDescription.SetTargetParameter(3);
            Generic_Lightning_Attack_Power.EffectDescription.SetTargetParameter(8);
            Generic_Lightning_Attack_Power.EffectDescription.SetRangeParameter(100);
            Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(12);
            Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D8);

            Generic_Lightning_Attack_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            Generic_Lightning_Attack_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            Generic_Lightning_Attack_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(17);
        }


        public static void BuildNew_AncientDragon_Breath_Power()
        {




            Dictionary<string, int> dictionaryofAncientDragonBreathExtraDamageDiceNumbers = new Dictionary<string, int>
            {
                { "Ancient Black Dragon", 15 },
                { "Ancient Blue Dragon", 16 },
                { "Ancient Green Dragon", 22 },
                { "Ancient Red Dragon", 26 },
                { "Ancient White Dragon", 16 }
            };

            Dictionary<string, RuleDefinitions.DieType> dictionaryofAncientDragonBreathExtraDamageDiceType = new Dictionary<string, RuleDefinitions.DieType>
            {
                { "Ancient Black Dragon", RuleDefinitions.DieType.D8 },
                { "Ancient Blue Dragon", RuleDefinitions.DieType.D10 },
                { "Ancient Green Dragon", RuleDefinitions.DieType.D6 },
                { "Ancient Red Dragon", RuleDefinitions.DieType.D6 },
                { "Ancient White Dragon", RuleDefinitions.DieType.D8 }
            };

            Dictionary<string, RuleDefinitions.TargetType> dictionaryofAncientDragonBreathShape = new Dictionary<string, RuleDefinitions.TargetType>
            {
                { "Ancient Black Dragon", RuleDefinitions.TargetType.Line },
                { "Ancient Blue Dragon", RuleDefinitions.TargetType.Line },
                { "Ancient Green Dragon", RuleDefinitions.TargetType.Cone },
                { "Ancient Red Dragon", RuleDefinitions.TargetType.Cone },
                { "Ancient White Dragon", RuleDefinitions.TargetType.Cone }
            };

            Dictionary<string, EffectParticleParameters> dictionaryofAncientDragonBreathEffectparticles = new Dictionary<string, EffectParticleParameters>
            {
                { "Ancient Black Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Acid.EffectDescription.EffectParticleParameters },
                { "Ancient Blue Dragon", DatabaseHelper.SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters },
                { "Ancient Green Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Poison.EffectDescription.EffectParticleParameters },
                { "Ancient Red Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Fire.EffectDescription.EffectParticleParameters },
                { "Ancient White Dragon", DatabaseHelper.SpellDefinitions.ConeOfCold.EffectDescription.EffectParticleParameters }
            };


            foreach (KeyValuePair<string, string> entry in NewMonsterAttributes.Dictionaryof_Dragon_DamageAffinity)
            {

                string text = entry.Value;
                text = text.Replace(" ", "");

                FeatureDefinitionPower Dragon_Breath_Power = BuildNewPower(
                       "PowerDragonBreath_DH_Custom_" + text,
                       DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Fire,
                       GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                      "MonsterPower/&DH_" + text + "_Breath_Power" + "_Title",
                       "MonsterPower/&DH_" + text + "_Breath_Power" + "_Description"
                        );

                Dragon_Breath_Power.EffectDescription.SetTargetType(dictionaryofAncientDragonBreathShape[entry.Key]);
                Dragon_Breath_Power.EffectDescription.SetTargetParameter(20);
                // generic ancient dragon Breath Power
                Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(dictionaryofAncientDragonBreathExtraDamageDiceNumbers[entry.Key]);
                Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(dictionaryofAncientDragonBreathExtraDamageDiceType[entry.Key]);
                Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(entry.Value);    // ListofDamageTypes_Dragon[i]);
                Dragon_Breath_Power.EffectDescription.SetEffectParticleParameters(dictionaryofAncientDragonBreathEffectparticles[entry.Key]);
                Dragon_Breath_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(23);

                DictionaryOfAncientDragonBreaths.Add(entry.Key, Dragon_Breath_Power);


                DictionaryOfGenericBreathsWithExtraDamage.Add(entry.Value, Dragon_Breath_Power);
            }




        }


        public static void BuildNewBalor_FireAura_Power()
        {

            string text = "Balor_FireAura_Power";


            Balor_FireAura_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerArrokAuraOfFire,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            Balor_FireAura_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            Balor_FireAura_Power.EffectDescription.SetTargetParameter(4);

            Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(3);
            Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeFire);



        }
        public static void BuildNewLich_DisruptLife_Power()
        {

            string text = "Lich_DisruptLife_Power";


            Lich_DisruptLife_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            Lich_DisruptLife_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            Lich_DisruptLife_Power.EffectDescription.SetTargetParameter(4);
            Lich_DisruptLife_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(18);
            Lich_DisruptLife_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
            Lich_DisruptLife_Power.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);


            Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(6);
            Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeNecrotic);



        }
        public static void BuildNewAncientDragon_Wing_Power()
        {

            string text = "AncientDragon_Wing_Power";


            AncientDragon_Wing_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            AncientDragon_Wing_Power.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(9);
            AncientDragon_Wing_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(24);
        }

        public static void BuildNewSummonCreature_Elemental_Power()
        {

            Dictionary<SpellDefinition, int> dictionaryelementals = new Dictionary<SpellDefinition, int>
            {
                { DatabaseHelper.SpellDefinitions.ConjureElementalAir, 1 },
                { DatabaseHelper.SpellDefinitions.ConjureElementalEarth, 1 },
                { DatabaseHelper.SpellDefinitions.ConjureElementalFire, 1 },
                { DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour, 4 },
                { DatabaseHelper.SpellDefinitions.ConjureMinorElementalsOne, 1 },
                { DatabaseHelper.SpellDefinitions.ConjureMinorElementalsTwo, 2 }
            };


            foreach (KeyValuePair<SpellDefinition, int> entry in dictionaryelementals)

            {
                string text = "SummonCreature_" + entry.Key.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName + "_Power";

                SummonCreature_Elemental_Power = BuildNewPower(
                    text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

                SummonCreature_Elemental_Power.SetHasCastingFailure(false);
                SummonCreature_Elemental_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.EffectDescription);
                SummonCreature_Elemental_Power.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(entry.Key.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName);
                SummonCreature_Elemental_Power.EffectDescription.EffectForms[0].SummonForm.SetNumber(entry.Value);

                SummonCreature_Elemental_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                SummonCreature_Elemental_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

                Dictionaryof_SummoningElementals.Add(entry.Key.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName, SummonCreature_Elemental_Power);
            }
        }


        public static void BuildNewSearingBurst_Power()
        {

            string text = "SearingBurst_Power";


            SearingBurst_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            SearingBurst_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            SearingBurst_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            SearingBurst_Power.EffectDescription.SetTargetParameter(4);

            SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(4);
            SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeFire);
            SearingBurst_Power.EffectDescription.EffectForms[0].SetHasSavingThrow(true);
            SearingBurst_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);

            DamageForm damageForm = new DamageForm();
            damageForm.SetDiceNumber(4);
            damageForm.SetDieType(RuleDefinitions.DieType.D6);
            damageForm.SetBonusDamage(0);
            damageForm.SetDamageType(RuleDefinitions.DamageTypeRadiant);


            EffectForm extraDamageEffect = new EffectForm();
            extraDamageEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            extraDamageEffect.SetLevelMultiplier(1);
            extraDamageEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            extraDamageEffect.SetCreatedByCharacter(true);
            extraDamageEffect.FormType = EffectForm.EffectFormType.Damage;
            extraDamageEffect.SetDamageForm(damageForm);
            extraDamageEffect.SetHasSavingThrow(true);
            extraDamageEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);

            SearingBurst_Power.EffectDescription.EffectForms.Add(extraDamageEffect);
            SearingBurst_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            SearingBurst_Power.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            SearingBurst_Power.EffectDescription.SetHasSavingThrow(true);
            SearingBurst_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(23);

        }
        public static void BuildNewBlindingGaze_Power()
        {

            string text = "BlindingGaze_Power";


            BlindingGaze_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerLaetharParalyzingGaze,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            BlindingGaze_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            BlindingGaze_Power.EffectDescription.EffectForms[0].SetCanSaveToCancel(false);

            BlindingGaze_Power.EffectDescription.EffectForms[0].ConditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionBlinded);



        }

        public static void BuildNewAtWillSelfBuff_Invisibility_Power()
        {

            string text = "AtWillSelfBuff_Invisibility_Power";


            AtWillSelfBuff_Invisibility_Power = BuildNewPower(
                   text + "_DH_Custom",
                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            AtWillSelfBuff_Invisibility_Power.SetGuiPresentation(DatabaseHelper.SpellDefinitions.Invisibility.GuiPresentation);
            AtWillSelfBuff_Invisibility_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Invisibility.EffectDescription);
            AtWillSelfBuff_Invisibility_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            AtWillSelfBuff_Invisibility_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }

        public static void BuildNewAtWillAOE_Fireball_Power()
        {

            string text = "AtWillAOE_Fireball_Power";


            AtWillAOE_Fireball_Power = BuildNewPower(
                   text + "_DH_Custom",
                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );
            AtWillAOE_Fireball_Power.SetGuiPresentation(DatabaseHelper.SpellDefinitions.Fireball.GuiPresentation);
            AtWillAOE_Fireball_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
            AtWillAOE_Fireball_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            AtWillAOE_Fireball_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            AtWillAOE_Fireball_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            AtWillAOE_Fireball_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            AtWillAOE_Fireball_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            AtWillAOE_Fireball_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
        }

        public static void BuildNewLimitedPerDayTargetDebuff_HoldMonster_Power()
        {

            string text = "LimitedPerDayTargetDebuff_HoldMonster_Power";


            LimitedPerDayTargetDebuff_HoldMonster_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawAnathema,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            LimitedPerDayTargetDebuff_HoldMonster_Power.SetUniqueInstance(true);
            LimitedPerDayTargetDebuff_HoldMonster_Power.SetGuiPresentation(DatabaseHelper.SpellDefinitions.HoldMonster.GuiPresentation);
            LimitedPerDayTargetDebuff_HoldMonster_Power.SetShortTitleOverride(DatabaseHelper.SpellDefinitions.HoldMonster.GuiPresentation.Title);
            LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.HoldMonster.EffectDescription);
            LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(21);

            LimitedPerDayTargetDebuff_HoldMonster_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            LimitedPerDayTargetDebuff_HoldMonster_Power.SetCostPerUse(1);
            LimitedPerDayTargetDebuff_HoldMonster_Power.SetFixedUsesPerRecharge(3);
            LimitedPerDayTargetDebuff_HoldMonster_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }


        public static void BuildNewLimitedPerDayAOE_WallOfFire_Power()
        {

            string text = "LimitedPerDayAOE_WallOfFire_Power";

            LimitedPerDayAOE_WallOfFire_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            LimitedPerDayAOE_WallOfFire_Power.SetUniqueInstance(false);
            LimitedPerDayAOE_WallOfFire_Power.SetGuiPresentation(DatabaseHelper.SpellDefinitions.WallOfFire.GuiPresentation);
            LimitedPerDayAOE_WallOfFire_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.WallOfFireRing_Outer.EffectDescription);
            LimitedPerDayAOE_WallOfFire_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            LimitedPerDayAOE_WallOfFire_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(21);


            LimitedPerDayAOE_WallOfFire_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            LimitedPerDayAOE_WallOfFire_Power.SetCostPerUse(1);
            LimitedPerDayAOE_WallOfFire_Power.SetFixedUsesPerRecharge(3);
            LimitedPerDayAOE_WallOfFire_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }

        public static void BuildNewSummonCreature_Erinyes_Power()
        {

            string text = "SummonCreature_Erinyes_Power";

            SummonCreature_Erinyes_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            SummonCreature_Erinyes_Power.SetHasCastingFailure(false);
            SummonCreature_Erinyes_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.EffectDescription);
            SummonCreature_Erinyes_Power.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName("Custom_Erinyes");

            SummonCreature_Erinyes_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            SummonCreature_Erinyes_Power.SetCostPerUse(1);
            SummonCreature_Erinyes_Power.SetFixedUsesPerRecharge(1);
            SummonCreature_Erinyes_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }

        public static void BuildNewSummonCreature_Nalfeshnee_Power()
        {

            string text = "SummonCreature_Nalfeshnee_Power";

            SummonCreature_Nalfeshnee_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            SummonCreature_Nalfeshnee_Power.SetHasCastingFailure(false);
            SummonCreature_Nalfeshnee_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.EffectDescription);
            SummonCreature_Nalfeshnee_Power.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName("Custom_Nalfeshnee");
            SummonCreature_Nalfeshnee_Power.EffectDescription.EffectForms[0].SummonForm.SetNumber(2);

            SummonCreature_Nalfeshnee_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            SummonCreature_Nalfeshnee_Power.SetCostPerUse(1);
            SummonCreature_Nalfeshnee_Power.SetFixedUsesPerRecharge(2);
            SummonCreature_Nalfeshnee_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }

        public static void BuildNewSummonCreature_Wolves_Power()
        {

            string text = "SummonCreature_Wolves_Power";

            SummonCreature_Wolves_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            SummonCreature_Wolves_Power.SetHasCastingFailure(false);
            SummonCreature_Wolves_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.EffectDescription);
            SummonCreature_Wolves_Power.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName("Wolf");
            SummonCreature_Wolves_Power.EffectDescription.EffectForms[0].SummonForm.SetNumber(3);

            SummonCreature_Wolves_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            SummonCreature_Wolves_Power.SetCostPerUse(1);
            SummonCreature_Wolves_Power.SetFixedUsesPerRecharge(3);
            SummonCreature_Wolves_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }



        public static void BuildNewAirTitan_Gale_Power()
        {

            string text = "AirTitan_Gale_Power";


            AirTitan_Gale_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            AirTitan_Gale_Power.EffectDescription.SetTargetParameter(10);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(4);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D10);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(0);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeThunder);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].SetHasSavingThrow(true);
            AirTitan_Gale_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            AirTitan_Gale_Power.EffectDescription.SetHasSavingThrow(true);
            AirTitan_Gale_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            AirTitan_Gale_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            AirTitan_Gale_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(17);

            MotionForm motionForm = new MotionForm();
            motionForm.SetType(MotionForm.MotionType.FallProne);
            motionForm.SetDistance(6);

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Motion;
            effectForm.SetMotionForm(motionForm);
            effectForm.SetHasSavingThrow(true);
            effectForm.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);


            AirTitan_Gale_Power.EffectDescription.EffectForms.Add(effectForm);

            MotionForm motionForm_2 = new MotionForm();
            motionForm_2.SetType(MotionForm.MotionType.PushFromOrigin);
            motionForm_2.SetDistance(6);

            EffectForm effectForm_2 = new EffectForm();
            effectForm_2.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm_2.SetLevelMultiplier(1);
            effectForm_2.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm_2.SetCreatedByCharacter(true);
            effectForm_2.FormType = EffectForm.EffectFormType.Motion;
            effectForm_2.SetMotionForm(motionForm_2);
            effectForm_2.SetHasSavingThrow(true);
            effectForm_2.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);

            AirTitan_Gale_Power.EffectDescription.EffectForms.Add(effectForm_2);


        }
        public static void BuildNewFireTitan_Aura_Power()
        {

            string text = "FireTitan_Aura_Power";


            FireTitan_Aura_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerArrokAuraOfFire,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            FireTitan_Aura_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            FireTitan_Aura_Power.EffectDescription.SetTargetParameter(10);

            FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(10);
            FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeFire);



        }


        public static void BuildNewAirTitan_Lightning_Power()
        {

            string text = "AirTitan_Lightning_Power";


            AirTitan_Lightning_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            AirTitan_Lightning_Power.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);

            AirTitan_Lightning_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            AirTitan_Lightning_Power.EffectDescription.SetTargetParameter(2);
            AirTitan_Lightning_Power.EffectDescription.SetRangeParameter(100);
            AirTitan_Lightning_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(3);
            AirTitan_Lightning_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D10);
            AirTitan_Lightning_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            AirTitan_Lightning_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            AirTitan_Lightning_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            AirTitan_Lightning_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
        }






        public static void BuildNewAirTitan_LightningStorm_Attack()
        {

            string text = "AirTitan_LightningStorm";
            /*
                        AirTitan_LightningStorm_Attack_Power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>
                               .createPower(
                               "LimitedPerDayAOE_"+text + "DH_Custom_Power",
                               GuidHelper.Create(new System.Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
                               "Feature/&DH_" + text + "_Custom_Power_Title",
                               "Feature/&DH_" + text + "_Custom_Power_Description",
                               DatabaseHelper.SpellDefinitions.LightningBolt.GuiPresentation.SpriteReference,
                               DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.EffectDescription,
                               RuleDefinitions.ActivationTime.Action,
                               1,
                               RuleDefinitions.UsesDetermination.Fixed,
                               RuleDefinitions.RechargeRate.D6_56,
                               "Charisma",
                               "Charisma",
                               1,
                               true
                               );
            */
            AirTitan_LightningStorm_Attack_Power = BuildNewPower(
                    "LimitedPerDayAOE_" + text + "DH_Custom_Power",
                     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
                     GuidHelper.Create(new System.Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
                     "Feature/&DH_" + text + "_Custom_Power_Title",
                     "Feature/&DH_" + text + "_Custom_Power_Description"
                      );


            AirTitan_LightningStorm_Attack_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            AirTitan_LightningStorm_Attack_Power.SetFixedUsesPerRecharge(1);
            AirTitan_LightningStorm_Attack_Power.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            AirTitan_LightningStorm_Attack_Power.SetRechargeRate(RuleDefinitions.RechargeRate.D6_56);
            AirTitan_LightningStorm_Attack_Power.SetUsesAbilityScoreName("Charisma");
            AirTitan_LightningStorm_Attack_Power.SetAbilityScore("Charisma");
            AirTitan_LightningStorm_Attack_Power.SetCostPerUse(1);
            AirTitan_LightningStorm_Attack_Power.SetShowCasting(true);


            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetTargetParameter(24);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetRangeParameter(0);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(6);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D8);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name);
            AirTitan_LightningStorm_Attack_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);

        }

        public static void BuildNewIlluminatingCrystals_Power()
        {

            string text = "IlluminatingCrystals_Power";


            IlluminatingCrystals_Power = BuildNewPower(
                   "AtWillAOE_DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.FaerieFire.EffectDescription);
            IlluminatingCrystals_Power.SetEffectDescription(effectDescription);
            IlluminatingCrystals_Power.EffectDescription.SetTargetExcludeCaster(true);
            IlluminatingCrystals_Power.EffectDescription.SetTargetParameter(6);
            IlluminatingCrystals_Power.EffectDescription.SetRangeParameter(0);
            IlluminatingCrystals_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            IlluminatingCrystals_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.PerceivingWithinDistance);
            IlluminatingCrystals_Power.EffectDescription.SetHasSavingThrow(false);


        }

        public static void BuildNewDisintegratingBeam_Power()
        {

            string text = "DisintegratingBeam_Power";


            DisintegratingBeam_Power = BuildNewPower(
                   "PowerDragonBreath_DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Acid,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.Disintegrate.EffectDescription);
            DisintegratingBeam_Power.SetEffectDescription(effectDescription);
            DisintegratingBeam_Power.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters);

            DisintegratingBeam_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(26);
            DisintegratingBeam_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            DisintegratingBeam_Power.EffectDescription.SetTargetParameter(30);
            DisintegratingBeam_Power.EffectDescription.SetTargetParameter2(2);
            DisintegratingBeam_Power.EffectDescription.SetRangeParameter(30);
            DisintegratingBeam_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            DisintegratingBeam_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Line);
            DisintegratingBeam_Power.EffectDescription.SetHasSavingThrow(true);

            DisintegratingBeam_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(11);
            DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D10);
            DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeRadiant);


        }
        public static void BuildNewIncreasedGravityZone_Attack()
        {

            string text = "IncreasedGravityZone_Attack";


            IncreasedGravityZone_Power = BuildNewPower(
                   "AtWillAOEDH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast.EffectDescription);
            IncreasedGravityZone_Power.SetEffectDescription(effectDescription);
            IncreasedGravityZone_Power.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            IncreasedGravityZone_Power.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Entangle.EffectDescription.EffectParticleParameters);

            IncreasedGravityZone_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(26);
            IncreasedGravityZone_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            IncreasedGravityZone_Power.EffectDescription.SetTargetParameter(4);
            IncreasedGravityZone_Power.EffectDescription.SetTargetParameter2(4);
            IncreasedGravityZone_Power.EffectDescription.SetRangeParameter(4);
            IncreasedGravityZone_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            IncreasedGravityZone_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
            IncreasedGravityZone_Power.EffectDescription.SetHasSavingThrow(true);

            IncreasedGravityZone_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            IncreasedGravityZone_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(6);
            IncreasedGravityZone_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D10);

            MotionForm motionForm = new MotionForm();
            motionForm.SetType(MotionForm.MotionType.FallProne);
            motionForm.SetDistance(6);

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Motion;
            effectForm.SetMotionForm(motionForm);

            IncreasedGravityZone_Power.EffectDescription.EffectForms.Add(effectForm);

            ConditionForm Condition = new ConditionForm();
            Condition.SetApplyToSelf(false);
            Condition.SetForceOnSelf(false);
            Condition.Operation = ConditionForm.ConditionOperation.Add;
            Condition.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionRestrained.Name);
            Condition.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionRestrained;

            EffectForm effect = new EffectForm();
            effect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effect.SetLevelMultiplier(1);
            effect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effect.SetCreatedByCharacter(true);
            effect.FormType = EffectForm.EffectFormType.Condition;
            effect.ConditionForm = Condition;
            effect.SetCanSaveToCancel(true);
            effect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);

            IncreasedGravityZone_Power.EffectDescription.EffectForms.Add(effect);

        }
        public static void BuildNewSummonCreature_LesserConstruct_Power()
        {

            string text = "SummonCreature_LesserConstruct_Power";

            SummonCreature_LesserConstruct_Power = BuildNewPower(
                   text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            SummonCreature_LesserConstruct_Power.SetHasCastingFailure(false);
            SummonCreature_LesserConstruct_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.EffectDescription);
            SummonCreature_LesserConstruct_Power.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName("Magic_Mouth");
            SummonCreature_LesserConstruct_Power.EffectDescription.EffectForms[0].SummonForm.SetNumber(3);

            SummonCreature_LesserConstruct_Power.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            SummonCreature_LesserConstruct_Power.SetCostPerUse(1);
            SummonCreature_LesserConstruct_Power.SetFixedUsesPerRecharge(3);
            SummonCreature_LesserConstruct_Power.SetActivationTime(RuleDefinitions.ActivationTime.Action);

        }

        public static void BuildNewEarthTitan_Earthquake_Power()
        {

            string text = "EarthTitan_Earthquake_Power";


            EarthTitan_Earthquake_Power = BuildNewPower(
                   "DH_Custom_" + text,
                   DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
                   GuidHelper.Create(new System.Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                  "MonsterPower/&DH_" + text + "_Title",
                   "MonsterPower/&DH_" + text + "_Description"
                    );

            EarthTitan_Earthquake_Power.EffectDescription.SetTargetParameter(20);
            EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(4);
            EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D10);
            EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(0);
            EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeBludgeoning);
            EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);

            MotionForm motionForm = new MotionForm();
            motionForm.SetType(MotionForm.MotionType.FallProne);
            motionForm.SetDistance(6);

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Motion;
            effectForm.SetMotionForm(motionForm);
            effectForm.SetHasSavingThrow(true);
            effectForm.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);

            EarthTitan_Earthquake_Power.EffectDescription.EffectForms.Add(effectForm);


        }


        //************************************************************************************************************************************
        //************************************************************************************************************************************


        public static FeatureDefinitionPower BuildNewPower(string name, FeatureDefinitionPower basePower, string guid, string title, string description)
        {
            return FeatureDefinitionPowerBuilder
                .Create(basePower, name, guid)
                .SetOrUpdateGuiPresentation(title, description)
                .AddToDB();
        }
    }
}
