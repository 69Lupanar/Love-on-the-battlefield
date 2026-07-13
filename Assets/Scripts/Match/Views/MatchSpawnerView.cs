using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère la création des joueurs
    /// </summary>
    [RequireComponent(typeof(MatchSpawnerViewModel))]
    public sealed class MatchSpawnerView : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchSpawnerViewModel _vm;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchSpawnerViewModel>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Nettoie le terrain si on a déjà lancé un match
        /// </summary>
        internal void CleanupField()
        {
            if (_vm.AlliesT != null)
            {
                _vm.DisableActivePlayersAndBalls();
            }
        }

        /// <summary>
        /// Crée de nouveaux joueurs et ballons
        /// </summary>
        internal (List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT) Spawn(int nbAllies, int nbEnemies, int nbBalls)
        {
            _vm.SpawnPlayersAndBalls(nbAllies, nbEnemies, nbBalls);
            _vm.ResetPlayersAndBallsPoses();

            return (_vm.AlliesT, _vm.EnemiesT, _vm.BallsT);
        }

        #endregion
    }
}