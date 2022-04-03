using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
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

         return   CharacterSubclassDefinitionBuilder
                .Create("DHWarlockSubclassRiftWalker", CENamespaceGuid)
                .SetGuiPresentation(Category.Subclass, DatabaseHelper.CharacterSubclassDefinitions.PathMagebane.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(RiftWalkerMagicAffinity, 1)
                .AddFeatureAtLevel(RiftWalk, 1)
                .AddFeatureAtLevel(RiftBlink, 1)
                .AddFeatureAtLevel(RiftCloak, 6)
                .AddFeatureAtLevel(RiftStrike, 6)
                .AddFeatureAtLevel(RiftJump, 10)
                .AddFeatureAtLevel(FadeIntoTheVoid, 10)
                .AddFeatureAtLevel(WardingBondBonusCantrip, 14) //RiftCloak,14 )
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
                .Create(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance, "DH_FadeIntoTheVoid", CENamespaceGuid)
                .SetGuiPresentation(Category.Feature, Blur.GuiPresentation.SpriteReference)
                .AddToDB();
        }

        public static void RiftStrikeBuilder()
        {
            RiftStrike = FeatureDefinitionPowerBuilder
                .Create("DH_RiftStrike", CENamespaceGuid)
                .SetGuiPresentation(Category.Feature, DatabaseHelper.FeatureDefinitionPowers.PowerSpellBladeSpellTyrant.GuiPresentation.SpriteReference)
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
            RiftStrike.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
            RiftStrike.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            RiftStrike.EffectDescription.HasSavingThrow = false;
            RiftStrike.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);
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
                .SetGuiPresentation(Category.Feature, DatabaseHelper.FeatureDefinitionPowers.PowerShadowcasterShadowDodge.GuiPresentation.SpriteReference)
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
                       true).AddToDB();

            RiftBlink.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            RiftBlink.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
            RiftBlink.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            RiftBlink.EffectDescription.HasSavingThrow = false;
        }

        public static void RiftCloakBuilder()
        {

            RiftCloak =  FeatureDefinitionConditionAffinityBuilder
             .Create(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity, "RiftWalkerMovementAffinityRestrainedImmunity", DefinitionBuilder.CENamespaceGuid)
             .AddToDB();
            RiftCloak.GuiPresentation.SetTitle("Feature/&RiftWalkerRestrainedImmunityTitle");


        }

        public static void AtWillWardingBond()
        {
            WardingBondBonusCantrip = FeatureDefinitionBonusCantripsBuilder
                .Create(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion, "DHWardingBondBonusCantrip", CENamespaceGuid)
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
            SpellListDefinition RiftWalkerSpellList = SpellListDefinitionBuilder
                   .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "RiftWalkerSpellsList", CENamespaceGuid)
                   .SetGuiPresentation("RiftWalkerSpellsList", Category.SpellList)
                   .ClearSpells()
                 
                //   .SetSpellsAtLevel(1, Jump, Longstrider)
                //   .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
                //   .SetSpellsAtLevel(3, Haste, Slow)
                //   .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
                //   .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
                   .SetMaxSpellLevel(5, false)
                   .AddToDB();
            RiftWalkerSpellList.ClearSpellsByLevel();
            RiftWalkerSpellList.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
             {
                // new SpellListDefinition.SpellsByLevelDuplet
                // {
                //     Level =0,
                //     Spells = new List<SpellDefinition>
                //     {
                //     }
                // },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =1,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.Jump,
                         DatabaseHelper.SpellDefinitions.Longstrider
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =2,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.Blur,
                         DatabaseHelper.SpellDefinitions.PassWithoutTrace
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =3,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.Haste,
                         DatabaseHelper.SpellDefinitions.Slow
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =4,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.FreedomOfMovement,
                         DatabaseHelper.SpellDefinitions.GreaterInvisibility
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =5,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.MindTwist,
                         DatabaseHelper.SpellDefinitions.DispelEvilAndGood
                     }
                 },

             });
         
            

            RiftWalkerMagicAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("RiftWalkerSpellsMagicAffinity", CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                
                .SetExtendedSpellList(RiftWalkerSpellList)
                .AddToDB();
        }
    }
}
