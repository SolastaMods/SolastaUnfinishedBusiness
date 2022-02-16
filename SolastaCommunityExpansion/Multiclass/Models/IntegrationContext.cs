using System;
using System.Linq;
using System.Reflection;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class IntegrationContext
    {
        public const string CLASS_TINKERER = "ClassTinkerer";
        public const string CLASS_WARDEN = "ClassWarden";
        public const string CLASS_WITCH = "ClassWitch";
        public const string CLASS_ALCHEMIST = "AlchemistClass";
        public const string CLASS_BARD = "BardClass";
        public const string CLASS_MONK = "MonkClass";
        public const string CLASS_WARLOCK = "WarlockClass";

        internal static CharacterClassDefinition DummyClass { get; private set; }
        internal static CharacterClassDefinition TinkererClass { get; private set; }
        internal static CharacterClassDefinition WardenClass { get; private set; }
        internal static CharacterClassDefinition WitchClass { get; private set; }
        internal static CharacterClassDefinition AlchemistClass { get; private set; }
        internal static CharacterClassDefinition BardClass { get; private set; }
        internal static CharacterClassDefinition MonkClass { get; private set; }
        internal static CharacterClassDefinition WarlockClass { get; private set; }

        private static void GetReferencesOnUnofficialClasses()
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            dbCharacterClassDefinition.TryGetElement(CLASS_TINKERER, out var unofficialTinkerer);
            dbCharacterClassDefinition.TryGetElement(CLASS_WARDEN, out var unofficialWarden);
            dbCharacterClassDefinition.TryGetElement(CLASS_WITCH, out var unofficialWitch);
            dbCharacterClassDefinition.TryGetElement(CLASS_ALCHEMIST, out var unofficialAlchemist);
            dbCharacterClassDefinition.TryGetElement(CLASS_BARD, out var unofficialBard);
            dbCharacterClassDefinition.TryGetElement(CLASS_MONK, out var unofficialMonk);
            dbCharacterClassDefinition.TryGetElement(CLASS_WARLOCK, out var unofficialWarlock);

            DummyClass = CharacterClassDefinitionBuilder.Create("DummyClass", "062d696ab44146e0b316188f943d8079").AddToDB();

            // NOTE: don't use ?? here which bypasses Unity object lifetime check
            TinkererClass = unofficialTinkerer ? unofficialTinkerer : DummyClass;
            WardenClass = unofficialWarden ? unofficialWarden : DummyClass;
            WitchClass = unofficialWitch ? unofficialWitch : DummyClass;
            AlchemistClass = unofficialAlchemist ? unofficialAlchemist : DummyClass;
            BardClass = unofficialBard ? unofficialBard : DummyClass;
            MonkClass = unofficialMonk ? unofficialMonk : DummyClass;
            WarlockClass = unofficialWarlock ? unofficialWarlock : DummyClass;
        }

        internal static bool IsExtraContentInstalled => WarlockClass != DummyClass;

        internal static void Load()
        {
            GetReferencesOnUnofficialClasses();
            Main.Logger.Log(WarlockClass != DummyClass ? "Pact magic integration enabled." : "Pact magic integration disabled.");
        }

        internal static Assembly GetModAssembly(string modName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains(modName));
        }

        internal static Type GetModType(string modName, string typeName)
        {
            return GetModAssembly(modName)?.GetExportedTypes().FirstOrDefault(x => x.FullName.Contains(typeName));
        }

        internal static bool SetModField(string modName, string typeName, string fieldName, object value)
        {
            var type = GetModType(modName, typeName);

            if (type != null)
            {
                var fieldInfo = type.GetField(fieldName);

                if (fieldInfo != null)
                {
                    try
                    {
                        fieldInfo.SetValue(type, value);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
