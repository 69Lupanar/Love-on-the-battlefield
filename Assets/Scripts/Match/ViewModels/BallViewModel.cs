using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Logique du ballon
    /// </summary>
    internal sealed class BallViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si le ballon est actif
        /// </summary>
        internal bool IsLive { get; set; }

        /// <summary>
        /// Indique l'équipe à laquelle la balle est réservée.
        /// Utilisé au début du match avant lorsque les joueurs partent récupérer la balle.
        /// Une fois la balle récupérée, cette variable passe à -1 pour permettre à toutes les équipes de la ramasser.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        internal int ReservedTeamID { get; private set; }

        /// <summary>
        /// Indique quelle équipe porte la balle.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        internal int ActiveTeamID { get; set; }

        /// <summary>
        /// ID du dernier joueur ayant porté le ballon dans son équipe
        /// </summary>
        internal int LastHoldingPlayerID { get; set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Réinitialise la balle pour la prochaine manche
        /// </summary>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        internal void ResetBall(int index, int nbBalls)
        {
            IsLive = false;
            ActiveTeamID = -1;
            ReservedTeamID = GetBallTeamID(index, nbBalls);
        }

        /// <summary>
        /// Change l'état de la balle pour indiquer qu'elle a été ramassée
        /// </summary>
        /// <param name="isAlly">true si récupérée par un membre de l'équipe du joueur, false pour l'équipe adverse</param>
        internal void PickUp(bool isAlly)
        {
            IsLive = false;
            ReservedTeamID = -1;    //Une fois la balle récupérée, cette variable passe à -1 pour permettre à toutes les équipes de la ramasser.
            ActiveTeamID = isAlly ? 0 : 1;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Calcule l'ID d'équipe de la balle
        /// </summary>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        private int GetBallTeamID(int index, int nbBalls)
        {
            // Selon les règles du dodgeball avec balles en tissu, il y a par défaut 5 balles ;
            // Les 2 balles les plus à gauche sont réservées à l'ennemi,
            // les 2 à droite sont aux alliés, celles au centre sont neutres.
            // Comme on peut changer le nombre de balles avant chaque match,
            // on essaye de calculer automatiquement le nb de balles à réserver à chaque équipe.

            if (nbBalls == 1)
                return -1;  // Neutre

            int nbReserved = Mathf.CeilToInt(nbBalls / 3f);

            if (index < nbReserved)
                return 1;   // Ennemi
            else if (index >= nbBalls - nbReserved)
                return 0;   // Allié
            else
                return -1; // Neutre
        }

        #endregion
    }
}