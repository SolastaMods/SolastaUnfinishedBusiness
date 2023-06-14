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
                        .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"Condition{NAME}")
                                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
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
            .SetFeatFamily("BlessedSoul")
            .AddToDB();

        var blessedSoulPaladin = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Paladin")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierClericChannelDivinityAdd,
                AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsPaladinLevel4)
            .SetFeatFamily("BlessedSoul")
            .AddToDB();

        feats.AddRange(blessedSoulCleric, blessedSoulPaladin);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupBlessedSoul", "BlessedSoul", ValidatorsFeat.IsClericOrPaladinLevel4,
            blessedSoulCleric,
            blessedSoulPaladin);
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
            .SetFeatFamily(Name)
            .AddToDB();

        var primalRageCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierBarbarianRagePointsAdd,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsBarbarianLevel4)
            .SetFeatFamily(Name)
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

        DieType UpgradeCloseQuartersDice(
            FeatureDefinitionAdditionalDamage additionalDamage,
            DamageForm damageForm,
            RulesetAttackMode attackMode,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (attackMode.Ranged ||
                !additionalDamage.NotificationTag.EndsWith(TagsDefinitions.AdditionalDamageSneakAttackTag) ||
                gameLocationBattleService == null ||
                !gameLocationBattleService.IsWithin1Cell(attacker, defender))
            {
                return additionalDamage.DamageDieType;
            }

            attacker.RulesetCharacter.LogCharacterUsedFeature(featureCloseQuarters);

            return DieType.D8;
        }

        featureCloseQuarters.SetCustomSubFeatures((DamageDieProviderFromCharacter)UpgradeCloseQuartersDice);

        static (bool result, string output) HasSneakAttack(FeatDefinition feat, RulesetCharacterHero hero)
        {
            var hasSneakAttack = hero.GetFeaturesByType<FeatureDefinitionAdditionalDamage>()
                .Any(x => x.NotificationTag.EndsWith(TagsDefinitions.AdditionalDamageSneakAttackTag));

            var guiFormat = Gui.Format("Tooltip/&PreReqMustKnow", "Feature/&RogueSneakAttackTitle");

            return hasSneakAttack ? (true, guiFormat) : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        }

        var closeQuartersDex = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(featureCloseQuarters, AttributeModifierCreed_Of_Misaye)
            .SetFeatFamily(Family)
            .SetValidators(HasSneakAttack)
            .AddToDB();

        var closeQuartersInt = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Int")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(featureCloseQuarters, AttributeModifierCreed_Of_Einar)
            .SetFeatFamily(Family)
            .SetValidators(HasSneakAttack)
            .AddToDB();

        feats.AddRange(closeQuartersDex, closeQuartersInt);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupCloseQuarters", Family, HasSneakAttack, closeQuartersDex, closeQuartersInt);
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
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new ValidateDeviceFunctionUse((_, device, _) =>
                        device.UsableDeviceDescription.UsableDeviceTags.Contains("Poison")))
                    .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
                    .AddToDB(),
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

    #endregion

    #region Exploiter

    private static FeatDefinition BuildExploiter()
    {
        const string Name = "FeatExploiter";

        var featureExploiter = FeatureDefinitionBuilder
            .Create("FeatureExploiter")
            .SetGuiPresentation("FeatExploiter", Category.Feat)
            .AddToDB();

        featureExploiter.SetCustomSubFeatures(new ReactToAttackOnMeOrAllyFinishedFeatExploiter(featureExploiter));

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(featureExploiter)
            .SetValidators(ValidatorsFeat.IsRogueLevel5)
            .AddToDB();
    }

    private class ReactToAttackOnMeOrAllyFinishedFeatExploiter : IReactToAttackOnEnemyFinished
    {
        private readonly FeatureDefinition _featureExploiter;

        public ReactToAttackOnMeOrAllyFinishedFeatExploiter(FeatureDefinition featureExploiter)
        {
            _featureExploiter = featureExploiter;
        }

        public IEnumerator HandleReactToAttackOnEnemyFinished(
            GameLocationCharacter ally,
            GameLocationCharacter me,
            GameLocationCharacter enemy,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            //do not trigger on my own turn, so won't exploit on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            var rulesetEnemy = enemy.RulesetCharacter;

            if (!me.CanReact() ||
                me == ally ||
                rulesetEnemy == null ||
                rulesetEnemy.IsDeadOrDying)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(enemy);

            if (retaliationMode == null || retaliationMode.ranged)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(enemy);
            reactionParams.StringParameter = ally.Name;
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("Exploiter", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            me.RulesetCharacter.LogCharacterUsedFeature(_featureExploiter);
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
                    .SetFeatFamily(NAME)
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
            .SetFeatFamily(Name)
            .AddToDB();

        var hardyCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsFighterLevel4)
            .SetFeatFamily(Name)
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

        var spellLists = new List<List<SpellListDefinition>>
        {
            new() { SpellListDefinitions.SpellListBard },
            new() { SpellListDefinitions.SpellListCleric },
            new()
            {
                SpellListDefinitions.SpellListDruid,
                GetDefinition<SpellListDefinition>("SpellListFeatSpellSniperDruid")
            },
            new()
            {
                SpellListDefinitions.SpellListSorcerer,
                GetDefinition<SpellListDefinition>("SpellListFeatSpellSniperSorcerer")
            },
            new()
            {
                SpellListDefinitions.SpellListWizard,
                GetDefinition<SpellListDefinition>("SpellListFeatSpellSniperWizard")
            },
            new() { InventorClass.SpellList }
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
            var className = spellList[0].Name.Replace("SpellList", String.Empty);
            var classTitle = GetDefinition<CharacterClassDefinition>(className).FormatTitle();
            var featPotentSpellcaster = FeatDefinitionWithPrerequisitesBuilder
                .Create($"{Name}{className}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatPotentSpellcasterTitle", classTitle),
                    Gui.Format("Feat/&FeatPotentSpellcasterDescription", classTitle))
                .SetCustomSubFeatures(new ModifyMagicEffectFeatPotentSpellcaster(spellList.ToArray()))
                .SetValidators(validator)
                .SetFeatFamily("PotentSpellcaster")
                .AddToDB();

            potentSpellcasterFeats.Add(featPotentSpellcaster);
        }

        var potentSpellcasterGroup = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupPotentSpellcaster", "PotentSpellcaster", ValidatorsFeat.IsLevel4,
            potentSpellcasterFeats.ToArray());

        feats.AddRange(potentSpellcasterFeats);

        return potentSpellcasterGroup;
    }

    private sealed class ModifyMagicEffectFeatPotentSpellcaster : IModifyMagicEffect
    {
        private readonly SpellListDefinition[] _spellListDefinition;

        public ModifyMagicEffectFeatPotentSpellcaster(params SpellListDefinition[] spellListDefinition)
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
                !_spellListDefinition.Any(x => x.SpellsByLevel
                    .Any(y => y.Level == 0 && y.Spells.Contains(spellDefinition))))
            {
                return effectDescription;
            }

            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return effectDescription;
            }

            string attribute;

            if (_spellListDefinition[0] == SpellListDefinitions.SpellListBard ||
                _spellListDefinition[0] == SpellListDefinitions.SpellListSorcerer)
            {
                attribute = AttributeDefinitions.Charisma;
            }
            else if (_spellListDefinition[0] == SpellListDefinitions.SpellListCleric ||
                     _spellListDefinition[0] == SpellListDefinitions.SpellListDruid)
            {
                attribute = AttributeDefinitions.Wisdom;
            }
            else if (_spellListDefinition[0] == SpellListDefinitions.SpellListWizard ||
                     _spellListDefinition[0] == InventorClass.SpellList)
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

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("SlayTheEnemy")
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.FlatWithProgression)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetIgnoreCriticalDoubleDice(true)
            .SetFlatDamageBonus(0)
            .SetAdvancement(ExtraAdditionalDamageAdvancement.ConditionAmount,
                DiceByRankBuilder.Build((1, 1), (2, 2), (3, 3)))
            .AddToDB();

        var advantageOnFavorite = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}Favorite")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetSituationalContext(ExtraSituationalContext.TargetIsFavoriteEnemy)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var toHitOnRegular = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}Regular")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetMyAttackModifier(ExtraCombatAffinityValueDetermination.ConditionAmountIfNotFavoriteEnemy)
            .AddToDB();

        for (var i = 3; i >= 1; i--)
        {
            // closure
            var a = i;

            var rounds = 2 + i;

            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}{i}")
                .SetGuiPresentation(
                    Gui.Format($"Feature/&Power{NAME}Title", Gui.ToRoman(i)),
                    Gui.Format($"Feature/&Power{NAME}Description", i.ToString(), rounds.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, powerPool)
                .SetEffectDescription(EffectDescriptionBuilder.Create()
                    .SetDurationData(DurationType.Round, rounds)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(
                        ConditionDefinitionBuilder
                            .Create($"Condition{NAME}{i}")
                            .SetGuiPresentation(
                                "Condition/&ConditionFeatSlayTheEnemiesTitle",
                                Gui.Format("Condition/&ConditionFeatSlayTheEnemiesDescription",
                                    i.ToString()), ConditionDefinitions.ConditionTrueStrike)
                            .SetPossessive()
                            .SetFixedAmount(i)
                            .SetFeatures(additionalDamage, advantageOnFavorite, toHitOnRegular)
                            .AddToDB()))
                    .Build())
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        c =>
                        {
                            var remaining = 0;

                            c.GetClassSpellRepertoire(Ranger)?
                                .GetSlotsNumber(a, out remaining, out _);

                            var noCondition = !c.HasAnyConditionOfType(
                                "ConditionFeatSlayTheEnemies1",
                                "ConditionFeatSlayTheEnemies2",
                                "ConditionFeatSlayTheEnemies3");

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
            .SetCustomSubFeatures(new ActionFinishedFeatSlayTheEnemies())
            .AddToDB();
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
}
