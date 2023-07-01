using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using System.Collections;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;
using SolastaUnfinishedBusiness.CustomValidators;
using static ActionDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TA;

namespace SolastaUnfinishedBusiness.Races;

internal static class WyrmkinRaceBuilder
{
    internal static CharacterRaceDefinition RaceWyrmkin { get; } = BuildWyrmkin();

    [NotNull]
    private static CharacterRaceDefinition BuildWyrmkin()
    {
        var proficiencyWyrmkinLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWyrmkinLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceWyrmkin = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceWyrmkin")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite("Wyrmkin", Resources.Wyrmkin, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(200)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyWyrmkinLanguages)
            .AddToDB();

        raceWyrmkin.subRaces =
            new List<CharacterRaceDefinition> { 
                BuildHighWyrmkin(raceWyrmkin),
                BuildCaveWyrmkin(raceWyrmkin)
                };
        RacesContext.RaceScaleMap[raceWyrmkin] = 7.0f / 6.4f;
        return raceWyrmkin;
    }

    private static CharacterRaceDefinition BuildCaveWyrmkin(CharacterRaceDefinition characterRaceDefinition)
    {
        string Name = "CaveWyrmkin";
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

        var featureCaveWyrmkinPowerfulClaws = FeatureDefinitionBuilder
            .Create($"Feature{Name}PowerfulClaws")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
        featureCaveWyrmkinPowerfulClaws.SetCustomSubFeatures(
            new ModifyWeaponAttackModeCaveWyrmkinClaws(),
            new CaveWyrmkinShovingAttack(featureCaveWyrmkinPowerfulClaws));

        var featureCaveWyrmkinChargingStrike = FeatureDefinitionBuilder
            .Create($"Feature{Name}ChargingStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
        featureCaveWyrmkinChargingStrike.SetCustomSubFeatures(
            new AfterActionFinishedCaveWyrmkinChargingStrike(featureCaveWyrmkinChargingStrike));
        var caveWyrmkinRacePresentation = Dragonborn.RacePresentation.DeepCopy();
        caveWyrmkinRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        var raceCaveWyrmkin = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceCaveWyrmkin")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite("Wyrmkin", Resources.Wyrmkin, 1024, 512))
            .SetRacePresentation(caveWyrmkinRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierCaveWyrmkinStrengthAbilityScoreIncrease,
                attributeModifierCaveWyrmkinConstitutionAbilityScoreIncrease,
                featureCaveWyrmkinPowerfulClaws,
                featureCaveWyrmkinChargingStrike,
                abilityCheckAffinityCaveWyrmkinCaveSenses
            ).AddToDB();

        return raceCaveWyrmkin;
    }

    private sealed class CaveWyrmkinShovingAttack : IPhysicalAttackFinished
    {
        private ConditionDefinition conditionDefinition;
        private FeatureDefinition parentFeature;

        public CaveWyrmkinShovingAttack(FeatureDefinition parentFeature)
        {
            conditionDefinition = ConditionDefinitionBuilder
                .Create("ConditionCaveWyrmkinShovingAttack")
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetGuiPresentationNoContent(true)
                .SetFeatures(FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityBonusShove")
                    .SetAuthorizedActions(Id.ShoveBonus)
                    .AddToDB())
                .AddToDB();
            this.parentFeature = parentFeature;
        }
        public IEnumerator OnAttackFinished(
            GameLocationBattleManager battleManager, 
            CharacterAction action, 
            GameLocationCharacter attacker, 
            GameLocationCharacter defender, 
            RulesetAttackMode attackerAttackMode, 
            RollOutcome attackRollOutcome, 
            int damageAmount)
        {
            if (attackRollOutcome != RollOutcome.Success && attackRollOutcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }
            if (attackerAttackMode.Ranged)
            {
                yield break;
            }
            var character = attacker.RulesetCharacter;
            character.AddConditionOfCategory(conditionDefinition.Name,
                RulesetCondition.CreateActiveCondition(
                    character.Guid,
                    conditionDefinition,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    character.Guid,
                    character.CurrentFaction.Name));
            character.LogCharacterUsedFeature(parentFeature);
        }
    }
    private sealed class AfterActionFinishedCaveWyrmkinChargingStrike : IActionFinished
    {
        private ConditionDefinition ConditionAfterDash { get; }
        private FeatureDefinition parentFeature;
        
        public AfterActionFinishedCaveWyrmkinChargingStrike(FeatureDefinition parentFeature)
        {
            ConditionAfterDash = ConditionDefinitionBuilder
                .Create("ConditionCaveWyrmChargingStrike")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetCustomSubFeatures(new AddExtraUnarmedAttack(ActionType.Bonus))
                .AddToDB();
            this.parentFeature = parentFeature;
        }
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action.ActionId != Id.DashMain)
            {
                yield break;
            }
            var character = action.ActingCharacter.RulesetCharacter;
            character.AddConditionOfCategory(ConditionAfterDash.Name,
                RulesetCondition.CreateActiveCondition(
                    character.Guid,
                    ConditionAfterDash,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    character.Guid,
                    character.CurrentFaction.Name));
            character.LogCharacterUsedFeature(parentFeature);
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

            var strength = character.TryGetAttributeValue(AttributeDefinitions.Strength);
            var strengthModifier = AttributeDefinitions.ComputeAbilityScoreModifier(strength);

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
    private static CharacterRaceDefinition BuildHighWyrmkin(CharacterRaceDefinition characterRaceDefinition)
    {
        var attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
            .AddToDB();

        var attributeModifierHighWyrmkinStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifierHighWyrmkinStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var powerHighWyrmkinReactiveRetribution = FeatureDefinitionPowerBuilder
            .Create("PowerHighWyrmkinReactiveRetribution")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerHighWyrmkinReactiveRetribution.SetCustomSubFeatures(
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
            .Create("PowerHighWyrmkinPsionicWave")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDraconicCry", Resources.PowerDraconicCry, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectPsionicWave)
            .AddToDB();
        var highWyrmkinRacePresentation = Dragonborn.RacePresentation.DeepCopy();
        highWyrmkinRacePresentation.preferedSkinColors = new RangedInt(20, 25);

        var raceHighWyrmkin = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceHighWyrmkin")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite("Wyrmkin", Resources.Wyrmkin, 1024, 512))
            .SetRacePresentation(highWyrmkinRacePresentation)
            .SetFeaturesAtLevel(1,
            attributeModifierHighWyrmkinStrengthAbilityScoreIncrease,
            attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease,
            powerHighWyrmkinPsionicWave,
            powerHighWyrmkinReactiveRetribution
            ).AddToDB();

        return raceHighWyrmkin;
    }

    private class ReactToAttackOnMeReactiveRetribution : IReactToAttackOnMeFinished
    {
        private FeatureDefinitionPower pool;

        public ReactToAttackOnMeReactiveRetribution(FeatureDefinitionPower powerHighWyrmkinSwiftRetribution)
        {
            pool = powerHighWyrmkinSwiftRetribution;
        }

        public IEnumerator OnReactToAttackOnMeFinished(GameLocationCharacter attacker, 
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode, 
            ActionModifier modifier)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }
            // only trigger on a hit
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }
            var rulesetEnemy = attacker.RulesetCharacter;

            if (!me.CanReact() ||
            rulesetEnemy == null ||
                rulesetEnemy.IsDeadOrDying)
            {
                yield break;
            }

            if (me.RulesetCharacter.GetRemainingPowerCharges(pool) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);
            }

            if (retaliationMode == null)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var rulesetCharacter = me.RulesetCharacter;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var tag = "ReactiveRetribution";
            var reactionRequest = new ReactionRequestReactionAttack(tag, reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (reactionParams.ReactionValidated)
            {
                rulesetCharacter.UsePower(UsablePowersProvider.Get(pool, rulesetCharacter));
            }

        }
    }
}
