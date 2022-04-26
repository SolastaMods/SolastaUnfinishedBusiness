using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    public static class EldritchInvocationsBuilder
    {
        public static Dictionary<string, SpellDefinition> DictionaryofEIPseudoCantrips { get; private set; } = new();
        public static Dictionary<string, SpellDefinition> DictionaryofEIPseudoSpells { get; private set; } = new();

        public static Dictionary<string, FeatureDefinitionPower> DictionaryofEIPowers { get; private set; } = new();

        public static SpellDefinition EldritchBlast { get; set; }
        public static Dictionary<string, FeatureDefinition> DictionaryofEBInvocations { get; private set; } = new();
        public static List<string> ListofEIAttributeModifers { get; private set; } = new();
        public static Dictionary<string, FeatureDefinitionFeatureSet> DictionaryofEIAttributeModifers { get; private set; } = new();
        public static FeatureDefinitionFeatureSetWithPreRequisites AgonizingBlastFeatureSet { get; set; }
        public static FeatureDefinitionFeatureSetWithPreRequisites HinderingBlastFeatureSet { get; set; }

        public static void Build()
        {
            SpellsToCantripsForEldritchInvocations();
            EldritchBlastAndEBInvocations();
            EIAttributeModifers();
        }

        private static void SpellsToCantripsForEldritchInvocations()
        {
            //at will
            DictionaryofEIPseudoCantrips.Add("ArmorofShadows", DatabaseHelper.SpellDefinitions.MageArmor);// self
            DictionaryofEIPseudoCantrips.Add("EldritchSight", DatabaseHelper.SpellDefinitions.DetectMagic);
            DictionaryofEIPseudoCantrips.Add("FiendishVigor", DatabaseHelper.SpellDefinitions.FalseLife);// self
            DictionaryofEIPseudoCantrips.Add("AscendantStep", DatabaseHelper.SpellDefinitions.Levitate);// self
            DictionaryofEIPseudoCantrips.Add("OtherworldlyLeap", DatabaseHelper.SpellDefinitions.Jump);// self
            DictionaryofEIPseudoCantrips.Add("ChainsofCarceri", DatabaseHelper.SpellDefinitions.HoldMonster);
            DictionaryofEIPseudoCantrips.Add("ShroudofShadow", DatabaseHelper.SpellDefinitions.Invisibility);
            // 1/day
            DictionaryofEIPseudoSpells.Add("ThiefofFiveFates", DatabaseHelper.SpellDefinitions.Bane);
            DictionaryofEIPseudoSpells.Add("MiretheMind", DatabaseHelper.SpellDefinitions.Slow);
            //cant do it this way as it uses subspells  DictionaryofEIPseudoSpells.Add("SignofIllOmen", DatabaseHelper.SpellDefinitions.BestowCurse);
            DictionaryofEIPseudoSpells.Add("DreadfulWord", DatabaseHelper.SpellDefinitions.Confusion);
            DictionaryofEIPseudoSpells.Add("Trickster'sEscape", DatabaseHelper.SpellDefinitions.FreedomOfMovement);// self

            // EI that arent valid for game right now
            //{"BeastSpeech",         DatabaseHelper.SpellDefinitions.},
            //{"BookofAncientSecrets",DatabaseHelper.SpellDefinitions.},
            //{"MaskofManyFaces",     DatabaseHelper.SpellDefinitions.},
            //{"MistyVisions",        DatabaseHelper.SpellDefinitions.},
            //{"FarScribe",           DatabaseHelper.SpellDefinitions.},
            //{"GiftoftheDepths",     DatabaseHelper.SpellDefinitions.},
            //{"UndyingServitude",    DatabaseHelper.SpellDefinitions.},
            //{"BewitchingWhispers",  DatabaseHelper.SpellDefinitions.},
            //{"SculptorofFlesh",     DatabaseHelper.SpellDefinitions.},
            //{"WhispersoftheGrave",  DatabaseHelper.SpellDefinitions.},
            //{"MasterofMyriadForms", DatabaseHelper.SpellDefinitions.},
            // {"VisionsofDistantRealms",DatabaseHelper.SpellDefinitions.},
            // cant convert spellsbundle to power
            //  DictionaryofEIPseudoSpells.Add("MinionsofChaos", DatabaseHelper.SpellDefinitions.ConjureElemental);


            // at will EI
            foreach (KeyValuePair<string, SpellDefinition> entry in DictionaryofEIPseudoCantrips)
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

                DictionaryofEIPowers.Add(entry.Key, EIPower);
            }

            // 1/day EI
            foreach (KeyValuePair<string, SpellDefinition> entry in DictionaryofEIPseudoSpells)
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

                DictionaryofEIPowers.Add(entry.Key, EIPower);
            }
        }

        private static readonly string EldritchBlastName = "DHEldritchBlast"; 

        private static void EldritchBlastAndEBInvocations()
        {

            var eldritchBlastGui = new GuiPresentationBuilder(
                    "Spell/&EldritchBlastTitle",
                    "Spell/&EldritchBlastDescription",
                    DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference)
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
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters)
            .Build();


            var agonizingForm = new EffectFormBuilder(blastDamage)
                .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                .Build();

            var hinderingForm = new EffectFormBuilder()
                .SetConditionForm(
                    DatabaseHelper.ConditionDefinitions.ConditionHindered_By_Frost,
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
            
            var hinderingBlastFeature =  FeatureDefinitionBuilder
                .Create("AdditionalDamageHinderingBlast", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();


            var eldritchBlast = SpellWithCasterFeatureDependentEffectsBuilder
                .Create(EldritchBlastName, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(eldritchBlastGui)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
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

                DictionaryofEBInvocations.Add(name, bonusCantrip);
            }

            MakeEldritchBlastVariant("RepellingBlast", pushForm);
            MakeEldritchBlastVariant("GraspingHand", pullForm);
            
            AgonizingBlastFeatureSet = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                .Create("AgonizingBlastFeatureSet", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(agonizingBlastFeature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .SetValidators(() => Global.ActiveLevelUpHeroHasCantrip(eldritchBlast))
                .AddToDB();

            HinderingBlastFeatureSet = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
                .Create("HinderingBlastFeatureSet", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(hinderingBlastFeature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                //.SetValidator(() => Global.ActiveLevelUpHeroHasCantrip(eldritchBlast))
                .AddToDB();
        }

        private static void EIAttributeModifers()
        {
            ListofEIAttributeModifers.AddRange(new List<String>
                {
                    "AspectoftheMoon",               // DatabaseHelper.FeatureDefinitionCampAffinitys.CampAffinityElfTrance,DatabaseHelper.FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
                    "BeguilingInfluence",            // DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
                    "EldritchMind",                  // DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
                    "EyesoftheRuneKeeper",           // DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
                    "GiftoftheEver-LivingOnes",      // DatabaseHelper.FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
                    "ImprovedPactWeapon",            // AttackModifierMagicWeapon + MagicAffinitySpellBladeIntoTheFray
                    "EldritchSmite",                 // DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
                    "ThirstingBlade",                // DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
                    "GiftoftheProtectors",           // DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
                    "BondoftheTalisman",             // DatabaseHelper.FeatureDefinitionPowers.PowerSorakShadowEscape);
                    "WitchSight",                    // DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12;
                    "OneWithShadows"
                    //   "Lifedrinker",                   // similar to AdditionalDamageDomainOblivionStrikeOblivion +damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
                    //"Devil'sSight",                // DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24 or maybe similar to ConditionAffinityVeilImmunity needs to cover multiple darkness conditions ConditionDarkness ConditionVeil
                    //"GazeofTwoMinds",              //
                    //"InvestmentoftheChainMaster",  // multiple featurs through summoning affinity, could just reuse SummoningAffinityKindredSpiritBond for a similar but not direct copy of non srd EI
                    //"RebukeoftheTalisman",         //
                    //"VoiceoftheChainMaster",       //
                    //"CloakofFlies",                //
                    //"MaddeningHex",                //
                    //"TombofLevistus",              //
                    //"GhostlyGaze",                 //
                    //"ProtectionoftheTalisman",     //
                    //"RelentlessHex",               //
                });

            // EI AttributeModifers
            foreach (string entry in ListofEIAttributeModifers)
            {

                string textEIAttributeModifers = "DHEIAttributeModifers" + entry;



                GuiPresentation guiFeatureSetEldritchInvocations = new GuiPresentationBuilder(
                 "Feature/&" + entry + "Title",
                "Feature/&" + entry + "Description")
                .Build();
                var FeatureSetEldritchInvocationsBuilder = FeatureDefinitionFeatureSetBuilder
                    .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, textEIAttributeModifers, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiFeatureSetEldritchInvocations);

                FeatureDefinitionFeatureSet FeatureSetEldritchInvocations = FeatureSetEldritchInvocationsBuilder
                    .ClearFeatureSet()
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(false)
                    .AddToDB();


                DictionaryofEIAttributeModifers.Add(entry, FeatureSetEldritchInvocations);
            }


            DictionaryofEIAttributeModifers["AspectoftheMoon"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionCampAffinitys.CampAffinityElfTrance);
            DictionaryofEIAttributeModifers["AspectoftheMoon"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);

            DictionaryofEIAttributeModifers["BeguilingInfluence"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
            DictionaryofEIAttributeModifers["EldritchMind"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
            DictionaryofEIAttributeModifers["EyesoftheRuneKeeper"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
            DictionaryofEIAttributeModifers["GiftoftheEver-LivingOnes"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);

            DictionaryofEIAttributeModifers["ImprovedPactWeapon"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon);
            DictionaryofEIAttributeModifers["ImprovedPactWeapon"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray);

            DictionaryofEIAttributeModifers["EldritchSmite"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
            DictionaryofEIAttributeModifers["ThirstingBlade"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
            DictionaryofEIAttributeModifers["GiftoftheProtectors"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
            DictionaryofEIAttributeModifers["BondoftheTalisman"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionPowers.PowerSorakShadowEscape);
            //  DictionaryofEIAttributeModifers["Lifedrinker"].FeatureSet.Add();
            DictionaryofEIAttributeModifers["WitchSight"].FeatureSet.Add(DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12);


            var Unlit = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = DatabaseHelper.ConditionDefinitions.ConditionInvisible
            };
            var Dim = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim,
                condition = DatabaseHelper.ConditionDefinitions.ConditionInvisible
            };
            var Darkness = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness,
                condition = DatabaseHelper.ConditionDefinitions.ConditionInvisible
            };

            FeatureDefinitionLightAffinity OneWithShadowsLightAffinity = FeatureDefinitionLightAffinityBuilder
                .Create("OneWithShadowsLightAffinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("OneWithShadowsLightAffinity", Category.Feature)
                .AddLightingEffectAndCondition(Unlit)
                .AddLightingEffectAndCondition(Dim)
                .AddLightingEffectAndCondition(Darkness)
                .AddToDB();

            DictionaryofEIAttributeModifers["OneWithShadows"].FeatureSet.Add(OneWithShadowsLightAffinity);


        }
    }
}







