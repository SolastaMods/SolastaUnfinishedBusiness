using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FeatHelpers
{
    internal sealed class ModifyWeaponAttackModeTypeFilter : IModifyWeaponAttackMode
    {
        private readonly FeatDefinition _source;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = [];

        public ModifyWeaponAttackModeTypeFilter(
            FeatDefinition source,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _source = source;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(1, FeatureSourceType.Feat, _source.Name, _source));
        }
    }

    internal sealed class SkillOrExpertise(
        SkillDefinition skillDefinition,
        FeatureDefinitionProficiency skill,
        FeatureDefinitionProficiency expertise) : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            var buildingData = hero.GetHeroBuildingData();

            hero.ActiveFeatures[tag].TryAdd(
                hero.TrainedSkills.Contains(skillDefinition) ||
                buildingData.LevelupTrainedSkills.Any(x => x.Value.Contains(skillDefinition))
                    ? expertise
                    : skill);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    internal sealed class ToolOrExpertise(
        ToolTypeDefinition toolTypeDefinition,
        FeatureDefinitionProficiency tool,
        FeatureDefinitionProficiency expertise) : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            var buildingData = hero.GetHeroBuildingData();

            hero.ActiveFeatures[tag].TryAdd(
                hero.TrainedToolTypes.Contains(toolTypeDefinition) ||
                buildingData.LevelupTrainedToolTypes.Any(x => x.Value.Contains(toolTypeDefinition))
                    ? expertise
                    : tool);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    internal sealed class SpellTag
    {
        internal SpellTag(string spellTag, bool forceFixedList = false)
        {
            Name = spellTag;
            ForceFixedList = forceFixedList;
        }

        internal string Name { get; }
        internal bool ForceFixedList { get; }
    }
}
