using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class InvocationDefinitionCustom : InvocationDefinition, IDefinitionWithPrerequisites
{
    internal InvocationPoolTypeCustom PoolType { get; set; }

    /**Used for tooltip in selection screen*/
    internal ItemDefinition Item { get; set; }

    internal Id MainActionId => PoolType?.MainActionId ?? Id.CastInvocation;
    private Id BonusActionId => PoolType?.BonusActionId ?? (Id)ExtraActionId.CastInvocationBonus;
    private Id NoCostActionId => PoolType?.NoCostActionId ?? (Id)ExtraActionId.CastInvocationNoCost;

    internal Id BattleActionId
    {
        get
        {
            var type = this.GetActionType();

            return type switch
            {
                ActionType.Main => MainActionId,
                ActionType.Bonus => BonusActionId,
                ActionType.NoCost => NoCostActionId,
                _ => MainActionId
            };
        }
    }

    //TODO: add validator setter
    public IEnumerable<IDefinitionWithPrerequisites.Validate> Validators { get; } =
        new IDefinitionWithPrerequisites.Validate[] { CheckRequiredLevel, CheckRequiredSpell, CheckRequiredPact };

    private static bool CheckRequiredLevel(
        RulesetCharacter character,
        BaseDefinition definition,
        out string requirement)
    {
        requirement = null;

        if (character is not RulesetCharacterHero hero
            || definition is not InvocationDefinitionCustom invocation)
        {
            return true;
        }

        var requiredLevel = invocation.RequiredLevel;

        if (requiredLevel <= 1)
        {
            return true;
        }

        int level;
        var requiredClassName = invocation.PoolType.RequireClassLevels;

        if (requiredClassName != null)
        {
            var requiredClass = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                .FirstOrDefault(x => x.Name == requiredClassName);

            level = hero.GetClassLevel(requiredClass);

            var levelText = requiredLevel.ToString();
            var classText = "<UNKNOWN>";

            if (requiredClass != null)
            {
                classText = requiredClass.FormatTitle();
            }

            if (level < requiredLevel)
            {
                levelText = Gui.Colorize(levelText, Gui.ColorFailure);
            }

            requirement = Gui.Format(CustomTooltipProvider.RequireClassLevel, levelText, classText);
        }
        else
        {
            level = hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

            var levelText = level.ToString();

            if (level < requiredLevel)
            {
                levelText = Gui.Colorize(levelText, Gui.ColorFailure);
            }

            requirement = Gui.Format(CustomTooltipProvider.RequireCharacterLevel, levelText);
        }

        return level >= requiredLevel;
    }

    private static bool CheckRequiredSpell(
        RulesetCharacter character,
        BaseDefinition definition,
        out string requirement)
    {
        requirement = null;

        if (character is not RulesetCharacterHero hero
            || definition is not InvocationDefinitionCustom invocation)
        {
            return true;
        }

        var requiredSpell = invocation.RequiredKnownSpell;

        if (requiredSpell == null)
        {
            return true;
        }

        var text = requiredSpell.FormatTitle();
        var valid = hero.spellRepertoires.Any(r => r.HasKnowledgeOfSpell(requiredSpell));

        if (!valid)
        {
            text = Gui.Colorize(text, Gui.ColorFailure);
        }

        requirement = Gui.Format(GuiInvocationDefinition.InvocationPrerequisiteKnownSpellFormat, text);

        return valid;
    }

    private static bool CheckRequiredPact(RulesetCharacter character, BaseDefinition definition, out string requirement)
    {
        requirement = null;

        if (character is not RulesetCharacterHero hero
            || definition is not InvocationDefinitionCustom invocation)
        {
            return true;
        }

        var requiredPact = invocation.RequiredPact;

        if (requiredPact == null)
        {
            return true;
        }

        var text = requiredPact.FormatTitle();
        var valid = hero.HasAnyFeature(requiredPact);

        if (!valid)
        {
            text = Gui.Colorize(text, Gui.ColorFailure);
        }

        requirement = Gui.Format(GuiInvocationDefinition.InvocationPrerequisitePactFormat, text);

        return valid;
    }
}
