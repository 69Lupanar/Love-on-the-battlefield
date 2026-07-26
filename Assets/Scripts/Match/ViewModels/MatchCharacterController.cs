using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    [RequireComponent(typeof(MatchAIInput), typeof(Rigidbody))]
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

        /// <summary>
        /// true si le perso porte un ballon
        /// </summary>
        internal bool IsHoldingABall { get; set; }

        /// <summary>
        /// Le dernier adversaire ciblé par le joueur
        /// </summary>
        internal int LastOpponentTargetIndex { get; set; }

        #endregion

        #region Inspecteur

        [Header("Components")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Tag du ballon")]
        private string _ballTag;

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
        /// Commandes du joueur
        /// </summary>
        private MatchPlayerInput _playerInput;

        /// <summary>
        /// Commandes de l'IA
        /// </summary>
        private MatchAIInput _aiInput;

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
            _playerInput = FindAnyObjectByType<MatchPlayerInput>();
            _aiInput = GetComponent<MatchAIInput>();
        }

        /// <summary>
        /// Appelée quand collision avec un autre objet
        /// </summary>
        /// <param name="collision">Infos sur la collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            GameObject go = collision.gameObject;

            if (go.CompareTag(_ballTag))
            {
                Ball ball = go.GetComponent<Ball>();

                if (!ball.IsLive)
                {
                    // TAF: Ramasser la balle
                    PickUpBall(ball);
                }
                else
                {
                    switch (ball.ActiveTeamID)
                    {
                        case 0:
                            if (IsAlly)
                            {
                                // TAF : Balle alliée, c'est une passe donc le perso la récupère
                                PickUpBall(ball);

                            }
                            else
                            {
                                // TAF : Balle ennemie, le perso est éliminé

                            }
                            break;
                        case 1:
                            if (!IsAlly)
                            {
                                // TAF : Balle alliée, c'est une passe donc le perso la récupère
                                PickUpBall(ball);

                            }
                            else
                            {
                                // TAF : Balle ennemie, le perso est éliminé

                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Donne le contrôle du perso au joueur
        /// </summary>
        internal void GiveControlToPlayer()
        {
            _activeInput = _playerInput;
        }

        /// <summary>
        /// Donne le contrôle du perso à l'IA
        /// </summary>
        internal void GiveControlToAI()
        {
            _activeInput = _aiInput;
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
            _rb.linearVelocity = Vector3.zero;
            _meshHolder.localEulerAngles = Vector3.zero;
            LastOpponentTargetIndex = -1;
            IsHoldingABall = false;

            // TAF: Relâcher le ballon s'il en a un
            DislayHalo(false);
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
        /// Affiche le halo du perso comme étant celui d'un allié ou d'un ennemi
        /// </summary>
        internal void DislayHalo(bool show)
        {
            _haloAlly.SetActive(show && IsAlly);
            _haloEnemy.SetActive(show && !IsAlly);
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="ball">Le ballon</param>
        private void PickUpBall(Ball ball)
        {
            // Si le perso détient déjà un ballon
            // ou qu'il tente de récupérer une balle réservée à l'ennemi,
            // il ne peut pas en ramasser une nouvelle

            if (IsHoldingABall || (IsAlly && ball.ReservedTeamID == 1) || (!IsAlly && ball.ReservedTeamID == 0))
                return;

            IsHoldingABall = true;
            ball.transform.SetParent(_ballHoldingPos);
            ball.PickUp(IsAlly);
        }

        #endregion
    }
}