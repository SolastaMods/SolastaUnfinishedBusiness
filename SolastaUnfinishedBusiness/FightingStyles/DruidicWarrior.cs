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

internal class DruidicWarrior : AbstractFightingStyle
{
    internal const string Name = "DruidicWarrior";

    internal static readonly FeatureDefinitionCastSpell CastSpellDruidicWarrior = FeatureDefinitionCastSpellBuilder
        .Create($"CastSpell{Name}")
        .SetGuiPresentation(Name, Category.FightingStyle)
        .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
        .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
        .SetSpellKnowledge(SpellKnowledge.Selection)
        .SetSpellReadyness(SpellReadyness.AllKnown)
        .SetSlotsRecharge(RechargeRate.LongRest)
        .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
        .SetSpellList(SpellListDruid)
        .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
        .SetKnownSpells(0, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
        .SetReplacedSpells(1, 0)
        .SetUniqueLevelSlots(false)
        .AddCustomSubFeatures(new FeatHelpers.SpellTag(Name))
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.DruidicWarrior, 256))
        .SetFeatures(
            CastSpellDruidicWarrior,
            FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, SpellListDruid, Name)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStyleRanger
    ];
}
