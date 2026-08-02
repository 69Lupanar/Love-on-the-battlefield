using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement des joueurs et ballons
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchPlayerControllerViewModel))]
    public class MatchPlayerControllerView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelée quand le joueur actif change
        /// </summary>
        internal Action<int> OnActivePlayerChanged { get; set; }

        #endregion

        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<MatchCharacterController> Allies => _vm.Allies;

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterController> Enemies => _vm.Enemies;

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<Ball> Balls => _vm.Balls;

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        internal int ActivePlayerIndex
        {
            get => _vm.ActivePlayerIndex;
            set
            {
                if (_vm.ActivePlayerIndex != value)
                {
                    _vm.ActivePlayerIndex = value;
                    OnActivePlayerChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// L'ID du perso allié actuellement sélectionné comme cible
        /// lors du changement de contrôle
        /// (-1 si aucun)
        /// </summary>
        internal int CurAllyTargetForSwapIndex => _vm.CurAllyTargetForSwapIndex;

        #endregion

        #region Inspecteur

        [Header("Swap Characters")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Longueur du raycast du changement de contrôle")]
        private float _swapCharacterSpherecastLength = 20f;

        [SerializeField]
        [Tooltip("Distance d'un allié au rayon pour que celui-ci soit considéré éligible pour le changement de contrôle")]
        private float _swapCharacterSpherecastRadius = 1f;

        [SerializeField]
        [Tooltip("Layermask utilisé pour le changement de contrôle")]
        private LayerMask _swapCharacterLayerMask;

        #endregion

        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchPlayerControllerViewModel _vm;

        /// <summary>
        /// Commandes du joueur
        /// </summary>
        private MatchPlayerInput _playerInput;

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
            _vm = GetComponent<MatchPlayerControllerViewModel>();
            _playerInput = GetComponent<MatchPlayerInput>();
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

            for (int i = 0; i < Allies.Count; ++i)
            {
                _vm.ComputeCommonInput(Allies[i]);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                _vm.ComputeCommonInput(Enemies[i]);
            }

            _vm.ComputePlayerInput(Allies[ActivePlayerIndex], _playerInput, _swapCharacterSpherecastLength, _swapCharacterSpherecastRadius, _swapCharacterLayerMask);
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void FixedUpdate()
        {
            if (_matchV.MatchIsOver)
                return;

            for (int i = 0; i < Allies.Count; ++i)
            {
                _vm.ComputeCommonInputFixed(Allies[i]);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                _vm.ComputeCommonInputFixed(Enemies[i]);
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        private void OnNewMatchStarted(MatchSettingsData _)
        {
            if (Allies != null)
            {
                // Désactive les inputs des joueurs déjà présents avant de les retirer
                EnablePlayersInput(false);
            }

            SetEntities(_spawnerV.AlliesT, _spawnerV.EnemiesT, _spawnerV.BallsT);
            SetTeams();
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        private void OnNewSetStarted()
        {
            ResetController();
            SetActivePlayer(ActivePlayerIndex);
            EnablePlayersInput(false);

            // A retirer une fois les tests finis
            EnablePlayersInput(true);
        }

        /// <summary>
        /// Attache les callbacks aux entités de jeu
        /// </summary>
        internal void SubscribeEntities()
        {

        }

        /// <summary>
        /// Attache les callbacks aux entités de jeu
        /// </summary>
        internal void UnsubscribeEntities()
        {

        }

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        internal void SetActivePlayer(int index)
        {
            _vm.SetActivePlayer(index);
        }

        /// <summary>
        /// Désactive les joueurs et ballons actifs
        /// </summary>
        internal void EnablePlayersInput(bool enable)
        {
            _vm.EnablePlayersInput(enable);
        }

        /// <summary>
        /// Assigne les persos et ballons
        /// </summary>
        /// <param name="alliesT">Transforms des persos alliés</param>
        /// <param name="enemiesT">Transforms des persos ennemis</param>
        /// <param name="ballsT">Transforms des ballons</param>
        internal void SetEntities(List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT)
        {
            _vm.SetEntities(alliesT, enemiesT, ballsT);
        }

        /// <summary>
        /// Assigne les équipes à chaque perso
        /// </summary>
        internal void SetTeams()
        {
            _vm.SetTeams();
        }

        /// <summary>
        /// Réinitialise les données du contrôleur pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            _vm.ResetController();
        }

        #endregion

    }
}