using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement des joueurs et ballons
    /// </summary>
    internal sealed class MatchPlayerControllerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<MatchCharacterController> Allies { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterController> Enemies { get; private set; } = new();

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<Ball> Balls { get; private set; } = new();

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        internal int ActivePlayerIndex { get; set; }

        /// <summary>
        /// L'ID du perso allié actuellement sélectionné comme cible
        /// lors du changement de contrôle
        /// (-1 si aucun)
        /// </summary>
        internal int CurAllyTargetForSwapIndex { get; private set; }

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
        /// Le MatchManagerViewModel
        /// </summary>
        private MatchManagerViewModel _matchVM;

        /// <summary>
        /// true si le joueur est en cours de changement de personnage
        /// </summary>
        private bool _isSwappingCharacter;

        /// <summary>
        /// la dernière position du joystick droit
        /// </summary>
        private Vector2 _lastSwapCharacterAxis;

        /// <summary>
        /// la dernière cible du changement de contrôle
        /// </summary>
        private MatchCharacterController _lastSwapCharacterTarget;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _matchVM = FindAnyObjectByType<MatchManagerViewModel>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_matchVM.MatchIsOver)
                return;

            for (int i = 0; i < _matchVM.NbAllies; ++i)
            {
                ComputeInput(Allies[i]);
            }

            for (int i = 0; i < _matchVM.NbEnemies; ++i)
            {
                ComputeInput(Enemies[i]);
            }

            ComputePlayerInput(Allies[ActivePlayerIndex]);
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        internal void SetActivePlayer(int index)
        {
            ActivePlayerIndex = index;

            for (int i = 0; i < _matchVM.NbAllies; ++i)
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
        /// Assigne les persos et ballons
        /// </summary>
        /// <param name="alliesT">Transforms des persos alliés</param>
        /// <param name="enemiesT">Transforms des persos ennemis</param>
        /// <param name="ballsT">Transforms des ballons</param>
        internal void SetPlayersAndBalls(List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT)
        {
            Allies.Clear();
            Allies.Clear();
            Allies.Clear();

            for (int i = 0; i < alliesT.Count; ++i)
            {
                Allies.Add(alliesT[i].GetComponent<MatchCharacterController>());
            }

            for (int i = 0; i < enemiesT.Count; ++i)
            {
                Enemies.Add(enemiesT[i].GetComponent<MatchCharacterController>());
            }

            for (int i = 0; i < ballsT.Count; ++i)
            {
                Balls.Add(ballsT[i].GetComponent<Ball>());
            }
        }

        /// <summary>
        /// Assigne les équipes à chaque perso
        /// </summary>
        internal void SetTeams()
        {
            for (int i = 0; i < _matchVM.NbAllies; ++i)
            {
                Allies[i].IsAlly = true;
            }

            for (int i = 0; i < _matchVM.NbEnemies; ++i)
            {
                Enemies[i].IsAlly = false;
                Enemies[i].GiveControlToAI();
            }
        }

        /// <summary>
        /// Réinitialise les persos pour une nouvelle manche
        /// </summary>
        internal void ResetPlayersAndBalls()
        {
            for (int i = 0; i < _matchVM.NbAllies; ++i)
            {
                Allies[i].ResetPlayer();
            }

            for (int i = 0; i < _matchVM.NbEnemies; ++i)
            {
                Enemies[i].ResetPlayer();
            }

            for (int i = 0; i < _matchVM.NbBalls; ++i)
            {
                Balls[i].ResetBall();
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="character">Le perso</param>
        private void ComputeInput(MatchCharacterController character)
        {
            IMatchCharacterInput activeInput = character.ActiveInput;

            // Translation + Rotation
            if (activeInput.MoveAxis != Vector2.zero)
            {
                character.Move(activeInput.MoveAxis);
                character.RotateMesh(activeInput.MoveAxis);
            }
        }

        /// <summary>
        /// Exécute les commandes du joueur
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        private void ComputePlayerInput(MatchCharacterController activePlayer)
        {
            MatchPlayerInput input = activePlayer.ActiveInput as MatchPlayerInput;

            if (_lastSwapCharacterAxis == Vector2.zero && input.SwapCharacterAxis != Vector2.zero)
            {
                _isSwappingCharacter = true;
            }

            // Changement d'allié contrôlé par le joueur
            if (_isSwappingCharacter)
            {
                MatchCharacterController target = GetClosestAllyInDirection(activePlayer, input.SwapCharacterAxis);

                if (_lastSwapCharacterTarget != null && _lastSwapCharacterTarget != target)
                {
                    // Fait disparaître le halo de la cible précédente
                    _lastSwapCharacterTarget.DislayHalo(false);
                }

                if (target && target.IsAlly)
                {
                    // On n'affiche le halo que si c'est un allié
                    target.DislayHalo(true);
                    _lastSwapCharacterTarget = target;
                    CurAllyTargetForSwapIndex = Allies.IndexOf(target);
                }
                else
                {
                    _lastSwapCharacterTarget = null;
                    CurAllyTargetForSwapIndex = -1;
                }
            }

            if (_isSwappingCharacter && input.FireTrigger)
            {
                _isSwappingCharacter = false;

                // On passe le contrôle à l'allié sélectionné

                if (_lastSwapCharacterTarget != null)
                {
                    int index = Allies.IndexOf(_lastSwapCharacterTarget);
                    SetActivePlayer(index);

                    // On transfère la valeur du joystick pour éviter de pouvoir imméiatement changer de joueur à nouveau
                    (_lastSwapCharacterTarget.ActiveInput as MatchPlayerInput).SwapCharacterAxis = input.SwapCharacterAxis;
                }
            }

            _lastSwapCharacterAxis = input.SwapCharacterAxis;
        }

        /// <summary>
        /// Trace une ligne et récupère l'allié le plus proche de celle-ci
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="dirXZ">La direction de la ligne sur l'axe XZ</param>
        /// <returns>L'allié le plus près du joueur et de ce rayon</returns>
        private MatchCharacterController GetClosestAllyInDirection(MatchCharacterController activePlayer, Vector2 dirXZ)
        {
            Vector3 origin = activePlayer.transform.position + Vector3.up;
            Vector3 dir = new(dirXZ.x, 0f, dirXZ.y);

            if (Physics.SphereCast(origin, _swapCharacterSpherecastRadius, dir, out RaycastHit hit, _swapCharacterSpherecastLength, _swapCharacterLayerMask))
            {
                return hit.collider.GetComponent<MatchCharacterController>();
            }

            return null;
        }

        #endregion
    }
}