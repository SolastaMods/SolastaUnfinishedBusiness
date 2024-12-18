using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionPointPool PointPoolClericThaumaturgeCantrip =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolClericThaumaturgeCantrip")
            .SetGuiPresentation(Category.Feature)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, SpellListDefinitions.SpellListCleric,
                "Thaumaturge")
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

#if false
    private static readonly FeatureDefinitionFeatureSet FeatureSetClericDivineIntervention =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetClericDivineIntervention")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetClericDivineInterventionImproved =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetClericDivineInterventionImproved")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
#endif

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
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 2), (13, 3), (18, 4))
                            .SetHealingForm(
                                HealingComputation.Dice, 0, DieType.D8, 1, false, HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(CureWounds)
                    .Build())
            .AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

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

    internal static void SwitchClericDomainLearningLevel()
    {
        var domains = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x => x.Name.StartsWith("Domain"))
            .ToList();

        var fromLevel = 3;
        var toLevel = 1;

        if (Main.Settings.EnableClericToLearnDomainAtLevel3)
        {
            fromLevel = 1;
            toLevel = 3;
        }

        foreach (var featureUnlock in domains
                     .SelectMany(school => school.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        // handle level 2 grants
        var featuresGrantedAt2 = new[]
        {
            ("DomainBattle", "PowerDomainBattleDecisiveStrike"), //
            ("DomainDefiler", "FeatureSetDomainDefilerDefileLife"), // UB
            ("DomainElementalCold", "PowerDomainElementalIceLance"), //
            ("DomainElementalFire", "PowerDomainElementalFireBurst"), //
            ("DomainElementalLighting", "PowerDomainElementalLightningBlade"), //
            ("DomainInsight", "PowerDomainInsightForeknowledge"), //
            ("DomainLaw", "PowerDomainLawForceOfLaw"), //
            ("DomainLife", "PowerDomainLifePreserveLife"), //
            ("DomainMischief", "PowerDomainMischiefStrikeOfChaos"), //
            ("DomainNature", "FeatureSetDomainNatureCharmAnimalsAndPlants"), // UB
            ("DomainOblivion", "CampAffinityDomainOblivionPeacefulRest"), //
            ("DomainOblivion", "PowerDomainOblivionHeraldOfPain"), //
            ("DomainSmith", "FeatureSetDomainSmithDefileLife"), // UB
            ("DomainSun", "PowerDomainSunHeraldOfTheSun"), //
            ("DomainTempest", "FeatureSetDomainTempestDestructiveWrath") // UB
        };

        var level = Main.Settings.EnableClericToLearnDomainAtLevel3 ? 3 : 2;

        foreach (var (subClassName, featureName) in featuresGrantedAt2)
        {
            var subClass = GetDefinition<CharacterSubclassDefinition>(subClassName);
            var feature = GetDefinition<FeatureDefinition>(featureName);

            subClass.FeatureUnlocks.FirstOrDefault(x => x.FeatureDefinition == feature)!.level = level;
        }

        // change spell casting level on Cleric itself
        Cleric.FeatureUnlocks
            .FirstOrDefault(x =>
                x.FeatureDefinition == SubclassChoiceClericDivineDomains)!.level = toLevel;

        foreach (var domain in domains)
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

    private static int GetWisdomModifierMinimumOne(RulesetCharacter character)
    {
        var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
        var wisMod = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);

        return Math.Max(wisMod, 1);
    }
}
