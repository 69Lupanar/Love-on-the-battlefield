using System;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement des joueurs et ballons
    /// </summary>
    [RequireComponent(typeof(MatchAIInput), typeof(Rigidbody))]
    internal sealed class MatchCharacterControllerView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelée quand le perso entre en collision avec un ballon
        /// </summary>
        internal EventHandler<Collision> OnCollisionEnterEvent;

        /// <summary>
        /// Appelé quand le ballon heurte un objet
        /// </summary>
        internal EventHandler<Collider> OnTriggerEnterEvent;

        #endregion

        #region Propriétés

        /// <summary>
        /// Commandes actives du personnage
        /// </summary>
        internal IMatchCharacterInput ActiveInput => _activeInput;

        #endregion

        #region Inspecteur

        [Header("Components")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Emplacement de la balle quand tenue par le joueur")]
        private Transform _ballHolder;

        [SerializeField]
        [Tooltip("Parent du mesh du personnage")]
        private Transform _meshHolder;

        [SerializeField]
        [Tooltip("Halo du perso s'il est un allié")]
        private GameObject _haloAlly;

        [SerializeField]
        [Tooltip("Halo du perso s'il est un ennemi")]
        private GameObject _haloEnemy;

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

        /// <summary>
        /// Rigidbody
        /// </summary>
        private Collider _col;

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
            _col = GetComponent<Collider>();
        }

        /// <summary>
        /// Appelée quand collision avec un autre objet
        /// </summary>
        /// <param name="collision">Infos sur la collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterEvent?.Invoke(this, collision);
        }

        /// <summary>
        /// Appelée quand collision avec un autre objet
        /// </summary>
        /// <param name="other">L'objet entré en collision</param>
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(this, other);
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
        /// Active ou non la physique sur cet objet
        /// </summary>
        internal void EnablePhysics(bool enable)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.isKinematic = !enable;
            _rb.useGravity = enable;
            _col.enabled = enable;
        }

        /// <summary>
        /// Réinitialise le perso pour une nouvelle manche
        /// </summary>
        internal void ResetPlayer()
        {
            _meshHolder.localEulerAngles = Vector3.zero;
            EnablePhysics(true);
            HideHalo();
        }

        /// <summary>
        /// Déplace le personnage
        /// </summary>
        /// <param name="moveDir">Direction du mouvement</param>
        /// <param name="moveSpeed">Vitesse de déplacement</param>
        internal void Move(Vector2 moveDir, float moveSpeed)
        {
            Vector3 moveXZ = new(moveDir.x, 0f, moveDir.y);
            _rb.MovePosition(_rb.position + moveSpeed * Time.deltaTime * moveXZ);
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
        /// <param name="characterIsAlly">true si le perso est dans l'équipe du joueur</param>
        internal void DislayHalo(bool characterIsAlly)
        {
            _haloAlly.SetActive(characterIsAlly);
            _haloEnemy.SetActive(!characterIsAlly);
        }

        /// <summary>
        /// Masque le halo du perso
        /// </summary>
        internal void HideHalo()
        {
            _haloAlly.SetActive(false);
            _haloEnemy.SetActive(false);
        }

        /// <summary>
        /// Tire le ballon
        /// </summary>
        /// <param name="fireForceInterval">Intervale de force du tir</param>
        /// <param name="energy">Energie du perso au moment du tir</param>
        internal void Shoot(Vector2 fireForceInterval, float energy)
        {
            // Plus le joueur charge longtemps, plus son tir sera puissant
            Vector3 dir = _meshHolder.forward;
            float force = math.lerp(fireForceInterval.y, fireForceInterval.x, energy);

            // Libère la balle et lui applique une force
            BallView ball = ReleaseBall();
            ball.AddImpulseForce(dir, force);
        }

        /// <summary>
        /// Force le joueur à relâcher le ballon
        /// </summary>
        /// <returns>La Transform du ballon</returns>
        internal BallView ReleaseBall()
        {
            BallView ball = _ballHolder.GetChild(0).GetComponent<BallView>();
            ball.transform.SetParent(null);
            ball.EnablePhysics(true);
            return ball;
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="ball">Le ballon</param>
        internal void PickUpBall(BallView ball)
        {
            ball.transform.SetParent(_ballHolder);
        }

        #endregion
    }
}