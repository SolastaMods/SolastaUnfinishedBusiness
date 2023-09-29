using System.Collections;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Poison Wave

    internal static SpellDefinition BuildPoisonWave()
    {
        const string NAME = "PoisonWave";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PoisonWave, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagGlass, 50, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 4)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .ExcludeCaster()
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePoison, 6, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionPoisoned,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PoisonSpray)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference = PowerDragonBreath_Poison
            .EffectDescription.EffectParticleParameters.impactParticleReference;

        return spell;
    }

    #endregion

    #region Heroic Infusion

    internal static SpellDefinition BuildHeroicInfusion()
    {
        const string NAME = "HeroicInfusion";

        var attackModifierHeroicInfusion = FeatureDefinitionCombatAffinityBuilder
            .Create($"AttackModifier{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(ExtraSituationalContext.HasSimpleOrMartialWeaponInHands)
            .AddToDB();

        var additionalDamageHeroicInfusion = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetNotificationTag(NAME)
            .SetDamageDice(DieType.D12, 2)
            .SetSpecificDamageType(DamageTypeForce)
            .AddToDB();

        var actionAffinityHeroicInfusion = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetAuthorizedActions()
            .SetForbiddenActions(
                ActionDefinitions.Id.CastBonus, ActionDefinitions.Id.CastInvocation,
                ActionDefinitions.Id.CastMain, ActionDefinitions.Id.CastReaction,
                ActionDefinitions.Id.CastReadied, ActionDefinitions.Id.CastRitual, ActionDefinitions.Id.CastNoCost)
            .AddToDB();

        var conditionExhausted = ConditionDefinitionBuilder
            .Create(ConditionExhausted, $"Condition{NAME}Exhausted")
            .SetOrUpdateGuiPresentation("ConditionExhausted", Category.Rules, ConditionLethargic)
            .AddToDB();

        var conditionHeroicInfusion = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetPossessive()
            .SetFeatures(
                attackModifierHeroicInfusion,
                additionalDamageHeroicInfusion,
                actionAffinityHeroicInfusion,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionProficiencys.ProficiencyFighterArmor,
                FeatureDefinitionProficiencys.ProficiencyFighterSavingThrow,
                FeatureDefinitionProficiencys.ProficiencyFighterWeapon)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedHeroicInfusion(conditionExhausted))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.HeroicInfusion, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHeroicInfusion),
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(50, DieType.D1, 0, true)
                            .Build())
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedHeroicInfusion : IOnConditionAddedOrRemoved
    {
        private readonly ConditionDefinition _conditionExhausted;

        public OnConditionAddedOrRemovedHeroicInfusion(ConditionDefinition conditionExhausted)
        {
            _conditionExhausted = conditionExhausted;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.TemporaryHitPoints = 0;


            var modifierTrend = target.actionModifier.savingThrowModifierTrends;
            var advantageTrends = target.actionModifier.savingThrowAdvantageTrends;
            var conModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                target.TryGetAttributeValue(AttributeDefinitions.Constitution));

            target.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, conModifier, 15, false, out var savingOutcome, out _);

            if (savingOutcome is RollOutcome.Success)
            {
                return;
            }

            target.InflictCondition(
                _conditionExhausted.Name,
                _conditionExhausted.DurationType,
                _conditionExhausted.DurationParameter,
                _conditionExhausted.TurnOccurence,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                0,
                null,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Ring of Blades

    internal static SpellDefinition BuildRingOfBlades()
    {
        const string NAME = "RingOfBlades";

        var powerRingOfBlades = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite($"Power{NAME}", Resources.PowerRingOfBlades, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None, 1, 6)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 4, DieType.D8))
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        var conditionRingOfBlades = ConditionDefinitionBuilder
            .Create(ConditionStrikeOfChaosAttackAdvantage, $"Condition{NAME}")
            .SetGuiPresentation($"Power{NAME}", Category.Feature, ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerRingOfBlades)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        conditionRingOfBlades.GuiPresentation.description = Gui.NoLocalization;

        powerRingOfBlades.AddCustomSubFeatures(
            new CustomBehaviorRingOfBlades(powerRingOfBlades, conditionRingOfBlades));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.RingOfBlades, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 500, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionRingOfBlades))
                    .SetParticleEffectParameters(HypnoticPattern)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorRingOfBlades : IMagicEffectInitiatedByMe, IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionRingOfBlades;
        private readonly FeatureDefinitionPower _powerRingOfBlades;

        public CustomBehaviorRingOfBlades(
            FeatureDefinitionPower powerRingOfBlades,
            ConditionDefinition conditionRingOfBlades)
        {
            _powerRingOfBlades = powerRingOfBlades;
            _conditionRingOfBlades = conditionRingOfBlades;
        }

        // STEP 1: change attackRollModifier to use spell casting feature
        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.ActingCharacter.RulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionRingOfBlades.Name,
                    out var activeCondition)
                && action.ActionParams.actionModifiers.Count > 0)
            {
                action.ActionParams.actionModifiers[0].attackRollModifier =
                    activeCondition.SourceAbilityBonus + activeCondition.SourceProficiencyBonus;
            }

            yield break;
        }

        // STEP 2: add additional dice if required
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _powerRingOfBlades;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionRingOfBlades.Name,
                    out var activeCondition))
            {
                return effectDescription;
            }

            damageForm.diceNumber = 4 + activeCondition.EffectLevel - 6;

            return effectDescription;
        }
    }

    #endregion
}
