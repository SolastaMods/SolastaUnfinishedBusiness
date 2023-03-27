using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishDuelist : AbstractSubclass
{
    private const string Name = "RoguishDuelist";
    private const string SureFooted = "SureFooted";
    private const string MasterDuelist = "MasterDuelist";

    internal RoguishDuelist()
    {
        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, $"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            // need to set next 3 even with a template as builder clears them out
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .AddToDB();

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
            .AddToDB();

        var featureSetSureFooted = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle,
                attributeModifierSureFooted)
            .AddToDB();

        var actionAffinitySwirlingDance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwirlingDance")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.SwirlingDance)
            .AddToDB();

        var actionAffinityGracefulTakeDown = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}GracefulTakeDown")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .AddToDB();

        var powerMasterDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{MasterDuelist}")
            .SetGuiPresentation($"FeatureSet{Name}MasterDuelist", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        var featureMasterDuelist = FeatureDefinitionBuilder
            .Create($"Feature{Name}{MasterDuelist}")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new AlterAttackOutcomeMasterDuelist(powerMasterDuelist))
            .AddToDB();

        var featureSetMasterDuelist = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{MasterDuelist}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureMasterDuelist, powerMasterDuelist)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishDuelist", Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, featureSetSureFooted)
            .AddFeaturesAtLevel(9, actionAffinitySwirlingDance)
            .AddFeaturesAtLevel(13, actionAffinityGracefulTakeDown)
            .AddFeaturesAtLevel(17, featureSetMasterDuelist)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class AlterAttackOutcomeMasterDuelist : IAlterAttackOutcome
    {
        private readonly FeatureDefinitionPower _power;

        public AlterAttackOutcomeMasterDuelist(FeatureDefinitionPower power)
        {
            _power = power;
        }

        public IEnumerator TryAlterAttackOutcome(GameLocationBattleManager battle, CharacterAction action,
            GameLocationCharacter me, GameLocationCharacter target, ActionModifier attackModifier)
        {
            var attackMode = action.actionParams.attackMode;
            var character = me.RulesetCharacter;

            if (character == null || character.GetRemainingPowerCharges(_power) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = $"Reaction/&CustomReaction{Name}{MasterDuelist}ReactDescription"
            };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom($"{Name}{MasterDuelist}", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                attackModifier.ignoreAdvantage,
                new List<TrendInfo> { new(1, FeatureSourceType.CharacterFeature, _power.Name, _power) },
                attackMode.ranged,
                false, // check this
                attackModifier.attackRollModifier,
                out var outcome,
                out _,
                -1,
                false);

            action.AttackRollOutcome = outcome;

            GameConsoleHelper.LogCharacterUsedPower(character, _power);
        }
    }
}
