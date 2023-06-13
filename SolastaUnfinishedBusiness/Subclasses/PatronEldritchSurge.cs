using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal class PatronEldritchSurge : AbstractSubclass
{
    private static readonly ConditionDefinition ConditionBlastPursuit = BuildConditionBlastPursuit();
    private static readonly ConditionDefinition ConditionConstantBlastPursuit = BuildConditionConstantBlastPursuit();

    private static readonly ConditionDefinition ConditionBlastOverload = BuildConditionBlastOverload();

    private static readonly ConditionDefinition ConditionBlastPursuitExtraAction = BuildConditionBlastPursuitExtraAction();

    private static readonly ConditionDefinition ConditionBlastReload = BuildConditionBlastReload();

    private const string PowerBlastPursuitName = "PowerBlastPursuit";

    internal PatronEldritchSurge()
    {
        SpellDefinitions.EldritchBlast.SetCustomSubFeatures(new ModifyEldritchBlast());

        var featureEldritchSurgeBlastExlucsive = FeatureDefinitionBuilder
            .Create("FeatureEldritchSurgeBlastExlucsive")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
        
        var featureBlastPursuit = FeatureDefinitionPowerBuilder
            .Create(PowerBlastPursuitName)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.AtWill)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionBlastPursuit)
                )
                .Build())
            .AddToDB();

        featureBlastPursuit.SetCustomSubFeatures(
            new ValidateBlastPursuit(),
            ValidatorsPowerUse.HasNoneOfConditions(ConditionBlastPursuit.Name),
            new SpendPactSlotOnActivation(featureBlastPursuit)
            );

        var featurePursuitPermanency = FeatureDefinitionPowerBuilder
            .Create("PowerPursuitPermanency")
            .SetUsesFixed(ActivationTime.Permanent)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionConstantBlastPursuit)
                )
                .Build())
            .AddToDB();

        var featureBlastOverload = FeatureDefinitionPowerBuilder
            .Create("PowerBlastOverload")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.AtWill)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionBlastOverload)
                )
                .Build())
            .AddToDB();

        featureBlastOverload.SetCustomSubFeatures(
            new ValidateBlastPursuit(),
            ValidatorsPowerUse.HasNoneOfConditions(ConditionBlastOverload.Name),
            new SpendPactSlotOnActivation(featureBlastOverload)
            );

        var featureBlastReload = FeatureDefinitionPowerBuilder
            .Create("PowerBlastReload")
            .SetUsesFixed(ActivationTime.Permanent)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.LightningBolt)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionBlastReload)
                )
                .Build())
            .SetCustomSubFeatures(new HandleCastCantrip())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronEldritchSurge")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("PatronRiftWalker", Resources.PatronRiftWalker, 256))
            .AddFeaturesAtLevel(1,
                featureEldritchSurgeBlastExlucsive)
            .AddFeaturesAtLevel(6,
                featureBlastPursuit)
            .AddFeaturesAtLevel(10,
                featureBlastOverload)
            .AddFeaturesAtLevel(14,
                featureBlastReload)
            .AddFeaturesAtLevel(17,
                featurePursuitPermanency
            )
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class PreventRemoveConcentrationRiftWalker : IPreventRemoveConcentrationWithPowerUse
    {
    }

    private sealed class ModifyEldritchBlast : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effectDescription, RulesetCharacter character, RulesetEffect rulesetEffect)
        {
            var characterHero = character as RulesetCharacterHero;
            var WarlockClass = CharacterClassDefinitions.Warlock;
            if (characterHero is null) return effectDescription;
            if (characterHero.GetSubclassLevel(WarlockClass, "PatronEldritchSurge") == 0) return effectDescription;
            var warlockClassLevel = characterHero.GetClassLevel(WarlockClass);
            var totalLevel = characterHero.classesHistory.Count();
            var determinant = warlockClassLevel - 2 * (totalLevel - warlockClassLevel);
            int[] increaseLevels = { 4, 8, 13, 19 };
            var additionalBeamNumber = increaseLevels.Where(level => determinant >= level).Count();
            effectDescription.targetParameter = 1 + additionalBeamNumber;
            effectDescription.effectAdvancement.Clear();
            var stackNum = GetPursuitStacks(character);
            int overloadStatus = character.HasConditionOfType(ConditionBlastOverload)? 1 : 0;
            effectDescription.rangeParameter -= 6 * (stackNum + overloadStatus);
            return effectDescription;

        }
    }
    private sealed class ValidateBlastPursuit : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var hero = character as RulesetCharacterHero;
            var warlockRepertoire = hero.spellRepertoires.Where(s => s.spellCastingClass == CharacterClassDefinitions.Warlock).First();
            var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);
            return pactMagicMaxSlots - pactMagicUsedSlots >= 1;
        }
    }

    private static ConditionDefinition BuildConditionBlastPursuit()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionBlastPursuit")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
            .AddFeatures(
                FeatureDefinitionBuilder
                .Create("FeatureConditionBlastPursuitCustom")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(
                        new ExtraActionBlastPursuit(),
                        new ExtendedBlastPursuitOnKill())
                    .AddToDB())
            .AddToDB();
    }
    private static ConditionDefinition BuildConditionConstantBlastPursuit()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionConstantBlastPursuit")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
            .AddFeatures(
                FeatureDefinitionBuilder
                .Create("FeatureConditionConstantBlastPursuitCustom")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(
                        new ExtraActionBlastPursuit())
                    .AddToDB())
            .AddToDB();
    }
    private static ConditionDefinition BuildConditionBlastOverload()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionBlastOverload")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
            .AddFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageInvocationAgonizingBlast,
                        "AdditionalDamageBlastOverload")
                    .SetNotificationTag("BlastOverload")
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                    .AddToDB(),
                FeatureDefinitionMagicAffinityBuilder
                    .Create("MagicAffinityBlastOverload")
                    .SetGuiPresentation(Category.Feature)
                    .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
                    .AddToDB()
            )
            .AddToDB();
    }
    private static ConditionDefinition BuildConditionBlastPursuitExtraAction()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionBlastPursuitExtraAction")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .AllowMultipleInstances()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create("AdditionalActionBlastPursuit")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(AllowDuplicates.Mark)
                    .SetActionType(ActionType.Main)
                    .SetRestrictedActions(
                        Id.AttackMain,
                        Id.CastMain,
                        Id.Dodge,
                        Id.DisengageMain,
                        Id.DashMain)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            //.AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();
    }
    private static ConditionDefinition BuildConditionBlastReload()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionBlastReload")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();
    }
    private sealed class ExtendedBlastPursuitOnKill: ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            // activeEffect != null means a magical attack
            if (activeEffect is not RulesetEffectSpell spell ||
                spell.spellDefinition != SpellDefinitions.EldritchBlast ||
                !attacker.RulesetCharacter.HasConditionOfType(ConditionBlastPursuit))
            {
                yield break;
            }


            attacker.RulesetCharacter.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                    ConditionBlastPursuit.Name, out var activeCondition);

            activeCondition.remainingRounds += 1;
            activeCondition.endOccurence = TurnOccurenceType.EndOfTurn;

            foreach (var power in attacker.RulesetCharacter.powersUsedByMe.Where(power =>
                         power.Name == PowerBlastPursuitName))
            {
                power.remainingRounds += 1;
                break;
            }
        }
    }
    private sealed class SpendPactSlotOnActivation : IActionFinished
    {
        private readonly FeatureDefinitionPower _triggerPower;

        public SpendPactSlotOnActivation(FeatureDefinitionPower triggerPower)
        {
            _triggerPower = triggerPower;
        }

        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _triggerPower
            )
            {
                yield break;
            }
            var hero = characterAction.ActingCharacter.RulesetCharacter as RulesetCharacterHero;
            var slotLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
            SharedSpellsContext.GetWarlockSpellRepertoire(hero).SpendSpellSlot(slotLevel);
        }
    }

    private sealed class ExtraActionBlastPursuit: IActionExecutionHandled
    {
        public void OnActionExecutionHandled(GameLocationCharacter hero, CharacterActionParams actionParams, ActionDefinitions.ActionScope scope)
        {
            var actionDefinition = actionParams.ActionDefinition;
            var rulesetHero = hero.RulesetCharacter;
            var stacks = GetPursuitStacks(rulesetHero);
            //Wrong scope or type of action, skip
            if (scope != ActionScope.Battle
                || actionDefinition.ActionType != ActionType.Main)
            {
                return;
            }

            //Still has attacks, skip
            if (hero.GetActionAvailableIterations(Id.AttackMain) > 0)
            {
                return;
            }
            var rch = rulesetHero as RulesetCharacterHero;
            var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(rch);
            var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(rch);
            if (rulesetHero.HasConditionOfType(ConditionConstantBlastPursuit) && stacks <= 1
                && pactMagicMaxSlots - pactMagicUsedSlots <= 0)
            {
                if (actionParams.activeEffect is not RulesetEffectSpell spell ||
                    spell.spellDefinition != SpellDefinitions.EldritchBlast) return;
            }
            else if (rulesetHero.HasConditionOfType(ConditionBlastPursuit) && stacks == 0) { }
            else return;

            rulesetHero.InflictCondition(
                ConditionBlastPursuitExtraAction.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetHero.guid,
                rulesetHero.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0
                );
        }


    }
    private sealed class HandleCastCantrip : IActionExecutionHandled, ICharacterTurnEndListener, IQualifySpellToRepertoireLine
    {
        private List<SpellDefinition> usedCantrips = new List<SpellDefinition>();

        public void OnActionExecutionHandled(GameLocationCharacter hero, CharacterActionParams actionParams, ActionScope scope)
        {
            var actionDefinition = actionParams.ActionDefinition;
            var rulesetHero = hero.RulesetCharacter;

            if (rulesetHero is null || scope != ActionScope.Battle || actionDefinition.ActionType != ActionType.Main
                || actionParams.activeEffect is not RulesetEffectSpell spell
                || spell.spellDefinition.spellLevel != 0 || usedCantrips.Contains(spell.SpellDefinition)
                )
            {
                return;
            }
            usedCantrips.Add(spell.SpellDefinition);
            rulesetHero.InflictCondition(
                ConditionBlastReload.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetHero.guid,
                rulesetHero.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0
                );

        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
                usedCantrips.Clear();
        }

        public void QualifySpells(RulesetCharacter character, SpellRepertoireLine line, List<SpellDefinition> spells)
        {
            if (!character.HasConditionOfType(ConditionBlastReload) || line.actionType != ActionType.Bonus) return;
            line.relevantSpells.AddRange(usedCantrips.Intersect(spells));
        }
    }
    private static int GetPursuitStacks(RulesetCharacter character)
    {
        return character?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == ConditionBlastPursuitExtraAction) ?? 0;
    }
}
