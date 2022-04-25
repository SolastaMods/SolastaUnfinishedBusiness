using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine.AddressableAssets;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal static class WarlockPactOfTheChainSummons
    {
        public static FeatureDefinitionPower PactofChainFamiliarInvisibilityPower { get; private set; }
        public static MonsterDefinition PactChainSprite { get; private set; }
        public static MonsterDefinition PactChainImp { get; private set; }
        public static MonsterDefinition PactChainQuasit { get; private set; }

        public static void buildPactofChainFamiliarInvisibilityPower()
        {
            SpellDefinition invisibilty = DatabaseHelper.SpellDefinitions.Invisibility;
            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(invisibilty.EffectDescription);

            PactofChainFamiliarInvisibilityPower = FeatureDefinitionPowerBuilder
                .Create("PactofChainFamiliarInvisibilityPower", DefinitionBuilder.CENamespaceGuid)
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

        // public static FeatureDefinition buildSummoningAffinity()
        // {
        //     var acConditionDefinition = ConditionDefinitionBuilder
        //         .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondAC, "ConditionWarlockFamiliarAC",
        //             DefinitionBuilder.CENamespaceGuid)
        //         .SetGuiPresentationNoContent()
        //         .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
        //         .AddToDB();
        //
        //     var stConditionDefinition = ConditionDefinitionBuilder
        //         .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSavingThrows,
        //             "ConditionWarlockFamiliarST", DefinitionBuilder.CENamespaceGuid)
        //         .SetGuiPresentationNoContent()
        //         .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
        //         .AddToDB();
        //      
        //      var damageConditionDefinition = ConditionDefinitionBuilder
        //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage,
        //              "ConditionWarlockFamiliarDamage", DefinitionBuilder.CENamespaceGuid)
        //          .SetGuiPresentationNoContent()
        //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
        //          .AddToDB();
        //
        //      var hitConditionDefinition = ConditionDefinitionBuilder
        //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack,
        //              "ConditionWarlockFamiliarHit", DefinitionBuilder.CENamespaceGuid)
        //          .SetGuiPresentationNoContent()
        //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceSpellAttack)
        //          .AddToDB();
        //
        //      var hpConditionDefinition = ConditionDefinitionBuilder
        //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondHP, "ConditionWarlockFamiliarHP",
        //              DefinitionBuilder.CENamespaceGuid)
        //          .SetGuiPresentationNoContent()
        //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
        //          .SetAllowMultipleInstances(true)
        //          .AddToDB();
        //
        //      var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
        //          .Create(DatabaseHelper.FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond,
        //              "SummoningAffinityWarlockFamiliar", DefinitionBuilder.CENamespaceGuid)
        //          .ClearEffectForms()
        //          .SetRequiredMonsterTag("WarlockFamiliar")
        //          .SetAddedConditions(
        //              acConditionDefinition, stConditionDefinition, damageConditionDefinition, hitConditionDefinition, 
        //              hpConditionDefinition, hpConditionDefinition)
        //          .AddToDB();
        //
        //      return summoningAffinity;
        // }

        public static MonsterDefinition buildCustomPseudodragon()
        {
            var baseMonster = DatabaseHelper.MonsterDefinitions.Young_GreenDragon;
            
            var biteAttack = MonsterAttackDefinitionBuilder
                .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Wolf_Bite, "AttackWarlockDragonBite", DefinitionBuilder.CENamespaceGuid)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetToHitBonus(4)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage:2, damageType:DamageTypePiercing)
                        .Build()
                    )
                    .Build()
                )
                .AddToDB();
            
            var stingAttack = MonsterAttackDefinitionBuilder
                .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Badlands_Spider_Bite, "AttackWarlockDragonSting", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.MonsterAttack)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetToHitBonus(4)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetSavingThrowData(
                        true,
                        true,
                        AttributeDefinitions.Constitution,
                        false, EffectDifficultyClassComputation.FixedValue,
                        null,
                        11
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage:2, damageType:DamageTypePiercing)
                        .Build(),
                        new EffectFormBuilder()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(
                                DatabaseHelper.ConditionDefinitions.ConditionPoisoned,
                                ConditionForm.ConditionOperation.Add
                            )
                            .Build()
                    )
                    .Build()
                )
                .AddToDB();


            var monster = MonsterDefinitionBuilder
                .Create(baseMonster, "PactOfChainDragon", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("PactOfChainCustomPseudodragon", Category.Monster, baseMonster.GuiPresentation.SpriteReference)
                .SetFeatures(
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2,
                    DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                    DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                    DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,
                    
                    //DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity
                    
                    DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityMagebaneRejectMagic,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing
                )
                .SetAttackIterations(stingAttack, biteAttack)
                .SetSkillScores(
                    (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                    (DatabaseHelper.SkillDefinitions.Stealth.Name, 4)
                )
                .SetArmorClass(13)
                .SetAbilityScores(6,15,13,10,12,10)
                .SetStandardHitPoints(7)
                .SetHitDice(DieType.D4, 2)
                .SetHitPointsBonus(2)
                .SetSavingThrowScores()//TODO: check if this sets bonuses to 0
                .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions)
                .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
                .SetAlignment(DatabaseHelper.AlignmentDefinitions.NeutralGood.Name)
                .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Dragon.name)
                .SetChallengeRating(0)
                .SetLegendaryCreature(false)
                .SetDroppedLootDefinition(null)
                .SetFullyControlledWhenAllied(true)
                .SetDefaultFaction("Party")
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                .SetInDungeonEditor(false)
                .SetModelScale(0.1f)
                .SetCreatureTags("WarlockFamiliar")
                .SetNoExperienceGain(false)
                .SetHasPhantomDistortion(true)
                .SetForceNoFlyAnimation(true)
                .SetGroupAttacks(true)
                .AddToDB();
            
            
            monster.MonsterPresentation.SetHasPrefabVariants(false);
            monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            monster.MonsterPresentation.SetUseCustomMaterials(true);
            monster.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Young_Green_Dragon_Presentation.CustomMaterials);
            monster.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            monster.MonsterPresentation.SetCanGeneratePortrait(true);
            
            return monster; 
        }

        public static void buildCustomSprite()
        {

            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.Dryad;
            //MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.FeyBear;

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
                text + NewName, DefinitionBuilder.CENamespaceGuid)
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
            //MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Ghoul;

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
              // PactofChainFamiliarSpellResistencePower
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

            MonsterDefinitionBuilder Definition = MonsterDefinitionBuilder
                .Create(BaseTemplateName, text + NewName, DefinitionBuilder.CENamespaceGuid)
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
            PactChainImp.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("femalePrefabReference"));

            PactChainImp.SetFullyControlledWhenAllied(true);

            PactChainImp.SetDefaultFaction("Party");
            PactChainImp.MonsterPresentation.SetHasPrefabVariants(false);

        }


        public static void buildCustomQuasit()
        {


            string text = "PactOfChain";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.Goblin;
            //MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Goblin;

            string NewName = "CustomQuasit";
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
              // PactofChainFamiliarSpellResistencePower
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
                .Create(BaseTemplateName, text + NewName, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Monster);

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
