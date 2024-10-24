using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardEvocation : AbstractSubclass
{
    private const string Name = "WizardEvocation";

    public WizardEvocation()
    {
        // LEVEL 02

        // Evocation Savant

        var magicAffinitySavant = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Savant")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(
                0.5f, 0.5f, 0, AdvantageType.None, PreparedSpellsModifier.None)
            .AddCustomSubFeatures(new ModifyScribeCostAndDurationEvocationSavant())
            .AddToDB();

        // Sculpt Spells

        var featureSculptSpells = FeatureDefinitionBuilder
            .Create($"Feature{Name}SculptSpells")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 06

        // Potent Cantrip

        var magicAffinityPotentCantrip = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}PotentCantrip")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        magicAffinityPotentCantrip.forceHalfDamageOnCantrips = true;

        // LEVEL 10

        // Empowered Evocation

        var featureEmpoweredEvocation = FeatureDefinitionBuilder
            .Create($"Feature{Name}EmpoweredEvocation")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation())
            .AddToDB();

        // LEVEL 14

        // Over Channel

        var featureOverChannel = FeatureDefinitionBuilder
            .Create($"Feature{Name}OverChannel")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardSpellMaster, 256))
            .AddFeaturesAtLevel(2, magicAffinitySavant, featureSculptSpells)
            .AddFeaturesAtLevel(6, magicAffinityPotentCantrip)
            .AddFeaturesAtLevel(10, featureEmpoweredEvocation)
            .AddFeaturesAtLevel(14, featureOverChannel)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Evocation Savant
    //

    private sealed class ModifyScribeCostAndDurationEvocationSavant : IModifyScribeCostAndDuration
    {
        public void ModifyScribeCostMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                costMultiplier = 1;
            }
        }

        public void ModifyScribeDurationMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                durationMultiplier = 1;
            }
        }
    }

    //
    // Sculpt Spells
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemySculptSpells : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation } ||
                attacker.Side != defender.Side)
            {
                yield break;
            }
        }
    }

    //
    // Empowered Evocation
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation })
            {
                yield break;
            }

            if (rulesetEffect.EffectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique &&
                !firstTarget)
            {
                yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (effectForm == null)
            {
                yield break;
            }

            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence));

            effectForm.DamageForm.BonusDamage += Math.Max(1, intelligenceModifier);
        }
    }
}
