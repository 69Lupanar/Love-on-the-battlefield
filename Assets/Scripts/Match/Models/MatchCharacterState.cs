namespace Assets.Scripts.Match
{
    /// <summary>
    /// Données représentant l'état d'un perso lors d'une partie
    /// </summary>
    public struct MatchCharacterState
    {
        #region Propriétés

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        internal bool IsAlly;

        /// <summary>
        /// true si le perso porte un ballon
        /// </summary>
        internal bool IsHoldingABall;

        /// <summary>
        /// true si le perso est éliminé
        /// </summary>
        internal bool IsEliminated;

        /// <summary>
        /// Le dernier adversaire ciblé par le joueur
        /// </summary>
        internal int OpponentTargetIndex;

        /// <summary>
        /// Energie du joueur
        /// </summary>
        internal float Energy;

        /// <summary>
        /// Données de mouvement d'un personnage lors d'un match
        /// </summary>
        internal MatchCharacterMovementData MovementData;

        #endregion
    }
}