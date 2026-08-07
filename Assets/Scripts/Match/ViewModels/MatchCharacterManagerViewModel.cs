using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des joueurs et ballons
    /// </summary>
    internal sealed class MatchCharacterManagerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<MatchCharacterControllerState> AllyStates { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterControllerState> EnemyStates { get; private set; } = new();

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        internal int ActivePlayerIndex { get; set; }

        /// <summary>
        /// L'ID du perso allié actuellement sélectionné comme cible
        /// lors du changement de contrôle
        /// (-1 si aucun)
        /// </summary>
        internal int CurAllyTargetForSwapIndex { get; set; }

        /// <summary>
        /// true si le joueur est en cours de changement de personnage
        /// </summary>
        internal bool IsSwappingCharacter { get; set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Crée les données des entités en jeu
        /// </summary>
        /// <param name="nbAllies">Nb d'alliés à instancier</param>
        /// <param name="nbEnemies">Nb d'ennemis à instancier</param>
        internal void SetEntities(int nbAllies, int nbEnemies)
        {
            AllyStates.Clear();
            EnemyStates.Clear();

            for (int i = 0; i < nbAllies; ++i)
            {
                AllyStates.Add(new MatchCharacterControllerState());
            }

            for (int i = 0; i < nbEnemies; ++i)
            {
                EnemyStates.Add(new MatchCharacterControllerState());
            }

        }

        /// <summary>
        /// Assigne les équipes à chaque perso
        /// </summary>
        internal void SetTeams()
        {
            for (int i = 0; i < AllyStates.Count; ++i)
            {
                MatchCharacterControllerState ally = AllyStates[i];
                ally.IsAlly = true;
                AllyStates[i] = ally;
            }

            for (int i = 0; i < EnemyStates.Count; ++i)
            {
                MatchCharacterControllerState enemy = EnemyStates[i];
                enemy.IsAlly = false;
                EnemyStates[i] = enemy;
            }
        }

        /// <summary>
        /// Réinitialise les données des persos pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            CancelSwap();

            for (int i = 0; i < AllyStates.Count; ++i)
            {
                AllyStates[i] = ResetPlayer(AllyStates[i]);
            }

            for (int i = 0; i < EnemyStates.Count; ++i)
            {
                EnemyStates[i] = ResetPlayer(EnemyStates[i]);
            }
        }

        /// <summary>
        /// Obtient la direction de l'axe vers où détecter un allié pour le changement de contrôle
        /// </summary>
        /// <param name="activePlayerPos">Position du joueur actif</param>
        /// <param name="swapCharacterAxis">La direction de la ligne sur l'axe XZ</param>
        /// <param name="origin">Début du rayon</param>
        /// <param name="dir">Direction du rayon</param>
        internal void GetSwapDirectionXZ(Vector3 activePlayerPos, Vector2 swapCharacterAxis, out Vector3 origin, out Vector3 dir)
        {
            origin = activePlayerPos + Vector3.up;
            dir = new Vector3(swapCharacterAxis.x, 0f, swapCharacterAxis.y);
        }

        /// <summary>
        /// Echange le contrôle d'alliés entre l'IA et le joueur
        /// </summary>
        /// <param name="newActivePlayerIndex">Index du nouveau joueur actif (-1 si aucun)</param>
        internal void SwapControl(int newActivePlayerIndex)
        {
            // On passe le contrôle à l'allié sélectionné
            IsSwappingCharacter = false;

            if (newActivePlayerIndex > -1)
            {
                ActivePlayerIndex = newActivePlayerIndex;
                CurAllyTargetForSwapIndex = -1;
            }
        }

        /// <summary>
        /// Annule le changement de perso
        /// </summary>
        internal void CancelSwap()
        {
            IsSwappingCharacter = false;
            CurAllyTargetForSwapIndex = -1;
        }

        /// <summary>
        /// Charge un tir
        /// </summary>
        /// <param name="characterState">Le personnage concerné</param>
        /// <param name="fireChargeSpeed">Vitesse de charge du tir</param>
        /// <param name="deltaTime">Durée d'une frame</param>
        internal MatchCharacterControllerState ChargeShot(MatchCharacterControllerState characterState, float fireChargeSpeed, float deltaTime)
        {
            if (characterState.Energy > 0f)
            {
                characterState.Energy -= deltaTime * fireChargeSpeed;
            }

            return characterState;
        }

        /// <summary>
        /// Tire le ballon
        /// </summary>
        /// <param name="characterState">Le personnage concerné</param>
        internal MatchCharacterControllerState Shoot(MatchCharacterControllerState characterState)
        {
            characterState.IsHoldingABall = false;
            characterState.Energy = 1f;
            return characterState;
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="characterIndex">L'ID du personnage concerné</param>
        /// <param name="isAlly">true si le perso est un allié</param>
        internal void PickUpBall(int characterIndex, bool isAlly)
        {
            if (isAlly)
            {
                MatchCharacterControllerState characterState = AllyStates[characterIndex];
                characterState.IsHoldingABall = true;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterControllerState characterState = EnemyStates[characterIndex];
                characterState.IsHoldingABall = true;
                EnemyStates[characterIndex] = characterState;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Réinitialise les données du perso pour une nouvelle manche
        /// </summary>
        private MatchCharacterControllerState ResetPlayer(MatchCharacterControllerState matchCharacterData)
        {
            matchCharacterData.IsHoldingABall = false;
            matchCharacterData.IsEliminated = false;
            matchCharacterData.Energy = 1f;
            matchCharacterData.LastOpponentTargetIndex = -1;
            return matchCharacterData;
        }

        #endregion
    }
}