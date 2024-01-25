using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

internal static class CustomSituationalContext
{
    private static readonly WeaponTypeDefinition[] SimpleOrMartialWeapons = DatabaseRepository
        .GetDatabase<WeaponTypeDefinition>()
        .Where(x => x.WeaponCategory is "MartialWeaponCategory" or "SimpleWeaponCategory")
        .ToArray();

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
            ExtraSituationalContext.IsRagingAndDualWielding =>
                contextParams.source.HasAnyConditionOfType(ConditionRaging) &&
                ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(contextParams.source),

            ExtraSituationalContext.IsNotInBrightLight =>
                ValidatorsCharacter.IsNotInBrightLight(contextParams.source),

            ExtraSituationalContext.HasSpecializedWeaponInHands =>
                MartialWeaponMaster.HasSpecializedWeapon(contextParams.source),

            ExtraSituationalContext.HasLongswordInHands =>
                ValidatorsCharacter.HasWeaponType(LongswordType)(contextParams.source),

            ExtraSituationalContext.HasGreatswordInHands =>
                ValidatorsCharacter.HasWeaponType(GreatswordType)(contextParams.source),

            ExtraSituationalContext.HasBladeMasteryWeaponTypesInHands =>
                ValidatorsCharacter.HasWeaponType(
                    ShortswordType, LongswordType, ScimitarType, RapierType, GreatswordType)(contextParams.source),

            ExtraSituationalContext.HasSimpleOrMartialWeaponInHands =>
                ValidatorsCharacter.HasWeaponType(SimpleOrMartialWeapons)(contextParams.source),

            ExtraSituationalContext.HasMeleeWeaponInMainHandWithFreeOffhand =>
                ValidatorsCharacter.HasFreeHandWithoutTwoHandedInMain(contextParams.source) &&
                ValidatorsCharacter.HasMeleeWeaponInMainHand(contextParams.source),

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

            ExtraSituationalContext.TargetIsFavoriteEnemy =>
                contextParams.source.IsMyFavoriteEnemy(contextParams.target),

            ExtraSituationalContext.SummonerIsNextToBeast =>
                IsConsciousSummonerNextToBeast(GameLocationCharacter.GetFromActor(contextParams.source)),

            ExtraSituationalContext.NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget =>
                NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget(contextParams),

            ExtraSituationalContext.MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow =>
                MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow(contextParams),

            ExtraSituationalContext.HasShieldInHands => contextParams.source.IsWearingShield(),

            ExtraSituationalContext.IsNotSourceOfCondition => IsNotSourceOfCondition(contextParams),
            // supports Monk Shield Expert scenarios
            (ExtraSituationalContext)SituationalContext.NotWearingArmorOrShield =>
                !contextParams.source.IsWearingArmor() &&
                (!contextParams.source.IsWearingShield() || contextParams.source.HasMonkShieldExpert()),

            // supports Monk Shield Expert scenarios
            (ExtraSituationalContext)SituationalContext.NotWearingArmorOrMageArmorOrShield =>
                !contextParams.source.IsWearingArmor() &&
                !contextParams.source.HasConditionOfTypeOrSubType(ConditionMagicallyArmored) &&
                (!contextParams.source.IsWearingShield() || contextParams.source.HasMonkShieldExpert()),

            _ => def
        };
    }

    private static bool IsNotSourceOfCondition(
        RulesetImplementationDefinitions.SituationalContextParams contextParams)
    {
        var target = contextParams.target;
        var condition = contextParams.condition;
        var rulesetCondition = target.AllConditions.FirstOrDefault(x => x.ConditionDefinition == condition);

        if (rulesetCondition == null)
        {
            return false;
        }

        return contextParams.source.Guid != rulesetCondition.SourceGuid;
    }

    private static bool MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow(
        RulesetImplementationDefinitions.SituationalContextParams contextParams)
    {
        var source = contextParams.source;
        var mainWeaponIsMeleeOrUnarmed =
            ValidatorsCharacter.HasMeleeWeaponInMainHand(source) ||
            ValidatorsCharacter.IsUnarmedInMainHand(source);
        var levels = source.GetSubclassLevel(
            DatabaseHelper.CharacterClassDefinitions.Barbarian, PathOfTheYeoman.Name);

        return mainWeaponIsMeleeOrUnarmed || (levels >= 6 && ValidatorsCharacter.HasLongbow(source));
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
        var gridAccessor = GridAccessor.Default;
        var battleSizeParameters = character.BattleSizeParameters;
        var characterLocation = character.LocationPosition;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;
        var boxInt = new BoxInt(
            minExtent - new int3(1, 1, 1) + characterLocation,
            maxExtent + new int3(1, 1, 1) + characterLocation);

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

    private static bool IsConsciousSummonerNextToBeast(GameLocationCharacter character)
    {
        var gridAccessor = GridAccessor.Default;
        var battleSizeParameters = character.BattleSizeParameters;
        var characterLocation = character.LocationPosition;
        var minExtent = battleSizeParameters.minExtent;
        var maxExtent = battleSizeParameters.maxExtent;
        var boxInt = new BoxInt(
            minExtent - new int3(1, 1, 1) + characterLocation,
            maxExtent + new int3(1, 1, 1) + characterLocation);

        var summoner = character.RulesetCharacter.GetMySummoner();

        foreach (var position in boxInt.EnumerateAllPositionsWithin())
        {
            if (!gridAccessor.Occupants_TryGet(position, out var locationCharacters))
            {
                continue;
            }

            if (locationCharacters.Any(x => x == summoner && x.CanAct()))
            {
                return true;
            }
        }

        return false;
    }
}
