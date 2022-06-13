using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

// ensures reaction spells have the correct repertoire assigned for bonus calculation
[HarmonyPatch(typeof(CharacterActionCastSpell), "SpendMagicEffectUses")]
internal static class CharacterActionCastSpell_SpendMagicEffectUses
{
    internal static void Prefix(CharacterActionCastSpell __instance)
    {
        if (__instance.ActionId != ActionDefinitions.Id.CastReaction || __instance.activeSpell.spellRepertoire != null)
        {
            return;
        }

        var spell = __instance.activeSpell.spellDefinition;
        var bonus = 0;
        var selectedRepertoire = new RulesetSpellRepertoire();
        var repertoires = __instance.ActingCharacter.RulesetCharacter.SpellRepertoires
            .Where(x =>
                x.KnownSpells.Contains(spell)
                || x.KnownCantrips.Contains(spell)
                || x.PreparedSpells.Contains(spell));

        foreach (var repertoire in repertoires)
        {
            if (repertoire.spellAttackBonus < bonus)
            {
                continue;
            }

            bonus = repertoire.spellAttackBonus;
            selectedRepertoire = repertoire;
        }

        __instance.activeSpell.spellRepertoire = selectedRepertoire;
    }
}
