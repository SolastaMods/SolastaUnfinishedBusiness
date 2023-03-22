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
            .Create("PowerSoulBladeEmpowerWeapon")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerSoulEmpower", Resources.PowerSoulEmpower, 256, 128))
            .SetUniqueInstance()
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanWeaponBeEmpowered))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(
                        ItemPropertyUsage.Unlimited,
                        1, new FeatureUnlockByLevel(
                            FeatureDefinitionAttackModifierBuilder
                                .Create("AttackModifierSoulBladeEmpowerWeapon")
                                .SetGuiPresentation(Category.Feature, PowerOathOfDevotionSacredWeapon)
                                .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
                                .SetMagicalWeapon()
                                .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
                                .AddToDB(),
                            0))
                    .Build())
                .Build())
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
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        conditionHexDefender.SetCustomSubFeatures(new NotifyConditionRemovalHex(conditionHexDefender));

        var featureHex = FeatureDefinitionBuilder
            .Create("FeatureSoulBladeHex")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new OnComputeAttackModifierHex(conditionHexAttacker, conditionHexDefender))
            .AddToDB();

        var spriteSoulHex = Sprites.GetSprite("PowerSoulHex", Resources.PowerSoulHex, 256, 128);

        var effectDescriptionHex = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
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

    private sealed class OnComputeAttackModifierHex : IOnComputeAttackModifier
    {
        private readonly ConditionDefinition _conditionHexAttacker;
        private readonly ConditionDefinition _conditionHexDefender;

        public OnComputeAttackModifierHex(
            ConditionDefinition conditionHexAttacker,
            ConditionDefinition conditionHexDefender)
        {
            _conditionHexAttacker = conditionHexAttacker;
            _conditionHexDefender = conditionHexDefender;
        }

        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            if (defender.HasAnyConditionOfType(_conditionHexDefender.Name))
            {
                var rulesetCondition = RulesetCondition.CreateActiveCondition(
                    myself.guid,
                    _conditionHexAttacker,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    myself.guid,
                    myself.CurrentFaction.Name);

                myself.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }
            else
            {
                var rulesetCondition =
                    myself.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionHexAttacker);

                if (rulesetCondition != null)
                {
                    myself.RemoveConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
                }
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
            var characterLevel = rulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            var charisma = rulesetCharacter.GetAttribute(AttributeDefinitions.Charisma).CurrentValue;
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var healingReceived = characterLevel + charismaModifier;

            if (rulesetCharacter.MissingHitPoints > 0)
            {
                rulesetCharacter.ReceiveHealing(healingReceived, true, rulesetCharacter.Guid);
            }
        }
    }
}
