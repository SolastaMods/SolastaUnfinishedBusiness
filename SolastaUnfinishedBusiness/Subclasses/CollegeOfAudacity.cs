using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfAudacity : AbstractSubclass
{
    private const string Name = "CollegeOfAudacity";
    private const string ConditionDefensiveWhirl = $"Condition{Name}DefensiveWhirl";
    private const ActionDefinitions.Id AudaciousWhirlToggle = (ActionDefinitions.Id)ExtraActionId.AudaciousWhirlToggle;
    private const ActionDefinitions.Id MasterfulWhirlToggle = (ActionDefinitions.Id)ExtraActionId.MasterfulWhirlToggle;
    private const string WhirlDamage = "WhirlDamage";

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

        _ = ConditionDefinitionBuilder
            .Create(ConditionDefensiveWhirl)
            .SetGuiPresentation($"AttributeModifier{Name}DefensiveWhirl", Category.Feature, Gui.EmptyContent,
                ConditionDefinitions.ConditionMagicallyArmored.GuiPresentation.SpriteReference)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}DefensiveWhirl")
                    .SetGuiPresentation(Category.Feature)
                    .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .AddToDB();

        var powerDefensiveWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DefensiveWhirl")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        // Mobile Whirl

        var powerMobileWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MobileWhirl")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        // Slashing Whirl

        var powerSlashingWhirlDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SlashingWhirlDamage")
            .SetGuiPresentation($"Power{Name}SlashingWhirl", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1, DieType.D6))
                    .Build())
            .AddToDB();

        var powerSlashingWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SlashingWhirl")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
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
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerAudaciousWhirl.AddCustomSubFeatures(
            ReactionResourceBardicInspiration.Instance,
            new CustomBehaviorAudaciousWhirl(
                powerAudaciousWhirl, powerSlashingWhirl, powerSlashingWhirlDamage,
                conditionAudaciousWhirlExtraMovement));

        PowerBundle.RegisterPowerBundle(
            powerAudaciousWhirl,
            false,
            powerDefensiveWhirl,
            powerSlashingWhirl,
            powerMobileWhirl);

        _ = ActionDefinitionBuilder
            .Create(MetamagicToggle, "AudaciousWhirlToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.AudaciousWhirlToggle)
            .OverrideClassName("Toggle")
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerAudaciousWhirl)))
            .AddToDB();

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

        var actionAffinityMasterfulWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityMasterfulWhirlToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(MasterfulWhirlToggle)
            .AddToDB();

        var featureSetMasterfulWhirl = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterfulWhirl")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityMasterfulWhirlToggle)
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

    internal static void HandleDefensiveWhirl(RulesetCharacter rulesetCharacter, DamageForm damageForm, int damage)
    {
        if (damageForm.AncestryType != (AncestryType)ExtraAncestryType.CollegeOfAudacityDefensiveWhirl)
        {
            return;
        }

        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

        character.UsedSpecialFeatures.TryAdd(WhirlDamage, 0);
        character.UsedSpecialFeatures[WhirlDamage] = damage;
    }

    private sealed class CustomBehaviorAudaciousWhirl(
        FeatureDefinitionPower powerAudaciousWhirl,
        FeatureDefinitionPower powerSlashingWhirl,
        FeatureDefinitionPower powerSlashingWhirlDamage,
        ConditionDefinition conditionExtraMovement)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyEffectDescription,
            IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private const string WhirlMarker = "WhirlMarker";
        private const string WhirlDamageType = "WhirlDamageType";
        private const string WhirlSelectedPower = "WhirlSelectedPower";

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.SourceDefinition != powerSlashingWhirl)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerSlashingWhirlDamage, rulesetAttacker);
            var targets = Gui.Battle.GetContenders(attacker, withinRange: 1).Where(x => x != defender).ToArray();

            attacker.MyExecuteActionPowerNoCost(usablePower, targets);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerSlashingWhirlDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter rulesetCharacter,
            RulesetEffect rulesetEffect)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (character == null)
            {
                return effectDescription;
            }

            var damageForm = effectDescription.FindFirstDamageForm();

            if (character.UsedSpecialFeatures.TryGetValue(WhirlDamageType, out var damageIndex))
            {
                var damageTypeDefinition =
                    DatabaseRepository.GetDatabase<DamageDefinition>().ToList().ElementAt(damageIndex);

                damageForm.DamageType = damageTypeDefinition.Name;
            }

            var isMasterfulWhirl = rulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle);

            damageForm.OverrideWithBardicInspirationDie = !isMasterfulWhirl;

            return effectDescription;
        }

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
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerAudaciousWhirl, rulesetAttacker);
            var isAudaciousWhirl = rulesetAttacker.IsToggleEnabled(AudaciousWhirlToggle);
            var hasAvailablePowerUses = rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0;
            var isMasterfulWhirl = rulesetAttacker.IsToggleEnabled(MasterfulWhirlToggle);

            if (!attacker.OnceInMyTurnIsValid(WhirlMarker) ||
                // !ValidatorsWeapon.IsMelee(attackMode) ||
                !((isAudaciousWhirl && hasAvailablePowerUses) || isMasterfulWhirl))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(WhirlSelectedPower, -1);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerAudaciousWhirl.Name,
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                attacker.UsedSpecialFeatures[WhirlSelectedPower] = reactionRequest.SelectedSubOption;
                attacker.UsedSpecialFeatures.TryAdd(WhirlMarker, 1);

                var firstDamageForm =
                    actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

                if (firstDamageForm != null)
                {
                    var damageType = firstDamageForm.DamageForm.DamageType;
                    var effectDamageForm = EffectFormBuilder.DamageForm(damageType, 1, DieType.D6);

                    effectDamageForm.DamageForm.OverrideWithBardicInspirationDie = !isMasterfulWhirl;

                    if (reactionRequest.SelectedSubOption == 0)
                    {
                        effectDamageForm.DamageForm.AncestryType =
                            (AncestryType)ExtraAncestryType.CollegeOfAudacityDefensiveWhirl;
                    }

                    var index = actualEffectForms.IndexOf(firstDamageForm);

                    actualEffectForms.Insert(index + 1, effectDamageForm);

                    var damageTypes = DatabaseRepository.GetDatabase<DamageDefinition>().ToList();
                    var damageTypeDefinition = damageTypes.FirstOrDefault(x => x.Name == damageType);
                    var damageIndex = damageTypes.IndexOf(damageTypeDefinition);

                    attacker.UsedSpecialFeatures.TryAdd(WhirlDamageType, damageIndex);
                }

                if (isMasterfulWhirl)
                {
                    return;
                }

                rulesetAttacker.UsedBardicInspiration++;
                rulesetAttacker.BardicInspirationAltered?.Invoke(
                    rulesetAttacker, rulesetAttacker.RemainingBardicInspirations);
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionExtraMovement.Name))
            {
                rulesetAttacker.InflictCondition(
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

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                !attacker.UsedSpecialFeatures.TryGetValue(WhirlSelectedPower, out var value))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.Remove(WhirlSelectedPower);

            switch (value)
            {
                case 0 when
                    attacker.UsedSpecialFeatures.TryGetValue(WhirlDamage, out var damage):
                    var isMasterfulWhirl = rulesetAttacker.IsToggleEnabled(MasterfulWhirlToggle);
                    var dieType = isMasterfulWhirl ? DieType.D6 : rulesetAttacker.GetBardicInspirationDieValue();

                    rulesetAttacker.LogCharacterActivatesAbility(
                        Gui.NoLocalization, "Feedback/&CollegeOfAudacityDefensiveWhirl", true,
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                            (ConsoleStyleDuplet.ParameterType.Positive, damage.ToString())
                        ]);
                    rulesetAttacker.InflictCondition(
                        ConditionDefensiveWhirl,
                        DurationType.Round,
                        1,
                        TurnOccurenceType.StartOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        ConditionDefensiveWhirl,
                        damage,
                        0,
                        0);
                    break;
                case 1:
                    rulesetAttacker.LogCharacterUsedPower(powerSlashingWhirl);
                    break;
                case 2:
                    rulesetAttacker.InflictCondition(
                        ConditionDisengaging,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        // all disengaging in game is set under TagCombat (why?)
                        AttributeDefinitions.TagCombat,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        ConditionDisengaging,
                        0,
                        0,
                        0);
                    break;
            }
        }
    }
}
