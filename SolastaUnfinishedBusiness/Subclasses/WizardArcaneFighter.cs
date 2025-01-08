using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardArcaneFighter : AbstractSubclass
{
    private const string Name = "ArcaneFighter";

    public WizardArcaneFighter()
    {
        // LEVEL 02

        var magicAffinityArcaneFighterConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        // kept name for backward compatibility
        var attackModifierEnchantWeapon = FeatureDefinitionBuilder
            .Create($"AttackModifier{Name}EnchantWeapon")
            .SetGuiPresentation("AttackModifierEnchantWeapon", Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Intelligence, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
            .AddToDB();

        // LEVEL 10

        var additionalActionSpellFighting = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}SpellFighting")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.CastMain)
            .AddToDB();

        var conditionSpellFighting = ConditionDefinitionBuilder
            .Create($"Condition{Name}SpellFighting")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalActionSpellFighting)
            .AddToDB();

        // kept name for backward compatibility
        var featureSpellFighting = FeatureDefinitionBuilder
            .Create($"AdditionalAction{Name}")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new OnReducedToZeroHpByMeSpellFighting(conditionSpellFighting))
            .AddToDB();

        // LEVEL 14

        var additionalDamageArcaneFighterBonusWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}BonusWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(Name)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetDamageDice(DieType.D8, 1)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Wizard{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardArcaneFighter, 256))
            .AddFeaturesAtLevel(2,
                FeatureSetCasterFightingProficiency,
                magicAffinityArcaneFighterConcentrationAdvantage,
                attackModifierEnchantWeapon)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                featureSpellFighting)
            .AddFeaturesAtLevel(14,
                additionalDamageArcaneFighterBonusWeapon)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class OnReducedToZeroHpByMeSpellFighting(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionSpellFighting) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!attacker.IsMyTurn() ||
                !ValidatorsWeapon.IsMelee(attackMode) ||
                attacker.RulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSpellFighting.Name))
            {
                yield break;
            }

            attacker.RulesetCharacter.InflictCondition(
                conditionSpellFighting.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                conditionSpellFighting.Name,
                0,
                0,
                0);
        }
    }
}
