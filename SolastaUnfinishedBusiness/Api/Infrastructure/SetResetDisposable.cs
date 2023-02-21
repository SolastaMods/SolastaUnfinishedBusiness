#if false
using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

internal class SetResetDisposable : Disposable
{
    private Action _reset;

    protected SetResetDisposable([NotNull] Action set, [NotNull] Action reset)
    {
        PreConditions.ArgumentIsNotNull(set, nameof(set));
        PreConditions.ArgumentIsNotNull(reset, nameof(reset));

        _reset = reset;

        set();
    }

    protected override void Dispose(bool disposing)
    {
        if (_reset == null)
        {
            return;
        }

        _reset();
        _reset = null;
    }
}
#endif
