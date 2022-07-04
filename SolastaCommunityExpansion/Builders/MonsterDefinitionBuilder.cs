using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;
using static BestiaryDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Builders;

public class MonsterDefinitionBuilder : DefinitionBuilder<MonsterDefinition, MonsterDefinitionBuilder>
{
    public MonsterDefinitionBuilder SetSpriteReference(AssetReferenceSprite sprite)
    {
        Definition.GuiPresentation.spriteReference = sprite;
        return this;
    }

    public MonsterDefinitionBuilder SetAlignment(string alignment)
    {
        Definition.alignment = alignment;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetAlwaysHideStats(bool value)
    {
        Definition.alwaysHideStats = value;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetArmor(string armor)
    {
        Definition.armor = armor;
        return this;
    }

    public MonsterDefinitionBuilder SetArmorClass(int armorClass)
    {
        Definition.armorClass = armorClass;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetAudioRaceRTPCValue(float audioRaceRTPCValue)
    {
        Definition.audioRaceRTPCValue = audioRaceRTPCValue;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetBestiaryEntry(BestiaryEntry entry)
    {
        Definition.bestiaryEntry = entry;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetBestiaryReference(MonsterDefinition monster)
    {
        Definition.bestiaryReference = monster;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetBestiarySpriteReference(AssetReferenceSprite sprite)
    {
        Definition.bestiarySpriteReference = sprite;
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

#if false
    public MonsterDefinitionBuilder SetDifferentActionEachTurn(bool value)
    {
        Definition.differentActionEachTurn = value;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetDroppedLootDefinition(LootPackDefinition lootPack)
    {
        Definition.droppedLootDefinition = lootPack;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetDualSex(bool value)
    {
        Definition.dualSex = value;
        return this;
    }

    public MonsterDefinitionBuilder SetFollowFloorAngle(bool value)
    {
        Definition.followFloorAngle = value;
        return this;
    }

    public MonsterDefinitionBuilder SetForceCombatStartsAnimation(bool value)
    {
        Definition.forceCombatStartsAnimation = value;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetForceNoFlyAnimation(bool value)
    {
        Definition.forceNoFlyAnimation = value;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetForcePersistentBody(bool value)
    {
        Definition.forcePersistentBody = value;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetFullyControlledWhenAllied(bool value)
    {
        Definition.fullyControlledWhenAllied = value;
        return this;
    }

    public MonsterDefinitionBuilder SetGroupAttacks(bool value)
    {
        Definition.groupAttacks = value;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetHasLookAt(bool value)
    {
        Definition.hasLookAt = value;
        return this;
    }

    public MonsterDefinitionBuilder SetHeight(int Height)
    {
        Definition.height = Height;
        return this;
    }
#endif

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

    public MonsterDefinitionBuilder SetHitDice(DieType dieType, int numberOf)
    {
        Definition.hitDiceType = dieType;
        Definition.hitDice = numberOf;
        return this;
    }

    public MonsterDefinitionBuilder SetHitPointsBonus(int bonus)
    {
        Definition.hitPointsBonus = bonus;
        return this;
    }

    public MonsterDefinitionBuilder SetInDungeonEditor(bool value)
    {
        if (value)
        {
            Definition.dungeonMakerPresence = MonsterDefinition.DungeonMaker.Monster;
        }
        else
        {
            Definition.dungeonMakerPresence = MonsterDefinition.DungeonMaker.None;
        }

        return this;
    }

#if false
    public MonsterDefinitionBuilder SetInterceptStance(MoveStance interceptStance)
    {
        Definition.interceptStance = interceptStance;
        return this;
    }

    public MonsterDefinitionBuilder SetIsHusk(bool value)
    {
        Definition.isHusk = value;
        return this;
    }

    public MonsterDefinitionBuilder SetIsUnique(bool value)
    {
        Definition.isUnique = value;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetLegendaryCreature(bool value)
    {
        Definition.legendaryCreature = value;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetMaximalAge(int Age)
    {
        Definition.maximalAge = Age;
        return this;
    }

    public MonsterDefinitionBuilder SetMaxLegendaryActionPoints(int maxLegendaryActionPoints)
    {
        Definition.maxLegendaryActionPoints = maxLegendaryActionPoints;
        return this;
    }

    public MonsterDefinitionBuilder SetMaxLegendaryResistances(int maxLegendaryResistances)
    {
        Definition.maxLegendaryResistances = maxLegendaryResistances;
        return this;
    }

    public MonsterDefinitionBuilder SetMinimalAge(int minimalAge)
    {
        Definition.minimalAge = minimalAge;
        return this;
    }

    public MonsterDefinitionBuilder SetMonsterPresentation(MonsterPresentation monsterPresentation)
    {
        Definition.monsterPresentation = monsterPresentation;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetNoExperienceGain(bool value)
    {
        Definition.noExperienceGain = value;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetPatrolStance(MoveStance patrolStance)
    {
        Definition.patrolStance = patrolStance;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition sizeDefinition)
    {
        Definition.sizeDefinition = sizeDefinition;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetSneakStance(MoveStance SneakStance)
    {
        Definition.sneakStance = SneakStance;
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetStandardHitPoints(int Hp)
    {
        Definition.standardHitPoints = Hp;
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetStealableLootDefinition(LootPackDefinition stealableLoot)
    {
        Definition.stealableLootDefinition = stealableLoot;
        return this;
    }

    public MonsterDefinitionBuilder SetThreatEvaluatorDefinition(ThreatEvaluatorDefinition threatEvaluator)
    {
        Definition.threatEvaluatorDefinition = threatEvaluator;
        return this;
    }

    public MonsterDefinitionBuilder SetUniqueNameId(string uniqueName)
    {
        Definition.uniqueNameId = uniqueName;
        return this;
    }

    public MonsterDefinitionBuilder SetWeight(int weight)
    {
        Definition.weight = weight;
        return this;
    }
#endif

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

    public MonsterDefinitionBuilder ClearSkillScores()
    {
        Definition.SkillScores.Clear();
        return this;
    }

#if false
    public MonsterDefinitionBuilder AddSkillScores(params (string skillName, int bonus)[] skillScores)
    {
        return AddSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
    }

    public MonsterDefinitionBuilder AddSkillScores(params MonsterSkillProficiency[] skillScores)
    {
        return AddSkillScores(skillScores.AsEnumerable());
    }
#endif

    public MonsterDefinitionBuilder AddSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
    {
        Definition.SkillScores.AddRange(skillScores);
        Definition.SkillScores.Sort(Sorting.Compare);
        return this;
    }

    public MonsterDefinitionBuilder SetSkillScores(params (string skillName, int bonus)[] skillScores)
    {
        return SetSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
    }

#if false
    public MonsterDefinitionBuilder SetSkillScores(params MonsterSkillProficiency[] skillScores)
    {
        return SetSkillScores(skillScores.AsEnumerable());
    }
#endif

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

#if false
    public MonsterDefinitionBuilder AddAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
    {
        return AddAttackIterations(monsterAttackIterations.AsEnumerable());
    }
#endif

    public MonsterDefinitionBuilder AddAttackIterations(IEnumerable<MonsterAttackIteration> monsterAttackIterations)
    {
        Definition.AttackIterations.AddRange(monsterAttackIterations);
        Definition.AttackIterations.Sort(Sorting.Compare);
        return this;
    }

    public MonsterDefinitionBuilder SetAttackIterations(params MonsterAttackDefinition[] monsterAttackIterations)
    {
        return SetAttackIterations(monsterAttackIterations.Select(
            d => new MonsterAttackIteration(d, 1))
        );
    }

#if false
    public MonsterDefinitionBuilder SetAttackIterations(
        params (MonsterAttackDefinition, int)[] monsterAttackIterations)
    {
        return SetAttackIterations(monsterAttackIterations.Select(
            d => new MonsterAttackIteration(d.Item1, d.Item2)
        ));
    }
#endif

    public MonsterDefinitionBuilder SetAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
    {
        return SetAttackIterations(monsterAttackIterations.AsEnumerable());
    }

    public MonsterDefinitionBuilder SetAttackIterations(IEnumerable<MonsterAttackIteration> monsterAttackIterations)
    {
        Definition.AttackIterations.SetRange(monsterAttackIterations);
        return this;
    }

    public MonsterDefinitionBuilder ClearLegendaryActionOptions()
    {
        Definition.LegendaryActionOptions.Clear();
        return this;
    }

#if false
    public MonsterDefinitionBuilder AddLegendaryActionOptions(
        params LegendaryActionDescription[] legendaryActionDescriptions)
    {
        return AddLegendaryActionOptions(legendaryActionDescriptions.AsEnumerable());
    }
#endif

    public MonsterDefinitionBuilder AddLegendaryActionOptions(
        IEnumerable<LegendaryActionDescription> legendaryActionDescriptions)
    {
        Definition.LegendaryActionOptions.AddRange(legendaryActionDescriptions);
        Definition.LegendaryActionOptions.Sort(Sorting.Compare);
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetLegendaryActionOptions(
        params LegendaryActionDescription[] legendaryActionDescriptions)
    {
        return SetLegendaryActionOptions(legendaryActionDescriptions.AsEnumerable());
    }
#endif

    public MonsterDefinitionBuilder SetLegendaryActionOptions(
        IEnumerable<LegendaryActionDescription> legendaryActionDescriptions)
    {
        Definition.LegendaryActionOptions.SetRange(legendaryActionDescriptions);
        return this;
    }

    public MonsterDefinitionBuilder SetHasPhantomDistortion(bool value)
    {
        Definition.MonsterPresentation.hasPhantomDistortion = value;
        return this;
    }

    public MonsterDefinitionBuilder SetAttachedParticlesReference(AssetReference assetReference)
    {
        Definition.MonsterPresentation.attachedParticlesReference = assetReference;
        return this;
    }

    public MonsterDefinitionBuilder ClearSavingThrowScores()
    {
        Definition.SavingThrowScores.Clear();
        return this;
    }

    public MonsterDefinitionBuilder ClearCreatureTags()
    {
        Definition.CreatureTags.Clear();
        return this;
    }

    public MonsterDefinitionBuilder SetSavingThrowScores(
        params (string attributeName, int bonus)[] savingThrowScores)
    {
        Definition.SavingThrowScores.Clear();
        return AddSavingThrowScores(savingThrowScores
            .Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
    }

#if false
    public MonsterDefinitionBuilder AddSavingThrowScores(
        params (string attributeName, int bonus)[] savingThrowScores)
    {
        return AddSavingThrowScores(savingThrowScores
            .Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
    }

    public MonsterDefinitionBuilder AddSavingThrowScores(params MonsterSavingThrowProficiency[] savingThrowScores)
    {
        return AddSavingThrowScores(savingThrowScores.AsEnumerable());
    }
#endif

    public MonsterDefinitionBuilder AddSavingThrowScores(
        IEnumerable<MonsterSavingThrowProficiency> savingThrowScores)
    {
        Definition.SavingThrowScores.AddRange(savingThrowScores);
        Definition.SavingThrowScores.Sort(Sorting.Compare);
        return this;
    }

    public MonsterDefinitionBuilder SetHasMonsterPortraitBackground(bool value)
    {
        Definition.MonsterPresentation.hasMonsterPortraitBackground = value;
        return this;
    }

    public MonsterDefinitionBuilder SetCanGeneratePortrait(bool value)
    {
        Definition.MonsterPresentation.canGeneratePortrait = value;
        return this;
    }

    public MonsterDefinitionBuilder SetCustomShaderReference(AssetReference shaderReference)
    {
        Definition.MonsterPresentation.customShaderReference = shaderReference;
        return this;
    }

    public MonsterDefinitionBuilder SetModelScale(float scale)
    {
        Definition.MonsterPresentation.femaleModelScale = scale;
        Definition.MonsterPresentation.maleModelScale = scale;
        return this;
    }

    public MonsterDefinitionBuilder SetCreatureTags(params string[] tags)
    {
        Definition.CreatureTags.SetRange(tags);
        return this;
    }

#if false
    public MonsterDefinitionBuilder SetCreatureTags(IEnumerable<string> tags)
    {
        Definition.CreatureTags.SetRange(tags);
        return this;
    }
#endif

    public MonsterDefinitionBuilder SetPrefabReference(AssetReference assetReference)
    {
        Definition.MonsterPresentation.malePrefabReference = assetReference;
        Definition.MonsterPresentation.femalePrefabReference = assetReference;
        return this;
    }

    public MonsterDefinitionBuilder SetHasPrefabVariants(bool value)
    {
        Definition.MonsterPresentation.hasPrefabVariants = value;
        return this;
    }

    public MonsterDefinitionBuilder SetUseCustomMaterials(bool value)
    {
        Definition.MonsterPresentation.useCustomMaterials = value;
        return this;
    }

    public MonsterDefinitionBuilder SetCustomMaterials(AssetReference[] assetReference)
    {
        Definition.MonsterPresentation.customMaterials = assetReference;
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
