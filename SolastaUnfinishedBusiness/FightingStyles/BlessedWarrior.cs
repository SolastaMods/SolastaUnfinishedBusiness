using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class BlessedWarrior : AbstractFightingStyle
{
    internal const string Name = "BlessedWarrior";

    internal static readonly FeatureDefinitionCastSpell CastSpellBlessedWarrior = FeatureDefinitionCastSpellBuilder
        .Create($"CastSpell{Name}")
        .SetGuiPresentationNoContent(true)
        .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
        .SetSpellCastingAbility(AttributeDefinitions.Charisma)
        .SetSpellKnowledge(SpellKnowledge.Selection)
        .SetSpellReadyness(SpellReadyness.AllKnown)
        .SetSlotsRecharge(RechargeRate.LongRest)
        .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
        .SetSpellList(SpellListCleric)
        .SetKnownCantrips(2, 2, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
        .SetKnownSpells(0, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
        .SetReplacedSpells(1, 0)
        .SetUniqueLevelSlots(false)
        .AddCustomSubFeatures(new FeatHelpers.SpellTag(Name))
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.BlessedWarrior, 256), hidden: true)
        .SetFeatures(
            CastSpellBlessedWarrior,
            FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, SpellListCleric,
                    Name)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStylePaladin
    ];
}
