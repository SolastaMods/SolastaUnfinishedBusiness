using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
    internal static class BattleContext
    {
        internal class HandleCharacterAttackDamageContext
        {
            public GameLocationCharacter attacker;
            public GameLocationCharacter defender;
            public ActionModifier attackModifier;
            public RulesetAttackMode attackMode;
            public bool rangedAttack;
            public RuleDefinitions.AdvantageType advantageType;
            public List<EffectForm> actualEffectForms;
            public RulesetEffect rulesetEffect;
            public bool criticalHit;
            public bool firstTarget;
        }

        internal static HandleCharacterAttackDamageContext HandleCharacterAttackDamage { get; private set; } = new();

        internal static void SetHandleCharacterAttackDamageContext(
            bool isPrefix = false,
            GameLocationCharacter attacker = null,
            GameLocationCharacter defender = null,
            ActionModifier attackModifier = null,
            RulesetAttackMode attackMode = null,
            bool rangedAttack = false,
            RuleDefinitions.AdvantageType advantageType = RuleDefinitions.AdvantageType.None,
            List<EffectForm> actualEffectForms = null,
            RulesetEffect rulesetEffect = null,
            bool criticalHit = false,
            bool firstTarget = false)
        {
            var stage = isPrefix ? "in" : "out";
            var attackerName = attacker != null ? attacker.RulesetActor != null ? attacker.RulesetActor.Name : "NONE" : "NONE";
            var defenderName = defender != null ? defender.RulesetActor != null ? defender.RulesetActor.Name : "NONE" : "NONE";

            Main.Warning($"{stage} SetHandleCharacterAttackDamageContext => attacker:{attackerName}, defender:{defenderName}");

            HandleCharacterAttackDamage.attacker = attacker;
            HandleCharacterAttackDamage.defender = defender;
            HandleCharacterAttackDamage.attackModifier = attackModifier;
            HandleCharacterAttackDamage.attackMode = attackMode;
            HandleCharacterAttackDamage.rangedAttack = rangedAttack;
            HandleCharacterAttackDamage.advantageType = advantageType;
            HandleCharacterAttackDamage.actualEffectForms = actualEffectForms;
            HandleCharacterAttackDamage.rulesetEffect = rulesetEffect;
            HandleCharacterAttackDamage.criticalHit = criticalHit;
            HandleCharacterAttackDamage.firstTarget = firstTarget;
        }
    }
}
