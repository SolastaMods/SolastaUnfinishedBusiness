using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Subclasses;
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

        // supports Martial Weapon Master use case
        static bool HasSpecializedWeaponInHands(RulesetCharacter rulesetCharacter)
        {
            var specializedWeapons = MartialWeaponMaster.GetSpecializedWeaponTypes(rulesetCharacter);

            return specializedWeapons.Any(x => ValidatorsCharacter.HasWeaponType(x)(rulesetCharacter));
        }

        return (ExtraSituationalContext)context switch
        {
            ExtraSituationalContext.HasSpecializedWeaponInHands => HasSpecializedWeaponInHands(contextParams.source),

            ExtraSituationalContext.HasLongswordInHands =>
                ValidatorsCharacter.HasWeaponType(DatabaseHelper.WeaponTypeDefinitions.LongswordType)
                    (contextParams.source),
            
            ExtraSituationalContext.HasGreatswordInHands =>
                ValidatorsCharacter.HasWeaponType(DatabaseHelper.WeaponTypeDefinitions.GreatswordType)
                (contextParams.source),

            ExtraSituationalContext.MainWeaponIsMeleeOrUnarmed =>
                ValidatorsCharacter.HasMeleeWeaponInMainHand(contextParams.source) ||
                ValidatorsCharacter.IsUnarmedInMainHand(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                (ValidatorsCharacter.HasNoArmor(contextParams.source) ||
                 ValidatorsCharacter.HasLightArmor(contextParams.source)) &&
                ValidatorsCharacter.HasNoShield(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff =>
                (ValidatorsCharacter.HasNoArmor(contextParams.source) ||
                 ValidatorsCharacter.HasLightArmor(contextParams.source)) &&
                ValidatorsCharacter.HasTwoHandedQuarterstaff(contextParams.source),

            ExtraSituationalContext.TargetIsNotEffectSource =>
                contextParams.target != effectSource,

            ExtraSituationalContext.SummonerIsNextToBeast =>
                IsConsciousSummonerNextToBeast(GameLocationCharacter.GetFromActor(contextParams.source)),

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
