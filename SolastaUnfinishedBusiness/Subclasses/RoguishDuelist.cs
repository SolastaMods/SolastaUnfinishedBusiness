using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishDuelist : AbstractSubclass
{
    internal const string Name = "RoguishDuelist";
    internal const string ConditionReflexiveParryName = $"Condition{Name}ReflexiveParry";
    private const string SureFooted = "SureFooted";

    public RoguishDuelist()
    {
        // LEVEL 03

        // Daring Duel

        var conditionDaringDuel = ConditionDefinitionBuilder
            .Create($"Condition{Name}DaringDuel")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionDaringDuel)
            //.AddCustomSubFeatures(ClassHolder.Rogue)
            .AddToDB();

        // Riposte

        var actionAffinitySwirlingDance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwirlingDance")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(Id.SwirlingDance)
            .AddToDB();

        // LEVEL 09

        // Bravado

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{SureFooted}")
            .SetGuiPresentation($"FeatureSet{Name}{SureFooted}", Category.Feature)
            .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
            .AddToDB();

        var conditionSureFooted = ConditionDefinitionBuilder
            .Create($"Condition{Name}{SureFooted}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetSilent(Silent.WhenAdded)
            .SetFixedAmount(1)
            .SetFeatures(attributeModifierSureFooted)
            .AddToDB();

        var featureSureFooted = FeatureDefinitionBuilder
            .Create($"FeatureSet{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorSureFooted(conditionSureFooted))
            .AddToDB();

        // LEVEL 13

        // Reflexive Parry

        var conditionReflexiveParty = ConditionDefinitionBuilder
            .Create($"Condition{Name}ReflexiveParty")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureReflexiveParry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ReflexiveParry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureReflexiveParry.AddCustomSubFeatures(
            new CustomBehaviorReflexiveParry(featureReflexiveParry, conditionReflexiveParty));

        // LEVEL 17

        // Master Duelist

        var featureMasterDuelist = FeatureDefinitionBuilder
            .Create($"Feature{Name}MasterDuelist")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeMasterDuelist(conditionDaringDuel))
            .AddToDB();

        var featureSetMasterDuelist = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterDuelist")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureMasterDuelist)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, actionAffinitySwirlingDance)
            .AddFeaturesAtLevel(9, featureSureFooted)
            .AddFeaturesAtLevel(13, featureReflexiveParry)
            .AddFeaturesAtLevel(17, featureSetMasterDuelist)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool TargetIsDuelingWithRoguishDuelist(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        AdvantageType advantageType)
    {
        return
            advantageType != AdvantageType.Disadvantage &&
            attacker.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Rogue, Name) > 0 &&
            attacker.IsWithinRange(defender, 1) &&
            Gui.Battle.AllContenders
                .Where(x => x != attacker && x != defender)
                .All(x => !attacker.IsWithinRange(x, 1));
    }

    //
    // Reflexive Parry
    //

    private sealed class CustomBehaviorReflexiveParry(
        FeatureDefinition featureReflexiveParry,
        ConditionDefinition conditionReflexiveParty) : IPhysicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
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
            var rulesetDefender = defender.RulesetCharacter;

            if (!ValidatorsWeapon.IsMeleeOrUnarmed(attackMode) ||
                rulesetDefender.IsIncapacitated ||
                rulesetDefender.HasAnyConditionOfTypeOrSubType(
                    conditionReflexiveParty.Name,
                    ConditionDefinitions.ConditionDazzled.Name,
                    ConditionDefinitions.ConditionShocked.Name,
                    ConditionDefinitions.ConditionSlowed.Name))
            {
                yield break;
            }

            actionModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, featureReflexiveParry);
            rulesetDefender.InflictCondition(
                conditionReflexiveParty.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionReflexiveParty.Name,
                0,
                0,
                0);
        }
    }

    //
    // Master Duelist
    //

    private sealed class PhysicalAttackFinishedByMeMasterDuelist(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDaringDuel) : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            // should only check the condition from the same source
            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDaringDuel.Name, out var activeCondition) ||
                activeCondition.SourceGuid != attacker.Guid)
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var attackModeMain = attacker.FindActionAttackMode(Id.AttackMain);

            attacker.MyExecuteActionAttack(
                Id.AttackFree,
                defender,
                attackModeMain,
                new ActionModifier());
        }
    }

    //
    // Sure Footed
    //

    private sealed class CustomBehaviorSureFooted(ConditionDefinition conditionSureFooted)
        : IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (!attacker.UsedSpecialFeatures.ContainsKey("SureFooted"))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var roll = rulesetAttacker.RollDie(
                DieType.D6, RollContext.None, false, AdvantageType.None, out var firstRoll, out var secondRoll);

            rulesetAttacker.ShowDieRoll(
                DieType.D6, firstRoll, secondRoll, advantage: AdvantageType.None,
                title: conditionSureFooted.GuiPresentation.Title);

            var hasBravado = rulesetAttacker.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionSureFooted.Name, out var activeCondition);

            if (hasBravado &&
                roll <= activeCondition.Amount)
            {
                rulesetAttacker.LogCharacterActivatesAbility(
                    Gui.NoLocalization, "Feedback/&RoguishDuelistBravadoReroll", true,
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D6)),
                        (ConsoleStyleDuplet.ParameterType.Negative, roll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Positive, activeCondition.Amount.ToString())
                    ]);

                yield break;
            }

            rulesetAttacker.LogCharacterActivatesAbility(
                Gui.NoLocalization, "Feedback/&RoguishDuelistBravado", true,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D6)),
                    (ConsoleStyleDuplet.ParameterType.Positive, roll.ToString())
                ]);

            rulesetAttacker.InflictCondition(
                conditionSureFooted.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionSureFooted.Name,
                roll,
                0,
                0);
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var isSneakAttackValid = Tabletop2024Context.IsSneakAttackValid(attackModifier, attacker, defender);

            if (isSneakAttackValid)
            {
                attacker.UsedSpecialFeatures.TryAdd("SureFooted", 0);
            }
            else
            {
                attacker.UsedSpecialFeatures.Remove("SureFooted");
            }

            yield break;
        }
    }
}
