using System;
using UnityEngine.Pool;

namespace Assets.Scripts.ObjectPool
{
    /// <summary>
    /// A Pooled object wraps a reference to an instance that will be returned to the pool when the Pooled object is disposed.
    /// The purpose is to automate the return of references so that they do not need to be returned manually.
    /// A PooledObject can be used like so:
    /// <code>
    /// 
    /// using(myPool.Get(out MyClass myInstance)) // When leaving the scope myInstance will be returned to the pool.
    /// {
    ///     // Do something with myInstance
    /// }
    /// </code>
    /// </summary>
    public readonly struct PooledObject<T> : IDisposable where T : class
    {
        #region Instance

        /// <summary>
        /// L'objet à recycler
        /// </summary>
        private readonly T _objToReturn;

        /// <summary>
        /// La réserve parente
        /// </summary>
        private readonly ObjectPool<T> _pool;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="obj">L'objet à recycler</param>
        /// <param name="pool">La réserve parente</param>
        internal PooledObject(T obj, ObjectPool<T> pool)
        {
            _objToReturn = obj;
            _pool = pool;
        }

        #endregion

        #region Fonctions privées

        /// <summary>
        /// Nettoyage
        /// </summary>
        void IDisposable.Dispose() => _pool.Release(_objToReturn);

        #endregion
    }
}