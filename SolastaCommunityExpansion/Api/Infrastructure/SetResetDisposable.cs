#if DEBUG
using System;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public class SetResetDisposable : Disposable
{
    private Action _reset;

    protected SetResetDisposable([NotNull] Action set, [NotNull] Action reset)
    {
        Preconditions.IsNotNull(set, nameof(set));
        Preconditions.IsNotNull(reset, nameof(reset));

        _reset = reset;

        set();
    }

    protected override void Dispose(bool disposing)
    {
        if (_reset != null)
        {
            _reset();
            _reset = null;
        }
    }
}
#endif
