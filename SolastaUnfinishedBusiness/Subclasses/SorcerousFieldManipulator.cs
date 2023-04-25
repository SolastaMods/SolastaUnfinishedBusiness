using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class SorcerousFieldManipulator : AbstractSubclass
{
    private const string Name = "SorcerousFieldManipulator";

    internal SorcerousFieldManipulator()
    {
        // LEVEL 03

        var autoPreparedSpellsFieldManipulator = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Sleep)
            .AddPreparedSpellGroup(3, HoldPerson)
            .AddPreparedSpellGroup(5, Counterspell)
            .AddPreparedSpellGroup(7, Banishment)
            .AddPreparedSpellGroup(9, HoldMonster)
            .AddPreparedSpellGroup(11, GlobeOfInvulnerability)
            .AddToDB();

        var powerDisplacement = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Displacement")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(MistyStep)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Charisma,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 12)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 06

        MagicAffinityHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ArcaneManipulation")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1)
            .AddToDB();

        // LEVEL 14

        var proficiencyMentalResistance = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}MentalResistance")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        // LEVEL 18

        var powerForcefulStepApply = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepApply")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature, hidden: true)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(EldritchBlast)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionProne, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        var effectDescriptionForcefulStep = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.IndividualsUnique)
            .SetParticleEffectParameters(PowerMelekTeleport)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 24)
                    .Build())
            .Build();

        var powerForcefulStepFixed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepFixed")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature,
                PowerMonkStepOfTheWindDash)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 3)
            .SetEffectDescription(effectDescriptionForcefulStep)
            .AddToDB();

        powerForcefulStepFixed.SetCustomSubFeatures(
            new PowerUseValidityForcefulStepFixed(powerForcefulStepFixed),
            new OnAfterActionFeatureForcefulStep(powerForcefulStepApply));

        var powerForcefulStepPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepPoints")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.SorceryPoints, 4)
            .SetEffectDescription(effectDescriptionForcefulStep)
            .SetCustomSubFeatures(new OnAfterActionFeatureForcefulStep(powerForcefulStepApply))
            .AddToDB();

        powerForcefulStepPoints.SetCustomSubFeatures(
            new PowerUseValidityForcefulStepPoints(powerForcefulStepFixed));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererFieldManipulator, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsFieldManipulator,
                powerDisplacement)
            .AddFeaturesAtLevel(6,
                MagicAffinityHeightened)
            .AddFeaturesAtLevel(14,
                proficiencyMentalResistance)
            .AddFeaturesAtLevel(18,
                powerForcefulStepFixed, powerForcefulStepPoints, powerForcefulStepApply)
            .AddToDB();
    }

    private static FeatureDefinitionMagicAffinity MagicAffinityHeightened { get; set; }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        MagicAffinityHeightened.WarListSpells.SetRange(SpellListDefinitions.SpellListAllSpells
            .SpellsByLevel
            .SelectMany(x => x.Spells)
            .Where(x => x.SchoolOfMagic is SchoolEnchantement or SchoolAbjuration or SchoolIllusion)
            .Select(x => x.Name));
    }

    //
    // Forceful Step Fixed
    //

    private class PowerUseValidityForcefulStepFixed : IPowerUseValidity
    {
        private readonly FeatureDefinitionPower _powerForcefulStepFixed;

        public PowerUseValidityForcefulStepFixed(FeatureDefinitionPower powerForcefulStepFixed)
        {
            _powerForcefulStepFixed = powerForcefulStepFixed;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(_powerForcefulStepFixed, character);

            return usablePower.RemainingUses > 0;
        }
    }

    //
    // Forceful Step Points
    //

    private class PowerUseValidityForcefulStepPoints : IPowerUseValidity
    {
        private readonly FeatureDefinitionPower _powerForcefulStepFixed;

        public PowerUseValidityForcefulStepPoints(FeatureDefinitionPower powerForcefulStepFixed)
        {
            _powerForcefulStepFixed = powerForcefulStepFixed;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(_powerForcefulStepFixed, character);

            return usablePower.RemainingUses == 0;
        }
    }

    //
    // Forceful Step Apply
    //

    private sealed class OnAfterActionFeatureForcefulStep : IOnAfterActionFeature
    {
        private readonly FeatureDefinitionPower _powerApply;

        public OnAfterActionFeatureForcefulStep(FeatureDefinitionPower powerApply)
        {
            _powerApply = powerApply;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                (characterActionUsePower.activePower.PowerDefinition.Name != $"Power{Name}ForcefulStepFixed" &&
                 characterActionUsePower.activePower.PowerDefinition.Name != $"Power{Name}ForcefulStepPoints"))
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerApply, rulesetAttacker);
            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);

            foreach (var gameLocationTarget in gameLocationBattleService.Battle.EnemyContenders
                         .Where(x =>
                             gameLocationBattleService.IsWithinXCells(action.ActingCharacter, x, 2) &&
                             !x.RulesetCharacter.IsDeadOrDying))
            {
                effectPower.ApplyEffectOnCharacter(gameLocationTarget.RulesetCharacter, true,
                    gameLocationTarget.LocationPosition);
            }
        }
    }
}
