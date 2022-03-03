using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier.AttributeModifierOperation;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

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
        private const string DruidForestGuardianDruidSubclassGuid = "45a7595b-5d5f-4351-b7f1-cb78c9d0a136";

        public static readonly Guid DFG_BASE_GUID = new(DruidForestGuardianDruidSubclassGuid);
        public static readonly FeatureDefinitionAutoPreparedSpells druid_forestGuardian_magic = CreateDruidForestGuardianMagic();
        public static readonly FeatureDefinitionAttributeModifier extra_attack = CreateExtraAttack();
        public static readonly FeatureDefinitionAttributeModifier sylvan_resistance = CreateSylvanDurability();
        public static readonly FeatureDefinitionMagicAffinity sylvan_war_magic = CreateSylvanWarMagic();
        public static readonly Dictionary<int, FeatureDefinitionPowerSharedPool> bark_ward_dict = CreateBarkWard();

        public static CharacterSubclassDefinition BuildAndAddSubclass()
        {
            return CharacterSubclassDefinitionBuilder
                .Create(DruidForestGuardianDruidSubclassName, DruidForestGuardianDruidSubclassGuid)
                .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(druid_forestGuardian_magic, 2)
                .AddFeatureAtLevel(sylvan_resistance, 2)
                .AddFeatureAtLevel(sylvan_war_magic, 2)
                .AddFeatureAtLevel(bark_ward_dict[2], 2)
                .AddFeatureAtLevel(extra_attack, 6)
                .AddFeatureAtLevel(bark_ward_dict[10], 10)
                .AddFeatureAtLevel(bark_ward_dict[14], 14)
                .AddToDB();
        }

        // Create Auto-prepared Spell list
        private static FeatureDefinitionAutoPreparedSpells CreateDruidForestGuardianMagic()
        {
            return FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ForestGuardianAutoPreparedSpells", DFG_BASE_GUID)
                .SetGuiPresentation("DruidForestGuardianMagic", Category.Feature)
                .SetPreparedSpellGroups(
                    AutoPreparedSpellsGroupBuilder.Build(2, Shield, FogCloud),
                    AutoPreparedSpellsGroupBuilder.Build(3, Blur, FlameBlade),
                    AutoPreparedSpellsGroupBuilder.Build(5, ProtectionFromEnergy, DispelMagic),
                    AutoPreparedSpellsGroupBuilder.Build(7, FireShield, DeathWard),
                    AutoPreparedSpellsGroupBuilder.Build(9, HoldMonster, GreaterRestoration))
                .SetSpellcastingClass(DatabaseHelper.CharacterClassDefinitions.Druid)
                .AddToDB();
        }

        // Create Sylvan War Magic
        private static FeatureDefinitionMagicAffinity CreateSylvanWarMagic()
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic, "DruidForestGuardianSylvanWarMagic", DFG_BASE_GUID)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }

        // Create Sylvan Durability
        private static FeatureDefinitionAttributeModifier CreateSylvanDurability()
        {
            var guid = GuidHelper.Create(DFG_BASE_GUID, "DruidForestGuardianSylvanDurability").ToString();

            // NOTE: unable to use preferred ctor because of legacy guid generation
            return FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierDruidForestGuardianSylvanDurability", guid)
                .SetGuiPresentation("DruidForestGuardianSylvanDurability", Category.Feature)
                .SetModifier(Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
                .AddToDB();
        }

        // Create Bark Ward Wild Shape Power (and the two higher variants, improved and superior)
        private static Dictionary<int, FeatureDefinitionPowerSharedPool> CreateBarkWard()
        {
            GuiPresentationBuilder barkWardGui = new GuiPresentationBuilder(
               "Feature/&DruidForestGuardianBarkWardTitle",
               "Feature/&DruidForestGuardianBarkWardDescription")
                .SetSpriteReference(PowerDruidWildShape.GuiPresentation.SpriteReference);

            GuiPresentationBuilder improvedBarkWardGui = new GuiPresentationBuilder(
               "Feature/&DruidForestGuardianImprovedBarkWardTitle",
               "Feature/&DruidForestGuardianImprovedBarkWardDescription")
                .SetSpriteReference(PowerDruidWildShape.GuiPresentation.SpriteReference);

            GuiPresentationBuilder superiorBarkWardGui = new GuiPresentationBuilder(
               "Feature/&DruidForestGuardianSuperiorBarkWardTitle",
               "Feature/&DruidForestGuardianSuperiorBarkWardDescription")
                .SetSpriteReference(PowerDruidWildShape.GuiPresentation.SpriteReference);

            EffectFormBuilder tempHPEffect = new EffectFormBuilder();
            tempHPEffect.SetTempHPForm(4, RuleDefinitions.DieType.D1, 0);
            tempHPEffect.SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, RuleDefinitions.LevelSourceType.ClassLevel, 1);
            tempHPEffect.CreatedByCharacter();

            EffectFormBuilder barkWardBuff = new EffectFormBuilder();
            barkWardBuff.SetConditionForm(ConditionBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectFormBuilder improvedBarkWardBuff = new EffectFormBuilder();
            improvedBarkWardBuff.SetConditionForm(ConditionImprovedBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectFormBuilder superiorBarkWardBuff = new EffectFormBuilder();
            superiorBarkWardBuff.SetConditionForm(ConditionSuperiorBarkWardBuilder.GetOrAdd(), ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>());

            EffectDescriptionBuilder barkWardEffectDescription = new EffectDescriptionBuilder();
            barkWardEffectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            barkWardEffectDescription.SetCreatedByCharacter();
            barkWardEffectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 10, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            barkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            barkWardEffectDescription.AddEffectForm(barkWardBuff.Build());
            barkWardEffectDescription.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectDescriptionBuilder improvedBarkWardEffectDescription = new EffectDescriptionBuilder();
            improvedBarkWardEffectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            improvedBarkWardEffectDescription.SetCreatedByCharacter();
            improvedBarkWardEffectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 10, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            improvedBarkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            improvedBarkWardEffectDescription.AddEffectForm(improvedBarkWardBuff.Build());
            improvedBarkWardEffectDescription.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectDescriptionBuilder superiorBarkWardEffectDescription = new EffectDescriptionBuilder();
            superiorBarkWardEffectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            superiorBarkWardEffectDescription.SetCreatedByCharacter();
            superiorBarkWardEffectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 10, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            superiorBarkWardEffectDescription.AddEffectForm(tempHPEffect.Build());
            superiorBarkWardEffectDescription.AddEffectForm(superiorBarkWardBuff.Build());
            superiorBarkWardEffectDescription.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            var barkWard = new FeatureDefinitionPowerSharedPoolBuilder(
                "DruidForestGuardianBarkWard",
                GuidHelper.Create(DFG_BASE_GUID, "DruidForestGuardianBarkWard").ToString(),
                PowerDruidWildShape,
                RuleDefinitions.RechargeRate.ShortRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                barkWardEffectDescription.Build(),
                barkWardGui.Build(),
                true).AddToDB();

            var improvedBarkWard = new FeatureDefinitionPowerSharedPoolBuilder(
                "DruidForestGuardianImprovedBarkWard",
                GuidHelper.Create(DFG_BASE_GUID, "DruidForestGuardianImprovedBarkWard").ToString(),
                PowerDruidWildShape,
                RuleDefinitions.RechargeRate.ShortRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                improvedBarkWardEffectDescription.Build(),
                improvedBarkWardGui.Build(),
                true).AddToDB();
            improvedBarkWard.SetOverriddenPower(barkWard);

            var superiorBarkWard = new FeatureDefinitionPowerSharedPoolBuilder(
                "DruidForestGuardianSuperiorBarkWard",
                GuidHelper.Create(DFG_BASE_GUID, "DruidForestGuardianSuperiorBarkWard").ToString(),
                PowerDruidWildShape,
                RuleDefinitions.RechargeRate.ShortRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                superiorBarkWardEffectDescription.Build(),
                superiorBarkWardGui.Build(),
                true).AddToDB();
            superiorBarkWard.SetOverriddenPower(improvedBarkWard);

            return new Dictionary<int, FeatureDefinitionPowerSharedPool>{
                {2, barkWard},
                {10, improvedBarkWard},
                {14, superiorBarkWard} };
        }

        // Create Extra Attack
        private static FeatureDefinitionAttributeModifier CreateExtraAttack()
        {
            var guid = GuidHelper.Create(DFG_BASE_GUID, "DruidForestGuardianExtraAttack").ToString();

            // NOTE: unable to use preferred ctor because of legacy guid generation
            return FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierDruidForestGuardianExtraAttack", guid)
                .SetGuiPresentation("DruidForestGuardianExtraAttack", Category.Feature)
                .SetModifier(Additive, AttributeDefinitions.AttacksNumber, 1)
                .AddToDB();
        }

        // A builder to help us build a custom damage affinity for our Bark Ward conditions
        internal class FeatureDefinitionDamageAffinityBuilder : Builders.Features.FeatureDefinitionDamageAffinityBuilder
        {
            public FeatureDefinitionDamageAffinityBuilder(string name, string guid, bool retaliateWhenHit, int retaliationRange,
                FeatureDefinitionPower retaliationPower, RuleDefinitions.DamageAffinityType damageAffinityType, string damageType,
                GuiPresentation guiPresentation) : base(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireShieldWarm, name, guid)
            {
                Definition
                    .SetDamageAffinityType(damageAffinityType)
                    .SetDamageType(damageType)
                    .SetRetaliateWhenHit(retaliateWhenHit)
                    .SetRetaliateRangeCells(retaliationRange)
                    .SetRetaliatePower(retaliationPower)
                    .SetGuiPresentation(guiPresentation)
                    .SetAncestryDefinesDamageType(false);
            }
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
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionBarkWardBuilder("BarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "BarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("BarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "BarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal class ConditionImprovedBarkWardBuilder : ConditionDefinitionBuilder
    {
        private static FeatureDefinitionPower CreateImprovedBarkWardRetaliate()
        {
            EffectFormBuilder damageEffect = new EffectFormBuilder();
            damageEffect.SetDamageForm(false, RuleDefinitions.DieType.D8,
                "DamagePiercing",
                0, RuleDefinitions.DieType.D8,
                2, RuleDefinitions.HealFromInflictedDamage.Never,
                new List<RuleDefinitions.TrendInfo>());
            damageEffect.CreatedByCondition();

            EffectDescriptionBuilder improvedBarkWardRetaliationEffect = new EffectDescriptionBuilder();
            improvedBarkWardRetaliationEffect.AddEffectForm(damageEffect.Build());

            return FeatureDefinitionPowerBuilder
                .Create("improvedBarkWardRetaliate", CircleOfTheForestGuardian.DFG_BASE_GUID)
                .SetGuiPresentationNoContent()
                .Configure(
                    0,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Wisdom,
                    RuleDefinitions.ActivationTime.NoCost,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
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
            GuiPresentationBuilder improvedBarkWardDamageGui = new GuiPresentationBuilder(
                "Feature/&NoContentTitle",
                "Feature/&NoContentTitle");

            return new CircleOfTheForestGuardian.FeatureDefinitionDamageAffinityBuilder("ImprovedBarkWardRetaliationDamage",
                GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "ImprovedBarkWardRetaliationDamage").ToString(),
                true,
                1,
                CreateImprovedBarkWardRetaliate(),
                RuleDefinitions.DamageAffinityType.None,
                RuleDefinitions.DamageTypePoison,
                improvedBarkWardDamageGui.Build()).AddToDB();
        }

        protected ConditionImprovedBarkWardBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBarkskin, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionImprovedBarkWardTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionImprovedBarkWardDescription";

            SetFeatures(CreateImprovedBarkWardDamage());
            SetAllowMultipleInstances(false);
            SetDuration(RuleDefinitions.DurationType.Minute, 10);
            SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionImprovedBarkWardBuilder("ImprovedBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "ImprovedBarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ImprovedBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "ImprovedBarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal class ConditionSuperiorBarkWardBuilder : ConditionDefinitionBuilder
    {
        private static FeatureDefinitionPower CreateSuperiorBarkWardRetaliate()
        {
            EffectFormBuilder damageEffect = new EffectFormBuilder();
            damageEffect.SetDamageForm(false, RuleDefinitions.DieType.D8,
                "DamagePiercing",
                0, RuleDefinitions.DieType.D8,
                3, RuleDefinitions.HealFromInflictedDamage.Never,
                new List<RuleDefinitions.TrendInfo>());
            damageEffect.CreatedByCondition();

            EffectDescriptionBuilder superiorBarkWardRetaliationEffect = new EffectDescriptionBuilder();
            superiorBarkWardRetaliationEffect.AddEffectForm(damageEffect.Build());

            return FeatureDefinitionPowerBuilder
                .Create("superiorBarkWardRetaliate", CircleOfTheForestGuardian.DFG_BASE_GUID)
                .SetGuiPresentationNoContent()
                .Configure(
                    0,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Wisdom,
                    RuleDefinitions.ActivationTime.NoCost,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Wisdom,
                    superiorBarkWardRetaliationEffect.Build(),
                    true)
                .AddToDB();
        }

        private static FeatureDefinitionDamageAffinity CreateSuperiorBarkWardDamage()
        {
            GuiPresentationBuilder superiorBarkWardDamageGui = new GuiPresentationBuilder(
                "Feature/&NoContentTitle",
                "Feature/&NoContentTitle");

            return new CircleOfTheForestGuardian.FeatureDefinitionDamageAffinityBuilder("SuperiorBarkWardRetaliationDamage",
                GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "SuperiorBarkWardRetaliationDamage").ToString(),
                true,
                1,
                CreateSuperiorBarkWardRetaliate(),
                RuleDefinitions.DamageAffinityType.Immunity,
                RuleDefinitions.DamageTypePoison,
               superiorBarkWardDamageGui.Build()).AddToDB();
        }

        protected ConditionSuperiorBarkWardBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBarkskin, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionSuperiorBarkWardTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionSuperiorBarkWardDescription";

            Definition.Features.Clear();
            Definition.Features.Add(CreateSuperiorBarkWardDamage());
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(10);
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionSuperiorBarkWardBuilder("SuperiorBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "SuperiorBarkWard").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("SuperiorBarkWard", GuidHelper.Create(CircleOfTheForestGuardian.DFG_BASE_GUID, "SuperiorBarkWard").ToString()) ?? CreateAndAddToDB();
        }
    }
}
