using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronRiftWalker : AbstractSubclass
{
    internal PatronRiftWalker()
    {
        var spellListRiftWalker = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListRiftWalker")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Jump, Longstrider)
            .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
            .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
            .FinalizeSpells(true, 9)
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
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.LongRest,
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
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Banishment.EffectDescription,
                true)
            .AddToDB();

        powerRiftWalkerBlink.EffectDescription.DurationType = DurationType.Round;
        powerRiftWalkerBlink.EffectDescription.EndOfEffect = TurnOccurenceType.StartOfTurn;
        powerRiftWalkerBlink.EffectDescription.HasSavingThrow = false;
        powerRiftWalkerBlink.EffectDescription.TargetType = TargetType.Self;

        var conditionAffinityRiftWalkerRestrainedImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                "ConditionAffinityRiftWalkerRestrainedImmunity")
            .SetGuiPresentation(Category.Condition)
            .AddToDB();

        var powerRiftWalkerRiftStrike = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftStrike")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerSpellBladeSpellTyrant.GuiPresentation.SpriteReference)
            .Configure(
                1,
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.Reaction,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Banishment.EffectDescription,
                true)
            .AddToDB();

        powerRiftWalkerRiftStrike.EffectDescription.DurationType = DurationType.Round;
        powerRiftWalkerRiftStrike.EffectDescription.EndOfEffect = TurnOccurenceType.StartOfTurn;
        powerRiftWalkerRiftStrike.EffectDescription.HasSavingThrow = false;
        powerRiftWalkerRiftStrike.reactionContext = ReactionTriggerContext.HitByMelee;

        var powerRiftWalkerRiftControl = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftControl")
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetOverriddenPower(powerRiftWalkerRiftWalk)
            .Configure(
                1,
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                DimensionDoor.EffectDescription,
                true)
            .AddToDB();

        var damageAffinityRiftWalkerFadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance,
                "DamageAffinityRiftWalkerFadeIntoTheVoid")
            .SetGuiPresentation(Category.Feature, Blur.GuiPresentation.SpriteReference)
            .AddToDB();

        var bonusCantripRiftWalkWardingBond = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripRiftWalkWardingBond")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitionBuilder
                    .Create(WardingBond, "AtWillWardingBond")
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
                powerRiftWalkerRiftStrike)
            .AddFeaturesAtLevel(10,
                powerRiftWalkerRiftControl,
                damageAffinityRiftWalkerFadeIntoTheVoid)
            .AddFeaturesAtLevel(14,
                bonusCantripRiftWalkWardingBond)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
}
