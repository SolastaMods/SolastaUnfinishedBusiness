using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine;
using UnityEngine.UI;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class CustomTooltipProvider : GuiBaseDefinitionWrapper, ISubTitleProvider, IPrerequisitesProvider
{
    internal const string RequireCharacterLevel = "Requirement/&FeatureSelectionRequireCharacterLevel";
    internal const string RequireClassLevel = "Requirement/&FeatureSelectionRequireClassLevel";

    private readonly GuiPresentation _guiPresentation;
    private string _prerequisites = string.Empty;
    private string _subtitle;

    internal CustomTooltipProvider(BaseDefinition baseDefinition, GuiPresentation guiPresentation) : base(
        baseDefinition)
    {
        _guiPresentation = guiPresentation;
        _subtitle = GetDefaultSubtitle();
    }

    public override string TooltipClass => "FeatDefinition";

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
            InvocationDefinitionCustom f => $"UI/&CustomFeatureSelectionTooltipType{f.PoolType.Name}",
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

        if (_guiPresentation is { SpriteReference: not null } && _guiPresentation.SpriteReference.RuntimeKeyIsValid())
        {
            image.gameObject.SetActive(true);
            image.sprite = Gui.LoadAssetSync<Sprite>(_guiPresentation.SpriteReference);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    internal void SetPrerequisites(params string[] missingRequirements)
    {
        SetPrerequisites(missingRequirements.ToList());
    }

    internal void SetPrerequisites(List<string> missingRequirements)
    {
        _prerequisites = missingRequirements == null || missingRequirements.Count == 0
            ? string.Empty
            : String.Join("\n", missingRequirements.Select(e => Gui.Localize(e)));
    }

    internal void SetSubtitle(string subtitle)
    {
        _subtitle = string.IsNullOrEmpty(subtitle)
            ? GetDefaultSubtitle()
            : subtitle;
    }
}

internal class CustomItemTooltipProvider : CustomTooltipProvider,
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
    internal const string ItemWithPreReqsTooltip = "ItemWithPrereqsDefinition";

    [NotNull] private readonly GuiItemDefinition _guiItem;

    internal CustomItemTooltipProvider(BaseDefinition baseDefinition, GuiPresentation guiPresentation,
        ItemDefinition item)
        : base(baseDefinition, guiPresentation)
    {
        _guiItem = new GuiItemDefinition(item);
    }

    public override string TooltipClass => ItemWithPreReqsTooltip;
    public string AmmunitionDescription => _guiItem.AmmunitionDescription;

    //IArmorParametersProvider
    public string ArmorDescription => _guiItem.ArmorDescription;
    public bool IsContainer => _guiItem.IsContainer;
    public string ContainerWeightCapacityMultiplier => _guiItem.ContainerWeightCapacityMultiplier;

    public string FormatFunctionDescription(RulesetDeviceFunction function, RulesetCharacter character, bool inCombat)
    {
        return _guiItem.FormatFunctionDescription(function, character, inCombat);
    }

    public bool FunctionListIsKnown => _guiItem.FunctionListIsKnown;
    public bool HasUsableFunctions => _guiItem.HasUsableFunctions;
    public List<DeviceFunctionDescription> FunctionDescriptions => _guiItem.FunctionDescriptions;
    public List<RulesetDeviceFunction> UsableFunctions => _guiItem.UsableFunctions;

    public bool CanAccessDeviceParameters => _guiItem.CanAccessDeviceParameters;
    public EquipmentDefinitions.ItemUsage Usage => _guiItem.Usage;
    public string UsageText => _guiItem.UsageText;
    public string Charges => _guiItem.Charges;
    public string Recharge => _guiItem.Recharge;

    public string AttunementInfo => _guiItem.AttunementInfo;
    public bool DynamicDuration => _guiItem.DynamicDuration;
    public string DurationDescription => _guiItem.DurationDescription;
    public bool VersatileOnFirstDamage => _guiItem.VersatileOnFirstDamage;
    public bool HasSavingThrow => _guiItem.HasSavingThrow;
    public string EffectsHeader => _guiItem.EffectsHeader;
    public int RangeParameter => _guiItem.RangeParameter;
    public bool ForceTight => _guiItem.ForceTight;
    public EffectApplication EffectApplication => _guiItem.EffectApplication;
    public string SpecialFormsDescription => _guiItem.SpecialFormsDescription;
    public List<EffectForm> EffectForms => _guiItem.EffectForms;
    public ItemDefinition ItemDefinition => _guiItem.ItemDefinition;
    public string BaseDamageType => _guiItem.BaseDamageType;

    public bool IsAttunementValid(RulesetCharacter character)
    {
        return _guiItem.IsAttunementValid(character);
    }

    public bool HasProperties => _guiItem.HasProperties;
    public bool PropertyListIsKnown => _guiItem.PropertyListIsKnown;
    public bool IsUsableDevice => _guiItem.IsUsableDevice;

    public List<ItemPropertyDescription> PropertiesList => _guiItem.PropertiesList;
    public string LightSourceDescription => _guiItem.LightSourceDescription;
    public string SpellbookDescription => _guiItem.SpellbookDescription;
    public string StackableDescription => _guiItem.StackableDescription;
    public string StarterPackDescription => _guiItem.StarterPackDescription;

    public Dictionary<string, TagsDefinitions.Criticity> EnumerateTags(object context)
    {
        return _guiItem.EnumerateTags(context);
    }

    //IWeaponParametersProvider
    public bool IsWeapon => _guiItem.IsWeapon;
    public string WeaponInfoHeader => _guiItem.WeaponInfoHeader;
    public float ReachDistance => _guiItem.ReachDistance;
    public float CloseRangeDistance => _guiItem.CloseRangeDistance;
    public float MaxRangeDistance => _guiItem.MaxRangeDistance;
    public int AttackRollModifier => _guiItem.AttackRollModifier;
    public int DamageRollModifier => _guiItem.DamageRollModifier;
}
