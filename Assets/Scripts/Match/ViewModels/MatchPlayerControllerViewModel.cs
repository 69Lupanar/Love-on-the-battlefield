using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement des joueurs et ballons
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput))]
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

        #region Instance

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

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        private int _activePlayerIndex;

        #endregion

        #region Méthodes internes

        #region Match

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        internal void SetActivePlayer(int index)
        {
            ActivePlayerIndex = index;

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
        /// Assigne les persos et ballons
        /// </summary>
        /// <param name="alliesT">Transforms des persos alliés</param>
        /// <param name="enemiesT">Transforms des persos ennemis</param>
        /// <param name="ballsT">Transforms des ballons</param>
        internal void SetEntities(List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT)
        {
            Allies.Clear();
            Enemies.Clear();
            Balls.Clear();

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
            for (int i = 0; i < Allies.Count; ++i)
            {
                Allies[i].IsAlly = true;
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].IsAlly = false;
                Enemies[i].GiveControlToAI();
            }
        }

        /// <summary>
        /// Réinitialise les données du contrôleur pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            _lastSwapCharacterTarget = null;
            CurAllyTargetForSwapIndex = -1;

            for (int i = 0; i < Allies.Count; ++i)
            {
                Allies[i].ResetPlayer();
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i].ResetPlayer();
            }

            for (int i = 0; i < Balls.Count; ++i)
            {
                Balls[i].ResetBall(i, Balls.Count);
            }
        }

        #endregion

        #region Input

        /// <summary>
        /// Execute les actions en fonction des commandes actives du perso
        /// </summary>
        /// <param name="character">Le perso</param>
        internal void ComputeCommonInputFixed(MatchCharacterController character)
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
        internal void ComputeCommonInput(MatchCharacterController character)
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
            if (character.IsHoldingABall && !_isSwappingCharacter)
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
        internal void ComputePlayerInput(MatchCharacterController activePlayer, MatchPlayerInput playerInput, float swapCharacterSpherecastLength, float swapCharacterSpherecastRadius, LayerMask swapCharacterLayerMask)
        {
            if (_lastSwapCharacterAxis == Vector2.zero && playerInput.SwapCharacterAxis != Vector2.zero)
            {
                _isSwappingCharacter = true;
            }

            // Changement d'allié contrôlé par le joueur
            if (_isSwappingCharacter)
            {
                MatchCharacterController target = GetClosestAllyInDirection(activePlayer, playerInput.SwapCharacterAxis, swapCharacterSpherecastLength, swapCharacterSpherecastRadius, swapCharacterLayerMask);

                if (target && target.IsAlly)
                {
                    DisplayHaloes(_lastSwapCharacterTarget, target);
                    _lastSwapCharacterTarget = target;
                    CurAllyTargetForSwapIndex = Allies.IndexOf(target);
                }
            }

            // On passe le contrôle à l'allié sélectionné
            if (_isSwappingCharacter && playerInput.FireTrigger)
            {
                SwapControl(activePlayer, _lastSwapCharacterTarget);

                // Pour empêcher le nouveau perso de tirer
                playerInput.IsHoldingFire = false;
                playerInput.HasReleasedFire = false;
            }

            // On annule le changement de perso
            if (_isSwappingCharacter && playerInput.CancelSwapTrigger)
            {
                CancelSwap();
            }

            _lastSwapCharacterAxis = playerInput.SwapCharacterAxis;
        }

        #endregion

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Affiche les halos des persos dont on doit échanger le contrôle
        /// </summary>
        /// <param name="newTarget"></param>
        private void DisplayHaloes(MatchCharacterController lastTarget, MatchCharacterController newTarget)
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
        private void SwapControl(MatchCharacterController activePlayer, MatchCharacterController target)
        {
            // On passe le contrôle à l'allié sélectionné
            _isSwappingCharacter = false;

            if (_lastSwapCharacterTarget != null)
            {
                int index = Allies.IndexOf(target);
                SetActivePlayer(index);
                _lastSwapCharacterTarget = null;
                CurAllyTargetForSwapIndex = -1;
            }

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
            _isSwappingCharacter = false;

            if (_lastSwapCharacterTarget != null)
            {
                // Fait disparaître le halo de la cible précédente
                _lastSwapCharacterTarget.DislayHalo(false);
            }

            _lastSwapCharacterTarget = null;
            CurAllyTargetForSwapIndex = -1;
        }

        /// <summary>
        /// Sélectionne une nouvelle cible pour le perso
        /// </summary>
        /// <param name="character">Le perso concerné</param>
        /// <param name="increment">Position si suivant, négatif si précédent</param>
        private void SelectNewOpponentTarget(MatchCharacterController character, int increment)
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
        /// Retire au perso les infos sur sa cible. Utilisée quand la cible se fait éliminer
        /// </summary>
        /// <param name="character">Le perso concerné</param>>
        private void ClearTarget(MatchCharacterController character)
        {
            // Si le perso est celui du joueur, on masque le halo de sa cible
            if (character.LastOpponentTargetIndex > -1 && Allies.Contains(character) && Allies.IndexOf(character) == ActivePlayerIndex)
            {
                Enemies[character.LastOpponentTargetIndex].DislayHalo(false);
            }

            character.LastOpponentTargetIndex = -1;
        }

        /// <summary>
        /// Trace une ligne et récupère l'allié le plus proche de celle-ci
        /// </summary>
        /// <param name="activePlayer">Le perso contrôlé par le joueur</param>
        /// <param name="dirXZ">La direction de la ligne sur l'axe XZ</param>
        /// <param name="swapCharacterSpherecastLength">Longueur du raycast du changement de contrôle</param>
        /// <param name="swapCharacterSpherecastRadius">Distance d'un allié au rayon pour que celui-ci soit considéré éligible pour le changement de contrôle</param>
        /// <param name="swapCharacterLayerMask">Layermask utilisé pour le changement de contrôle</param>
        /// <returns>L'allié le plus près du joueur et de ce rayon</returns>
        private MatchCharacterController GetClosestAllyInDirection(MatchCharacterController activePlayer, Vector2 dirXZ, float swapCharacterSpherecastLength, float swapCharacterSpherecastRadius, LayerMask swapCharacterLayerMask)
        {
            Vector3 origin = activePlayer.transform.position + Vector3.up;
            Vector3 dir = new(dirXZ.x, 0f, dirXZ.y);

            if (Physics.SphereCast(origin, swapCharacterSpherecastRadius, dir, out RaycastHit hit, swapCharacterSpherecastLength, swapCharacterLayerMask))
            {
                return hit.collider.GetComponent<MatchCharacterController>();
            }

            return null;
        }

        #endregion
    }
}