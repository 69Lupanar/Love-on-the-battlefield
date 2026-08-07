using System;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Infos sur l'événement associé
    /// </summary>
    internal sealed class BallPickedUpEventArgs : EventArgs
    {
        #region Propriétés

        /// <summary>
        /// ID du porteur dans son équipe
        /// </summary>
        internal int CharacterIndex { get; private set; }

        /// <summary>
        /// true si le porteur est un allié
        /// </summary>
        internal bool CharacterIsAlly { get; private set; }

        /// <summary>
        /// ID du ballon
        /// </summary>
        internal int BallIndex { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="characterIndex">ID du porteur dans son équipe</param>
        /// <param name="characterIsAlly">true si le porteur est un allié</param>
        public BallPickedUpEventArgs(int characterIndex, bool characterIsAlly, int ballIndex)
        {
            CharacterIndex = characterIndex;
            CharacterIsAlly = characterIsAlly;
            BallIndex = ballIndex;
        }

        #endregion
    }
}