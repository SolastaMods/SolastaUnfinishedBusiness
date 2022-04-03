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
using System.Linq;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    // NOTE: not currently used
    public static class DHWarlockSubclassUrPriestPatron
    {
        public static CharacterSubclassDefinition Build()
        {
            var urPriestExpandedSpelllistAfinity = FeatureDefinitionMagicAffinityBuilder
                .Create("UrPriestExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetExtendedSpellList(SpellListCleric)
                .AddToDB();

            var listofClericDomains = new [] {
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

            FeatureDefinitionFeatureSet urPriestDomainTheft = FeatureDefinitionFeatureSetBuilder
                .Create("UrPriestDomainTheft", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .AddFeatureSet(
                    listofClericDomains.Select(subclass => FeatureDefinitionFeatureSetBuilder
                        .Create(subclass.name + "level1Features", DefinitionBuilder.CENamespaceGuid)
                        .SetGuiPresentation(subclass.name + "Domainlevel1Features", Category.Feature)
                        .SetEnumerateInDescription(true)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .AddFeatureSet(subclass.FeatureUnlocks.Where(fu => fu.Level == 1).Select(fu => fu.FeatureDefinition))
                        .AddToDB()
                ))
                .AddToDB();

            FeatureDefinitionPower controlUndead = FeatureDefinitionPowerBuilder
                .Create("UrPriestControlUndead", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       ActivationTime.Action,
                       1,
                       RechargeRate.ChannelDivinity,
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
                                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                                DatabaseHelper.SmartAttributeDefinitions.Wisdom.name,
                                20,
                                false,
                                new List<SaveAffinityBySenseDescription>())
                            .SetTargetingData(
                                    Side.Enemy,
                                    RangeType.Distance,
                                    1,
                                    TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.HoldMonster.EffectDescription.EffectParticleParameters)
                            .AddRestrictedCreatureFamilies(Undead)
                            .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionPower radiantWard = FeatureDefinitionPowerBuilder
                .Create("UrPriestRadiantWard", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       ActivationTime.Action,
                       1,
                       RechargeRate.ChannelDivinity,
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
                                    Side.All,
                                    RangeType.Self,
                                    1,
                                    TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(PowerDomainBattleDivineWrath.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionPower spellSiphon = FeatureDefinitionPowerBuilder
                .Create("UrPriestSpellSiphon", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       ActivationTime.Action,
                       1,
                       RechargeRate.ChannelDivinity,
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
                                    Side.All,
                                    RangeType.Self,
                                    1,
                                    TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetParticleEffectParameters(PowerDomainBattleDivineWrath.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();

            ConditionDefinition halfLifeCondition = ConditionDefinitionBuilder
                .Create("HalfLifeCondition", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Condition)
                .SetCharacterShaderReference(DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.CustomShaderReference)
                .SetConditionParticleReference(new AssetReference("62a22b407b0619549997a357f6344630"))
                .SetFeatures(
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
                    DamageAffinityThunderResistance
                )
                .AddToDB();

            FeatureDefinitionPower halfLife = FeatureDefinitionPowerBuilder
                .Create("UrPriestHalfLife", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       ActivationTime.Action,
                       1,
                       RechargeRate.ChannelDivinity,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .SetDurationData(
                                DurationType.Minute,
                                1,
                                TurnOccurenceType.EndOfTurn)
                            .SetTargetingData(
                                    Side.All,
                                    RangeType.Self,
                                    1,
                                    TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    halfLifeCondition,
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
                .AddFeaturesAtLevel(1, urPriestDomainTheft, urPriestExpandedSpelllistAfinity)
                .AddFeaturesAtLevel(6, controlUndead, radiantWard, AttributeModifierClericChannelDivinity)
                .AddFeaturesAtLevel(10, SavingThrowAffinitySpellResistance)
                .AddFeaturesAtLevel(14, halfLife, spellSiphon)
                .AddToDB();
        }
    }
}
