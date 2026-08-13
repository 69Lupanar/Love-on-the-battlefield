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
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent += OnNewSetStarted;
            _matchV.OnMatchEndedEvent += OnMatchEnded;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent -= OnNewSetStarted;
            _matchV.OnMatchEndedEvent -= OnMatchEnded;
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        private void OnNewMatchStarted(object _, MatchSettingsData matchSettings)
        {
            CleanupField();
            _vm.SpawnPlayersAndBalls(matchSettings.NbAllies, matchSettings.NbEnemies, matchSettings.NbBalls);
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        private void OnNewSetStarted(object _, EventArgs e)
        {
            _vm.ResetEntitiesPoses();
        }

        /// <summary>
        /// Appelée quand un match est terminé
        /// </summary>
        /// <param name="e">Données de l'événement</param>
        private void OnMatchEnded(object sender, TeamID e)
        {
            CleanupField();
        }

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

        #endregion
    }
}