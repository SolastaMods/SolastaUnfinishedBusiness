using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
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
                BuildSpellGroup(7, AuraOfVitality, FreedomOfMovement),
                BuildSpellGroup(9, GreaterRestoration, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attributeModifierForestGuardianSylvanDurability = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        #region

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedBarkWard")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 2, DieType.D8)
                            .Build())
                    .Build())
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SuperiorBarkWard")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 2, DieType.D8)
                            .Build())
                    .Build())
            .AddToDB();

        #endregion

        var conditionBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}BarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .AddToDB();

        var conditionImprovedBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionDamageAffinityBuilder
                    .Create($"DamageAffinity{Name}ImprovedBarkWard")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageAffinityType(DamageAffinityType.Immunity)
                    .SetDamageType(DamageTypePoison)
                    .AddToDB())
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
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(4)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionBarkWard))
                    .Build())
            .AddToDB();

        powerBarkWard.AddCustomSubFeatures(
            new ModifyEffectDescriptionBarkWard(powerBarkWard));

        powerBarkWard.EffectDescription.EffectParticleParameters.casterParticleReference =
            SpikeGrowth.EffectDescription.EffectParticleParameters.casterParticleReference;

        var powerImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(4)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionImprovedBarkWard))
                    .Build())
            .SetOverriddenPower(powerBarkWard)
            .AddToDB();

        powerImprovedBarkWard.AddCustomSubFeatures(
            new BeforeHitConfirmedOnMeBarkWard(powerImprovedBarkWard),
            new ModifyEffectDescriptionBarkWard(powerImprovedBarkWard));

        powerImprovedBarkWard.EffectDescription.EffectParticleParameters
                .casterParticleReference =
            SpikeGrowth.EffectDescription.EffectParticleParameters.casterParticleReference;

        // kept as power for backward compatibility
        var powerSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}SuperiorBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        conditionBarkWard.AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard));
        conditionImprovedBarkWard.AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"CircleOfThe{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.CircleOfTheForestGuardian, 256))
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierForestGuardianSylvanDurability,
                powerBarkWard)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                powerImprovedBarkWard)
            .AddFeaturesAtLevel(14,
                powerSuperiorBarkWard)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static int HitPoints(RulesetCharacter character)
    {
        var levels = character.GetClassLevel(CharacterClassDefinitions.Druid);
        var hitPoints = levels switch
        {
            >= 14 => 10,
            >= 10 => 8,
            >= 6 => 6,
            _ => 4
        };

        return hitPoints;
    }

    //
    // Bark Ward
    //

    private sealed class BeforeHitConfirmedOnMeBarkWard(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerBarkOrImprovedBarkWard)
        : IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe, IActionFinishedByEnemy
    {
        private bool _shouldTrigger;

        public IEnumerator OnActionFinishedByEnemy(CharacterAction action, GameLocationCharacter target)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetAttacker = actingCharacter.RulesetCharacter;
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (_shouldTrigger &&
                rulesetAttacker is { IsDeadOrDyingOrUnconscious: false } &&
                target.RulesetCharacter.TemporaryHitPoints > 0 &&
                gameLocationBattleService is {IsBattleInProgress: true} &&
                gameLocationBattleService.IsWithin1Cell(target, actingCharacter))
            {
                InflictDamage(actingCharacter, target);
            }

            _shouldTrigger = false;

            yield break;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
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
            if (attackMode != null)
            {
                _shouldTrigger = true;
            }

            yield break;
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            _shouldTrigger = true;

            yield break;
        }

        private void InflictDamage(GameLocationCharacter attacker, GameLocationCharacter me)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetMe = me.RulesetCharacter;

            var rolls = new List<int>();
            var damageForm = new DamageForm
            {
                DamageType = DamageTypePiercing, DieType = DieType.D8, DiceNumber = 2, BonusDamage = 0
            };
            var damageRoll =
                rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

            rulesetMe.LogCharacterUsedPower(powerBarkOrImprovedBarkWard);
            EffectHelpers.StartVisualEffect(
                me, attacker, PowerPatronTreeExplosiveGrowth, EffectHelpers.EffectType.Caster);
            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                damageForm.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetAttacker },
                rulesetAttacker,
                false,
                rulesetAttacker.Guid,
                false,
                [],
                new RollInfo(damageForm.DieType, rolls, 0),
                true,
                out _);
        }
    }

    private sealed class ModifyEffectDescriptionBarkWard(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerBarkOrImprovedBarkWard) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerBarkOrImprovedBarkWard;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var hitPoints = HitPoints(character);

            effectDescription.EffectForms[0].TemporaryHitPointsForm.bonusHitPoints = hitPoints;

            return effectDescription;
        }
    }

    private sealed class CharacterTurnStartListenerBarkWard(FeatureDefinitionPower powerSuperiorBarkWard)
        : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var hitPoints = HitPoints(rulesetCharacter);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                hitPoints, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);

            var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Druid);
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (levels < 14 || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedPower(powerSuperiorBarkWard);

            foreach (var ally in locationCharacter.PerceivedAllies
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     gameLocationBattleService.IsWithinXCells(locationCharacter, x, 6))
                         .ToList())
            {
                ally.RulesetCharacter.ReceiveTemporaryHitPoints(
                    hitPoints, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
            }
        }
    }
}
