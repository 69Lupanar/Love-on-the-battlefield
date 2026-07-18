using System;

namespace Assets.Scripts.ObjectPool
{
    /// <summary>
    /// Conteneur pour le type de classe à cloner
    /// </summary>
    /// <typeparam name="T">Un type à cloner</typeparam>
    public class Pool<T> : IPool<T> where T : class
    {
        #region Propriétés

        /// <summary>
        /// ID du type enregistré dans la réserve
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Taille par défaut de la réserve
        /// </summary>
        public int DefaultCpapcity { get; }

        /// <summary>
        /// Méthode de création d'une instance
        /// </summary>
        public Func<T> CreateFunc { get; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="key">ID du type enregistré dans la réserve</param>
        /// <param name="defaultCapacity">Taille par défaut de la réserve</param>
        /// <param name="createFunc">Méthode de création d'une instance</param>
        public Pool(string key, int defaultCapacity, Func<T> createFunc)
        {
            Key = key;
            DefaultCpapcity = defaultCapacity;
            CreateFunc = createFunc;
        }

        #endregion
    }
}