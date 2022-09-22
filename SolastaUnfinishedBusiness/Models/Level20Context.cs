using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20Context
{
    private const string PowerSorcerousRestorationName = "PowerSorcerousRestoration";

    public const int MaxSpellLevel = 9;

    public const int ModMaxLevel = 20;
    public const int GameMaxLevel = 12;
    public const int GameFinalMaxLevel = 16;

    public const int ModMaxExperience = 355000;
    public const int GameMaxExperience = 100000;

    [NotNull]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> Level20Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (!Main.Settings.EnableLevel20)
        {
            return code;
        }

        code
            .FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GameFinalMaxLevel)
            .ForEach(x => x.operand = ModMaxLevel);

        code
            .FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GameMaxLevel)
            .ForEach(x => x.operand = ModMaxLevel);

        return code;
    }

    internal static void Load()
    {
        BarbarianLoad();
        BardLoad();
        ClericLoad();
        DruidLoad();
        FighterLoad();
        PaladinLoad();
        RangerLoad();
        RogueLoad();
        SorcererLoad();
        WarlockLoad();
        WizardLoad();
        MartialSpellBladeLoad();
        RoguishShadowcasterLoad();
        TraditionLightLoad();
    }

    internal static void LateLoad()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = typeof(Level20Context).GetMethod("Level20Transpiler");
        // these are currently the hard-coded levels on below methods
        var methods = new[]
        {
            typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding), // 12
            typeof(CharacterBuildingManager).GetMethod("CreateCharacterFromTemplate"), // 16
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding), // 12
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"), // 16
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"), // 12
            typeof(InvocationSubPanel).GetMethod("SetState"), // 12
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"), // 16
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"), // 12, 16
            typeof(RulesetEntity).GetMethod("SerializeElements") // 12, 16
        };

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error("cannot fully patch Level 20");
            }
        }
    }

    private static void BarbarianLoad()
    {
        Barbarian.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AttributeModifierBarbarianBrutalCriticalAdd, 17),
            // TODO 18: Barbarian Indomitable Might
            // new(FeatureDefinitionIndomitableMightBuilder.FeatureDefinitionIndomitableMight, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Barbarian Primal Champion
            // new(FeatureDefinitionPrimalChampionBuilder.FeatureDefinitionPrimalChampion, 20)
        });
    }

    private static void BardLoad()
    {
        Bard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureDefinitionPointPoolBuilder
                    .Create(PointPoolBardMagicalSecrets14, "PointPoolBardMagicalSecrets18")
                    .AddToDB(),
                18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Bard Superior Inspiration
        });

        CastSpellBard.SlotsPerLevels.SetRange(SharedSpellsContext.FullCastingSlots);
        CastSpellBard.ReplacedSpells.SetRange(SharedSpellsContext.FullCasterReplacedSpells);
    }

    private static void ClericLoad()
    {
        var effectPowerClericTurnUndead17 = new EffectDescription();

        effectPowerClericTurnUndead17.Copy(PowerClericTurnUndead14.EffectDescription);
        effectPowerClericTurnUndead17.EffectForms[0].KillForm.challengeRating = 4;

        Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureDefinitionPowerBuilder
                    .Create(PowerClericTurnUndead14, "PowerClericTurnUndead17")
                    .SetEffectDescription(effectPowerClericTurnUndead17)
                    .AddToDB(),
                17),
            new(AttributeModifierClericChannelDivinityAdd, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // Solasta handles divine intervention on subclasses below
        });

        CastSpellCleric.SlotsPerLevels.SetRange(SharedSpellsContext.FullCastingSlots);
        CastSpellCleric.ReplacedSpells.SetRange(SharedSpellsContext.EmptyReplacedSpells);

        var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
            .Create(
                PowerClericDivineInterventionCleric,
                "PowerClericDivineInterventionImprovementCleric")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

        var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
            .Create(
                PowerClericDivineInterventionPaladin,
                "PowerClericDivineInterventionImprovementPaladin")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

        var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
            .Create(
                PowerClericDivineInterventionWizard,
                "PowerClericDivineInterventionImprovementWizard")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionWizard)
            .AddToDB();

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionPaladin,
                20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
    }

    private static void DruidLoad()
    {
        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Druid Beast Spells
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Druid Arch Druid
        });

        CastSpellDruid.SlotsPerLevels.SetRange(SharedSpellsContext.FullCastingSlots);
        CastSpellDruid.ReplacedSpells.SetRange(SharedSpellsContext.EmptyReplacedSpells);
    }

    private static void FighterLoad()
    {
        Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureDefinitionPowerBuilder
                    .Create(PowerFighterActionSurge, "PowerFighterActionSurge2")
                    .SetFixedUsesPerRecharge(2)
                    .SetOverriddenPower(PowerFighterActionSurge)
                    .AddToDB(),
                17),
            new(AttributeModifierFighterIndomitableAdd1, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(AttributeModifierFighterExtraAttack, 20)
        });
    }

    private static void PaladinLoad()
    {
        var effectPowerPaladinAuraOfCourage18 = new EffectDescription();

        effectPowerPaladinAuraOfCourage18.Copy(PowerPaladinAuraOfCourage.EffectDescription);
        effectPowerPaladinAuraOfCourage18.targetParameter = 6;
        effectPowerPaladinAuraOfCourage18.rangeParameter = 0;
        effectPowerPaladinAuraOfCourage18.requiresTargetProximity = false;

        var effectPowerPaladinAuraOfProtection18 = new EffectDescription();

        effectPowerPaladinAuraOfProtection18.Copy(PowerPaladinAuraOfProtection.EffectDescription);
        effectPowerPaladinAuraOfProtection18.targetParameter = 6;
        effectPowerPaladinAuraOfProtection18.rangeParameter = 0;
        effectPowerPaladinAuraOfProtection18.requiresTargetProximity = false;

        Paladin.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(FeatureDefinitionPowerBuilder
                    .Create(PowerPaladinAuraOfCourage, "PowerPaladinAuraOfCourage18")
                    .SetGuiPresentation(Category.Feature)
                    .SetEffectDescription(effectPowerPaladinAuraOfCourage18)
                    .SetOverriddenPower(PowerPaladinAuraOfCourage)
                    .AddToDB(),
                18),
            new FeatureUnlockByLevel(FeatureDefinitionPowerBuilder
                    .Create(PowerPaladinAuraOfProtection, "PowerPaladinAuraOfProtection18")
                    .SetGuiPresentation(Category.Feature)
                    .SetEffectDescription(effectPowerPaladinAuraOfProtection18)
                    .SetOverriddenPower(PowerPaladinAuraOfCourage)
                    .AddToDB(),
                18),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
        );

        AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17,
                SpellsList = new List<SpellDefinition>
                {
                    // Commune,
                    FlameStrike
                }
            });

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { FlameStrike }
            });

        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { WallOfForce, HoldMonster }
            });

        CastSpellPaladin.SlotsPerLevels.SetRange(SharedSpellsContext.HalfCastingSlots);
        CastSpellPaladin.ReplacedSpells.SetRange(SharedSpellsContext.EmptyReplacedSpells);
    }

    private static void RangerLoad()
    {
        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureDefinitionSenseBuilder
                    .Create(SenseSeeInvisible12, "SenseRangerFeralSenses")
                    .SetGuiPresentation(Category.Feature)
                    .SetSenseRange(6)
                    .AddToDB(),
                18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Ranger Foe Slayer
        });

        CastSpellRanger.SlotsPerLevels.SetRange(SharedSpellsContext.HalfCastingSlots);
        CastSpellRanger.ReplacedSpells.SetRange(SharedSpellsContext.HalfCasterReplacedSpells);
    }

    private static void RogueLoad()
    {
        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Rogue Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Rogue Stroke of Luck
        });
    }

    private static void SorcererLoad()
    {
        var powerSorcerousRestoration = new EffectFormBuilder()
            .CreatedByCharacter()
            .SetSpellForm(9)
            .Build();

        powerSorcerousRestoration.SpellSlotsForm.type = SpellSlotsForm.EffectType.GainSorceryPoints;
        powerSorcerousRestoration.SpellSlotsForm.sorceryPointsGain = 4;

        _ = RestActivityBuilder.RestActivityRestoration;

        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PointPoolSorcererAdditionalMetamagic, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(FeatureDefinitionPowerBuilder
                    .Create(PowerSorcerousRestorationName)
                    .SetGuiPresentation("PowerSorcerousRestoration", Category.Feature)
                    .SetFixedUsesPerRecharge(1)
                    .SetActivationTime(RuleDefinitions.ActivationTime.Rest)
                    .SetUsesAbilityScoreName(AttributeDefinitions.Charisma)
                    .SetCostPerUse(1)
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetEffectDescription(new EffectDescriptionBuilder()
                        .SetEffectForms(powerSorcerousRestoration)
                        .SetTargetingData(
                            RuleDefinitions.Side.Ally,
                            RuleDefinitions.RangeType.Self,
                            1,
                            RuleDefinitions.TargetType.Self)
                        .SetParticleEffectParameters(PowerWizardArcaneRecovery.EffectDescription
                            .EffectParticleParameters)
                        .Build())
                    .AddToDB(),
                20)
        });

        CastSpellSorcerer.SlotsPerLevels.SetRange(SharedSpellsContext.FullCastingSlots);
        CastSpellSorcerer.ReplacedSpells.SetRange(SharedSpellsContext.FullCasterReplacedSpells);
        CastSpellSorcerer.KnownSpells.SetRange(SharedSpellsContext.SorcererKnownSpells);
    }

    private static void WarlockLoad()
    {
        Warlock.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureDefinitionPointPoolBuilder
                    .Create(PointPoolWarlockMysticArcanum8, "PointPoolWarlockMysticArcanum9")
                    .SetGuiPresentation(
                        "Feature/&PointPoolWarlockMysticArcanum9Title",
                        "Feature/&PointPoolWarlockMysticArcanumDescription")
                    .AddToDB(),
                18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(FeatureDefinitionPowerBuilder
                    .Create(PowerWizardArcaneRecovery, "PowerWarlockEldritchMaster")
                    .SetGuiPresentation(Category.Feature)
                    .SetActivationTime(RuleDefinitions.ActivationTime.Minute1)
                    .AddToDB(),
                20)
        });

        CastSpellWarlock.KnownSpells.SetRange(SharedSpellsContext.WarlockKnownSpells);
    }

    private static void WizardLoad()
    {
        Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Spell Mastery
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Signature Spells
        });

        CastSpellWizard.SlotsPerLevels.SetRange(SharedSpellsContext.FullCastingSlots);
        CastSpellWizard.ReplacedSpells.SetRange(SharedSpellsContext.EmptyReplacedSpells);
    }

    private static void MartialSpellBladeLoad()
    {
        CastSpellMartialSpellBlade.SlotsPerLevels.SetRange(SharedSpellsContext.OneThirdCastingSlots);
        CastSpellMartialSpellBlade.ReplacedSpells.SetRange(SharedSpellsContext.OneThirdCasterReplacedSpells);
    }

    private static void RoguishShadowcasterLoad()
    {
        CastSpellShadowcaster.SlotsPerLevels.SetRange(SharedSpellsContext.OneThirdCastingSlots);
        CastSpellShadowcaster.ReplacedSpells.SetRange(SharedSpellsContext.OneThirdCasterReplacedSpells);
    }

    private static void TraditionLightLoad()
    {
        CastSpellTraditionLight.SlotsPerLevels.SetRange(SharedSpellsContext.OneThirdCastingSlots);
        CastSpellTraditionLight.ReplacedSpells.SetRange(SharedSpellsContext.OneThirdCasterReplacedSpells);
    }

    private sealed class RestActivityBuilder : RestActivityDefinitionBuilder
    {
        private const string SorcerousRestorationName = "SorcerousRestoration";

        // An alternative pattern for lazily creating definition.
        private static RestActivityDefinition _restActivityRestoration;

        private RestActivityBuilder(string name) : base(
            DatabaseHelper.RestActivityDefinitions.ArcaneRecovery, name, GuidHelper.Create(CENamespaceGuid, name).ToString())
        {
            Definition.stringParameter = PowerSorcerousRestorationName;
        }

        // get only property
        public static RestActivityDefinition RestActivityRestoration =>
            _restActivityRestoration ??=
                new RestActivityBuilder(SorcerousRestorationName)
                    .SetGuiPresentation("PowerSorcerousRestoration", Category.Feature)
                    .AddToDB();
    }
}
