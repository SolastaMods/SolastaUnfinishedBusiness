using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMovementAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionMovementAffinity, FeatureDefinitionMovementAffinityBuilder>
{
#if false
    internal FeatureDefinitionMovementAffinityBuilder SetAdditiveModifierAdvancement(
        RuleDefinitions.MovementAffinityAdvancement value)
    {
        Definition.additiveModifierAdvancement = value;
        return this;
    }
#endif

    internal FeatureDefinitionMovementAffinityBuilder SetBaseSpeedAdditiveModifier(int value)
    {
        Definition.baseSpeedAdditiveModifier = value;
        return this;
    }

#if false
    internal FeatureDefinitionMovementAffinityBuilder SetSituationalContext(ExtraSituationalContext situationalContext)
    {
        Definition.situationalContext = (RuleDefinitions.SituationalContext)situationalContext;
        return this;
    }
#endif

    internal FeatureDefinitionMovementAffinityBuilder SetBaseSpeedMultiplicativeModifier(float value)
    {
        Definition.baseSpeedMultiplicativeModifier = value;
        return this;
    }

    internal FeatureDefinitionMovementAffinityBuilder SetImmunities(
        bool encumbranceImmunity = false,
        bool heavyArmorImmunity = false,
        bool difficultTerrainImmunity = false)
    {
        Definition.encumbranceImmunity = encumbranceImmunity;
        Definition.heavyArmorImmunity = heavyArmorImmunity;
        Definition.immuneDifficultTerrain = difficultTerrainImmunity;
        return this;
    }

    internal FeatureDefinitionMovementAffinityBuilder SetClimbing(
        bool fastClimber = false, bool expertClimber = false, bool canMoveOnWalls = false)
    {
        Definition.fastClimber = fastClimber;
        Definition.expertClimber = expertClimber;
        Definition.canMoveOnWalls = canMoveOnWalls;
        return this;
    }

    internal FeatureDefinitionMovementAffinityBuilder SetAdditionalJumpCells(int additionalJumpCells)
    {
        Definition.additionalJumpCells = additionalJumpCells;
        return this;
    }

    internal FeatureDefinitionMovementAffinityBuilder SetAdditionalFallThreshold(int additionalFallThreshold)
    {
        Definition.additionalFallThreshold = additionalFallThreshold;
        return this;
    }


    #region Constructors

    protected FeatureDefinitionMovementAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
