using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishDuelist : AbstractSubclass
{
    internal const string Name = "RoguishDuelist";
    internal const string ConditionReflexiveParry = $"Condition{Name}ReflexiveParry";
    private const string SureFooted = "SureFooted";

    public RoguishDuelist()
    {
        // LEVEL 3

        var conditionDaringDuel = ConditionDefinitionBuilder
            .Create($"Condition{Name}DaringDuel")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, $"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    Operation = ConditionOperationDescription.ConditionOperation.Add,
                    ConditionDefinition = conditionDaringDuel
                })
            .AddCustomSubFeatures(new RogueHolder())
            .AddToDB();

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
            .AddToDB();

        var featureSetSureFooted = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle,
                attributeModifierSureFooted)
            .AddToDB();

        // LEVEL 9

        var actionAffinitySwirlingDance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwirlingDance")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(ActionDefinitions.Id.SwirlingDance)
            .AddToDB();

        // LEVEL 13

        var conditionReflexiveParry = ConditionDefinitionBuilder
            .Create(ConditionReflexiveParry)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureReflexiveParry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ReflexiveParry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureReflexiveParry.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeReflexiveParty(featureReflexiveParry, conditionReflexiveParry));

        // LEVEL 17

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

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, featureSetSureFooted)
            .AddFeaturesAtLevel(9, actionAffinitySwirlingDance)
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

    private sealed class RogueHolder : IModifyAdditionalDamageClassLevel
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    //
    // Reflexive Party
    //

    private sealed class AttackBeforeHitConfirmedOnMeReflexiveParty(
        FeatureDefinition featureReflexiveParty,
        ConditionDefinition conditionReflexiveParty)
        : IAttackBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.HasAnyConditionOfTypeOrSubType(
                    conditionReflexiveParty.Name,
                    ConditionDefinitions.ConditionIncapacitated.Name,
                    ConditionDefinitions.ConditionShocked.Name,
                    ConditionDefinitions.ConditionSlowed.Name))
            {
                yield break;
            }

            actionModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, featureReflexiveParty);
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!defender.CanAct())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                conditionReflexiveParty.Name,
                conditionReflexiveParty.DurationType,
                conditionReflexiveParty.DurationParameter,
                conditionReflexiveParty.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetDefender.Guid,
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
        ConditionDefinition conditionDaringDuel)
        : IPhysicalAttackFinishedByMe
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
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDaringDuel.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();
            var attackModeMain = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.AttackFree];
            actionParams.AttackMode = attackModeMain;

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }
}
