using System.Collections;
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

internal sealed class WizardArcaneFighter : AbstractSubclass
{
    internal WizardArcaneFighter()
    {
        var magicAffinityArcaneFighterConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArcaneFighterConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        // LEFT AS A POWER FOR BACKWARD COMPATIBILITY
        var powerArcaneFighterEnchantWeapon = FeatureDefinitionPowerBuilder
            .Create("PowerArcaneFighterEnchantWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action)
            .SetCustomSubFeatures(
                PowerVisibilityModifier.Hidden,
                new CanUseAttribute(AttributeDefinitions.Intelligence, CanWeaponBeEnchanted))
            .AddToDB();

        var additionalActionArcaneFighter = FeatureDefinitionBuilder
            .Create("AdditionalActionArcaneFighter") //left old name for compatibility
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new SpellFighting(ConditionDefinitionBuilder
                .Create("ConditionArcaneFighterSpellFighting")
                .SetGuiPresentationNoContent()
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(FeatureDefinitionAdditionalActionBuilder
                    .Create("AdditionalActionSpellFighting")
                    .SetGuiPresentation("AdditionalActionArcaneFighter", Category.Feature)
                    .SetActionType(ActionDefinitions.ActionType.Main)
                    .SetRestrictedActions(ActionDefinitions.Id.CastMain)
                    .AddToDB())
                .AddToDB()))
            .AddToDB();

        var additionalDamageArcaneFighterBonusWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageArcaneFighterBonusWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("ArcaneFighter")
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetDamageDice(DieType.D8, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardArcaneFighter")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WizardArcaneFighter", Resources.WizardArcaneFighter, 256))
            .AddFeaturesAtLevel(2,
                FeatureSetCasterFightingProficiency,
                magicAffinityArcaneFighterConcentrationAdvantage,
                powerArcaneFighterEnchantWeapon)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                additionalActionArcaneFighter)
            .AddFeaturesAtLevel(14,
                additionalDamageArcaneFighterBonusWeapon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
    
    private sealed class SpellFighting : ITargetReducedToZeroHp
    {
        private readonly ConditionDefinition condition;

        public SpellFighting(ConditionDefinition condition)
        {
            this.condition = condition;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
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

            if (Global.ControlledLocationCharacter.Guid != attacker.Guid)
            {
                yield break;
            }

            attacker.RulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }
}
