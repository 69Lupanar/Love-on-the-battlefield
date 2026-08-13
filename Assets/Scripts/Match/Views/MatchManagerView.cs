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
        internal EventHandler<MatchSettingsData> OnNewMatchStartedEvent;

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        internal EventHandler OnNewSetStartedEvent;

        /// <summary>
        /// Appelée quand un match est terminé
        /// </summary>
        internal EventHandler<TeamID> OnMatchEndedEvent;

        #endregion

        #region Propriétés

        /// <summary>
        /// true si aucun match n'est en cours
        /// </summary>
        internal bool MatchIsOngoing => _vm.MatchIsOngoing;

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
        private MatchCharacterManagerView _playerV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
            _playerV = FindAnyObjectByType<MatchCharacterManagerView>();
        }

        /// <summary>
        /// init
        /// </summary>
        private void Start()
        {
            _playerV.OnCharacterEliminatedEvent += OnCharacterEliminated;
        }

        /// <summary>
        /// nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _playerV.OnCharacterEliminatedEvent -= OnCharacterEliminated;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Démarre un nouveau match
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        internal void StartNewMatch(MatchSettingsData matchSettings)
        {
            _vm.StartNewMatch(matchSettings);
            OnNewMatchStartedEvent?.Invoke(this, matchSettings);
        }

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        internal void StartNewSet()
        {
            _vm.StartNewSet();
            OnNewSetStartedEvent?.Invoke(this, EventArgs.Empty);

            // TAF: Démarrer le décompte avant de rendre le contrôle aux persos
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelée quand un perso est éliminé
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnCharacterEliminated(object _, CharacterEliminatedEventArgs e)
        {
            _vm.OnCharacterEliminated(e.CharacterIsAlly);

            // TAF : Vérifier les conditions de victoire

            if (_vm.NbLiveAllies == 0)
            {
                // Victoire ennemie
                EndMatch(TeamID.Enemy);
            }
            else if (_vm.NbLiveEnemies == 0)
            {
                // Victoire alliée
                EndMatch(TeamID.Ally);
            }
        }

        /// <summary>
        /// Met fin au match
        /// </summary>
        /// <param name="victoriousTeamID">ID de l'équipe victorieuse</param>
        private void EndMatch(TeamID victoriousTeamID)
        {
            switch (victoriousTeamID)
            {
                case TeamID.Ally:
                    // TAF : Victoire alliée
                    break;
                case TeamID.Enemy:
                    // TAF : Victoire ennemie
                    break;
                case TeamID.None:
                    // TAF : Match null, on lance une manche décisive
                    break;
            }

            OnMatchEndedEvent?.Invoke(this, victoriousTeamID);
        }

        #endregion
    }
}