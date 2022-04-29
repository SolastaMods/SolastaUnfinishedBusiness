using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warlock.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    public static class EldritchInvocationsBuilder
    {
        public static SpellDefinition EldritchBlast { get; set; }

        public static Dictionary<string, FeatureDefinitionFeatureSetWithPreRequisites> EldritchInvocationsAttributeModifiers { get; private set; } = new();
        public static Dictionary<string, FeatureDefinition> EldritchInvocationsBlasts { get; private set; } = new();
        public static Dictionary<string, FeatureDefinitionPower> EldritchInvocationsPowers { get; private set; } = new();

        public static void Build()
        {
            BuildEldritchBlastAndInvocations();
            BuildEldritchInvocationsSpellsToCantrips();
            BuildEldritchInvocationsAttributeModifiers();
        }

        private const string EldritchBlastName = "DHEldritchBlast";

        private static void BuildEldritchBlastAndInvocations()
        {
            var eldritchBlastGui = new GuiPresentationBuilder(
                    "Spell/&EldritchBlastTitle",
                    "Spell/&EldritchBlastDescription",
                    SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference)
                .Build();

            var blastDamage = new EffectFormBuilder().SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D10,
                    RuleDefinitions.DamageTypeForce,
                    0,
                    RuleDefinitions.DieType.D10,
                    1,
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>())
                .Build();

            EffectDescription EldritchBlastEffect = new EffectDescriptionBuilder()
            .AddEffectForm(blastDamage)
            .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.RangeHit,
                    24,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
            .SetEffectAdvancement(
                RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                5,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                RuleDefinitions.AdvancementDuration.None
                )
            .SetParticleEffectParameters(SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters)
            .Build();

            var agonizingForm = new EffectFormBuilder(blastDamage)
                .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                .Build();

            var hinderingForm = new EffectFormBuilder()
                .SetConditionForm(
                    ConditionDefinitions.ConditionHindered_By_Frost,
                    ConditionForm.ConditionOperation.Add
                )
                .Build();

            var agonizingBlastEffect = new EffectDescriptionBuilder(EldritchBlastEffect)
                .SetEffectForms(agonizingForm)
                .Build();

            var hinderingBlastEffect = new EffectDescriptionBuilder(EldritchBlastEffect)
                .AddEffectForm(hinderingForm)
                .Build();

            var hinderingAgonizingBlastEffect = new EffectDescriptionBuilder(EldritchBlastEffect)
                .SetEffectForms(agonizingForm, hinderingForm)
                .Build();

            var agonizingBlastFeature = FeatureDefinitionBuilder
                .Create("AdditionalDamageAgonizingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            var hinderingBlastFeature = FeatureDefinitionBuilder
                .Create("AdditionalDamageHinderingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            var eldritchBlast = SpellWithCasterFeatureDependentEffectsBuilder
                .Create(EldritchBlastName, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(eldritchBlastGui)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(EldritchBlastEffect)
                .SetFeatureEffects(
                    (new List<FeatureDefinition> { agonizingBlastFeature, hinderingBlastFeature }, hinderingAgonizingBlastEffect),
                    (new List<FeatureDefinition> { agonizingBlastFeature }, agonizingBlastEffect),
                    (new List<FeatureDefinition> { hinderingBlastFeature }, hinderingBlastEffect)
                )
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
            EldritchBlast = eldritchBlast;

            var pushForm = new EffectFormBuilder()
                .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                .Build();
            var pullForm = new EffectFormBuilder()
                .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                .Build();

            void MakeEldritchBlastVariant(string name, params EffectForm[] forms)
            {
                string cantripName = EldritchBlastName + name;

                EffectDescription effect = new EffectDescription();
                effect.Copy(EldritchBlastEffect);

                var cantrip = SpellWithCasterFeatureDependentEffectsBuilder
                    .Create(eldritchBlast, cantripName, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Spell, eldritchBlastGui.SpriteReference)
                    .SetEffectDescription(CustomFeaturesContext.AddEffectForms(EldritchBlastEffect, forms))
                    .SetFeatureEffects(eldritchBlast.FeaturesEffectList
                        .Select(t => (t.Item1, CustomFeaturesContext.AddEffectForms(t.Item2, forms)))
                        .ToArray()
                    )
                    .AddToDB();

                var bonusCantrip = FeatureDefinitionFreeBonusCantripsWithPrerequisitesBuilder
                    .Create(cantripName + "BonusCantrip", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .ClearBonusCantrips()
                    .AddBonusCantrip(cantrip)
                    .SetValidators(() => Global.ActiveLevelUpHeroHasCantrip(EldritchBlast))
                    .AddToDB();

                EldritchInvocationsBlasts.Add(name, bonusCantrip);
            }

            MakeEldritchBlastVariant("RepellingBlast", pushForm);
            MakeEldritchBlastVariant("GraspingHand", pullForm);

            var agonizingBlast = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                .Create("AgonizingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(agonizingBlastFeature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .SetValidators(() => Global.ActiveLevelUpHeroHasCantrip(eldritchBlast))
                .AddToDB();

            EldritchInvocationsBlasts.Add(agonizingBlast.Name, agonizingBlast);

            var hinderingBlast = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                .Create("HinderingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(hinderingBlastFeature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .SetValidators(() => Global.ActiveLevelUpHeroHasCantrip(eldritchBlast))
                .AddToDB();

            EldritchInvocationsBlasts.Add(hinderingBlast.Name, hinderingBlast);
        }

        private static void BuildEldritchInvocationsSpellsToCantrips()
        {
            Dictionary<string, SpellDefinition> dictionaryofEIPseudoCantrips = new();
            Dictionary<string, SpellDefinition> dictionaryofEIPseudoSpells = new();

            //at will
            dictionaryofEIPseudoCantrips.Add("ArmorofShadows", SpellDefinitions.MageArmor);// self
            dictionaryofEIPseudoCantrips.Add("EldritchSight", SpellDefinitions.DetectMagic);
            dictionaryofEIPseudoCantrips.Add("FiendishVigor", SpellDefinitions.FalseLife);// self
            dictionaryofEIPseudoCantrips.Add("AscendantStep", SpellDefinitions.Levitate);// self
            dictionaryofEIPseudoCantrips.Add("OtherworldlyLeap", SpellDefinitions.Jump);// self
            dictionaryofEIPseudoCantrips.Add("ChainsofCarceri", SpellDefinitions.HoldMonster);
            dictionaryofEIPseudoCantrips.Add("ShroudofShadow", SpellDefinitions.Invisibility);
            // 1/day
            dictionaryofEIPseudoSpells.Add("ThiefofFiveFates", SpellDefinitions.Bane);
            dictionaryofEIPseudoSpells.Add("MiretheMind", SpellDefinitions.Slow);
            //cant do it this way as it uses subspells  dictionaryofEIPseudoSpells.Add("SignofIllOmen", SpellDefinitions.BestowCurse);
            dictionaryofEIPseudoSpells.Add("DreadfulWord", SpellDefinitions.Confusion);
            dictionaryofEIPseudoSpells.Add("Trickster'sEscape", SpellDefinitions.FreedomOfMovement);// self

            // EI that arent valid for game right now
            //{"BeastSpeech",         SpellDefinitions.},
            //{"BookofAncientSecrets",SpellDefinitions.},
            //{"MaskofManyFaces",     SpellDefinitions.},
            //{"MistyVisions",        SpellDefinitions.},
            //{"FarScribe",           SpellDefinitions.},
            //{"GiftoftheDepths",     SpellDefinitions.},
            //{"UndyingServitude",    SpellDefinitions.},
            //{"BewitchingWhispers",  SpellDefinitions.},
            //{"SculptorofFlesh",     SpellDefinitions.},
            //{"WhispersoftheGrave",  SpellDefinitions.},
            //{"MasterofMyriadForms", SpellDefinitions.},
            // {"VisionsofDistantRealms",SpellDefinitions.},
            // cant convert spellsbundle to power
            //  dictionaryofEIPseudoSpells.Add("MinionsofChaos", SpellDefinitions.ConjureElemental);

            // at will EI
            foreach (KeyValuePair<string, SpellDefinition> entry in dictionaryofEIPseudoCantrips)
            {
                string textPseudoCantrips = "DHEldritchInvocation" + entry.Value.name;

                var guiPresentationEIPseudoCantrips = new GuiPresentationBuilder(
                    "Feature/&" + entry.Key + "Title",
                    Gui.Format("Feature/&SpellAsInvocationAtWillDescription", entry.Value.FormatTitle()))
                        .SetSpriteReference(entry.Value.GuiPresentation.SpriteReference)
                        .Build();

                var EIPowerBuilder = FeatureDefinitionPowerBuilder
                    .Create(textPseudoCantrips, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiPresentationEIPseudoCantrips)
                    .Configure(
                     1,
                     RuleDefinitions.UsesDetermination.Fixed,
                     AttributeDefinitions.Charisma,
                     entry.Value.ActivationTime,
                     1,
                     RuleDefinitions.RechargeRate.AtWill,
                     false,
                     false,
                     AttributeDefinitions.Charisma,
                     entry.Value.EffectDescription.DeepCopy(), // need to copy to avoid issues with the source spells
                     true);
                FeatureDefinitionPower EIPower = EIPowerBuilder.AddToDB();

                if (entry.Key == "ArmorofShadows" || entry.Key == "FiendishVigor" || entry.Key == "AscendantStep" || entry.Key == "OtherworldlyLeap")
                {
                    EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
                }

                EldritchInvocationsPowers.Add(entry.Key, EIPower);
            }

            // 1/day EI
            foreach (KeyValuePair<string, SpellDefinition> entry in dictionaryofEIPseudoSpells)
            {
                string textPseudoSpells = "DHEldritchInvocation" + entry.Value.name;

                var guiPresentationEIPseudoSpells = new GuiPresentationBuilder(
                    "Feature/&" + entry.Key + "Title",
                    Gui.Format("Feature/&SpellAsInvocationOncePerDayDescription", entry.Value.FormatTitle()))
                        .SetSpriteReference(entry.Value.GuiPresentation.SpriteReference)
                        .Build();

                var EIPowerBuilder = FeatureDefinitionPowerBuilder
                    .Create(textPseudoSpells, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiPresentationEIPseudoSpells)
                    .Configure(
                     1,
                     RuleDefinitions.UsesDetermination.Fixed,
                     AttributeDefinitions.Charisma,
                     entry.Value.ActivationTime,
                     1,
                     RuleDefinitions.RechargeRate.LongRest,
                     false,
                     false,
                     AttributeDefinitions.Charisma,
                     entry.Value.EffectDescription.DeepCopy(), // need to copy to avoid issues with the source spells
                     true);
                FeatureDefinitionPower EIPower = EIPowerBuilder.AddToDB();

                if (entry.Key == "Trickster'sEscape")
                {
                    EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
                }

                EldritchInvocationsPowers.Add(entry.Key, EIPower);
            }
        }

        private static void BuildEldritchInvocationsAttributeModifiers()
        {
            var listofEIAttributeModifiers = new List<String>
                {
                    "AspectoftheMoon",               // FeatureDefinitionCampAffinitys.CampAffinityElfTrance,FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
                    "BeguilingInfluence",            // FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
                    "EldritchMind",                  // FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
                    "EyesoftheRuneKeeper",           // FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
                    "GiftoftheEver-LivingOnes",      // FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
                    "ImprovedPactWeapon",            // AttackModifierMagicWeapon + MagicAffinitySpellBladeIntoTheFray
                    "EldritchSmite",                 // FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
                    "ThirstingBlade",                // FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
                    "GiftoftheProtectors",           // FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
                    "BondoftheTalisman",             // FeatureDefinitionPowers.PowerSorakShadowEscape);
                    "WitchSight",                    // FeatureDefinitionSenses.SenseSeeInvisible12;
                    "OneWithShadows",
                    "OneWithShadowsStronger",
                    //"Lifedrinker",                 // similar to AdditionalDamageDomainOblivionStrikeOblivion +damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
                    "DevilsSight",                   // FeatureDefinitionSenses.SenseDarkvision24 or maybe similar to ConditionAffinityVeilImmunity needs to cover multiple darkness conditions ConditionDarkness ConditionVeil
                    //"GazeofTwoMinds",              //
                    //"InvestmentoftheChainMaster",  // multiple features through summoning affinity, could just reuse SummoningAffinityKindredSpiritBond for a similar but not direct copy of non srd EI
                    //"RebukeoftheTalisman",         //
                    //"VoiceoftheChainMaster",       //
                    //"CloakofFlies",                //
                    //"MaddeningHex",                //
                    //"TombofLevistus",              //
                    //"GhostlyGaze",                 //
                    //"ProtectionoftheTalisman",     //
                    //"RelentlessHex",               //
                };

            foreach (string entry in listofEIAttributeModifiers)
            {
                List<string> ListofEIAttributeModifiers = new();

                var textEIAttributeModifiers = "DHEIAttributeModifiers" + entry;

                var FeatureSetEldritchInvocations = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                    .Create(textEIAttributeModifiers, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .ClearFeatureSet()
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(false)
                    .AddToDB();

                EldritchInvocationsAttributeModifiers.Add(entry, FeatureSetEldritchInvocations);
            }

            EldritchInvocationsAttributeModifiers["AspectoftheMoon"].FeatureSet.Add(FeatureDefinitionCampAffinitys.CampAffinityElfTrance);
            EldritchInvocationsAttributeModifiers["AspectoftheMoon"].FeatureSet.Add(FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
            EldritchInvocationsAttributeModifiers["BeguilingInfluence"].FeatureSet.Add(FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
            EldritchInvocationsAttributeModifiers["EldritchMind"].FeatureSet.Add(FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
            EldritchInvocationsAttributeModifiers["EyesoftheRuneKeeper"].FeatureSet.Add(FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
            EldritchInvocationsAttributeModifiers["GiftoftheEver-LivingOnes"].FeatureSet.Add(FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
            EldritchInvocationsAttributeModifiers["ImprovedPactWeapon"].FeatureSet.Add(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon);
            EldritchInvocationsAttributeModifiers["ImprovedPactWeapon"].FeatureSet.Add(FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray);
            EldritchInvocationsAttributeModifiers["EldritchSmite"].FeatureSet.Add(FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
            EldritchInvocationsAttributeModifiers["ThirstingBlade"].FeatureSet.Add(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
            EldritchInvocationsAttributeModifiers["GiftoftheProtectors"].FeatureSet.Add(FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
            EldritchInvocationsAttributeModifiers["BondoftheTalisman"].FeatureSet.Add(FeatureDefinitionPowers.PowerSorakShadowEscape);
            EldritchInvocationsAttributeModifiers["WitchSight"].FeatureSet.Add(FeatureDefinitionSenses.SenseSeeInvisible12);
         
            var shadowsInvisibiityConditionDefinition = ConditionDefinitionBuilder
                .Create( "WarlockConditionShadowsSpecial", DefinitionBuilder.CENamespaceGuid)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetGuiPresentationNoContent()
                .SetFeatures(DHWarlockSubclassMoonLitPatron.InvisibilityFeature)
                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .AddToDB();

            var Unlit = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = shadowsInvisibiityConditionDefinition
            };

            var Dim = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim,
                condition = shadowsInvisibiityConditionDefinition
            };

            var Darkness = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness,
                condition = shadowsInvisibiityConditionDefinition
            };

            FeatureDefinitionLightAffinity OneWithShadowsLightAffinity = FeatureDefinitionLightAffinityBuilder
                .Create("OneWithShadowsLightAffinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddLightingEffectAndCondition(Unlit)
                .AddToDB();
            
            FeatureDefinitionLightAffinity OneWithShadowsLightAffinityStrong = FeatureDefinitionLightAffinityBuilder
                .Create("OneWithShadowsLightAffinityStrong", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddLightingEffectAndCondition(Dim)
                .AddLightingEffectAndCondition(Darkness)
                .AddToDB();

            EldritchInvocationsAttributeModifiers["OneWithShadows"].FeatureSet.Add(OneWithShadowsLightAffinity);
            EldritchInvocationsAttributeModifiers["OneWithShadows"].Validators.SetRange(() => !Global.ActiveLevelUpHeroHasSubclass("MoonLit"));
            
            EldritchInvocationsAttributeModifiers["OneWithShadowsStronger"].FeatureSet.Add(OneWithShadowsLightAffinityStrong);
            EldritchInvocationsAttributeModifiers["OneWithShadowsStronger"].Validators.SetRange(() => Global.ActiveLevelUpHeroHasFeature(OneWithShadowsLightAffinity));

            EldritchInvocationsAttributeModifiers["DevilsSight"].FeatureSet
                .AddRange(IgnoreDynamicVisionImpairmentBuilder
                    .Create("EldritchInvocationDevilsSight", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent()
                    .SetMaxRange(24)
                    .AddForbiddenFeatures(FeatureDefinitionCombatAffinitys.CombatAffinityHeavilyObscured)
                    .AddRequiredFeatures()
                    .AddToDB(),
                    FeatureDefinitionSenses.SenseDarkvision24
                );
        }
    }
}







