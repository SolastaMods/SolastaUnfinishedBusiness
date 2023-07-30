#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal class PatronEldritchSurge : AbstractSubclass
{
    public const string Name = "PatronEldritchSurge";

    private const string PowerBlastPursuitName = $"Power{Name}BlastPursuit";

    // LEVEL 06 Blast Pursuit
    // Original name to keep save integrity
    private static readonly FeatureDefinitionAdditionalAction AdditionalActionBlastPursuit =
        FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BlastPursuit")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(AllowDuplicates.Mark)
            .SetActionType(ActionType.Main)
            .SetForbiddenActions(Id.AttackMain)
            .AddToDB();

    private static readonly FeatureDefinitionAdditionalAction AdditionalActionBlastPursuitOff =
        FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BlastPursuitOff")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(AllowDuplicates.Mark)
            .SetActionType(ActionType.Bonus)
            .SetForbiddenActions(Id.AttackOff)
            .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(AdditionalActionBlastPursuit)
        .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuitOff = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuitOff")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(AdditionalActionBlastPursuitOff)
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
        .SetConditionParticleReference(ConditionDefinitions.ConditionShine.conditionParticleReference)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new OnTargetReducedToZeroHpBlastPursuit())
                .AddToDB())
        .AddToDB();

    // LEVEL 10 Blast Overload

    private static readonly ConditionDefinition ConditionBlastOverload = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastOverload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
        .CopyParticleReferences(ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageInvocationAgonizingBlast, $"AdditionalDamage{Name}BlastOverload")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("BlastOverload")
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                .AddToDB(),
            FeatureDefinitionMagicAffinityBuilder
                .Create($"MagicAffinity{Name}BlastOverload")
                .SetGuiPresentationNoContent(true)
                .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
                .AddToDB())
        .AddToDB();

    // LEVEL 14 Blast Reload

    private static readonly ConditionDefinition ConditionBlastReloadSupport = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastReloadSupport")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    public static readonly FeatureDefinition FeatureBlastReload = FeatureDefinitionBuilder
        .Create($"Feature{Name}BlastReload")
        .SetGuiPresentation(Category.Feature)
        .SetCustomSubFeatures(new CustomBehaviorBlastReload())
        .AddToDB();

    internal PatronEldritchSurge()
    {
        // LEVEL 01

        // Blast Exclusive

        var bonusCantripsEldritchSurgeBlastExclusive = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}BlastExclusive")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(EldritchBlast)
            .SetCustomSubFeatures(new ModifyEffectDescriptionEldritchBlast())
            .AddToDB();

        // LEVEL 06

        // Blast Pursuit

        var powerBlastPursuit = FeatureDefinitionPowerBuilder
            .Create(PowerBlastPursuitName)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 2, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(Haste)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionBlastPursuit),
                    EffectFormBuilder.ConditionForm(ConditionExtraActionBlastPursuit),
                    EffectFormBuilder.ConditionForm(ConditionExtraActionBlastPursuitOff))
                .Build())
            .AddToDB();

        powerBlastPursuit.effectDescription.effectParticleParameters.casterParticleReference = null;

        powerBlastPursuit.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastPursuit, ConditionBlastPursuit),
            new ExtraActionBlastPursuit());

        // LEVEL 10

        // Blast Overload

        var powerBlastOverload = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlastOverload")
            .SetGuiPresentation(Category.Feature, EldritchBlast)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastOverload))
                .Build())
            .AddToDB();

        powerBlastOverload.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastOverload, ConditionBlastOverload));

        // LEVEL 14

        // Blast Reload
        // Moved to static property

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1, bonusCantripsEldritchSurgeBlastExclusive)
            .AddFeaturesAtLevel(6, powerBlastPursuit)
            .AddFeaturesAtLevel(10, powerBlastOverload)
            .AddFeaturesAtLevel(14, FeatureBlastReload)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool IsEldritchBlast(RulesetEffect rulesetEffect)
    {
        return rulesetEffect is RulesetEffectSpell rulesetEffectSpell &&
               rulesetEffectSpell.SpellDefinition == EldritchBlast;
    }

    // place this method elsewhere fit
    public static T CreateCustomCondition<T>(
        ulong targetGuid,
        ConditionDefinition conditionDefinition,
        int effectLevel = 1,
        int amount = 0,
        int sourceAbilityBonus = 0,
        int sourceProficiencyBonus = 0) where T : RulesetCondition, new()
    {
        var rulesetCondition = new T();

        rulesetCondition.ResetGuid();
        rulesetCondition.Clear();
        rulesetCondition.targetGuid = targetGuid;

        if (conditionDefinition.SpecialDuration)
        {
            rulesetCondition.durationType = conditionDefinition.DurationType;
            rulesetCondition.durationParameter = RollStaticDiceAndSum(
                conditionDefinition.DurationParameter, conditionDefinition.DurationParameterDie);
            rulesetCondition.remainingRounds = ComputeRoundsDuration(
                rulesetCondition.durationType, rulesetCondition.durationParameter);
        }
        else
        {
            rulesetCondition.durationType = DurationType.Irrelevant;
            rulesetCondition.durationParameter = 0;
            rulesetCondition.remainingRounds = 0;
        }

        rulesetCondition.endOccurence = TurnOccurenceType.EndOfTurn;
        rulesetCondition.ConditionDefinition = conditionDefinition;
        rulesetCondition.sourceGuid = 0UL;
        rulesetCondition.sourceFactionName = string.Empty;
        rulesetCondition.effectLevel = effectLevel;
        rulesetCondition.effectDefinitionName = string.Empty;

        if (conditionDefinition.AmountOrigin != ConditionDefinition.OriginOfAmount.None)
        {
            rulesetCondition.amount = amount;
        }

        rulesetCondition.sourceAbilityBonus = sourceAbilityBonus;
        rulesetCondition.sourceProficiencyBonus = sourceProficiencyBonus;
        rulesetCondition.doNotTerminateWhenRemoved = false;

        return rulesetCondition;
    }

    public sealed class ModifyEffectDescriptionEldritchBlast : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == EldritchBlast
                   && character.GetOriginalHero() != null
                   && character.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) > 0;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter rulesetCharacter,
            RulesetEffect rulesetEffect)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return effectDescription;
            }
            var totalLevel = rulesetHero.classesHistory.Count;
            var warlockClassLevel = rulesetHero.GetClassLevel(CharacterClassDefinitions.Warlock);
            var additionalBeamCount = ComputeAdditionalBeamCount(totalLevel, warlockClassLevel);
            var pursuitStatus = rulesetCharacter.HasConditionOfType(ConditionBlastPursuit)
                ? 1 + (warlockClassLevel >= 16 ? 1 : 0)
                : 0;
            var overloadStatus = rulesetCharacter.HasConditionOfType(ConditionBlastOverload) ? 1 : 0;

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;
            effectDescription.rangeParameter = Math.Max(1, 24 - (6 * (pursuitStatus + overloadStatus)));

            return effectDescription;
        }

        public static int ComputeAdditionalBeamCount(int totalLevel, int warlockClassLevel)
        {
            var determinantLevel = warlockClassLevel - (2 * (totalLevel - warlockClassLevel));
            var increaseLevels = new[] { 3, 8, 13, 18 };
            return increaseLevels.Count(level => determinantLevel >= level);
        }
    }

    private sealed class CustomBehaviorBlastPursuitOrOverload : IUsePowerFinishedByMe, IPowerUseValidity
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _triggerPower;

        public CustomBehaviorBlastPursuitOrOverload(
            FeatureDefinitionPower triggerPower,
            ConditionDefinition conditionDefinition)
        {
            _triggerPower = triggerPower;
            _conditionDefinition = conditionDefinition;
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            return !rulesetCharacter.HasConditionOfType(_conditionDefinition) &&
                   SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) >= 1;
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power != _triggerPower)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var slotLevel = SharedSpellsContext.GetWarlockSpellLevel(rulesetHero);

            SharedSpellsContext.GetWarlockSpellRepertoire(rulesetHero)?.SpendSpellSlot(slotLevel);
        }
    }

    private sealed class OnTargetReducedToZeroHpBlastPursuit : IOnTargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!IsEldritchBlast(activeEffect) ||
                !rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionBlastPursuit.Name, out var rulesetCondition))
            {
                yield break;
            }

            rulesetCondition.EndOccurence = TurnOccurenceType.EndOfTurn;
            rulesetCondition.RemainingRounds += 1; // always use setter to avoid issues with game effects manager

            var rulesetEffectPower =
                attacker.RulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.Name == PowerBlastPursuitName);

            if (rulesetEffectPower != null)
            {
                rulesetEffectPower.RemainingRounds += 1; // always use setter to avoid issues with game effects manager
            }
        }
    }

    private sealed class ExtraActionBlastPursuit : IActionExecutionHandled, ICharacterTurnStartListener
    {
        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // skip if wrong scope or action type
            if (scope != ActionScope.Battle)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.HasConditionOfType(ConditionBlastPursuit))
            {
                return;
            }

            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var eldritchSurgeLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            if (eldritchSurgeLevel >= 16)
            {
                return;
            }

            var actionType = actionParams.ActionDefinition.ActionType;
            var featureToCheck = actionType switch
            {
                ActionType.Main => AdditionalActionBlastPursuit,
                ActionType.Bonus => AdditionalActionBlastPursuitOff,
                _ => null
            };
            var conditionToRemove = actionType switch
            {
                ActionType.Main => ConditionExtraActionBlastPursuitOff.Name,
                ActionType.Bonus => ConditionExtraActionBlastPursuit.Name,
                _ => null
            };

            if (featureToCheck == null)
            {
                return;
            }

            var filters = gameLocationCharacter.ActionPerformancesByType[actionType];
            var k = gameLocationCharacter.CurrentActionRankByType[actionType] - 1;
            var f = k >= 0 && k < filters.Count ? PerformanceFilterExtraData.GetData(filters[k]) : null;
            var feature = f?.Feature;

            if (feature == featureToCheck)
            {
                rulesetCharacter.RemoveAllConditionsOfType(conditionToRemove);
            }
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var eldritchSurgeLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            if (eldritchSurgeLevel == 20 && SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) <= 0)
            {
                InflictCondition(rulesetCharacter, ConditionBlastPursuit);
            }

            if (!rulesetCharacter.HasConditionOfType(ConditionBlastPursuit))
            {
                return;
            }

            InflictCondition(rulesetCharacter, ConditionExtraActionBlastPursuit);
            InflictCondition(rulesetCharacter, ConditionExtraActionBlastPursuitOff);
        }

        private static void InflictCondition(RulesetCharacter rulesetCharacter, ConditionDefinition condition)
        {
            if (rulesetCharacter.HasConditionOfType(condition))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0
            );

            if (condition != ConditionBlastPursuit)
            {
                return;
            }

            var title = condition.GuiPresentation.Title;

            rulesetCharacter.ShowLabel(title, Gui.ColorPositive);
            rulesetCharacter.LogCharacterActivatesAbility(title, "Feedback/&BlastPursuitExtraAction", true);
        }
    }

    private sealed class CustomBehaviorBlastReload :
        IActionExecutionHandled, ICharacterTurnStartListener, IQualifySpellToRepertoireLine
    {
        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // only collect cantrips
            if (scope != ActionScope.Battle ||
                actionParams.activeEffect is not RulesetEffectSpell rulesetEffectSpell ||
                rulesetEffectSpell.SpellDefinition.SpellLevel != 0)
            {
                return;
            }

            // only collect cantrips if an Eldritch Surge of at least level 14
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCharacter.GetOriginalHero().GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) < 14)
            {
                return;
            }

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType("10Combat", ConditionBlastReloadSupport.Name,
                    out var supportCondition))
            {
                supportCondition =
                    CreateCustomCondition<BlastReloadSupportRulesetCondition>(rulesetCharacter.guid,
                        ConditionBlastReloadSupport);
                rulesetCharacter.AddConditionOfCategory("10Combat", supportCondition);
            }

            var eldritchSurgeSupportCondition = supportCondition as BlastReloadSupportRulesetCondition;

            eldritchSurgeSupportCondition?.CantripsUsedThisTurn.TryAdd(rulesetEffectSpell.SpellDefinition);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter gameLocationCharacter)
        {
            // clean up cantrips list on every turn start
            // combat condition will be removed automatically after combat
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCharacter.GetOriginalHero().GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) < 14)
            {
                return;
            }

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionBlastReloadSupport.Name, out var supportCondition))
            {
                return;
            }

            var eldritchSurgeSupportCondition = supportCondition as BlastReloadSupportRulesetCondition;

            eldritchSurgeSupportCondition?.CantripsUsedThisTurn.Clear();
        }

        public void QualifySpells(
            RulesetCharacter rulesetCharacter,
            SpellRepertoireLine spellRepertoireLine,
            IEnumerable<SpellDefinition> spells)
        {
            // _cantripsUsedThisTurn only has entries for Eldritch Surge of at least level 14
            if (spellRepertoireLine.actionType != ActionType.Bonus ||
                !rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionBlastReloadSupport.Name, out var supportCondition))
            {
                return;
            }

            if (supportCondition is BlastReloadSupportRulesetCondition eldritchSurgeSupportCondition)
            {
                spellRepertoireLine.relevantSpells.AddRange(
                    eldritchSurgeSupportCondition.CantripsUsedThisTurn.Intersect(spells));
            }
        }
    }

    private class BlastReloadSupportRulesetCondition : RulesetCondition
    {
        public List<SpellDefinition> CantripsUsedThisTurn { get; } = new();

        public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
        {
            base.SerializeElements(serializer, versionProvider);

            try
            {
                BaseDefinition.SerializeDatabaseReferenceList(
                    serializer, "CantripsUsedThisTurn", "SpellDefinition", CantripsUsedThisTurn);

                if (serializer.Mode == Serializer.SerializationMode.Read)
                {
                    CantripsUsedThisTurn.RemoveAll(x => x is null);
                }
            }
            catch (Exception ex)
            {
                Trace.LogException(
                    new Exception("Error with EldritchSurgeSupportCondition serialization" + ex.Message, ex));
            }
        }
    }
}
#endif
