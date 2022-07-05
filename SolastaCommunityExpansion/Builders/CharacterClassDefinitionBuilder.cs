using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Builders;

public class
    CharacterClassDefinitionBuilder : DefinitionBuilder<CharacterClassDefinition, CharacterClassDefinitionBuilder>
{
    public CharacterClassDefinitionBuilder SetHitDice(RuleDefinitions.DieType die)
    {
        Definition.hitDice = die;
        return this;
    }

    public CharacterClassDefinitionBuilder SetAbilityScorePriorities(string first, string second, string third,
        string fourth, string fifth, string sixth)
    {
        Definition.AbilityScoresPriority.SetRange(first, second, third, fourth, fifth, sixth);
        return this;
    }

    public CharacterClassDefinitionBuilder AddPersonality(PersonalityFlagDefinition personalityType, int weight)
    {
        Definition.PersonalityFlagOccurences.Add(
            new PersonalityFlagOccurence(
                DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
            {
                weight = weight, personalityFlag = personalityType.Name
            });

        Definition.PersonalityFlagOccurences.Sort((x, y) => x.PersonalityFlag.CompareTo(y.PersonalityFlag));

        return this;
    }

    public CharacterClassDefinitionBuilder SetIngredientGatheringOdds(int odds)
    {
        Definition.ingredientGatheringOdds = odds;
        return this;
    }

    public CharacterClassDefinitionBuilder SetBattleAI(DecisionPackageDefinition decisionPackage)
    {
        Definition.defaultBattleDecisions = decisionPackage;
        return this;
    }

    public CharacterClassDefinitionBuilder SetPictogram(AssetReferenceSprite sprite)
    {
        Definition.classPictogramReference = sprite;
        return this;
    }

    public CharacterClassDefinitionBuilder SetAnimationId(AnimationDefinitions.ClassAnimationId animId)
    {
        Definition.classAnimationId = animId;
        return this;
    }

    public CharacterClassDefinitionBuilder RequireDeity()
    {
        Definition.requiresDeity = true;
        return this;
    }

    public CharacterClassDefinitionBuilder AddEquipmentRow(params HeroEquipmentOption[] equipmentList)
    {
        return AddEquipmentRow(equipmentList.AsEnumerable());
    }

    public CharacterClassDefinitionBuilder AddEquipmentRow(IEnumerable<HeroEquipmentOption> equipmentList)
    {
        var equipmentColumn = new HeroEquipmentColumn();
        equipmentColumn.EquipmentOptions.AddRange(equipmentList);

        var equipmentRow = new HeroEquipmentRow();
        equipmentRow.EquipmentColumns.Add(equipmentColumn);

        Definition.EquipmentRows.Add(equipmentRow);

        return this;
    }

    public CharacterClassDefinitionBuilder AddEquipmentRow(IEnumerable<HeroEquipmentOption> equipmentListA,
        IEnumerable<HeroEquipmentOption> equipmentListB)
    {
        var equipmentColumnA = new HeroEquipmentColumn();
        equipmentColumnA.EquipmentOptions.AddRange(equipmentListA);

        var equipmentColumnB = new HeroEquipmentColumn();
        equipmentColumnB.EquipmentOptions.AddRange(equipmentListB);

        var equipmentRow = new HeroEquipmentRow();
        equipmentRow.EquipmentColumns.Add(equipmentColumnA);
        equipmentRow.EquipmentColumns.Add(equipmentColumnB);

        Definition.EquipmentRows.Add(equipmentRow);

        return this;
    }

    public CharacterClassDefinitionBuilder AddFeatureAtLevel(int level, FeatureDefinition feature, int number = 1)
    {
        for (var i = 0; i < number; i++)
        {
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));
        }

        Definition.FeatureUnlocks.Sort(Sorting.Compare);
        return this;
    }

    public CharacterClassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
        Definition.FeatureUnlocks.Sort(Sorting.Compare);
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

    public CharacterClassDefinitionBuilder AddToolPreference(ToolTypeDefinition toolType)
    {
        Definition.ToolAutolearnPreference.Add(toolType.Name);
        Definition.ToolAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddToolPreferences(params ToolTypeDefinition[] toolTypes)
    {
        AddToolPreferences(toolTypes.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddToolPreferences(IEnumerable<ToolTypeDefinition> toolTypes)
    {
        Definition.ToolAutolearnPreference.AddRange(toolTypes.Select(tt => tt.Name));
        Definition.ToolAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Skill preference

    public CharacterClassDefinitionBuilder AddSkillPreference(SkillDefinition skillType)
    {
        Definition.SkillAutolearnPreference.Add(skillType.Name);
        Definition.SkillAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddSkillPreferences(params SkillDefinition[] skillTypes)
    {
        AddSkillPreferences(skillTypes.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddSkillPreferences(IEnumerable<SkillDefinition> skillTypes)
    {
        Definition.SkillAutolearnPreference.AddRange(skillTypes.Select(st => st.Name));
        Definition.SkillAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Expertise preference

#if false
    public CharacterClassDefinitionBuilder AddExpertisePreference(SkillDefinition skillType)
    {
        Definition.ExpertiseAutolearnPreference.Add(skillType.Name);
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddExpertisePreference(ToolTypeDefinition toolType)
    {
        Definition.ExpertiseAutolearnPreference.Add(toolType.Name);
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddExpertisePreferences(params SkillDefinition[] skillTypes)
    {
        AddExpertisePreferences(skillTypes.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddExpertisePreferences(params ToolTypeDefinition[] toolTypes)
    {
        AddExpertisePreferences(toolTypes.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddExpertisePreferences(IEnumerable<SkillDefinition> skillTypes)
    {
        Definition.ExpertiseAutolearnPreference.AddRange(skillTypes.Select(st => st.Name));
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddExpertisePreferences(IEnumerable<ToolTypeDefinition> toolTypes)
    {
        Definition.ExpertiseAutolearnPreference.AddRange(toolTypes.Select(tt => tt.Name));
        Definition.ExpertiseAutolearnPreference.Sort();
        return this;
    }
#endif

    #endregion

    #region Feat preference

    public CharacterClassDefinitionBuilder AddFeatPreference(FeatDefinition featType)
    {
        Definition.FeatAutolearnPreference.Add(featType.Name);
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddFeatPreferences(params FeatDefinition[] featTypes)
    {
        AddFeatPreferences(featTypes.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddFeatPreferences(IEnumerable<FeatDefinition> featTypes)
    {
        Definition.FeatAutolearnPreference.AddRange(featTypes.Select(ft => ft.Name));
        Definition.FeatAutolearnPreference.Sort();
        return this;
    }

    #endregion

    #region Metamagic preference

#if false
    public CharacterClassDefinitionBuilder AddMetamagicPreference(MetamagicOptionDefinition option)
    {
        Definition.MetamagicAutolearnPreference.Add(option.Name);
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }

    public CharacterClassDefinitionBuilder AddMetamagicPreferences(params MetamagicOptionDefinition[] options)
    {
        AddMetamagicPreferences(options.AsEnumerable());
        return this;
    }

    public CharacterClassDefinitionBuilder AddMetamagicPreferences(IEnumerable<MetamagicOptionDefinition> options)
    {
        Definition.FeatAutolearnPreference.AddRange(options.Select(o => o.Name));
        Definition.MetamagicAutolearnPreference.Sort();
        return this;
    }
#endif

    #endregion
}
