using System;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static AttributeDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialSpellShield : AbstractSubclass
{
    private const string Name = "SpellShield";
    internal const string FullName = $"Martial{Name}";

    internal MartialSpellShield()
    {
        var castSpellSpellShield = FeatureDefinitionCastSpellBuilder
            .Create($"CastSpell{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        var magicAffinitySpellShieldCombatMagicVigor = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}CombatMagicVigor")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        magicAffinitySpellShieldCombatMagicVigor.SetCustomSubFeatures(
            new ActionModifierMagicAffinityCombatMagicVigor(magicAffinitySpellShieldCombatMagicVigor));

        var conditionSpellShieldArcaneDeflection = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcaneDeflection")
            .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature, ConditionShielded)
            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{Name}ArcaneDeflection")
                .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    ArmorClass,
                    3)
                .AddToDB())
            .AddToDB();

        var powerSpellShieldArcaneDeflection = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneDeflection")
            .SetGuiPresentation(Category.Feature, ConditionShielded)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionSpellShieldArcaneDeflection,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .AddToDB();

        var actionAffinitySpellShieldRangedDefense = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales,
                $"ActionAffinity{Name}RangedDefense")
            .SetGuiPresentation($"Power{Name}RangedDeflection", Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Martial{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialSpellShield, 256))
            .AddFeaturesAtLevel(3,
                MagicAffinityCasterFightingCombatMagicImproved,
                castSpellSpellShield)
            .AddFeaturesAtLevel(7,
                PowerCasterFightingWarMagic,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                magicAffinitySpellShieldCombatMagicVigor)
            .AddFeaturesAtLevel(15,
                powerSpellShieldArcaneDeflection)
            .AddFeaturesAtLevel(18,
                actionAffinitySpellShieldRangedDefense)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ActionModifierMagicAffinityCombatMagicVigor : IModifyAttackActionModifier, IModifySpellDC
    {
        private readonly FeatureDefinitionMagicAffinity _featureDefinitionMagicAffinity;

        public ActionModifierMagicAffinityCombatMagicVigor(
            FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity)
        {
            _featureDefinitionMagicAffinity = featureDefinitionMagicAffinity;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (attackProximity != BattleDefinitions.AttackProximity.MagicDistance &&
                attackProximity != BattleDefinitions.AttackProximity.MagicRange &&
                attackProximity != BattleDefinitions.AttackProximity.MagicReach)
            {
                return;
            }

            var modifier = GetSpellDC(myself);

            attackModifier.attackRollModifier += modifier;
            attackModifier.attackToHitTrends.Add(new TrendInfo(
                modifier, FeatureSourceType.CharacterFeature, _featureDefinitionMagicAffinity.Name,
                _featureDefinitionMagicAffinity));
        }

        public int GetSpellDC(RulesetCharacter caster)
        {
            var strModifier =
                ComputeAbilityScoreModifier(caster.TryGetAttributeValue(Strength));
            var dexModifier =
                ComputeAbilityScoreModifier(caster.TryGetAttributeValue(Dexterity));

            return Math.Max(strModifier, dexModifier);
        }
    }
}
