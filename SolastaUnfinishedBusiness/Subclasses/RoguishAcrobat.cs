using System.Collections;
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

        var isQuarterstaff = ValidatorsWeapon.IsOfWeaponType(QuarterstaffType);

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
                new AddTagToWeapon(
                    TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, isQuarterstaff),
                // should not stack with Lunger or Wendigo
                new IncreaseWeaponReach(1, isQuarterstaff, Lunger.Name))
            .AddToDB();

        // LEVEL 09

        // Swift as the Wind

        const string SWIFT_WIND = $"FeatureSet{Name}SwiftWind";

        var movementAffinitySwiftWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetClimbing(true, true)
            .SetAdditionalJumpCells(2)
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
                new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D6, DieType.D10), isQuarterstaff))
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

        // LEVEL 13

        //  Fluid Motions

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
            .SetAdditionalJumpCells(3)
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
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDodging))
                    .Build())
            .AddToDB();

        // LEVEL 17

        //  Heroic Uncanny Dodge

        var conditionHeroicUncannyDodge = ConditionDefinitionBuilder
            .Create($"Condition{Name}HeroicUncannyDodge")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .SetFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create($"ReduceDamage{Name}HeroicUncannyDodge")
                    .SetGuiPresentation($"Power{Name}HeroicUncannyDodge", Category.Feature)
                    .SetAlwaysActiveReducedDamage((_, _) => 999)
                    .AddToDB())
            .AddToDB();

        var powerHeroicUncannyDodge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HeroicUncannyDodge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Dexterity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHeroicUncannyDodge))
                    .Build())
            .AddToDB();

        powerHeroicUncannyDodge.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitPossibleOnMeOrAllyUncannyDodge(powerHeroicUncannyDodge));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishAcrobat, 256))
            .AddFeaturesAtLevel(3,
                proficiencyAcrobatConnoisseur, attributeModifierAcrobatDefender, featureAcrobatWarrior)
            .AddFeaturesAtLevel(9,
                featureSetSwiftWind)
            .AddFeaturesAtLevel(13,
                combatAffinityFluidMotions, movementAffinityFluidMotions, powerReflexes)
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

    private class AttackBeforeHitPossibleOnMeOrAllyUncannyDodge(FeatureDefinitionPower powerHeroicUncannyDodge)
        : ITryAlterOutcomeAttack

    {
        public int HandlerPriority => 0;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (defender != helper ||
                defender.IsMyTurn() ||
                !defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                rulesetDefender.GetRemainingPowerUses(powerHeroicUncannyDodge) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerHeroicUncannyDodge, rulesetDefender);
            var actionParams =
                new CharacterActionParams(defender, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "HeroicUncannyDodge",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }
}
