using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterClassDefinitionBuilder
    : DefinitionBuilder<CharacterClassDefinition, CharacterClassDefinitionBuilder>
{
    internal CharacterClassDefinitionBuilder SetHitDice(RuleDefinitions.DieType die)
    {
        Definition.hitDice = die;
        return this;
    }

    internal CharacterClassDefinitionBuilder SetAbilityScorePriorities(string first, string second, string third,
        string fourth, string fifth, string sixth)
    {
        Definition.AbilityScoresPriority.SetRange(first, second, third, fourth, fifth, sixth);
        return this;
    }

#if false
    internal CharacterClassDefinitionBuilder AddPersonality(PersonalityFlagDefinition personalityType, int weight)
    {
        Definition.PersonalityFlagOccurences.Add(
            new PersonalityFlagOccurence(
                DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
            {
                weight = weight, personalityFlag = personalityType.Name
            });

        Definition.PersonalityFlagOccurences.Sort((x, y) =>
            String.Compare(x.PersonalityFlag, y.PersonalityFlag, StringComparison.CurrentCultureIgnoreCase));

        return this;
    }
    
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
        IEnumerable<CharacterClassDefinition.HeroEquipmentOption> equipmentListA,
        IEnumerable<CharacterClassDefinition.HeroEquipmentOption> equipmentListB)
    {
        var equipmentColumnA = new CharacterClassDefinition.HeroEquipmentColumn();
        equipmentColumnA.EquipmentOptions.AddRange(equipmentListA);

        var equipmentColumnB = new CharacterClassDefinition.HeroEquipmentColumn();
        equipmentColumnB.EquipmentOptions.AddRange(equipmentListB);

        var equipmentRow = new CharacterClassDefinition.HeroEquipmentRow();
        equipmentRow.EquipmentColumns.Add(equipmentColumnA);
        equipmentRow.EquipmentColumns.Add(equipmentColumnB);

        Definition.EquipmentRows.Add(equipmentRow);

        return this;
    }

    internal CharacterClassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
        }
        else
        {
            features.Do(x => x.GuiPresentation.sortOrder = level);
        }

        return this;
    }

    #region Constructors

    protected CharacterClassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterClassDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CharacterClassDefinitionBuilder(CharacterClassDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected CharacterClassDefinitionBuilder(CharacterClassDefinition original, string name, string definitionGuid)
        : base(original, name, definitionGuid)
    {
    }

    #endregion

    #region Tool preference

    internal CharacterClassDefinitionBuilder AddToolPreferences(params ToolTypeDefinition[] toolTypes)
    {
        Definition.ToolAutolearnPreference.AddRange(toolTypes.Select(tt => tt.Name));
        Definition.ToolAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Skill preference

    internal CharacterClassDefinitionBuilder AddSkillPreferences(params SkillDefinition[] skillTypes)
    {
        Definition.SkillAutolearnPreference.AddRange(skillTypes.Select(st => st.Name));
        return this;
    }

    internal CharacterClassDefinitionBuilder AddSkillPreferences(params string[] skillTypes)
    {
        Definition.SkillAutolearnPreference.AddRange(skillTypes);
        return this;
    }

    #endregion

    #region Expertise preference

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

    #endregion

    #region Feat preference

    internal CharacterClassDefinitionBuilder AddFeatPreferences(params FeatDefinition[] featTypes)
    {
        Definition.FeatAutolearnPreference.AddRange(featTypes.Select(ft => ft.Name));
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }
    
    internal CharacterClassDefinitionBuilder AddFeatPreferences(params string[] feats)
    {
        Definition.FeatAutolearnPreference.AddRange(feats);
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }

    #endregion

#if false
    #region Metamagic preference

    internal CharacterClassDefinitionBuilder AddMetamagicPreference(MetamagicOptionDefinition option)
    {
        Definition.MetamagicAutolearnPreference.Add(option.Name);
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }

    internal CharacterClassDefinitionBuilder AddMetamagicPreferences(params MetamagicOptionDefinition[] options)
    {
        AddMetamagicPreferences(options.AsEnumerable());
        return this;
    }

    internal CharacterClassDefinitionBuilder AddMetamagicPreferences(IEnumerable<MetamagicOptionDefinition> options)
    {
        Definition.FeatAutolearnPreference.AddRange(options.Select(o => o.Name));
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }

    #endregion
#endif
}
