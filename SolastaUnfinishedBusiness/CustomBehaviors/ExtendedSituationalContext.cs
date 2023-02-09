using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class CustomSituationalContext
{
    internal static bool IsContextValid(
        RulesetImplementationDefinitions.SituationalContextParams contextParams,
        bool def)
    {
        var context = contextParams.situationalContext;
        RulesetEntity effectSource = null;

        if (contextParams.sourceEffectId != 0)
        {
            RulesetEntity.TryGetEntity(contextParams.sourceEffectId, out effectSource);
        }

        return (ExtraSituationalContext)context switch
        {
            ExtraSituationalContext.MainWeaponIsMeleeOrUnarmed =>
                ValidatorsCharacter.MainHandIsMeleeWeapon(contextParams.source) ||
                ValidatorsCharacter.MainHandIsUnarmed(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                ValidatorsCharacter.HasNoArmor(contextParams.source)
                || (ValidatorsCharacter.LightArmor(contextParams.source)
                    && ValidatorsCharacter.HasNoShield(contextParams.source)),

#if false
            ExtraSituationalContext.MainWeaponIsFinesseOrLightRange =>
                ValidatorsCharacter.MainHandIsFinesseWeapon(contextParams.source)
                || ValidatorsCharacter.HasLightRangeWeapon(contextParams.source),

            ExtraSituationalContext.MainWeaponIsVersatileWithoutShield =>
                ValidatorsCharacter.MainHandIsVersatileWeapon(contextParams.source)
                && ValidatorsCharacter.NoShield(contextParams.source),
#endif

            ExtraSituationalContext.TargetIsNotEffectSource =>
                contextParams.target != effectSource,

            ExtraSituationalContext.SummonerIsNextToBeast =>
                IsConsciousSummonerNextToBeast(GameLocationCharacter.GetFromActor(contextParams.source)),

            ExtraSituationalContext.MainWeaponIsGreatSword =>
                ValidatorsCharacter
                    .MainHandHasWeaponType(DatabaseHelper.WeaponTypeDefinitions.GreatswordType)(contextParams.source),

            _ => def
        };
    }

    private static bool IsConsciousSummonerNextToBeast(GameLocationCharacter beast)
    {
        const int NUM = 1;

        var battleSizeParameters = beast.BattleSizeParameters;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;

        minExtent.x -= NUM;
        minExtent.y -= NUM;
        minExtent.z -= NUM;
        maxExtent.x += NUM;
        maxExtent.y += NUM;
        maxExtent.z += NUM;

        var boxInt = new BoxInt(minExtent + beast.LocationPosition, maxExtent + beast.LocationPosition);
        var gridAccessor = GridAccessor.Default;
        var summoner = beast.RulesetCharacter.GetMySummoner();

        foreach (var position in boxInt.EnumerateAllPositionsWithin())
        {
            if (!gridAccessor.Occupants_TryGet(position, out var locationCharacters))
            {
                continue;
            }

            if (locationCharacters.Any(
                    locationCharacter =>
                        locationCharacter == summoner &&
                        !locationCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                        !locationCharacter.RulesetCharacter.HasConditionOfType(ConditionIncapacitated)))
            {
                return true;
            }
        }

        return false;
    }
}
