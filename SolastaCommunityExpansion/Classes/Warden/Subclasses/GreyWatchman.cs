using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.Classes.Warden.Subclasses
{
    internal class GreyWatchman
    {
        public static readonly Guid WARDEN_GW_BASE_GUID = new("0503f780-2d85-4926-8114-8e07f79090e7");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WardenClass { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerBattleTactics { get; private set; }
        public static FeatureDefinitionPowerSharedPool FeatureDefinitionPowerSharedPoolManeuverBullRush { get; private set; }
        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition wardenClass)
        {
            return Subclass ??= BuildAndAddSubclass(wardenClass);
        }

        private static void BuildBattleTacticsPool()
        {
            
            var builder = new FeatureDefinitionPowerPoolBuilder(
                "BattleTacticsPool",
                GuidHelper.Create(WARDEN_GW_BASE_GUID, "BattleTacticsPool").ToString(),
                2,
                RuleDefinitions.UsesDetermination.Fixed, 
                AttributeDefinitions.Strength, 
                RuleDefinitions.RechargeRate.ShortRest,
                new GuiPresentationBuilder(
                    "Feature/&BattleTacticsPoolDescription",
                    "Feature/&BattleTacticsPoolTitle").Build());

            FeatureDefinitionPowerBattleTactics = builder.AddToDB();
        }

        private static void BuildBattleTacticsManeuvers()
        {
            EffectForm damageEffect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D8,
                    BonusDamage = 2,
                    DamageType = "DamageBludgeoning"
                },
                SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
            };

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);

            var bullRush = new FeatureDefinitionPowerSharedPoolBuilder(
                "BattleTacticsManeuverBullRush",
                GuidHelper.Create(WARDEN_GW_BASE_GUID, "BattleTacticsManeuverBullRush").ToString(),
                FeatureDefinitionPowerBattleTactics,
                RuleDefinitions.RechargeRate.ShortRest,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Strength,
                newEffectDescription,
                new GuiPresentationBuilder(
                    "Feature/&BattleTacticsManeuverBullRushDescription",
                    "Feature/&BattleTacticsManeuverBullRushTitle").Build(),
                false);

            FeatureDefinitionPowerSharedPoolManeuverBullRush = bullRush.AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionPowerBattleTactics, 1);
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionPowerSharedPoolManeuverBullRush, 1);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition wardenClassDefinition)
        {
            WardenClass = wardenClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&GreyWatchmanDescription",
                    "Subclass/&GreyWatchmanTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "GreyWatchman",
                    GuidHelper.Create(WARDEN_GW_BASE_GUID, "GreyWatchman").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildBattleTacticsPool();
            BuildBattleTacticsManeuvers();

            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
