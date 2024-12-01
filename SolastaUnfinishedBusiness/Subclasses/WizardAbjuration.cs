using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardAbjuration : AbstractSubclass
{
    private const string Name = "WizardAbjuration";
    internal const string SpellTag = "Abjurer";
    private const string ArcaneWardConditionName = $"Condition{Name}ArcaneWard";
    private const string ProjectedWardConditionName = $"Condition{Name}ProjectedWard";
    private const int WardRechargeMultiplier = 2;

    private static FeatureDefinitionPower _powerArcaneWard;

    private static ConditionDefinition _conditionArcaneWard;

    private static readonly SpellListDefinition SpellListAbjurer = SpellListDefinitionBuilder
        .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
        .AddToDB();

    private static readonly FeatureDefinitionMagicAffinity MagicAffinitySavant = FeatureDefinitionMagicAffinityBuilder
        .Create($"MagicAffinity{Name}Scriber")
        .SetGuiPresentation($"MagicAffinity{Name}Scriber", Category.Feature)
        .SetSpellLearnAndPrepModifiers(
            0.5f, 0.5f, 0, AdvantageType.None, PreparedSpellsModifier.None)
        .AddCustomSubFeatures(new ModifyScribeCostAndDurationAbjurationSavant())
        .AddToDB();

    // no spell tag here as this work correctly with vanilla
    private static readonly FeatureDefinitionPointPool MagicAffinitySavant2024 = FeatureDefinitionPointPoolBuilder
        .Create($"MagicAffinity{Name}Savant2024")
        .SetGuiPresentation(Category.Feature)
        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 2, SpellListAbjurer)
        .AddToDB();

    // need spell tag here to get this offered on level up and
    // let custom behavior at CharacterBuildingManager.FinalizeCharacter grant the spell
    private static readonly FeatureDefinitionPointPool MagicAffinitySavant2024Progression =
        FeatureDefinitionPointPoolBuilder
            .Create($"MagicAffinity{Name}Savant2024Progression")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 1, SpellListAbjurer, SpellTag)
            .AddToDB();

    private static CharacterSubclassDefinition _subclass;

    public WizardAbjuration()
    {
        // Lv.2 Abjuration Savant

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
            ForceUsesAttributeDeserialization.Mark,
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
        var powerCounterSpellAffinity = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CounterSpell")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new ModifyCounterspellAddProficiency())
            .AddToDB();

        var powerCounterDispelAffinity = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CounterDispel")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new ModifyDispelMagicAddProficiency())
            .AddToDB();

        var featureSetImprovedAbjuration = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ImprovedAbjuration")
            .SetGuiPresentation($"Power{Name}ImprovedAbjuration", Category.Feature)
            .SetFeatureSet(powerCounterSpellAffinity, powerCounterDispelAffinity)
            .AddToDB();


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
            .AddFeaturesAtLevel(2, MagicAffinitySavant, PowerArcaneWard, BuildRechargeArcaneWard())
            .AddFeaturesAtLevel(6, powerProjectedWard)
            .AddFeaturesAtLevel(10, featureSetImprovedAbjuration)
            .AddFeaturesAtLevel(14, featureSetSpellResistance)
            .AddToDB();

        _subclass = Subclass;
    }

    private static FeatureDefinitionPower PowerArcaneWard => _powerArcaneWard ??= BuildArcaneWard();
    private static ConditionDefinition ConditionArcaneWard => _conditionArcaneWard ??= BuildArcaneWardCondition();

    private static bool IsBg3Mode => Main.Settings.EnableBg3AbjurationArcaneWard;

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        SwapSavantAndSavant2024();
        SwapAbjurationBaldurGate3Mode();

        SpellListAbjurer.SpellsByLevel.SetRange(
            SpellListDefinitions.SpellListWizard.SpellsByLevel
                .Select(spellByLevel => new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellByLevel.Level,
                    Spells = [..spellByLevel.Spells.Where(x => x.SchoolOfMagic == SchoolAbjuration)]
                }));
    }

    internal static void SwapSavantAndSavant2024()
    {
        var level = Main.Settings.EnableWizardToLearnSchoolAtLevel3 ? 3 : 2;

        _subclass.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == MagicAffinitySavant ||
            x.FeatureDefinition == MagicAffinitySavant2024 ||
            x.FeatureDefinition == MagicAffinitySavant2024Progression);

        if (Main.Settings.SwapAbjurationSavant)
        {
            _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant2024, level));

            for (var i = 5; i <= 20; i += 2)
            {
                _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant2024Progression, i));
            }
        }
        else
        {
            _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant, level));
        }

        _subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

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
            .SetGuiPresentation(
                Category.Condition,
                Gui.NoLocalization,
                ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Beneficial)
            .SetSilent(Silent.WhenRefreshedOrRemoved)
            .SetPossessive()
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildRechargeArcaneWard()
    {
        var powerRechargeArcaneWard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RechargeArcaneWard")
            .SetGuiPresentation(Category.Feature,
                $"Feature/&Power{Name}RechargeArcaneWardDescription", PowerTraditionCourtMageSpellShield)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .AddToDB();

        powerRechargeArcaneWard.AddCustomSubFeatures(new CustomBehaviorRechargeArcaneWard(powerRechargeArcaneWard));

        return powerRechargeArcaneWard;
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
        return (RulesetCharacter character, ref int damage, string _, bool _, ulong _, RollInfo _) =>
        {
            ArcaneWardModifyDamage(character, ref damage);
        };
    }

    private static void ArcaneWardModifyDamage(
        RulesetCharacter character, ref int damage, RulesetCharacter affected = null)
    {
        if (!HasActiveArcaneWard(character)) { return; }

        var ward = character.GetRemainingPowerCharges(PowerArcaneWard);
        var prevented = ward <= damage ? ward : damage;
        var spent = IsBg3Mode ? 1 : prevented;

        (affected ?? character).LogCharacterUsedFeature(PowerArcaneWard, "Feedback/&ArcaneWard", true,
            (ConsoleStyleDuplet.ParameterType.Positive, prevented.ToString()));

        character.UpdateUsageForPower(PowerArcaneWard, spent);

        damage -= prevented;
    }

    private static ModifySustainedDamageHandler ProjectedWardModifyDamage()
    {
        return (RulesetCharacter character, ref int damage, string _, bool _, ulong _, RollInfo _) =>
        {
            if (!character.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ProjectedWardConditionName,
                    out var condition)) { return; }

            var protector = EffectHelpers.GetCharacterByGuid(condition.sourceGuid);
            if (!HasActiveArcaneWard(protector)) { return; }

            ArcaneWardModifyDamage(protector, ref damage, character);
        };
    }

    internal static void SwapAbjurationBaldurGate3Mode()
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
                var amount = WardRechargeMultiplier * spellCast.EffectLevel;

                rulesetCharacter.UpdateUsageForPowerPool(-amount, usablePowerArcaneWard);

                rulesetCharacter.LogCharacterUsedFeature(PowerArcaneWard, "Feedback/&ArcaneWardRecharge", true,
                    (ConsoleStyleDuplet.ParameterType.Positive, amount.ToString()));
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

    private sealed class CustomBehaviorRechargeArcaneWard(FeatureDefinition feature)
        : IValidatePowerUse, IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;
            var glc = action.ActingCharacter;

            var actionParams = new CharacterActionParams(glc, ActionDefinitions.Id.SpendSpellSlot)
            {
                IntParameter = 1,
                StringParameter = feature.Name,
                SpellRepertoire = glc.RulesetCharacter.GetClassSpellRepertoire(WizardClass)
            };

            actionService.ReactToSpendSpellSlot(actionParams);

            yield return battleManager.WaitForReactions(glc, actionService, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            EffectHelpers.StartVisualEffect(glc, glc, Shield, EffectHelpers.EffectType.QuickCaster);

            // recharge ward
            var amount = WardRechargeMultiplier * actionParams.IntParameter;

            glc.RulesetCharacter.UpdateUsageForPower(PowerArcaneWard, -amount);

            glc.RulesetCharacter.LogCharacterUsedFeature(
                PowerArcaneWard, "Feedback/&ArcaneWardRecharge", true,
                (ConsoleStyleDuplet.ParameterType.Positive, amount.ToString()));
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower _)
        {
            var usablePower = PowerProvider.Get(PowerArcaneWard, rulesetCharacter);
            var rulesetRepertoire = rulesetCharacter.GetClassSpellRepertoire(WizardClass);

            return rulesetRepertoire.AtLeastOneSpellSlotAvailable() &&
                   rulesetCharacter.GetRemainingUsesOfPower(usablePower) <
                   rulesetCharacter.GetMaxUsesOfPower(usablePower);
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
            if (action.AttackRollOutcome == RollOutcome.Failure || defender.RulesetCharacter == null)
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
            if (defender.RulesetCharacter == null) { yield break; }

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

                EffectHelpers.StartVisualEffect(helper, helper,
                    Counterspell, EffectHelpers.EffectType.Caster);
                EffectHelpers.StartVisualEffect(defender, defender,
                    PowerTraditionCourtMageSpellShield, EffectHelpers.EffectType.Caster);
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

    private sealed class ModifyCounterspellAddProficiency : IModifyEffectDescription
    {
        private static readonly EffectForm EffectForm = EffectFormBuilder.Create()
            .SetCounterForm(CounterForm.CounterType.InterruptSpellcasting, 3, 10, true, true)
            .Build();

        EffectDescription IModifyEffectDescription.GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms.SetRange(EffectForm);

            return effectDescription;
        }

        bool IModifyEffectDescription.IsValid(
            BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return character.GetSubclassLevel(CharacterClassDefinitions.Wizard, Name) >= 10 &&
                   definition == Counterspell;
        }
    }

    private sealed class ModifyDispelMagicAddProficiency : IModifyEffectDescription
    {
        private static readonly List<EffectForm> EffectForms =
        [
            EffectFormBuilder.Create()
                .SetCounterForm(CounterForm.CounterType.DissipateSpells, 3, 10, true, true)
                .SetCreatedBy(true)
                .SetBonusMode(AddBonusMode.None)
                .Build(),
            EffectFormBuilder.Create()
                .SetAlterationForm(AlterationForm.Type.DissipateSpell)
                .SetCreatedBy(true)
                .SetBonusMode(AddBonusMode.None)
                .Build()
        ];

        EffectDescription IModifyEffectDescription.GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms.SetRange(EffectForms);

            return effectDescription;
        }

        bool IModifyEffectDescription.IsValid(
            BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return character.GetSubclassLevel(CharacterClassDefinitions.Wizard, Name) >= 10 &&
                   definition == DispelMagic;
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
        public ArcaneWardPortraitPoints() : base(PowerArcaneWard, Sprites.ArcaneWardPoints)
        {
            IsActiveHandler = character => IsBg3Mode || character.HasConditionOfType(ArcaneWardConditionName);
        }

        public override string TooltipFormat => IsBg3Mode
            ? $"PortraitPool{PowerArcaneWard.Name}BG3PointsFormat"
            : $"PortraitPool{PowerArcaneWard.Name}PointsFormat";
    }
}
