#if DEBUG
using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

public class SetResetDisposable : Disposable
{
    private Action _reset;

    protected SetResetDisposable([NotNull] Action set, [NotNull] Action reset)
    {
        Preconditions.ArgumentIsNotNull(set, nameof(set));
        Preconditions.ArgumentIsNotNull(reset, nameof(reset));

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
