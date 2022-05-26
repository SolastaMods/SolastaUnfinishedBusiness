using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    public static class AlchemistBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer)
        {
            var alchemistPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtificerAlchemistAutoPrepSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation("AlchemistSubclassSpells", Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(3, HealingWord, InflictWounds, DetectPoisonAndDisease),
                    BuildSpellGroup(5, FlamingSphere, AcidArrow, RayOfEnfeeblement),
                    BuildSpellGroup(9, MassHealingWord, VampiricTouch, RemoveCurse, Slow),
                    BuildSpellGroup(13, Blight, DeathWard),
                    BuildSpellGroup(17, CloudKill, RaiseDead))
                .AddToDB();

            var bonusRecovery = BuildSpellFormPower(
                    "PowerAlchemistSpellBonusRecovery", 2 /* usePerRecharge */, UsesDetermination.Fixed,
                    ActivationTime.Rest, 1 /* cost */, RechargeRate.LongRest)
                .SetGuiPresentation("MagicAffinityAlchemistSpellRecovery", Category.Subclass,
                    PowerWizardArcaneRecovery.GuiPresentation.SpriteReference)
                .AddToDB();

            BuildRestActivity("AlcemicalPreparationRestAction", RestDefinitions.RestStage.AfterRest, RestType.ShortRest,
                    RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", bonusRecovery.Name)
                .SetGuiPresentation("MagicAffinityAlchemistSpellRecovery", Category.Subclass,
                    PowerWizardArcaneRecovery.GuiPresentation.SpriteReference)
                .AddToDB();

            // Healing 2d4+int
            var healEffect = new EffectDescriptionBuilder();
            healEffect.AddEffectForm(new EffectFormBuilder()
                .SetHealingForm(HealingComputation.Dice, 0, DieType.D4, 2, false, HealingCap.MaximumHitPoints)
                .SetBonusMode(AddBonusMode.AbilityBonus)
                .Build());
            healEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals);
            healEffect.SetDurationData(DurationType.Hour, 1, TurnOccurenceType.EndOfTurn);
            healEffect.SetParticleEffectParameters(CureWounds.EffectDescription.EffectParticleParameters);

            var healElixirSpell = SpellDefinitionBuilder
                .Create("AlchemistHealElixir", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(healEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation("ArtificerAlchemistHealElixir", Category.Feat,
                    PowerFunctionPotionOfHealing.GuiPresentation.SpriteReference)
                .AddToDB();

            var swiftnessGui = GuiPresentationBuilder.Build(
                "ArtificerAlchemistSwiftnessElixir", Category.Feat,
                ConditionExpeditiousRetreat.GuiPresentation.SpriteReference);

            // Swiftness speed increase by 10ft 1 hour
            var swiftness = BuildCondition("AlchemistSwiftnessElixirCondition", DurationType.Hour, 1, false,
                swiftnessGui,
                BuildMovementAffinity("AlchemistSwiftnessMovementAffinity", true, 2, 1, swiftnessGui));
            var cancelSwiftness = new CancelConditionPowerBuilder("CancelElixirSwiftness",
                "86888f66-c8b2-49db-910a-bde389dd69df",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirSwiftnessTitle",
                        "Subclass/&CancelCancelElixirSwiftnessDescription")
                    .SetSpriteReference(ConditionExpeditiousRetreat.GuiPresentation.SpriteReference).Build(),
                swiftness).AddToDB();
            swiftness.Features.Add(cancelSwiftness);
            var swiftnessEffect = new EffectDescriptionBuilder();
            var swiftnessForm = new EffectFormBuilder();
            swiftnessForm.SetConditionForm(swiftness, ConditionForm.ConditionOperation.Add, false, false,
                new List<ConditionDefinition>());
            swiftnessEffect.AddEffectForm(swiftnessForm.Build());
            swiftnessEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D1, 0).Build());
            swiftnessEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals);
            swiftnessEffect.SetDurationData(DurationType.Hour, 1, TurnOccurenceType.EndOfTurn);
            swiftnessEffect.SetParticleEffectParameters(Longstrider.EffectDescription.EffectParticleParameters);

            var swiftnessElixirSpell = SpellDefinitionBuilder
                .Create("AlchemistSwiftnessElixir", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(swiftnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(swiftnessGui)
                .AddToDB();

            // Resilience +1 AC 10 minutes
            var resilienceGui = GuiPresentationBuilder
                .Build("ArtificerAlchemistResilienceElixir", Category.Feat,
                    ConditionAuraOfProtection.GuiPresentation.SpriteReference);

            var resilience = BuildCondition("AlchemistResilienceElixirCondition", DurationType.Minute, 10, false,
                resilienceGui,
                BuildAttributeModifier("AlchemistResilienceMovementAffinity",
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass,
                    1, resilienceGui));
            var cancelResilience = new CancelConditionPowerBuilder("CancelElixirResilience",
                "4de693d2-f193-4434-8741-57335d7cefdc",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirResilienceTitle",
                        "Subclass/&CancelCancelElixirResilienceDescription")
                    .SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                resilience).AddToDB();
            resilience.Features.Add(cancelResilience);
            var resilienceEffect = new EffectDescriptionBuilder();
            resilienceEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(resilience,
                ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            resilienceEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals);
            resilienceEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            resilienceEffect.SetParticleEffectParameters(Blur.EffectDescription.EffectParticleParameters);
            var resilienceElixirSpell = SpellDefinitionBuilder
                .Create("AlchemistResilienceElixir", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(resilienceEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(resilienceGui)
                .AddToDB();

            // Boldness effectively 1 minute bless
            var boldnessEffect = new EffectDescriptionBuilder();
            boldnessEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(ConditionBlessed,
                ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            boldnessEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals);
            boldnessEffect.SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn);
            boldnessEffect.SetParticleEffectParameters(Bless.EffectDescription.EffectParticleParameters);

            var boldnessElixirSpell = SpellDefinitionBuilder
                .Create("AlchemistBoldnessElixir", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(boldnessEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation("ArtificerAlchemistBoldnessElixir", Category.Feat,
                    Bless.GuiPresentation.SpriteReference)
                .AddToDB();

            // Flight slow fly speed for 10 minutes
            var flyElixirGui = GuiPresentationBuilder.Build("ArtificerAlchemistFlyElixir", Category.Feat,
                PowerFunctionBootsWinged.GuiPresentation.SpriteReference);

            var fly = BuildCondition("AlchemistFlyElixirCondition", DurationType.Minute, 10, false, flyElixirGui,
                FeatureDefinitionMoveModes.MoveModeFly2);

            var cancelFly = new CancelConditionPowerBuilder("CancelElixirFly", "64385214-7b0f-4372-a12f-8346b2b33884",
                new GuiPresentationBuilder("Subclass/&CancelCancelElixirFlyTitle",
                        "Subclass/&CancelCancelElixirFlyDescription")
                    .SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference).Build(),
                fly).AddToDB();
            fly.Features.Add(cancelFly);

            var flyEffect = new EffectDescriptionBuilder();
            flyEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(fly, ConditionForm.ConditionOperation.Add,
                false, false, new List<ConditionDefinition>()).Build());
            flyEffect.SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals);
            flyEffect.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            flyEffect.SetParticleEffectParameters(Fly.EffectDescription.EffectParticleParameters);
            var flyElixirSpell = SpellDefinitionBuilder
                .Create("AlchemistFlyElixir", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(flyEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation(flyElixirGui)
                .AddToDB();

            var alchemistElixirs = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtificerAlchemistElixirSpellPrep", TinkererClass.GuidNamespace)
                .SetGuiPresentation("AlchemistSubclassElixirs", Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(3, healElixirSpell, swiftnessElixirSpell, resilienceElixirSpell,
                        boldnessElixirSpell, flyElixirSpell))
                .AddToDB();

            // Transformation- Alter Self spell

            // Level 5: Alchemical Savant
            var alchemicalSavantGui = new GuiPresentationBuilder(
                "Feat/&ArtificerAlchemistAlchemicalSavantTitle",
                "Feat/&ArtificerAlchemistAlchemicalSavantDescription");
            var improvedHealing = BuildHealingModifier("ArtificerAlchemistAlchemicalSavantHealing", 1, DieType.D4,
                LevelSourceType.CharacterLevel, alchemicalSavantGui.Build());

            var alchemicalSavantSpellsGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityAlchemicalSavantListTitle",
                "Subclass/&MagicAffinityAlchemicalSavantListDescription");
            var alchemicalSavantSpells = BuildMagicAffinityHeightenedList(
                new List<string> {AcidArrow.Name, FlamingSphere.Name}, 2,
                "MagicAffinityArtificerAlchemicalSavantHeightened", alchemicalSavantSpellsGui.Build());
            alchemicalSavantSpells.SetForceHalfDamageOnCantrips(true);

            var restorativeElixirsPower = new FeatureHelpers
                    .FeatureDefinitionPowerBuilder(
                        "PowerAlchemistRestorativeElixirs", TinkererClass.GuidNamespace,
                        0, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                        ActivationTime.Action, 1, RechargeRate.LongRest,
                        false, false, AttributeDefinitions.Intelligence,
                        LesserRestoration.EffectDescription)
                .SetGuiPresentation("PowerAlchemistRestorativeElixirs", Category.Feat,
                    LesserRestoration.GuiPresentation.SpriteReference)
                .AddToDB();

            var emboldeningShotsEffect = new EffectDescriptionBuilder();
            emboldeningShotsEffect.AddEffectForm(new EffectFormBuilder().SetTempHPForm(0, DieType.D6, 4)
                .SetBonusMode(AddBonusMode.AbilityBonus).Build());
            emboldeningShotsEffect.SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals);
            emboldeningShotsEffect.SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.EndOfTurn);
            emboldeningShotsEffect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

            var emboldeningShots = SpellDefinitionBuilder
                .Create("CantripAlchemistEmboldeningShots", TinkererClass.GuidNamespace)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(ActivationTime.BonusAction)
                .SetEffectDescription(emboldeningShotsEffect.Build())
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetGuiPresentation("PowerAlchemistEmboldeningShots", Category.Feat)
                .AddToDB();

            var emboldeningCantrips = FeatureDefinitionBonusCantripsBuilder
                .Create("ArtificerAlchemistShotsSpellPrep", TinkererClass.GuidNamespace)
                .SetBonusCantrips(emboldeningShots)
                .SetGuiPresentation("PowerAlchemistEmboldeningShots", Category.Feat)
                .AddToDB();

            var greaterRestorativeElixirs = new FeatureHelpers
                    .FeatureDefinitionPowerBuilder("PowerAlchemistGreaterRestorativeElixirs",
                        TinkererClass.GuidNamespace,
                        1, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                        ActivationTime.Action, 1, RechargeRate.LongRest,
                        false, false, AttributeDefinitions.Intelligence,
                        GreaterRestoration.EffectDescription)
                .SetGuiPresentation("PowerAlchemistGreaterRestorativeElixirs", Category.Feat,
                    GreaterRestoration.GuiPresentation.SpriteReference)
                .AddToDB();

            var healSpellEffect = new EffectDescriptionBuilder();
            healSpellEffect.AddEffectForm(new EffectFormBuilder()
                .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 70, false, HealingCap.MaximumHitPoints)
                .SetBonusMode(AddBonusMode.AbilityBonus)
                .Build());
            healSpellEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(
                ConditionDefinitions.ConditionParalyzed, ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                false, false,
                new List<ConditionDefinition>
                {
                    ConditionDefinitions.ConditionBlinded,
                    ConditionDefinitions.ConditionDeafened,
                    ConditionDefinitions.ConditionDiseased,
                    ConditionContagionBlindingSickness,
                    ConditionContagionFilthFever,
                    ConditionContagionFleshRot,
                    ConditionContagionMindfire,
                    ConditionContagionSeizure,
                    ConditionContagionSlimyDoom
                }).Build());
            healSpellEffect.SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Individuals);
            healSpellEffect.SetDurationData(DurationType.Instantaneous, 1, TurnOccurenceType.EndOfTurn);
            healSpellEffect.SetParticleEffectParameters(FalseLife.EffectDescription.EffectParticleParameters);

            var greatHealElixirs = new FeatureHelpers
                    .FeatureDefinitionPowerBuilder("PowerAlchemistHealElixirs", TinkererClass.GuidNamespace,
                        1, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                        ActivationTime.Action, 1, RechargeRate.LongRest,
                        false, false, AttributeDefinitions.Intelligence,
                        healSpellEffect.Build())
                .SetGuiPresentation("PowerAlchemistHealElixirs", Category.Feat)
                .AddToDB();

            return CharacterSubclassDefinitionBuilder
                .Create("Alchemist", TinkererClass.GuidNamespace)
                .SetGuiPresentation("ArtificerAlchemist", Category.Subclass, DomainLife.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(alchemistElixirs, 3)
                .AddFeatureAtLevel(alchemistPreparedSpells, 3)
                .AddFeatureAtLevel(bonusRecovery, 3)
                .AddFeatureAtLevel(improvedHealing, 5)
                .AddFeatureAtLevel(alchemicalSavantSpells, 5)
                .AddFeatureAtLevel(restorativeElixirsPower, 9)
                .AddFeatureAtLevel(emboldeningCantrips, 9)
                .AddFeatureAtLevel(greaterRestorativeElixirs, 15)
                .AddFeatureAtLevel(greatHealElixirs, 15)
                // todo immune poisoned condition
                .AddFeatureAtLevel(FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance, 15)
                .AddFeatureAtLevel(FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance, 15)
                .AddToDB();
        }
    }
}
