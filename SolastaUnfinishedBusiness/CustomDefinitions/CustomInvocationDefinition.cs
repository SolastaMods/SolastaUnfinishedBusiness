using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public class CustomInvocationDefinition : InvocationDefinition, IDefinitionWithPrerequisites
{
    public CustomInvocationPoolType PoolType { get; set; }

    //TODO: add validator setter
    public List<IDefinitionWithPrerequisites.Validate> Validators { get; } =
        new List<IDefinitionWithPrerequisites.Validate>() {CheckRequiredLevel, CheckRequiredSpell, CheckRequiredPact};

    public static string CheckRequiredLevel(RulesetCharacter character, BaseDefinition definition)
    {
        if (character is not RulesetCharacterHero hero
            || definition is not CustomInvocationDefinition invocation)
        {
            return null;
        }

        var requiredLevel = invocation.RequiredLevel;
        if (requiredLevel <= 1)
        {
            return null;
        }

        int level;
        var requiredClassName = invocation.PoolType.RequireClassLevels;
        if (requiredClassName != null)
        {
            var requiredClass = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                .GetAllElements()
                .FirstOrDefault(x => x.Name == requiredClassName);

            level = hero.GetClassLevel(requiredClass);
        }
        else
        {
            level = hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
        }

        if (level < requiredLevel)
        {
            return Gui.Format(GuiInvocationDefinition.InvocationPrerequisiteLevelFormat, requiredLevel.ToString());
        }

        return null;
    }

    private static string CheckRequiredSpell(RulesetCharacter character, BaseDefinition definition)
    {
        if (character is not RulesetCharacterHero hero
            || definition is not CustomInvocationDefinition invocation)
        {
            return null;
        }

        var requiredSpell = invocation.RequiredKnownSpell;
        if (requiredSpell == null)
        {
            return null;
        }

        if (!hero.spellRepertoires.Any(r => r.HasKnowledgeOfSpell(requiredSpell)))
        {
            return Gui.Format(GuiInvocationDefinition.InvocationPrerequisiteKnownSpellFormat,
                Gui.Localize(requiredSpell.GuiPresentation.Title));
        }

        return null;
    }

    private static string CheckRequiredPact(RulesetCharacter character, BaseDefinition definition)
    {
        if (character is not RulesetCharacterHero hero
            || definition is not CustomInvocationDefinition invocation)
        {
            return null;
        }

        var requiredPact = invocation.RequiredPact;
        if (requiredPact == null)
        {
            return null;
        }

        if (!hero.HasAnyFeature(requiredPact))
        {
            return Gui.Format(GuiInvocationDefinition.InvocationPrerequisitePactFormat,
                Gui.Localize(requiredPact.GuiPresentation.Title));
        }

        return null;
    }
}

public class CustomInvocationDefinitionBuilder : InvocationDefinitionBuilder<CustomInvocationDefinition,
    CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(CustomInvocationDefinition original, string name, Guid namespaceGuid) :
        base(
            original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(CustomInvocationDefinition original, string name, string definitionGuid)
        :
        base(original, name, definitionGuid)
    {
    }

    public CustomInvocationDefinitionBuilder SetPoolType(CustomInvocationPoolType poolType)
    {
        Definition.PoolType = poolType;
        return this;
    }
}
