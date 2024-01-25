using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// Get through Immunity!
internal interface IRemoveSpellOrSpellLevelImmunity
{
    bool IsValid(
        [UsedImplicitly] RulesetCharacter character,
        [UsedImplicitly] RulesetCondition holdingCondition);

    bool ShouldRemoveImmunity(Func<SpellDefinition, bool> isImmuneToSpell);
    bool ShouldRemoveImmunityLevel(Func<int, int, bool> isImmuneToSpellLevel);
}
