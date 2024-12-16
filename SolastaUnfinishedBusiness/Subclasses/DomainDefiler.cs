using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
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

        powerDefileLife.AddCustomSubFeatures(
            new UpgradeEffectDamageBonusBasedOnClassLevel(powerDefileLife, CharacterClassDefinitions.Cleric));

        var featureSetDefileLife = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife")
            .SetGuiPresentation(
                divinePowerPrefix + powerDefileLife.FormatTitle(), powerDefileLife.FormatDescription())
            .AddFeatureSet(powerDefileLife)
            .AddToDB();

        //
        // LEVEL 6 - Beacon of Corruption
        //

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
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionInsidiousDeathMagic)
            //.AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

        // LEVEL 14

        // kept for backward compatibility
        _ = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineImmunity")
            .SetGuiPresentationNoContent(true)
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
            .AddFeaturesAtLevel(20,
                Level20SubclassesContext.PowerClericDivineInterventionImprovementPaladin)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    private static string GetAdditionalDamageType(
        // ReSharper disable once SuggestBaseTypeForParameter
        GameLocationCharacter attacker,
        DamageForm additionalDamageForm,
        // ReSharper disable once SuggestBaseTypeForParameter
        FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage)

    {
        if (additionalDamageForm.DiceNumber <= 0 && additionalDamageForm.BonusDamage <= 0)
        {
            return string.Empty;
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (featureDefinitionAdditionalDamage.AdditionalDamageType)
        {
            case AdditionalDamageType.Specific:
                return featureDefinitionAdditionalDamage.SpecificDamageType;

            case AdditionalDamageType.AncestryDamageType:
                attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(
                    FeatureDefinitionAncestry.FeaturesToBrowse);

                foreach (var definitionAncestry in FeatureDefinitionAncestry.FeaturesToBrowse
                             .Select(definition => definition as FeatureDefinitionAncestry)
                             .Where(definitionAncestry =>
                                 definitionAncestry &&
                                 definitionAncestry.Type ==
                                 featureDefinitionAdditionalDamage.AncestryTypeForDamageType &&
                                 !string.IsNullOrEmpty(definitionAncestry.DamageType)))
                {
                    return definitionAncestry.DamageType;
                }

                break;
        }

        return string.Empty;
    }

    //
    // Insidious Death Magic
    //

    private sealed class CustomBehaviorInsidiousDeathMagic(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionInsidiousDeathMagic)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IMagicEffectBeforeHitConfirmedOnEnemy
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
            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

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
            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

        private IEnumerator TryAddCondition(
            IEnumerable<EffectForm> actualEffectForms,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter defender)
        {
            if (!actualEffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType is DamageTypeNecrotic))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetActor;

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
                conditionInsidiousDeathMagic.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionInsidiousDeathMagic.Name,
                0,
                0,
                0);
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

    private sealed class CustomBehaviorDyingLight(FeatureDefinitionPower powerDyingLight)
        : IForceMaxDamageTypeDependent, IModifyAdditionalDamage, IActionFinishedByMe,
            IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var hasTag = actingCharacter.GetSpecialFeatureUses(powerDyingLight.Name) == 1;

            actingCharacter.SetSpecialFeatureUses(powerDyingLight.Name, 0);

            if (action is not (CharacterActionAttack or CharacterActionMagicEffect or CharacterActionSpendPower) ||
                !hasTag)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter.GetEffectControllerOrSelf();
            var usablePower = PowerProvider.Get(powerDyingLight, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);
        }

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetActor);

            return
                character != null &&
                character.UsedSpecialFeatures.TryGetValue(powerDyingLight.Name, out var value) &&
                value == 1 &&
                damageForm.DamageType is DamageTypeNecrotic;
        }

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
            Validate(attacker, actualEffectForms);

            yield break;
        }

        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm additionalDamageForm)
        {
            var damageType = GetAdditionalDamageType(attacker, additionalDamageForm, featureDefinitionAdditionalDamage);
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDyingLight, rulesetAttacker);
            var isValid =
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0 &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DyingLightToggle) &&
                damageType is DamageTypeNecrotic;

            if (attacker.GetSpecialFeatureUses(powerDyingLight.Name) == 1)
            {
                return;
            }

            attacker.SetSpecialFeatureUses(powerDyingLight.Name, isValid ? 1 : 0);
            rulesetAttacker.LogCharacterUsedPower(powerDyingLight);
        }

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
            Validate(attacker, actualEffectForms);

            yield break;
        }

        private void Validate(GameLocationCharacter attacker, List<EffectForm> actualEffectForms)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDyingLight, rulesetAttacker);

            var isValid =
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0 &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DyingLightToggle) &&
                actualEffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage &&
                    x.DamageForm.DamageType is DamageTypeNecrotic);

            attacker.UsedSpecialFeatures.TryAdd(powerDyingLight.Name, 0);

            if (attacker.GetSpecialFeatureUses(powerDyingLight.Name) == 1)
            {
                return;
            }

            attacker.SetSpecialFeatureUses(powerDyingLight.Name, isValid ? 1 : 0);
            rulesetAttacker.LogCharacterUsedPower(powerDyingLight);
        }
    }
}
