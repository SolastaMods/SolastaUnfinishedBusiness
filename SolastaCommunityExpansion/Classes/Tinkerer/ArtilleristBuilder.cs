using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using System.Collections.Generic;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    public static class ArtilleristBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer, FeatureDefinitionCastSpell spellCasting)
        {
            // Make Artillerist subclass
            CharacterSubclassDefinitionBuilder artillerist = new CharacterSubclassDefinitionBuilder("Artillerist", GuidHelper.Create(TinkererClass.GuidNamespace, "Artillerist").ToString());
            GuiPresentationBuilder meleePresentation = new GuiPresentationBuilder(
                "Subclass/&ArtificerArtilleristDescription",
                "Subclass/&ArtificerArtilleristTitle");
            meleePresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionShockArcanist.GuiPresentation.SpriteReference);
            artillerist.SetGuiPresentation(meleePresentation.Build());

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells1 = FeatureHelpers.BuildAutoPreparedSpellGroup(3,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Shield, DatabaseHelper.SpellDefinitions.Thunderwave });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells2 = FeatureHelpers.BuildAutoPreparedSpellGroup(5,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.ScorchingRay, DatabaseHelper.SpellDefinitions.Shatter });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells3 = FeatureHelpers.BuildAutoPreparedSpellGroup(9,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Fireball, DatabaseHelper.SpellDefinitions.WindWall });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells4 = FeatureHelpers.BuildAutoPreparedSpellGroup(13,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.IceStorm, DatabaseHelper.SpellDefinitions.WallOfFire });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells5 = FeatureHelpers.BuildAutoPreparedSpellGroup(17,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.ConeOfCold, DatabaseHelper.SpellDefinitions.WallOfForce });

            GuiPresentationBuilder artilleristSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&ArtilleristSubclassSpellsDescription",
                "Feat/&ArtilleristSubclassSpellsTitle");
            FeatureDefinitionAutoPreparedSpells ArtilleristPrepSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    artilleristSpells1, artilleristSpells2, artilleristSpells3, artilleristSpells4, artilleristSpells5 },
                artificer, "ArtificerArtilleristAutoPrepSpells", artilleristSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleristPrepSpells, 3);

            // Level 3: Cannons
            // Flame
            GuiPresentationBuilder flameGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannonDescription",
                "Feat/&ArtilleristFlameCannonTitle");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fireEffect = new EffectDescriptionBuilder();
            fireEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fireEffect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fireEffect.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fireEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flameAttack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristFlameCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlameCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fireEffect.Build(),
                flameGui.Build()).AddToDB();
            //    artillerist.AddFeatureAtLevel(flameAttack, 3);

            GuiPresentationBuilder forceGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannonDescription",
                "Feat/&ArtilleristForceCannonTitle");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder forceEffect = new EffectDescriptionBuilder();
            forceEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            forceEffect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            forceEffect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.RangeHit, 24, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            forceEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower forceAttack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristForceCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, forceEffect.Build(),
                forceGui.Build()).AddToDB();
            //    artillerist.AddFeatureAtLevel(forceAttack, 3);

            // Protector
            GuiPresentationBuilder protectorGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristProtectorCannonDescription",
                "Feat/&ArtilleristProtectorCannonTitle");
            protectorGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder protectorEffect = new EffectDescriptionBuilder();
            protectorEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 1).SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus).Build());
            protectorEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 2, RuleDefinitions.TargetType.Sphere, 2, 2, ActionDefinitions.ItemSelectionType.None);
            protectorEffect.SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            protectorEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower protectorActivation = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristProtectorCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristProtectorCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, protectorEffect.Build(),
                protectorGui.Build()).AddToDB();
            //     artillerist.AddFeatureAtLevel(protectorActivation, 3);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryConstructlevel03FeatureSet, 3);

            GuiPresentationBuilder ArtilleryConstructLevel03AutopreparedSpellsPresentation = new GuiPresentationBuilder(
                 "Feat/&ArtilleryConstructLevel03AutopreparedSpellsDescription",
                 "Feat/&ArtilleryConstructLevel03AutopreparedSpellsTitle");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel03AutopreparedSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() {
                        ClassLevel=1,
                        SpellsList=new List<SpellDefinition>
                            {SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct}} },
                artificer,
                "ArtilleryConstructLevel03AutopreparedSpells",
                ArtilleryConstructLevel03AutopreparedSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleryConstructLevel03AutopreparedSpells, 03);

            // Level 5: Arcane Firearm-- additional damage, school of evocation spells
            GuiPresentationBuilder arcaneFirearmGui = new GuiPresentationBuilder(
                "Feat/&ArtificerArtilleristArcaneFirearmDescription",
                "Feat/&ArtificerArtilleristArcaneFirearmTitle");
            FeatureDefinitionAdditionalDamage arcaneFirearm = new FeatureDefinitionAdditionalDamageBuilder("ArtificerArtilleristArcaneFirearm",
                 GuidHelper.Create(TinkererClass.GuidNamespace, "ArtificerArtilleristArcaneFirearm").ToString(), "ArcaneFirearm",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn, RuleDefinitions.AdditionalDamageValueDetermination.Die, RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage, RuleDefinitions.AdditionalDamageRequiredProperty.None,
                false /* attack only */, RuleDefinitions.DieType.D8, 1 /* dice number */, RuleDefinitions.AdditionalDamageType.SameAsBaseDamage, "", RuleDefinitions.AdditionalDamageAdvancement.None,
                new List<DiceByRank>(), false, AttributeDefinitions.Wisdom, 0, EffectSavingThrowType.None, new List<ConditionOperationDescription>(),
                arcaneFirearmGui.Build()).AddToDB();
            artillerist.AddFeatureAtLevel(arcaneFirearm, 5);

            GuiPresentationBuilder detonationGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristCannonDetonationDescription",
                "Feat/&ArtilleristCannonDetonationTitle");

            EffectDescriptionBuilder detonationEffect = new EffectDescriptionBuilder();
            detonationEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                .Build());
            detonationEffect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            detonationEffect.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Sphere, 4, 4, ActionDefinitions.ItemSelectionType.None);
            detonationEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            SpellDefinition detonation = new SpellBuilder("ArtilleristCannonDetonation", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristCannonDetonation").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(1)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetEffectDescription(detonationEffect.Build())
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
                .SetGuiPresentation(detonationGui.Build())
                .AddToDB();

            // TODO- add an option to enable the power/spell version of the Blaster (there have been some requests for this) instead of the summons
            GuiPresentationBuilder artilleristDetonationPreparedPresentation = new GuiPresentationBuilder(
                "Feat/&ArtificerArtillerstDetonationSpellPreparedDescription",
                "Feat/&ArtificerArtillerstDetonationSpellPreparedTitle");
            FeatureDefinitionAutoPreparedSpells ArtilleristDetonationSpell = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    FeatureHelpers.BuildAutoPreparedSpellGroup(9, new List<SpellDefinition>() { detonation })
                },
                artificer, "ArtificerArtillerstDetonationSpellPrepared", artilleristDetonationPreparedPresentation.Build());
            //    artillerist.AddFeatureAtLevel(ArtilleristDetonationSpell, 9);

            // cannons with boosted damage
            GuiPresentationBuilder flame9Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannon9Description",
                "Feat/&ArtilleristFlameCannon9Title");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fire9Effect = new EffectDescriptionBuilder();
            fire9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire9Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire9Effect.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire9Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flame9Attack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristFlame9CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlame9CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire9Effect.Build(),
                flame9Gui.Build(), flameAttack).AddToDB();
            //    artillerist.AddFeatureAtLevel(flame9Attack, 9);

            // Force
            GuiPresentationBuilder force9Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannon9Description",
                "Feat/&ArtilleristForceCannon9Title");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder force9Effect = new EffectDescriptionBuilder();
            force9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force9Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force9Effect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.RangeHit, 24, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            force9Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower force9Attack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristForceCannon9Attack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannon9Attack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force9Effect.Build(),
                force9Gui.Build(), forceAttack).AddToDB();
            //    artillerist.AddFeatureAtLevel(force9Attack, 9);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel09FeatureSetBuilder.ArtilleryConstructlevel09FeatureSet, 9);

            GuiPresentationBuilder ArtilleryConstructLevel09AutopreparedSpellsPresentation = new GuiPresentationBuilder(
                 "Feat/&ArtilleryConstructLevel09AutopreparedSpellsDescription",
                 "Feat/&ArtilleryConstructLevel09AutopreparedSpellsTitle");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel09AutopreparedSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() {
                        ClassLevel=1,
                        SpellsList=new List<SpellDefinition>
                            {SummonArtillerySpellConstruct_9Builder.SummonArtillerySpellConstruct_9}} },
                artificer,
                "ArtilleryConstructLevel09AutopreparedSpells",
                ArtilleryConstructLevel09AutopreparedSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleryConstructLevel09AutopreparedSpells, 09);

            // cannons doubled
            GuiPresentationBuilder flame15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannon15Description",
                "Feat/&ArtilleristFlameCannon15Title");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fire15Effect = new EffectDescriptionBuilder();
            fire15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 6, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire15Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire15Effect.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flame15Attack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristFlame15CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlame15CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire15Effect.Build(),
                flame15Gui.Build(), flame9Attack).AddToDB();
            //    artillerist.AddFeatureAtLevel(flame15Attack, 15);

            // Force
            GuiPresentationBuilder force15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannon15Description",
                "Feat/&ArtilleristForceCannon15Title");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder force15Effect = new EffectDescriptionBuilder();
            force15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force15Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force15Effect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.RangeHit, 24, RuleDefinitions.TargetType.Individuals, 2, 2, ActionDefinitions.ItemSelectionType.None);
            force15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower force15Attack = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristForceCannon15Attack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannon15Attack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force15Effect.Build(),
                force15Gui.Build(), force9Attack).AddToDB();
            //    artillerist.AddFeatureAtLevel(force15Attack, 15);
            // Protector
            GuiPresentationBuilder protector15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristProtectorCannon15Description",
                "Feat/&ArtilleristProtectorCannon15Title");
            protectorGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder protector15Effect = new EffectDescriptionBuilder();
            protector15Effect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 2).SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus).Build());
            protector15Effect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 2, RuleDefinitions.TargetType.Sphere, 4, 4, ActionDefinitions.ItemSelectionType.None);
            protector15Effect.SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            protector15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower protector15Activation = new FeatureHelpers.FeatureDefinitionPowerBuilder("ArtilleristProtector15CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristProtector15CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, protector15Effect.Build(),
                protector15Gui.Build(), protectorActivation).AddToDB();
            //    artillerist.AddFeatureAtLevel(protector15Activation, 15);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel15FeatureSetBuilder.ArtilleryConstructlevel15FeatureSet, 15);

            GuiPresentationBuilder ArtilleryConstructLevel15AutopreparedSpellsPresentation = new GuiPresentationBuilder(
    "Feat/&ArtilleryConstructLevel15AutopreparedSpellsDescription",
    "Feat/&ArtilleryConstructLevel15AutopreparedSpellsTitle");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel15AutopreparedSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() {
                        ClassLevel=1,
                        SpellsList=new List<SpellDefinition>
                            {SummonArtillerySpellConstruct_15Builder.SummonArtillerySpellConstruct_15}} },
                artificer,
                "ArtilleryConstructLevel15AutopreparedSpells",
                ArtilleryConstructLevel15AutopreparedSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleryConstructLevel15AutopreparedSpells, 15);

            // build the subclass and add to the db
            return artillerist.AddToDB();
        }
    }
}
