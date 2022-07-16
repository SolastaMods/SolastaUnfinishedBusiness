using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class DeadMaster : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("58ce31d8-6a37-4d6e-ffea-b9b60658a3ef");

    private static readonly Dictionary<int, List<MonsterDefinition>> CreateDeadSpellMonsters = new()
    {
        {3, new List<MonsterDefinition> {Ghast, Ghoul, Ogre_Zombie, Skeleton_Enforcer}},
        {5, new List<MonsterDefinition> {Mummy, Skeleton_Knight, Skeleton_Marksman, Skeleton_Sorcerer}},
        {7, new List<MonsterDefinition> {Brood_of_flesh, Brood_of_blood, Ghost, Wraith }}
    };

    private readonly CharacterSubclassDefinition Subclass;

    internal DeadMaster()
    {
        var autoPreparedSpellsWizardDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsWizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(autoPreparedSpellsWizardDeadMaster, 2)
            .AddToDB();
    }

    [NotNull]
    private static IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> GetDeadSpellAutoPreparedGroups()
    {
        var result = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>();

        foreach (var kvp in CreateDeadSpellMonsters)
        {
            var level = kvp.Key;
            var monsters = kvp.Value;
            var spells = new List<SpellDefinition>();

            foreach (var monster in monsters)
            {
                var subSpell = SpellDefinitionBuilder
                    .Create(ConjureFey, $"CreateDead{monster.name}", SubclassNamespace)
                    .SetGuiPresentation(Category.Spell)
                    .SetSchoolOfMagic(SchoolNecromancy)
                    .SetSpellLevel(level)
                    .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                    .SetSubSpells()
                    .AddToDB();

                monster.fullyControlledWhenAllied = true;
                subSpell.EffectDescription.EffectForms[0].SummonForm.monsterDefinitionName = monster.name;
                spells.Add(subSpell);
            }

            var spell = SpellDefinitionBuilder
                .Create(ConjureFey, $"CreateDead{level}", SubclassNamespace)
                .SetGuiPresentation(Category.Spell)
                .SetSchoolOfMagic(SchoolNecromancy)
                .SetSpellLevel(level)
                .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                .SetSubSpells(spells)
                .AddToDB();

            var autoPreparedSpellsGroup =
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
                {
                    ClassLevel = level, SpellsList = new List<SpellDefinition> {spell}
                };

            result.Add(autoPreparedSpellsGroup);
        }

        return result;
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
