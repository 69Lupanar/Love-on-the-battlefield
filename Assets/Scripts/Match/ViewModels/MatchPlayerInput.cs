using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les entrées du joueur
    /// </summary>
    public class MatchPlayerInput : MonoBehaviour, ICharacterInput
    {
        #region Instance

        /// <summary>
        /// La table des inputs
        /// </summary>
        private MatchPlayerInputActions _input;

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Active les commandes
        /// </summary>
        public void Enable()
        {
            _input ??= new MatchPlayerInputActions();
            _input.Enable();
        }

        /// <summary>
        /// Désactive les commandes
        /// </summary>
        public void Disable()
        {
            _input.Disable();
        }

        #endregion
    }
}