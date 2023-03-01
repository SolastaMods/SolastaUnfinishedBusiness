using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomCharacterStatsPanel
{
    private readonly AbilityScoresListingPanel abilities;
    private readonly AttackModesPanel attacks;
    private readonly Button button;

    private readonly Transform root;
    private readonly CharacterStatsPanel stats;
    private bool bound;
    private RulesetCharacter character;
    private GuiCharacter guiCharacter;
    private bool visible;

    private CustomCharacterStatsPanel()
    {
        root = new GameObject().transform;
        var inspector = Gui.GuiService.GetScreen<CharacterInspectionScreen>();

        root.localPosition = new Vector3(225, 125, 0);
        root.gameObject.SetActive(false);

        // ReSharper disable once Unity.UnknownResource
        var prefab = Resources.Load<GameObject>("Gui/Prefabs/Component/SmallButtonRoundImage");
        button = Object.Instantiate(prefab).GetComponent<Button>();
        button.gameObject.SetActive(false);
        button.transform.localPosition = new Vector3(50, 50, 0);
        button.onClick.AddListener(() =>
        {
            if (!bound) { return; }

            visible = !visible;
            UpdateVisibility();
        });

        #region Abilities

        abilities = Object.Instantiate(inspector.abilityScoresListingPanelPrefab, root)
            .GetComponent<AbilityScoresListingPanel>();
        abilities.transform.localPosition = new Vector3(0, 100, 0);

        for (var index = 0; index < AttributeDefinitions.AbilityScoreNames.Length; ++index)
        {
            abilities.abilityScoreBoxes.Add(Gui.GetPrefabFromPool(abilities.boxPrefab, abilities.table)
                .GetComponent<AbilityScoreBox>());
        }

        #endregion

        #region Stats

        stats = Object.Instantiate(inspector.characterStatsPanelPrefab, root)
            .GetComponent<CharacterStatsPanel>();
        stats.transform.localPosition = new Vector3(0, 0, 0);

        #endregion

        #region Attacks

        attacks = Gui.GetPrefabFromPool(inspector.attackModesPanelPrefab, root)
            .GetComponent<AttackModesPanel>();
        attacks.transform.localPosition = new Vector3(275, 200, 0);

        #endregion
    }

    public static CustomCharacterStatsPanel Instance => MaybeInstance ??= new CustomCharacterStatsPanel();
    public static CustomCharacterStatsPanel MaybeInstance { get; private set; }

    private void UpdateVisibility()
    {
        root.gameObject.SetActive(visible);
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

        character = target;
        guiCharacter = new GuiCharacter(character);
        var parent = screen.CharacterControlPanel.ActiveCharacterPanel.transform;

        root.parent = parent;
        button.transform.parent = parent;
        button.gameObject.SetActive(true);

        #region Abilities

        for (var index = 0; index < AttributeDefinitions.AbilityScoreNames.Length; ++index)
        {
            var attribute = character.GetAttribute(AttributeDefinitions.AbilityScoreNames[index]);
            abilities.abilityScoreBoxes[index].Bind(attribute, abilities.abilityScoreBoxes);
        }

        #endregion

        #region Stats

        stats.guiCharacter = guiCharacter;

        stats.initiativeBox.Bind(character.GetAttribute(AttributeDefinitions.Initiative), "+0;-#");
        stats.initiativeBox.Activate(true);

        var proficiencyBonus = character.GetAttribute(AttributeDefinitions.ProficiencyBonus);
        stats.proficiencyBox.Activate(proficiencyBonus != null);
        if (stats.proficiencyBox.Activated)
        {
            stats.proficiencyBox.Bind(proficiencyBonus, "+0;-#");
        }

        stats.armorClassBox.Activate(proficiencyBonus != null);
        stats.armorClassBox.gameObject.SetActive(false);
        stats.hitDiceBox.Activate(proficiencyBonus != null);
        stats.hitDiceBox.gameObject.SetActive(false);
        stats.hitPointBox.Activate(proficiencyBonus != null);
        stats.hitPointBox.gameObject.SetActive(false);

        #endregion

        bound = true;

        Refresh();
        UpdateVisibility();
    }

    public void Unbind()
    {
        bound = false;
        root.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        character = null;
        guiCharacter = null;

        #region Abilities

        abilities.Unbind();

        #endregion

        #region Stats

        stats.Unbind();
        stats.initiativeBox.Unbind();
        stats.moveBox.Unbind();
        stats.proficiencyBox.Unbind();

        #endregion

        #region Attacks

        attacks.Unbind();

        #endregion
    }

    public void Refresh()
    {
        if (!bound || character == null || guiCharacter == null)
        {
            return;
        }

        #region Stats

        stats.initiativeBox.Refresh();
        stats.moveBox.ValueLabel.Text = guiCharacter.MovePoints;
        stats.proficiencyBox.Refresh();

        #endregion

        #region Attacks

        //refresh

        attacks.relevantAttackModes.Clear();
        attacks.relevantAttackModes.AddRange(character.AttackModes
            .Where(m => m.ActionType is ActionDefinitions.ActionType.Main or ActionDefinitions.ActionType.Bonus));

        while (attacks.attackModesTable.childCount < attacks.relevantAttackModes.Count)
        {
            Gui.GetPrefabFromPool(attacks.attackModePrefab, attacks.attackModesTable);
        }

        for (var index = 0; index < attacks.attackModesTable.childCount; ++index)
        {
            var child = attacks.attackModesTable.GetChild(index);
            if (index < attacks.relevantAttackModes.Count)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<AttackModeBox>().Bind(attacks.relevantAttackModes[index]);
            }
            else
            {
                child.GetComponent<AttackModeBox>().Unbind();
                child.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
