using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using UnityEngine;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    public class DHEldritchInvocationsBuilder
    {
        public static readonly Dictionary<string, SpellDefinition> DictionaryofEIPseudoCantrips = new();
        public static readonly Dictionary<string, SpellDefinition> DictionaryofEIPseudoSpells = new();

        public static readonly Dictionary<string, FeatureDefinitionPower> DictionaryofEIPowers = new();

        // TODO: use a builder
        public static SpellDefinition EldritchBlast = ScriptableObject.CreateInstance<SpellDefinition>();
        public static readonly List<string> ListofEBImprovements = new();
        public static readonly Dictionary<string, FeatureDefinitionBonusCantrips> DictionaryofEBInvocations = new();

        public static readonly List<string> ListofEIAttributeModifers = new();
        public static readonly Dictionary<string, FeatureDefinitionFeatureSet> DictionaryofEIAttributeModifers = new();
        public static FeatureDefinitionFeatureSet AgonizingBlastFeatureSet;
        public static FeatureDefinitionFeatureSet HinderingBlastFeatureSet;

        public static void Build()
        {
            SpellsToCantripsForEldritchInvocations();
            EldritchBlastAndEBInvocations();
            EIAttributeModifers();
        }

        private static void SpellsToCantripsForEldritchInvocations()
        {



            var castSpellName = "SpellsToCantripsForEldritchInvocations";
            var castSpellGuid = GuidHelper.Create(new Guid(Settings.GUID), castSpellName).ToString();

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
                    "Cast " + entry.Value.name + " at will",  //entry.Value.GuiPresentation.Description,// textPseudoCantrips + "Description",
                    "Feature/&" + entry.Key + "Title")         //                    entry.Value.GuiPresentation.title) // textPseudoCantrips + "Title")
                     .SetSpriteReference(entry.Value.GuiPresentation.SpriteReference)
                     .Build();

                var EIPowerBuilder = FeatureDefinitionPowerBuilder
                    .Create(textPseudoCantrips, GuidHelper.Create(new Guid(Settings.GUID), textPseudoCantrips).ToString())
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
                     entry.Value.EffectDescription,
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
                    "Cast " + entry.Value.name + " once per day",                   // entry.Value.GuiPresentation.Description,  //textPseudoSpells + "Description",
                    "Feature/&" + entry.Key + "Title")                   // entry.Value.GuiPresentation.title)        //textPseudoSpells + "Title")
                    .SetSpriteReference(entry.Value.GuiPresentation.SpriteReference)
                    .Build();
                var EIPowerBuilder = FeatureDefinitionPowerBuilder
                    .Create(textPseudoSpells, GuidHelper.Create(new Guid(Settings.GUID), textPseudoSpells).ToString())
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
                     entry.Value.EffectDescription,
                     true);
                FeatureDefinitionPower EIPower = EIPowerBuilder.AddToDB();

                if (entry.Key == "Trickster'sEscape")
                {
                    EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
                }

                DictionaryofEIPowers.Add(entry.Key, EIPower);
            }

        }


        private static void EldritchBlastAndEBInvocations()
        {

            GuiPresentationBuilder EldritchBlastGui = new GuiPresentationBuilder(
            "Spell/&EldritchBlastDescription",
            "Spell/&EldritchBlastTitle");
            EldritchBlastGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescription EldritchBlastEffect = new EffectDescriptionBuilder()
            .AddEffectForm(
                new EffectFormBuilder().SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D10,
                    RuleDefinitions.DamageTypeForce,
                    0,
                    RuleDefinitions.DieType.D10,
                    1,
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>())
                .Build())
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



            SpellDefinitionBuilder EldritchBlastBuilder = SpellDefinitionBuilder
                .Create("DHEldritchBlast", GuidHelper.Create(new System.Guid(Settings.GUID), "DHEldritchBlast").ToString())
                .SetGuiPresentation(EldritchBlastGui.Build())
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(EldritchBlastEffect)
                .SetAiParameters(new SpellAIParameters());
            EldritchBlast = EldritchBlastBuilder.AddToDB();


            //  AdditionalDamageSorcererDraconicElementalAffinity and AncestrySorcererDraconicGold - works for single class but would be a bug/buff for multiclass
            // ListofEBImprovements.Add("AgonizingBlast");    // AttackModifierOathOfDevotionSacredWeapon//PowerTraditionShockArcanistArcaneFury
            // ListofEBImprovements.Add("EldritchSpear" );  // not really useful for game
            //    ListofEBImprovements.Add("HinderingBlast");// ConditionHindered
            ListofEBImprovements.Add("RepellingBlast");// pushFromOrigin
            ListofEBImprovements.Add("GraspingHand");//rope grapple but part of EB



            // at will EI
            foreach (string entry in ListofEBImprovements)
            {



                string textEBImprovements = "DHEldritchBlast" + entry;

                EffectDescription effect = new EffectDescription();
                effect.Copy(EldritchBlastEffect);

                SpellDefinitionBuilder EIcantripBuilder = SpellDefinitionBuilder
                    .Create(textEBImprovements, GuidHelper.Create(new Guid(Settings.GUID), textEBImprovements).ToString())
                .SetGuiPresentation(new GuiPresentationBuilder
                    (
                    "Spell/&" + entry + "Description",
                    "Spell/&" + entry + "Title"
                    )
                    .SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference).Build()
                    )
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(0)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effect)
                .SetAiParameters(new SpellAIParameters());
                SpellDefinition EIcantrip = EIcantripBuilder.AddToDB();


                GuiPresentation guiPresentationEBImprovements = new GuiPresentationBuilder("Feature/&" + entry + "MagicAffinityDescription", "Feature/&" + entry + "MagicAffinityTitle").Build();

                FeatureDefinitionBonusCantrips BonusCantrip = FeatureDefinitionBonusCantripsBuilder
                    .Create(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion, textEBImprovements + "BonusCantrip", GuidHelper.Create(new Guid(Settings.GUID), textEBImprovements + "BonusCantrip").ToString())
                    .SetGuiPresentation(guiPresentationEBImprovements)
                        .ClearBonusCantrips()
                        .AddBonusCantrip(EIcantrip)
                        .AddToDB();


                DictionaryofEBInvocations.Add(entry, BonusCantrip);
            }

            DictionaryofEBInvocations["RepellingBlast"].BonusCantrips[0].EffectDescription.EffectForms
                .Add(new EffectFormBuilder()
                .SetMotionForm(
                    MotionForm.MotionType.PushFromOrigin,
                    2)
                .Build());

            DictionaryofEBInvocations["GraspingHand"].BonusCantrips[0].EffectDescription.EffectForms
                .Add(new EffectFormBuilder()
                .SetMotionForm(
                    MotionForm.MotionType.DragToOrigin,
                    2)
                .Build());




            // Agonizing blast could have Cha added via SpellDamageMatchesSourceAncestry for force damage
            // not a problem for warlocks but could buff magic missile if a warlock wizard multiclass is a thing
            // Bigger problem is that its limited to first roll only...
            // could do the same for Hindering/lethargic blast (via condition operations)

            // TODO: FeatureDefinitionAncestryBuilder
            var EldritchBlastAncestry = ScriptableObject.CreateInstance<FeatureDefinitionAncestry>();
            EldritchBlastAncestry.SetDamageType(RuleDefinitions.DamageTypeForce);
            EldritchBlastAncestry.name = "AgonizingBlastForceAncestry";
            EldritchBlastAncestry.SetGuid(GuidHelper.Create(new Guid(Settings.GUID), "AgonizingBlastForceAncestry").ToString());
            EldritchBlastAncestry.GuiPresentation = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAdditionalDamage AdditionalDamageAgonizingBlast = new FeatureDefinitionAdditionalDamageBuilder(
                     "AdditionalDamageAgonizingBlast",
                     GuidHelper.Create(new Guid(Settings.GUID), "AdditionalDamageAgonizingBlast").ToString(),
                     "Agonizing_Blast",
                     RuleDefinitions.FeatureLimitedUsage.None,
                     RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus,
                     RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry,
                     RuleDefinitions.AdditionalDamageRequiredProperty.None,
                     false,
                     RuleDefinitions.DieType.D4,
                     1,
                     RuleDefinitions.AdditionalDamageType.AncestryDamageType,
                     RuleDefinitions.DamageTypeForce,
                     RuleDefinitions.AdditionalDamageAdvancement.None,
                     new List<DiceByRank>(),
                     new GuiPresentationBuilder("Feature/&AdditionalDamageAgonizingBlastDescription", "Feature/&AdditionalDamageAgonizingBlastTitle").Build()
                ).AddToDB();

            AgonizingBlastFeatureSet = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "AgonizingBlastFeatureSet", GuidHelper.Create(new Guid(Settings.GUID), "AgonizingBlastFeatureSet").ToString())
                .SetGuiPresentation(new GuiPresentationBuilder("Feature/&AgonizingBlastFeatureSetDescription", "Feature/&AgonizingBlastFeatureSetTitle").Build())
               .ClearFeatureSet()
               .AddFeatureSet(EldritchBlastAncestry)
               .AddFeatureSet(AdditionalDamageAgonizingBlast)
               .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
               .SetUniqueChoices(false)
               .AddToDB();

            var hinderingConditionOperation = new ConditionOperationDescription();
            hinderingConditionOperation.SetOperation(ConditionOperationDescription.ConditionOperation.Add);
            hinderingConditionOperation.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionHindered_By_Frost);
            hinderingConditionOperation.SetConditionName(DatabaseHelper.ConditionDefinitions.ConditionHindered.name);

            FeatureDefinitionAdditionalDamage AdditionalDamageHinderingBlast = new FeatureDefinitionAdditionalDamageBuilder(
                     "AdditionalDamageHinderingBlast",
                     GuidHelper.Create(new Guid(Settings.GUID), "AdditionalDamageHinderingBlast").ToString(),
                     "Hindering Blast",
                     RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                     RuleDefinitions.AdditionalDamageValueDetermination.Die,
                     RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry,
                     RuleDefinitions.AdditionalDamageRequiredProperty.None,
                     false,
                     RuleDefinitions.DieType.D1,
                     0,
                     RuleDefinitions.AdditionalDamageType.AncestryDamageType,
                     RuleDefinitions.DamageTypeForce,
                     RuleDefinitions.AdditionalDamageAdvancement.None,
                     new List<DiceByRank>(),
                     new GuiPresentationBuilder("Feature/&AdditionalDamageHinderingBlastDescription", "Feature/&AdditionalDamageHinderingBlastTitle").Build()
                )
                .SetConditionOperations(new List<ConditionOperationDescription> { hinderingConditionOperation })
                .AddToDB();

            HinderingBlastFeatureSet = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "HinderingBlastFeatureSet", GuidHelper.Create(new Guid(Settings.GUID), "HinderingBlastFeatureSet").ToString())
               .SetGuiPresentation(new GuiPresentationBuilder("Feature/&HinderingBlastFeatureSetDescription", "Feature/&HinderingBlastFeatureSetTitle").Build())
               .ClearFeatureSet()
               .AddFeatureSet(EldritchBlastAncestry)
               .AddFeatureSet(AdditionalDamageHinderingBlast)
               .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
               .SetUniqueChoices(false)
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
                    "WitchSight"                     // DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible12;

                    //   "Lifedrinker",                   // similar to AdditionalDamageDomainOblivionStrikeOblivion +damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
                    //"Devil'sSight",                // DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24 or maybe similar to ConditionAffinityVeilImmunity needs to cover multiple darkness conditions ConditionDarkness ConditionVeil
                    //"GazeofTwoMinds",              //
                    //"InvestmentoftheChainMaster",  // multiple featurs through summoning affinity, could just reuse SummoningAffinityKindredSpiritBond for a similar but not direct copy of non srd EI
                    //"RebukeoftheTalisman",         //
                    //"VoiceoftheChainMaster",       //
                    //"CloakofFlies",                //
                    //"MaddeningHex",                //
                    //"OnewithShadows",              //
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
                 "Feature/&" + entry + "Description",
                "Feature/&" + entry + "Title")
                .Build();
                var FeatureSetEldritchInvocationsBuilder = FeatureDefinitionFeatureSetBuilder
                    .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, textEIAttributeModifers, GuidHelper.Create(new Guid(Settings.GUID), textEIAttributeModifers).ToString())
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

        }
    }
}







