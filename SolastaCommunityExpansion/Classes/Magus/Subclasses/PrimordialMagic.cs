using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Classes.Magus.Subclasses;

public static class PrimordialMagic
{
    internal sealed class FeatureDefinitionCastSpellChange : FeatureDefinition, IFeatureDefinitionCustomCode
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var spellRepertoire in hero.spellRepertoires.Where(spellRepertoire => spellRepertoire.spellCastingClass == Magus.ClassMagus))
            {
                spellRepertoire.spellCastingFeature = PrimordialMagicCastSpell;
                hero.RefreshSpellRepertoires();
                break;
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var spellRepertoire in hero.spellRepertoires.Where(spellRepertoire => spellRepertoire.spellCastingClass == Magus.ClassMagus))
            {
                spellRepertoire.spellCastingFeature = Magus.FeatureDefinitionClassMagusCastSpell;
                hero.RefreshSpellRepertoires();
                break;
            }
        }
    }
    
    internal sealed class FeatureDefinitionCastSpellChangeBuilder : FeatureDefinitionBuilder<
        FeatureDefinitionCastSpellChange, FeatureDefinitionCastSpellChangeBuilder>
    {
        private FeatureDefinitionCastSpellChangeBuilder(string name, string guid) : base(name, guid)
        {
        }

        public static FeatureDefinitionCastSpellChange CreateAndAddToDB(string name, string guid)
        {
            return new FeatureDefinitionCastSpellChangeBuilder(name, guid)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }
    }
    
    private static readonly FeatureDefinitionCastSpellChange primordialMagic = FeatureDefinitionCastSpellChangeBuilder
        .CreateAndAddToDB("ClassMagusPrimordialMagicCastSpellChange", DefinitionBuilder.CENamespaceGuid.ToString());
    
    // using strength as casting ability
    private static FeatureDefinitionCastSpell PrimordialMagicCastSpell = FeatureDefinitionCastSpellBuilder
            .Create("ClassMagusPrimordialMagicCastSpell", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusPrimordialMagicCastSpell", Category.Class)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
                .SetSpellCastingAbility(AttributeDefinitions.Strength)
                .SetSpellList(MagusSpells.MagusSpellList)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared)
                .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusLevel)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
                .SetSpellCastingLevel(1)
                .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .SetSlotsPerLevel(1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .AddToDB();
    internal static CharacterSubclassDefinition Build()
    {
        // grant bonus attack after casting a spell
        // the attack is similar to the frenzy barb
        var conditionSpellStrikeBonusAttack = ConditionDefinitionBuilder
            .Create("ClassMagusPrimordialMagicConditionSpellStrikeBonusAttack", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass ,"ClassMagusPrimordialMagicConditionSpellStrikeBonusAttack", DatabaseHelper.ConditionDefinitions.ConditionBerserkerFrenzy.guiPresentation.SpriteReference)
            .AddFeatures(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
            .AddToDB();
        
        var spellStrikeBonusAttackEffect = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Self, 0, RuleDefinitions.TargetType.Self)
            .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionSpellStrikeBonusAttack, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();
        spellStrikeBonusAttackEffect.canBePlacedOnCharacter = true;
        spellStrikeBonusAttackEffect.targetExcludeCaster = false;

        var spellStrikeBonusAttack = FeatureDefinitionPowerBuilder
            .Create("ClassMagusPrimordialMagicSpellStrikeBonusAttack", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusPrimordialMagicSpellStrikeBonusAttack")
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetCustomSubFeatures(new PowerUseValidity(CharacterValidators.UseSpellStrike))
            .SetEffectDescription(spellStrikeBonusAttackEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.OnSpellCast)
            .AddToDB();
        
        return CharacterSubclassDefinitionBuilder
            .Create("MagusSubclassPrimordialMagic", DefinitionBuilder.CENamespaceGuid)
            .SetOrUpdateGuiPresentation(Category.Subclass,
                DatabaseHelper.CharacterSubclassDefinitions.OathOfTheMotherland.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, primordialMagic)
            .AddFeaturesAtLevel(7, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierMartialChampionImprovedCritical)
            .AddFeaturesAtLevel(11, spellStrikeBonusAttack)
            .AddToDB(); 
    }
}
