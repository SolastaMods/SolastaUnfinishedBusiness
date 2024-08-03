using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheBattlerager : AbstractSubclass
{
    private const string Name = "PathOfTheBattlerager";

    public PathOfTheBattlerager()
    {
        // Level 3

        var proficiencyBattleragerArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}BattleragerArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddCustomSubFeatures(new ModifyAttackModeBattleragerArmor())
            .AddToDB();

        var abilityCheckAffinityRagingBattlerager = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionRaging,
                $"AbilityCheckAffinity{Name}RagingBattleRager")
            .AddToDB();

        abilityCheckAffinityRagingBattlerager.affinityGroups[0].abilityCheckContext = AbilityCheckContext.None;

        var damageAffinityRagingBattleragerBludgeoning = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingBludgeoning,
                $"DamageAffinity{Name}RagingBattleragerBludgeoning")
            .AddToDB();

        damageAffinityRagingBattleragerBludgeoning.situationalContext = SituationalContext.None;

        var damageAffinityRagingBattleragerPiercing = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingPiercing,
                $"DamageAffinity{Name}RagingBattleragerPiercing")
            .AddToDB();

        damageAffinityRagingBattleragerPiercing.situationalContext = SituationalContext.None;

        var damageAffinityRagingBattleragerSlashing = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingSlashing,
                $"DamageAffinity{Name}RagingBattleragerSlashing")
            .AddToDB();

        damageAffinityRagingBattleragerSlashing.situationalContext = SituationalContext.None;

        var additionalDamageRagingBattlerager = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}RagingBattlerager")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(FeatureDefinitionAdditionalDamages.AdditionalDamageConditionRaging.NotificationTag)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.RageDamage)
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty(
                    (_, _, rulesetCharacter, _, _, mode, _) =>
                        (OperationType.Set, rulesetCharacter.IsWearingHeavyArmor() && mode is { Ranged: false })))
            .AddToDB();

        var conditionRagingBattlerager = ConditionDefinitionBuilder
            .Create($"Condition{Name}RagingBattlerager")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                abilityCheckAffinityRagingBattlerager,
                damageAffinityRagingBattleragerBludgeoning,
                damageAffinityRagingBattleragerPiercing,
                damageAffinityRagingBattleragerSlashing,
                additionalDamageRagingBattlerager)
            .AddCustomSubFeatures(
                new AddExtraUnarmedAttack(ActionType.Bonus, ValidatorsCharacter.HasArmor))
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerRagingBattlerager = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RagingBattlerager")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionRagingBattlerager))
                    .Build())
            .AddToDB();

        var featureSetBattleragerArmor = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BattleragerArmor")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(proficiencyBattleragerArmor, powerRagingBattlerager)
            .AddToDB();

        // Level 6

        var featureRecklessAbandon = FeatureDefinitionBuilder
            .Create($"Feature{Name}RecklessAbandon")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ActionFinishedByMeRecklessAbandon())
            .AddToDB();

        // Level 10

        var conditionBattleragerCharge = ConditionDefinitionBuilder
            .Create($"Condition{Name}BattleragerCharge")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityAggressive)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerBattleragerCharge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BattleragerCharge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBattleragerCharge))
                    .Build())
            .AddToDB();

        // Level 14

        var powerArmoredRetributionRetaliate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArmoredRetributionRetaliate")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.TurnStart)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Individuals)
                    .UseQuickAnimations()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 0, DieType.D1, 3)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.MagicMissile)
                    .Build())
            .AddToDB();

        var damageAffinityArmoredRetribution = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}ArmoredRetribution")
            .SetGuiPresentationNoContent(true)
            .SetRetaliate(powerArmoredRetributionRetaliate, 1)
            .AddToDB();

        damageAffinityArmoredRetribution.retaliateProximity = AttackProximity.Melee;
        damageAffinityArmoredRetribution.situationalContext = SituationalContext.WearingArmor;

        var conditionArmoredRetribution = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArmoredRetribution")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(damageAffinityArmoredRetribution)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerArmorRetribution = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArmoredRetribution")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionArmoredRetribution))
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheBattlerager, 256))
            .AddFeaturesAtLevel(3, featureSetBattleragerArmor)
            .AddFeaturesAtLevel(6, featureRecklessAbandon)
            .AddFeaturesAtLevel(10, powerBattleragerCharge)
            .AddFeaturesAtLevel(14, powerArmorRetribution)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class ActionFinishedByMeRecklessAbandon : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionRecklessAttack)
            {
                yield break;
            }

            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;

            if (!rulesetCharacter.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                yield break;
            }

            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);

            EffectHelpers.StartVisualEffect(
                character, character, SpellDefinitions.CureWounds, EffectHelpers.EffectType.Effect);
            rulesetCharacter.ReceiveTemporaryHitPoints(
                constitutionModifier, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid);
        }
    }

    private class ModifyAttackModeBattleragerArmor : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            if (!character.IsWearingArmor())
            {
                return;
            }

            if (character is RulesetCharacterHero hero)
            {
                var equipedItem =
                    hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

                if (equipedItem != null && equipedItem.ItemDefinition.IsArmor && equipedItem.ItemDefinition.Magical)
                {
                    attackMode.attackTags.Add(TagsDefinitions.MagicalWeapon);
                }
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if ((int)damage.DieType < 3)
            {
                damage.DieType = DieType.D6;
            }

            if (damage.DiceNumber < 1)
            {
                damage.DiceNumber = 1;
            }
        }
    }
}
