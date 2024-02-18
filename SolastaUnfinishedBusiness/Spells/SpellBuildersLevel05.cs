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
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Far Step

    internal static SpellDefinition BuildFarStep()
    {
        var condition = ConditionDefinitionBuilder
            .Create("ConditionFarStep")
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(CustomActionIdContext.FarStep)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("FarStep")
            .SetGuiPresentation(Category.Spell, Sprites.FarStep)
            .SetSpellLevel(5)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                            .Build())
                    .SetParticleEffectParameters(MistyStep)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Mantle of Thorns

    internal static SpellDefinition BuildMantleOfThorns()
    {
        const string NAME = "MantleOfThorns";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MantleOfThorns, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(SpikeGrowth)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnMove | RecurrentEffect.OnTurnStart)
                    .AddEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePiercing, 2, DieType.D8),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DifficultThrough, false))
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sonic Boom

    internal static SpellDefinition BuildSonicBoom()
    {
        const string NAME = "SonicBoom";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 6)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
            .SetParticleEffectParameters(Thunderwave)
            .SetSavingThrowData(
                false, AttributeDefinitions.Strength, false, EffectDifficultyClassComputation.SpellCastingFeature)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeThunder, 6, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        effectDescription.EffectParticleParameters.zoneParticleReference =
            Shatter.EffectDescription.EffectParticleParameters.zoneParticleReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SonicBoom, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(effectDescription)
            .AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Incineration

    internal static SpellDefinition BuildIncineration()
    {
        const string NAME = "Incineration";

        var lightSourceForm = FaerieFire.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

        var conditionIncineration = ConditionDefinitionBuilder
            .Create(ConditionOnFire, $"Condition{NAME}")
            .SetSpecialInterruptions(ConditionInterruption.Revive)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 8, DieType.D6)
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

        conditionIncineration.specialDuration = false;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Immolation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeFire, 8, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionIncineration, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6, lightSourceForm.Color,
                                lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(Fireball)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Divine Wrath

    internal static SpellDefinition BuildDivineWrath()
    {
        const string NAME = "DivineWrath";

        var spellRadiant = SpellDefinitionBuilder
            .Create($"{NAME}Radiant")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeRadiant, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        spellRadiant.EffectDescription.EffectParticleParameters.impactParticleReference =
            Sunburst.EffectDescription.EffectParticleParameters.impactParticleReference;

        var spellNecrotic = SpellDefinitionBuilder
            .Create($"{NAME}Necrotic")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSubSpells(spellNecrotic, spellRadiant)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Steel Whirlwind

    internal static SpellDefinition BuildSteelWhirlwind()
    {
        const string Name = "SteelWhirlwind";

        var powerTeleport = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Teleport")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerMelekTeleport)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerMelekTeleport)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingPositionSteelWhirlwind())
            .AddToDB();

        var conditionTeleport = ConditionDefinitionBuilder
            .Create($"Condition{Name}Teleport")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.UsedActionOrReaction, ConditionInterruption.Moved)
            .SetFeatures(powerTeleport)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.SteelWhirlwind, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique, 5)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeForce, 6, DieType.D10),
                        EffectFormBuilder.ConditionForm(
                            conditionTeleport, ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(GravitySlam)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeSteelWhirlwind())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            ArcaneSword.EffectDescription.EffectParticleParameters.impactParticleReference;

        return spell;
    }

    // keep a tab of all allowed positions for filtering using ContextualFormation collection
    // ContextualFormation is only used by the game when spawning new locations so it's safe in this context
    private sealed class MagicEffectFinishedByMeSteelWhirlwind : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var actingCharacter = action.ActingCharacter;

            actingCharacter.contextualFormation = [];

            foreach (var boxInt in action.ActionParams.TargetCharacters
                         .Select(targetCharacter => new BoxInt(
                             targetCharacter.LocationPosition, new int3(-1, -1, -1), new int3(1, 1, 1))))
            {
                foreach (var position in boxInt.EnumerateAllPositionsWithin())
                {
                    if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                        !positioningService.CanPlaceCharacter(
                            actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                        !positioningService.CanCharacterStayAtPosition_Floor(
                            actingCharacter, position, onlyCheckCellsWithRealGround: true))
                    {
                        continue;
                    }

                    actingCharacter.ContextualFormation.Add(position);
                }
            }

            yield break;
        }
    }

    private sealed class FilterTargetingPositionSteelWhirlwind : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var source = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            cursorLocationSelectPosition.validPositionsCache.SetRange(source.ContextualFormation);

            yield break;
        }
    }

    #endregion

    #region Banishing Smite

    internal static SpellDefinition BuildBanishingSmite()
    {
        const string NAME = "BanishingSmite";

        var conditionBanishingSmiteEnemy = ConditionDefinitionBuilder
            .Create(ConditionBanished, $"Condition{NAME}Enemy")
            .SetSpecialDuration(DurationType.Minute, 1)
            .AddToDB();

        var additionalDamageBanishingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .AddCustomSubFeatures(
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack,
                new PhysicalAttackFinishedByMeBanishingSmite(conditionBanishingSmiteEnemy))
            .SetDamageDice(DieType.D10, 5)
            .SetSpecificDamageType(DamageTypeForce)
            // doesn't follow the standard impact particle reference
            .SetImpactParticleReference(Banishment.EffectDescription.EffectParticleParameters.effectParticleReference)
            .AddToDB();

        var conditionBanishingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBanishingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ThunderousSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(5)
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
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBanishingSmite))
                    .SetParticleEffectParameters(Banishment)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class PhysicalAttackFinishedByMeBanishingSmite(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.CurrentHitPoints > 50)
            {
                yield break;
            }

            //TODO: ideally we need to banish extra planar creatures forever (kill them?)
            rulesetDefender.InflictCondition(
                conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Circle of Magical Negation

    internal static SpellDefinition BuildCircleOfMagicalNegation()
    {
        const string NAME = "CircleOfMagicalNegation";

        var savingThrowAffinityCircleOfMagicalNegation = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionCircleOfMagicalNegation = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionShielded)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(savingThrowAffinityCircleOfMagicalNegation)
            .AddToDB();

        conditionCircleOfMagicalNegation.GuiPresentation.Description = Gui.NoLocalization;

        conditionCircleOfMagicalNegation.AddCustomSubFeatures(
            new MagicEffectBeforeHitConfirmedOnMeCircleOfMagicalNegation(conditionCircleOfMagicalNegation));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.CircleOfMagicalNegation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCircleOfMagicalNegation))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class MagicEffectBeforeHitConfirmedOnMeCircleOfMagicalNegation(
        ConditionDefinition conditionCircleOfMagicalNegation) :
        IMagicEffectBeforeHitConfirmedOnMe, IRollSavingThrowFinished
    {
        private RollOutcome _saveOutcome;

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (_saveOutcome != RollOutcome.Success)
            {
                yield break;
            }

            actualEffectForms.RemoveAll(x =>
                x.HasSavingThrow
                && x.FormType == EffectForm.EffectFormType.Damage
                && x.SavingThrowAffinity == EffectSavingThrowType.HalfDamage);

            defender.RulesetCharacter.LogCharacterAffectedByCondition(conditionCircleOfMagicalNegation);
        }

        public void OnSavingThrowFinished(
            RulesetCharacter caster,
            RulesetCharacter defender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            _saveOutcome = outcome;
        }
    }

    #endregion

    #region Telekinesis

    private const int TelekinesisRange = 12;

    internal static SpellDefinition BuildTelekinesis()
    {
        const string Name = "Telekinesis";

        var sprite = Sprites.GetSprite(Name, Resources.Telekinesis, 128, 128);

        var powerTelekinesis = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, TelekinesisRange, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerSpellBladeSpellTyrant)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        var conditionTelekinesis = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionRevealedByDetectGoodOrEvil)
            .SetPossessive()
            .SetFeatures(powerTelekinesis)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var powerTelekinesisNoCost = FeatureDefinitionPowerBuilder
            .Create(powerTelekinesis, $"Power{Name}NoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionTelekinesisNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}NoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerTelekinesisNoCost)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTelekinesis),
                        EffectFormBuilder.ConditionForm(conditionTelekinesisNoCost))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerSpellBladeSpellTyrant)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        var conditionRestrained = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, $"Condition{Name}Restrained")
            .SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .SetFeatures()
            .AddToDB();

        var customBehavior = new CustomBehaviorTelekinesis(conditionRestrained, conditionTelekinesisNoCost, spell);

        powerTelekinesis.AddCustomSubFeatures(customBehavior);
        powerTelekinesisNoCost.AddCustomSubFeatures(customBehavior, ValidatorsValidatePowerUse.InCombat);

        return spell;
    }

    private sealed class CustomBehaviorTelekinesis(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionRestrained,
        ConditionDefinition conditionTelekinesisNoCost,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellTelekinesis)
        : IMagicEffectFinishedByMe, IFilterTargetingPosition, ISelectPositionAfterCharacter
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var targetCharacter = cursorLocationSelectPosition.ActionParams.TargetCharacters[0];
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var boxInt = new BoxInt(targetCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(TelekinesisRange, 0, TelekinesisRange);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        targetCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        targetCharacter, position, onlyCheckCellsWithRealGround: true) ||
                    DistanceCalculation.GetDistanceFromPositions(actingCharacter.LocationPosition, position) > 12)
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var actingRulesetCharacter = actingCharacter.RulesetCharacter;

            if (actingRulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionTelekinesisNoCost.Name, out var activeTelekinesis))
            {
                actingRulesetCharacter.RemoveCondition(activeTelekinesis);
            }

            var rulesetSpell =
                actingRulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellTelekinesis);

            if (rulesetSpell == null)
            {
                yield break;
            }

            RemoveExistingRestrainedInstances(actingRulesetCharacter, rulesetSpell, conditionRestrained);

            var targetCharacter = action.ActionParams.TargetCharacters[0];

            yield return RollAbilityCheckAndTryMoveApplyRestrained(
                actingRulesetCharacter,
                targetCharacter,
                conditionRestrained,
                rulesetSpell,
                action);
        }

        public int PositionRange => TelekinesisRange;

        private static void RemoveExistingRestrainedInstances(
            // ReSharper disable once SuggestBaseTypeForParameter
            RulesetCharacter rulesetCaster,
            RulesetEffect rulesetEffect,
            // ReSharper disable once SuggestBaseTypeForParameter
            ConditionDefinition conditionRestrained)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            foreach (var rulesetContender in Gui.Battle.EnemyContenders
                         .Select(locationContender => locationContender.RulesetCharacter))
            {
                if (!rulesetContender.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                        conditionRestrained.Name, out var activeCondition) ||
                    activeCondition.SourceGuid != rulesetCaster.Guid)
                {
                    continue;
                }

                rulesetEffect.TrackedConditionGuids.Remove(activeCondition.Guid);
                rulesetContender.RemoveCondition(activeCondition);

                break;
            }
        }

        private static IEnumerator RollAbilityCheckAndTryMoveApplyRestrained(
            RulesetCharacter actingRulesetCharacter,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter targetCharacter,
            // ReSharper disable once SuggestBaseTypeForParameter
            ConditionDefinition conditionRestrained,
            RulesetEffectSpell rulesetSpell,
            CharacterAction action)
        {
            var isEnemy = actingRulesetCharacter.Side != targetCharacter.Side;

            if (isEnemy)
            {
                var checkDC = action is CharacterActionCastSpell actionCastSpell
                    ? actionCastSpell.ActiveSpell.SaveDC
                    : actingRulesetCharacter.SpellsCastByMe
                        .FirstOrDefault(x => x.SpellDefinition == rulesetSpell.SpellDefinition)?.SaveDC ?? 0;

                targetCharacter.RollAbilityCheck(
                    AttributeDefinitions.Strength, string.Empty, checkDC, AdvantageType.None, new ActionModifier(),
                    false, -1, out var outcome, out _, true);

                if (outcome == RollOutcome.Success)
                {
                    yield break;
                }
            }

            var actionParams = new CharacterActionParams(targetCharacter, ActionDefinitions.Id.SpiritRallyTeleport)
            {
                Positions = { action.ActionParams.Positions[0] }
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);

            if (!isEnemy)
            {
                yield break;
            }

            var targetRulesetCharacter = targetCharacter.RulesetCharacter;
            var activeCondition = targetRulesetCharacter.InflictCondition(
                conditionRestrained.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                actingRulesetCharacter.guid,
                actingRulesetCharacter.CurrentFaction.Name,
                1,
                conditionRestrained.Name,
                0,
                0,
                0);

            rulesetSpell.TrackedConditionGuids.Add(activeCondition.Guid);
        }
    }

    #endregion
}
