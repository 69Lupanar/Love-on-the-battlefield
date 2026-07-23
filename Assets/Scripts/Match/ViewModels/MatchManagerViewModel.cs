using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    internal sealed class MatchManagerViewModel : MonoBehaviour
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
        internal int CurAllyTargetForSwapIndex { get; }

        #endregion

        #region Inspecteur

        [field: SerializeField]
        [field: Tooltip("Nombre d'alliés à instancier")]
        internal int NbAllies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre d'ennemis à instancier")]
        internal int NbEnemies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre de ballons à instancier")]
        internal int NbBalls { get; set; } = 5;

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        internal void SetActivePlayer(int index)
        {
            ActivePlayerIndex = index;

            for (int i = 0; i < NbAllies; ++i)
            {
                if (i == index)
                {
                    Allies[i].GiveControlToPlayer();
                }
                else
                {
                    Allies[i].GiveControlToAI();
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
            for (int i = 0; i < NbAllies; ++i)
            {
                Allies[i].IsAlly = true;
            }

            for (int i = 0; i < NbEnemies; ++i)
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
            for (int i = 0; i < NbAllies; ++i)
            {
                Allies[i].ResetPlayer();
            }

            for (int i = 0; i < NbEnemies; ++i)
            {
                Enemies[i].ResetPlayer();
            }

            for (int i = 0; i < NbBalls; ++i)
            {
                Balls[i].ResetBall();
            }
        }

        #endregion
    }
}