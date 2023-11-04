using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using SolastaUnfinishedBusiness.Models;

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

    // LEFT AS A POWER FOR BACKWARD COMPATIBILITY
    internal static readonly FeatureDefinitionPower PowerCasterFightingWarMagic = FeatureDefinitionPowerBuilder
        .Create("PowerCasterFightingWarMagic")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new MagicalAttackBeforeHitConfirmedOnEnemyCasterFightingWarMagic(
                ConditionDefinitionBuilder
                    .Create("ConditionCasterFightingWarMagic")
                    .SetGuiPresentation("PowerCasterFightingWarMagic", Category.Feature)
                    .SetSilent(Silent.WhenRemoved)
                    .SetPossessive()
                    .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
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

    internal static readonly FeatureDefinitionAttackReplaceWithCantrip AttackReplaceWithCantripCasterFighting =
        FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripCasterFighting")
            .SetGuiPresentation(Category.Feature)
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

    private sealed class
        MagicalAttackBeforeHitConfirmedOnEnemyCasterFightingWarMagic :
            IMagicalAttackBeforeHitConfirmedOnEnemy, IAttackBeforeHitConfirmedOnEnemy
    {
        private readonly ConditionDefinition _conditionDefinition;

        public MagicalAttackBeforeHitConfirmedOnEnemyCasterFightingWarMagic(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        //supports Sunlit Blade and Resonating Strike
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
            if (!Main.Settings.EnableCantripsTriggeringOnWarMagic)
            {
                yield break;
            }

            yield return TryAddCondition(attacker);
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
            if (rulesetEffect is not RulesetEffectSpell rulesetEffectSpell)
            {
                yield break;
            }

            if (rulesetEffectSpell.SpellDefinition.SpellLevel == 0 && !Main.Settings.EnableCantripsTriggeringOnWarMagic)
            {
                yield break;
            }

            yield return TryAddCondition(attacker);
        }

        private IEnumerator TryAddCondition(IControllableCharacter attacker)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker.HasAnyConditionOfType(_conditionDefinition.Name))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                _conditionDefinition.Name,
                _conditionDefinition.durationType,
                _conditionDefinition.durationParameter,
                _conditionDefinition.turnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    internal class ModifyWeaponAttackUnarmedStrikeDamage : IModifyWeaponAttackMode
    {
        private DieType dieType;

        public ModifyWeaponAttackUnarmedStrikeDamage(DieType dieType)
        {
            this.dieType = dieType;
        }
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            if (k < 0 || damage == null)
            {
                return;
            }

            if (damage.DieType < dieType)
            {
                damage.DieType = dieType;
            }
        }
    }
    internal class AddExtraUnarmedStrikeClawAttack : AddExtraAttackBase
    {
        public AddExtraUnarmedStrikeClawAttack()
            : base(ActionDefinitions.ActionType.None)
        {
        }

        // process subfeature later than unarmed strikes
        public override int Priority() => 1;

        // move before base unarmed strikes are added
        protected override AttackModeOrder GetOrder(RulesetCharacter character)
        {
            return AttackModeOrder.Start;
        }
        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
        {
            var hero = character as RulesetCharacterHero;
            if (hero == null)
            {
                return null;
            }
            var attackModifiers = hero.attackModifiers;

            // find attack mode that is unarmed strike
            var attackModes = new List<RulesetAttackMode>();

            hero.attackModes.ForEach(attackMode => {
                if (attackMode.SourceDefinition == hero.UnarmedStrikeDefinition)
                {
                    var newAttackMode = character.TryRefreshAttackMode(
                        attackMode.actionType,
                        CustomWeaponsContext.UnarmedStrikeClaws,
                        CustomWeaponsContext.UnarmedStrikeClaws.WeaponDescription,
                        ValidatorsCharacter.IsFreeOffhandVanilla(character),
                        true,
                        attackMode.SlotName,
                        attackModifiers,
                        character.FeaturesOrigin,
                        null
                    );
                    newAttackMode.attacksNumber = attackMode.attacksNumber;
                    attackModes.Add(newAttackMode);
                }
            });
            return attackModes;
        }
    }
}
