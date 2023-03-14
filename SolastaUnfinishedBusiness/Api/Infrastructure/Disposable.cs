#if DEBUG
using System;

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

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected abstract void Dispose(bool _);

    #endregion
}
#endif
