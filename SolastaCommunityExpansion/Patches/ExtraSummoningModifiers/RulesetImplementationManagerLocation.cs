using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolastaCommunityExpansion.Helpers;
using TA;
using TA.AI;
using HarmonyLib;
using System.Runtime.CompilerServices;

namespace SolastaCommunityExpansion.Patches.ExtraSummoningModifiers
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetImplementationManagerLocation_ApplySummonForm
    {

        internal static bool Prefix(
            RulesetImplementationManagerLocation __instance,
            EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var rulesetImplementationManager = (RulesetImplementationManager)__instance;

            SummonForm summonForm = effectForm.SummonForm;
            if (summonForm.SummonType == SummonForm.Type.Creature && !string.IsNullOrEmpty(summonForm.MonsterDefinitionName) && DatabaseRepository.GetDatabase<MonsterDefinition>().HasElement(summonForm.MonsterDefinitionName))
            {
            IGameLocationCharacterService service1 = ServiceRepository.GetService<IGameLocationCharacterService>();
            ulong encounterId = service1.GenerateEncounterId((GameGadget) null);
            MonsterDefinition element = DatabaseRepository.GetDatabase<MonsterDefinition>().GetElement(summonForm.MonsterDefinitionName);
            string sourceFaction = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
            int sourceAbilityBonus = formsParams.activeEffect.ComputeSourceAbilityBonus(formsParams.sourceCharacter);
            string effectDefinitionName = string.Empty;
            if (formsParams.attackMode != null)
                effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
            else if (formsParams.activeEffect != null)
                effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
            for (int index = 0; index < summonForm.Number; ++index)
            {
                RulesetCharacterMonster characterMonster = new RulesetCharacterMonster(element, 0, new RuleDefinitions.SpawnOverrides(), GadgetDefinitions.CreatureSex.Random);
                RulesetCondition condition1 = (RulesetCondition) null;
                if ((BaseDefinition) summonForm.ConditionDefinition != (BaseDefinition) null)
                condition1 = characterMonster.InflictCondition(summonForm.ConditionDefinition.Name, formsParams.durationType, formsParams.durationParameter, formsParams.endOfEffect, "11Effect", formsParams.sourceCharacter.Guid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
                RulesetCondition condition2 = characterMonster.InflictCondition("ConditionConjuredCreature", formsParams.durationType, formsParams.durationParameter, formsParams.endOfEffect, "17TagConjure", formsParams.sourceCharacter.Guid, sourceFaction, formsParams.effectLevel, string.Empty, 0, sourceAbilityBonus);
                formsParams.sourceCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionSummoningAffinity>(formsParams.sourceCharacter.FeaturesToBrowse);
                foreach (FeatureDefinitionSummoningAffinity summoningAffinity in formsParams.sourceCharacter.FeaturesToBrowse)
                {
                if (string.IsNullOrEmpty(summoningAffinity.RequiredMonsterTag) || element.CreatureTags.Contains(summoningAffinity.RequiredMonsterTag))
                {
                    foreach (ConditionDefinition addedCondition in summoningAffinity.AddedConditions)
                    {
                    int sourceAmount = 0;
                    switch (addedCondition.AmountOrigin)
                    {
                        case ConditionDefinition.OriginOfAmount.SourceHalfHitPoints:
                        sourceAmount = addedCondition.BaseAmount + formsParams.sourceCharacter.TryGetAttributeValue("HitPoints") / 2;
                        break;
                        case ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility:
                        int num1 = 0;
                        foreach (RulesetSpellRepertoire spellRepertoire in formsParams.sourceCharacter.SpellRepertoires)
                        {
                            int abilityScoreModifier = AttributeDefinitions.ComputeAbilityScoreModifier(formsParams.sourceCharacter.TryGetAttributeValue(spellRepertoire.SpellCastingAbility));
                            if (abilityScoreModifier > num1)
                            num1 = abilityScoreModifier;
                        }
                        sourceAmount = num1;
                        break;
                        case ConditionDefinition.OriginOfAmount.SourceSpellAttack:
                        int num2 = 0;
                        foreach (RulesetSpellRepertoire spellRepertoire in formsParams.sourceCharacter.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellAttackBonus > num2)
                            num2 = spellRepertoire.SpellAttackBonus;
                        }
                        sourceAmount = num2;
                        break;
                        case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                            sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                        break;
                        case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                            sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                        break;
                        case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                            var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;
                            // Find a better place to put this in?
                            string classType = addedCondition.AdditionalDamageType;
                            if (DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement(classType, out CharacterClassDefinition classDef)){
                                if (sourceCharacter.ClassesAndLevels != null)
                                    if (sourceCharacter.ClassesAndLevels.TryGetValue(classDef, out int classLevel))
                                        sourceAmount = classLevel;
                            }
                        break;

                    }
                    characterMonster.InflictCondition(addedCondition.Name, formsParams.durationType, formsParams.durationParameter, formsParams.endOfEffect, "11Effect", formsParams.sourceCharacter.Guid, sourceFaction, formsParams.effectLevel, string.Empty, sourceAmount, sourceAbilityBonus);
                    }
                }
                }
                characterMonster.RefreshAll();
                int attributeValue = characterMonster.TryGetAttributeValue("HitPoints");
                if (characterMonster.CurrentHitPoints < attributeValue)
                characterMonster.ReceiveHealing(attributeValue - characterMonster.CurrentHitPoints, false, 0UL, RuleDefinitions.HealingCap.MaximumHitPoints, (IHealingModificationProvider) null);
                GameLocationBehaviourPackage behaviourPackage = new GameLocationBehaviourPackage();
                behaviourPackage.NodePositions = new List<int3>();
                behaviourPackage.NodeOrientations = new List<LocationDefinitions.Orientation>();
                behaviourPackage.NodeDecisionPackages = new List<DecisionPackageDefinition>();
                behaviourPackage.EncounterId = encounterId;
                behaviourPackage.FormationDefinition = (FormationDefinition) null;
                behaviourPackage.BattleStartBehavior = GameLocationBehaviourPackage.BattleStartBehaviorType.RaisesAlarm;
                behaviourPackage.IsLeader = false;
                behaviourPackage.DecisionPackageDefinition = summonForm.DecisionPackage;
                IGameFactionService service2 = ServiceRepository.GetService<IGameFactionService>();
                bool battleInProgress = ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress;
                FactionDefinition currentFaction = characterMonster.CurrentFaction;
                int num = battleInProgress ? 1 : 0;
                RuleDefinitions.Side side = service2.ComputeSide(currentFaction, num != 0);
                characterMonster.ConjuredByParty = side == RuleDefinitions.Side.Ally;
                GameLocationCharacter character = service1.CreateCharacter(4242, (RulesetCharacter) characterMonster, side, behaviourPackage);
                if (condition1 != null)
                formsParams.activeEffect.TrackCondition(formsParams.sourceCharacter, formsParams.sourceCharacter.Guid, (RulesetActor) characterMonster, character.Guid, condition1, "11Effect");
                if (!summonForm.PersistOnConcentrationLoss)
                formsParams.activeEffect.TrackCondition(formsParams.sourceCharacter, formsParams.sourceCharacter.Guid, (RulesetActor) characterMonster, character.Guid, condition2, "17TagConjure");
                IGameLocationPositioningService service3 = ServiceRepository.GetService<IGameLocationPositioningService>();
                int3 position = formsParams.position;
                LocationDefinitions.Orientation orientation = LocationDefinitions.Orientation.North;

                var dummyCharacterList = new List<GameLocationCharacter>(); 
                var dummySizesList = new List<RulesetActor.SizeParameters>(); 
                var placementPositions = new List<int3>();
                var emptyFormationPositions = new List<int3>();

                dummyCharacterList.Clear();
                dummyCharacterList.Add(character);
                dummySizesList.Clear();
                dummySizesList.Add(characterMonster.SizeParams);
                placementPositions.Clear();
                service3.ComputeFormationPlacementPositions(dummyCharacterList, position, orientation, emptyFormationPositions, CellHelpers.PlacementMode.Station, placementPositions, dummySizesList);
                if (placementPositions.Count == 0)
                {
                Gui.GuiService.ShowAlert("Feedback/&NoRoomToConjureCreatureDescription", "EA7171");
                service1.KillCharacter(character, false, true, true, false);
                }
                else
                {
                service3.PlaceCharacter(character, placementPositions[0], orientation);
                character.RefreshActionPerformances();
                service1.RevealCharacter(character);
                }
            }
            }
            else if (summonForm.SummonType == SummonForm.Type.EffectProxy && !string.IsNullOrEmpty(summonForm.EffectProxyDefinitionName) && DatabaseRepository.GetDatabase<EffectProxyDefinition>().HasElement(summonForm.EffectProxyDefinitionName))
            {
            EffectProxyDefinition element = DatabaseRepository.GetDatabase<EffectProxyDefinition>().GetElement(summonForm.EffectProxyDefinitionName);
            bool flag = false;
            foreach (RulesetCharacterEffectProxy controlledEffectProxy in formsParams.sourceCharacter.ControlledEffectProxies)
            {
                if ((long) controlledEffectProxy.EffectGuid == (long) formsParams.activeEffect.Guid)
                flag = true;
            }
            if (flag)
                return false;
            ServiceRepository.GetService<IGameLocationCharacterService>().CreateAndBindEffectProxy((RulesetActor) formsParams.sourceCharacter, formsParams.activeEffect, formsParams.position, element);
            }
            else
            RulesetImplementationManager_ApplySummonForm.BaseMethodDummy(__instance,effectForm,formsParams);

            return false;

        }

    }


    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetImplementationManager_ApplySummonForm
    {

        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void BaseMethodDummy(
            RulesetImplementationManagerLocation instance,
            EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {

            SummonForm summonForm = effectForm.SummonForm;
            if (summonForm.SummonType != SummonForm.Type.InventoryItem || !((BaseDefinition) summonForm.ItemDefinition != (BaseDefinition) null) || (summonForm.Number <= 0 || !(formsParams.sourceCharacter is RulesetCharacterHero)))
            return;
            IRulesetItemFactoryService service = ServiceRepository.GetService<IRulesetItemFactoryService>();
            RulesetItem rulesetItem = (RulesetItem) null;
            if (formsParams.targetType == RuleDefinitions.TargetType.FreeSlot && summonForm.Number == 1)
            {
            foreach (string slotType in formsParams.activeEffect.EffectDescription.SlotTypes)
            {
                RulesetInventory characterInventory = formsParams.sourceCharacter.CharacterInventory;
                int num = characterInventory.CurrentConfiguration < 2 ? characterInventory.CurrentConfiguration : 0;
                if (slotType == EquipmentDefinitions.SlotTypeMainHand || slotType == EquipmentDefinitions.SlotTypeOffHand)
                {
                RulesetWieldedConfiguration itemsConfiguration = characterInventory.WieldedItemsConfigurations[num];
                if (slotType == EquipmentDefinitions.SlotTypeMainHand && itemsConfiguration.MainHandSlot.EquipedItem == null || slotType == EquipmentDefinitions.SlotTypeOffHand && itemsConfiguration.OffHandSlot.EquipedItem == null)
                {
                    rulesetItem = service.CreateStandardItem(summonForm.ItemDefinition);
                    characterInventory.DefineWieldedItemsConfiguration(num, rulesetItem, slotType);
                }
                }
                else if (characterInventory.InventorySlotsByName.ContainsKey(slotType) && characterInventory.InventorySlotsByName[slotType].EquipedItem == null)
                {
                rulesetItem = service.CreateStandardItem(summonForm.ItemDefinition);
                characterInventory.InventorySlotsByName[slotType].EquipItem(rulesetItem);
                }
                if (rulesetItem != null)
                {
                formsParams.activeEffect?.TrackSummonedItem(rulesetItem);
                formsParams.sourceCharacter.RefreshAll();
                break;
                }
            }
            }
            else
            {
            for (int index = 0; index < summonForm.Number; ++index)
            {
                bool flag = false;
                if (rulesetItem == null)
                flag = true;
                else if (summonForm.ItemDefinition.CanBeStacked)
                {
                if (rulesetItem.StackCount == summonForm.ItemDefinition.StackSize)
                {
                    formsParams.sourceCharacter.GrantItem(rulesetItem, false);
                    flag = true;
                }
                else
                    rulesetItem.IncreaseStack();
                }
                if (flag)
                rulesetItem = service.CreateStandardItem(summonForm.ItemDefinition);
            }
            if (rulesetItem == null)
                return;
            formsParams.sourceCharacter.GrantItem(rulesetItem, false);
            }
            
        }
    
    }

}
