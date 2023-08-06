#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SolastaUnfinishedBusiness.DataMiner
{
    internal class DataMinerContractResolver : DefaultContractResolver
    {
        private readonly DefinitionConverter _definitionConverter = new();
        private readonly DefinitionReferenceConverter _definitionReferenceConverter = new();

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return JsonUtil.GetUnitySerializableMembers(objectType).Distinct().ToList();
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == null)
            {
                return null;
            }

            if (!typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                return null;
            }

            if (_definitionConverter.CanRead && _definitionConverter.CanWrite)
            {
                return _definitionConverter;
            }

            return _definitionReferenceConverter;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (!typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                return contract;
            }

            contract.IsReference = false;
            contract.OnSerializedCallbacks.Add((_, _) => contract.Converter = _definitionConverter);
            contract.OnSerializingCallbacks.Add((_, _) => contract.Converter = _definitionReferenceConverter);
            contract.OnDeserializedCallbacks.Add((_, _) => contract.Converter = _definitionConverter);
            contract.OnDeserializingCallbacks.Add((_, _) => contract.Converter = _definitionReferenceConverter);

            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member is FieldInfo)
            {
                property.Readable = true;
                property.Writable = true;
            }

            if (property.DeclaringType == typeof(EffectForm))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var effectForm = (EffectForm)instance;

                        return property.PropertyName switch
                        {
                            "damageForm" => effectForm.FormType == EffectForm.EffectFormType.Damage,
                            "healingForm" => effectForm.FormType == EffectForm.EffectFormType.Healing,
                            "conditionForm" => effectForm.FormType == EffectForm.EffectFormType.Condition,
                            "lightSourceForm" => effectForm.FormType == EffectForm.EffectFormType.LightSource,
                            "summonForm" => effectForm.FormType == EffectForm.EffectFormType.Summon,
                            "counterForm" => effectForm.FormType == EffectForm.EffectFormType.Counter,
                            "temporaryHitPointsForm" => effectForm.FormType ==
                                                        EffectForm.EffectFormType.TemporaryHitPoints,
                            "motionForm" => effectForm.FormType == EffectForm.EffectFormType.Motion,
                            "spellSlotsForm" => effectForm.FormType == EffectForm.EffectFormType.SpellSlots,
                            "divinationForm" => effectForm.FormType == EffectForm.EffectFormType.Divination,
                            "itemPropertyForm" => effectForm.FormType == EffectForm.EffectFormType.ItemProperty,
                            "alterationForm" => effectForm.FormType == EffectForm.EffectFormType.Alteration,
                            "topologyForm" => effectForm.FormType == EffectForm.EffectFormType.Topology,
                            "reviveForm" => effectForm.FormType == EffectForm.EffectFormType.Revive,
                            "killForm" => effectForm.FormType == EffectForm.EffectFormType.Kill,
                            "shapeChangeForm" => effectForm.FormType == EffectForm.EffectFormType.ShapeChange,
                            _ => true
                        };
                    };
            }
            else if (property.DeclaringType == typeof(ItemDefinition))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var definition = (ItemDefinition)instance;

                        return property.PropertyName switch
                        {
                            "armorDefinition" => definition.IsArmor,
                            "weaponDefinition" => definition.IsWeapon,
                            "ammunitionDefinition" => definition.IsAmmunition,
                            "usableDeviceDescription" => definition.IsUsableDevice,
                            "toolDefinition" => definition.IsTool,
                            "starterPackDefinition" => definition.IsStarterPack,
                            "containerItemDefinition" => definition.IsContainerItem,
                            "lightSourceItemDefinition" => definition.IsLightSourceItem,
                            "focusItemDefinition" => definition.IsFocusItem,
                            "wealthPileDefinition" => definition.IsWealthPile,
                            "spellbookDefinition" => definition.IsSpellbook,
                            "documentDescription" => definition.IsDocument,
                            "foodDescription" => definition.IsFood,
                            "factionRelicDescription" => definition.IsFactionRelic,
                            _ => true
                        };
                    };
            }

            return property;
        }
    }
}
#endif
