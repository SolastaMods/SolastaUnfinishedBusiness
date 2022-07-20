using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public sealed class CustomMissileDeflection : ICustomMissileDeflection
{
    private const RuleDefinitions.AdvantageType Advantage = RuleDefinitions.AdvantageType.None;
    private const string Attribute = AttributeDefinitions.Dexterity;
    private const int DieNumber = 1;
    private const RuleDefinitions.DieType DieType = RuleDefinitions.DieType.D10;

    public string CharacterClass = null;

    //private const int ProficiencyBonusMult = 0;
    public int ClassLevelMult = 0;
    public string DescriptionTag = null;

    public int GetDamageReduction([NotNull] RulesetCharacter target, RulesetCharacter attacker)
    {
        var reduction = 0;

        for (var i = 0; i < DieNumber; i++)
        {
            reduction += RuleDefinitions.RollDie(DieType, Advantage, out _, out _);
        }

        if (!string.IsNullOrEmpty(Attribute))
        {
            var attr = target.GetAttribute(Attribute, true);
            if (attr != null)
            {
                reduction += AttributeDefinitions.ComputeAbilityScoreModifier(attr.CurrentValue);
            }
        }

        //     var characterLevel = target.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

        // if (characterLevelMult != 0)
        // {
        //     reduction += characterLevel * characterLevelMult;
        // }

        if (!string.IsNullOrEmpty(CharacterClass) && ClassLevelMult != 0 && target is RulesetCharacterHero hero)
        {
            var classLevel = hero.ClassesAndLevels.FirstOrDefault(e => e.Key.Name == CharacterClass).Value;
            reduction += classLevel * ClassLevelMult;
        }

        // if (ProficiencyBonusMult != 0)
        // {
        //     reduction += AttributeDefinitions.ComputeProficiencyBonus(characterLevel) * ProficiencyBonusMult;
        // }

        return reduction;
    }

    public string FormatDescription(RulesetCharacter target, RulesetCharacter attacker, string def)
    {
        if (string.IsNullOrEmpty(DescriptionTag))
        {
            return def;
        }

        var guiDefender = new GuiCharacter(target);
        var guiAttacker = new GuiCharacter(attacker);
        var format = Gui.Localize($"Reaction/&CustomDeflectMissile{DescriptionTag}Title");

        return string.Format(format, guiAttacker.Name, guiDefender.Name);
    }
}
