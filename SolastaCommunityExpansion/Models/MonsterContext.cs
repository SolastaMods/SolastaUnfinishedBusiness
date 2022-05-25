using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaMonsters.Models
{
    internal static class MonsterContext
    {
        internal static string GUID = "d825a5ac-75bf-4ffa-81d3-b8c8b5f5d2a3";

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
            public TA.AI.DecisionPackageDefinition DefaultBattleDecisionPackage;
            public bool GroupAttacks;
            public bool PhantomDistortion;
            public AssetReference AttachedParticlesReference;
            public AssetReferenceSprite SpriteReference;
        }

        public static void Load()
        {
            if (!SolastaCommunityExpansion.Main.Settings.EnableExtraHighLevelMonsters)
            {
                return;
            }

            SolastaCommunityExpansion.Utils.Translations.LoadTranslations("Monsters");

            //following order of new blueprint creation should be maintained
            Monsters.NewMonsterAttributes.Create();
            Monsters.NewMonsterAttacks.Create();
            Monsters.NewMonsterPowers.Create();

            Monsters.MonstersHomebrew.EnableInDungeonMaker();
            Monsters.MonstersSolasta.EnableInDungeonMaker();

            Monsters.MonstersAttributes.EnableInDungeonMaker();
            Monsters.MonstersSRD.EnableInDungeonMaker();
        }
    }
}
