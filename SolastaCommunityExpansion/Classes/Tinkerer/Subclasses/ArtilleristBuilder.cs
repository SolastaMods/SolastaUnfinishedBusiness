using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    public static class ArtilleristBuilder
    {
#pragma warning disable IDE0060, RCS1163 // Unused parameter.
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer, FeatureDefinitionCastSpell spellCasting)
#pragma warning restore IDE0060, RCS1163 // Unused parameter.
        {
            // Make Artillerist subclass
            CharacterSubclassDefinitionBuilder artillerist = CharacterSubclassDefinitionBuilder
                .Create("Artillerist", TinkererClass.GuidNamespace)
                .SetGuiPresentation("ArtificerArtillerist", Category.Subclass, TraditionShockArcanist.GuiPresentation.SpriteReference);

            var artilleristPreparedSpells = Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtificerArtilleristAutoPrepSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation("ArtilleristSubclassSpells", Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(3, Shield, Thunderwave),
                    BuildSpellGroup(5, ScorchingRay, Shatter),
                    BuildSpellGroup(9, Fireball, WindWall),
                    BuildSpellGroup(13, IceStorm, WallOfFire),
                    BuildSpellGroup(17, ConeOfCold, WallOfForce))
                .AddToDB();

            artillerist.AddFeatureAtLevel(artilleristPreparedSpells, 3);

            // Level 3: Cannons
            // Flame
            EffectDescriptionBuilder fireEffect = new EffectDescriptionBuilder();
            fireEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fireEffect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fireEffect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fireEffect.SetParticleEffectParameters(BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flameAttack = new FeatureDefinitionPowerBuilder("ArtilleristFlameCannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fireEffect.Build())
                .SetGuiPresentation("ArtilleristFlameCannon", Category.Feat, BurningHands.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(flameAttack, 3);

            GuiPresentationBuilder forceGui = new GuiPresentationBuilder(
                "Feat/&ArtilleristForceCannonTitle",
                "Feat/&ArtilleristForceCannonDescription");
            forceGui.SetSpriteReference(MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder forceEffect = new EffectDescriptionBuilder();
            forceEffect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 2, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            forceEffect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            forceEffect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            forceEffect.SetParticleEffectParameters(MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower forceAttack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, forceEffect.Build())
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(forceAttack, 3);

            // Protector
            EffectDescriptionBuilder protectorEffect = new EffectDescriptionBuilder();
            protectorEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 1).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            protectorEffect.SetTargetingData(Side.Ally, RangeType.Self, 2, TargetType.Sphere, 2, 2, ActionDefinitions.ItemSelectionType.None);
            protectorEffect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            protectorEffect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower protectorActivation = new FeatureDefinitionPowerBuilder("ArtilleristProtectorCannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, protectorEffect.Build())
                .SetGuiPresentation("ArtilleristProtectorCannon", Category.Feat, FalseLife.GuiPresentation.SpriteReference)
                .AddToDB();
            //     artillerist.AddFeatureAtLevel(protectorActivation, 3);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryConstructlevel03FeatureSet, 3);

            var artilleryConstructLevel03AutopreparedSpells = Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtilleryConstructLevel03AutopreparedSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(1, SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct))
                .AddToDB();

            artillerist.AddFeatureAtLevel(artilleryConstructLevel03AutopreparedSpells, 03);

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
            detonationEffect.SetParticleEffectParameters(MagicMissile.EffectDescription.EffectParticleParameters);

            SpellDefinition detonation = SpellDefinitionBuilder
                .Create("ArtilleristCannonDetonation", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.Action)
                .SetEffectDescription(detonationEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(detonationGui.Build())
                .AddToDB();

            // TODO- add an option to enable the power/spell version of the Blaster (there have been some requests for this) instead of the summons
#pragma warning disable IDE0059, S1481 // Unused local variables should be removed
            FeatureDefinitionAutoPreparedSpells artilleristDetonationSpell = Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtificerArtillerstDetonationSpellPrepared", TinkererClass.GuidNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(BuildSpellGroup(9, detonation))
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(artilleristDetonationSpell, 9);
#pragma warning restore IDE0059, S1481 // Unused local variables should be removed

            // cannons with boosted damage
            EffectDescriptionBuilder fire9Effect = new EffectDescriptionBuilder();
            fire9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire9Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire9Effect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire9Effect.SetParticleEffectParameters(BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower flame9Attack = new FeatureDefinitionPowerBuilder("ArtilleristFlame9CannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                ActivationTime.BonusAction, 0, RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire9Effect.Build(), flameAttack)
                .SetGuiPresentation("ArtilleristFlameCannon9", Category.Feat, BurningHands.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(flame9Attack, 9);

            // Force
            EffectDescriptionBuilder force9Effect = new EffectDescriptionBuilder();
            force9Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force9Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force9Effect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            force9Effect.SetParticleEffectParameters(MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
            FeatureDefinitionPower force9Attack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannon9Attack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                ActivationTime.BonusAction, 0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force9Effect.Build(), forceAttack)
                .SetGuiPresentation("ArtilleristForceCannon9", Category.Feat, MagicMissile.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(force9Attack, 9);

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel09FeatureSetBuilder.ArtilleryConstructlevel09FeatureSet, 9);

            var artilleryConstructLevel09AutopreparedSpells = Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtilleryConstructLevel09AutopreparedSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetCastingClass(artificer)
                // TODO: should this be level 1 or level 9?
                .SetPreparedSpellGroups(BuildSpellGroup(1, SummonArtillerySpellConstruct9Builder.SummonArtillerySpellConstruct9))
                .AddToDB();

            artillerist.AddFeatureAtLevel(artilleryConstructLevel09AutopreparedSpells, 09);

            // cannons doubled
            EffectDescriptionBuilder fire15Effect = new EffectDescriptionBuilder();
            fire15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeFire, 0, DieType.D8, 6, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build());
            fire15Effect.SetSavingThrowData(true, false, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence,
                15, false, new List<SaveAffinityBySenseDescription>());
            fire15Effect.SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Cone, 3, 2, ActionDefinitions.ItemSelectionType.None);
            fire15Effect.SetParticleEffectParameters(BurningHands.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower flame15Attack = new FeatureDefinitionPowerBuilder("ArtilleristFlame15CannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0,
                RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, fire15Effect.Build(), flame9Attack)
                .SetGuiPresentation("ArtilleristFlameCannon15", Category.Feat, BurningHands.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(flame15Attack, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            // Force
            EffectDescriptionBuilder force15Effect = new EffectDescriptionBuilder();
            force15Effect.AddEffectForm(new EffectFormBuilder().SetDamageForm(false, DieType.D8, DamageTypeForce, 0, DieType.D8, 3, HealFromInflictedDamage.Never, new List<TrendInfo>())
                .Build());
            force15Effect.AddEffectForm(new EffectFormBuilder().SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1).Build());
            force15Effect.SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.Individuals, 2, 2, ActionDefinitions.ItemSelectionType.None);
            force15Effect.SetParticleEffectParameters(MagicMissile.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower force15Attack = new FeatureDefinitionPowerBuilder("ArtilleristForceCannon15Attack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction,
                0, RechargeRate.AtWill, true, true, AttributeDefinitions.Intelligence, force15Effect.Build(), force9Attack)
                .SetGuiPresentation("", Category.Feat, MagicMissile.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(force15Attack, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            // Protector
            EffectDescriptionBuilder protector15Effect = new EffectDescriptionBuilder();
            protector15Effect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D8, 2).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            protector15Effect.SetTargetingData(Side.Ally, RangeType.Self, 2, TargetType.Sphere, 4, 4, ActionDefinitions.ItemSelectionType.None);
            protector15Effect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            protector15Effect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

            // TODO- add an option to enable the power version of the Blaster (there have been some requests for this) instead of the summons
#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionPower protector15Activation = new FeatureDefinitionPowerBuilder("ArtilleristProtector15CannonAttack", TinkererClass.GuidNamespace,
                1, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, ActivationTime.BonusAction, 0, RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, protector15Effect.Build(), protectorActivation)
                .SetGuiPresentation("ArtilleristProtectorCannon15", Category.Feat, FalseLife.GuiPresentation.SpriteReference)
                .AddToDB();
            //    artillerist.AddFeatureAtLevel(protector15Activation, 15);
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed

            artillerist.AddFeatureAtLevel(ArtilleryConstructlevel15FeatureSetBuilder.ArtilleryConstructlevel15FeatureSet, 15);

            FeatureDefinitionAutoPreparedSpells artilleryConstructLevel15AutopreparedSpells = Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtilleryConstructLevel15AutopreparedSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetCastingClass(artificer)
                // TODO: should this be level 1 or level 15?
                .SetPreparedSpellGroups(BuildSpellGroup(1, SummonArtillerySpellConstruct15Builder.SummonArtillerySpellConstruct15))
                .AddToDB();

            artillerist.AddFeatureAtLevel(artilleryConstructLevel15AutopreparedSpells, 15);

            // build the subclass and add to the db
            return artillerist.AddToDB();
        }
    }
}
