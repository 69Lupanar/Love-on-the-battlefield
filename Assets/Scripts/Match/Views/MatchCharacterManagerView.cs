using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        internal Action<int> OnActivePlayerChanged { get; set; }

        /// <summary>
        /// Appelée quand un ballon est ramassé par un joueur
        /// </summary>
        internal EventHandler<BallPickedUpEventArgs> OnBallPickedupEvent { get; set; }

        #endregion

        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<MatchCharacterControllerView> Allies { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterControllerView> Enemies { get; private set; } = new();

        /// <summary>
        /// Les données de l'état des persos du joueur
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterControllerState> AllyStates => _vm.AllyStates.AsReadOnly();

        /// <summary>
        /// Les données de l'état des persos ennemis
        /// </summary>
        internal ReadOnlyCollection<MatchCharacterControllerState> EnemyStates => _vm.EnemyStates.AsReadOnly();

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
        internal int ActivePlayerIndex
        {
            get => _vm.ActivePlayerIndex;
            private set
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
        internal int CurAllyTargetForSwapIndex
        {
            get => _vm.CurAllyTargetForSwapIndex;
            private set => _vm.CurAllyTargetForSwapIndex = value;
        }
        internal Action<object, BallPickedUpEventArgs> OnBallPickedUpEvent { get; set; }

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

        #endregion

        #region Instance

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
        /// la dernière cible du changement de contrôle
        /// </summary>
        private MatchCharacterControllerView _lastSwapCharacterTarget;

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
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent -= OnNewSetStarted;
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
                ComputeCommonInput(Allies[i]);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                ComputeCommonInput(Enemies[i]);
            }

            ComputePlayerInput(Allies[ActivePlayerIndex], _playerInput, _swapCharacterSpherecastLength, _swapCharacterSpherecastRadius, _swapCharacterLayerMask);
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
                ComputeCommonInputFixed(Allies[i]);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                ComputeCommonInputFixed(Enemies[i]);
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
                ally.OnBallCollisionEnterEvent += OnBallCollisionEnter;
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                MatchCharacterControllerView enemy = Enemies[i];
                enemy.OnBallCollisionEnterEvent += OnBallCollisionEnter;
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
                ally.OnBallCollisionEnterEvent -= OnBallCollisionEnter;
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                MatchCharacterControllerView enemy = Enemies[i];
                enemy.OnBallCollisionEnterEvent -= OnBallCollisionEnter;
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
                    Allies[i].DislayHalo(false);
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
        internal void SetEntities(List<Transform> alliesT, List<Transform> enemiesT)
        {
            _vm.SetEntities(alliesT.Count, enemiesT.Count);

            Allies.Clear();
            Enemies.Clear();

            for (int i = 0; i < alliesT.Count; ++i)
            {
                Allies.Add(alliesT[i].GetComponent<MatchCharacterControllerView>());
            }

            for (int i = 0; i < enemiesT.Count; ++i)
            {
                Enemies.Add(enemiesT[i].GetComponent<MatchCharacterControllerView>());
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
        /// Réinitialise les données du contrôleur pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            _vm.ResetController();

            _lastSwapCharacterTarget = null;
            CancelSwap();

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

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        private void OnNewMatchStarted(MatchSettingsData _)
        {
            if (Allies != null)
            {
                // Désactive les inputs des joueurs déjà présents avant de les retirer
                EnablePlayersInput(false);

                // Détache les callbacks des anciennes instances
                UnsubscribeEntities();
            }

            SetEntities(_spawnerV.AlliesT, _spawnerV.EnemiesT);
            SetTeams();
            SubscribeEntities();
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
        /// Appelée quand le perso entre en collision avec un ballon
        /// </summary>
        /// <param name="sender">Le perso</param>
        /// <param name="ball">Le ballon</param>
        private void OnBallCollisionEnter(object sender, BallView ball)
        {
            MatchCharacterControllerView character = sender as MatchCharacterControllerView;
            bool characterIsAlly = Allies.Contains(character);
            int characterIndex = characterIsAlly ? Allies.IndexOf(character) : Enemies.IndexOf(character);
            int ballIndex = _ballManagerV.Balls.IndexOf(ball);
            MatchCharacterControllerState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];
            BallState ballState = _ballManagerV.BallStates[ballIndex];

            if (!ballState.IsLive)
            {
                // TAF: Ramasser la balle
                PickUpBall(characterIndex, ballIndex, character, ball, in characterState, in ballState);
            }
            else
            {
                switch (ballState.ActiveTeamID)
                {
                    case 0:
                        if (characterState.IsAlly)
                        {
                            // TAF : Balle alliée, c'est une passe donc le perso la récupère
                            PickUpBall(characterIndex, ballIndex, character, ball, in characterState, in ballState);
                            //TAF : Changer le contrôle du joueur pour contrôler le receveur

                        }
                        else
                        {
                            // TAF : Balle ennemie, le perso est éliminé

                        }
                        break;
                    case 1:
                        if (!characterState.IsAlly)
                        {
                            // TAF : Balle alliée, c'est une passe donc le perso la récupère
                            PickUpBall(characterIndex, ballIndex, character, ball, in characterState, in ballState);
                            //TAF : Changer le contrôle du joueur pour contrôler le receveur

                        }
                        else
                        {
                            // TAF : Balle ennemie, le perso est éliminé

                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="character">Le perso</param>
        private void ComputeCommonInputFixed(MatchCharacterControllerView character)
        {
            IMatchCharacterInput activeInput = character.ActiveInput;

            // Translation + Rotation
            if (activeInput.MoveAxis != Vector2.zero)
            {
                character.Move(activeInput.MoveAxis);
            }
        }

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="character">Le perso</param>
        private void ComputeCommonInput(MatchCharacterControllerView character)
        {
            IMatchCharacterInput activeInput = character.ActiveInput;

            // Translation + Rotation
            if (activeInput.MoveAxis != Vector2.zero)
            {
                character.RotateMesh(activeInput.MoveAxis);
            }

            // Changement de cible
            if (activeInput.PreviousTargetTrigger)
            {
                SelectNewOpponentTarget(character, -1);
            }
            if (activeInput.NextTargetTrigger)
            {
                SelectNewOpponentTarget(character, 1);
            }

            // Tir
            if (character.IsHoldingABall && !_vm.IsSwappingCharacter)
            {
                if (activeInput.IsHoldingFire)
                {
                    character.ChargeShot();
                }
                if (activeInput.HasReleasedFire && character.Energy < 1f)
                {
                    character.Shoot();
                }
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
            if (_lastSwapCharacterAxis == Vector2.zero && playerInput.SwapCharacterAxis != Vector2.zero)
            {
                IsSwappingCharacter = true;
            }

            // Changement d'allié contrôlé par le joueur
            if (IsSwappingCharacter)
            {
                MatchCharacterControllerView target = GetClosestAllyInDirection(activePlayer, playerInput.SwapCharacterAxis, swapCharacterSpherecastLength, swapCharacterSpherecastRadius, swapCharacterLayerMask);

                if (target && target.IsAlly)
                {
                    DisplayHaloes(_lastSwapCharacterTarget, target);
                    _lastSwapCharacterTarget = target;
                    CurAllyTargetForSwapIndex = Allies.IndexOf(target);
                }
            }

            // On passe le contrôle à l'allié sélectionné
            if (IsSwappingCharacter && playerInput.FireTrigger)
            {
                SwapControl(activePlayer, _lastSwapCharacterTarget);

                // Pour empêcher le nouveau perso de tirer
                playerInput.IsHoldingFire = false;
                playerInput.HasReleasedFire = false;
            }

            // On annule le changement de perso
            if (IsSwappingCharacter && playerInput.CancelSwapTrigger)
            {
                CancelSwap();
            }

            _lastSwapCharacterAxis = playerInput.SwapCharacterAxis;
        }

        /// <summary>
        /// Sélectionne une nouvelle cible pour le perso
        /// </summary>
        /// <param name="character">Le perso concerné</param>
        /// <param name="increment">Position si suivant, négatif si précédent</param>
        private void SelectNewOpponentTarget(MatchCharacterControllerView character, int increment)
        {
            int previousTargetIndex = character.LastOpponentTargetIndex;

            // On choisit la cible adverse
            character.LastOpponentTargetIndex += increment;

            if (character.LastOpponentTargetIndex < 0)
                character.LastOpponentTargetIndex = character.IsAlly ? Enemies.Count - 1 : Allies.Count - 1;
            if (character.IsAlly && character.LastOpponentTargetIndex == Enemies.Count || !character.IsAlly && character.LastOpponentTargetIndex == Allies.Count)
                character.LastOpponentTargetIndex = 0;

            // Si le perso est celui du joueur, on affiche le halo de sa cible parmi les ennemis
            // et on masque celle de la cible précédente s'il y en a une
            if (Allies.Contains(character) && Allies.IndexOf(character) == ActivePlayerIndex)
            {
                if (previousTargetIndex > -1)
                {
                    Enemies[previousTargetIndex].DislayHalo(false);
                }

                Enemies[character.LastOpponentTargetIndex].DislayHalo(true);
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
        /// <returns>L'allié le plus près du joueur et de ce rayon</returns>
        private MatchCharacterControllerView GetClosestAllyInDirection(MatchCharacterControllerView activePlayer, Vector2 swapCharacterAxis, float swapCharacterSpherecastLength, float swapCharacterSpherecastRadius, LayerMask swapCharacterLayerMask)
        {
            _vm.GetSwapDirectionXZ(activePlayer.transform.position, swapCharacterAxis, out Vector3 origin, out Vector3 dir);

            if (Physics.SphereCast(origin, swapCharacterSpherecastRadius, dir, out RaycastHit hit, swapCharacterSpherecastLength, swapCharacterLayerMask))
            {
                return hit.collider.GetComponent<MatchCharacterControllerView>();
            }

            return null;
        }

        /// <summary>
        /// Affiche les halos des persos dont on doit échanger le contrôle
        /// </summary>
        /// <param name="lastTarget">Le perso précédent</param>
        /// <param name="newTarget">Le nouveau perso du joueru</param>
        private void DisplayHaloes(MatchCharacterControllerView lastTarget, MatchCharacterControllerView newTarget)
        {
            if (lastTarget != null && lastTarget != newTarget)
            {
                // Fait disparaître le halo de la cible précédente
                lastTarget.DislayHalo(false);
            }

            //Affiche le halo de la nouvelle cible
            newTarget.DislayHalo(true);
        }

        /// <summary>
        /// Echange le contrôle d'alliés entre l'IA et le joueur
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="target">La cible à contrôler</param>
        private void SwapControl(MatchCharacterControllerView activePlayer, MatchCharacterControllerView target)
        {
            // On passe le contrôle à l'allié sélectionné
            int index = _lastSwapCharacterTarget != null ? Allies.IndexOf(target) : -1;
            _vm.SwapControl(index);
            SetActivePlayer(index);
            _lastSwapCharacterTarget = null;

            // On masque également le halo de la cible ennemi s'il y en a une
            if (activePlayer.LastOpponentTargetIndex > -1)
            {
                Enemies[activePlayer.LastOpponentTargetIndex].DislayHalo(false);
            }
        }

        /// <summary>
        /// Annule le changement de perso
        /// </summary>
        private void CancelSwap()
        {
            _vm.CancelSwap();

            if (_lastSwapCharacterTarget != null)
            {
                // Fait disparaître le halo de la cible précédente
                _lastSwapCharacterTarget.DislayHalo(false);
            }

            _lastSwapCharacterTarget = null;
        }

        /// <summary>
        /// Retire au perso les infos sur sa cible. Utilisée quand la cible se fait éliminer
        /// </summary>
        /// <param name="character">Le perso concerné</param>>
        private void ClearTarget(MatchCharacterControllerView character)
        {
            // Si le perso est celui du joueur, on masque le halo de sa cible
            if (character.LastOpponentTargetIndex > -1 && Allies.Contains(character) && Allies.IndexOf(character) == ActivePlayerIndex)
            {
                Enemies[character.LastOpponentTargetIndex].DislayHalo(false);
            }

            character.LastOpponentTargetIndex = -1;
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="characterIndex">L'ID du perso</param>
        /// <param name="ballIndex">L'ID du ballon</param>
        /// <param name="character">Le perso</param>
        /// <param name="ball">Le ballon</param>
        /// <param name="characterState">L'état du perso</param>
        /// <param name="ballState">L'état du ballon</param>
        private void PickUpBall(int characterIndex, int ballIndex, MatchCharacterControllerView character, BallView ball, in MatchCharacterControllerState characterState, in BallState ballState)
        {
            // Si le perso détient déjà un ballon
            // ou qu'il tente de récupérer une balle réservée à l'ennemi,
            // il ne peut pas en ramasser une nouvelle

            if (characterState.IsHoldingABall || (characterState.IsAlly && ballState.ReservedTeamID == 1) || (!characterState.IsAlly && ballState.ReservedTeamID == 0))
                return;

            _vm.PickUpBall(characterIndex, characterState.IsAlly);

            character.PickUpBall(ball);

            OnBallPickedupEvent?.Invoke(null, new BallPickedUpEventArgs(characterIndex, characterState.IsAlly, ballIndex));
        }

        #endregion
    }
}