using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// Free modification to AC
public interface IModifyAC
{
    void ModifyAC(RulesetCharacter owner,
        [UsedImplicitly] bool callRefresh,
        [UsedImplicitly] bool dryRun,
        [UsedImplicitly] FeatureDefinition dryRunFeature,
        RulesetAttribute armorClass);
}
