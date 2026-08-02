using System;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    [RequireComponent(typeof(MatchManagerViewModel))]
    internal sealed class MatchManagerView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelée quand un nouveau match commence
        /// </summary>
        internal Action<MatchSettingsData> OnNewMatchStarted { get; set; }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        internal Action OnNewSetStarted { get; set; }

        #endregion

        #region Propriétés

        /// <summary>
        /// true si aucun match n'est en cours
        /// </summary>
        internal bool MatchIsOver => _vm.MatchIsOver;

        #endregion

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
        /// Le contrôleur des persos
        /// </summary>
        private MatchPlayerControllerView _playerV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
            _playerV = FindAnyObjectByType<MatchPlayerControllerView>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Démarre un nouveau match
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        internal void StartNewMatch(MatchSettingsData matchSettings)
        {
            _vm.MatchIsOver = false;
            OnNewMatchStarted?.Invoke(matchSettings);

            StartNewSet();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        private void StartNewSet()
        {
            OnNewSetStarted?.Invoke();

            // TAF: Démarrer le décompte avant de rendre le contrôle aux persos
        }

        #endregion
    }
}