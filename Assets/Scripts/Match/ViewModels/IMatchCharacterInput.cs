using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les inputs du joueur et de l'IA
    /// </summary>
    public interface IMatchCharacterInput
    {
        /// <summary>
        /// Valeur de l'axe de mouvement (joystick gauche)
        /// </summary>
        Vector2 MoveAxis { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de passage à la cible suivante
        /// </summary>
        bool NextTargetTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de passage à la cible précédente
        /// </summary>
        bool PreviousTargetTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche d'esquive
        /// </summary>
        bool DodgeTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de blocage
        /// </summary>
        bool BlockTrigger { get; set; }

        /// <summary>
        /// true si le joueur appuie sur le bouton de tir
        /// </summary>
        bool HasPressedFire { get; set; }

        /// <summary>
        /// true si le joueur relâche le bouton de tir
        /// </summary>
        bool HasReleasedFire { get; set; }

        /// <summary>
        /// true si le joueur appuie sur le bouton de saut
        /// </summary>
        bool HasPressedJump { get; set; }

        /// <summary>
        /// true si le joueur relâche le bouton de saut
        /// </summary>
        bool HasReleasedJump { get; set; }

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