using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using SolastaCommunityExpansion.Models;
//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters
{
    internal static class MonstersSRD
    {

        public static List<Models.MonsterContext.CustomMonster> Definitions = new List<Models.MonsterContext.CustomMonster>()
        {
            new Models.MonsterContext.CustomMonster()
            {
               MonsterName = "Ancient Black Dragon",
               BaseTemplateName = DatabaseHelper.MonsterDefinitions.BlackDragon_MasterOfNecromancy,
               MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
               NewName = "Custom_AncientBlackDragon",
               NewTitle = "Custom_AncientBlackDragon_Title",
               NewDescription = "Custom_AncientBlackDragon_Description",
               Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
               Alignment = "ChaoticEvil",
               ArmorClass = 22,
               HitDice = 21,
               HitDiceType = RuleDefinitions.DieType.D20,
               HitPointsBonus = 147,
               StandardHitPoints = 367,
               AttributeStrength = 27,
               AttributeDexterity = 14,
               AttributeConstitution = 25,
               AttributeIntelligence = 16,
               AttributeWisdom = 15,
               AttributeCharisma = 19,
               SavingThrowStrength = 0,
               SavingThrowDexterity = 9,
               SavingThrowConstitution = 14,
               SavingThrowIntelligence = 0,
               SavingThrowWisdom = 9,
               SavingThrowCharisma = 11,
               CR = 21,
               LegendaryCreature = true,
               Type = "Dragon",
               Features = new List<FeatureDefinition>()
               {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8 ,
                 DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12    ,
                 DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                 DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityAcidImmunity ,
                 DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence    ,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity ,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,

                 NewMonsterPowers.DictionaryOfAncientDragonBreaths["Ancient Black Dragon"]
               },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DragonmonsterSkillProficiency_1,
                    MonstersAttributes.DragonmonsterSkillProficiency_2
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.BlackDragonBiteAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.DragonlegendaryActionDescription_2,
                    MonstersAttributes.DragonlegendaryActionDescription
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.AncientDragon_CombatDecisions,
                GroupAttacks=true,


                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.BlackDragon_MasterOfNecromancy.GuiPresentation.SpriteReference,


            },

            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Ancient Blue Dragon",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.SpectralDragon_02,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SpectralDragon_02,
                NewName = "Custom_AncientBlueDragon",
                NewTitle = "Custom_AncientBlueDragon_Title",
                NewDescription = "Custom_AncientBlueDragon_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
                Alignment = "LawfulEvil",
                ArmorClass = 22,
                HitDice = 26,
                HitDiceType = RuleDefinitions.DieType.D20,
                HitPointsBonus = 208,
                StandardHitPoints = 481,
                AttributeStrength = 29,
                AttributeDexterity = 10,
                AttributeConstitution = 27,
                AttributeIntelligence = 18,
                AttributeWisdom = 17,
                AttributeCharisma = 21,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 7,
                SavingThrowConstitution = 15,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 10,
                SavingThrowCharisma = 12,
                CR = 23,
                LegendaryCreature = true,
                Type = "Dragon",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeBurrow8   ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8    ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12    ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningImmunity    ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence    ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity
                    ,
                 NewMonsterPowers.DictionaryOfAncientDragonBreaths["Ancient Blue Dragon"]
               },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DragonmonsterSkillProficiency_1,
                    MonstersAttributes.DragonmonsterSkillProficiency_2
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.BlueDragonBiteAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.DragonlegendaryActionDescription_2,
                    MonstersAttributes.DragonlegendaryActionDescription
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.AncientDragon_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.SpectralDragon_02.GuiPresentation.SpriteReference,


            },

            new Models.MonsterContext.CustomMonster()
            {

                MonsterName = "Ancient Green Dragon",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_AncientGreenDragon",
                NewTitle = "Custom_AncientGreenDragon_Title",
                NewDescription = "Custom_AncientGreenDragon_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
                Alignment = "LawfulEvil",
                ArmorClass = 21,
                HitDice = 22,
                HitDiceType = RuleDefinitions.DieType.D20,
                HitPointsBonus = 154,
                StandardHitPoints = 385,
                AttributeStrength = 27,
                AttributeDexterity = 12,
                AttributeConstitution = 25,
                AttributeIntelligence = 20,
                AttributeWisdom = 17,
                AttributeCharisma = 19,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 8,
                SavingThrowConstitution = 14,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 10,
                SavingThrowCharisma = 11,
                CR = 22,
                LegendaryCreature = true,
                Type = "Dragon",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity
                    ,
                 NewMonsterPowers.DictionaryOfAncientDragonBreaths["Ancient Green Dragon"]
               },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DragonmonsterSkillProficiency_1,
                    MonstersAttributes.DragonmonsterSkillProficiency_2,
                    MonstersAttributes.GreenDragonmonsterSkillProficiency_3,
                    MonstersAttributes.GreenDragonmonsterSkillProficiency_4,
                    MonstersAttributes.GreenDragonmonsterSkillProficiency_5,

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.GreenDragonBiteAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.DragonlegendaryActionDescription_2,
                    MonstersAttributes.DragonlegendaryActionDescription
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.AncientDragon_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.GuiPresentation.SpriteReference,


            },

            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Ancient Red Dragon",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.GoldDragon_AerElai,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_AncientRedDragon",
                NewTitle = "Custom_AncientRedDragon_Title",
                NewDescription = "Custom_AncientRedDragon_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
                Alignment = "ChaoticEvil",
                ArmorClass = 22,
                HitDice = 28,
                HitDiceType = RuleDefinitions.DieType.D20,
                HitPointsBonus = 252,
                StandardHitPoints = 546,
                AttributeStrength = 30,
                AttributeDexterity = 10,
                AttributeConstitution = 29,
                AttributeIntelligence = 18,
                AttributeWisdom = 15,
                AttributeCharisma = 23,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 7,
                SavingThrowConstitution = 16,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 9,
                SavingThrowCharisma = 13,
                CR = 24,
                LegendaryCreature = true,
                Type = "Dragon",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeClimb6    ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6    ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12    ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence    ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity
                ,
                 NewMonsterPowers.DictionaryOfAncientDragonBreaths["Ancient Red Dragon"]
               },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DragonmonsterSkillProficiency_1,
                    MonstersAttributes.DragonmonsterSkillProficiency_2
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.RedDragonBiteAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.DragonlegendaryActionDescription_2,
                    MonstersAttributes.DragonlegendaryActionDescription
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.AncientDragon_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.GoldDragon_AerElai.GuiPresentation.SpriteReference,



            },

            new Models.MonsterContext.CustomMonster()
            {

                MonsterName = "Ancient White Dragon" ,
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.SilverDragon_Princess    ,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug ,
                NewName = "Custom_AncientWhiteDragon"    ,
                NewTitle = "Custom_AncientWhiteDragon_Title"  ,
                NewDescription = "Custom_AncientWhiteDragon_Description"    ,
                Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize ,
                Alignment = "ChaoticEvil"  ,
                ArmorClass = 20  ,
                HitDice = 18  ,
                HitDiceType = RuleDefinitions.DieType.D20    ,
                HitPointsBonus = 144 ,
                StandardHitPoints = 333 ,
                AttributeStrength = 26  ,
                AttributeDexterity = 10  ,
                AttributeConstitution = 26  ,
                AttributeIntelligence = 10  ,
                AttributeWisdom = 13  ,
                AttributeCharisma = 14  ,
                SavingThrowStrength = 0   ,
                SavingThrowDexterity = 6   ,
                SavingThrowConstitution = 14  ,
                SavingThrowIntelligence = 0   ,
                SavingThrowWisdom = 7   ,
                SavingThrowCharisma = 8   ,
                CR = 20  ,
                LegendaryCreature = true   ,
                Type = "Dragon"   ,
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeBurrow8   ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6    ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8 ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence    ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity
                ,
                 NewMonsterPowers.DictionaryOfAncientDragonBreaths["Ancient White Dragon"]
               },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DragonmonsterSkillProficiency_1,
                    MonstersAttributes.DragonmonsterSkillProficiency_2
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.WhiteDragonBiteAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration,
                    MonstersAttributes.DragonClawAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.DragonlegendaryActionDescription_2,
                    MonstersAttributes.DragonlegendaryActionDescription
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.AncientDragon_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.SilverDragon_Princess.GuiPresentation.SpriteReference,



            },


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Archmage",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.SRD_Mage,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_Archmage",
                NewTitle = "Custom_Archmage_Title",
                NewDescription = "Custom_Archmage_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Medium,
                Alignment = "Neutral",
                ArmorClass = 15,
                HitDice = 18,
                HitDiceType = RuleDefinitions.DieType.D8,
                HitPointsBonus = 18,
                StandardHitPoints = 99,
                AttributeStrength = 10,
                AttributeDexterity = 14,
                AttributeConstitution = 12,
                AttributeIntelligence = 20,
                AttributeWisdom = 15,
                AttributeCharisma = 16,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 0,
                SavingThrowIntelligence = 9,
                SavingThrowWisdom = 6,
                SavingThrowCharisma = 0,
                CR = 12,
                LegendaryCreature = false,
                Type = "Humanoid",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionFrightenedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance

                    ,
                     NewMonsterAttributes.CastSpell_ArchMage
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.ArchmagemonsterSkillProficiency_1,
                    MonstersAttributes.ArchmagemonsterSkillProficiency_2
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    DatabaseHelper.MonsterDefinitions.SRD_Mage.AttackIterations[0]
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.HighLevelCaster_CombatDecisions,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.SRD_Mage.GuiPresentation.SpriteReference,
            },


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Balor",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Minotaur,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Fire_Spider,
                NewName = "Custom_Balor",
                NewTitle = "Custom_Balor_Title",
                NewDescription = "Custom_Balor_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Huge,
                Alignment = "ChaoticEvil",
                ArmorClass = 19,
                HitDice = 21,
                HitDiceType = RuleDefinitions.DieType.D12,
                HitPointsBonus = 126,
                StandardHitPoints = 262,
                AttributeStrength = 26,
                AttributeDexterity = 15,
                AttributeConstitution = 22,
                AttributeIntelligence = 20,
                AttributeWisdom = 16,
                AttributeCharisma = 22,
                SavingThrowStrength = 14,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 12,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 9,
                SavingThrowCharisma = 12,
                CR = 19,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinBludgeoning,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinPiercing,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinSlashing,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionPowers.PowerPhaseSpiderTeleport,
                     DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon
                    ,
                        NewMonsterPowers.Balor_FireAura_Power,
                        NewMonsterAttributes.Balor_Retaliate_DamageAffinity,
                        NewMonsterPowers.SummonCreature_Nalfeshnee_Power
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.BalorWhipAttackIteration,
                    MonstersAttributes.BalorLongswordAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.Balor_CombatDecisions,
                GroupAttacks=true,


                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.BalorassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.MinotaurElite.GuiPresentation.SpriteReference,

            },


             new Models.MonsterContext.CustomMonster()
             {
                    MonsterName = "Deva",
                    BaseTemplateName = DatabaseHelper.MonsterDefinitions.Divine_Avatar,
                    MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Divine_Avatar,
                    NewName = "Custom_Deva",
                    NewTitle = "Custom_Deva_Title",
                    NewDescription = "Custom_Deva_Description",
                    Size = DatabaseHelper.CharacterSizeDefinitions.Medium,
                    Alignment = "LawfulGood",
                    ArmorClass = 17,
                    HitDice = 16,
                    HitDiceType = RuleDefinitions.DieType.D8,
                    HitPointsBonus = 64,
                    StandardHitPoints = 136,
                    AttributeStrength = 18,
                    AttributeDexterity = 18,
                    AttributeConstitution = 18,
                    AttributeIntelligence = 17,
                    AttributeWisdom = 20,
                    AttributeCharisma = 20,
                    SavingThrowStrength = 0,
                    SavingThrowDexterity = 0,
                    SavingThrowConstitution = 0,
                    SavingThrowIntelligence = 0,
                    SavingThrowWisdom = 9,
                    SavingThrowCharisma = 9,
                    CR = 10,
                    LegendaryCreature = false,
                    Type = "Celestial",
                    Features = new List<FeatureDefinition>()
                    {
                        DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                         DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                         DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                         DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12,
                         DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                         DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDivineAvatar_Paladin

                    },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.DevamonsterSkillProficiency_1,
                    MonstersAttributes.DevamonsterSkillProficiency_2
                },
                AttackIterations=(DatabaseHelper.MonsterDefinitions.Divine_Avatar.AttackIterations)
                ,
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.MonsterDefinitions.Divine_Avatar.DefaultBattleDecisionPackage,
                GroupAttacks=DatabaseHelper.MonsterDefinitions.Divine_Avatar.GroupAttacks,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Divine_Avatar.GuiPresentation.SpriteReference,
             },


             new Models.MonsterContext.CustomMonster()
             {
                MonsterName = "Djinni",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Fire_Jester,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Air_Elemental,
                NewName = "Custom_Djinni",
                NewTitle = "Custom_Djinni_Title",
                NewDescription = "Custom_Djinni_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "ChaoticGood",
                ArmorClass = 17,
                HitDice = 14,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 84,
                StandardHitPoints = 161,
                AttributeStrength = 21,
                AttributeDexterity = 15,
                AttributeConstitution = 22,
                AttributeIntelligence = 15,
                AttributeWisdom = 16,
                AttributeCharisma = 20,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 6,
                SavingThrowConstitution = 0,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 7,
                SavingThrowCharisma = 9,
                CR = 11,
                LegendaryCreature = false,
                Type = "Elemental",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderImmunity,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonWingAttack
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.DjinniAttackIteration,
                    MonstersAttributes.DjinniAttackIteration_2

                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.MonsterDefinitions.Air_Elemental.DefaultBattleDecisionPackage,
                GroupAttacks=false,


                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.DjinniassetReference,
                SpriteReference=DatabaseHelper.HumanoidMonsterPresentationDefinitions.NPC_Presentation_Emtan_Ghost.GuiPresentation.SpriteReference,

             },


           new Models.MonsterContext.CustomMonster()
           {
                MonsterName = "Efreeti",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Fire_Jester,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Fire_Spider,
                NewName = "Custom_Efreeti",
                NewTitle = "Custom_Efreeti_Title",
                NewDescription = "Custom_Efreeti_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulEvil",
                ArmorClass = 17,
                HitDice = 16,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 112,
                StandardHitPoints = 200,
                AttributeStrength = 22,
                AttributeDexterity = 12,
                AttributeConstitution = 24,
                AttributeIntelligence = 16,
                AttributeWisdom = 15,
                AttributeCharisma = 16,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 0,
                SavingThrowIntelligence = 7,
                SavingThrowWisdom = 6,
                SavingThrowCharisma = 7,
                CR = 11,
                LegendaryCreature = false,
                Type = "Elemental",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.EfreetiAttackIteration,
                    MonstersAttributes.EfreetiAttackIteration_2
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.MonsterDefinitions.Fire_Elemental.DefaultBattleDecisionPackage,
                GroupAttacks=false,


                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EfreetiassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Fire_Jester.GuiPresentation.SpriteReference,

           },


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Erinyes",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Estalla,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_Erinyes",
                NewTitle = "Custom_Erinyes_Title",
                NewDescription = "Custom_Erinyes_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Medium,
                Alignment = "LawfulEvil",
                ArmorClass = 18,
                HitDice = 18,
                HitDiceType = RuleDefinitions.DieType.D8,
                HitPointsBonus = 72,
                StandardHitPoints = 153,
                AttributeStrength = 18,
                AttributeDexterity = 16,
                AttributeConstitution = 18,
                AttributeIntelligence = 14,
                AttributeWisdom = 14,
                AttributeCharisma = 18,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 7,
                SavingThrowConstitution = 8,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 6,
                SavingThrowCharisma = 8,
                CR = 12,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6 ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12    ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16    ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver    ,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance    ,
                     DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon ,
                     DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePoison_QueenSpidersBlood
                    ,
                     NewMonsterPowers.ErinyesParry_Power
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.ErinyesAttackIteration,
                    MonstersAttributes.ErinyesAttackIteration_2
                },
                LegendaryActionOptions=DatabaseHelper.MonsterDefinitions.Estalla.LegendaryActionOptions,

                DefaultBattleDecisionPackage=DatabaseHelper.DecisionPackageDefinitions.DefaultRangeWithBackupMeleeDecisions,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Estalla.GuiPresentation.SpriteReference,
            },

           new Models.MonsterContext.CustomMonster()
           {

                MonsterName = "Guardian Naga",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Tiger_Drake,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_GuardianNaga",
                NewTitle = "Custom_GuardianNaga_Title",
                NewDescription = "Custom_GuardianNaga_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulGood",
                ArmorClass = 18,
                HitDice = 15,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 45,
                StandardHitPoints = 127,
                AttributeStrength = 19,
                AttributeDexterity = 18,
                AttributeConstitution = 16,
                AttributeIntelligence = 16,
                AttributeWisdom = 19,
                AttributeCharisma = 18,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 8,
                SavingThrowConstitution = 7,
                SavingThrowIntelligence = 7,
                SavingThrowWisdom = 8,
                SavingThrowCharisma = 8,
                CR = 10,
                LegendaryCreature = false,
                Type = "Monstrosity",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8 ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity   ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity  ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity
                    ,
                     NewMonsterAttributes.CastSpell_GuardianNaga
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.NagaAttackIteration_2,
                    MonstersAttributes.NagaAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.Naga_CombatDecisions,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Tiger_Drake.GuiPresentation.SpriteReference,
            },


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Horned Devil",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Assassin,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Wraith,
                NewName = "Custom_HornedDevil",
                NewTitle = "Custom_HornedDevil_Title",
                NewDescription = "Custom_HornedDevil_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulEvil",
                ArmorClass = 18,
                HitDice = 17,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 85,
                StandardHitPoints = 178,
                AttributeStrength = 22,
                AttributeDexterity = 17,
                AttributeConstitution = 21,
                AttributeIntelligence = 12,
                AttributeWisdom = 16,
                AttributeCharisma = 17,
                SavingThrowStrength = 10,
                SavingThrowDexterity = 7,
                SavingThrowConstitution = 0,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 7,
                SavingThrowCharisma = 7,
                CR = 11,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision    ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove4    ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12    ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver    ,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance    ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast    ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16
                    ,
                      DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.HornedDevilForkAttackIteration,
                    MonstersAttributes.HornedDevilForkAttackIteration,
                    MonstersAttributes.HornedDevilTailAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.DecisionPackageDefinitions.SorakShikkathAndBossCombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Assassin.GuiPresentation.SpriteReference,
            },


             new Models.MonsterContext.CustomMonster()
             {
                MonsterName = "Ice Devil",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.SkarnGhoul,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SkarnGhoul,
                NewName = "Custom_IceDevil",
                NewTitle = "Custom_IceDevil_Title",
                NewDescription = "Custom_IceDevil_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulEvil",
                ArmorClass = 18,
                HitDice = 19,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 76,
                StandardHitPoints = 180,
                AttributeStrength = 21,
                AttributeDexterity = 14,
                AttributeConstitution = 18,
                AttributeIntelligence = 18,
                AttributeWisdom = 15,
                AttributeCharisma = 18,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 7,
                SavingThrowConstitution = 9,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 7,
                SavingThrowCharisma = 9,
                CR = 14,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance
                    ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerWinterWolfBreath
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.IceDevilBiteAttackIteration,
                    MonstersAttributes.IceDevilClawAttackIteration,
                    MonstersAttributes.IceDevilTailAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.DecisionPackageDefinitions.SorakShikkathAndBossCombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.SkarnGhoul.GuiPresentation.SpriteReference,
             },


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Lich",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.MummyLord,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_Lich",
                NewTitle = "Custom_Lich_Title",
                NewDescription = "Custom_Lich_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Medium,
                Alignment = "NeutralEvil",
                ArmorClass = 17,
                HitDice = 18,
                HitDiceType = RuleDefinitions.DieType.D8,
                HitPointsBonus = 54,
                StandardHitPoints = 135,
                AttributeStrength = 11,
                AttributeDexterity = 16,
                AttributeConstitution = 16,
                AttributeIntelligence = 20,
                AttributeWisdom = 14,
                AttributeCharisma = 16,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 10,
                SavingThrowIntelligence = 12,
                SavingThrowWisdom = 9,
                SavingThrowCharisma = 0,
                CR = 21,
                LegendaryCreature = true,
                Type = "Undead",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionFrightenedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_BludgeoningImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_PiercingImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_SlashingImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity
                    ,
                     NewMonsterAttributes.CastSpell_Lich
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.LichmonsterSkillProficiency_1,
                    MonstersAttributes.LichmonsterSkillProficiency_2,
                    MonstersAttributes.LichmonsterSkillProficiency_3,
                    MonstersAttributes.LichmonsterSkillProficiency_4
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.LichAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.LichlegendaryActionDescription_0,
                    MonstersAttributes.LichlegendaryActionDescription,
                    MonstersAttributes.LichlegendaryActionDescription_2,
                    MonstersAttributes.LichlegendaryActionDescription_3,
                    MonstersAttributes.LichlegendaryActionDescription_4
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.HighLevelCaster_CombatDecisions,
                GroupAttacks=false,


                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.LichassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Adam_The_Twelth.GuiPresentation.SpriteReference,

            }       ,

            new Models.MonsterContext.CustomMonster()
            {

                MonsterName = "Nalfeshnee",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Ogre_Zombie,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                NewName = "Custom_Nalfeshnee",
                NewTitle = "Custom_Nalfeshnee_Title",
                NewDescription = "Custom_Nalfeshnee_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "ChaoticEvil",
                ArmorClass = 18,
                HitDice = 16,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 96,
                StandardHitPoints = 184,
                AttributeStrength = 21,
                AttributeDexterity = 10,
                AttributeConstitution = 22,
                AttributeIntelligence = 19,
                AttributeWisdom = 12,
                AttributeCharisma = 15,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 11,
                SavingThrowIntelligence = 9,
                SavingThrowWisdom = 6,
                SavingThrowCharisma = 7,
                CR = 13,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove4,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinBludgeoning,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinPiercing,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityStoneskinSlashing,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence,
                     DatabaseHelper.FeatureDefinitionPowers.PowerPhaseSpiderTeleport
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.NalfeshneeBiteAttackIteration,
                    MonstersAttributes.NalfeshneeClawAttackIteration,
                    MonstersAttributes.NalfeshneeClawAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.Nalfeshnee_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Ogre_Zombie.GuiPresentation.SpriteReference,
            }       ,

            new Models.MonsterContext.CustomMonster()
            {

                MonsterName = "Pit Fiend",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Abomination,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.PhaseSpider,
                NewName = "Custom_PitFiend",
                NewTitle = "Custom_PitFiend_Title",
                NewDescription = "Custom_PitFiend_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulEvil",
                ArmorClass = 19,
                HitDice = 24,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 168,
                StandardHitPoints = 300,
                AttributeStrength = 26,
                AttributeDexterity = 14,
                AttributeConstitution = 24,
                AttributeIntelligence = 22,
                AttributeWisdom = 18,
                AttributeCharisma = 24,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 8,
                SavingThrowConstitution = 13,
                SavingThrowIntelligence = 0,
                SavingThrowWisdom = 10,
                SavingThrowCharisma = 0,
                CR = 20,
                LegendaryCreature = false,
                Type = "Fiend",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6  ,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16    ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver    ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity ,
                     DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance    ,
                     DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence    ,
                     DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon
                    ,
                        NewMonsterPowers.AtWillAOE_Fireball_Power,
                        NewMonsterPowers.LimitedPerDayTargetDebuff_HoldMonster_Power,
                        NewMonsterPowers.LimitedPerDayAOE_WallOfFire_Power,
                        NewMonsterPowers.SummonCreature_Erinyes_Power
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {

                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.PitFiendBiteAttackIteration,
                    MonstersAttributes.PitFiendClawAttackIteration,
                    MonstersAttributes.PitFiendTailAttackIteration,
                    MonstersAttributes.PitFiendWeaponAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.PitFiend_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Abomination.GuiPresentation.SpriteReference,
            },

              new Models.MonsterContext.CustomMonster()
              {

                    MonsterName = "Planetar",
                    BaseTemplateName = DatabaseHelper.MonsterDefinitions.Divine_Avatar_Wizard,
                    MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Ghost_Wolf,
                    NewName = "Custom_Planetar",
                    NewTitle = "Custom_Planetar_Title",
                    NewDescription = "Custom_Planetar_Description",
                    Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                    Alignment = "LawfulGood",
                    ArmorClass = 19,
                    HitDice = 16,
                    HitDiceType = RuleDefinitions.DieType.D10,
                    HitPointsBonus = 112,
                    StandardHitPoints = 200,
                    AttributeStrength = 24,
                    AttributeDexterity = 20,
                    AttributeConstitution = 24,
                    AttributeIntelligence = 19,
                    AttributeWisdom = 22,
                    AttributeCharisma = 25,
                    SavingThrowStrength = 0,
                    SavingThrowDexterity = 0,
                    SavingThrowConstitution = 12,
                    SavingThrowIntelligence = 0,
                    SavingThrowWisdom = 11,
                    SavingThrowCharisma = 12,
                    CR = 16,
                    LegendaryCreature = false,
                    Type = "Celestial",
                    Features = new List<FeatureDefinition>()
                    {
                        DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                         DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                         DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                         DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12,
                         DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                         DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDivineAvatar_Wizard

                    },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.PlanetarmonsterSkillProficiency_1
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.PlanetarLongswordAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.MonsterDefinitions.Divine_Avatar_Cleric.DefaultBattleDecisionPackage,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Divine_Avatar_Wizard.GuiPresentation.SpriteReference,
              } ,

             new Models.MonsterContext.CustomMonster()
             {

                    MonsterName = "Roc",
                    BaseTemplateName = DatabaseHelper.MonsterDefinitions.Giant_Eagle,
                    MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                    NewName = "Custom_Roc",
                    NewTitle = "Custom_Roc_Title",
                    NewDescription = "Custom_Roc_Description",
                    Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
                    Alignment = "Unaligned",
                    ArmorClass = 15,
                    HitDice = 16,
                    HitDiceType = RuleDefinitions.DieType.D20,
                    HitPointsBonus = 80,
                    StandardHitPoints = 248,
                    AttributeStrength = 28,
                    AttributeDexterity = 10,
                    AttributeConstitution = 20,
                    AttributeIntelligence = 3,
                    AttributeWisdom = 10,
                    AttributeCharisma = 9,
                    SavingThrowStrength = 0,
                    SavingThrowDexterity = 4,
                    SavingThrowConstitution = 9,
                    SavingThrowIntelligence = 0,
                    SavingThrowWisdom = 4,
                    SavingThrowCharisma = 3,
                    CR = 11,
                    LegendaryCreature = false,
                    Type = "Monstrosity",
                    Features = new List<FeatureDefinition>()
                    {
                        DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                         DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove4,
                         DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                         DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                         DatabaseHelper.FeatureDefinitionPowers.PowerRemorhazSwallow
                            ,
                    },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.RocmonsterSkillProficiency_1
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.RocBiteAttackIteration,
                    MonstersAttributes.RocClawAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.DecisionPackageDefinitions.RemorhazCombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Giant_Eagle.GuiPresentation.SpriteReference,
            }       ,


            new Models.MonsterContext.CustomMonster()
            {
                MonsterName = "Solar",
                BaseTemplateName = DatabaseHelper.MonsterDefinitions.Divine_Avatar_Cleric,
                MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Fire_Elemental,
                NewName = "Custom_Solar",
                NewTitle = "Custom_Solar_Title",
                NewDescription = "Custom_Solar_Description",
                Size = DatabaseHelper.CharacterSizeDefinitions.Large,
                Alignment = "LawfulGood",
                ArmorClass = 21,
                HitDice = 18,
                HitDiceType = RuleDefinitions.DieType.D10,
                HitPointsBonus = 144,
                StandardHitPoints = 243,
                AttributeStrength = 26,
                AttributeDexterity = 22,
                AttributeConstitution = 26,
                AttributeIntelligence = 25,
                AttributeWisdom = 25,
                AttributeCharisma = 30,
                SavingThrowStrength = 0,
                SavingThrowDexterity = 0,
                SavingThrowConstitution = 0,
                SavingThrowIntelligence = 14,
                SavingThrowWisdom = 14,
                SavingThrowCharisma = 17,
                CR = 21,
                LegendaryCreature = true,
                Type = "Celestial",
                Features = new List<FeatureDefinition>()
                {
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                     DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                     DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDivineAvatar_Cleric
                        ,
                     NewMonsterPowers.AtWillSelfBuff_Invisibility_Power
                },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.SolarmonsterSkillProficiency_1
                },
                AttackIterations=new List<MonsterAttackIteration> ()
                {
                    MonstersAttributes.SolarLongswordAttackIteration,
                    MonstersAttributes.SolarLongbowAttackIteration
                },
                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.SolarlegendaryActionDescription,
                    MonstersAttributes.SolarlegendaryActionDescription_2,
                    MonstersAttributes.SolarlegendaryActionDescription_3
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.Solar_CombatDecisions,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Divine_Avatar_Cleric.GuiPresentation.SpriteReference,
            }       ,

/*
             new Models.MonsterContext.CustomMonster()
             {
                    MonsterName = "Storm Giant",
                    BaseTemplateName = DatabaseHelper.MonsterDefinitions.Giant_Frost,
                    MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                    NewName = "Custom_StormGiant",
                    NewTitle = "Custom_StormGiant_Title",
                    NewDescription = "Custom_StormGiant_Description",
                    Size = DatabaseHelper.CharacterSizeDefinitions.Huge,
                    Alignment = "ChaoticGood",
                    ArmorClass = 16,
                    HitDice = 20,
                    HitDiceType = RuleDefinitions.DieType.D12,
                    HitPointsBonus = 100,
                    StandardHitPoints = 230,
                    AttributeStrength = 29,
                    AttributeDexterity = 14,
                    AttributeConstitution = 20,
                    AttributeIntelligence = 16,
                    AttributeWisdom = 18,
                    AttributeCharisma = 18,
                    SavingThrowStrength = 14,
                    SavingThrowDexterity = 0,
                    SavingThrowConstitution = 10,
                    SavingThrowIntelligence = 0,
                    SavingThrowWisdom = 9,
                    SavingThrowCharisma = 9,
                    CR = 13,
                    LegendaryCreature = false,
                    Type = "Giant",
                    Features = new List<FeatureDefinition>()
                    {
                        DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10,
                         DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningImmunity,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderImmunity,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance
                        ,
                         NewMonsterPowers.Generic_Lightning_Attack_Power
                    },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.StormGiantmonsterSkillProficiency_1,
                    MonstersAttributes.StormGiantmonsterSkillProficiency_2,
                    MonstersAttributes.StormGiantmonsterSkillProficiency_3,
                    MonstersAttributes.StormGiantmonsterSkillProficiency_4
                },
                AttackIterations=DatabaseHelper.MonsterDefinitions.Giant_Frost.AttackIterations,

                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {

                },

                DefaultBattleDecisionPackage=DatabaseHelper.DecisionPackageDefinitions.SorakShikkathAndBossCombatDecisions,
                GroupAttacks=false,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Giant_Frost.GuiPresentation.SpriteReference,
             }      ,
*/
            new Models.MonsterContext.CustomMonster()
            {
               MonsterName = "Tarrasque",
               BaseTemplateName = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Abomination,
               MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Abomination,
               NewName = "Custom_Tarrasque",
               NewTitle = "Custom_Tarrasque_Title",
               NewDescription = "Custom_Tarrasque_Description",
               Size = DatabaseHelper.CharacterSizeDefinitions.DragonSize,
               Alignment = "Unaligned",
               ArmorClass = 25,
               HitDice = 33,
               HitDiceType = RuleDefinitions.DieType.D20,
               HitPointsBonus = 330,
               StandardHitPoints = 676,
               AttributeStrength = 30,
               AttributeDexterity = 11,
               AttributeConstitution = 30,
               AttributeIntelligence = 3,
               AttributeWisdom = 11,
               AttributeCharisma = 11,
               SavingThrowStrength = 0,
               SavingThrowDexterity = 0,
               SavingThrowConstitution = 0,
               SavingThrowIntelligence = 5,
               SavingThrowWisdom = 9,
               SavingThrowCharisma = 9,
               CR = 30,
               LegendaryCreature = true,
               Type = "Monstrosity",
               Features = new List<FeatureDefinition>()
               {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8 ,
                 DatabaseHelper.FeatureDefinitionSenses.SenseTremorsense16    ,
                 DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight12   ,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity ,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity ,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_BludgeoningImmunity,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_PiercingImmunity,
                 DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_SlashingImmunity,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity ,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionFrightenedImmunity,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity ,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                 DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                 DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,
                 DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence,
                 NewMonsterPowers.TarrasqueSwallowPower,
                 NewMonsterAttributes.TarrasqueReflectiveCarapace
               },

                SkillScores = new List<MonsterSkillProficiency>()
                {
                },
                AttackIterations = new List<MonsterAttackIteration>()
                {
                    MonstersAttributes.TarrasqueBiteAttackIteration,
                    MonstersAttributes.TarrasqueClawAttackIteration,
                    MonstersAttributes.TarrasqueClawAttackIteration,
                    MonstersAttributes.TarrasqueHornAttackIteration,
                    MonstersAttributes.TarrasqueTailAttackIteration,
                },
                LegendaryActionOptions = new List<LegendaryActionDescription>()
                {
                   MonstersAttributes.TarrasquelegendaryActionDescription,
                   MonstersAttributes.TarrasquelegendaryActionDescription_2,
                   MonstersAttributes.TarrasquelegendaryActionDescription_3,
                 //  MonstersAttributes.TarrasquelegendaryActionDescription_4,
                },

                DefaultBattleDecisionPackage = NewMonsterAttributes.Tarrasque_CombatDecisions,
                GroupAttacks = true,


                PhantomDistortion = true,
                AttachedParticlesReference = MonstersAttributes.EmptyassetReference,
                SpriteReference = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Abomination.GuiPresentation.SpriteReference,


            },

             new Models.MonsterContext.CustomMonster()
             {

                    MonsterName = "Vampire",
                    BaseTemplateName = DatabaseHelper.MonsterDefinitions.Razan,
                    MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SRD_Thug,
                    NewName = "Custom_Vampire",
                    NewTitle = "Custom_Vampire_Title",
                    NewDescription = "Custom_Vampire_Description",
                    Size = DatabaseHelper.CharacterSizeDefinitions.Medium,
                    Alignment = "LawfulEvil",
                    ArmorClass = 16,
                    HitDice = 17,
                    HitDiceType = RuleDefinitions.DieType.D8,
                    HitPointsBonus = 68,
                    StandardHitPoints = 144,
                    AttributeStrength = 18,
                    AttributeDexterity = 18,
                    AttributeConstitution = 18,
                    AttributeIntelligence = 17,
                    AttributeWisdom = 15,
                    AttributeCharisma = 18,
                    SavingThrowStrength = 0,
                    SavingThrowDexterity = 9,
                    SavingThrowConstitution = 0,
                    SavingThrowIntelligence = 0,
                    SavingThrowWisdom = 7,
                    SavingThrowCharisma = 9,
                    CR = 13,
                    LegendaryCreature = true,
                    Type = "Undead",
                    Features = new List<FeatureDefinition>()
                    {
                        DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6,
                         DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                         DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                         DatabaseHelper.FeatureDefinitionLightAffinitys.LightAffinityHypersensitivity,
                         DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                         DatabaseHelper.FeatureDefinitionSenses.SenseTremorsense16,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                         DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                         DatabaseHelper.FeatureDefinitionPowers.PowerDefilerDarkness,
                         DatabaseHelper.FeatureDefinitionPowers.PowerDefilerMistyFormEscape,
                         DatabaseHelper.FeatureDefinitionLightAffinitys.LightAffinity_Defiler_Darkness,
                         DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionImmuneToShine,
                         DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityVeilImmunity,
                         DatabaseHelper.FeatureDefinitionRegenerations.RegenerationVampire,
                         DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySpiderClimb
                        ,
                          NewMonsterPowers.SummonCreature_Wolves_Power
                    },

                SkillScores=new List<MonsterSkillProficiency> ()
                {
                    MonstersAttributes.VampiremonsterSkillProficiency_1,
                    MonstersAttributes.VampiremonsterSkillProficiency_2
                },
                AttackIterations=DatabaseHelper.MonsterDefinitions.Razan.AttackIterations ,

                LegendaryActionOptions=new List<LegendaryActionDescription> ()
                {
                    MonstersAttributes.VampirelegendaryActionDescription,
                    MonstersAttributes.VampirelegendaryActionDescription_2,
                    MonstersAttributes.VampirelegendaryActionDescription_3
                },

                DefaultBattleDecisionPackage=NewMonsterAttributes.Vampire_CombatDecisions,
                GroupAttacks=true,

                PhantomDistortion=true,
                AttachedParticlesReference=MonstersAttributes.EmptyassetReference,
                SpriteReference=DatabaseHelper.MonsterDefinitions.Brood_of_blood.GuiPresentation.SpriteReference,
             }
        };



        public static void EnableInDungeonMaker()
        {
            for (int i = 0; i < Definitions.Count; i++)
            {

                /* original version that worked just fine
                 * 
                 * MonsterDefinitionBuilder NewMonster = new MonsterDefinitionBuilder(
                    Definitions[i].NewName,
                    GuidHelper.Create(new System.Guid(Settings.GUID), Definitions[i].NewName).ToString(),
                    "Monster/&" + "DH_" + Definitions[i].NewTitle,
                    "Monster/&" + "DH_" + Definitions[i].NewDescription,
                    Definitions[i].BaseTemplateName);
                */

                MonsterDefinitionBuilder NewMonster = MonsterDefinitionBuilder
                        .Create(
                            Definitions[i].BaseTemplateName, Definitions[i].NewName, 
                            DefinitionBuilder.CENamespaceGuid)
                        .SetGuiPresentation(
                            "Monster/&" + "DH_" + Definitions[i].NewTitle, 
                            "Monster/&" + "DH_" + Definitions[i].NewDescription, 
                            Definitions[i].BaseTemplateName.GuiPresentation.SpriteReference);


                NewMonster.SetInDungeonEditor(false);
                if (SolastaCommunityExpansion.Main.Settings.EnableExtraHighLevelMonsters)
                {
                    NewMonster.SetInDungeonEditor(true);
                };

                NewMonster.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
                NewMonster.SetSizeDefinition(Definitions[i].Size);
                NewMonster.SetChallengeRating(Definitions[i].CR);
                NewMonster.SetAlignment(Definitions[i].Alignment);
                NewMonster.SetCharacterFamily(Definitions[i].Type);
                NewMonster.SetLegendaryCreature(Definitions[i].LegendaryCreature);

                NewMonster.SetArmorClass(Definitions[i].ArmorClass);
                NewMonster.SetHitDiceNumber(Definitions[i].HitDice);
                NewMonster.SetHitDiceType(Definitions[i].HitDiceType);
                NewMonster.SetHitPointsBonus(Definitions[i].HitPointsBonus);
                NewMonster.SetStandardHitPoints(Definitions[i].StandardHitPoints);

                NewMonster.ClearFeatures();
                NewMonster.AddFeatures(Definitions[i].Features);

                NewMonster.ClearAbilityScores();
                NewMonster.SetAbilityScores(
                    Definitions[i].AttributeStrength,
                    Definitions[i].AttributeDexterity,
                    Definitions[i].AttributeConstitution,
                    Definitions[i].AttributeIntelligence,
                    Definitions[i].AttributeWisdom,
                    Definitions[i].AttributeCharisma);

                NewMonster.SetDefaultBattleDecisionPackage(NewMonsterAttributes.Titan_CombatDecisions);

                MonsterSavingThrowProficiency Str_save = new MonsterSavingThrowProficiency();
                Str_save.SetField("abilityScoreName", "Strength");
                Str_save.SetField("bonus", Definitions[i].SavingThrowStrength);

                MonsterSavingThrowProficiency Dex_save = new MonsterSavingThrowProficiency();
                Dex_save.SetField("abilityScoreName", "Dexterity");
                Dex_save.SetField("bonus", Definitions[i].SavingThrowDexterity);

                MonsterSavingThrowProficiency Con_save = new MonsterSavingThrowProficiency();
                Con_save.SetField("abilityScoreName", "Constitution");
                Con_save.SetField("bonus", Definitions[i].SavingThrowConstitution);

                MonsterSavingThrowProficiency Int_save = new MonsterSavingThrowProficiency();
                Int_save.SetField("abilityScoreName", "Intelligence");
                Int_save.SetField("bonus", Definitions[i].SavingThrowIntelligence);

                MonsterSavingThrowProficiency Wis_save = new MonsterSavingThrowProficiency();
                Wis_save.SetField("abilityScoreName", "Wisdom");
                Wis_save.SetField("bonus", Definitions[i].SavingThrowWisdom);

                MonsterSavingThrowProficiency Cha_save = new MonsterSavingThrowProficiency();
                Cha_save.SetField("abilityScoreName", "Charisma");
                Cha_save.SetField("bonus", Definitions[i].SavingThrowCharisma);

                NewMonster.ClearSavingThrowScores();
                NewMonster.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
                {
                    Str_save,
                    Dex_save,
                    Con_save,
                    Int_save,
                    Wis_save,
                    Cha_save
                });


                NewMonster.ClearSkillScores();
                NewMonster.AddSkillScores(Definitions[i].SkillScores);

                NewMonster.ClearAttackIterations();
                NewMonster.AddAttackIterations(Definitions[i].AttackIterations);

                NewMonster.SetDefaultBattleDecisionPackage(Definitions[i].DefaultBattleDecisionPackage);
                NewMonster.SetGroupAttacks(Definitions[i].GroupAttacks);

                NewMonster.ClearLegendaryActionOptions();
                NewMonster.AddLegendaryActionOptions(Definitions[i].LegendaryActionOptions);


                NewMonster.SetSpriteReference(Definitions[i].SpriteReference);
                NewMonster.SetHasPhantomDistortion(Definitions[i].PhantomDistortion);
                NewMonster.SetAttachedParticlesReference(Definitions[i].AttachedParticlesReference);


                NewMonster.SetNoExperienceGain(false);
                NewMonster.SetHasMonsterPortraitBackground(true);
                NewMonster.SetCanGeneratePortrait(true);
                NewMonster.SetCustomShaderReference(Definitions[i].MonsterShaderReference.MonsterPresentation.CustomShaderReference);


                if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Large)
                {
                    NewMonster.SetModelScale(1);
                }

                if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Huge)
                {
                    NewMonster.SetModelScale(1.5f);
                }

                if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Gargantuan)
                {
                    NewMonster.SetModelScale(2f);
                }
                if (Definitions[i].MonsterName == "Roc")
                {
                    NewMonster.SetModelScale(4);

                    NewMonster.SetHasPrefabVariants(false);

                }
                if (Definitions[i].MonsterName == "Ancient Green Dragon")
                {
                    DatabaseHelper.MonsterPresentationDefinitions.Green_Dragon_Presentation.SetModelScale(0.75f);


                }
                if (Definitions[i].MonsterName == "Ancient Blue Dragon")
                {
                    NewMonster.SetSpriteReference(DatabaseHelper.MonsterDefinitions.Young_BlackDragon.GuiPresentation.SpriteReference);


                }
                if (Definitions[i].MonsterName == "Planetar" || Definitions[i].MonsterName == "Solar")
                {
                    // only way to give planetar and solar correct visual sizes as you can't scale up humanoid presentation, need to switch to monster prefab
                    //    AssetReference assetReference = new AssetReference();
                    //    assetReference.SetField("m_AssetGUID", "2351ec8d0f454e04ebcc2826e16f6ed5");
                    //
                    //    NewMonster.MonsterPresentation.SetMalePrefabReference(assetReference);
                    //    NewMonster.MonsterPresentation.SetFemalePrefabReference(assetReference);
                    //    NewMonster.MonsterPresentation.SetHumanoidMonsterPresentationDefinitions(DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.HumanoidMonsterPresentationDefinitions);

                    NewMonster.SetModelScale(0.66f);
                }
                if (Definitions[i].MonsterName == "Djinni")
                {
                    NewMonster.SetBestiarySpriteReference(DatabaseHelper.MonsterDefinitions.Ghost.GuiPresentation.SpriteReference);
                    NewMonster.SetModelScale(1.5f);
                }
                if (Definitions[i].MonsterName == "Efreeti")
                {

                    NewMonster.SetModelScale(1.5f);
                }
                if (Definitions[i].MonsterName == "Lich")
                {
                    AssetReference assetReference = new AssetReference();
                    assetReference.SetField("m_AssetGUID", "cab8992a98c1d3749bc0a50d53fcc378");


                    NewMonster.SetPrefabReference(assetReference);
                }
                if (Definitions[i].MonsterName == "Tarrasque")
                {

                    NewMonster.SetModelScale(4.5f);

                    // monster powres, AI and combat needs work, too repetitive at the moment as some powers/attacks are not triggering
                    NewMonster.SetInDungeonEditor(false);
                }
                MonsterDefinition monster= NewMonster.AddToDB();

                // temporary fix until builder is changed
                monster.CreatureTags.Clear();
            }
        }
    }
}
