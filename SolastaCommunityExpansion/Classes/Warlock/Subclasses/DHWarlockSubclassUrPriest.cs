using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterFamilyDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using UnityEngine.AddressableAssets;


//******************************************************************************************
//  DO NOT REFACTOR OR CHANGE WITHOUT TESTING OR TAKING RESPOSBILITY FOR CODE GOING FORWARD
//******************************************************************************************

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class DHWarlockSubclassUrPriestPatron
    {
        public static CharacterSubclassDefinition Build()
        {

            FeatureDefinitionMagicAffinity UrPriestExpandedSpelllistAfinity = FeatureDefinitionMagicAffinityBuilder
                .Create("UrPriestExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("UrPriestExpandedSpelllistAfinity", Category.Feature)
                .SetExtendedSpellList(SpellListCleric)
                .AddToDB();

            List<CharacterSubclassDefinition> ListofClericDomains = new List<CharacterSubclassDefinition> {
                DomainBattle,
                DomainElementalCold,
                DomainElementalFire,
                DomainElementalLighting,
                DomainInsight,
                DomainLaw,
                DomainLife,
                DomainMischief,
                DomainOblivion,
                DomainSun
            };

            FeatureDefinitionFeatureSet UrPriestDomainTheft = FeatureDefinitionFeatureSetBuilder
                .Create("UrPriestDomainTheft", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("UrPriestDomainTheft", Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .AddToDB();

            foreach (CharacterSubclassDefinition subclass in ListofClericDomains)
            {

                FeatureDefinitionFeatureSet Domainlevel1Features = FeatureDefinitionFeatureSetBuilder
                    .Create(subclass.name+"level1Features", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(subclass.name + "Domainlevel1Features", Category.Feature)
                    .SetEnumerateInDescription(true)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .AddToDB();

                foreach (FeatureUnlockByLevel FeatureUnlockByLevel in subclass.FeatureUnlocks)
                {
                    if (FeatureUnlockByLevel.Level==1)
                    {
                        Domainlevel1Features.FeatureSet.Add(FeatureUnlockByLevel.FeatureDefinition);
                    };
                }


                UrPriestDomainTheft.FeatureSet.Add(Domainlevel1Features);
            }

            FeatureDefinitionPower ControlUndead = FeatureDefinitionPowerBuilder
                .Create("UrPriestControlUndead", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.ChannelDivinity,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    ConditionMindDominatedByCaster,
                                    ConditionForm.ConditionOperation.Add,
                                    false,
                                    false,
                                    new List<ConditionDefinition>()
                                    )
                                .Build()
                                .SetHasSavingThrow(true)
                                .SetSavingThrowAffinity(EffectSavingThrowType.Negates)
                                )
                            .SetSavingThrowData(
                                true,
                                false,
                                DatabaseHelper.SmartAttributeDefinitions.Wisdom.name,
                                true,
                                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                                DatabaseHelper.SmartAttributeDefinitions.Wisdom.name,
                                20,
                                false,
                                new List<SaveAffinityBySenseDescription>())
                            .SetTargetingData(
                                    RuleDefinitions.Side.Enemy,
                                    RuleDefinitions.RangeType.Distance,
                                    1,
                                    RuleDefinitions.TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.HoldMonster.EffectDescription.EffectParticleParameters)
                            .AddRestrictedCreatureFamilies(Undead)
                            .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionPower RadiantWard = FeatureDefinitionPowerBuilder
                .Create("UrPriestRadiantWard", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.ChannelDivinity,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetTempHPForm(
                                    0,
                                    DieType.D1,
                                    1
                                    )
                                .Build()
                                .SetApplyLevel(EffectForm.LevelApplianceType.MultiplyDice)
                                .SetLevelType(LevelSourceType.ClassLevel)
                                .SetLevelMultiplier(2)
                                )
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.Self,
                                    1,
                                    RuleDefinitions.TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(PowerDomainBattleDivineWrath.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();


            FeatureDefinitionPower SpellSiphon = FeatureDefinitionPowerBuilder
                .Create("UrPriestSpellSiphon", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.ChannelDivinity,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetSpellForm(
                                    6
                                    )
                                .Build()
                                .SetApplyLevel(EffectForm.LevelApplianceType.MultiplyDice)
                                .SetLevelType(LevelSourceType.ClassLevel)
                                .SetLevelMultiplier(2)
                                )
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.Self,
                                    1,
                                    RuleDefinitions.TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(PowerDomainBattleDivineWrath.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();


            ConditionDefinition HalfLifeCondition = ConditionDefinitionBuilder
            .Create("HalfLifeCondition", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition)
            .SetCharacterShaderReference(DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.CustomShaderReference)
            .SetConditionParticleReference(new AssetReference("62a22b407b0619549997a357f6344630"))
            .SetFeatures(new List<FeatureDefinition>()
            {
                MoveModeFly8,
                ConditionAffinityCharmImmunity,
                ConditionAffinityExhaustionImmunity,
                ConditionAffinityPoisonImmunity,
                ConditionAffinityProneImmunity,
                ConditionAffinityHinderedByFrostImmunity,
                ConditionAffinityFrightenedImmunity,
                ConditionAffinityGrappledImmunity,
                ConditionAffinityParalyzedmmunity,
                ConditionAffinityPetrifiedImmunity,
                ConditionAffinityRestrainedmmunity,
                
                DamageAffinitySlashingResistance,
                DamageAffinityNecroticImmunity,
                DamageAffinityPoisonImmunity,
                DamageAffinityColdImmunity,
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityLightningResistance,
                DamageAffinityFireResistance,
                DamageAffinityAcidResistance,
                DamageAffinityThunderResistance,
            })
            .AddToDB();


            FeatureDefinitionPower HalfLife = FeatureDefinitionPowerBuilder
                .Create("UrPriestHalfLife", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.ChannelDivinity,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .SetDurationData(
                                DurationType.Minute,
                                1,  
                                TurnOccurenceType.EndOfTurn)
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.Self,
                                    1,
                                    RuleDefinitions.TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    HalfLifeCondition,
                                    ConditionForm.ConditionOperation.Add,
                                    false,
                                    false,
                                    new List<ConditionDefinition>()
                                    )
                                .Build()
                                )
                            .SetParticleEffectParameters(PowerFunctionPotionOfInvisibility.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();


            return CharacterSubclassDefinitionBuilder
                .Create("UrPriest", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("WarlockUrPriest", Category.Subclass, SorcerousChildRift.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(UrPriestExpandedSpelllistAfinity, 1)
                .AddFeatureAtLevel(UrPriestDomainTheft, 1)
                .AddFeatureAtLevel(AttributeModifierClericChannelDivinity, 6)
                .AddFeatureAtLevel(ControlUndead, 6)
                .AddFeatureAtLevel(RadiantWard, 6)
                 .AddFeatureAtLevel(SavingThrowAffinitySpellResistance, 10)
                 .AddFeatureAtLevel(SpellSiphon, 14)
                .AddFeatureAtLevel(HalfLife,14)
                .AddToDB();
        }

    }
}
