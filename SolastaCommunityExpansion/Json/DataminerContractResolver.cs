using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolastaCommunityExpansion.Json
{
    internal class DataminerContractResolver : DefaultContractResolver
    {
        private readonly DefinitionReferenceConverter DefinitionReferenceConverter = new DefinitionReferenceConverter();

        private readonly DefinitionConverter DefinitionConverter = new DefinitionConverter();

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

            if (typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                if (DefinitionConverter.CanRead && DefinitionConverter.CanWrite)
                {
                    return DefinitionConverter;
                }
                else
                {
                    return DefinitionReferenceConverter;
                }
            }

            return null;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                contract.IsReference = false;
                contract.OnSerializedCallbacks.Add((_, __) => contract.Converter = DefinitionConverter);
                contract.OnSerializingCallbacks.Add((_, __) => contract.Converter = DefinitionReferenceConverter);
                contract.OnDeserializedCallbacks.Add((_, __) => contract.Converter = DefinitionConverter);
                contract.OnDeserializingCallbacks.Add((_, __) => contract.Converter = DefinitionReferenceConverter);
            }
            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

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

                        switch (property.PropertyName)
                        {
                            case "damageForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Damage;
                            case "healingForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Healing;
                            case "conditionForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Condition;
                            case "lightSourceForm":
                                return effectForm.FormType == EffectForm.EffectFormType.LightSource;
                            case "summonForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Summon;
                            case "counterForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Counter;
                            case "temporaryHitPointsForm":
                                return effectForm.FormType == EffectForm.EffectFormType.TemporaryHitPoints;
                            case "motionForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Motion;
                            case "spellSlotsForm":
                                return effectForm.FormType == EffectForm.EffectFormType.SpellSlots;
                            case "divinationForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Divination;
                            case "itemPropertyForm":
                                return effectForm.FormType == EffectForm.EffectFormType.ItemProperty;
                            case "alterationForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Alteration;
                            case "topologyForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Topology;
                            case "reviveForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Revive;
                            case "killForm":
                                return effectForm.FormType == EffectForm.EffectFormType.Kill;
                            case "shapeChangeForm":
                                return effectForm.FormType == EffectForm.EffectFormType.ShapeChange;
                        }

                        return true;
                    };
            }
            else if (property.DeclaringType == typeof(ItemDefinition))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var definition = (ItemDefinition)instance;

                        switch (property.PropertyName)
                        {
                            case "armorDefinition":
                                return definition.IsArmor;
                            case "weaponDefinition":
                                return definition.IsWeapon;
                            case "ammunitionDefinition":
                                return definition.IsAmmunition;
                            case "usableDeviceDescription":
                                return definition.IsUsableDevice;
                            case "toolDefinition":
                                return definition.IsTool;
                            case "starterPackDefinition":
                                return definition.IsStarterPack;
                            case "containerItemDefinition":
                                return definition.IsContainerItem;
                            case "lightSourceItemDefinition":
                                return definition.IsLightSourceItem;
                            case "focusItemDefinition":
                                return definition.IsFocusItem;
                            case "wealthPileDefinition":
                                return definition.IsWealthPile;
                            case "spellbookDefinition":
                                return definition.IsSpellbook;
                            case "documentDescription":
                                return definition.IsDocument;
                            case "foodDescription":
                                return definition.IsFood;
                            case "factionRelicDescription":
                                return definition.IsFactionRelic;
                        }
                        return true;
                    };
            }

            return property;
        }
    }
}
