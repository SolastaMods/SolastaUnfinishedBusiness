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

namespace SolastaUnfinishedBusiness.Subclasses;

internal class PatronEldritchSurge : AbstractSubclass
{
    private const string Name = "PatronEldritchSurge";

    private const string PowerBlastPursuitName = $"Power{Name}BlastPursuit";

    // LEVEL 03 conditions

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AllowMultipleInstances()
        .SetFeatures(
            FeatureDefinitionAdditionalActionBuilder
                .Create($"AdditionalAction{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(AllowDuplicates.Mark)
                .SetActionType(ActionType.Main)
                .SetRestrictedActions(
                    Id.AttackMain,
                    Id.CastMain,
                    Id.DashMain,
                    Id.DisengageMain,
                    Id.Dodge)
                .SetMaxAttacksNumber(1)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new ExtraActionBlastPursuit(),
                    new TargetReducedToZeroHpBlastPursuit())
                .AddToDB())
        .AddToDB();

    // LEVEL 10 conditions

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

    // LEVEL 14 conditions

    private static readonly ConditionDefinition ConditionBlastReload = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastReload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    // LEVEL 18 conditions

    private static readonly ConditionDefinition ConditionPermanentBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}PermanentBlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}PermanentBlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new ExtraActionBlastPursuit())
                .AddToDB())
        .AddToDB();

    internal PatronEldritchSurge()
    {
        // LEVEL 03

        // Blast Exclusive

        SpellDefinitions.EldritchBlast.SetCustomSubFeatures(new ModifyEldritchBlast());

        var featureEldritchSurgeBlastExclusive = FeatureDefinitionBuilder
            .Create($"Feature{Name}BlastExclusive")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 06

        // Blast Pursuit

        var powerBlastPursuit = FeatureDefinitionPowerBuilder
            .Create(PowerBlastPursuitName)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastPursuit))
                .Build())
            .AddToDB();

        powerBlastPursuit.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastPursuit, ConditionBlastPursuit));

        // LEVEL 10

        // Blast Overload

        var powerBlastOverload = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlastOverload")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
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

        var powerBlastReload = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlastReload")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.LightningBolt)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastReload))
                .Build())
            .SetCustomSubFeatures(new CustomBehaviorBlastReload())
            .AddToDB();

        // LEVEL 18

        // Permanent Blast Pursuit

        var powerPermanentBlastPursuit = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PermanentBlastPursuit")
            .SetUsesFixed(ActivationTime.Permanent)
            .SetGuiPresentationNoContent(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionPermanentBlastPursuit))
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1, featureEldritchSurgeBlastExclusive)
            .AddFeaturesAtLevel(6, powerBlastPursuit)
            .AddFeaturesAtLevel(10, powerBlastOverload)
            .AddFeaturesAtLevel(14, powerBlastReload)
            .AddFeaturesAtLevel(18, powerPermanentBlastPursuit)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static int GetBlastPursuitExtraActionCount(RulesetActor character)
    {
        return character.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == ConditionExtraActionBlastPursuit);
    }

    private sealed class ModifyEldritchBlast : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var rulesetHero = character.GetOriginalHero();

            if (rulesetHero == null ||
                rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) == 0)
            {
                return effectDescription;
            }

            var totalLevel = rulesetHero.classesHistory.Count;
            var warlockClassLevel = rulesetHero.GetClassLevel(CharacterClassDefinitions.Warlock);
            var determinantLevel = warlockClassLevel - (2 * (totalLevel - warlockClassLevel));
            var increaseLevels = new[] { 3, 7, 11, 15, 19 };
            var additionalBeamCount = increaseLevels.Count(level => determinantLevel >= level);
            var blastPursuitExtraActionCount = GetBlastPursuitExtraActionCount(character);
            var overloadStatus = character.HasConditionOfType(ConditionBlastOverload) ? 1 : 0;

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;
            effectDescription.rangeParameter = 24 - (6 * (blastPursuitExtraActionCount + overloadStatus));

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
                characterActionUsePower.activePower.PowerDefinition != _triggerPower
               )
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

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var hero = character.GetOriginalHero();

            if (hero == null)
            {
                return false;
            }

            var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);

            return !hero.HasConditionOfType(_conditionDefinition.Name) && pactMagicMaxSlots - pactMagicUsedSlots >= 1;
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

            if (activeEffect is not RulesetEffectSpell spell ||
                spell.spellDefinition != SpellDefinitions.EldritchBlast ||
                !rulesetAttacker.HasConditionOfType(ConditionBlastPursuit))
            {
                yield break;
            }

            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionBlastPursuit.Name, out var activeCondition))
            {
                activeCondition.endOccurence = TurnOccurenceType.EndOfTurn;
                activeCondition.remainingRounds += 1;
            }

            var power = attacker.RulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.Name == PowerBlastPursuitName);

            if (power != null)
            {
                power.remainingRounds += 1;
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
            var actionDefinition = actionParams.ActionDefinition;

            // skip if wrong scope or action type
            if (scope != ActionScope.Battle || actionDefinition.ActionType != ActionType.Main)
            {
                return;
            }

            // skip if still has attacks available
            if (gameLocationCharacter.GetActionAvailableIterations(Id.AttackMain) > 0)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;
            var hero = rulesetCharacter.GetOriginalHero();

            if (hero == null)
            {
                return;
            }

            var blastPursuitExtraActionCount = GetBlastPursuitExtraActionCount(rulesetCharacter);

            var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);
            var pactMagicRemainingSlots = pactMagicMaxSlots - pactMagicUsedSlots;

            var isValidBlastPursuit =
                rulesetCharacter.HasConditionOfType(ConditionBlastPursuit) &&
                blastPursuitExtraActionCount == 0;

            var isValidPermanentBlastPursuit =
                rulesetCharacter.HasConditionOfType(ConditionPermanentBlastPursuit) &&
                blastPursuitExtraActionCount <= 1 &&
                pactMagicRemainingSlots <= 0;

            var isEldritchBlast =
                actionParams.activeEffect is RulesetEffectSpell spell &&
                spell.spellDefinition == SpellDefinitions.EldritchBlast;

            if (!isValidBlastPursuit && (!isValidPermanentBlastPursuit || !isEldritchBlast))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                ConditionExtraActionBlastPursuit.Name,
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

            var text = ConditionExtraActionBlastPursuit.GuiPresentation.Title;

            rulesetCharacter.ShowLabel(text, Gui.ColorPositive);
            rulesetCharacter.LogCharacterActivatesAbility(text, "Feedback/&BlastPursuitExtraAction", true);
        }
    }

    private sealed class CustomBehaviorBlastReload :
        IActionExecutionHandled, ICharacterTurnStartListener, IQualifySpellToRepertoireLine
    {
        private readonly List<SpellDefinition> usedCantrips = new();

        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                actionParams.activeEffect is not RulesetEffectSpell spell ||
                spell.spellDefinition.spellLevel != 0 ||
                usedCantrips.Contains(spell.SpellDefinition))
            {
                return;
            }

            usedCantrips.Add(spell.SpellDefinition);
            rulesetCharacter.InflictCondition(
                ConditionBlastReload.Name,
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
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            usedCantrips.Clear();
        }

        public void QualifySpells(RulesetCharacter character, SpellRepertoireLine line, List<SpellDefinition> spells)
        {
            if (!character.HasConditionOfType(ConditionBlastReload) || line.actionType != ActionType.Bonus)
            {
                return;
            }

            line.relevantSpells.AddRange(usedCantrips.Intersect(spells));
        }
    }
}
