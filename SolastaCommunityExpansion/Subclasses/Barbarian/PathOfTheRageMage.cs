using System;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;

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

        var supernaturalExploitsDarkvision = FeatureDefinitionPowerBuilder
            .Create("supernaturalExploitsDarkvisionPathOfTheRagemage", SubclassNamespace)
            .SetGuiPresentation("supernaturalExploitsPathOfTheRagemage", Category.Feature)
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


        Subclass = CharacterSubclassDefinitionBuilder
            .Create("BarbarianPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheRageMage", Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
            .AddFeatureAtLevel(skillProf, 3)
            .AddFeatureAtLevel(supernaturalExploitsDarkvision, 10)
            .AddFeatureAtLevel(supernaturalExploitsFeatherfall, 10)
            .AddFeatureAtLevel(supernaturalExploitsJump, 10)
            .AddFeatureAtLevel(supernaturalExploitsSeeInvisibility, 10).AddToDB();

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
