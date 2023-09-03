using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PatronSoulBlade : AbstractSubclass
{
    private const string Name = "SoulBlade";

    public PatronSoulBlade()
    {
        //
        // LEVEL 01
        //

        // Expanded Spell List

        var spellListSoulBlade = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
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
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetOrUpdateGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListSoulBlade)
            .AddToDB();

        // Empower Weapon

        // LEFT AS A POWER FOR BACKWARD COMPATIBILITY
        var powerSoulBladeEmpowerWeapon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EmpowerWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action)
            .SetCustomSubFeatures(
                PowerVisibilityModifier.Hidden,
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEmpowered))
            .AddToDB();

        // Common Hex Feature
        var conditionHexDefender = ConditionDefinitionBuilder
            .Create($"Condition{Name}HexDefender")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var additionalDamageHex = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Hex")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Hex")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetTargetCondition(conditionHexDefender, AdditionalDamageTriggerCondition.TargetHasCondition)
            .SetCustomSubFeatures(new ModifyCriticalThresholdAgainstHexedTargets(conditionHexDefender.Name))
            .AddToDB();

        var conditionHexAttacker = ConditionDefinitionBuilder
            .Create($"Condition{Name}HexAttacker")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageHex)
            .AddToDB();

        conditionHexDefender.SetCustomSubFeatures(new NotifyConditionRemovalHex(conditionHexDefender));

        var spriteSoulHex = Sprites.GetSprite("PowerSoulHex", Resources.PowerSoulHex, 256, 128);

        var effectDescriptionHex = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(Bane)
            .SetEffectForms(
                EffectFormBuilder.ConditionForm(conditionHexDefender),
                EffectFormBuilder.ConditionForm(conditionHexAttacker, ConditionForm.ConditionOperation.Add, true))
            .Build();

        // Soul Hex - Basic

        var powerHex = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Hex")
            .SetGuiPresentation(Category.Feature, spriteSoulHex)
            .SetCustomSubFeatures(ForceRetargetAvailability.Mark)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetShowCasting(true)
            .SetEffectDescription(effectDescriptionHex)
            .AddToDB();

        //
        // LEVEL 06
        //

        // Summon Pact Weapon

        var powerSoulBladeSummonPactWeapon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SummonPactWeapon")
            .SetGuiPresentation(Category.Feature, SpiritualWeapon)
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(ArcaneSword.EffectDescription)
                .Build())
            .AddToDB();

        powerSoulBladeSummonPactWeapon.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        //
        // LEVEL 10
        //

        // Soul Shield

        var powerSoulBladeSoulShield = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SoulShield")
            .SetGuiPresentation(Category.Feature, PowerFighterSecondWind)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(Shield.EffectDescription)
            .SetReactionContext(ReactionTriggerContext.None)
            .AddToDB();

        //
        // Level 14
        //

        // Master Hex

        var effectDescriptionMasterHex = EffectDescriptionBuilder
            .Create(effectDescriptionHex)
            .AllowRetarget()
            .Build();

        var powerMasterHex = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MasterHex")
            .SetGuiPresentation($"Power{Name}Hex", Category.Feature, spriteSoulHex)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .SetShowCasting(true)
            .SetEffectDescription(effectDescriptionMasterHex)
            .SetOverriddenPower(powerHex)
            .AddToDB();

        var featureSetMasterHex = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterHex")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerMasterHex)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.PatronSoulBlade, 256))
            .AddFeaturesAtLevel(1,
                FeatureSetCasterFightingProficiency,
                magicAffinitySoulBladeExpandedSpells,
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

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool CanWeaponBeEmpowered(RulesetAttackMode mode, RulesetItem item, RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        var canWeaponBeEmpowered = CanWeaponBeEnchanted(mode, item, character);
        var canTwoHandedBeEmpowered =
            ValidatorsWeapon.HasTwoHandedTag(mode) &&
            hero.ActiveFeatures.Any(p => p.Value.Contains(FeatureDefinitionFeatureSets.FeatureSetPactBlade));

        return canWeaponBeEmpowered || canTwoHandedBeEmpowered;
    }

    private sealed class ModifyCriticalThresholdAgainstHexedTargets : IModifyAttackCriticalThreshold
    {
        private readonly string _hexCondition;

        public ModifyCriticalThresholdAgainstHexedTargets(string hexCondition)
        {
            _hexCondition = hexCondition;
        }

        public int GetCriticalThreshold(int current, RulesetCharacter me, RulesetCharacter target,
            BaseDefinition attackMethod)
        {
            if (target == null || attackMethod == null)
            {
                return current;
            }

            if (target.HasConditionOfType(_hexCondition))
            {
                return current - 1;
            }

            return current;
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
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition != _conditionHexDefender)
            {
                return;
            }

            var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (caster == null)
            {
                return;
            }

            ReceiveHealing(caster);
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
