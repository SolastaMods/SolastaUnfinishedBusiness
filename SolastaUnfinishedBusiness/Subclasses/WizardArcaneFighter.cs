using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
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
        var magicAffinityArcaneFighterConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        var attackModifierEnchantWeapon = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}EnchantWeapon")
            .SetGuiPresentation("AttackModifierEnchantWeapon", Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Intelligence, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
            .AddToDB();

        var additionalActionArcaneFighter = FeatureDefinitionBuilder
            .Create($"AdditionalAction{Name}") //left old name for compatibility
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new OnReducedToZeroHpByMeSpellFighting(
                    ConditionDefinitionBuilder
                        .Create($"Condition{Name}SpellFighting")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(
                            FeatureDefinitionAdditionalActionBuilder
                                .Create("AdditionalActionSpellFighting")
                                .SetGuiPresentation($"AdditionalAction{Name}", Category.Feature)
                                .SetActionType(ActionDefinitions.ActionType.Main)
                                .SetRestrictedActions(ActionDefinitions.Id.CastMain)
                                .AddToDB())
                        .AddToDB()))
            .AddToDB();

        var additionalDamageArcaneFighterBonusWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}BonusWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(Name)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetDamageDice(DieType.D8, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Wizard{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.WizardArcaneFighter, 256))
            .AddFeaturesAtLevel(2,
                FeatureSetCasterFightingProficiency,
                magicAffinityArcaneFighterConcentrationAdvantage,
                attackModifierEnchantWeapon)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                additionalActionArcaneFighter)
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
        ConditionDefinition condition) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (activeEffect != null || !ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            if (attacker.RulesetCharacter.HasAnyConditionOfType(condition.Name))
            {
                yield break;
            }

            // only process in my own turn
            if (Gui.Battle?.ActiveContender != attacker)
            {
                yield break;
            }

            attacker.RulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }
}
