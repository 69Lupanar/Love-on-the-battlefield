using System.Collections.Generic;
using Assets.Scripts.Teams;
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
        internal List<MatchCharacterState> AllyStates { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterState> EnemyStates { get; private set; } = new();

        /// <summary>
        /// Données de mouvement des personnages lors d'un match
        /// </summary>
        internal List<MatchCharacterMovementData> AllyMovementDatas { get; private set; } = new();

        /// <summary>
        /// Données de mouvement des personnages lors d'un match
        /// </summary>
        internal List<MatchCharacterMovementData> EnemyMovementDatas { get; private set; } = new();

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
        /// <param name="minBaseMovementData">Données de base du mouvement des persos</param>
        /// <param name="maxBaseMovementData">Données de base du mouvement des persos</param>
        /// <param name="allyTeamComposition">Composition de joueurs de l'équipe alliée</param>
        /// <param name="enemyTeamComposition">Composition de joueurs de l'équipe ennemie</param>
        internal void SetEntities(int nbAllies, int nbEnemies, MatchCharacterMovementData minBaseMovementData, MatchCharacterMovementData maxBaseMovementData, List<TeamCharacterSO> allyTeamComposition, List<TeamCharacterSO> enemyTeamComposition)
        {
            AllyStates.Clear();
            EnemyStates.Clear();

            for (int i = 0; i < nbAllies; ++i)
            {
                AllyStates.Add(new MatchCharacterState());
                AllyMovementDatas.Add(new MatchCharacterMovementData(allyTeamComposition[i].Stats, minBaseMovementData, maxBaseMovementData));
            }

            for (int i = 0; i < nbEnemies; ++i)
            {
                EnemyStates.Add(new MatchCharacterState());
                EnemyMovementDatas.Add(new MatchCharacterMovementData(enemyTeamComposition[i].Stats, minBaseMovementData, maxBaseMovementData));
            }

        }

        /// <summary>
        /// Assigne les équipes à chaque perso
        /// </summary>
        internal void SetTeams()
        {
            for (int i = 0; i < AllyStates.Count; ++i)
            {
                MatchCharacterState ally = AllyStates[i];
                ally.IsAlly = true;
                AllyStates[i] = ally;
            }

            for (int i = 0; i < EnemyStates.Count; ++i)
            {
                MatchCharacterState enemy = EnemyStates[i];
                enemy.IsAlly = false;
                EnemyStates[i] = enemy;
            }
        }

        /// <summary>
        /// Réinitialise les données du gestionnaire pour une nouvelle manche
        /// </summary>
        internal void ResetManager()
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
            IsSwappingCharacter = false;
            ActivePlayerIndex = newActivePlayerIndex;
            CurAllyTargetForSwapIndex = -1;
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
        /// Sélectionne une nouvelle cible pour le perso
        /// </summary>
        /// <param name="characterIndex">L'ID du perso</param>
        /// <param name="increment">Position si suivant, négatif si précédent</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        internal void SelectNewOpponentTarget(int characterIndex, int increment, bool characterIsAlly)
        {
            MatchCharacterState characterState = characterIsAlly ? AllyStates[characterIndex] : EnemyStates[characterIndex];

            // On choisit la cible adverse.
            // Si la cible est déjà éliminée, on passe à la suivante.

            do
            {
                characterState.OpponentTargetIndex += increment;

                if (characterState.OpponentTargetIndex < 0)
                    characterState.OpponentTargetIndex = characterState.IsAlly ? EnemyStates.Count - 1 : AllyStates.Count - 1;
                if (characterState.IsAlly && characterState.OpponentTargetIndex == EnemyStates.Count || !characterState.IsAlly && characterState.OpponentTargetIndex == AllyStates.Count)
                    characterState.OpponentTargetIndex = 0;
            }
            while (characterState.IsAlly ? EnemyStates[characterState.OpponentTargetIndex].IsEliminated : AllyStates[characterState.OpponentTargetIndex].IsEliminated);

            if (characterIsAlly)
            {
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Charge un tir
        /// </summary>
        /// <param name="characterIndex">L'ID du personnage concerné</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        /// <param name="fireChargeSpeed">Vitesse de charge du tir</param>
        /// <param name="deltaTime">Durée d'une frame</param>
        internal void ChargeShot(int characterIndex, bool characterIsAlly, float fireChargeSpeed, float deltaTime)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];

                if (characterState.Energy > 0f)
                {
                    characterState.Energy -= deltaTime * fireChargeSpeed;
                }

                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];

                if (characterState.Energy > 0f)
                {
                    characterState.Energy -= deltaTime * fireChargeSpeed;
                }

                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Tire le ballon
        /// </summary>
        /// <param name="characterIndex">L'ID du personnage concerné</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        internal void Shoot(int characterIndex, bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];
                characterState.BallIndex = -1;
                characterState.Energy = 1f;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];
                characterState.BallIndex = -1;
                characterState.Energy = 1f;
                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="characterIndex">L'ID du personnage concerné</param>
        /// <param name="ballIndex">L'ID de la balle portée par ce perso</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        internal void PickUpBall(int characterIndex, int ballIndex, bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];
                characterState.BallIndex = ballIndex;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];
                characterState.BallIndex = ballIndex;
                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Retire au perso les infos sur sa cible. Utilisée quand la cible se fait éliminer
        /// </summary>
        /// <param name="characterIndex">L'ID perso concerné</param>
        /// <param name="characterIsAlly">true si le perso est un allié</param>
        internal void ClearTarget(int characterIndex, bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];
                characterState.OpponentTargetIndex = -1;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];
                characterState.OpponentTargetIndex = -1;
                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Eliminer un personnage
        /// </summary>
        /// <param name="characterIndex">ID du perso</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        internal void EliminateCharacter(int characterIndex, bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];
                characterState.IsEliminated = true;
                characterState.OpponentTargetIndex = -1;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];
                characterState.IsEliminated = true;
                characterState.OpponentTargetIndex = -1;
                EnemyStates[characterIndex] = characterState;
            }
        }

        /// <summary>
        /// Force le perso à lâcher le ballon
        /// </summary>
        /// <param name="characterIndex">ID du perso</param>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        internal void ReleaseBall(int characterIndex, bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                MatchCharacterState characterState = AllyStates[characterIndex];
                characterState.BallIndex = -1;
                AllyStates[characterIndex] = characterState;
            }
            else
            {
                MatchCharacterState characterState = EnemyStates[characterIndex];
                characterState.BallIndex = -1;
                EnemyStates[characterIndex] = characterState;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Réinitialise les données du perso pour une nouvelle manche
        /// </summary>
        private MatchCharacterState ResetPlayer(MatchCharacterState characterState)
        {
            characterState.BallIndex = -1;
            characterState.IsEliminated = false;
            characterState.Energy = 1f;
            characterState.OpponentTargetIndex = -1;
            return characterState;
        }

        #endregion
    }
}