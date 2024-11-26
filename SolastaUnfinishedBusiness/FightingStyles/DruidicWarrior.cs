using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class DruidicWarrior : AbstractFightingStyle
{
    private const string Name = "DruidicWarrior";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.DruidicWarrior, 256), hidden: true)
        .SetFeatures(
            FeatureDefinitionCastSpellBuilder
                .Create(CastSpellCleric, $"CastSpell{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
                .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
                .SetSpellKnowledge(SpellKnowledge.Selection)
                .SetSpellReadyness(SpellReadyness.AllKnown)
                .SetSlotsRecharge(RechargeRate.LongRest)
                .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
                .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                .AddCustomSubFeatures(new FeatHelpers.SpellTag(Name), ClassHolder.Ranger)
                .AddToDB(),
            FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, CastSpellDruid.SpellListDefinition,
                    Name)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStyleRanger
    ];
}
