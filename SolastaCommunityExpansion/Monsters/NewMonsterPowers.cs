using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaMonsters.Models;
using UnityEngine;

//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters;

public class NewMonsterPowers
{
    public static FeatureDefinitionPower Generic_Lightning_Attack_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower Balor_FireAura_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower IncreasedGravityZone_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower Lich_DisruptLife_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower FireTitan_Aura_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AirTitan_Lightning_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AirTitan_Gale_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AirTitan_LightningStorm_Attack_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower EarthTitan_Earthquake_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower IlluminatingCrystals_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower DisintegratingBeam_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AtWillSelfBuff_Invisibility_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AtWillAOE_Fireball_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower LimitedPerDayTargetDebuff_HoldMonster_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower LimitedPerDayAOE_WallOfFire_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SummonCreature_Erinyes_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower ErinyesParry_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SummonCreature_Nalfeshnee_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SearingBurst_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower BlindingGaze_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SummonCreature_Elemental_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SummonCreature_Wolves_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower SummonCreature_LesserConstruct_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower AncientDragon_Wing_Power =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower TarrasqueSwallowPower =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static FeatureDefinitionPower VampireCharmPower =
        ScriptableObject.CreateInstance<FeatureDefinitionPower>();

    public static Dictionary<string, FeatureDefinitionPower> DictionaryOfAncientDragonBreaths = new();
    public static Dictionary<string, FeatureDefinitionPower> DictionaryOfGenericBreathsWithExtraDamage = new();
    public static Dictionary<string, FeatureDefinitionPower> Dictionaryof_SummoningElementals = new();


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
        var text = "TarrasqueSwallowPower";


        var TarrasqueSwallowingCondition = NewMonsterAttacks.BuildNewCondition(
            "DH_Custom_TarrasqueSwallowingcondition",
            DatabaseHelper.ConditionDefinitions.ConditionSwallowingRemorhaz,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_TarrasqueSwallowingcondition")
                .ToString(),
            "MonsterAttack/&DH_Custom_TarrasqueSwallowingcondition_Title",
            "MonsterAttack/&DH_Custom_TarrasqueSwallowingcondition_Description"
        );

        TarrasqueSwallowingCondition.interruptionDamageThreshold = 60;
        TarrasqueSwallowingCondition.interruptionRequiresSavingThrow = false;


        var TarrasqueSwallowedCondition = NewMonsterAttacks.BuildNewCondition(
            "DH_Custom_TarrasqueSwallowedcondition",
            DatabaseHelper.ConditionDefinitions.ConditionSwallowedRemorhaz,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_TarrasqueSwallowedcondition")
                .ToString(),
            "MonsterAttack/&DH_Custom_TarrasqueSwallowedcondition_Title",
            "MonsterAttack/&DH_Custom_TarrasqueSwallowedcondition_Description"
        );

        TarrasqueSwallowedCondition.RecurrentEffectForms[0].DamageForm.diceNumber = 16;


        TarrasqueSwallowPower = BuildNewPower(
            text + "DH_Custom",
            DatabaseHelper.FeatureDefinitionPowers.PowerRemorhazSwallow,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        TarrasqueSwallowPower.EffectDescription.SetTargetConditionAsset(NewMonsterAttacks
            .TarrasqueGrappledRestrainedCondition);

        TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
        TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D12;
        TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm.bonusDamage = 10;
        TarrasqueSwallowPower.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypePiercing;

        TarrasqueSwallowPower.EffectDescription.EffectForms[2].ConditionForm
            .conditionDefinition = TarrasqueSwallowedCondition;

        TarrasqueSwallowPower.EffectDescription.EffectForms[3].ConditionForm
            .conditionDefinition = TarrasqueSwallowingCondition;

        TarrasqueSwallowPower.EffectDescription.EffectForms.RemoveAt(1);
    }

    public static void BuildNewErinyesParry_Power()
    {
        var text = "ErinyesParry_Power";


        ErinyesParry_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerFeatTwinBlade,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        ErinyesParry_Power.reactionContext = RuleDefinitions.ReactionTriggerContext.HitByMelee;
        ErinyesParry_Power.EffectDescription.EffectForms[0].ConditionForm.applyToSelf = true;
        ErinyesParry_Power.EffectDescription.EffectForms[0].ConditionForm.forceOnSelf = true;
    }

    public static void BuildNewVampireCharmPower()
    {
        var text = "VampireCharmPower";


        VampireCharmPower = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerLaetharParalyzingGaze,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        EffectDescription effectDescription = new();
        effectDescription.Copy(DatabaseHelper.SpellDefinitions.HypnoticPattern.EffectDescription);
        effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        effectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        effectDescription.SetTargetParameter(1);
        effectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation
            .FixedValue);
        effectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Wisdom.Name);
        effectDescription.SetFixedSavingThrowDifficultyClass(17);
        VampireCharmPower.effectDescription = effectDescription;
    }

    public static void BuildNewGeneric_Lightning_Attack()
    {
        // for storm giant
        var text = "Generic_Lightning_Attack";

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
                           AttributeDefinitions.Charisma,
                           AttributeDefinitions.Charisma,
                           1,
                           true
                           );
                    */

        Generic_Lightning_Attack_Power = BuildNewPower(
            text + "DH_Custom_Power",
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
            GuidHelper.Create(new Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
            "Feature/&DH_" + text + "_Custom_Power_Title",
            "Feature/&DH_" + text + "_Custom_Power_Description"
        );


        Generic_Lightning_Attack_Power.activationTime = RuleDefinitions.ActivationTime.Action;
        Generic_Lightning_Attack_Power.fixedUsesPerRecharge = 1;
        Generic_Lightning_Attack_Power.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        Generic_Lightning_Attack_Power.rechargeRate = RuleDefinitions.RechargeRate.D6_56;
        Generic_Lightning_Attack_Power.usesAbilityScoreName = AttributeDefinitions.Charisma;
        Generic_Lightning_Attack_Power.abilityScore = AttributeDefinitions.Charisma;
        Generic_Lightning_Attack_Power.costPerUse = 1;
        Generic_Lightning_Attack_Power.showCasting = true;


        Generic_Lightning_Attack_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
        Generic_Lightning_Attack_Power.EffectDescription.SetTargetParameter(3);
        Generic_Lightning_Attack_Power.EffectDescription.SetTargetParameter(8);
        Generic_Lightning_Attack_Power.EffectDescription.SetRangeParameter(100);
        Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 12;
        Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D8;

        Generic_Lightning_Attack_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        Generic_Lightning_Attack_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        Generic_Lightning_Attack_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        Generic_Lightning_Attack_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(17);
    }


    public static void BuildNew_AncientDragon_Breath_Power()
    {
        Dictionary<string, int> dictionaryofAncientDragonBreathExtraDamageDiceNumbers = new()
        {
            {"Ancient Black Dragon", 15},
            {"Ancient Blue Dragon", 16},
            {"Ancient Green Dragon", 22},
            {"Ancient Red Dragon", 26},
            {"Ancient White Dragon", 16}
        };

        Dictionary<string, RuleDefinitions.DieType> dictionaryofAncientDragonBreathExtraDamageDiceType = new()
        {
            {"Ancient Black Dragon", RuleDefinitions.DieType.D8},
            {"Ancient Blue Dragon", RuleDefinitions.DieType.D10},
            {"Ancient Green Dragon", RuleDefinitions.DieType.D6},
            {"Ancient Red Dragon", RuleDefinitions.DieType.D6},
            {"Ancient White Dragon", RuleDefinitions.DieType.D8}
        };

        Dictionary<string, RuleDefinitions.TargetType> dictionaryofAncientDragonBreathShape = new()
        {
            {"Ancient Black Dragon", RuleDefinitions.TargetType.Line},
            {"Ancient Blue Dragon", RuleDefinitions.TargetType.Line},
            {"Ancient Green Dragon", RuleDefinitions.TargetType.Cone},
            {"Ancient Red Dragon", RuleDefinitions.TargetType.Cone},
            {"Ancient White Dragon", RuleDefinitions.TargetType.Cone}
        };

        Dictionary<string, EffectParticleParameters> dictionaryofAncientDragonBreathEffectparticles = new()
        {
            {
                "Ancient Black Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Acid
                    .EffectDescription
                    .EffectParticleParameters
            },
            {
                "Ancient Blue Dragon",
                DatabaseHelper.SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters
            },
            {
                "Ancient Green Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Poison
                    .EffectDescription
                    .EffectParticleParameters
            },
            {
                "Ancient Red Dragon", DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Fire
                    .EffectDescription
                    .EffectParticleParameters
            },
            {
                "Ancient White Dragon",
                DatabaseHelper.SpellDefinitions.ConeOfCold.EffectDescription.EffectParticleParameters
            }
        };


        foreach (var entry in NewMonsterAttributes.Dictionaryof_Dragon_DamageAffinity)
        {
            var text = entry.Value;
            text = text.Replace(" ", "");

            var Dragon_Breath_Power = BuildNewPower(
                "PowerDragonBreath_DH_Custom_" + text,
                DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Fire,
                GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                "MonsterPower/&DH_" + text + "_Breath_Power" + "_Title",
                "MonsterPower/&DH_" + text + "_Breath_Power" + "_Description"
            );

            Dragon_Breath_Power.EffectDescription.SetTargetType(dictionaryofAncientDragonBreathShape[entry.Key]);
            Dragon_Breath_Power.EffectDescription.SetTargetParameter(20);
            // generic ancient dragon Breath Power
            Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm
                .diceNumber = dictionaryofAncientDragonBreathExtraDamageDiceNumbers[entry.Key];
            Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm
                .dieType = dictionaryofAncientDragonBreathExtraDamageDiceType[entry.Key];
            Dragon_Breath_Power.EffectDescription.EffectForms[0].DamageForm
                .damageType = entry.Value; // ListofDamageTypes_Dragon[i]);
            Dragon_Breath_Power.EffectDescription.SetEffectParticleParameters(
                dictionaryofAncientDragonBreathEffectparticles[entry.Key]);
            Dragon_Breath_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(23);

            DictionaryOfAncientDragonBreaths.Add(entry.Key, Dragon_Breath_Power);


            DictionaryOfGenericBreathsWithExtraDamage.Add(entry.Value, Dragon_Breath_Power);
        }
    }


    public static void BuildNewBalor_FireAura_Power()
    {
        var text = "Balor_FireAura_Power";


        Balor_FireAura_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerArrokAuraOfFire,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        Balor_FireAura_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        Balor_FireAura_Power.EffectDescription.SetTargetParameter(4);

        Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D6;
        Balor_FireAura_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeFire;
    }

    public static void BuildNewLich_DisruptLife_Power()
    {
        var text = "Lich_DisruptLife_Power";


        Lich_DisruptLife_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        Lich_DisruptLife_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        Lich_DisruptLife_Power.EffectDescription.SetTargetParameter(4);
        Lich_DisruptLife_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(18);
        Lich_DisruptLife_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Constitution.Name);
        Lich_DisruptLife_Power.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper
            .SmartAttributeDefinitions.Constitution.Name);


        Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 6;
        Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D6;
        Lich_DisruptLife_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeNecrotic;
    }

    public static void BuildNewAncientDragon_Wing_Power()
    {
        var text = "AncientDragon_Wing_Power";


        AncientDragon_Wing_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        AncientDragon_Wing_Power.EffectDescription.EffectForms[0].DamageForm.bonusDamage = 9;
        AncientDragon_Wing_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(24);
    }

    public static void BuildNewSummonCreature_Elemental_Power()
    {
        Dictionary<SpellDefinition, int> dictionaryelementals = new()
        {
            {DatabaseHelper.SpellDefinitions.ConjureElementalAir, 1},
            {DatabaseHelper.SpellDefinitions.ConjureElementalEarth, 1},
            {DatabaseHelper.SpellDefinitions.ConjureElementalFire, 1},
            {DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour, 4},
            {DatabaseHelper.SpellDefinitions.ConjureMinorElementalsOne, 1},
            {DatabaseHelper.SpellDefinitions.ConjureMinorElementalsTwo, 2}
        };


        foreach (var entry in dictionaryelementals)

        {
            var text = "SummonCreature_" +
                       entry.Key.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName + "_Power";

            SummonCreature_Elemental_Power = BuildNewPower(
                text,
                DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
                GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
                "MonsterPower/&DH_" + text + "_Title",
                "MonsterPower/&DH_" + text + "_Description"
            );

            SummonCreature_Elemental_Power.hasCastingFailure = false;
            SummonCreature_Elemental_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids
                .EffectDescription);
            SummonCreature_Elemental_Power.EffectDescription.EffectForms[0].SummonForm
                .monsterDefinitionName = entry.Key.EffectDescription.EffectForms[0].SummonForm
                .MonsterDefinitionName;
            SummonCreature_Elemental_Power.EffectDescription.EffectForms[0].SummonForm.number = entry.Value;

            SummonCreature_Elemental_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
            SummonCreature_Elemental_Power.activationTime = RuleDefinitions.ActivationTime.Action;

            Dictionaryof_SummoningElementals.Add(
                entry.Key.EffectDescription.EffectForms[0].SummonForm.MonsterDefinitionName,
                SummonCreature_Elemental_Power);
        }
    }


    public static void BuildNewSearingBurst_Power()
    {
        var text = "SearingBurst_Power";


        SearingBurst_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        SearingBurst_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        SearingBurst_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        SearingBurst_Power.EffectDescription.SetTargetParameter(4);

        SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
        SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D6;
        SearingBurst_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeFire;
        SearingBurst_Power.EffectDescription.EffectForms[0].hasSavingThrow = true;
        SearingBurst_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;

        DamageForm damageForm = new();
        damageForm.diceNumber = 4;
        damageForm.dieType = RuleDefinitions.DieType.D6;
        damageForm.bonusDamage = 0;
        damageForm.damageType = RuleDefinitions.DamageTypeRadiant;


        EffectForm extraDamageEffect = new();
        extraDamageEffect.applyLevel = EffectForm.LevelApplianceType.No;
        extraDamageEffect.levelMultiplier = 1;
        extraDamageEffect.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        extraDamageEffect.createdByCharacter = true;
        extraDamageEffect.FormType = EffectForm.EffectFormType.Damage;
        extraDamageEffect.damageForm = damageForm;
        extraDamageEffect.hasSavingThrow = true;
        extraDamageEffect.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;

        SearingBurst_Power.EffectDescription.EffectForms.Add(extraDamageEffect);
        SearingBurst_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        SearingBurst_Power.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        SearingBurst_Power.EffectDescription.hasSavingThrow = true;
        SearingBurst_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(23);
    }

    public static void BuildNewBlindingGaze_Power()
    {
        var text = "BlindingGaze_Power";


        BlindingGaze_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerLaetharParalyzingGaze,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        BlindingGaze_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        BlindingGaze_Power.EffectDescription.EffectForms[0].canSaveToCancel = false;

        BlindingGaze_Power.EffectDescription.EffectForms[0].ConditionForm
            .conditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionBlinded;
    }

    public static void BuildNewAtWillSelfBuff_Invisibility_Power()
    {
        var text = "AtWillSelfBuff_Invisibility_Power";


        AtWillSelfBuff_Invisibility_Power = BuildNewPower(
            text + "_DH_Custom",
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        AtWillSelfBuff_Invisibility_Power.guiPresentation = DatabaseHelper.SpellDefinitions.Invisibility
            .GuiPresentation;
        AtWillSelfBuff_Invisibility_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Invisibility
            .EffectDescription);
        AtWillSelfBuff_Invisibility_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        AtWillSelfBuff_Invisibility_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }

    public static void BuildNewAtWillAOE_Fireball_Power()
    {
        var text = "AtWillAOE_Fireball_Power";


        AtWillAOE_Fireball_Power = BuildNewPower(
            text + "_DH_Custom",
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );
        AtWillAOE_Fireball_Power.guiPresentation = DatabaseHelper.SpellDefinitions.Fireball.GuiPresentation;
        AtWillAOE_Fireball_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
        AtWillAOE_Fireball_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        AtWillAOE_Fireball_Power.activationTime = RuleDefinitions.ActivationTime.Action;

        AtWillAOE_Fireball_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        AtWillAOE_Fireball_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        AtWillAOE_Fireball_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        AtWillAOE_Fireball_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
    }

    public static void BuildNewLimitedPerDayTargetDebuff_HoldMonster_Power()
    {
        var text = "LimitedPerDayTargetDebuff_HoldMonster_Power";


        LimitedPerDayTargetDebuff_HoldMonster_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawAnathema,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        LimitedPerDayTargetDebuff_HoldMonster_Power.uniqueInstance = true;
        LimitedPerDayTargetDebuff_HoldMonster_Power.guiPresentation = DatabaseHelper.SpellDefinitions.HoldMonster
            .GuiPresentation;
        LimitedPerDayTargetDebuff_HoldMonster_Power.shortTitleOverride = DatabaseHelper.SpellDefinitions
            .HoldMonster.GuiPresentation.Title;
        LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions
            .HoldMonster.EffectDescription);
        LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        LimitedPerDayTargetDebuff_HoldMonster_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(21);

        LimitedPerDayTargetDebuff_HoldMonster_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        LimitedPerDayTargetDebuff_HoldMonster_Power.costPerUse = 1;
        LimitedPerDayTargetDebuff_HoldMonster_Power.fixedUsesPerRecharge = 3;
        LimitedPerDayTargetDebuff_HoldMonster_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }


    public static void BuildNewLimitedPerDayAOE_WallOfFire_Power()
    {
        var text = "LimitedPerDayAOE_WallOfFire_Power";

        LimitedPerDayAOE_WallOfFire_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        LimitedPerDayAOE_WallOfFire_Power.uniqueInstance = false;
        LimitedPerDayAOE_WallOfFire_Power.guiPresentation = DatabaseHelper.SpellDefinitions.WallOfFire
            .GuiPresentation;
        LimitedPerDayAOE_WallOfFire_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions
            .WallOfFireRing_Outer.EffectDescription);
        LimitedPerDayAOE_WallOfFire_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        LimitedPerDayAOE_WallOfFire_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(21);


        LimitedPerDayAOE_WallOfFire_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        LimitedPerDayAOE_WallOfFire_Power.costPerUse = 1;
        LimitedPerDayAOE_WallOfFire_Power.fixedUsesPerRecharge = 3;
        LimitedPerDayAOE_WallOfFire_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }

    public static void BuildNewSummonCreature_Erinyes_Power()
    {
        var text = "SummonCreature_Erinyes_Power";

        SummonCreature_Erinyes_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        SummonCreature_Erinyes_Power.hasCastingFailure = false;
        SummonCreature_Erinyes_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids
            .EffectDescription);
        SummonCreature_Erinyes_Power.EffectDescription.EffectForms[0].SummonForm
            .monsterDefinitionName = "Custom_Erinyes";

        SummonCreature_Erinyes_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        SummonCreature_Erinyes_Power.costPerUse = 1;
        SummonCreature_Erinyes_Power.fixedUsesPerRecharge = 1;
        SummonCreature_Erinyes_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }

    public static void BuildNewSummonCreature_Nalfeshnee_Power()
    {
        var text = "SummonCreature_Nalfeshnee_Power";

        SummonCreature_Nalfeshnee_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        SummonCreature_Nalfeshnee_Power.hasCastingFailure = false;
        SummonCreature_Nalfeshnee_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids
            .EffectDescription);
        SummonCreature_Nalfeshnee_Power.EffectDescription.EffectForms[0].SummonForm
            .monsterDefinitionName = "Custom_Nalfeshnee";
        SummonCreature_Nalfeshnee_Power.EffectDescription.EffectForms[0].SummonForm.number = 2;

        SummonCreature_Nalfeshnee_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        SummonCreature_Nalfeshnee_Power.costPerUse = 1;
        SummonCreature_Nalfeshnee_Power.fixedUsesPerRecharge = 2;
        SummonCreature_Nalfeshnee_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }

    public static void BuildNewSummonCreature_Wolves_Power()
    {
        var text = "SummonCreature_Wolves_Power";

        SummonCreature_Wolves_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        SummonCreature_Wolves_Power.hasCastingFailure = false;
        SummonCreature_Wolves_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureGoblinoids
            .EffectDescription);
        SummonCreature_Wolves_Power.EffectDescription.EffectForms[0].SummonForm.monsterDefinitionName = "Wolf";
        SummonCreature_Wolves_Power.EffectDescription.EffectForms[0].SummonForm.number = 3;

        SummonCreature_Wolves_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        SummonCreature_Wolves_Power.costPerUse = 1;
        SummonCreature_Wolves_Power.fixedUsesPerRecharge = 3;
        SummonCreature_Wolves_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }


    public static void BuildNewAirTitan_Gale_Power()
    {
        var text = "AirTitan_Gale_Power";


        AirTitan_Gale_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        AirTitan_Gale_Power.EffectDescription.SetTargetParameter(10);
        AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
        AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D10;
        AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm.bonusDamage = 0;
        AirTitan_Gale_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeThunder;
        AirTitan_Gale_Power.EffectDescription.EffectForms[0].hasSavingThrow = true;
        AirTitan_Gale_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
        AirTitan_Gale_Power.EffectDescription.hasSavingThrow = true;
        AirTitan_Gale_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        AirTitan_Gale_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        AirTitan_Gale_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(17);

        MotionForm motionForm = new();
        motionForm.type = MotionForm.MotionType.FallProne;
        motionForm.distance = 6;

        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Motion;
        effectForm.motionForm = motionForm;
        effectForm.hasSavingThrow = true;
        effectForm.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;


        AirTitan_Gale_Power.EffectDescription.EffectForms.Add(effectForm);

        MotionForm motionForm_2 = new();
        motionForm_2.type = MotionForm.MotionType.PushFromOrigin;
        motionForm_2.distance = 6;

        EffectForm effectForm_2 = new();
        effectForm_2.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm_2.levelMultiplier = 1;
        effectForm_2.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm_2.createdByCharacter = true;
        effectForm_2.FormType = EffectForm.EffectFormType.Motion;
        effectForm_2.motionForm = motionForm_2;
        effectForm_2.hasSavingThrow = true;
        effectForm_2.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

        AirTitan_Gale_Power.EffectDescription.EffectForms.Add(effectForm_2);
    }

    public static void BuildNewFireTitan_Aura_Power()
    {
        var text = "FireTitan_Aura_Power";


        FireTitan_Aura_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerArrokAuraOfFire,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        FireTitan_Aura_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        FireTitan_Aura_Power.EffectDescription.SetTargetParameter(10);

        FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 10;
        FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D6;
        FireTitan_Aura_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeFire;
    }


    public static void BuildNewAirTitan_Lightning_Power()
    {
        var text = "AirTitan_Lightning_Power";


        AirTitan_Lightning_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        AirTitan_Lightning_Power.rechargeRate = RuleDefinitions.RechargeRate.AtWill;

        AirTitan_Lightning_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
        AirTitan_Lightning_Power.EffectDescription.SetTargetParameter(2);
        AirTitan_Lightning_Power.EffectDescription.SetRangeParameter(100);
        AirTitan_Lightning_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        AirTitan_Lightning_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D10;
        AirTitan_Lightning_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        AirTitan_Lightning_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        AirTitan_Lightning_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions
            .Dexterity.Name);
        AirTitan_Lightning_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
    }


    public static void BuildNewAirTitan_LightningStorm_Attack()
    {
        var text = "AirTitan_LightningStorm";
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
                           AttributeDefinitions.Charisma,
                           AttributeDefinitions.Charisma,
                           1,
                           true
                           );
        */
        AirTitan_LightningStorm_Attack_Power = BuildNewPower(
            "LimitedPerDayAOE_" + text + "DH_Custom_Power",
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade,
            GuidHelper.Create(new Guid(MonsterContext.GUID), text + "DH_Custom_Power").ToString(),
            "Feature/&DH_" + text + "_Custom_Power_Title",
            "Feature/&DH_" + text + "_Custom_Power_Description"
        );


        AirTitan_LightningStorm_Attack_Power.activationTime = RuleDefinitions.ActivationTime.Action;
        AirTitan_LightningStorm_Attack_Power.fixedUsesPerRecharge = 1;
        AirTitan_LightningStorm_Attack_Power.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        AirTitan_LightningStorm_Attack_Power.rechargeRate = RuleDefinitions.RechargeRate.D6_56;
        AirTitan_LightningStorm_Attack_Power.usesAbilityScoreName = AttributeDefinitions.Charisma;
        AirTitan_LightningStorm_Attack_Power.abilityScore = AttributeDefinitions.Charisma;
        AirTitan_LightningStorm_Attack_Power.costPerUse = 1;
        AirTitan_LightningStorm_Attack_Power.showCasting = true;


        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetTargetParameter(24);
        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetRangeParameter(0);
        AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 6;
        AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D8;
        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        AirTitan_LightningStorm_Attack_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetSavingThrowAbility(DatabaseHelper
            .SmartAttributeDefinitions.Dexterity.Name);
        AirTitan_LightningStorm_Attack_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(20);
    }

    public static void BuildNewIlluminatingCrystals_Power()
    {
        var text = "IlluminatingCrystals_Power";


        IlluminatingCrystals_Power = BuildNewPower(
            "AtWillAOE_DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        EffectDescription effectDescription = new();
        effectDescription.Copy(DatabaseHelper.SpellDefinitions.FaerieFire.EffectDescription);
        IlluminatingCrystals_Power.effectDescription = effectDescription;
        IlluminatingCrystals_Power.EffectDescription.SetTargetExcludeCaster(true);
        IlluminatingCrystals_Power.EffectDescription.SetTargetParameter(6);
        IlluminatingCrystals_Power.EffectDescription.SetRangeParameter(0);
        IlluminatingCrystals_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        IlluminatingCrystals_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType
            .PerceivingWithinDistance);
        IlluminatingCrystals_Power.EffectDescription.hasSavingThrow = false;
    }

    public static void BuildNewDisintegratingBeam_Power()
    {
        var text = "DisintegratingBeam_Power";


        DisintegratingBeam_Power = BuildNewPower(
            "PowerDragonBreath_DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Acid,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        EffectDescription effectDescription = new();
        effectDescription.Copy(DatabaseHelper.SpellDefinitions.Disintegrate.EffectDescription);
        DisintegratingBeam_Power.effectDescription = effectDescription;
        DisintegratingBeam_Power.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions
            .LightningBolt.EffectDescription.EffectParticleParameters);

        DisintegratingBeam_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(26);
        DisintegratingBeam_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        DisintegratingBeam_Power.EffectDescription.SetTargetParameter(30);
        DisintegratingBeam_Power.EffectDescription.targetParameter2 = 2;
        DisintegratingBeam_Power.EffectDescription.SetRangeParameter(30);
        DisintegratingBeam_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        DisintegratingBeam_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Line);
        DisintegratingBeam_Power.EffectDescription.hasSavingThrow = true;

        DisintegratingBeam_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 11;
        DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D10;
        DisintegratingBeam_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeRadiant;
    }

    public static void BuildNewIncreasedGravityZone_Attack()
    {
        var text = "IncreasedGravityZone_Attack";


        IncreasedGravityZone_Power = BuildNewPower(
            "AtWillAOEDH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        EffectDescription effectDescription = new();
        effectDescription.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast.EffectDescription);
        IncreasedGravityZone_Power.effectDescription = effectDescription;
        IncreasedGravityZone_Power.activationTime = RuleDefinitions.ActivationTime.BonusAction;
        IncreasedGravityZone_Power.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions
            .Entangle.EffectDescription.EffectParticleParameters);

        IncreasedGravityZone_Power.EffectDescription.SetFixedSavingThrowDifficultyClass(26);
        IncreasedGravityZone_Power.EffectDescription.SetDifficultyClassComputation(RuleDefinitions
            .EffectDifficultyClassComputation.FixedValue);
        IncreasedGravityZone_Power.EffectDescription.SetTargetParameter(4);
        IncreasedGravityZone_Power.EffectDescription.SetTargetParameter2(4);
        IncreasedGravityZone_Power.EffectDescription.SetRangeParameter(4);
        IncreasedGravityZone_Power.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        IncreasedGravityZone_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cylinder);
        IncreasedGravityZone_Power.EffectDescription.hasSavingThrow = true;

        IncreasedGravityZone_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
        IncreasedGravityZone_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 6;
        IncreasedGravityZone_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D10;

        MotionForm motionForm = new();
        motionForm.type = MotionForm.MotionType.FallProne;
        motionForm.distance = 6;

        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Motion;
        effectForm.motionForm = motionForm;

        IncreasedGravityZone_Power.EffectDescription.EffectForms.Add(effectForm);

        ConditionForm Condition = new();
        Condition.applyToSelf = false;
        Condition.forceOnSelf = false;
        Condition.Operation = ConditionForm.ConditionOperation.Add;
        Condition.conditionDefinitionName = DatabaseHelper.ConditionDefinitions.ConditionRestrained.Name;
        Condition.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionRestrained;

        EffectForm effect = new();
        effect.applyLevel = EffectForm.LevelApplianceType.No;
        effect.levelMultiplier = 1;
        effect.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effect.createdByCharacter = true;
        effect.FormType = EffectForm.EffectFormType.Condition;
        effect.ConditionForm = Condition;
        effect.canSaveToCancel = true;
        effect.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

        IncreasedGravityZone_Power.EffectDescription.EffectForms.Add(effect);
    }

    public static void BuildNewSummonCreature_LesserConstruct_Power()
    {
        var text = "SummonCreature_LesserConstruct_Power";

        SummonCreature_LesserConstruct_Power = BuildNewPower(
            text,
            DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionPaladin,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        SummonCreature_LesserConstruct_Power.hasCastingFailure = false;
        SummonCreature_LesserConstruct_Power.EffectDescription.Copy(DatabaseHelper.SpellDefinitions
            .ConjureGoblinoids.EffectDescription);
        SummonCreature_LesserConstruct_Power.EffectDescription.EffectForms[0].SummonForm
            .monsterDefinitionName = "Magic_Mouth";
        SummonCreature_LesserConstruct_Power.EffectDescription.EffectForms[0].SummonForm.number = 3;

        SummonCreature_LesserConstruct_Power.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        SummonCreature_LesserConstruct_Power.costPerUse = 1;
        SummonCreature_LesserConstruct_Power.fixedUsesPerRecharge = 3;
        SummonCreature_LesserConstruct_Power.activationTime = RuleDefinitions.ActivationTime.Action;
    }

    public static void BuildNewEarthTitan_Earthquake_Power()
    {
        var text = "EarthTitan_Earthquake_Power";


        EarthTitan_Earthquake_Power = BuildNewPower(
            "DH_Custom_" + text,
            DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack,
            GuidHelper.Create(new Guid(MonsterContext.GUID), "DH_Custom_" + text).ToString(),
            "MonsterPower/&DH_" + text + "_Title",
            "MonsterPower/&DH_" + text + "_Description"
        );

        EarthTitan_Earthquake_Power.EffectDescription.SetTargetParameter(20);
        EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
        EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm
            .dieType = RuleDefinitions.DieType.D10;
        EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm.bonusDamage = 0;
        EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0].DamageForm
            .damageType = RuleDefinitions.DamageTypeBludgeoning;
        EarthTitan_Earthquake_Power.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;

        MotionForm motionForm = new();
        motionForm.type = MotionForm.MotionType.FallProne;
        motionForm.distance = 6;

        EffectForm effectForm = new();
        effectForm.applyLevel = EffectForm.LevelApplianceType.No;
        effectForm.levelMultiplier = 1;
        effectForm.levelType = RuleDefinitions.LevelSourceType.ClassLevel;
        effectForm.createdByCharacter = true;
        effectForm.FormType = EffectForm.EffectFormType.Motion;
        effectForm.motionForm = motionForm;
        effectForm.hasSavingThrow = true;
        effectForm.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

        EarthTitan_Earthquake_Power.EffectDescription.EffectForms.Add(effectForm);
    }


    //************************************************************************************************************************************
    //************************************************************************************************************************************


    public static FeatureDefinitionPower BuildNewPower(string name, FeatureDefinitionPower basePower, string guid,
        string title, string description)
    {
        return FeatureDefinitionPowerBuilder
            .Create(basePower, name, guid)
            .SetOrUpdateGuiPresentation(title, description)
            .AddToDB();
    }
}
