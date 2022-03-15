using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine.AddressableAssets;
using UnityEngine;

//******************************************************************************************
//  DO NOT REFACTOR OR CHANGE WITHOUT TESTING OR TAKING RESPOSBILITY FOR CODE GOING FORWARD
//******************************************************************************************


namespace SolastaCommunityExpansion.Classes.Warlock.Features
{

    internal class WarlockPactOfTheChainSummons
    {
        public static FeatureDefinitionPower PactofChainFamiliarInvisibilityPower;
        public static FeatureDefinitionPower PactofChainFamiliarSpellResistencePower;
        public static MonsterDefinition PactChainPseudodragon;
        public static MonsterDefinition PactChainSprite;
        public static MonsterDefinition PactChainImp;
        public static MonsterDefinition PactChainQuasit;

        public static void buildPactofChainFamiliarInvisibilityPower()
        {
            SpellDefinition invisibilty = DatabaseHelper.SpellDefinitions.Invisibility;
            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(invisibilty.EffectDescription);

            PactofChainFamiliarInvisibilityPower = FeatureDefinitionPowerBuilder
                .Create("PactofChainFamiliarInvisibilityPower", GuidHelper.Create(new Guid(Settings.GUID), "PactofChainFamiliarInvisibilityPower").ToString())
                .SetGuiPresentation(invisibilty.GuiPresentation)
                .Configure(
                   1,
                   RuleDefinitions.UsesDetermination.Fixed,
                   AttributeDefinitions.Charisma,
                   RuleDefinitions.ActivationTime.Action,
                   1,
                   RuleDefinitions.RechargeRate.AtWill,
                   false,
                   false,
                   AttributeDefinitions.Charisma,
                   effectDescription,
                   true)
               .AddToDB();

        }

        public static void PactofChainFamiliarAuraOfSpellResistence()
        {

            var guiPresentationSpellResistenceCondition = new GuiPresentationBuilder(
                    "SpellResistenceDesccription",
                    "SpellResistenceConditionTitle")
                    .Build();

            ConditionDefinition spellResistanceCondition = new Tinkerer.FeatureHelpers.ConditionDefinitionBuilder(
                "DHSpellResistenceCondition", GuidHelper.Create(new Guid(Settings.GUID), "DHSpellResistenceCondition").ToString(),
                RuleDefinitions.DurationType.Minute,
                1,
                false,
                guiPresentationSpellResistenceCondition,
                DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance
                ).AddToDB();


            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Sphere, 2, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(spellResistanceCondition, ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>()).Build());


            PactofChainFamiliarSpellResistencePower = FeatureDefinitionPowerBuilder
                .Create(
                "PactofChainFamiliarSpellResistencePower",
                GuidHelper.Create(new Guid(Settings.GUID), "PactofChainFamiliarSpellResistencePower").ToString())
                .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.Permanent,
                1,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Charisma,
                effectDescription.Build(),
                true)
                .SetGuiPresentation(new GuiPresentationBuilder("FamiliarSpellResistenceDescription", "FamiliarSpellResistenceTitle").Build())
                .AddToDB();
        }

        public static void buildCustomPseudodragon()
        {


            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.SilverDragon_Princess;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SilverDragon_Princess;

            string NewName = "CustomPseudodragon";
            string NewTitle = "CustomPseudodragonTitle";
            string NewDescription = "CustomPseudodragonDescription";
            CharacterSizeDefinition Size = DatabaseHelper.CharacterSizeDefinitions.Tiny;
            string Alignment = DatabaseHelper.AlignmentDefinitions.NeutralGood.name;
            int ArmorClass = 13;
            int HitDice = 2;
            RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D4;
            int HitPointsBonus = 4;
            int StandardHitPoints = 7;
            int AttributeStrength = 6;
            int AttributeDexterity = 15;
            int AttributeConstitution = 13;
            int AttributeIntelligence = 10;
            int AttributeWisdom = 12;
            int AttributeCharisma = 10;
            int SavingThrowStrength = 0;
            int SavingThrowDexterity = 0;
            int SavingThrowConstitution = 0;
            int SavingThrowIntelligence = 0;
            int SavingThrowWisdom = 0;
            int SavingThrowCharisma = 0;
            float CR = 0.25f;
            bool LegendaryCreature = false;
            string Type = DatabaseHelper.CharacterFamilyDefinitions.Dragon.name;

            List<FeatureDefinition> Features = new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,

                DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
               // DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance
              PactofChainFamiliarSpellResistencePower
                //  castSpellPseudodragon
            };

            MonsterSkillProficiency skillProficiency_1 = new MonsterSkillProficiency();
            skillProficiency_1.SetField("skillName", "Perception");
            skillProficiency_1.SetField("bonus", 3);

            MonsterSkillProficiency skillProficiency_2 = new MonsterSkillProficiency();
            skillProficiency_2.SetField("skillName", "Stealth");
            skillProficiency_2.SetField("bonus", 4);

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>() { skillProficiency_1, skillProficiency_2 };

            /* waiting until MonsterAttackDefinitionBuilder is available to use
             * need to update attackIterations
             * test before commiting
             //           MonsterAttackDefinition PseudodragonAttack = MonsterAttackDefinitionBuilder(
             //                    DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
             //                    "DH_Custom_" + text,
             //                    GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "PseudodragonAttack").ToString()
             //                     );
             //                    PseudodragonAttack.GuiPresentation.SetTitle       ("MonsterAttack/&DH_PseudodragonAttack_Title"      );
             //                    PseudodragonAttack.GuiPresentation.SetDescription("MonsterAttack/&DH_PseudodragonAttack_Description");

             //           PseudodragonAttack.SetToHitBonus(7);
             //           PseudodragonAttack.EffectDescription.SetRangeParameter(1);
             //           PseudodragonAttack.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
             //           PseudodragonAttack.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
             //           PseudodragonAttack.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(5);
             //           PseudodragonAttack.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);
             //
             //           ConditionForm conditionForm = new ConditionForm();
             //           conditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
             //           conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionPoisoned.name);
             //           conditionForm.SetOperation(ConditionForm.ConditionOperation.Add);
            //
             //           EffectForm extraPoisonEffect = new EffectForm();
             //           extraPoisonEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
             //           extraPoisonEffect.SetLevelMultiplier(1);
             //           extraPoisonEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
             //           extraPoisonEffect.SetCreatedByCharacter(true);
             //           extraPoisonEffect.FormType = EffectForm.EffectFormType.Condition;
             //           extraPoisonEffect.SetConditionForm(conditionForm);
             //           extraPoisonEffect.SetHasSavingThrow(true);
             //           extraPoisonEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
             //
             //           ConditionForm sleepForm = new ConditionForm();
             //           sleepForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionMagicallyAsleep);
             //           sleepForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionAsleep.name);
             //           sleepForm.SetOperation(ConditionForm.ConditionOperation.Add);
             //
             //           EffectForm extraSleepEffect = new EffectForm();
             //           extraSleepEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
             //           extraSleepEffect.SetLevelMultiplier(1);
             //           extraSleepEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
             //           extraSleepEffect.SetCreatedByCharacter(true);
             //           extraSleepEffect.FormType = EffectForm.EffectFormType.Condition;
             //           extraSleepEffect.SetConditionForm(sleepForm);
             //           extraSleepEffect.SetHasSavingThrow(true);
             //           extraSleepEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
             //

             //           PseudodragonAttack.EffectDescription.EffectForms.Add(extraSleepEffect);
             //           PseudodragonAttack.EffectDescription.EffectForms.Add(extraPoisonEffect);
             //           PseudodragonAttack.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
             //           PseudodragonAttack.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
             //           PseudodragonAttack.EffectDescription.SetHasSavingThrow(true);
             //           PseudodragonAttack.EffectDescription.SetFixedSavingThrowDifficultyClass(11);
             //           PseudodragonAttack.EffectDescription.SetDurationParameter(1);
             //           PseudodragonAttack.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);
                        */

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>
            {
                DatabaseHelper.MonsterDefinitions.Flying_Snake.AttackIterations[0]
                // PseudodragonAttack
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            bool GroupAttacks = false;

            bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            //  AssetReference AttachedParticlesReference = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<UnityEngine.AddressableAssets.AssetReference>("attachedParticlesReference");
            //   AssetReferenceSprite SpriteReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper.GuiPresentation.SpriteReference;


            MonsterDefinitionBuilder Definition = MonsterDefinitionBuilder
                .Create(BaseTemplateName,
                text + NewName,
                GuidHelper.Create(new Guid(Settings.GUID), text + NewName).ToString())
                .SetGuiPresentation("Monster/&" + text + NewTitle, "Monster/&" + text + NewDescription);

            Definition.SetInDungeonEditor(false);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            Definition.SetSizeDefinition(Size);
            Definition.SetChallengeRating(CR);
            Definition.SetAlignment(Alignment);
            Definition.SetCharacterFamily(Type);
            Definition.SetLegendaryCreature(LegendaryCreature);
            Definition.SetArmorClass(ArmorClass);
            Definition.SetHitDiceNumber(HitDice);
            Definition.SetHitDiceType(HitDiceType);
            Definition.SetHitPointsBonus(HitPointsBonus);
            Definition.SetStandardHitPoints(StandardHitPoints);

            Definition.ClearFeatures();
            Definition.AddFeatures(Features);
            Definition.ClearAbilityScores();
            Definition.SetAbilityScores(
                AttributeStrength,
                AttributeDexterity,
                AttributeConstitution,
                AttributeIntelligence,
                AttributeWisdom,
                AttributeCharisma);

            MonsterSavingThrowProficiency StrSave = new MonsterSavingThrowProficiency();
            StrSave.SetField("abilityScoreName", AttributeDefinitions.Strength);
            StrSave.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency DexSave = new MonsterSavingThrowProficiency();
            DexSave.SetField("abilityScoreName", AttributeDefinitions.Dexterity);
            DexSave.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency ConSave = new MonsterSavingThrowProficiency();
            ConSave.SetField("abilityScoreName", AttributeDefinitions.Constitution);
            ConSave.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency IntSave = new MonsterSavingThrowProficiency();
            IntSave.SetField("abilityScoreName", AttributeDefinitions.Intelligence);
            IntSave.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency WisSave = new MonsterSavingThrowProficiency();
            WisSave.SetField("abilityScoreName", AttributeDefinitions.Wisdom);
            WisSave.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency ChaSave = new MonsterSavingThrowProficiency();
            ChaSave.SetField("abilityScoreName", AttributeDefinitions.Charisma);
            ChaSave.SetField("bonus", SavingThrowCharisma);

            Definition.ClearSavingThrowScores();
            Definition.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            Definition.ClearSkillScores();
            Definition.AddSkillScores(SkillScores);
            Definition.ClearAttackIterations();
            Definition.AddAttackIterations(AttackIterations);
            Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions);
            Definition.SetGroupAttacks(GroupAttacks);
            Definition.ClearLegendaryActionOptions();
            Definition.AddLegendaryActionOptions(LegendaryActionOptions);
            //Definition.GuiPresentation.spriteReference=(SpriteReference);
            Definition.SetHasPhantomDistortion(PhantomDistortion);
            //Definition.MonsterPresentation.attachedParticlesReference=(AttachedParticlesReference);
            Definition.SetNoExperienceGain(false);
            Definition.SetHasMonsterPortraitBackground(true);
            Definition.SetCanGeneratePortrait(true);
            //  Definition.MonsterPresentation.customShaderReference=(MonsterShaderReference);



            Definition.SetModelScale(0.1f);


            Definition.SetFullyControlledWhenAllied(true);
            Definition.SetDefaultFaction("Party");

            PactChainPseudodragon = Definition.AddToDB();
            PactChainPseudodragon.MonsterPresentation.SetHasPrefabVariants(false);
            PactChainPseudodragon.CreatureTags.Clear();
            PactChainPseudodragon.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            PactChainPseudodragon.MonsterPresentation.SetUseCustomMaterials(true);
            PactChainPseudodragon.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Silver_Dragon_Presentation.CustomMaterials);

        }

        public static void buildCustomSprite()
        {

            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.Dryad;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.FeyBear;

            string NewName = "CustomSprite";
            string NewTitle = "CustomSpriteTitle";
            string NewDescription = "CustomSpriteDescription";
            CharacterSizeDefinition Size = DatabaseHelper.CharacterSizeDefinitions.Tiny;
            string Alignment = DatabaseHelper.AlignmentDefinitions.NeutralGood.name;
            int ArmorClass = 15;
            int HitDice = 1;
            RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D4;
            int HitPointsBonus = 0;
            int StandardHitPoints = 2;
            int AttributeStrength = 3;
            int AttributeDexterity = 18;
            int AttributeConstitution = 10;
            int AttributeIntelligence = 14;
            int AttributeWisdom = 13;
            int AttributeCharisma = 11;
            int SavingThrowStrength = 0;
            int SavingThrowDexterity = 0;
            int SavingThrowConstitution = 0;
            int SavingThrowIntelligence = 0;
            int SavingThrowWisdom = 0;
            int SavingThrowCharisma = 0;
            float CR = 0.25f;
            bool LegendaryCreature = false;
            string Type = DatabaseHelper.CharacterFamilyDefinitions.Fey.name;

            List<FeatureDefinition> Features = new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,

                PactofChainFamiliarInvisibilityPower
            };

            MonsterSkillProficiency skillProficiency_1 = new MonsterSkillProficiency();
            skillProficiency_1.SetField("skillName", "Perception");
            skillProficiency_1.SetField("bonus", 3);

            MonsterSkillProficiency skillProficiency_2 = new MonsterSkillProficiency();
            skillProficiency_2.SetField("skillName", "Stealth");
            skillProficiency_2.SetField("bonus", 8);

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>() { skillProficiency_1, skillProficiency_2 };

            /*waiting until MonsterAttackDefinitionBuilder is available to use
             *
             * need to update AttackIterations
             * test before commiting
             //           MonsterAttackDefinition SpriteAttack = MonsterAttackDefinitionBuilder(
             //                    DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
             //                    "DH_Custom_" + text,
             //                    GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "SpriteAttack").ToString()
             //                     );
             //                   SpriteAttack.GuiPresentation.SetTitle      ("MonsterAttack/&DH_SpriteAttack_Title");
             //                   SpriteAttack.GuiPresentation.SetDescription("MonsterAttack/&DH_SpriteAttack_Description");

             //           SpriteAttack.SetToHitBonus(6);

             //           SpriteAttack.EffectDescription.SetRangeParameter(8);
             //           SpriteAttack.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
             //           SpriteAttack.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
             //           SpriteAttack.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(0);
             //           SpriteAttack.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);

             //           ConditionForm conditionForm = new ConditionForm();
             //           conditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
             //           conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionPoisoned.name);
             //           conditionForm.SetOperation(ConditionForm.ConditionOperation.Add);

             //           EffectForm extraPoisonEffect = new EffectForm();
             //           extraPoisonEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
             //           extraPoisonEffect.SetLevelMultiplier(1);
             //           extraPoisonEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
             //           extraPoisonEffect.SetCreatedByCharacter(true);
             //           extraPoisonEffect.FormType = EffectForm.EffectFormType.Condition;
             //           extraPoisonEffect.SetConditionForm(conditionForm);
             //           extraPoisonEffect.SetHasSavingThrow(true);
             //           extraPoisonEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
             //
                          ConditionForm sleepForm = new ConditionForm();
             //           sleepForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionMagicallyAsleep);
             //           sleepForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionAsleep.name);
             //           sleepForm.SetOperation(ConditionForm.ConditionOperation.Add);

             //           EffectForm extraSleepEffect = new EffectForm();
             //           extraSleepEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
             //           extraSleepEffect.SetLevelMultiplier(1);
             //           extraSleepEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
             //           extraSleepEffect.SetCreatedByCharacter(true);
             //           extraSleepEffect.FormType = EffectForm.EffectFormType.Condition;
             //           extraSleepEffect.SetConditionForm(sleepForm);
             //           extraSleepEffect.SetHasSavingThrow(true);
             //           extraSleepEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);

             //           SpriteAttack.EffectDescription.EffectForms.Add(extraSleepEffect);
             //           SpriteAttack.EffectDescription.EffectForms.Add(extraPoisonEffect);
             //           SpriteAttack.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
             //           SpriteAttack.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
             //           SpriteAttack.EffectDescription.SetHasSavingThrow(true);
             //           SpriteAttack.EffectDescription.SetFixedSavingThrowDifficultyClass(13);
             //           SpriteAttack.EffectDescription.SetDurationParameter(1);
             //           SpriteAttack.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);


                        */

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>
            {
                new MonsterAttackIteration(DatabaseHelper.MonsterAttackDefinitions.Attack_Mage_Dagger,1),
                new MonsterAttackIteration(DatabaseHelper.MonsterAttackDefinitions.Attack_Goblin_PebbleThrow,1)
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            bool GroupAttacks = false;

            bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            //  AssetReference AttachedParticlesReference = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<UnityEngine.AddressableAssets.AssetReference>("attachedParticlesReference");
            //   AssetReferenceSprite SpriteReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper.GuiPresentation.SpriteReference;

            MonsterDefinitionBuilder Definition = MonsterDefinitionBuilder.Create(BaseTemplateName,
                text + NewName,
                GuidHelper.Create(new Guid(Settings.GUID), text + NewName).ToString())
                .SetGuiPresentation("Monster/&" + text + NewTitle, "Monster/&" + text + NewDescription);

            Definition.SetInDungeonEditor(false);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            Definition.SetSizeDefinition(Size);
            Definition.SetChallengeRating(CR);
            Definition.SetAlignment(Alignment);
            Definition.SetCharacterFamily(Type);
            Definition.SetLegendaryCreature(LegendaryCreature);
            Definition.SetArmorClass(ArmorClass);
            Definition.SetHitDiceNumber(HitDice);
            Definition.SetHitDiceType(HitDiceType);
            Definition.SetHitPointsBonus(HitPointsBonus);
            Definition.SetStandardHitPoints(StandardHitPoints);

            Definition.ClearFeatures();
            Definition.AddFeatures(Features);
            Definition.ClearAbilityScores();
            Definition.SetAbilityScores(
                AttributeStrength,
                AttributeDexterity,
                AttributeConstitution,
                AttributeIntelligence,
                AttributeWisdom,
                AttributeCharisma);

            MonsterSavingThrowProficiency StrSave = new MonsterSavingThrowProficiency();
            StrSave.SetField("abilityScoreName", AttributeDefinitions.Strength);
            StrSave.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency DexSave = new MonsterSavingThrowProficiency();
            DexSave.SetField("abilityScoreName", AttributeDefinitions.Dexterity);
            DexSave.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency ConSave = new MonsterSavingThrowProficiency();
            ConSave.SetField("abilityScoreName", AttributeDefinitions.Constitution);
            ConSave.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency IntSave = new MonsterSavingThrowProficiency();
            IntSave.SetField("abilityScoreName", AttributeDefinitions.Intelligence);
            IntSave.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency WisSave = new MonsterSavingThrowProficiency();
            WisSave.SetField("abilityScoreName", AttributeDefinitions.Wisdom);
            WisSave.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency ChaSave = new MonsterSavingThrowProficiency();
            ChaSave.SetField("abilityScoreName", AttributeDefinitions.Charisma);
            ChaSave.SetField("bonus", SavingThrowCharisma);

            Definition.ClearSavingThrowScores();
            Definition.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            Definition.ClearSkillScores();
            Definition.AddSkillScores(SkillScores);
            Definition.ClearAttackIterations();
            Definition.AddAttackIterations(AttackIterations);
            Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions);
            Definition.SetGroupAttacks(GroupAttacks);
            Definition.ClearLegendaryActionOptions();
            Definition.AddLegendaryActionOptions(LegendaryActionOptions);
            //Definition.GuiPresentation.spriteReference=(SpriteReference);
            Definition.SetHasPhantomDistortion(PhantomDistortion);
            //Definition.MonsterPresentation.attachedParticlesReference=(AttachedParticlesReference);
            Definition.SetNoExperienceGain(false);
            Definition.SetHasMonsterPortraitBackground(true);
            Definition.SetCanGeneratePortrait(true);

            // TODO;
            //Definition.SetCustomShaderReference(MonsterShaderReference);


            PactChainSprite = Definition.AddToDB();

            PactChainSprite.MonsterPresentation.SetOverrideCharacterShaderColors(true);
            PactChainSprite.MonsterPresentation.SetFirstCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<Color>("firstCharacterShaderColor"));
            PactChainSprite.MonsterPresentation.SetSecondCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<Color>("secondCharacterShaderColor"));

            PactChainSprite.CreatureTags.Clear();
            PactChainSprite.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            PactChainSprite.MonsterPresentation.SetMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
            PactChainSprite.MonsterPresentation.SetUseCustomMaterials(true);
            //  PactChainSprite.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Silver_Dragon_Presentation.customMaterials);

            PactChainSprite.MonsterPresentation.SetMaleModelScale(0.2f);
            PactChainSprite.MonsterPresentation.SetFemaleModelScale(0.2f);
            PactChainSprite.MonsterPresentation.SetMalePrefabReference(DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));
            PactChainSprite.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));

            PactChainSprite.SetFullyControlledWhenAllied(true);
            PactChainSprite.SetDefaultFaction("Party");


            PactChainSprite.MonsterPresentation.SetHasPrefabVariants(false);
        }


        public static void buildCustomImp()
        {


            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.Goblin;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Ghoul;

            string NewName = "CustomImp";
            string NewTitle = "CustomImpTitle";
            string NewDescription = "CustomImpDescription";
            CharacterSizeDefinition Size = DatabaseHelper.CharacterSizeDefinitions.Tiny;
            string Alignment = DatabaseHelper.AlignmentDefinitions.LawfulEvil.name;
            int ArmorClass = 13;
            int HitDice = 3;
            RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D2;
            int HitPointsBonus = 3;
            int StandardHitPoints = 10;
            int AttributeStrength = 6;
            int AttributeDexterity = 17;
            int AttributeConstitution = 13;
            int AttributeIntelligence = 11;
            int AttributeWisdom = 12;
            int AttributeCharisma = 14;
            int SavingThrowStrength = 0;
            int SavingThrowDexterity = 0;
            int SavingThrowConstitution = 0;
            int SavingThrowIntelligence = 0;
            int SavingThrowWisdom = 0;
            int SavingThrowCharisma = 0;
            float CR = 1f;
            bool LegendaryCreature = false;
            string Type = DatabaseHelper.CharacterFamilyDefinitions.Fiend.name;

            List<FeatureDefinition> Features = new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                PactofChainFamiliarInvisibilityPower,
              //  DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance
              PactofChainFamiliarSpellResistencePower
                //  game doesnt like when a monster uses shapechange
              //  not easy to stop see through magical darkness
            };

            MonsterSkillProficiency skillProficiency_1 = new MonsterSkillProficiency();
            skillProficiency_1.SetField("skillName", "Perception");
            skillProficiency_1.SetField("bonus", 4);

            MonsterSkillProficiency skillProficiency_2 = new MonsterSkillProficiency();
            skillProficiency_2.SetField("skillName", "Stealth");
            skillProficiency_2.SetField("bonus", 5);

            MonsterSkillProficiency skillProficiency_3 = new MonsterSkillProficiency();
            skillProficiency_3.SetField("skillName", "Deception");
            skillProficiency_3.SetField("bonus", 4);

            MonsterSkillProficiency skillProficiency_4 = new MonsterSkillProficiency();
            skillProficiency_4.SetField("skillName", "Insight");
            skillProficiency_4.SetField("bonus", 3);

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>()
            {
                skillProficiency_1,
                skillProficiency_2,
                skillProficiency_3,
                skillProficiency_4
            };

            /* waiting until MonsterAttackDefinitionBuilder is available to use
              * need to update attackIterations
              * test before commiting
              //           MonsterAttackDefinition ImpAttack = MonsterAttackDefinitionBuilder(
              //                    DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
              //                    "DH_Custom_" + text,
              //                    GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "ImpAttack").ToString()
              //                     );
              //             ImpAttack.GuiPresentation.SetTitle            ("MonsterAttack/&DH_ImpAttack_Title");
              //             ImpAttack.GuiPresentation.SetDescription      ("MonsterAttack/&DH_ImpAttack_Description");
              //           ImpAttack.SetToHitBonus(5);
              //           ImpAttack.EffectDescription.SetRangeParameter(1);
              //           ImpAttack.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
              //           ImpAttack.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
              //           ImpAttack.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(3);
              //           ImpAttack.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);
                         DamageForm DamageForm = new DamageForm();
                         DamageForm.SetDiceNumber(3);
                         DamageForm.SetDieType(RuleDefinitions.DieType.D6);
                         DamageForm.SetBonusDamage(0);
                         DamageForm.SetDamageType(RuleDefinitions.DamageTypePoison);
                         EffectForm extraPoisonEffect = new EffectForm();
                         extraPoisonEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
                         extraPoisonEffect.SetLevelMultiplier(1);
                         extraPoisonEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
                         extraPoisonEffect.SetCreatedByCharacter(true);
                         extraPoisonEffect.FormType = EffectForm.EffectFormType.Damage;
                         extraPoisonEffect.SetDamageForm(DamageForm);
                         extraPoisonEffect.SetHasSavingThrow(true);
                         extraPoisonEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);

              //           ImpAttack.EffectDescription.EffectForms.Add(extraPoisonEffect);
              //           ImpAttack.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
              //           ImpAttack.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
              //           ImpAttack.EffectDescription.SetHasSavingThrow(true);
              //           ImpAttack.EffectDescription.SetFixedSavingThrowDifficultyClass(11);
                         */

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>
            {
                new MonsterAttackIteration(DatabaseHelper.MonsterAttackDefinitions.Attack_PoisonousSnake_Bite,1)
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            bool GroupAttacks = false;

            bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            //  AssetReference AttachedParticlesReference = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<UnityEngine.AddressableAssets.AssetReference>("attachedParticlesReference");
            //   AssetReferenceImp ImpReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper.GuiPresentation.ImpReference;

            MonsterDefinitionBuilder Definition = MonsterDefinitionBuilder.Create(BaseTemplateName,
                text + NewName,
                GuidHelper.Create(new Guid(Settings.GUID), text + NewName).ToString())
                .SetGuiPresentation("Monster/&" + text + NewTitle, "Monster/&" + text + NewDescription);

            Definition.SetInDungeonEditor(false);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            Definition.SetSizeDefinition(Size);
            Definition.SetChallengeRating(CR);
            Definition.SetAlignment(Alignment);
            Definition.SetCharacterFamily(Type);
            Definition.SetLegendaryCreature(LegendaryCreature);
            Definition.SetArmorClass(ArmorClass);
            Definition.SetHitDiceNumber(HitDice);
            Definition.SetHitDiceType(HitDiceType);
            Definition.SetHitPointsBonus(HitPointsBonus);
            Definition.SetStandardHitPoints(StandardHitPoints);

            Definition.ClearFeatures();
            Definition.AddFeatures(Features);
            Definition.ClearAbilityScores();
            Definition.SetAbilityScores(
                AttributeStrength,
                AttributeDexterity,
                AttributeConstitution,
                AttributeIntelligence,
                AttributeWisdom,
                AttributeCharisma);

            MonsterSavingThrowProficiency StrSave = new MonsterSavingThrowProficiency();
            StrSave.SetField("abilityScoreName", AttributeDefinitions.Strength);
            StrSave.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency DexSave = new MonsterSavingThrowProficiency();
            DexSave.SetField("abilityScoreName", AttributeDefinitions.Dexterity);
            DexSave.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency ConSave = new MonsterSavingThrowProficiency();
            ConSave.SetField("abilityScoreName", AttributeDefinitions.Constitution);
            ConSave.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency IntSave = new MonsterSavingThrowProficiency();
            IntSave.SetField("abilityScoreName", AttributeDefinitions.Intelligence);
            IntSave.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency WisSave = new MonsterSavingThrowProficiency();
            WisSave.SetField("abilityScoreName", AttributeDefinitions.Wisdom);
            WisSave.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency ChaSave = new MonsterSavingThrowProficiency();
            ChaSave.SetField("abilityScoreName", AttributeDefinitions.Charisma);
            ChaSave.SetField("bonus", SavingThrowCharisma);

            Definition.ClearSavingThrowScores();
            Definition.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            Definition.ClearSkillScores();
            Definition.AddSkillScores(SkillScores);
            Definition.ClearAttackIterations();
            Definition.AddAttackIterations(AttackIterations);
            Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions);
            Definition.SetGroupAttacks(GroupAttacks);
            Definition.ClearLegendaryActionOptions();
            Definition.AddLegendaryActionOptions(LegendaryActionOptions);
            //Definition.GuiPresentation.ImpReference=(ImpReference);
            Definition.SetHasPhantomDistortion(PhantomDistortion);
            //Definition.MonsterPresentation.attachedParticlesReference=(AttachedParticlesReference);
            Definition.SetNoExperienceGain(false);
            Definition.SetHasMonsterPortraitBackground(true);
            Definition.SetCanGeneratePortrait(true);
            //  Definition.MonsterPresentation.customShaderReference=(MonsterShaderReference);

            PactChainImp = Definition.AddToDB();

            PactChainImp.CreatureTags.Clear();
            PactChainImp.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            PactChainImp.MonsterPresentation.SetMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
            PactChainImp.MonsterPresentation.SetUseCustomMaterials(true);
            PactChainImp.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Orc_Female_Archer_RedScar.CustomMaterials);

            PactChainImp.MonsterPresentation.SetMaleModelScale(0.2f);
            PactChainImp.MonsterPresentation.SetFemaleModelScale(0.2f);
            PactChainImp.MonsterPresentation.SetMalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));
            PactChainImp.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("mMalePrefabReference"));

            PactChainImp.SetFullyControlledWhenAllied(true);

            PactChainImp.SetDefaultFaction("Party");
            PactChainImp.MonsterPresentation.SetHasPrefabVariants(false);

        }


        public static void buildCustomQuasit()
        {


            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.Goblin;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Goblin;

            string NewName = "CustomQuasit";
            string NewTitle = "CustomQuasitTitle";
            string NewDescription = "CustomQuasitDescription";
            CharacterSizeDefinition Size = DatabaseHelper.CharacterSizeDefinitions.Tiny;
            string Alignment = DatabaseHelper.AlignmentDefinitions.NeutralGood.name;
            int ArmorClass = 13;
            int HitDice = 3;
            RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D4;
            int HitPointsBonus = 0;
            int StandardHitPoints = 7;
            int AttributeStrength = 5;
            int AttributeDexterity = 17;
            int AttributeConstitution = 10;
            int AttributeIntelligence = 7;
            int AttributeWisdom = 10;
            int AttributeCharisma = 10;
            int SavingThrowStrength = 0;
            int SavingThrowDexterity = 0;
            int SavingThrowConstitution = 0;
            int SavingThrowIntelligence = 0;
            int SavingThrowWisdom = 0;
            int SavingThrowCharisma = 0;
            float CR = 1f;
            bool LegendaryCreature = false;
            string Type = DatabaseHelper.CharacterFamilyDefinitions.Fiend.name;

            List<FeatureDefinition> Features = new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                PactofChainFamiliarInvisibilityPower,

               //  DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance
              PactofChainFamiliarSpellResistencePower
                //  castSpellQuasit
            };


            MonsterSkillProficiency skillProficiency_2 = new MonsterSkillProficiency();
            skillProficiency_2.SetField("skillName", "Stealth");
            skillProficiency_2.SetField("bonus", 5);

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>() { skillProficiency_2 };

            /*waiting until MonsterAttackDefinitionBuilder is available to use
             * need to update attackIterations, test before commiting
            //           MonsterAttackDefinition QuasitAttack = MonsterAttackDefinitionBuilder(
            //                    DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
            //                    "DH_Custom_" + text,
            //                    GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "QuasitAttack").ToString()
            //                     );
            //               QuasitAttack.GuiPresentation.SetTitle           "MonsterAttack/&DH_QuasitAttack_Title");
            //               QuasitAttack.GuiPresentation.SetDescription     "MonsterAttack/&DH_QuasitAttack_Description");
            //           QuasitAttack.SetToHitBonus(4);
            //           QuasitAttack.EffectDescription.SetRangeParameter(1);
            //           QuasitAttack.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
            //           QuasitAttack.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
            //           QuasitAttack.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(3);
            //           QuasitAttack.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);
            //           ConditionForm conditionForm = new ConditionForm();
            //           conditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
            //           conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionPoisoned.name);
            //           conditionForm.SetOperation(ConditionForm.ConditionOperation.Add);
            //           EffectForm extraPoisonEffect = new EffectForm();
            //           extraPoisonEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            //           extraPoisonEffect.SetLevelMultiplier(1);
            //           extraPoisonEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            //           extraPoisonEffect.SetCreatedByCharacter(true);
            //           extraPoisonEffect.FormType = EffectForm.EffectFormType.Condition;
            //           extraPoisonEffect.SetConditionForm(conditionForm);
            //           extraPoisonEffect.SetHasSavingThrow(true);
            //           extraPoisonEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            //           DamageForm DamageForm = new DamageForm();
                        DamageForm.SetDiceNumber(2);
                        DamageForm.SetDieType(RuleDefinitions.DieType.D4);
                        DamageForm.SetBonusDamage(0);
                        DamageForm.SetDamageType(RuleDefinitions.DamageTypePoison);
                        EffectForm extraPoisonDamage = new EffectForm();
                        extraPoisonDamage.SetApplyLevel(EffectForm.LevelApplianceType.No);
                        extraPoisonDamage.SetLevelMultiplier(1);
                        extraPoisonDamage.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
                        extraPoisonDamage.SetCreatedByCharacter(true);
                        extraPoisonDamage.FormType = EffectForm.EffectFormType.Damage;
                        extraPoisonDamage.SetDamageForm(DamageForm);
                        extraPoisonDamage.SetHasSavingThrow(true);
                        extraPoisonDamage.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);

            //           QuasitAttack.EffectDescription.EffectForms.Add(extraPoisonDamage);
            //           QuasitAttack.EffectDescription.EffectForms.Add(extraPoisonEffect);
            //           QuasitAttack.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
            //           QuasitAttack.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
            //           QuasitAttack.EffectDescription.SetHasSavingThrow(true);
            //           QuasitAttack.EffectDescription.SetFixedSavingThrowDifficultyClass(10);
             //           SpriteAttack.EffectDescription.SetDurationParameter(1);
             //           SpriteAttack.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
                        */

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>
            {
                new MonsterAttackIteration(DatabaseHelper.MonsterAttackDefinitions.Attack_PoisonousSnake_Bite,1)
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            bool GroupAttacks = false;

            bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            //  AssetReference AttachedParticlesReference = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<UnityEngine.AddressableAssets.AssetReference>("attachedParticlesReference");
            //   AssetReferenceQuasit QuasitReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper.GuiPresentation.QuasitReference;

            MonsterDefinitionBuilder Definition = MonsterDefinitionBuilder
                .Create(BaseTemplateName, text + NewName, GuidHelper.Create(new Guid(Settings.GUID), text + NewName).ToString())
                .SetGuiPresentation("Monster/&" + text + NewTitle, "Monster/&" + text + NewDescription);

            Definition.SetInDungeonEditor(false);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            Definition.SetSizeDefinition(Size);
            Definition.SetChallengeRating(CR);
            Definition.SetAlignment(Alignment);
            Definition.SetCharacterFamily(Type);
            Definition.SetLegendaryCreature(LegendaryCreature);
            Definition.SetArmorClass(ArmorClass);
            Definition.SetHitDiceNumber(HitDice);
            Definition.SetHitDiceType(HitDiceType);
            Definition.SetHitPointsBonus(HitPointsBonus);
            Definition.SetStandardHitPoints(StandardHitPoints);

            Definition.ClearFeatures();
            Definition.AddFeatures(Features);
            Definition.ClearAbilityScores();
            Definition.SetAbilityScores(
                AttributeStrength,
                AttributeDexterity,
                AttributeConstitution,
                AttributeIntelligence,
                AttributeWisdom,
                AttributeCharisma);

            MonsterSavingThrowProficiency StrSave = new MonsterSavingThrowProficiency();
            StrSave.SetField("abilityScoreName", AttributeDefinitions.Strength);
            StrSave.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency DexSave = new MonsterSavingThrowProficiency();
            DexSave.SetField("abilityScoreName", AttributeDefinitions.Dexterity);
            DexSave.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency ConSave = new MonsterSavingThrowProficiency();
            ConSave.SetField("abilityScoreName", AttributeDefinitions.Constitution);
            ConSave.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency IntSave = new MonsterSavingThrowProficiency();
            IntSave.SetField("abilityScoreName", AttributeDefinitions.Intelligence);
            IntSave.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency WisSave = new MonsterSavingThrowProficiency();
            WisSave.SetField("abilityScoreName", AttributeDefinitions.Wisdom);
            WisSave.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency ChaSave = new MonsterSavingThrowProficiency();
            ChaSave.SetField("abilityScoreName", AttributeDefinitions.Charisma);
            ChaSave.SetField("bonus", SavingThrowCharisma);

            Definition.ClearSavingThrowScores();
            Definition.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            Definition.ClearSkillScores();
            Definition.AddSkillScores(SkillScores);
            Definition.ClearAttackIterations();
            Definition.AddAttackIterations(AttackIterations);
            Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions);
            Definition.SetGroupAttacks(GroupAttacks);
            Definition.ClearLegendaryActionOptions();
            Definition.AddLegendaryActionOptions(LegendaryActionOptions);
            //Definition.GuiPresentation.QuasitReference=(QuasitReference);
            Definition.SetHasPhantomDistortion(PhantomDistortion);
            //Definition.MonsterPresentation.attachedParticlesReference=(AttachedParticlesReference);
            Definition.SetNoExperienceGain(false);
            Definition.SetHasMonsterPortraitBackground(true);
            Definition.SetCanGeneratePortrait(true);
            //  Definition.MonsterPresentation.customShaderReference=(MonsterShaderReference);

            PactChainQuasit = Definition.AddToDB();

            PactChainQuasit.CreatureTags.Clear();
            PactChainQuasit.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            PactChainQuasit.MonsterPresentation.SetMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
            PactChainQuasit.MonsterPresentation.SetUseCustomMaterials(true);
            PactChainQuasit.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Orc_Male_Chieftain_BladeFang.CustomMaterials);

            PactChainQuasit.MonsterPresentation.SetMaleModelScale(0.3f);
            PactChainQuasit.MonsterPresentation.SetFemaleModelScale(0.3f);
            PactChainQuasit.MonsterPresentation.SetMalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));
            PactChainQuasit.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));

            PactChainQuasit.SetFullyControlledWhenAllied(true);

            PactChainQuasit.SetDefaultFaction("Party");
            PactChainQuasit.MonsterPresentation.SetHasPrefabVariants(false);

        }


    }
}
