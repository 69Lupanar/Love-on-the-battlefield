using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des ballons
    /// </summary>
    internal sealed class MatchBallControllerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les données de chaque ballon
        /// </summary>
        internal List<BallData> Balls { get; private set; } = new();

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Crée les données des entités en jeu
        /// </summary>
        /// <param name="nbBalls">Nb de ballons à instancier</param>
        internal void SetEntities(int nbBalls)
        {
            Balls.Clear();

            for (int i = 0; i < nbBalls; ++i)
            {
                Balls.Add(new BallData());
            }
        }

        /// <summary>
        /// Réinitialise les données du contrôleur pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            for (int i = 0; i < Balls.Count; ++i)
            {
                Balls[i] = ResetBall(Balls[i], i, Balls.Count);
            }
        }

        /// <summary>
        /// Réinitialise le ballon pour la prochaine manche
        /// </summary>
        /// <param name="ballData">Les données du ballon</param>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        private BallData ResetBall(BallData ballData, int index, int nbBalls)
        {
            return ballData;
        }

        #endregion
    }
}