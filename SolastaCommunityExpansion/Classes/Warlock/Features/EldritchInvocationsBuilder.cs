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
    internal static class EldritchInvocationsBuilder
    {
        internal static SpellDefinition EldritchBlast { get; set; }

        internal static Dictionary<string, FeatureDefinition> EldritchInvocations { get; private set; } = new();

        internal static void Build()
        {
            BuildEldritchBlastAndInvocations();
            BuildEldritchInvocationsSpellsToCantrips();
            BuildEldritchInvocationsAttributeModifiers();
        }

        private const string EldritchBlastName = "EldritchBlast";

        private static void BuildEldritchBlastAndInvocations()
        {
            var blastDamage = new EffectFormBuilder()
                .SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D10,
                    RuleDefinitions.DamageTypeForce,
                    0,
                    RuleDefinitions.DieType.D10,
                    1,
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>())
                .Build();

            var eldritchBlastEffect = new EffectDescriptionBuilder()
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
                .SetConditionForm(ConditionDefinitions.ConditionHindered_By_Frost, ConditionForm.ConditionOperation.Add)
                .Build();

            var agonizingBlastEffect = new EffectDescriptionBuilder(eldritchBlastEffect)
                .SetEffectForms(agonizingForm)
                .Build();

            var hinderingBlastEffect = new EffectDescriptionBuilder(eldritchBlastEffect)
                .AddEffectForm(hinderingForm)
                .Build();

            var hinderingAgonizingBlastEffect = new EffectDescriptionBuilder(eldritchBlastEffect)
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
                .SetGuiPresentation(Category.Spell, SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(eldritchBlastEffect)
                .SetFeatureEffects(
                    (new() { agonizingBlastFeature, hinderingBlastFeature }, hinderingAgonizingBlastEffect),
                    (new() { agonizingBlastFeature }, agonizingBlastEffect),
                    (new() { hinderingBlastFeature }, hinderingBlastEffect)
                )
                .SetAiParameters(new())
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
                effect.Copy(eldritchBlastEffect);

                var cantrip = SpellWithCasterFeatureDependentEffectsBuilder
                    .Create(eldritchBlast, cantripName, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Spell, EldritchBlast.GuiPresentation.SpriteReference)
                    .SetEffectDescription(CustomFeaturesContext.AddEffectForms(eldritchBlastEffect, forms))
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

                EldritchInvocations.Add(name, bonusCantrip);
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

            EldritchInvocations.Add(agonizingBlast.Name, agonizingBlast);

            var hinderingBlast = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                .Create("HinderingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(hinderingBlastFeature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .SetValidators(() => Global.ActiveLevelUpHeroHasCantrip(eldritchBlast))
                .AddToDB();

            EldritchInvocations.Add(hinderingBlast.Name, hinderingBlast);
        }

        private static void BuildEldritchInvocationsSpellsToCantrips()
        {
            Dictionary<string, SpellDefinition> dictionaryofEIPseudoCantrips = new()
            {
                { "ArmorofShadows", SpellDefinitions.MageArmor }, // self
                { "EldritchSight", SpellDefinitions.DetectMagic },
                { "FiendishVigor", SpellDefinitions.FalseLife }, // self
                { "AscendantStep", SpellDefinitions.Levitate }, // self
                { "OtherworldlyLeap", SpellDefinitions.Jump }, // self
                { "ChainsofCarceri", SpellDefinitions.HoldMonster },
                { "ShroudofShadow", SpellDefinitions.Invisibility }
            };

            Dictionary<string, SpellDefinition> dictionaryofEIPseudoSpells = new()
            {
                { "ThiefofFiveFates", SpellDefinitions.Bane },
                { "MiretheMind", SpellDefinitions.Slow },
                { "DreadfulWord", SpellDefinitions.Confusion },
                { "TrickstersEscape", SpellDefinitions.FreedomOfMovement }
            };

            // EI that arent valid for game right now

            //{"BeastSpeech",            SpellDefinitions.},
            //{"BookofAncientSecrets",   SpellDefinitions.},
            //{"MaskofManyFaces",        SpellDefinitions.},
            //{"MistyVisions",           SpellDefinitions.},
            //{"FarScribe",              SpellDefinitions.},
            //{"GiftoftheDepths",        SpellDefinitions.},
            //{"UndyingServitude",       SpellDefinitions.},
            //{"BewitchingWhispers",     SpellDefinitions.},
            //{"SculptorofFlesh",        SpellDefinitions.},
            //{"WhispersoftheGrave",     SpellDefinitions.},
            //{"MasterofMyriadForms",    SpellDefinitions.},
            //{"VisionsofDistantRealms", SpellDefinitions.},

            // at will EI
            foreach (KeyValuePair<string, SpellDefinition> entry in dictionaryofEIPseudoCantrips)
            {
                string textPseudoCantrips = "EldritchInvocation" + entry.Value.name;

                var guiPresentationEIPseudoCantrips = new GuiPresentationBuilder(
                    $"Feature/&{entry.Key}Title",
                    Gui.Format(
                        "Feature/&SpellAsInvocationAtWillDescription",
                        entry.Value.FormatTitle()),
                    entry.Value.GuiPresentation.SpriteReference).Build();

                var EIPower = FeatureDefinitionPowerBuilder
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
                         true)
                    .AddToDB();

                if (entry.Key == "ArmorofShadows"
                    || entry.Key == "FiendishVigor"
                    || entry.Key == "AscendantStep"
                    || entry.Key == "OtherworldlyLeap")
                {
                    EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
                }

                EldritchInvocations.Add(entry.Key, EIPower);
            }

            // 1/day EI
            foreach (KeyValuePair<string, SpellDefinition> entry in dictionaryofEIPseudoSpells)
            {
                string textPseudoSpells = "EldritchInvocation" + entry.Value.name;

                var guiPresentationEIPseudoSpells = new GuiPresentationBuilder(
                    "Feature/&" + entry.Key + "Title",
                    Gui.Format(
                        "Feature/&SpellAsInvocationOncePerDayDescription",
                        entry.Value.FormatTitle()),
                    entry.Value.GuiPresentation.SpriteReference).Build();

                var EIPower = FeatureDefinitionPowerBuilder
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
                         true)
                    .AddToDB();

                if (entry.Key == "TrickstersEscape")
                {
                    EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
                }

                EldritchInvocations.Add(entry.Key, EIPower);
            }
        }

        private static void BuildEldritchInvocationsAttributeModifiers()
        {
            var listofEIAttributeModifiers = new List<string>
            {
                "AspectoftheMoon",               // FeatureDefinitionCampAffinitys.CampAffinityElfTrance,FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
                "BeguilingInfluence",            // FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
                "EldritchMind",                  // FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
                "EyesoftheRuneKeeper",           // FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
                "GiftoftheEverLivingOnes",      // FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
                "ImprovedPactWeapon",            // AttackModifierMagicWeapon + MagicAffinitySpellBladeIntoTheFray
                "EldritchSmite",                 // FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
                "ThirstingBlade",                // FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
                "GiftoftheProtectors",           // FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
                "BondoftheTalisman",             // FeatureDefinitionPowers.PowerSorakShadowEscape);
                "WitchSight",                    // FeatureDefinitionSenses.SenseSeeInvisible12;
                "OneWithShadows",
                "OneWithShadowsStronger",
                "DevilsSight",
                //"Lifedrinker",                 // similar to AdditionalDamageDomainOblivionStrikeOblivion +damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
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
                var textEIAttributeModifiers = "EldritchInvocation" + entry;

                var guiFeatureSetEldritchInvocations = new GuiPresentationBuilder(
                    "Feature/&" + entry + "Title",
                    "Feature/&" + entry + "Description")
                    .Build();

                var FeatureSetEldritchInvocations = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                    .Create(textEIAttributeModifiers, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiFeatureSetEldritchInvocations)
                    .ClearFeatureSet()
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(false)
                    .AddToDB();

                EldritchInvocations.Add(entry, FeatureSetEldritchInvocations);
            }

            ((FeatureDefinitionFeatureSet)EldritchInvocations["AspectoftheMoon"]).FeatureSet
                .Add(FeatureDefinitionCampAffinitys.CampAffinityElfTrance);
            ((FeatureDefinitionFeatureSet)EldritchInvocations["AspectoftheMoon"]).FeatureSet
                .Add(FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
            //((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["AspectoftheMoon"]).Validators.SetRange(() =>
            //    Global.ActiveLevelUpHeroHasFeature(DHPactOfTheTomeFeatureSetBuilder.DHPactOfTheTomeFeatureSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["BeguilingInfluence"]).FeatureSet
                .Add(FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
            
            ((FeatureDefinitionFeatureSet)EldritchInvocations["EldritchMind"]).FeatureSet
                .Add(FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["EldritchMind"]).Validators.SetRange(() =>
                Global.ActiveLevelUpHeroHasFeature(DHPactOfTheTomeFeatureSetBuilder.DHPactOfTheTomeFeatureSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["EyesoftheRuneKeeper"]).FeatureSet
                .Add(FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
            
            ((FeatureDefinitionFeatureSet)EldritchInvocations["GiftoftheEverLivingOnes"]).FeatureSet
                .Add(FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
            //((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["GiftoftheEverLivingOnes"]).Validators.SetRange(() =>
            //   Global.ActiveLevelUpHeroHasFeature(DHWarlockClassPactOfTheChainFeatureSetBuilder.DHWarlockClassPactOfTheChainFeatureSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["ImprovedPactWeapon"]).FeatureSet
                .Add(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon);
            ((FeatureDefinitionFeatureSet)EldritchInvocations["ImprovedPactWeapon"]).FeatureSet
                .Add(FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray);
            //((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["ImprovedPactWeapon"]).Validators.SetRange(() =>
            //    Global.ActiveLevelUpHeroHasFeature(AHWarlockClassPactOfTheBladeSetBuilder.AHWarlockClassPactOfTheBladeSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["EldritchSmite"]).FeatureSet
                .Add(FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["EldritchSmite"]).Validators.SetRange(() =>
                Global.ActiveLevelUpHeroHasFeature(AHWarlockClassPactOfTheBladeSetBuilder.AHWarlockClassPactOfTheBladeSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["ThirstingBlade"]).FeatureSet
                .Add(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["ThirstingBlade"]).Validators.SetRange(() =>
                Global.ActiveLevelUpHeroHasFeature(AHWarlockClassPactOfTheBladeSetBuilder.AHWarlockClassPactOfTheBladeSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["GiftoftheProtectors"]).FeatureSet
                .Add(FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["GiftoftheProtectors"]).Validators.SetRange(() =>
               Global.ActiveLevelUpHeroHasFeature(DHPactOfTheTomeFeatureSetBuilder.DHPactOfTheTomeFeatureSet, recursive: false));

            ((FeatureDefinitionFeatureSet)EldritchInvocations["BondoftheTalisman"]).FeatureSet
                .Add(FeatureDefinitionPowers.PowerSorakShadowEscape);
            
            ((FeatureDefinitionFeatureSet)EldritchInvocations["WitchSight"]).FeatureSet
                .Add(FeatureDefinitionSenses.SenseSeeInvisible12);

            var shadowsInvisibiityConditionDefinition = ConditionDefinitionBuilder
                .Create("WarlockConditionShadowsSpecial", DefinitionBuilder.CENamespaceGuid)
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

            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadows"]).FeatureSet.Add(OneWithShadowsLightAffinity);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadows"]).Validators.SetRange(() =>
                !Global.ActiveLevelUpHeroHasSubclass("MoonLit"));

            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadowsStronger"]).FeatureSet.Add(OneWithShadowsLightAffinityStrong);
            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadowsStronger"]).Validators.SetRange(() => 
                Global.ActiveLevelUpHeroHasFeature(OneWithShadowsLightAffinity));

            ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["DevilsSight"]).FeatureSet
                .AddRange(IgnoreDynamicVisionImpairmentBuilder
                    .Create("EldritchInvocationDevilsSightSet", DefinitionBuilder.CENamespaceGuid)
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
