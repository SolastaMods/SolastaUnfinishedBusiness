using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal static class CommonBuilders
{
    internal static readonly FeatureDefinitionAdditionalDamage AdditionalDamageMarshalFavoredEnemyHumanoid =
        FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageMarshalFavoredEnemyHumanoid")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("FavoredEnemy")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpecificCharacterFamily)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetRequiredCharacterFamily(Humanoid)
            .AddToDB();

    internal static readonly FeatureDefinitionAttributeModifier AttributeModifierCasterFightingExtraAttack =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierCasterFightingExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

    internal static readonly FeatureDefinitionAttributeModifier AttributeModifierThirdExtraAttack =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierThirdExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 3)
            .AddToDB();

    internal static readonly FeatureDefinitionDamageAffinity DamageAffinityGenericHardenToNecrotic =
        FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticImmunity, "DamageAffinityGenericHardenToNecrotic")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetCasterFightingProficiency =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetCasterFightingProficiency")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyCasterFightingArmor")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Armor,
                        EquipmentDefinitions.LightArmorCategory,
                        EquipmentDefinitions.MediumArmorCategory,
                        EquipmentDefinitions.ShieldCategory)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyCasterFightingWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Weapon,
                        EquipmentDefinitions.SimpleWeaponCategory,
                        EquipmentDefinitions.MartialWeaponCategory)
                    .AddToDB())
            .AddToDB();

    internal static readonly FeatureDefinitionMagicAffinity MagicAffinityCasterFightingCombatMagic =
        FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCasterFightingCombatMagic")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .AddToDB();

    internal static readonly FeatureDefinitionMagicAffinity MagicAffinityCasterFightingCombatMagicImproved =
        FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCasterFightingCombatMagicImproved")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
            .AddToDB();

    // kept as power for backward compatibility
    internal static readonly FeatureDefinitionPower PowerCasterFightingWarMagic = FeatureDefinitionPowerBuilder
        .Create("PowerCasterFightingWarMagic")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorCasterFightingWarMagic(
                ConditionDefinitionBuilder
                    .Create("ConditionCasterFightingWarMagic")
                    .SetGuiPresentation("PowerCasterFightingWarMagic", Category.Feature)
                    .SetSilent(Silent.WhenRemoved)
                    .SetPossessive()
                    .AddFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create("PowerCasterFightingWarMagicAttack")
                            .SetGuiPresentation("PowerCasterFightingWarMagic", Category.Feature, Gui.NoLocalization)
                            .SetDamageRollModifier(1)
                            .AddCustomSubFeatures(new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus))
                            .AddToDB())
                    .AddToDB()))
        .AddToDB();

    internal static readonly FeatureDefinitionPower PowerCasterCommandUndead = FeatureDefinitionPowerBuilder
        .Create("PowerCasterCommandUndead")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerCommandUndead", Resources.PowerCommandUndead, 256, 128))
        .SetUsesProficiencyBonus(ActivationTime.Action)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(DominateBeast.EffectDescription)
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetRestrictedCreatureFamilies(Undead.Name)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Charisma,
                    false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .Build())
        .AddToDB();

    internal static readonly FeatureDefinition AttackReplaceWithCantripCasterFighting =
        FeatureDefinitionBuilder
            .Create("ReplaceAttackWithCantripCasterFighting")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new AttackReplaceWithCantrip())
            .AddToDB();

    //
    // Enchant Weapon
    //

    internal static bool CanWeaponBeEnchanted(RulesetAttackMode mode, RulesetItem _, RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero || ValidatorsWeapon.HasTwoHandedTag(mode))
        {
            return false;
        }

        return mode.ActionType != ActionDefinitions.ActionType.Bonus ||
               ValidatorsWeapon.IsPolearmType(mode) ||
               ValidatorsWeapon.IsTwoHandedRanged(mode) ||
               hero.TrainedFeats.Contains(MeleeCombatFeats.FeatFencer) ||
               hero.TrainedFightingStyles.Contains(DatabaseHelper.FightingStyleDefinitions.TwoWeapon);
    }

    private sealed class AttackReplaceWithCantrip : IAttackReplaceWithCantrip;

    private sealed class CustomBehaviorCasterFightingWarMagic(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition) : IMagicEffectFinishedByMeAny, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.ActionParams.activeEffect is RulesetEffectSpell rulesetEffectSpell &&
                (Main.Settings.EnableCantripsTriggeringOnWarMagic ||
                 rulesetEffectSpell.SpellDefinition.SpellLevel > 0))
            {
                yield return TryAddCondition(attacker.RulesetCharacter);
            }
        }

        //supports Sunlit Blade and Resonating Strike
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
            if (Main.Settings.EnableCantripsTriggeringOnWarMagic &&
                attackMode.AttackTags.Contains(SpellBuilders.PhysicalAttackFromCantrip))
            {
                yield return TryAddCondition(attacker.RulesetCharacter);
            }
        }

        private IEnumerator TryAddCondition(RulesetCharacter rulesetAttacker)
        {
            if (!rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDefinition.Name))
            {
                rulesetAttacker.InflictCondition(
                    conditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionDefinition.Name,
                    0,
                    0,
                    0);
            }

            yield break;
        }
    }

    internal class ModifyWeaponAttackUnarmedStrikeDamage(DieType dieType) : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (damage.DieType < dieType)
            {
                damage.DieType = dieType;
            }
        }
    }

    internal class AddExtraUnarmedStrikeClawAttack() : AddExtraAttackBase(ActionDefinitions.ActionType.None)
    {
        // process sub feature after unarmed strikes
        public override int Priority()
        {
            return 1;
        }

        // move before base unarmed strikes are added
        protected override AttackModeOrder GetOrder(RulesetCharacter character)
        {
            return AttackModeOrder.Start;
        }

        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return null;
            }

            var attackModifiers = hero.attackModifiers;

            // find attack mode that is unarmed strike
            var attackModes = new List<RulesetAttackMode>();

            hero.attackModes.ForEach(attackMode =>
            {
                if (attackMode.SourceDefinition != hero.UnarmedStrikeDefinition)
                {
                    return;
                }

                var newAttackMode = character.TryRefreshAttackMode(
                    attackMode.actionType,
                    CustomWeaponsContext.UnarmedStrikeClaws,
                    CustomWeaponsContext.UnarmedStrikeClaws.WeaponDescription,
                    ValidatorsCharacter.IsFreeOffhandVanilla(character),
                    true,
                    attackMode.SlotName,
                    attackModifiers,
                    character.FeaturesOrigin
                );

                newAttackMode.attacksNumber = attackMode.attacksNumber;
                attackModes.Add(newAttackMode);
            });

            return attackModes;
        }
    }
}
