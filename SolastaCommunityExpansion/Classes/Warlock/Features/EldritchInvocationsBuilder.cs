using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warlock.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Warlock.Features;

internal static class EldritchInvocationsBuilder
{
    private const string EldritchBlastName = "EldritchBlast";

    public static AssetReferenceSprite EldritchBLastIcon =
        CustomIcons.CreateAssetReferenceSprite("EldritchBlast", Resources.EldritchBlast, 128, 128);

    public static AssetReferenceSprite EldritchBLastIconGrasp =
        CustomIcons.CreateAssetReferenceSprite("EldritchBlastGrasp", Resources.EldritchBlastGrasp, 128,
            128);

    public static AssetReferenceSprite EldritchBLastIconRepell =
        CustomIcons.CreateAssetReferenceSprite("EldritchBlastRepell", Resources.EldritchBlastRepell, 128,
            128);

    private static readonly IFeatureDefinitionWithPrerequisites.Validate RequireEldritchBlast = () =>
        Global.ActiveLevelUpHeroHasCantrip(EldritchBlast)
            ? null
            : "Requirement/&WarlockMissingEldritchBlast";

    private static readonly IFeatureDefinitionWithPrerequisites.Validate RequirePactOfTheBlade = () =>
        Global.ActiveLevelUpHeroHasFeature(WarlockClassPactOfTheBladeSetBuilder.WarlockClassPactOfTheBladeSet)
            ? null
            : "Requirement/&WarlockRequiresPactOfBlade";

    private static readonly IFeatureDefinitionWithPrerequisites.Validate RequirePactOfTheTome = () =>
        Global.ActiveLevelUpHeroHasFeature(PactOfTheTomeFeatureSetBuilder.PactOfTheTomeFeatureSet)
            ? null
            : "Requirement/&WarlockRequiresPactOfTome";

    private static readonly IFeatureDefinitionWithPrerequisites.Validate RequirePactOfTheChain = () =>
        Global.ActiveLevelUpHeroHasFeature(WarlockClassPactOfTheChainFeatureSetBuilder
            .WarlockClassPactOfTheChainFeatureSet)
            ? null
            : "Requirement/&WarlockRequiresPactOfChain";

    internal static SpellDefinition EldritchBlast { get; set; }

    internal static Dictionary<string, FeatureDefinition> EldritchInvocations { get; } = new();

    internal static void Build()
    {
        BuildEldritchBlastAndInvocations();
        BuildEldritchInvocationsSpellsToCantrips();
        BuildEldritchInvocationsAttributeModifiers();
    }

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
                RuleDefinitions.TargetType.Individuals)
            .SetEffectAdvancement(
                RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                5,
                1
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

        var eldritchBlast = SpellDefinitionWithDependentEffectsBuilder
            .Create(EldritchBlastName, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Spell, EldritchBLastIcon)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetEffectDescription(eldritchBlastEffect)
            .SetFeatureEffects(
                (new List<FeatureDefinition> {agonizingBlastFeature, hinderingBlastFeature},
                    hinderingAgonizingBlastEffect),
                (new List<FeatureDefinition> {agonizingBlastFeature}, agonizingBlastEffect),
                (new List<FeatureDefinition> {hinderingBlastFeature}, hinderingBlastEffect)
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

        void MakeEldritchBlastVariant(string name, AssetReferenceSprite icon, params EffectForm[] forms)
        {
            var cantripName = EldritchBlastName + name;

            var effect = new EffectDescription();
            effect.Copy(eldritchBlastEffect);

            var cantrip = SpellDefinitionWithDependentEffectsBuilder
                .Create(eldritchBlast, cantripName, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Spell, icon)
                .SetEffectDescription(CustomFeaturesContext.AddEffectForms(eldritchBlastEffect, forms))
                .SetFeatureEffects(eldritchBlast.FeaturesEffectList
                    .Select(t => (t.Item1, CustomFeaturesContext.AddEffectForms(t.Item2, forms)))
                    .ToArray()
                )
                .AddToDB();

            var bonusCantrip = FeatureDefinitionBonusCantripsWithPrerequisitesBuilder
                .Create(cantripName + "BonusCantrip", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature, icon)
                .ClearBonusCantrips()
                .AddBonusCantrip(cantrip)
                .SetValidators(RequireEldritchBlast)
                .AddToDB();

            EldritchInvocations.Add(name, bonusCantrip);
        }

        MakeEldritchBlastVariant("RepellingBlast", EldritchBLastIconRepell, pushForm);
        MakeEldritchBlastVariant("GraspingHand", EldritchBLastIconGrasp, pullForm);

        var agonizingBlast = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
            .Create("AgonizingBlast", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSet(agonizingBlastFeature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .SetValidators(RequireEldritchBlast)
            .AddToDB();

        EldritchInvocations.Add(agonizingBlast.Name, agonizingBlast);

        var hinderingBlast = FeatureDefinitionFeatureSetWithPreRequisitesBuilder
            .Create("HinderingBlast", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSet(hinderingBlastFeature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .SetValidators(RequireEldritchBlast)
            .AddToDB();

        EldritchInvocations.Add(hinderingBlast.Name, hinderingBlast);
    }

    private static void BuildEldritchInvocationsSpellsToCantrips()
    {
        Dictionary<string, SpellDefinition> dictionaryofEIPseudoCantrips = new()
        {
            {"ArmorofShadows", SpellDefinitions.MageArmor}, // self
            {"EldritchSight", SpellDefinitions.DetectMagic},
            {"FiendishVigor", SpellDefinitions.FalseLife}, // self
            {"AscendantStep", SpellDefinitions.Levitate}, // self
            {"OtherworldlyLeap", SpellDefinitions.Jump}, // self
            {"ChainsofCarceri", SpellDefinitions.HoldMonster},
            {"ShroudofShadow", SpellDefinitions.Invisibility}
        };

        Dictionary<string, SpellDefinition> dictionaryofEIPseudoSpells = new()
        {
            {"ThiefofFiveFates", SpellDefinitions.Bane},
            {"MiretheMind", SpellDefinitions.Slow},
            {"DreadfulWord", SpellDefinitions.Confusion},
            {"TrickstersEscape", SpellDefinitions.FreedomOfMovement}
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
        foreach (var entry in dictionaryofEIPseudoCantrips)
        {
            var invocationName = entry.Key;
            var baseSpell = entry.Value;
            var textPseudoCantrips = "EldritchInvocation" + baseSpell.name;

            var guiPresentationEIPseudoCantrips = new GuiPresentationBuilder(
                $"Feature/&{invocationName}Title",
                Gui.Format(
                    "Feature/&SpellAsInvocationAtWillDescription",
                    baseSpell.FormatTitle()),
                baseSpell.GuiPresentation.SpriteReference).Build();

            SpellDefinition EICantrip;
            if (invocationName == "ChainsofCarceri")
            {
                EICantrip = BuildChainsOfCarceriCantrip(textPseudoCantrips, invocationName, baseSpell);
            }
            else
            {
                EICantrip = SpellDefinitionBuilder
                    .Create(baseSpell, $"{textPseudoCantrips}Cantrip", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(
                        $"Feature/&{invocationName}Title",
                        baseSpell.GuiPresentation.Description,
                        baseSpell.GuiPresentation.SpriteReference
                    )
                    .SetSpellLevel(0)
                    .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
                    .AddToDB();
            }


            var EIPower = FeatureDefinitionBonusCantripsBuilder
                .Create(textPseudoCantrips, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(guiPresentationEIPseudoCantrips)
                .SetBonusCantrips(EICantrip)
                .AddToDB();

            var cantripEffect = EICantrip.EffectDescription;
            if (invocationName == "ArmorofShadows"
                || invocationName == "FiendishVigor"
                || invocationName == "AscendantStep"
                || invocationName == "OtherworldlyLeap")
            {
                cantripEffect.SetRangeType(RuleDefinitions.RangeType.Self);
                cantripEffect.TargetType = RuleDefinitions.TargetType.Self;
                cantripEffect.SetHasSavingThrow(false);
            }

            var effectAdvancement = new EffectAdvancement();
            effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.None;
            cantripEffect.SetEffectAdvancement(effectAdvancement);

            EldritchInvocations.Add(invocationName, EIPower);
        }

        // 1/day EI
        foreach (var entry in dictionaryofEIPseudoSpells)
        {
            var textPseudoSpells = "EldritchInvocation" + entry.Value.name;

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
                    entry.Value.EffectDescription,
                    true)
                .AddToDB();

            if (entry.Key == "TrickstersEscape")
            {
                EIPower.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;
            }

            EldritchInvocations.Add(entry.Key, EIPower);
        }
    }

    private static SpellDefinition BuildChainsOfCarceriCantrip(string textPseudoCantrips, string invocationName,
        SpellDefinition baseSpell)
    {
        // Rules say it can't be applied to same taget more than once per long rest, for now unlimited applications seem fine.
        return SpellDefinitionBuilder
            .Create($"{textPseudoCantrips}Cantrip", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(
                invocationName,
                Category.Feature,
                baseSpell.GuiPresentation.SpriteReference
            )
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetRequiresConcentration(true)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetFiltering(RuleDefinitions.TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    18,
                    RuleDefinitions.TargetType.Individuals
                )
                .AddRestrictedCreatureFamily(CharacterFamilyDefinitions.Celestial)
                .AddRestrictedCreatureFamily(CharacterFamilyDefinitions.Fiend)
                .AddRestrictedCreatureFamily(CharacterFamilyDefinitions.Elemental)
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Wisdom
                )
                .SetEffectForms(new EffectFormBuilder()
                    .SetConditionForm(
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionForm.ConditionOperation.Add
                    )
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .Build()
                )
                .Build()
            )
            .AddToDB();
    }

    private static void BuildEldritchInvocationsAttributeModifiers()
    {
        var listofEIAttributeModifiers = new List<string>
        {
            "AspectoftheMoon", // FeatureDefinitionCampAffinitys.CampAffinityElfTrance,FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest);
            "BeguilingInfluence", // FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise);
            "EldritchMind", // FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration);
            "EyesoftheRuneKeeper", // FeatureDefinitionFeatureSets.FeatureSetAllLanguages);
            "GiftoftheEverLivingOnes", // FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
            "ImprovedPactWeapon", // AttackModifierMagicWeapon + MagicAffinitySpellBladeIntoTheFray
            "EldritchSmite", // FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite);
            "ThirstingBlade", // FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);
            "GiftoftheProtectors", // FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance);
            "BondoftheTalisman", // FeatureDefinitionPowers.PowerSorakShadowEscape);
            "WitchSight", // FeatureDefinitionSenses.SenseSeeInvisible12;
            "OneWithShadows",
            "OneWithShadowsStronger",
            "DevilsSight"
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

        foreach (var entry in listofEIAttributeModifiers)
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

        ((FeatureDefinitionFeatureSet)EldritchInvocations["AspectoftheMoon"]).FeatureSet.AddRange(
            FeatureDefinitionCampAffinityBuilder.Create(FeatureDefinitionCampAffinitys.CampAffinityElfTrance,
                    "ClassWarlockEldritchInvocationAspectoftheMoonTrance", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(EldritchInvocations["AspectoftheMoon"].GuiPresentation)
                .AddToDB(),
            FeatureDefinitionCampAffinityBuilder.Create(
                    FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest,
                    "ClassWarlockEldritchInvocationAspectoftheMoonRest", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(EldritchInvocations["AspectoftheMoon"].GuiPresentation)
                .AddToDB()
        );
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["AspectoftheMoon"]).Validators.SetRange(
            RequirePactOfTheTome);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["BeguilingInfluence"]).FeatureSet
            .Add(FeatureDefinitionProficiencyBuilder
                .Create(FeatureDefinitionProficiencys.ProficiencyFeatManipulatorSkillOrExpertise,
                    "ClassWarlockEldritchInvocationBeguilingInfluence", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(EldritchInvocations["BeguilingInfluence"].GuiPresentation)
                .AddToDB());

        ((FeatureDefinitionFeatureSet)EldritchInvocations["EldritchMind"]).FeatureSet
            .Add(FeatureDefinitionMagicAffinityBuilder
                .Create("ClassWarlockEldritchInvocationEldritchMind", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(EldritchInvocations["EldritchMind"].GuiPresentation)
                .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                .AddToDB());

        ((FeatureDefinitionFeatureSet)EldritchInvocations["EyesoftheRuneKeeper"]).FeatureSet
            .Add(FeatureDefinitionFeatureSets.FeatureSetAllLanguages);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["GiftoftheEverLivingOnes"]).FeatureSet
            .Add(FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope);
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["GiftoftheEverLivingOnes"]).Validators
            .SetRange(RequirePactOfTheChain);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["ImprovedPactWeapon"]).FeatureSet
            .Add(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon);
        ((FeatureDefinitionFeatureSet)EldritchInvocations["ImprovedPactWeapon"]).FeatureSet
            .Add(FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray);

        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["ImprovedPactWeapon"]).Validators
            .SetRange(RequirePactOfTheBlade);

        var eldritchSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create("WarlockEldritchSmiteDamage", DefinitionBuilder.CENamespaceGuid)
            .Configure("EldritchSmite",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot,
                RuleDefinitions.AdditionalDamageRequiredProperty.None,
                true,
                RuleDefinitions.DieType.D8,
                0,
                RuleDefinitions.AdditionalDamageType.Specific,
                RuleDefinitions.DamageTypeForce,
                RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
                DiceByRankMaker.MakeBySteps()
            )
            .SetCustomSubFeatures(new WarlockClassHolder())
            .AddToDB();

        ((FeatureDefinitionFeatureSet)EldritchInvocations["EldritchSmite"]).FeatureSet.Add(eldritchSmite);
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["EldritchSmite"]).Validators.SetRange(
            RequirePactOfTheBlade);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["ThirstingBlade"]).FeatureSet
            .Add(FeatureDefinitionAttributeModifierBuilder
                .Create(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack,
                    "ClassWarlockEldritchInvocationThirstingBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(EldritchInvocations["ThirstingBlade"].GuiPresentation)
                .AddToDB()
            );
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["ThirstingBlade"]).Validators.SetRange(
            RequirePactOfTheBlade);

        var giftoftheProtectorsDamageAffinity = FeatureDefinitionDamageAffinityBuilder
            .Create(
                FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance,
                "DamageAffinityGiftoftheProtectorsRelentlessEndurance",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        ((FeatureDefinitionFeatureSet)EldritchInvocations["GiftoftheProtectors"]).FeatureSet
            .Add(giftoftheProtectorsDamageAffinity);
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["GiftoftheProtectors"]).Validators
            .SetRange(RequirePactOfTheBlade);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["BondoftheTalisman"]).FeatureSet
            .Add(FeatureDefinitionPowers.PowerSorakShadowEscape);

        ((FeatureDefinitionFeatureSet)EldritchInvocations["WitchSight"]).FeatureSet
            .Add(FeatureDefinitionSenses.SenseSeeInvisible12);

        var shadowsInvisibiityConditionDefinition = ConditionDefinitionBuilder
            .Create("WarlockConditionShadowsSpecial", DefinitionBuilder.CENamespaceGuid)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetGuiPresentationNoContent()
            .SetFeatures(WarlockSubclassMoonLitPatron.InvisibilityFeature)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var Unlit = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Unlit,
            condition = shadowsInvisibiityConditionDefinition
        };

        var Dim = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Dim, condition = shadowsInvisibiityConditionDefinition
        };

        var Darkness = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Darkness,
            condition = shadowsInvisibiityConditionDefinition
        };

        var OneWithShadowsLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create("OneWithShadowsLightAffinity", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(Unlit)
            .AddToDB();

        var OneWithShadowsLightAffinityStrong = FeatureDefinitionLightAffinityBuilder
            .Create("OneWithShadowsLightAffinityStrong", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(Dim)
            .AddLightingEffectAndCondition(Darkness)
            .AddToDB();

        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadows"]).FeatureSet.Add(
            OneWithShadowsLightAffinity);
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadows"]).Validators.SetRange(
            () => !Global.ActiveLevelUpHeroHasSubclass("MoonLit")
                ? null
                : "Requirement/&WarlockRequiresNoMoonLit"
        );

        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadowsStronger"]).FeatureSet
            .Add(OneWithShadowsLightAffinityStrong);
        ((FeatureDefinitionFeatureSetWithPreRequisites)EldritchInvocations["OneWithShadowsStronger"]).Validators
            .SetRange(() => Global.ActiveLevelUpHeroHasFeature(OneWithShadowsLightAffinity)
                ? null
                : "Requirement/&WarlockRequiresOneWithShadows"
            );

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

internal class WarlockClassHolder : IClassHoldingFeature
{
    public CharacterClassDefinition Class => Warlock.ClassWarlock;
}
