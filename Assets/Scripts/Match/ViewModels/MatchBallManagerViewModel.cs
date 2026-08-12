using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des ballons
    /// </summary>
    internal sealed class MatchBallManagerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les données de chaque ballon
        /// </summary>
        internal List<BallState> BallStates { get; private set; } = new();

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Crée les données des entités en jeu
        /// </summary>
        /// <param name="nbBalls">Nb de ballons à instancier</param>
        internal void SetEntities(int nbBalls)
        {
            BallStates.Clear();

            for (int i = 0; i < nbBalls; ++i)
            {
                BallStates.Add(new BallState());
            }
        }

        /// <summary>
        /// Réinitialise les données du gestionnaire pour une nouvelle manche
        /// </summary>
        internal void ResetManager()
        {
            for (int i = 0; i < BallStates.Count; ++i)
            {
                BallStates[i] = ResetBall(BallStates[i], i, BallStates.Count);
            }
        }

        /// <summary>
        /// Appelé quand le ballon heurte un mur ou le sol
        /// </summary>
        /// <param name="ballIndex">La position du ballon dans la liste</param>
        internal void SetBallAsDead(int ballIndex)
        {
            BallState ballState = BallStates[ballIndex];
            ballState.IsLive = false;
            ballState.ActiveTeamID = TeamID.None;
            ballState.LastHoldingPlayerID = -1;
            BallStates[ballIndex] = ballState;
        }

        /// <summary>
        /// Appelé quand le ballon est ramassé par un joueur
        /// </summary>
        /// <param name="ballIndex">ID du ballon dans la liste</param>
        /// <param name="characterIndex">ID du porteur dans son équipe</param>
        /// <param name="characterIsAlly">true si le porteur est un allié</param>
        internal void SetBallAsPickedUp(int ballIndex, int characterIndex, bool characterIsAlly)
        {
            BallState ballState = BallStates[ballIndex];
            ballState.IsLive = false;
            ballState.ActiveTeamID = characterIsAlly ? TeamID.Ally : TeamID.Enemy;
            ballState.LastHoldingPlayerID = characterIndex;
            ballState.ReservedTeamID = TeamID.None;    //Une fois la balle récupérée, cette variable passe à None pour permettre à toutes les équipes de la ramasser.
            BallStates[ballIndex] = ballState;
        }

        /// <summary>
        /// Indique si le ballon renseigné est actif
        /// </summary>
        /// <param name="ballIndex">L'ID du ballon</param>
        /// <returns>true si le ballon est actif</returns>
        internal bool GetIsBallLive(int ballIndex)
        {
            return BallStates[ballIndex].IsLive;
        }

        /// <summary>
        /// Réactive la balle
        /// </summary>
        /// <param name="ballIndex">L'ID de la balle</param>
        internal void SetBallAsLive(int ballIndex)
        {
            BallState ballState = BallStates[ballIndex];
            ballState.IsLive = true;
            BallStates[ballIndex] = ballState;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Réinitialise le ballon pour la prochaine manche
        /// </summary>
        /// <param name="ballState">Les données du ballon</param>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        private BallState ResetBall(BallState ballState, int index, int nbBalls)
        {
            ballState.IsLive = false;
            ballState.ActiveTeamID = TeamID.None;
            ballState.LastHoldingPlayerID = -1;
            ballState.ReservedTeamID = GetBallTeamID(index, nbBalls);
            return ballState;
        }

        /// <summary>
        /// Calcule l'ID d'équipe de la balle
        /// </summary>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        private TeamID GetBallTeamID(int index, int nbBalls)
        {
            // Selon les règles du dodgeball avec balles en tissu, il y a par défaut 5 balles ;
            // Les 2 balles les plus à gauche sont réservées à l'ennemi,
            // les 2 à droite sont aux alliés, celles au centre sont neutres.
            // Comme on peut changer le nombre de balles avant chaque match,
            // on essaye de calculer automatiquement le nb de balles à réserver à chaque équipe.

            if (nbBalls == 1)
                return TeamID.None;

            int nbReserved = Mathf.CeilToInt(nbBalls / 3f);

            if (index < nbReserved)
                return TeamID.Enemy;
            else if (index >= nbBalls - nbReserved)
                return TeamID.Ally;
            else
                return TeamID.None;
        }

        #endregion
    }
}