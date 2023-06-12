using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfThunder : AbstractSubclass
{
    internal const string Name = "OathOfThunder";

    internal static readonly IsWeaponValidHandler IsValidWeapon = (mode, item, character) =>
        character.GetSubclassLevel(CharacterClassDefinitions.Paladin, Name) >= 7
            ? mode.thrown && ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.BattleaxeType,
                    WeaponTypeDefinitions.WarhammerType)
                (mode, item, character)
            : mode.thrown &&
              ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.WarhammerType)(mode, item, character);

    internal OathOfThunder()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("Subclass/&OathOfThunderTitle", "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Thunderwave, SpellsContext.ThunderousSmite),
                BuildSpellGroup(5, Shatter, MistyStep),
                BuildSpellGroup(9, CallLightning, LightningBolt),
                BuildSpellGroup(13, FreedomOfMovement, Stoneskin),
                BuildSpellGroup(17, SpellsContext.FarStep, SpellsContext.SonicBoom))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        // Hammer's Boon

        var featureHammersBoon = FeatureDefinitionBuilder
            .Create($"Feature{Name}HammersBoon")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureHammersBoon.SetCustomSubFeatures(
            ReturningWeapon.Instance,
            new ModifyWeaponAttackModeHammerAndAxeBoon(featureHammersBoon));

        // ThunderousRebuke

        var powerThunderousRebuke = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ThunderousRebuke")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ChannelDivinity)
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(ShockingGrasp)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 6)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new ModifyMagicEffectThunderousRebuke())
            .AddToDB();

        // Divine Bolt

        var movementAffinityDivineBolt = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}DivineBolt")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedMultiplicativeModifier(0.5f)
            .AddToDB();

        var conditionDivineBolt = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLuminousKi, $"Condition{Name}DivineBolt")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShocked)
            .SetPossessive()
            .SetFeatures(movementAffinityDivineBolt)
            .AddToDB();

        var powerDivineBolt = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DivineBolt")
            .SetGuiPresentation(Category.Feature, PowerRangerPrimevalAwareness)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(CallLightning)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D6)
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice,
                                LevelSourceType.ClassLevelHalfUp)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDivineBolt, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Axe's Boom

        var featureAxesBoon = FeatureDefinitionBuilder
            .Create($"Feature{Name}AxesBoon")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfThunder, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureHammersBoon,
                powerThunderousRebuke,
                powerDivineBolt)
            .AddFeaturesAtLevel(7,
                featureAxesBoon)
            .AddFeaturesAtLevel(15)
            .AddFeaturesAtLevel(20)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyWeaponAttackModeHammerAndAxeBoon : IModifyWeaponAttackMode, IAttackComputeModifier
    {
        private readonly FeatureDefinition _featureHammersBoon;

        public ModifyWeaponAttackModeHammerAndAxeBoon(FeatureDefinition featureHammersBoon)
        {
            _featureHammersBoon = featureHammersBoon;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (IsValidWeapon(attackMode, null, myself))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.CharacterFeature, _featureHammersBoon.Name, _featureHammersBoon));
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!IsValidWeapon(attackMode, null, character))
            {
                return;
            }

            attackMode.thrown = true;
            attackMode.closeRange = 4;
            attackMode.maxRange = 12;
        }
    }

    private sealed class ModifyMagicEffectThunderousRebuke : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damage = effectDescription.FindFirstDamageForm();

            if (damage != null)
            {
                damage.bonusDamage = character.GetClassLevel(CharacterClassDefinitions.Paladin);
            }

            return effectDescription;
        }
    }
}
