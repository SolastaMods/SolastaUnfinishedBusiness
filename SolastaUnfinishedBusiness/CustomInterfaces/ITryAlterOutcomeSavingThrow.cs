using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ITryAlterOutcomeSavingThrow
{
    [UsedImplicitly]
    public void OnSavingTryAlterOutcome(
        RulesetCharacter caster,
        Side sourceSide,
        RulesetActor target,
        ActionModifier actionModifier,
        bool hasHitVisual,
        bool hasSavingThrow,
        string savingThrowAbility,
        int saveDC,
        bool disableSavingThrowOnAllies,
        bool advantageForEnemies,
        bool ignoreCover,
        FeatureSourceType featureSourceType,
        List<EffectForm> effectForms,
        List<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense,
        List<SaveAffinityByFamilyDescription> savingThrowAffinitiesByFamily,
        string sourceName,
        BaseDefinition sourceDefinition,
        string schoolOfMagic,
        MetamagicOptionDefinition metamagicOption,
        ref RollOutcome saveOutcome,
        ref int saveOutcomeDelta);
}
