using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Feats;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionStandUpPatcher
{
    [HarmonyPatch(typeof(CharacterActionStandUp), nameof(CharacterActionStandUp.StandUp))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StandUp_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GameLocationCharacter character, ref bool applyCost)
        {
            if (character.RulesetCharacter.GetOriginalHero() is { } hero &&
                (hero.TrainedFeats.Contains(OtherFeats.FeatAthleteStr) ||
                 hero.TrainedFeats.Contains(OtherFeats.FeatAthleteDex)))
            {
                applyCost = false;
            }
        }
    }
}
