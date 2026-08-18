using System;
using TMPro;
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
        /// Appelée quand on atteint la mi-temps
        /// </summary>
        internal EventHandler OnHalfTimeReachedEvent;

        /// <summary>
        /// Appelée quand une partie est terminée
        /// </summary>
        internal EventHandler OnMatchEndedEvent;

        /// <summary>
        /// Appelée quand une manche est terminée
        /// </summary>
        internal EventHandler<TeamID> OnSetEndedEvent;

        #endregion

        #region Propriétés

        /// <summary>
        /// true si aucun match n'est en cours
        /// </summary>
        internal bool MatchIsOngoing => _vm.MatchIsOngoing;

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Label affichant la durée de la partie")]
        private TextMeshProUGUI _matchDurationField;

        [SerializeField]
        [Tooltip("Label affichant la durée de la manche en cours")]
        private TextMeshProUGUI _setDurationField;

        [SerializeField]
        [Tooltip("Label affichant le nb de manches réalisées")]
        private TextMeshProUGUI _currentSetField;

        [SerializeField]
        [Tooltip("Label affichant le score des alliés")]
        private TextMeshProUGUI _alliesScoreField;

        [SerializeField]
        [Tooltip("Label affichant le score des ennemis")]
        private TextMeshProUGUI _enemiesScoreField;

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

        /// <summary>
        /// Décompte
        /// </summary>
        private float _timer;

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

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (MatchIsOngoing)
            {
                _timer += Time.deltaTime;

                if (_timer >= 1f)
                {
                    _timer = 0f;
                    OnTick();

                    if (_vm.MatchTimer == _vm.MatchSettingsData.HalfDuration)
                    {
                        // On a atteint la mi-temps, on arrête la manche en cours
                        OnHalfTimeReached();
                    }
                    else if (_vm.SetDuration == _vm.MatchSettingsData.SetDuration && !_vm.SuddenDeath)
                    {
                        // Arrête la manche une fois son temps écoulé
                        // si elle n'est pas en mort subite.
                        EndSet(_vm.GetSetWinningTeam());
                    }
                }
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Démarre un nouveau match
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        internal void StartNewMatch(MatchSettingsData matchSettings)
        {
            _vm.StartNewMatch(matchSettings);

            _matchDurationField.SetText("0:00");
            _currentSetField.SetText("0");

            OnNewMatchStartedEvent?.Invoke(this, matchSettings);
        }

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        /// <param name="suddenDeath">true si la manche doit se jouer en mort subite</param>
        internal void StartNewSet(bool suddenDeath = false)
        {
            _vm.StartNewSet(suddenDeath);

            _setDurationField.SetText("0:00");
            _currentSetField.SetText(_vm.CurrentSet.ToString());
            _alliesScoreField.SetText("0");
            _enemiesScoreField.SetText("0");

            OnNewSetStartedEvent?.Invoke(this, EventArgs.Empty);

            // TAF: Démarrer le décompte avant de rendre le contrôle aux persos
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelée à chaque seconde
        /// </summary>
        private void OnTick()
        {
            _vm.OnTick();
            _setDurationField.SetText(_vm.SetDuration.ToString("0:00"));

            if (_vm.MatchTimer <= _vm.MatchDuration)
                _matchDurationField.SetText(_vm.MatchTimer.ToString("0:00"));
            else
            {
                _matchDurationField.SetText($"{_vm.MatchSettingsData.HalfDuration:0:00} (+{_vm.OvertimeDuration:0:00})");
            }
        }

        /// <summary>
        /// Appelée quand on atteint la mi-temps
        /// </summary>
        private void OnHalfTimeReached()
        {
            _vm.PauseMatch();
            EndSet(_vm.GetSetWinningTeam());

            print("mi-temps, changement de persos");

            OnHalfTimeReachedEvent?.Invoke(this, EventArgs.Empty);

            // TAF: Implémenter l'écran de changement des membres de l'équipe du joueur
            // et retirer le ResumeMatch pour les tests

            _vm.ResumeMatch();
        }

        /// <summary>
        /// Appelée quand un perso est éliminé
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnCharacterEliminated(object _, CharacterEliminatedEventArgs e)
        {
            _vm.OnCharacterEliminated(e.CharacterIsAlly);

            if (_vm.SuddenDeath)
            {
                // Si on est en mort subite, le 1er perso éliminé
                // détermine l'issue de la manche
                switch (e.CharacterIsAlly)
                {
                    case true:
                        EndSet(TeamID.Enemy);
                        break;
                    case false:
                        EndSet(TeamID.Ally);
                        break;
                }
            }
            else
            {
                EndSet(_vm.GetSetWinningTeam());
            }
        }

        /// <summary>
        /// Met fin à la manche
        /// </summary>
        /// <param name="setWinningTeamID">ID de l'équipe gagnante de la manche</param>
        private void EndSet(TeamID setWinningTeamID)
        {
            _vm.EndSet(setWinningTeamID);

            _alliesScoreField.SetText(_vm.AlliesScore.ToString());
            _enemiesScoreField.SetText(_vm.EnemiesScore.ToString());

            switch (setWinningTeamID)
            {
                case TeamID.Ally:
                    print("Set victoire alliée");
                    break;
                case TeamID.Enemy:
                    print("Set victoire ennemie");
                    break;
                case TeamID.None:
                    print("Set nul");
                    break;
            }

            OnSetEndedEvent?.Invoke(this, setWinningTeamID);

            if (_vm.MatchTimer < _vm.MatchDuration)
            {
                // S'il reste du temps, on lance une nouvelle manche
                StartNewSet();
            }
            else
            {
                // Si le temps est écoulé, on détermine l'équipe gagnante

                TeamID matchWinningTeamID = _vm.GetMatchWinningTeam();
                if (matchWinningTeamID != TeamID.None || _vm.SuddenDeath)
                {
                    // Si une équipe a plus de points ou si c'est la dernière manche (mort subite), on arrête le match.
                    EndMatch(matchWinningTeamID);
                }
                else
                {
                    // Match nul, on lance une dernière manche en mort subite
                    StartNewSet(true);
                }
            }
        }

        /// <summary>
        /// Met fin à la partie
        /// </summary>
        /// <param name="matchWinningTeamID">ID de l'équipe gagnante de la manche</param>
        private void EndMatch(TeamID matchWinningTeamID)
        {
            _vm.EndMatch();

            switch (matchWinningTeamID)
            {
                case TeamID.Ally:
                    print("Match victoire alliée");
                    break;
                case TeamID.Enemy:
                    print("Match victoire ennemie");
                    break;
                case TeamID.None:
                    print("Match nul");
                    break;
            }

            OnMatchEndedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}