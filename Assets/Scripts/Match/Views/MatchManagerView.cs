using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    [RequireComponent(typeof(MatchManagerViewModel))]
    public class MatchManagerView : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchManagerViewModel _vm;

        /// <summary>
        /// Le spawner des joueurs et ballons
        /// </summary>
        private MatchSpawnerView _spawner;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _spawner = FindAnyObjectByType<MatchSpawnerView>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Démarre un nouveau match
        /// </summary>
        internal void StartNewMatch()
        {
            _spawner.CleanupField();
            (List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT) = _spawner.Spawn(_vm.NbAllies, _vm.NbEnemies, _vm.NbBalls);

            if (_vm.Allies != null)
            {
                // Désactive les inputs des joueurs déjà présents avant de les retirer
                _vm.EnablePlayersInput(false);
            }

            _vm.SetPlayersAndBalls(alliesT, enemiesT, ballsT);
            _vm.SetTeams();
            _vm.SetActivePlayer(_vm.ActivePlayerIndex);
            _vm.EnablePlayersInput(false);

            // A retirer une fois les tests finis
            _vm.EnablePlayersInput(true);
        }

        #endregion
    }
}