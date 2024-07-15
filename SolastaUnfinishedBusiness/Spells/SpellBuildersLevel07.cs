using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Reverse Gravity

    internal static SpellDefinition BuildReverseGravity()
    {
        const string NAME = "ReverseGravity";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ReverseGravity, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionLevitate, "ConditionReverseGravity")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetConditionType(ConditionType.Neutral)
                                    .SetParentCondition(ConditionDefinitions.ConditionFlying)
                                    .SetFeatures()
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(
                                MotionForm.MotionType.Levitate,
                                10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Draconic Transformation

    internal static SpellDefinition BuildDraconicTransformation()
    {
        const string NAME = "DraconicTransformation";

        var sprite = Sprites.GetSprite(NAME, Resources.DraconicTransformation, 128);

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();

        power.disableIfConditionIsOwned = conditionMark;

        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                power,
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionSenses.SenseBlindSight6)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        condition.GuiPresentation.description = Gui.NoLocalization;

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionMark,
                            ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(
                            condition,
                            ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Rescue the Dying

    internal static SpellDefinition BuildRescueTheDying()
    {
        const string RescueTheDyingName = "RescueTheDying";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{RescueTheDyingName}")
            .SetGuiPresentation(RescueTheDyingName, Category.Spell, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        condition.GuiPresentation.description = Gui.NoLocalization;

        var spell = SpellDefinitionBuilder
            .Create(RescueTheDyingName)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(RescueTheDyingName, Resources.RescueTheDying, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Reaction)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Dice, 30, DieType.D10, 4, false,
                                HealingCap.MaximumHitPoints)
                            .Build(),
                        EffectFormBuilder.ConditionForm(condition))
                    .SetParticleEffectParameters(Resurrection)
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(new CustomBehaviorRescueTheDying(spell));

        return spell;
    }

    internal sealed class CustomBehaviorRescueTheDying : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        private static SpellDefinition _rescueTheDying;

        internal CustomBehaviorRescueTheDying(SpellDefinition rescueTheDying)
        {
            _rescueTheDying = rescueTheDying;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetTarget = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            rulesetTarget.HealingReceived -= HealingReceivedHandler;

            yield break;
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetTarget = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            rulesetTarget.HealingReceived += HealingReceivedHandler;

            yield break;
        }

        internal static IEnumerator HandleRescueTheDyingReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter waiter,
            GameLocationCharacter defender)
        {
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders = locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters)
                .Where(x =>
                    x.CanReact() &&
                    x.IsWithinRange(defender, 18) &&
                    x.CanPerceiveTarget(defender) &&
                    x.RulesetCharacter.UsableSpells.Contains(_rescueTheDying) &&
                    x.RulesetCharacter.AreSpellComponentsValid(_rescueTheDying))
                .ToList();

            foreach (var contender in contenders)
            {
                yield return HandleReaction(battleManager, waiter, defender, contender);
            }
        }

        private static IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter waiter,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var slotLevel = rulesetHelper.GetLowestSlotLevelAndRepertoireToCastSpell(
                _rescueTheDying, out var spellRepertoire);

            if (slotLevel < 7 ||
                spellRepertoire == null ||
                helper.Side != defender.Side)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetHelper, spellRepertoire, SpellsContext.RescueTheDying, slotLevel, false);
            var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.SpendSpellSlot)
            {
                IntParameter = slotLevel,
                StringParameter = _rescueTheDying.Name,
                SpellRepertoire = spellRepertoire,
                RulesetEffect = effectSpell
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendSpellSlot(reactionParams);

            yield return battleManager.WaitForReactions(waiter, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var slotUsed = reactionParams.IntParameter;
            var actionParams = new CharacterActionParams(helper, ActionDefinitions.Id.CastReaction)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectSpell(rulesetHelper, spellRepertoire, _rescueTheDying, slotUsed, false),
                TargetCharacters = { defender }
            };

            actionService.ExecuteAction(actionParams, null, true);
        }

        private static void HealingReceivedHandler(
            RulesetCharacter character,
            int healing,
            ulong sourceGuid,
            HealingCap healingCaps,
            IHealingModificationProvider healingModificationProvider)
        {
            character.ReceiveTemporaryHitPoints(healing / 2, DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn,
                sourceGuid);
        }
    }

    #endregion

    #region Crown of Stars

    internal static SpellDefinition BuildCrownOfStars()
    {
        const string NAME = "CrownOfStars";

        var sprite = Sprites.GetSprite($"Power{NAME}", Resources.CrownOfStars, 128);
        var powerCrownOfStars = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None, 1, 7)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeRadiant, 4, DieType.D12))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetParticleEffectParameters(GuidingBolt)
                    .SetCasterEffectParameters(PowerPaladinAuraOfCourage)
                    .Build())
            .AddToDB();

        var conditionCrownOfStars = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation($"Power{NAME}", Category.Feature, ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerCrownOfStars)
            .AddCustomSubFeatures(
                // order matters
                new ConditionAddedOrRemovedCrownOfStars(),
                AddUsablePowersFromCondition.Marker)
            .CopyParticleReferences(DeathWard)
            .AddToDB();

        conditionCrownOfStars.GuiPresentation.description = Gui.NoLocalization;

        var lightSourceForm = FaerieFire.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalSummonsPerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionCrownOfStars),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6, lightSourceForm.Color,
                                lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetCasterEffectParameters(Sparkle)
                    .SetEffectEffectParameters(PowerOathOfJugementPurgeCorruption)
                    .Build())
            .AddToDB();

        powerCrownOfStars.AddCustomSubFeatures(
            new ModifyPowerPoolAmount
            {
                PowerPool = powerCrownOfStars,
                Type = PowerPoolBonusCalculationType.ConditionAmount,
                Attribute = conditionCrownOfStars.Name
            },
            new CustomBehaviorPowerCrownOfStars(spell, powerCrownOfStars, conditionCrownOfStars));

        return spell;
    }

    private sealed class ConditionAddedOrRemovedCrownOfStars : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            rulesetCondition.Amount = (rulesetCondition.EffectLevel - 7) * 2;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class CustomBehaviorPowerCrownOfStars(
        SpellDefinition spellCrownOfStars,
        FeatureDefinitionPower powerMotes,
        ConditionDefinition conditionCrownOfStars) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var remainingUses = action.ActingCharacter.RulesetCharacter.GetRemainingPowerUses(powerMotes);

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingUses == 0 &&
                rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionCrownOfStars.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }
            else if (remainingUses < 4)
            {
                var usablePower =
                    rulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellCrownOfStars);

                if (usablePower != null &&
                    usablePower.TrackedLightSourceGuids.Count > 0 &&
                    usablePower.TrackedLightSourceGuids[0] == rulesetCharacter.PersonalLightSource.Guid)
                {
                    rulesetCharacter.PersonalLightSource.brightRange = 0;
                }
            }

            yield break;
        }
    }

    #endregion
}
