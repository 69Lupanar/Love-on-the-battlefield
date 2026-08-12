using System;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Logique du ballon
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    internal sealed class BallView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelé quand le ballon heurte un objet
        /// </summary>
        internal EventHandler<Collision> OnCollisionEnterEvent;

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Vitesse de la balle en dessous de laquelle on affiche les halos")]
        private float _displayHaloSpeedThreshold = 1f;

        [SerializeField]
        [Tooltip("Transform parente des halos")]
        private Transform _haloParent;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par un allié")]
        private GameObject _haloAlly;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par aucun joueur")]
        private GameObject _haloNeutral;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par un ennemi")]
        private GameObject _haloEnemy;

        #endregion

        #region Instance

        /// <summary>
        /// Transform
        /// </summary>
        private Transform _t;

        /// <summary>
        /// Rigidbody
        /// </summary>
        private Rigidbody _rb;

        /// <summary>
        /// Le SphereCollider
        /// </summary>
        private SphereCollider _collisionCol;

        /// <summary>
        /// Le SphereCollider
        /// </summary>
        private SphereCollider _triggerCol;

        /// <summary>
        /// La vitesse de la balle à la frame précédente
        /// </summary>
        private Vector3 _lastLinearVelocity;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _t = transform;
            _rb = GetComponent<Rigidbody>();
            SphereCollider[] cols = GetComponents<SphereCollider>();
            _collisionCol = cols[0];
            _triggerCol = cols[1];
        }

        /// <summary>
        /// Appelée quand collision avec un autre objet
        /// </summary>
        /// <param name="collision">Infos sur la collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterEvent?.Invoke(this, collision);
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        /// <param name="ballState">Les données du ballon</param>
        internal void UpdateView(BallState ballState)
        {
            if (_rb.linearVelocity.sqrMagnitude < _displayHaloSpeedThreshold * _displayHaloSpeedThreshold &&
                _lastLinearVelocity.sqrMagnitude > _displayHaloSpeedThreshold * _displayHaloSpeedThreshold)
            {
                // Si la balle ralentit assez, on affiche son halo
                DisplayHalo(ballState);
            }
            if (_rb.linearVelocity.sqrMagnitude > _displayHaloSpeedThreshold * _displayHaloSpeedThreshold &&
                _lastLinearVelocity.sqrMagnitude < _displayHaloSpeedThreshold * _displayHaloSpeedThreshold)
            {
                // Si la balle n'est plus statique, on masque son halo
                HideHalo();
            }

            _lastLinearVelocity = _rb.linearVelocity;

            // Déplace manuellement les halos avec le ballon, vu que le ballon roule
            Vector3 y = _t.transform.position + new Vector3(0f, -_t.lossyScale.y / 2f, 0f);
            _haloParent.transform.position = y;
            _haloParent.transform.eulerAngles = Vector3.zero;
        }

        /// <summary>
        /// Réinitialise la balle pour la prochaine manche
        /// </summary>
        /// <param name="ballData">Les données du ballon</param>
        internal void ResetBall(BallState ballData)
        {
            DisplayHalo(ballData);

            _rb.linearVelocity = _rb.angularVelocity = Vector3.zero;
            _t.SetParent(null); // Si la balle est attachée à un joueur, on la libère
            EnablePhysics(true);
        }

        /// <summary>
        /// Change l'état de la balle pour indiquer qu'elle a été ramassée
        /// </summary>
        internal void SetAsPickedUp()
        {
            _t.localPosition = Vector3.zero;
            EnablePhysics(false);
            HideHalo();
        }

        /// <summary>
        /// Active ou non la physique sur cet objet
        /// </summary>
        internal void EnablePhysics(bool enable)
        {
            _rb.isKinematic = !enable;
            _rb.useGravity = enable;
            _collisionCol.enabled = enable;
            _triggerCol.enabled = enable;
        }

        /// <summary>
        /// Applique une impulsion au ballon
        /// </summary>
        internal void AddImpulseForce(Vector3 force)
        {
            _rb.AddForce(force, ForceMode.Impulse);
        }

        /// <summary>
        /// Applique une impulsion au ballon
        /// </summary>
        internal void AddImpulseForce(Vector3 direction, float force)
        {
            _rb.AddForce(direction * force, ForceMode.Impulse);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Affiche le halo de la balle en fonction de son équipe
        /// </summary>
        /// <param name="ballData">Les données du ballon</param>
        private void DisplayHalo(BallState ballData)
        {
            _haloAlly.SetActive(ballData.ReservedTeamID == TeamID.Ally && ballData.ActiveTeamID == TeamID.None && !ballData.IsLive);
            _haloNeutral.SetActive(ballData.ReservedTeamID == TeamID.None && ballData.ActiveTeamID == TeamID.None && !ballData.IsLive);
            _haloEnemy.SetActive(ballData.ReservedTeamID == TeamID.Enemy && ballData.ActiveTeamID == TeamID.None && !ballData.IsLive);
        }

        /// <summary>
        /// Masque le halo de la balle
        /// </summary>
        private void HideHalo()
        {
            _haloAlly.SetActive(false);
            _haloNeutral.SetActive(false);
            _haloEnemy.SetActive(false);
        }

        #endregion
    }
}