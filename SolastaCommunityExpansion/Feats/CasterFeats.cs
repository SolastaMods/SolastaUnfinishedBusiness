using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class CasterFeats
    {
        public static readonly Guid CasterFeatsNamespace = new("bf70984d-e7b9-446a-9ae3-0f2039de833d");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            FeatureDefinition intIncrement = BuildAdditiveAttributeModifier("FeatIntIncrement", AttributeDefinitions.Intelligence, 1);
            FeatureDefinition chaIncrement = BuildAdditiveAttributeModifier("FeatChaIncrement", AttributeDefinitions.Charisma, 1);
            FeatureDefinition wisIncrement = BuildAdditiveAttributeModifier("FeatWisIncrement", AttributeDefinitions.Wisdom, 1);

            static FeatureDefinitionAttributeModifier BuildAdditiveAttributeModifier(string name, string attribute, int amount)
            {
                return new FeatureDefinitionAttributeModifierBuilder(name, CasterFeatsNamespace, Category.Feat)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, attribute, amount)
                    .AddToDB();
            }

            // Note it seems that feats can't currently grant bonus cantrips (which is kind of fine since the game doesn't have mage hand).
            //GuiPresentationBuilder telekineticBonusCantripPresentation = new GuiPresentationBuilder(
            //    "Feat/&FeatTelekineticBonusCantripDescription", ""),
            //    "Feat/&FeatTelekineticBonusCantripTitle", ""));
            //FeatureDefinitionBonusCantrips telekineticBonusCantrips = FeatureBuilder.BuildBonusCantrips(new List<SpellDefinition>()
            //{
            //    DatabaseHelper.SpellDefinitions.AnnoyingBee,
            //    DatabaseHelper.SpellDefinitions.Sparkle,
            //    DatabaseHelper.SpellDefinitions.Dazzle,
            //}, "FeatTelekineticBonusCantrip", telekineticBonusCantripPresentation.Build());

            // telekinetic int
            GuiPresentationBuilder intPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticIntPushTitle",
                "Feat/&FeatTelekineticIntPushDescription");
            intPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower intPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticIntPush", intPushPresentation.Build());

            GuiPresentationBuilder intPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticIntPullTitle",
                "Feat/&FeatTelekineticIntPullDescription");
            intPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower intPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticIntPull", intPullPresentation.Build());

            FeatDefinitionBuilder intTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticInt", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(intPush, intPull, intIncrement); /* telekineticBonusCantrips, */

            feats.Add(intTelekineticFeat.AddToDB());

            // telekinetic cha
            GuiPresentationBuilder chaPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticChaPushTitle",
                "Feat/&FeatTelekineticChaPushDescription");
            chaPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower chaPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticChaPush", chaPushPresentation.Build());

            GuiPresentationBuilder chaPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticChaPullTitle",
                "Feat/&FeatTelekineticChaPullDescription");
            chaPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower chaPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticChaPull", chaPullPresentation.Build());

            FeatDefinitionBuilder chaTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticCha", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(chaPush, chaPull, chaIncrement); /* telekineticBonusCantrips, */

            feats.Add(chaTelekineticFeat.AddToDB());

            // telekinetic wis
            GuiPresentationBuilder wisPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticWisPushTitle",
                "Feat/&FeatTelekineticWisPushDescription");
            wisPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower wisPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticWisPush", wisPushPresentation.Build());

            GuiPresentationBuilder wisPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticWisPullTitle",
                "Feat/&FeatTelekineticWisPullDescription");
            wisPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower wisPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticWisPull", wisPullPresentation.Build());

            FeatDefinitionBuilder wisTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticWis", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(wisPush, wisPull, wisIncrement); /* telekineticBonusCantrips, */

            feats.Add(wisTelekineticFeat.AddToDB());

            // fey teleportation, misty step short rest, a language, an ability score
            // auto prepared spells: Misty Step
            // Power that mimics misty step once per short rest
            // DatabaseHelper.LanguageDefinitions.Language_Tirmarian
            // restrict to elf??
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup mistyStepGroup = AutoPreparedSpellsGroupBuilder.Build(0, MistyStep);

            CharacterClassDefinition[] classes = DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetAllElements();

            GuiPresentationBuilder learnMistyStepPresentation = new GuiPresentationBuilder(
                "Feat/&PowerMistyStepFromFeatTitle",
                "Feat/&PowerMistyStepFromFeatDescription");

            List<FeatureDefinition> mistyStepClassesPreparedSpells = AutoPreparedClassLists(classes,
                mistyStepGroup, learnMistyStepPresentation, "FeyTeleportationAutoPrepMisty", "FeyTeleport");

            GuiPresentationBuilder mistyStepBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerMistyStepFromFeatTitle",
                "Feat/&PowerMistyStepFromFeatDescription");
            mistyStepBonusPresentation.SetSpriteReference(MistyStep.GuiPresentation.SpriteReference);
            FeatureDefinitionPower mistyStepPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Intelligence,
                MistyStep.EffectDescription,
                "PowerMistyStepFromFeat", mistyStepBonusPresentation.Build());

            GuiPresentationBuilder feyTeleportationLanguagePresentation = new GuiPresentationBuilder(
                "Feat/&FeyTeleportationLanguageTirmarianTitle",
                "Feat/&FeyTeleportationLanguageTirmarianDescription");
            FeatureDefinitionProficiency feyTeleportationLanguage = BuildProficiency(RuleDefinitions.ProficiencyType.Language, new List<string>() { DatabaseHelper.LanguageDefinitions.Language_Tirmarian.Name },
                "FeyTeleportationLanguageTirmarian", feyTeleportationLanguagePresentation.Build());

            List<FeatureDefinition> feyFeatures = new List<FeatureDefinition>()
            {
                feyTeleportationLanguage,
                mistyStepPower,
            };
            feyFeatures.AddRange(mistyStepClassesPreparedSpells);

            feats.AddRange(
                // fey teleportation int
                FeatDefinitionBuilder
                    .Create("FeatFeyTeleportationInt", CasterFeatsNamespace)
                    .SetFeatures(intIncrement)
                    .AddFeatures(feyFeatures)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB(),
                // fey teleportation cha
                FeatDefinitionBuilder
                    .Create("FeatFeyTeleportationCha", CasterFeatsNamespace)
                    .SetFeatures(chaIncrement)
                    .AddFeatures(feyFeatures)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB(),
                // fey teleportation wis
                FeatDefinitionBuilder
                    .Create("FeatFeyTeleportationWis", CasterFeatsNamespace)
                    .SetFeatures(wisIncrement)
                    .AddFeatures(feyFeatures)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB()
            );

            // shadow touched: invisibility, false life, inflict wounds-- note inflict wounds is an attack that relies on casting stat, for the free cast power, tie it to the increment stat
            // auto prepared spells- see list ^
            // power that mimics ^^ spells once per long rest

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup shadowTouchedGroup =
                AutoPreparedSpellsGroupBuilder.Build(0, Invisibility, FalseLife, InflictWounds);

            GuiPresentationBuilder learnShadowTouchedPresentation = new GuiPresentationBuilder(
                "Feat/&PowerShadowTouchedFromFeatTitle",
                "Feat/&PowerShadowTouchedFromFeatDescription");

            List<FeatureDefinition> shadowTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
                shadowTouchedGroup, learnShadowTouchedPresentation, "ShadowTouchedAutoPrep", "ShadowTouched");

            GuiPresentationBuilder invisibilityBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerInvisibilityFromFeatTitle",
                "Feat/&PowerInvisibilityFromFeatDescription");
            invisibilityBonusPresentation.SetSpriteReference(Invisibility.GuiPresentation.SpriteReference);
            FeatureDefinitionPower invisibilityPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                Invisibility.EffectDescription, "PowerInvisibilityFromFeat", invisibilityBonusPresentation.Build());

            GuiPresentationBuilder falseLifeBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerFalseLifeFromFeatTitle",
                "Feat/&PowerFalseLifeFromFeatDescription");
            falseLifeBonusPresentation.SetSpriteReference(FalseLife.GuiPresentation.SpriteReference);
            FeatureDefinitionPower falseLifePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                FalseLife.EffectDescription, "PowerFalseLifeFromFeat", falseLifeBonusPresentation.Build());

            GuiPresentationBuilder inflictWoundsBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerInflictWoundsFromFeatTitle",
                "Feat/&PowerInflictWoundsFromFeatDescription");
            inflictWoundsBonusPresentation.SetSpriteReference(InflictWounds.GuiPresentation.SpriteReference);

            FeatureDefinitionPower inflictWoundsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Intelligence,
                InflictWounds.EffectDescription, "PowerInflictWoundsIntFromFeat", inflictWoundsBonusPresentation.Build());

            FeatureDefinitionPower inflictWoundsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Wisdom,
                InflictWounds.EffectDescription, "PowerInflictWoundsWisFromFeat", inflictWoundsBonusPresentation.Build());

            FeatureDefinitionPower inflictWoundsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Charisma,
                InflictWounds.EffectDescription, "PowerInflictWoundsChaFromFeat", inflictWoundsBonusPresentation.Build());

            feats.AddRange(
                // shadow touched int
                FeatDefinitionBuilder
                    .Create("FeatShadowTouchedInt", CasterFeatsNamespace)
                    .SetFeatures(invisibilityPower, inflictWoundsPowerInt, falseLifePower, intIncrement)
                    .AddFeatures(shadowTouchedClassesPreparedSpells)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB(),
                // shadow touched wis
                FeatDefinitionBuilder
                    .Create("FeatShadowTouchedWis", CasterFeatsNamespace)
                    .SetFeatures(invisibilityPower, inflictWoundsPowerWis, falseLifePower, wisIncrement)
                    .AddFeatures(shadowTouchedClassesPreparedSpells)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB(),
                // shadow touched cha
                FeatDefinitionBuilder
                    .Create("FeatShadowTouchedCha", CasterFeatsNamespace)
                    .SetFeatures(invisibilityPower, inflictWoundsPowerCha, falseLifePower, chaIncrement)
                    .AddFeatures(shadowTouchedClassesPreparedSpells)
                    .SetGuiPresentation(Category.Feat)
                    .AddToDB()
            );

            // fey touched? but it'd be 12 feats
            // fey touched: misty step, animal friendship, bane, bless, charm person, detect magic, hideous laughter, hunter's mark, identify, sleep, 

            // enchanter: charm person + sleep
            // divine: bane + bless
            // warrior: hunter's mark + hideous laughter
            // diviner: detect magic + identify
        }

        private static List<FeatureDefinition> AutoPreparedClassLists(IEnumerable<CharacterClassDefinition> classes, FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
            GuiPresentationBuilder learnShadowTouchedPresentation, string namePrefix, string autoPrepTag)
        {
            List<FeatureDefinition> autoPrepList = new List<FeatureDefinition>();
            foreach (CharacterClassDefinition klass in classes)
            {
                autoPrepList.Add(BuildAutoPreparedSpells(
                            new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() { spellGroup }, klass,
                            namePrefix + klass.Name, autoPrepTag, learnShadowTouchedPresentation.Build()));
            }
            return autoPrepList;
        }

        public static FeatureDefinitionPower BuildMotionFormPower(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType,
            RuleDefinitions.Side target, bool hasSavingThrow, bool disableSavingThrowOnAllies, string savingThrowAbility,
            RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation, string savingThrowDifficultyAbility,
            MotionForm.MotionType motionType, int motionDistance,
            int fixedSavingThrowDifficultyClass, string name, GuiPresentation guiPresentation)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(target, rangeType, rangeParameter, targetType, 1, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetSavingThrowData(hasSavingThrow, disableSavingThrowOnAllies, savingThrowAbility, true, difficultyClassComputation,
                savingThrowDifficultyAbility, fixedSavingThrowDifficultyClass, false, new List<SaveAffinityBySenseDescription>());
            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetMotionForm(motionType, motionDistance);
            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());

            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1, 0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return BuildPowerFromEffectDescription(usesPerRecharge, usesDetermination, activationTime, costPerUse, recharge,
                false, false, savingThrowDifficultyAbility, effectDescriptionBuilder.Build(), name, guiPresentation);
        }

        public static FeatureDefinitionPower BuildPowerFromEffectDescription(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(CasterFeatsNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, abilityScore, activationTime, costPerUse, recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack,
                abilityScore, effectDescription, guiPresentation, false /* unique */).AddToDB();
        }

        public static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells(
            IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, string name, string tag, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAutoPreparedSpellsBuilder(name, CasterFeatsNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetPreparedSpellGroups(autospelllists)
                .SetCharacterClass(characterclass)
                .SetAutoTag(tag)
                .AddToDB();
        }

        public static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(CasterFeatsNamespace, name).ToString(), type, proficiencies, guiPresentation).AddToDB();
        }
    }
}
