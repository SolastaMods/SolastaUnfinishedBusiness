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
    internal const string FullName = $"Patron{Name}";

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

        var attackModifierEmpowerWeapon = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}EmpowerWeapon")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEmpowered),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEmpowered))
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
            .AddCustomSubFeatures(new ModifyCriticalThresholdAgainstHexedTargets(conditionHexDefender.Name))
            .AddToDB();

        var conditionHexAttacker = ConditionDefinitionBuilder
            .Create($"Condition{Name}HexAttacker")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageHex)
            .AddToDB();

        conditionHexDefender.AddCustomSubFeatures(new OnConditionAddedOrRemovedHex(conditionHexDefender));

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
            .AddCustomSubFeatures(ForceRetargetAvailability.Mark)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetShowCasting(true)
            .SetEffectDescription(effectDescriptionHex)
            .AddToDB();

        //
        // LEVEL 06
        //

        // Summon Pact Weapon

        var proxyPactWeapons = new EffectProxyDefinition[3];

        for (var i = 1; i <= 3; i++)
        {
            var proxyPactWeapon = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyArcaneSword, $"ProxyPactWeapon{i}")
                .SetOrUpdateGuiPresentation("ProxyPactWeapon", Category.Proxy)
                .AddToDB();

            proxyPactWeapon.damageDie = DieType.D8;
            proxyPactWeapon.damageDieNum = i;
            proxyPactWeapon.addAbilityToDamage = true;
            proxyPactWeapons[i - 1] = proxyPactWeapon;
        }

        var conditionSummonPactWeapon = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionDivineFavor, $"Condition{Name}SummonPactWeapon")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Minute, 1)
            .SetFeatures()
            .AddToDB();

        var powerSoulBladeSummonPactWeapon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SummonPactWeapon")
            .SetGuiPresentation(Category.Feature, ArcaneSword)
            .SetUniqueInstance()
            .AddCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ArcaneSword.EffectDescription)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(proxyPactWeapons[0])
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionSummonPactWeapon, applyToSelf: true,
                            forceOnSelf: true))
                    .Build())
            .AddToDB();

        powerSoulBladeSummonPactWeapon.AddCustomSubFeatures(
            new ModifyEffectDescriptionSummonPactWeapon(powerSoulBladeSummonPactWeapon));

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
            .Create(FullName)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.PatronSoulBlade, 256))
            .AddFeaturesAtLevel(1,
                FeatureSetCasterFightingProficiency,
                magicAffinitySoulBladeExpandedSpells,
                powerHex,
                attackModifierEmpowerWeapon)
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

    private sealed class OnConditionAddedOrRemovedHex : IOnConditionAddedOrRemoved
    {
        private readonly ConditionDefinition _conditionHexDefender;

        public OnConditionAddedOrRemovedHex(ConditionDefinition conditionHexDefender)
        {
            _conditionHexDefender = conditionHexDefender;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // SHOULD ONLY TRIGGER ON DEATH
            if (target is not { IsDeadOrDyingOrUnconscious: true })
            {
                return;
            }

            if (rulesetCondition.ConditionDefinition != _conditionHexDefender)
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetCaster is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            ReceiveHealing(rulesetCaster);
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

    private sealed class ModifyEffectDescriptionSummonPactWeapon : IModifyEffectDescription
    {
        private readonly BaseDefinition _baseDefinition;

        public ModifyEffectDescriptionSummonPactWeapon(BaseDefinition baseDefinition)
        {
            _baseDefinition = baseDefinition;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _baseDefinition && character.GetClassLevel(CharacterClassDefinitions.Warlock) >= 10;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Warlock);
            var dice = classLevel switch
            {
                >= 14 => 3,
                >= 10 => 2,
                _ => 1
            };

            effectDescription.EffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Summon)!
                    .SummonForm.effectProxyDefinitionName = $"ProxyPactWeapon{dice}";

            return effectDescription;
        }
    }
}
