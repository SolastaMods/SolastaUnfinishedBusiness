using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaCommunityExpansion.Models
{
    internal static class MonsterContext
    {
       internal  class CopyAndCreateNewBlueprint<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : BaseDefinition
       {
           internal CopyAndCreateNewBlueprint(string name, string guid, string title_string, string description_string, TDefinition base_Blueprint) : base(base_Blueprint, name, guid)
           {
               Definition.GuiPresentation.SetTitle(title_string);
               Definition.GuiPresentation.SetDescription(description_string);
           }
           public static TDefinition CreateCopy(string name, string guid, string title_string, string description_string, TDefinition base_Blueprint)
           {
               return new CopyAndCreateNewBlueprint<TDefinition>(name, guid, title_string, description_string, base_Blueprint).AddToDB();
           }
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
            public TA.AI.DecisionPackageDefinition DefaultBattleDecisionPackage;
            public bool GroupAttacks;
            public bool PhantomDistortion;
            public AssetReference AttachedParticlesReference;
            public AssetReferenceSprite SpriteReference;

        }

        public static readonly List<MonsterDefinition> ModdedMonsters = new List<MonsterDefinition>();

        public static void AddNewMonsters()
        {
            if (Main.Settings.EnableExtraHighLevelMonsters)
            {
                //following order of new blueprint creation should be maintained
                Monsters.NewMonsterSpells.Create();
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
}
