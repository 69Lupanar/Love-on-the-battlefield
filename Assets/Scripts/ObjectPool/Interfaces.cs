using System;

namespace Assets.Scripts.ObjectPool
{
    /// <summary>
    /// Interface covariante pour permettre d'assigner n'importe quel type héritant de T
    /// </summary>
    /// <typeparam name="T">Le type de base à cloner</typeparam>
    public interface IPool<out T>
    {
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
    }

    /// <summary>
    /// Appelée quand un élément retourne dans la réserve
    /// </summary>
    public interface IEnqueued
    {
        void OnEnqueued();
    }

    /// <summary>
    /// Appelée quand un élément quitte la réserve
    /// </summary>
    public interface IDequeued
    {
        void OnDequeued();
    }
}