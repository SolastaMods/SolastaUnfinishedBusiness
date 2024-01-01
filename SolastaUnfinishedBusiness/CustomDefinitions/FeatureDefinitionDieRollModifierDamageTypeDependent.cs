using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionDieRollModifierDamageTypeDependent : FeatureDefinitionDieRollModifier
{
    public List<string> damageTypes = [];
}
