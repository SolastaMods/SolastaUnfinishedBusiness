using System;
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
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static FeatureDefinitionAttributeModifier;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Mystical Cloak

    internal static SpellDefinition BuildMysticalCloak()
    {
        const string NAME = "MysticalCloak";

        var attributeModifierAC = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .AddToDB();

        var conditionLowerPlane = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}LowerPlane")
            .SetGuiPresentation($"{NAME}LowerPlane", Category.Spell,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                attributeModifierAC,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionMoveModes.MoveModeFly8,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity)
            .AddCustomSubFeatures(
                CanUseAttribute.SpellCastingAbility,
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var lowerPlane = SpellDefinitionBuilder
            .Create($"{NAME}LowerPlane")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionLowerPlane))
                    .SetCasterEffectParameters(MageArmor)
                    .Build())
            .AddToDB();

        var conditionHigherPlane = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}HigherPlane")
            .SetGuiPresentation($"{NAME}HigherPlane", Category.Spell,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                attributeModifierAC,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionMoveModes.MoveModeFly8,
                FeatureDefinitionDamageAffinitys.DamageAffinityRadiantImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity)
            .AddCustomSubFeatures(
                CanUseAttribute.SpellCastingAbility,
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var higherPlane = SpellDefinitionBuilder
            .Create($"{NAME}HigherPlane")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHigherPlane))
                    .SetCasterEffectParameters(MageArmor)
                    .Build())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MysticalCloak, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetSubSpells(lowerPlane, higherPlane)
            .AddToDB();

        return spell;
    }

    #endregion

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
                    .SetImpactEffectParameters(PowerDragonBreath_Poison)
                    .SetEffectEffectParameters(PowerVrockSpores)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Gravity Fissure

    private static SpellDefinition _gravityFissure;
    internal static SpellDefinition GravityFissure => _gravityFissure ??= BuildGravityFissure();

    private static SpellDefinition BuildGravityFissure()
    {
        const string NAME = "GravityFissure";

        var sprite = Sprites.GetSprite(NAME, Resources.GravityFissure, 128);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetDamageForm(DamageTypeForce, 8, DieType.D8)
                        .Build(),
                    EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetMotionForm(MotionForm.MotionType.DragToOrigin, VerticalPushPullMotion.PullOntoCaster)
                        .Build())
                .SetImpactEffectParameters(EldritchBlast)
                .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ForcePushOrDragFromEffectPoint.Marker, new ModifyEffectDescriptionGravityFissure(power));

        var spell = SpellDefinitionBuilder
            .Create($"{NAME}")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(EffectDescriptionBuilder.Create(Earthquake)
                // only required to get the SFX in this particular scenario to activate
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Line, 12, onlyGround: true)
                .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                // only required to get the SFX in this particular scenario to activate
                .SetRecurrentEffect(RecurrentEffect.OnActivation)
                .SetEffectForms(EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                        .SetDamageForm(DamageTypeForce, 8, DieType.D8)
                        .Build(),
                    // only required to get the SFX in this particular scenario to activate
                    // dangerous zone won't be enforced here as not a concentration spell
                    EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false))
                .SetImpactEffectParameters(EldritchBlast)
                .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeGravityFissure(power))
            .AddToDB();

        return spell;
    }

    private sealed class ModifyEffectDescriptionGravityFissure(FeatureDefinitionPower powerDrag)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDrag;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (rulesetEffect is RulesetEffectPower rulesetEffectPower)
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber =
                    8 + (rulesetEffectPower.usablePower.spentPoints - 6);
            }

            return effectDescription;
        }
    }

    internal sealed class PowerOrSpellFinishedByMeGravityFissure(FeatureDefinitionPower powerDrag)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var positioningCharacterService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var dummy = locationCharacterService.DummyCharacter;

            // collect all covered positions
            var actionParams = action.ActionParams;
            var positions = GetAffectedPositions(action.ActingCharacter, actionParams.RulesetEffect,
                actionParams.hasMagneticTargeting, actionParams.Positions[0], actionParams.CursorHoveredPosition,
                positioningCharacterService);

            // collect all contenders that should be dragged
            var targets = GetPullTargets(actingCharacter, positions, locationCharacterService)
                .ToDictionary(x => x, x => GetPositionForGravityFissure(x, positions, positioningCharacterService));

            // issue drag to origin powers to all contenders with a non placeholder position
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDrag, rulesetCharacter);

            // use spentPoints to store effect level to be used later by power
            usablePower.spentPoints = actionParams.RulesetEffect.EffectLevel;

            // drag each contender to the selected position starting with the ones closer to the line
            foreach (var x in targets
                         .Where(x => x.Value != int3.invalid)
                         .OrderBy(x => int3.Distance(x.Key.LocationPosition, x.Value)))
            {
                var pullParams = new CharacterActionParams(actingCharacter, Id.SpendPower)
                {
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect =
                        implementationService.InstantiateEffectPower(rulesetCharacter, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { x.Key },
                    Positions = { x.Value }
                };

                actionService.ExecuteInstantSingleAction(pullParams);
            }

            // clean up the house as a good guest
            dummy.LocationPosition = int3.invalid;

            yield break;
        }

        internal static List<GameLocationCharacter> GetPullTargets(
            GameLocationCharacter caster, ICollection<int3> affectedPositions,
            IGameLocationCharacterService characterService
        )
        {
            var dummy = characterService.DummyCharacter;

            var targets = (Gui.Battle?.AllContenders ??
                           characterService.PartyCharacters.Union(characterService.GuestCharacters))
                .Where(x =>
                    // don't include caster
                    x != caster
                    && Valid(affectedPositions.Select(p => DistanceCalculation.GetDistanceFromCharacter(x, p)))
                ).ToList();

            dummy.LocationPosition = int3.zero;
            return targets;

            static bool Valid(IEnumerable<float> values)
            {
                var flag = false;

                foreach (var value in values)
                {
                    switch (value)
                    {
                        // don't include affected contenders
                        case < 1:
                            return false;
                        // include actions within 2 cells range
                        case <= 2:
                            flag = true;
                            break;
                    }
                }

                return flag;
            }
        }

        internal static int3 GetPositionForGravityFissure(GameLocationCharacter target,
            List<int3> affectedPositions, IGameLocationPositioningService positioningService)
        {
            //maybe better try to get line from caster to cursor point from params?
            var line = Vector3.zero;
            if (affectedPositions.Count >= 2)
            {
                var a = affectedPositions.First();
                var b = affectedPositions.Last();
                a.y = b.y = 0;

                if (a != b)
                {
                    line = (a - b).ToVector3();
                }
            }

            var center = positioningService.ComputeGravityCenterPosition(target) - CursorMotionHelper.Center;
            return affectedPositions
                .OrderBy(Value)
                .FirstOrDefault();

            float Value(int3 p)
            {
                var v = p.ToVector3() - center;
                var result = v.magnitude;
                if (line == Vector3.zero) { return result; }

                v.y = 0;
                return (1000000 * result) + Math.Abs(Vector3.Angle(line, v) - 90);
            }
        }

        private static List<int3> GetAffectedPositions(
            GameLocationCharacter actingCharacter,
            RulesetEffect rulesetEffect,
            bool hasMagneticTargeting,
            int3 worldPoint,
            Vector3 cursorHoveredPosition,
            IGameLocationPositioningService positioningService)
        {
            var targetingService = ServiceRepository.GetService<IGameLocationTargetingService>();
            var origin = new Vector3();
            var direction = new Vector3();
            List<int3> positions = [];
            List<GameLocationCharacter> affectedCharacters = [];


            var effectDescription = rulesetEffect.EffectDescription;
            var castingPosition = actingCharacter.LocationPosition;
            var useFloatingImpactPoint =
                IsUsingFloatingImpactPoint(effectDescription.RangeType, effectDescription.TargetType);

            var impactPoint = !hasMagneticTargeting && useFloatingImpactPoint
                ? cursorHoveredPosition
                : positioningService.GetWorldPositionFromGridPosition(worldPoint);

            targetingService.ComputeTargetingParameters(
                impactPoint,
                actingCharacter,
                castingPosition,
                MetricsDefinitions.GeometricShapeType.Line,
                effectDescription.RangeType,
                ref origin,
                ref direction);

            targetingService.ComputeTargetsOfAreaOfEffect(
                origin,
                direction,
                impactPoint,
                MetricsDefinitions.GeometricShapeType.Line,
                actingCharacter.Side,
                effectDescription,
                rulesetEffect.ComputeTargetParameter(),
                rulesetEffect.ComputeTargetParameter2(),
                affectedCharacters,
                false,
                actingCharacter,
                coveredFloorPositions: positions,
                groundOnly: true);

            return [.. positions.OrderBy(a => int3.Distance(castingPosition, a))];
        }
    }

    #endregion

    #region Shelter From Energy

    private static readonly List<(FeatureDefinitionDamageAffinity, IMagicEffect, AssetReference)> ShelterDamageTypes =
    [
        (FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance, AcidArrow,
            PowerDragonbornBreathWeaponBlack.EffectDescription.EffectParticleParameters.impactParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, SleetStorm,
            PowerBulette_Snow_Leap.EffectDescription.EffectParticleParameters.impactParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance, HeatMetal,
            FireStorm.EffectDescription.EffectParticleParameters.impactParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance, LightningBolt,
            Thunderstorm.EffectDescription.EffectParticleParameters.impactParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance, FingerOfDeath,
            PowerPatronFiendDarkOnesOwnLuck.EffectDescription.EffectParticleParameters.effectParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance, GuardianOfFaith,
            PowerOathOfJugementPurgeCorruption.EffectDescription.EffectParticleParameters.effectParticleReference),
        (FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance, Thunderwave,
            Thunderwave.EffectDescription.EffectParticleParameters.impactParticleReference)
    ];

    internal static SpellDefinition BuildShelterFromEnergy()
    {
        const string NAME = "ShelterFromEnergy";

        var subSpells = new List<SpellDefinition>();

        foreach (var (damageAffinity, casterEffect, impactEffect) in ShelterDamageTypes)
        {
            var damageType = damageAffinity.Name.Replace("Affinity", string.Empty).Replace("Resistance", string.Empty);
            var title = Gui.Localize($"Tooltip/&Tag{damageType}Title");
            var description = Gui.Format($"Feedback/&{NAME}Description", title);

            subSpells.Add(
                SpellDefinitionBuilder
                    .Create($"Spell{NAME}{damageType}")
                    .SetGuiPresentation(title, description)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
                    .SetSpellLevel(6)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.Mundane)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Buff)
                    .SetEffectDescription(EffectDescriptionBuilder.Create()
                        .SetDurationData(DurationType.Hour, 1)
                        .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalTargetsPerIncrement: 1)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{NAME}{damageType}")
                                .SetGuiPresentation(
                                    Gui.Format($"Condition/&Condition{NAME}Title", title),
                                    Gui.EmptyContent,
                                    ConditionAuraOfProtection)
                                .SetPossessive()
                                .SetFeatures(damageAffinity)
                                .AddToDB()))
                        .SetCasterEffectParameters(casterEffect)
                        .SetImpactEffectParameters(impactEffect)
                        .Build())
                    .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                    .AddToDB());
        }

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ShelterFromEnergy, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .Build())
            .SetSubSpells([.. subSpells])
            .AddToDB();

        return spell;
    }

    #endregion

    #region Fizban Platinum Shield

    internal static SpellDefinition BuildFizbanPlatinumShield()
    {
        const string NAME = "FizbanPlatinumShield";

        var conditionSelf = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, "UI/&HasHalfCover")
                    .SetPermanentCover(CoverType.Half)
                    .AddToDB())
            .SetConditionParticleReference(WardingBond)
            .AddToDB();

        conditionMark.GuiPresentation.description = Gui.EmptyContent;

        var lightSourceForm =
            SpellDefinitions.Light.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PrimordialWard, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Light)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionMark),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            conditionSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetCasterEffectParameters(PrismaticSpray)
                    .Build())
            .AddToDB();

        var behavior = new PowerOrSpellFinishedByMeFizbanPlatinumShield(spell, conditionMark);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, spell)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(spell)
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(behavior, new FilterTargetingCharacter(conditionMark))
            .AddToDB();

        conditionSelf.Features.Add(power);
        spell.AddCustomSubFeatures(behavior);

        return spell;
    }

    private sealed class FilterTargetingCharacter(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionMark.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustNotHaveFizbanPlatinumShield");
            }

            return isValid;
        }
    }

    private sealed class PowerOrSpellFinishedByMeFizbanPlatinumShield(
        SpellDefinition spell,
        ConditionDefinition condition) : IPowerOrSpellFinishedByMe
    {
        private const string FizbanPlatinumShieldTag = "FizbanPlatinumShieldTag";

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var rulesetSpell = rulesetCharacter.ConcentratedSpell;

            if (rulesetSpell == null ||
                rulesetSpell.SpellDefinition != spell)
            {
                yield break;
            }

            switch (action)
            {
                case CharacterActionUsePower:
                {
                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, 0);
                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, rulesetSpell.RemainingRounds);

                    var spellRepertoire = rulesetSpell.SpellRepertoire;

                    // rulesetCharacter.SpellsCastByMe.TryAdd(effectSpell);

                    action.ActingCharacter.MyExecuteActionCastNoCost(spell, 6, action.ActionParams, spellRepertoire);

                    break;
                }
                case CharacterActionCastSpell
                    when actingCharacter.UsedSpecialFeatures.TryGetValue(FizbanPlatinumShieldTag, out var value) &&
                         value > 0:
                    rulesetSpell.RemainingRounds = value;

                    if (action.ActionParams.TargetCharacters[0].RulesetActor.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect, condition.Name, out var activeCondition))
                    {
                        activeCondition.RemainingRounds = value;
                    }

                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, 0);
                    break;
            }
        }
    }

    #endregion

    #region Flash Freeze

    internal static SpellDefinition BuildFlashFreeze()
    {
        const string NAME = "FlashFreeze";

        var battlePackage = AiHelpers.BuildDecisionPackageBreakFree($"Condition{NAME}");

        var conditionFlashFreeze = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(
                RuleDefinitions.ConditionRestrained, Category.Rules, ConditionDefinitions.ConditionChilled)
            .SetConditionType(ConditionType.Detrimental)
            .SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .SetPossessive()
            .SetFixedAmount((int)AiHelpers.BreakFreeType.DoStrengthCheckAgainstCasterDC)
            .SetBrain(battlePackage, true)
            .SetFeatures(ActionAffinityGrappled)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.FlashFreeze, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeCold, 10, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionFlashFreeze, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsCold)
                    .SetCasterEffectParameters(SleetStorm)
                    .SetConditionEffectParameters(ConditionDefinitions.ConditionRestrained)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterFlashFreeze())
            .AddToDB();

        return spell;
    }

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class FilterTargetingCharacterFlashFreeze : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var rulesetCharacter = target.RulesetCharacter;
            var isValid = rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.DragonSize
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.Gargantuan
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.Huge
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.SpiderQueenSize;

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustBeLargeOrSmaller");
            }

            return isValid;
        }
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
                Id.CastBonus, Id.CastInvocation, Id.CastMain, Id.CastReaction, Id.CastReadied, Id.CastRitual,
                Id.CastNoCost)
            .AddToDB();

        var conditionExhausted = ConditionDefinitionBuilder
            .Create(ConditionExhausted, $"Condition{NAME}Exhausted")
            .SetOrUpdateGuiPresentation("ConditionExhausted", Category.Rules, ConditionLethargic)
            .AddToDB();

        var conditionHeroicInfusion = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Minute, 10)
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
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHeroicInfusion),
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(50)
                            .Build())
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedHeroicInfusion(ConditionDefinition conditionExhausted)
        : IOnConditionAddedOrRemoved
    {
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

            if (savingOutcome == RollOutcome.Success)
            {
                return;
            }

            target.InflictCondition(
                conditionExhausted.Name,
                conditionExhausted.DurationType,
                conditionExhausted.DurationParameter,
                conditionExhausted.TurnOccurence,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                1,
                conditionExhausted.Name,
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
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 4, DieType.D10))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetCasterEffectParameters(PowerDomainLawWordOfLaw)
                    .Build())
            .AddToDB();

        var powerRingOfBladesFree = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Free")
            .SetGuiPresentation($"Power{NAME}", Category.Feature,
                Sprites.GetSprite($"Power{NAME}", Resources.PowerRingOfBlades, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None, 1, 6)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 4, DieType.D10))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetCasterEffectParameters(PowerDomainLawWordOfLaw)
                    .Build())
            .AddToDB();

        var conditionRingOfBlades = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation($"Power{NAME}", Category.Feature, ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerRingOfBlades)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .CopyParticleReferences(PowerSorcererChildRiftDeflection)
            .AddToDB();

        conditionRingOfBlades.GuiPresentation.description = Gui.EmptyContent;

        var behavior = new ModifyEffectDescriptionRingOfBlades(powerRingOfBlades, conditionRingOfBlades);

        powerRingOfBlades.AddCustomSubFeatures(behavior);
        powerRingOfBladesFree.AddCustomSubFeatures(behavior);

        var conditionRingOfBladesFree = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Free")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerRingOfBladesFree)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        powerRingOfBladesFree.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            // it's indeed powerRingOfBlades here
            new PowerOrSpellFinishedByMeRingOfBladesFree(powerRingOfBlades, conditionRingOfBladesFree));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.RingOfBlades, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
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
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionRingOfBlades),
                        EffectFormBuilder.ConditionForm(conditionRingOfBladesFree))
                    .SetParticleEffectParameters(HypnoticPattern)
                    .SetEffectEffectParameters(PowerMagebaneSpellCrusher)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class PowerOrSpellFinishedByMeRingOfBladesFree(
        FeatureDefinitionPower powerRingOfBlades,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionRingOfBladesFree) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerRingOfBlades, rulesetCharacter);

            rulesetCharacter.UsePower(usablePower);

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionRingOfBladesFree.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class ModifyEffectDescriptionRingOfBlades(
        FeatureDefinitionPower powerRingOfBlades,
        ConditionDefinition conditionRingOfBlades) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerRingOfBlades;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionRingOfBlades.Name, out var activeCondition))
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber = 4 + (activeCondition.EffectLevel - 6);
            }

            return effectDescription;
        }
    }

    #endregion

    #region Scatter

    internal static SpellDefinition BuildScatter()
    {
        const string NAME = "Scatter";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite($"Power{NAME}", Resources.Scatter, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Detection)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Position)
                    .InviteOptionalAlly()
                    .SetSavingThrowData(true, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 6)
                            .Build())
                    .SetParticleEffectParameters(DimensionDoor)
                    .Build())
            .AddCustomSubFeatures(new ModifyTeleportEffectBehaviorScatter())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.targetParticleReference = new AssetReference();

        return spell;
    }

    private sealed class ModifyTeleportEffectBehaviorScatter : IModifyTeleportEffectBehavior
    {
        public bool AllyOnly => false;

        public bool TeleportSelf => false;

        public int MaxTargets => 5;
    }

    #endregion
}
