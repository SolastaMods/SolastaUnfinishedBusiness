#if false
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Api.Diagnostics
{
    /// <summary>
    /// <para>
    /// Usage
    /// internal class SomeType
    /// {
    ///     internal int SomeMethod(...)
    ///     {
    ///         using(var logger = new MethodLogger(nameof(SomeType)))
    ///         {
    ///             ...
    ///             logger.Log("This is a message");
    ///             ...
    ///         }
    ///     }
    /// }
    /// </para>
    /// <para>
    /// Log entries are:
    /// SomeType.SomeMethod: Enter
    /// SomeType.SomeMethod: This is a message
    /// SomeType.SomeMethod: Exit
    /// </para>
    /// </summary>
    internal sealed class MethodLogger : SetResetDisposable
    {
        private readonly string methodName;

        // Very annoying there's no CallerTypeNameAttribute
        private readonly string typeName;

        internal MethodLogger(string typeName, [CallerMemberName] [CanBeNull] string methodName = null) :
            base(() => Main.Log($"{typeName}.{methodName}: Enter"), () => Main.Log($"{typeName}.{methodName}: Leave"))
        {
            this.methodName = methodName;
            this.typeName = typeName;
        }

        internal void Log(string message)
        {
            Main.Log($"{typeName}.{methodName}: {message}");
        }
    }
}
#endif
