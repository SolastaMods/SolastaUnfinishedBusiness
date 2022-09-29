/*
 * Path Of The Rage Mage is a Third Caster for Barbarians, inspired by the subclass of the same name from Valda's Spire Of Secrets.
 * Created by William Smith AKA DemonSlayer730
 * Ver. 1.0
 * 07/08/2022 (MM/DD/YYYY)
 */

using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheRageMage : AbstractSubclass
{
    public const string Name = "PathOfTheRageMage";
    private readonly CharacterSubclassDefinition Subclass;

    internal PathOfTheRageMage()
    {
        var magicAffinityPathOfTheRageMage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityPathOfTheRageMage")
            .SetHandsFullCastingModifiers(true, true, true)
            .SetGuiPresentationNoContent(true)
            .SetCastingModifiers(
                0,
                SpellParamsModifierType.None,
                0,
                SpellParamsModifierType.FlatValue,
                true,
                false,
                false)
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
            .SetKnownCantrips(2, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetKnownSpells(3, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .AddToDB();

        var proficiencyPathOfTheRageMageSkill = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyPathOfTheRageMageSkill")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        // a general definition of the Supernatural Exploits feature at level up
        var pathOfTheRageMageSupernaturalExploits =
            FeatureDefinitionBuilder
                .Create("PathOfTheRageMageSupernaturalExploits")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsDarkvision =
            FeatureDefinitionPowerBuilder
                .Create("PowerPathOfTheRageMageSupernaturalExploitsDarkvision")
                .SetGuiPresentation(Category.Feature)
                .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkvision.GuiPresentation.SpriteReference)
                .SetEffectDescription(SpellDefinitions.Darkvision.EffectDescription.Copy())
                .SetActivationTime(ActivationTime.Action)
                .SetFixedUsesPerRecharge(1)
                .SetRechargeRate(RechargeRate.LongRest)
                .SetCostPerUse(1)
                .SetShowCasting(true)
                .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsFeatherfall =
            FeatureDefinitionPowerBuilder
                .Create("PowerPathOfTheRageMageSupernaturalExploitsFeatherfall")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.FeatherFall.GuiPresentation.SpriteReference)
                .SetEffectDescription(SpellDefinitions.FeatherFall.EffectDescription.Copy())
                .SetActivationTime(ActivationTime.Action)
                .SetFixedUsesPerRecharge(1)
                .SetRechargeRate(RechargeRate.LongRest)
                .SetCostPerUse(1)
                .SetShowCasting(true)
                .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsJump = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsJump")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Jump.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Jump.EffectDescription.Copy())
            .SetActivationTime(ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var powerPathOfTheRageMageSupernaturalExploitsSeeInvisibility =
            FeatureDefinitionPowerBuilder
                .Create("PowerPathOfTheRageMageSupernaturalExploitsSeeInvisibility")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference)
                .SetEffectDescription(SpellDefinitions.SeeInvisibility.EffectDescription.Copy())
                .SetActivationTime(ActivationTime.Action)
                .SetFixedUsesPerRecharge(1)
                .SetRechargeRate(RechargeRate.LongRest)
                .SetCostPerUse(1)
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
            // damage increases to 2d6 at 14th level
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel,
                (6, 1),
                (7, 1),
                (8, 1),
                (9, 1),
                (10, 1),
                (11, 1),
                (12, 1),
                (13, 1),
                (14, 2),
                (15, 2),
                (16, 2),
                (17, 2),
                (18, 2),
                (19, 2),
                (20, 2)
            )
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.Raging)
            .SetSpecificDamageType(DamageTypeForce).AddToDB();

        var pathOfTheRageMageEnhancedArcaneExplosion = FeatureDefinitionBuilder
            .Create("PathOfTheRageMageEnhancedArcaneExplosion")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass =
            CharacterSubclassDefinitionBuilder
                .Create(Name)
                .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
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

    internal override FeatureDefinitionSubclassChoice
        GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
    }

    internal override CharacterSubclassDefinition
        GetSubclass()
    {
        return Subclass;
    }
}
