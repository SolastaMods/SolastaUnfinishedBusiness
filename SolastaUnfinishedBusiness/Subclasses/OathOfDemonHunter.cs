using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class OathOfDemonHunter : AbstractSubclass
{
    internal const string Name = "OathOfDemonHunter";

    internal static readonly IsWeaponValidHandler IsOathOfDemonHunterWeapon = (mode, item, character) =>
    {
        var levels = character.GetSubclassLevel(CharacterClassDefinitions.Paladin, Name);

        return levels switch
        {
            >= 3 => ValidatorsWeapon
                .IsOfWeaponType(LightCrossbowType, HeavyCrossbowType, CustomWeaponsContext.HandXbowWeaponType)
                (mode, item, character),
            _ => false
        };
    };

    public OathOfDemonHunter()
    {
        //
        // LEVEL 03
        //

        // auto prepared spells

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Name, Category.Subclass, "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, HuntersMark, ProtectionFromEvilGood),
                BuildSpellGroup(5, MagicWeapon, MistyStep),
                BuildSpellGroup(9, Haste, DispelMagic),
                BuildSpellGroup(13, GuardianOfFaith, GreaterInvisibility),
                BuildSpellGroup(17, HoldMonster, DispelEvilAndGood))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        // Demon Hunter - Level up UI only

        var featureDemonHunter = FeatureDefinitionBuilder
            .Create($"Feature{Name}DemonHunter")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Trial Mark

        var conditionTrialMark = ConditionDefinitionBuilder
            .Create(ConditionMarkedByHunter, $"Condition{Name}TrialMark")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();

        var additionalDamageTrialMark = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}TrialMark")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("TrialMark")
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
            .SetTargetCondition(conditionTrialMark, AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetImpactParticleReference(
                FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.impactParticleReference)
            //.AddCustomSubFeatures(ClassHolder.Paladin)
            .AddToDB();

        var powerTrialMark = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TrialMark")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerTrialMark", Resources.PowerTrialMark, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(LightningBolt)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTrialMark, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // LEVEL 07
        //

        // Light Energy Crossbow Bolt

        var lightEnergyCrossbowBolt = FeatureDefinitionBuilder
            .Create($"Feature{Name}LightEnergyCrossbowBolt")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new RemoveRangedAttackInMeleeDisadvantage(IsOathOfDemonHunterWeapon),
                new PhysicalAttackFinishedByMeLightEnergyCrossbowBolt(conditionTrialMark, powerTrialMark))
            .AddToDB();

        //
        // LEVEL 15
        //

        // Extra Attack (3)

        // Divine Crossbow

        var attributeModifierDivineCrossbow = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DivineCrossbow")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyCrossbowAttackModeDivineCrossbow())
            .SetModifier(AttributeModifierOperation.Additive,
                AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();

        //
        // LEVEL 20
        //

        // Demon Slayer

        const string DEMON_SLAYER_NAME = $"FeatureSet{Name}DemonSlayer";

        var dieRollModifierDemonSlayer = FeatureDefinitionDieRollModifierBuilder
            .Create($"Feature{Name}DemonSlayer")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(RollContext.AttackDamageValueRoll | RollContext.MagicDamageValueRoll, 1, 0, 3,
                "Feedback/&OathOfDemonHunterDemonSlayerReroll")
            .AddCustomSubFeatures(ValidateDieRollModifierDemonSlayerDamageTypeRadiant.Marker)
            .AddToDB();

        var featureSetDemonSlayer = FeatureDefinitionFeatureSetBuilder
            .Create(DEMON_SLAYER_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(dieRollModifierDemonSlayer)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDemonHunter, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureDemonHunter,
                additionalDamageTrialMark,
                powerTrialMark)
            .AddFeaturesAtLevel(7,
                lightEnergyCrossbowBolt)
            .AddFeaturesAtLevel(15,
                CommonBuilders.AttributeModifierThirdExtraAttack,
                attributeModifierDivineCrossbow)
            .AddFeaturesAtLevel(20,
                featureSetDemonSlayer)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class PhysicalAttackFinishedByMeLightEnergyCrossbowBolt(
        ConditionDefinition conditionTrialMark,
        FeatureDefinitionPower powerTrialMark)
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
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!IsOathOfDemonHunterWeapon(attackMode, null, rulesetAttacker))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.HasConditionOfType(conditionTrialMark))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerTrialMark, rulesetAttacker);

            if (rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return attacker.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                [defender],
                attacker,
                "LightEnergyCrossbowBolt");
        }
    }

    private sealed class ModifyCrossbowAttackModeDivineCrossbow : IModifyWeaponAttackMode
    {
        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!IsOathOfDemonHunterWeapon(attackMode, null, character))
            {
                return;
            }

            attackMode.maxRange += 6;
        }
    }

    private sealed class ValidateDieRollModifierDemonSlayerDamageTypeRadiant : IValidateDieRollModifier
    {
        internal static readonly ValidateDieRollModifierDemonSlayerDamageTypeRadiant Marker = new();

        public bool CanModifyRoll(
            RulesetCharacter character, List<FeatureDefinition> features, List<string> damageTypes)
        {
            return damageTypes.Contains(DamageTypeRadiant);
        }
    }
}
