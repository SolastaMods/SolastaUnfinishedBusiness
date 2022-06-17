using System.Collections.Generic;
using SolastaCommunityExpansion;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaMonsters.Models;
using UnityEngine.AddressableAssets;

//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters;

internal static class MonstersHomebrew
{
    public static List<MonsterContext.CustomMonster> Definitions = new()
    {
        new MonsterContext.CustomMonster
        {
            MonsterName = "Air Titan",
            CR = 23,
            LegendaryCreature = true,
            BaseTemplateName = DatabaseHelper.MonsterDefinitions.Air_Elemental,
            MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Air_Elemental,
            NewName = "Custom_AirTitan",
            NewTitle = "Custom_AirTitan_Title",
            NewDescription = "Custom_AirTitan_Description",
            Size = DatabaseHelper.CharacterSizeDefinitions.Gargantuan,
            Alignment = DatabaseHelper.AlignmentDefinitions.Neutral.Name,
            ArmorClass = 19,
            HitDice = 16,
            HitDiceType = RuleDefinitions.DieType.D20,
            HitPointsBonus = 96,
            StandardHitPoints = 264,
            AttributeStrength = 23,
            AttributeDexterity = 28,
            AttributeConstitution = 23,
            AttributeIntelligence = 2,
            AttributeWisdom = 21,
            AttributeCharisma = 18,
            SavingThrowStrength = 0,
            SavingThrowDexterity = 0,
            SavingThrowConstitution = 0,
            SavingThrowIntelligence = 0,
            SavingThrowWisdom = 12,
            SavingThrowCharisma = 11,
            Type = DatabaseHelper.CharacterFamilyDefinitions.Elemental.Name,
            Features = new List<FeatureDefinition>
            {
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseTremorsense16,
                DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityGrappledImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPetrifiedImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityUnconsciousImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                DatabaseHelper.FeatureDefinitionAttackModifiers
                    .AttackModifierMartialSpellBladeMagicWeapon,
                DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderImmunity
            }
        },
        new MonsterContext.CustomMonster
        {
            MonsterName = "Earth Titan",
            CR = 20,
            LegendaryCreature = true,
            BaseTemplateName = DatabaseHelper.MonsterDefinitions.Golem_Clay,
            MonsterShaderReference = DatabaseHelper.MonsterDefinitions.SkarnGhoul,
            NewName = "Custom_EarthTitan",
            NewTitle = "Custom_EarthTitan_Title",
            NewDescription = "Custom_EarthTitan_Description",
            Size = DatabaseHelper.CharacterSizeDefinitions.Gargantuan,
            Alignment = DatabaseHelper.AlignmentDefinitions.Neutral.Name,
            ArmorClass = 21,
            HitDice = 27,
            HitDiceType = RuleDefinitions.DieType.D20,
            HitPointsBonus = 243,
            StandardHitPoints = 526,
            AttributeStrength = 30,
            AttributeDexterity = 10,
            AttributeConstitution = 30,
            AttributeIntelligence = 2,
            AttributeWisdom = 21,
            AttributeCharisma = 18,
            SavingThrowStrength = 14,
            SavingThrowDexterity = 0,
            SavingThrowConstitution = 15,
            SavingThrowIntelligence = 0,
            SavingThrowWisdom = 12,
            SavingThrowCharisma = 0,
            Type = DatabaseHelper.CharacterFamilyDefinitions.Elemental.Name,
            Features = new List<FeatureDefinition>
            {
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseTremorsense16,
                DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeBurrow8,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPetrifiedImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityUnconsciousImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                DatabaseHelper.FeatureDefinitionAttackModifiers
                    .AttackModifierMartialSpellBladeMagicWeapon
            }
        },
        new MonsterContext.CustomMonster
        {
            MonsterName = "Fire Titan",
            CR = 23,
            LegendaryCreature = true,
            BaseTemplateName = DatabaseHelper.MonsterDefinitions.Fire_Elemental,
            MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Fire_Elemental,
            NewName = "Custom_FireTitan",
            NewTitle = "Custom_FireTitan_Title",
            NewDescription = "Custom_FireTitan_Description",
            Size = DatabaseHelper.CharacterSizeDefinitions.Gargantuan,
            Alignment = DatabaseHelper.AlignmentDefinitions.Neutral.Name,
            ArmorClass = 16,
            HitDice = 22,
            HitDiceType = RuleDefinitions.DieType.D20,
            HitPointsBonus = 110,
            StandardHitPoints = 241,
            AttributeStrength = 21,
            AttributeDexterity = 22,
            AttributeConstitution = 20,
            AttributeIntelligence = 2,
            AttributeWisdom = 10,
            AttributeCharisma = 19,
            SavingThrowStrength = 0,
            SavingThrowDexterity = 0,
            SavingThrowConstitution = 12,
            SavingThrowIntelligence = 0,
            SavingThrowWisdom = 7,
            SavingThrowCharisma = 11,
            Type = DatabaseHelper.CharacterFamilyDefinitions.Elemental.Name,
            Features = new List<FeatureDefinition>
            {
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseTremorsense16,
                DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityGrappledImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPetrifiedImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityUnconsciousImmunity,
                DatabaseHelper.FeatureDefinitionLightSources.LightSourceFireElemental,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                DatabaseHelper.FeatureDefinitionAttackModifiers
                    .AttackModifierMartialSpellBladeMagicWeapon
            }
        },
        new MonsterContext.CustomMonster
        {
            MonsterName = "Construct Titan",
            CR = 25,
            LegendaryCreature = true,
            BaseTemplateName = DatabaseHelper.MonsterDefinitions.Golem_Iron,
            MonsterShaderReference = DatabaseHelper.MonsterDefinitions.Golem_Iron,
            NewName = "Custom_ConstructTitan",
            NewTitle = "Custom_ConstructTitan_Title",
            NewDescription = "Custom_ConstructTitan_Description",
            Size = DatabaseHelper.CharacterSizeDefinitions.Gargantuan,
            Alignment = DatabaseHelper.AlignmentDefinitions.Neutral.Name,
            ArmorClass = 23,
            HitDice = 20,
            HitDiceType = RuleDefinitions.DieType.D20,
            HitPointsBonus = 200,
            StandardHitPoints = 410,
            AttributeStrength = 30,
            AttributeDexterity = 11,
            AttributeConstitution = 30,
            AttributeIntelligence = 3,
            AttributeWisdom = 11,
            AttributeCharisma = 8,
            SavingThrowStrength = 0,
            SavingThrowDexterity = 0,
            SavingThrowConstitution = 0,
            SavingThrowIntelligence = 4,
            SavingThrowWisdom = 8,
            SavingThrowCharisma = 7,
            Type = DatabaseHelper.CharacterFamilyDefinitions.Elemental.Name,
            Features = new List<FeatureDefinition>
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove10,
                DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys
                    .ConditionAffinityCalmEmotionCharmedImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys
                    .ConditionAffinityCharmImmunityHypnoticPattern,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys
                    .ConditionAffinityProtectedFromEvilCharmImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPetrifiedImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,
                DatabaseHelper.FeatureDefinitionAttackModifiers
                    .AttackModifierMartialSpellBladeMagicWeapon,
                NewMonsterPowers.IncreasedGravityZone_Power,
                NewMonsterPowers.DisintegratingBeam_Power
            }
        }
    };

    public static void EnableInDungeonMaker()
    {
        for (var i = 0; i < Definitions.Count; i++)
        {
            /*
            MonsterDefinitionBuilder NewMonster = new MonsterDefinitionBuilder(
                Definitions[i].NewName,
                GuidHelper.Create(new System.Guid(Settings.GUID), Definitions[i].NewName).ToString(),
                "Monster/&" + "DH_" + Definitions[i].NewTitle,
                "Monster/&" + "DH_" + Definitions[i].NewDescription,
                Definitions[i].BaseTemplateName);
            */

            var NewMonster = MonsterDefinitionBuilder
                .Create(
                    Definitions[i].BaseTemplateName, Definitions[i].NewName,
                    DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(
                    "Monster/&" + "DH_" + Definitions[i].NewTitle,
                    "Monster/&" + "DH_" + Definitions[i].NewDescription,
                    Definitions[i].BaseTemplateName.GuiPresentation.SpriteReference);

            NewMonster.SetInDungeonEditor(false);
            if (Main.Settings.EnableExtraHighLevelMonsters)
            {
                NewMonster.SetInDungeonEditor(Main.Settings.EnableExtraHighLevelMonsters);
            }

            ;

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

            MonsterSavingThrowProficiency Str_save = new();
            Str_save.abilityScoreName = AttributeDefinitions.Strength;
            Str_save.bonus = Definitions[i].SavingThrowStrength;

            MonsterSavingThrowProficiency Dex_save = new();
            Dex_save.abilityScoreName = AttributeDefinitions.Dexterity;
            Dex_save.bonus = Definitions[i].SavingThrowDexterity;

            MonsterSavingThrowProficiency Con_save = new();
            Con_save.abilityScoreName = AttributeDefinitions.Constitution;
            Con_save.bonus = Definitions[i].SavingThrowConstitution;

            MonsterSavingThrowProficiency Int_save = new();
            Int_save.abilityScoreName = AttributeDefinitions.Intelligence;
            Int_save.bonus = Definitions[i].SavingThrowIntelligence;

            MonsterSavingThrowProficiency Wis_save = new();
            Wis_save.abilityScoreName = AttributeDefinitions.Wisdom;
            Wis_save.bonus = Definitions[i].SavingThrowWisdom;

            MonsterSavingThrowProficiency Cha_save = new();
            Cha_save.abilityScoreName = AttributeDefinitions.Charisma;
            Cha_save.bonus = Definitions[i].SavingThrowCharisma;

            NewMonster.ClearSavingThrowScores();
            NewMonster.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>
            {
                Str_save,
                Dex_save,
                Con_save,
                Int_save,
                Wis_save,
                Cha_save
            });

            NewMonster.ClearSkillScores();

            if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Large)
            {
                NewMonster.SetModelScale(1);
            }

            else if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Huge)
            {
                NewMonster.SetModelScale(1.5f);
            }

            else if (Definitions[i].Size == DatabaseHelper.CharacterSizeDefinitions.Gargantuan)
            {
                NewMonster.SetModelScale(2f);
            }

            if (Definitions[i].MonsterName == "Air Titan")
            {
                NewMonster.SetModelScale(4f);
                NewMonster.SetSpriteReference(DatabaseHelper.MonsterDefinitions.KindredSpiritEagle.GuiPresentation
                    .SpriteReference);
                //AssetReference assetReference = new AssetReference();
                //assetReference.m_AssetGUID =  "3f4ea7931d5f6ba4e96caace01b265b7";
                //NewMonster.SetPrefabReference(assetReference);
                AssetReference assetReference = new();
                assetReference.SetField("m_AssetGUID", "b1bd642eab224bd4bbbf5ce48c869a9e");
                NewMonster.SetPrefabReference(assetReference);

                NewMonster.AddFeatures(new List<FeatureDefinition>
                {
                    NewMonsterPowers.Dictionaryof_SummoningElementals["Air_Elemental"],
                    NewMonsterPowers.Dictionaryof_SummoningElementals["WindSnake"],
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration,
                    NewMonsterAttributes.AirTitan_SleetStorm_Immunity,
                    NewMonsterPowers.AirTitan_LightningStorm_Attack_Power
                });

                MonsterAttackIteration AttackIteration = new();
                AttackIteration.monsterAttackDefinition = NewMonsterAttacks.AirTitan_Slam_Attack;
                AttackIteration.number = 2;

                NewMonster.ClearAttackIterations();
                NewMonster.AddAttackIterations(new List<MonsterAttackIteration> {AttackIteration});

                LegendaryActionDescription legendaryActionDescription = new();
                legendaryActionDescription.cost = 1;
                legendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription.featureDefinitionPower = NewMonsterPowers.AirTitan_Lightning_Power;
                legendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryDragonAttack;

                LegendaryActionDescription legendaryActionDescription_2 = new();
                legendaryActionDescription_2.cost = 1;
                legendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.Spell;
                legendaryActionDescription_2.spellDefinition = DatabaseHelper.SpellDefinitions.SleetStorm;
                legendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryAoE_Debuff;

                LegendaryActionDescription legendaryActionDescription_3 = new();
                legendaryActionDescription_3.cost = 2;
                legendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription_3.featureDefinitionPower = NewMonsterPowers.AirTitan_Gale_Power;
                legendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryDragonWingAttack;

                NewMonster.ClearLegendaryActionOptions();
                NewMonster.AddLegendaryActionOptions(new List<LegendaryActionDescription>
                {
                    legendaryActionDescription, legendaryActionDescription_2, legendaryActionDescription_3
                });
            }

            ;

            if (Definitions[i].MonsterName == "Earth Titan")
            {
                //AssetReference assetReference = new AssetReference();
                //assetReference.m_AssetGUID =  "aad57f1f96869a3409a5c064473c454d";
                //NewMonster.SetPrefabReference(assetReference);

                NewMonster.AddFeatures(new List<FeatureDefinition>
                {
                    NewMonsterPowers.Dictionaryof_SummoningElementals["Earth_Elemental"],
                    NewMonsterPowers.Dictionaryof_SummoningElementals["SkarnGhoul"],
                    NewMonsterPowers.EarthTitan_Earthquake_Power
                });

                MonsterAttackIteration AttackIteration = new();
                AttackIteration.monsterAttackDefinition = NewMonsterAttacks.EarthTitan_Boulder_Attack;
                AttackIteration.number = 2;

                MonsterAttackIteration AttackIteration_2 = new();
                AttackIteration_2.monsterAttackDefinition = NewMonsterAttacks.EarthTitan_Slam_Attack;
                AttackIteration_2.number = 2;

                NewMonster.ClearAttackIterations();
                NewMonster.AddAttackIterations(
                    new List<MonsterAttackIteration> {AttackIteration, AttackIteration_2});

                LegendaryActionDescription legendaryActionDescription_1 = new();
                legendaryActionDescription_1.cost = 1;
                legendaryActionDescription_1.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription_1.featureDefinitionPower = NewMonsterPowers.IlluminatingCrystals_Power;
                legendaryActionDescription_1.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryAoE_Debuff;

                LegendaryActionDescription legendaryActionDescription_2 = new();
                legendaryActionDescription_2.cost = 1;
                legendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
                legendaryActionDescription_2.monsterAttackDefinition = NewMonsterAttacks
                    .EarthTitan_Boulder_Attack;
                legendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryDragonAttack;

                LegendaryActionDescription legendaryActionDescription_3 = new();
                legendaryActionDescription_3.cost = 2;
                legendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription_3.featureDefinitionPower = NewMonsterPowers
                    .EarthTitan_Earthquake_Power;
                legendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryAoE_DpS;

                NewMonster.AddLegendaryActionOptions(new List<LegendaryActionDescription>
                {
                    legendaryActionDescription_1, legendaryActionDescription_3
                });

                AssetReference assetReference = new();
                assetReference.SetField("m_AssetGUID", "0ff0b1c4180816e468ec3dcab4b18c35");
                NewMonster.SetPrefabReference(assetReference);

                NewMonster.SetUseCustomMaterials(true);
                NewMonster.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Golem_Clay_Presentation
                    .CustomMaterials);
            }

            ;

            if (Definitions[i].MonsterName == "Fire Titan")
            {
                AssetReference assetReference = new();
                assetReference.SetField("m_AssetGUID", "0829dcc3d2af9764582f0c4f3b70c914");

                NewMonster.SetPrefabReference(assetReference);

                NewMonster.AddFeatures(new List<FeatureDefinition>
                {
                    NewMonsterPowers.Dictionaryof_SummoningElementals["Fire_Elemental"],
                    NewMonsterPowers.Dictionaryof_SummoningElementals["Fire_Jester"],
                    NewMonsterAttributes.FireTitan_Retaliate_DamageAffinity,
                    NewMonsterPowers.FireTitan_Aura_Power
                });

                MonsterAttackIteration AttackIteration = new();
                AttackIteration.monsterAttackDefinition = NewMonsterAttacks.FireTitan_Slam_Attack;
                AttackIteration.number = 3;

                NewMonster.ClearAttackIterations();
                NewMonster.AddAttackIterations(new List<MonsterAttackIteration> {AttackIteration});

                LegendaryActionDescription legendaryActionDescription_2 = new();
                legendaryActionDescription_2.cost = 1;
                legendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.Spell;
                legendaryActionDescription_2.spellDefinition = DatabaseHelper.SpellDefinitions.WallOfFireRing_Outer;
                legendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryAoE_DpS;

                LegendaryActionDescription legendaryActionDescription = new();
                legendaryActionDescription.cost = 1;
                legendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription.featureDefinitionPower = NewMonsterPowers.AtWillAOE_Fireball_Power;
                legendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryDefilerAoE_DpS;

                NewMonster.AddLegendaryActionOptions(new List<LegendaryActionDescription>
                {
                    legendaryActionDescription_2, legendaryActionDescription
                });
            }

            ;

            if (Definitions[i].MonsterName == "Construct Titan")
            {
                MonsterAttackIteration AttackIteration = new();
                AttackIteration.monsterAttackDefinition = NewMonsterAttacks.ConstructTitan_Slam_Attack;
                AttackIteration.number = 1;

                MonsterAttackIteration AttackIteration_2 = new();
                AttackIteration_2.monsterAttackDefinition =
                    NewMonsterAttacks.ConstructTitan_ForceCannon_Attack;
                AttackIteration_2.number = 2;

                NewMonster.ClearAttackIterations();
                NewMonster.AddAttackIterations(
                    new List<MonsterAttackIteration> {AttackIteration}); //  , AttackIteration_2, AttackIteration_2 });
                NewMonster.SetGroupAttacks(true);
                NewMonster.ClearLegendaryActionOptions();

                LegendaryActionDescription legendaryActionDescription = new();
                legendaryActionDescription.cost = 1;
                legendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
                legendaryActionDescription.monsterAttackDefinition = NewMonsterAttacks
                    .ConstructTitan_ForceCannon_Attack;
                legendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryMummyLordAttack_Default;

                LegendaryActionDescription legendaryActionDescription2 = new();
                legendaryActionDescription2.cost = 2;
                legendaryActionDescription2.subaction = LegendaryActionDescription.SubactionType.Power;
                legendaryActionDescription2.featureDefinitionPower = NewMonsterPowers.DisintegratingBeam_Power;
                legendaryActionDescription2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
                    .LegendaryLaetharCast_Debuff;

                NewMonster.AddLegendaryActionOptions(
                    new List<LegendaryActionDescription> {legendaryActionDescription, legendaryActionDescription2});

                NewMonster.AddFeatures(new List<FeatureDefinition>
                {
                    DatabaseHelper.FeatureDefinitionPowers.PowerDragonFrightfulPresence,
                    NewMonsterPowers.DisintegratingBeam_Power
                });


                AssetReference assetReference = new();
                assetReference.SetField("m_AssetGUID", "0ff0b1c4180816e468ec3dcab4b18c35");
                NewMonster.SetPrefabReference(assetReference);

                NewMonster.SetUseCustomMaterials(true);
                NewMonster.SetCustomMaterials(DatabaseHelper.MonsterPresentationDefinitions.Golem_Iron_Presentation
                    .CustomMaterials);


                NewMonster.SetDefaultBattleDecisionPackage(NewMonsterAttributes.ConstructTitan_CombatDecisions);


                // monster powres, AI and combat needs work, too repetitive at the moment as some powers/attacks are not triggering
                NewMonster.SetInDungeonEditor(false);
            }

            ;

            NewMonster.SetNoExperienceGain(false);
            NewMonster.SetHasMonsterPortraitBackground(true);
            NewMonster.SetCanGeneratePortrait(true);
            NewMonster.SetCustomShaderReference(Definitions[i].MonsterShaderReference.MonsterPresentation
                .CustomShaderReference);
            NewMonster.SetHasPrefabVariants(false);
        }
    }
}
