using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    public static class AlchemistBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer)
        {
            // Make Alchemist subclass
            CharacterSubclassDefinitionBuilder alchemist = new CharacterSubclassDefinitionBuilder("Alchemist", GuidHelper.Create(TinkererClass.GuidNamespace, "Alchemist").ToString());
            GuiPresentationBuilder meleePresentation = new GuiPresentationBuilder(
                "Subclass/&ArtificerAlchemistTitle",
                "Subclass/&ArtificerAlchemistDescription");
            meleePresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference);
            alchemist.SetGuiPresentation(meleePresentation.Build());

            GuiPresentationBuilder alchemistSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&AlchemistSubclassSpellsTitle",
                "Feat/&AlchemistSubclassSpellsDescription");

            var alchemistPreparedSpells = FeatureHelpers.BuildAutoPreparedSpells(
                artificer, "ArtificerAlchemistAutoPrepSpells", alchemistSpellsPresentation.Build(),
                FeatureHelpers.BuildAutoPreparedSpellGroup(3, HealingWord, InflictWounds, DetectPoisonAndDisease),
                FeatureHelpers.BuildAutoPreparedSpellGroup(5, FlamingSphere, AcidArrow, RayOfEnfeeblement),
                FeatureHelpers.BuildAutoPreparedSpellGroup(9, MassHealingWord, VampiricTouch, RemoveCurse, Slow),
                FeatureHelpers.BuildAutoPreparedSpellGroup(13, Blight, DeathWard),
                FeatureHelpers.BuildAutoPreparedSpellGroup(17, CloudKill, RaiseDead));

            alchemist.AddFeatureAtLevel(alchemistPreparedSpells, 3);

            GuiPresentationBuilder spellRecoveryGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityAlchemistSpellRecoveryTitle",
                "Subclass/&MagicAffinityAlchemistSpellRecoveryDescription");
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
            healEffect.SetParticleEffectParameters(CureWounds.EffectDescription.EffectParticleParameters);
            GuiPresentationBuilder healingElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistHealElixirTitle",
                "Feat/&ArtificerAlchemistHealElixirDescription");
            healingElixirGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionPotionOfHealing.GuiPresentation.SpriteReference);
            SpellDefinition healElixirSpell = new SpellDefinitionBuilder("AlchemistHealElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistHealElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(healEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(healingElixirGui.Build())
                .AddToDB();

            // Swiftness speed increase by 10ft 1 hour
            GuiPresentationBuilder swiftnessGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistSwiftnessElixirTitle",
                "Feat/&ArtificerAlchemistSwiftnessElixirDescription");
            swiftnessGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionExpeditiousRetreat.GuiPresentation.SpriteReference);
            ConditionDefinition swiftness = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                FeatureHelpers.BuildMovementAffinity(true, 2, 1, "AlchemistSwiftnessMovementAffinity", swiftnessGui.Build())
            }, DurationType.Hour, 1, false, "AlchemistSwiftnessElixirCondition", swiftnessGui.Build());
            FeatureDefinitionPower cancelSwiftness = new CancelConditionPowerBuilder("CancelElixirSwiftness", "86888f66-c8b2-49db-910a-bde389dd69df",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirSwiftnessTitle", "Subclass/&CancelCancelElixirSwiftnessDescription")
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
            swiftnessEffect.SetParticleEffectParameters(Longstrider.EffectDescription.EffectParticleParameters);
            SpellDefinition swiftnessElixirSpell = new SpellDefinitionBuilder("AlchemistSwiftnessElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistSwiftnessElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(swiftnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(swiftnessGui.Build())
                .AddToDB();

            // Resilience +1 AC 10 minutes
            GuiPresentationBuilder resilienceGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistResilienceElixirTitle",
                "Feat/&ArtificerAlchemistResilienceElixirDescription");
            resilienceGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            ConditionDefinition resilience = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                FeatureHelpers.BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1,
                "AlchemistResilienceMovementAffinity", resilienceGui.Build())
            }, DurationType.Minute, 10, false, "AlchemistResilienceElixirCondition", resilienceGui.Build());
            FeatureDefinitionPower cancelResilience = new CancelConditionPowerBuilder("CancelElixirResilience", "4de693d2-f193-4434-8741-57335d7cefdc",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirResilienceTitle", "Subclass/&CancelCancelElixirResilienceDescription")
                .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                resilience).AddToDB();
            resilience.Features.Add(cancelResilience);
            EffectDescriptionBuilder resilienceEffect = new EffectDescriptionBuilder();
            resilienceEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(resilience, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            resilienceEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            resilienceEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            resilienceEffect.SetParticleEffectParameters(Blur.EffectDescription.EffectParticleParameters);
            SpellDefinition resilienceElixirSpell = new SpellDefinitionBuilder("AlchemistResilienceElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistResilienceElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(resilienceEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(resilienceGui.Build())
                .AddToDB();

            // Boldness effectively 1 minute bless
            GuiPresentationBuilder boldnessElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistBoldnessElixirTitle",
                "Feat/&ArtificerAlchemistBoldnessElixirDescription");
            boldnessElixirGui.SetSpriteReference(Bless.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder boldnessEffect = new EffectDescriptionBuilder();
            boldnessEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(DatabaseHelper.ConditionDefinitions.ConditionBlessed, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            boldnessEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            boldnessEffect.SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn);
            boldnessEffect.SetParticleEffectParameters(Bless.EffectDescription.EffectParticleParameters);

            SpellDefinition boldnessElixirSpell = new SpellDefinitionBuilder("AlchemistBoldnessElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistBoldnessElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(boldnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(boldnessElixirGui.Build())
                .AddToDB();

            // Flight slow fly speed for 10 minutes
            GuiPresentationBuilder flyElixirGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistFlyElixirTitle",
                "Feat/&ArtificerAlchemistFlyElixirDescription");
            flyElixirGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionBootsWinged.GuiPresentation.SpriteReference);
            ConditionDefinition fly = FeatureHelpers.BuildCondition(new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly2,
            }, DurationType.Minute, 10, false, "AlchemistFlyElixirCondition", flyElixirGui.Build());

            FeatureDefinitionPower cancelFly = new CancelConditionPowerBuilder("CancelElixirFly", "64385214-7b0f-4372-a12f-8346b2b33884",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirFlyTitle", "Subclass/&CancelCancelElixirFlyDescription")
                .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                fly).AddToDB();
            fly.Features.Add(cancelFly);

            EffectDescriptionBuilder flyEffect = new EffectDescriptionBuilder();
            flyEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(fly, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            flyEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            flyEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            flyEffect.SetParticleEffectParameters(Fly.EffectDescription.EffectParticleParameters);
            SpellDefinition flyElixirSpell = new SpellDefinitionBuilder("AlchemistFlyElixir", GuidHelper.Create(TinkererClass.GuidNamespace, "AlchemistFlyElixir").ToString())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(flyEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(flyElixirGui.Build())
                .AddToDB();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup alchemistElixirs = FeatureHelpers.BuildAutoPreparedSpellGroup(
                3, healElixirSpell, swiftnessElixirSpell, resilienceElixirSpell, boldnessElixirSpell, flyElixirSpell);

            GuiPresentationBuilder alchemistElixirsPresentation = new GuiPresentationBuilder(
                "Feat/&AlchemistSubclassElixirsTitle",
                "Feat/&AlchemistSubclassElixirsDescription");
            FeatureDefinitionAutoPreparedSpells AlchemistElixirs = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() { alchemistElixirs },
                artificer, "ArtificerAlchemistElixirSpellPrep", alchemistElixirsPresentation.Build());
            alchemist.AddFeatureAtLevel(AlchemistElixirs, 3);

            // Transformation- Alter Self spell

            // Level 5: Alchemical Savant
            GuiPresentationBuilder alchemicalSavantGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistAlchemicalSavantTitle",
                "Feat/&ArtificerAlchemistAlchemicalSavantDescription");
            FeatureDefinitionHealingModifier improvedHealing = FeatureHelpers.BuildHealingModifier(1, DieType.D4, LevelSourceType.CharacterLevel,
                "ArtificerAlchemistAlchemicalSavantHealing", alchemicalSavantGui.Build());
            alchemist.AddFeatureAtLevel(improvedHealing, 5);

            GuiPresentationBuilder alchemicalSavantSpellsGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityAlchemicalSavantListTitle",
                "Subclass/&MagicAffinityAlchemicalSavantListDescription");
            FeatureDefinitionMagicAffinity alchemicalSavantSpells = FeatureHelpers.BuildMagicAffinityHeightenedList(new List<string>() {
                AcidArrow.Name,
                FlamingSphere.Name,
            }, 2,
                "MagicAffinityArtificerAlchemicalSavantHeightened", alchemicalSavantSpellsGui.Build());
            alchemicalSavantSpells.SetForceHalfDamageOnCantrips(true);
            alchemist.AddFeatureAtLevel(alchemicalSavantSpells, 5);

            GuiPresentationBuilder restorativeElixirs = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistRestorativeElixirsTitle",
                "Feat/&PowerAlchemistRestorativeElixirsDescription");
            restorativeElixirs.SetSpriteReference(LesserRestoration.GuiPresentation.SpriteReference);
            FeatureDefinitionPower restorativeElixisrPower = new FeatureHelpers.FeatureDefinitionPowerBuilder("PowerAlchemistRestorativeElixirs", GuidHelper.Create(TinkererClass.GuidNamespace, "PowerAlchemistRestorativeElixirs").ToString(),
                0, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                ActivationTime.Action, 1, RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                LesserRestoration.EffectDescription,
                restorativeElixirs.Build()).AddToDB();
            alchemist.AddFeatureAtLevel(restorativeElixisrPower, 9);

            GuiPresentationBuilder emboldeningShotsGui = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistEmboldeningShotsTitle",
                "Feat/&PowerAlchemistEmboldeningShotsDescription");
            EffectDescriptionBuilder emboldeningShotsEffect = new EffectDescriptionBuilder();
            emboldeningShotsEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D6, 4).SetBonusMode(AddBonusMode.AbilityBonus).Build());
            emboldeningShotsEffect.SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            emboldeningShotsEffect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            emboldeningShotsEffect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

            SpellDefinition emboldeningShots = new SpellDefinitionBuilder("CantripAlchemistEmboldeningShots", GuidHelper.Create(TinkererClass.GuidNamespace, "CantripAlchemistEmboldeningShots").ToString())
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
                "Feat/&PowerAlchemistGreaterRestorativeElixirsTitle",
                "Feat/&PowerAlchemistGreaterRestorativeElixirsDescription");
            restorativeElixirs.SetSpriteReference(GreaterRestoration.GuiPresentation.SpriteReference);
            FeatureDefinitionPower greaterRestorativeElixirs = new FeatureHelpers.FeatureDefinitionPowerBuilder("PowerAlchemistGreaterRestorativeElixirs", GuidHelper.Create(TinkererClass.GuidNamespace, "PowerAlchemistGreaterRestorativeElixirs").ToString(),
                1, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                ActivationTime.Action, 1, RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                GreaterRestoration.EffectDescription,
                greaterRestorativeElixirsGui.Build()).AddToDB();
            alchemist.AddFeatureAtLevel(greaterRestorativeElixirs, 15);

            GuiPresentationBuilder healElixirsGui = new GuiPresentationBuilder(
                "Feat/&PowerAlchemistHealElixirsTitle",
                "Feat/&PowerAlchemistHealElixirsDescription");
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
            healSpellEffect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

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
