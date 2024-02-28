using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
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

        var dieRollModifierDemonSlayerPhysics = FeatureDefinitionDieRollModifierBuilder
            .Create($"Feature{Name}DemonSlayerPhysics")
            .SetGuiPresentation(DEMON_SLAYER_NAME, Category.Feature, hidden: true)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 3, 3, $"Feature/&DieRollModifier{Name}DemonSlayer")
            .AddCustomSubFeatures(ValidateDieRollModifierDemonSlayerDamageTypeRadiant.Marker)
            .AddToDB();

        var dieRollModifierDemonSlayerMagic = FeatureDefinitionDieRollModifierBuilder
            .Create($"Feature{Name}DemonSlayerMagic")
            .SetGuiPresentation(DEMON_SLAYER_NAME, Category.Feature, hidden: true)
            .SetModifiers(RollContext.MagicDamageValueRoll, 1, 3, 3, $"Feature/&DieRollModifier{Name}DemonSlayer")
            .AddCustomSubFeatures(ValidateDieRollModifierDemonSlayerDamageTypeRadiant.Marker)
            .AddToDB();

        var featureSetDemonSlayer = FeatureDefinitionFeatureSetBuilder
            .Create(DEMON_SLAYER_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                dieRollModifierDemonSlayerPhysics,
                dieRollModifierDemonSlayerMagic)
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

    internal static IsWeaponValidHandler IsOathOfDemonHunterWeapon { get; } =
        ValidatorsWeapon.IsOfWeaponType(LightCrossbowType, HeavyCrossbowType, CustomWeaponsContext.HandXbowWeaponType);

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

            if (!IsOathOfDemonHunterWeapon(attackMode, null, null))
            {
                yield break;
            }

            if (!battleManager.IsBattleInProgress)
            {
                yield break;
            }

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionService == null)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.HasConditionOfType(conditionTrialMark))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerUses(powerTrialMark) == 0)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerTrialMark, rulesetAttacker);
            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "LightEnergyCrossbowBolt",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = gameLocationActionService.PendingReactionRequestGroups.Count;

            gameLocationActionService.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionService, count);
        }
    }

    private sealed class ModifyCrossbowAttackModeDivineCrossbow : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!IsOathOfDemonHunterWeapon(attackMode, null, null))
            {
                return;
            }

            attackMode.maxRange += 6;
        }
    }

    private sealed class ValidateDieRollModifierDemonSlayerDamageTypeRadiant : IValidateDieRollModifier
    {
        internal static readonly ValidateDieRollModifierDemonSlayerDamageTypeRadiant Marker = new();

        public bool CanModifyRoll(RulesetCharacter character, List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return damageTypes.Contains(DamageTypeRadiant);
        }
    }
}
