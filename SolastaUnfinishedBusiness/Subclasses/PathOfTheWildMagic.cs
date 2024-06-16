using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.Playables;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

public sealed class PathOfTheWildMagic : AbstractSubclass
{
    private const string Name = "PathOfTheWildMagic";
    private const string ConditionWildSurgePrefix = $"Condition{Name}WildSurge";

    public PathOfTheWildMagic()
    {
        // Controlled Surge
        var featureControlledSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}ControlledSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var wildSurgeHandler = new WildSurgeHandler(featureControlledSurge);

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
        var featureBolsteringMagic = BuildFeatureBolsteringMagic();

        // Unstable Backslash
        var featureUnstableBacklash = BuildFeatureUnstableBacklash(wildSurgeHandler);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheWildMagic, 256))
            .AddFeaturesAtLevel(3, featureWildSurge, powerWildMagicAwareness)
            .AddFeaturesAtLevel(6, featureBolsteringMagic)
            .AddFeaturesAtLevel(10, featureUnstableBacklash)
            .AddFeaturesAtLevel(14, featureControlledSurge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    #region Wild Surge

    private class WildSurgeAfterRage(WildSurgeHandler handler) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction.ActionId == Id.RageStart)
            {
                yield return handler.HandleWildSurge(characterAction.ActingCharacter);
            }
        }
    }

    private sealed class WildSurgeEffect
    {
        public ConditionDefinition condition;
        public ConditionDefinition conditionFirstTurn;
        public string name;
        public FeatureDefinitionPower power;
        public FeatureDefinitionPower reactPower;
    }

    private class WildSurgeHandler
    {
        private readonly ConditionDefinition conditionPreventAction;
        private readonly FeatureDefinition featureControlledSurge;
        private readonly FeatureDefinitionPower powerPool;
        private readonly List<FeatureDefinitionPower> powers;
        private readonly List<WildSurgeEffect> wildSurgeEffects = [];

        public WildSurgeHandler(FeatureDefinition featureControlledSurge)
        {
            this.featureControlledSurge = featureControlledSurge;
            wildSurgeEffects.Add(BuildWildSurgeDrain());
            wildSurgeEffects.Add(BuildWildSurgeTeleport());
            wildSurgeEffects.Add(BuildWildSurgeSummon());
            wildSurgeEffects.Add(BuildWildSurgeWeapon());
            wildSurgeEffects.Add(BuildWildSurgeRetribution());
            wildSurgeEffects.Add(BuildWildSurgeAura());
            wildSurgeEffects.Add(BuildWildSurgeGrowth());
            wildSurgeEffects.Add(BuildWildSurgeBolt());

            conditionPreventAction = ConditionDefinitionBuilder
                .Create($"Condition{Name}PreventAction")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(
                    FeatureDefinitionActionAffinityBuilder
                        .Create($"ActionAffinity{Name}PreventAction")
                        .SetGuiPresentationNoContent(true)
                        .SetForbiddenActions(
                            Id.Shove,
                            Id.ShoveBonus,
                            Id.AttackMain,
                            Id.AttackOff,
                            Id.AttackFree)
                        .AddToDB())
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .AddToDB();

            powerPool = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}WildSurgePool")
                .SetGuiPresentationNoContent(true)
                .SetUsesFixed(ActivationTime.NoCost)
                .AddToDB();
            powers = [];

            foreach (var effect in wildSurgeEffects)
            {
                var title = Gui.Localize($"Condition/&{ConditionWildSurgePrefix}{effect.name}Title");
                var description = Gui.Localize($"Condition/&{ConditionWildSurgePrefix}{effect.name}Description");
                var power = FeatureDefinitionPowerSharedPoolBuilder
                    .Create($"Power{Name}WildSurge{effect.name}")
                    .SetGuiPresentation(title, description)
                    .SetSharedPool(ActivationTime.NoCost, powerPool)
                    .AddToDB();
                powers.Add(power);
            }

            PowerBundle.RegisterPowerBundle(powerPool, false, powers);
        }

        private static WildSurgeEffect BuildWildSurgeDrain()
        {
            var title = Gui.Localize($"Condition/&{ConditionWildSurgePrefix}DrainTitle");
            var description = Gui.Localize($"Condition/&{ConditionWildSurgePrefix}DrainDescription");

            var powerDrain = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Drain")
                .SetGuiPresentation(title, description)
                .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
                .SetShowCasting(false)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                        .ExcludeCaster()
                        .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            AttributeDefinitions.Constitution, 8)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeNecrotic, 1, DieType.D12)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetTempHpForm(0, DieType.D12, 1, true)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.ChillTouch)
                        .Build())
                .AddToDB();

            return new WildSurgeEffect { name = "Drain", power = powerDrain };
        }

        private static WildSurgeEffect BuildWildSurgeTeleport()
        {
            var actionAffinityTeleport = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Teleport")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeTeleport)
                .AddToDB();

            var actionAffinityTeleportFree = FeatureDefinitionActionAffinityBuilder
                .Create(FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack,
                    $"ActionAffinity{Name}TeleportFree")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeTeleportFree)
                .AddToDB();

            var conditionWildSurgeTeleport = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Teleport")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(actionAffinityTeleport)
                .AddToDB();

            var conditionWildSurgeTeleportFree = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}TeleportFree")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(actionAffinityTeleportFree)
                .AddToDB();

            var powerTeleport = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Teleport")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.MistyStep)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(Side.Ally, RangeType.Distance, 7, TargetType.Position)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(conditionWildSurgeTeleportFree,
                                    ConditionForm.ConditionOperation.Remove, true, true)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.MistyStep)
                        .Build())
                .AddToDB();

            var actionWildSurgeTeleport = ActionDefinitionBuilder
                .Create("WildSurgeTeleport")
                .SetGuiPresentation(Category.Action, SpellDefinitions.MistyStep, 20)
                .SetActionId(ExtraActionId.WildSurgeTeleport)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .SetActionScope(ActionScope.All)
                .RequiresAuthorization()
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerTeleport)
                .AddToDB();

            ActionDefinitionBuilder
                .Create(actionWildSurgeTeleport, "WildSurgeTeleportFree")
                .SetActionId(ExtraActionId.WildSurgeTeleportFree)
                .SetActionType(ActionType.NoCost)
                .AddToDB();

            return new WildSurgeEffect
            {
                name = "Teleport",
                condition = conditionWildSurgeTeleport,
                conditionFirstTurn = conditionWildSurgeTeleportFree,
                reactPower = powerTeleport
            };
        }

        private static WildSurgeEffect BuildWildSurgeSummon()
        {
            var proxySummon = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyDancingLights, $"Proxy{Name}Summon")
                .SetGuiPresentation(Category.Proxy, EffectProxyDefinitions.ProxyDelayedBlastFireball)
                .SetPortrait(EffectProxyDefinitions.ProxyDancingLights.PortraitSpriteReference)
                .SetCanMove(false, false)
                .SetAttackMethod(ProxyAttackMethod.ReproduceDamageForms)
                .AddToDB();

            proxySummon.autoTerminateOnTriggerPower = true;
            proxySummon.actionId = Id.NoAction;
            proxySummon.canTriggerPower = true;

            var actionAffinitySummon = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Summon")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeSummon)
                .AddCustomSubFeatures(new WildSurgeSummonOnTurnEnd(proxySummon))
                .AddToDB();

            var actionAffinitySummonFree = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}SummonFree")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeSummonFree)
                .AddToDB();

            var conditionWildSurgeSummon = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Summon")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(actionAffinitySummon)
                .AddToDB();

            var conditionWildSurgeSummonFree = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}SummonFree")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(actionAffinitySummonFree)
                .AddToDB();

            var effectDescriptionBlast = EffectDescriptionBuilder
                .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate.effectDescription)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution, 8)
                .UseQuickAnimations()
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .Build();

            effectDescriptionBlast.EffectParticleParameters.impactParticleReference
                = SpellDefinitions.MagicMissile.effectDescription.EffectParticleParameters.impactParticleReference;
            effectDescriptionBlast.EffectParticleParameters.zoneParticleReference
                = SpellDefinitions.Shatter.effectDescription.EffectParticleParameters.zoneParticleReference;

            var powerSummonBlast = FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerDelayedBlastFireballDetonate, $"Power{Name}SummonBlast")
                .SetGuiPresentation(Category.Feature)
                .SetEffectDescription(effectDescriptionBlast)
                .AddToDB();
            proxySummon.attackPower = powerSummonBlast;

            var powerSummonBlastReaction = FeatureDefinitionPowerBuilder
                .Create(powerSummonBlast, $"Power{Name}SummonBlastReaction")
                .SetGuiPresentation(Category.Feature)
                .SetEffectDescription(EffectDescriptionBuilder.Create(effectDescriptionBlast)
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Cube, 3)
                    .Build())
                .AddToDB();
            powerSummonBlastReaction.guiPresentation = powerSummonBlast.guiPresentation;

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 2, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                .SetEffectForms(
                    EffectFormBuilder.Create().SetSummonEffectProxyForm(proxySummon)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionWildSurgeSummonFree, ConditionForm.ConditionOperation.Remove, true,
                            true)
                        .Build())
                .Build();

            var powerSummon = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Summon")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.DelayedBlastFireball)
                .SetUsesFixed(ActivationTime.BonusAction)
                .SetEffectDescription(effectDescription)
                .AddToDB();

            var actionSummon = ActionDefinitionBuilder.Create("WildSurgeSummon")
                .SetGuiPresentation(Category.Action, SpellDefinitions.DancingLights, 20)
                .SetActionId(ExtraActionId.WildSurgeSummon)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetActionScope(ActionScope.Battle)
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerSummon)
                .AddToDB();

            ActionDefinitionBuilder.Create(actionSummon, "WildSurgeSummonFree")
                .SetActionId(ExtraActionId.WildSurgeSummonFree)
                .SetActionType(ActionType.NoCost)
                .AddToDB();

            return new WildSurgeEffect
            {
                name = "Summon",
                condition = conditionWildSurgeSummon,
                conditionFirstTurn = conditionWildSurgeSummonFree,
                reactPower = powerSummonBlastReaction
            };
        }

        private static WildSurgeEffect BuildWildSurgeWeapon()
        {
            var featureWildSurgeWeapon = FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{Name}Weapon")
                .SetGuiPresentationNoContent(true)
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
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(featureWildSurgeWeapon)
                .AddToDB();

            return new WildSurgeEffect { name = "Weapon", condition = condition };
        }

        private static WildSurgeEffect BuildWildSurgeRetribution()
        {
            var powerWildSurgeRetribution = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Retribution")
                .SetGuiPresentationNoContent(true)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Irrelevant, 1,
                            (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                        .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Individuals)
                        .SetNoSavingThrow()
                        .UseQuickAnimations()
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.MagicMissile)
                        .Build())
                .AddToDB();

            var featureWildSurgeRetributionMelee = FeatureDefinitionDamageAffinityBuilder
                .Create($"DamageAffinity{Name}RetributionMelee")
                .SetGuiPresentationNoContent(true)
                .SetRetaliate(powerWildSurgeRetribution, 1)
                .AddToDB();
            featureWildSurgeRetributionMelee.retaliateProximity = AttackProximity.Melee;

            var featureWildSurgeRetributionRanged = FeatureDefinitionDamageAffinityBuilder
                .Create($"DamageAffinity{Name}RetributionRange")
                .SetGuiPresentationNoContent(true)
                .SetRetaliate(powerWildSurgeRetribution, 24)
                .AddToDB();
            featureWildSurgeRetributionRanged.retaliateProximity = AttackProximity.Range;

            var condition = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Retribution")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(featureWildSurgeRetributionMelee, featureWildSurgeRetributionRanged)
                .AddToDB();

            return new WildSurgeEffect
            {
                name = "Retribution", condition = condition, reactPower = powerWildSurgeRetribution
            };
        }

        private static WildSurgeEffect BuildWildSurgeAura()
        {
            var attributeModifierAuraBonus = FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{Name}AuraBonus")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass, 1)
                .AddToDB();

            var conditionAuraBonus = ConditionDefinitionBuilder
                .Create($"Condition{Name}AuraBonus")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionParticleReference(ConditionDefinitions.ConditionHolyAura)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(attributeModifierAuraBonus)
                .AddToDB();

            conditionAuraBonus.forceTurnOccurence = true;

            var powerAura = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Aura")
                .SetGuiPresentation(Category.Feature)
                .SetUsesFixed(ActivationTime.Permanent)
                .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                .SetShowCasting(false)
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
                                .SetConditionForm(conditionAuraBonus, ConditionForm.ConditionOperation.Add, true, true)
                                .Build()
                        )
                        .Build())
                .AddToDB();
            var conditionWildSurgeAura = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Aura")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetFeatures(powerAura)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
                .AddToDB();

            return new WildSurgeEffect { name = "Aura", condition = conditionWildSurgeAura, power = powerAura };
        }

        private static WildSurgeEffect BuildWildSurgeGrowth()
        {
            var proxyGrowth = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxySpikeGrowth, $"Proxy{Name}Growth")
                .SetOrUpdateGuiPresentation($"Proxy{Name}Growth", Category.Proxy)
                .AddToDB();

            var powerGrowth = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Growth")
                .SetGuiPresentationNoContent(true)
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
            var featureGrowth = FeatureDefinitionMovementAffinityBuilder
                .Create($"Feature{Name}Growth")
                .SetGuiPresentationNoContent(true)
                .SetImmunities(false, false, true)
                .AddCustomSubFeatures(growthHandler)
                .AddToDB();

            var conditionWildSurgeGrowth = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Growth")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(featureGrowth)
                .AddToDB();

            return new WildSurgeEffect
            {
                name = "Growth", condition = conditionWildSurgeGrowth, reactPower = powerGrowth
            };
        }

        private static WildSurgeEffect BuildWildSurgeBolt()
        {
            var actionAffinityBolt = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Bolt")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeBolt)
                .AddToDB();

            var actionAffinityBoltFree = FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}BoltFree")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes()
                .SetAuthorizedActions((Id)ExtraActionId.WildSurgeBoltFree)
                .AddToDB();

            var conditionWildSurgeBolt = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}Bolt")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
                .SetConditionType(ConditionType.Beneficial)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                    ConditionInterruption.RageStop)
                .SetPossessive()
                .SetFeatures(actionAffinityBolt)
                .AddToDB();

            var conditionWildSurgeBoltFree = ConditionDefinitionBuilder
                .Create($"{ConditionWildSurgePrefix}BoltFree")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(actionAffinityBoltFree)
                .AddToDB();

            var powerBolt = FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Bolt")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.GuidingBolt)
                .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, 1,
                            (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                        .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                        .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            AttributeDefinitions.Constitution, 8)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(ConditionDefinitions.ConditionBlinded,
                                    ConditionForm.ConditionOperation.Add)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(conditionWildSurgeBoltFree, ConditionForm.ConditionOperation.Remove,
                                    true, true)
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .Build())
                        .UseQuickAnimations()
                        .SetParticleEffectParameters(SpellDefinitions.GuidingBolt)
                        .Build())
                .AddToDB();

            var actionWildSurgeBolt = ActionDefinitionBuilder.Create("WildSurgeBolt")
                .SetGuiPresentation(Category.Action, SpellDefinitions.GuidingBolt, 20)
                .SetActionId(ExtraActionId.WildSurgeBolt)
                .SetActionType(ActionType.Bonus)
                .SetFormType(ActionFormType.Large)
                .RequiresAuthorization()
                .SetActionScope(ActionScope.Battle)
                .OverrideClassName("UsePower")
                .SetActivatedPower(powerBolt)
                .AddToDB();

            ActionDefinitionBuilder
                .Create(actionWildSurgeBolt, "WildSurgeBoltFree")
                .SetActionId(ExtraActionId.WildSurgeBoltFree)
                .SetActionType(ActionType.NoCost)
                .AddToDB();

            return new WildSurgeEffect
            {
                name = "Bolt",
                reactPower = powerBolt,
                conditionFirstTurn = conditionWildSurgeBoltFree,
                condition = conditionWildSurgeBolt
            };
        }

        public IEnumerator HandleWildSurge(GameLocationCharacter character, GameLocationCharacter attacker = null)
        {
            if (character == null)
            {
                yield break;
            }

            var rulesetCharacter = character.RulesetCharacter;

            var cursorService = ServiceRepository.GetService<ICursorService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            List<int> dieRoll = [1];
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var reactingOutOfTurn = battleManager?.Battle?.ActiveContender != character && attacker != null;

            if (rulesetCharacter.HasAnyFeature(featureControlledSurge))
            {
                yield return HandleControlledSurge(character, dieRoll);
            }
            else
            {
                dieRoll[0] =
                    rulesetCharacter.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);
            }

            var wildSurgeEffect = wildSurgeEffects.ElementAt(dieRoll[0] - 1);
            if (wildSurgeEffect.condition != null)
            {
                var existingCondition = GetExistingWildSurgeCondition(rulesetCharacter);
                if (existingCondition?.Name == wildSurgeEffect.condition.Name)
                {
                }
                else
                {
                    if (existingCondition != null)
                    {
                        rulesetCharacter.RemoveAllConditionsOfType(existingCondition.Name);
                    }

                    rulesetCharacter.InflictCondition(
                        wildSurgeEffect.condition.Name,
                        DurationType.Minute,
                        1,
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
            else
            {
                RemoveExistingWildSurgeCondition(rulesetCharacter);
            }

            if (wildSurgeEffect.conditionFirstTurn != null && !reactingOutOfTurn)
            {
                rulesetCharacter.InflictCondition(
                    wildSurgeEffect.conditionFirstTurn.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    character.RulesetCharacter.Guid,
                    character.RulesetCharacter.CurrentFaction.Name,
                    1,
                    wildSurgeEffect.conditionFirstTurn.Name,
                    0,
                    0,
                    0);
            }

            var power =
                reactingOutOfTurn ? wildSurgeEffect.reactPower ?? wildSurgeEffect.power : wildSurgeEffect.power;

            if (power != null)
            {
                var usablePower = PowerProvider.Get(power, rulesetCharacter);

                if (power.EffectDescription.TargetType == TargetType.Position)
                {
                    var actionParams = new CharacterActionParams(character, Id.PowerNoCost);
                    actionParams.RulesetEffect =
                        implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, true);
                    if (reactingOutOfTurn)
                    {
                        ResetCamera();
                        PreventEnemyAction(attacker, rulesetCharacter);

                        cursorService.ActivateCursor<CursorLocationSelectPosition>([actionParams]);
                        while (reactingOutOfTurn && cursorService.CurrentCursor is CursorLocationSelectPosition)
                        {
                            yield return null;
                        }
                    }
                }
                else if (power.EffectDescription.TargetType == TargetType.Individuals
                         || power.EffectDescription.TargetType == TargetType.IndividualsUnique)
                {
                    if (reactingOutOfTurn)
                    {
                        var actionParams = new CharacterActionParams(character, Id.SpendPower);
                        actionParams.RulesetEffect =
                            implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, false);
                        actionParams.IsReactionEffect = true;
                        actionParams.TargetCharacters.Add(attacker);
                        actionParams.ActionModifiers.Add(new ActionModifier());
                        ServiceRepository.GetService<ICommandService>()?.ExecuteInstantSingleAction(actionParams);
                    }
                }
                else if (power.EffectDescription.TargetType == TargetType.Cube &&
                         power.EffectDescription.RangeType == RangeType.Distance)
                {
                    var actionParams = new CharacterActionParams(character, Id.PowerNoCost);
                    actionParams.RulesetEffect =
                        implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, false);
                    actionParams.IsReactionEffect = true;
                    if (reactingOutOfTurn)
                    {
                        ResetCamera();
                        PreventEnemyAction(attacker, rulesetCharacter);
                    }

                    cursorService.ActivateCursor<CursorLocationGeometricShape>([actionParams]);
                    while (reactingOutOfTurn && cursorService.CurrentCursor is CursorLocationGeometricShape)
                    {
                        yield return null;
                    }
                }
                else
                {
                    if (reactingOutOfTurn)
                    {
                        if (power.ActivationTime == ActivationTime.Permanent)
                        {
                            if (!rulesetCharacter.IsPowerActive(usablePower))
                            {
                                PreventEnemyAction(attacker, rulesetCharacter);
                                var actionParams = new CharacterActionParams(character, Id.PowerNoCost);
                                actionParams.RulesetEffect =
                                    implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, false);
                                ServiceRepository.GetService<ICommandService>()
                                    ?.ExecuteAction(actionParams, null, true);
                            }
                        }
                        else
                        {
                            var actionParams = new CharacterActionParams(character, Id.PowerNoCost);
                            actionParams.RulesetEffect =
                                implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, false);
                            actionParams.IsReactionEffect = true;
                            ServiceRepository.GetService<ICommandService>()?.ExecuteInstantSingleAction(actionParams);
                        }
                    }
                    else
                    {
                        var actionParams = new CharacterActionParams(character, Id.PowerNoCost);
                        actionParams.RulesetEffect =
                            implementationManager.InstantiateEffectPower(rulesetCharacter, usablePower, false);
                        ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, true);
                    }
                }
            }
        }

        private void PreventEnemyAction(GameLocationCharacter attacker, RulesetCharacter source)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                conditionPreventAction.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                source.guid,
                source.CurrentFaction.Name,
                1,
                conditionPreventAction.Name,
                0,
                0,
                0);
        }

        private IEnumerator HandleControlledSurge(GameLocationCharacter character, List<int> result)
        {
            var rulesetAttacker = character.RulesetCharacter;
            var firstRoll =
                rulesetAttacker.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);
            var secondRoll =
                rulesetAttacker.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var myUsablePowers = new List<RulesetUsablePower>();
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var usablePowerPool = PowerProvider.Get(powerPool, rulesetAttacker);

            myUsablePowers.Add(usablePowerPool);
            if (firstRoll == secondRoll)
            {
                foreach (var power in powers)
                {
                    myUsablePowers.Add(PowerProvider.Get(power, rulesetAttacker));
                }
            }
            else
            {
                myUsablePowers.Add(PowerProvider.Get(powers[firstRoll - 1], rulesetAttacker));
                myUsablePowers.Add(PowerProvider.Get(powers[secondRoll - 1], rulesetAttacker));
            }

            var usablePowersOrig = rulesetAttacker.usablePowers;
            rulesetAttacker.usablePowers = myUsablePowers;

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPool, rulesetAttacker);
            var actionParams =
                new CharacterActionParams(GameLocationCharacter.GetFromActor(rulesetAttacker), Id.SpendPower)
                {
                    StringParameter = "ControlledSurge",
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                    UsablePower = usablePower,
                    targetCharacters = [character]
                };
            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(character, actionManager, count);

            rulesetAttacker.usablePowers = usablePowersOrig;

            if (reactionRequest.Validated && reactionRequest.SelectedSubOption >= 0)
            {
                result[0] = reactionRequest.SelectedSubOption + 1;
            }
            else
            {
                result[0] = firstRoll;
            }

            yield return null;
        }

        private static void ResetCamera()
        {
            var viewLocationContextualManager =
                ServiceRepository.GetService<IViewLocationContextualService>() as ViewLocationContextualManager;

            if (!viewLocationContextualManager)
            {
                return;
            }

            if (viewLocationContextualManager.rangeAttackDirector.state == PlayState.Playing)
            {
                viewLocationContextualManager.rangeAttackDirector.Stop();
                viewLocationContextualManager.ContextualSequenceEnd?.Invoke();
            }

            // ReSharper disable once InvertIf
            if (viewLocationContextualManager.meleeAttackDirector.state == PlayState.Playing)
            {
                viewLocationContextualManager.meleeAttackDirector.Stop();
                viewLocationContextualManager.ContextualSequenceEnd?.Invoke();
            }
        }

        private void RemoveExistingWildSurgeCondition(RulesetCharacter character)
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
                if (wildEffect.condition != null &&
                    character.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, wildEffect.condition.Name))
                {
                    return wildEffect.condition;
                }
            }

            return null;
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

                foreach (var proxy in locationCharacter.RulesetCharacter.controlledEffectProxies)
                {
                    if (proxy?.EffectProxyDefinition?.Name != powerSummon?.Name || proxy?.ControllerGuid == null)
                    {
                        continue;
                    }

                    if (RulesetEntity.TryGetEntity<RulesetCharacter>(proxy.ControllerGuid, out var controller))
                    {
                        var service = ServiceRepository.GetService<IRulesetImplementationService>();
                        service.AutoTriggerProxy(proxy, controller);
                    }
                }
            }
        }

        private sealed class WildSurgeWeaponModifyAttackMode :
            IModifyWeaponAttackMode
        {
            public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
            {
                if (ValidatorsWeapon.IsMelee(attackMode))
                {
                    attackMode.AddAttackTagAsNeeded(TagsDefinitions.WeaponTagThrown);
                    attackMode.thrown = true;
                    attackMode.closeRange = 4;
                    attackMode.maxRange = 12;
                }

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

        private sealed class WildSurgeGrowthOnTurnEnd(FeatureDefinitionPower power) : ICharacterBeforeTurnEndListener
        {
            public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
            {
                var rulesetCharacter = locationCharacter.RulesetCharacter;
                var actionParams = new CharacterActionParams(locationCharacter, Id.PowerBonus);
                var usablePower = PowerProvider.Get(power, rulesetCharacter);
                actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectPower(rulesetCharacter, usablePower, false);
                actionParams.SkipAnimationsAndVFX = true;
                ServiceRepository.GetService<ICommandService>()?
                    .ExecuteInstantSingleAction(actionParams);
            }
        }
    }

    #endregion

    #region Bolstering Magic

    private static FeatureDefinition BuildFeatureBolsteringMagic()
    {
        var powerBolsteringMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BolsteringMagic")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerBolsteringMagic", Resources.PowerBolsteringMagic, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        // Bolstering Magic: Roll

        var abilityCheckAffinityBolsteringMagicRoll = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}BolsteringMagicRoll")
            .SetGuiPresentationNoContent(true)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D3, 1,
                abilityProficiencyPairs:
                [
                    (AttributeDefinitions.Strength, ""),
                    (AttributeDefinitions.Dexterity, ""),
                    (AttributeDefinitions.Wisdom, ""),
                    (AttributeDefinitions.Constitution, ""),
                    (AttributeDefinitions.Intelligence, ""),
                    (AttributeDefinitions.Charisma, "")
                ]
            )
            .AddToDB();

        var combatAffinityBolsteringMagicRoll = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}BolsteringMagicRoll")
            .SetGuiPresentationNoContent(true)
            .SetMyAttackModifier((ExtraCombatAffinityValueDetermination)CombatAffinityValueDetermination.Die)
            .SetMyAttackModifierDieType(DieType.D3)
            .SetMyAttackModifierSign(AttackModifierSign.Add)
            .AddToDB();

        var conditionBolsteringMagicRoll = ConditionDefinitionBuilder
            .Create($"Condition{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .SetFeatures(abilityCheckAffinityBolsteringMagicRoll, combatAffinityBolsteringMagicRoll)
            .SetPossessive()
            .AddToDB();

        var powerBolsteringMagicRoll = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectEffectParameters(SpellDefinitions.Aid.EffectDescription.effectParticleParameters
                        .effectParticleReference)
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBolsteringMagicRoll))
                    .Build())
            .AddToDB();

        // Bolstering Magic: Spell
        var db = DatabaseRepository.GetDatabase<FeatureDefinitionMagicAffinity>();
        var conditions = new ConditionDefinition[3];

        for (var i = 1; i <= 3; i++)
        {
            conditions[i - 1] = ConditionDefinitionBuilder
                .Create($"Condition{Name}BolsteringMagicSpell{i}")
                .SetGuiPresentation($"Condition{Name}BolsteringMagicSpell", Category.Condition)
                .SetSilent(Silent.WhenRemoved)
                .SetFeatures(db.GetElement($"MagicAffinityAdditionalSpellSlot{i}"))
                .SetPossessive()
                .AddToDB();
        }

        var powerBolsteringMagicSpell = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicSpell")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectEffectParameters(SpellDefinitions.Aid.EffectDescription.effectParticleParameters
                        .effectParticleReference)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditions[0],
                                ConditionForm.ConditionOperation.AddRandom,
                                false, false, conditions)
                            .Build())
                    .Build()
            )
            .AddToDB();

        powerBolsteringMagicSpell.AddCustomSubFeatures(
            new FilterTargetingCharacterBolsteringMagicSpell(conditions));

        PowerBundle.RegisterPowerBundle(powerBolsteringMagic, false,
            powerBolsteringMagicRoll, powerBolsteringMagicSpell);

        var featureSetBolsteringMagic = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BolsteringMagic")
            .SetGuiPresentation($"Power{Name}BolsteringMagic", Category.Feature)
            .SetFeatureSet(powerBolsteringMagic, powerBolsteringMagicRoll, powerBolsteringMagicSpell)
            .AddToDB();

        return featureSetBolsteringMagic;
    }

    private sealed class FilterTargetingCharacterBolsteringMagicSpell(ConditionDefinition[] conditions)
        : IFilterTargetingCharacter
    {
        private readonly string[] _conditionNames = conditions.Select(x => x.Name).ToArray();
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var hero = GetOriginalHero(target);
            if (hero == null)
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}NotAHero");
                return false;
            }

            if (hero.HasAnyConditionOfType(_conditionNames))
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}AlreadyBolstered");
                return false;
            }

            if (!hero.CanCastSpells())
            {
                __instance.actionModifier.FailureFlags.Add($"Tooltip/&{Name}CannotCastSpells");
                return false;
            }

            return true;
        }

        private RulesetCharacterHero GetOriginalHero(GameLocationCharacter targetCharacter)
        {
            return targetCharacter.RulesetCharacter switch
            {
                RulesetCharacterHero hero => hero,
                RulesetCharacterMonster monster when monster.originalFormCharacter is RulesetCharacterHero originalHero
                    => originalHero,
                _ => null
            };
        }
    }

    #endregion

    #region Unstable Backlash

    private static FeatureDefinition BuildFeatureUnstableBacklash(WildSurgeHandler wildSurgeHandler)
    {
        var powerUnstableBackslash = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UnstableBacklash")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ReactionTriggerContext.DamagedWithinRange)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.Individuals)
                .SetNoSavingThrow()
                .UseQuickAnimations()
                .SetDurationData(DurationType.Round)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDummy)
                )
                .Build())
            .AddToDB();
        powerUnstableBackslash.AddCustomSubFeatures(new UnstableBackslashHandler(wildSurgeHandler,
            powerUnstableBackslash));

        var conditionUnstableBacklash = ConditionDefinitionBuilder
            .Create($"Condition{Name}UnstableBacklash")

            .SetGuiPresentationNoContent(true)
            .SetFeatures(powerUnstableBackslash)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialDuration(DurationType.UntilLongRest)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.NoAttackOrDamagedInTurn,
                ConditionInterruption.RageStop)
            .AddToDB();

        var featureUnstableBacklash = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}UnstableBackslash")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                .SetNoSavingThrow()
                .SetDurationData(DurationType.UntilLongRest)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(conditionUnstableBacklash)
                )
                .Build())
            .AddToDB();
        return featureUnstableBacklash;
    }

    private sealed class UnstableBackslashHandler(
        WildSurgeHandler wildSurgeHandler,
        FeatureDefinitionPower powerUnstableBackslash)
        : IActionFinishedByContender, IActionFinishedByMe
    {
        private const string TagUnstableBacklash = "UnstableBacklash";

        public IEnumerator OnActionFinishedByContender(CharacterAction characterAction, GameLocationCharacter target)
        {
            if (target == characterAction.ActingCharacter)
            {
                yield break;
            }

            if (!target.UsedSpecialFeatures.ContainsKey(TagUnstableBacklash))
            {
                yield break;
            }

            target.UsedSpecialFeatures.Remove(TagUnstableBacklash);
            yield return wildSurgeHandler.HandleWildSurge(target, characterAction.ActingCharacter);
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction.ActionType != ActionType.Reaction)
            {
                yield break;
            }

            if (characterAction is not CharacterActionUsePower characterPowerAction)
            {
                yield break;
            }

            if (characterPowerAction.activePower.PowerDefinition != powerUnstableBackslash)
            {
                yield break;
            }

            characterAction.ActingCharacter.UsedSpecialFeatures.TryAdd(TagUnstableBacklash, 0);
        }
    }

    #endregion
}
