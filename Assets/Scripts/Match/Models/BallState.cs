namespace Assets.Scripts.Match
{
    /// <summary>
    /// Données représentant l'état d'un ballon
    /// </summary>
    public struct BallState
    {
        #region Propriétés

        /// <summary>
        /// true si le ballon est actif
        /// </summary>
        internal bool IsLive;

        /// <summary>
        /// Indique l'équipe à laquelle la balle est réservée.
        /// Utilisé au début du match avant lorsque les joueurs partent récupérer la balle.
        /// Une fois la balle récupérée, cette variable passe à -1 pour permettre à toutes les équipes de la ramasser.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        internal int ReservedTeamID;

        /// <summary>
        /// Indique quelle équipe porte la balle.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        internal int ActiveTeamID;

        /// <summary>
        /// ID du dernier joueur ayant porté le ballon dans son équipe
        /// </summary>
        internal int LastHoldingPlayerID;

        #endregion
    }
}