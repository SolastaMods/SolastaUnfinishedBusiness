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
    private const string ConditionWildSurgePrefix = "ConditionWildSurge";
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

        var effectMagicAwareness = SpellDefinitions.DetectMagic.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Divination).DeepCopy();

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

        var featuresBolsteringMagic = BuildFeatureSetBolsteringMagic();

        // Unstable Backslash

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "WildSurgeBacklashToggle")
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
            .AddCustomSubFeatures(new UnstableBackslash(wildSurgeHandler))
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

    private FeatureDefinition[] BuildFeatureSetBolsteringMagic()
    {
        var powerBolsteringMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BolsteringMagic")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(HasModifiedUses.Marker, IsModifyPowerPool.Marker)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
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

        var conditionBolsteringMagicSpellSlot = ConditionDefinitionBuilder.Create($"Condition{Name}BolsteringMagicSpell")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerBolsteringMagicSpell = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicSpell")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerImpHospitality", Resources.PowerImpHospitality, 256, 128))
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals)
                .SetCasterEffectParameters(FeatureDefinitionPowers.PowerKnightLeadership)
                .Build()
            )
            .AddToDB();

        powerBolsteringMagicSpell.AddCustomSubFeatures(new BolsteringMagicSpell(powerBolsteringMagicSpell, conditionBolsteringMagicSpellSlot));

        PowerBundle.RegisterPowerBundle(powerBolsteringMagic, false,
            powerBolsteringMagicRoll, powerBolsteringMagicSpell);

        return [powerBolsteringMagic, powerBolsteringMagicRoll, powerBolsteringMagicSpell];
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

    private sealed class BolsteringMagicSpell(FeatureDefinitionPower power, ConditionDefinition condition) : IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var hero = GetOriginalHero(target);
            if (hero == null)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&PathOfTheWildMagicNotAHero");
                return false;
            }

            if (hero.HasConditionOfType(condition.name))
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&PathOfTheWildMagicAlreadyBolstered");
                return false;
            }

            if (!hero.CanCastSpells())
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&PathOfTheWildMagicCannotCastSpells");
                return false;
            }

            foreach (var item in hero.SpellRepertoires)
            {
                if (!item.HasMissingSpellSlots())
                {
                    __instance.actionModifier.FailureFlags.Add("Tooltip/&PathOfTheWildMagicDoesNotHaveMissingSpellSlots");
                    return false;
                }
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
            var targetCharacter = action.actionParams.TargetCharacters?.First();

            if (targetCharacter == null)
            {
                yield break;
            }

            var rulesetCharacter = GetOriginalHero(targetCharacter);
            // TODO: let user choose the repertoire
            // TODO: let user only choose targets with spell casting

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

            // TODO: Die roll for spell slot to recover
            int slot = 1;
            bool recovered = false;
            foreach (var repertory in rulesetCharacter.spellRepertoires)
            {
                if (!repertory.HasMissingSpellSlots()) { continue; }
                recovered = true;
                break;
            }

            var console = Gui.Game.GameConsole;
            var entryMessage = recovered
                ? "Feedback/&PowerBolsteringMagicRecoveredSlot"
                : "Feedback/&PowerPathOfTheWildMagicBolsteringMagicFailed";
            var entry = new GameConsoleEntry(entryMessage, console.consoleTableDefinition);

            console.AddCharacterEntry(rulesetCharacter, entry);

            if (recovered)
            {
                entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, slot.ToString());
            }

            console.AddEntry(entry);
        }
    }

    private class WildSurgeAfterRage(WildSurgeHandler handler) : IActionFinishedByMe, IOnConditionAddedOrRemoved
    {

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            switch (characterAction.ActionId)
            {
                case Id.RageStart:
                    yield return handler.HandleWildSurge(characterAction.ActingCharacter);
                    break;
                case Id.RageStop:
                    handler.RemoveExistingWildSurge(characterAction.ActingCharacter.RulesetCharacter);
                    break;
                default:
                    yield break;
            }
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // this function is intentionally left blank
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.Name == ConditionDefinitions.ConditionRaging.Name
                || rulesetCondition.Name == ConditionDefinitions.ConditionRagingNormal.Name
                || rulesetCondition.Name == ConditionDefinitions.ConditionRagingPersistent.Name
                )
            {
                Trace.LogWarning("Removing raging condition from " + target.Name);
                handler.RemoveExistingWildSurge(target);
            } 
        }
    }

    private sealed class WildSurgeEffect()
    {
        public ConditionDefinition condition;
        public FeatureDefinitionPower power;
    }

    private class WildSurgeHandler
    {
        readonly List<WildSurgeEffect> wildSurgeEffects = [];

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

            featureWildSurgeRetribution.AddCustomSubFeatures(
                new WildSurgeRetributionPhysicalAttackFinishedOnMe(featureWildSurgeRetribution));

            var condition = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Retribution")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(featureWildSurgeRetribution)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = condition
            };
        }

        private WildSurgeEffect BuildWildSurgeWeapon()
        {
            var featureWildSurgeWeapon = FeatureDefinitionBuilder
                .Create($"Feature{Name}Weapon")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            featureWildSurgeWeapon.AddCustomSubFeatures(
                new ReturningWeapon(ValidatorsWeapon.AlwaysValid),
                new WildSurgeWeaponModifyAttackMode(featureWildSurgeWeapon));

            var condition = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Weapon")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(featureWildSurgeWeapon)
                .AddToDB();

            return new WildSurgeEffect()
            {
                condition = condition
            };
        }

        #region Wild Surge - Drain
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
        #endregion

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
                        .SetParticleEffectParameters(SpellDefinitions.DimensionDoor)
                        .Build())
                .AddToDB();

            ActionDefinitionBuilder.Create("WildSurgeTeleport")
                .SetActionId(ExtraActionId.WildSurgeTeleport)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetGuiPresentation(Category.Action, SpellDefinitions.DimensionDoor, 20)
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
                .SetTerminateWhenRemoved()
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
            powerSummonBlast.EffectDescription.effectParticleParameters.zoneParticleReference 
                = SpellDefinitions.Shatter.EffectDescription.effectParticleParameters.zoneParticleReference;

            var proxySummon = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyDancingLights, $"Proxy{Name}Summon")
                .SetGuiPresentation(Category.Proxy, EffectProxyDefinitions.ProxyFlamingSphere)
                .SetPortrait(EffectProxyDefinitions.ProxyFlamingSphere.PortraitSpriteReference)
                .SetCanMove(false, false)
                .AddToDB();

            proxySummon.autoTerminateOnTriggerPower = true;
            proxySummon.attackPower = powerSummonBlast;
            proxySummon.actionId = Id.NoAction;
            proxySummon.canTriggerPower = true;

            var powerSummon = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Summon")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.DelayedBlastFireball)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                        .SetEffectForms(EffectFormBuilder.Create()
                            .SetSummonEffectProxyForm(proxySummon)
                            .Build())
                        .UseQuickAnimations()
                        .Build())
                .AddToDB();

            ActionDefinitionBuilder.Create("WildSurgeSummon")
                .SetActionId(ExtraActionId.WildSurgeSummon)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetGuiPresentation(Category.Action, SpellDefinitions.FlamingSphere, 20)
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
                .SetTerminateWhenRemoved()
                .SetFeatures(actionAffinitySummon)
                .AddToDB();
            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeSummon,
                power = powerSummon
            };
        }
        private sealed class WildSurgeSummonOnTurnEnd(EffectProxyDefinition powerSummon) : ICharacterBeforeTurnEndListener
        {
            public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
            {
                if (locationCharacter?.RulesetCharacter?.controlledEffectProxies == null
                    || locationCharacter.RulesetCharacter.controlledEffectProxies.Count == 0)
                {
                    return;
                }

                foreach (var proxyCharacter in locationCharacter.RulesetCharacter.controlledEffectProxies)
                {
                    if (proxyCharacter?.EffectProxyDefinition?.Name != powerSummon?.Name)
                    {
                        continue;
                    }

                    if (proxyCharacter?.ControllerGuid == null)
                    {
                        continue;
                    }

                    if (RulesetEntity.TryGetEntity<RulesetCharacter>(proxyCharacter.ControllerGuid, out var entity2))
                    {
                        ServiceRepository.GetService<IRulesetImplementationService>()?.AutoTriggerProxy(proxyCharacter, entity2);
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
                .SetTerminateWhenRemoved()
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
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 5)
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
            

            var featureGrowth = FeatureDefinitionBuilder.Create($"Feature{Name}Growth")
                .AddCustomSubFeatures(new WildSurgeGrowthOnTurnEnd(powerGrowth))
                .AddToDB();

            var conditionWildSurgeGrowth = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Growth")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetTerminateWhenRemoved()
                .SetFeatures(featureGrowth)
                .AddToDB();
            return new WildSurgeEffect()
            {
                condition = conditionWildSurgeGrowth
            };
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
                .SetTerminateWhenRemoved()
                .SetFeatures(actionAffinityBolt)
                .AddToDB();
            return new WildSurgeEffect()
            {
                power = powerBolt,
                condition = conditionWildSurgeBolt
            };
        }

        public IEnumerator HandleWildSurge(GameLocationCharacter character)
        {
            if (character == null)
            {
                yield break;
            }

            var rulesetCharacter = character.RulesetCharacter;
            // remove all preexisting conditions
            RemoveExistingWildSurge(rulesetCharacter);

            ICursorService cursorService = ServiceRepository.GetService<ICursorService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            // chooce surge effect
            var dieRoll =
                rulesetCharacter.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);

            dieRoll = 7;

            var wildSurgeEffect = wildSurgeEffects.ElementAt(dieRoll - 1);
            if (wildSurgeEffect.condition != null)
            {
                rulesetCharacter.InflictCondition(
                    wildSurgeEffect.condition.name,
                    DurationType.Irrelevant,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    character.RulesetCharacter.guid,
                    character.RulesetCharacter.CurrentFaction.name,
                    1,
                    wildSurgeEffect.condition.name,
                    0,
                    0,
                    0);
            }

            if (wildSurgeEffect.power != null)
            {
                CharacterActionParams actionParams = new CharacterActionParams(character, ActionDefinitions.Id.PowerBonus);
                var usablePower = PowerProvider.Get(wildSurgeEffect.power, rulesetCharacter);
                actionParams.RulesetEffect = implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, true);

                if (wildSurgeEffect.power.EffectDescription.TargetType == TargetType.Position)
                {
                    cursorService.ActivateCursor<CursorLocationSelectPosition>([actionParams]);
                }
                else if (wildSurgeEffect.power.EffectDescription.TargetType == TargetType.Individuals
                    || wildSurgeEffect.power.EffectDescription.TargetType == TargetType.IndividualsUnique)
                {
                    cursorService.ActivateCursor<CursorLocationSelectTarget>([actionParams]);
                }
                else
                {
                    ServiceRepository.GetService<ICommandService>()?
                        .ExecuteAction(actionParams, null, true);
                }

            }
        }

        public void RemoveExistingWildSurge(RulesetCharacter character)
        {
            foreach (var wildEffect in wildSurgeEffects)
            {
                if (wildEffect.condition == null)
                {
                    continue;
                }
                Trace.LogWarning("Removing type: " + wildEffect.condition.name + " from " + character.Name);
                character.RemoveAllConditionsOfType(wildEffect.condition.Name);
            }
        }

    }

    private sealed class UnstableBackslash(WildSurgeHandler wildSurgeHandler) : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = defender.RulesetCharacter;
            if (!defender.CanReact() ||
                rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.IsToggleEnabled(WildSurgeUnstableBacklashToggle))
            {
                yield break;
            }

            yield return wildSurgeHandler.HandleWildSurge(defender);
        }
    }

    private sealed class WildSurgeGrowthOnTurnEnd(FeatureDefinitionPower power) : ICharacterBeforeTurnEndListener
    {

        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            CharacterActionParams actionParams = new CharacterActionParams(locationCharacter, ActionDefinitions.Id.PowerBonus);
            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetCharacter, usablePower, true);

            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }
    private sealed class WildSurgeWeaponModifyAttackMode(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureHammersBoon) :
        IModifyWeaponAttackMode, IModifyAttackActionModifier
    {

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.CharacterFeature, featureHammersBoon.Name, featureHammersBoon));
        }

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

    private sealed class WildSurgeRetributionPhysicalAttackFinishedOnMe(FeatureDefinition featureWildSurgeRetribution) : IPhysicalAttackFinishedOnMe
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

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            rulesetDefender.LogCharacterUsedFeature(featureWildSurgeRetribution);
            EffectHelpers.StartVisualEffect(defender, attacker, SpellDefinitions.MagicMissile);

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

            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                DamageTypeForce,
                applyFormsParams,
                rulesetAttacker,
                false,
                rulesetDefender.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(damageForm.DieType, rolls, 0),
                false,
                out _);
        }
    }

}

