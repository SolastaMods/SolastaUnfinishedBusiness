using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Inventor;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ClassFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featCallForCharge = BuildCallForCharge();
        var featCunningEscape = BuildCunningEscape();
        var featExpandTheHunt = BuildExpandTheHunt();
        var featNaturalFluidity = BuildNaturalFluidity();
        var featPoisoner = BuildPoisoner();
        var featSlayTheEnemies = BuildSlayTheEnemies();
        var featSpiritualFluidity = BuildSpiritualFluidity();

        var awakenTheBeastWithinGroup = BuildAwakenTheBeastWithin(feats);
        var blessedSoulGroup = BuildBlessedSoul(feats);
        var hardyGroup = BuildHardy(feats);
        var potentSpellcasterGroup = BuildPotentSpellcaster(feats);
        var primalRageGroup = BuildPrimalRage(feats);

        feats.AddRange(
            featCallForCharge,
            featCunningEscape,
            featExpandTheHunt,
            featNaturalFluidity,
            featPoisoner,
            featSlayTheEnemies,
            featSpiritualFluidity);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(
            featCunningEscape);

        GroupFeats.FeatGroupSpellCombat.AddFeats(
            potentSpellcasterGroup);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featCallForCharge,
            hardyGroup);

        GroupFeats.MakeGroup("FeatGroupClassBound", null,
            featCallForCharge,
            featCunningEscape,
            featExpandTheHunt,
            featNaturalFluidity,
            featPoisoner,
            featSlayTheEnemies,
            featSpiritualFluidity,
            awakenTheBeastWithinGroup,
            blessedSoulGroup,
            hardyGroup,
            potentSpellcasterGroup,
            primalRageGroup);
    }

    #region Call for Charge

    private static FeatDefinition BuildCallForCharge()
    {
        const string NAME = "FeatCallForCharge";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCallForCharge")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionPowerBuilder
                .Create($"Power{NAME}")
                .SetGuiPresentation(Category.Feature, PowerOathOfTirmarGoldenSpeech)
                .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                        .SetDurationData(DurationType.Round, 1)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}")
                                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                                        .SetSpecialInterruptions(ConditionInterruption.Attacked)
                                        .SetPossessive()
                                        .SetFeatures(
                                            FeatureDefinitionMovementAffinityBuilder
                                                .Create($"MovementAffinity{NAME}")
                                                .SetGuiPresentation($"Condition{NAME}", Category.Condition)
                                                .SetBaseSpeedAdditiveModifier(3)
                                                .AddToDB(),
                                            FeatureDefinitionCombatAffinityBuilder
                                                .Create($"CombatAffinity{NAME}")
                                                .SetGuiPresentation($"Condition{NAME}", Category.Condition)
                                                .SetMyAttackAdvantage(AdvantageType.Advantage)
                                                .AddToDB())
                                        .AddToDB(),
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .SetParticleEffectParameters(MagicWeapon)
                        .Build())
                .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetValidators(ValidatorsFeat.IsPaladinLevel1)
            .AddToDB();
    }

    #endregion

    #region Blessed Soul

    private static FeatDefinition BuildBlessedSoul(List<FeatDefinition> feats)
    {
        const string Name = "FeatBlessedSoul";

        // BACKWARD COMPATIBILITY
        _ = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var blessedSoulCleric = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cleric")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierClericChannelDivinityAdd,
                AttributeModifierCreed_Of_Maraike)
            .SetValidators(ValidatorsFeat.IsClericLevel4)
            .AddToDB();

        var blessedSoulPaladin = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Paladin")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierClericChannelDivinityAdd,
                AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsPaladinLevel4)
            .AddToDB();

        feats.AddRange(blessedSoulCleric, blessedSoulPaladin);

        return GroupFeats.MakeGroup(
            "FeatGroupBlessedSoul", null, blessedSoulCleric, blessedSoulPaladin);
    }

    #endregion

    #region Primal Rage

    private static FeatDefinition BuildPrimalRage(List<FeatDefinition> feats)
    {
        const string Name = "FeatPrimalRage";

        var primalRageStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierBarbarianRagePointsAdd,
                AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsBarbarianLevel4)
            .AddToDB();

        var primalRageCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierBarbarianRagePointsAdd,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsBarbarianLevel4)
            .AddToDB();

        feats.AddRange(primalRageStr, primalRageCon);

        return GroupFeats.MakeGroup(
            "FeatGroupPrimalRage", Name, primalRageStr, primalRageCon);
    }

    #endregion

    #region Expand the Hunt

    private static FeatDefinition BuildExpandTheHunt()
    {
        const string Name = "FeatExpandTheHunt";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                FeatureDefinitionPointPools.PointPoolBackgroundLanguageChoice_one,
                CharacterContext.InvocationPoolRangerPreferredEnemy,
                CharacterContext.InvocationPoolRangerTerrainType)
            .SetValidators(ValidatorsFeat.IsRangerLevel4)
            .AddToDB();
    }

    #endregion

    #region Poisoner

    private static FeatDefinition BuildPoisoner()
    {
        const string Name = "FeatPoisoner";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionActionAffinitys.ActionAffinityThiefFastHands,
                FeatureDefinitionCraftingAffinityBuilder
                    .Create($"CraftingAffinity{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetAffinityGroups(0.5f, true, ToolTypeDefinitions.ThievesToolsType,
                        ToolTypeDefinitions.PoisonersKitType)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, PoisonersKitType)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsRangerOrRogueLevel4)
            .AddToDB();
    }

    #endregion

    #region Awaken The Beast Within

    private static FeatDefinition BuildAwakenTheBeastWithin([NotNull] List<FeatDefinition> feats)
    {
        const string NAME = "FeatAwakenTheBeastWithin";

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPointBonusPerLevel)
            .AddToDB();

        var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{NAME}")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(TagsDefinitions.CreatureTagWildShape)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{NAME}")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel, DruidClass)
                    .SetFeatures(hpBonus, hpBonus) // 2 HP per level
                    .AddToDB())
            .AddToDB();

        var awakenTheBeastWithinFeats = AttributeDefinitions.AbilityScoreNames
            .Select(abilityScore => new
            {
                abilityScore,
                attributeModifier = DatabaseRepository.GetDatabase<FeatureDefinitionAttributeModifier>()
                    .FirstOrDefault(x =>
                        x.Name.StartsWith("AttributeModifierCreed") && x.ModifiedAttribute == abilityScore)
            })
            .Select(t =>
                FeatDefinitionWithPrerequisitesBuilder
                    .Create($"{NAME}{t.abilityScore}")
                    .SetGuiPresentation(
                        Gui.Format($"Feat/&{NAME}Title",
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                Gui.Localize($"Attribute/&{t.abilityScore}Title").ToLower())),
                        Gui.Format($"Feat/&{NAME}Description", t.abilityScore))
                    .SetFeatures(t.attributeModifier, summoningAffinity)
                    .SetValidators(ValidatorsFeat.IsDruidLevel4)
                    .AddToDB())
            .ToArray();

        // avoid run-time exception on write operation
        var temp = new List<FeatDefinition>();

        temp.AddRange(awakenTheBeastWithinFeats);

        var awakenTheBeastWithinGroup = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupAwakenTheBeastWithin", NAME, ValidatorsFeat.IsDruidLevel4, temp.ToArray());

        feats.AddRange(awakenTheBeastWithinFeats);

        return awakenTheBeastWithinGroup;
    }

    #endregion

    #region Cunning Escape

    private static FeatDefinition BuildCunningEscape()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCunningEscape")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new OnAfterActionFeatureFeatCunningEscape())
            .SetValidators(ValidatorsFeat.IsRogueLevel4)
            .AddToDB();
    }

    private class OnAfterActionFeatureFeatCunningEscape : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action.ActionDefinition != DatabaseHelper.ActionDefinitions.DashBonus)
            {
                return;
            }

            var actingCharacter = action.ActingCharacter.RulesetCharacter;

            var condition = RulesetCondition.CreateActiveCondition(
                actingCharacter.Guid,
                ConditionDefinitions.ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                actingCharacter.Guid, actingCharacter.CurrentFaction.Name);

            actingCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, condition);
        }
    }

    #endregion

    #region Hardy

    private static FeatDefinition BuildHardy(List<FeatDefinition> feats)
    {
        const string Name = "FeatHardy";

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new OnAfterActionHardy())
            .AddToDB();

        var hardyStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsFighterLevel4)
            .AddToDB();

        var hardyCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsFighterLevel4)
            .AddToDB();

        feats.AddRange(hardyStr, hardyCon);

        return GroupFeats.MakeGroup(
            "FeatGroupHardy", Name, hardyStr, hardyCon);
    }

    private sealed class OnAfterActionHardy : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != PowerFighterSecondWind)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var characterLevel = rulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            var dieRoll = RollDie(DieType.D10, AdvantageType.None, out _, out _);
            var healingReceived = characterLevel + dieRoll;

            if (rulesetCharacter.TemporaryHitPoints <= healingReceived)
            {
                rulesetCharacter.ReceiveTemporaryHitPoints(healingReceived, DurationType.Minute, 10,
                    TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
            }
        }
    }

    #endregion

    #region Natural Fluidity

    private static FeatDefinition BuildNaturalFluidity()
    {
        const string NAME = "FeatNaturalFluidity";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .AddToDB();

        //
        // Gain Slots
        //

        var powerGainSlotPoolList = new List<FeatureDefinitionPower>();

        var powerGainSlotPool = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}GainSlotPool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerGainSlot", Resources.PowerGainSlot, 128, 64))
            .SetSharedPool(ActivationTime.BonusAction, power)
            .AddToDB();

        for (var i = 3; i >= 1; i--)
        {
            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}GainSlot{i}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerFeatNaturalFluidityGainSlotTitle", i.ToString()),
                    Gui.Format("Feature/&PowerFeatNaturalFluidityGainSlotDescription", i.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, power)
                .SetEffectDescription(EffectDescriptionBuilder.Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create($"Condition{NAME}Gain{i}Slot")
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetFeatures(
                                GetDefinition<FeatureDefinitionMagicAffinity>($"MagicAffinityAdditionalSpellSlot{i}"))
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
                .SetCustomSubFeatures(SpendWildShapeUse.Mark,
                    new ValidatorsPowerUse(c => c.GetRemainingPowerUses(PowerDruidWildShape) > 0))
                .AddToDB();

            powerGainSlotPoolList.Add(powerGainSlot);
        }

        PowerBundle.RegisterPowerBundle(powerGainSlotPool, false, powerGainSlotPoolList);

        //
        // Gain Wild Shape
        //

        var powerGainWildShapeList = new List<FeatureDefinitionPower>();

        var powerWildShapePool = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}WildShapePool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerGainWildShape", Resources.PowerGainWildShape, 128, 64))
            .SetSharedPool(ActivationTime.BonusAction, power)
            .AddToDB();

        for (var i = 8; i >= 3; i--)
        {
            var wildShapeAmount = i switch
            {
                >= 6 => 2,
                >= 3 => 1,
                _ => 0
            };

            var powerGainWildShapeFromSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}GainWildShapeFromSlot{i}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerFeatNaturalFluidityGainWildShapeFromSlotTitle",
                        wildShapeAmount.ToString()),
                    Gui.Format("Feature/&PowerFeatNaturalFluidityGainWildShapeFromSlotDescription",
                        wildShapeAmount.ToString(), i.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, power)
                .SetCustomSubFeatures(new GainWildShapeCharges(i, wildShapeAmount))
                .AddToDB();

            powerGainWildShapeList.Add(powerGainWildShapeFromSlot);
        }

        PowerBundle.RegisterPowerBundle(powerWildShapePool, false, powerGainWildShapeList);

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(power, powerWildShapePool, powerGainSlotPool)
            .SetValidators(ValidatorsFeat.IsDruidLevel4)
            .AddToDB();
    }

    private class GainWildShapeCharges : ICustomMagicEffectAction, IPowerUseValidity
    {
        private readonly int slotLevel;
        private readonly int wildShapeAmount;

        public GainWildShapeCharges(int slotLevel, int wildShapeAmount)
        {
            this.slotLevel = slotLevel;
            this.wildShapeAmount = wildShapeAmount;
        }

        public IEnumerator ProcessCustomEffect(CharacterActionMagicEffect action)
        {
            var character = action.ActingCharacter.RulesetCharacter;
            var repertoire = character.GetClassSpellRepertoire(CharacterClassDefinitions.Druid);
            var rulesetUsablePower = character.UsablePowers.Find(p => p.PowerDefinition == PowerDruidWildShape);

            if (repertoire == null || rulesetUsablePower == null)
            {
                yield break;
            }

            repertoire.SpendSpellSlot(slotLevel);
            character.UpdateUsageForPowerPool(-wildShapeAmount, rulesetUsablePower);
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var remaining = 0;

            character.GetClassSpellRepertoire(CharacterClassDefinitions.Druid)?
                .GetSlotsNumber(slotLevel, out remaining, out _);

            var notMax = character.GetMaxUsesForPool(PowerDruidWildShape) >
                         character.GetRemainingPowerUses(PowerDruidWildShape);

            return remaining > 0 && notMax;
        }
    }

    private class SpendWildShapeUse : ICustomMagicEffectAction
    {
        private SpendWildShapeUse()
        {
        }

        public static SpendWildShapeUse Mark { get; } = new();

        public IEnumerator ProcessCustomEffect(CharacterActionMagicEffect action)
        {
            var character = action.ActingCharacter.RulesetCharacter;
            var rulesetUsablePower = character.UsablePowers.Find(p => p.PowerDefinition == PowerDruidWildShape);

            if (rulesetUsablePower != null)
            {
                character.UpdateUsageForPowerPool(1, rulesetUsablePower);
            }

            yield break;
        }
    }

    #endregion

    #region Potent Spellcaster

    private static FeatDefinition BuildPotentSpellcaster(List<FeatDefinition> feats)
    {
        const string Name = "FeatPotentSpellcaster";

        // BACKWARD COMPATIBILITY
        _ = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var spellLists = new List<SpellListDefinition>
        {
            SpellListDefinitions.SpellListBard,
            SpellListDefinitions.SpellListCleric,
            SpellListDefinitions.SpellListDruid,
            SpellListDefinitions.SpellListSorcerer,
            SpellListDefinitions.SpellListWizard,
            InventorClass.SpellList
        };

        var validators = new List<Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>>
        {
            ValidatorsFeat.IsBardLevel4,
            ValidatorsFeat.IsClericLevel4,
            ValidatorsFeat.IsDruidLevel4,
            ValidatorsFeat.IsSorcererLevel4,
            ValidatorsFeat.IsWizardLevel4,
            ValidatorsFeat.IsInventorLevel4
        };

        var potentSpellcasterFeats = new List<FeatDefinition>();

        for (var i = 0; i < spellLists.Count; i++)
        {
            var spellList = spellLists[i];
            var validator = validators[i];
            var className = spellList.Name.Replace("SpellList", String.Empty);
            var classTitle = GetDefinition<CharacterClassDefinition>(className).FormatTitle();
            var featPotentSpellcaster = FeatDefinitionWithPrerequisitesBuilder
                .Create($"{Name}{className}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatPotentSpellcasterTitle", classTitle),
                    Gui.Format("Feat/&FeatPotentSpellcasterDescription", classTitle))
                .SetCustomSubFeatures(new ModifyMagicEffectFeatPotentSpellcaster(spellList))
                .SetValidators(validator)
                .AddToDB();

            potentSpellcasterFeats.Add(featPotentSpellcaster);
        }

        var potentSpellcasterGroup = GroupFeats.MakeGroup(
            "FeatGroupPotentSpellcaster", null, potentSpellcasterFeats);

        feats.AddRange(potentSpellcasterFeats);

        return potentSpellcasterGroup;
    }

    private sealed class ModifyMagicEffectFeatPotentSpellcaster : IModifyMagicEffect
    {
        private readonly SpellListDefinition _spellListDefinition;

        public ModifyMagicEffectFeatPotentSpellcaster(SpellListDefinition spellListDefinition)
        {
            _spellListDefinition = spellListDefinition;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter character)
        {
            if (definition is not SpellDefinition spellDefinition ||
                !_spellListDefinition.SpellsByLevel
                    .Any(x => x.Level == 0 && x.Spells.Contains(spellDefinition)))
            {
                return effect;
            }

            var damage = effect.FindFirstDamageForm();

            if (damage == null)
            {
                return effect;
            }

            string attribute;

            if (_spellListDefinition == SpellListDefinitions.SpellListBard ||
                _spellListDefinition == SpellListDefinitions.SpellListSorcerer)

            {
                attribute = AttributeDefinitions.Charisma;
            }
            else if (_spellListDefinition == SpellListDefinitions.SpellListCleric ||
                     _spellListDefinition == SpellListDefinitions.SpellListDruid)
            {
                attribute = AttributeDefinitions.Wisdom;
            }
            else if (_spellListDefinition == SpellListDefinitions.SpellListWizard ||
                     _spellListDefinition == InventorClass.SpellList)
            {
                attribute = AttributeDefinitions.Intelligence;
            }
            else
            {
                return effect;
            }

            var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(
                character.GetAttribute(attribute).CurrentValue);

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                "Feat/&FeatPotentSpellcasterTitle", null));

            return effect;
        }
    }

    #endregion

    #region Spiritual Fluidity

    private static FeatDefinition BuildSpiritualFluidity()
    {
        const string NAME = "FeatSpiritualFluidity";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .AddToDB();

        //
        // Gain Slots
        //

        var powerGainSlotPoolList = new List<FeatureDefinitionPower>();

        var powerGainSlotPool = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}GainSlotPool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerGainSlot", Resources.PowerGainSlot, 128, 64))
            .SetSharedPool(ActivationTime.BonusAction, power)
            .AddToDB();

        for (var i = 3; i >= 1; i--)
        {
            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}GainSlot{i}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerFeatSpiritualFluidityGainSlotTitle", i.ToString()),
                    Gui.Format("Feature/&PowerFeatSpiritualFluidityGainSlotDescription", i.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, power)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.UntilLongRest)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}Gain{i}Slot")
                                        .SetGuiPresentationNoContent(true)
                                        .SetSilent(Silent.WhenAddedOrRemoved)
                                        .SetFeatures(GetDefinition<FeatureDefinitionMagicAffinity>(
                                            $"MagicAffinityAdditionalSpellSlot{i}"))
                                        .AddToDB(),
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .Build())
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        c => c.GetAttribute(AttributeDefinitions.ChannelDivinityNumber).CurrentValue >
                             c.UsedChannelDivinity))
                .AddToDB();

            powerGainSlotPoolList.Add(powerGainSlot);
        }

        PowerBundle.RegisterPowerBundle(powerGainSlotPool, false, powerGainSlotPoolList);

        //
        // Gain Channel Divinity
        //

        var pickFeatList = new List<FeatureDefinitionAttributeModifier>
        {
            AttributeModifierClericChannelDivinityAdd,
            AttributeModifierClericChannelDivinityAdd,
            AttributeModifierClericChannelDivinityAdd
        };

        var powerGainChannelDivinityList = new List<FeatureDefinitionPower>();

        var powerChannelDivinityPool = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}ChannelDivinityPool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerGainChannelDivinity", Resources.PowerGainChannelDivinity, 128, 64))
            .SetSharedPool(ActivationTime.BonusAction, power)
            .AddToDB();

        for (var i = 8; i >= 3; i--)
        {
            // closure
            var a = i;

            var channelDivinityAmount = i switch
            {
                >= 7 => 3,
                >= 5 => 2,
                >= 3 => 1,
                _ => 0
            };

            var powerGainChannelDivinityFromSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}GainChannelDivinityFromSlot{i}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerFeatSpiritualFluidityGainChannelDivinityFromSlotTitle",
                        channelDivinityAmount.ToString()),
                    Gui.Format("Feature/&PowerFeatSpiritualFluidityGainChannelDivinityFromSlotDescription",
                        channelDivinityAmount.ToString(), i.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, power)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.UntilLongRest)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}GainChannelDivinityFromSlot{i}")
                                        .SetGuiPresentationNoContent(true)
                                        .SetSilent(Silent.WhenAddedOrRemoved)
                                        .SetFeatures(pickFeatList.Take(channelDivinityAmount))
                                        .AddToDB(),
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .Build())
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        c =>
                        {
                            var remaining = 0;

                            c.GetClassSpellRepertoire(CharacterClassDefinitions.Cleric)?
                                .GetSlotsNumber(a, out remaining, out _);

                            return remaining > 0;
                        }))
                .AddToDB();

            powerGainChannelDivinityList.Add(powerGainChannelDivinityFromSlot);
        }

        PowerBundle.RegisterPowerBundle(powerChannelDivinityPool, false, powerGainChannelDivinityList);

        return
            FeatDefinitionWithPrerequisitesBuilder
                .Create(NAME)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(power, powerChannelDivinityPool, powerGainSlotPool)
                .SetValidators(ValidatorsFeat.IsClericLevel4)
                .SetCustomSubFeatures(new OnAfterActionFeatureFeatSpiritualFluidity())
                .AddToDB();
    }

    private sealed class OnAfterActionFeatureFeatSpiritualFluidity : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            switch (action)
            {
                case CharacterActionUsePower characterActionUsePowerGainChannel when
                    characterActionUsePowerGainChannel.activePower.PowerDefinition.Name.StartsWith(
                        "PowerFeatSpiritualFluidityGainChannelDivinityFromSlot"):
                {
                    var character = action.ActingCharacter.RulesetCharacter;
                    var name = characterActionUsePowerGainChannel.activePower.PowerDefinition.Name;
                    var level = int.Parse(name.Substring(name.Length - 1, 1));
                    var repertoire = character.GetClassSpellRepertoire(CharacterClassDefinitions.Cleric);

                    repertoire?.SpendSpellSlot(level);

                    break;
                }
                case CharacterActionUsePower characterActionUsePowerGainSlot when
                    characterActionUsePowerGainSlot.activePower.PowerDefinition.Name.StartsWith(
                        "PowerFeatSpiritualFluidityGainSlot"):
                {
                    var character = action.ActingCharacter.RulesetCharacter;

                    character.UsedChannelDivinity += 1;

                    break;
                }
            }
        }
    }

    #endregion

    #region Slay the Enemies

    private static FeatDefinition BuildSlayTheEnemies()
    {
        const string NAME = "FeatSlayTheEnemies";

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Pool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerSlayTheEnemies", Resources.PowerSlayTheEnemies, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var powerPoolList = new List<FeatureDefinitionPower>();

        for (var i = 3; i >= 1; i--)
        {
            // closure
            var a = i;

            var rounds = 2 + i;

            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}{i}")
                .SetGuiPresentation(
                    Gui.Format($"Feature/&Power{NAME}Title", i.ToString(), rounds.ToString()),
                    Gui.Format($"Feature/&Power{NAME}Description", i.ToString(), rounds.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, powerPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, rounds)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}{i}")
                                        .SetGuiPresentation(
                                            "Condition/&ConditionFeatSlayTheEnemiesTitle",
                                            Gui.Format("Condition/&ConditionFeatSlayTheEnemiesDescription",
                                                i.ToString()), ConditionDefinitions.ConditionTrueStrike)
                                        .SetPossessive()
                                        .AddToDB(),
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .Build())
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        c =>
                        {
                            var remaining = 0;

                            c.GetClassSpellRepertoire(CharacterClassDefinitions.Ranger)?
                                .GetSlotsNumber(a, out remaining, out _);

                            var noCondition = ValidatorsCharacter.HasNoneOfConditions(
                                "ConditionFeatSlayTheEnemies1",
                                "ConditionFeatSlayTheEnemies2",
                                "ConditionFeatSlayTheEnemies3")(c);

                            return remaining > 0 && noCondition;
                        }))
                .AddToDB();

            powerPoolList.Add(powerGainSlot);
        }

        PowerBundle.RegisterPowerBundle(powerPool, false, powerPoolList);

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerPool)
            .SetValidators(ValidatorsFeat.IsRangerLevel1)
            .SetCustomSubFeatures(
                new OnComputeAttackModifierSlayTheEnemies(powerPool),
                new OnAfterActionFeatureFeatSlayTheEnemies())
            .AddToDB();
    }

    private sealed class OnComputeAttackModifierSlayTheEnemies : IOnComputeAttackModifier
    {
        private readonly FeatureDefinition _featureDefinition;

        public OnComputeAttackModifierSlayTheEnemies(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (ValidatorsCharacter.HasNoneOfConditions(
                    "ConditionFeatSlayTheEnemies1",
                    "ConditionFeatSlayTheEnemies2",
                    "ConditionFeatSlayTheEnemies3")(myself))
            {
                return;
            }

            if (attackMode.ToHitBonusTrends.Any(x => x.source as FeatureDefinition == _featureDefinition))
            {
                return;
            }

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var spellLevel = 0;

            if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies1")(myself))
            {
                spellLevel = 1;
            }
            else if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies2")(myself))
            {
                spellLevel = 2;
            }
            else if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies3")(myself))
            {
                spellLevel = 3;
            }


            if (IsFavoriteEnemy(myself, defender))
            {
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
            }
            else
            {
                attackMode.ToHitBonus += spellLevel;
                attackMode.ToHitBonusTrends.Add(new TrendInfo(spellLevel, FeatureSourceType.CharacterFeature,
                    _featureDefinition.Name, _featureDefinition));
            }

            damage.BonusDamage += spellLevel;
            damage.DamageBonusTrends.Add(new TrendInfo(spellLevel, FeatureSourceType.CharacterFeature,
                _featureDefinition.Name, _featureDefinition));
        }

        private static bool IsFavoriteEnemy(RulesetActor attacker, RulesetCharacter defender)
        {
            var favoredEnemyChoices = FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice.FeatureSet
                .Cast<FeatureDefinitionAdditionalDamage>();
            var characterAdditionalDamages = attacker.GetFeaturesByType<FeatureDefinitionAdditionalDamage>();

            return favoredEnemyChoices
                .Intersect(characterAdditionalDamages)
                .Any(x => x.RequiredCharacterFamily.Name == defender.CharacterFamily);
        }
    }

    private sealed class OnAfterActionFeatureFeatSlayTheEnemies : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePowerSlayTheEnemies ||
                !characterActionUsePowerSlayTheEnemies.activePower.PowerDefinition.Name.StartsWith(
                    "PowerFeatSlayTheEnemies"))
            {
                return;
            }

            var character = action.ActingCharacter.RulesetCharacter;
            var name = characterActionUsePowerSlayTheEnemies.activePower.PowerDefinition.Name;
            var level = int.Parse(name.Substring(name.Length - 1, 1));
            var repertoire = character.GetClassSpellRepertoire(CharacterClassDefinitions.Ranger);

            repertoire?.SpendSpellSlot(level);
        }
    }

    #endregion
}
