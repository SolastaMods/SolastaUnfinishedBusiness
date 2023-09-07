using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ITryAlterOutcomeSavingThrow
{
    [UsedImplicitly]
    public void OnFailedSavingTryAlterOutcome(
        RulesetCharacter caster,
        RuleDefinitions.Side sourceSide,
        RulesetActor target,
        ActionModifier actionModifier,
        bool hasHitVisual,
        bool hasSavingThrow,
        string savingThrowAbility,
        int saveDC,
        bool disableSavingThrowOnAllies,
        bool advantageForEnemies,
        bool ignoreCover,
        RuleDefinitions.FeatureSourceType featureSourceType,
        List<EffectForm> effectForms,
        List<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense,
        List<SaveAffinityByFamilyDescription> savingThrowAffinitiesByFamily,
        string sourceName,
        BaseDefinition sourceDefinition,
        string schoolOfMagic,
        MetamagicOptionDefinition metamagicOption,
        ref RuleDefinitions.RollOutcome saveOutcome,
        ref int saveOutcomeDelta);
}
