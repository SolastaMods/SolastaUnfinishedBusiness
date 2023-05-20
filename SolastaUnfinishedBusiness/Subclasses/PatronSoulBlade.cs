using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronSoulBlade : AbstractSubclass
{
    internal PatronSoulBlade()
    {
        //
        // LEVEL 01
        //

        // Expanded Spell List

        var spellListSoulBlade = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListSoulBlade")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Shield, SpellsContext.WrathfulSmite)
            .SetSpellsAtLevel(2, Blur, BrandingSmite)
            .SetSpellsAtLevel(3, SpellsContext.BlindingSmite, SpellsContext.ElementalWeapon)
            .SetSpellsAtLevel(4, PhantasmalKiller, SpellsContext.StaggeringSmite)
            .SetSpellsAtLevel(5, SpellsContext.BanishingSmite, ConeOfCold)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinitySoulBladeExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySoulBladeExpandedSpells")
            .SetOrUpdateGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListSoulBlade)
            .AddToDB();

        // Empower Weapon

        var powerSoulBladeEmpowerWeapon = FeatureDefinitionPowerBuilder
            .Create(PowerArcaneFighterEnchantWeapon, "PowerSoulBladeEmpowerWeapon")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerSoulEmpower", Resources.PowerSoulEmpower, 256, 128))
            .SetBonusToAttack(false, false, AttributeDefinitions.Charisma)
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanWeaponBeEmpowered))
            .AddToDB();

        // Common Hex Feature

        var additionalDamageHex = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageSoulBladeHex")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Hex")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .AddToDB();

        var attributeModifierHex = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierSoulBladeHex")
            .SetGuiPresentationNoContent(true)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();

        var conditionHexAttacker = ConditionDefinitionBuilder
            .Create("ConditionSoulBladeHexAttacker")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(additionalDamageHex, attributeModifierHex)
            .AddToDB();

        var conditionHexDefender = ConditionDefinitionBuilder
            .Create("ConditionSoulBladeHexDefender")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        conditionHexDefender.SetCustomSubFeatures(new NotifyConditionRemovalHex(conditionHexDefender));

        var featureHex = FeatureDefinitionBuilder
            .Create("FeatureSoulBladeHex")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new AttackOrMagicAttackInitiatedHex(conditionHexAttacker, conditionHexDefender))
            .AddToDB();

        var spriteSoulHex = Sprites.GetSprite("PowerSoulHex", Resources.PowerSoulHex, 256, 128);

        var effectDescriptionHex = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(Bane)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionHexDefender, ConditionForm.ConditionOperation.Add)
                    .Build())
            .Build();

        // Soul Hex - Basic

        var powerHex = FeatureDefinitionPowerBuilder
            .Create("PowerSoulBladeHex")
            .SetGuiPresentation(Category.Feature, spriteSoulHex)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetShowCasting(true)
            .SetEffectDescription(effectDescriptionHex)
            .AddToDB();

        //
        // LEVEL 06
        //

        // Summon Pact Weapon

        var powerSoulBladeSummonPactWeapon = FeatureDefinitionPowerBuilder
            .Create("PowerSoulBladeSummonPactWeapon")
            .SetGuiPresentation(Category.Feature, SpiritualWeapon)
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpiritualWeapon.EffectDescription)
                    .Build())
            .AddToDB();

        powerSoulBladeSummonPactWeapon.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        //
        // LEVEL 10
        //

        // Soul Shield

        var powerSoulBladeSoulShield = FeatureDefinitionPowerBuilder
            .Create("PowerSoulBladeSoulShield")
            .SetGuiPresentation("PowerSoulBladeSoulShield", Category.Feature, PowerFighterSecondWind)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(Shield.EffectDescription)
            .SetReactionContext(ReactionTriggerContext.None)
            .AddToDB();

        //
        // Level 14
        //

        // Master Hex

        var powerMasterHex = FeatureDefinitionPowerBuilder
            .Create("PowerSoulBladeMasterHex")
            .SetGuiPresentation("PowerSoulBladeHex", Category.Feature, spriteSoulHex)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .SetShowCasting(true)
            .SetEffectDescription(effectDescriptionHex)
            .SetOverriddenPower(powerHex)
            .AddToDB();

        var featureSetMasterHex = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetSoulBladeMasterHex")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerMasterHex)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronSoulBlade")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("PatronSoulBlade", Resources.PatronSoulBlade, 256))
            .AddFeaturesAtLevel(1,
                FeatureSetCasterFightingProficiency,
                magicAffinitySoulBladeExpandedSpells,
                featureHex,
                powerHex,
                powerSoulBladeEmpowerWeapon)
            .AddFeaturesAtLevel(6,
                powerSoulBladeSummonPactWeapon)
            .AddFeaturesAtLevel(10,
                powerSoulBladeSoulShield)
            .AddFeaturesAtLevel(14,
                featureSetMasterHex)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool CanWeaponBeEmpowered(RulesetCharacter character, RulesetItem item)
    {
        var definition = item.ItemDefinition;

        if (!definition.IsWeapon || !character.IsProficientWithItem(definition))
        {
            return false;
        }

        if (character is RulesetCharacterHero hero &&
            hero.ActiveFeatures.Any(p => p.Value.Contains(FeatureDefinitionFeatureSets.FeatureSetPactBlade)))
        {
            return true;
        }

        return !definition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagTwoHanded);
    }

    private sealed class AttackOrMagicAttackInitiatedHex : IPhysicalAttackInitiated, IMagicalAttackInitiated
    {
        private readonly ConditionDefinition _conditionHexAttacker;
        private readonly ConditionDefinition _conditionHexDefender;

        public AttackOrMagicAttackInitiatedHex(
            ConditionDefinition conditionHexAttacker,
            ConditionDefinition conditionHexDefender)
        {
            _conditionHexAttacker = conditionHexAttacker;
            _conditionHexDefender = conditionHexDefender;
        }

        public IEnumerator OnMagicalAttackInitiated(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return ApplyCondition(attacker, defender);
        }

        public IEnumerator OnAttackInitiated(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            yield return ApplyCondition(attacker, defender);
        }

        private IEnumerator ApplyCondition(IControllableCharacter attacker, IControllableCharacter defender)
        {
            var rulesetDefender = defender.RulesetCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetDefender == null || rulesetAttacker == null || rulesetDefender.IsDeadOrDying)
            {
                yield break;
            }

            if (rulesetDefender.HasAnyConditionOfType(_conditionHexDefender.Name))
            {
                rulesetAttacker.InflictCondition(
                    _conditionHexAttacker.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
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
    }

    private sealed class NotifyConditionRemovalHex : INotifyConditionRemoval
    {
        private readonly ConditionDefinition _conditionHexDefender;

        public NotifyConditionRemovalHex(ConditionDefinition conditionHexDefender)
        {
            _conditionHexDefender = conditionHexDefender;
        }

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition != _conditionHexDefender)
            {
                return;
            }

            var sourceGuid = rulesetCondition.SourceGuid;

            if (RulesetEntity.TryGetEntity<RulesetCharacter>(sourceGuid, out var rulesetCharacter))
            {
                ReceiveHealing(rulesetCharacter);
            }
        }

        private static void ReceiveHealing(RulesetCharacter rulesetCharacter)
        {
            var characterLevel = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var charisma = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var healingReceived = characterLevel + charismaModifier;

            if (rulesetCharacter.MissingHitPoints > 0 && !rulesetCharacter.IsDeadOrDyingOrUnconscious)
            {
                rulesetCharacter.ReceiveHealing(healingReceived, true, rulesetCharacter.Guid);
            }
        }
    }
}
