using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishAcrobat : AbstractSubclass
{
    private const string Name = "RoguishAcrobat";

    public RoguishAcrobat()
    {
        // LEVEL 03

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(QuarterstaffType);

        // Acrobat Maven
        var proficiencyAcrobatConnoisseur = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Maven")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Acrobatics)
            .AddToDB();

        // Acrobat Protector
        var attributeModifierAcrobatDefender = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}Protector")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff)
            .AddToDB();

        // Acrobat Trooper
        var featureAcrobatWarrior = FeatureDefinitionBuilder
            .Create($"Feature{Name}Trooper")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new AddPolearmFollowUpAttack(QuarterstaffType),
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon),
                new IncreaseWeaponReach(1, validWeapon, Lunger.Name)) // should not stack with Lunger or Wendigo
            .AddToDB();

        // LEVEL 09 - Swift as the Wind

        const string SWIFT_WIND = $"FeatureSet{Name}SwiftWind";

        var movementAffinitySwiftWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetClimbing(true, true)
            .SetEnhancedJump(2)
            .AddCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var savingThrowAffinitySwiftWind = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm, $"SavingThrowAffinity{Name}SwiftWind")
            .SetOrUpdateGuiPresentation(SWIFT_WIND, Category.Feature)
            .AddCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var abilityCheckAffinitySwiftWind = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(AbilityCheckAffinityDomainLawUnyieldingEnforcerShove, $"AbilityCheckAffinity{Name}SwiftWind")
            .SetOrUpdateGuiPresentation(SWIFT_WIND, Category.Feature)
            .AddCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var featureSwiftWind = FeatureDefinitionBuilder
            .Create($"Feature{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D6, DieType.D10), validWeapon))
            .AddToDB();

        var featureSetSwiftWind = FeatureDefinitionFeatureSetBuilder
            .Create(SWIFT_WIND)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                movementAffinitySwiftWind,
                savingThrowAffinitySwiftWind,
                abilityCheckAffinitySwiftWind,
                featureSwiftWind)
            .AddToDB();

        // LEVEL 13 - Fluid Motions

        var combatAffinityFluidMotions = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}FluidMotions")
            .SetGuiPresentationNoContent(true)
            .SetAttackOfOpportunityOnMeAdvantage(AdvantageType.Disadvantage)
            .AddCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var movementAffinityFluidMotions = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}FluidMotions")
            .SetGuiPresentation(Category.Feature)
            .SetAdditionalFallThreshold(4)
            .SetClimbing(canMoveOnWalls: true)
            .SetEnhancedJump(3)
            .SetImmunities(difficultTerrainImmunity: true)
            .AddCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var powerReflexes = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Reflexes")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerReflexes", Resources.PowerReflexes, 256, 128))
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Dexterity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionDodging,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 17 - Heroic Uncanny Dodge

        var reduceDamageHeroicUncannyDodge = FeatureDefinitionReduceDamageBuilder
            .Create($"ReduceDamage{Name}HeroicUncannyDodge")
            .SetGuiPresentation($"Power{Name}HeroicUncannyDodge", Category.Feature)
            .SetAlwaysActiveReducedDamage((_, _) => Int32.MaxValue)
            .AddToDB();

        var conditionHeroicUncannyDodge = ConditionDefinitionBuilder
            .Create($"Condition{Name}HeroicUncannyDodge")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .SetFeatures(reduceDamageHeroicUncannyDodge)
            .AddToDB();

        var powerHeroicUncannyDodge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HeroicUncannyDodge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Dexterity)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerHeroicUncannyDodge.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeHeroicUncannyDodge(powerHeroicUncannyDodge, conditionHeroicUncannyDodge));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("RoguishAcrobat", Resources.RoguishAcrobat, 256))
            .AddFeaturesAtLevel(3,
                proficiencyAcrobatConnoisseur,
                attributeModifierAcrobatDefender,
                featureAcrobatWarrior)
            .AddFeaturesAtLevel(9,
                featureSetSwiftWind)
            .AddFeaturesAtLevel(13,
                combatAffinityFluidMotions,
                movementAffinityFluidMotions,
                powerReflexes)
            .AddFeaturesAtLevel(17,
                powerHeroicUncannyDodge)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class AttackBeforeHitConfirmedOnMeHeroicUncannyDodge(
        FeatureDefinitionPower featureDefinitionPower,
        ConditionDefinition conditionDefinition)
        : IAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (me.IsMyTurn())
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (!rulesetMe.CanUsePower(featureDefinitionPower))
            {
                yield break;
            }

            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(featureDefinitionPower, rulesetMe);
            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "HeroicUncannyDodge", UsablePower = usablePower
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetMe.UpdateUsageForPower(featureDefinitionPower, featureDefinitionPower.CostPerUse);
            rulesetMe.InflictCondition(
                conditionDefinition.Name,
                conditionDefinition.DurationType,
                conditionDefinition.DurationParameter,
                conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetMe.guid,
                rulesetMe.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }
}
