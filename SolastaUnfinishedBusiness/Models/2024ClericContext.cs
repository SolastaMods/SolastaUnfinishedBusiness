using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private const string BlessedStrikes = "BlessedStrikes";

    private static readonly FeatureDefinitionPointPool PointPoolClericThaumaturgeCantrip =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolClericThaumaturgeCantrip")
            .SetGuiPresentation(Category.Feature)
            .SetSpellOrCantripPool
                (HeroDefinitions.PointsPoolType.Cantrip, 1, SpellListDefinitions.SpellListCleric, "Thaumaturge")
            .AddCustomSubFeatures(new ModifyAbilityCheckThaumaturge())
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetClericDivineOrder = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetClericDivineOrder")
        .SetGuiPresentation(Category.Feature)
        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
        .SetFeatureSet(
            PointPoolClericThaumaturgeCantrip,
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetClericProtector")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyClericProtectorArmor")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
                        .AddToDB(),
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyClericProtectorWeapons")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
                        .AddToDB())
                .AddToDB())
        .AddToDB();

    private static readonly FeatureDefinition FeatureClericSearUndead = FeatureDefinitionBuilder
        .Create("FeatureClericSearUndead")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerClericDivineSpark = FeatureDefinitionPowerBuilder
        .Create("PowerClericDivineSpark")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerDivineSpark", Resources.PowerDivineSpark, 256, 128))
        .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
        .AddToDB();

    private static readonly EffectForm SearUndeadDamageForm = EffectFormBuilder
        .Create()
        .HasSavingThrow(EffectSavingThrowType.Negates)
        .SetDamageForm(DamageTypeRadiant, 0, DieType.D8)
        .Build();

    private static readonly FeatureDefinitionFeatureSet FeatureSetClericBlessedStrikes =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetClericBlessedStrikes")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerClericImprovedBlessedStrikes = FeatureDefinitionPowerBuilder
        .Create("PowerClericImprovedBlessedStrikes")
        .SetGuiPresentation(Category.Feature, PowerPaladinLayOnHands)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.UntilAnyRest)
                .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetTempHpForm()
                        .Build())
                .SetParticleEffectParameters(PowerPaladinLayOnHands)
                .Build())
        .AddCustomSubFeatures(new ModifyEffectDescriptionPowerClericImprovedBlessedStrikes())
        .AddToDB();

    private static readonly ConditionDefinition ConditionClericImprovedBlessedStrikes = ConditionDefinitionBuilder
        .Create("ConditionClericImprovedBlessedStrikes")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(PowerClericImprovedBlessedStrikes)
        .AddCustomSubFeatures(new AddUsablePowersFromCondition())
        .AddToDB();

    private static readonly FeatureDefinition FeatureClericImprovedBlessedStrikes =
        FeatureDefinitionBuilder
            .Create("FeatureClericImprovedBlessedStrikes")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorFeatureClericImprovedBlessedStrikes())
            .AddToDB();

    private sealed class CustomBehaviorFeatureClericImprovedBlessedStrikes : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (actualEffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage) &&
                rulesetEffect.SourceDefinition is SpellDefinition { SpellLevel: 0 } &&
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionClericImprovedBlessedStrikes.Name))
            {
                rulesetAttacker.InflictCondition(
                    ConditionClericImprovedBlessedStrikes.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    FactionDefinitions.Party.Name,
                    1,
                    ConditionClericImprovedBlessedStrikes.Name,
                    0,
                    0,
                    0);
            }

            yield break;
        }
    }

    private static readonly Dictionary<string, List<string>> AdditionalDamageBlessedStrikes = new()
    {
        { "DomainMischief", ["PowerClericBlessedStrikesDamagePsychic"] },
        {
            "DomainNature", [
                "PowerClericBlessedStrikesDamageCold" //, //
                //"PowerClericBlessedStrikesDamageFire", //
                //"PowerClericBlessedStrikesDamageLighting" //
            ]
        },
        { "DomainSmith", ["PowerClericBlessedStrikesDamageFire"] },
        { "DomainTempest", ["PowerClericBlessedStrikesDamageThunder"] }
    };

    private static void LoadClericBlessedStrikes()
    {
        var featurePotentSpellcasting = FeatureDefinitionBuilder
            .Create("FeatureClericBlessedStrikesPotentSpellcasting")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featurePotentSpellcasting.AddCustomSubFeatures(
            new ClassFeats.CustomBehaviorFeatPotentSpellcaster(featurePotentSpellcasting, Cleric));

        var damageTypes = new (string, IMagicEffect)[]
        {
            (DamageTypeRadiant, Sunburst), (DamageTypeNecrotic, VampiricTouch),
            // these are specific to some domains and not added on feature set but on demand before reaction
            (DamageTypeCold, ConeOfCold), // DomainNature
            (DamageTypeLightning, LightningBolt), // DomainNature
            (DamageTypeFire, FireBolt), // DomainNature and DomainSmith
            (DamageTypeThunder, Shatter), // DomainTempest
            (DamageTypePsychic, PowerMagebaneWarcry) // DomainMischief
        };

        var powers = new List<FeatureDefinitionPower>();
        var powerBlessedStrikes = FeatureDefinitionPowerBuilder
            .Create("PowerClericBlessedStrikes")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        powerBlessedStrikes.AddCustomSubFeatures(new CustomBehaviorBlessedStrikes(powerBlessedStrikes));

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var (damageType, effect) in damageTypes)
        {
            var additionalDamageBlessedStrikes = FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamageClericBlessedStrikes{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("DivineStrike")
                .SetDamageDice(DieType.D8, 1)
                .SetSpecificDamageType(damageType)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 7, 7)
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .SetAttackModeOnly()
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(ClassHolder.Cleric)
                .SetImpactParticleReference(effect)
                .AddToDB();

            var conditionBlessedStrikes = ConditionDefinitionBuilder
                .Create($"ConditionClericBlessedStrikes{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(additionalDamageBlessedStrikes)
                .AddToDB();

            var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");

            var powerDivineStrike = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerClericBlessedStrikes{damageType}")
                .SetGuiPresentation(
                    $"Tooltip/&Tag{damageType}Title",
                    Gui.Format("Feature/&PowerClericBlessedStrikesSubPowerDescription", damageTitle))
                .SetShowCasting(false)
                .SetSharedPool(ActivationTime.NoCost, powerBlessedStrikes)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBlessedStrikes))
                        .Build())
                .AddToDB();

            powerDivineStrike.GuiPresentation.hidden = true;

            powers.Add(powerDivineStrike);

            FeatureDefinitionPowerBuilder
                .Create($"FeatureClericAdditionalDamageGenericBlessed{damageType}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&FeatureClericAdditionalDamageGenericBlessedTitle", damageTitle),
                    Gui.Format("Feature/&FeatureClericAdditionalDamageGenericBlessedDescription", damageTitle))
                .AddToDB();
        }

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityBlessedStrikesToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.BlessedStrikesToggle)
            .AddToDB();

        var featureSetPrimalStrike = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetClericBlessedStrikesPrimalStrike")
            .SetGuiPresentation(powerBlessedStrikes.GuiPresentation)
            .SetFeatureSet(powerBlessedStrikes, actionAffinityToggle, powers[0], powers[1])
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerBlessedStrikes, false, powers);
        FeatureSetClericBlessedStrikes.FeatureSet.SetRange(featurePotentSpellcasting, featureSetPrimalStrike);
    }

    private static void LoadClericChannelDivinity()
    {
        var powerDivineSparkHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerClericDivineSparkHeal")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, PowerClericDivineSpark)
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetHealingForm(
                                HealingComputation.Dice, 0, DieType.D8, 1, false, HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(CureWounds)
                    .Build())
            .AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

        powerDivineSparkHeal.AddCustomSubFeatures(
            new ModifyEffectDescriptionPowerDivineSparkHeal(powerDivineSparkHeal));

        var powerDivineSparkDamageNecrotic = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerClericDivineSparkDamageNecrotic")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, PowerClericDivineSpark)
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (13, 2), (18, 3))
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D8)
                            .Build())
                    .SetCasterEffectParameters(FalseLife)
                    .SetImpactEffectParameters(PowerWightLordRetaliate)
                    .Build())
            .AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

        var powerDivineSparkDamageRadiant = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerClericDivineSparkDamageRadiant")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, PowerClericDivineSpark)
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (13, 2), (18, 3))
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                            .Build())
                    .SetCasterEffectParameters(ShadowArmor)
                    .SetImpactEffectParameters(PowerDomainBattleDecisiveStrike)
                    .Build())
            .AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(PowerClericDivineSpark, false,
            powerDivineSparkHeal, powerDivineSparkDamageNecrotic, powerDivineSparkDamageRadiant);
    }

    private static void LoadClericSearUndead()
    {
        PowerClericTurnUndead.EffectDescription.EffectForms.Insert(0, SearUndeadDamageForm);
        PowerClericTurnUndead.AddCustomSubFeatures(new ModifyEffectDescriptionPowerTurnUndead());
    }

    internal static void SwitchClericBlessedStrikes()
    {
        var domains = new[]
        {
            ("DomainDefiler", "AdditionalDamageDomainDefilerDivineStrike", string.Empty),
            ("DomainLife", "AdditionalDamageDomainLifeDivineStrike", string.Empty),
            ("DomainMischief", "AdditionalDamageDomainMischiefDivineStrike", DamageTypePsychic),
            ("DomainNature", "FeatureSetDomainNatureNatureStrikes", DamageTypeCold),
            ("DomainSmith", "AdditionalDamageDomainSmithDivineStrike", DamageTypeFire),
            ("DomainSun", "AdditionalDamageDomainLifeDivineStrike", string.Empty),
            ("DomainTempest", "AdditionalDamageDomainTempestDivineStrike", DamageTypeThunder)
        };

        foreach (var (subclassName, additionalDamageName, damageType) in domains)
        {
            var subclass = GetDefinition<CharacterSubclassDefinition>(subclassName);
            var additionalDamage = GetDefinition<FeatureDefinition>(additionalDamageName);
            var featureDamage = !string.IsNullOrEmpty(damageType)
                ? GetDefinition<FeatureDefinition>($"FeatureClericAdditionalDamageGenericBlessed{damageType}")
                : null;
            int fromLevel;
            int toLevel;

            if (Main.Settings.EnableClericBlessedStrikes2024)
            {
                subclass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == additionalDamage);
                subclass.FeatureUnlocks.AddRange(
                    new FeatureUnlockByLevel(FeatureSetClericBlessedStrikes, 7),
                    new FeatureUnlockByLevel(FeatureClericImprovedBlessedStrikes, 14));

                if (featureDamage)
                {
                    subclass.FeatureUnlocks.AddRange(new FeatureUnlockByLevel(featureDamage, 7));
                }

                subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

                fromLevel = 8;
                toLevel = 7;
            }
            else
            {
                subclass.FeatureUnlocks.RemoveAll(x =>
                    x.FeatureDefinition == featureDamage ||
                    x.FeatureDefinition == FeatureSetClericBlessedStrikes ||
                    x.FeatureDefinition == FeatureClericImprovedBlessedStrikes);
                subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(additionalDamage, 8));
                subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

                fromLevel = 7;
                toLevel = 8;
            }

            foreach (var otherSub in ClericDomains)
            {
                var localFromLevel = fromLevel;
                var localToLevel = toLevel;

                otherSub.FeatureUnlocks.Where(x => x.Level == localFromLevel).Do(x => x.level = localToLevel);
            }
        }
    }

    internal static void SwitchClericChannelDivinity()
    {
        Cleric.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PowerClericDivineSpark);

        if (Main.Settings.EnableClericChannelDivinity2024)
        {
            Cleric.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineSpark, 2));
            AttributeModifierClericChannelDivinity.modifierValue = 2;
            AttributeModifierClericChannelDivinity.GuiPresentation.description =
                "Feature/&ClericChannelDivinityExtendedDescription";
        }
        else
        {
            AttributeModifierClericChannelDivinity.modifierValue = 1;
            AttributeModifierClericChannelDivinity.GuiPresentation.description =
                "Feature/&ClericChannelDivinityDescription";
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

#if false
    internal static void SwitchClericDivineIntervention()
    {
        var divineInterventions = new[]
        {
            ("DomainBattle", "PowerClericDivineInterventionPaladin"), //
            ("DomainDefiler", "PowerClericDivineInterventionPaladin"), // UB
            ("DomainElementalCold", "PowerClericDivineInterventionWizard"), //
            ("DomainElementalFire", "PowerClericDivineInterventionWizard"), //
            ("DomainElementalLighting", "PowerClericDivineInterventionWizard"), //
            ("DomainInsight", "PowerClericDivineInterventionCleric"), //
            ("DomainLaw", "PowerClericDivineInterventionPaladin"), //
            ("DomainLife", "PowerClericDivineInterventionCleric"), //
            ("DomainMischief", "PowerClericDivineInterventionWizard"), //
            ("DomainNature", "PowerClericDivineInterventionWizard"), // UB
            ("DomainOblivion", "PowerClericDivineInterventionCleric"), //
            ("DomainSmith", "PowerClericDivineInterventionPaladin"), // UB
            ("DomainSun", "PowerClericDivineInterventionWizard"), //
            ("DomainTempest", "PowerClericDivineInterventionPaladin") // UB
        };

        foreach (var (subclassName, powerName) in divineInterventions)
        {
            var subClass = GetDefinition<CharacterSubclassDefinition>(subclassName);
            var power = GetDefinition<FeatureDefinitionPower>(powerName);

            Cleric.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericDivineIntervention);
            subClass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == power);

            if (Main.Settings.EnableClericDivineIntervention2024)
            {
                Cleric.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericDivineIntervention, 10));
            }
            else
            {
                subClass.FeatureUnlocks.Add(new FeatureUnlockByLevel(power, 10));
            }
        }

        foreach (var (subclassName, powerName) in divineInterventions)
        {
            var subClass = GetDefinition<CharacterSubclassDefinition>(subclassName);
            var improvementPowerName = powerName.Replace("Intervention", "InterventionImprovement");
            var improvementPower = GetDefinition<FeatureDefinitionPower>(improvementPowerName);

            Cleric.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericDivineInterventionImproved);
            subClass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == improvementPower);

            if (Main.Settings.EnableClericDivineIntervention2024)
            {
                Cleric.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericDivineInterventionImproved, 20));
            }
            else
            {
                subClass.FeatureUnlocks.Add(new FeatureUnlockByLevel(improvementPower, 20));
            }
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
#endif

    internal static void SwitchClericDivineOrder()
    {
        Cleric.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureSetClericDivineOrder);

        if (Main.Settings.EnableClericDivineOrder2024)
        {
            Cleric.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericDivineOrder, 1));
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static readonly CharacterSubclassDefinition[] ClericDomains = DatabaseRepository
        .GetDatabase<CharacterSubclassDefinition>()
        .Where(x => x.Name.StartsWith("Domain"))
        .ToArray();

    private static readonly (CharacterSubclassDefinition, FeatureDefinition)[] ClericFeaturesGrantedAt2 =
        ClericDomains
            .SelectMany(y =>
                    y.FeatureUnlocks.Where(z => z.Level == 2),
                (subclass, feature) => (subclass, feature.FeatureDefinition))
            .ToArray();

    internal static void SwitchClericDomainLearningLevel()
    {
        var fromLevel = 3;
        var toLevel = 1;

        if (Main.Settings.EnableClericToLearnDomainAtLevel3)
        {
            fromLevel = 1;
            toLevel = 3;
        }

        foreach (var featureUnlock in ClericDomains
                     .SelectMany(domain => domain.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        var level = Main.Settings.EnableClericToLearnDomainAtLevel3 ? 3 : 2;

        foreach (var (subClass, feature) in ClericFeaturesGrantedAt2)
        {
            subClass.FeatureUnlocks.FirstOrDefault(x => x.FeatureDefinition == feature)!.level = level;
        }

        // change spell casting level on Cleric itself
        Cleric.FeatureUnlocks
            .FirstOrDefault(x =>
                x.FeatureDefinition == SubclassChoiceClericDivineDomains)!.level = toLevel;

        foreach (var domain in ClericDomains)
        {
            domain.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchClericSearUndead()
    {
        Cleric.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == FeatureClericSearUndead ||
                x.FeatureDefinition == PowerClericTurnUndead5 ||
                x.FeatureDefinition == PowerClericTurnUndead11 ||
                x.FeatureDefinition == PowerClericTurnUndead14 ||
                x.FeatureDefinition == Level20Context.PowerClericTurnUndead17);

        if (Main.Settings.EnableClericSearUndead2024)
        {
            Cleric.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureClericSearUndead, 5));
        }
        else
        {
            Cleric.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(PowerClericTurnUndead5, 5),
                new FeatureUnlockByLevel(PowerClericTurnUndead11, 11),
                new FeatureUnlockByLevel(PowerClericTurnUndead14, 14),
                new FeatureUnlockByLevel(Level20Context.PowerClericTurnUndead17, 17));
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static int GetWisdomModifierMinimumOne(RulesetCharacter character)
    {
        var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
        var wisMod = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);

        return Math.Max(wisMod, 1);
    }

    private sealed class ModifyEffectDescriptionPowerDivineSparkHeal(FeatureDefinitionPower powerDivineSparkHeal)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDivineSparkHeal;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(Cleric);
            var diceNumber = levels switch
            {
                >= 18 => 4,
                >= 13 => 3,
                >= 7 => 2,
                _ => 1
            };

            effectDescription.EffectForms[0].HealingForm.diceNumber = diceNumber;

            return effectDescription;
        }
    }

    private sealed class ModifyEffectDescriptionPowerTurnUndead : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerClericTurnUndead;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.EnableClericSearUndead2024)
            {
                effectDescription.EffectForms.Remove(SearUndeadDamageForm);

                return effectDescription;
            }

            var diceNumber = GetWisdomModifierMinimumOne(character);

            effectDescription.EffectForms[0].DamageForm.diceNumber = diceNumber;

            return effectDescription;
        }
    }

    private sealed class ModifyAbilityCheckThaumaturge : IModifyAbilityCheck
    {
        public void MinRoll(
            RulesetCharacter character,
            int baseBonus,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends,
            ref int rollModifier, ref int minRoll)
        {
            if (abilityScoreName is not AttributeDefinitions.Intelligence ||
                proficiencyName is not (SkillDefinitions.Arcana or SkillDefinitions.Religion))
            {
                return;
            }

            var modifier = GetWisdomModifierMinimumOne(character);

            rollModifier += modifier;

            modifierTrends.Add(new TrendInfo(modifier, FeatureSourceType.CharacterFeature,
                PointPoolClericThaumaturgeCantrip.Name, PointPoolClericThaumaturgeCantrip));
        }
    }

    private sealed class CustomBehaviorBlessedStrikes(FeatureDefinitionPower powerBlessedStrikes)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget, bool criticalHit)
        {
            yield return HandleReaction(attacker, battleManager);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (ValidatorsWeapon.IsMelee(attackMode))
            {
                yield return HandleReaction(attacker, battleManager);
            }
        }

        private IEnumerator HandleReaction(GameLocationCharacter attacker, GameLocationBattleManager battleManager)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BlessedStrikesToggle) ||
                !attacker.OnceInMyTurnIsValid(BlessedStrikes))
            {
                yield break;
            }

            var hero = rulesetAttacker.GetOriginalHero();
            var powers = new List<RulesetUsablePower>();

            foreach (var subclass in hero!.ClassesAndSubclasses.Values)
            {
                if (!AdditionalDamageBlessedStrikes.TryGetValue(subclass.Name, out var powerNames))
                {
                    continue;
                }

                powers.AddRange(powerNames.Select(x =>
                    PowerProvider.Get(GetDefinition<FeatureDefinitionPower>(x), rulesetAttacker)));

                break;
            }


            hero.UsablePowers.AddRange(powers);

            var usablePowerPool = PowerProvider.Get(powerBlessedStrikes, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePowerPool,
                [attacker],
                attacker,
                powerBlessedStrikes.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            powers.Do(x => hero.UsablePowers.Remove(x));

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                attacker.SetSpecialFeatureUses(BlessedStrikes, 1);
            }
        }
    }

    private sealed class ModifyEffectDescriptionPowerClericImprovedBlessedStrikes : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerClericImprovedBlessedStrikes;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var tempHp = GetWisdomModifierMinimumOne(character) * 2;

            effectDescription.EffectForms[0].TemporaryHitPointsForm.BonusHitPoints = tempHp;

            return effectDescription;
        }
    }
}
