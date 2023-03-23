#if DEBUG
using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

internal abstract class Disposable : IDisposable
{
    #region Public Methods and Operators

    /// <summary>
    ///     A base class for disposable classes.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///     Releases unmanaged resources and performs other cleanup operations before the
    ///     <see cref="Disposable" /> is reclaimed by garbage collection.
    /// </summary>
    ~Disposable()
    {
        Dispose(false);
    }

    #endregion

    #region Methods

    protected abstract void Dispose([UsedImplicitly] bool _);

    #endregion
}
#endif
