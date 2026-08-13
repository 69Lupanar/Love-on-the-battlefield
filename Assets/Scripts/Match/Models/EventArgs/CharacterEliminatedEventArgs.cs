using System;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Données de l'événement
    /// </summary>
    internal sealed class CharacterEliminatedEventArgs : EventArgs
    {
        #region Propriétés

        /// <summary>
        /// true si le perso éliminé est un allié
        /// </summary>
        internal bool CharacterIsAlly { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="characterIsAlly">true si le perso éliminé est un allié</param>
        public CharacterEliminatedEventArgs(bool characterIsAlly)
        {
            CharacterIsAlly = characterIsAlly;
        }

        #endregion
    }
}