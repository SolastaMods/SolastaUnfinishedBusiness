using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using TA.AI;
using static BestiaryDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class MonsterDefinitionBuilder : DefinitionBuilder<MonsterDefinition, MonsterDefinitionBuilder>
{
    public MonsterDefinitionBuilder SetAlignment(string alignment)
    {
        Definition.alignment = alignment;
        return this;
    }

    public MonsterDefinitionBuilder SetArmorClass(int armorClass)
    {
        Definition.armorClass = armorClass;
        return this;
    }

    public MonsterDefinitionBuilder SetBestiaryEntry(BestiaryEntry entry)
    {
        Definition.bestiaryEntry = entry;
        return this;
    }

    public MonsterDefinitionBuilder SetChallengeRating(float challengeRating)
    {
        Definition.challengeRating = challengeRating;
        return this;
    }

    public MonsterDefinitionBuilder SetCharacterFamily(string family)
    {
        Definition.characterFamily = family;
        return this;
    }

    public MonsterDefinitionBuilder SetDefaultBattleDecisionPackage(DecisionPackageDefinition decisionPackage)
    {
        Definition.defaultBattleDecisionPackage = decisionPackage;
        return this;
    }

    public MonsterDefinitionBuilder SetDefaultFaction(string faction)
    {
        Definition.defaultFaction = faction;
        return this;
    }

    public MonsterDefinitionBuilder SetDroppedLootDefinition(LootPackDefinition lootPack)
    {
        Definition.droppedLootDefinition = lootPack;
        return this;
    }

    public MonsterDefinitionBuilder SetFullyControlledWhenAllied(bool value)
    {
        Definition.fullyControlledWhenAllied = value;
        return this;
    }

    public MonsterDefinitionBuilder SetHitDiceNumber(int hitDice)
    {
        Definition.hitDice = hitDice;
        return this;
    }

    public MonsterDefinitionBuilder SetHitDiceType(DieType dieType)
    {
        Definition.hitDiceType = dieType;
        return this;
    }

    public MonsterDefinitionBuilder SetHitPointsBonus(int bonus)
    {
        Definition.hitPointsBonus = bonus;
        return this;
    }

    public MonsterDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition sizeDefinition)
    {
        Definition.sizeDefinition = sizeDefinition;
        return this;
    }

    public MonsterDefinitionBuilder SetStandardHitPoints(int Hp)
    {
        Definition.standardHitPoints = Hp;
        return this;
    }

    public MonsterDefinitionBuilder ClearAbilityScores()
    {
        Array.Clear(Definition.AbilityScores, 0, Definition.AbilityScores.Length);
        return this;
    }

    public MonsterDefinitionBuilder SetAbilityScores(int STR, int DEX, int CON, int INT, int WIS, int CHA)
    {
        ClearAbilityScores();

        Definition.AbilityScores.SetValue(STR, 0); // STR
        Definition.AbilityScores.SetValue(DEX, 1); // DEX
        Definition.AbilityScores.SetValue(CON, 2); // CON
        Definition.AbilityScores.SetValue(INT, 3); // INT
        Definition.AbilityScores.SetValue(WIS, 4); // WIS
        Definition.AbilityScores.SetValue(CHA, 5); // CHA
        return this;
    }

    public MonsterDefinitionBuilder ClearFeatures()
    {
        Definition.Features.Clear();
        return this;
    }

    public MonsterDefinitionBuilder AddFeatures(params FeatureDefinition[] features)
    {
        return AddFeatures(features.AsEnumerable());
    }

    public MonsterDefinitionBuilder AddFeatures(IEnumerable<FeatureDefinition> features)
    {
        Definition.Features.AddRange(features);
        return this;
    }

    public MonsterDefinitionBuilder SetFeatures(params FeatureDefinition[] features)
    {
        return SetFeatures(features.AsEnumerable());
    }

    public MonsterDefinitionBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    public MonsterDefinitionBuilder SetSkillScores(params (string skillName, int bonus)[] skillScores)
    {
        return SetSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
    }

    public MonsterDefinitionBuilder SetSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
    {
        Definition.SkillScores.SetRange(skillScores);
        Definition.SkillScores.Sort(Sorting.Compare);
        return this;
    }

    public MonsterDefinitionBuilder ClearAttackIterations()
    {
        Definition.AttackIterations.Clear();
        return this;
    }

    public MonsterDefinitionBuilder SetAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
    {
        return SetAttackIterations(monsterAttackIterations.AsEnumerable());
    }

    public MonsterDefinitionBuilder SetAttackIterations(IEnumerable<MonsterAttackIteration> monsterAttackIterations)
    {
        Definition.AttackIterations.SetRange(monsterAttackIterations);
        return this;
    }

    public MonsterDefinitionBuilder SetCreatureTags(params string[] tags)
    {
        Definition.CreatureTags.SetRange(tags);
        return this;
    }

    #region Constructors

    protected MonsterDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MonsterDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected MonsterDefinitionBuilder(MonsterDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected MonsterDefinitionBuilder(MonsterDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
