using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomCharacterStatsPanel
{
    private readonly AbilityScoresListingPanel _abilities;
    private readonly AttackModesPanel _attacks;
    private readonly Button _button;

    private readonly Transform _root;
    private readonly CharacterStatsPanel _stats;
    private bool _bound;
    private RulesetCharacter _character;
    private GuiCharacter _guiCharacter;
    private bool _visible;

    private CustomCharacterStatsPanel()
    {
        _root = new GameObject().transform;
        var inspector = Gui.GuiService.GetScreen<CharacterInspectionScreen>();

        _root.localPosition = new Vector3(225, 125, 0);
        _root.gameObject.SetActive(false);

        // ReSharper disable once Unity.UnknownResource
        var prefab = Resources.Load<GameObject>("Gui/Prefabs/Component/SmallButtonRoundImage");
        _button = Object.Instantiate(prefab).GetComponent<Button>();
        _button.gameObject.SetActive(false);
        _button.transform.localPosition = new Vector3(50, 50, 0);
        _button.onClick.AddListener(() =>
        {
            if (!_bound) { return; }

            _visible = !_visible;
            UpdateVisibility();
        });

        #region Abilities

        _abilities = Object.Instantiate(inspector.abilityScoresListingPanelPrefab, _root)
            .GetComponent<AbilityScoresListingPanel>();
        _abilities.transform.localPosition = new Vector3(0, 100, 0);

        for (var index = 0; index < AttributeDefinitions.AbilityScoreNames.Length; ++index)
        {
            _abilities.abilityScoreBoxes.Add(Gui.GetPrefabFromPool(_abilities.boxPrefab, _abilities.table)
                .GetComponent<AbilityScoreBox>());
        }

        #endregion

        #region Stats

        _stats = Object.Instantiate(inspector.characterStatsPanelPrefab, _root)
            .GetComponent<CharacterStatsPanel>();
        _stats.transform.localPosition = new Vector3(0, 0, 0);

        #endregion

        #region Attacks

        _attacks = Gui.GetPrefabFromPool(inspector.attackModesPanelPrefab, _root)
            .GetComponent<AttackModesPanel>();
        _attacks.transform.localPosition = new Vector3(275, 200, 0);

        #endregion
    }

    public static CustomCharacterStatsPanel Instance => MaybeInstance ??= new CustomCharacterStatsPanel();
    public static CustomCharacterStatsPanel MaybeInstance { get; private set; }

    private void UpdateVisibility()
    {
        _root.gameObject.SetActive(_visible);
    }

    private static GameLocationBaseScreen GetActiveScreen()
    {
        var explore = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
        if (explore != null && explore.Visible)
        {
            return explore;
        }

        var battle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();
        if (battle != null && battle.Visible)
        {
            return battle;
        }

        return null;
    }

    public void Bind(RulesetCharacter target)
    {
        var screen = GetActiveScreen();
        if (screen == null)
        {
            return;
        }

        _character = target;
        _guiCharacter = new GuiCharacter(_character);
        var parent = screen.CharacterControlPanel.ActiveCharacterPanel.transform;

        _root.parent = parent;
        _button.transform.parent = parent;
        _button.gameObject.SetActive(true);

        #region Abilities

        for (var index = 0; index < AttributeDefinitions.AbilityScoreNames.Length; ++index)
        {
            var attribute = _character.GetAttribute(AttributeDefinitions.AbilityScoreNames[index]);
            _abilities.abilityScoreBoxes[index].Bind(attribute, _abilities.abilityScoreBoxes);
        }

        #endregion

        #region Stats

        _stats.guiCharacter = _guiCharacter;

        _stats.initiativeBox.Bind(_character.GetAttribute(AttributeDefinitions.Initiative), "+0;-#");
        _stats.initiativeBox.Activate(true);

        _character.TryGetAttribute(AttributeDefinitions.ProficiencyBonus, out var proficiencyBonus);
        _stats.proficiencyBox.Activate(proficiencyBonus != null);
        if (_stats.proficiencyBox.Activated)
        {
            _stats.proficiencyBox.Bind(proficiencyBonus, "+0;-#");
        }

        _stats.armorClassBox.Activate(proficiencyBonus != null);
        _stats.armorClassBox.gameObject.SetActive(false);
        _stats.hitDiceBox.Activate(proficiencyBonus != null);
        _stats.hitDiceBox.gameObject.SetActive(false);
        _stats.hitPointBox.Activate(proficiencyBonus != null);
        _stats.hitPointBox.gameObject.SetActive(false);

        #endregion

        _bound = true;

        Refresh();
        UpdateVisibility();
    }

    public void Unbind()
    {
        _bound = false;
        _root.gameObject.SetActive(false);
        _button.gameObject.SetActive(false);
        _character = null;
        _guiCharacter = null;

        #region Abilities

        _abilities.Unbind();

        #endregion

        #region Stats

        _stats.Unbind();
        _stats.initiativeBox.Unbind();
        _stats.moveBox.Unbind();
        _stats.proficiencyBox.Unbind();

        #endregion

        #region Attacks

        _attacks.Unbind();

        #endregion
    }

    public void Refresh()
    {
        if (!_bound || _character == null || _guiCharacter == null)
        {
            return;
        }

        _root.position = new Vector3(225, 125, 0);
        _button.transform.position = new Vector3(50, 50, 0);

        #region Stats

        _stats.initiativeBox.Refresh();
        _stats.moveBox.ValueLabel.Text = _guiCharacter.MovePoints;
        _stats.proficiencyBox.Refresh();

        #endregion

        #region Attacks

        //refresh

        _attacks.relevantAttackModes.Clear();
        _attacks.relevantAttackModes.AddRange(_character.AttackModes
            .Where(m => m.ActionType is ActionDefinitions.ActionType.Main or ActionDefinitions.ActionType.Bonus));

        while (_attacks.attackModesTable.childCount < _attacks.relevantAttackModes.Count)
        {
            Gui.GetPrefabFromPool(_attacks.attackModePrefab, _attacks.attackModesTable);
        }

        for (var index = 0; index < _attacks.attackModesTable.childCount; ++index)
        {
            var child = _attacks.attackModesTable.GetChild(index);
            var attackModeBox = child.GetComponent<AttackModeBox>();
            attackModeBox.Unbind();

            if (index >= _attacks.relevantAttackModes.Count)
            {
                child.gameObject.SetActive(false);
                continue;
            }

            child.gameObject.SetActive(true);
            attackModeBox.Bind(_attacks.relevantAttackModes[index]);
        }

        #endregion
    }
}
