using SolastaModApi;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class AHWarlockSubclassSoulBladePact
    {

        public const string AHWarlockSubclassSoulBladePactName = "AHWarlockSubclassSoulBladePact";
        private static readonly string AHWarlockSubclassSoulBladePactNameGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockSubclassSoulBladePactName).ToString();

        public static CharacterSubclassDefinition Build()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                            "Subclass/&AHWarlockSubclassSoulBladePactTitle",
                            "Subclass/&AHWarlockSubclassSoulBladePactDescription"            )
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.OathOfTheMotherland.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = CharacterSubclassDefinitionBuilder.Create(AHWarlockSubclassSoulBladePactName, AHWarlockSubclassSoulBladePactNameGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(AHWarlockSoulBladePactExtendedSpellListMagicAffinityBuilder.AHSoulBladeSpellList, 1) // Extra Soulblade spells
                    .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon, 1) // Martial weapons
                    .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyClericArmor, 1) // Medium armor and shield
                    .AddFeatureAtLevel(AHWarlockSoulBladePactEmpowerWeaponPowerBuilder.AHWarlockSoulBladePactEmpowerWeaponPower, 1) //Feature to rival hexblade curse
                    .AddFeatureAtLevel(SoulBladeSummonPactWeaponPowerBuilder.SummonPactWeaponPower, 6)
                    .AddFeatureAtLevel(AHWarlockSoulBladePactSoulShieldPowerBuilder.AHWarlockSoulBladePactSoulShieldPower, 10)
                    .AddToDB();

            return definition;
        }
    }

    internal class AHWarlockSoulBladePactSpellListBuilder : SpellListDefinitionBuilder
    {
        const string AHWarlockSoulBladePactSpellListName = "AHWarlockSoulBladePactSpellList";
        private static readonly string AHWarlockSoulBladePactSpellListGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockSoulBladePactSpellListName).ToString();

        protected AHWarlockSoulBladePactSpellListBuilder(string name, string guid) : base(DatabaseHelper.SpellListDefinitions.SpellListPaladin, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockSoulBladePactSpellListTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockSoulBladePactSpellListDescription";
            Definition.SpellsByLevel.Clear();
            Definition.SetMaxSpellLevel(5);


            //Seems to need a blank cantrip list?
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>(),
                Level = 0
            });

            //Level 1
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>() {
                DatabaseHelper.SpellDefinitions.Shield,
                DatabaseHelper.SpellDefinitions.FalseLife
                },
                Level = 1
            });

            //Level 2
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>() {
                DatabaseHelper.SpellDefinitions.Blur,
                DatabaseHelper.SpellDefinitions.BrandingSmite, },
                Level = 2
            });

            //Level 3
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>() {
                DatabaseHelper.SpellDefinitions.Haste,
                DatabaseHelper.SpellDefinitions.Slow,
                },
                Level = 3
            });

            //Level 4
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>() {
                DatabaseHelper.SpellDefinitions.PhantasmalKiller,
                DatabaseHelper.SpellDefinitions.BlackTentacles,
                },
                Level = 4
            });

            //Level 5
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet()
            {
                Spells = new List<SpellDefinition>() {
                DatabaseHelper.SpellDefinitions.ConeOfCold,
                DatabaseHelper.SpellDefinitions.MindTwist,
                },
                Level = 5
            });
        }

        public static SpellListDefinition CreateAndAddToDB(string name, string guid)
            => new AHWarlockSoulBladePactSpellListBuilder(name, guid).AddToDB();

        public static SpellListDefinition AHWarlockSoulBladePactSpellList = CreateAndAddToDB(AHWarlockSoulBladePactSpellListName, AHWarlockSoulBladePactSpellListGuid);
    }

    internal class AHWarlockSoulBladePactExtendedSpellListMagicAffinityBuilder : FeatureDefinitionMagicAffinityBuilder
    {
        const string Name = "AHWarlockSoulBladePactExtendedSpellList";
        private static readonly string Guid = GuidHelper.Create(new Guid(Settings.GUID), Name).ToString();

        protected AHWarlockSoulBladePactExtendedSpellListMagicAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockSoulBladePactExtendedSpellListTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockSoulBladePactExtendedSpellListDescription";
            Definition.SetExtendedSpellList(AHWarlockSoulBladePactSpellListBuilder.AHWarlockSoulBladePactSpellList);
        }

        public static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
            => new AHWarlockSoulBladePactExtendedSpellListMagicAffinityBuilder(name, guid).AddToDB();

        public static FeatureDefinitionMagicAffinity AHSoulBladeSpellList = CreateAndAddToDB(Name, Guid);
    }


    internal class AHWarlockSoulBladePactEmpowerWeaponPowerBuilder : FeatureDefinitionPowerBuilder
    {
        const string AHWarlockSoulBladePactEmpowerWeaponPowerName = "AHWarlockSoulBladePactEmpowerWeaponPower";
        private static readonly string AHWarlockSoulBladePactEmpowerWeaponPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockSoulBladePactEmpowerWeaponPowerName).ToString();

        protected AHWarlockSoulBladePactEmpowerWeaponPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockSoulBladePactEmpowerWeaponPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockSoulBladePactEmpowerWeaponPowerDescription";
            Definition.SetShortTitleOverride("Feature/&AHWarlockSoulBladePactEmpowerWeaponPowerTitle");

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(1);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);

            //Create additional charisma attack item property
            var conditionForm = new ConditionForm();
            conditionForm.ConditionDefinition = AHSoulBladeEmpowerWeaponConditionBuilder.AHSoulBladeEmpowerWeaponCondition;
            conditionForm.SetForceOnSelf(true);
            var conditionEffect = new EffectForm();
            conditionEffect.FormType = EffectForm.EffectFormType.Condition;
            conditionEffect.ConditionForm = conditionForm;

            //Add the damage to the same effect of sacred weapon gives +cha to hit and damage
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
            newEffectDescription.DurationParameter = 1;
            newEffectDescription.EffectForms.Add(conditionEffect);
            newEffectDescription.EffectForms.AddRange(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.EffectDescription.EffectForms);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new AHWarlockSoulBladePactEmpowerWeaponPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower AHWarlockSoulBladePactEmpowerWeaponPower = CreateAndAddToDB(AHWarlockSoulBladePactEmpowerWeaponPowerName, AHWarlockSoulBladePactEmpowerWeaponPowerGuid);
    }

    internal class AHSoulBladeEmpowerWeaponDamageBonusBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        const string AHSoulBladeEmpowerWeaponAttackAndDamageBonusName = "AHSoulBladeEmpowerWeaponDamageBonus";
        private static readonly string AHSoulBladeEmpowerWeaponAttackAndDamageBonusGuid = GuidHelper.Create(new Guid(Settings.GUID), AHSoulBladeEmpowerWeaponAttackAndDamageBonusName).ToString();

        protected AHSoulBladeEmpowerWeaponDamageBonusBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHSoulBladeEmpowerDamageBonusTitle";
            Definition.GuiPresentation.Description = "Feature/&AHSoulBladeEmpowerDamageBonusTitleDescription";

            Definition.SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.None);
            Definition.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage);
            Definition.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus);
            Definition.SetNotificationTag("SoulEmpowered");
        }

        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
            => new AHSoulBladeEmpowerWeaponDamageBonusBuilder(name, guid).AddToDB();

        public static FeatureDefinitionAdditionalDamage EmpowerWeaponDamageBonus
            = CreateAndAddToDB(AHSoulBladeEmpowerWeaponAttackAndDamageBonusName, AHSoulBladeEmpowerWeaponAttackAndDamageBonusGuid);
    }


    internal class AHSoulBladeEmpowerWeaponConditionBuilder : ConditionDefinitionBuilder
    {
        const string AHSoulBladeEmpowerWeaponConditionName = "AHSoulBladeEmpowerWeaponCondition";
        private static readonly string AHSoulBladeEmpowerWeaponConditionNameGuid = GuidHelper.Create(new Guid(Settings.GUID), AHSoulBladeEmpowerWeaponConditionName).ToString();

        protected AHSoulBladeEmpowerWeaponConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHSoulBladeEmpowerWeaponConditionTitle";
            Definition.GuiPresentation.Description = "Feature/&AHSoulBladeEmpowerWeaponConditionDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();

            Definition.Features.Add(AHSoulBladeEmpowerWeaponDamageBonusBuilder.EmpowerWeaponDamageBonus);
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetDurationParameter(1);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new AHSoulBladeEmpowerWeaponConditionBuilder(name, guid).AddToDB();

        public static ConditionDefinition AHSoulBladeEmpowerWeaponCondition
            = CreateAndAddToDB(AHSoulBladeEmpowerWeaponConditionName, AHSoulBladeEmpowerWeaponConditionNameGuid);
    }

    internal class AHWarlockSoulBladePactSoulShieldPowerBuilder : FeatureDefinitionPowerBuilder
    {
        const string AHWarlockSoulBladePactSoulShieldPowerName = "AHWarlockSoulBladePactSoulShieldPower";
        private static readonly string AHWarlockSoulBladePactSoulShieldPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockSoulBladePactSoulShieldPowerName).ToString();

        protected AHWarlockSoulBladePactSoulShieldPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockSoulBladePactSoulShieldPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockSoulBladePactSoulShieldPowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(1);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetAbilityScore("Charisma");

            //Create the temp hp effect
            EffectForm tempHPEffect = new EffectForm();
            tempHPEffect.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
            var tempHPForm = new TemporaryHitPointsForm();
            tempHPForm.DieType = RuleDefinitions.DieType.D1;
            tempHPForm.BonusHitPoints = -1;
            tempHPEffect.SetTemporaryHitPointsForm(tempHPForm);
            tempHPEffect.SetApplyLevel(EffectForm.LevelApplianceType.AddBonus);
            tempHPEffect.SetAddBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(tempHPEffect);
            newEffectDescription.DurationType = RuleDefinitions.DurationType.UntilLongRest;

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new AHWarlockSoulBladePactSoulShieldPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower AHWarlockSoulBladePactSoulShieldPower = CreateAndAddToDB(AHWarlockSoulBladePactSoulShieldPowerName, AHWarlockSoulBladePactSoulShieldPowerGuid);
    }


    public class SoulBladeSummonPactWeaponPowerBuilder : FeatureDefinitionPowerBuilder 
    {
        private const string SoulBladeSummonPactWeaponPowerName = "AHSoulBladeSummonPactWeaponPower";
        private static readonly string SoulBladeSummonPactWeaponPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), SoulBladeSummonPactWeaponPowerName).ToString();

        protected SoulBladeSummonPactWeaponPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury, name, guid)
        {
            Definition.GuiPresentation.Title =       "Feature/&AHSoulBladeSummonPactWeaponPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&AHSoulBladeSummonPactWeaponPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.SpiritualWeapon.GuiPresentation.SpriteReference);
            Definition.SetEffectDescription(DatabaseHelper.SpellDefinitions.SpiritualWeapon.EffectDescription);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SoulBladeSummonPactWeaponPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower SummonPactWeaponPower = CreateAndAddToDB(SoulBladeSummonPactWeaponPowerName, SoulBladeSummonPactWeaponPowerGuid);
    }

}
