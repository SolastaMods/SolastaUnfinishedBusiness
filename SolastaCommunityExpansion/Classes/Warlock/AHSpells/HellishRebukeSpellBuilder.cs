using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.AHSpells
{
    /*
    internal class HellishRebukeSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string HellishRebukeSpellName = "AHHellishRebukeSpell";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(new Guid(Settings.GUID), HellishRebukeSpellName).ToString();

        protected HellishRebukeSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.SacredFlame, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&AHHellishRebukeSpellTitle";
            Definition.GuiPresentation.Description = "Feat/&AHHellishRebukeSpellDescription";
            Definition.SetSpellLevel(1);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Reaction);

            //D10 damage
            var damageForm = new DamageForm
            {
                DiceNumber = 2,
                DamageType = "DamageFire",
                DieType = RuleDefinitions.DieType.D10
            };

            var damageEffectForm = new EffectForm
            {
                HasSavingThrow = true,
                FormType = EffectForm.EffectFormType.Damage,
                SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage,
                SaveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn,
                DamageForm = damageForm
            };


            //Additional die per spell level
            var advancement = new EffectAdvancement();
            advancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);
            advancement.SetAdditionalDicePerIncrement(1);

            var effectDescription = Definition.EffectDescription;
            effectDescription.SetRangeParameter(12);
            effectDescription.EffectForms.Clear();
            effectDescription.SetTargetParameter(1);
            damageEffectForm.HasSavingThrow = true;
            effectDescription.SavingThrowAbility = "Dexterity";
            effectDescription.SetEffectAdvancement(advancement);
            effectDescription.EffectForms.Add(damageEffectForm);

            Definition.SetEffectDescription(effectDescription);
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new HellishRebukeSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition HellishRebukeSpell = CreateAndAddToDB(HellishRebukeSpellName, HellishRebukeSpellNameGuid);
    }
*/
}
