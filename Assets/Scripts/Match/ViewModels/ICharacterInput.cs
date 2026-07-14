using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les inputs du joueur et de l'IA
    /// </summary>
    public interface ICharacterInput
    {
        /// <summary>
        /// Active les commandes
        /// </summary>
        Vector2 MoveAxis { get; set; }

        /// <summary>
        /// Active les commandes
        /// </summary>
        void Enable();

        /// <summary>
        /// Désactive les commandes
        /// </summary>
        void Disable();
    }
}