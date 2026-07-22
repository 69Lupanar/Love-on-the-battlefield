using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchAIInput), typeof(Rigidbody))]
    public class MatchCharacterController : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        public bool IsAlly { get; set; }

        #endregion

        #region Inspecteur

        [Header("Components")]
        [Space(10)]

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
        [Tooltip("Halo du perso s'il est un allié")]
        private GameObject _haloAlly;

        [SerializeField]
        [Tooltip("Halo du perso s'il est un ennemi")]
        private GameObject _haloEnemy;

        [Space(10)]
        [Header("Physics")]
        [Space(10)]

        [Tooltip("Données de mouvement d'un personnage lors d'un match")]
        public MatchCharacterMovementData MovementData;

        [Tooltip("Distance du raycast vérifiant la présence d'un allié à proximité")]
        public float _swapCharacterRaycastDst = 10f;

        #endregion

        #region Instance

        /// <summary>
        /// Commandes actives du personnage
        /// </summary>
        private IMatchCharacterInput _activeInput;

        /// <summary>
        /// Rigidbody
        /// </summary>
        private Rigidbody _rb;

        /// <summary>
        /// true si le joueur est en cours de changement de personnage
        /// </summary>
        private bool _isSwappingCharacter;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            // Translation + Rotation
            if (_activeInput.MoveAxis != Vector2.zero)
            {
                Move(_activeInput.MoveAxis);
                RotateMesh(_activeInput.MoveAxis);
            }

            // Changement d'allié contrôlé par le joueur
            if (_activeInput.SwapCharacterAxis != Vector2.zero)
            {
                _isSwappingCharacter = true;
                // TAF : Indiquer l'allié sélectionné
            }

            if (_isSwappingCharacter && _activeInput.SwapCharacterAxis == Vector2.zero)
            {
                _isSwappingCharacter = false;
                //TAF : Passer le contrôle à l'allié sélectionné
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

        /// <summary>
        /// Réinitialise le perso pour une nouvelle manche
        /// </summary>
        public void ResetPlayer()
        {
            _haloAlly.SetActive(false);
            _haloAlly.SetActive(false);
            _rb.linearVelocity = Vector3.zero;
            _meshHolder.rotation = Quaternion.identity;
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
            _rb.MovePosition(_rb.position + MovementData.MoveSpeed * Time.deltaTime * moveXZ);
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