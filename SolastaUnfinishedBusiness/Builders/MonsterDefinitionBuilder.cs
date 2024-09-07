using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;
using static BestiaryDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class MonsterDefinitionBuilder : DefinitionBuilder<MonsterDefinition, MonsterDefinitionBuilder>
{
    internal const string NeutralAlignment = "Neutral";

    internal MonsterDefinitionBuilder SetGroupAttacks(bool value)
    {
        Definition.groupAttacks = value;
        return this;
    }

    internal MonsterDefinitionBuilder SetAlignment(string name)
    {
        Definition.alignment = name;
        return this;
    }

    internal MonsterDefinitionBuilder SetArmorClass(int armorClass, string type = "")
    {
        Definition.armorClass = armorClass;
        Definition.armor = type;
        return this;
    }

    internal MonsterDefinitionBuilder SetBestiaryEntry(BestiaryEntry entry)
    {
        Definition.bestiaryEntry = entry;
        return this;
    }

    internal MonsterDefinitionBuilder SetChallengeRating(float challengeRating)
    {
        Definition.challengeRating = challengeRating;
        return this;
    }

    internal MonsterDefinitionBuilder SetCharacterFamily(string family)
    {
        Definition.characterFamily = family;
        return this;
    }

    public MonsterDefinitionBuilder SetMonsterPresentation(MonsterPresentation presentation)
    {
        Definition.monsterPresentation = presentation;
        return this;
    }

    public MonsterDefinitionBuilder SetSavingThrowScores(params (string, int)[] saves)
    {
        Definition.savingThrowScores.SetRange(saves.Select(x =>
            new MonsterSavingThrowProficiency { abilityScoreName = x.Item1, bonus = x.Item2 }));
        return this;
    }

    public MonsterDefinitionBuilder NoExperienceGain()
    {
        Definition.noExperienceGain = true;
        return this;
    }

#if false
    internal MonsterDefinitionBuilder SetDefaultBattleDecisionPackage(DecisionPackageDefinition decisionPackage)
    {
        Definition.defaultBattleDecisionPackage = decisionPackage;
        return this;
    }
#endif

    internal MonsterDefinitionBuilder SetDefaultFaction(FactionDefinition faction)
    {
        Definition.defaultFaction = faction.name;
        return this;
    }

    internal MonsterDefinitionBuilder SetDroppedLootDefinition(LootPackDefinition lootPack)
    {
        Definition.droppedLootDefinition = lootPack;
        return this;
    }

    internal MonsterDefinitionBuilder SetFullyControlledWhenAllied(bool value)
    {
        Definition.fullyControlledWhenAllied = value;
        return this;
    }

    internal MonsterDefinitionBuilder SetDungeonMakerPresence(MonsterDefinition.DungeonMaker presence)
    {
        Definition.dungeonMakerPresence = presence;
        return this;
    }

    internal MonsterDefinitionBuilder SetHitDice(DieType dieType, int hitDice)
    {
        Definition.hitDiceType = dieType;
        Definition.hitDice = hitDice;
        return this;
    }

#if false
    internal MonsterDefinitionBuilder SetHitDiceNumber(int hitDice)
    {
        Definition.hitDice = hitDice;
        return this;
    }

    internal MonsterDefinitionBuilder SetHitDiceType(DieType dieType)
    {
        Definition.hitDiceType = dieType;
        return this;
    }
#endif

    internal MonsterDefinitionBuilder SetHitPointsBonus(int bonus)
    {
        Definition.hitPointsBonus = bonus;
        return this;
    }

    internal MonsterDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition sizeDefinition)
    {
        Definition.sizeDefinition = sizeDefinition;
        return this;
    }

    internal MonsterDefinitionBuilder SetStandardHitPoints(int hp)
    {
        Definition.standardHitPoints = hp;
        return this;
    }

    internal MonsterDefinitionBuilder SetAbilityScores(params int[] a)
    {
        Array.Clear(Definition.AbilityScores, 0, Definition.AbilityScores.Length);

        // must follow the std order STR, DEX, CON, INT, WIS, CHA
        for (var i = 0; i <= 5; i++)
        {
            Definition.AbilityScores.SetValue(a[i], i);
        }

        return this;
    }

    internal MonsterDefinitionBuilder AddFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.AddRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    internal MonsterDefinitionBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    internal MonsterDefinitionBuilder SetSkillScores(params (string skillName, int bonus)[] skillScores)
    {
        Definition.SkillScores.SetRange(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
        Definition.SkillScores.Sort(Sorting.Compare);
        return this;
    }

    internal MonsterDefinitionBuilder ClearAttackIterations()
    {
        Definition.AttackIterations.Clear();
        return this;
    }

    internal MonsterDefinitionBuilder SetAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
    {
        Definition.AttackIterations.SetRange(monsterAttackIterations);
        return this;
    }

    internal MonsterDefinitionBuilder SetAttackIterations(params (int, MonsterAttackDefinition)[] iterations)
    {
        Definition.AttackIterations.SetRange(iterations.Select(x =>
            new MonsterAttackIteration { number = x.Item1, monsterAttackDefinition = x.Item2 }));
        return this;
    }

    internal MonsterDefinitionBuilder SetCreatureTags(params string[] tags)
    {
        Definition.CreatureTags.SetRange(tags);
        return this;
    }

    internal MonsterDefinitionBuilder AddCreatureTags(params string[] tags)
    {
        Definition.CreatureTags.AddRange(tags);
        return this;
    }

    internal MonsterDefinitionBuilder SetHeight(int height)
    {
        Definition.height = height;
        return this;
    }

    #region Constructors

    protected MonsterDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MonsterDefinitionBuilder(MonsterDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
