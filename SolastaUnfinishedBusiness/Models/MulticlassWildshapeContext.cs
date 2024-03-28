using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static FeatureDefinitionFeatureSet;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class MulticlassWildshapeContext
{
    private const string TagWildShape = "99WildShape";
    private const string NaturalAcTitle = "Tooltip/&MonsterNaturalArmorTitle";
    private const string TagMonsterBase = "<Base>";
    private const string TagNaturalAc = "<NaturalArmor>";

    private static readonly List<string> AllowedAttributes =
    [
        AttributeDefinitions.RageDamage,
        AttributeDefinitions.RagePoints,
        AttributeDefinitions.KiPoints,
        AttributeDefinitions.SorceryPoints,
        AttributeDefinitions.BardicInspirationDie,
        AttributeDefinitions.BardicInspirationNumber
    ];

    private static readonly List<string> MentalAttributes =
        [AttributeDefinitions.Intelligence, AttributeDefinitions.Wisdom, AttributeDefinitions.Charisma];

    internal static void FinalizeMonster(RulesetCharacterMonster monster, bool keepMentalAbilityScores)
    {
        UpdateSenses(monster);
        UpdateUsablePowers(monster);
        UpdateAttributeModifiers(monster, keepMentalAbilityScores);
        FixShapeShiftedAc(monster);
    }

    internal static void HandleExtraUnarmedAttacks(RulesetCharacterMonster monster)
    {
        if (monster.originalFormCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        RulesetAttackMode bonusUnarmedAttack = null;

        monster.attackModifiers = monster.GetSubFeaturesByType<IAttackModificationProvider>();

        foreach (var attackModifier in monster.attackModifiers
                     .Where(attackModifier => attackModifier.AdditionalBonusUnarmedStrikeAttacksFromMain))
        {
            if (bonusUnarmedAttack != null)
            {
                if (attackModifier.AdditionalBonusUnarmedStrikeAttacksCount <= bonusUnarmedAttack.AttacksNumber)
                {
                    continue;
                }

                bonusUnarmedAttack.AttacksNumber = attackModifier.AdditionalBonusUnarmedStrikeAttacksCount;

                if (!string.IsNullOrEmpty(attackModifier.AdditionalBonusUnarmedStrikeAttacksTag))
                {
                    bonusUnarmedAttack.AddAttackTagAsNeeded(attackModifier.AdditionalBonusUnarmedStrikeAttacksTag);
                }
            }
            else
            {
                var strikeDefinition = hero.UnarmedStrikeDefinition;

                bonusUnarmedAttack = monster.RefreshAttackMode(
                    ActionType.Bonus, strikeDefinition, strikeDefinition.WeaponDescription, true,
                    monster.attackModifiers, monster.FeaturesOrigin);
                bonusUnarmedAttack.AttacksNumber = attackModifier.AdditionalBonusUnarmedStrikeAttacksCount;

                if (!string.IsNullOrEmpty(attackModifier.AdditionalBonusUnarmedStrikeAttacksTag))
                {
                    bonusUnarmedAttack.AddAttackTagAsNeeded(attackModifier.AdditionalBonusUnarmedStrikeAttacksTag);
                }

                bonusUnarmedAttack.HasPriority = true;
                monster.AttackModes.Add(bonusUnarmedAttack);
            }
        }
    }

    private static void UpdateSenses(RulesetCharacterMonster monster)
    {
        if (monster.originalFormCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        hero.EnumerateFeaturesToBrowse<FeatureDefinitionSense>(hero.FeaturesToBrowse);

        foreach (var feature in hero.FeaturesToBrowse
                     .Cast<FeatureDefinitionSense>()
                     .Where(feature => !monster.ActiveFeatures.Contains(feature)))
        {
            monster.ActiveFeatures.Add(feature);
        }
    }

    //PATCH: correctly syncs powers used by hero into WS monster
    private static void UpdateUsablePowers(RulesetCharacterMonster monster)
    {
        if (monster.originalFormCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        foreach (var usablePower in hero.UsablePowers)
        {
            var monsterUsablePower =
                monster.UsablePowers.FirstOrDefault(x => x.PowerDefinition == usablePower.PowerDefinition);

            if (monsterUsablePower != null)
            {
                monsterUsablePower.remainingUses = usablePower.remainingUses;
            }
        }
    }

    private static void UpdateAttributeModifiers(RulesetCharacterMonster monster, bool keepMentalAbilityScores)
    {
        if (monster.originalFormCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        // copy modifiers from original hero
        hero.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(monster.FeaturesToBrowse);

        foreach (var feature in monster.FeaturesToBrowse)
        {
            if (feature is not FeatureDefinitionAttributeModifier mod
                || !AllowedAttributes.Contains(mod.ModifiedAttribute)
                || !monster.TryGetAttribute(mod.ModifiedAttribute, out _))
            {
                continue;
            }

            mod.ApplyModifiers(monster.Attributes, TagWildShape);
        }

        // TA copies only base values of hero, let's copy current value to not tackle modifiers
        if (keepMentalAbilityScores)
        {
            foreach (var attribute in MentalAttributes)
            {
                var heroAttr = hero.GetAttribute(attribute);
                var monsterAttr = monster.GetAttribute(attribute);

                monsterAttr.BaseValue = heroAttr.BaseValue;
                //copy all race/class/subclass modifiers
                monsterAttr.ActiveModifiers.AddRange(heroAttr.ActiveModifiers
                    .Where(x => x.Tags.Any(t => t.Contains(AttributeDefinitions.TagRace)
                                                || t.Contains(AttributeDefinitions.TagClass)
                                                || t.Contains(AttributeDefinitions.TagSubclass))));
            }
        }

        // sync various spent pools
        monster.usedKiPoints = hero.usedKiPoints;
        monster.usedRagePoints = hero.usedRagePoints;
        monster.usedSorceryPoints = hero.usedSorceryPoints;
        monster.usedBardicInspiration = hero.usedBardicInspiration;
    }

    private static void FixShapeShiftedAc(RulesetCharacterMonster monster)
    {
        if (monster.originalFormCharacter is not RulesetCharacterHero)
        {
            return;
        }

        //for some reason TA didn't set armor properly and many game checks consider these forms as armored (1.4.24)
        //set armor to 'Natural' as intended
        monster.MonsterDefinition.armor = EquipmentDefinitions.EmptyMonsterArmor;

        var ac = monster.GetAttribute(AttributeDefinitions.ArmorClass);

        //Vanilla game (as of 1.4.24) sets this to monster's AC from definition
        //this breaks many AC stacking rules
        //set base AC to 0, so we can properly apply modifiers to it
        ac.BaseValue = 0;

        //basic AC - sets AC to 10
        var mod = RulesetAttributeModifier.BuildAttributeModifier(AttributeModifierOperation.Set, 10, TagMonsterBase);

        ac.AddModifier(mod);

        //natural armor of the monster
        mod = RulesetAttributeModifier.BuildAttributeModifier(
            AttributeModifierOperation.ForceIfBetter, monster.MonsterDefinition.ArmorClass, TagNaturalAc);

        ac.AddModifier(mod);

        //DEX bonus to AC
        mod = RulesetAttributeModifier.BuildAttributeModifier(
            AttributeModifierOperation.AddAbilityScoreBonus, 0, AttributeDefinitions.TagAbilityScore,
            AttributeDefinitions.Dexterity);

        ac.AddModifier(mod);
    }

    private static void RefreshWildShapeAcFeatures(RulesetCharacter rulesetCharacter, RulesetAttribute ac)
    {
        if (rulesetCharacter.OriginalFormCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        var ruleset = ServiceRepository.GetService<IRulesetImplementationService>();

        ac.RemoveModifiersByTags(TagWildShape);

        rulesetCharacter.FeaturesToBrowse.Clear();

        foreach (var pair in hero.ActiveFeatures
                     .Where(pair =>
                         pair.Key.Contains(AttributeDefinitions.TagClass) ||
                         pair.Key.Contains(AttributeDefinitions.TagSubclass)))
        {
            EnumerateFeaturesToBrowseHierarchicaly<FeatureDefinition>(
                pair.Value, rulesetCharacter.FeaturesToBrowse, FeatureSourceType.MonsterFeature, null, hero);
        }

        rulesetCharacter.RefreshArmorClassInFeatures(
            ruleset, ac, rulesetCharacter.FeaturesToBrowse, TagWildShape, FeatureSourceType.CharacterFeature,
            string.Empty);
    }

    private static void UpdateWildShapeAcTrends(
        List<RulesetAttributeModifier> modifiers,
        RulesetCharacterMonster monster,
        RulesetAttribute ac)
    {
        //Add trends for built-in AC mods (base ac, natural armor, dex bonus)
        foreach (var mod in modifiers)
        {
            if (mod.Tags.Contains(TagMonsterBase))
            {
                ac.ValueTrends.Add(
                    new TrendInfo((int)mod.value, FeatureSourceType.Base, string.Empty, monster, mod)
                    {
                        additive = false
                    });
            }
            else if (mod.Tags.Contains(TagNaturalAc))
            {
                ac.ValueTrends.Add(
                    new TrendInfo((int)mod.value, FeatureSourceType.ExplicitFeature, NaturalAcTitle, monster, mod)
                    {
                        additive = false
                    });
            }
            else if (mod.operation == AttributeModifierOperation.AddAbilityScoreBonus &&
                     mod.SourceAbility == AttributeDefinitions.Dexterity)
            {
                ac.ValueTrends.Add(
                    new TrendInfo((int)mod.Value, FeatureSourceType.AbilityScore, mod.SourceAbility, monster, mod)
                    {
                        additive = true
                    });
            }
        }
    }

    internal static class ArmorClassStacking
    {
        internal static void ProcessWildShapeAc(List<RulesetAttributeModifier> modifiers,
            RulesetCharacterMonster monster)
        {
            //process only for wild-shaped heroes
            if (monster.OriginalFormCharacter is RulesetCharacterHero)
            {
                var ac = monster.GetAttribute(AttributeDefinitions.ArmorClass);

                RefreshWildShapeAcFeatures(monster, ac);
                UpdateWildShapeAcTrends(modifiers, monster, ac);
            }

            //sort modifiers, since we replaced this call
            RulesetAttributeModifier.SortAttributeModifiersList(modifiers);
        }
    }
}
