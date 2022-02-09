using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    public static class ArtilleristBuilder
    {
        // TODO: unused parameter FeatureDefinitionCastSpell
#pragma warning disable IDE0060, RCS1163 // Unused parameter.
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer, FeatureDefinitionCastSpell spellCasting)
#pragma warning restore IDE0060, RCS1163 // Unused parameter.
        {
            // Make Artillerist subclass
            CharacterSubclassDefinitionBuilder artillerist = new CharacterSubclassDefinitionBuilder("Artillerist", GuidHelper.Create(TinkererClass.GuidNamespace, "Artillerist").ToString());
            GuiPresentationBuilder meleePresentation = new GuiPresentationBuilder(
                "Subclass/&ArtificerArtilleristTitle",
                "Subclass/&ArtificerArtilleristDescription");
            meleePresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionShockArcanist.GuiPresentation.SpriteReference);
            artillerist.SetGuiPresentation(meleePresentation.Build());

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells1 = BuildAutoPreparedSpellGroup(
                3, DatabaseHelper.SpellDefinitions.Shield, DatabaseHelper.SpellDefinitions.Thunderwave);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells2 = BuildAutoPreparedSpellGroup(
                5, DatabaseHelper.SpellDefinitions.ScorchingRay, DatabaseHelper.SpellDefinitions.Shatter);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells3 = BuildAutoPreparedSpellGroup(
                9, DatabaseHelper.SpellDefinitions.Fireball, DatabaseHelper.SpellDefinitions.WindWall);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells4 = BuildAutoPreparedSpellGroup(
                13, DatabaseHelper.SpellDefinitions.IceStorm, DatabaseHelper.SpellDefinitions.WallOfFire);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup artilleristSpells5 = BuildAutoPreparedSpellGroup(
                17, DatabaseHelper.SpellDefinitions.ConeOfCold, DatabaseHelper.SpellDefinitions.WallOfForce);

            GuiPresentationBuilder artilleristSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&ArtilleristSubclassSpellsTitle",
                "Feat/&ArtilleristSubclassSpellsDescription");
            FeatureDefinitionAutoPreparedSpells ArtilleristPrepSpells = BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    artilleristSpells1, artilleristSpells2, artilleristSpells3, artilleristSpells4, artilleristSpells5 },
                artificer, "ArtificerArtilleristAutoPrepSpells", artilleristSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleristPrepSpells, 3);

            // Level 3: Cannons
            // Flame
            GuiPresentationBuilder flameGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannonTitle",
                "Feat/&ArtilleristFlameCannonDescription");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fireEffect = new EffectDescriptionBuilder();
            fireEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fireEffect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fireEffect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fireEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flameAttack = new FeatureDefinitionPowerBuilder("ArtilleristFlameCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlameCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fireEffect.Build(),
                flameGui.Build()).AddToDB();
            //    artillerist.AddFeatureAtLevel(flameAttack, 3);

            GuiPresentationBuilder forceGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannonTitle",
                "Feat/&ArtilleristForceCannonDescription");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder forceEffect = new EffectDescriptionBuilder();
            forceEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            forceEffect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            forceEffect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            forceEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower forceAttack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, forceEffect.Build(),
                forceGui.Build()).AddToDB();
            //    artillerist.AddFeatureAtLevel(forceAttack, 3);

            // Protector
            GuiPresentationBuilder protectorGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristProtectorCannonTitle",
                "Feat/&ArtilleristProtectorCannonDescription");
            protectorGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder protectorEffect = new EffectDescriptionBuilder();
            protectorEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 1).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            protectorEffect.SetTargetingData(Side.Ally, RangeType.Self, 2, TargetType.Sphere, 2, 2, ActionDefinitions.ItemSelectionType.None);
            protectorEffect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            protectorEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower protectorActivation = new FeatureDefinitionPowerBuilder("ArtilleristProtectorCannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristProtectorCannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, protectorEffect.Build(),
                protectorGui.Build()).AddToDB();
            //     artillerist.AddFeatureAtLevel(protectorActivation, 3);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryConstructlevel03FeatureSet, 3);

            GuiPresentationBuilder ArtilleryConstructLevel03AutopreparedSpellsPresentation = new GuiPresentationBuilder(
                 "Feat/&ArtilleryConstructLevel03AutopreparedSpellsTitle",
                 "Feat/&ArtilleryConstructLevel03AutopreparedSpellsDescription");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel03AutopreparedSpells = BuildAutoPreparedSpells(
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
                "Feat/&ArtificerArtilleristArcaneFirearmTitle",
                "Feat/&ArtificerArtilleristArcaneFirearmDescription");
            FeatureDefinitionAdditionalDamage arcaneFirearm = new FeatureDefinitionAdditionalDamageBuilder("ArtificerArtilleristArcaneFirearm",
                 GuidHelper.Create(TinkererClass.GuidNamespace, "ArtificerArtilleristArcaneFirearm").ToString(), "ArcaneFirearm",
                FeatureLimitedUsage.OncePerTurn, AdditionalDamageValueDetermination.Die, AdditionalDamageTriggerCondition.EvocationSpellDamage, AdditionalDamageRequiredProperty.None,
                false /* attack only */, DieType.D8, 1 /* dice number */, AdditionalDamageType.SameAsBaseDamage, "", AdditionalDamageAdvancement.None,
                new List<DiceByRank>(), false, AttributeDefinitions.Wisdom, 0, EffectSavingThrowType.None, new List<ConditionOperationDescription>(),
                arcaneFirearmGui.Build()).AddToDB();
            artillerist.AddFeatureAtLevel(arcaneFirearm, 5);

            GuiPresentationBuilder detonationGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristCannonDetonationTitle",
                "Feat/&ArtilleristCannonDetonationDescription");

            EffectDescriptionBuilder detonationEffect = new EffectDescriptionBuilder();
            detonationEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                .Build());
            detonationEffect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            detonationEffect.SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 4, 4, ActionDefinitions.ItemSelectionType.None);
            detonationEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            SpellDefinition detonation = SpellDefinitionBuilder
                .Create("ArtilleristCannonDetonation", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.Action)
                .SetEffectDescription(detonationEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(detonationGui.Build())
                .AddToDB();

            // TODO- add an option to enable the power/spell version of the Blaster (there have been some requests for this) instead of the summons
            GuiPresentationBuilder artilleristDetonationPreparedPresentation = new GuiPresentationBuilder(
                "Feat/&ArtificerArtillerstDetonationSpellPreparedTitle",
                "Feat/&ArtificerArtillerstDetonationSpellPreparedDescription");

            // TODO: unused
#pragma warning disable IDE0059, S1481 // Unused local variables should be removed
            FeatureDefinitionAutoPreparedSpells artilleristDetonationSpell = BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    BuildAutoPreparedSpellGroup(9, new List<SpellDefinition>() { detonation })
                },
                artificer, "ArtificerArtillerstDetonationSpellPrepared", artilleristDetonationPreparedPresentation.Build());
            //    artillerist.AddFeatureAtLevel(artilleristDetonationSpell, 9);
#pragma warning restore IDE0059, S1481 // Unused local variables should be removed

            // cannons with boosted damage
            GuiPresentationBuilder flame9Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannon9Title",
                "Feat/&ArtilleristFlameCannon9Description");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fire9Effect = new EffectDescriptionBuilder();
            fire9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire9Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire9Effect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire9Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flame9Attack = new FeatureDefinitionPowerBuilder("ArtilleristFlame9CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlame9CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire9Effect.Build(),
                flame9Gui.Build(), flameAttack).AddToDB();
            //    artillerist.AddFeatureAtLevel(flame9Attack, 9);

            // Force
            GuiPresentationBuilder force9Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannon9Title",
                "Feat/&ArtilleristForceCannon9Description");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder force9Effect = new EffectDescriptionBuilder();
            force9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force9Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force9Effect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            force9Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower force9Attack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannon9Attack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannon9Attack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force9Effect.Build(),
                force9Gui.Build(), forceAttack).AddToDB();
            //    artillerist.AddFeatureAtLevel(force9Attack, 9);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel09FeatureSetBuilder.ArtilleryConstructlevel09FeatureSet, 9);

            GuiPresentationBuilder ArtilleryConstructLevel09AutopreparedSpellsPresentation = new GuiPresentationBuilder(
                 "Feat/&ArtilleryConstructLevel09AutopreparedSpellsTitle",
                 "Feat/&ArtilleryConstructLevel09AutopreparedSpellsDescription");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel09AutopreparedSpells = BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() {
                        ClassLevel=1,
                        SpellsList=new List<SpellDefinition>
                            {SummonArtillerySpellConstruct9Builder.SummonArtillerySpellConstruct9}} },
                artificer,
                "ArtilleryConstructLevel09AutopreparedSpells",
                ArtilleryConstructLevel09AutopreparedSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleryConstructLevel09AutopreparedSpells, 09);

            // cannons doubled
            GuiPresentationBuilder flame15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristFlameCannon15Title",
                "Feat/&ArtilleristFlameCannon15Description");
            flameGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder fire15Effect = new EffectDescriptionBuilder();
            fire15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 6, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire15Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire15Effect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            // TODO: @Chris - review
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower flame15Attack = new FeatureDefinitionPowerBuilder("ArtilleristFlame15CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristFlame15CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire15Effect.Build(),
                flame15Gui.Build(), flame9Attack).AddToDB();
            //    artillerist.AddFeatureAtLevel(flame15Attack, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            // Force
            GuiPresentationBuilder force15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannon15Title",
                "Feat/&ArtilleristForceCannon15Description");
            forceGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder force15Effect = new EffectDescriptionBuilder();
            force15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force15Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force15Effect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 2, 2, ActionDefinitions.ItemSelectionType.None);
            force15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            // TODO: @Chris - review
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower force15Attack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannon15Attack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristForceCannon15Attack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force15Effect.Build(),
                force15Gui.Build(), force9Attack).AddToDB();
            //    artillerist.AddFeatureAtLevel(force15Attack, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            // Protector
            GuiPresentationBuilder protector15Gui = new GuiPresentationBuilder(
                "Feat/&ArtilleristProtectorCannon15Title",
                "Feat/&ArtilleristProtectorCannon15Description");
            protectorGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder protector15Effect = new EffectDescriptionBuilder();
            protector15Effect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 2).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            protector15Effect.SetTargetingData(Side.Ally, RangeType.Self, 2, TargetType.Sphere, 4, 4, ActionDefinitions.ItemSelectionType.None);
            protector15Effect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            protector15Effect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            // TODO: @Chris - review
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower protector15Activation = new FeatureDefinitionPowerBuilder("ArtilleristProtector15CannonAttack", GuidHelper.Create(TinkererClass.GuidNamespace, "ArtilleristProtector15CannonAttack").ToString(),
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, protector15Effect.Build(),
                protector15Gui.Build(), protectorActivation).AddToDB();
            //    artillerist.AddFeatureAtLevel(protector15Activation, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel15FeatureSetBuilder.ArtilleryConstructlevel15FeatureSet, 15);

            GuiPresentationBuilder ArtilleryConstructLevel15AutopreparedSpellsPresentation = new GuiPresentationBuilder(
    "Feat/&ArtilleryConstructLevel15AutopreparedSpellsTitle",
    "Feat/&ArtilleryConstructLevel15AutopreparedSpellsDescription");
            FeatureDefinitionAutoPreparedSpells ArtilleryConstructLevel15AutopreparedSpells = BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() {
                        ClassLevel=1,
                        SpellsList=new List<SpellDefinition>
                            {SummonArtillerySpellConstruct15Builder.SummonArtillerySpellConstruct15}} },
                artificer,
                "ArtilleryConstructLevel15AutopreparedSpells",
                ArtilleryConstructLevel15AutopreparedSpellsPresentation.Build());
            artillerist.AddFeatureAtLevel(ArtilleryConstructLevel15AutopreparedSpells, 15);

            // build the subclass and add to the db
            return artillerist.AddToDB();
        }
    }
}
