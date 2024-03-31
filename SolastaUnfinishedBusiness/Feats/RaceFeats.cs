using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RaceFeats
{
    private const string ElvenPrecision = "ElvenPrecision";
    private const string FadeAway = "FadeAway";
    private const string RevenantGreatSword = "RevenantGreatSword";
    private const string SquatNimbleness = "SquatNimbleness";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Dragon Wings
        var featDragonWings = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonWings")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatDragonWings")
                    .SetGuiPresentation("FeatDragonWings", Category.Feat,
                        Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
                    .SetUsesProficiencyBonus(ActivationTime.BonusAction)
                    .AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.DoesNotHaveHeavyArmor))
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                            .SetDurationData(DurationType.Minute, 1)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetConditionForm(
                                        DatabaseHelper.ConditionDefinitions.ConditionFlying12,
                                        ConditionForm.ConditionOperation.Add)
                                    .Build())
                            .Build())
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .AddToDB();

        //
        // Fade Away support
        //

        var powerFeatFadeAwayInvisible = FeatureDefinitionPowerBuilder
            .Create("PowerFeatFadeAwayInvisible")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ReactionTriggerContext.DamagedByAnySource)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Invisibility.EffectDescription)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .Build())
            .AddToDB();

        // Fade Away (Dexterity)
        var featFadeAwayDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Fade Away (Intelligence)
        var featFadeAwayInt = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        //
        // Revenant support
        //

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(GreatswordType);

        var attributeModifierFeatRevenantGreatSwordArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatRevenantGreatSwordArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasGreatswordInHands)
            .AddCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        // Revenant Great Sword (Dexterity)
        var featRevenantGreatSwordDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        // Revenant Great Sword (Strength)
        var featRevenantGreatSwordStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        //
        // Squat Nimbleness
        //
        var featSquatNimblenessDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAcrobatics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        var featSquatNimblenessStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAthletics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Athletics)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        //Infernal Constitution
        var featInfernalConstitution = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatInfernalConstitution")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Arun,
                SavingThrowAffinityAntitoxin,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityPoisonResistance)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .AddToDB();

        var featDwarvenFortitude = BuildDwarvenFortitude();
        var featGroupFlamesOfPhlegethos = BuildFlamesOfPhlegethos(feats);
        var featGroupSecondChance = BuildSecondChance(feats);

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featDragonWings,
            featDwarvenFortitude,
            featFadeAwayDex,
            featFadeAwayInt,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr,
            featSquatNimblenessDex,
            featSquatNimblenessStr,
            featInfernalConstitution);

        var featGroupsElvenAccuracy = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupElvenAccuracy",
            ElvenPrecision,
            ValidatorsFeat.IsElfOfHalfElf,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        var featGroupFadeAway = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFadeAway",
            FadeAway,
            ValidatorsFeat.IsGnome,
            featFadeAwayDex,
            featFadeAwayInt);

        var featGroupRevenantGreatSword = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupRevenantGreatSword",
            RevenantGreatSword,
            ValidatorsFeat.IsElfOfHalfElf,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr);

        var featGroupSquatNimbleness = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSquatNimbleness",
            SquatNimbleness,
            ValidatorsFeat.IsSmallRace,
            featSquatNimblenessDex,
            featSquatNimblenessStr);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(featDragonWings);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featGroupFadeAway);

        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(featGroupRevenantGreatSword);

        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featGroupsElvenAccuracy,
            featGroupFadeAway,
            featGroupFlamesOfPhlegethos,
            featGroupRevenantGreatSword,
            featGroupSecondChance,
            featGroupSquatNimbleness);
    }

    #region Dwarven Fortitude

    private static FeatDefinitionWithPrerequisites BuildDwarvenFortitude()
    {
        const string Name = "FeatDwarvenFortitude";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"Feature{Name}")
                    .SetGuiPresentation(Name, Category.Feat)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 1)
                    .AddCustomSubFeatures(new ActionFinishedByMeDwarvenFortitude())
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsDwarf)
            .AddToDB();
    }

    private sealed class ActionFinishedByMeDwarvenFortitude : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (characterAction.ActionId is not (ActionDefinitions.Id.Dodge or ActionDefinitions.Id.UncannyDodge))
            {
                yield break;
            }

            var attacker = characterAction.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetHero = rulesetAttacker.GetOriginalHero();

            if (rulesetHero == null || rulesetHero.RemainingHitDiceCount() == 0)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("DwarvenFortitude", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(attacker, gameLocationActionService,
                previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetHero.RollHitDie();
        }
    }

    #endregion

    #region Flames of Phlegethos

    private static FeatDefinition BuildFlamesOfPhlegethos(List<FeatDefinition> feats)
    {
        const string Name = "FeatFlamesOfPhlegethos";

        var powerRetaliate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Retaliate")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeFire, 1, DieType.D4))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        var damageAffinity = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetRetaliate(powerRetaliate, 1)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat,
                DatabaseHelper.ConditionDefinitions.ConditionDivineFavor)
            .SetPossessive()
            .AddFeatures(damageAffinity)
            .AddToDB();

        condition.GuiPresentation.description = Gui.NoLocalization;

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(condition),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        power.AddCustomSubFeatures(new MagicEffectFinishedByMeAnyFlamesOfPhlegethos(power));

        var dieRollModifierFire = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}Fire")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetModifiers(RollContext.MagicDamageValueRoll, 1, 1, 1,
                "Feature/&DieRollModifierFeatFlamesOfPhlegethosReroll")
            .AddCustomSubFeatures(new ValidateDieRollModifierFlamesOfPhlegethos())
            .AddToDB();

        var flamesCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, dieRollModifierFire, power)
            .SetFeatFamily(Name)
            .AddToDB();

        var flamesInt = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Int")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, dieRollModifierFire, power)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(flamesCha, flamesInt);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFlamesOfPhlegethos", Name, ValidatorsFeat.IsTiefling, flamesCha, flamesInt);
    }

    private sealed class MagicEffectFinishedByMeAnyFlamesOfPhlegethos(FeatureDefinitionPower power)
        : IMagicEffectFinishedByMeAny
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            if (!action.ActionParams.activeEffect.EffectDescription.EffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType == DamageTypeFire))
            {
                yield break;
            }

            if (ServiceRepository.GetService<IGameLocationBattleService>()
                    is not GameLocationBattleManager gameLocationBattleManager ||
                ServiceRepository.GetService<IGameLocationActionService>()
                    is not GameLocationActionManager gameLocationActionManager ||
                ServiceRepository.GetService<IRulesetImplementationService>()
                    is not RulesetImplementationManager implementationManagerService)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                StringParameter = "PowerFeatFlamesOfPhlegethos",
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { attacker }
            };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return gameLocationBattleManager.WaitForReactions(attacker, gameLocationActionManager, count);
        }
    }

    private sealed class ValidateDieRollModifierFlamesOfPhlegethos : IValidateDieRollModifier
    {
        public bool CanModifyRoll(
            RulesetCharacter character,
            List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return damageTypes.Contains(DamageTypeFire);
        }
    }

    #endregion

    #region Second Chance

    private static FeatDefinition BuildSecondChance(List<FeatDefinition> feats)
    {
        const string Name = "FeatSecondChance";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.AddCustomSubFeatures(new TryAlterOutcomeAttackSecondChance(feature, condition));

        var secondChanceDex = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Misaye)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        var secondChanceCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        var secondChanceCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(secondChanceDex, secondChanceCon, secondChanceCha);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSecondChance", Name, ValidatorsFeat.IsHalfling, secondChanceDex, secondChanceCon,
            secondChanceCha);
    }

    private sealed class TryAlterOutcomeAttackSecondChance(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureSecondChance,
        ConditionDefinition conditionSecondChance) : ITryAlterOutcomeAttack
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (defender != helper ||
                !defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                rulesetDefender.HasConditionOfType(conditionSecondChance))
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionSecondChanceDescription"
                };
            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("SecondChance", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                conditionSecondChance.Name,
                DurationType.UntilAnyRest,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionSecondChance.Name,
                0,
                0,
                0);

            var attackRoll = action.AttackRoll;
            var outcome = action.AttackRollOutcome;
            var rollCaption = outcome == RollOutcome.CriticalSuccess
                ? "Feedback/&RollAttackCriticalSuccessTitle"
                : "Feedback/&RollAttackSuccessTitle";

            var rulesetAttacker = attacker.RulesetCharacter;
            var attackMode = action.actionParams.attackMode;
            var activeEffect = action.ActionParams.activeEffect;

            int roll;
            int toHitBonus;
            int successDelta;

            if (attackMode != null)
            {
                toHitBonus = attackMode.ToHitBonus;
                roll = rulesetAttacker.RollAttack(
                    toHitBonus,
                    defender.RulesetCharacter,
                    attackMode.SourceDefinition,
                    attackMode.ToHitBonusTrends,
                    false,
                    attackModifier.AttackAdvantageTrends,
                    attackMode.ranged,
                    false,
                    attackModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            else if (activeEffect != null)
            {
                toHitBonus = activeEffect.MagicAttackBonus;
                roll = rulesetAttacker.RollMagicAttack(
                    activeEffect,
                    defender.RulesetCharacter,
                    activeEffect.GetEffectSource(),
                    attackModifier.AttacktoHitTrends,
                    attackModifier.AttackAdvantageTrends,
                    false,
                    attackModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            // should never happen
            else
            {
                yield break;
            }

            rulesetDefender.LogCharacterUsedFeature(
                featureSecondChance,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}+{toHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.SuccessfulRoll,
                    Gui.Format(rollCaption, $"{attackRoll + toHitBonus}")));

            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }

    #endregion
}
