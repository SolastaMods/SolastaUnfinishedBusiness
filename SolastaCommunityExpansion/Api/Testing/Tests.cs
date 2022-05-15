#if DEBUG
using System;
using System.Linq;
using System.Reflection;
using SolastaModApi.Diagnostics;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaModApi.Testing
{
    public static class Tests
    {
        internal static void CheckDatabaseDefinitions()
        {
            using var logger = new MethodLogger(nameof(Tests));

            try
            {
                var dbHelperTypes = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.Namespace == "SolastaModApi")
                    .Where(t => t.FullName.StartsWith("SolastaModApi.DatabaseHelper"))
                    .Where(t => t.MemberType == MemberTypes.NestedType)
                    .OrderBy(t => t.Name);

                var totalGettersSucceeded = 0;

                foreach (var dbHelperType in dbHelperTypes)
                {
                    var propertyGetters = dbHelperType
                        .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);

                    var gettersSucceeded = 0;

                    foreach (var getter in propertyGetters)
                    {
                        try
                        {
                            var result = getter.GetMethod.Invoke(null, Array.Empty<object>());

                            if (result == null)
                            {
                                logger.Log($"ERROR property '{dbHelperType.Name}.{getter.Name}' returned NULL.");
                            }
                            else
                            {
                                gettersSucceeded++;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Log($"ERROR getting property '{dbHelperType.Name}.{getter.Name}': {ex.Message}.");
                        }
                    }

                    totalGettersSucceeded += gettersSucceeded;
                }

                logger.Log($"Successfully invoked grand total of {totalGettersSucceeded} db helper properties.");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }

        internal static void CheckExtensions()
        {
            using var logger = new MethodLogger(nameof(Tests));

            try
            {
                var extensions = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.Namespace == "SolastaModApi.Extensions")
                    .Where(t => t.Name.EndsWith("Extensions"))
                    .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(TargetTypeAttribute)))
                    .Select(t => new
                    {
                        Type = t,
                        t.GetCustomAttributes<TargetTypeAttribute>().First().TargetType
                    })
                    .OrderBy(t => t.Type.Name)
                    .ToList();

                var totalMethodsSucceeded = 0;

                foreach (var extension in extensions)
                {
                    try
                    {
                        var ctor = extension.TargetType.GetConstructor(Array.Empty<Type>());

                        if (ctor == null)
                        {
                            logger.Log($"WARN: skipping type {extension.TargetType.Name}.  No default constructor.");
                        }
                        else if (extension.TargetType.IsAbstract)
                        {
                            logger.Log($"WARN: skipping type {extension.TargetType.Name}. Abstract type.");
                        }
                        else
                        {
                            var instance = extension.TargetType.IsSubclassOf(typeof(ScriptableObject))
                                ? ScriptableObject.CreateInstance(extension.TargetType.FullName)
                                : ctor.Invoke(Array.Empty<object>());

                            if (instance == null)
                            {
                                logger.Log($"ERROR: unable to create {extension.TargetType.Name} - Unknown reason.");
                            }
                            else
                            {
                                var setters = extension.Type
                                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                    .Where(m => m.Name.StartsWith("Set"));

                                var methodsSucceeded = 0;

                                foreach (var setter in setters)
                                {
                                    try
                                    {
                                        var parms = setter.GetParameters();

                                        if (parms.Length == 2)
                                        {
                                            if (setter.IsGenericMethod)
                                            {
                                                setter
                                                    .MakeGenericMethod(extension.TargetType)
                                                    .Invoke(null, new[] { instance, GetDefaultValue() });
                                            }
                                            else
                                            {
                                                // sealed type extensions aren't generic
                                                setter
                                                    .Invoke(null, new[] { instance, GetDefaultValue() });
                                            }

                                            methodsSucceeded++;

                                            object GetDefaultValue()
                                            {
                                                var valueParm = parms[1];

                                                return valueParm.ParameterType.IsValueType
                                                    ? Activator.CreateInstance(valueParm.ParameterType)
                                                    : null;
                                            }
                                        }
                                        else
                                        {
                                            logger.Log($"Skipping method '{extension.TargetType.Name}.{setter.Name}', doesn't have 2 params.");
                                        }
                                    }
                                    catch (Exception ex1)
                                    {
                                        logger.Log($"Error calling method '{extension.TargetType.Name}.{setter.Name}': {ex1.Message}.");
                                    }
                                }

                                totalMethodsSucceeded += methodsSucceeded;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"ERROR: testing '{extension.TargetType.Name}': {ex.Message}");
                    }
                }

                logger.Log("---------------------------");
                logger.Log($"Successfully called grand total of {totalMethodsSucceeded} extension setter methods.");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }

        private sealed class TestObj
        {
            public TestObj(string testValue)
            {
                TestProp = testValue;
            }

            internal string TestProp { get; set; }
        }

        internal static void CheckHelpers()
        {
            var definition = ScriptableObject.CreateInstance<EffectProxyDefinition>();

            // Lacking a standard unit testing framework, just cobble some stuff together.

            var failures = 0;

            if (!CheckSetFieldSucceeds(definition, "actionId", ActionDefinitions.Id.ActionSurge)) { failures++; }
            if (!CheckSetFieldSucceeds(definition, "addLightSource", true)) { failures++; }

            if (!CheckSetFieldThrows(definition, "addLightSource2", true)) { failures++; }
            if (!CheckSetFieldThrows((EffectProxyDefinition)null, "addLightSource", true)) { failures++; }
            if (!CheckSetFieldThrows(definition, "addLightSource", 5)) { failures++; }

            SolastaCommunityExpansion.Main.Log($"{failures} calls to SetField helpers failed");

            if (!CheckGetFieldSucceeds(definition, "addLightSource", true, false)) { failures++; }
            if (!CheckGetFieldSucceeds(definition, "damageType", "d1", "d2")) { failures++; }
            if (!CheckGetFieldThrows<EffectProxyDefinition, string>(definition, "damageType2")) { failures++; }
            if (!CheckGetFieldThrows<EffectProxyDefinition, string>(null, "damageType2")) { failures++; }
            if (!CheckGetFieldThrows<EffectProxyDefinition, bool>(definition, "damageType")) { failures++; }

            SolastaCommunityExpansion.Main.Log($"{failures} calls to GetField helpers failed");

            var testObj = new TestObj("Test value");

            failures = 0;
            if (!CheckSetPropertySucceeds(testObj, "TestProp", "Test 1")) { failures++; }
            if (!CheckGetPropertySucceeds(testObj, "TestProp", "Test 1", "Test 2")) { failures++; }
            SolastaCommunityExpansion.Main.Log($"{failures} calls to Get/SetProperty helpers failed");

            bool CheckGetFieldSucceeds<T, V>(T entity, string fieldName, V v1, V v2)
                where T : class
                where V : IEquatable<V>
            {
                var success = true;

                try
                {
                    entity.SetField(fieldName, v1);

                    if (!v1.Equals(entity.GetField<V>(fieldName)))
                    {
                        SolastaCommunityExpansion.Main.Log($"GetField({fieldName}) failed.");
                        success = false;
                    }

                    entity.SetField(fieldName, v2);

                    if (!v2.Equals(entity.GetField<V>(fieldName)))
                    {
                        SolastaCommunityExpansion.Main.Log($"GetField({fieldName}) failed.");
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"GetField({fieldName}) failed. {ex.Message}.");
                    success = false;
                }

                return success;
            }

            bool CheckGetFieldThrows<T, V>(T entity, string fieldName)
                where T : class
                where V : IEquatable<V>
            {
                var success = false;

                try
                {
                    entity.GetField<V>(fieldName);
                    SolastaCommunityExpansion.Main.Log($"GetField({fieldName}) failed. Did not throw exception.");
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"GetField({fieldName}) threw exception as expected. {ex.Message}.");
                    success = true;
                }

                return success;
            }

            bool CheckSetFieldSucceeds<T, V>(T entity, string fieldName, V value) where T : class
            {
                var success = false;

                try
                {
                    entity.SetField(fieldName, value);

                    success = true;
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"SetField({fieldName}) failed. {ex.Message}");
                }

                return success;
            }

            bool CheckSetFieldThrows<T, V>(T entity, string fieldName, V value) where T : class
            {
                var success = false;

                try
                {
                    entity.SetField(fieldName, value);

                    SolastaCommunityExpansion.Main.Log($"SetField({fieldName}) didn't throw.");
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"SetField({fieldName}) threw exception as expected. {ex.Message}.");
                    success = true;
                }

                return success;
            }

            bool CheckGetPropertySucceeds<T, V>(T entity, string propertyName, V v1, V v2)
                where T : class
                where V : IEquatable<V>
            {
                var success = true;

                try
                {
                    entity.SetProperty(propertyName, v1);

                    if (!v1.Equals(entity.GetProperty<V>(propertyName)))
                    {
                        SolastaCommunityExpansion.Main.Log($"GetProperty({propertyName}) failed.");
                        success = false;
                    }

                    entity.SetProperty(propertyName, v2);

                    if (!v2.Equals(entity.GetProperty<V>(propertyName)))
                    {
                        SolastaCommunityExpansion.Main.Log($"GetProperty({propertyName}) failed.");
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"GetProperty({propertyName}) failed. {ex.Message}.");
                    success = false;
                }

                return success;
            }

            bool CheckSetPropertySucceeds<T, V>(T entity, string PropertyName, V value)
                where T : class
                where V : IEquatable<V>
            {
                var success = false;

                try
                {
                    entity.SetProperty(PropertyName, value);

                    if (!entity.GetProperty<V>(PropertyName).Equals(value))
                    {
                        SolastaCommunityExpansion.Main.Log($"SetProperty({PropertyName}) failed.  Values differ.");
                    }
                    else
                    {
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    SolastaCommunityExpansion.Main.Log($"SetProperty({PropertyName}) failed. {ex.Message}");
                }

                return success;
            }
        }
    }
}
#endif
