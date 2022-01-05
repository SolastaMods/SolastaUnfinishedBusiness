using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Feats
{
    internal static class CasterFeats
    {
        public static readonly Guid CasterFeatsNamespace = new Guid("bf70984d-e7b9-446a-9ae3-0f2039de833d");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            GuiPresentationBuilder intPresentation = new GuiPresentationBuilder(
                "Feat/&FeatIntIncrementDescription", "Feat/&FeatIntIncrementTitle");
            FeatureDefinition intIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Intelligence, 1, "FeatIntIncrement", intPresentation.Build());

            GuiPresentationBuilder chaPresentation = new GuiPresentationBuilder(
                "Feat/&FeatIntIncrementDescription",
                "Feat/&FeatIntIncrementTitle");
            FeatureDefinition chaIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Charisma, 1, "FeatChaIncrement", chaPresentation.Build());

            GuiPresentationBuilder wisPresentation = new GuiPresentationBuilder(
                "Feat/&FeatIntIncrementDescription",
                "Feat/&FeatIntIncrementTitle");
            FeatureDefinition wisIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Wisdom, 1, "FeatWisIncrement", wisPresentation.Build());

            // Note it seems that feats can't currently grant bonus cantrips (which is kind of fine since the game doesn't have mage hand).
            //GuiPresentationBuilder telekineticBonusCantripPresentation = new GuiPresentationBuilder(
            //    "Feat/&FeatTelekineticBonusCantripDescription", ""),
            //    "Feat/&FeatTelkineticBonusCantripTitle", ""));
            //FeatureDefinitionBonusCantrips telekineticBonusCantrips = FeatureBuilder.BuildBonusCantrips(new List<SpellDefinition>()
            //{
            //    DatabaseHelper.SpellDefinitions.AnnoyingBee,
            //    DatabaseHelper.SpellDefinitions.Sparkle,
            //    DatabaseHelper.SpellDefinitions.Dazzle,
            //}, "FeatTelekineticBonusCantrip", telekineticBonusCantripPresentation.Build());

            // telekinetic int
            GuiPresentationBuilder intPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticIntPushDescription",
                "Feat/&FeatTelkineticIntPushTitle");
            intPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower intPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticIntPush", intPushPresentation.Build());

            GuiPresentationBuilder intPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticIntPullDescription",
                "Feat/&FeatTelkineticIntPullTitle");
            intPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower intPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticIntPull", intPullPresentation.Build());

            GuiPresentationBuilder telekineticIntPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticIntDescription",
                "Feat/&FeatTelkineticIntTitle");

            FeatDefinitionBuilder intTelekineticFeat = new FeatDefinitionBuilder("FeatTelekineticInt", GuidHelper.Create(CasterFeatsNamespace, "FeatTelekineticInt").ToString(),
                new List<FeatureDefinition>()
            {
                intPush,
                intPull,
                //telekineticBonusCantrips,
                intIncrement,
            }, telekineticIntPresentation.Build());
            feats.Add(intTelekineticFeat.AddToDB());

            // telekinetic cha
            GuiPresentationBuilder chaPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticChaPushDescription",
                "Feat/&FeatTelkineticChaPushTitle");
            chaPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower chaPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticChaPush", chaPushPresentation.Build());

            GuiPresentationBuilder chaPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticChaPullDescription",
                "Feat/&FeatTelkineticChaPullTitle");
            chaPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower chaPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticChaPull", chaPullPresentation.Build());

            GuiPresentationBuilder telekineticChaPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticChaDescription",
                "Feat/&FeatTelkineticChaTitle");
            FeatDefinitionBuilder chaTelekineticFeat = new FeatDefinitionBuilder("FeatTelekineticCha", GuidHelper.Create(CasterFeatsNamespace, "FeatTelekineticCha").ToString(),
                new List<FeatureDefinition>()
            {
                chaPush,
                chaPull,
                //telekineticBonusCantrips,
                chaIncrement,
            }, telekineticChaPresentation.Build());
            feats.Add(chaTelekineticFeat.AddToDB());

            // telekinetic wis
            GuiPresentationBuilder wisPushPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticWisPushDescription",
                "Feat/&FeatTelkineticWisPushTitle");
            wisPushPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower wisPush = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin, 1, 10, "PowerTelekineticWisPush", wisPushPresentation.Build());

            GuiPresentationBuilder wisPullPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticWisPullDescription",
                "Feat/&FeatTelkineticWisPullTitle");
            wisPullPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.GuiPresentation.SpriteReference);
            FeatureDefinitionPower wisPull = BuildMotionFormPower(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
                RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength, RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin, 1, 10, "PowerTelekineticWisPull", wisPullPresentation.Build());

            GuiPresentationBuilder telekineticWisPresentation = new GuiPresentationBuilder(
                "Feat/&FeatTelekineticWisDescription",
                "Feat/&FeatTelkineticWisTitle");
            FeatDefinitionBuilder wisTelekineticFeat = new FeatDefinitionBuilder("FeatTelekineticWis", GuidHelper.Create(CasterFeatsNamespace, "FeatTelekineticWis").ToString(),
                new List<FeatureDefinition>()
            {
                wisPush,
                wisPull,
                //telekineticBonusCantrips,
                wisIncrement,
            }, telekineticWisPresentation.Build());
            feats.Add(wisTelekineticFeat.AddToDB());

            // fey teleportation, misty step short rest, a language, an ability score
            // auto prepared spells: Misty Step
            // Power that mimics misty step once per short rest
            // DatabaseHelper.LanguageDefinitions.Language_Tirmarian
            // restrict to elf??
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup mistyStepGroup = BuildAutoPreparedSpellGroup(0,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.MistyStep });

            CharacterClassDefinition[] classes = DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetAllElements();

            GuiPresentationBuilder learnMistyStepPresentation = new GuiPresentationBuilder(
                "Feat/&PowerMistyStepFromFeatDescription",
                "Feat/&PowerMistyStepFromFeatTitle");

            List<FeatureDefinition> mistyStepClassesPreparedSpells = AutoPreparedClassLists(classes,
                mistyStepGroup, learnMistyStepPresentation, "FeyTeleportationAutoPrepMisty", "FeyTeleport");

            GuiPresentationBuilder mistyStepBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerMistyStepFromFeatDescription",
                "Feat/&PowerMistyStepFromFeatTitle");
            mistyStepBonusPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MistyStep.GuiPresentation.SpriteReference);
            FeatureDefinitionPower mistyStepPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.MistyStep.EffectDescription,
                "PowerMistyStepFromFeat", mistyStepBonusPresentation.Build());

            GuiPresentationBuilder feyTeleportationLanguagePresentation = new GuiPresentationBuilder(
                "Feat/&FeyTeleportationLanguageTirmarianDescription",
                "Feat/&FeyTeleportationLanguageTirmarianTitle");
            FeatureDefinitionProficiency feyTeleportationLanguage = BuildProficiency(RuleDefinitions.ProficiencyType.Language, new List<string>() { DatabaseHelper.LanguageDefinitions.Language_Tirmarian.Name },
                "FeyTeleportationLanguageTirmarian", feyTeleportationLanguagePresentation.Build());

            List<FeatureDefinition> feyFeatures = new List<FeatureDefinition>()
            {
                feyTeleportationLanguage,
                mistyStepPower,
            };
            feyFeatures.AddRange(mistyStepClassesPreparedSpells);

            // fey teleportation int
            GuiPresentationBuilder intFeyTeleportationPresentation = new GuiPresentationBuilder(
                "Feat/&FeatFeyTeleportationIntDescription",
                "Feat/&FeatFeyTeleportationIntTitle");
            List<FeatureDefinition> feyIntFeatures = new List<FeatureDefinition>()
            {
                intIncrement,
            };
            feyIntFeatures.AddRange(feyFeatures);
            FeatDefinitionBuilder intFeyTeleportation = new FeatDefinitionBuilder("FeatFeyTeleportationInt", GuidHelper.Create(CasterFeatsNamespace, "FeatFeyTeleportationInt").ToString(),
               feyIntFeatures, intFeyTeleportationPresentation.Build());
            feats.Add(intFeyTeleportation.AddToDB());

            // fey teleportation cha
            GuiPresentationBuilder chaFeyTeleportationPresentation = new GuiPresentationBuilder(
                "Feat/&FeatFeyTeleportationChaDescription",
                "Feat/&FeatFeyTeleportationChaTitle");
            List<FeatureDefinition> feyChaFeatures = new List<FeatureDefinition>()
            {
                chaIncrement,
            };
            feyChaFeatures.AddRange(feyFeatures);
            FeatDefinitionBuilder chaFeyTeleportation = new FeatDefinitionBuilder("FeatFeyTeleportationCha", GuidHelper.Create(CasterFeatsNamespace, "FeatFeyTeleportationCha").ToString(),
                feyChaFeatures, chaFeyTeleportationPresentation.Build());
            feats.Add(chaFeyTeleportation.AddToDB());

            GuiPresentationBuilder wisFeyTeleportationPresentation = new GuiPresentationBuilder(
                "Feat/&FeatFeyTeleportationWisDescription",
                "Feat/&FeatFeyTeleportationWisTitle");
            List<FeatureDefinition> feyWisFeatures = new List<FeatureDefinition>()
            {
                wisIncrement,
            };
            feyWisFeatures.AddRange(feyFeatures);
            FeatDefinitionBuilder wisFeyTeleportation = new FeatDefinitionBuilder("FeatFeyTeleportationWis", GuidHelper.Create(CasterFeatsNamespace, "FeatFeyTeleportationWis").ToString(),
                feyWisFeatures, wisFeyTeleportationPresentation.Build());
            feats.Add(wisFeyTeleportation.AddToDB());

            // shadow touched: invisibility, false life, inflict wounds-- note inflict wounds is an attack that relies on casting stat, for the free cast power, tie it to the increment stat
            // auto prepared spells- see list ^
            // power that mimics ^^ spells once per long rest

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup shadowTouchedGroup = BuildAutoPreparedSpellGroup(0,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Invisibility, DatabaseHelper.SpellDefinitions.FalseLife, DatabaseHelper.SpellDefinitions.InflictWounds });

            GuiPresentationBuilder learnShadowTouchedPresentation = new GuiPresentationBuilder(
                "Feat/&PowerShadowTouchedFromFeatDescription",
                "Feat/&PowerShadowTouchedFromFeatTitle");

            List<FeatureDefinition> shadowTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
                shadowTouchedGroup, learnShadowTouchedPresentation, "ShadowTouchedAutoPrep", "ShadowTouched");

            GuiPresentationBuilder invisibilityBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerInvisibilityFromFeatDescription",
                "Feat/&PowerInvisibilityFromFeatTitle");
            invisibilityBonusPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Invisibility.GuiPresentation.SpriteReference);
            FeatureDefinitionPower invisibilityPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.Invisibility.EffectDescription, "PowerInvisibilityFromFeat", invisibilityBonusPresentation.Build());

            GuiPresentationBuilder falseLifeBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerFalseLifeFromFeatDescription",
                "Feat/&PowerFalseLifeFromFeatTitle");
            falseLifeBonusPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);
            FeatureDefinitionPower falseLifePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription, "PowerFalseLifeFromFeat", falseLifeBonusPresentation.Build());

            GuiPresentationBuilder inflictWoundsBonusPresentation = new GuiPresentationBuilder(
                "Feat/&PowerInflictWoundsFromFeatDescription",
                "Feat/&PowerInflictWoundsFromFeatTitle");
            inflictWoundsBonusPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.InflictWounds.GuiPresentation.SpriteReference);

            FeatureDefinitionPower inflictWoundsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.InflictWounds.EffectDescription, "PowerInflictWoundsIntFromFeat", inflictWoundsBonusPresentation.Build());

            FeatureDefinitionPower inflictWoundsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Wisdom,
                DatabaseHelper.SpellDefinitions.InflictWounds.EffectDescription, "PowerInflictWoundsWisFromFeat", inflictWoundsBonusPresentation.Build());

            FeatureDefinitionPower inflictWoundsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                true, true, AttributeDefinitions.Charisma,
                DatabaseHelper.SpellDefinitions.InflictWounds.EffectDescription, "PowerInflictWoundsChaFromFeat", inflictWoundsBonusPresentation.Build());

            // shadow touched int
            GuiPresentationBuilder intShadowTouchedPresentation = new GuiPresentationBuilder(
                "Feat/&FeatShadowTouchedIntDescription",
                "Feat/&FeatShadowTouchedIntTitle");
            List<FeatureDefinition> shadowIntFeatures = new List<FeatureDefinition>()
            {
                invisibilityPower,
                inflictWoundsPowerInt,
                falseLifePower,
                intIncrement,
            };
            shadowIntFeatures.AddRange(shadowTouchedClassesPreparedSpells);
            FeatDefinitionBuilder intShadowTouched = new FeatDefinitionBuilder("FeatShadowTouchedInt", GuidHelper.Create(CasterFeatsNamespace, "FeatShadowTouchedInt").ToString(),
                shadowIntFeatures, intShadowTouchedPresentation.Build());
            feats.Add(intShadowTouched.AddToDB());
            // shadow touched wis
            GuiPresentationBuilder wisShadowTouchedPresentation = new GuiPresentationBuilder(
                "Feat/&FeatShadowTouchedWisDescription",
                "Feat/&FeatShadowTouchedWisTitle");
            List<FeatureDefinition> shadowWisFeatures = new List<FeatureDefinition>()
            {
                invisibilityPower,
                inflictWoundsPowerWis,
                falseLifePower,
                wisIncrement,
            };
            shadowWisFeatures.AddRange(shadowTouchedClassesPreparedSpells);
            FeatDefinitionBuilder wisShadowTouched = new FeatDefinitionBuilder("FeatShadowTouchedWis", GuidHelper.Create(CasterFeatsNamespace, "FeatShadowTouchedWis").ToString(),
              shadowWisFeatures, wisShadowTouchedPresentation.Build());
            feats.Add(wisShadowTouched.AddToDB());
            // shadow touched cha
            GuiPresentationBuilder chaShadowTouchedPresentation = new GuiPresentationBuilder(
                "Feat/&FeatShadowTouchedChaDescription",
                "Feat/&FeatShadowTouchedChaTitle");
            List<FeatureDefinition> shadowChaFeatures = new List<FeatureDefinition>()
            {
                invisibilityPower,
                inflictWoundsPowerCha,
                falseLifePower,
                chaIncrement,
            };
            shadowChaFeatures.AddRange(shadowTouchedClassesPreparedSpells);
            FeatDefinitionBuilder chaShadowTouched = new FeatDefinitionBuilder("FeatShadowTouchedCha", GuidHelper.Create(CasterFeatsNamespace, "FeatShadowTouchedCha").ToString(),
                shadowChaFeatures, chaShadowTouchedPresentation.Build());
            feats.Add(chaShadowTouched.AddToDB());

            // fey touched? but it'd be 12 feats
            // fey touched: misty step, animal friendship, bane, bless, charm person, detect magic, hideous laughter, hunter's mark, identify, sleep, 

            // enchanter: charm person + sleep
            // divine: bane + bless
            // warrior: hunter's mark + hideous laughter
            // diviner: detect magic + identify
        }

        private static List<FeatureDefinition> AutoPreparedClassLists(CharacterClassDefinition[] classes, FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
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

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(CasterFeatsNamespace, name).ToString(), modifierType, attribute, amount, guiPresentation).AddToDB();
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

        public static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells(List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, string name, string tag, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAutoPreparedSpellsBuilder(name, GuidHelper.Create(CasterFeatsNamespace, name).ToString(),
                autospelllists, guiPresentation).SetCharacterClass(characterclass).SetAutoTag(tag).AddToDB();
        }

        public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(int classLevel, List<SpellDefinition> spellnames)
        {
            return new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = classLevel,
                SpellsList = spellnames.ToList()
            };
        }

        public static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(CasterFeatsNamespace, name).ToString(), type, proficiencies, guiPresentation).AddToDB();
        }
    }
}
