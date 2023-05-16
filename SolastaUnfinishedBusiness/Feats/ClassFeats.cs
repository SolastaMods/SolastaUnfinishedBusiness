using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Feats.FeatHelpers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ClassFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featCallForCharge = BuildCallForCharge();
        var featCunningEscape = BuildCunningEscape();
        var featExpandTheHunt = BuildExpandTheHunt();
        var featExploiter = BuildExploiter();
        var featNaturalFluidity = BuildNaturalFluidity();
        var featPoisoner = BuildPoisoner();
        var featSlayTheEnemies = BuildSlayTheEnemies();
        var featSpiritualFluidity = BuildSpiritualFluidity();

        var awakenTheBeastWithinGroup = BuildAwakenTheBeastWithin(feats);
        var blessedSoulGroup = BuildBlessedSoul(feats);
        var closeQuartersGroup = BuildCloseQuarters(feats);
        var hardyGroup = BuildHardy(feats);
        var potentSpellcasterGroup = BuildPotentSpellcaster(feats);
        var primalRageGroup = BuildPrimalRage(feats);

        feats.AddRange(
            featCallForCharge,
            featCunningEscape,
            featExpandTheHunt,
            featExploiter,
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
            featExploiter,
            featNaturalFluidity,
            featPoisoner,
            featSlayTheEnemies,
            featSpiritualFluidity,
            awakenTheBeastWithinGroup,
            blessedSoulGroup,
            closeQuartersGroup,
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
                .SetGuiPresentation(Category.Feature,
                    Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
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

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupPrimalRage", Name, ValidatorsFeat.IsBarbarianLevel4, primalRageStr, primalRageCon);
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

    #region Close Quarters

    private static FeatDefinition BuildCloseQuarters(List<FeatDefinition> feats)
    {
        const string Family = "CloseQuarters";
        const string Name = "FeatCloseQuarters";

        var featureCloseQuarters = FeatureDefinitionBuilder
            .Create("FeatureCloseQuarters")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureCloseQuarters.SetCustomSubFeatures(new ModifyAdditionalDamageFeatCloseQuarters(featureCloseQuarters));

        var closeQuartersDex = FeatDefinitionBuilder
            .Create($"{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye)
            .SetFeatFamily(Family)
            .SetCustomSubFeatures(new ModifyAdditionalDamageFeatCloseQuarters(featureCloseQuarters))
            .AddToDB();

        var closeQuartersInt = FeatDefinitionBuilder
            .Create($"{Name}Int")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar)
            .SetFeatFamily(Family)
            .SetCustomSubFeatures(new ModifyAdditionalDamageFeatCloseQuarters(featureCloseQuarters))
            .AddToDB();

        feats.AddRange(closeQuartersDex, closeQuartersInt);

        return GroupFeats.MakeGroup(
            "FeatGroupCloseQuarters", Family, closeQuartersDex, closeQuartersInt);
    }

    private sealed class ModifyAdditionalDamageFeatCloseQuarters : IModifyAdditionalDamage
    {
        private readonly FeatureDefinition _featureCloseQuarters;

        public ModifyAdditionalDamageFeatCloseQuarters(FeatureDefinition featureCloseQuarters)
        {
            _featureCloseQuarters = featureCloseQuarters;
        }

        public FeatureDefinitionAdditionalDamage ModifyAdditionalDamage(
            GameLocationBattleManager gameLocationBattleManager,
            FeatureDefinitionAdditionalDamage additionalDamage,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack, AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            // not the best pattern to replace the blueprint directly but best I could do here
            // as on this scenario it's guaranteed all sneak dice are D6 so covered on else
            if (additionalDamage.NotificationTag.EndsWith(TagsDefinitions.AdditionalDamageSneakAttackTag) &&
                gameLocationBattleManager.IsWithin1Cell(attacker, defender))
            {
                additionalDamage.damageDieType = DieType.D8;
                GameConsoleHelper.LogCharacterUsedFeature(attacker.RulesetCharacter, _featureCloseQuarters);
            }
            else
            {
                additionalDamage.damageDieType = DieType.D6;
            }

            return additionalDamage;
        }
    }

    #endregion

    #region Exploiter

    private static FeatDefinition BuildExploiter()
    {
        const string Name = "FeatExploiter";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new ReactToAttackOnMeOrAllyFinishedFeatExploiter())
            .SetValidators(ValidatorsFeat.IsRogueLevel5)
            .AddToDB();
    }

    private class ReactToAttackOnMeOrAllyFinishedFeatExploiter : IReactToAttackOnEnemyFinished
    {
        public IEnumerator HandleReactToAttackOnEnemyFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            GameLocationCharacter ally,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (!me.CanReact())
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            if (!battle.IsWithin1Cell(me, attacker))
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.StringParameter = ally.Name;
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("Exploiter", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);
        }
    }

    #endregion

    #region Awaken The Beast Within

    private static FeatDefinition BuildAwakenTheBeastWithin([NotNull] List<FeatDefinition> feats)
    {
        const string NAME = "FeatAwakenTheBeastWithin";

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
                    .SetFeatures(t.attributeModifier)
                    .SetValidators(ValidatorsFeat.IsDruidLevel4)
                    .SetCustomSubFeatures(new CustomBehaviorFeatAwakenTheBeastWithin())
                    .AddToDB())
            .Cast<FeatDefinition>()
            .ToArray();

        var awakenTheBeastWithinGroup = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupAwakenTheBeastWithin", NAME, ValidatorsFeat.IsDruidLevel4, awakenTheBeastWithinFeats);

        feats.AddRange(awakenTheBeastWithinFeats);

        return awakenTheBeastWithinGroup;
    }

    internal sealed class CustomBehaviorFeatAwakenTheBeastWithin : IActionFinished
    {
        // A towel is just about the most massively useful thing an interstellar hitchhiker can carry
        private const ulong TemporaryHitPointsGuid = 42424242;

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionRevertShape ||
                action.ActingCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
            {
                yield break;
            }

            var rulesetCondition =
                rulesetCharacterMonster.AllConditions.FirstOrDefault(x => x.SourceGuid == TemporaryHitPointsGuid);

            if (rulesetCondition != null)
            {
                rulesetCharacterMonster.RemoveCondition(rulesetCondition);
            }
        }

        // ReSharper disable once InconsistentNaming
        internal static void GrantTempHP(RulesetCharacterMonster __instance)
        {
            if (__instance.OriginalFormCharacter is not RulesetCharacterHero rulesetCharacterHero ||
                !rulesetCharacterHero.TrainedFeats.Exists(x => x.Name.StartsWith("FeatAwakenTheBeastWithin")))
            {
                return;
            }

            var classLevel = rulesetCharacterHero.GetClassLevel(Druid);

            __instance.ReceiveTemporaryHitPoints(2 * classLevel, DurationType.Hour, classLevel / 2,
                TurnOccurenceType.EndOfTurn,
                TemporaryHitPointsGuid);
        }
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
                    .SetProficiencies(ProficiencyType.ToolOrExpertise, PoisonersKitType)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsRangerOrRogueLevel4)
            .AddToDB();
    }

    internal static void TweakUseItemBonusActionId(
        IControllableCharacter __instance,
        ref ActionDefinitions.ActionStatus __result,
        ActionDefinitions.Id actionId)
    {
        // no changes if not an available use item bonus action in battle
        if (Gui.Battle == null ||
            actionId != ActionDefinitions.Id.UseItemBonus ||
            __result != ActionDefinitions.ActionStatus.Available)
        {
            return;
        }

        // no changes if character is Roguish Thief or Grenadier
        var hero = __instance.RulesetCharacter as RulesetCharacterHero ??
                   __instance.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

        if (hero == null ||
            (hero.ClassesAndSubclasses.TryGetValue(Rogue, out var rogueSub) &&
             rogueSub.Name == "RoguishThief") ||
            (hero.ClassesAndSubclasses.TryGetValue(InventorClass.Class, out var inventorSub) &&
             inventorSub.Name == "InnovationAlchemy"))
        {
            return;
        }

        // no changes if device is poison
        __instance.RulesetCharacter.RefreshUsableDeviceFunctions();

        if (__instance.RulesetCharacter.EnumerateAvailableDevices(false)
            .Where(enumerateAvailableDevice =>
                __instance.RulesetCharacter.UsableDeviceFunctionsByDevice.ContainsKey(enumerateAvailableDevice))
            .All(enumerateAvailableDevice =>
                enumerateAvailableDevice.UsableDeviceDescription.UsableDeviceTags.Contains("Poison")))
        {
            return;
        }

        __result = ActionDefinitions.ActionStatus.CannotPerform;
    }

    #endregion

    #region Cunning Escape

    private static FeatDefinition BuildCunningEscape()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCunningEscape")
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new ActionFinishedFeatCunningEscape())
            .SetValidators(ValidatorsFeat.IsRogueLevel3)
            .AddToDB();
    }

    private class ActionFinishedFeatCunningEscape : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action.ActionDefinition != DatabaseHelper.ActionDefinitions.DashBonus)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
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
            .SetCustomSubFeatures(new ActionFinishedHardy())
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

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupHardy", Name, ValidatorsFeat.IsFighterLevel4, hardyStr, hardyCon);
    }

    private sealed class ActionFinishedHardy : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != PowerFighterSecondWind)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(Fighter);
            var dieRoll = RollDie(DieType.D10, AdvantageType.None, out _, out _);
            var healingReceived = classLevel + dieRoll;

            if (rulesetCharacter.TemporaryHitPoints <= healingReceived)
            {
                rulesetCharacter.ReceiveTemporaryHitPoints(healingReceived, DurationType.UntilLongRest, 0,
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
            var repertoire = character.GetClassSpellRepertoire(Druid);
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

            character.GetClassSpellRepertoire(Druid)?
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
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition is not SpellDefinition spellDefinition ||
                !_spellListDefinition.SpellsByLevel
                    .Any(x => x.Level == 0 && x.Spells.Contains(spellDefinition)))
            {
                return effectDescription;
            }

            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return effectDescription;
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
                return effectDescription;
            }

            var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(character.TryGetAttributeValue(attribute));

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                "Feat/&FeatPotentSpellcasterTitle", null));

            return effectDescription;
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
                    new ValidatorsPowerUse(c =>
                        c.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber) > c.UsedChannelDivinity))
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

                            c.GetClassSpellRepertoire(Cleric)?
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
                .SetCustomSubFeatures(new ActionFinishedFeatSpiritualFluidity())
                .AddToDB();
    }

    private sealed class ActionFinishedFeatSpiritualFluidity : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
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
                    var repertoire = character.GetClassSpellRepertoire(Cleric);

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

            yield break;
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

                            c.GetClassSpellRepertoire(Ranger)?
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
                new ActionFinishedFeatSlayTheEnemies())
            .AddToDB();
    }

    private sealed class OnComputeAttackModifierSlayTheEnemies : IPhysicalAttackInitiated
    {
        private readonly FeatureDefinition _featureDefinition;

        public OnComputeAttackModifierSlayTheEnemies(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public IEnumerator OnAttackInitiated(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var rulesetCharacter = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetCharacter == null || rulesetDefender == null)
            {
                yield break;
            }

            if (ValidatorsCharacter.HasNoneOfConditions(
                    "ConditionFeatSlayTheEnemies1",
                    "ConditionFeatSlayTheEnemies2",
                    "ConditionFeatSlayTheEnemies3")(rulesetCharacter))
            {
                yield break;
            }

            if (attackerAttackMode.ToHitBonusTrends.Any(x => x.source as FeatureDefinition == _featureDefinition))
            {
                yield break;
            }

            var damage = attackerAttackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                yield break;
            }

            var spellLevel = 0;

            if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies1")(rulesetCharacter))
            {
                spellLevel = 1;
            }
            else if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies2")(rulesetCharacter))
            {
                spellLevel = 2;
            }
            else if (ValidatorsCharacter.HasAnyOfConditions("ConditionFeatSlayTheEnemies3")(rulesetCharacter))
            {
                spellLevel = 3;
            }

            if (IsFavoriteEnemy(rulesetCharacter, rulesetDefender))
            {
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
            }
            else
            {
                attackerAttackMode.ToHitBonus += spellLevel;
                attackerAttackMode.ToHitBonusTrends.Add(new TrendInfo(spellLevel, FeatureSourceType.CharacterFeature,
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

    private sealed class ActionFinishedFeatSlayTheEnemies : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePowerSlayTheEnemies ||
                !characterActionUsePowerSlayTheEnemies.activePower.PowerDefinition.Name.StartsWith(
                    "PowerFeatSlayTheEnemies"))
            {
                yield break;
            }

            var character = action.ActingCharacter.RulesetCharacter;
            var name = characterActionUsePowerSlayTheEnemies.activePower.PowerDefinition.Name;
            var level = int.Parse(name.Substring(name.Length - 1, 1));
            var repertoire = character.GetClassSpellRepertoire(Ranger);

            repertoire?.SpendSpellSlot(level);
        }
    }

    #endregion

    #region Devastating Strikes

    private static FeatDefinition BuildDevastatingStrikes()
    {
        const string NAME = "FeatDevastatingStrikes";

        var weaponTypes = new[] { GreatswordType, GreataxeType, MaulType };

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(
                new AttackEffectAfterDamageFeatDevastatingStrikes(weaponTypes),
                new ModifyWeaponAttackModeTypeFilter(
                    $"Feature/&ModifyAttackMode{NAME}Title", weaponTypes))
            .AddToDB();

        return feat;
    }

    private sealed class AttackEffectAfterDamageFeatDevastatingStrikes : IAttackEffectAfterDamage
    {
        private const string DevastatingStrikesDescription = "Feat/&FeatDevastatingStrikesTitle";
        private const string DevastatingStrikesTitle = "Feat/&FeatDevastatingStrikesDescription";
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public AttackEffectAfterDamageFeatDevastatingStrikes(params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null)
            {
                return;
            }

            var modifier = attackMode.ToHitBonus + attackModifier.AttackRollModifier;

            if (attackModifier.AttackAdvantageTrend <= 0)
            {
                return;
            }

            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                return;
            }

            var lowerRoll = Math.Min(Global.FirstAttackRoll, Global.SecondAttackRoll);
            var lowOutcome =
                GameLocationBattleManagerTweaks.GetAttackResult(lowerRoll, modifier, rulesetDefender);

            Gui.Game.GameConsole.AttackRolled(
                rulesetAttacker,
                rulesetDefender,
                attackMode.SourceDefinition,
                lowOutcome,
                lowerRoll + modifier,
                lowerRoll,
                modifier,
                attackModifier.AttacktoHitTrends,
                new List<TrendInfo>());

            if (lowOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                return;
            }

            var strength = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Strength);
            var strengthMod = AttributeDefinitions.ComputeAbilityScoreModifier(strength);
            var dexterity = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Dexterity);
            var dexterityMod = AttributeDefinitions.ComputeAbilityScoreModifier(dexterity);

            if (strengthMod <= 0 && dexterityMod <= 0)
            {
                return;
            }

            GameConsoleHelper.LogCharacterAffectsTarget(rulesetAttacker, rulesetDefender,
                //TODO: move this feedback term to others-en.txt
                DevastatingStrikesTitle, "Feedback/&FeatFeatFellHandedDisadvantage",
                tooltipContent: DevastatingStrikesDescription);

            var originalDamageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (originalDamageForm == null)
            {
                return;
            }

            var damage = new DamageForm
            {
                DamageType = originalDamageForm.DamageType,
                DieType = DieType.D1,
                DiceNumber = 0,
                BonusDamage = Math.Max(strengthMod, dexterityMod)
            };

            RulesetActor.InflictDamage(
                strengthMod,
                damage,
                DamageTypeBludgeoning,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                attacker.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(DieType.D1, new List<int>(), strengthMod),
                true,
                out _);

            if (outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            var advantageType = attackModifier.AttackAdvantageTrend switch
            {
                > 0 => AdvantageType.Advantage,
                < 0 => AdvantageType.Disadvantage,
                _ => AdvantageType.None
            };

            var dieRoll = RollDie(originalDamageForm.DieType, advantageType, out var _, out var _);

            rulesetDefender.SustainDamage(
                dieRoll,
                originalDamageForm.DamageType,
                false,
                attacker.Guid,
                new RollInfo(originalDamageForm.DieType, new List<int> { dieRoll }, 0),
                out _);
        }
    }

    #endregion
}
