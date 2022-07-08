using System;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Subclasses.Barbarian;

internal sealed class PathOfTheRageMage : AbstractSubclass
{

    private readonly CharacterSubclassDefinition Subclass;
    private static readonly Guid SubclassNamespace = new("6a9ec115-29db-40b4-9b1d-ad55abede214");
    internal PathOfTheRageMage()
    {
        var magicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityBarbarianPathOfTheRageMage", SubclassNamespace)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetGuiPresentationNoContent(true)
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None, 0,
                RuleDefinitions.SpellParamsModifierType.FlatValue, true, false, false)
            .AddToDB();

        var spellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheRageMageSpellcasting", Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListSorcerer)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetKnownCantrips(2, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetKnownSpells(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

        var skillProf = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencySkillPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation("ProficiencySkillPathOfTheRageMage", Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        var supernaturalExploits = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation("supernaturalExploitsPathOfTheRagemage", Category.Feature)
            .AddToDB();

        var supernaturalExploitsDarkvision = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsDarkvisionPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation("supernaturalExploitsDarkvisionPathOfTheRagemage", Category.Feature)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkvision.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Darkvision.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var supernaturalExploitsFeatherfall = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsFeatherfallPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.FeatherFall.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.FeatherFall.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var supernaturalExploitsJump = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsJumpPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Jump.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Jump.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var supernaturalExploitsSeeInvisibility = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsSeeInvisibilityPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.SeeInvisibility.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        /* var bonusAttack = FeatureDefinitionOnMagicalAttackDamageEffectBuilder
             .Create("ArcaneRampagePathOfTheRagemage", SubclassNamespace)
             .SetGuiPresentation(Category.Feature)
             .AddFeatures(FeatureDefinitionAdditionalActionBuilder
             .SetActionType(ActionDefinitions.ActionType.Bonus)
             .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
             .SetMaxAttacksNumber(-1)
             .AddToDB())
             .AddToDB(); */

        var arcaneExplosion = FeatureDefinitionAdditionalDamageBuilder
            .Create("arcaneExplosionPathOfTheMageRage", SubclassNamespace)
            .SetGuiPresentation("arcaneExplosionPathOfTheMageRage", Category.Feature)
            .SetDamageDice(RuleDefinitions.DieType.D6, 1)
            .SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel,
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
                .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.OncePerTurn)
                .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.Raging)
                .SetSpecificDamageType(DamageTypeForce).AddToDB();

        var enhancedArcaneExplosion = FeatureDefinitionPowerBuilder
            .Create("enhancedArcaneExplosionPathOfTheMageRage", SubclassNamespace)
            .SetGuiPresentation("enhancedArcaneExplosionPathOfTheMageRage", Category.Feature)
            .AddToDB();


        Subclass = CharacterSubclassDefinitionBuilder
            .Create("BarbarianPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheRageMage", Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
            .AddFeatureAtLevel(magicAffinity, 3)
            .AddFeatureAtLevel(skillProf, 3)
            .AddFeatureAtLevel(arcaneExplosion, 6)
            .AddFeatureAtLevel(supernaturalExploits, 10)
            .AddFeatureAtLevel(supernaturalExploitsDarkvision, 10)
            .AddFeatureAtLevel(supernaturalExploitsFeatherfall, 10)
            .AddFeatureAtLevel(supernaturalExploitsJump, 10)
            .AddFeatureAtLevel(supernaturalExploitsSeeInvisibility, 10)
            .AddFeatureAtLevel(enhancedArcaneExplosion, 14).AddToDB();

    }



    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
