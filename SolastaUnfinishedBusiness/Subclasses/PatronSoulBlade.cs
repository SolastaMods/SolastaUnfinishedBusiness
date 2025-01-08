using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
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
    internal const string ConditionHex = $"Condition{Name}HexDefender";

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

        // kept name for backward compatibility
        var attackModifierEmpowerWeapon = FeatureDefinitionBuilder
            .Create($"AttackModifier{Name}EmpowerWeapon")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEmpowered),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEmpowered))
            .AddToDB();

        // Hex

        var conditionHexDefender = ConditionDefinitionBuilder
            .Create(ConditionHex)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        conditionHexDefender.AddCustomSubFeatures(new OnConditionAddedOrRemovedHex(conditionHexDefender));

        var additionalDamageHex = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Hex")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Hex")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetTargetCondition(conditionHexDefender, AdditionalDamageTriggerCondition.TargetHasCondition)
            .AddCustomSubFeatures(new ModifyCriticalThresholdHex(conditionHexDefender))
            .AddToDB();

        var conditionHexAttacker = ConditionDefinitionBuilder
            .Create($"Condition{Name}HexAttacker")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageHex)
            .AddToDB();

        var spriteSoulHex = Sprites.GetSprite("PowerSoulHex", Resources.PowerSoulHex, 256, 128);

        var powerHex = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Hex")
            .SetGuiPresentation(Category.Feature, spriteSoulHex)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHexDefender),
                        EffectFormBuilder.ConditionForm(
                            conditionHexAttacker, ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(Bane)
                    .Build())
            .AddCustomSubFeatures(ForceRetargetAvailability.Mark)
            .AddToDB();

        //
        // LEVEL 06
        //

        // Summon Pact Weapon

        var proxyPactWeapons = new EffectProxyDefinition[3];

        for (var i = 1; i <= 3; i++)
        {
            proxyPactWeapons[i - 1] = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyArcaneSword, $"ProxyPactWeapon{i}")
                .SetOrUpdateGuiPresentation("ProxyPactWeapon", Category.Proxy)
                .SetActionId(ExtraActionId.ProxyPactWeapon, ExtraActionId.ProxyPactWeaponFree)
                .SetAttackMethod(ProxyAttackMethod.CasterSpellAbility, DamageTypeForce, DieType.D8, i, true)
                .AddToDB();
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Shield)
                    .Build())
            .AddToDB();

        //
        // Level 14
        //

        // Master Hex

        var powerMasterHex = FeatureDefinitionPowerBuilder
            .Create(powerHex, $"Power{Name}MasterHex")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(powerHex)
                    .AllowRetarget()
                    .Build())
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

        // give one last chance if a pact blade wielding a two handed
        if (!canWeaponBeEmpowered)
        {
            canWeaponBeEmpowered =
                ValidatorsWeapon.IsTwoHanded(mode) &&
                (hero.ActiveFeatures.Any(p => p.Value.Contains(FeatureDefinitionFeatureSets.FeatureSetPactBlade)) ||
                 hero.HasActiveInvocation(Tabletop2024Context.InvocationPactBlade));
        }

        return canWeaponBeEmpowered;
    }

    private sealed class ModifyCriticalThresholdHex(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionHex) : IModifyAttackCriticalThreshold
    {
        public int GetCriticalThreshold(
            int current, RulesetCharacter me, RulesetCharacter target, BaseDefinition attackMethod)
        {
            if (target == null || !attackMethod)
            {
                return current;
            }

            if (target.HasConditionOfType(conditionHex.Name))
            {
                return current - 1;
            }

            return current;
        }
    }

    private sealed class OnConditionAddedOrRemovedHex(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionHexDefender)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not { IsDeadOrDyingOrUnconscious: true } ||
                rulesetCondition.ConditionDefinition != conditionHexDefender)
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetCaster is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCaster.MissingHitPoints == 0)
            {
                return;
            }

            var characterLevel = rulesetCaster.GetClassLevel(CharacterClassDefinitions.Warlock);
            var charisma = rulesetCaster.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var healingReceived = characterLevel + charismaModifier;

            rulesetCaster.ReceiveHealing(healingReceived, true, rulesetCaster.Guid);
        }
    }

    private sealed class ModifyEffectDescriptionSummonPactWeapon(BaseDefinition baseDefinition)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == baseDefinition && character.GetClassLevel(CharacterClassDefinitions.Warlock) >= 10;
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
