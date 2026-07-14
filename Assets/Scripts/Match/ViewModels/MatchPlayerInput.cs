using UnityEngine;
using UnityEngine.InputSystem;

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

        /// <summary>
        /// Valeur de l'axe de mouvement (joystick gauche)
        /// </summary>
        public Vector2 MoveAxis { get; set; }

        /// <summary>
        /// Valeur de l'axe du joystick droit
        /// </summary>
        public Vector2 SwapCharacterAxis { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de passage à la cible suivante
        /// </summary>
        public bool NextTargetTrigger { get; set; }

        /// <summary>
        /// true si le joueur presse la touche de passage à la cible précédente
        /// </summary>
        public bool PreviousTargetTrigger { get; set; }

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
            _input ??= new MatchPlayerInputActions();
            _input.Player.Fire.started += OnFireButtonDown;
            _input.Player.Fire.canceled += OnFireButtonUp;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        public void Update()
        {
            MoveAxis = _input.Player.Move.ReadValue<Vector2>();
            SwapCharacterAxis = _input.Player.SwapCharacter.ReadValue<Vector2>();
            NextTargetTrigger = _input.Player.NextTarget.triggered;
            PreviousTargetTrigger = _input.Player.PreviousTarget.triggered;
            JumpTrigger = _input.Player.Jump.triggered;
            DodgeTrigger = _input.Player.Dodge.triggered;
            BlockTrigger = _input.Player.Block.triggered;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.Player.Fire.started -= OnFireButtonDown;
                _input.Player.Fire.canceled -= OnFireButtonUp;
            }
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

            _input ??= new MatchPlayerInputActions();
            _input.Enable();
        }

        /// <summary>
        /// Désactive les commandes
        /// </summary>
        public void Disable()
        {
            // On doit le faire pour désactiver les fonctions d'Unity
            enabled = false;

            _input?.Disable();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelée quand on appuie sur le bouton de tir
        /// </summary>
        private void OnFireButtonDown(InputAction.CallbackContext _)
        {
            IsHoldingFire = true;
        }

        /// <summary>
        /// Appelée quand on relâche le bouton de tir
        /// </summary>
        private void OnFireButtonUp(InputAction.CallbackContext _)
        {
            IsHoldingFire = false;
        }

        #endregion
    }
}