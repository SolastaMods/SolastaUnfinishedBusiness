using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
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
                return FeatureDefinitionAttributeModifierBuilder
                    .Create(name, CasterFeatsNamespace)
                    .SetGuiPresentation(Category.Feat)
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
            var intPushPresentation = GuiPresentationBuilder.Build("FeatTelekineticIntPush", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower intPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticIntPush", intPushPresentation);

            var intPullPresentation = GuiPresentationBuilder.Build("FeatTelekineticIntPull", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower intPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticIntPull", intPullPresentation);

            var intTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticInt", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(intPush, intPull, intIncrement) /* telekineticBonusCantrips, */
                .AddToDB();

            feats.Add(intTelekineticFeat);

            // telekinetic cha
            var chaPushPresentation = GuiPresentationBuilder.Build("FeatTelekineticChaPush", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower chaPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticChaPush", chaPushPresentation);

            var chaPullPresentation = GuiPresentationBuilder.Build("FeatTelekineticChaPull", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower chaPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticChaPull", chaPullPresentation);

            var chaTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticCha", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(chaPush, chaPull, chaIncrement) /* telekineticBonusCantrips, */
                .AddToDB();

            feats.Add(chaTelekineticFeat);

            // telekinetic wis
            var wisPushPresentation = GuiPresentationBuilder.Build("FeatTelekineticWisPush", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower wisPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticWisPush", wisPushPresentation);

            var wisPullPresentation = GuiPresentationBuilder.Build("FeatTelekineticWisPull", Category.Feat, PowerVampiricTouch.GuiPresentation.SpriteReference);

            FeatureDefinitionPower wisPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticWisPull", wisPullPresentation);

            var wisTelekineticFeat = FeatDefinitionBuilder
                .Create("FeatTelekineticWis", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(wisPush, wisPull, wisIncrement) /* telekineticBonusCantrips, */
                .AddToDB();

            feats.Add(wisTelekineticFeat);

            // fey teleportation, misty step short rest, a language, an ability score
            // auto prepared spells: Misty Step
            // Power that mimics misty step once per short rest
            // DatabaseHelper.LanguageDefinitions.Language_Tirmarian
            // restrict to elf??
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup mistyStepGroup = AutoPreparedSpellsGroupBuilder.Build(0, MistyStep);

            CharacterClassDefinition[] classes = DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetAllElements();

            var learnMistyStepPresentation = GuiPresentationBuilder.Build("PowerMistyStepFromFeat", Category.Feat);

            List<FeatureDefinition> mistyStepClassesPreparedSpells = AutoPreparedClassLists(classes,
                mistyStepGroup, learnMistyStepPresentation, "FeyTeleportationAutoPrepMisty", "FeyTeleport");

            var mistyStepBonusPresentation = GuiPresentationBuilder.Build(
                "PowerMistyStepFromFeat", Category.Feat, MistyStep.GuiPresentation.SpriteReference);
            FeatureDefinitionPower mistyStepPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Intelligence,
                MistyStep.EffectDescription,
                "PowerMistyStepFromFeat", mistyStepBonusPresentation);

            FeatureDefinitionProficiency feyTeleportationLanguage = FeatureDefinitionProficiencyBuilder
                .Create("FeyTeleportationLanguageTirmarian", CasterFeatsNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Language, DatabaseHelper.LanguageDefinitions.Language_Tirmarian.Name)
                .AddToDB();

            var feyFeatures = new List<FeatureDefinition>()
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

            var learnShadowTouchedPresentation = GuiPresentationBuilder.Build("PowerShadowTouchedFromFeat", Category.Feat);

            List<FeatureDefinition> shadowTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
                shadowTouchedGroup, learnShadowTouchedPresentation, "ShadowTouchedAutoPrep", "ShadowTouched");

            var invisibilityBonusPresentation = GuiPresentationBuilder.Build("PowerInvisibilityFromFeat", Category.Feat, Invisibility.GuiPresentation.SpriteReference);

            FeatureDefinitionPower invisibilityPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                Invisibility.EffectDescription, "PowerInvisibilityFromFeat", invisibilityBonusPresentation);

            var falseLifeBonusPresentation = GuiPresentationBuilder.Build("PowerFalseLifeFromFeat", Category.Feat, FalseLife.GuiPresentation.SpriteReference);

            FeatureDefinitionPower falseLifePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                FalseLife.EffectDescription, "PowerFalseLifeFromFeat", falseLifeBonusPresentation);

            var inflictWoundsBonusPresentation = GuiPresentationBuilder.Build("PowerInflictWoundsFromFeat", Category.Feat, InflictWounds.GuiPresentation.SpriteReference);

            FeatureDefinitionPower inflictWoundsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Intelligence,
                InflictWounds.EffectDescription, "PowerInflictWoundsIntFromFeat", inflictWoundsBonusPresentation);

            FeatureDefinitionPower inflictWoundsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Wisdom,
                InflictWounds.EffectDescription, "PowerInflictWoundsWisFromFeat", inflictWoundsBonusPresentation);

            FeatureDefinitionPower inflictWoundsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Charisma,
                InflictWounds.EffectDescription, "PowerInflictWoundsChaFromFeat", inflictWoundsBonusPresentation);

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
            GuiPresentation learnShadowTouchedPresentation, string namePrefix, string autoPrepTag)
        {
            List<FeatureDefinition> autoPrepList = new List<FeatureDefinition>();
            foreach (CharacterClassDefinition klass in classes)
            {
                autoPrepList.Add(BuildAutoPreparedSpells(
                            new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() { spellGroup }, klass,
                            namePrefix + klass.Name, autoPrepTag, learnShadowTouchedPresentation));
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
            effectDescriptionBuilder.SetSavingThrowData(
                hasSavingThrow, disableSavingThrowOnAllies, savingThrowAbility, true, difficultyClassComputation,
                savingThrowDifficultyAbility, fixedSavingThrowDifficultyClass, false);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetMotionForm(motionType, motionDistance);
            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());

            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1, 0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return BuildPowerFromEffectDescription(usesPerRecharge, usesDetermination, activationTime, costPerUse, recharge,
                false, false, savingThrowDifficultyAbility, effectDescriptionBuilder.Build(), name, guiPresentation);
        }

        private static FeatureDefinitionPower BuildPowerFromEffectDescription(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionPowerBuilder
                .Create(name, CasterFeatsNamespace)
                .SetGuiPresentation(guiPresentation)
                .Configure(usesPerRecharge, usesDetermination, abilityScore, activationTime, costPerUse, recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription, false /* unique */)
                .AddToDB();
        }

        private static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells(
            IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, string name, string tag, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionAutoPreparedSpellsBuilder
                .Create(name, CasterFeatsNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetPreparedSpellGroups(autospelllists)
                .SetCharacterClass(characterclass)
                .SetAutoTag(tag)
                .AddToDB();
        }
    }
}
