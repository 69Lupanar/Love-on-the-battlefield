using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(AIInput), typeof(Rigidbody))]
    public class MatchCharacterController : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        public bool IsAlly { get; set; }

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Commandes du joueur")]
        private MatchPlayerInput _playerInput;

        [SerializeField]
        [Tooltip("Commandes de l'IA")]
        private AIInput _aiInput;

        [SerializeField]
        [Tooltip("Emplacement de la balle quand tenue par le joueur")]
        private Transform _ballHoldingPos;

        #endregion

        #region Instance

        /// <summary>
        /// Commandes actives du personnage
        /// </summary>
        private ICharacterInput _activeInput;

        /// <summary>
        /// Rigidbody
        /// </summary>
        private Rigidbody _rb;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Donne le contrôle du perso au joueur
        /// </summary>
        public void GiveControlToPlayer()
        {
            _activeInput = _playerInput;
            _aiInput.Disable();
            _playerInput.Enable();
        }

        /// <summary>
        /// Donne le contrôle du perso à l'IA
        /// </summary>
        public void GiveControlToAI()
        {
            _activeInput = _aiInput;
            _aiInput.Enable();
            _playerInput.Disable();
        }

        /// <summary>
        /// Active ou non les commandes du personnages
        /// </summary>
        public void EnableInput(bool enable)
        {
            if (enable)
            {
                _activeInput.Enable();
            }
            else
            {
                _activeInput.Disable();
            }
        }

        #endregion
    }
}