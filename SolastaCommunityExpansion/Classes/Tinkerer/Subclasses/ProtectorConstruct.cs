using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    //*****************************************************************************************************************************************
    //***********************************		ProtectorConstructFeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class ProtectorConstructFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string Name = "ProtectorConstructFeatureSet";
        private const string Guid = "9b699719-0d02-4949-ad94-cff6a05f36c7";

        private ProtectorConstructFeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(SummoningAffinityTinkererConstructBuilder.SummoningAffinityTinkererConstruct);
            Definition.FeatureSet.Add(SummonProtectorPowerConstructBuilder.SummonProtectorConstruct);
            Definition.FeatureSet.Add(ProtectorConstructLevel3AutopreparedSpellsBuilder.ProtectorConstructLevel3AutopreparedSpells);
        }

        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructFeatureSetBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet ProtectorConstructFeatureSet = CreateAndAddToDB(Name, Guid);
    }

    public sealed class ProtectorConstructLevel3AutopreparedSpellsBuilder : FeatureDefinitionAutoPreparedSpellsBuilder
    {
        private const string Name = "ProtectorConstructLevel3AutopreparedSpells";
        private const string Guid = "25403813-58eb-47f4-b5ee-b7956cc02ccf";

        private ProtectorConstructLevel3AutopreparedSpellsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ProtectorConstructLevel3AutopreparedSpellsTitle";
            Definition.GuiPresentation.Description = "Feat/&ProtectorConstructLevel3AutopreparedSpellsDescription";

#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 1,
                SpellsList = new List<SpellDefinition> { SummonProtectorSpellConstructBuilder.SummonProtectorConstruct }
            };
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed
        }

        private static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructLevel3AutopreparedSpellsBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionAutoPreparedSpells ProtectorConstructLevel3AutopreparedSpells = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummoningAffinityTinkererConstructBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummoningAffinityTinkererConstructBuilder : FeatureDefinitionSummoningAffinityBuilder
    {
        private const string Name = "SummoningAffinityTinkererConstruct";
        private const string Guid = "0dbd3d80-96ce-4cf9-8ffa-597f1ea84c3b";

        private SummoningAffinityTinkererConstructBuilder(string name, string guid)
            : base(DatabaseHelper.FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";
            Definition.GuiPresentation.SetSpriteReference(null);

            Definition.SetEffectOnConjuredDeath(false);
            Definition.AddedConditions.Clear();
            Definition.AddedConditions.Empty();
            Definition.EffectForms.Clear();
            Definition.EffectForms.Empty();

            // changed the tag here and in relevant constructs
            // so the scaling is only applied to the Protector and Artillry Constructs
            Definition.SetRequiredMonsterTag("ScalingTinkererConstruct");
            Definition.AddedConditions.AddRange(new List<ConditionDefinition>
            {
            // using kindred conditions for following reasons
            // 1- Didnt want to create custom conditions until custom ConditionDefintionBuilder and
            //    FeatureDefinitionAttributeModifierBuilder are available as it is likely a rewrite
            //    would be requested as soon as such builders were added.
            // 2- The tabletop scaling using the class level and proficiency bonus of the summoner
            //    is not possible using base game features/database manipulation. A patch would be
            //    required to add such scaling to the game.
            // 3- A new scaling set via new summoningAffinity, conditionDefinitions and attributeModifers
            //    could be added but custom conditions may not be worthwhile as without the above patch,
            //    meaning the any new scaling would not match the required scaling
            // 4- The default summons scaling used in the base game is similar in magnitude to the original
            //    concept for the protector construct, so it seemed acceptable for a first implementation.
            //
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondTotalControl,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondAC,
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondHP,
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack,
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage,
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSkillProficiency,
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSavingThrows
            });
        }

        private static FeatureDefinitionSummoningAffinity CreateAndAddToDB(string name, string guid)
        {
            return new SummoningAffinityTinkererConstructBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionSummoningAffinity SummoningAffinityTinkererConstruct = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonProtectorConstructBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonProtectorPowerConstructBuilder : FeatureDefinitionPowerBuilder
    {
        private const string SummonProtectorConstructName = "SummonProtectorConstruct";
        private const string SummonProtectorConstructNameGuid = "20b5ab3e-5124-4d08-9907-347f2f1284d4";

        private SummonProtectorPowerConstructBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionCleric, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureAnimalsFourBeasts.GuiPresentation.SpriteReference);

            Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(1);
            Definition.SetHasCastingFailure(false);
            Definition.SetUniqueInstance(true);

            EffectDescriptionBuilder ProtectorConstructEffect = new EffectDescriptionBuilder();
            ProtectorConstructEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            ProtectorConstructEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            ProtectorConstructEffect.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ProtectorConstructBuilder.ProtectorConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            Definition.SetEffectDescription(ProtectorConstructEffect.Build());
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SummonProtectorPowerConstructBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower SummonProtectorConstruct = CreateAndAddToDB(SummonProtectorConstructName, SummonProtectorConstructNameGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonProtectorPowerConstruct_UpgradeBuilder		***************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonProtectorPowerConstructUpgradeBuilder : FeatureDefinitionPowerBuilder
    {
        private const string Name = "SummonProtectorPowerConstruct_Upgrade";
        private const string Guid = "34c307e9-5883-438c-9130-1f286b9cdafc";

        private SummonProtectorPowerConstructUpgradeBuilder(string name, string guid) : base(SummonProtectorPowerConstructBuilder.SummonProtectorConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle_2";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription_2";

            Definition.SetOverriddenPower(SummonProtectorPowerConstructBuilder.SummonProtectorConstruct);

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ProtectorConstructUpgradeBuilder.ProtectorConstructUpgrade.Name);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SummonProtectorPowerConstructUpgradeBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower SummonProtectorPowerConstructUpgrade = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonProtectorSpellConstructBuilder		***************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonProtectorSpellConstructBuilder : SpellDefinitionBuilder
    {
        private const string SummonProtectorConstructName = "SummonProtectorConstruct";
        private const string SummonProtectorConstructNameGuid = "60f2462e-b801-48ee-a543-c69771e3917c";

        private SummonProtectorSpellConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.DancingLights, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureAnimalsFourBeasts.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(false);
            Definition.SetUniqueInstance(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.NoCost);
            Definition.SetSomaticComponent(false);
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
            Definition.SetVerboseComponent(false);

            Definition.SetEffectDescription(SummonProtectorPowerConstructBuilder.SummonProtectorConstruct.EffectDescription);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonProtectorSpellConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonProtectorConstruct = CreateAndAddToDB(SummonProtectorConstructName, SummonProtectorConstructNameGuid);
    }
    //*****************************************************************************************************************************************
    //***********************************		SummonProtectorSpellConstruct_UpgradeBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    //
    // need a new featuredefintionAutopreparedSpells list with the summon upgrade spell for the two different subclasses
    // as features can be added at any lvl and it's 1st lvl spell
    //
    internal sealed class SummonProtectorSpellConstructUpgradeBuilder : SpellDefinitionBuilder
    {
        private const string Name = "SummonProtectorConstruct_Upgrade";
        private const string Guid = "ccd2a793-e566-4c1e-9588-ac36b578ae89";

        private SummonProtectorSpellConstructUpgradeBuilder(string name, string guid) : base(SummonProtectorSpellConstructBuilder.SummonProtectorConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle_Upgrade";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription_Upgrade";

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ProtectorConstructUpgradeBuilder.ProtectorConstructUpgrade.Name);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonProtectorSpellConstructUpgradeBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonProtectorConstructUpgrade = CreateAndAddToDB(Name, Guid);
    }

    public sealed class ProtectorConstructLevel15AutopreparedSpellsBuilder : FeatureDefinitionAutoPreparedSpellsBuilder
    {
        private const string Name = "ProtectorConstructLevel15AutopreparedSpells";
        private const string Guid = "4515c27b-f17b-4262-9e8c-a19c251f666e";

        private ProtectorConstructLevel15AutopreparedSpellsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ProtectorConstructLevel15AutopreparedSpellsTitle";
            Definition.GuiPresentation.Description = "Feat/&ProtectorConstructLevel15AutopreparedSpellsDescription";

#pragma warning disable S1481, IDE0059 // Unused local variables should be removed
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 1,
                SpellsList = (new List<SpellDefinition>
            {
                SummonProtectorSpellConstructUpgradeBuilder.SummonProtectorConstructUpgrade
            })
            };
#pragma warning restore S1481, IDE0059 // Unused local variables should be removed
        }

        private static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructLevel15AutopreparedSpellsBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionAutoPreparedSpells ProtectorConstructLevel15AutopreparedSpells = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ProtectorConstructUpgradeFeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class ProtectorConstructUpgradeFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string Name = "ProtectorConstructUpgradeFeatureSet";
        private const string Guid = "59bc566d-5204-4e53-89cb-eebc537ae6ab";

        private ProtectorConstructUpgradeFeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonProtectorConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonProtectorConstructDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ProtectorConstructLevel15AutopreparedSpellsBuilder.ProtectorConstructLevel15AutopreparedSpells);
            Definition.FeatureSet.Add(SummonProtectorPowerConstructUpgradeBuilder.SummonProtectorPowerConstructUpgrade);
        }

        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructUpgradeFeatureSetBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet ProtectorConstructUpgradeFeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ProtectorConstructBuilder		***************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ProtectorConstructBuilder : MonsterDefinitionBuilder
    {
        private const string ProtectorConstructName = "ProtectorConstruct";
        private const string ProtectorConstructNameGuid = "db1cd36f-7dc7-454f-baeb-143cd9dd374f";

        private ProtectorConstructBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.ConjuredEightBeast_Wolf, name, guid)
        {
            Definition.SetMonsterPresentation(DatabaseHelper.MonsterDefinitions.Ghost_Wolf.MonsterPresentation);

            Definition.GuiPresentation.Title = "Feat/&ProtectorConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&ProtectorConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.MonsterDefinitions.Ghost_Wolf.GuiPresentation.SpriteReference);
            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            Definition.MonsterPresentation.SetCanGeneratePortrait(true);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

            Definition.SetArmorClass(15);
            Definition.SetNoExperienceGain(true);
            Definition.SetHitDice(3);
            Definition.SetHitDiceType(RuleDefinitions.DieType.D8);

            Definition.AbilityScores.Empty();
            Definition.AbilityScores.AddToArray(14);    // STR
            Definition.AbilityScores.AddToArray(12);    // DEX
            Definition.AbilityScores.AddToArray(14);    // CON
            Definition.AbilityScores.AddToArray(4);     // INT
            Definition.AbilityScores.AddToArray(10);    // WIS
            Definition.AbilityScores.AddToArray(6);     // CHA

            //replaced by new bond methods?
            //assume PB=4

            Definition.SavingThrowScores.SetRange(
                new MonsterSavingThrowProficiency(AttributeDefinitions.Constitution, 2),
                new MonsterSavingThrowProficiency(AttributeDefinitions.Dexterity, 1));

            Definition.SkillScores.SetRange(
                new MonsterSkillProficiency(SkillDefinitions.Athletics, 2),
                new MonsterSkillProficiency(SkillDefinitions.Perception, 4));

            Definition.SetFullyControlledWhenAllied(true);
            Definition.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None);
            Definition.SetStandardHitPoints(20);
            Definition.SetDefaultFaction("Party");
            Definition.SetCharacterFamily(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);
            // the following tag is used for scaling purposes
            Definition.CreatureTags.Add("ScalingTinkererConstruct");

            Definition.Features.Clear();
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityFightingStyleProtection);
            Definition.Features.Add(SelfRepairBuilder.SelfRepair);

            Definition.AttackIterations.SetRange(new MonsterAttackIteration(ProtectorConstructAttackBuilder.ProtectorConstructAttack, 1));
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ProtectorConstruct = CreateAndAddToDB(ProtectorConstructName, ProtectorConstructNameGuid);
    }
    //*****************************************************************************************************************************************
    //***********************************		ProtectorConstruct_UpgradeBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal sealed class ProtectorConstructUpgradeBuilder : MonsterDefinitionBuilder
    {
        private const string Name = "ProtectorConstruct_Upgrade";
        private const string Guid = "c6f711d8-9b83-497f-8e90-6440776cf644";

        private ProtectorConstructUpgradeBuilder(string name, string guid) : base(ProtectorConstructBuilder.ProtectorConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ProtectorConstructTitle_5";
            Definition.GuiPresentation.Description = "Feat/&ProtectorConstructDescription_5";

            Definition.SetArmorClass(17);
            Definition.Features.Add(RetributionBuilder.Retribution);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructUpgradeBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ProtectorConstructUpgrade = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ProtectorConstructAttacksBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal sealed class ProtectorConstructAttackBuilder : MonsterAttackDefinitionBuilder
    {
        private const string ProtectorConstructAttacksName = "ProtectorConstructAttacks";
        private const string ProtectorConstructAttacksGuid = "dad5a3f6-3b44-4476-85fc-d5235b9ad9cd";

        private ProtectorConstructAttackBuilder(string name, string guid) : base(DatabaseHelper.MonsterAttackDefinitions.Attack_Wolf_Bite, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ProtectorConstructAttackTitle";
            Definition.GuiPresentation.Description = "Feat/&ProtectorConstructAttackDescription";

            EffectForm damageEffect = new EffectForm
            {
                AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus,
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D8,

                    //replaced by new bond methods?
                    //int assumedIntModifier = 3;
                    //int assumedProficiencyBonus = 2;
                    //damageEffect.DamageForm.BonusDamage = assumedProficiencyBonus;
                    DamageType = "DamageForce"
                }
            };

            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
            newEffectDescription.SetTargetProximityDistance(1);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);
            newEffectDescription.SetRangeParameter(1);

            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.MonsterAttackDefinitions.Attack_Wolf_Bite.EffectDescription.EffectParticleParameters);

            //replaced by new bond methods?
            Definition.SetEffectDescription(newEffectDescription);
            Definition.SetToHitBonus(0);
        }

        private static MonsterAttackDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ProtectorConstructAttackBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterAttackDefinition ProtectorConstructAttack = CreateAndAddToDB(ProtectorConstructAttacksName, ProtectorConstructAttacksGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SelfRepairBuilder		***********************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SelfRepairBuilder : FeatureDefinitionPowerBuilder
    {
        private const string SelfRepairName = "SelfRepair";
        private const string SelfRepairNameGuid = "68db5cab-6fc9-4795-88a6-f89d81b0e4ef";

        private SelfRepairBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SelfRepairTitle";
            Definition.GuiPresentation.Description = "Feat/&SelfRepairDescription";

            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetFixedUsesPerRecharge(3);
            Definition.SetCostPerUse(1);

            HealingForm selfrepair = new HealingForm
            {
                BonusHealing = 4,
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 2
            };

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Healing
            };
            effect.SetHealingForm(selfrepair);
            effect.SetCreatedByCharacter(true);

            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(effect);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
            Definition.EffectDescription.RestrictedCreatureFamilies.Add(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SelfRepairBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower SelfRepair = CreateAndAddToDB(SelfRepairName, SelfRepairNameGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		RetributionBuilder		***********************************************************************
    //*****************************************************************************************************************************************

    internal sealed class RetributionBuilder : FeatureDefinitionPowerBuilder
    {
        private const string RetributionName = "Retribution";
        private const string RetributionNameGuid = "1fc63d9f-263c-4642-b75c-f7684ca6dd3d";

        private RetributionBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&RetributionTitle";
            Definition.GuiPresentation.Description = "Feat/&RetributionDescription";
            Definition.SetShortTitleOverride("Feat/&RetributionTitle");

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 1;
            Definition.EffectDescription.EffectForms[0].DamageForm.DieType = RuleDefinitions.DieType.D4;

            //replaced by new bond methods?
            //int assumedIntModifier = 5;
            //Definition.EffectDescription.EffectForms[0].DamageForm.BonusDamage = assumedIntModifier;
            Definition.EffectDescription.EffectForms[0].DamageForm.DamageType = RuleDefinitions.DamageTypeForce;
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new RetributionBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower Retribution = CreateAndAddToDB(RetributionName, RetributionNameGuid);
    }
}
