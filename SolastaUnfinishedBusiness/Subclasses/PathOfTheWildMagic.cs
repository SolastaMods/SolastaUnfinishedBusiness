using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using Mono.CSharp;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA.AI.Activities;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static UnityEngine.EventSystems.EventTrigger;

namespace SolastaUnfinishedBusiness.Subclasses;
public sealed class PathOfTheWildMagic : AbstractSubclass
{
    private const string Name = "PathOfTheWildMagic";
    private const string ConditionWildSurgePrefix = $"Condition{Name}WildSurge";
    private const ActionDefinitions.Id WildSurgeUnstableBacklashToggle = (ActionDefinitions.Id)ExtraActionId.WildSurgeUnstableBacklashToggle;
    internal override CharacterSubclassDefinition Subclass { get; }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    public PathOfTheWildMagic()
    {

        var wildSurgeHandler = new WildSurgeHandler();
        // LEVEL 03

        var featureWildSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}WildSurge")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new WildSurgeAfterRage(wildSurgeHandler))
            .AddToDB();

        var effectMagicAwareness = SpellDefinitions.DetectMagic.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Divination)
            .DeepCopy();

        var powerWildMagicAwareness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MagicAwareness")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.DetectMagic)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(effectMagicAwareness)
                    .SetParticleEffectParameters(SpellDefinitions.DetectMagic)
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                    .Build())
            .AddToDB();
        
        // Bolstering Magic

        var featuresBolsteringMagic = BuildFeaturesBolsteringMagic();

        // Unstable Backslash

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "WildSurgeUnstableBacklashToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.WildSurgeUnstableBacklashToggle)
            .AddToDB();

        var actionAffinityUnstableBacklashToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                $"ActionAffinity{Name}UnstableBacklashToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(WildSurgeUnstableBacklashToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAnyOfConditions(ConditionDefinitions.ConditionRaging.Name)))
            .AddToDB();

        var featureUnstableBackslash = FeatureDefinitionBuilder
            .Create($"Feature{Name}UnstableBackslash")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new UnstableBackslashHandler(wildSurgeHandler))
            .AddToDB();

        // Controlled Surge
        var featureControlledSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}ControlledSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheElements, 256))
            .AddFeaturesAtLevel(3, featureWildSurge, powerWildMagicAwareness)
            .AddFeaturesAtLevel(6, featuresBolsteringMagic)
            .AddFeaturesAtLevel(10, featureUnstableBackslash, actionAffinityUnstableBacklashToggle)
            .AddFeaturesAtLevel(14, featureControlledSurge)
            .AddToDB();
    }

    private static FeatureDefinition[] BuildFeaturesBolsteringMagic()
    {
        var powerBolsteringMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BolsteringMagic")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var conditionBolsteringMagicRoll = ConditionDefinitionBuilder
            .Create($"Condition{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Condition)
            .AddToDB();

        conditionBolsteringMagicRoll.AddCustomSubFeatures(
            new CustomBehaviorBolsteringMagicRoll(conditionBolsteringMagicRoll));

        var powerBolsteringMagicRoll = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBolsteringMagicRoll))
                    .Build())
            .AddToDB();

        var conditionBolsteringMagicSpell = ConditionDefinitionBuilder.Create($"Condition{Name}BolsteringMagicSpell")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerBolsteringMagicSpell = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicSpell")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                .Build()
            )
            .AddToDB();

        powerBolsteringMagicSpell.AddCustomSubFeatures(
            new WildMagicBolsteringMagicSpellHandler(powerBolsteringMagicSpell, conditionBolsteringMagicSpell));

        PowerBundle.RegisterPowerBundle(powerBolsteringMagic, false,
            powerBolsteringMagicRoll, powerBolsteringMagicSpell);

        var featureSetBolsteringMagic = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BolsteringMagic")
            .SetGuiPresentation($"Power{Name}BolsteringMagic", Category.Feature)
            .SetFeatureSet(powerBolsteringMagic, powerBolsteringMagicRoll, powerBolsteringMagicSpell)
            .AddToDB();

        return [featureSetBolsteringMagic];
    }

    private sealed class CustomBehaviorBolsteringMagicRoll(
        ConditionDefinition conditionBolsteringMagicRoll) : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier)
        {
            if (helper != defender)
            {
                yield break;
            }

            var advantageType = actionModifier.AttackAdvantageTrend switch
            {
                > 0 => AdvantageType.Advantage,
                < 0 => AdvantageType.Disadvantage,
                _ => AdvantageType.None
            };

            var roll = RollDie(DieType.D3, advantageType, out _, out _);

            actionModifier.AttacktoHitTrends.Add(
                new TrendInfo(roll, FeatureSourceType.CharacterFeature, conditionBolsteringMagicRoll.Name,
                    conditionBolsteringMagicRoll));
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier abilityCheckModifier)
        {
            if (helper != defender ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure)
            {
                yield break;
            }
        }
    }

    private sealed class WildMagicBolsteringMagicSpellHandler(FeatureDefinitionPower power, ConditionDefinition condition) : IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var hero = GetOriginalHero(target);
            if (hero == null)
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}NotAHero");
                return false;
            }

            if (hero.HasConditionOfType(condition.name))
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}AlreadyBolstered");
                return false;
            }

            if (!hero.CanCastSpells())
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}CannotCastSpells");
                return false;
            }

            if (!hero.SpellRepertoires.Any(repertoire => repertoire.HasMissingSpellSlots()))
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}DoesNotHaveMissingSpellSlots");
                return false;
            }

            return true;
        }

        private RulesetCharacterHero GetOriginalHero(GameLocationCharacter targetCharacter)
        {
            return targetCharacter.RulesetCharacter switch
            {
                RulesetCharacterHero hero => hero,
                RulesetCharacterMonster monster when monster.originalFormCharacter is RulesetCharacterHero originalHero => originalHero,
                _ => null
            };
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var targetCharacter = action.actionParams.TargetCharacters?.FirstOrDefault();

            if (targetCharacter == null)
            {
                yield break;
            }

            var rulesetCharacter = GetOriginalHero(targetCharacter);
            if (!targetCharacter.RulesetCharacter.CanCastSpells())
            {
                yield break;
            }

            rulesetCharacter.InflictCondition(
                condition.name,
                DurationType.UntilLongRest,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.name,
                1,
                condition.name,
                0,
                0,
                0);

            var dieRoll = rulesetCharacter.RollDie(
                DieType.D3,
                RollContext.None,
                false,
                AdvantageType.None,
                out _,
                out _);

            bool recovered = false;

            // TODO: let user choose the repertoire to recover
            foreach (var repertory in rulesetCharacter.spellRepertoires)
            {
                if (!repertory.HasMissingSpellSlots()) 
                { 
                    continue; 
                }
                if (ServiceRepository.GetService<IPlayerControllerService>()?.ActivePlayerController?.IsCharacterControlled(rulesetCharacter) ?? false)
                {
                    Gui.GuiService.GetScreen<SlotRecoveryModal>().ShowSlotRecovery(
                        rulesetCharacter, power.Name, repertory, dieRoll, dieRoll);
                }
                recovered = true;
                break;
            }

            var console = Gui.Game.GameConsole;
            var entryMessage = recovered
                ? $"Feedback/&Power{Name}BolsteringMagicRecoveredSlot"
                : $"Feedback/&Power{Name}BolsteringMagicFailed";
            var entry = new GameConsoleEntry(entryMessage, console.consoleTableDefinition);

            console.AddCharacterEntry(rulesetCharacter, entry);
            console.AddEntry(entry);
        }
    }

    private class WildSurgeAfterRage(WildSurgeHandler handler) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            switch (characterAction.ActionId)
            {
                case Id.RageStart:
                    yield return handler.HandleWildSurge(characterAction.ActingCharacter);
                    break;
                case Id.RageStop:
                    handler.RemoveExistingWildSurgeCondition(characterAction.ActingCharacter.RulesetCharacter);
                    break;
                default:
                    yield break;
            }
        }
    }

    private sealed class WildSurgeEffect
    {
        public ConditionDefinition condition;
        public FeatureDefinitionPower power;
        public FeatureDefinitionPower reactPower;
        public WildSurgeReactionHandler ReactionHandler { get; set; }

        public delegate IEnumerator WildSurgeReactionHandler(GameLocationCharacter me, GameLocationCharacter attacker);
    }

    private class WildSurgeHandler
    {
        readonly List<WildSurgeEffect> wildSurgeEffects = [];
        
        enum WildSurgeType
        {
            Drain = 1,
            Teleport,
            Summon,
            Weapon,
            Retribution,
            Aura,
            Growth,
            Bolt
        }
        public WildSurgeHandler()
        {
            wildSurgeEffects.Add(BuildWildSurgeDrain());
            wildSurgeEffects.Add(BuildWildSurgeTeleport());
            wildSurgeEffects.Add(BuildWildSurgeSummon());
            wildSurgeEffects.Add(BuildWildSurgeWeapon());
            wildSurgeEffects.Add(BuildWildSurgeRetribution());
            wildSurgeEffects.Add(BuildWildSurgeAura());
            wildSurgeEffects.Add(BuildWildSurgeGrowth());
            wildSurgeEffects.Add(BuildWildSurgeBolt());
        }

        private static WildSurgeEffect BuildWildSurgeRetribution()
        {
            var featureWildSurgeRetribution = FeatureDefinitionBuilder
                .Create($"Feature{Name}Retribution")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
            var wildRetributionHandler = new WildSurgeRetributionPhysicalAttackFinishedOnMe();

            featureWildSurgeRetribution.AddCustomSubFeatures(
                wildRetributionHandler);

            var condition = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Retribution")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(featureWildSurgeRetribution)
                .AddToDB();

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
                .SetNoSavingThrow()
                .SetEffectForms(
                    EffectFormBuilder.DamageForm(DamageTypeForce, 1, DieType.D6)
                )
                .SetParticleEffectParameters(SpellDefinitions.MagicMissile)
                .Build();


            var powerWildSurgeRetribution = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Retribution")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicMissile)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Irrelevant, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique, 1)
                        .SetNoSavingThrow()
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.MagicMissile)
                        .Build())
                .AddToDB();
            return new WildSurgeEffect()
            {
                condition = condition,
                ReactionHandler = wildRetributionHandler.HandleWildRetribution
            };
        }

        private sealed class WildSurgeRetributionPhysicalAttackFinishedOnMe() : IPhysicalAttackFinishedOnMe
        {

            public IEnumerator OnPhysicalAttackFinishedOnMe(
                GameLocationBattleManager battleManager,
                CharacterAction action,
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                RulesetAttackMode attackMode,
                RollOutcome rollOutcome,
                int damageAmount)
            {
                if (rollOutcome != RollOutcome.Success && rollOutcome != RollOutcome.CriticalSuccess)
                {
                    yield break;
                }
                yield return HandleWildRetribution(defender, attacker);
            }

            public IEnumerator HandleWildRetribution(GameLocationCharacter defender, GameLocationCharacter attacker)
            {
                var rulesetAttacker = attacker.RulesetCharacter;
                var rulesetDefender = defender.RulesetCharacter;

                if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                    rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
                {
                    yield break;
                }

                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeForce,
                    DieType = DieType.D6,
                    DiceNumber = 1,
                    BonusDamage = 0
                };

                var rolls = new List<int>();
                var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetDefender,
                    targetCharacter = rulesetAttacker,
                    position = attacker.LocationPosition
                };

                EffectHelpers.StartVisualEffect(attacker, defender, SpellDefinitions.MagicMissile, EffectHelpers.EffectType.Impact);
                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    DamageTypeForce,
                    applyFormsParams,
                    rulesetAttacker,
                    false,
                    rulesetDefender.Guid,
                    false,
                    [],
                    new RollInfo(damageForm.DieType, rolls, 0),
                    false,
                    out _);
            }
        }

        private static WildSurgeEffect BuildWildSurgeWeapon()
        {
            var featureWildSurgeWeapon = FeatureDefinitionAdditionalDamageBuilder
                .Create($"Feature{Name}Weapon")
                .SetGuiPresentation(Category.Feature)
                .SetNotificationTag(FeatureDefinitionAdditionalDamages.AdditionalDamageConditionRaging.NotificationTag)
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.RageDamage)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
               .AddCustomSubFeatures(new ValidateContextInsteadOfRestrictedProperty(
                (_, _, _, _, rangedAttack, mode, effect) =>
                    (OperationType.Set, mode != null && mode.Thrown && rangedAttack)
                        ))
                .AddToDB();

            featureWildSurgeWeapon.AddCustomSubFeatures(
                new ReturningWeapon(ValidatorsWeapon.AlwaysValid),
                new WildSurgeWeaponModifyAttackMode());

            var condition = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Weapon")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(featureWildSurgeWeapon)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = condition
            };
        }

        private sealed class WildSurgeWeaponModifyAttackMode() :
            IModifyWeaponAttackMode
        {
            public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
            {
                attackMode.AddAttackTagAsNeeded(TagsDefinitions.WeaponTagThrown);
                attackMode.thrown = true;
                attackMode.closeRange = 4;
                attackMode.maxRange = 12;

                if (attackMode.EffectDescription?.EffectForms == null)
                {
                    return;
                }

                foreach (var item in attackMode.EffectDescription.EffectForms)
                {
                    var damageForm = item.DamageForm;
                    if (damageForm != null)
                    {
                        damageForm.DamageType = DamageTypeForce;
                    }
                }
            }
        }

        private static WildSurgeEffect BuildWildSurgeDrain()
        {
            return new WildSurgeEffect()
            {
                power = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Drain")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.ChillTouch)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                        .ExcludeCaster()
                        .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution, 8)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeNecrotic, 1, DieType.D12)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetTempHpForm(0, DieType.D12, 1, true)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.ChillTouch)
                        .Build())
                .AddToDB()
            };
        }

        private static WildSurgeEffect BuildWildSurgeTeleport()
        {
            var powerTeleport = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Teleport")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.DimensionDoor)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.MistyStep)
                        .Build())
                .AddToDB();

            ActionDefinitionBuilder.Create("WildSurgeTeleport")
                .SetActionId(ExtraActionId.WildSurgeTeleport)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetGuiPresentation(Category.Action, SpellDefinitions.MistyStep, 20)
                .SetActionScope(ActionScope.Battle)
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerTeleport)
                .AddToDB();

            var actionAffinityTeleport = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Teleport")
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeTeleport)
                .AddToDB();

            var conditionWildSurgeTeleport = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Teleport")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(actionAffinityTeleport)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeTeleport,
                power = powerTeleport
            };
        }

        private static WildSurgeEffect BuildWildSurgeSummon()
        {
            var powerSummonBlast = FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate, $"Power{Name}SummonBlast")
                .SetGuiPresentation(Category.Feature)
                .SetEffectDescription(EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution, 8)
                    .SetEffectForms(
                        EffectFormBuilder.Create()
                        .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                    .Build())
                .AddToDB();

            var powerReactionSummonBlast = FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate, $"Power{Name}ReactionSummonBlast")
                .SetGuiPresentation(Category.Feature)
                .SetEffectDescription(EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate)
                    .SetTargetingData(Side.All, RangeType.Distance, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution, 8)
                    .SetEffectForms(
                        EffectFormBuilder.Create()
                        .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Shatter)
                    .Build())
                .AddToDB();

            var proxySummon = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyDancingLights, $"Proxy{Name}Summon")
                .SetGuiPresentation(Category.Proxy, EffectProxyDefinitions.ProxyDancingLights)
                .SetPortrait(EffectProxyDefinitions.ProxyDancingLights.PortraitSpriteReference)
                .SetCanMove(false, false)
                .AddToDB();

            proxySummon.autoTerminateOnTriggerPower = true;
            proxySummon.attackPower = powerSummonBlast;
            proxySummon.actionId = Id.NoAction;
            proxySummon.canTriggerPower = true;

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 2, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                .SetEffectForms(
                    EffectFormBuilder.Create().SetSummonEffectProxyForm(proxySummon)
                    .Build())
                .Build();

            var powerSummon = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Summon")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.DelayedBlastFireball)
                .SetUsesFixed(ActivationTime.BonusAction)
                .SetEffectDescription(effectDescription)
                .AddToDB();

            ActionDefinitionBuilder.Create("WildSurgeSummon")
                .SetActionId(ExtraActionId.WildSurgeSummon)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetGuiPresentation(Category.Action, SpellDefinitions.DancingLights, 20)
                .SetActionScope(ActionScope.Battle)
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerSummon)
                .AddToDB();

            var actionAffinitySummon = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Summon")
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeSummon)
                .AddCustomSubFeatures(new WildSurgeSummonOnTurnEnd(proxySummon))
                .AddToDB();

            var conditionWildSurgeSummon = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Summon")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(actionAffinitySummon)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeSummon,
                power = powerSummon,
                reactPower = powerReactionSummonBlast
            };
        }
        private sealed class WildSurgeSummonOnTurnEnd(EffectProxyDefinition powerSummon) 
            : ICharacterBeforeTurnEndListener, ICharacterBeforeTurnStartListener
        {
            public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
            {
                ProcessWildSurgeSummon(locationCharacter);
            }

            public void OnCharacterBeforeTurnStarted(GameLocationCharacter locationCharacter)
            {
                ProcessWildSurgeSummon(locationCharacter);
            }

            private void ProcessWildSurgeSummon(GameLocationCharacter locationCharacter)
            {
                if (locationCharacter?.RulesetCharacter?.controlledEffectProxies == null
                    || locationCharacter.RulesetCharacter.controlledEffectProxies.Count == 0)
                {
                    return;
                }

                foreach (var proxyCharacter in locationCharacter.RulesetCharacter.controlledEffectProxies)
                {
                    if (proxyCharacter?.EffectProxyDefinition?.Name != powerSummon?.Name || proxyCharacter?.ControllerGuid == null)
                    {
                        continue;
                    }

                    if (RulesetEntity.TryGetEntity<RulesetCharacter>(proxyCharacter.ControllerGuid, out var entity))
                    {
                        ServiceRepository.GetService<IRulesetImplementationService>()?.AutoTriggerProxy(proxyCharacter, entity);
                    }
                }
            }
        }

        private static WildSurgeEffect BuildWildSurgeAura()
        {
            var attributeModifierAuraBonus = FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{Name}AuraBonus")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                .AddToDB();

            var conditionAuraBonus = ConditionDefinitionBuilder
                .Create($"Condition{Name}AuraBonus")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(attributeModifierAuraBonus)
                .AddToDB();

            var auraPower = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Aura")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.ChillTouch)
                .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
                .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 5)
                        .SetDurationData(DurationType.Permanent)
                        .SetRecurrentEffect(
                            RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(conditionAuraBonus, ConditionForm.ConditionOperation.Add)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(conditionAuraBonus, ConditionForm.ConditionOperation.Add, true)
                                .Build()
                                )
                        .Build())
                .AddToDB();

            var conditionWildSurgeAura = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Aura")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(auraPower)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeAura,
                power = auraPower
            };
        }

        private static WildSurgeEffect BuildWildSurgeGrowth()
        {
            var proxyGrowth = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxySpikeGrowth, $"Proxy{Name}Growth")
                .SetOrUpdateGuiPresentation($"Proxy{Name}Growth", Category.Proxy)
                .AddToDB();

            var powerGrowth = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Growth")
                .SetUsesFixed(ActivationTime.NoCost)
                .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Entangle)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .UseQuickAnimations()
                    .SetNoSavingThrow()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(proxyGrowth)
                            .Build(),
                        EffectFormBuilder.CreateTopologyForm(TopologyForm.Type.DifficultThrough, false)
                        .Build())
                    .Build())
                .AddToDB();
            var growthHandler = new WildSurgeGrowthOnTurnEnd(powerGrowth);
            var featureGrowth = FeatureDefinitionMovementAffinityBuilder.Create($"Feature{Name}Growth")
                .SetImmunities(false, false, true)
                .AddCustomSubFeatures(growthHandler)
                .AddToDB();

            var conditionWildSurgeGrowth = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Growth")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(featureGrowth)
                .AddToDB();
            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeGrowth,
                ReactionHandler = growthHandler.ReactionHandler
            };
        }

        private sealed class WildSurgeGrowthOnTurnEnd(FeatureDefinitionPower power) : ICharacterBeforeTurnEndListener
        {

            public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
            {
                var rulesetCharacter = locationCharacter.RulesetCharacter;
                CharacterActionParams actionParams = new CharacterActionParams(locationCharacter, ActionDefinitions.Id.PowerBonus);
                var usablePower = PowerProvider.Get(power, rulesetCharacter);
                actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectPower(rulesetCharacter, usablePower, false);
                actionParams.SkipAnimationsAndVFX = true;
                ServiceRepository.GetService<ICommandService>()?
                    .ExecuteAction(actionParams, null, false);
            }

            internal IEnumerator ReactionHandler(GameLocationCharacter locationCharacter, GameLocationCharacter _)
            {
                OnCharacterBeforeTurnEnded(locationCharacter);
                yield return null;
            }
        }

        private static WildSurgeEffect BuildWildSurgeBolt()
        {
            var powerBolt = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Bolt")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.GuidingBolt)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 1)
                        .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution, 8)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(ConditionDefinitions.ConditionBlinded, ConditionForm.ConditionOperation.Add)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.GuidingBolt)
                        .Build())
                .AddToDB();

            ActionDefinitionBuilder.Create("WildSurgeBolt")
                .SetActionId(ExtraActionId.WildSurgeBolt)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetGuiPresentation(Category.Action, SpellDefinitions.GuidingBolt, 20)
                .SetActionScope(ActionScope.Battle)
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerBolt)
                .AddToDB();

            var actionAffinityBolt = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Bolt")
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeBolt)
                .AddToDB();

            var conditionWildSurgeBolt = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Bolt")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn)
                .SetFeatures(actionAffinityBolt)
                .AddToDB();
            return new WildSurgeEffect()
            {
                power = powerBolt,
                condition = conditionWildSurgeBolt
            };
        }

        int _dieRoll = 1;
        public IEnumerator HandleWildSurge(GameLocationCharacter character, GameLocationCharacter attacker = null)
        {
            if (character == null)
            {
                yield break;
            }

            var rulesetCharacter = character.RulesetCharacter;

            ICursorService cursorService = ServiceRepository.GetService<ICursorService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            // choose surge effect
            var dieRoll =
                rulesetCharacter.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);
            _dieRoll = (_dieRoll % 8) + 1;
            dieRoll = _dieRoll;

            var wildSurgeEffect = wildSurgeEffects.ElementAt(dieRoll - 1);

            var firstTime = true;
            if (wildSurgeEffect.condition != null)
            {
                var existingCondition = GetExistingWildSurgeCondition(rulesetCharacter);
                if (existingCondition?.Name == wildSurgeEffect.condition.Name)
                {
                    firstTime = false;
                }
                else
                {
                    if (existingCondition != null)
                    {
                        rulesetCharacter.RemoveAllConditionsOfType(existingCondition.Name);
                    }
                    rulesetCharacter.InflictCondition(
                        wildSurgeEffect.condition.Name,
                        DurationType.Irrelevant,
                        0,
                        TurnOccurenceType.StartOfTurn,
                        AttributeDefinitions.TagEffect,
                        character.RulesetCharacter.Guid,
                        character.RulesetCharacter.CurrentFaction.Name,
                        1,
                        wildSurgeEffect.condition.Name,
                        0,
                        0,
                        0);
                }
            }
            var power = 
                attacker != null ? wildSurgeEffect.reactPower ?? wildSurgeEffect.power :
                wildSurgeEffect.power;
            if (power != null)
            {
                CharacterActionParams actionParams = new CharacterActionParams(character, ActionDefinitions.Id.PowerBonus);
                var usablePower = PowerProvider.Get(power, rulesetCharacter);
                actionParams.RulesetEffect = implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, true);
                
                if (power.EffectDescription.TargetType == TargetType.Position)
                {
                    cursorService.ActivateCursor<CursorLocationSelectPosition>([actionParams]);
                }
                else if (power.EffectDescription.TargetType == TargetType.Individuals
                    || power.EffectDescription.TargetType == TargetType.IndividualsUnique)
                {
                    if (attacker != null)
                    {
                        actionParams.TargetCharacters.Add(attacker);
                        actionParams.ActionModifiers.Add(new ActionModifier());
                        ServiceRepository.GetService<ICommandService>()?
                            .ExecuteAction(actionParams, null, true);
                    }
                    else
                    {
                        cursorService.ActivateCursor<CursorLocationSelectTarget>([actionParams]);
                    }
                }
                else
                {
                    if (power.EffectDescription.TargetType == TargetType.Cube && attacker != null)
                    {

                        actionParams.positions = [attacker.LocationPosition];
                    }
                    ServiceRepository.GetService<ICommandService>()?
                        .ExecuteAction(actionParams, null, true);
                }
            }
            if (firstTime && attacker != null && wildSurgeEffect.ReactionHandler != null)
            {
                yield return wildSurgeEffect.ReactionHandler(character, attacker);
            }
        }

        public void RemoveExistingWildSurgeCondition(RulesetCharacter character)
        {
            var matchingCondition = GetExistingWildSurgeCondition(character);
            if (matchingCondition != null)
            {
                character.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect, matchingCondition.Name);
            }
        }

        private ConditionDefinition GetExistingWildSurgeCondition(RulesetCharacter character)
        {
            foreach (var wildEffect in wildSurgeEffects)
            {
                if (wildEffect.condition != null && character.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, wildEffect.condition.Name))
                {
                    return wildEffect.condition;
                }
            }
            return null;
        }
    }

    private sealed class UnstableBackslashHandler(WildSurgeHandler wildSurgeHandler) 
        : IPhysicalAttackFinishedOnMe, IMagicEffectBeforeHitConfirmedOnMe
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
            // check if it hit
            yield return HandleUnstableBacklash(defender, attacker);
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            yield return HandleUnstableBacklash(defender, attacker);
        }

        private IEnumerator HandleUnstableBacklash(GameLocationCharacter defender, GameLocationCharacter attacker)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!actionManager)
            {
                yield break;
            }

            if (!defender.RulesetCharacter.HasAnyConditionOfTypeOrSubType(
                    ConditionDefinitions.ConditionRaging.Name,
                    ConditionDefinitions.ConditionRagingNormal.Name,
                    ConditionDefinitions.ConditionRagingPersistent.Name) ||
                !defender.CanReact() ||
                !defender.IsReactionAvailable() ||
                defender.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!defender.RulesetCharacter.IsToggleEnabled(WildSurgeUnstableBacklashToggle))
            {
                yield break;
            }

            var actionParams = new CharacterActionParams(defender, (Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = $"CustomReactionUnstableBacklashDescription"
                    .Formatted(Category.Reaction, defender.Name)
            };

            var count = actionManager.PendingReactionRequestGroups.Count;

            var reactionRequest = new ReactionRequestCustom($"UnstableBacklash", actionParams);

            actionManager.AddInterruptRequest(reactionRequest);
            
            yield return battleManager.WaitForReactions(attacker, actionManager, count);
            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            defender.ReactionEngaged = true;

            yield return wildSurgeHandler.HandleWildSurge(defender, attacker);
        }
    }

}
