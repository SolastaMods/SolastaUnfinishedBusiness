using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.DamageDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Classes.Warden.Subclasses
{
    internal static class GreyWatchman
    {
        private static readonly Guid Namespace = new("0503f780-2d85-4926-8114-8e07f79090e7");

        private static CharacterSubclassDefinition Subclass;
        public static FeatureDefinitionPower FeatureDefinitionPowerBattleTactics { get; private set; }
        public static FeatureDefinitionPowerSharedPool FeatureDefinitionPowerSharedPoolManeuverBullRush { get; private set; }

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition wardenClass)
        {
            return Subclass ??= BuildAndAddSubclass(wardenClass);
        }

        //
        // @BAZOU: please review below change
        // 
        private static void BuildBattleTacticsPool()
        {
            FeatureDefinitionPowerBattleTactics = FeatureDefinitionPowerPoolBuilder
                .Create("BattleTacticsPool", GuidHelper.Create(Namespace, "BattleTacticsPool").ToString())
                .SetGuiPresentation(new GuiPresentationBuilder(
                    "Feature/&BattleTacticsPoolDescription",
                    "Feature/&BattleTacticsPoolTitle").Build())
                .SetAbilityScore(AttributeDefinitions.Strength)
                .SetUsesFixed(2)
                .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
                .AddToDB();
        }

        private static void BuildBattleTacticsManeuvers()
        {
            var bullRushDamageForm = new DamageForm()
                .SetBonusDamage(2)
                .SetDamageType(DatabaseHelper.DamageDefinitions.DamageBludgeoning.ToString())
                .SetDiceNumber(1)
                .SetDieType(RuleDefinitions.DieType.D8);

            var bullRushEffectForm = new EffectForm()
                .SetFormType(EffectForm.EffectFormType.Damage)
                .SetCreatedByCharacter(true)
                .SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.None)
                .SetDamageForm(bullRushDamageForm);

            var bullRushEffectDescription = new EffectDescription();
            bullRushEffectDescription.Copy(PowerDomainLawHolyRetribution.EffectDescription);
            bullRushEffectDescription.EffectForms.Clear();
            bullRushEffectDescription.EffectForms.Add(bullRushEffectForm);

            FeatureDefinitionPowerSharedPoolManeuverBullRush = new FeatureDefinitionPowerSharedPoolBuilder(
                "BattleTacticsManeuverBullRush",
                GuidHelper.Create(Namespace, "BattleTacticsManeuverBullRush").ToString(),
                FeatureDefinitionPowerBattleTactics,
                RuleDefinitions.RechargeRate.ShortRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Strength,
                bullRushEffectDescription,
                new GuiPresentationBuilder(
                    "Feature/&BattleTacticsManeuverBullRushDescription",
                    "Feature/&BattleTacticsManeuverBullRushTitle").Build(),
                false)
                .AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder
                .AddFeatureAtLevel(FeatureDefinitionPowerBattleTactics, 1)
                .AddFeatureAtLevel(FeatureDefinitionPowerSharedPoolManeuverBullRush, 1);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition wardenClass)
        {
            var subclassBuilder = CharacterSubclassDefinitionBuilder
                .Create("GreyWatchman", Namespace)
                .SetGuiPresentation(Category.Subclass, RangerShadowTamer.GuiPresentation.SpriteReference);

            BuildBattleTacticsPool();
            BuildBattleTacticsManeuvers();

            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
