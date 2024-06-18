using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfAudacity : AbstractSubclass
{
    private const string Name = "CollegeOfAudacity";
    private const string WhirlMarker = "Whirl";

    private const ActionDefinitions.Id AudaciousWhirlToggle = (ActionDefinitions.Id)ExtraActionId.AudaciousWhirlToggle;
    private const ActionDefinitions.Id MasterfulWhirlToggle = (ActionDefinitions.Id)ExtraActionId.MasterfulWhirlToggle;

    public CollegeOfAudacity()
    {
        // LEVEL 03

        // Bonus Proficiencies

        var magicAffinityWeaponAsFocus = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetHandsFullCastingModifiers(false, false, true)
            .AddToDB();

        var proficiencyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Armor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory)
            .AddToDB();

        var proficiencyScimitar = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Scimitar")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon, ScimitarType.Name)
            .AddToDB();

        var featureSetBonusProficiencies = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BonusProficiencies")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(magicAffinityWeaponAsFocus, proficiencyArmor, proficiencyScimitar)
            .AddToDB();

        // Fighting Style

        var proficiencyFightingStyle = FeatureDefinitionFightingStyleChoiceBuilder
            .Create($"FightingStyleChoice{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetFightingStyles("Dueling", "TwoWeapon")
            .AddToDB();

        // Defensive Whirl

        var attributeModifierDefensiveWhirl = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DefensiveWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
            .AddToDB();

        var conditionDefensiveWhirl = ConditionDefinitionBuilder
            .Create($"Condition{Name}DefensiveWhirl")
            .SetGuiPresentation($"AttributeModifier{Name}DefensiveWhirl", Category.Feature, Global.Empty,
                ConditionDefinitions.ConditionMagicallyArmored.GuiPresentation.SpriteReference)
            .SetPossessive()
            .SetFeatures(attributeModifierDefensiveWhirl)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .AddToDB();

        var powerDefensiveWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DefensiveWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Mobile Whirl

        var powerMobileWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MobileWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Slashing Whirl

        var powerSlashingWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SlashingWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Audacious Whirl

        var actionAffinityAudaciousWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityAudaciousWhirlToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(AudaciousWhirlToggle)
            .AddToDB();

        var movementAffinityAudaciousWhirl = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}AudaciousWhirl")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var conditionAudaciousWhirlExtraMovement = ConditionDefinitionBuilder
            .Create($"Condition{Name}AudaciousWhirlExtraMovement")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(movementAffinityAudaciousWhirl)
            .AddToDB();

        var powerAudaciousWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AudaciousWhirl")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitMelee, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerAudaciousWhirl.AddCustomSubFeatures(
            new CustomBehaviorWhirl(
                conditionAudaciousWhirlExtraMovement,
                conditionDefensiveWhirl,
                powerDefensiveWhirl,
                powerSlashingWhirl,
                powerMobileWhirl),
            ReactionResourceBardicInspiration.Instance,
            new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                attacker.OnceInMyTurnIsValid(WhirlMarker) &&
                attacker.RulesetCharacter.IsToggleEnabled(AudaciousWhirlToggle)));

        PowerBundle.RegisterPowerBundle(
            powerAudaciousWhirl,
            true,
            powerDefensiveWhirl,
            powerSlashingWhirl,
            powerMobileWhirl);

        var featureSetAudaciousWhirl = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AudaciousWhirl")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerDefensiveWhirl,
                powerSlashingWhirl,
                powerMobileWhirl,
                powerAudaciousWhirl,
                actionAffinityAudaciousWhirlToggle)
            .AddToDB();

        // LEVEL 14

        // Masterful Whirl

        var powerMasterfulWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MasterfulWhirl")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitMelee)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerMasterfulWhirl.AddCustomSubFeatures(
            new CustomBehaviorWhirl(
                conditionAudaciousWhirlExtraMovement,
                conditionDefensiveWhirl,
                powerDefensiveWhirl,
                powerSlashingWhirl,
                powerMobileWhirl),
            new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                attacker.OnceInMyTurnIsValid(WhirlMarker) &&
                attacker.RulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle)));

        var actionAffinityMasterfulWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityMasterfulWhirlToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(MasterfulWhirlToggle)
            .AddToDB();

        var featureSetMasterfulWhirl = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterfulWhirl")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityMasterfulWhirlToggle, powerMasterfulWhirl)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, featureSetBonusProficiencies, proficiencyFightingStyle, featureSetAudaciousWhirl)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(14, featureSetMasterfulWhirl)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorWhirl(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionExtraMovement,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefensiveWhirl,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerDefensiveWhirl,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerSlashingWhirl,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerMobileWhirl)
        : IActionFinishedByMe, IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private readonly List<string> _tags = [];
        private bool _criticalHit;
        private string _damageType;

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (_damageType == null)
            {
                yield break;
            }

            if (characterAction is not CharacterActionSpendPower actionSpendPower)
            {
                yield break;
            }

            var power = actionSpendPower.activePower.PowerDefinition;

            if (power != powerDefensiveWhirl &&
                power != powerSlashingWhirl &&
                power != powerMobileWhirl)
            {
                yield break;
            }

            var actingCharacter = actionSpendPower.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            actingCharacter.UsedSpecialFeatures.TryAdd(WhirlMarker, 1);

            // targets
            var targetCharacters = new List<GameLocationCharacter>();

            // masterful whirl?
            var isMasterfulWhirl = rulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle);

            // damage roll
            var dieType = isMasterfulWhirl ? DieType.D6 : rulesetCharacter.GetBardicInspirationDieValue();
            var damageForm = new DamageForm
            {
                DamageType = _damageType, DieType = dieType, DiceNumber = 1, BonusDamage = 0
            };
            var rolls = new List<int>();
            var damageRoll =
                rulesetCharacter.RollDamage(damageForm, 0, _criticalHit, 0, 0, 1, false, false, false, rolls);

            // add damage whirl condition and target
            if (power == powerDefensiveWhirl)
            {
                targetCharacters.Add(actionSpendPower.ActionParams.TargetCharacters[0]);

                var firstRoll = rolls[0];

                rulesetCharacter.InflictCondition(
                    conditionDefensiveWhirl.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    conditionDefensiveWhirl.Name,
                    firstRoll,
                    0,
                    0);
            }

            // add mobile whirl condition and target
            else if (power == powerMobileWhirl)
            {
                targetCharacters.Add(actionSpendPower.ActionParams.TargetCharacters[0]);

                rulesetCharacter.InflictCondition(
                    ConditionDisengaging,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    // all disengaging in game is set under TagCombat (why?)
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    ConditionDisengaging,
                    0,
                    0,
                    0);
            }

            // add slashing whirl targets
            else if (power == powerSlashingWhirl)
            {
                if (Gui.Battle != null)
                {
                    targetCharacters.AddRange(Gui.Battle
                        .GetContenders(actingCharacter, hasToPerceiveTarget: true, withinRange: 1));
                }
            }

            // apply damage to targets
            var isFirstTarget = true;

            foreach (var defender in targetCharacters)
            {
                if (isFirstTarget)
                {
                    isFirstTarget = false;
                }
                // reroll for each target on Slashing Whirl scenario
                else
                {
                    rolls = [];
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, _criticalHit, 0, 0, 1, false, false, false, rolls);
                }

                var rulesetDefender = defender.RulesetActor;
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetCharacter,
                    targetCharacter = rulesetDefender,
                    position = defender.LocationPosition
                };

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    _damageType,
                    applyFormsParams,
                    rulesetDefender,
                    false,
                    rulesetCharacter.Guid,
                    false,
                    _tags,
                    new RollInfo(dieType, rolls, 0),
                    false,
                    out _);
            }

            // consume bardic inspiration if not a masterful whirl
            if (isMasterfulWhirl)
            {
                yield break;
            }

            rulesetCharacter.UsedBardicInspiration++;
            rulesetCharacter.BardicInspirationAltered?.Invoke(
                rulesetCharacter, rulesetCharacter.RemainingBardicInspirations);
        }

        // collect damage type
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            _damageType = damageForm?.damageType;
            _criticalHit = criticalHit;
            _tags.SetRange(attackMode.AttackTags);

            yield break;
        }

        // add extra movement on any attack
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionExtraMovement.Name))
            {
                rulesetCharacter.InflictCondition(
                    conditionExtraMovement.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    attacker.RulesetCharacter.guid,
                    attacker.RulesetCharacter.CurrentFaction.Name,
                    1,
                    conditionExtraMovement.Name,
                    0,
                    0,
                    0);
            }
        }
    }
}
