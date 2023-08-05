using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Used in FeatureDefinitionAdditionalDamage ExtraAdditionalDamageValueDetermination
namespace SolastaUnfinishedBusiness.CustomBehaviors;

// return AttributeDefinitions.[SomeAbilityScoreName]
internal delegate string AbilityScoreNameProvider();
