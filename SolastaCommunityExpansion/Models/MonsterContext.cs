using System.Collections.Generic;
using SolastaCommunityExpansion;
using SolastaMonsters.Monsters;
using TA.AI;
using UnityEngine.AddressableAssets;

namespace SolastaMonsters.Models;

internal static class MonsterContext
{
    internal static string GUID = "d825a5ac-75bf-4ffa-81d3-b8c8b5f5d2a3";

    public static void LateLoad()
    {
        if (!Main.Settings.EnableExtraHighLevelMonsters)
        {
            return;
        }

        //following order of new blueprint creation should be maintained
        NewMonsterAttributes.Create();
        NewMonsterAttacks.Create();
        NewMonsterPowers.Create();

        MonstersHomebrew.EnableInDungeonMaker();
        MonstersSolasta.EnableInDungeonMaker();

        MonstersAttributes.EnableInDungeonMaker();
        MonstersSRD.EnableInDungeonMaker();
    }

    internal struct CustomMonster
    {
        public string MonsterName;
        public MonsterDefinition BaseTemplateName;
        public MonsterDefinition MonsterShaderReference;
        public string NewName;
        public string NewTitle;
        public string NewDescription;
        public CharacterSizeDefinition Size;
        public string Alignment;
        public int ArmorClass;
        public int HitDice;
        public RuleDefinitions.DieType HitDiceType;
        public int HitPointsBonus;
        public int StandardHitPoints;
        public int AttributeStrength;
        public int AttributeDexterity;
        public int AttributeConstitution;
        public int AttributeIntelligence;
        public int AttributeWisdom;
        public int AttributeCharisma;
        public int SavingThrowStrength;
        public int SavingThrowDexterity;
        public int SavingThrowConstitution;
        public int SavingThrowIntelligence;
        public int SavingThrowWisdom;
        public int SavingThrowCharisma;
        public float CR;
        public bool LegendaryCreature;
        public string Type;
        public List<FeatureDefinition> Features;
        public List<MonsterSkillProficiency> SkillScores;
        public List<MonsterAttackIteration> AttackIterations;
        public List<LegendaryActionDescription> LegendaryActionOptions;
        public DecisionPackageDefinition DefaultBattleDecisionPackage;
        public bool GroupAttacks;
        public bool PhantomDistortion;
        public AssetReference AttachedParticlesReference;
        public AssetReferenceSprite SpriteReference;
    }
}
