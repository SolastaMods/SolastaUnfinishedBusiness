using System.Collections.Generic;
using System.Runtime.Serialization;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses;

internal static class ScoutSentinelTinkererSubclassBuilder
{
    public const string Name = "ScoutSentinel";
    public const string Guid = "fb2e5f73-d552-430f-b329-1f0a2ecdf6bd";

    // ArmorModePool needs to be instantiated before the two modes.
    private static FeatureDefinitionPower armorModePool;

    private static FeatureDefinitionPowerSharedPool scoutModePower;

    private static FeatureDefinitionPowerSharedPool sentinelModePower;
    public static FeatureDefinitionPower ArmorModePool => armorModePool ??= CreateArmorModePool();
    public static FeatureDefinitionPowerSharedPool ScoutModePower => scoutModePower ??= CreateScoutModePower();

    public static FeatureDefinitionPowerSharedPool SentinelModePower =>
        sentinelModePower ??= CreateSentinelModePower();

    private static EffectForm CreateEffectItem()
    {
        var itemPropertyForm = new ItemPropertyForm();
        itemPropertyForm.FeatureBySlotLevel.Add(
            new FeatureUnlockByLevel(IntToAttackAndDamageBuilder.IntToAttackAndDamage, 0));
        var effectItem = new EffectForm {FormType = EffectForm.EffectFormType.ItemProperty};
        effectItem.itemPropertyForm = itemPropertyForm;
        return effectItem;
    }

    private static FeatureDefinitionPowerSharedPool CreateScoutModePower()
    {
        var guiPresentationScout = new GuiPresentation();
        guiPresentationScout.description = "Feature/&ScoutModePowerDescription";
        guiPresentationScout.title = "Feature/&ScoutModePowerTitle";
        guiPresentationScout.spriteReference = ShadowArmor.GuiPresentation.SpriteReference;
        guiPresentationScout.sortOrder = 1;

        var effectScoutMode = new EffectDescription();
        effectScoutMode.Copy(ProduceFlameHold.EffectDescription);
        effectScoutMode.SlotTypes.Clear();
        effectScoutMode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
        effectScoutMode.durationType = RuleDefinitions.DurationType.UntilAnyRest;
        effectScoutMode.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);
        effectScoutMode.EffectForms[0].SummonForm.itemDefinition = ScoutSuitWeaponBuilder.ScoutSuitWeapon;
        effectScoutMode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
        effectScoutMode.EffectForms[0].SummonForm.trackItem = false;
        effectScoutMode.EffectForms[0].SummonForm.number = 1;
        effectScoutMode.EffectForms.Add(CreateEffectItem());

        return new FeatureDefinitionPowerSharedPoolBuilder(
                "ScoutModePower" // string name
                , "ff6b9eb1-01ad-4100-ab12-7d6dc38ccc70" // string guid
                , ArmorModePool // FeatureDefinitionPower poolPower
                , RuleDefinitions.RechargeRate.ShortRest // RuleDefinitions.RechargeRate recharge
                , RuleDefinitions.ActivationTime.NoCost // RuleDefinitions.ActivationTime activationTime
                , 1 // int costPerUse
                , false // bool proficiencyBonusToAttack
                , false // bool abilityScoreBonusToAttack
                , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name // string abilityScore
                , effectScoutMode // EffectDescription effectDescription
                , guiPresentationScout // GuiPresentation guiPresentation
                , true // bool uniqueInstanc
            )
            .AddToDB();
    }

    private static FeatureDefinitionPowerSharedPool CreateSentinelModePower()
    {
        var effectsentinelmode = new EffectDescription();
        effectsentinelmode.Copy(ProduceFlameHold.EffectDescription);
        effectsentinelmode.SlotTypes.Clear();
        effectsentinelmode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
        effectsentinelmode.durationType = RuleDefinitions.DurationType.UntilAnyRest;
        effectsentinelmode.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);
        effectsentinelmode.EffectForms[0].SummonForm
            .itemDefinition = SentinelSuitWeaponBuilder.SentinelSuitWeapon;
        effectsentinelmode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
        effectsentinelmode.EffectForms[0].SummonForm.trackItem = false;
        effectsentinelmode.EffectForms[0].SummonForm.number = 1;
        effectsentinelmode.EffectForms.Add(CreateEffectItem());

        var guiPresentationSentinel = new GuiPresentation();
        guiPresentationSentinel.description = "Feature/&SentinelModePowerDescription";
        guiPresentationSentinel.title = "Feature/&SentinelModePowerTitle";
        guiPresentationSentinel.spriteReference = MageArmor.GuiPresentation.SpriteReference;

        return new FeatureDefinitionPowerSharedPoolBuilder
        (
            "SentinelModePower" // string name
            , "410768a3-757f-48ee-8a2f-bffd963c0a5b" // string guid
            , ArmorModePool // FeatureDefinitionPower poolPower
            , RuleDefinitions.RechargeRate.ShortRest // RuleDefinitions.RechargeRate recharge
            , RuleDefinitions.ActivationTime.NoCost // RuleDefinitions.ActivationTime activationTime
            , 1 // int costPerUse
            , false // bool proficiencyBonusToAttack
            , false // bool abilityScoreBonusToAttack
            , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name // string abilityScore
            , effectsentinelmode // EffectDescription effectDescription
            , guiPresentationSentinel // GuiPresentation guiPresentation
            , true // bool uniqueInstanc
        ).AddToDB();
    }

    private static FeatureDefinitionPower CreateArmorModePool()
    {
        return FeatureDefinitionPowerPoolBuilder
            .Create("ArmorModePool", "fd0567d8-a728-4459-8569-273f3ead3f73")
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.RechargeRate.ShortRest)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    public static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        return CharacterSubclassDefinitionBuilder
            .Create(Name, Guid)
            .SetGuiPresentation("ScoutSentinelTinkererSubclass", Category.Subclass,
                MartialMountaineer.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(ScoutSentinelAutopreparedSpellsBuilder.SubclassAutopreparedSpells, 3)
            .AddFeatureAtLevel(ScoutSentinelFeatureSet03Builder.ScoutSentinelFeatureSet03, 3)
            .AddFeatureAtLevel(ScoutSentinelFeatureSet05Builder.ScoutSentinelFeatureSet05, 5)
            .AddFeatureAtLevel(ScoutSentinelFeatureSet09Builder.ScoutSentinelFeatureSet09, 9)
            .AddFeatureAtLevel(ScoutSentinelFeatureSet15Builder.ScoutSentinelFeatureSet15, 15)
            .AddToDB();
    }
}

internal sealed class ScoutSentinelFeatureSet03Builder : FeatureDefinitionFeatureSetBuilder
{
    private const string ScoutSentinelFeatureSet_level03Name = "ScoutSentinelFeatureSet_level03";
    private const string ScoutSentinelFeatureSet_level03Guid = "a6560212-c665-49fd-94b7-378512e68edb";

    public static readonly FeatureDefinitionFeatureSet ScoutSentinelFeatureSet03 =
        CreateAndAddToDB(ScoutSentinelFeatureSet_level03Name, ScoutSentinelFeatureSet_level03Guid);

    private ScoutSentinelFeatureSet03Builder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level03Title";
        Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level03Description";

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(ScoutSentinelTinkererSubclassBuilder.ArmorModePool);
        Definition.FeatureSet.Add(ScoutSentinelTinkererSubclassBuilder.ScoutModePower);
        Definition.FeatureSet.Add(ScoutSentinelTinkererSubclassBuilder.SentinelModePower);
        Definition.FeatureSet.Add(SubclassProficienciesBuilder.SubclassProficiencies);
        Definition.FeatureSet.Add(UseArmorWeaponsAsFocusBuilder.UseArmorWeaponsAsFocus);
        Definition.FeatureSet.Add(SubclassMovementAffinitiesBuilder.SubclassMovementAffinities);
        // Definition.FeatureSet.Add(ScoutSentinelAutopreparedSpellsBuilder.SubclassAutopreparedSpells);
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSentinelFeatureSet03Builder(name, guid).AddToDB();
    }
}

internal sealed class ScoutSentinelFeatureSet05Builder : FeatureDefinitionFeatureSetBuilder
{
    private const string ScoutSentinelFeatureSet_level05Name = "ScoutSentinelFeatureSet_level05";
    private const string ScoutSentinelFeatureSet_level05Guid = "a881c02a-7add-426b-a2d4-1f3994d12fa9";

    public static readonly FeatureDefinitionFeatureSet ScoutSentinelFeatureSet05 =
        CreateAndAddToDB(ScoutSentinelFeatureSet_level05Name, ScoutSentinelFeatureSet_level05Guid);

    private ScoutSentinelFeatureSet05Builder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level05Title";
        Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level05Description";

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttributeModifiers
            .AttributeModifierFighterExtraAttack);
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSentinelFeatureSet05Builder(name, guid).AddToDB();
    }
}

internal sealed class ScoutSentinelFeatureSet09Builder : FeatureDefinitionFeatureSetBuilder
{
    private const string ScoutSentinelFeatureSet_level09Name = "ScoutSentinelFeatureSet_level09";
    private const string ScoutSentinelFeatureSet_level09Guid = "87e8b110-4590-4791-b31c-b8bba5f362b1";

    public static readonly FeatureDefinitionFeatureSet ScoutSentinelFeatureSet09 =
        CreateAndAddToDB(ScoutSentinelFeatureSet_level09Name, ScoutSentinelFeatureSet_level09Guid);

    private ScoutSentinelFeatureSet09Builder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level09Title";
        Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level09Description";

        var ExtraInfusionSlots = FeatureDefinitionPowerPoolModifierBuilder
            .Create("ExtraInfusionSlots", "350902fd-cf99-48e4-8edc-115c82886bdb")
            .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                TinkererClass.InfusionPool)
            .SetGuiPresentation(Category.Feat, null, 1)
            .AddToDB();

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(ExtraInfusionSlots);
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSentinelFeatureSet09Builder(name, guid).AddToDB();
    }
}

internal sealed class ScoutSentinelFeatureSet15Builder : FeatureDefinitionFeatureSetBuilder
{
    private const string ScoutSentinelFeatureSet_level15Name = "ScoutSentinelFeatureSet_level15";
    private const string ScoutSentinelFeatureSet_level15Guid = "69a9ba53-4949-47ec-9693-467d053e4646";

    public static readonly FeatureDefinitionFeatureSet ScoutSentinelFeatureSet15 =
        CreateAndAddToDB(ScoutSentinelFeatureSet_level15Name, ScoutSentinelFeatureSet_level15Guid);

    private ScoutSentinelFeatureSet15Builder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level15Title";
        Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level15Description";

        Definition.FeatureSet.Clear();

        var guiPresentationImprovedSentinel = new GuiPresentation();
        guiPresentationImprovedSentinel.description = "Feature/&ImprovedSentinelModePowerDescription";
        guiPresentationImprovedSentinel.title = "Feature/&ImprovedSentinelModePowerTitle";
        guiPresentationImprovedSentinel.spriteReference = MageArmor.GuiPresentation.SpriteReference;

        var itemPropertyForm = new ItemPropertyForm();
        itemPropertyForm.FeatureBySlotLevel.Add(
            new FeatureUnlockByLevel(IntToAttackAndDamageBuilder.IntToAttackAndDamage, 0));
        var effectItem = new EffectForm {FormType = EffectForm.EffectFormType.ItemProperty};
        effectItem.itemPropertyForm = itemPropertyForm;

        var effectImprovedSentinelmode = new EffectDescription();
        effectImprovedSentinelmode.Copy(ProduceFlameHold.EffectDescription);
        effectImprovedSentinelmode.SlotTypes.Clear();
        effectImprovedSentinelmode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
        effectImprovedSentinelmode.durationType = RuleDefinitions.DurationType.UntilAnyRest;
        effectImprovedSentinelmode.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);
        effectImprovedSentinelmode.EffectForms[0].SummonForm
            .itemDefinition = ImprovedSentinelSuitWeaponBuilder.ImprovedSentinelSuitWeapon;
        effectImprovedSentinelmode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
        effectImprovedSentinelmode.EffectForms[0].SummonForm.trackItem = false;
        effectImprovedSentinelmode.EffectForms[0].SummonForm.number = 1;
        effectImprovedSentinelmode.EffectForms.Add(effectItem);

        var Improvedsentinelmodepowerbuilder = new FeatureDefinitionPowerSharedPoolBuilder
        (
            "ImprovedSentinelModePower" // string name
            , "b216988a-5905-47fd-9359-093cd77d41f9" // string guid
            , ScoutSentinelTinkererSubclassBuilder.ArmorModePool // FeatureDefinitionPower poolPower
            , RuleDefinitions.RechargeRate.ShortRest // RuleDefinitions.RechargeRate recharge
            , RuleDefinitions.ActivationTime.NoCost // RuleDefinitions.ActivationTime activationTime
            , 1 // int costPerUse
            , false // bool proficiencyBonusToAttack
            , false // bool abilityScoreBonusToAttack
            , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name // string abilityScore
            , effectImprovedSentinelmode // EffectDescription effectDescription
            , guiPresentationImprovedSentinel // GuiPresentation guiPresentation
            , true // bool uniqueInstanc
        );
        var Improvedsentinelmodepower = Improvedsentinelmodepowerbuilder.AddToDB();

        Improvedsentinelmodepower.overriddenPower = ScoutSentinelTinkererSubclassBuilder.SentinelModePower;

        var guiPresentationImprovedScout = new GuiPresentation();
        guiPresentationImprovedScout.description = "Feature/&ImprovedScoutModePowerDescription";
        guiPresentationImprovedScout.title = "Feature/&ImprovedScoutModePowerTitle";
        guiPresentationImprovedScout.spriteReference = ShadowArmor.GuiPresentation.SpriteReference;
        guiPresentationImprovedScout.sortOrder = 1;

        var effectImprovedScoutMode = new EffectDescription();
        effectImprovedScoutMode.Copy(ProduceFlameHold.EffectDescription);
        effectImprovedScoutMode.SlotTypes.Clear();
        effectImprovedScoutMode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
        effectImprovedScoutMode.durationType = RuleDefinitions.DurationType.UntilAnyRest;
        effectImprovedScoutMode.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);
        effectImprovedScoutMode.EffectForms[0].SummonForm
            .itemDefinition = ImprovedScoutSuitWeaponBuilder.ImprovedScoutSuitWeapon;
        effectImprovedScoutMode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
        effectImprovedScoutMode.EffectForms[0].SummonForm.trackItem = false;
        effectImprovedScoutMode.EffectForms[0].SummonForm.number = 1;
        effectImprovedScoutMode.EffectForms.Add(effectItem);

        var Improvedscoutmodepowerbuilder = new FeatureDefinitionPowerSharedPoolBuilder
        (
            "ImprovedScoutModePower" // string name
            , "89ec2647-5560-4e82-aa7b-59a8489de492" // string guid
            , ScoutSentinelTinkererSubclassBuilder.ArmorModePool // FeatureDefinitionPower poolPower
            , RuleDefinitions.RechargeRate.ShortRest // RuleDefinitions.RechargeRate recharge
            , RuleDefinitions.ActivationTime.NoCost // RuleDefinitions.ActivationTime activationTime
            , 1 // int costPerUse
            , false // bool proficiencyBonusToAttack
            , false // bool abilityScoreBonusToAttack
            , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name // string abilityScore
            , effectImprovedScoutMode // EffectDescription effectDescription
            , guiPresentationImprovedScout // GuiPresentation guiPresentation
            , true // bool uniqueInstanc
        );
        var Improvedscoutmodepower = Improvedscoutmodepowerbuilder.AddToDB();

        Improvedscoutmodepower.overriddenPower = ScoutSentinelTinkererSubclassBuilder.ScoutModePower;

        Definition.FeatureSet.Add(Improvedscoutmodepower);
        Definition.FeatureSet.Add(Improvedsentinelmodepower);
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSentinelFeatureSet15Builder(name, guid).AddToDB();
    }
}

//*****************************************************************************************************************************************
//***********************************		SubclassAutopreparedSpellsBuilder		*******************************************************
//*****************************************************************************************************************************************

internal sealed class ScoutSentinelAutopreparedSpellsBuilder : FeatureDefinitionAutoPreparedSpellsBuilder
{
    private const string SubclassAutopreparedSpellsName = "SubclassAutopreparedSpells";
    private const string SubclassAutopreparedSpellsGuid = "c5f03caa-7078-4da7-b166-00f292fcebfc";

    public static readonly FeatureDefinitionAutoPreparedSpells SubclassAutopreparedSpells =
        CreateAndAddToDB(SubclassAutopreparedSpellsName, SubclassAutopreparedSpellsGuid);

    private ScoutSentinelAutopreparedSpellsBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&AutoPreparedSpellsTitle";
        Definition.GuiPresentation.Description = "Feat/&AutoPreparedSpellsDescription";

        SetPreparedSpellGroups(
            BuildSpellGroup(3, Thunderwave, MagicMissile),
            BuildSpellGroup(5, Shatter, Blur),
            BuildSpellGroup(9, LightningBolt, HypnoticPattern),
            BuildSpellGroup(13, FireShield, GreaterInvisibility),
            // added extra spells to balance spells withput "implemented"=true flag yet
            // blur for mirror image
            // dimension door for passwall
            // wall of fire (4th lvl) and wind wall (3th lvl) for wall of force (5th lvl)
            BuildSpellGroup(17, DimensionDoor, WallOfFire, WindWall));

        // todo: refactor so the Tinkerer class can easily get passed in to BuildAndAddSubclass and into the auto prepared spells builder instead of using a getter.
        var tinkerer = DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement("ClassTinkerer",
            GuidHelper.Create(TinkererClass.GuidNamespace, "ClassTinkerer").ToString());
        Definition.spellcastingClass = tinkerer;
    }

    private static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSentinelAutopreparedSpellsBuilder(name, guid).AddToDB();
    }
}

//*****************************************************************************************************************************************
//***********************************		SubclassProficienciesBuilder		*******************************************************
//*****************************************************************************************************************************************

internal sealed class SubclassProficienciesBuilder : FeatureDefinitionProficiencyBuilder
{
    private const string SubclassProficienciesName = "SubclassProficiencies";
    private const string SubclassProficienciesGuid = "1923caf6-672b-475a-bcdf-50d535ce65d1";

    public static readonly FeatureDefinitionProficiency SubclassProficiencies =
        CreateAndAddToDB(SubclassProficienciesName, SubclassProficienciesGuid);

    private SubclassProficienciesBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDomainLifeArmor, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&SubclassProficienciesTitle"; //Feature/&NoContentTitle
        Definition.GuiPresentation.Description = "Feat/&SubclassProficienciesDescription"; //Feature/&NoContentTitle
        Definition.Proficiencies.Add(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory.Name);
    }

    private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
    {
        return new SubclassProficienciesBuilder(name, guid).AddToDB();
    }
}

//*****************************************************************************************************************************************
//***********************************		SubclassMovementAffinitiesBuilder		*******************************************************
//*****************************************************************************************************************************************

internal sealed class SubclassMovementAffinitiesBuilder : FeatureDefinitionMovementAffinityBuilder
{
    private const string SubclassMovementAffinitiesName = "SubclassMovementAffinities";
    private const string SubclassMovementAffinitiesGuid = "14ef799a-edc6-4749-866b-9a6afc26d4fa";

    public static readonly FeatureDefinitionMovementAffinity SubclassMovementAffinities =
        CreateAndAddToDB(SubclassMovementAffinitiesName, SubclassMovementAffinitiesGuid);

    private SubclassMovementAffinitiesBuilder(string name, string guid) : base(name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&SubclassMovementTitle";
        Definition.GuiPresentation.Description = "Feat/&SubclassMovementDescription";
        Definition.heavyArmorImmunity = true;
    }

    private static FeatureDefinitionMovementAffinity CreateAndAddToDB(string name, string guid)
    {
        return new SubclassMovementAffinitiesBuilder(name, guid).AddToDB();
    }
}

//*****************************************************************************************************************************************
//***********************************		UseArmorWeaponsAsFocusBuilder		*******************************************************
//*****************************************************************************************************************************************

internal sealed class UseArmorWeaponsAsFocusBuilder : FeatureDefinitionMagicAffinityBuilder
{
    private const string UseArmorWeaponsAsFocusName = "UseArmorWeaponsAsFocus";
    private const string UseArmorWeaponsAsFocusGuid = "55a4d71f-2b9d-4df6-abf1-c0cc6682eb9d";

    public static readonly FeatureDefinitionMagicAffinity UseArmorWeaponsAsFocus =
        CreateAndAddToDB(UseArmorWeaponsAsFocusName, UseArmorWeaponsAsFocusGuid);

    private UseArmorWeaponsAsFocusBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&UseArmorWeaponsAsFocusTitle";
        Definition.GuiPresentation.Description = "Feat/&UseArmorWeaponsAsFocusDescription";
        Definition.canUseProficientWeaponAsFocus = true;
        Definition.somaticWithWeapon = true;
        Definition.rangeSpellNoProximityPenalty = false;
    }

    private static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
    {
        return new UseArmorWeaponsAsFocusBuilder(name, guid).AddToDB();
    }
}

//*****************************************************************************************************************************************
//***********************************		IntToAttackAndDamageBuilder		*******************************************************
//*****************************************************************************************************************************************

internal sealed class IntToAttackAndDamageBuilder : FeatureDefinitionAttackModifierBuilder
{
    private const string IntToAttackAndDamageName = "IntToAttackAndDamage";
    private const string IntToAttackAndDamageGuid = "ebb243f7-382c-4caf-9d7f-40c80dab4623";

    public static readonly FeatureDefinitionAttackModifier IntToAttackAndDamage =
        CreateAndAddToDB(IntToAttackAndDamageName, IntToAttackAndDamageGuid);

    private IntToAttackAndDamageBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierShillelagh, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&IntToAttackAndDamageTitle";
        Definition.GuiPresentation.Description = "Feat/&IntToAttackAndDamageDescription";

        Definition.damageDieReplacement = RuleDefinitions.DamageDieReplacement.None;
        Definition.canAddAbilityBonusToSecondary = true;
        Definition.abilityScoreReplacement = RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility;

        //    AssetReference assetReference = new AssetReference();
        //    assetReference.SetField("m_AssetGUID", "ad68a1be3193a314c911afd02ca8d360");
        //    Definition.impactParticleReference = assetReference;
    }

    private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
    {
        return new IntToAttackAndDamageBuilder(name, guid).AddToDB();
    }
}

//*************************************************************************************************************************
//***********************************		Sentinel Suit Weapon           		*******************************************
//*************************************************************************************************************************

internal sealed class SentinelSuitWeaponBuilder : ItemDefinitionBuilder
{
    private const string SentinelSuitWeaponName = "SentinelSuitWeapon";
    private const string SentinelSuitWeaponGuid = "c86a1f25-3364-42bc-92b8-64f1358cbf15";

    public static readonly ItemDefinition SentinelSuitWeapon =
        CreateAndAddToDB(SentinelSuitWeaponName, SentinelSuitWeaponGuid);

    private SentinelSuitWeaponBuilder(string name, string guid) : base(
        DatabaseHelper.ItemDefinitions.UnarmedStrikeBase, name, guid)
    {
        // can only take 3 ( at game launch in may, havent checked since)
        Definition.IsFocusItem = true;
        Definition.IsUsableDevice = true;
        Definition.IsWeapon = true;
        //         Definition.IsFood = true;

        Definition.inDungeonEditor = true;

        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D8,
                BonusDamage = 0,
                DamageType = "DamageThunder"
            },
            AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
        };

        var ThunderStruckEffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        ThunderStruckEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        ThunderStruckEffect.ConditionForm.ConditionDefinition = ThunderStruckConditionBuilder.ThunderStruck;

        var balancingeffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        balancingeffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        balancingeffect.ConditionForm.ConditionDefinition =
            ThunderStruckBalancingAdvantageConditionBuilder.ThunderStruckBalancingAdvantage;

        //Add to our new effect
        var newEffectDescription = new EffectDescription();
        newEffectDescription.Copy(Definition.WeaponDescription.EffectDescription);
        newEffectDescription.EffectForms.Clear();
        newEffectDescription.EffectForms.Add(damageEffect);
        newEffectDescription.EffectForms.Add(balancingeffect);
        newEffectDescription.EffectForms.Add(ThunderStruckEffect);

        newEffectDescription.HasSavingThrow = false;
        newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        newEffectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        newEffectDescription.SetTargetProximityDistance(1);
        newEffectDescription.SetCanBePlacedOnCharacter(true);
        newEffectDescription.SetRangeType(RuleDefinitions.RangeType.MeleeHit);

        newEffectDescription.SetEffectParticleParameters(Shatter.EffectDescription.EffectParticleParameters);

        var ThunderPunch = new WeaponDescription();
        ThunderPunch.weaponType = "UnarmedStrikeType";
        ThunderPunch.reachRange = 1;
        ThunderPunch.maxRange = 1;
        ThunderPunch.closeRange = 1;
        ThunderPunch.ammunitionType = "";
        ThunderPunch.effectDescription = newEffectDescription;
        ThunderPunch.WeaponTags.Add("ScoutSentinelWeapon");

        var usingBonusActionItemPower =
            (ItemPropertyDescription)FormatterServices.GetUninitializedObject(typeof(ItemPropertyDescription));
        usingBonusActionItemPower.featureDefinition = UsingitemPowerBuilder.UsingitemPower;
        usingBonusActionItemPower.type = ItemPropertyDescription.PropertyType.Feature;
        usingBonusActionItemPower.knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden;

        var deviceFunctionDescription = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions
            .PotionOfComprehendLanguages.UsableDeviceDescription.DeviceFunctions[0]);
        deviceFunctionDescription.canOverchargeSpell = false;
        deviceFunctionDescription.durationType = RuleDefinitions.DurationType.UntilLongRest;
        deviceFunctionDescription.featureDefinitionPower = ThunderShieldBuilder.ThunderShield;
        deviceFunctionDescription.parentUsage = EquipmentDefinitions.ItemUsage.ByFunction;
        deviceFunctionDescription.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        deviceFunctionDescription.type = DeviceFunctionDescription.FunctionType.Power;
        deviceFunctionDescription.useAffinity = DeviceFunctionDescription.FunctionUseAffinity
            .IterationPerRecharge;
        deviceFunctionDescription.useAmount = 6;

        var usableDeviceDescription = new UsableDeviceDescription();
        usableDeviceDescription.usage = EquipmentDefinitions.ItemUsage.ByFunction;
        usableDeviceDescription.chargesCapitalNumber = 5;
        usableDeviceDescription.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        usableDeviceDescription.rechargeNumber = 0;
        usableDeviceDescription.rechargeDie = RuleDefinitions.DieType.D1;
        usableDeviceDescription.rechargeBonus = 5;
        usableDeviceDescription.outOfChargesConsequence = EquipmentDefinitions.ItemOutOfCharges.Persist;
        usableDeviceDescription.magicAttackBonus = 11;
        usableDeviceDescription.saveDC = 19;
        usableDeviceDescription.DeviceFunctions.Add(deviceFunctionDescription);

        Definition.SlotTypes.AddRange(new List<string> {"MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot"});
        Definition.SlotsWhereActive.AddRange(new List<string>
        {
            "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot"
        });

        var focusItemDescription = new FocusItemDescription();
        focusItemDescription.shownAsFocus = true;
        focusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;

        Definition.activeOnGround = false;
        Definition.canBeStacked = true;
        Definition.defaultStackCount = 2;
        Definition.focusItemDefinition = focusItemDescription;
        Definition.forceEquip = false;
        Definition.forceEquipSlot = "";
        Definition.inDungeonEditor = true;
        Definition.itemRarity = RuleDefinitions.ItemRarity.VeryRare;
        Definition.magical = true; //
        Definition.merchantCategory = "Weapon";
        Definition.requiresAttunement = false;

        Definition.requiresIdentification = false;
        Definition.stackSize = 2;
        Definition.usableDeviceDescription = usableDeviceDescription;
        Definition.weaponDefinition = ThunderPunch;
        Definition.weight = 1;
        Definition.StaticProperties.Add(usingBonusActionItemPower);

        Definition.GuiPresentation.Title = "Equipment/&ThunderPunchTitle";
        Definition.GuiPresentation.Description = "Equipment/&ThunderPunchDescription";
        Definition.GuiPresentation.spriteReference = DatabaseHelper.ItemDefinitions.GauntletsOfOgrePower
            .GuiPresentation.SpriteReference;
    }

    private static ItemDefinition CreateAndAddToDB(string name, string guid)
    {
        return new SentinelSuitWeaponBuilder(name, guid).AddToDB();
    }
}

internal sealed class ThunderShieldBuilder : FeatureDefinitionPowerBuilder
{
    private const string ThunderShieldName = "ThunderShield";
    private const string ThunderShieldGuid = "f5ca9b23-0326-4b26-86e7-33ebcc061faf";

    public static readonly FeatureDefinitionPower ThunderShield =
        CreateAndAddToDB(ThunderShieldName, ThunderShieldGuid);

    private ThunderShieldBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&ThunderShieldTitle";
        Definition.GuiPresentation.Description = "Feat/&ThunderShieldDescription";
        Definition.GuiPresentation.spriteReference = Shield.GuiPresentation.SpriteReference;

        Definition.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        Definition.costPerUse = 1;
        Definition.fixedUsesPerRecharge = 5;
        Definition.activationTime = RuleDefinitions.ActivationTime.BonusAction;
        Definition.shortTitleOverride = "Feat/&ThunderShieldTitle";

        var healingEffect = new EffectForm {FormType = EffectForm.EffectFormType.TemporaryHitPoints};

        var tempHPForm = new TemporaryHitPointsForm
        {
            DiceNumber = 1, DieType = RuleDefinitions.DieType.D1, BonusHitPoints = 0
        };

        healingEffect.temporaryHitPointsForm = tempHPForm;
        healingEffect.applyLevel = EffectForm.LevelApplianceType.MultiplyBonus;
        healingEffect.levelType = RuleDefinitions.LevelSourceType.CharacterLevel;
        healingEffect.levelMultiplier = 1;

        //Add to our new effect
        var newEffectDescription = new EffectDescription();
        newEffectDescription.Copy(Definition.EffectDescription);
        newEffectDescription.EffectForms.Clear();
        newEffectDescription.EffectForms.Add(healingEffect);
        newEffectDescription.HasSavingThrow = false;
        newEffectDescription.DurationType = RuleDefinitions.DurationType.Day;
        newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
        newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
        newEffectDescription.SetTargetProximityDistance(12);
        newEffectDescription.SetCanBePlacedOnCharacter(true);
        newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);
        newEffectDescription.SetTargetFilteringTag(RuleDefinitions.TargetFilteringTag.No);

        newEffectDescription.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);

        Definition.effectDescription = newEffectDescription;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new ThunderShieldBuilder(name, guid).AddToDB();
    }
}

internal sealed class ThunderStruckConditionBuilder : ConditionDefinitionBuilder
{
    private const string ThunderStruckName = "ThunderStruck";
    private const string ThunderStruckGuid = "63e2091c-4186-43d5-a099-7c8ca97224d6";

    public static readonly ConditionDefinition ThunderStruck =
        CreateAndAddToDB(ThunderStruckName, ThunderStruckGuid);

    private ThunderStruckConditionBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionPoisoned, name, guid)
    {
        Definition.Features.Clear();
        Definition.GuiPresentation.Title = "Rules/&ThunderStruckTitle";
        Definition.GuiPresentation.Description = "Rules/&ThunderStruckDescription";

        Definition.allowMultipleInstances = true;
        Definition.conditionType = RuleDefinitions.ConditionType.Neutral;

        Definition.Features.Add(ThunderStruckDisadvantageCombatAffintityBuilder.Disadvantage);
        Definition.specialDuration = true;
        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.durationParameter = 1;
        Definition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;

        // Fx
        var assetReference = new AssetReference();
        assetReference.SetField("m_AssetGUID", "3e25fca5d3585174f9b7e20aca6ef3d9");
        Definition.conditionStartParticleReference = assetReference;
        Definition.conditionParticleReference = assetReference;
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ThunderStruckConditionBuilder(name, guid).AddToDB();
    }
}

internal sealed class ThunderStruckDisadvantageCombatAffintityBuilder : FeatureDefinitionCombatAffinityBuilder
{
    private const string ThunderStruckDisadvantageName = "ThunderStruckDisadvantage";
    private const string ThunderStruckDisadvantageGuid = "276daa3c-3ef2-4ea8-938f-f006d2721467";

    public static readonly FeatureDefinitionCombatAffinity Disadvantage =
        CreateAndAddToDB(ThunderStruckDisadvantageName, ThunderStruckDisadvantageGuid);

    private ThunderStruckDisadvantageCombatAffintityBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityPoisoned, name, guid)
    {
        Definition.myAttackAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
    }

    private static FeatureDefinitionCombatAffinity CreateAndAddToDB(string name, string guid)
    {
        return new ThunderStruckDisadvantageCombatAffintityBuilder(name, guid).AddToDB();
    }
}

internal sealed class ThunderStruckBalancingAdvantageConditionBuilder : ConditionDefinitionBuilder
{
    private const string ThunderStruckBalancingAdvantageName = "ThunderStruckBalancingAdvantage";
    private const string ThunderStruckBalancingAdvantageGuid = "0b2b4bee-21b0-46c7-9504-1374bd226cb0";

    public static readonly ConditionDefinition ThunderStruckBalancingAdvantage =
        CreateAndAddToDB(ThunderStruckBalancingAdvantageName, ThunderStruckBalancingAdvantageGuid);

    private ThunderStruckBalancingAdvantageConditionBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
    {
        Definition.Features.Clear();
        Definition.GuiPresentation.Title = "Rules/&ThunderStruckBalancingAdvantageTitle";
        Definition.GuiPresentation.Description = "Rules/&ThunderStruckBalancingAdvantageDescription";

        Definition.allowMultipleInstances = true;
        Definition.conditionType = RuleDefinitions.ConditionType.Neutral;
        Definition.specialDuration = true;
        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.durationParameter = 1;
        Definition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;

        Definition.Features.Add(BalancingAdvantageCombatAffintityBuilder.BalancingAdvantage);
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ThunderStruckBalancingAdvantageConditionBuilder(name, guid).AddToDB();
    }
}

internal sealed class BalancingAdvantageCombatAffintityBuilder : FeatureDefinitionCombatAffinityBuilder
{
    private const string BalancingAdvantageName = "BalancingAdvantage";
    private const string BalancingAdvantageGuid = "9dc28618-3adb-497d-930d-2a01bcac42d5";

    public static readonly FeatureDefinitionCombatAffinity BalancingAdvantage =
        CreateAndAddToDB(BalancingAdvantageName, BalancingAdvantageGuid);

    private BalancingAdvantageCombatAffintityBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityCursedByBestowCurseOnAttackRoll, name, guid)
    {
        Definition.myAttackAdvantage = RuleDefinitions.AdvantageType.Advantage;
        Definition.situationalContext = RuleDefinitions.SituationalContext.TargetIsEffectSource;
    }

    private static FeatureDefinitionCombatAffinity CreateAndAddToDB(string name, string guid)
    {
        return new BalancingAdvantageCombatAffintityBuilder(name, guid).AddToDB();
    }
}

internal sealed class UsingitemPowerBuilder : FeatureDefinitionActionAffinityBuilder
{
    private const string UsingitemPowerName = "UsingitemPower";
    private const string UsingitemPowerGuid = "39f8cb05-5475-456a-a9b2-022b6e07850b";

    public static readonly FeatureDefinitionActionAffinity UsingitemPower =
        CreateAndAddToDB(UsingitemPowerName, UsingitemPowerGuid);

    private UsingitemPowerBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityThiefFastHands, name, guid)
    {
        Definition.AuthorizedActions.Clear();
        Definition.AuthorizedActions.Add(ActionDefinitions.Id.UseItemBonus);

        Definition.GuiPresentation.Title = "Feat/&UsingitemPowerTitle";
        Definition.GuiPresentation.Description = "Feat/&UsingitemPowerDescription";
    }

    private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
    {
        return new UsingitemPowerBuilder(name, guid).AddToDB();
    }
}

//**************************************************************************************************************************************
//************************************************      Scout Suit Weapon        *******************************************************
//**************************************************************************************************************************************

internal sealed class ScoutSuitWeaponBuilder : ItemDefinitionBuilder
{
    private const string ScoutSuitWeaponName = "ScoutSuitWeapon";
    private const string ScoutSuitWeaponGuid = "21f90fad-b039-4efe-bee8-5afd44453664";

    public static readonly ItemDefinition ScoutSuitWeapon =
        CreateAndAddToDB(ScoutSuitWeaponName, ScoutSuitWeaponGuid);

    private ScoutSuitWeaponBuilder(string name, string guid) : base(DatabaseHelper.ItemDefinitions.Dart, name, guid)
    {
        // can only take 3    (at game launch in may, havent checked since)
        Definition.IsFocusItem = true;
        Definition.IsWeapon = true;
        //     Definition.IsFood = true;
        //   Definition.IsUsableDevice = true;

        Definition.inDungeonEditor = true;

        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D6,
                BonusDamage = 0,
                DamageType = "DamageLightning"
            },
            AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
        };

        var newEffectDescription = new EffectDescription();
        newEffectDescription.Copy(Definition.WeaponDescription.EffectDescription);
        newEffectDescription.EffectForms.Clear();
        newEffectDescription.EffectForms.Add(damageEffect);

        newEffectDescription.HasSavingThrow = false;
        newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
        newEffectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        newEffectDescription.SetTargetProximityDistance(1);
        newEffectDescription.SetCanBePlacedOnCharacter(true);
        newEffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);
        newEffectDescription.SetRangeParameter(12);

        newEffectDescription.SetEffectParticleParameters(LightningBolt.EffectDescription.EffectParticleParameters);

        var LightningSpear = new WeaponDescription();
        LightningSpear.weaponType = "DartType";
        LightningSpear.reachRange = 1;
        LightningSpear.maxRange = 60;
        LightningSpear.closeRange = 18;
        LightningSpear.ammunitionType = "";
        LightningSpear.effectDescription = newEffectDescription;
        LightningSpear.WeaponTags.Add("ScoutSentinelWeapon");

        var LightningSpearAdditionalDamage =
            (ItemPropertyDescription)FormatterServices.GetUninitializedObject(typeof(ItemPropertyDescription));
        LightningSpearAdditionalDamage.featureDefinition = LightningSpearAdditionalDamageBuilder
            .LightningSpearAdditionalDamage;
        LightningSpearAdditionalDamage.type = ItemPropertyDescription.PropertyType.Feature;
        LightningSpearAdditionalDamage.knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden;

        var LightningCloakStealth =
            (ItemPropertyDescription)FormatterServices.GetUninitializedObject(typeof(ItemPropertyDescription));
        LightningCloakStealth.featureDefinition = LightningCloakAbilityCheckAffinityBuilder
            .LightningCloakAbilityCheckAffinity;
        LightningCloakStealth.type = ItemPropertyDescription.PropertyType.Feature;
        LightningCloakStealth.knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden;

        var LightningCloakMovement =
            (ItemPropertyDescription)FormatterServices.GetUninitializedObject(typeof(ItemPropertyDescription));
        LightningCloakMovement.featureDefinition = LightningCloakMovementAffinitiesBuilder
            .LightningCloakMovementAffinities;
        LightningCloakMovement.type = ItemPropertyDescription.PropertyType.Feature;
        LightningCloakMovement.knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden;

        Definition.SlotTypes.AddRange(new List<string> {"MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot"});
        Definition.SlotsWhereActive.AddRange(new List<string>
        {
            "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot"
        });

        var focusItemDescription = new FocusItemDescription();
        focusItemDescription.shownAsFocus = true;
        focusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;

        Definition.activeOnGround = false;
        Definition.canBeStacked = true;
        Definition.defaultStackCount = -1;
        Definition.focusItemDefinition = focusItemDescription;
        Definition.forceEquip = false;
        Definition.forceEquipSlot = "MainHandSlot";
        Definition.inDungeonEditor = true;
        Definition.itemRarity = RuleDefinitions.ItemRarity.VeryRare;
        Definition.magical = true;
        Definition.merchantCategory = "Weapon";
        Definition.requiresAttunement = false;
        Definition.requiresIdentification = false;
        Definition.stackSize = 2;
        Definition.weaponDefinition = LightningSpear;
        Definition.weight = 1;
        Definition.StaticProperties.Add(LightningSpearAdditionalDamage);
        Definition.StaticProperties.Add(LightningCloakStealth);
        Definition.StaticProperties.Add(LightningCloakMovement);

        Definition.GuiPresentation.Title = "Equipment/&LightningSpearTitle";
        Definition.GuiPresentation.Description = "Equipment/&LightningSpearDescription";
        Definition.GuiPresentation.spriteReference = DatabaseHelper.ItemDefinitions.GlovesOfMissileSnaring
            .GuiPresentation.SpriteReference;

        Definition.itemPresentation = DatabaseHelper.ItemDefinitions.UnarmedStrikeBase.ItemPresentation;
    }

    private static ItemDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ScoutSuitWeaponBuilder(name, guid).AddToDB();
    }
}

internal sealed class LightningSpearAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder
{
    private const string LightningSpearAdditionalDamageName = "LightningSpearAdditionalDamage";
    private const string LightningSpearAdditionalDamageGuid = "52d19882-ca63-422d-aece-d80f806859a8";

    public static readonly FeatureDefinitionAdditionalDamage LightningSpearAdditionalDamage =
        CreateAndAddToDB(LightningSpearAdditionalDamageName, LightningSpearAdditionalDamageGuid);

    private LightningSpearAdditionalDamageBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, name, guid)
    {
        Definition.GuiPresentation.Title = "Feedback/&LightningSpearAdditionalDamageTitle";
        Definition.GuiPresentation.Description = "Feedback/&LightningSpearAdditionalDamageDescription";
        Definition.notificationTag = "LightningSpear";
        Definition.triggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive;
        Definition.limitedUsage = RuleDefinitions.FeatureLimitedUsage.OncePerTurn;
        Definition.requiredProperty = RuleDefinitions.AdditionalDamageRequiredProperty.RangeWeapon;
        Definition.damageDieType = RuleDefinitions.DieType.D6;
        Definition.damageDiceNumber = 1;
        Definition.specificDamageType = RuleDefinitions.DamageTypeLightning;
    }

    private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
    {
        return new LightningSpearAdditionalDamageBuilder(name, guid).AddToDB();
    }
}

internal sealed class LightningCloakMovementAffinitiesBuilder : FeatureDefinitionMovementAffinityBuilder
{
    private const string LightningCloakMovementAffinitiesName = "LightningCloakMovementAffinities";
    private const string LightningCloakMovementAffinitiesGuid = "9a098f7c-0286-4a04-ba42-23b7dfc45e7b";

    public static readonly FeatureDefinitionMovementAffinity LightningCloakMovementAffinities =
        CreateAndAddToDB(LightningCloakMovementAffinitiesName, LightningCloakMovementAffinitiesGuid);

    private LightningCloakMovementAffinitiesBuilder(string name, string guid) : base(name, guid)
    {
        Definition.baseSpeedAdditiveModifier = 1;
    }

    private static FeatureDefinitionMovementAffinity CreateAndAddToDB(string name, string guid)
    {
        return new LightningCloakMovementAffinitiesBuilder(name, guid)
            .SetGuiPresentationNoContent()
            .AddToDB();
    }
}

internal sealed class LightningCloakAbilityCheckAffinityBuilder : FeatureDefinitionAbilityCheckAffinityBuilder
{
    private const string LightningCloakAbilityCheckAffinityName = "LightningCloakAbilityCheckAffinity";
    private const string LightningCloakAbilityCheckAffinityGuid = "160e74ef-1596-4b68-8a5b-48b65b155b26";

    public static readonly FeatureDefinitionAbilityCheckAffinity LightningCloakAbilityCheckAffinity =
        CreateAndAddToDB(LightningCloakAbilityCheckAffinityName, LightningCloakAbilityCheckAffinityGuid);

    private LightningCloakAbilityCheckAffinityBuilder(string name, string guid) : base(name, guid)
    {
        var DampeningField = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
        {
            abilityScoreName = AttributeDefinitions.Dexterity,
            proficiencyName = SkillDefinitions.Stealth,
            affinity = RuleDefinitions.CharacterAbilityCheckAffinity.Advantage
        };
        Definition.AffinityGroups.Add(DampeningField);
    }

    private static FeatureDefinitionAbilityCheckAffinity CreateAndAddToDB(string name, string guid)
    {
        return new LightningCloakAbilityCheckAffinityBuilder(name, guid)
            .SetGuiPresentationNoContent()
            .AddToDB();
    }
}

//*************************************************************************************************************************
//***********************************		Improved Sentinel Suit Weapon   		****************************************
//*************************************************************************************************************************
internal sealed class ImprovedSentinelSuitWeaponBuilder : ItemDefinitionBuilder
{
    private const string ImprovedSentinelSuitWeaponName = "ImprovedSentinelSuitWeapon";
    private const string ImprovedSentinelSuitWeaponGuid = "f65108aa-1f69-4b08-b170-5fffd8444606";

    public static readonly ItemDefinition ImprovedSentinelSuitWeapon =
        CreateAndAddToDB(ImprovedSentinelSuitWeaponName, ImprovedSentinelSuitWeaponGuid);

    private ImprovedSentinelSuitWeaponBuilder(string name, string guid) : base(
        SentinelSuitWeaponBuilder.SentinelSuitWeapon, name, guid)
    {
        Definition.GuiPresentation.Title = "Equipment/&ImprovedThunderPunchTitle";
        Definition.GuiPresentation.Description = "Equipment/&ImprovedThunderPunchDescription";
        Definition.GuiPresentation.spriteReference = DatabaseHelper.ItemDefinitions.GauntletsOfOgrePower
            .GuiPresentation.SpriteReference;

        var grapplefunction = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions
            .PotionOfComprehendLanguages.UsableDeviceDescription.DeviceFunctions[0]);
        grapplefunction.canOverchargeSpell = false;
        grapplefunction.durationType = RuleDefinitions.DurationType.UntilLongRest;
        grapplefunction.featureDefinitionPower = GauntletsGrappleBuilder.GauntletsGrapple;
        grapplefunction.parentUsage = EquipmentDefinitions.ItemUsage.ByFunction;
        grapplefunction.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        grapplefunction.type = DeviceFunctionDescription.FunctionType.Power;
        grapplefunction.useAffinity = DeviceFunctionDescription.FunctionUseAffinity.IterationPerRecharge;
        grapplefunction.useAmount = 6;

        Definition.UsableDeviceDescription.DeviceFunctions.Add(grapplefunction);
        Definition.UsableDeviceDescription.chargesCapitalNumber = 10;
    }

    private static ItemDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ImprovedSentinelSuitWeaponBuilder(name, guid).AddToDB();
    }
}

internal sealed class GauntletsGrappleBuilder : FeatureDefinitionPowerBuilder
{
    private const string GauntletsGrappleName = "GauntletsGrapple";
    private const string GauntletsGrappleGuid = "71b309c2-1f8b-4df4-955e-3f8504bc381e";

    public static readonly FeatureDefinitionPower GauntletsGrapple =
        CreateAndAddToDB(GauntletsGrappleName, GauntletsGrappleGuid);

    private GauntletsGrappleBuilder(string name, string guid) : base(
        DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution, name, guid)
    {
        Definition.GuiPresentation.Title = "Feat/&GauntletsGrappleTitle";
        Definition.GuiPresentation.Description = "Feat/&GauntletsGrappleDescription";
        Definition.GuiPresentation.spriteReference = DatabaseHelper.FeatureDefinitionPowers
            .PowerShadowTamerRopeGrapple.GuiPresentation.SpriteReference;

        Definition.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        Definition.costPerUse = 1;
        Definition.fixedUsesPerRecharge = 6;
        Definition.activationTime = RuleDefinitions.ActivationTime.BonusAction;
        Definition.shortTitleOverride = "Feat/&GauntletsGrappleTitle";
        Definition.reactionContext = RuleDefinitions.ReactionTriggerContext.HitByMelee;

        var motionEffect = new EffectForm {FormType = EffectForm.EffectFormType.Motion};

        var motion = new MotionForm();
        motion.distance = 6;
        motion.type = MotionForm.MotionType.DragToOrigin;

        motionEffect.motionForm = motion;
        motionEffect.applyLevel = EffectForm.LevelApplianceType.MultiplyBonus;
        motionEffect.levelType = RuleDefinitions.LevelSourceType.CharacterLevel;
        motionEffect.levelMultiplier = 1;
        motionEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D8,
                BonusDamage = 0,
                DamageType = "DamageThunder"
            },
            AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
        };

        var newEffectDescription = new EffectDescription();
        newEffectDescription.Copy(Definition.EffectDescription);
        newEffectDescription.EffectForms.Clear();
        newEffectDescription.EffectForms.Add(motionEffect);
        newEffectDescription.EffectForms.Add(damageEffect);
        newEffectDescription.HasSavingThrow = true;
        newEffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Strength.Name;
        newEffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Intelligence
            .Name);
        newEffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation
            .SpellCastingFeature);
        newEffectDescription.FixedSavingThrowDifficultyClass = 19;
        newEffectDescription.SetCreatedByCharacter(true);

        newEffectDescription.DurationType = RuleDefinitions.DurationType.UntilLongRest;
        newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        newEffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
        newEffectDescription.SetTargetProximityDistance(12);
        newEffectDescription.SetCanBePlacedOnCharacter(true);
        newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        newEffectDescription.SetRangeParameter(6);

        newEffectDescription.SetEffectParticleParameters(DatabaseHelper.FeatureDefinitionPowers
            .PowerShadowTamerRopeGrapple.EffectDescription.EffectParticleParameters);

        Definition.effectDescription = newEffectDescription;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new GauntletsGrappleBuilder(name, guid).AddToDB();
    }
}

//**************************************************************************************************************************************
//******************************************       Improved Scout Suit Weapon        ***************************************************
//**************************************************************************************************************************************
internal sealed class ImprovedScoutSuitWeaponBuilder : ItemDefinitionBuilder
{
    private const string ImprovedScoutSuitWeaponName = "ImprovedScoutSuitWeapon";
    private const string ImprovedScoutSuitWeaponGuid = "c2ebf9a1-5c45-4f70-ba6c-c791cd607ea7";

    public static readonly ItemDefinition ImprovedScoutSuitWeapon =
        CreateAndAddToDB(ImprovedScoutSuitWeaponName, ImprovedScoutSuitWeaponGuid);

    private ImprovedScoutSuitWeaponBuilder(string name, string guid) : base(ScoutSuitWeaponBuilder.ScoutSuitWeapon,
        name, guid)
    {
        Definition.GuiPresentation.Title = "Equipment/&ImprovedLightningSpearTitle";
        Definition.GuiPresentation.Description = "Equipment/&ImprovedLightningSpearDescription";
        Definition.GuiPresentation.spriteReference = DatabaseHelper.ItemDefinitions.GlovesOfMissileSnaring
            .GuiPresentation.SpriteReference;

        Definition.inDungeonEditor = true;

        //next attack advantage // condition true strike or guiding bolt
        var NextAttackAdvantage = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        NextAttackAdvantage.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        NextAttackAdvantage.ConditionForm.ConditionDefinition =
            AdvantageAttackOnEnemyBuilder.AdvantageAttackOnEnemy;

        // combat affinity cursed on attack roll
        var EnemyAttackDisadvantageEffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        EnemyAttackDisadvantageEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        EnemyAttackDisadvantageEffect.ConditionForm.ConditionDefinition =
            DisadvantageOnAttackByEnemyBuilder.DisadvantageOnAttackByEnemy;

        //extra damage on attack
        var ExtraAttackEffect = new EffectForm
        {
            ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
        };
        ExtraAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
        ExtraAttackEffect.ConditionForm.ConditionDefinition =
            ExtraDamageOnAttackConditionBuilder.ExtraDamageOnAttackCondition;

        // target illuminated
        var lightSourceForm = new LightSourceForm();
        lightSourceForm.Copy(Shine.EffectDescription.EffectForms[0].LightSourceForm);

        var MagicalLightSourceEffect = new EffectForm();
        MagicalLightSourceEffect.levelMultiplier = 1;
        MagicalLightSourceEffect.levelType = RuleDefinitions.LevelSourceType.CharacterLevel;
        MagicalLightSourceEffect.HasSavingThrow = false;
        MagicalLightSourceEffect.createdByCharacter = true;
        MagicalLightSourceEffect.FormType = EffectForm.EffectFormType.LightSource;
        MagicalLightSourceEffect.lightSourceForm = lightSourceForm;

        Definition.WeaponDescription.EffectDescription.EffectForms.Add(EnemyAttackDisadvantageEffect);
        Definition.WeaponDescription.EffectDescription.EffectForms.Add(ExtraAttackEffect);
        Definition.WeaponDescription.EffectDescription.EffectForms.Add(NextAttackAdvantage);
        // game hangs when light effect is added, dont know why
        //Definition.WeaponDescription.EffectDescription.EffectForms.Add(MagicalLightSourceEffect);
    }

    private static ItemDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ImprovedScoutSuitWeaponBuilder(name, guid).AddToDB();
    }
}

internal sealed class DisadvantageOnAttackByEnemyBuilder : ConditionDefinitionBuilder
{
    private const string DisadvantageOnAttackByEnemyName = "DisadvantageOnAttackByEnemy";
    private const string DisadvantageOnAttackByEnemyGuid = "94bbcf4e-c376-4804-a157-2a5a5dd003e9";

    public static readonly ConditionDefinition DisadvantageOnAttackByEnemy =
        CreateAndAddToDB(DisadvantageOnAttackByEnemyName, DisadvantageOnAttackByEnemyGuid);

    private DisadvantageOnAttackByEnemyBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionCursedByBestowCurseAttackRoll, name, guid)
    {
        // Jolted - enemey has disadvantage on scout sentinel after weapon hits
        Definition.GuiPresentation.Title = "Rules/&DisadvantageOnAttackByEnemyTitle";
        Definition.GuiPresentation.Description = "Rules/&DisadvantageOnAttackByEnemyDescription";
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new DisadvantageOnAttackByEnemyBuilder(name, guid).AddToDB();
    }
}

internal sealed class AdvantageAttackOnEnemyBuilder : ConditionDefinitionBuilder
{
    private const string AdvantageAttackOnEnemyName = "AdvantageAttackOnEnemy";
    private const string AdvantageAttackOnEnemyGuid = "e10ff259-0294-4814-86de-327eaa1486a6";

    public static readonly ConditionDefinition AdvantageAttackOnEnemy =
        CreateAndAddToDB(AdvantageAttackOnEnemyName, AdvantageAttackOnEnemyGuid);

    private AdvantageAttackOnEnemyBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionTargetedByGuidingBolt, name, guid)
    {
        //Lit Up
        Definition.GuiPresentation.Title = "Rules/&AdvantageAttackOnEnemyTitle";
        Definition.GuiPresentation.Description = "Rules/&AdvantageAttackOnEnemyDescription";
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new AdvantageAttackOnEnemyBuilder(name, guid).AddToDB();
    }
}

internal sealed class ExtraDamageOnAttackConditionBuilder : ConditionDefinitionBuilder
{
    private const string ExtraDamageOnAttackConditionName = "ExtraDamageOnAttackCondition";
    private const string ExtraDamageOnAttackConditionGuid = "2a9bc931-adb9-4751-8770-3a1367920a57";

    public static readonly ConditionDefinition ExtraDamageOnAttackCondition =
        CreateAndAddToDB(ExtraDamageOnAttackConditionName, ExtraDamageOnAttackConditionGuid);

    private ExtraDamageOnAttackConditionBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionMarkedByFate, name, guid)
    {
        Definition.Features.Clear();
        Definition.GuiPresentation.Title = "Rules/&ExtraDamageOnAttackConditionTitle";
        Definition.GuiPresentation.Description = "Rules/&ExtraDamageOnAttackConditionDescription";
        //Static shocked
        Definition.allowMultipleInstances = true;
        Definition.conditionType = RuleDefinitions.ConditionType.Detrimental;
        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.durationParameter = 1;
        Definition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
        Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);

        Definition.additionalDamageWhenHit = true;
        Definition.additionalDamageDieNumber = 1;
        Definition.additionalDamageDieType = RuleDefinitions.DieType.D6;
        Definition.additionalDamageType = RuleDefinitions.DamageTypeLightning;
        Definition.additionalDamageQuantity = ConditionDefinition.DamageQuantity.Dice;
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ExtraDamageOnAttackConditionBuilder(name, guid).AddToDB();
    }
}
