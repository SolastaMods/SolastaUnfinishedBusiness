using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string Name = "ForestGuardian";

    public CircleOfTheForestGuardian()
    {
        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, EnsnaringStrike, Shield),
                BuildSpellGroup(3, LesserRestoration, SpikeGrowth),
                BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                BuildSpellGroup(7, AuraOfLife, FreedomOfMovement),
                BuildSpellGroup(9, GreaterRestoration, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attackModifierSylvanMagic = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}SylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Wisdom, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
            .AddToDB();

        // kept name for backward compatibility
        var powerSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}SuperiorBarkWard")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var conditionBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}BarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetCancellingConditions(
                DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                    x.IsSubtypeOf(ConditionIncapacitated)).ToArray())
            .AddCancellingConditions(ConditionDefinitions.ConditionCharmedByHypnoticPattern)
            .AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard))
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        var conditionImprovedBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetCancellingConditions(
                DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                    x.IsSubtypeOf(ConditionIncapacitated)).ToArray())
            .AddCancellingConditions(ConditionDefinitions.ConditionCharmedByHypnoticPattern)
            .SetParentCondition(conditionBarkWard)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity)
            .AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard))
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        var powerBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}BarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBarkWard))
                    .SetCasterEffectParameters(SpikeGrowth)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeBarkWard(powerSuperiorBarkWard))
            .AddToDB();

        var powerImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionImprovedBarkWard))
                    .SetCasterEffectParameters(SpikeGrowth)
                    .Build())
            .SetOverriddenPower(powerBarkWard)
            .AddToDB();

        var powerImprovedBarkWardDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedBarkWardDamage")
            .SetGuiPresentation($"PowerSharedPool{Name}ImprovedBarkWard", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypePiercing, 2, DieType.D8))
                    .SetImpactEffectParameters(PowerPatronTreeExplosiveGrowth)
                    .Build())
            .AddToDB();

        powerImprovedBarkWard.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMeBarkWard(powerSuperiorBarkWard),
            new PhysicalAttackFinishedOnMeImprovedBarkWard(powerImprovedBarkWardDamage, conditionBarkWard));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"CircleOfThe{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.CircleOfTheForestGuardian, 256))
            .AddFeaturesAtLevel(2, autoPreparedSpellsForestGuardian, attackModifierSylvanMagic, powerBarkWard)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10, powerImprovedBarkWard, powerImprovedBarkWardDamage)
            .AddFeaturesAtLevel(14, powerSuperiorBarkWard)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static void ApplyTemporaryHitPoints(
        GameLocationCharacter locationCharacter,
        FeatureDefinitionPower powerSuperiorBarkWard)
    {
        var rulesetCharacter = locationCharacter.RulesetCharacter;
        var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Druid);
        var hitPoints = levels switch
        {
            >= 14 => 10,
            >= 10 => 8,
            >= 6 => 6,
            _ => 4
        };

        rulesetCharacter.ReceiveTemporaryHitPoints(
            hitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);

        if (levels < 14)
        {
            return;
        }

        if (Gui.Battle == null)
        {
            return;
        }

        rulesetCharacter.LogCharacterUsedPower(powerSuperiorBarkWard);

        foreach (var ally in Gui.Battle
                     .GetContenders(locationCharacter, isOppositeSide: false, withinRange: 3))
        {
            ally.RulesetCharacter.ReceiveTemporaryHitPoints(
                hitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }
    }

    private sealed class CharacterTurnStartListenerBarkWard(FeatureDefinitionPower powerSuperiorBarkWard)
        : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            ApplyTemporaryHitPoints(locationCharacter, powerSuperiorBarkWard);
        }
    }

    private sealed class PowerOrSpellFinishedByMeBarkWard(FeatureDefinitionPower powerSuperiorBarkWard)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            ApplyTemporaryHitPoints(action.ActingCharacter, powerSuperiorBarkWard);

            yield break;
        }
    }

    private sealed class PhysicalAttackFinishedOnMeImprovedBarkWard(
        FeatureDefinitionPower powerImprovedBarkWardDamage,
        ConditionDefinition conditionBarkWard) : IPhysicalAttackFinishedOnMe
    {
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

            if (action.AttackRoll == 0 ||
                action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                rulesetDefender.TemporaryHitPoints <= 0 ||
                !rulesetDefender.HasConditionOfTypeOrSubType(conditionBarkWard.Name) ||
                !defender.IsWithinRange(attacker, 1))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerImprovedBarkWardDamage, rulesetDefender);

            // improved bark ward damage is a use at will power
            defender.MyExecuteActionSpendPower(usablePower, attacker);
        }
    }
}
