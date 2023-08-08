using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronRiftWalker : AbstractSubclass
{
    private const string Name = "RiftWalker";

    public PatronRiftWalker()
    {
        var spellListRiftWalker = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
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
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListRiftWalker)
            .AddToDB();

        var powerRiftWalkerRiftWalk = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftWalk")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(MistyStep.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var powerRiftWalkerBlink = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Blink")
            .SetGuiPresentation(Category.Feature, PowerShadowcasterShadowDodge)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Banishment.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetNoSavingThrow()
                .Build())
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        var conditionAffinityRiftWalkerRestrainedImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityRestrainedmmunity, $"ConditionAffinity{Name}RestrainedImmunity")
            .SetGuiPresentation(Category.Condition)
            .AddToDB();

        var powerRiftWalkerRiftStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftStrike")
            .SetGuiPresentation(Category.Feature, Banishment)
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Banishment.EffectDescription)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                .SetNoSavingThrow()
                .Build())
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        var powerRiftWalkerRiftControl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftControl")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerRiftWalkerRiftWalk)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        var damageAffinityRiftWalkerFadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityHalfOrcRelentlessEndurance, $"DamageAffinity{Name}FadeIntoTheVoid")
            .SetGuiPresentation(Category.Feature, Blur)
            .AddToDB();

        // kept name for backward compatibility
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
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.PatronRiftWalker, 256))
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

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class PreventRemoveConcentrationRiftWalker : IPreventRemoveConcentrationWithPowerUse
    {
    }
}
