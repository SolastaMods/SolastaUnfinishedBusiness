using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static BestiaryDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Builders
{
    public class MonsterDefinitionBuilder : DefinitionBuilder<MonsterDefinition, MonsterDefinitionBuilder>
    {
        protected MonsterDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected MonsterDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected MonsterDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected MonsterDefinitionBuilder(MonsterDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected MonsterDefinitionBuilder(MonsterDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected MonsterDefinitionBuilder(MonsterDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        protected MonsterDefinitionBuilder(MonsterDefinition original) : base(original)
        {
        }

        public static MonsterDefinitionBuilder Create(MonsterDefinition original, string name, string guid)
        {
            return new MonsterDefinitionBuilder(original, name, guid);
        }

        public static MonsterDefinitionBuilder Create(MonsterDefinition original, string name, Guid guidNamespace)
        {
            return new MonsterDefinitionBuilder(original, name, guidNamespace);
        }

        public MonsterDefinitionBuilder SetAlignment(string alignment)
        {
            Definition.SetAlignment(alignment);
            return this;
        }
        public MonsterDefinitionBuilder SetAlwaysHideStats(bool value)
        {
            Definition.SetAlwaysHideStats(value);
            return this;
        }
        public MonsterDefinitionBuilder SetArmor(string armor)
        {
            Definition.SetArmor(armor);
            return this;
        }
        public MonsterDefinitionBuilder SetArmorClass(int armorClass)
        {
            Definition.SetArmorClass(armorClass);
            return this;
        }
        public MonsterDefinitionBuilder SetAudioRaceRTPCValue(float audioRaceRTPCValue)
        {
            Definition.SetAudioRaceRTPCValue(audioRaceRTPCValue);
            return this;
        }
        public MonsterDefinitionBuilder SetBestiaryEntry(BestiaryEntry entry)
        {
            Definition.SetBestiaryEntry(entry);
            return this;
        }
        public MonsterDefinitionBuilder SetBestiaryReference(MonsterDefinition monster)
        {
            Definition.SetBestiaryReference(monster);
            return this;
        }
        public MonsterDefinitionBuilder SetBestiarySpriteReference(AssetReferenceSprite sprite)
        {
            Definition.SetBestiarySpriteReference(sprite);
            return this;
        }
        public MonsterDefinitionBuilder SetChallengeRating(float challengeRating)
        {
            Definition.SetChallengeRating(challengeRating);
            return this;
        }
        public MonsterDefinitionBuilder SetCharacterFamily(string family)
        {
            Definition.SetCharacterFamily(family);
            return this;
        }
        public MonsterDefinitionBuilder SetDefaultBattleDecisionPackage(DecisionPackageDefinition decisionPackage)
        {
            Definition.SetDefaultBattleDecisionPackage(decisionPackage);
            return this;
        }
        public MonsterDefinitionBuilder SetDefaultFaction(string faction)
        {
            Definition.SetDefaultFaction(faction);
            return this;
        }
        public MonsterDefinitionBuilder SetDifferentActionEachTurn(bool value)
        {
            Definition.SetDifferentActionEachTurn(value);
            return this;
        }
        public MonsterDefinitionBuilder SetDroppedLootDefinition(LootPackDefinition lootPack)
        {
            Definition.SetDroppedLootDefinition(lootPack);
            return this;
        }
        public MonsterDefinitionBuilder SetDualSex(bool value)
        {
            Definition.SetDualSex(value);
            return this;
        }
        public MonsterDefinitionBuilder SetFollowFloorAngle(bool value)
        {
            Definition.SetFollowFloorAngle(value);
            return this;
        }
        public MonsterDefinitionBuilder SetForceCombatStartsAnimation(bool value)
        {
            Definition.SetForceCombatStartsAnimation(value);
            return this;
        }
        public MonsterDefinitionBuilder SetForceNoFlyAnimation(bool value)
        {
            Definition.SetForceNoFlyAnimation(value);
            return this;
        }
        public MonsterDefinitionBuilder SetForcePersistentBody(bool value)
        {
            Definition.SetForcePersistentBody(value);
            return this;
        }
        public MonsterDefinitionBuilder SetFullyControlledWhenAllied(bool value)
        {
            Definition.SetFullyControlledWhenAllied(value);
            return this;
        }
        public MonsterDefinitionBuilder SetGroupAttacks(bool value)
        {
            Definition.SetGroupAttacks(value);
            return this;
        }
        public MonsterDefinitionBuilder SetHasLookAt(bool value)
        {
            Definition.SetHasLookAt(value);
            return this;
        }
        public MonsterDefinitionBuilder SetHeight(int Height)
        {
            Definition.SetHeight(Height);
            return this;
        }
        public MonsterDefinitionBuilder SetHitDiceNumber(int hitDice)
        {
            Definition.SetHitDice(hitDice);
            return this;
        }
        public MonsterDefinitionBuilder SetHitDiceType(DieType dieType)
        {
            Definition.SetHitDiceType(dieType);
            return this;
        }

        public MonsterDefinitionBuilder SetHitDice(DieType dieType, int numberOf)
        {
            Definition.SetHitDiceType(dieType);
            Definition.SetHitDice(numberOf);
            return this;
        }

        public MonsterDefinitionBuilder SetHitPointsBonus(int bonus)
        {
            Definition.SetHitPointsBonus(bonus);
            return this;
        }
        public MonsterDefinitionBuilder SetInDungeonEditor(bool value)
        {
            if (value)
            {
                Definition.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.Monster);
            }
            else
            {
                Definition.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None);
            }

            return this;
        }
        public MonsterDefinitionBuilder SetInterceptStance(MoveStance interceptStance)
        {
            Definition.SetInterceptStance(interceptStance);
            return this;
        }
        public MonsterDefinitionBuilder SetIsHusk(bool value)
        {
            Definition.SetIsHusk(value);
            return this;
        }
        public MonsterDefinitionBuilder SetIsUnique(bool value)
        {
            Definition.SetIsUnique(value);
            return this;
        }
        public MonsterDefinitionBuilder SetLegendaryCreature(bool value)
        {
            Definition.SetLegendaryCreature(value);
            return this;
        }
        public MonsterDefinitionBuilder SetMaximalAge(int Age)
        {
            Definition.SetMaximalAge(Age);
            return this;
        }
        public MonsterDefinitionBuilder SetMaxLegendaryActionPoints(int maxLegendaryActionPoints)
        {
            Definition.SetMaxLegendaryActionPoints(maxLegendaryActionPoints);
            return this;
        }
        public MonsterDefinitionBuilder SetMaxLegendaryResistances(int maxLegendaryResistances)
        {
            Definition.SetMaxLegendaryResistances(maxLegendaryResistances);
            return this;
        }
        public MonsterDefinitionBuilder SetMinimalAge(int minimalAge)
        {
            Definition.SetMinimalAge(minimalAge);
            return this;
        }
        public MonsterDefinitionBuilder SetMonsterPresentation(MonsterPresentation monsterPresentation)
        {
            Definition.SetMonsterPresentation(monsterPresentation);
            return this;
        }
        public MonsterDefinitionBuilder SetNoExperienceGain(bool value)
        {
            Definition.SetNoExperienceGain(value);
            return this;
        }
        public MonsterDefinitionBuilder SetPatrolStance(MoveStance patrolStance)
        {
            Definition.SetPatrolStance(patrolStance);
            return this;
        }
        public MonsterDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition sizeDefinition)
        {
            Definition.SetSizeDefinition(sizeDefinition);
            return this;
        }
        public MonsterDefinitionBuilder SetSneakStance(MoveStance SneakStance)
        {
            Definition.SetSneakStance(SneakStance);
            return this;
        }
        public MonsterDefinitionBuilder SetStandardHitPoints(int Hp)
        {
            Definition.SetStandardHitPoints(Hp);
            return this;
        }
        public MonsterDefinitionBuilder SetStealableLootDefinition(LootPackDefinition stealableLoot)
        {
            Definition.SetStealableLootDefinition(stealableLoot);
            return this;
        }
        public MonsterDefinitionBuilder SetThreatEvaluatorDefinition(ThreatEvaluatorDefinition threatEvaluator)
        {
            Definition.SetThreatEvaluatorDefinition(threatEvaluator);
            return this;
        }
        public MonsterDefinitionBuilder SetUniqueNameId(string uniqueName)
        {
            Definition.SetUniqueNameId(uniqueName);
            return this;
        }
        public MonsterDefinitionBuilder SetWeight(int weight)
        {
            Definition.SetWeight(weight);
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

            Definition.AbilityScores.SetValue(STR, 0);      // STR
            Definition.AbilityScores.SetValue(DEX, 1);      // DEX
            Definition.AbilityScores.SetValue(CON, 2);      // CON
            Definition.AbilityScores.SetValue(INT, 3);      // INT
            Definition.AbilityScores.SetValue(WIS, 4);      // WIS
            Definition.AbilityScores.SetValue(CHA, 5);      // CHA
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
            return this;
        }

        public MonsterDefinitionBuilder ClearSkillScores()
        {
            Definition.SkillScores.Clear();
            return this;
        }

        public MonsterDefinitionBuilder AddSkillScores(params (string skillName, int bonus)[] skillScores)
        {
            return AddSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
        }

        public MonsterDefinitionBuilder AddSkillScores(params MonsterSkillProficiency[] skillScores)
        {
            return AddSkillScores(skillScores.AsEnumerable());
        }

        public MonsterDefinitionBuilder AddSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
        {
            Definition.SkillScores.AddRange(skillScores);
            return this;
        }

        public MonsterDefinitionBuilder SetSkillScores(params (string skillName, int bonus)[] skillScores)
        {
            return SetSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
        }

        public MonsterDefinitionBuilder SetSkillScores(params MonsterSkillProficiency[] skillScores)
        {
            return SetSkillScores(skillScores.AsEnumerable());
        }

        public MonsterDefinitionBuilder SetSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
        {
            Definition.SkillScores.SetRange(skillScores);
            return this;
        }

        public MonsterDefinitionBuilder ClearAttackIterations()
        {
            Definition.AttackIterations.Clear();
            return this;
        }

        public MonsterDefinitionBuilder AddAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
        {
            return AddAttackIterations(monsterAttackIterations.AsEnumerable());
        }

        public MonsterDefinitionBuilder AddAttackIterations(IEnumerable<MonsterAttackIteration> monsterAttackIterations)
        {
            Definition.AttackIterations.AddRange(monsterAttackIterations);
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

        public MonsterDefinitionBuilder ClearLegendaryActionOptions()
        {
            Definition.ClearLegendaryActionOptions();
            return this;
        }

        public MonsterDefinitionBuilder AddLegendaryActionOptions(params LegendaryActionDescription[] legendaryActionDescriptions)
        {
            return AddLegendaryActionOptions(legendaryActionDescriptions.AsEnumerable());
        }

        public MonsterDefinitionBuilder AddLegendaryActionOptions(IEnumerable<LegendaryActionDescription> legendaryActionDescriptions)
        {
            Definition.AddLegendaryActionOptions(legendaryActionDescriptions);
            return this;
        }

        public MonsterDefinitionBuilder SetLegendaryActionOptions(params LegendaryActionDescription[] legendaryActionDescriptions)
        {
            return SetLegendaryActionOptions(legendaryActionDescriptions.AsEnumerable());
        }

        public MonsterDefinitionBuilder SetLegendaryActionOptions(IEnumerable<LegendaryActionDescription> legendaryActionDescriptions)
        {
            Definition.SetLegendaryActionOptions(legendaryActionDescriptions);
            return this;
        }

        public MonsterDefinitionBuilder SetHasPhantomDistortion(bool value)
        {
            Definition.MonsterPresentation.SetHasPhantomDistortion(value);
            return this;
        }

        public MonsterDefinitionBuilder SetAttachedParticlesReference(AssetReference assetReference)
        {
            Definition.MonsterPresentation.SetAttachedParticlesReference(assetReference);
            return this;
        }

        public MonsterDefinitionBuilder ClearSavingThrowScores()
        {
            Definition.SavingThrowScores.Clear();
            return this;
        }

        public MonsterDefinitionBuilder ClearCreatureTags()
        {
            Definition.ClearCreatureTags();
            return this;
        }

        public MonsterDefinitionBuilder SetSavingThrowScores(params (string attributeName, int bonus)[] savingThrowScores)
        {
            Definition.SavingThrowScores.Clear();
            return AddSavingThrowScores(savingThrowScores.Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
        }

        public MonsterDefinitionBuilder AddSavingThrowScores(params (string attributeName, int bonus)[] savingThrowScores)
        {
            return AddSavingThrowScores(savingThrowScores.Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
        }

        public MonsterDefinitionBuilder AddSavingThrowScores(params MonsterSavingThrowProficiency[] savingThrowScores)
        {
            return AddSavingThrowScores(savingThrowScores.AsEnumerable());
        }

        public MonsterDefinitionBuilder AddSavingThrowScores(IEnumerable<MonsterSavingThrowProficiency> savingThrowScores)
        {
            Definition.SavingThrowScores.AddRange(savingThrowScores);
            return this;
        }

        public MonsterDefinitionBuilder SetHasMonsterPortraitBackground(bool value)
        {
            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(value);
            return this;
        }

        public MonsterDefinitionBuilder SetCanGeneratePortrait(bool value)
        {
            Definition.MonsterPresentation.SetCanGeneratePortrait(value);
            return this;
        }

        public MonsterDefinitionBuilder SetCustomShaderReference(AssetReference shaderReference)
        {
            Definition.MonsterPresentation.SetCustomShaderReference(shaderReference);
            return this;
        }

        public MonsterDefinitionBuilder SetModelScale(float scale)
        {
            Definition.MonsterPresentation.SetFemaleModelScale(scale);
            Definition.MonsterPresentation.SetMaleModelScale(scale);
            return this;
        }

        public MonsterDefinitionBuilder SetPrefabReference(AssetReference assetReference)
        {
            Definition.MonsterPresentation.SetMalePrefabReference(assetReference);
            Definition.MonsterPresentation.SetFemalePrefabReference(assetReference);
            return this;
        }
    }
}
