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
    private const string Name = "PatronEldritchSurge";

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
        .AllowMultipleInstances()
        .SetFeatures(AdditionalActionBlastPursuit)
        .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuitOff = ConditionDefinitionBuilder
    .Create($"Condition{Name}ExtraActionBlastPursuitOff")
    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
    .SetSilent(Silent.WhenAddedOrRemoved)
    .AllowMultipleInstances()
    .SetFeatures(AdditionalActionBlastPursuitOff)
    .AddToDB();

    private static readonly ConditionDefinition ConditionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new TargetReducedToZeroHpBlastPursuit())
                .AddToDB())
        .AddToDB();

    // LEVEL 10 Blast Overload

    private static readonly ConditionDefinition ConditionBlastOverload = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastOverload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
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

    internal PatronEldritchSurge()
    {
        // LEVEL 01

        // Blast Exclusive

        EldritchBlast.SetCustomSubFeatures(new ModifyMagicEffectEldritchBlast());

        var bonusCantripsEldritchSurgeBlastExclusive = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}BlastExclusive")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(EldritchBlast)
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
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastPursuit))
                .Build())
            .AddToDB();

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

        var featureBlastReload = FeatureDefinitionBuilder
            .Create($"Feature{Name}BlastReload")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomBehaviorBlastReload())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1, bonusCantripsEldritchSurgeBlastExclusive)
            .AddFeaturesAtLevel(6, powerBlastPursuit)
            .AddFeaturesAtLevel(10, powerBlastOverload)
            .AddFeaturesAtLevel(14, featureBlastReload)
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

    private static int GetBlastPursuitExtraActionCount(RulesetCharacter rulesetCharacter)
    {
        return 
               (rulesetCharacter.HasConditionOfType(ConditionExtraActionBlastPursuit.Name)?1:0) +
               (rulesetCharacter.HasConditionOfType(ConditionExtraActionBlastPursuitOff.Name)?1:0);
    }

    private sealed class ModifyMagicEffectEldritchBlast : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter rulesetCharacter,
            RulesetEffect rulesetEffect)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) == 0)
            {
                return effectDescription;
            }

            var totalLevel = rulesetHero.classesHistory.Count;
            var warlockClassLevel = rulesetHero.GetClassLevel(CharacterClassDefinitions.Warlock);
            var determinantLevel = warlockClassLevel - (2 * (totalLevel - warlockClassLevel));
            var increaseLevels = new[] { 3, 8, 13, 18 };
            var additionalBeamCount = increaseLevels.Count(level => determinantLevel >= level);
            var blastPursuitExtraActionCount = GetBlastPursuitExtraActionCount(rulesetCharacter);
            if(warlockClassLevel < 16)
            { 
                blastPursuitExtraActionCount = Math.Min(0, blastPursuitExtraActionCount);
            }
            var overloadStatus = rulesetCharacter.HasConditionOfType(ConditionBlastOverload) ? 1 : 0;

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;
            effectDescription.rangeParameter = Math.Max(1, 24 - (6 * (blastPursuitExtraActionCount + overloadStatus)));

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorBlastPursuitOrOverload : IActionFinished, IPowerUseValidity
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

        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _triggerPower)
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var slotLevel = SharedSpellsContext.GetWarlockSpellLevel(rulesetHero);

            SharedSpellsContext.GetWarlockSpellRepertoire(rulesetHero)?.SpendSpellSlot(slotLevel);
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            return !rulesetHero.HasConditionOfType(_conditionDefinition.Name) &&
                   SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) >= 1;
        }
    }

    private sealed class TargetReducedToZeroHpBlastPursuit : ITargetReducedToZeroHp
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

    private sealed class ExtraActionBlastPursuit : IActionExecutionHandled
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

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var warlockLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            var actionType = actionParams.ActionDefinition.ActionType;

            switch(actionType)
            {
                case ActionType.Main:
                    if(gameLocationCharacter.GetActionAvailableIterations(Id.AttackMain) > 0)
                    {
                        return;
                    }
                    if(rulesetCharacter.HasConditionOfType(ConditionExtraActionBlastPursuit))
                    {
                        if(warlockLevel < 16)
                        {
                            rulesetCharacter.RemoveAllConditionsOfType(ConditionExtraActionBlastPursuitOff.Name);
                        }
                        return;
                    }
                    
                    break;
                case ActionType.Bonus:
                    if (rulesetCharacter.HasConditionOfType(ConditionExtraActionBlastPursuitOff))
                    {
                        if (warlockLevel < 16)
                        {
                            rulesetCharacter.RemoveAllConditionsOfType(ConditionExtraActionBlastPursuit.Name);
                        }
                        return;
                    }
                    break;
                default:
                    return;
            }

            var isValidBlastPursuit =
                rulesetCharacter.HasConditionOfType(ConditionBlastPursuit) ||
                (rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) >= 16 &&
                SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) <= 0);

            if (isValidBlastPursuit)
            {
                InflictCondition(rulesetCharacter, actionType);
            }  
        }

        private static void InflictCondition(RulesetCharacter rulesetCharacter,ActionType actionType)
        {
            var condition = actionType == ActionType.Main ? ConditionExtraActionBlastPursuit : ConditionExtraActionBlastPursuitOff;
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

            var title = ConditionBlastPursuit.GuiPresentation.Title;

            rulesetCharacter.ShowLabel(title, Gui.ColorPositive);
            rulesetCharacter.LogCharacterActivatesAbility(title, "Feedback/&BlastPursuitExtraAction", true);
        }
    }

    private sealed class CustomBehaviorBlastReload :
        IActionExecutionHandled, ICharacterTurnStartListener, IQualifySpellToRepertoireLine
    {
        // use a map to ensure any collateral scenarios with other warlocks casting cantrips on this warlock turn
        private readonly Dictionary<RulesetCharacter, List<SpellDefinition>> _cantripsUsedThisTurn = new();

        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // only collect cantrips
            if (actionParams.activeEffect is not RulesetEffectSpell rulesetEffectSpell ||
                rulesetEffectSpell.SpellDefinition.SpellLevel != 0)
            {
                return;
            }

            // only collect cantrips if an Eldritch Surge of at least level 14
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) < 14)
            {
                return;
            }

            _cantripsUsedThisTurn.TryAdd(rulesetCharacter, new List<SpellDefinition>());
            _cantripsUsedThisTurn[rulesetCharacter].TryAdd(rulesetEffectSpell.SpellDefinition);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // clean up cantrips map on every turn start
            _cantripsUsedThisTurn.Clear();
        }

        public void QualifySpells(
            RulesetCharacter rulesetCharacter,
            SpellRepertoireLine spellRepertoireLine,
            IEnumerable<SpellDefinition> spells)
        {
            // _cantripsUsedThisTurn only has entries for Eldritch Surge of at least level 14
            if (spellRepertoireLine.actionType != ActionType.Bonus ||
                !_cantripsUsedThisTurn.TryGetValue(rulesetCharacter, out var cantrips))
            {
                return;
            }

            spellRepertoireLine.relevantSpells.AddRange(cantrips.Intersect(spells));
        }
    }
}
