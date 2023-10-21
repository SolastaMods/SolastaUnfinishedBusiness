using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceWyrmkinBuilder
{
    private const string RaceName = "RaceWyrmkin";
    internal static CharacterRaceDefinition RaceWyrmkin { get; } = BuildWyrmkin();

    [NotNull]
    private static CharacterRaceDefinition BuildWyrmkin()
    {
        var proficiencyWyrmkinLanguages = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{RaceName}Languages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceWyrmkin = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, RaceName)
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Wyrmkin, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetBaseHeight(72)
            .SetBaseWeight(185)
            .SetMinimalAge(18)
            .SetMaximalAge(750)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyWyrmkinLanguages)
            .AddToDB();

        RacesContext.RaceScaleMap[raceWyrmkin] = 7.0f / 6.4f;
        raceWyrmkin.subRaces =
            new List<CharacterRaceDefinition> { BuildHighWyrmkin(raceWyrmkin), BuildCaveWyrmkin(raceWyrmkin) };

        return raceWyrmkin;
    }

    private static CharacterRaceDefinition BuildCaveWyrmkin(CharacterRaceDefinition characterRaceDefinition)
    {
        const string Name = "CaveWyrmkin";

        var attributeModifierCaveWyrmkinConstitutionAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ConstitutionAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 1)
            .AddToDB();

        var attributeModifierCaveWyrmkinStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}StrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var abilityCheckAffinityCaveWyrmkinCaveSenses = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}CaveSenses")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Survival))
            .AddToDB();

        var conditionCaveWyrmkinShovingAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}ShovingAttack")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetGuiPresentationNoContent(true)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}BonusShove")
                    .SetGuiPresentationNoContent(true)
                    .SetAuthorizedActions(Id.ShoveBonus)
                    .AddToDB())
            .AddToDB();

        var featureCaveWyrmkinPowerfulClaws = FeatureDefinitionBuilder
            .Create($"Feature{Name}PowerfulClaws")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureCaveWyrmkinPowerfulClaws.AddCustomSubFeatures(
            new ModifyWeaponAttackModeCaveWyrmkinClaws(),
            new CaveWyrmkinShovingAttack(featureCaveWyrmkinPowerfulClaws, conditionCaveWyrmkinShovingAttack));

        var conditionChargingStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}ChargingStrike")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new AddExtraUnarmedAttack(ActionType.Bonus))
            .AddToDB();

        var featureCaveWyrmkinChargingStrike = FeatureDefinitionBuilder
            .Create($"Feature{Name}ChargingStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureCaveWyrmkinChargingStrike.AddCustomSubFeatures(
            new AfterActionFinishedByMeCaveWyrmkinChargingStrike(featureCaveWyrmkinChargingStrike,
                conditionChargingStrike));

        var caveWyrmkinRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        caveWyrmkinRacePresentation.preferedSkinColors = Main.Settings.UnlockSkinColors
            ? new RangedInt(48, 53)
            : new RangedInt(14, 14);

        var raceCaveWyrmkin = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Wyrmkin, 1024, 512))
            .SetRacePresentation(caveWyrmkinRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierCaveWyrmkinStrengthAbilityScoreIncrease,
                attributeModifierCaveWyrmkinConstitutionAbilityScoreIncrease,
                featureCaveWyrmkinPowerfulClaws,
                featureCaveWyrmkinChargingStrike,
                abilityCheckAffinityCaveWyrmkinCaveSenses)
            .AddToDB();

        return raceCaveWyrmkin;
    }

    private static CharacterRaceDefinition BuildHighWyrmkin(CharacterRaceDefinition characterRaceDefinition)
    {
        const string Name = "HighWyrmkin";

        var attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}IntelligenceAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
            .AddToDB();

        var attributeModifierHighWyrmkinStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}StrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var powerHighWyrmkinReactiveRetribution = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ReactiveRetribution")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerHighWyrmkinReactiveRetribution.AddCustomSubFeatures(
            new ReactToAttackOnMeReactiveRetribution(powerHighWyrmkinReactiveRetribution));

        var effectPsionicWave = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .ExcludeCaster()
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Intelligence,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence)
            .SetParticleEffectParameters(ColorSpray.EffectDescription.effectParticleParameters)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypePsychic, 2, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .SetDiceAdvancement(LevelSourceType.CharacterLevel, 1, 1, 5, 6)
                    .Build())
            .Build();

        var powerHighWyrmkinPsionicWave = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicWave")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDraconicCry", Resources.PowerDraconicCry, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectPsionicWave)
            .AddToDB();

        var highWyrmkinRacePresentation = Dragonborn.RacePresentation.DeepCopy();


        highWyrmkinRacePresentation.preferedSkinColors =
            Main.Settings.UnlockSkinColors ? new RangedInt(65, 65) : new RangedInt(11, 11);

        var raceHighWyrmkin = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Wyrmkin, 1024, 512))
            .SetRacePresentation(highWyrmkinRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierHighWyrmkinStrengthAbilityScoreIncrease,
                attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease,
                powerHighWyrmkinPsionicWave,
                powerHighWyrmkinReactiveRetribution
            ).AddToDB();

        return raceHighWyrmkin;
    }

    private sealed class CaveWyrmkinShovingAttack : IPhysicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _parentFeature;

        public CaveWyrmkinShovingAttack(FeatureDefinition parentFeature, ConditionDefinition conditionShoveAttack)
        {
            _conditionDefinition = conditionShoveAttack;
            _parentFeature = parentFeature;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (attackerAttackMode.Ranged)
            {
                yield break;
            }

            var character = attacker.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (Gui.Battle?.ActiveContender != attacker)
            {
                yield break;
            }

            if (character.HasConditionOfType(_conditionDefinition.Name))
            {
                yield break;
            }

            character.LogCharacterUsedFeature(_parentFeature);
            character.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                character.guid,
                character.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class AfterActionFinishedByMeCaveWyrmkinChargingStrike : IActionFinishedByMe
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _parentFeature;

        public AfterActionFinishedByMeCaveWyrmkinChargingStrike(
            FeatureDefinition parentFeature,
            ConditionDefinition conditionChargingStrike)
        {
            _conditionDefinition = conditionChargingStrike;
            _parentFeature = parentFeature;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action.ActionId != Id.DashMain)
            {
                yield break;
            }

            var character = action.ActingCharacter.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            character.LogCharacterUsedFeature(_parentFeature);
            character.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                character.guid,
                character.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyWeaponAttackModeCaveWyrmkinClaws : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(character, attackMode) || attackMode.ranged)
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            if (k < 0 || damage == null)
            {
                return;
            }

            if (damage.DieType < DieType.D6)
            {
                damage.DieType = DieType.D6;
            }

            damage.DamageType = DamageTypeSlashing;
        }
    }

    private class ReactToAttackOnMeReactiveRetribution : IPhysicalAttackFinishedOnMe
    {
        private readonly FeatureDefinitionPower _pool;

        public ReactToAttackOnMeReactiveRetribution(FeatureDefinitionPower powerHighWyrmkinSwiftRetribution)
        {
            _pool = powerHighWyrmkinSwiftRetribution;
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == defender)
            {
                yield break;
            }

            // only trigger on a hit
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!defender.CanReact() ||
                rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (defender.RulesetCharacter.GetRemainingPowerCharges(_pool) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = defender.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = defender.GetFirstRangedModeThatCanAttack(attacker);
            }

            if (retaliationMode == null)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(defender, Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var rulesetCharacter = defender.RulesetCharacter;
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("ReactiveRetribution", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (reactionParams.ReactionValidated)
            {
                rulesetCharacter.UsePower(UsablePowersProvider.Get(_pool, rulesetCharacter));
            }
        }
    }
}
