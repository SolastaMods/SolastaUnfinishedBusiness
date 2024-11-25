using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class BlessedWarrior : AbstractFightingStyle
{
    private const string Name = "BlessedWarrior";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.BlessedWarrior, 256))
        .SetFeatures(
            FeatureDefinitionCastSpellBuilder
                .Create(CastSpellCleric, $"CastSpell{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
                .SetSpellCastingAbility(AttributeDefinitions.Charisma)
                .SetSpellKnowledge(SpellKnowledge.Selection)
                .SetSpellReadyness(SpellReadyness.AllKnown)
                .SetSlotsRecharge(RechargeRate.LongRest)
                .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
                .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                .AddCustomSubFeatures(new FeatHelpers.SpellTag(Name), CharacterClassDefinitions.Paladin)
                .AddToDB(),
            FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{Name}")
                .SetGuiPresentationNoContent(true)
                .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, CastSpellCleric.SpellListDefinition,
                    Name)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStylePaladin
    ];
}
