using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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

        return (ExtraSituationalContext)context switch
        {
            ExtraSituationalContext.MainWeaponIsMelee =>
                ValidatorsCharacter.MainHandIsMeleeWeapon(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                ValidatorsCharacter.NoArmor(contextParams.source)
                || (ValidatorsCharacter.LightArmor(contextParams.source)
                    && ValidatorsCharacter.NoShield(contextParams.source)),

            // ExtraSituationalContext.MainWeaponIsFinesseOrLightRange =>
            //     ValidatorsCharacter.MainHandIsFinesseWeapon(contextParams.source)
            //     || ValidatorsCharacter.HasLightRangeWeapon(contextParams.source),

            // ExtraSituationalContext.MainWeaponIsVersatileWithoutShield =>
            //     ValidatorsCharacter.MainHandIsVersatileWeapon(contextParams.source)
            //     && ValidatorsCharacter.NoShield(contextParams.source),

            ExtraSituationalContext.TargetIsNotEffectSource =>
                contextParams.target != effectSource,

            ExtraSituationalContext.SummonerIsNextToBeast =>
                IsConsciousSummonerNextToBeast(GameLocationCharacter.GetFromActor(contextParams.source)),

            ExtraSituationalContext.BeastIsNextToSummoner =>
                IsConsciousBeastNextToSummoner(GameLocationCharacter.GetFromActor(contextParams.source)),

            _ => def
        };
    }

    //TODO: make this generic as it currently only support Ranger Wild Master
    private static RulesetCharacter GetSpiritBeast(RulesetCharacter character)
    {
        var spiritBeastEffect = character.powersUsedByMe.Find(
            p => p.sourceDefinition.Name.StartsWith(RangerWildMaster.SummonSpiritBeastPower));
        var summons = EffectHelpers.GetSummonedCreatures(spiritBeastEffect);

        return summons.Empty() ? null : summons[0];
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

    private static bool IsConsciousBeastNextToSummoner(GameLocationCharacter summoner)
    {
        const int NUM = 1;

        var battleSizeParameters = summoner.BattleSizeParameters;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;

        minExtent.x -= NUM;
        minExtent.y -= NUM;
        minExtent.z -= NUM;
        maxExtent.x += NUM;
        maxExtent.y += NUM;
        maxExtent.z += NUM;

        var boxInt = new BoxInt(minExtent + summoner.LocationPosition, maxExtent + summoner.LocationPosition);
        var gridAccessor = GridAccessor.Default;
        var beast = GameLocationCharacter.GetFromActor(GetSpiritBeast(summoner.RulesetCharacter));

        foreach (var position in boxInt.EnumerateAllPositionsWithin())
        {
            if (!gridAccessor.Occupants_TryGet(position, out var locationCharacters))
            {
                continue;
            }

            if (locationCharacters.Any(
                    locationCharacter =>
                        locationCharacter == beast &&
                        !locationCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                        !locationCharacter.RulesetCharacter.HasConditionOfType(ConditionIncapacitated)))
            {
                return true;
            }
        }

        return false;
    }
}
