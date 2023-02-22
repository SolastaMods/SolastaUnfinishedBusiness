using System;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using static AttributeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialSpellShield : AbstractSubclass
{
    internal const string Name = "MartialSpellShield";

    internal MartialSpellShield()
    {
        var castSpellSpellShield = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellSpellShield")
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
            .Create("MagicAffinitySpellShieldCombatMagicVigor")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ComputeAttackModifierMagicAffinityCombatMagicVigor())
            .AddToDB();

        var conditionSpellShieldArcaneDeflection = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldArcaneDeflection")
            .SetGuiPresentation("PowerSpellShieldArcaneDeflection", Category.Feature, ConditionShielded)
            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierSpellShieldArcaneDeflection")
                .SetGuiPresentation("PowerSpellShieldArcaneDeflection", Category.Feature)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    ArmorClass,
                    3)
                .AddToDB())
            .AddToDB();

        var powerSpellShieldArcaneDeflection = FeatureDefinitionPowerBuilder
            .Create("PowerSpellShieldArcaneDeflection")
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
                "ActionAffinitySpellShieldRangedDefense")
            .SetGuiPresentation("PowerSpellShieldRangedDeflection", Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, DomainBattle)
            .AddFeaturesAtLevel(3,
                MagicAffinityCasterFightingCombatMagic,
                castSpellSpellShield)
            .AddFeaturesAtLevel(7,
                PowerCasterFightingWarMagic,
                ReplaceAttackWithCantripCasterFighting)
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

    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ComputeAttackModifierMagicAffinityCombatMagicVigor : IOnComputeAttackModifier, IIncreaseSpellDc
    {
        public int GetSpellModifier(RulesetCharacter caster)
        {
            var strModifier =
                ComputeAbilityScoreModifier(caster.GetAttribute(Strength)
                    .CurrentValue);
            var dexModifier =
                ComputeAbilityScoreModifier(caster.GetAttribute(Dexterity)
                    .CurrentValue);

            return Math.Max(strModifier, dexModifier);
        }

        public void ComputeAttackModifier(
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

            var modifier = GetSpellModifier(myself);

            attackModifier.attackRollModifier += modifier;
            attackModifier.attackToHitTrends.Add(new TrendInfo(modifier, FeatureSourceType.ExplicitFeature,
                "Feature/&MagicAffinitySpellShieldCombatMagicVigorTitle", null));
        }
    }
}
