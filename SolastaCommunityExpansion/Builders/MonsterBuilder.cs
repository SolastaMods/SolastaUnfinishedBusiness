using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static BestiaryDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Builders
{
    public class MonsterBuilder : BaseDefinitionBuilder<MonsterDefinition>
    {
        public MonsterBuilder(string name, string guid, string title, string description, MonsterDefinition baseMonster)
            : base(baseMonster, name, guid, title, description)
        {
        }

        public MonsterBuilder(string name, Guid namespaceGuid, MonsterDefinition baseMonster)
            : base(baseMonster, name, namespaceGuid, null)
        {
        }

        public MonsterBuilder SetAlignment(string alignment)
        {
            Definition.SetAlignment(alignment);
            return this;
        }
        public MonsterBuilder SetAlwaysHideStats(bool value)
        {
            Definition.SetAlwaysHideStats(value);
            return this;
        }
        public MonsterBuilder SetArmor(string armor)
        {
            Definition.SetArmor(armor);
            return this;
        }
        public MonsterBuilder SetArmorClass(int armorClass)
        {
            Definition.SetArmorClass(armorClass);
            return this;
        }
        public MonsterBuilder SetAudioRaceRTPCValue(float audioRaceRTPCValue)
        {
            Definition.SetAudioRaceRTPCValue(audioRaceRTPCValue);
            return this;
        }
        public MonsterBuilder SetBestiaryEntry(BestiaryEntry entry)
        {
            Definition.SetBestiaryEntry(entry);
            return this;
        }
        public MonsterBuilder SetBestiaryReference(MonsterDefinition monster)
        {
            Definition.SetBestiaryReference(monster);
            return this;
        }
        public MonsterBuilder SetBestiarySpriteReference(AssetReferenceSprite sprite)
        {
            Definition.SetBestiarySpriteReference(sprite);
            return this;
        }
        public MonsterBuilder SetChallengeRating(float challengeRating)
        {
            Definition.SetChallengeRating(challengeRating);
            return this;
        }
        public MonsterBuilder SetCharacterFamily(string family)
        {
            Definition.SetCharacterFamily(family);
            return this;
        }
        public MonsterBuilder SetDefaultBattleDecisionPackage(DecisionPackageDefinition decisionPackage)
        {
            Definition.SetDefaultBattleDecisionPackage(decisionPackage);
            return this;
        }
        public MonsterBuilder SetDefaultFaction(string faction)
        {
            Definition.SetDefaultFaction(faction);
            return this;
        }
        public MonsterBuilder SetDifferentActionEachTurn(bool value)
        {
            Definition.SetDifferentActionEachTurn(value);
            return this;
        }
        public MonsterBuilder SetDroppedLootDefinition(LootPackDefinition lootPack)
        {
            Definition.SetDroppedLootDefinition(lootPack);
            return this;
        }
        public MonsterBuilder SetDualSex(bool value)
        {
            Definition.SetDualSex(value);
            return this;
        }
        public MonsterBuilder SetFollowFloorAngle(bool value)
        {
            Definition.SetFollowFloorAngle(value);
            return this;
        }
        public MonsterBuilder SetForceCombatStartsAnimation(bool value)
        {
            Definition.SetForceCombatStartsAnimation(value);
            return this;
        }
        public MonsterBuilder SetForceNoFlyAnimation(bool value)
        {
            Definition.SetForceNoFlyAnimation(value);
            return this;
        }
        public MonsterBuilder SetForcePersistentBody(bool value)
        {
            Definition.SetForcePersistentBody(value);
            return this;
        }
        public MonsterBuilder SetFullyControlledWhenAllied(bool value)
        {
            Definition.SetFullyControlledWhenAllied(value);
            return this;
        }
        public MonsterBuilder SetGroupAttacks(bool value)
        {
            Definition.SetGroupAttacks(value);
            return this;
        }
        public MonsterBuilder SetHasLookAt(bool value)
        {
            Definition.SetHasLookAt(value);
            return this;
        }
        public MonsterBuilder SetHeight(int Height)
        {
            Definition.SetHeight(Height);
            return this;
        }
        public MonsterBuilder SetHitDiceNumber(int hitDice)
        {
            Definition.SetHitDice(hitDice);
            return this;
        }
        public MonsterBuilder SetHitDiceType(DieType dieType)
        {
            Definition.SetHitDiceType(dieType);
            return this;
        }
        public MonsterBuilder SetHitPointsBonus(int bonus)
        {
            Definition.SetHitPointsBonus(bonus);
            return this;
        }
        public MonsterBuilder SetInDungeonEditor(bool value)
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
        public MonsterBuilder SetInterceptStance(MoveStance interceptStance)
        {
            Definition.SetInterceptStance(interceptStance);
            return this;
        }
        public MonsterBuilder SetIsHusk(bool value)
        {
            Definition.SetIsHusk(value);
            return this;
        }
        public MonsterBuilder SetIsUnique(bool value)
        {
            Definition.SetIsUnique(value);
            return this;
        }
        public MonsterBuilder SetLegendaryCreature(bool value)
        {
            Definition.SetLegendaryCreature(value);
            return this;
        }
        public MonsterBuilder SetMaximalAge(int Age)
        {
            Definition.SetMaximalAge(Age);
            return this;
        }
        public MonsterBuilder SetMaxLegendaryActionPoints(int maxLegendaryActionPoints)
        {
            Definition.SetMaxLegendaryActionPoints(maxLegendaryActionPoints);
            return this;
        }
        public MonsterBuilder SetMaxLegendaryResistances(int maxLegendaryResistances)
        {
            Definition.SetMaxLegendaryResistances(maxLegendaryResistances);
            return this;
        }
        public MonsterBuilder SetMinimalAge(int minimalAge)
        {
            Definition.SetMinimalAge(minimalAge);
            return this;
        }
        public MonsterBuilder SetMonsterPresentation(MonsterPresentation monsterPresentation)
        {
            Definition.SetMonsterPresentation(monsterPresentation);
            return this;
        }
        public MonsterBuilder SetNoExperienceGain(bool value)
        {
            Definition.SetNoExperienceGain(value);
            return this;
        }
        public MonsterBuilder SetPatrolStance(MoveStance patrolStance)
        {
            Definition.SetPatrolStance(patrolStance);
            return this;
        }
        public MonsterBuilder SetSizeDefinition(CharacterSizeDefinition sizeDefinition)
        {
            Definition.SetSizeDefinition(sizeDefinition);
            return this;
        }
        public MonsterBuilder SetSneakStance(MoveStance SneakStance)
        {
            Definition.SetSneakStance(SneakStance);
            return this;
        }
        public MonsterBuilder SetStandardHitPoints(int Hp)
        {
            Definition.SetStandardHitPoints(Hp);
            return this;
        }
        public MonsterBuilder SetStealableLootDefinition(LootPackDefinition stealableLoot)
        {
            Definition.SetStealableLootDefinition(stealableLoot);
            return this;
        }
        public MonsterBuilder SetThreatEvaluatorDefinition(ThreatEvaluatorDefinition threatEvaluator)
        {
            Definition.SetThreatEvaluatorDefinition(threatEvaluator);
            return this;
        }
        public MonsterBuilder SetUniqueNameId(string uniqueName)
        {
            Definition.SetUniqueNameId(uniqueName);
            return this;
        }
        public MonsterBuilder SetWeight(int weight)
        {
            Definition.SetWeight(weight);
            return this;
        }

        public MonsterBuilder ClearAbilityScores()
        {
            Array.Clear(Definition.AbilityScores, 0, Definition.AbilityScores.Length);
            return this;
        }

        public MonsterBuilder SetAbilityScores(int STR, int DEX, int CON, int INT, int WIS, int CHA)
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

        public MonsterBuilder ClearFeatures()
        {
            Definition.Features.Clear();
            return this;
        }

        public MonsterBuilder AddFeatures(params FeatureDefinition[] features)
        {
            return AddFeatures(features.AsEnumerable());
        }

        public MonsterBuilder AddFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.Features.AddRange(features);
            return this;
        }

        public MonsterBuilder SetFeatures(params FeatureDefinition[] features)
        {
            return SetFeatures(features.AsEnumerable());
        }

        public MonsterBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.Features.SetRange(features);
            return this;
        }

        public MonsterBuilder ClearSkillScores()
        {
            Definition.SkillScores.Clear();
            return this;
        }

        public MonsterBuilder AddSkillScores(params (string skillName, int bonus)[] skillScores)
        {
            return AddSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
        }

        public MonsterBuilder AddSkillScores(params MonsterSkillProficiency[] skillScores)
        {
            return AddSkillScores(skillScores.AsEnumerable());
        }

        public MonsterBuilder AddSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
        {
            Definition.SkillScores.AddRange(skillScores);
            return this;
        }

        public MonsterBuilder SetSkillScores(params (string skillName, int bonus)[] skillScores)
        {
            return SetSkillScores(skillScores.Select(ss => new MonsterSkillProficiency(ss.skillName, ss.bonus)));
        }

        public MonsterBuilder SetSkillScores(params MonsterSkillProficiency[] skillScores)
        {
            return SetSkillScores(skillScores.AsEnumerable());
        }

        public MonsterBuilder SetSkillScores(IEnumerable<MonsterSkillProficiency> skillScores)
        {
            Definition.SkillScores.SetRange(skillScores);
            return this;
        }

        public MonsterBuilder ClearAttackIterations()
        {
            Definition.AttackIterations.Clear();
            return this;
        }

        public MonsterBuilder AddAttackIterations(params MonsterAttackIteration[] monsterAttackIterations)
        {
            return AddAttackIterations(monsterAttackIterations.AsEnumerable());
        }

        public MonsterBuilder AddAttackIterations(IEnumerable<MonsterAttackIteration> monsterAttackIterations)
        {
            Definition.AttackIterations.AddRange(monsterAttackIterations);
            return this;
        }

        public MonsterBuilder ClearLegendaryActionOptions()
        {
            Definition.LegendaryActionOptions.Clear();
            return this;
        }

        public MonsterBuilder AddLegendaryActionOptions(params LegendaryActionDescription[] legendaryActionDescriptions)
        {
            return AddLegendaryActionOptions(legendaryActionDescriptions.AsEnumerable());
        }

        public MonsterBuilder AddLegendaryActionOptions(IEnumerable<LegendaryActionDescription> legendaryActionDescriptions)
        {
            Definition.LegendaryActionOptions.AddRange(legendaryActionDescriptions);
            return this;
        }

        public MonsterBuilder SetHasPhantomDistortion(bool value)
        {
            Definition.MonsterPresentation.SetHasPhantomDistortion(value);
            return this;
        }

        public MonsterBuilder SetAttachedParticlesReference(AssetReference assetReference)
        {
            Definition.MonsterPresentation.SetAttachedParticlesReference(assetReference);
            return this;
        }

        public MonsterBuilder SetSpriteReference(AssetReferenceSprite sprite)
        {
            Definition.GuiPresentation.SetSpriteReference(sprite);
            return this;
        }

        public MonsterBuilder ClearSavingThrowScores()
        {
            Definition.SavingThrowScores.Clear();
            return this;
        }

        public MonsterBuilder SetSavingThrowScores(params (string attributeName, int bonus)[] savingThrowScores)
        {
            Definition.SavingThrowScores.Clear();
            return AddSavingThrowScores(savingThrowScores.Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
        }

        public MonsterBuilder AddSavingThrowScores(params (string attributeName, int bonus)[] savingThrowScores)
        {
            return AddSavingThrowScores(savingThrowScores.Select(s => new MonsterSavingThrowProficiency(s.attributeName, s.bonus)).AsEnumerable());
        }

        public MonsterBuilder AddSavingThrowScores(params MonsterSavingThrowProficiency[] savingThrowScores)
        {
            return AddSavingThrowScores(savingThrowScores.AsEnumerable());
        }

        public MonsterBuilder AddSavingThrowScores(IEnumerable<MonsterSavingThrowProficiency> savingThrowScores)
        {
            Definition.SavingThrowScores.AddRange(savingThrowScores);
            return this;
        }

        public MonsterBuilder SetHasMonsterPortraitBackground(bool value)
        {
            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(value);
            return this;
        }

        public MonsterBuilder SetCanGeneratePortrait(bool value)
        {
            Definition.MonsterPresentation.SetCanGeneratePortrait(value);
            return this;
        }

        public MonsterBuilder SetCustomShaderReference(MonsterDefinition baseMonsterShaderReference)
        {
            Definition.MonsterPresentation.SetCustomShaderReference(baseMonsterShaderReference.MonsterPresentation.CustomShaderReference);
            return this;
        }
        public MonsterBuilder SetModelScale(float scale)
        {
            Definition.MonsterPresentation.SetFemaleModelScale(scale);
            Definition.MonsterPresentation.SetMaleModelScale(scale);
            return this;
        }

        public MonsterBuilder SetPrefabReference(AssetReference assetReference)
        {
            Definition.MonsterPresentation.SetMalePrefabReference(assetReference);
            Definition.MonsterPresentation.SetFemalePrefabReference(assetReference);
            return this;
        }
    }
}
