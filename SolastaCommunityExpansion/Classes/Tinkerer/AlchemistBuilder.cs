using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    public static class AlchemistBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer)
        {
            // Make Alchemist subclass
            CharacterSubclassDefinitionBuilder alchemist = new CharacterSubclassDefinitionBuilder("Alchemist", GuidHelper.Create(TinkererClass.GuidNamespace, "Alchemist").ToString());
            GuiPresentationBuilder meleePresentation = new GuiPresentationBuilder(
                "Subclass/&ArtificerAlchemistDescription",
                "Subclass/&ArtificerAlchemistTitle");
            meleePresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference);
            alchemist.SetGuiPresentation(meleePresentation.Build());

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistSpells1 = FeatureHelpers.BuildAutoPreparedSpellGroup(3,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.HealingWord, DatabaseHelper.SpellDefinitions.InflictWounds, DatabaseHelper.SpellDefinitions.DetectPoisonAndDisease });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistSpells2 = FeatureHelpers.BuildAutoPreparedSpellGroup(5,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.FlamingSphere, DatabaseHelper.SpellDefinitions.AcidArrow, DatabaseHelper.SpellDefinitions.RayOfEnfeeblement });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistSpells3 = FeatureHelpers.BuildAutoPreparedSpellGroup(9,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.MassHealingWord, DatabaseHelper.SpellDefinitions.VampiricTouch, DatabaseHelper.SpellDefinitions.RemoveCurse, DatabaseHelper.SpellDefinitions.Slow });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistSpells4 = FeatureHelpers.BuildAutoPreparedSpellGroup(13,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Blight, DatabaseHelper.SpellDefinitions.DeathWard });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistSpells5 = FeatureHelpers.BuildAutoPreparedSpellGroup(17,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.CloudKill, DatabaseHelper.SpellDefinitions.RaiseDead });

            GuiPresentationBuilder alchemistSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&AlchemistSubclassSpellsDescription",
                "Feat/&AlchemistSubclassSpellsTitle");
            FeatureDefinitionAutoPreparedSpells AlchemistPrepSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    alchemistSpells1, alchemistSpells2, alchemistSpells3, alchemistSpells4, alchemistSpells5 },
                artificer, "ArtificerAlchemistAutoPrepSpells", alchemistSpellsPresentation.Build());
            alchemist.AddFeatureAtLevel(AlchemistPrepSpells, 3);

            GuiPresentationBuilder spellRecoveryGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityAlchemistSpellRecoveryDescription",
                "Subclass/&MagicAffinityAlchemistSpellRecoveryTitle");
            spellRecoveryGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            FeatureDefinitionPower bonusRecovery = FeatureHelpers.BuildSpellFormPower(2 /* usePerRecharge */, UsesDetermination.Fixed, ActivationTime.Rest,
                1 /* cost */, RechargeRate.LongRest, "PowerAlchemistSpellBonusRecovery", spellRecoveryGui.Build());
            alchemist.AddFeatureAtLevel(bonusRecovery, 3);

            FeatureHelpers.BuildRestActivity(RestDefinitions.RestStage.AfterRest, RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", bonusRecovery.Name, "AlcemicalPreparationRestAction", spellRecoveryGui.Build());

            // Healing 2d4+int
            EffectDescriptionBuilder healEffect = new EffectDescriptionBuilder();
            healEffect.AddEffectForm(new EffectFormBuilder()
                .SetHealingForm(HealingComputation.Dice, 0, DieType.D4, 2, false, HealingCap.MaximumHitPoints)
                .SetBonusMode(AddBonusMode.AbilityBonus)
                .Build());
            healEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            healEffect.SetDurationData(DurationType.Hour, 1, TurnOccurenceType.EndOfTurn);
            healEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription.EffectParticleParameters);
            GuiPresentationBuilder healingElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistHealElixirDescription",
                "Feat/&ArtificerAlchemistHealElixirTitle");
            healingElixirGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionPotionOfHealing.GuiPresentation.SpriteReference);
            SpellDefinition healElixirSpell = new SpellBuilder("AlchemistHealElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistHealElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(healEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(healingElixirGui.Build())
                .AddToDB();

            // Swiftness speed increase by 10ft 1 hour
            GuiPresentationBuilder swiftnessGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistSwiftnessElixirDescription",
                "Feat/&ArtificerAlchemistSwiftnessElixirTitle");
            swiftnessGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionExpeditiousRetreat.GuiPresentation.SpriteReference);
            ConditionDefinition swiftness = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                FeatureHelpers.BuildMovementAffinity(true, 2, 1, "AlchemistSwiftnessMovementAffinity", swiftnessGui.Build())
            }, DurationType.Hour, 1, false, "AlchemistSwiftnessElixirCondition", swiftnessGui.Build());
            FeatureDefinitionPower cancelSwiftness = new CancelConditionPowerBuilder("CancelElixirSwiftness", "86888f66-c8b2-49db-910a-bde389dd69df",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirSwiftnessDescription", "Subclass/&CancelCancelElixirSwiftnessTitle")
                .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionExpeditiousRetreat.GuiPresentation.SpriteReference).Build(),
                swiftness).AddToDB();
            swiftness.Features.Add(cancelSwiftness);
            EffectDescriptionBuilder swiftnessEffect = new EffectDescriptionBuilder();
            EffectFormBuilder swiftnessForm = new EffectFormBuilder();
            swiftnessForm.SetConditionForm(swiftness, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>());
            swiftnessEffect.AddEffectForm(swiftnessForm.Build());
            swiftnessEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D1, 0).Build());
            swiftnessEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            swiftnessEffect.SetDurationData(DurationType.Hour, 1, TurnOccurenceType.EndOfTurn);
            swiftnessEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Longstrider.EffectDescription.EffectParticleParameters);
            SpellDefinition swiftnessElixirSpell = new SpellBuilder("AlchemistSwiftnessElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistSwiftnessElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(swiftnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(swiftnessGui.Build())
                .AddToDB();

            // Resilience +1 AC 10 minutes
            GuiPresentationBuilder resilienceGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistResilienceElixirDescription",
                "Feat/&ArtificerAlchemistResilienceElixirTitle");
            resilienceGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            ConditionDefinition resilience = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                FeatureHelpers.BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1,
                "AlchemistResilienceMovementAffinity", resilienceGui.Build())
            }, DurationType.Minute, 10, false, "AlchemistResilienceElixirCondition", resilienceGui.Build());
            FeatureDefinitionPower cancelResilience = new CancelConditionPowerBuilder("CancelElixirResilience", "4de693d2-f193-4434-8741-57335d7cefdc",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirResilienceDescription", "Subclass/&CancelCancelElixirResilienceTitle")
                .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                resilience).AddToDB();
            resilience.Features.Add(cancelResilience);
            EffectDescriptionBuilder resilienceEffect = new EffectDescriptionBuilder();
            resilienceEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(resilience, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            resilienceEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            resilienceEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            resilienceEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Blur.EffectDescription.EffectParticleParameters);
            SpellDefinition resilienceElixirSpell = new SpellBuilder("AlchemistResilienceElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistResilienceElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(resilienceEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(resilienceGui.Build())
                .AddToDB();

            // Boldness effectively 1 minute bless
            GuiPresentationBuilder boldnessElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistBoldnessElixirDescription",
                "Feat/&ArtificerAlchemistBoldnessElixirTitle");
            boldnessElixirGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.Bless.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder boldnessEffect = new EffectDescriptionBuilder();
            boldnessEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(DatabaseHelper.ConditionDefinitions.ConditionBlessed, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            boldnessEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            boldnessEffect.SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn);
            boldnessEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Bless.EffectDescription.EffectParticleParameters);

            SpellDefinition boldnessElixirSpell = new SpellBuilder("AlchemistBoldnessElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistBoldnessElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(boldnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(boldnessElixirGui.Build())
                .AddToDB();

            // Flight slow fly speed for 10 minutes
            GuiPresentationBuilder flyElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistFlyElixirDescription",
                "Feat/&ArtificerAlchemistFlyElixirTitle");
            flyElixirGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionBootsWinged.GuiPresentation.SpriteReference);
            ConditionDefinition fly = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly2,
            }, DurationType.Minute, 10, false, "AlchemistFlyElixirCondition", flyElixirGui.Build());

            FeatureDefinitionPower cancelFly = new CancelConditionPowerBuilder("CancelElixirFly", "64385214-7b0f-4372-a12f-8346b2b33884",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirFlyDescription", "Subclass/&CancelCancelElixirFlyTitle")
                .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                fly).AddToDB();
            fly.Features.Add(cancelFly);

            EffectDescriptionBuilder flyEffect = new EffectDescriptionBuilder();
            flyEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(fly, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            flyEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            flyEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            flyEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Fly.EffectDescription.EffectParticleParameters);
            SpellDefinition flyElixirSpell = new SpellBuilder("AlchemistFlyElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistFlyElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(flyEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(flyElixirGui.Build())
                .AddToDB();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistElixirs = FeatureHelpers.BuildAutoPreparedSpellGroup(3,
                new List<SpellDefinition>() { healElixirSpell, swiftnessElixirSpell, resilienceElixirSpell, boldnessElixirSpell, flyElixirSpell });

            GuiPresentationBuilder alchemistElixirsPresentation = new GuiPresentationBuilder(
                "Feat/&AlchemistSubclassElixirsDescription",
                "Feat/&AlchemistSubclassElixirsTitle");
            FeatureDefinitionAutoPreparedSpells AlchemistElixirs = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() { alchemistElixirs },
                artificer, "ArtificerAlchemistElixirSpellPrep", alchemistElixirsPresentation.Build());
            alchemist.AddFeatureAtLevel(AlchemistElixirs, 3);

            // Transformation- Alter Self spell

            // Level 5: Alchemical Savant
            GuiPresentationBuilder alchemicalSavantGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistAlchemicalSavantDescription",
                "Feat/&ArtificerAlchemistAlchemicalSavantTitle");
            FeatureDefinitionHealingModifier improvedHealing = FeatureHelpers.BuildHealingModifier(1, DieType.D4, LevelSourceType.CharacterLevel,
                "ArtificerAlchemistAlchemicalSavantHealing", alchemicalSavantGui.Build());
            alchemist.AddFeatureAtLevel(improvedHealing, 5);

            GuiPresentationBuilder alchemicalSavantSpellsGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityAlchemicalSavantListDescription",
                "Subclass/&MagicAffinityAlchemicalSavantListTitle");
            FeatureDefinitionMagicAffinity alchemicalSavantSpells = FeatureHelpers.BuildMagicAffinityHeightenedList(new List<string>() {
                DatabaseHelper.SpellDefinitions.AcidArrow.Name,
                DatabaseHelper.SpellDefinitions.FlamingSphere.Name,
            }, 2,
                "MagicAffinityArtificerAlchemicalSavantHeightened", alchemicalSavantSpellsGui.Build());
            alchemicalSavantSpells.SetForceHalfDamageOnCantrips(true);
            alchemist.AddFeatureAtLevel(alchemicalSavantSpells, 5);

            GuiPresentationBuilder restorativeElixirs = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistRestorativeElixirsDescription",
                "Feat/&PowerAlchemistRestorativeElixirsTitle");
            restorativeElixirs.SetSpriteReference(DatabaseHelper.SpellDefinitions.LesserRestoration.GuiPresentation.SpriteReference);
            FeatureDefinitionPower restorativeElixisrPower = new FeatureHelpers.FeatureDefinitionPowerBuilder("PowerAlchemistRestorativeElixirs", GuidHelper.Create(TinkererClass.GuidNamespace, "PowerAlchemistRestorativeElixirs").ToString(),
                0, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                ActivationTime.Action, 1, RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.LesserRestoration.EffectDescription,
                restorativeElixirs.Build()).AddToDB();
            alchemist.AddFeatureAtLevel(restorativeElixisrPower, 9);

            GuiPresentationBuilder emboldeningShotsGui = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistEmboldeningShotsDescription",
                "Feat/&PowerAlchemistEmboldeningShotsTitle");
            EffectDescriptionBuilder emboldeningShotsEffect = new EffectDescriptionBuilder();
            emboldeningShotsEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D6, 4).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            emboldeningShotsEffect.SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            emboldeningShotsEffect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            emboldeningShotsEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            SpellDefinition emboldeningShots = new SpellBuilder("CantripAlchemistEmboldeningShots", GuidHelper.Create(TinkererClass.GuidNamespace, "CantripAlchemistEmboldeningShots").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(emboldeningShotsEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(emboldeningShotsGui.Build())
                .AddToDB();

            FeatureDefinitionBonusCantrips emboldeningCantrips = FeatureHelpers.BuildBonusCantrips(new List<SpellDefinition>()
            {
                emboldeningShots
            }, "ArtificerAlchemistShotsSpellPrep", emboldeningShotsGui.Build());
            alchemist.AddFeatureAtLevel(emboldeningCantrips, 9);

            GuiPresentationBuilder greaterRestorativeElixirsGui = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistGreaterRestorativeElixirsDescription",
                "Feat/&PowerAlchemistGreaterRestorativeElixirsTitle");
            restorativeElixirs.SetSpriteReference(DatabaseHelper.SpellDefinitions.GreaterRestoration.GuiPresentation.SpriteReference);
            FeatureDefinitionPower greaterRestorativeElixirs = new FeatureHelpers.FeatureDefinitionPowerBuilder("PowerAlchemistGreaterRestorativeElixirs", GuidHelper.Create(TinkererClass.GuidNamespace, "PowerAlchemistGreaterRestorativeElixirs").ToString(),
                1, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                ActivationTime.Action, 1, RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.GreaterRestoration.EffectDescription,
                greaterRestorativeElixirsGui.Build()).AddToDB();
            alchemist.AddFeatureAtLevel(greaterRestorativeElixirs, 15);

            GuiPresentationBuilder healElixirsGui = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistHealElixirsDescription",
                "Feat/&PowerAlchemistHealElixirsTitle");
            EffectDescriptionBuilder healSpellEffect = new EffectDescriptionBuilder();
            healSpellEffect.AddEffectForm(new EffectFormBuilder()
                .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 70, false, HealingCap.MaximumHitPoints)
                .SetBonusMode(AddBonusMode.AbilityBonus)
                .Build());
            healSpellEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(DatabaseHelper.ConditionDefinitions.ConditionParalyzed, ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                false, false, new List<ConditionDefinition>() {
                    DatabaseHelper.ConditionDefinitions.ConditionBlinded,
                    DatabaseHelper.ConditionDefinitions.ConditionDeafened,
                    DatabaseHelper.ConditionDefinitions.ConditionDiseased,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionBlindingSickness,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionFilthFever,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionFleshRot,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionMindfire,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionSeizure,
                    DatabaseHelper.ConditionDefinitions.ConditionContagionSlimyDoom,
                }).Build());
            healSpellEffect.SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            healSpellEffect.SetDurationData(DurationType.Instantaneous, 1, TurnOccurenceType.EndOfTurn);
            healSpellEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription.EffectParticleParameters);

            FeatureDefinitionPower greatHealElixirs = new FeatureHelpers.FeatureDefinitionPowerBuilder("PowerAlchemistHealElixirs",
                GuidHelper.Create(TinkererClass.GuidNamespace, "PowerAlchemistHealElixirs").ToString(),
                1, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                ActivationTime.Action, 1, RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                healSpellEffect.Build(),
                healElixirsGui.Build()).AddToDB();
            alchemist.AddFeatureAtLevel(greatHealElixirs, 15);

            // todo immune poisoned condition
            alchemist.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance, 15);
            alchemist.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance, 15);

            // build the subclass and add tot he db
            return alchemist.AddToDB();
        }
    }
}
