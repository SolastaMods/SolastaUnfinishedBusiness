using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;


namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerLightBearer : AbstractSubclass
{
    internal const string Name = "RangerLightBearer";

    public RangerLightBearer()
    {
        // LEVEL 03

        // Lightbearer Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Bless),
                BuildSpellGroup(5, BrandingSmite),
                BuildSpellGroup(9, SpellsContext.BlindingSmite),
                BuildSpellGroup(13, GuardianOfFaith),
                BuildSpellGroup(17, SpellsContext.BanishingSmite))
            .AddToDB();

        var powerLight = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Light")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerLight", Resources.PowerLight, 256, 128))
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(Light.EffectDescription)
            .AddToDB();

        var featureSetLight = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Light")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerLight)
            .AddToDB();

        // Blessed Warrior

        var conditionBlessedWarrior = ConditionDefinitionBuilder
            .Create($"Condition{Name}BlessedWarrior")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionMarkedByHunter)
            .AddToDB();

        var powerBlessedWarrior = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlessedWarrior")
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerBlessedWarrior", Resources.PowerBlessedWarrior, 256, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBlessedWarrior, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new PhysicalAttackInitiatedByMeBlessedWarrior(conditionBlessedWarrior))
            .AddToDB();

        // Lifebringer

        var attributeModifierLifeBringerBase = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}LifeBringerBase")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                AttributeModifierOperation.Set,
                AttributeDefinitions.HealingPool)
            .AddToDB();

        var attributeModifierLifeBringerAdditive = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}LifeBringerAdditive")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                AttributeModifierOperation.Additive,
                AttributeDefinitions.HealingPool, 1)
            .AddToDB();

        var powerLifeBringer = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerPaladinLayOnHands, $"Power{Name}LifeBringer")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerLifeBringer", Resources.PowerLifeBringer, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.HealingPool, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerPaladinLayOnHands.EffectDescription)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Blessed Glow

        var powerBlessedGlow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlessedGlow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 4)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionBlinded,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .Build())
            .AddToDB();

        powerBlessedGlow.EffectDescription.savingThrowAffinitiesByFamily = new List<SaveAffinityByFamilyDescription>
        {
            new() { advantageType = AdvantageType.Disadvantage, family = "Fiend" },
            new() { advantageType = AdvantageType.Disadvantage, family = "Undead" }
        };

        var powerLightEnhanced = FeatureDefinitionPowerBuilder
            .Create(powerLight, $"Power{Name}LightEnhanced")
            .SetOverriddenPower(powerLight)
            .AddToDB();

        powerLightEnhanced.SetCustomSubFeatures(
            new MagicEffectFinishedByMeBlessedGlow(powerBlessedGlow, powerLightEnhanced));

        var featureSetBlessedGlow = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BlessedGlow")
            .SetGuiPresentation($"Power{Name}BlessedGlow", Category.Feature)
            .AddFeatureSet(powerLightEnhanced, powerBlessedGlow)
            .AddToDB();

        // LEVEL 11

        // Angelic Form

        var conditionAngelicForm = ConditionDefinitionBuilder
            .Create($"Condition{Name}AngelicForm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShine)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{Name}AngelicForm")
                    .SetGuiPresentationNoContent(true)
                    .SetMagicalWeapon()
                    .AddToDB())
            .AddToDB();

        var powerAngelicFormSprout = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicFormSprout")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerAngelicFormSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAngelicForm,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerAngelicFormSprout.SetCustomSubFeatures(new ActionFinishedByMeAngelicForm(powerAngelicFormSprout));

        var powerAngelicFormDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicFormDismiss")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerAngelicFormDismiss", Resources.PowerAngelicFormDismiss, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAngelicForm,
                                ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(ValidatorsCharacter.HasAnyOfConditions(conditionAngelicForm.Name)))
            .AddToDB();

        var featureSetAngelicForm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AngelicForm")
            .SetGuiPresentation($"Power{Name}AngelicFormSprout", Category.Feature)
            .AddFeatureSet(powerAngelicFormSprout, powerAngelicFormDismiss)
            .AddToDB();

        // LEVEL 15

        // Warding Light

        var actionAffinityWardingLight = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}WardingLight")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(Id.BlockAttack)
            .AddToDB();

        var featureWardingLight = FeatureDefinitionBuilder
            .Create($"Feature{Name}WardingLight")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new PhysicalAttackInitiatedOnMeOrAllyWardingLight())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerLightBearer, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetLight,
                powerBlessedWarrior,
                attributeModifierLifeBringerBase,
                attributeModifierLifeBringerAdditive,
                powerLifeBringer)
            .AddFeaturesAtLevel(7,
                featureSetBlessedGlow)
            .AddFeaturesAtLevel(11,
                featureSetAngelicForm)
            .AddFeaturesAtLevel(15,
                actionAffinityWardingLight,
                featureWardingLight)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Blessed Warrior
    //

    private sealed class PhysicalAttackInitiatedByMeBlessedWarrior : IPhysicalAttackInitiatedByMe
    {
        private readonly ConditionDefinition _conditionDefinition;

        public PhysicalAttackInitiatedByMeBlessedWarrior(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetDefender.HasAnyConditionOfType(_conditionDefinition.Name))
            {
                yield break;
            }

            var effectDescription = attackMode.EffectDescription;

            foreach (var damageForm in effectDescription.EffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                damageForm.DamageForm.damageType = DamageTypeRadiant;
            }

            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            // add additional radiant dice
            var classLevel = attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);
            var diceNumber = classLevel < 11 ? 1 : 2;
            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeRadiant, diceNumber, DieType.D8)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);

            // remove condition on successful attack
            var rulesetCondition =
                rulesetDefender.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

            if (rulesetCondition != null)
            {
                rulesetDefender.RemoveCondition(rulesetCondition);
            }
        }
    }

    //
    // Blessed Glow
    //

    private class MagicEffectFinishedByMeBlessedGlow : IUsePowerFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerBlessedGlow;
        private readonly FeatureDefinitionPower _powerLightEnhanced;

        public MagicEffectFinishedByMeBlessedGlow(
            FeatureDefinitionPower featureDefinitionPower,
            FeatureDefinitionPower powerLightEnhanced)
        {
            _powerBlessedGlow = featureDefinitionPower;
            _powerLightEnhanced = powerLightEnhanced;
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power != _powerLightEnhanced)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerCharges(_powerBlessedGlow) <= 0)
            {
                yield break;
            }

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(attacker, (Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = "Reaction/&CustomReactionBlessedGlowDescription"
                };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("BlessedGlow", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(_powerBlessedGlow, _powerBlessedGlow.CostPerUse);

            rulesetAttacker.LogCharacterUsedPower(_powerBlessedGlow);

            var usablePower = UsablePowersProvider.Get(_powerBlessedGlow, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            // was expecting 4 (20 ft) to work but game is odd on distance calculation so used 5
            foreach (var enemy in gameLocationBattleService.Battle.EnemyContenders
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .Where(enemy => rulesetAttacker.DistanceTo(enemy.RulesetActor) <= 5)
                         .ToList()) // avoid changing enumerator
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }

    //
    // Angelic Form
    //

    private sealed class ActionFinishedByMeAngelicForm : IUsePowerFinishedByMe
    {
        private static FeatureDefinitionPower _featureDefinitionPower;

        public ActionFinishedByMeAngelicForm(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power != _featureDefinitionPower)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                classLevel, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }
    }

    //
    // Warding Light
    //

    private sealed class PhysicalAttackInitiatedOnMeOrAllyWardingLight : IPhysicalAttackInitiatedOnMeOrAlly
    {
        public IEnumerator OnAttackInitiatedOnMeOrAlly(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter ally,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            if (__instance.battle == null)
            {
                yield break;
            }

            yield return __instance.battle
                .GetOpposingContenders(attacker.Side)
                .Where(opposingContender =>
                    opposingContender != defender &&
                    opposingContender.CanReact() &&
                    __instance.IsWithinXCells(opposingContender, defender, 6) &&
                    opposingContender.GetActionStatus(Id.BlockAttack, ActionScope.Battle, ActionStatus.Available) ==
                    ActionStatus.Available)
                .ToList() // avoid enumerator changes
                .Select(opposingContender => __instance.PrepareAndReact(
                    opposingContender, attacker, attacker, Id.BlockAttack, attackModifier,
                    additionalTargetCharacter: defender))
                .GetEnumerator();
        }
    }
}
