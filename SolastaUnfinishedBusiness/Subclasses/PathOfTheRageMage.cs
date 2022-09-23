/*
 * Path Of The Rage Mage is a Third Caster for Barbarians, inspired by the subclass of the same name from Valda's Spire Of Secrets.
 * Created by William Smith AKA DemonSlayer730
 * Ver. 1.0
 * 07/08/2022 (MM/DD/YYYY)
 */

using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheRageMage : AbstractSubclass
{
    private readonly CharacterSubclassDefinition Subclass;

    internal PathOfTheRageMage()
    {
        var magicAffinity = FeatureDefinitionMagicAffinityBuilder // Adds some rules for magic
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

        var spellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellPathOfTheRageMage")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListSorcerer)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(SharedSpellsContext.OneThirdCasterReplacedSpells)
            // know 2 cantrips at level 3, gain at rate of third caster
            .SetKnownCantrips(
                2,
                3,
                FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            // start with 2 level 1 spells, gain at rate of third caster
            .SetKnownSpells(
                3,
                3,
                FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            // gain spell slots at rate of third caster
            .SetSlotsPerLevel(
                3,
                FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

        var skillProf = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyPathOfTheRageMageSkill")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        // A general definition of the Supernatural Exploits feature at level up
        var supernaturalExploits =
            FeatureDefinitionBuilder
                .Create("PathOfTheRageMageSupernaturalExploits")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

        var supernaturalExploitsDarkvision =
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

        var supernaturalExploitsFeatherfall =
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

        var supernaturalExploitsJump = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheRageMageSupernaturalExploitsJump")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Jump.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Jump.EffectDescription.Copy())
            .SetActivationTime(ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var supernaturalExploitsSeeInvisibility =
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
        
        var arcaneExplosion = FeatureDefinitionAdditionalDamageBuilder
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

        var enhancedArcaneExplosion = FeatureDefinitionBuilder
            .Create("PathOfTheRageMageEnhancedArcaneExplosion")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass =
            CharacterSubclassDefinitionBuilder
                .Create("PathOfTheRageMage")
                .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
                .AddFeatureAtLevel(magicAffinity, 3)
                .AddFeatureAtLevel(skillProf, 3)
                .AddFeatureAtLevel(arcaneExplosion, 6)
                .AddFeatureAtLevel(supernaturalExploits, 10)
                .AddFeatureAtLevel(supernaturalExploitsDarkvision, 10)
                .AddFeatureAtLevel(supernaturalExploitsFeatherfall, 10)
                .AddFeatureAtLevel(supernaturalExploitsJump, 10)
                .AddFeatureAtLevel(supernaturalExploitsSeeInvisibility, 10)
                .AddFeatureAtLevel(enhancedArcaneExplosion, 14)
                .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice
        GetSubclassChoiceList() // required method by abstract class, gets which class this subclass belongs to
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
    }

    internal override CharacterSubclassDefinition
        GetSubclass() // required method by abstract class, returns subclass object
    {
        return Subclass;
    }
}
