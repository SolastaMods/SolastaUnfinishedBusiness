using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier.AttributeModifierOperation;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Druid
{
    internal class CircleOfTheForestGuardian : AbstractSubclass
    {
        private CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass ??= BuildAndAddSubclass();
        }

        private const string DruidForestGuardianDruidSubclassName = "DruidForestGuardianDruidSubclass";
        internal static readonly Guid BaseGuid = new("45a7595b-5d5f-4351-b7f1-cb78c9d0a136");

        private static CharacterSubclassDefinition BuildAndAddSubclass()
        {
            // Create Auto-prepared Spell list
            var druidForestGuardianMagic = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ForestGuardianAutoPreparedSpells", BaseGuid)
                .SetGuiPresentation("DruidForestGuardianMagic", Category.Feature)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(2, Shield, FogCloud),
                    BuildSpellGroup(3, Blur, FlameBlade),
                    BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                    BuildSpellGroup(7, FireShield, DeathWard),
                    BuildSpellGroup(9, HoldMonster, GreaterRestoration))
                .SetSpellcastingClass(DatabaseHelper.CharacterClassDefinitions.Druid)
                .AddToDB();

            // NOTE: unable to use preferred Create because of legacy guid generation
            var extraAttack = FeatureDefinitionAttributeModifierBuilder
                .Create(
                    "AttributeModifierDruidForestGuardianExtraAttack", // unless we can change this name to DruidForestGuardianExtraAttack
                    GuidHelper.Create(BaseGuid, "DruidForestGuardianExtraAttack").ToString())
                .SetGuiPresentation(Category.Feature)
                .SetModifier(Additive, AttributeDefinitions.AttacksNumber, 1)
                .AddToDB();

            // NOTE: unable to use preferred Create because of legacy guid generation
            var sylvanResistance = FeatureDefinitionAttributeModifierBuilder
                .Create(
                    "AttributeModifierDruidForestGuardianSylvanDurability", // unless we can change this name to DruidForestGuardianSylvanDurability
                    GuidHelper.Create(BaseGuid, "DruidForestGuardianSylvanDurability").ToString())
                .SetGuiPresentation(Category.Feature)
                .SetModifier(Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
                .AddToDB();

            // Create Sylvan War Magic
            var sylvanWarMagic = FeatureDefinitionMagicAffinityBuilder
                .Create(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic, "DruidForestGuardianSylvanWarMagic", BaseGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            var (barkWard, improvedBarkWard, superiorBarkWard) = CreateBarkWard();

            return CharacterSubclassDefinitionBuilder
                .Create(DruidForestGuardianDruidSubclassName, BaseGuid)
                .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(druidForestGuardianMagic, 2)
                .AddFeatureAtLevel(sylvanResistance, 2)
                .AddFeatureAtLevel(sylvanWarMagic, 2)
                .AddFeatureAtLevel(barkWard, 2)
                .AddFeatureAtLevel(extraAttack, 6)
                .AddFeatureAtLevel(improvedBarkWard, 10)
                .AddFeatureAtLevel(superiorBarkWard, 14)
                .AddToDB();
        }

        // Create Bark Ward Wild Shape Power (and the two higher variants, improved and superior)
        private static (FeatureDefinitionPowerSharedPool barkWard, FeatureDefinitionPowerSharedPool improvedBarkWard, FeatureDefinitionPowerSharedPool superiorBarkWard) CreateBarkWard()
        {
            EffectFormBuilder tempHPEffect = new EffectFormBuilder();
            tempHPEffect.SetTempHPForm(4, DieType.D1, 0);
            tempHPEffect.SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel, 1);
            tempHPEffect.CreatedByCharacter();

            EffectFormBuilder barkWardBuff = new EffectFormBuilder();
            barkWardBuff.SetConditionForm(ConditionBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectFormBuilder improvedBarkWardBuff = new EffectFormBuilder();
            improvedBarkWardBuff.SetConditionForm(ConditionImprovedBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectFormBuilder superiorBarkWardBuff = new EffectFormBuilder();
            superiorBarkWardBuff.SetConditionForm(ConditionSuperiorBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectDescriptionBuilder barkWardEffectDescription = new EffectDescriptionBuilder();
            barkWardEffectDescription.SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            barkWardEffectDescription.SetCreatedByCharacter();
            barkWardEffectDescription.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            barkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            barkWardEffectDescription.AddEffectForm(barkWardBuff.Build());
            barkWardEffectDescription.SetEffectAdvancement(EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, AdvancementDuration.None);

            EffectDescriptionBuilder improvedBarkWardEffectDescription = new EffectDescriptionBuilder();
            improvedBarkWardEffectDescription.SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            improvedBarkWardEffectDescription.SetCreatedByCharacter();
            improvedBarkWardEffectDescription.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            improvedBarkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            improvedBarkWardEffectDescription.AddEffectForm(improvedBarkWardBuff.Build());
            improvedBarkWardEffectDescription.SetEffectAdvancement(EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, AdvancementDuration.None);

            EffectDescriptionBuilder superiorBarkWardEffectDescription = new EffectDescriptionBuilder();
            superiorBarkWardEffectDescription.SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            superiorBarkWardEffectDescription.SetCreatedByCharacter();
            superiorBarkWardEffectDescription.SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn);
            superiorBarkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            superiorBarkWardEffectDescription.AddEffectForm(superiorBarkWardBuff.Build());
            superiorBarkWardEffectDescription.SetEffectAdvancement(EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, AdvancementDuration.None);

            var barkWard = FeatureDefinitionPowerSharedPoolBuilder
                .Create("DruidForestGuardianBarkWard", BaseGuid)
                .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
                .Configure(
                    PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                    AttributeDefinitions.Wisdom, barkWardEffectDescription.Build(), true)
                .AddToDB();

            var improvedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
                .Create("DruidForestGuardianImprovedBarkWard", BaseGuid)
                .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
                .Configure(
                    PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                    AttributeDefinitions.Wisdom, improvedBarkWardEffectDescription.Build(), true)
                .SetOverriddenPower(barkWard)
                .AddToDB();

            var superiorBarkWard = FeatureDefinitionPowerSharedPoolBuilder
                .Create("DruidForestGuardianSuperiorBarkWard", BaseGuid)
                .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
                .Configure(
                    PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                    AttributeDefinitions.Wisdom, superiorBarkWardEffectDescription.Build(), true)
                .SetOverriddenPower(improvedBarkWard)
                .AddToDB();

            return (barkWard, improvedBarkWard, superiorBarkWard);
        }
    }

    // Creates a dedicated builder for the the three Bark Ward conditions
    internal class ConditionBarkWardBuilder : ConditionDefinitionBuilder
    {
        protected ConditionBarkWardBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBarkskin, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionBarkWardTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionBarkWardDescription";

            Definition.Features.Clear();
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(10);
            Definition.SetDurationType(DurationType.Minute);
            Definition.SetTurnOccurence(TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionBarkWardBuilder("BarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "BarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("BarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "BarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal class ConditionImprovedBarkWardBuilder : ConditionDefinitionBuilder
    {
        private static FeatureDefinitionPower CreateImprovedBarkWardRetaliate()
        {
            EffectFormBuilder damageEffect = new EffectFormBuilder();
            damageEffect.SetDamageForm(false, DieType.D8,
                "DamagePiercing",
                0, DieType.D8,
                2, HealFromInflictedDamage.Never,
                new List<RuleDefinitions.TrendInfo>());
            damageEffect.CreatedByCondition();

            EffectDescriptionBuilder improvedBarkWardRetaliationEffect = new EffectDescriptionBuilder();
            improvedBarkWardRetaliationEffect.AddEffectForm(damageEffect.Build());

            return FeatureDefinitionPowerBuilder
                .Create("improvedBarkWardRetaliate", CircleOfTheForestGuardian.BaseGuid)
                .SetGuiPresentationNoContent()
                .Configure(
                    0,
                    UsesDetermination.Fixed,
                    AttributeDefinitions.Wisdom,
                    ActivationTime.NoCost,
                    0,
                    RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Wisdom,
                    improvedBarkWardRetaliationEffect.Build(),
                    true
                    )
                .AddToDB();
        }

        private static FeatureDefinitionDamageAffinity CreateImprovedBarkWardDamage()
        {
            return FeatureDefinitionDamageAffinityBuilder
                .Create("ImprovedBarkWardRetaliationDamage", CircleOfTheForestGuardian.BaseGuid)
                .SetGuiPresentationNoContent()
                .SetDamageAffinityType(DamageAffinityType.None)
                .SetDamageType(DamageTypePoison)
                .SetRetaliate(CreateImprovedBarkWardRetaliate(), 1, true)
                .SetAncestryDefinesDamageType(false)
                .AddToDB();
        }

        protected ConditionImprovedBarkWardBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBarkskin, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionImprovedBarkWardTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionImprovedBarkWardDescription";

            SetFeatures(CreateImprovedBarkWardDamage());
            SetAllowMultipleInstances(false);
            SetDuration(DurationType.Minute, 10);
            SetTurnOccurence(TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionImprovedBarkWardBuilder("ImprovedBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "ImprovedBarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ImprovedBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "ImprovedBarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal class ConditionSuperiorBarkWardBuilder : ConditionDefinitionBuilder
    {
        private static FeatureDefinitionPower CreateSuperiorBarkWardRetaliate()
        {
            EffectFormBuilder damageEffect = new EffectFormBuilder();
            damageEffect.SetDamageForm(false, DieType.D8,
                "DamagePiercing",
                0, DieType.D8,
                3, HealFromInflictedDamage.Never,
                new List<TrendInfo>());
            damageEffect.CreatedByCondition();

            EffectDescriptionBuilder superiorBarkWardRetaliationEffect = new EffectDescriptionBuilder();
            superiorBarkWardRetaliationEffect.AddEffectForm(damageEffect.Build());

            return FeatureDefinitionPowerBuilder
                .Create("superiorBarkWardRetaliate", CircleOfTheForestGuardian.BaseGuid)
                .SetGuiPresentationNoContent()
                .Configure(
                    0,
                    UsesDetermination.Fixed,
                    AttributeDefinitions.Wisdom,
                    ActivationTime.NoCost,
                    0,
                    RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Wisdom,
                    superiorBarkWardRetaliationEffect.Build(),
                    true)
                .AddToDB();
        }

        private static FeatureDefinitionDamageAffinity CreateSuperiorBarkWardDamage()
        {
            return FeatureDefinitionDamageAffinityBuilder
                .Create("SuperiorBarkWardRetaliationDamage", CircleOfTheForestGuardian.BaseGuid)
                .SetGuiPresentationNoContent()
                .SetDamageAffinityType(DamageAffinityType.Immunity)
                .SetDamageType(DamageTypePoison)
                .SetRetaliate(CreateSuperiorBarkWardRetaliate(), 1, true)
                .SetAncestryDefinesDamageType(false)
                .AddToDB();
        }

        protected ConditionSuperiorBarkWardBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBarkskin, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionSuperiorBarkWardTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionSuperiorBarkWardDescription";

            Definition.Features.Clear();
            Definition.Features.Add(CreateSuperiorBarkWardDamage());
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(10);
            Definition.SetDurationType(DurationType.Minute);
            Definition.SetTurnOccurence(TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionSuperiorBarkWardBuilder("SuperiorBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "SuperiorBarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("SuperiorBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.BaseGuid, "SuperiorBarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }
}
