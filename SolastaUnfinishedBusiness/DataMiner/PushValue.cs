#if DEBUG
using System;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.DataMiner
{
    internal class PushValue<T> : Disposable
    {
        private readonly T _oldValue;
        private Action<T> _setValue;

        internal PushValue(T value, Func<T> getValue, Action<T> setValue)
        {
            if (getValue == null) { throw new ArgumentNullException(nameof(getValue)); }

            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
            _oldValue = getValue();
            setValue(value);
        }

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            _setValue?.Invoke(_oldValue);
            _setValue = null;
        }

        #endregion
    }
}
#endif
