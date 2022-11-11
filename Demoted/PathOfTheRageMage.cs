#if false
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheRageMage : AbstractSubclass
{
    internal const string Name = "PathOfTheRageMage";

    internal PathOfTheRageMage()
    {
        var magicAffinityPathOfTheRageMage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityPathOfTheRageMage")
            .SetHandsFullCastingModifiers(true, true, true)
            .SetGuiPresentationNoContent(true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
            .AddToDB();

        var castSpellPathOfTheRageMage = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellPathOfTheRageMage")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListSorcerer)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(2, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        var proficiencyPathOfTheRageMageSkill = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyPathOfTheRageMageSkill")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        // a general definition of the Supernatural Exploits feature at level up
        var pathOfTheRageMageSupernaturalExploits = FeatureDefinitionBuilder
            .Create("FeaturePathOfTheRageMageSupernaturalExploits")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsDarkvision = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsDarkvision")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkvision)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(SpellDefinitions.Darkvision.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsFeatherfall = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsFeatherfall")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.FeatherFall)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(SpellDefinitions.FeatherFall.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsJump = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsJump")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Jump)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(SpellDefinitions.Jump.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsSeeInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsSeeInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.SeeInvisibility)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(SpellDefinitions.SeeInvisibility.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        // TODO: add one of two bonus effects for this subclass
        // A.) You can cast spells while raging, this keeps your rage going same as hitting opponent or
        // B.) at 6th level, when you cast a cantrip you can attack once AND at 14th level,
        // when you cast spell of 1st level or higher, you can attack once

#if false
        var bonusAttack = FeatureDefinitionOnMagicalAttackDamageEffectBuilder
             .Create("OnMagicalAttackDamageEffectPathOfTheRageMageArcaneRampage")
             .SetGuiPresentation(Category.Feature)
             .AddFeatures(FeatureDefinitionAdditionalActionBuilder
             .SetActionType(ActionDefinitions.ActionType.Bonus)
             .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
             .SetMaxAttacksNumber(-1)
             .AddToDB())
             .AddToDB();
#endif

        var additionalDamagePathOfTheRageMageArcaneExplosion = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamagePathOfTheRageMageArcaneExplosion")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("ArcaneExplosion")
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 13)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.Raging)
            .SetSpecificDamageType(DamageTypeForce)
            .AddToDB();

        var pathOfTheRageMageEnhancedArcaneExplosion = FeatureDefinitionBuilder
            .Create("FeaturePathOfTheRageMageEnhancedArcaneExplosion")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass =
            CharacterSubclassDefinitionBuilder
                .Create(Name)
                .SetGuiPresentation(Category.Subclass, DomainBattle)
                .AddFeaturesAtLevel(3,
                    castSpellPathOfTheRageMage,
                    magicAffinityPathOfTheRageMage,
                    proficiencyPathOfTheRageMageSkill)
                .AddFeaturesAtLevel(6,
                    additionalDamagePathOfTheRageMageArcaneExplosion)
                .AddFeaturesAtLevel(10,
                    pathOfTheRageMageSupernaturalExploits,
                    powerPathOfTheRageMageSupernaturalExploitsDarkvision,
                    powerPathOfTheRageMageSupernaturalExploitsFeatherfall,
                    powerPathOfTheRageMageSupernaturalExploitsJump,
                    powerPathOfTheRageMageSupernaturalExploitsSeeInvisibility)
                .AddFeaturesAtLevel(14,
                    pathOfTheRageMageEnhancedArcaneExplosion)
                .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
}
#endif
