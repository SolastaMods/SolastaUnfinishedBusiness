using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses;

public static class WarlockSubclassRiftWalkerPatron
{
    private static FeatureDefinitionPower _riftWalk;
    private static FeatureDefinitionDamageAffinity _fadeIntoTheVoid;
    private static FeatureDefinitionPower _riftBlink;
    private static FeatureDefinitionPower _riftStrike;
    private static FeatureDefinitionPower _riftJump;
    private static FeatureDefinitionConditionAffinity _riftCloak;
    private static FeatureDefinitionBonusCantrips _wardingBondBonusCantrip;
    private static FeatureDefinitionMagicAffinity _riftWalkerMagicAffinity;

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
            .AddFeatureAtLevel(_riftWalkerMagicAffinity, 1)
            .AddFeatureAtLevel(_riftWalk, 1)
            .AddFeatureAtLevel(_riftBlink, 1)
            .AddFeatureAtLevel(_riftCloak, 6)
            .AddFeatureAtLevel(_riftStrike, 6)
            .AddFeatureAtLevel(_riftJump, 10)
            .AddFeatureAtLevel(_fadeIntoTheVoid, 10)
            .AddFeatureAtLevel(_wardingBondBonusCantrip, 14) //RiftCloak,14)
            .AddToDB();
    }

    private static void RiftStepBuilder()
    {
        _riftWalk = FeatureDefinitionPowerBuilder
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

    private static void FadeIntoTheVoidBuilder()
    {
        _fadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance,
                "DH_FadeIntoTheVoid", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, Blur.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static void RiftStrikeBuilder()
    {
        _riftStrike = FeatureDefinitionPowerBuilder
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

        _riftStrike.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        _riftStrike.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        _riftStrike.EffectDescription.HasSavingThrow = false;
        _riftStrike.reactionContext = RuleDefinitions.ReactionTriggerContext.HitByMelee;
    }

    private static void RiftJumpBuilder()
    {
        _riftJump = FeatureDefinitionPowerBuilder
            .Create("DH_RiftControl", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetOverriddenPower(_riftWalk)
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

    private static void RiftBlinkBuilder()
    {
        _riftBlink = FeatureDefinitionPowerBuilder
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

        _riftBlink.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        _riftBlink.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
        _riftBlink.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
        _riftBlink.EffectDescription.HasSavingThrow = false;
    }

    private static void RiftCloakBuilder()
    {
        _riftCloak = FeatureDefinitionConditionAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                "RiftWalkerMovementAffinityRestrainedImmunity", CENamespaceGuid)
            .AddToDB();
        _riftCloak.GuiPresentation.title = "Feature/&RiftWalkerRestrainedImmunityTitle";
    }

    private static void AtWillWardingBond()
    {
        _wardingBondBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("DHWardingBondBonusCantrip", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .ClearBonusCantrips()
            .AddBonusCantrip(SpellDefinitionBuilder
                .Create(WardingBond, "DHAtWillWardingBond", CENamespaceGuid)
                .SetSpellLevel(0)
                .AddToDB())
            .AddToDB();
    }

    private static void RiftWalkerSpells()
    {
        var riftWalkerSpellList = SpellListDefinitionBuilder
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

        _riftWalkerMagicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("RiftWalkerSpellsMagicAffinity", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(riftWalkerSpellList)
            .AddToDB();
    }
}
