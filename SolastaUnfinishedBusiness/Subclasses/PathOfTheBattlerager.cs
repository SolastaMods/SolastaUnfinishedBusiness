using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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
public sealed class PathOfTheBattlerager : AbstractSubclass
{
    internal const string Name = "PathOfTheBattlerager";

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    internal override DeityDefinition DeityDefinition { get; }

    public PathOfTheBattlerager()
    {
        // Level 3
        var proficiencyBattleragerArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}BattleragerArmor")
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddCustomSubFeatures(new ModifyAttackModeBattleragerArmor())
            .AddToDB();

        var abilityCheckAffinityRagingBattlerager = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionRaging, $"AbilityCheckAffinity{Name}RagingBattleRager")
            .AddToDB();

        abilityCheckAffinityRagingBattlerager.affinityGroups[0].abilityCheckContext = AbilityCheckContext.None;

        var damageAffinityRagingBattleragerBludgeoning = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingBludgeoning, $"DamageAffinity{Name}RagingBattleragerBludgeoning")
            .AddToDB();
        damageAffinityRagingBattleragerBludgeoning.situationalContext = SituationalContext.None;
        var damageAffinityRagingBattleragerPiercing = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingBludgeoning, $"DamageAffinity{Name}RagingBattleragerPiercing")
            .AddToDB();
        damageAffinityRagingBattleragerPiercing.situationalContext = SituationalContext.None;
        var damageAffinityRagingBattleragerSlashing = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityConditionRagingBludgeoning, $"DamageAffinity{Name}RagingBattleragerSlashing")
            .AddToDB();
        damageAffinityRagingBattleragerSlashing.situationalContext = SituationalContext.None;

        var additionalDamageRagingBattlerager = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}RagingBattlerager")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(FeatureDefinitionAdditionalDamages.AdditionalDamageConditionRaging.NotificationTag)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.RageDamage)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .AddCustomSubFeatures(new ValidateContextInsteadOfRestrictedProperty(
                (_, _, rulesetCharacter, _, rangedAttack, mode, effect) =>
                    (OperationType.Set, rulesetCharacter.IsWearingHeavyArmor() && mode != null && !mode.Ranged)
             ))
            .AddToDB();

        var conditionRagingBattlerager = ConditionDefinitionBuilder
            .Create($"Condition{Name}RagingBattlerager")
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(abilityCheckAffinityRagingBattlerager,
                damageAffinityRagingBattleragerBludgeoning,
                damageAffinityRagingBattleragerPiercing,
                damageAffinityRagingBattleragerSlashing,
                additionalDamageRagingBattlerager)
            .AddCustomSubFeatures(
                new AddExtraUnarmedAttack(ActionType.Bonus, ValidatorsCharacter.HasArmor))
            .AddToDB();

        var powerRagingBattlerager = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RagingBattlerager")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                .SetNoSavingThrow()
                .SetDurationData(DurationType.UntilLongRest)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(conditionRagingBattlerager)
                )
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
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityAggressive)
            .AddToDB();

        var powerBattleragerCharge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BattleragerCharge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                .SetNoSavingThrow()
                .SetDurationData(DurationType.UntilLongRest)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(conditionBattleragerCharge)
                )
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
                    .SetDurationData(DurationType.Irrelevant, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Individuals, 1)
                    .SetNoSavingThrow()
                    .UseQuickAnimations()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 0, DieType.D1, 3)
                            .Build())
                    .UseQuickAnimations()
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
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(damageAffinityArmoredRetribution)
            .AddToDB();

        var powerArmorRetribution = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArmoredRetribution")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Individuals)
                .SetNoSavingThrow()
                .SetDurationData(DurationType.UntilLongRest)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(conditionArmoredRetribution)
                )
                .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("PathOfTheBattlerager", Resources.PathOfTheBattlerager, 256))
            .AddFeaturesAtLevel(3, featureSetBattleragerArmor)
            .AddFeaturesAtLevel(6, featureRecklessAbandon)
            .AddFeaturesAtLevel(10, powerBattleragerCharge)
            .AddFeaturesAtLevel(14, powerArmorRetribution)
            .AddToDB();
    }

    private class ActionFinishedByMeRecklessAbandon : IActionFinishedByMe
    {

        public ActionFinishedByMeRecklessAbandon()
        {
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            var character = characterAction.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;

            if (characterAction.ActionId != Id.RecklessAttack)
            {
                yield break;
            }

            if (!rulesetCharacter.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                yield break;
            }

            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);

            EffectHelpers.StartVisualEffect(character, character, SpellDefinitions.CureWounds,
                EffectHelpers.EffectType.Effect);
            rulesetCharacter.ReceiveTemporaryHitPoints(
                constitutionModifier, DurationType.Minute, 0, TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid);
        }
    }

    private class ModifyAttackModeBattleragerArmor : IModifyWeaponAttackMode
    {
        public ModifyAttackModeBattleragerArmor()
        {
        }

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
            
            var damage = attackMode?.EffectDescription.FindFirstDamageForm();
            if (character is RulesetCharacterHero hero) {
                RulesetItem equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;
                if (equipedItem != null && equipedItem.ItemDefinition.IsArmor)
                {
                    if (equipedItem.ItemDefinition.Magical)
                    {
                        attackMode.attackTags.Add("MagicalWeapon");
                    }
                }
            }


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
