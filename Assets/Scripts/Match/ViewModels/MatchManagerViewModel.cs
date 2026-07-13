using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match de dodgeball
    /// </summary>
    public class MatchManagerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        public MatchCharacterController[] Allies { get; private set; }

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        public MatchCharacterController[] Enemies { get; private set; }

        /// <summary>
        /// Les ballons
        /// </summary>
        public Ball[] Balls { get; private set; }

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

        #region Méthodes publiques

        /// <summary>
        /// Alloue les tables pour les persos et les ballons
        /// </summary>
        public void AllocateArrays()
        {
            Allies = new MatchCharacterController[NbAllies];
            Enemies = new MatchCharacterController[NbEnemies];
            Balls = new Ball[NbBalls];
        }

        /// <summary>
        /// Assigne le perso à contrôler par le joueur
        /// </summary>
        /// <param name="index">L'id du perso actif</param>
        public void SetActivePlayer(int index)
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
        internal void DisableActivePlayersAndBalls()
        {
            for (int i = 0; i < Allies.Length; ++i)
            {
                Allies[i].EnableInput(false);
            }

            for (int i = 0; i < Enemies.Length; ++i)
            {
                Enemies[i].EnableInput(false);
            }

            // TAF : Ballons
        }

        /// <summary>
        /// Assigne les persos et ballons
        /// </summary>
        internal void SetPlayersAndBalls(Transform[] alliesT, Transform[] enemiesT, Transform[] ballsT)
        {
            for (int i = 0; i < alliesT.Length; ++i)
            {
                Allies[i] = alliesT[i].GetComponent<MatchCharacterController>();
            }

            for (int i = 0; i < enemiesT.Length; ++i)
            {
                Enemies[i] = enemiesT[i].GetComponent<MatchCharacterController>();
                Enemies[i].GiveControlToAI();
            }

            for (int i = 0; i < ballsT.Length; ++i)
            {
                Balls[i] = ballsT[i].GetComponent<Ball>();
            }
        }

        #endregion
    }
}