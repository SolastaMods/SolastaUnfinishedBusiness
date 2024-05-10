using System;
using System.Collections.Generic;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class RulesetCharacterMonsterExtensions
{
    public static RulesetAttackMode RefreshAttackMode(
        this RulesetCharacterMonster monster,
        ActionDefinitions.ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool canAddAbilityDamageBonus,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
    {
        var hero = monster.GetOriginalHero();
        var slotName = EquipmentDefinitions.SlotTypeMainHand;
        var attackMode = RulesetAttackMode.AttackModesPool.Get();

        attackMode.Clear();
        attackMode.FreeOffHand = true;
        attackMode.ActionType = actionType;
        attackMode.SourceDefinition = itemDefinition;
        attackMode.SlotName = slotName;
        attackMode.SourceObject = null;

        if (actionType == ActionDefinitions.ActionType.Main)
        {
            attackMode.AttacksNumber = monster.TryGetAttributeValue(AttributeDefinitions.AttacksNumber);
        }

        var weaponType = DatabaseRepository.GetDatabase<WeaponTypeDefinition>()
            .GetElement(weaponDescription.WeaponType);

        attackMode.AbilityScore = weaponType.WeaponProximity == AttackProximity.Melee
            ? AttributeDefinitions.Strength
            : AttributeDefinitions.Dexterity;

        var dexterity = monster.TryGetAttributeValue(AttributeDefinitions.Dexterity);
        var strength = monster.TryGetAttributeValue(AttributeDefinitions.Strength);

        if (weaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagFinesse))
        {
            if (dexterity > strength)
            {
                attackMode.AbilityScore = AttributeDefinitions.Dexterity;
            }
            else if (strength > dexterity)
            {
                attackMode.AbilityScore = AttributeDefinitions.Strength;
            }
        }

        attackMode.Ranged = weaponType.WeaponProximity == AttackProximity.Range;
        attackMode.Thrown = weaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagThrown);
        attackMode.Reach = weaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagReach);
        attackMode.ReachRange = attackMode.Reach ? weaponDescription.ReachRange : 1;

        if (attackMode.Ranged || attackMode.Thrown)
        {
            attackMode.CloseRange = weaponDescription.CloseRange;
            attackMode.MaxRange = weaponDescription.MaxRange;
        }

        if (monster.IsProficientWithItem(itemDefinition) || (hero != null && hero.IsProficientWithItem(itemDefinition)))
        {
            var currentValue = monster.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            attackMode.ToHitBonus += currentValue;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(currentValue,
                FeatureSourceType.Proficiency, string.Empty, null));
        }

        attackMode.EffectDescription.Copy(weaponDescription.EffectDescription);
        attackMode.UseVersatileDamage =
            weaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagVersatile) & true;

        foreach (var itemTag in itemDefinition.ItemTags)
        {
            attackMode.AddAttackTagAsNeeded(itemTag);
        }

        var service = ServiceRepository.GetService<IRulesetImplementationService>();

        foreach (var attackModifier in attackModifiers)
        {
            if (attackModifier == null)
            {
                Trace.LogException(new Exception("[Tactical - Invisible for players] attackModifier is null"));
            }
            else if (service.IsValidContextForRestrictedContextProvider(
                         attackModifier, monster, itemDefinition, attackMode.Ranged, attackMode, null))
            {
                if (attackModifier.MagicalWeapon)
                {
                    attackMode.AddAttackTagAsNeeded(TagsDefinitions.MagicalWeapon);
                }

                var attackRollModifier = attackModifier.AttackRollModifier;

                attackMode.ToHitBonus += attackRollModifier;

                var key = attackModifier as FeatureDefinition;

                if (key && featuresOrigin.TryGetValue(key, out var value))
                {
                    attackMode.ToHitBonusTrends.Add(new TrendInfo(attackRollModifier, value.sourceType,
                        featuresOrigin[key].sourceName, featuresOrigin[key].source));
                }

                if (attackModifier.AbilityScoreReplacement == AbilityScoreReplacement.DexterityIfBetterThanStrength)
                {
                    if (dexterity >= strength)
                    {
                        attackMode.AbilityScore = AttributeDefinitions.Dexterity;
                    }
                    else if (strength > dexterity)
                    {
                        attackMode.AbilityScore = AttributeDefinitions.Strength;
                    }
                }

                if (attackModifier.DamageDieReplacement != DamageDieReplacement.None)
                {
                    var firstDamageForm = attackMode.EffectDescription.FindFirstDamageForm();

                    if (firstDamageForm != null)
                    {
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (attackModifier.DamageDieReplacement)
                        {
                            case DamageDieReplacement.FirstDamageForm:
                                firstDamageForm.DieType = attackModifier.ReplacedDieType;
                                if (firstDamageForm.VersatileDieType < attackModifier.ReplacedDieType)
                                {
                                    firstDamageForm.VersatileDieType = attackModifier.ReplacedDieType;
                                }

                                break;
                            case DamageDieReplacement.DieTypeByRankIfBetterThanNatural:
                                hero.UpgradeAttackModeDieTypeWithAttackModifierByCharacterLevel(attackMode,
                                    attackModifier);
                                break;
                        }
                    }
                }

                if (attackModifier.AdditionalEffectForms == null)
                {
                    continue;
                }

                foreach (var additionalEffectForm in attackModifier.AdditionalEffectForms)
                {
                    attackMode.EffectDescription.EffectForms.Add(EffectForm.GetCopy(additionalEffectForm));
                }
            }
        }

        var abilityScoreModifier =
            AttributeDefinitions.ComputeAbilityScoreModifier(monster.TryGetAttributeValue(attackMode.AbilityScore));

        attackMode.ToHitBonus += abilityScoreModifier;
        attackMode.ToHitBonusTrends.Add(new TrendInfo(abilityScoreModifier,
            FeatureSourceType.AbilityScore, attackMode.AbilityScore, null));

        var firstDamageForm1 = attackMode.EffectDescription.FindFirstDamageForm();

        if (firstDamageForm1 == null)
        {
            return attackMode;
        }

        firstDamageForm1.DamageBonusTrends.Clear();

        if (canAddAbilityDamageBonus)
        {
            firstDamageForm1.BonusDamage += abilityScoreModifier;
            firstDamageForm1.DamageBonusTrends.Add(new TrendInfo(abilityScoreModifier,
                FeatureSourceType.AbilityScore, attackMode.AbilityScore, null));
        }

        foreach (var attackModifier in attackModifiers)
        {
            if (attackModifier == null)
            {
                Trace.LogException(new Exception("[Tactical - Invisible for players] attackModifier is null"));
            }
            else if (service.IsValidContextForRestrictedContextProvider(
                         attackModifier, monster, itemDefinition, attackMode.Ranged, attackMode, null)
                     && attackModifier.DamageRollModifierMethod != AttackModifierMethod.None)
            {
                var num = attackModifier.DamageRollModifier;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (attackModifier.DamageRollModifierMethod)
                {
                    case AttackModifierMethod.SourceConditionAmount:
                        num = monster.FindFirstConditionHoldingFeature(attackModifier as FeatureDefinition).Amount;
                        break;
                    case AttackModifierMethod.AddAbilityScoreBonus
                        when !string.IsNullOrEmpty(attackModifier.DamageRollAbilityScore):
                        num += AttributeDefinitions.ComputeAbilityScoreModifier(
                            monster.TryGetAttributeValue(attackModifier.DamageRollAbilityScore));
                        break;
                }

                firstDamageForm1.BonusDamage += num;

                var key = attackModifier as FeatureDefinition;

                if (key && featuresOrigin.TryGetValue(key, out var value))
                {
                    firstDamageForm1.DamageBonusTrends.Add(new TrendInfo(num, value.sourceType,
                        featuresOrigin[key].sourceName, featuresOrigin[key].source));
                }
            }
        }

        return attackMode;
    }
}
