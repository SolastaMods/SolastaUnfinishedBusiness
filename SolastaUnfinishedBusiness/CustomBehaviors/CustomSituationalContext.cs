using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomValidators;
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
            ExtraSituationalContext.IsRagingAndDualWielding =>
                contextParams.source.HasAnyConditionOfType(ConditionRaging) &&
                ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(contextParams.source),

            ExtraSituationalContext.IsNotInBrightLight => ValidatorsCharacter.IsNotInBrightLight(contextParams.source),

            ExtraSituationalContext.TargetIsNotInBrightLight => ValidatorsCharacter.IsNotInBrightLight(
                contextParams.target),

            ExtraSituationalContext.HasSpecializedWeaponInHands => HasSpecializedWeaponInHands(contextParams.source),

            ExtraSituationalContext.HasLongswordInHands =>
                ValidatorsCharacter.HasWeaponType(DatabaseHelper.WeaponTypeDefinitions.LongswordType)
                    (contextParams.source),

            ExtraSituationalContext.HasGreatswordInHands =>
                ValidatorsCharacter.HasWeaponType(DatabaseHelper.WeaponTypeDefinitions.GreatswordType)
                    (contextParams.source),

            ExtraSituationalContext.HasTwoHandedVersatileWeapon => ValidatorsCharacter
                .HasTwoHandedVersatileWeapon(contextParams.source),

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

            ExtraSituationalContext.NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget =>
                NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget(contextParams),

            ExtraSituationalContext.MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow =>
                MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow(contextParams),

            _ => def
        };
    }

    private static bool MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow(
        RulesetImplementationDefinitions.SituationalContextParams contextParams)
    {
        var source = GameLocationCharacter.GetFromActor(contextParams.source);
        var target = GameLocationCharacter.GetFromActor(contextParams.target);

        if (source == null || target == null)
        {
            return false;
        }

        var mainWeaponIsMeleeOrUnarmed =
            ValidatorsCharacter.HasMeleeWeaponInMainHand(contextParams.source) ||
            ValidatorsCharacter.IsUnarmedInMainHand(contextParams.source);
        var levels = source.RulesetCharacter.GetSubclassLevel(
            DatabaseHelper.CharacterClassDefinitions.Barbarian, PathOfTheYeoman.Name);

        return mainWeaponIsMeleeOrUnarmed || (levels >= 6 && ValidatorsCharacter.HasLongbow(source.RulesetCharacter));
    }

    private static bool NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget(
        RulesetImplementationDefinitions.SituationalContextParams contextParams)
    {
        if (!contextParams.source.IsWearingShield())
        {
            return false;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(contextParams.source);

        if (gameLocationCharacter == null)
        {
            return false;
        }

        var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

        return HasVisibleCharactersOfSideNextToCharacter(gameLocationCharacter) ||
               gameLocationPositioningService.IsNextToWall(gameLocationCharacter.LocationPosition);
    }

    private static bool HasVisibleCharactersOfSideNextToCharacter(GameLocationCharacter character)
    {
        var battleSizeParameters = character.BattleSizeParameters;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;

        minExtent.x -= 1;
        minExtent.y -= 1;
        minExtent.z -= 1;
        maxExtent.x += 1;
        maxExtent.y += 1;
        maxExtent.z += 1;

        var boxInt = new BoxInt(minExtent + character.LocationPosition, maxExtent + character.LocationPosition);
        var gridAccessor = GridAccessor.Default;

        foreach (var position in boxInt.EnumerateAllPositionsWithin())
        {
            if (!gridAccessor.Occupants_TryGet(position, out var locationCharacterList))
            {
                continue;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var locationCharacter in locationCharacterList)
            {
                if (!locationCharacter.CanAct() ||
                    locationCharacter == character ||
                    locationCharacter.Side != Side.Ally)
                {
                    continue;
                }

                var deltaX = Math.Abs(locationCharacter.LocationPosition.x - character.LocationPosition.x);
                var deltaY = Math.Abs(locationCharacter.LocationPosition.y - character.LocationPosition.y);
                var deltaZ = Math.Abs(locationCharacter.LocationPosition.z - character.LocationPosition.z);
                var deltas = deltaX + deltaY + deltaZ;

                return deltas == 1;
            }
        }

        return false;
    }

    private static bool IsConsciousSummonerNextToBeast(GameLocationCharacter beast)
    {
        var battleSizeParameters = beast.BattleSizeParameters;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;

        minExtent.x -= 1;
        minExtent.y -= 1;
        minExtent.z -= 1;
        maxExtent.x += 1;
        maxExtent.y += 1;
        maxExtent.z += 1;

        var boxInt = new BoxInt(minExtent + beast.LocationPosition, maxExtent + beast.LocationPosition);
        var gridAccessor = GridAccessor.Default;
        var summoner = beast.RulesetCharacter.GetMySummoner();

        foreach (var position in boxInt.EnumerateAllPositionsWithin())
        {
            if (!gridAccessor.Occupants_TryGet(position, out var locationCharacters))
            {
                continue;
            }

            if (locationCharacters.Any(locationCharacter =>
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
