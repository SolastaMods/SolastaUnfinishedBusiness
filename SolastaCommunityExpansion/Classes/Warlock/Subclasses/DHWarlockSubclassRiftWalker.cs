using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses;

public static class DHWarlockSubclassRiftWalkerPatron
{
    private static FeatureDefinitionPower RiftWalk;
    private static FeatureDefinitionDamageAffinity FadeIntoTheVoid;
    private static FeatureDefinitionPower RiftBlink;
    private static FeatureDefinitionPower RiftStrike;
    private static FeatureDefinitionPower RiftJump;
    private static FeatureDefinitionConditionAffinity RiftCloak;
    private static FeatureDefinitionBonusCantrips WardingBondBonusCantrip;
    private static FeatureDefinitionMagicAffinity RiftWalkerMagicAffinity;

    public static CharacterSubclassDefinition Build()
    {
        RiftStepBuilder();
        RiftBlinkBuilder();
        RiftWalkerSpells();
        RiftStrikeBuilder();
        RiftJumpBuilder();
        FadeIntoTheVoidBuilder();
        RiftCloakBuilder();
        AtWillWardingBond();

        return CharacterSubclassDefinitionBuilder
            .Create("DHWarlockSubclassRiftWalker", CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass,
                DatabaseHelper.CharacterSubclassDefinitions.PathMagebane.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(RiftWalkerMagicAffinity, 1)
            .AddFeatureAtLevel(RiftWalk, 1)
            .AddFeatureAtLevel(RiftBlink, 1)
            .AddFeatureAtLevel(RiftCloak, 6)
            .AddFeatureAtLevel(RiftStrike, 6)
            .AddFeatureAtLevel(RiftJump, 10)
            .AddFeatureAtLevel(FadeIntoTheVoid, 10)
            .AddFeatureAtLevel(WardingBondBonusCantrip, 14) //RiftCloak,14)
            .AddToDB();
    }

    public static void RiftStepBuilder()
    {
        RiftWalk = FeatureDefinitionPowerBuilder
            .Create("DH_RiftWalk", CENamespaceGuid)
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
    }

    public static void FadeIntoTheVoidBuilder()
    {
        FadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance,
                "DH_FadeIntoTheVoid", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, Blur.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    public static void RiftStrikeBuilder()
    {
        RiftStrike = FeatureDefinitionPowerBuilder
            .Create("DH_RiftStrike", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature,
                DatabaseHelper.FeatureDefinitionPowers.PowerSpellBladeSpellTyrant.GuiPresentation.SpriteReference)
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

        RiftStrike.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        RiftStrike.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        RiftStrike.EffectDescription.HasSavingThrow = false;
        RiftStrike.reactionContext = RuleDefinitions.ReactionTriggerContext.HitByMelee;
    }

    public static void RiftJumpBuilder()
    {
        RiftJump = FeatureDefinitionPowerBuilder
            .Create("DH_RiftControl", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetOverriddenPower(RiftWalk)
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
    }

    public static void RiftBlinkBuilder()
    {
        RiftBlink = FeatureDefinitionPowerBuilder
            .Create("DH_Blink", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature,
                DatabaseHelper.FeatureDefinitionPowers.PowerShadowcasterShadowDodge.GuiPresentation.SpriteReference)
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

        RiftBlink.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        RiftBlink.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
        RiftBlink.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        RiftBlink.EffectDescription.HasSavingThrow = false;
    }

    public static void RiftCloakBuilder()
    {
        RiftCloak = FeatureDefinitionConditionAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                "RiftWalkerMovementAffinityRestrainedImmunity", CENamespaceGuid)
            .AddToDB();
        RiftCloak.GuiPresentation.title = "Feature/&RiftWalkerRestrainedImmunityTitle";
    }

    public static void AtWillWardingBond()
    {
        WardingBondBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("DHWardingBondBonusCantrip", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .ClearBonusCantrips()
            .AddBonusCantrip(SpellDefinitionBuilder
                .Create(WardingBond, "DHAtWillWardingBond", CENamespaceGuid)
                .SetSpellLevel(0)
                .AddToDB())
            .AddToDB();
    }

    public static void RiftWalkerSpells()
    {
        var RiftWalkerSpellList = SpellListDefinitionBuilder
            .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "RiftWalkerSpellsList", CENamespaceGuid)
            .SetGuiPresentation("RiftWalkerSpellsList", Category.SpellList)
            .ClearSpells()
            .SetSpellsAtLevel(1, Jump, Longstrider)
            .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
            .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
            .FinalizeSpells()
            .AddToDB();

        RiftWalkerMagicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("RiftWalkerSpellsMagicAffinity", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(RiftWalkerSpellList)
            .AddToDB();
    }
}
