using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterBackgroundDefinitionBuilder
    : DefinitionBuilder<CharacterBackgroundDefinition, CharacterBackgroundDefinitionBuilder>
{
    internal CharacterBackgroundDefinitionBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features);
        return this;
    }

    internal CharacterBackgroundDefinitionBuilder SetBanterList(BanterDefinitions.BanterList banterList)
    {
        Definition.banterList = banterList;
        return this;
    }

    internal CharacterBackgroundDefinitionBuilder AddStaticPersonality(string personalityFlag, int weight)
    {
        Definition.staticPersonalityFlags.Add(
            new PersonalityFlagOccurence(
                DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
            {
                weight = weight, personalityFlag = personalityFlag
            });

        Definition.staticPersonalityFlags.Sort((x, y) =>
            String.Compare(x.PersonalityFlag, y.PersonalityFlag, StringComparison.CurrentCultureIgnoreCase));

        return this;
    }

    internal CharacterBackgroundDefinitionBuilder AddOptionalPersonality(string personalityFlag, int weight)
    {
        Definition.optionalPersonalityFlags.Add(
            new PersonalityFlagOccurence(
                DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
            {
                weight = weight, personalityFlag = personalityFlag
            });

        Definition.optionalPersonalityFlags.Sort((x, y) =>
            String.Compare(x.PersonalityFlag, y.PersonalityFlag, StringComparison.CurrentCultureIgnoreCase));

        return this;
    }

    internal CharacterBackgroundDefinitionBuilder AddEquipmentRow(
        params IEnumerable<CharacterClassDefinition.HeroEquipmentOption>[] equipmentLists)
    {
        var equipmentRow = new CharacterClassDefinition.HeroEquipmentRow();

        foreach (var list in equipmentLists)
        {
            var column = new CharacterClassDefinition.HeroEquipmentColumn();

            column.EquipmentOptions.AddRange(list);
            equipmentRow.EquipmentColumns.Add(column);
        }

        Definition.EquipmentRows.Add(equipmentRow);

        return this;
    }

    #region Constructors

    protected CharacterBackgroundDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterBackgroundDefinitionBuilder(CharacterBackgroundDefinition original, string name,
        Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
