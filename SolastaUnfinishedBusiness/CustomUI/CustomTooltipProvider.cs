using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomTooltipProvider : GuiBaseDefinitionWrapper, ISubTitleProvider, IPrerequisitesProvider
{
    public const string REQUIRE_CHARACTER_LEVEL = "Requirement/&FeatureSelectionRequireCharacterLevel";
    public const string REQUIRE_CLASS_LEVEL = "Requirement/&FeatureSelectionRequireClassLevel";

    private readonly GuiPresentation _guiPresentation;
    private string _prerequisites = string.Empty;
    private string _subtitle;

    public override string TooltipClass => "FeatDefinition";

    public CustomTooltipProvider(BaseDefinition baseDefinition, GuiPresentation guiPresentation) : base(
        baseDefinition)
    {
        _guiPresentation = guiPresentation;
        _subtitle = GetDefaultSubtitle();
    }

    public override string Description => BaseDefinition.FormatDescription();

    public string EnumeratePrerequisites(RulesetCharacterHero hero)
    {
        return _prerequisites;
    }

    public string Subtitle =>
        _subtitle ??=
            GetDefaultSubtitle(); //Just in case. This is actually set in constructor + check for null in the setter.

    private string GetDefaultSubtitle()
    {
        return BaseDefinition switch
        {
            FeatureDefinitionPower => "UI/&CustomFeatureSelectionTooltipTypePower",
            FeatureDefinitionBonusCantrips => "UI/&CustomFeatureSelectionTooltipTypeCantrip",
            FeatureDefinitionProficiency => "UI/&CustomFeatureSelectionTooltipTypeProficiency",
            CustomInvocationDefinition f => $"UI/&CustomFeatureSelectionTooltipType{f.PoolType.Name}",
            _ => "UI/&CustomFeatureSelectionTooltipTypeFeature"
        };
    }

    public override void SetupSprite(Image image, object context = null)
    {
        if (image.sprite != null)
        {
            ReleaseSprite(image);
            image.sprite = null;
        }

        if (_guiPresentation is {SpriteReference: { }} && _guiPresentation.SpriteReference.RuntimeKeyIsValid())
        {
            image.gameObject.SetActive(true);
            image.sprite = Gui.LoadAssetSync<Sprite>(_guiPresentation.SpriteReference);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    public void SetPrerequisites(params string[] missingRequirements)
    {
        SetPrerequisites(missingRequirements.ToList());
    }

    public CustomTooltipProvider SetPrerequisites(List<string> missingRequirements)
    {
        _prerequisites = missingRequirements == null || missingRequirements.Empty()
            ? string.Empty
            : String.Join("\n", missingRequirements.Select(e => Gui.Localize(e)));

        return this;
    }

    public CustomTooltipProvider SetSubtitle(string subtitle)
    {
        _subtitle = string.IsNullOrEmpty(subtitle)
            ? GetDefaultSubtitle()
            : subtitle;

        return this;
    }
}

class CustomItemTooltipProvider : CustomTooltipProvider,
    IArmorParametersProvider,
    IWeaponParametersProvider,
    IAmmunitionParametersProvider,
    IStarterPackParametersProvider,
    ILightSourceParametersProvider,
    IContainerParametersProvider,
    ISpellbookParametersProvider,
    IStackableParametersProvider,
    IDurationProvider,
    IItemDefinitionProvider,
    ITagsProvider,
    IEffectFormsProvider,
    IDeviceParametersProvider,
    IDeviceFunctionsEnumeratorProvider,
    IItemPropertiesEnumeratorProvider
{
    public const string ItemWithPrereqsTooltip = "ItemWithPrereqsDefinition";

    [NotNull] private GuiItemDefinition _guiItem;

    public CustomItemTooltipProvider(BaseDefinition baseDefinition, GuiPresentation guiPresentation,
        ItemDefinition item)
        : base(baseDefinition, guiPresentation)
    {
        _guiItem = new GuiItemDefinition(item);
    }

    public override string TooltipClass => ItemWithPrereqsTooltip;


    //IArmorParametersProvider
    public string ArmorDescription => _guiItem.ArmorDescription;

    //IWeaponParametersProvider
    public bool IsWeapon => _guiItem.IsWeapon;
    public string WeaponInfoHeader => _guiItem.WeaponInfoHeader;
    public float ReachDistance => _guiItem.ReachDistance;
    public float CloseRangeDistance => _guiItem.CloseRangeDistance;
    public float MaxRangeDistance => _guiItem.MaxRangeDistance;
    public int AttackRollModifier => _guiItem.AttackRollModifier;
    public bool VersatileOnFirstDamage => _guiItem.VersatileOnFirstDamage;
    public bool HasSavingThrow => _guiItem.HasSavingThrow;
    public string EffectsHeader => _guiItem.EffectsHeader;
    public int DamageRollModifier => _guiItem.DamageRollModifier;
    public int RangeParameter => _guiItem.RangeParameter;
    public bool ForceTight => _guiItem.ForceTight;
    public RuleDefinitions.EffectApplication EffectApplication => _guiItem.EffectApplication;
    public string SpecialFormsDescription => _guiItem.SpecialFormsDescription;
    public List<EffectForm> EffectForms => _guiItem.EffectForms;
    public string AmmunitionDescription => _guiItem.AmmunitionDescription;
    public string StarterPackDescription => _guiItem.StarterPackDescription;
    public string LightSourceDescription => _guiItem.LightSourceDescription;
    public bool IsContainer => _guiItem.IsContainer;
    public string ContainerWeightCapacityMultiplier => _guiItem.ContainerWeightCapacityMultiplier;
    public string SpellbookDescription => _guiItem.SpellbookDescription;
    public string StackableDescription => _guiItem.StackableDescription;
    public bool DynamicDuration => _guiItem.DynamicDuration;
    public string DurationDescription => _guiItem.DurationDescription;
    public ItemDefinition ItemDefinition => _guiItem.ItemDefinition;

    public Dictionary<string, TagsDefinitions.Criticity> EnumerateTags(object context)
    {
        return _guiItem.EnumerateTags(context);
    }

    public bool CanAccessDeviceParameters => _guiItem.CanAccessDeviceParameters;
    public EquipmentDefinitions.ItemUsage Usage => _guiItem.Usage;
    public string UsageText => _guiItem.UsageText;
    public string Charges => _guiItem.Charges;
    public string Recharge => _guiItem.Recharge;

    public bool IsAttunementValid(RulesetCharacter character)
    {
        return _guiItem.IsAttunementValid(character);
    }

    public bool HasProperties => _guiItem.HasProperties;
    public bool PropertyListIsKnown => _guiItem.PropertyListIsKnown;
    public bool IsUsableDevice => _guiItem.IsUsableDevice;

    public List<ItemPropertyDescription> PropertiesList => _guiItem.PropertiesList;

    public string AttunementInfo => _guiItem.AttunementInfo;

    public string FormatFunctionDescription(RulesetDeviceFunction function, RulesetCharacter character, bool inCombat)
    {
        return _guiItem.FormatFunctionDescription(function, character, inCombat);
    }

    public bool FunctionListIsKnown => _guiItem.FunctionListIsKnown;
    public bool HasUsableFunctions => _guiItem.HasUsableFunctions;
    public List<DeviceFunctionDescription> FunctionDescriptions => _guiItem.FunctionDescriptions;
    public List<RulesetDeviceFunction> UsableFunctions => _guiItem.UsableFunctions;
}
