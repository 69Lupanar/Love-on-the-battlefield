using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des ballons
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchBallControllerViewModel))]
    public class MatchBallControllerView : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<BallView> Balls { get; private set; } = new();

        #endregion

        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchBallControllerViewModel _vm;

        /// <summary>
        /// Le MatchManagerView
        /// </summary>
        private MatchManagerView _matchV;

        /// <summary>
        /// Le MatchSpawnerView
        /// </summary>
        private MatchSpawnerView _spawnerV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchBallControllerViewModel>();
            _matchV = FindAnyObjectByType<MatchManagerView>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchV.OnNewMatchStarted += OnNewMatchStarted;
            _matchV.OnNewSetStarted += OnNewSetStarted;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStarted -= OnNewMatchStarted;
            _matchV.OnNewSetStarted -= OnNewSetStarted;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_matchV.MatchIsOver)
                return;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void FixedUpdate()
        {
            if (_matchV.MatchIsOver)
                return;

            // TAF : Gérer les retours de OnCollision/TriggerEnter
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Attache les callbacks aux entités de jeu
        /// </summary>
        internal void SubscribeEntities()
        {

        }

        /// <summary>
        /// Détache les callbacks aux entités de jeu
        /// </summary>
        internal void UnsubscribeEntities()
        {

        }

        /// <summary>
        /// Assigne les persos et ballons
        /// </summary>
        /// <param name="ballsT">Transforms des ballons</param>
        internal void SetEntities(List<Transform> ballsT)
        {
            _vm.SetEntities(ballsT.Count);

            Balls.Clear();

            for (int i = 0; i < ballsT.Count; ++i)
            {
                Balls.Add(ballsT[i].GetComponent<BallView>());
            }
        }

        /// <summary>
        /// Réinitialise les données du contrôleur pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            _vm.ResetController();

            for (int i = 0; i < Balls.Count; ++i)
            {
                Balls[i].ResetBall(i, Balls.Count);
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        private void OnNewMatchStarted(MatchSettingsData _)
        {
            if (Balls != null)
            {
                // Détache les callbacks des anciennes instances
                UnsubscribeEntities();
            }

            SetEntities(_spawnerV.BallsT);
            SubscribeEntities();
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        private void OnNewSetStarted()
        {
            ResetController();
        }

        #endregion
    }
}