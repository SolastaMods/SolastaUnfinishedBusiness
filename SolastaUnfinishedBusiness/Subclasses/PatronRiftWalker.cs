using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronRiftWalker : AbstractSubclass
{
    internal override CharacterSubclassDefinition Subclass { get; }

    internal PatronRiftWalker()
    {
        var spellListRiftWalker = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListPaladin, "SpellListRiftWalker")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Jump, Longstrider)
            .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
            .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
            .FinalizeSpells()
            .AddToDB();

        var magicAffinityRiftWalkerExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityRiftWalkerExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListRiftWalker)
            .AddToDB();

        var powerRiftWalkerRiftWalk = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftWalk")
            .SetGuiPresentation(Category.Feature, MistyStep.GuiPresentation.SpriteReference)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                MistyStep.EffectDescription,
                true)
            .AddToDB();

        var powerRiftWalkerBlink = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerBlink")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerShadowcasterShadowDodge.GuiPresentation.SpriteReference)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Banishment.EffectDescription,
                true)
            .AddToDB();

        //TODO: refactor into a builder
        powerRiftWalkerBlink.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        powerRiftWalkerBlink.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
        powerRiftWalkerBlink.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        powerRiftWalkerBlink.EffectDescription.HasSavingThrow = false;

        var conditionAffinityRiftWalkerRestrainedImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                "ConditionAffinityRiftWalkerRestrainedImmunity")
            .AddToDB();

        var damageAffinityRiftWalkerFadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance,
                "DamageAffinityRiftWalkerFadeIntoTheVoid")
            .SetGuiPresentation(Category.Feature, Blur.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerRiftWalkerRiftStrike = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftStrike")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerSpellBladeSpellTyrant.GuiPresentation.SpriteReference)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.Reaction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Banishment.EffectDescription,
                true)
            .AddToDB();

        //TODO: refactor into a builder
        powerRiftWalkerRiftStrike.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        powerRiftWalkerRiftStrike.EffectDescription.DurationParameter = 2;
        powerRiftWalkerRiftStrike.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        powerRiftWalkerRiftStrike.EffectDescription.HasSavingThrow = false;
        powerRiftWalkerRiftStrike.reactionContext = RuleDefinitions.ReactionTriggerContext.HitByMelee;

        var damageAffinityRiftWalkerRiftStrike = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityRiftWalkerRiftStrike")
            .SetGuiPresentation("PowerRiftWalkerRiftStrike", Category.Feature)
            .SetDamageAffinityType(RuleDefinitions.DamageAffinityType.None)
            .SetRetaliate(powerRiftWalkerRiftStrike, 1, true)
            .AddToDB();

        var powerRiftWalkerRiftControl = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftControl")
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetOverriddenPower(powerRiftWalkerRiftWalk)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                DimensionDoor.EffectDescription,
                true)
            .AddToDB();

        var bonusCantripRiftWalkWardingBond = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripRiftWalkWardingBond")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(SpellDefinitionBuilder
                .Create(WardingBond, "WardingBondAtWill")
                .SetSpellLevel(0)
                .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronRiftWalker")
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.PathMagebane.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(1,
                magicAffinityRiftWalkerExpandedSpells,
                powerRiftWalkerRiftWalk,
                powerRiftWalkerBlink)
            .AddFeaturesAtLevel(6,
                conditionAffinityRiftWalkerRestrainedImmunity,
                damageAffinityRiftWalkerRiftStrike)
            .AddFeaturesAtLevel(10,
                powerRiftWalkerRiftControl,
                damageAffinityRiftWalkerFadeIntoTheVoid)
            .AddFeaturesAtLevel(14,
                bonusCantripRiftWalkWardingBond)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
}
