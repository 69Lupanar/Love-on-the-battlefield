using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match de dodgeball
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Prefab des alliés dirigés par le joueur")]
        private GameObject _allyPrefab;

        [SerializeField]
        [Tooltip("Prefab des ennemis dirigés par l'IA")]
        private GameObject _enemyPrefab;

        [SerializeField]
        [Tooltip("Prefab des ballons")]
        private GameObject _ballPrefab;

        [SerializeField]
        [Tooltip("Point de placement des ballons en début de partie")]
        private Transform _ballSpawnPoints;

        [SerializeField]
        [Tooltip("Point de placement des alliés en début de partie")]
        private Transform _allySpawnPoints;

        [SerializeField]
        [Tooltip("Point de placement des ennemis en début de partie")]
        private Transform _enemySpawnPoints;

        #endregion

        #region Instance

        /// <summary>
        /// Alliés instanciés
        /// </summary>
        private CharacterController[] _allies;

        /// <summary>
        /// Ennemis instanciés
        /// </summary>
        private CharacterController[] _enemies;

        /// <summary>
        /// Ballons instanciés
        /// </summary>
        private Ball[] _balls;

        #endregion
    }
}