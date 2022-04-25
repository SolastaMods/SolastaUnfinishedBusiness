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
        public static MonsterDefinition PactChainQuasit { get; private set; }

        public static void buildPactofChainFamiliarInvisibilityPower()
        {
            SpellDefinition invisibilty = DatabaseHelper.SpellDefinitions.Invisibility;
            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(invisibilty.EffectDescription);

            PactofChainFamiliarInvisibilityPower = FeatureDefinitionPowerBuilder
                .Create("PactofChainFamiliarInvisibilityPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power, DatabaseHelper.SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
                .SetUsesFixed(1)
                .SetActivation(ActivationTime.Action, 0)
                .SetRechargeRate(RechargeRate.AtWill)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetEffectForms(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create(DatabaseHelper.ConditionDefinitions.ConditionInvisible, "PactofChainFamiliarInvisibilityCondition", DefinitionBuilder.CENamespaceGuid)
                                .SetGuiPresentation(DatabaseHelper.ConditionDefinitions.ConditionInvisible.GuiPresentation)
                                .SetConditionType(ConditionType.Beneficial)
                                .SetDuration(DurationType.Permanent)
                                .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.CastSpell, ConditionInterruption.UsePower, ConditionInterruption.Damaged)
                                .SetInterruptionDamageThreshold(1)
                                .AddToDB(), ConditionForm.ConditionOperation.Add
                        )
                        .Build()
                    )
                    .Build())
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
                .SetSavingThrowScores()
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

        public static MonsterDefinition buildCustomSprite()
        {
            var baseMonster = DatabaseHelper.MonsterDefinitions.Dryad;

            var swordAttack = MonsterAttackDefinitionBuilder
                .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Veteran_Longsword, "AttackWarlockSpriteSword",
                    DefinitionBuilder.CENamespaceGuid)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetProximity(AttackProximity.Melee)
                .SetToHitBonus(2)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D1, diceNumber: 1, bonusDamage: 0,
                            damageType: DamageTypeSlashing)
                        .Build()
                    )
                    .Build()
                )
                .AddToDB();

            var bowAttack = MonsterAttackDefinitionBuilder
                .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Goblin_ShortBow, "AttackWarlockSpriteBow",
                    DefinitionBuilder.CENamespaceGuid)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetProximity(AttackProximity.Range)
                .SetToHitBonus(6)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetSavingThrowData(
                        true,
                        true,
                        AttributeDefinitions.Constitution,
                        false, EffectDifficultyClassComputation.FixedValue,
                        null,
                        10
                    )
                    .SetEffectForms(new EffectFormBuilder()
                            .SetDamageForm(dieType: DieType.D1, diceNumber: 1, bonusDamage: 0,
                                damageType: DamageTypePiercing)
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
                .Create(baseMonster, "PactOfChainCustomSprite", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation( Category.Monster, baseMonster.GuiPresentation.SpriteReference)
                .SetFeatures(
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2,
                    DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                    PactofChainFamiliarInvisibilityPower
                )
                .SetAttackIterations(bowAttack, swordAttack)
                .SetSkillScores(
                    (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                    (DatabaseHelper.SkillDefinitions.Stealth.Name, 8)
                )
                .SetArmorClass(15)
                .SetArmor(DatabaseHelper.ArmorTypeDefinitions.LeatherType.Name)
                .SetAbilityScores(3, 18, 10, 14, 12, 11)
                .SetStandardHitPoints(2)
                .SetHitDice(DieType.D4, 1)
                .SetHitPointsBonus(2)
                .SetSavingThrowScores()
                .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DryadCombatDecisions)
                .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
                .SetAlignment(DatabaseHelper.AlignmentDefinitions.NeutralGood.Name)
                .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fey.name)
                .SetChallengeRating(0.25f)
                .SetLegendaryCreature(false)
                .SetDroppedLootDefinition(null)
                .SetFullyControlledWhenAllied(true)
                .SetDefaultFaction("Party")
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                .SetInDungeonEditor(false)
                .SetModelScale(0.4f)
                .SetCreatureTags("WarlockFamiliar")
                .SetNoExperienceGain(false)
                .SetHasPhantomDistortion(true)
                .SetForceNoFlyAnimation(true)
                .SetGroupAttacks(false)
                .AddToDB();


            monster.MonsterPresentation.SetHasPrefabVariants(false);
            monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            monster.MonsterPresentation.SetUseCustomMaterials(true);
            // monster.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions
            //     .Young_Green_Dragon_Presentation.CustomMaterials);
            monster.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            monster.MonsterPresentation.SetCanGeneratePortrait(true);
            
            // monster.MonsterPresentation.SetOverrideCharacterShaderColors(true);
            // monster.MonsterPresentation.SetFirstCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<Color>("firstCharacterShaderColor"));
            // monster.MonsterPresentation.SetSecondCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<Color>("secondCharacterShaderColor"));
            //
            // // monster.CreatureTags.Clear();
            // monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            // monster.MonsterPresentation.SetMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
            // monster.MonsterPresentation.SetUseCustomMaterials(true);
            // // //  monster.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Silver_Dragon_Presentation.customMaterials);
            // //
            // monster.MonsterPresentation.SetMaleModelScale(0.4f);
            // monster.MonsterPresentation.SetFemaleModelScale(0.4f);
            // monster.MonsterPresentation.SetMalePrefabReference(DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));
            // monster.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));

            return monster;
        }

        public static MonsterDefinition buildCustomImp()
        {
           var baseMonster = DatabaseHelper.MonsterDefinitions.Goblin;

           var stingAttack = MonsterAttackDefinitionBuilder
               .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Badlands_Spider_Bite, "AttackWarlockImpSting", DefinitionBuilder.CENamespaceGuid)
               .SetGuiPresentation(Category.MonsterAttack)
               .SetActionType(ActionDefinitions.ActionType.Main)
               .SetToHitBonus(5)
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
                           .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage: 3, damageType: DamageTypePiercing)
                           .Build(),
                       new EffectFormBuilder()
                           .SetDamageForm(dieType: DieType.D6, diceNumber: 3, damageType: DamageTypePoison)
                           .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                           .Build()
                   )
                   .Build()
               )
               .AddToDB();


            var monster = MonsterDefinitionBuilder
                .Create(baseMonster, "PactOfChainCustomImp", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation( Category.Monster, baseMonster.GuiPresentation.SpriteReference)
                .SetFeatures(
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove4,
                    DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                    DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                    //Todo: add devil's sight - magical darkness doesn't affect vision
                    
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver,
                    
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                    
                    DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityMagebaneRejectMagic,

                    //TODO: can we implement shapechange for monsters at all?
                    PactofChainFamiliarInvisibilityPower
                )
                .SetAttackIterations(stingAttack)
                .SetSkillScores(
                    (DatabaseHelper.SkillDefinitions.Deception.Name, 4),
                    (DatabaseHelper.SkillDefinitions.Insight.Name, 3),
                    (DatabaseHelper.SkillDefinitions.Persuasion.Name, 4),
                    (DatabaseHelper.SkillDefinitions.Stealth.Name, 5)
                )
                .SetArmorClass(13)
                .SetAbilityScores(6, 17, 13, 11, 12, 14)
                .SetStandardHitPoints(10)
                .SetHitDice(DieType.D4, 3)
                .SetHitPointsBonus(3)
                .SetSavingThrowScores()
                .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DryadCombatDecisions)
                .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
                .SetAlignment(DatabaseHelper.AlignmentDefinitions.LawfulEvil.Name)
                .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fiend.name)
                .SetChallengeRating(1f)
                .SetLegendaryCreature(false)
                .SetDroppedLootDefinition(null)
                .SetFullyControlledWhenAllied(true)
                .SetDefaultFaction("Party")
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.Reference)
                .SetInDungeonEditor(false)
                .SetModelScale(0.4f)
                .SetCreatureTags("WarlockFamiliar")
                .SetNoExperienceGain(false)
                .SetHasPhantomDistortion(true)
                .SetForceNoFlyAnimation(true)
                .SetGroupAttacks(false)
                .AddToDB();
            
            monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
            monster.MonsterPresentation.SetMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
            monster.MonsterPresentation.SetUseCustomMaterials(true);
            monster.MonsterPresentation.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Orc_Female_Archer_RedScar.CustomMaterials);

            monster.MonsterPresentation.SetMaleModelScale(0.4f);
            monster.MonsterPresentation.SetFemaleModelScale(0.4f);
            monster.MonsterPresentation.SetMalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("malePrefabReference"));
            monster.MonsterPresentation.SetFemalePrefabReference(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.GetField<AssetReference>("femalePrefabReference"));

            monster.MonsterPresentation.SetHasPrefabVariants(false);

            return monster;
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
