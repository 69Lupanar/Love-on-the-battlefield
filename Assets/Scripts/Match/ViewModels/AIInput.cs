using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les entrées de l'IA
    /// </summary>
    public class AIInput : MonoBehaviour, ICharacterInput
    {
        #region Instance

        /// <summary>
        /// Valeur de l'axe de mouvement
        /// </summary>
        public Vector2 MoveAxis { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de saut
        /// </summary>
        public bool JumpTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche d'esquive
        /// </summary>
        public bool DodgeTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de blocage
        /// </summary>
        public bool BlockTrigger { get; set; }

        /// <summary>
        /// true si le joueur maintient le bouton de tir
        /// </summary>
        public bool IsHoldingFire { get; set; }

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        public void Update()
        {

        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Active les commandes
        /// </summary>
        public void Enable()
        {

            // On doit le faire pour désactiver les fonctions d'Unity
            enabled = true;

        }

        /// <summary>
        /// Désactive les commandes
        /// </summary>
        public void Disable()
        {

            // On doit le faire pour désactiver les fonctions d'Unity
            enabled = false;

        }

        #endregion
    }
}