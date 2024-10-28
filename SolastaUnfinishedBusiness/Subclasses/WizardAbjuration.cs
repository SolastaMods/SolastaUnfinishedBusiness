using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardAbjuration : AbstractSubclass
{
    private const string Name = "WizardAbjuration";
    private const string ArcaneWardConditionName = $"Condition{Name}ArcaneWard";
    private const string ProjectedWardConditionName = $"Condition{Name}ProjectedWard";

    private static FeatureDefinitionPower _powerArcaneWard;

    private static ConditionDefinition _conditionArcaneWard;

    public WizardAbjuration()
    {
        // Lv.2 Abjuration Savant
        var magicAffinityAbjurationScriber = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Scriber")
            .SetGuiPresentation($"MagicAffinity{Name}Scriber", Category.Feature)
            .SetSpellLearnAndPrepModifiers(
                0.5f, 0.5f, 0, AdvantageType.None, PreparedSpellsModifier.None)
            .AddCustomSubFeatures(new ModifyScribeCostAndDurationAbjurationSavant())
            .AddToDB();

        // LV.2 Arcane Ward
        // initialize power points pool with INT mod points

        _ = ConditionArcaneWard;

        // + 2 * Wiz Lv. points in the pool max
        PowerArcaneWard.AddCustomSubFeatures(
            new ArcaneWardPortraitPoints(),
            //handle damage reduction
            ArcaneWardModifyDamage(),
            // handle applying the condition or refunding points to the pool when casting Abjuration spells
            new CustomBehaviorArcaneWard(),
            ModifyPowerVisibility.Hidden,
            HasModifiedUses.Marker,
            new ModifyPowerRechargeHandler(LimitArcaneWardRecharge),
            new ModifyPowerPoolAmount
            {
                PowerPool = PowerArcaneWard,
                Type = PowerPoolBonusCalculationType.ClassLevel,
                Attribute = WizardClass,
                Value = 2
            });

        ////////
        // Lv.6 Projected Ward
        var conditionProjectedWard = ConditionDefinitionBuilder
            .Create(ProjectedWardConditionName)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .SetSpecialInterruptions(
                (ConditionInterruption)ExtraConditionInterruption.AfterWasAttacked,
                ConditionInterruption.Damaged,
                ConditionInterruption.AnyBattleTurnEnd)
            .SetPossessive()
            .AddCustomSubFeatures(ProjectedWardModifyDamage())
            .AddToDB();

        // Can only use when Arcane is both Active and has non-zero points remaining
        var powerProjectedWard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ProjectedWard")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetShowCasting(false)
            .AddToDB();

        powerProjectedWard.AddCustomSubFeatures(new CustomBehaviorProjectedWard(
            powerProjectedWard,
            conditionProjectedWard));

        ////////
        // Lv.10 Improved Abjuration
        var powerCounterSpell = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CounterSpell")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .SetParticleEffectParameters(Counterspell)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetCounterForm(CounterForm.CounterType.InterruptSpellcasting, 3, 10, true, true)
                    .Build())
                .Build())
            .AddToDB();

        var powerCounterDispel = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CounterDispel")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous, 1)
                .SetTargetingData(
                    Side.All,
                    RangeType.Distance, 24,
                    TargetType.IndividualsUnique, 1, 2,
                    ActionDefinitions.ItemSelectionType.Equiped)
                .SetParticleEffectParameters(DispelMagic)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetCounterForm(CounterForm.CounterType.DissipateSpells, 3, 10, true, true)
                    .Build())
                .Build())
            .AddToDB();

        var featureSetImprovedAbjuration = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ImprovedAbjuration")
            .SetGuiPresentation($"Power{Name}ImprovedAbjuration", Category.Feature)
            .SetFeatureSet(powerCounterSpell, powerCounterDispel)
            .AddToDB();

        powerCounterDispel.EffectDescription.targetFilteringMethod =
            TargetFilteringMethod.CharacterGadgetEffectProxyItems;
        powerCounterDispel.EffectDescription.targetFilteringTag = TargetFilteringTag.No;

        //////// 
        // Lv.14 Spell Resistance
        // Adv. on saves against magic

        // Resist damage from spells
        var conditionSpellResistance = ConditionDefinitionBuilder
            .Create($"Condition{Name}SpellResistance")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistanceTrue,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistanceTrue,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistanceTrue,
                DamageAffinityThunderResistance)
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerSpellResistance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SpellResistance")
            .SetGuiPresentation($"Power{Name}SpellResistance", Category.Feature,
                Sprites.GetSprite("CircleOfMagicalNegation", Resources.CircleOfMagicalNegation, 128))
            .SetUsesFixed(ActivationTime.Permanent)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnMeSpellResistance(conditionSpellResistance))
            .AddToDB();

        var featureSetSpellResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SpellResistance")
            .SetGuiPresentation($"Power{Name}SpellResistance", Category.Feature)
            .SetFeatureSet(SavingThrowAffinitySpellResistance, powerSpellResistance)
            .AddToDB();

        // Assemble the subclass
        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardAbjuration, 256))
            .AddFeaturesAtLevel(2, magicAffinityAbjurationScriber, PowerArcaneWard)
            .AddFeaturesAtLevel(6, powerProjectedWard)
            .AddFeaturesAtLevel(10, featureSetImprovedAbjuration)
            .AddFeaturesAtLevel(14, featureSetSpellResistance)
            .AddToDB();

        UpdateBg3ModeStatus();
    }

    private static FeatureDefinitionPower PowerArcaneWard => _powerArcaneWard ??= BuildArcaneWard();
    private static ConditionDefinition ConditionArcaneWard => _conditionArcaneWard ??= BuildArcaneWardCondition();

    private static bool IsBg3Mode => Main.Settings.EnableBg3AbjurationArcaneWard;

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower BuildArcaneWard()
    {
        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneWard")
            .SetGuiPresentation($"Power{Name}ArcaneWard", Category.Feature, GlobeOfInvulnerability)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetShowCasting(false)
            .AddToDB();
    }

    private static ConditionDefinition BuildArcaneWardCondition()
    {
        // marks Arcane Ward as active
        return ConditionDefinitionBuilder
            .Create(ArcaneWardConditionName)
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization)
            .SetConditionType(ConditionType.Beneficial)
            .SetSilent(Silent.WhenRefreshedOrRemoved)
            .SetPossessive()
            .AddToDB();
    }

    private static int LimitArcaneWardRecharge(RulesetCharacter character, RulesetUsablePower _, int maxUses)
    {
        return IsBg3Mode
            ? maxUses / 2
            : maxUses;
    }

    private static bool HasActiveArcaneWard(RulesetCharacter character)
    {
        return (IsBg3Mode || character.HasConditionOfType(ArcaneWardConditionName))
               && character.GetRemainingPowerCharges(PowerArcaneWard) > 0;
    }

    private static ModifySustainedDamageHandler ArcaneWardModifyDamage()
    {
        return (RulesetCharacter character, ref int damage, string _, bool _, ulong _, RollInfo roll) =>
        {
            ArcaneWardModifyDamage(character, ref damage, roll);
        };
    }

    private static void ArcaneWardModifyDamage(
        RulesetCharacter character, ref int damage, RollInfo roll, RulesetCharacter affected = null)
    {
        if (!HasActiveArcaneWard(character)) { return; }

        var ward = character.GetRemainingPowerCharges(PowerArcaneWard);

        var prevented = ward <= damage ? ward : damage;
        var spent = IsBg3Mode ? 1 : prevented;

        (affected ?? character).LogCharacterUsedFeature(PowerArcaneWard, "Feedback/&ArcaneWard", true,
            (ConsoleStyleDuplet.ParameterType.Positive, prevented.ToString()));

        character.UpdateUsageForPower(PowerArcaneWard, spent);
        roll.modifier -= prevented;
        damage -= prevented;
    }

    private static ModifySustainedDamageHandler ProjectedWardModifyDamage()
    {
        return (RulesetCharacter character, ref int damage, string _, bool _, ulong _, RollInfo roll) =>
        {
            if (!character.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ProjectedWardConditionName,
                    out var condition)) { return; }

            var protector = EffectHelpers.GetCharacterByGuid(condition.sourceGuid);
            if (!HasActiveArcaneWard(protector)) { return; }

            ArcaneWardModifyDamage(protector, ref damage, roll, character);
        };
    }

    public static void UpdateBg3ModeStatus()
    {
        if (IsBg3Mode)
        {
            PowerArcaneWard.usesDetermination = UsesDetermination.Fixed;
            PowerArcaneWard.GuiPresentation.Description = $"Feature/&{PowerArcaneWard.Name}BG3Description";
            ConditionArcaneWard.silentWhenAdded = true;
        }
        else
        {
            PowerArcaneWard.usesDetermination = UsesDetermination.AbilityBonusPlusFixed;
            PowerArcaneWard.GuiPresentation.Description = $"Feature/&{PowerArcaneWard.Name}Description";
            ConditionArcaneWard.silentWhenAdded = false;
        }

        Global.RefreshControlledCharacter();
    }

    // Handle Behaviour related to Abjuration Savant
    private sealed class ModifyScribeCostAndDurationAbjurationSavant : IModifyScribeCostAndDuration
    {
        public void ModifyScribeCostMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolAbjuration)
            {
                costMultiplier = 1;
            }
        }

        public void ModifyScribeDurationMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolAbjuration)
            {
                durationMultiplier = 1;
            }
        }
    }

    // Handle Behaviour related to Arcane Ward
    private sealed class CustomBehaviorArcaneWard : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action is not CharacterActionCastSpell actionCastSpell ||
                actionCastSpell.Countered ||
                actionCastSpell.ExecutionFailed)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var spellCast = actionCastSpell.ActiveSpell;

            if (GetDefinition<SchoolOfMagicDefinition>(spellCast.SchoolOfMagic) !=
                SchoolOfMagicDefinitions.SchoolAbjuration)
            {
                yield break;
            }

            var hasActiveWardCondition = rulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ArcaneWardConditionName);

            if (hasActiveWardCondition || IsBg3Mode)
            {
                // if ward already exists, update pool
                var usablePowerArcaneWard = PowerProvider.Get(PowerArcaneWard, rulesetCharacter);

                rulesetCharacter.UpdateUsageForPowerPool(-2 * spellCast.EffectLevel, usablePowerArcaneWard);
            }

            if (!hasActiveWardCondition)
            {
                // if no ward condition, add condition (which should last until long rest)
                rulesetCharacter.InflictCondition(
                    ArcaneWardConditionName,
                    DurationType.UntilLongRest,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    14,
                    ArcaneWardConditionName,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class CustomBehaviorProjectedWard(
        FeatureDefinitionPower projectedWard,
        ConditionDefinition conditionProjectedWard) : ITryAlterOutcomeAttack, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => 25;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (action.AttackRollOutcome == RollOutcome.Failure)
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return HandleReactionProjectedWard(battleManager, attacker, defender, helper);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            var effectDescription = savingThrowData.EffectDescription;
            var canForceHalfDamage = attacker != null
                                     && savingThrowData.SourceDefinition is SpellDefinition spell
                                     && attacker.RulesetCharacter.CanForceHalfDamage(spell);
            var hasSpecialHalfDamage =
                defender.RulesetCharacter.HasSpecialHalfDamage(effectDescription.SavingThrowAbility);
            if (!effectDescription.HasNotNegatedDamageForm(savingThrowData, canForceHalfDamage, hasSpecialHalfDamage))
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return HandleReactionProjectedWard(battleManager, helper, defender, helper);
        }

        private IEnumerator HandleReactionProjectedWard(
            GameLocationBattleManager battleManager,
            [CanBeNull] GameLocationCharacter waiter,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;

            if (defender == helper ||
                !helper.CanReact() ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender) ||
                !HasActiveArcaneWard(rulesetHelper))
            {
                yield break;
            }

            var usableProjectedWard = PowerProvider.Get(projectedWard, rulesetHelper);

            yield return helper.MyReactToSpendPower(
                usableProjectedWard,
                waiter,
                "ProjectedWard",
                "SpendPowerProjectedWardDescription".Formatted(
                    Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager
            );

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var activeCondition = RulesetCondition.CreateCondition(defender.Guid, conditionProjectedWard);

                activeCondition.sourceGuid = helper.RulesetCharacter.Guid;

                defender.RulesetCharacter.AddConditionOfCategory(
                    AttributeDefinitions.TagEffect,
                    activeCondition, false);

                helper.RulesetCharacter.LogCharacterUsedPower(
                    projectedWard,
                    "Feedback/&ProjectedWard",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Player, defender.Name)
                    ]);
            }
        }
    }

    private sealed class MagicEffectBeforeHitConfirmedOnMeSpellResistance(ConditionDefinition conditionSpellResistance)
        : IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                conditionSpellResistance.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                14,
                conditionSpellResistance.Name,
                0,
                0,
                0);

            yield break;
        }
    }

    private sealed class ArcaneWardPortraitPoints : PowerPortraitPointPool
    {
        public override string TooltipFormat => IsBg3Mode
            ? $"PortraitPool{PowerArcaneWard.Name}BG3PointsFormat"
            : $"PortraitPool{PowerArcaneWard.Name}PointsFormat";

        public ArcaneWardPortraitPoints() : base(PowerArcaneWard, Sprites.ArcaneWardPoints)
        {
            IsActiveHandler = character => IsBg3Mode || character.HasConditionOfType(ArcaneWardConditionName);
        }
    }
}
