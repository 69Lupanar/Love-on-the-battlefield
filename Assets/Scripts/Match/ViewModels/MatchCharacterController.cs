using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    [RequireComponent(typeof(MatchPlayerInput), typeof(MatchAIInput), typeof(Rigidbody))]
    internal sealed class MatchCharacterController : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Commandes actives du personnage
        /// </summary>
        internal IMatchCharacterInput ActiveInput => _activeInput;

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        internal bool IsAlly { get; set; }

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

        [SerializeField]
        [Tooltip("Données de mouvement d'un personnage lors d'un match")]
        internal MatchCharacterMovementData MovementData;

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

        #region Méthodes internes

        /// <summary>
        /// Donne le contrôle du perso au joueur
        /// </summary>
        internal void GiveControlToPlayer()
        {
            _activeInput = _playerInput;
            _aiInput.Disable();
            _playerInput.Enable();
        }

        /// <summary>
        /// Donne le contrôle du perso à l'IA
        /// </summary>
        internal void GiveControlToAI()
        {
            _activeInput = _aiInput;
            _aiInput.Enable();
            _playerInput.Disable();
        }

        /// <summary>
        /// Active ou non les commandes du personnages
        /// </summary>
        internal void EnableInput(bool enable)
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
        internal void ResetPlayer()
        {
            DislayHalo(false);
            _rb.linearVelocity = Vector3.zero;
            _meshHolder.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// Déplace le personnage
        /// </summary>
        /// <param name="moveDir">Direction du mouvement</param>
        internal void Move(Vector2 moveDir)
        {
            Vector3 moveXZ = new(moveDir.x, 0f, moveDir.y);
            _rb.MovePosition(_rb.position + MovementData.MoveSpeed * Time.deltaTime * moveXZ);
        }

        /// <summary>
        /// Pivote le mesh dans la direction du mouvement
        /// </summary>
        /// <param name="moveDir">Direction du mouvement</param>
        internal void RotateMesh(Vector2 moveDir)
        {
            Vector3 moveXZ = new(moveDir.x, 0f, moveDir.y);
            float angle = Mathf.Atan2(moveXZ.x, moveXZ.z) * Mathf.Rad2Deg;

            _meshHolder.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        /// <summary>
        /// Affiche le halo du perso comme étant celui d'un alli"é ou d'un ennemi
        /// </summary>
        internal void DislayHalo(bool show)
        {
            _haloAlly.SetActive(show && IsAlly);
            _haloEnemy.SetActive(show && !IsAlly);
        }

        #endregion
    }
}