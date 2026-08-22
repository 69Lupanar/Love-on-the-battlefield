using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des joueurs et ballons
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchCharacterManagerViewModel))]
    public class MatchCharacterManagerView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelée quand le joueur actif change
        /// </summary>
        internal EventHandler<int> OnActivePlayerChanged { get; set; }

        /// <summary>
        /// Appelée quand un ballon est ramassé par un joueur
        /// </summary>
        internal EventHandler<BallPickedUpEventArgs> OnBallPickedUpEvent { get; set; }

        /// <summary>
        /// Appelée quand un ballon est lancé par un joueur
        /// </summary>
        internal EventHandler<ShootEventArgs> OnShootEvent { get; set; }

        /// <summary>
        /// Appelée quand un perso est éliminé
        /// </summary>
        internal EventHandler<CharacterEliminatedEventArgs> OnCharacterEliminatedEvent { get; set; }

        #endregion

        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterControllerView> Allies => _allies.AsReadOnly();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterControllerView> Enemies => _enemies.AsReadOnly();

        /// <summary>
        /// Les données de l'état des persos du joueur
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterState> AllyStates => _vm.AllyStates.AsReadOnly();

        /// <summary>
        /// Les données de l'état des persos ennemis
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterState> EnemyStates => _vm.EnemyStates.AsReadOnly();

        /// <summary>
        /// Les données de mouvement des persos du joueur
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterMovementData> AllyMovementDatas => _vm.AllyMovementDatas.AsReadOnly();

        /// <summary>
        /// Les données de mouvement des persos ennemis
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterMovementData> EnemyMovementDatas => _vm.EnemyMovementDatas.AsReadOnly();

        /// <summary>
        /// true si le joueur est en cours de changement de personnage
        /// </summary>
        internal bool IsSwappingCharacter
        {
            get => _vm.IsSwappingCharacter;
            private set => _vm.IsSwappingCharacter = value;
        }

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        internal int ActivePlayerIndex => _vm.ActivePlayerIndex;

        /// <summary>
        /// L'ID du perso allié actuellement sélectionné comme cible
        /// lors du changement de contrôle
        /// (-1 si aucun)
        /// </summary>
        internal int CurAllyTargetForSwapIndex
        {
            get => _vm.CurAllyTargetForSwapIndex;
            private set => _vm.CurAllyTargetForSwapIndex = value;
        }

        #endregion

        #region Inspecteur

        [Header("Swap Characters")]
        [Space(10)]

        [SerializeField]
        [Tooltip("True si le perso contrôlé par le joueur doit changer après une passe")]
        private bool _shouldSwapControlAfterPass = true;

        [SerializeField]
        [Tooltip("Longueur du raycast du changement de contrôle")]
        private float _swapCharacterSpherecastLength = 20f;

        [SerializeField]
        [Tooltip("Distance d'un allié au rayon pour que celui-ci soit considéré éligible pour le changement de contrôle")]
        private float _swapCharacterSpherecastRadius = 1f;

        [SerializeField]
        [Tooltip("Layermask utilisé pour le changement de contrôle")]
        private LayerMask _swapCharacterLayerMask;

        [Space(10)]
        [Header("Elimination")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Espacement entre les persos d'une queue")]
        private float _charactersInQueueSpacing = 1.5f;

        [SerializeField]
        [Tooltip("La Transform du début de la file pour les alliés éliminés")]
        private Transform _eliminatedAlliesQueueT;

        [SerializeField]
        [Tooltip("La Transform du début de la file pour les ennemis éliminés")]
        private Transform _eliminatedEnemiesQueueT;

        [Space(10)]
        [Header("Physics")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Tag du ballon")]
        private string _ballTag;

        [SerializeField]
        [Tooltip("Valeurs minimales possibles de mouvement d'un personnage lors d'un match")]
        internal MatchCharacterMovementData _minBaseMovementData;

        [SerializeField]
        [Tooltip("Valeurs maximales possibles de mouvement d'un personnage lors d'un match")]
        internal MatchCharacterMovementData _maxBaseMovementData;

        #endregion

        #region Instance

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        private readonly List<MatchCharacterControllerView> _allies = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        private readonly List<MatchCharacterControllerView> _enemies = new();

        /// <summary>
        /// Les persos du joueur éliminés
        /// </summary>
        private readonly Queue<MatchCharacterControllerView> _eliminatedAllies = new();

        /// <summary>
        /// Les persos ennemis éliminés
        /// </summary>
        private readonly Queue<MatchCharacterControllerView> _eliminatedEnemies = new();

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchCharacterManagerViewModel _vm;

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

        /// <summary>
        /// Le MatchBallManagerView
        /// </summary>
        private MatchBallManagerView _ballManagerV;

        /// <summary>
        /// la dernière position du joystick droit
        /// </summary>
        private Vector2 _lastSwapCharacterAxis;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchCharacterManagerViewModel>();
            _playerInput = GetComponent<MatchPlayerInput>();
            _matchV = FindAnyObjectByType<MatchManagerView>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
            _ballManagerV = FindAnyObjectByType<MatchBallManagerView>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent += OnNewSetStarted;
            _matchV.OnSetEndedEvent += OnSetEnded;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent -= OnNewSetStarted;
            _matchV.OnSetEndedEvent -= OnSetEnded;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_matchV.MatchIsOngoing)
            {
                for (int i = 0; i < Allies.Count; ++i)
                {
                    ComputeCommonInput(i, true, Allies[i], AllyMovementDatas[i]);
                }

                for (int i = 0; i < Enemies.Count; ++i)
                {
                    ComputeCommonInput(i, false, Enemies[i], EnemyMovementDatas[i]);
                }

                ComputePlayerInput(Allies[ActivePlayerIndex], _playerInput, _swapCharacterSpherecastLength, _swapCharacterSpherecastRadius, _swapCharacterLayerMask);
            }
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void FixedUpdate()
        {
            if (_matchV.MatchIsOngoing)
            {
                for (int i = 0; i < Allies.Count; ++i)
                {
                    ComputeCommonInputFixed(Allies[i], AllyMovementDatas[i]);
                }

                for (int i = 0; i < Enemies.Count; ++i)
                {
                    ComputeCommonInputFixed(Enemies[i], EnemyMovementDatas[i]);
                }
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Attache les callbacks aux entités de jeu
        /// </summary>
        internal void SubscribeEntities()
        {
            for (int i = 0; i < Allies.Count; ++i)
            {
                MatchCharacterControllerView ally = Allies[i];
                ally.OnCollisionEnterEvent += OnCharacterCollisionEnter;
                ally.OnTriggerEnterEvent += OnCharacterTriggerEnter;
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                MatchCharacterControllerView enemy = Enemies[i];
                enemy.OnCollisionEnterEvent += OnCharacterCollisionEnter;
                enemy.OnTriggerEnterEvent += OnCharacterTriggerEnter;
            }
        }

        /// <summary>
        /// Détache les callbacks aux entités de jeu
        /// </summary>
        internal void UnsubscribeEntities()
        {
            for (int i = 0; i < Allies.Count; ++i)
            {
                MatchCharacterControllerView ally = Allies[i];
                ally.OnCollisionEnterEvent -= OnCharacterCollisionEnter;
                ally.OnTriggerEnterEvent -= OnCharacterTriggerEnter;
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                MatchCharacterControllerView enemy = Enemies[i];
                enemy.OnCollisionEnterEvent -= OnCharacterCollisionEnter;
                enemy.OnTriggerEnterEvent -= OnCharacterTriggerEnter;
            }
        }

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        internal void SetActivePlayer(int index)
        {
            for (int i = 0; i < Allies.Count; ++i)
            {
                if (i == index)
                {
                    Allies[i].GiveControlToPlayer();
                    Allies[i].DislayHalo(true);
                }
                else
                {
                    Allies[i].GiveControlToAI();
                    Allies[i].HideHalo();
                }
            }
        }

        /// <summary>
        /// Désactive les joueurs et ballons actifs
        /// </summary>
        internal void EnablePlayersInput(bool enable)
        {
            for (int i = 0; i < Allies.Count; ++i)
            {
                Allies[i].EnableInput(enable);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].EnableInput(enable);
            }
        }

        /// <summary>
        /// Assigne les persos
        /// </summary>
        /// <param name="alliesT">Transforms des persos alliés</param>
        /// <param name="enemiesT">Transforms des persos ennemis</param>
        /// <param name="allyTeamComposition">Composition de joueurs de l'équipe alliée</param>
        /// <param name="enemyTeamComposition">Composition de joueurs de l'équipe ennemie</param>
        internal void SetEntities(List<Transform> alliesT, List<Transform> enemiesT, List<CharacterData> allyTeamComposition, List<CharacterData> enemyTeamComposition)
        {
            _vm.SetEntities(alliesT.Count, enemiesT.Count, _minBaseMovementData, _maxBaseMovementData, allyTeamComposition, enemyTeamComposition);

            _allies.Clear();
            _enemies.Clear();

            for (int i = 0; i < alliesT.Count; ++i)
            {
                _allies.Add(alliesT[i].GetComponent<MatchCharacterControllerView>());
            }

            for (int i = 0; i < enemiesT.Count; ++i)
            {
                _enemies.Add(enemiesT[i].GetComponent<MatchCharacterControllerView>());
            }
        }

        /// <summary>
        /// Assigne les équipes à chaque perso
        /// </summary>
        internal void SetTeams()
        {
            _vm.SetTeams();

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].GiveControlToAI();
            }
        }

        /// <summary>
        /// Réinitialise les données du gestionnaire pour une nouvelle manche
        /// </summary>
        internal void ResetManager()
        {
            _vm.ResetManager();

            for (int i = 0; i < Allies.Count; ++i)
            {
                Allies[i].ResetPlayer();
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].ResetPlayer();
            }
        }

        #endregion

        #region Méthodes privées

        #region Callbacks

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        private void OnNewMatchStarted(object _, NewMatchStartedEventArgs e)
        {
            // Désactive les inputs des joueurs déjà présents avant de les retirer
            EnablePlayersInput(false);

            // Détache les callbacks des anciennes instances
            UnsubscribeEntities();

            SetEntities(_spawnerV.AlliesT, _spawnerV.EnemiesT, e.AllyTeamComposition.MainCharacters, e.EnemyTeamComposition.MainCharacters);
            SetTeams();
            SubscribeEntities();
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        private void OnNewSetStarted(object _, EventArgs e)
        {
            ResetManager();
            SetActivePlayer(ActivePlayerIndex);
            EnablePlayersInput(false);

            // A retirer une fois les tests finis
            EnablePlayersInput(true);
        }

        /// <summary>
        /// Appelée quand une manche est terminée
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e">Données de l'événement</param>
        private void OnSetEnded(object _, TeamID e)
        {
            EnablePlayersInput(false);
        }

        /// <summary>
        /// Appelée quand le perso entre en collision avec un ballon
        /// </summary>
        /// <param name="sender">Le perso</param>
        /// <param name="ball">Le ballon</param>
        private void OnCharacterCollisionEnter(object sender, Collision other)
        {
            if (!other.gameObject.CompareTag(_ballTag) || !_matchV.MatchIsOngoing)
            {
                return;
            }

            BallView ball = other.gameObject.GetComponent<BallView>();
            int ballIndex = _ballManagerV.Balls.IndexOf(ball);
            BallState ballState = _ballManagerV.BallStates[ballIndex];

            if (ballState.IsLive)
            {
                MatchCharacterControllerView character = sender as MatchCharacterControllerView;
                bool characterIsAlly = Allies.Contains(character);
                int characterIndex = characterIsAlly ? Allies.IndexOf(character) : Enemies.IndexOf(character);
                MatchCharacterState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];

                if (characterState.IsEliminated)
                    return;

                switch (ballState.ActiveTeamID)
                {
                    case TeamID.Ally:
                        if (!characterState.IsAlly)
                        {
                            // TAF : Balle adverse, le perso est éliminé
                            EliminateCharacter(characterIndex, characterIsAlly);
                        }
                        break;

                    case TeamID.Enemy:
                        if (characterState.IsAlly)
                        {
                            // TAF : Balle adverse, le perso est éliminé
                            EliminateCharacter(characterIndex, characterIsAlly);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Appelée quand le perso entre en collision avec un ballon
        /// </summary>
        /// <param name="sender">Le perso</param>
        /// <param name="ball">Le ballon</param>
        private void OnCharacterTriggerEnter(object sender, Collider other)
        {
            if (!other.CompareTag(_ballTag) || !_matchV.MatchIsOngoing)
            {
                return;
            }

            BallView ball = other.GetComponent<BallView>();
            MatchCharacterControllerView character = sender as MatchCharacterControllerView;
            bool characterIsAlly = Allies.Contains(character);
            int characterIndex = characterIsAlly ? Allies.IndexOf(character) : Enemies.IndexOf(character);
            int ballIndex = _ballManagerV.Balls.IndexOf(ball);
            MatchCharacterState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];
            BallState ballState = _ballManagerV.BallStates[ballIndex];

            // Si le perso détient déjà un ballon
            // ou qu'il tente de récupérer une balle réservée à l'ennemi,
            // il ne peut pas en ramasser une nouvelle

            if (characterState.IsEliminated || characterState.IsHoldingABall || (characterState.IsAlly && ballState.ReservedTeamID == TeamID.Enemy) || (!characterState.IsAlly && ballState.ReservedTeamID == TeamID.Ally))
                return;

            if (!ballState.IsLive)
            {
                PickUpBall(characterIndex, ballIndex, character, ball, in characterState);
            }
            else
            {
                switch (ballState.ActiveTeamID)
                {
                    case TeamID.Ally:
                        if (characterState.IsAlly)
                        {
                            // Balle alliée, c'est une passe donc le perso la récupère
                            PickUpBall(characterIndex, ballIndex, character, ball, in characterState);

                            if (_shouldSwapControlAfterPass && !IsSwappingCharacter && ballState.LastHoldingPlayerID == ActivePlayerIndex)
                            {
                                // Si le lanceur est le joueur,
                                // on change le contrôle des persos pour contrôler le receveur
                                SwapControl(ActivePlayerIndex, characterIndex);
                                SetActivePlayer(ActivePlayerIndex);
                                LockInputs(character.ActiveInput);
                            }
                        }
                        break;

                    case TeamID.Enemy:
                        if (!characterState.IsAlly)
                        {
                            // Balle alliée, c'est une passe donc le perso la récupère
                            PickUpBall(characterIndex, ballIndex, character, ball, in characterState);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Inputs

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="characterIndex">ID du perso</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        /// <param name="character">Le perso</param>
        /// <param name="movementData">Données de mouvement</param>
        private void ComputeCommonInput(int characterIndex, bool characterIsAlly, MatchCharacterControllerView character, MatchCharacterMovementData movementData)
        {
            IMatchCharacterInput activeInput = character.ActiveInput;

            // Translation + Rotation
            if (activeInput.MoveAxis != Vector2.zero)
            {
                character.RotateMesh(activeInput.MoveAxis);
            }

            // Changement de cible
            int previousTargetIndex = characterIsAlly ? AllyStates[characterIndex].OpponentTargetIndex : EnemyStates[characterIndex].OpponentTargetIndex;

            if (activeInput.PreviousTargetTrigger)
            {
                SelectNewOpponentTarget(characterIndex, -1, previousTargetIndex, characterIsAlly);
            }
            if (activeInput.NextTargetTrigger)
            {
                SelectNewOpponentTarget(characterIndex, 1, previousTargetIndex, characterIsAlly);
            }

            // L'état du perso a été màj, on doit le récupérer à nouveau
            MatchCharacterState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];

            // Tir
            if (characterState.IsHoldingABall)
            {
                if (activeInput.IsHoldingFire)
                {
                    _vm.ChargeShot(characterIndex, characterState.IsAlly, movementData.FireChargeSpeed, Time.deltaTime);
                }
                if (activeInput.HasReleasedFire && characterState.Energy < 1f)
                {
                    Shoot(characterIndex, character, characterState, movementData);
                }
            }
        }

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="characterView">Le perso</param>
        /// <param name="characterState">L'état du perso</param>
        /// <param name="movementData">Les données de mouvement du joueur</param>
        private void ComputeCommonInputFixed(MatchCharacterControllerView characterView, MatchCharacterMovementData movementData)
        {
            IMatchCharacterInput activeInput = characterView.ActiveInput;

            // Translation + Rotation
            if (activeInput.MoveAxis != Vector2.zero)
            {
                characterView.Move(activeInput.MoveAxis, movementData.MoveSpeed);
            }
        }

        /// <summary>
        /// Exécute les commandes du joueur
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="playerInput">Les commandes du joueur</param>
        /// <param name="swapCharacterSpherecastLength">Longueur du raycast du changement de contrôle</param>
        /// <param name="swapCharacterSpherecastRadius">Distance d'un allié au rayon pour que celui-ci soit considéré éligible pour le changement de contrôle</param>
        /// <param name="swapCharacterLayerMask">Layermask utilisé pour le changement de contrôle</param>
        private void ComputePlayerInput(MatchCharacterControllerView activePlayer, MatchPlayerInput playerInput, float swapCharacterSpherecastLength, float swapCharacterSpherecastRadius, LayerMask swapCharacterLayerMask)
        {
            if (!playerInput.IsHoldingFire && _lastSwapCharacterAxis == Vector2.zero && playerInput.SwapCharacterAxis != Vector2.zero)
            {
                IsSwappingCharacter = true;
            }

            // Changement d'allié contrôlé par le joueur
            if (IsSwappingCharacter)
            {
                int targetIndex = GetClosestAllyInDirection(activePlayer, playerInput.SwapCharacterAxis, swapCharacterSpherecastLength, swapCharacterSpherecastRadius, swapCharacterLayerMask);

                if (targetIndex > -1)
                {
                    MatchCharacterControllerView target = Allies[targetIndex];
                    MatchCharacterState targetState = AllyStates[targetIndex];

                    // La cible n'est valide que si elle est alliée
                    // et qu'elle ne charge pas de tir
                    if (targetState.IsAlly && !target.ActiveInput.IsHoldingFire)
                    {
                        if (CurAllyTargetForSwapIndex > -1)
                        {
                            // Fait disparaître le halo de la cible précédente
                            Allies[CurAllyTargetForSwapIndex].HideHalo();
                        }

                        //Affiche le halo de la nouvelle cible
                        target.DislayHalo(true);

                        CurAllyTargetForSwapIndex = targetIndex;
                    }
                }
            }

            // On passe le contrôle à l'allié sélectionné
            if (IsSwappingCharacter && CurAllyTargetForSwapIndex > -1 && playerInput.FireTrigger)
            {
                SwapControl(ActivePlayerIndex, CurAllyTargetForSwapIndex);
                SetActivePlayer(ActivePlayerIndex);
                LockInputs(playerInput);
            }

            if (playerInput.CancelTargetTrigger)
            {

                if (IsSwappingCharacter)
                {
                    // On annule le changement de perso
                    CancelSwap();
                }
                else
                {
                    // On retire au joueur sa cible
                    ClearTarget(ActivePlayerIndex, true);
                }
            }

            _lastSwapCharacterAxis = playerInput.SwapCharacterAxis;
        }

        /// <summary>
        /// Pour empêcher le nouveau perso d'agir à la même frame où il devient actif
        /// </summary>
        /// <param name="playerInput">Les commandes du joueur</param>
        private static void LockInputs(IMatchCharacterInput playerInput)
        {
            // TAF : Bloquer aussi les commandes pour le saut, l'esquive et le blocage
            playerInput.IsHoldingFire = false;
            playerInput.HasReleasedFire = false;
        }

        #endregion

        /// <summary>
        /// Sélectionne une nouvelle cible pour le perso
        /// </summary>
        /// <param name="characterIndex">L'ID du perso</param>
        /// <param name="increment">Position si suivant, négatif si précédent</param>
        /// <param name="previousTargetIndex">L'ID de la cible précédente</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        private void SelectNewOpponentTarget(int characterIndex, int increment, int previousTargetIndex, bool characterIsAlly)
        {
            _vm.SelectNewOpponentTarget(characterIndex, increment, characterIsAlly);

            // Si le perso est celui du joueur, on affiche le halo de sa cible parmi les ennemis
            // et on masque celle de la cible précédente s'il y en a une
            if (characterIsAlly && characterIndex == ActivePlayerIndex)
            {
                if (previousTargetIndex > -1)
                {
                    Enemies[previousTargetIndex].HideHalo();
                }

                Enemies[AllyStates[characterIndex].OpponentTargetIndex].DislayHalo(true);
            }
        }

        /// <summary>
        /// Trace une ligne et récupère l'allié le plus proche de celle-ci
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="swapCharacterAxis">La direction de la ligne sur l'axe XZ</param>
        /// <param name="swapCharacterSpherecastLength">Longueur du raycast du changement de contrôle</param>
        /// <param name="swapCharacterSpherecastRadius">Distance d'un allié au rayon pour que celui-ci soit considéré éligible pour le changement de contrôle</param>
        /// <param name="swapCharacterLayerMask">Layermask utilisé pour le changement de contrôle</param>
        /// <returns>L'ID de l'allié le plus proche du joueur</returns>
        private int GetClosestAllyInDirection(MatchCharacterControllerView activePlayer, Vector2 swapCharacterAxis, float swapCharacterSpherecastLength, float swapCharacterSpherecastRadius, LayerMask swapCharacterLayerMask)
        {
            _vm.GetSwapDirectionXZ(activePlayer.transform.position, swapCharacterAxis, out Vector3 origin, out Vector3 dir);

            if (Physics.SphereCast(origin, swapCharacterSpherecastRadius, dir, out RaycastHit hit, swapCharacterSpherecastLength, swapCharacterLayerMask))
            {
                return Allies.IndexOf(hit.collider.GetComponent<MatchCharacterControllerView>());
            }

            return -1;
        }

        /// <summary>
        /// Echange le contrôle d'alliés entre l'IA et le joueur
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="target">La cible à contrôler</param>
        private void SwapControl(int previousActivePlayerIndex, int newActivePlayerIndex)
        {
            // On masque le halo de la cible ennemie s'il y en avait une
            MatchCharacterState previousActivePlayerState = AllyStates[previousActivePlayerIndex];

            if (previousActivePlayerState.OpponentTargetIndex > -1)
                Enemies[previousActivePlayerState.OpponentTargetIndex].HideHalo();

            // On passe le contrôle à l'allié sélectionné
            _vm.SwapControl(newActivePlayerIndex);

            // On active le halo de la nouvelle cible s'il y en a une, vu qu'on change de perso

            MatchCharacterState targetState = AllyStates[newActivePlayerIndex];

            if (targetState.OpponentTargetIndex > -1)
                Enemies[targetState.OpponentTargetIndex].DislayHalo(false);

            OnActivePlayerChanged?.Invoke(this, newActivePlayerIndex);
        }

        /// <summary>
        /// Annule le changement de perso
        /// </summary>
        private void CancelSwap()
        {
            if (CurAllyTargetForSwapIndex > -1)
            {
                // Fait disparaître le halo de la cible précédente
                Allies[CurAllyTargetForSwapIndex].HideHalo();
            }

            _vm.CancelSwap();
        }

        /// <summary>
        /// Retire au perso les infos sur sa cible. Utilisée quand la cible se fait éliminer
        /// </summary>
        /// <param name="characterIndex">L'ID perso concerné</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        private void ClearTarget(int characterIndex, bool characterIsAlly)
        {
            MatchCharacterState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];

            // Si le perso est celui du joueur, on masque le halo de sa cible
            if (characterState.OpponentTargetIndex > -1 && characterIsAlly && characterIndex == ActivePlayerIndex)
            {
                Enemies[characterState.OpponentTargetIndex].HideHalo();
            }

            _vm.ClearTarget(characterIndex, characterIsAlly);
        }

        /// <summary>
        /// Eliminer un personnage
        /// </summary>
        /// <param name="characterIndex">ID du perso</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        private void EliminateCharacter(int characterIndex, bool characterIsAlly)
        {
            _vm.EliminateCharacter(characterIndex, characterIsAlly);

            if (characterIsAlly)
            {
                // Si le perso éjecté est le joueur et qu'il n'est pas le dernier éliminé,
                // on le fait changer de perso à contrôler

                if (characterIndex == ActivePlayerIndex && GetNextLivePlayer(ActivePlayerIndex, out int nextPlayerIndex))
                {
                    SwapControl(ActivePlayerIndex, nextPlayerIndex);
                }

                // On désactive le perso et on le fait lâcher son ballon
                MatchCharacterControllerView character = Allies[characterIndex];
                character.EnableInput(false);
                character.EnablePhysics(false);

                if (AllyStates[characterIndex].IsHoldingABall)
                    ReleaseBall(character, characterIndex, characterIsAlly);

                // Efface les cibles de tous les persos ayant ciblé celui-ci
                for (int i = 0; i < EnemyStates.Count; ++i)
                {
                    if (EnemyStates[i].OpponentTargetIndex == characterIndex)
                    {
                        ClearTarget(i, false);
                    }
                }

                // Déplace le perso vers la queue des éliminés
                _eliminatedAllies.Enqueue(character);
                character.transform.SetParent(_eliminatedAlliesQueueT);
                DisplayEliminatedCharacters(_eliminatedAlliesQueueT);
            }
            else
            {
                // On désactive le perso et on le fait lâcher son ballon
                MatchCharacterControllerView character = Enemies[characterIndex];
                character.EnableInput(false);
                character.EnablePhysics(false);

                if (EnemyStates[characterIndex].IsHoldingABall)
                    ReleaseBall(character, characterIndex, characterIsAlly);

                // Efface les cibles de tous les persos ayant ciblé celui-ci
                for (int i = 0; i < AllyStates.Count; ++i)
                {
                    if (AllyStates[i].OpponentTargetIndex == characterIndex)
                    {
                        ClearTarget(i, true);
                    }
                }

                // Déplace le perso vers la queue des éliminés
                _eliminatedEnemies.Enqueue(character);
                character.transform.SetParent(_eliminatedEnemiesQueueT);
                DisplayEliminatedCharacters(_eliminatedEnemiesQueueT);
            }


            OnCharacterEliminatedEvent?.Invoke(this, new CharacterEliminatedEventArgs(characterIsAlly));
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="characterIndex">L'ID du perso</param>
        /// <param name="ballIndex">L'ID du ballon</param>
        /// <param name="character">Le perso</param>
        /// <param name="ball">Le ballon</param>
        /// <param name="characterState">L'état du perso</param>
        private void PickUpBall(int characterIndex, int ballIndex, MatchCharacterControllerView character, BallView ball, in MatchCharacterState characterState)
        {
            _vm.PickUpBall(characterIndex, ballIndex, characterState.IsAlly);

            character.PickUpBall(ball);

            OnBallPickedUpEvent?.Invoke(null, new BallPickedUpEventArgs(characterIndex, characterState.IsAlly, ballIndex));
        }

        /// <summary>
        /// Tire le ballon
        /// </summary>
        /// <param name="characterIndex">ID du tireur</param>
        /// <param name="character">Le perso</param>
        /// <param name="characterState">Etat du tireur</param>
        /// <param name="movementData">Données de mouvement</param>
        private void Shoot(int characterIndex, MatchCharacterControllerView character, MatchCharacterState characterState, MatchCharacterMovementData movementData)
        {
            _vm.Shoot(characterIndex, characterState.IsAlly);
            character.Shoot(movementData.FireForceInterval, characterState.Energy);
            OnShootEvent?.Invoke(null, new ShootEventArgs(characterIndex, characterState.BallIndex));
        }

        /// <summary>
        /// Force le perso à lâcher le ballon
        /// </summary>
        /// <param name="character">Le perso</param>
        /// <param name="characterIndex">ID du perso</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        private void ReleaseBall(MatchCharacterControllerView character, int characterIndex, bool characterIsAlly)
        {
            _vm.ReleaseBall(characterIndex, characterIsAlly);
            character.ReleaseBall();
        }

        /// <summary>
        /// Obtient l'index du prochain perso allié contrôlable
        /// </summary>
        /// <param name="activePlayerIndex">ID du perso actuellement contrôlé par le joueur</param>
        /// <param name="nextPlayerIndex">ID du perso à contrôler</param>
        /// <returns>true si un autre allié a pu être trouvé</returns>
        private bool GetNextLivePlayer(int activePlayerIndex, out int nextPlayerIndex)
        {
            nextPlayerIndex = activePlayerIndex;

            for (int i = 0; i < Allies.Count - 1;)
            {
                nextPlayerIndex = nextPlayerIndex == AllyStates.Count - 1 ? 0 : nextPlayerIndex + 1;

                if (!AllyStates[nextPlayerIndex].IsEliminated)
                {
                    return true;
                }

                ++i;
            }

            return false;
        }

        /// <summary>
        /// Affiche les persos éliminés dans leur file respective
        /// </summary>
        /// <param name="queueT">La file parente des persos éliminés</param>
        private void DisplayEliminatedCharacters(Transform queueT)
        {
            for (int i = 0; i < queueT.childCount; ++i)
            {
                queueT.GetChild(i).SetPositionAndRotation(queueT.position + _charactersInQueueSpacing * i * -queueT.right, queueT.rotation);
            }
        }

        #endregion
    }
}