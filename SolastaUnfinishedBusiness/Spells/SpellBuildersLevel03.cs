using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Adder's Fangs

    internal static SpellDefinition BuildAdderFangs()
    {
        const string Name = "AdderFangs";

        var movementAffinityAdderFangs = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedMultiplicativeModifier(0.5f)
            .AddToDB();

        var conditionAdderFangs = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionPoisoned, $"Condition{Name}")
            .SetPossessive()
            .AddFeatures(movementAffinityAdderFangs)
            .AddToDB();

        conditionAdderFangs.GuiPresentation.description = Gui.EmptyContent;

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.AdderFangs, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePoison, 4, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionAdderFangs, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(VenomousSpike)
                    .SetEffectEffectParameters(InflictWounds)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Blinding Smite

    internal static SpellDefinition BuildBlindingSmite()
    {
        const string NAME = "BlindingSmite";

        var conditionBlindedByBlindingSmite = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"ConditionBlindedBy{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .SetSpecialDuration(DurationType.Minute, 1)
            .SetFeatures()
            .AddToDB();

        conditionBlindedByBlindingSmite.GuiPresentation.description = "Rules/&ConditionBlindedDescription";

        var additionalDamageBlindingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetAttackModeOnly()
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D8, 3)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.None)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionBlindedByBlindingSmite,
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.EndOfTurn
                })
            // doesn't follow the standard impact particle reference
            .SetImpactParticleReference(DivineFavor.EffectDescription.EffectParticleParameters.casterParticleReference)
            .AddToDB();

        var conditionBlindingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBlindingSmite)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithMeleeAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BlindingSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBlindingSmite))
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Crusaders Mantle

    internal static SpellDefinition BuildCrusadersMantle()
    {
        const string NAME = "CrusadersMantle";

        var additionalDamageCrusadersMantle = FeatureDefinitionAdditionalDamageBuilder
            .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor, $"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(NAME)
            .SetAttackOnly()
            .SetDamageDice(DieType.D4, 1)
            .AddToDB();

        var conditionCrusadersMantle = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(additionalDamageCrusadersMantle)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite(NAME, Resources.CrusadersMantle, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCrusadersMantle))
                    .SetCasterEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Intellect Fortress

    internal static SpellDefinition BuildIntellectFortress()
    {
        const string NAME = "IntellectFortress";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionProtectedInsideMagicCircle)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB(),
                FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance)
            .SetConditionParticleReference(ConditionFeebleMinded)
            .AddToDB();

        condition.GuiPresentation.description = Gui.EmptyContent;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.IntellectFortress, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(condition))
                    .SetCasterEffectParameters(Confusion)
                    .SetImpactEffectParameters(
                        PowerMagebaneSpellCrusher.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Winter Breath

    internal static SpellDefinition BuildWinterBreath()
    {
        const string NAME = "WinterBreath";

        var spriteReference = Sprites.GetSprite(NAME, Resources.WinterBreath, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetParticleEffectParameters(ConeOfCold)
            .ExcludeCaster()
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeCold, 4, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Corrupting Bolt

    internal static SpellDefinition BuildCorruptingBolt()
    {
        const string Name = "CorruptingBolt";

        var conditionCorruptingBolt = ConditionDefinitionBuilder
            .Create(ConditionEyebiteSickened, $"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                DatabaseRepository.GetDatabase<DamageDefinition>()
                    .Select(damageDefinition =>
                        FeatureDefinitionDamageAffinityBuilder
                            .Create($"DamageAffinity{Name}{damageDefinition.Name}")
                            .SetGuiPresentationNoContent(true)
                            .SetDamageAffinityType(DamageAffinityType.Vulnerability)
                            .SetDamageType(damageDefinition.Name)
                            .AddToDB()))
            .AddCustomSubFeatures(new CustomBehaviorCorruptingBolt())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.CorruptingBolt, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeNecrotic, 4, DieType.D8),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionCorruptingBolt, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(FingerOfDeath)
                    .SetImpactEffectParameters(Disintegrate)
                    .SetEffectEffectParameters(Disintegrate)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorCorruptingBolt : IPhysicalAttackFinishedOnMe, IMagicEffectFinishedOnMe
    {
        private const string ConditionCorruptingBoltName = "ConditionCorruptingBolt";

        public IEnumerator OnMagicEffectFinishedOnMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<GameLocationCharacter> targets)
        {
            var rulesetEffect = action.ActionParams.RulesetEffect;

            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionCorruptingBoltName, out var activeCondition) &&
                !rulesetAttacker.SpellsCastByMe.Any(x => x.TrackedConditionGuids.Contains(activeCondition.Guid)))
            {
                rulesetDefender.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionCorruptingBoltName);
            }
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
            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionCorruptingBoltName);

            yield break;
        }
    }

    #endregion

    #region Ashardalon's Stride

    internal static SpellDefinition BuildAshardalonStride()
    {
        const string Name = "AshardalonStride";

        var combatAffinity = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetAttackOfOpportunityImmunity(true)
            .AddToDB();

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeFire, 1, DieType.D6),
                        EffectFormBuilder.ConditionForm(conditionMark))
                    .SetImpactEffectParameters(Fireball)
                    .Build())
            .AddToDB();

        var conditions = new List<ConditionDefinition>();

        for (var effectLevel = 3; effectLevel <= 9; effectLevel++)
        {
            var movementAffinity = FeatureDefinitionMovementAffinityBuilder
                .Create($"MovementAffinity{Name}{effectLevel}")
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(effectLevel + 1)
                .AddToDB();

            var conditionAshardalonStride = ConditionDefinitionBuilder
                .Create($"Condition{Name}{effectLevel}")
                .SetGuiPresentation(Name, Category.Spell, ConditionFreedomOfMovement)
                .SetPossessive()
                .AddFeatures(movementAffinity, combatAffinity)
                .AddCustomSubFeatures(new MoveStepFinishedAshardalonStride(powerDamage, conditionMark))
                .SetConditionParticleReference(ConditionOnFire)
                .AddToDB();

            conditionAshardalonStride.GuiPresentation.description = Gui.EmptyContent;

            conditions.Add(conditionAshardalonStride);
        }

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.AshardalonStride, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1,
                        additionalTargetCellsPerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditions[0]))
                    .SetCasterEffectParameters(FireBolt)
                    .SetImpactEffectParameters(PowerOathOfMotherlandFieryWrath
                        .EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        powerDamage.AddCustomSubFeatures(new ModifyEffectDescriptionAshardalonStride(powerDamage, spell));
        spell.AddCustomSubFeatures(new CustomBehaviorAshardalonStride([.. conditions]));

        return spell;
    }

    private sealed class ModifyEffectDescriptionAshardalonStride(
        FeatureDefinitionPower powerDamage,
        SpellDefinition spell)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.ConcentratedSpell != null &&
                character.ConcentratedSpell.SpellDefinition == spell)
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber =
                    1 + (character.ConcentratedSpell.EffectLevel - 3);
            }

            return effectDescription;
        }
    }

    private sealed class MoveStepFinishedAshardalonStride(
        FeatureDefinitionPower powerDamage,
        ConditionDefinition conditionMark) : IMoveStepStarted
    {
        public void MoveStepStarted(GameLocationCharacter mover, int3 source, int3 destination)
        {
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var targets =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .Where(x =>
                    x != mover &&
                    DistanceCalculation.GetDistanceFromCharacter(x, destination) <= 1 &&
                    !x.RulesetCharacter.HasConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionMark.Name))
                .ToArray();

            var rulesetAttacker = mover.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDamage, rulesetAttacker);

            mover.MyExecuteActionSpendPower(usablePower, targets);
        }
    }

    private sealed class CustomBehaviorAshardalonStride(params ConditionDefinition[] conditions)
        : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition =
                conditions[0];

            yield break;
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition =
                conditions[action.ActionParams.RulesetEffect.EffectLevel - 3];

            yield break;
        }
    }

    #endregion

    #region Aura of Vitality

    internal static SpellDefinition BuildAuraOfVitality()
    {
        // kept name for backward compatibility
        const string NAME = "AuraOfLife";

        var sprite = Sprites.GetSprite(NAME, Resources.AuraOfVitality, 128);

        var powerAuraOfLife = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Dice, 0, DieType.D6, 2, false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(HealingWord)
                    .Build())
            .AddToDB();

        var conditionAuraOfLifeSelf = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerAuraOfLife)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionAuraOfLife = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionAuraOfLife),
                        EffectFormBuilder.ConditionForm(
                            conditionAuraOfLifeSelf,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterAuraOfVitality(conditionAuraOfLife))
            .AddToDB();

        return spell;
    }

    private sealed class FilterTargetingCharacterAuraOfVitality(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionAuraOfVitality) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionAuraOfVitality.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustBeAuraOfLife");
            }

            return isValid;
        }
    }

    #endregion

    #region Booming Step

    internal static SpellDefinition BuildBoomingStep()
    {
        const string Name = "BoomingStep";

        var powerExplode = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Explode")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeThunder, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(Thunderwave)
                    .Build())
            .AddToDB();

        var conditionExplode = ConditionDefinitionBuilder
            .Create($"Condition{Name}Explode")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerExplode)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        powerExplode.AddCustomSubFeatures(
            new ModifyEffectDescriptionBoomingStepExplode(powerExplode, conditionExplode));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.ThunderStep, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 18, TargetType.Position)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            conditionExplode,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .InviteOptionalAlly()
                    .ExcludeCaster()
                    .SetParticleEffectParameters(Thunderwave)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorBoomingStep(powerExplode))
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorBoomingStep(FeatureDefinitionPower powerExplode)
        : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        private readonly List<GameLocationCharacter> _targets = [];

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid =
                target.RulesetCharacter is not RulesetCharacterEffectProxy &&
                __instance.ActionParams.ActingCharacter.IsWithinRange(target, 1);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustBeWithin5ft");
            }

            return isValid;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.Countered || action.ExecutionFailed)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerExplode, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, [.. _targets]);
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                Gui.Battle?.AllContenders ??
                locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

            _targets.SetRange(contenders
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    x != attacker &&
                    !action.ActionParams.TargetCharacters.Contains(x) &&
                    attacker.IsWithinRange(x, 2)));

            yield break;
        }
    }

    private sealed class ModifyEffectDescriptionBoomingStepExplode(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerExplode,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionExplode)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerExplode;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionExplode.Name, out var activeCondition))
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber = 3 + (activeCondition.EffectLevel - 3);
            }

            return effectDescription;
        }
    }

    #endregion

    #region Elemental Weapon

    internal static SpellDefinition BuildElementalWeapon()
    {
        const string NAME = "ElementalWeapon";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ElementalWeapon, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .Build())
            .SetSubSpells(DamagesAndEffects
                .Where(x => x.Item1 != DamageTypePoison)
                .Select(x => BuildElementalWeaponSubspell(x.Item1, x.Item2)).ToArray())
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildElementalWeaponSubspell(string damageType, IMagicEffect magicEffect)
    {
        var effectParticleParameters = magicEffect.EffectDescription.EffectParticleParameters;

        const string NOTIFICATION_TAG = "ElementalWeapon";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION = "Feature/&AdditionalDamageElementalWeaponDescription";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION1 = "Feature/&AdditionalDamageElementalWeapon1Description";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION2 = "Feature/&AdditionalDamageElementalWeapon2Description";

        const string ELEMENTAL_WEAPON_MODIFIER_DESCRIPTION = "Feature/&AttackModifierElementalWeaponDescription";

        var additionalDamageElementalWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 1)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var additionalDamageElementalWeapon1 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon1")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription1(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 2)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var additionalDamageElementalWeapon2 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon2")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription2(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetSpecificDamageType(damageType)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 3)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var attackModifierElementalWeapon = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                $"AttackModifier{damageType}ElementalWeapon")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(1), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var attackModifierElementalWeapon1 = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                $"AttackModifier{damageType}ElementalWeapon1")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(2), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var attackModifierElementalWeapon2 = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3,
                $"AttackModifier{damageType}ElementalWeapon2")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(3), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create($"ElementalWeapon{damageType}")
            .SetGuiPresentation(Category.Spell, MagicWeapon)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MagicWeapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.EffectLevel)
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice,
                                LevelSourceType.EffectLevel)
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1,
                                new FeatureUnlockByLevel(attackModifierElementalWeapon, 3),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon, 4),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon1, 5),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon1, 6),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 7),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 8),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 9))
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.EffectLevel)
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice,
                                LevelSourceType.EffectLevel)
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1,
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon, 3),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon, 4),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon1, 5),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon1, 6),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 7),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 8),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 9))
                            .Build())
                    .SetParticleEffectParameters(effectParticleParameters)
                    .Build())
            .AddCustomSubFeatures(TrackItemsCarefully.Marker)
            .AddToDB();

        return spell;

        static string AdditionalDamageElementalWeaponDescription(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION, x);
        }

        static string AdditionalDamageElementalWeaponDescription1(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION1, x);
        }

        static string AdditionalDamageElementalWeaponDescription2(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION2, x);
        }

        static string AttackModifierElementalWeaponDescription(int x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_MODIFIER_DESCRIPTION, x.ToString());
        }
    }

    #endregion

    #region Flame Arrows

    internal static SpellDefinition BuildFlameArrows()
    {
        const string Name = "FlameArrows";

        var conditionFlameArrows = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionFeatTakeAim)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag(Name)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D6, 1)
                    .SetSpecificDamageType(DamageTypeFire)
                    .SetImpactParticleReference(FireBolt)
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorFlameArrows())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.FlameArrows, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagRange, 0, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalItemBonus: 2)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionFlameArrows))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorFlameArrows : IOnConditionAddedOrRemoved, IPhysicalAttackFinishedByMe
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            rulesetCondition.Amount = 12 + (2 * (rulesetCondition.EffectLevel - 3));
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (attackMode is not { Ranged: true } or { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    "ConditionFlameArrows",
                    out var activeCondition))
            {
                yield break;
            }

            activeCondition.Amount--;

            if (activeCondition.Amount <= 0)
            {
                rulesetAttacker.BreakConcentration();
            }
        }
    }

    #endregion

    #region Hunger of the Void

    internal static SpellDefinition BuildHungerOfTheVoid()
    {
        const string Name = "HungerOfTheVoid";

        var powerHungerOfTheVoidDamageAcid = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Acid")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D6)
                            .Build())
                    .SetImpactEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        var powerHungerOfTheVoidDamageCold = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Cold")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, 2, DieType.D6))
                    .SetImpactEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();

        var conditionHungerOfTheVoid = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"ConditionBlindedBy{Name}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .SetFeatures()
            .AddToDB();

        conditionHungerOfTheVoid.GuiPresentation.description = "Rules/&ConditionBlindedDescription";
        conditionHungerOfTheVoid.AddCustomSubFeatures(
            new CustomBehaviorHungerOfTheVoid(
                powerHungerOfTheVoidDamageAcid, powerHungerOfTheVoidDamageCold, conditionHungerOfTheVoid));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.HungerOfTheVoid, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHungerOfTheVoid),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, true),
                        Darkness.EffectDescription.EffectForms[2],
                        Darkness.EffectDescription.EffectForms[3])
                    .SetParticleEffectParameters(Darkness)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorHungerOfTheVoid(
        FeatureDefinitionPower powerHungerOfTheVoidDamageAcid,
        FeatureDefinitionPower powerHungerOfTheVoidDamageCold,
        ConditionDefinition conditionHungerOfTheVoid) : ICharacterTurnStartListener, ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionHungerOfTheVoid.Name, out var activeCondition))
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);
            var usablePower = PowerProvider.Get(powerHungerOfTheVoidDamageAcid, rulesetCaster);

            caster.MyExecuteActionSpendPower(usablePower, character);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionHungerOfTheVoid.Name, out var activeCondition))
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);
            var usablePower = PowerProvider.Get(powerHungerOfTheVoidDamageCold, rulesetCaster);

            caster.MyExecuteActionSpendPower(usablePower, character);
        }
    }

    #endregion

    #region Lightning Arrow

    internal static SpellDefinition BuildLightningArrow()
    {
        const string Name = "LightningArrow";

        var powerLightningArrowLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Leap")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeLightning, 2, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        var conditionLightningArrow = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionFeatTakeAim)
            .SetPossessive()
            .SetFeatures(powerLightningArrowLeap)
            .AddToDB();

        conditionLightningArrow.AddCustomSubFeatures(
            AddUsablePowersFromCondition.Marker,
            new CustomBehaviorLightningArrow(powerLightningArrowLeap, conditionLightningArrow));

        powerLightningArrowLeap.AddCustomSubFeatures(
            new ModifyEffectDescriptionLightningArrowLeap(powerLightningArrowLeap, conditionLightningArrow));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.LightningArrow, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionLightningArrow))
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ModifyEffectDescriptionLightningArrowLeap(
        FeatureDefinitionPower featureDefinitionPower,
        ConditionDefinition conditionLightningArrow)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == featureDefinitionPower;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionLightningArrow.Name, out var activeCondition))
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber = 2 + (activeCondition.EffectLevel - 3);
            }

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorLightningArrow(
        FeatureDefinitionPower powerLightningArrowLeap,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionLightningArrow)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private const int MainTargetDiceNumber = 3;

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionLightningArrow.Name,
                    out var activeCondition))
            {
                yield break;
            }

            var index = actualEffectForms.FindIndex(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (index < 0)
            {
                yield break;
            }

            var diceNumber = MainTargetDiceNumber + (activeCondition.EffectLevel - 3);
            var effectForm = EffectFormBuilder.DamageForm(DamageTypeLightning, diceNumber, DieType.D8);

            effectForm.DamageForm.IgnoreCriticalDoubleDice = true;

            actualEffectForms.Insert(index + 1, effectForm);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionLightningArrow.Name,
                    out var activeCondition))
            {
                yield break;
            }

            // keep a tab on additionalDice for leap power later on
            var additionalDice = activeCondition.EffectLevel - 3;

            rulesetAttacker.RemoveCondition(activeCondition);

            // half damage on target on a miss
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                var rolls = new List<int>();
                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeLightning,
                    DieType = DieType.D8,
                    DiceNumber = MainTargetDiceNumber + additionalDice
                };
                var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
                var rulesetDefender = defender.RulesetActor;
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetAttacker,
                    targetCharacter = rulesetDefender,
                    position = defender.LocationPosition
                };

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    applyFormsParams,
                    rulesetDefender,
                    false,
                    rulesetAttacker.Guid,
                    true,
                    attackMode.AttackTags,
                    new RollInfo(damageForm.DieType, rolls, 0),
                    false,
                    out _);
            }

            // leap damage on enemies within 10 ft from target
            var usablePower = PowerProvider.Get(powerLightningArrowLeap, rulesetAttacker);
            var targets = battleManager.Battle
                .GetContenders(defender, isOppositeSide: false, withinRange: 2)
                .ToArray();

            attacker.MyExecuteActionSpendPower(usablePower, targets);
        }
    }

    #endregion

    #region Pulse Wave

    internal static SpellDefinition BuildPulseWave()
    {
        const string NAME = "PulseWave";

        var spellPush = SpellDefinitionBuilder
            .Create($"{NAME}Push")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 6)
                    .ExcludeCaster()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, dieType: DieType.D6, diceNumber: 6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerFunctionWandFearCone)
                    .SetCasterEffectParameters(Darkness)
                    .SetImpactEffectParameters(MindTwist)
                    .Build())
            .AddToDB();

        spellPush.AddCustomSubFeatures(new ModifyEffectDescriptionPulseWave(spellPush));

        var spellPull = SpellDefinitionBuilder
            .Create($"{NAME}Pull")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 6)
                    .ExcludeCaster()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, dieType: DieType.D6, diceNumber: 6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerFunctionWandFearCone)
                    .SetCasterEffectParameters(Darkness)
                    .SetImpactEffectParameters(MindTwist)
                    .Build())
            .AddToDB();

        spellPull.AddCustomSubFeatures(new ModifyEffectDescriptionPulseWave(spellPull));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PulseWave, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                // UI Only
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 6)
                    .ExcludeCaster()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, dieType: DieType.D6, diceNumber: 6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetSubSpells(spellPush, spellPull)
            .AddToDB();

        return spell;
    }

    private sealed class ModifyEffectDescriptionPulseWave(SpellDefinition spellDefinition)
        : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.SourceDefinition != spellDefinition)
            {
                yield break;
            }

            var motionEffect = actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Motion);

            if (motionEffect == null)
            {
                yield break;
            }

            motionEffect.MotionForm.distance = rulesetEffect.EffectLevel;
        }
    }

    #endregion

    #region Spirit Shroud

    private const string SpiritShroudName = "SpiritShroud";

    internal static SpellDefinition BuildSpiritShroud()
    {
        var sprite = Sprites.GetSprite(SpiritShroudName, Resources.SpiritShroud, 128);

        var hinder = ConditionDefinitionBuilder
            .Create(ConditionHindered_By_Frost, $"Condition{SpiritShroudName}Hinder")
            .SetSilent(Silent.None)
            .SetConditionType(ConditionType.Detrimental)
            .SetParentCondition(ConditionHindered)
            .CopyParticleReferences(ConditionSpiritGuardians)
            .AddToDB();

        var noHeal = ConditionDefinitionBuilder
            .Create($"Condition{SpiritShroudName}NoHeal")
            .SetGuiPresentation(Category.Condition, ConditionChilledByTouch.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create(SpiritShroudName)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
                    .Build())
            .SetSubSpells(
                BuildSpiritShroudSubSpell(DamageTypeRadiant, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeNecrotic, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeCold, hinder, noHeal, sprite))
            .AddToDB();
    }

    private static SpellDefinition BuildSpiritShroudSubSpell(
        string damage,
        ConditionDefinition hinder,
        ConditionDefinition noHeal,
        AssetReferenceSprite sprite)
    {
        return SpellDefinitionBuilder
            .Create($"{SpiritShroudName}{damage}")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2,
                        additionalDicePerIncrement: 1)
                    //RAW it should only trigger if target starts turn in the area, but game doesn't trigger on turn start for some reason without OnEnter
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetParticleEffectParameters(SpiritGuardians)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(hinder, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{SpiritShroudName}{damage}")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .CopyParticleReferences(ConditionSpiritGuardiansSelf)
                                .SetFeatures(
                                    FeatureDefinitionAdditionalDamageBuilder
                                        .Create($"AdditionalDamage{SpiritShroudName}{damage}")
                                        .SetGuiPresentationNoContent(true)
                                        .SetNotificationTag($"{SpiritShroudName}{damage}")
                                        .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsWithin10Ft)
                                        .SetAttackOnly()
                                        .SetDamageDice(DieType.D8, 1)
                                        .SetSpecificDamageType(damage)
                                        .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 0, 1, 2)
                                        .AddConditionOperation(
                                            ConditionOperationDescription.ConditionOperation.Add, noHeal)
                                        .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, true),
                        EffectFormBuilder
                            .Create()
                            .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                            .Build())
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Vitality Transfer

    internal static SpellDefinition BuildVitalityTransfer()
    {
        const string Name = "VitalityTransfer";

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.VitalityTransfer, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .ExcludeCaster()
                    .SetParticleEffectParameters(FalseLife)
                    .SetEffectEffectParameters(CureWounds)
                    .Build())
            .AddCustomSubFeatures(new ModifyDiceRollVitalityTransfer())
            .AddToDB();

        return spell;
    }

    private sealed class ModifyDiceRollVitalityTransfer : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action is not CharacterActionCastSpell actionCastSpell || action.Countered || action.ExecutionFailed)
            {
                yield break;
            }

            var caster = action.ActingCharacter;
            var rulesetCaster = caster.RulesetCharacter;
            var diceNumber = 4 + (actionCastSpell.activeSpell.EffectLevel - 3);
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeNecrotic, DiceNumber = diceNumber, DieType = DieType.D8
            };

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var target in action.ActionParams.TargetCharacters)
            {
                var rulesetTarget = target.RulesetCharacter;
                var rolls = new List<int>();
                var totalDamage = rulesetCaster.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
                var totalHealing = totalDamage * 2;
                var currentHitPoints = rulesetCaster.CurrentHitPoints;

                rulesetCaster.SustainDamage(totalDamage, damageForm.DamageType, false, rulesetCaster.Guid,
                    new RollInfo(damageForm.DieType, rolls, 0), out _);

                EffectHelpers.StartVisualEffect(caster, caster, PowerSorcererChildRiftOffering);

                rulesetCaster.DamageSustained?.Invoke(rulesetCaster, totalDamage, damageForm.DamageType, true,
                    currentHitPoints > totalDamage, false);

                rulesetTarget.ReceiveHealing(totalHealing, true, rulesetCaster.Guid);
            }
        }
    }

    #endregion
}
