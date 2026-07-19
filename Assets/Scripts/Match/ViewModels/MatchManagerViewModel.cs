using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    public class MatchManagerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        public List<MatchCharacterController> Allies { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        public List<MatchCharacterController> Enemies { get; private set; } = new();

        /// <summary>
        /// Les ballons
        /// </summary>
        public List<Ball> Balls { get; private set; } = new();

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        public int ActivePlayerIndex { get; set; }

        #endregion

        #region Inspecteur

        [field: SerializeField]
        [field: Tooltip("Nombre d'alliés à instancier")]
        public int NbAllies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre d'ennemis à instancier")]
        public int NbEnemies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre de ballons à instancier")]
        public int NbBalls { get; set; } = 5;

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

        internal void SetTeams()
        {
            for (int i = 0; i < NbAllies; ++i)
            {
                Allies[i].IsAlly = false;
            }

            for (int i = 0; i < NbEnemies; ++i)
            {
                Enemies[i].GiveControlToAI();
                Enemies[i].IsAlly = false;
            }
        }

        #endregion
    }
}