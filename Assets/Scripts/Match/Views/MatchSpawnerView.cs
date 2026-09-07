using System;
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
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<Transform> AlliesT => _vm.AlliesT;

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<Transform> EnemiesT => _vm.EnemiesT;

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<Transform> BallsT => _vm.BallsT;

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Material par défaut des ballons")]
        private Material _defaultBallMaterial;

        [SerializeField]
        [Tooltip("Material par défaut des ballons en mort subite")]
        private Material _defaultBallMaterialSuddenDeath;

        #endregion

        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchSpawnerViewModel _vm;

        /// <summary>
        /// Le MatchManagerView
        /// </summary>
        private MatchManagerView _matchV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchSpawnerViewModel>();
            _matchV = FindAnyObjectByType<MatchManagerView>();
            _matchV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent += OnNewSetStarted;
            _matchV.OnMatchEndedEvent += OnMatchEnded;
            _matchV.OnHalfTimeEndedEvent += OnHalfTimeEnded;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent -= OnNewSetStarted;
            _matchV.OnMatchEndedEvent -= OnMatchEnded;
            _matchV.OnHalfTimeEndedEvent -= OnHalfTimeEnded;
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnNewMatchStarted(object _, NewMatchStartedEventArgs e)
        {
            CleanupField();
            _vm.SpawnPlayers(e.MatchSettings.NbAllies, e.MatchSettings.NbEnemies);
            _vm.SpawnBalls(e.MatchSettings.NbBalls);
            _vm.SetPlayersSkins(e.AllyTeam.CompositionData.MainCharacters, e.EnemyTeam.CompositionData.MainCharacters);
            _vm.SetBallsSkins(e.AllyTeam.TeamData.BallMaterial, e.EnemyTeam.TeamData.BallMaterial, _defaultBallMaterial);
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnNewSetStarted(object _, NewSetStartedEventArgs e)
        {
            _vm.ResetEntitiesPoses();

            if (e.SuddenDeath)
            {
                _vm.SetBallsSkins(e.AllyTeamData.BallMaterialSuddenDeath, e.EnemyTeamData.BallMaterialSuddenDeath, _defaultBallMaterialSuddenDeath);
            }
        }

        /// <summary>
        /// Appelée quand une partie est terminée
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnMatchEnded(object _, EventArgs e)
        {
            CleanupField();
        }

        /// <summary>
        /// Appelée quand la mi-temps est terminée
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnHalfTimeEnded(object _, HalfTimeEndedEventArgs e)
        {
            // On recrée entièrement les persos
            // car leur nombre peut avoir changé lors de la mi-temps

            _vm.DisableActivePlayers();
            _vm.SpawnPlayers(e.MatchSettings.NbAllies, e.MatchSettings.NbEnemies);
            _vm.SetPlayersSkins(e.AllyTeamComposition.MainCharacters, e.EnemyTeamComposition.MainCharacters);
        }

        /// <summary>
        /// Nettoie le terrain si on a déjà lancé un match
        /// </summary>
        private void CleanupField()
        {
            if (_vm.AlliesT != null)
            {
                _vm.DisableActivePlayers();
                _vm.DisableActiveBalls();
            }
        }

        #endregion
    }
}