using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using TMPro;
using UnityEngine;
using static RuleDefinitions.EffectDifficultyClassComputation;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.CustomUI;

internal static class Tooltips
{
    private static GameObject _tooltipInfoCharacterDescription;
    private static GameObject _distanceTextObject;
    private static TextMeshProUGUI _tmpUGui;

    internal static void AddContextToRecoveredFeature(RecoveredFeatureItem item, RulesetCharacterHero character)
    {
        item.GuiTooltip.Context = character;
    }

    internal static void UpdatePowerUses(ITooltip tooltip, TooltipFeaturePowerParameters parameters)
    {
        if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
        {
            return;
        }

        if (tooltip.Context is not RulesetCharacter character)
        {
            return;
        }

        var power = guiPowerDefinition.PowerDefinition;
        var usesLabel = parameters.usesLabel;

        usesLabel.Text = FormatUses(power, character, usesLabel.Text);
    }

    private static string FormatUses(FeatureDefinitionPower power, RulesetCharacter character, string def)
    {
        if (power.UsesDetermination != RuleDefinitions.UsesDetermination.Fixed)
        {
            return def;
        }

        if (power.RechargeRate == RuleDefinitions.RechargeRate.AtWill)
        {
            return def;
        }

        if (power.CostPerUse == 0)
        {
            return def;
        }

        var usablePower = PowerProvider.Get(power, character);
        var maxUses = character.GetMaxUsesOfPower(usablePower);
        // must use GetRemainingPowerUses as power could be a Shared Pool
        var remainingUses = character.GetRemainingPowerUses(power);

        return $"{remainingUses}/{maxUses}";
    }

    internal static void UpdatePowerSaveDC(ITooltip tooltip, TooltipFeaturePowerParameters parameters)
    {
        if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
        {
            return;
        }

        if (!guiPowerDefinition.HasSavingThrow)
        {
            return;
        }

        var effectDescription = guiPowerDefinition.EffectDescription;
        if (effectDescription.DifficultyClassComputation
            is not (AbilityScoreAndProficiency or SpellCastingFeature))
        {
            return;
        }

        if (tooltip.Context is not RulesetCharacter character)
        {
            return;
        }

        var attribute = DatabaseRepository.GetDatabase<SmartAttributeDefinition>()
            .GetElement(effectDescription.SavingThrowAbility);

        var power = guiPowerDefinition.PowerDefinition;
        var classDefinition = character.FindClassHoldingFeature(power);
        var saveDC = EffectHelpers.CalculateSaveDc(character, effectDescription, classDefinition);

        parameters.savingThrowLabel.Text = Gui.Format("{0} {1}", attribute.GuiPresentation.Title, saveDC.ToString());
    }

    internal static void UpdateCraftingTooltip(TooltipFeatureDescription description, ITooltip tooltip)
    {
        if (!Main.Settings.ShowCraftingRecipeInDetailedTooltips)
        {
            return;
        }

        if (tooltip.DataProvider is not IItemDefinitionProvider itemDefinitionProvider)
        {
            return;
        }

        var item = itemDefinitionProvider.ItemDefinition;

        if (!item.IsDocument || item.DocumentDescription.LoreType != RuleDefinitions.LoreType.CraftingRecipe)
        {
            return;
        }

        var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

        foreach (var contentFragmentDescription in item.DocumentDescription.ContentFragments
                     .Where(x => x.Type == ContentFragmentDescription.FragmentType.Body))
        {
            var guiRecipeDefinition =
                guiWrapperService.GetGuiRecipeDefinition(item.DocumentDescription.RecipeDefinition.Name);

            description.DescriptionLabel.Text = Gui.Format(contentFragmentDescription.Text,
                guiRecipeDefinition.Title, guiRecipeDefinition.IngredientsText);
        }
    }

    internal static void AddDistanceToTooltip(EntityDescription entityDescription)
    {
        _tooltipInfoCharacterDescription ??= GameObject.Find("TooltipFeatureCharacterDescription");

        if (_tooltipInfoCharacterDescription is null)
        {
            return;
        }

        var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (Main.Settings.EnableDistanceOnTooltip && battleService.Battle is not null)
        {
            entityDescription.header += "<br><br>";

            GameLocationCharacter characterToMeasureFrom = null;
            var distance = GetDistanceFromCharacter(ref characterToMeasureFrom, battleService);

            if (characterToMeasureFrom is null)
            {
                return;
            }

            // don't use ? on a type deriving from an unity object
            if (_tooltipInfoCharacterDescription)
            {
                _tmpUGui ??= _tooltipInfoCharacterDescription.transform.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (!_distanceTextObject)
            {
                GenerateDistanceText(distance, _tmpUGui, characterToMeasureFrom);
            }
            else
            {
                UpdateDistanceText(distance, characterToMeasureFrom);
            }

            // don't use ? on a type deriving from an unity object
#pragma warning disable IDE0031
            if (_distanceTextObject)
#pragma warning restore IDE0031
            {
                _distanceTextObject.SetActive(true);
            }
        }
        else if (!Main.Settings.EnableDistanceOnTooltip || battleService.Battle is null)
        {
            // don't use ? on a type deriving from an unity object
#pragma warning disable IDE0031
            if (_distanceTextObject)
#pragma warning restore IDE0031
            {
                _distanceTextObject.SetActive(false);
            }
        }
    }

    private static void GenerateDistanceText(int distance, TextMeshProUGUI tmpUGui,
        GameLocationCharacter characterToMeasureFrom)
    {
        var anchorObject = new GameObject();

        anchorObject.transform.SetParent(tmpUGui.transform);
        anchorObject.transform.localPosition = Vector3.zero;
        _distanceTextObject = Object.Instantiate(tmpUGui).gameObject;
        _distanceTextObject.name = "DistanceTextObject";
        _distanceTextObject.transform.SetParent(anchorObject.transform);
        _distanceTextObject.transform.position = Vector3.zero;
        _distanceTextObject.transform.localPosition = new Vector3(0, -10, 0);

        UpdateDistanceText(distance, characterToMeasureFrom);
    }

    private static int GetDistanceFromCharacter(
        ref GameLocationCharacter characterToMeasureFrom,
        IGameLocationBattleService battleService)
    {
        var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

        if (gameLocationSelectionService.HoveredCharacters.Count is 0)
        {
            return 0;
        }

        var hoveredCharacter = gameLocationSelectionService.HoveredCharacters[0];
        var initiativeSortedContenders = battleService.Battle.InitiativeSortedContenders;
        var activePlayerController = ServiceRepository.GetService<IPlayerControllerService>().ActivePlayerController;
        var activePlayerControlledCharacters = activePlayerController.ControlledCharacters;
        var actingCharacter = battleService.Battle?.activeContender;

        if (actingCharacter is null)
        {
            return 0;
        }

        characterToMeasureFrom = activePlayerControlledCharacters.Contains(actingCharacter)
            ? actingCharacter
            : GetNextControlledCharacterInInitiative(
                initiativeSortedContenders, activePlayerController, actingCharacter);

        return (int)Math.Round(DistanceCalculation.GetDistanceFromCharacters(characterToMeasureFrom, hoveredCharacter));
    }

    private static GameLocationCharacter GetNextControlledCharacterInInitiative(
        List<GameLocationCharacter> initiativeSortedContenders,
        PlayerController activePlayerController,
        GameLocationCharacter actingCharacter)
    {
        return initiativeSortedContenders.Find(character =>
            character.controllerId == activePlayerController.controllerId
            && character.lastInitiative < actingCharacter.lastInitiative) ?? initiativeSortedContenders.Find(
            character =>
                character.controllerId == activePlayerController.controllerId);
    }

    private static void UpdateDistanceText(int distance, GameLocationCharacter characterToMeasureFrom)
    {
        _distanceTextObject.GetComponent<TextMeshProUGUI>().text =
            Gui.Format("UI/&DistanceFormat", Gui.FormatDistance(distance))
            + $" {Gui.Localize("UI/&From")} "
            + GetReducedName(characterToMeasureFrom.Name);
    }

    private static string GetReducedName(string characterName)
    {
        return characterName.Length >= 12
            ? characterName.Substring(0, 9) + "..."
            : characterName;
    }

    internal static void ModifyWidth<TMod, TParent>(TParent parent)
        where TMod : BaseTooltipWidthModifier<TParent>
        where TParent : MonoBehaviour
    {
        if (!parent.TryGetComponent<TMod>(out var component))
        {
            component = parent.gameObject.AddComponent<TMod>();
            component.Init(parent);
        }

        component.Apply();
    }

    internal static TooltipFeatureWidthMod ModifyWidth(TooltipFeature parent)
    {
        if (!parent.TryGetComponent<TooltipFeatureWidthMod>(out var component))
        {
            component = parent.gameObject.AddComponent<TooltipFeatureWidthMod>();
            component.Init(parent);
        }

        component.Apply();
        return component;
    }
}

internal abstract class BaseTooltipWidthModifier<T> : MonoBehaviour where T : MonoBehaviour
{
    private const int DEF_WIDTH = 340;
    protected const int WIDTH = (int)(1.5 * DEF_WIDTH);
    protected const int PAD = 30; // default is 30?

    protected readonly Dictionary<string, float> Defaults = new();
    protected T Parent;
    protected abstract Dictionary<string, float> Modified { get; }
    protected const string Self = "Self";

    internal void Apply()
    {
        Modify(Main.Settings.WidenTooltips ? Modified : Defaults);
    }

    internal void Init(T parent)
    {
        if (parent is TooltipFeature feature && this is not TooltipFeatureWidthMod)
        {
            var component = Tooltips.ModifyWidth(feature);
            Defaults[Self] = component.Defaults[Self];
            Modified[Self] = component.Modified[Self];
        }

        Parent = parent;
        Init();
    }

    protected abstract void Init();
    protected abstract void Modify(Dictionary<string, float> values);

    protected static void SizeWithAnchors(Transform t, float width)
    {
        if (!t) { return; }

        SizeWithAnchors(t.GetComponent<RectTransform>(), width);
    }

    protected static void SizeWithAnchors(RectTransform rt, float width)
    {
        if (!rt) { return; }

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    protected static void FromEdge(RectTransform rt, float pad, float width)
    {
        if (!rt) { return; }

        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pad, width);
    }

    protected RectTransform Rect(Transform t, string path = null)
    {
        if (!t) { return null; }

        if (path == null) { return t.GetComponent<RectTransform>(); }

        t = t.Find(path);
        return !t ? null : t.GetComponent<RectTransform>();
    }

    protected RectTransform Rect(MonoBehaviour b, string path = null)
    {
        return Rect(b.transform, path);
    }
}

internal class TooltipPanelWidthModifier : BaseTooltipWidthModifier<TooltipPanel>
{
    protected const string Size = "Size";
    private const string BackgroundBlur = "BackgroundBlur";
    private const string Frame = "Frame";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Size, WIDTH }
    };

    protected override void Init()
    {
        Defaults[Size] = Parent.RectTransform.sizeDelta.x;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var width = values[Size];

        SizeWithAnchors(Parent.RectTransform, width);
        SizeWithAnchors(Rect(Parent, BackgroundBlur), width);
        SizeWithAnchors(Parent.featuresTable, width);

        var frame = Rect(Parent, Frame);
        if (frame) { SizeWithAnchors(frame, width); }
    }
}

internal class TooltipFeatureWidthMod : BaseTooltipWidthModifier<TooltipFeature>
{
    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Self, WIDTH }
    };

    protected override void Init()
    {
        Defaults[Self] = Parent.RectTransform.sizeDelta.x;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.RectTransform, values[Self]);
    }
}

internal class TooltipFeatureEffectsEnumWidthMod : BaseTooltipWidthModifier<TooltipFeatureEffectsEnumerator>
{
    private const string Effects = "Effects";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Effects, WIDTH - 2 * PAD }
    };

    protected override void Init()
    {
        Defaults[Effects] = Defaults[Self] - 2 * PAD;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var table = Parent.effectFormater.Table;
        var width = values[Effects];
        SizeWithAnchors(table, width);
        for (var i = 0; i < table.childCount; i++)
        {
            var line = table.GetChild(i).GetComponent<FeatureElementEffectLine>();
            if (!line) { continue; }

            SizeWithAnchors(line.RectTransform, width);
            SizeWithAnchors(line.effectLabel.RectTransform, width);
            SizeWithAnchors(line.effectDescription.RectTransform, width);
        }
    }
}

internal class TooltipSubSpellEnumWidthModifier : BaseTooltipWidthModifier<TooltipFeatureSubSpellsEnumerator>
{
    private const string Size = "Size";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Size, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        var table = Parent.table;
        Defaults[Size] = table.sizeDelta.x;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var table = Parent.table;
        var width = values[Size];
        SizeWithAnchors(table, width);
        for (var i = 0; i < table.childCount; i++)
        {
            var line = table.GetChild(i).GetComponent<FeatureElementSubSpell>();
            if (!line) { continue; }

            SizeWithAnchors(line.RectTransform, width);
        }
    }
}

internal class TooltipFeatureSpellParamsWidthModifier : BaseTooltipWidthModifier<TooltipFeatureSpellParameters>
{
    private const string VerticalLayout = "VerticalLayout";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { VerticalLayout, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[VerticalLayout] = Rect(Parent, VerticalLayout).sizeDelta.x;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var width = values[VerticalLayout];
        SizeWithAnchors(Rect(Parent, VerticalLayout), width);

        for (var i = 0; i < Parent.verticalLayout.childCount; i++)
        {
            SizeWithAnchors(Parent.verticalLayout.GetChild(i), width);
        }
    }
}

internal class
    TooltipFeatureBaseMagicParamsWidthModifier : BaseTooltipWidthModifier<TooltipFeatureBaseMagicParameters>
{
    private const string Table = "Table";
    private const string Pad = "Pad";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
        { Pad, PAD },
    };

    protected override void Init()
    {
        var rect = Rect(Parent, Table);
        Defaults[Table] = rect.sizeDelta.x;
        //TODO: find proper way of getting this
        Defaults[Pad] = PAD; //rect.rect.x + rect.sizeDelta.x / 2;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var width = values[Table];
        var pad = values[Pad];
        FromEdge(Rect(Parent, Table), pad, width);
    }
}

internal class TooltipFeatureTagsEnumWidthModifier : BaseTooltipWidthModifier<TooltipFeatureTagsEnumerator>
{
    private const string Table = "Table";
    private const string Label = "Background/PropertiesLabel";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
        { Label, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Parent.table.sizeDelta.x;
        Defaults[Label] = Rect(Parent, Label).sizeDelta.x;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.table, values[Table]);
        SizeWithAnchors(Rect(Parent, Label), values[Label]);
    }
}

internal class TooltipFeatureSpellAdvancementWidthMod : BaseTooltipWidthModifier<TooltipFeatureSpellAdvancement>
{
    private const string Title = "Title";
    private const string Label = "AdvancementLabel";
    private const string Pad = "Pad";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Title, WIDTH - 2 * PAD },
        { Label, WIDTH - 2 * PAD },
        { Pad, PAD },
    };

    protected override void Init()
    {
        Defaults[Title] = Rect(Parent, Title).sizeDelta.x;
        Defaults[Label] = Rect(Parent, Label).sizeDelta.x;
        //TODO: find proper way of getting this
        Defaults[Pad] = PAD;
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        var pad = values[Pad];
        FromEdge(Rect(Parent, Title), pad, values[Title]);
        FromEdge(Rect(Parent, Label), pad, values[Label]);
    }
}

internal class TooltipFeatureDeviceParametersWidthMod : BaseTooltipWidthModifier<TooltipFeatureDeviceParameters>
{
    private const string Table = "Table";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.usageGroup, values[Table]);
    }
}

internal class
    TooltipFeatureItemPropertiesEnumWidthMod : BaseTooltipWidthModifier<TooltipFeatureItemPropertiesEnumerator>
{
    private const string Table = "Table";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.propertiesTable, values[Table]);
        // FromEdge(Rect(Parent, Title), pad, values[Title]);
        // FromEdge(Rect(Parent, Label), pad, values[Label]);
    }
}

internal class
    TooltipFeatureDeviceFunctionsEnumWidthMod : BaseTooltipWidthModifier<TooltipFeatureDeviceFunctionsEnumerator>
{
    private const string Table = "Table";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.functionsTable, values[Table]);
    }
}

internal class TooltipFeatureItemStatsWidthMod : BaseTooltipWidthModifier<TooltipFeatureItemStats>
{
    private const string Table = "Table";
    private const string SecondTable = "VerticalLayout/SecondTable";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.topTable, values[Table]);
        SizeWithAnchors(Rect(Parent, SecondTable), values[Table]);
    }
}

internal class TooltipFeatureWeaponParametersWidthMod : BaseTooltipWidthModifier<TooltipFeatureWeaponParameters>
{
    private const string Table = "Table";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.masterTable, values[Table]);
    }
}

internal class TooltipFeatureArmorParamsWidthMod : BaseTooltipWidthModifier<TooltipFeatureArmorParameters>
{
    private const string Table = "Table";
    private const string HeaderLabel = "HeaderLabel";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.descriptionLabel.transform, values[Table]);
        SizeWithAnchors(Rect(Parent, HeaderLabel), values[Table]);
    }
}

internal class TooltipFeatureLightSourceParamsWidthMod : BaseTooltipWidthModifier<TooltipFeatureLightSourceParameters>
{
    private const string Table = "Table";
    private const string HeaderLabel = "HeaderLabel";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        SizeWithAnchors(Parent.descriptionLabel.transform, values[Table]);
        SizeWithAnchors(Rect(Parent, HeaderLabel), values[Table]);
    }
}

internal class TooltipFeaturePowerParamsWidthMod : BaseTooltipWidthModifier<TooltipFeaturePowerParameters>
{
    private const string Table = "Table";

    protected override Dictionary<string, float> Modified { get; } = new()
    {
        { Table, WIDTH - 2 * PAD },
    };

    protected override void Init()
    {
        Defaults[Table] = Defaults[Self];
    }

    protected override void Modify(Dictionary<string, float> values)
    {
        for (var i = 0; i < Parent.verticalLayout.childCount; i++)
        {
            SizeWithAnchors(Parent.verticalLayout.GetChild(i), values[Table]);
        }
    }
}
