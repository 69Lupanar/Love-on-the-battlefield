using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    [RequireComponent(typeof(MatchManagerViewModel))]
    internal sealed class MatchManagerView : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchManagerViewModel _vm;

        /// <summary>
        /// Le spawner des joueurs et ballons
        /// </summary>
        private MatchSpawnerView _spawnerV;

        /// <summary>
        /// Le spawner des joueurs et ballons
        /// </summary>
        private MatchSpawnerViewModel _spawnerVM;

        /// <summary>
        /// Lae MatchPlayerManagerViewModel
        /// </summary>
        private MatchPlayerControllerViewModel _playerVM;

        /// <summary>
        /// La caméra
        /// </summary>
        private MatchPlayerCameraView _cameraV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
            _spawnerVM = FindAnyObjectByType<MatchSpawnerViewModel>();
            _playerVM = FindAnyObjectByType<MatchPlayerControllerViewModel>();
            _cameraV = FindAnyObjectByType<MatchPlayerCameraView>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Démarre un nouveau match
        /// </summary>
        internal void StartNewMatch()
        {
            _spawnerV.CleanupField();
            (List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT) = _spawnerV.Spawn(_vm.NbAllies, _vm.NbEnemies, _vm.NbBalls);

            if (_playerVM.Allies != null)
            {
                // Désactive les inputs des joueurs déjà présents avant de les retirer
                _playerVM.EnablePlayersInput(false);
            }

            _playerVM.SetPlayersAndBalls(alliesT, enemiesT, ballsT);
            _playerVM.SetTeams();
            _vm.MatchIsOver = false;

            StartNewSet();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        private void StartNewSet()
        {
            _spawnerVM.ResetPlayersAndBallsPoses();
            _playerVM.ResetPlayersAndBalls();
            _playerVM.SetActivePlayer(_playerVM.ActivePlayerIndex);
            _playerVM.EnablePlayersInput(false);

            // A retirer une fois les tests finis
            _playerVM.EnablePlayersInput(true);

            // TAF: Démarrer le décompte avant de rendre le contrôle aux persos
        }

        #endregion
    }
}