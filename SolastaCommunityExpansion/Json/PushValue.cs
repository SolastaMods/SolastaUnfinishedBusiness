using System;

namespace SolastaCommunityExpansion.Json
{
    public struct PushValue<T> : IDisposable
    {
        private readonly Action<T> setValue;
        private readonly T oldValue;

        public PushValue(T value, Func<T> getValue, Action<T> setValue)
        {
            if (getValue == null) { throw new ArgumentNullException(nameof(getValue)); }
            this.setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
            oldValue = getValue();
            setValue(value);
        }

        #region IDisposable Members

        // By using a disposable struct we avoid the overhead of allocating and freeing an instance of a finalizable class.
        public void Dispose()
        {
            setValue?.Invoke(oldValue);
        }

        #endregion
    }
}
