using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.ObjectPool
{
    /// <summary>
    /// Réserve clonant l'objet et appelant ses interfaces
    /// à son activation et désactivation.
    /// Peut instancier tout type de class, y compris celles n'héritant pas
    /// de MonoBehaviour ou ScriptableObject.
    /// </summary>
    /// <typeparam name="TBase">Le type de base dont les pools pourront contenir des types hérités.</typeparam>
    [Serializable]
    public class ClassPooler<TBase> where TBase : class
    {
        #region Instance

        /// <summary>
        /// Le dictionnaire contenant toutes les ObjectPools
        /// </summary>
        private readonly Dictionary<string, ObjectPool<TBase>> _poolDictionary;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="newPools">Les réserves à enregistrer</param>
        public ClassPooler(params IPool<TBase>[] newPools)
        {
            _poolDictionary = new Dictionary<string, ObjectPool<TBase>>(newPools.Length);
            AddPools(newPools);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Ajoute une nouvelle ObjectPool pour chaque IPool dans la liste.
        /// </summary>
        /// <param name="newPools">Les pools à convertir en ObjectPool</param>
        public void AddPools(params IPool<TBase>[] newPools)
        {
            for (int i = 0; i < newPools.Length; i++)
            {
                _poolDictionary.Add(newPools[i].Key, CreatePool(newPools[i].DefaultCpapcity, newPools[i].CreateFunc));
            }

        }

        /// <summary>
        /// Supprime les ObjectPools aux adresses renseignées
        /// </summary>
        /// <param name="keys">Les clés correspondant aux entrées des ObjectPools à supprimer</param>
        public void RemovePools(params string[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (_poolDictionary.ContainsKey(keys[i]))
                {
                    _poolDictionary[keys[i]].Dispose();
                    _poolDictionary.Remove(keys[i]);
                }
            }
        }

        /// <summary>
        /// Utilise un clone dans une instance "using" qui appelle IDisposable immédiatement
        /// après la fin du "using"
        /// </summary>
        /// <typeparam name="TChild">Le type du clone à récupérer</typeparam>
        /// <param name="key">L'adresse de l'ObjectPool contenant le <typeparamref name="TChild"/></param>
        /// <returns>Une version IDisposable du clone</returns>
        public PooledObject<TChild> UsingFromPool<TChild>(string key = null) where TChild : class, TBase
        {

            key ??= typeof(TChild).Name;

            if (!_poolDictionary.ContainsKey(key))
            {
                throw new ArgumentException($"Pooler Error : The key '{key}' does not exist.");
            }

            ObjectPool<TChild> pool = _poolDictionary[key] as ObjectPool<TChild>;

            return new PooledObject<TChild>(pool.Get(), pool);
        }

        /// <summary>
        /// Retourne un clone du type en paramètre via son adresse "<paramref name="key"/>" dans le dictionnaire
        /// </summary>
        /// <typeparam name="TChild">Le type du clone à récupérer</typeparam>
        /// <param name="key">L'adresse de l'ObjectPool contenant le <typeparamref name="TChild"/></param>
        /// <returns>Une instance du clone</returns>
        public TChild GetFromPool<TChild>(string key = null) where TChild : TBase
        {

            key ??= typeof(TChild).Name;

            if (!_poolDictionary.ContainsKey(key))
            {
                throw new ArgumentException($"Pooler Error : The key '{key}' does not exist.");
            }

            return (TChild)_poolDictionary[key].Get();
        }

        /// <summary>
        /// Renvoie un clone dans son ObjectPool correspondante une fois utilisé
        /// </summary>
        /// <param name="obj">Le clone à renvoyer</param>
        /// <param name="key">L'adresse de son ObjectPool dans le dictionnaire</param>
        public void ReturnToPool(TBase obj, string key = null)
        {
            key ??= obj.GetType().Name;

            if (!_poolDictionary.ContainsKey(key))
            {
                throw new ArgumentException($"Pooler Error : The key '{key}' does not exist.");
            }

            _poolDictionary[key].Release(obj);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crée un ObjectPool à partir de <typeparamref name="TBase"/>.
        /// </summary>
        /// <param name="defaultCapacity">La taille de base de la liste contenant les clones.</param>
        /// <param name="createFunc">La fonction retournant un clone de <typeparamref name="TBase"/>.</param>
        /// <returns>Un ObjectPool ajouté au dictionnaire du ClassPooler.</returns>
        private ObjectPool<TBase> CreatePool(int defaultCapacity, Func<TBase> createFunc)
        {
            ObjectPool<TBase> newPool = new
                (
                    createFunc: () => createFunc.Invoke(),
                    actionOnGet: (obj) => Dequeue(obj),
                    actionOnRelease: (obj) => Enqueue(obj),
                    collectionCheck: false,
                    defaultCapacity: defaultCapacity
                );

            return newPool;
        }

        /// <summary>
        /// Crée un clone (ou en sort un existant de l'ObjectPool) et appele sa fonction d'activation
        /// </summary>
        /// <param name="obj">L'objet à activer</param>
        private static void Dequeue(TBase obj)
        {
            // (IDequeued)obj causes an InvalidCastException if obj does not derive from the interface
            switch (obj)
            {
                case IDequeued pooledObj:
                    pooledObj.OnDequeued();
                    break;

                case GameObject go:
                    IDequeued[] components = go.GetComponents<IDequeued>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnDequeued();
                    }
                    break;
            }
        }

        /// <summary>
        /// Crée un clone (ou en sort un existant de l'ObjectPool) et appele sa fonction de désactivation
        /// </summary>
        /// <param name="obj">L'objet à activer</param>
        private static void Enqueue(TBase obj)
        {
            // (IEnqueued)obj causes an InvalidCastException if obj does not derive from the interface
            switch (obj)
            {
                case IEnqueued pooledObj:
                    pooledObj.OnEnqueued();
                    break;

                case GameObject go:
                    IEnqueued[] components = go.GetComponents<IEnqueued>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].OnEnqueued();
                    }
                    break;
            }
        }

        #endregion
    }
}