#if false
using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerLightBearer : AbstractSubclass
{
    private const string Name = "RangerLightBearer";

    internal RangerLightBearer()
    {
        // LEVEL 03

        var autoPreparedSpellsArcanist = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Bless),
                BuildSpellGroup(5, PassWithoutTrace),
                BuildSpellGroup(9, SpellsContext.BlindingSmite),
                BuildSpellGroup(13, SpellsContext.StaggeringSmite),
                BuildSpellGroup(17, SpellsContext.BanishingSmite))
            .SetCustomSubFeatures(new ModifyAttackModeForWeaponBlessedWarrior())
            .AddToDB();

        // Blessed Warrior

        var powerBlessedWarrior = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlessedWarrior")
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{Name}BlessedWarrior")
                                    .SetGuiPresentation(Category.Condition)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerLightBearer, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsArcanist)
            .AddFeaturesAtLevel(7)
            .AddFeaturesAtLevel(11)
            .AddFeaturesAtLevel(15)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAttackModeForWeaponBlessedWarrior : IOnComputeAttackModifier
    {
        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null)
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;

            foreach (var damageForm in effectDescription.EffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                damageForm.DamageForm.damageType = DamageTypeRadiant;
            }

            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            if (k < 0)
            {
                return;
            }

            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);
        }
    }
}
#endif
