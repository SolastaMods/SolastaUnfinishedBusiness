using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainDefiler : AbstractSubclass
{
    public DomainDefiler()
    {
        const string NAME = "DomainDefiler";

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        //
        // Level 1
        //

        var autoPreparedSpellsDomainDefiler = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FalseLife, InflictWounds),
                BuildSpellGroup(3, Blindness, RayOfEnfeeblement),
                BuildSpellGroup(5, CorruptingBolt, Fear),
                BuildSpellGroup(7, Blight, PhantasmalKiller),
                BuildSpellGroup(9, CloudKill, Contagion))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        var bonusCantripDomainDefiler = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrip{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Wrack)
            .AddToDB();

        var conditionInsidiousDeathMagic = ConditionDefinitionBuilder
            .Create($"Condition{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .CopyParticleReferences(ConditionDefinitions.Condition_MummyLord_ChannelNegativeEnergy)
            .AddToDB();

        var featureInsidiousDeathMagic = FeatureDefinitionBuilder
            .Create($"Feature{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorInsidiousDeathMagic(conditionInsidiousDeathMagic))
            .AddToDB();

        //
        // Level 2
        //

        var powerDefileLife = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 2, DieType.D10)
                            .Build())
                    .Build())
            .AddToDB();

        powerDefileLife.AddCustomSubFeatures(new ModifyEffectDescriptionDefileLife(powerDefileLife));

        var featureSetDefileLife = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife")
            .SetGuiPresentation(
                divinePowerPrefix + powerDefileLife.FormatTitle(), powerDefileLife.FormatDescription())
            .AddFeatureSet(powerDefileLife)
            .AddToDB();

        //
        // LEVEL 6 - Beacon of Corruption
        //

        var conditionMarkForDeath = ConditionDefinitionBuilder
            .Create($"Condition{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDead)
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionChilled)
            .AddFeatures(
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityNecroticVulnerability")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageAffinityType(DamageAffinityType.Vulnerability)
                    .SetDamageType(DamageTypeNecrotic)
                    .AddToDB())
            .AddToDB();

        var powerMarkForDeath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerMarkForDeath", Resources.PowerMarkForDeath, 128, 64))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(Bane)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Charisma,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                conditionMarkForDeath,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}MarkForDeath")
            .SetGuiPresentation(
                divinePowerPrefix + powerMarkForDeath.FormatTitle(), powerMarkForDeath.FormatDescription())
            .AddFeatureSet(powerMarkForDeath)
            .AddToDB();

        var featureBeaconOfCorruption = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}BeaconOfCorruption")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeNecrotic)
            .AddCustomSubFeatures(new ModifyDamageAffinityBeaconsOfCorruption())
            .AddToDB();

        //
        // LEVEL 08
        //

        // Divine Strike

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionInsidiousDeathMagic,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        // Divine Resistance

        // kept for backward compatibility
        _ = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        // LEVEL 14

        // Divine Immunity

        _ = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineImmunity")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Immunity)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        // LEVEL 17

        // Dying Light

        var powerDyingLight = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DyingLight")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .AddToDB();

        powerDyingLight.AddCustomSubFeatures(new CustomBehaviorDyingLight(powerDyingLight));

        var actionAffinityDyingLightToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityDyingLightToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.DyingLightToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerDyingLight)))
            .AddToDB();

        var autoPreparedSpellsDyingLight = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}DyingLight")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(BuildSpellGroup(17, CircleOfDeath, FingerOfDeath))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Resources.DomainDefiler, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainDefiler,
                bonusCantripDomainDefiler,
                featureInsidiousDeathMagic)
            .AddFeaturesAtLevel(2,
                featureSetDefileLife)
            .AddFeaturesAtLevel(6,
                featureBeaconOfCorruption)
            .AddFeaturesAtLevel(8,
                additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10,
                PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(17,
                powerDyingLight,
                actionAffinityDyingLightToggle,
                autoPreparedSpellsDyingLight)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    //
    // Insidious Death Magic
    //

    private sealed class CustomBehaviorInsidiousDeathMagic :
        IAttackBeforeHitConfirmedOnEnemy, IMagicalAttackBeforeHitConfirmedOnEnemy
    {
        private readonly ConditionDefinition _conditionInsidiousDeathMagic;

        internal CustomBehaviorInsidiousDeathMagic(ConditionDefinition conditionInsidiousDeathMagic)
        {
            _conditionInsidiousDeathMagic = conditionInsidiousDeathMagic;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect != null)
            {
                yield break;
            }

            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

        private IEnumerator TryAddCondition(
            IEnumerable<EffectForm> actualEffectForms,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter defender)
        {
            if (actualEffectForms.All(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType != DamageTypeNecrotic))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionInsidiousDeathMagic.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionInsidiousDeathMagic.Name,
                0,
                0,
                0);
        }
    }

    //
    // Defile Life
    //

    private sealed class ModifyEffectDescriptionDefileLife : IModifyEffectDescription
    {
        private readonly BaseDefinition _baseDefinition;

        internal ModifyEffectDescriptionDefileLife(BaseDefinition baseDefinition)
        {
            _baseDefinition = baseDefinition;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == _baseDefinition;
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

            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Cleric);

            damageForm.bonusDamage = classLevel;

            return effectDescription;
        }
    }

    //
    // Beacons of Corruption
    //

    private sealed class ModifyDamageAffinityBeaconsOfCorruption : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor attacker, RulesetActor defender, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Resistance, DamageType: DamageTypeNecrotic
                });
        }
    }

    //
    // Dying Light
    //

    private sealed class CustomBehaviorDyingLight(FeatureDefinitionPower powerDyingLight) :
        IForceMaxDamageTypeDependent,
        IMagicalAttackBeforeHitConfirmedOnEnemy,
        IActionFinishedByMe
    {
        private bool _isValid;

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            _isValid = false;

            yield break;
        }

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            return damageForm.DamageType == DamageTypeNecrotic && _isValid;
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDyingLight, rulesetAttacker);

            _isValid = actualEffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                                                  x.DamageForm.DamageType == DamageTypeNecrotic) &&
                       rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DyingLightToggle) &&
                       rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0;

            if (!_isValid)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.LogCharacterUsedPower(powerDyingLight);
        }
    }
}
