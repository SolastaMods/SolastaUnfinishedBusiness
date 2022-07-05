using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
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
            .SetGuiPresentation(Category.Subclass)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None, 0,
                RuleDefinitions.SpellParamsModifierType.FlatValue, true, false, false)
            .AddToDB();

        var spellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheRageMageSpellcasting", Category.Subclass)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListSorcerer)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetKnownCantrips(2, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetKnownSpells(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("BarbarianPathOfTheRageMage", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(magicAffinity, 3)
            .AddFeatureAtLevel(spellCasting.AddToDB(), 3).AddToDB();

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
