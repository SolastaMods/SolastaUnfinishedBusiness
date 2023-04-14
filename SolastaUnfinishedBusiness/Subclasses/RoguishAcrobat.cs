using System;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishAcrobat : AbstractSubclass
{
    private const string Name = "RoguishAcrobat";

    internal RoguishAcrobat()
    {
        // LEVEL 03

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(QuarterstaffType);

        // Acrobat Maven
        var proficiencyAcrobatConnoisseur = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Maven")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
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
            .SetCustomSubFeatures(
                new AddPolearmFollowUpAttack(QuarterstaffType),
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon),
                new ModifyAttackModeForWeaponTypeQuarterstaff(validWeapon))
            .AddToDB();

        // LEVEL 09 - Swift as the Wind

        const string SWIFT_WIND = $"FeatureSet{Name}SwiftWind";

        var movementAffinitySwiftWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetClimbing(true, true)
            .SetEnhancedJump(2)
            .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var savingThrowAffinitySwiftWind = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm, $"SavingThrowAffinity{Name}SwiftWind")
            .SetOrUpdateGuiPresentation(SWIFT_WIND, Category.Feature)
            .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var abilityCheckAffinitySwiftWind = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(AbilityCheckAffinityDomainLawUnyieldingEnforcerShove, $"AbilityCheckAffinity{Name}SwiftWind")
            .SetOrUpdateGuiPresentation(SWIFT_WIND, Category.Feature)
            .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var featureSwiftWind = FeatureDefinitionBuilder
            .Create($"Feature{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new UpgradeWeaponDice((_, _) => (1, DieType.D6, DieType.D10), validWeapon))
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
            .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
            .AddToDB();

        var movementAffinityFluidMotions = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}FluidMotions")
            .SetGuiPresentation(Category.Feature)
            .SetAdditionalFallThreshold(4)
            .SetClimbing(canMoveOnWalls: true)
            .SetEnhancedJump(3)
            .SetImmunities(difficultTerrainImmunity: true)
            .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedQuarterstaff)
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

        var powerHeroicUncannyDodge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HeroicUncannyDodge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Dexterity)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerHeroicUncannyDodge.SetCustomSubFeatures(
            new DefenderBeforeAttackHitConfirmedHeroicUncannyDodge(powerHeroicUncannyDodge));

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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAttackModeForWeaponTypeQuarterstaff : IModifyAttackModeForWeapon
    {
        private readonly IsWeaponValidHandler _isWeaponValid;

        public ModifyAttackModeForWeaponTypeQuarterstaff(IsWeaponValidHandler isWeaponValid)
        {
            _isWeaponValid = isWeaponValid;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!_isWeaponValid(attackMode, null, character))
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }
    }

    private class DefenderBeforeAttackHitConfirmedHeroicUncannyDodge : IDefenderBeforeAttackHitConfirmed
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public DefenderBeforeAttackHitConfirmedHeroicUncannyDodge(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator DefenderBeforeAttackHitConfirmed(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (!rulesetMe.CanUsePower(_featureDefinitionPower))
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionHeroicUncannyDodgeDescription"
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("HeroicUncannyDodge", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            attackModifier.damageRollReduction = Int32.MaxValue;

            GameConsoleHelper.LogCharacterUsedPower(rulesetMe, _featureDefinitionPower);
        }
    }
}
