using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRegenerations;

namespace SolastaUnfinishedBusiness.Models;

internal static class SrdAndHouseRulesContext
{
    private const string InvisibleStalkerSubspellName = "ConjureElementalInvisibleStalker";

    internal static readonly HashSet<MonsterDefinition> ConjuredMonsters =
    [
        ConjuredOneBeastTiger_Drake,
        ConjuredTwoBeast_Direwolf,
        ConjuredFourBeast_BadlandsSpider,
        ConjuredEightBeast_Wolf,

        // Conjure minor elemental (4)
        SkarnGhoul, // CR 2
        WindSnake, // CR 2
        Fire_Jester, // CR 1

        // Conjure woodland beings (4) - not implemented

        // Conjure elemental (5)
        Air_Elemental, // CR 5
        Fire_Elemental, // CR 5
        Earth_Elemental, // CR 5

        InvisibleStalker, // CR 6

        // Conjure fey (6)
        FeyGiantApe, // CR 6
        FeyGiant_Eagle, // CR 5
        FeyBear, // CR 4
        Green_Hag, // CR 3
        FeyWolf, // CR 2
        FeyDriad
    ];

    private static readonly Dictionary<string, TagsDefinitions.Criticity> Tags = [];

    private static readonly List<MonsterDefinition> MonstersThatEmitLight =
    [
        CubeOfLight,
        Fire_Elemental,
        Fire_Jester,
        Fire_Osprey,
        Fire_Spider
    ];

    internal static readonly FeatureDefinitionActionAffinity ActionAffinityConditionBlind =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityConditionBlind")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.AttackOpportunity)
            .AddToDB();

    private static SpellDefinition ConjureElementalInvisibleStalker { get; set; }

    internal static void LateLoad()
    {
        BuildConjureElementalInvisibleStalker();
        LoadAfterRestIdentify();
        LoadAllowTargetingSelectionWhenCastingChainLightningSpell();
        LoadSenseNormalVisionRangeMultiplier();
        SwitchAddBleedingToLesserRestoration();
        SwitchAllowClubsToBeThrown();
        SwitchChangeSleetStormToCube();
        SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        SwitchConditionBlindedShouldNotAllowOpportunityAttack();
        SwitchDruidAllowMetalArmor();
        SwitchEldritchBlastRange();
        SwitchEnableUpcastConjureElementalAndFey();
        SwitchFilterOnHideousLaughter();
        SwitchFullyControlConjurations();
        SwitchAllowBladeCantripsToUseReach();
        SwitchHastedCasing();
        SwitchMagicStaffFoci();
        SwitchAllowTargetingSelectionWhenCastingChainLightningSpell();
        SwitchOfficialFoodRationsWeight();
        SwitchOneDndHealingSpellsBuf();
        SwitchRecurringEffectOnEntangle();
        SwitchRingOfRegenerationHealRate();
        SwitchSchoolRestrictionsFromShadowCaster();
        SwitchSchoolRestrictionsFromSpellBlade();
        SwitchUniversalSylvanArmorAndLightbringer();
        SwitchUseHeightOneCylinderEffect();
        NoTwinnedBladeCantrips();
    }

    private static void LoadSenseNormalVisionRangeMultiplier()
    {
        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision24")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision24")
                .SetSense(SenseMode.Type.NormalVision, 24)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision48")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision48")
                .SetSense(SenseMode.Type.NormalVision, 48)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();
    }

    internal static void SwitchSchoolRestrictionsFromShadowCaster()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromShadowCaster)
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.SetRange(
                SchoolAbjuration,
                SchoolDivination,
                SchoolIllusion,
                SchoolNecromancy);
        }
    }

    internal static void SwitchSchoolRestrictionsFromSpellBlade()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromSpellBlade)
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.SetRange(
                SchoolConjuration,
                SchoolEnchantement,
                SchoolEvocation,
                SchoolTransmutation);
        }
    }

    internal static void AddLightSourceIfNeeded(GameLocationCharacter gameLocationCharacter)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
        {
            return;
        }

        if (!MonstersThatEmitLight.Contains(rulesetCharacterMonster.MonsterDefinition))
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacterMonster, "ShouldEmitLightFromMonster");
    }

    internal static void AddLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (rulesetCondition == null || !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter)
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacter, "ShouldEmitLightFromCondition");
    }

    private static void AddLightSource(
        GameLocationCharacter gameLocationCharacter,
        RulesetCharacter rulesetCharacter,
        string name)
    {
        var lightSourceForm = Shine.EffectDescription.EffectForms[0].LightSourceForm;

        rulesetCharacter.PersonalLightSource?.Unregister();
        rulesetCharacter.PersonalLightSource = new RulesetLightSource(
            lightSourceForm.Color,
            2,
            4,
            lightSourceForm.GraphicsPrefabAssetGUID,
            LightSourceType.Basic,
            name,
            rulesetCharacter.Guid);

        rulesetCharacter.PersonalLightSource.Register(true);

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .AddCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);
    }

    internal static void RemoveLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (rulesetCondition == null ||
            !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter ||
            rulesetCharacter.PersonalLightSource == null) // if using extinguish fire light source will come null here
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .RemoveCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);

        rulesetCharacter.PersonalLightSource.Unregister();
        rulesetCharacter.PersonalLightSource = null;
    }

    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        var allowedClasses = new[] { Wizard, Sorcerer, Warlock };

        GreenmageArmor.RequiredAttunementClasses.AddRange(allowedClasses);
        WizardClothes_Alternate.RequiredAttunementClasses.AddRange(allowedClasses);
    }

    internal static void SwitchDruidAllowMetalArmor()
    {
        var active = Main.Settings.AllowDruidToWearMetalArmor;

        if (active)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchMagicStaffFoci()
    {
        if (!Main.Settings.MakeAllMagicStaveArcaneFoci)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon) // WeaponDescription could be null
                     .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                     .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
        {
            item.IsFocusItem = true;
            item.FocusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;
        }
    }

    internal static void SwitchConditionBlindedShouldNotAllowOpportunityAttack()
    {
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            ConditionDefinitions.ConditionBlinded.Features.TryAdd(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.TryAdd(ActionAffinityConditionBlind);
        }
        else
        {
            ConditionDefinitions.ConditionBlinded.Features.Remove(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.Remove(ActionAffinityConditionBlind);
        }
    }

    private static void LoadAllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        ChainLightning.AddCustomSubFeatures(new FilterTargetingCharacterChainLightning());
    }

    internal static void SwitchAllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        var spell = ChainLightning.EffectDescription;

        if (Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell)
        {
            spell.targetType = TargetType.IndividualsUnique;
            spell.targetParameter = 4;
            spell.effectAdvancement.additionalTargetsPerIncrement = 1;
        }
        else
        {
            spell.targetType = TargetType.ArcFromIndividual;
            spell.targetParameter = 3;
            spell.effectAdvancement.additionalTargetsPerIncrement = 0;
        }
    }

    internal static void SwitchOfficialFoodRationsWeight()
    {
        var foodSrdWeight = Food_Ration;
        var foodForagedSrdWeight = Food_Ration_Foraged;

        if (Main.Settings.UseOfficialFoodRationsWeight)
        {
            foodSrdWeight.weight = 2.0f;
            foodForagedSrdWeight.weight = 2.0f;
        }
        else
        {
            foodSrdWeight.weight = 3.0f;
            foodForagedSrdWeight.weight = 3.0f;
        }
    }

    internal static void SwitchOneDndHealingSpellsBuf()
    {
        var dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        HealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 5 : 3;

        MassCureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
    }

    internal static void SwitchFilterOnHideousLaughter()
    {
        HideousLaughter.effectDescription.restrictedCreatureFamilies.Clear();

        if (!Main.Settings.RemoveHumanoidFilterOnHideousLaughter)
        {
            HideousLaughter.effectDescription.restrictedCreatureFamilies.Add("Humanoid");
        }
    }

    internal static void SwitchRecurringEffectOnEntangle()
    {
        if (Main.Settings.RemoveRecurringEffectOnEntangle)
        {
            // Remove recurring effect on Entangle (as per SRD, any creature is only affected at cast time)
            Entangle.effectDescription.recurrentEffect = RecurrentEffect.OnActivation;
            Entangle.effectDescription.EffectForms[2].canSaveToCancel = false;
            ConditionRestrainedByEntangle.Features.Add(FeatureDefinitionActionAffinitys.ActionAffinityGrappled);
        }
        else
        {
            Entangle.effectDescription.recurrentEffect =
                RecurrentEffect.OnActivation | RecurrentEffect.OnTurnEnd | RecurrentEffect.OnEnter;
            Entangle.effectDescription.EffectForms[2].canSaveToCancel = true;
            ConditionRestrainedByEntangle.Features.Remove(FeatureDefinitionActionAffinitys.ActionAffinityGrappled);
        }
    }

    internal static void SwitchChangeSleetStormToCube()
    {
        var sleetStormEffect = SleetStorm.EffectDescription;

        if (Main.Settings.ChangeSleetStormToCube)
        {
            // Set to Cube side 8, default height
            sleetStormEffect.targetType = TargetType.Cube;
            sleetStormEffect.targetParameter = 8;
            sleetStormEffect.targetParameter2 = 0;
        }
        else
        {
            // Restore to cylinder radius 4, height 3
            sleetStormEffect.targetType = TargetType.Cylinder;
            sleetStormEffect.targetParameter = 4;
            sleetStormEffect.targetParameter2 = 3;
        }
    }

    internal static void SwitchEldritchBlastRange()
    {
        EldritchBlast.effectDescription.rangeParameter = Main.Settings.FixEldritchBlastRange ? 24 : 16;
    }

    internal static void SwitchRingOfRegenerationHealRate()
    {
        var ringDefinition = RegenerationRing;

        if (Main.Settings.FixRingOfRegenerationHealRate)
        {
            // Heal by 1 hp per 3 minutes which is roughly the same as 
            // RAW of 1d6 (avg 3.5) every 10 minutes.
            ringDefinition.tickType = DurationType.Minute;
            ringDefinition.tickNumber = 3;
            ringDefinition.diceNumber = 1;
        }
        else
        {
            ringDefinition.tickType = DurationType.Round;
            ringDefinition.tickNumber = 1;
            ringDefinition.diceNumber = 2;
        }

        ringDefinition.dieType = DieType.D1;
    }

    internal static void SwitchUseHeightOneCylinderEffect()
    {
        // always applicable
        ClearTargetParameter2ForTargetTypeCube();

        // Change SpikeGrowth to be height 1 round cylinder/sphere
        var spikeGrowthEffect = SpikeGrowth.EffectDescription;

        spikeGrowthEffect.targetParameter = 4;

        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Set to Cylinder radius 4, height 1
            spikeGrowthEffect.targetType = TargetType.Cylinder;
            spikeGrowthEffect.targetParameter2 = 1;
        }
        else
        {
            // Restore default of Sphere radius 4
            spikeGrowthEffect.targetType = TargetType.Sphere;
            spikeGrowthEffect.targetParameter2 = 0;
        }

        // Spells with TargetType.Cube and defaults values of (tp, tp2)
        // Note that tp2 should be 0 for Cube and is ignored in game.
        // BlackTentacles: (4, 2)
        // Entangle: (4, 1)
        // FaerieFire: (4, 2)
        // FlamingSphere: (3, 2) <- a flaming sphere is a cube?
        // Grease: (2, 2)
        // HypnoticPattern: (6, 2)
        // Slow: (8, 2)
        // Thunderwave: (3, 2)

        // Change Black Tentacles, Entangle, Grease to be height 1 square cylinder/cube
        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Setting height switches to square cylinder (if originally cube)
            SetHeight(BlackTentacles, 1);
            SetHeight(Entangle, 1);
            SetHeight(Grease, 1);
        }
        else
        {
            // Setting height to 0 restores original behaviour
            SetHeight(BlackTentacles, 0);
            SetHeight(Entangle, 0);
            SetHeight(Grease, 0);
        }

        return;

        static void SetHeight([NotNull] IMagicEffect spellDefinition, int height)
        {
            spellDefinition.EffectDescription.targetParameter2 = height;
        }

        static void ClearTargetParameter2ForTargetTypeCube()
        {
            foreach (var sd in DatabaseRepository
                         .GetDatabase<SpellDefinition>()
                         .Where(sd =>
                             sd.EffectDescription.TargetType is TargetType.Cube
                                 or TargetType.CubeWithOffset))
            {
                // TargetParameter2 is not used by TargetType.Cube but has random values assigned.
                // We are going to use it to create a square cylinder with height so set to zero for all spells with TargetType.Cube.
                sd.EffectDescription.targetParameter2 = 0;
            }
        }
    }

    internal static void SwitchAllowBladeCantripsToUseReach()
    {
        var db = DatabaseRepository.GetDatabase<SpellDefinition>();
        var cantrips = new List<string> { "BoomingBlade", "ResonatingStrike", "SunlightBlade" };

        foreach (var bladeCantrip in db.Where(x => cantrips.Contains(x.Name)))
        {
            var text = Main.Settings.AllowBladeCantripsToUseReach ? "Feedback/&WithinReach" : "Feedback/&Within5Ft";

            bladeCantrip.GuiPresentation.Description = Gui.Format($"Spell/&{bladeCantrip.Name}Description", text);
        }
    }

    internal static void SwitchHastedCasing()
    {
        var restrictedActions = FeatureDefinitionAdditionalActions.AdditionalActionHasted.RestrictedActions;
        if (Main.Settings.AllowHasteCasting)
        {
            restrictedActions.TryAdd(Id.CastMain);
        }
        else
        {
            restrictedActions.RemoveAll(id => id == Id.CastMain);
        }
    }

    internal static void SwitchAddBleedingToLesserRestoration()
    {
        var cf = LesserRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cf != null)
        {
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cf.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cf.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }

        var cfg = GreaterRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cfg == null)
        {
            return;
        }

        // NOTE: using the same setting as for Lesser Restoration for compatibility
        if (Main.Settings.AddBleedingToLesserRestoration)
        {
            cfg.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
        }
        else
        {
            cfg.ConditionForm.ConditionsList.Remove(ConditionBleeding);
        }
    }

    /// <summary>
    ///     Allow conjurations to fully controlled party members instead of AI controlled.
    /// </summary>
    private static void BuildConjureElementalInvisibleStalker()
    {
        ConjureElementalInvisibleStalker = SpellDefinitionBuilder
            .Create(ConjureElementalFire, InvisibleStalkerSubspellName)
            .SetOrUpdateGuiPresentation("Spell/&ConjureElementalInvisibleStalkerTitle",
                "Spell/&ConjureElementalDescription")
            .AddToDB();

        var summonForm = ConjureElementalInvisibleStalker
            .EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Summon)?.SummonForm;

        if (summonForm != null)
        {
            summonForm.monsterDefinitionName = InvisibleStalker.Name;
        }
    }

    internal static void SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity()
    {
        foreach (var featureSet in DatabaseRepository.GetDatabase<FeatureDefinitionFeatureSet>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);

                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity);

                // ReSharper disable once InvertIf
                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledInsteadOfFrozenImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x =>
                    x.FeatureDefinition == ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }
        }
    }

    internal static void SwitchFullyControlConjurations()
    {
        foreach (var conjuredMonster in ConjuredMonsters)
        {
            conjuredMonster.fullyControlledWhenAllied = Main.Settings.FullyControlConjurations;
        }
    }

    internal static void SwitchAllowClubsToBeThrown()
    {
        var db = DatabaseRepository.GetDatabase<ItemDefinition>();

        foreach (var itemDefinition in db
                     .Where(x => x.IsWeapon &&
                                 x.WeaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.ClubType))
        {
            if (Main.Settings.AllowClubsToBeThrown)
            {
                itemDefinition.WeaponDescription.WeaponTags.Add(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 10;
            }
            else
            {
                itemDefinition.WeaponDescription.WeaponTags.Remove(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 5;
            }
        }
    }

    internal static void SwitchEnableUpcastConjureElementalAndFey()
    {
        if (!Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            ConjureElemental.SubspellsList.Remove(ConjureElementalInvisibleStalker);

            return;
        }

        ConfigureAdvancement(ConjureFey);
        ConfigureAdvancement(ConjureElemental);
        ConfigureAdvancement(ConjureMinorElementals);
        ConjureElemental.SubspellsList.Add(ConjureElementalInvisibleStalker);

        return;

        // Set advancement at spell level, not sub-spell
        static void ConfigureAdvancement([NotNull] IMagicEffect spell)
        {
            var advancement = spell.EffectDescription.EffectAdvancement;

            advancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;
            advancement.additionalSpellLevelPerIncrement = 1;
        }
    }

    private static void LoadAfterRestIdentify()
    {
        const string AfterRestIdentifyName = "PowerAfterRestIdentify";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("RestActivityLongRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        var afterRestIdentifyCondition = ConditionDefinitionBuilder
            .Create("AfterRestIdentify")
            .SetGuiPresentation(Category.Condition)
            .AddCustomSubFeatures(OnConditionAddedOrRemovedIdentifyItems.Mark)
            .AddToDB();

        FeatureDefinitionPowerBuilder
            .Create(AfterRestIdentifyName)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(CanIdentifyOnRest.Mark)
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .SetDurationData(
                        DurationType.Minute,
                        1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                afterRestIdentifyCondition,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();
    }

    internal static bool IsAttackModeInvalid(RulesetCharacter character, RulesetAttackMode mode)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        return IsHandCrossbowUseInvalid(mode.sourceObject as RulesetItem, hero,
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand),
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand));
    }

    internal static bool IsHandCrossbowUseInvalid(
        RulesetItem item,
        RulesetCharacterHero hero,
        RulesetItem main,
        RulesetItem off)
    {
        if (Main.Settings.IgnoreHandXbowFreeHandRequirements)
        {
            return false;
        }

        if (item == null || hero == null)
        {
            return false;
        }

        Tags.Clear();
        item.FillTags(Tags, hero, true);

        if (!Tags.ContainsKey(TagsDefinitions.WeaponTagAmmunition) ||
            Tags.ContainsKey(TagsDefinitions.WeaponTagTwoHanded))
        {
            return false;
        }

        if (main == item && off != null)
        {
            return true;
        }

        return off == item
               && main != null
               && main.ItemDefinition.WeaponDescription?.WeaponType != WeaponTypeDefinitions.UnarmedStrikeType.Name;
    }

    internal static void HandleSmallRaces(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons)
        {
            return;
        }

        var hero = evaluationParams.attacker.RulesetCharacter.GetOriginalHero();

        if (hero?.RaceDefinition.SizeDefinition == CharacterSizeDefinitions.Small &&
            evaluationParams.attackMode is { SourceDefinition: ItemDefinition { IsWeapon: true } itemDefinition } &&
            itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagHeavy))
        {
            evaluationParams.attackModifier.AttackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.Unknown, "Feedback/&SmallRace", null));
        }
    }

    private static void NoTwinnedBladeCantrips()
    {
        MetamagicOptionDefinitions.MetamagicTwinnedSpell.AddCustomSubFeatures(NoTwinned.Validator);
    }

    private sealed class FilterTargetingCharacterChainLightning : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var selectedTargets = __instance.SelectionService.SelectedTargets;

            if (selectedTargets.Count == 0)
            {
                return true;
            }

            var firstTarget = selectedTargets[0];

            var isValid = firstTarget.IsWithinRange(target, 6);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&SecondTargetNotWithinRange");
            }

            return isValid;
        }
    }

    private sealed class CanIdentifyOnRest : IValidatePowerUse
    {
        private CanIdentifyOnRest()
        {
        }

        public static CanIdentifyOnRest Mark { get; } = new();

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            //does this work properly for wild-shaped heroes?
            if (character is not RulesetCharacterHero hero)
            {
                return false;
            }

            return Main.Settings.IdentifyAfterRest && hero.HasNonIdentifiedItems();
        }
    }

    private sealed class OnConditionAddedOrRemovedIdentifyItems : IOnConditionAddedOrRemoved
    {
        public static OnConditionAddedOrRemovedIdentifyItems Mark { get; } = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.GetOriginalHero()?.AutoIdentifyInventoryItems();
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    internal sealed class NoTwinned
    {
        public static readonly ValidateMetamagicApplication Validator =
            (RulesetCharacter _, RulesetEffectSpell spell, MetamagicOptionDefinition _, ref bool result,
                ref string failure) =>
            {
                if (!spell.SpellDefinition.HasSubFeatureOfType<NoTwinned>())
                {
                    return;
                }

                result = false;
                failure = "Failure/&FailureFlagInvalidSingleTarget";
            };

        public static NoTwinned Mark { get; } = new();
    }
}
