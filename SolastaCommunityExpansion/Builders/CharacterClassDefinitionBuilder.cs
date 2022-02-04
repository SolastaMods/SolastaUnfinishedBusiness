using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using TA.AI;
using UnityEngine.AddressableAssets;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Builders
{
    public class CharacterClassDefinitionBuilder : BaseDefinitionBuilder<CharacterClassDefinition>
    {
        public CharacterClassDefinitionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public CharacterClassDefinitionBuilder(string name, Guid namespaceGuid, Category category = Category.None) : base(name, namespaceGuid, category)
        {
        }

        public CharacterClassDefinitionBuilder(CharacterClassDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public CharacterClassDefinitionBuilder(CharacterClassDefinition original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public CharacterClassDefinitionBuilder SetHitDice(RuleDefinitions.DieType die)
        {
            Definition.SetHitDice(die);
            return this;
        }

        public CharacterClassDefinitionBuilder SetAbilityScorePriorities(string first, string second, string third, string fourth, string fifth, string sixth)
        {
            Definition.AbilityScoresPriority.SetRange(new[] { first, second, third, fourth, fifth, sixth });
            return this;
        }

        public CharacterClassDefinitionBuilder AddPersonality(PersonalityFlagDefinition personalityType, int weight)
        {
            Definition.PersonalityFlagOccurences.Add(
              new PersonalityFlagOccurence(DatabaseHelper.CharacterClassDefinitions.Fighter.PersonalityFlagOccurences[0])
                .SetWeight(weight)
                .SetPersonalityFlag(personalityType.Name));
            return this;
        }

        #region Tool preference
        public CharacterClassDefinitionBuilder AddToolPreference(ToolTypeDefinition toolType)
        {
            Definition.ToolAutolearnPreference.Add(toolType.Name);
            return this;
        }

        public CharacterClassDefinitionBuilder AddToolPreferences(params ToolTypeDefinition[] toolTypes)
        {
            AddToolPreferences(toolTypes.AsEnumerable());
            return this;
        }

        public CharacterClassDefinitionBuilder AddToolPreferences(IEnumerable<ToolTypeDefinition> toolTypes)
        {
            foreach (var toolType in toolTypes)
            {
                Definition.ToolAutolearnPreference.Add(toolType.Name);
            }
            return this;
        }
        #endregion

        #region Skill preference
        public CharacterClassDefinitionBuilder AddSkillPreference(SkillDefinition skillType)
        {
            Definition.SkillAutolearnPreference.Add(skillType.Name);
            return this;
        }

        public CharacterClassDefinitionBuilder AddSkillPreferences(params SkillDefinition[] skillTypes)
        {
            AddSkillPreferences(skillTypes.AsEnumerable());
            return this;
        }

        public CharacterClassDefinitionBuilder AddSkillPreferences(IEnumerable<SkillDefinition> skillTypes)
        {
            foreach (var skillType in skillTypes)
            {
                Definition.SkillAutolearnPreference.Add(skillType.Name);
            }
            return this;
        }
        #endregion

        #region Expertise preference

        public CharacterClassDefinitionBuilder AddExpertisePreference(SkillDefinition skillType)
        {
            Definition.ExpertiseAutolearnPreference.Add(skillType.Name);
            return this;
        }

        public CharacterClassDefinitionBuilder AddExpertisePreference(ToolTypeDefinition toolType)
        {
            Definition.ExpertiseAutolearnPreference.Add(toolType.Name);
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
            foreach (var skillType in skillTypes)
            {
                Definition.ExpertiseAutolearnPreference.Add(skillType.Name);
            }
            return this;
        }

        public CharacterClassDefinitionBuilder AddExpertisePreferences(IEnumerable<ToolTypeDefinition> toolTypes)
        {
            foreach (var toolType in toolTypes)
            {
                Definition.ExpertiseAutolearnPreference.Add(toolType.Name);
            }
            return this;
        }

        #endregion

        #region Feat preference

        public CharacterClassDefinitionBuilder AddFeatPreference(FeatDefinition featType)
        {
            Definition.FeatAutolearnPreference.Add(featType.Name);
            return this;
        }

        public CharacterClassDefinitionBuilder AddFeatPreferences(params FeatDefinition[] featTypes)
        {
            AddFeatPreferences(featTypes.AsEnumerable());
            return this;
        }

        public CharacterClassDefinitionBuilder AddFeatPreferences(IEnumerable<FeatDefinition> featTypes)
        {
            foreach (var featType in featTypes)
            {
                Definition.FeatAutolearnPreference.Add(featType.Name);
            }
            return this;
        }

        #endregion

        #region Metamagic preference

        public CharacterClassDefinitionBuilder AddMetamagicPreference(MetamagicOptionDefinition option)
        {
            Definition.MetamagicAutolearnPreference.Add(option.Name);
            return this;
        }

        public CharacterClassDefinitionBuilder AddMetamagicPreferences(params MetamagicOptionDefinition[] options)
        {
            AddMetamagicPreferences(options.AsEnumerable());
            return this;
        }

        public CharacterClassDefinitionBuilder AddMetamagicPreferences(IEnumerable<MetamagicOptionDefinition> options)
        {
            foreach (var option in options)
            {
                Definition.FeatAutolearnPreference.Add(option.Name);
            }
            return this;
        }

        #endregion

        public CharacterClassDefinitionBuilder SetIngredientGatheringOdds(int odds)
        {
            Definition.SetIngredientGatheringOdds(odds);
            return this;
        }

        public CharacterClassDefinitionBuilder SetBattleAI(DecisionPackageDefinition decisionPackage)
        {
            Definition.SetDefaultBattleDecisions(decisionPackage);
            return this;
        }

        public CharacterClassDefinitionBuilder SetPictogram(AssetReferenceSprite sprite)
        {
            Definition.SetClassPictogramReference(sprite);
            return this;
        }

        public CharacterClassDefinitionBuilder SetAnimationId(AnimationDefinitions.ClassAnimationId animId)
        {
            Definition.SetClassAnimationId(animId);
            return this;
        }

        public CharacterClassDefinitionBuilder RequireDeity()
        {
            Definition.SetRequiresDeity(true);
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

        public CharacterClassDefinitionBuilder AddEquipmentRow(IEnumerable<HeroEquipmentOption> equipmentListA, IEnumerable<HeroEquipmentOption> equipmentListB)
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

        public CharacterClassDefinitionBuilder AddFeatureAtLevel(FeatureDefinition feature, int level)
        {
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));
            return this;
        }

        public CharacterClassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
        {
            Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
            return this;
        }

        public FeatureDefinitionSubclassChoice BuildSubclassChoice(int level, string subclassSuffix, bool requireDeity, string name, GuiPresentation guiPresentation, string guid)
        {
            var builder = new FeatureDefinitionSubclassChoiceBuilder(name, guid);

            var subclassChoice = builder
                .SetSubclassSuffix(subclassSuffix)
                .SetFilterByDeity(requireDeity)
                .SetGuiPresentation(guiPresentation)
                .AddToDB();

            AddFeatureAtLevel(subclassChoice, level);

            return subclassChoice;
        }
    }
}
