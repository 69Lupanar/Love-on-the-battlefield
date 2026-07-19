using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchAIInput), typeof(Rigidbody))]
    public class MatchCharacterController : MonoBehaviour, IStateContext<MatchCharacterController, ICharacterInput, MatchCharacterState>
    {
        #region Propriétés

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        public bool IsAlly { get; set; }

        /// <summary>
        /// L'état actuel du perso
        /// </summary>
        public MatchCharacterState RootState { get; set; }

        /// <summary>
        /// Transform
        /// </summary>
        internal Transform T { get; private set; }

        /// <summary>
        /// Rigidbody
        /// </summary>
        internal Rigidbody Rb { get; private set; }

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Commandes du joueur")]
        private MatchPlayerInput _playerInput;

        [SerializeField]
        [Tooltip("Commandes de l'IA")]
        private MatchAIInput _aiInput;

        [SerializeField]
        [Tooltip("Emplacement de la balle quand tenue par le joueur")]
        private Transform _ballHoldingPos;

        [SerializeField]
        [Tooltip("Parent du mesh du personnage")]
        private Transform _meshHolder;

        [SerializeField]
        [Tooltip("Vitesse de mouvement")]
        private float _moveSpeed = 7.5f;

        [Tooltip("Données de mouvement d'un personnage lors d'un match")]
        public MatchCharacterMovementData MovementData;

        #endregion

        #region Instance

        /// <summary>
        /// Commandes actives du personnage
        /// </summary>
        private ICharacterInput _activeInput;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            T = transform;
            Rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_activeInput.MoveAxis != Vector2.zero)
            {
                Move(_activeInput.MoveAxis);
                RotateMesh(_activeInput.MoveAxis);
            }
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

        #region Méthodes privées

        /// <summary>
        /// Déplace le personnage
        /// </summary>
        /// <param name="moveDir">Direction du mouvement</param>
        private void Move(Vector2 moveDir)
        {
            Vector3 moveXZ = new(moveDir.x, 0f, moveDir.y);
            T.Translate(_moveSpeed * Time.deltaTime * moveXZ);
        }

        /// <summary>
        /// Pivote le mesh dans la direction du mouvement
        /// </summary>
        /// <param name="moveDir">Direction du mouvement</param>
        private void RotateMesh(Vector2 moveDir)
        {
            Vector3 moveXZ = new(moveDir.x, 0f, moveDir.y);
            float angle = Mathf.Atan2(moveXZ.x, moveXZ.z) * Mathf.Rad2Deg;

            _meshHolder.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        #endregion
    }
}