using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{


    public static class DHWarlockSubclassRiftWalker
    {
        public const string Name = "DHWarlockSubclassRiftWalker";
        private static readonly string Guid = GuidHelper.Create(new Guid(Settings.GUID), Name).ToString();
        public static FeatureDefinitionPower RiftWalk;
        public static FeatureDefinitionDamageAffinity FadeIntoTheVoid;
        public static FeatureDefinitionPower RiftBlink;
        public static FeatureDefinitionPower RiftStrike;
        public static FeatureDefinitionPower RiftJump;
        public static FeatureDefinitionPower RiftCloak;
        public static SpellDefinition AtWillCantripWardingBond;
        public static FeatureDefinitionBonusCantrips WardingBondBonusCantrip;
        public static FeatureDefinitionMagicAffinity RiftWalkerMagicAffinity;

        public static void Build()
        {
            RiftStepBuilder();
            RiftBlinkBuilder();
            RiftWalkerSpells();
            RiftStrikeBuilder();
            RiftJumpBuilder();
            FadeIntoTheVoidBuilder();
            // RiftCloakBuilder();
            AtWillWardingBond();




            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&DHWarlockSubclassRiftWalkerDescription",
                    "Subclass/&DHWarlockSubclassRiftWalkerTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.PathMagebane.GuiPresentation.SpriteReference)
                    .Build();



            var definition = CharacterSubclassDefinitionBuilder.Create(Name, Guid);
            definition.SetGuiPresentation(subclassGuiPresentation)



               .AddFeatureAtLevel(RiftWalkerMagicAffinity, 1)
               .AddFeatureAtLevel(RiftWalk, 1)
               .AddFeatureAtLevel(RiftBlink, 1)
               .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity, 6)
               .AddFeatureAtLevel(RiftStrike, 6)
               .AddFeatureAtLevel(RiftJump, 10)
               .AddFeatureAtLevel(FadeIntoTheVoid, 10)
               .AddFeatureAtLevel(WardingBondBonusCantrip, 14) //RiftCloak,14 )
               .AddToDB();


        }

        public static void RiftStepBuilder()
        {
            var guiPresentationRiftWalk = new GuiPresentationBuilder(
             "Feature/&DH_RiftWalkDescription",
             "Feature/&DH_RiftWalkTitle")
             .SetSpriteReference(DatabaseHelper.SpellDefinitions.MistyStep.GuiPresentation.SpriteReference)
             .Build();

            var RiftWalkBuilder = FeatureDefinitionPowerBuilder.Create(
                       "DH_RiftWalk",
                       GuidHelper.Create(new Guid(Settings.GUID), "DH_RiftWalk").ToString())
                .SetGuiPresentation(guiPresentationRiftWalk)
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
                       DatabaseHelper.SpellDefinitions.MistyStep.EffectDescription,
                       true);
            RiftWalk = RiftWalkBuilder.AddToDB();
        }

        public static void FadeIntoTheVoidBuilder()
        {
            var guiPresentationFadeIntoTheVoid = new GuiPresentationBuilder(
                "Feature/&DH_FadeIntoTheVoidDescription",
                "Feature/&DH_FadeIntoTheVoidTitle")
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Blur.GuiPresentation.SpriteReference)
                .Build();

            var FadeIntoTheVoidBuilder = FeatureDefinitionDamageAffinityBuilder
                .Create(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance, "DH_FadeIntoTheVoid", GuidHelper.Create(new Guid(Settings.GUID), "DH_FadeIntoTheVoid").ToString())
                .SetGuiPresentation(guiPresentationFadeIntoTheVoid);

            FadeIntoTheVoid = FadeIntoTheVoidBuilder.AddToDB();


        }

        public static void RiftStrikeBuilder()
        {

            var guiPresentationRiftStrike = new GuiPresentationBuilder(
                "Feature/&DH_RiftStrikeDescription",
                "Feature/&DH_RiftStrikeTitle")
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerSpellBladeSpellTyrant.GuiPresentation.SpriteReference)
                .Build();

            var RiftStrikeBuilder = FeatureDefinitionPowerBuilder
                .Create("DH_RiftStrike", GuidHelper.Create(new Guid(Settings.GUID), "DH_RiftStrike").ToString())
                .SetGuiPresentation(guiPresentationRiftStrike)
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
                       DatabaseHelper.SpellDefinitions.Banishment.EffectDescription,
                       true);
            RiftStrike = RiftStrikeBuilder.AddToDB();

            RiftStrike.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            RiftStrike.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
            RiftStrike.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            RiftStrike.EffectDescription.HasSavingThrow = false;
            RiftStrike.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);

        }

        public static void RiftJumpBuilder()
        {
            var guiPresentationRiftControl = new GuiPresentationBuilder(
            "Feature/&DH_RiftControlDescription",
            "Feature/&DH_RiftControlTitle")
            .SetSpriteReference(DatabaseHelper.SpellDefinitions.DimensionDoor.GuiPresentation.SpriteReference)
            .Build();

            var RiftControlBuilder = FeatureDefinitionPowerBuilder
                .Create("DH_RiftControl", GuidHelper.Create(new Guid(Settings.GUID), "DH_RiftControl").ToString())
                .SetGuiPresentation(guiPresentationRiftControl)
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
                       DatabaseHelper.SpellDefinitions.DimensionDoor.EffectDescription,
                       true);
            RiftJump = RiftControlBuilder.AddToDB();
            RiftJump.SetOverriddenPower(RiftWalk);


        }

        public static void RiftBlinkBuilder()
        {
            var guiPresentationBlink = new GuiPresentationBuilder(
           "Feature/&DH_BlinkDescription",
           "Feature/&DH_BlinkTitle")
           .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerShadowcasterShadowDodge.GuiPresentation.SpriteReference)
           .Build();

            var BlinkBuilder = FeatureDefinitionPowerBuilder
                .Create("DH_Blink", GuidHelper.Create(new Guid(Settings.GUID), "DH_Blink").ToString())
                .SetGuiPresentation(guiPresentationBlink)
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
                       DatabaseHelper.SpellDefinitions.Banishment.EffectDescription,
                       true);
            RiftBlink = BlinkBuilder.AddToDB();
            RiftBlink.EffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            RiftBlink.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
            RiftBlink.EffectDescription.EndOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            RiftBlink.EffectDescription.HasSavingThrow = false;
        }

        public static void RiftCloakBuilder()
        {
            var guiPresentationRiftCloak = new GuiPresentationBuilder(
                "Feature/&DH_RiftCloakDescription",
                "Feature/&DH_RiftCloakTitle")
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.DimensionDoor.GuiPresentation.SpriteReference)
                .Build();

            var RiftCloakBuilder = FeatureDefinitionPowerBuilder
                .Create("DH_RiftCloak", GuidHelper.Create(new Guid(Settings.GUID), "DH_RiftCloak").ToString())
                .SetGuiPresentation(guiPresentationRiftCloak)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.BonusAction,
                       1,
                       RuleDefinitions.RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       DatabaseHelper.SpellDefinitions.WardingBond.EffectDescription,
                       true);
            RiftCloak = RiftCloakBuilder.AddToDB();

            RiftCloak.SetUniqueInstance(true);
        }

        public static void AtWillWardingBond()
        {

            SpellDefinitionBuilder AtWillWardingBondBuilder = SpellDefinitionBuilder
                .Create(DatabaseHelper.SpellDefinitions.WardingBond, "DHAtWillWardingBond", GuidHelper.Create(new System.Guid(Settings.GUID), "DHAtWillWardingBond").ToString());
            AtWillWardingBondBuilder.SetSpellLevel(0);
            AtWillCantripWardingBond = AtWillWardingBondBuilder.AddToDB();

            FeatureDefinitionBonusCantripsBuilder WardingBondBonusCantripBuilder = FeatureDefinitionBonusCantripsBuilder
                .Create(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion, "DHWardingBondBonusCantrip", GuidHelper.Create(new System.Guid(Settings.GUID), "DHAtWillWardingBondBonusCantrip").ToString())
                .SetGuiPresentation(new GuiPresentationBuilder("Feature/&DHWardingBondBonusCantripDescription", "Feature/&DHWardingBondBonusCantripTitle").Build());
            WardingBondBonusCantripBuilder.ClearBonusCantrips();
            WardingBondBonusCantripBuilder.AddBonusCantrip(AtWillCantripWardingBond);
            WardingBondBonusCantrip = WardingBondBonusCantripBuilder.AddToDB();



        }

        public static void RiftWalkerSpells()
        {
            SpellListDefinition RiftWalkerSpellList = SpellListDefinitionBuilder
                .Create(
                DatabaseHelper.SpellListDefinitions.SpellListSkeletonKnight,
                "RiftWalkerSpellsList",
                GuidHelper.Create(new Guid(Settings.GUID), "RiftWalkerSpellsList").ToString())
                .SetGuiPresentation("RiftWalkerSpellsList", Category.SpellList)
            .ClearSpells().AddToDB();

            RiftWalkerSpellList.SpellsByLevel.Clear();
            RiftWalkerSpellList.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
            {
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =0,
                    Spells = new List<SpellDefinition>
                    {
                    }
                },
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


            var RiftWalkerMagicAffintyBuilder = FeatureDefinitionMagicAffinityBuilder
                .Create("RiftWalkerSpellsMagicAffinity", GuidHelper.Create(new Guid(Settings.GUID), "RiftWalkerSpellsMagicAffinity").ToString())
                .SetGuiPresentation(
                         new GuiPresentationBuilder(
                         "Feature/&RiftWalkerSpellsMagicAffinityDescription",
                         "Feature/&RiftWalkerSpellsMagicAffinityTitle"
                         ).Build()
                     );

            RiftWalkerMagicAffinity = RiftWalkerMagicAffintyBuilder.AddToDB();

            RiftWalkerMagicAffinity.SetExtendedSpellList(RiftWalkerSpellList);

        }


    }

}
