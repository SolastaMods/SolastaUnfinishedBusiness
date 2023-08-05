using System;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// Get through Immunity!
internal interface IRemoveSpellOrSpellLevelImmunity
{
    bool IsValid(
        RulesetCharacter character,
        RulesetCondition holdingCondition);
    bool ShouldRemoveImmunity(Func<SpellDefinition, bool> isImmuneToSpell);
    bool ShouldRemoveImmunityLevel(Func<int, int, bool> isImmuneToSpellLevel);
}
