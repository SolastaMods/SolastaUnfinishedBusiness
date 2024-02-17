using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TA.AI;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterClassDefinitionBuilder
    : DefinitionBuilder<CharacterClassDefinition, CharacterClassDefinitionBuilder>
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    internal CharacterClassDefinitionBuilder SetVocalSpellSemeClass(VocalSpellSemeClass vocalSpellSemeClass)
    {
        Definition.vocalSpellSemeClass = vocalSpellSemeClass;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetHitDice(DieType die)
    {
        Definition.hitDice = die;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetAbilityScorePriorities(params string[] abilityScores)
    {
        Definition.AbilityScoresPriority.SetRange(abilityScores);
        return this;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    internal CharacterClassDefinitionBuilder AddPersonality(string personalityFlag, int weight)
    {
        Definition.PersonalityFlagOccurences.Add(
            new PersonalityFlagOccurence(
                DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
            {
                weight = weight, personalityFlag = personalityFlag
            });

        Definition.PersonalityFlagOccurences.Sort((x, y) =>
            String.Compare(x.PersonalityFlag, y.PersonalityFlag, StringComparison.CurrentCultureIgnoreCase));

        return this;
    }

#if false
    internal CharacterClassDefinitionBuilder RequireDeity()
    {
        Definition.requiresDeity = true;
        return this;
    }
#endif

    internal CharacterClassDefinitionBuilder SetIngredientGatheringOdds(int odds)
    {
        Definition.ingredientGatheringOdds = odds;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetBattleAI(DecisionPackageDefinition decisionPackage)
    {
        Definition.defaultBattleDecisions = decisionPackage;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetPictogram(AssetReferenceSprite sprite)
    {
        Definition.classPictogramReference = sprite;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetAnimationId(AnimationDefinitions.ClassAnimationId animId)
    {
        Definition.classAnimationId = animId;
        return this;
    }

    internal CharacterClassDefinitionBuilder AddEquipmentRow(
        params CharacterClassDefinition.HeroEquipmentOption[] equipmentList)
    {
        var equipmentColumn = new CharacterClassDefinition.HeroEquipmentColumn();

        equipmentColumn.EquipmentOptions.AddRange(equipmentList);

        var equipmentRow = new CharacterClassDefinition.HeroEquipmentRow();

        equipmentRow.EquipmentColumns.Add(equipmentColumn);

        Definition.EquipmentRows.Add(equipmentRow);
        return this;
    }

    internal CharacterClassDefinitionBuilder AddEquipmentRow(
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

    internal CharacterClassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
        return this;
    }

    #region Tool preference

    internal CharacterClassDefinitionBuilder AddToolPreferences(params ToolTypeDefinition[] toolTypes)
    {
        Definition.ToolAutolearnPreference.AddRange(toolTypes.Select(tt => tt.Name));
        Definition.ToolAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Skill preference

    internal CharacterClassDefinitionBuilder AddSkillPreferences(params string[] skillTypes)
    {
        Definition.SkillAutolearnPreference.AddRange(skillTypes);
        return this;
    }

    #endregion

    #region Expertise preference

#if false
    internal CharacterClassDefinitionBuilder AddExpertisePreferences(params SkillDefinition[] skillTypes)
    {
        Definition.ExpertiseAutolearnPreference.AddRange(skillTypes.Select(st => st.Name));
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }

    internal CharacterClassDefinitionBuilder AddExpertisePreferences(params ToolTypeDefinition[] toolTypes)
    {
        Definition.ExpertiseAutolearnPreference.AddRange(toolTypes.Select(tt => tt.Name));
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }
#endif

    #endregion

    #region Feat preference

#if false
    internal CharacterClassDefinitionBuilder AddFeatPreferences(params FeatDefinition[] featTypes)
    {
        Definition.FeatAutolearnPreference.AddRange(featTypes.Select(ft => ft.Name));
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }
#endif

    internal CharacterClassDefinitionBuilder AddFeatPreferences(params string[] feats)
    {
        Definition.FeatAutolearnPreference.AddRange(feats);
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Metamagic preference

#if false
    internal CharacterClassDefinitionBuilder AddMetamagicPreference(MetamagicOptionDefinition option)
    {
        Definition.MetamagicAutolearnPreference.Add(option.Name);
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }

    internal CharacterClassDefinitionBuilder AddMetamagicPreferences(params MetamagicOptionDefinition[] options)
    {
        Definition.FeatAutolearnPreference.AddRange(options.Select(o => o.Name));
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }
#endif

    #endregion

    #region Constructors

    protected CharacterClassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterClassDefinitionBuilder(CharacterClassDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
