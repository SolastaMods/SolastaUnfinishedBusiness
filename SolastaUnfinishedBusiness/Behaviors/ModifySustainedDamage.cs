using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

public delegate void ModifySustainedDamageHandler(RulesetCharacter character, ref int damage, string damageType,
    bool criticalSuccess, ulong sourceGuid, RollInfo rollInfo);

internal static class ModifySustainedDamage
{
    internal static void ModifyDamage(RulesetCharacter character, ref int damage, string damageType,
        bool criticalSuccess, ulong sourceGuid, RollInfo rollInfo)
    {
        var modifiers = character.GetSubFeaturesByType<ModifySustainedDamageHandler>();

        foreach (var modifier in modifiers)
        {
            modifier(character, ref damage, damageType, criticalSuccess, sourceGuid, rollInfo);
        }
    }
}
