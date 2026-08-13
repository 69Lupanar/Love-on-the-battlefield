using System;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Arguments de l'événement associé
    /// </summary>
    public class ShootEventArgs : EventArgs
    {
        #region Propriétéss

        /// <summary>
        /// L'ID du tireur
        /// </summary>
        internal int CharacterIndex { get; private set; }

        /// <summary>
        /// L'ID du ballon
        /// </summary>
        internal int BallIndex { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="characterIndex">L'ID du tireur</param>
        /// <param name="ballIndex">L'ID du ballon</param>
        public ShootEventArgs(int characterIndex, int ballIndex)
        {
            CharacterIndex = characterIndex;
            BallIndex = ballIndex;
        }
        #endregion
    }
}