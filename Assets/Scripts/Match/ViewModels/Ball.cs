using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Logique du ballon
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Ball : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si le ballon est actif
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// Indique l'équipe à laquelle la balle est réservée.
        /// Utilisé au début du match avant lorsque les joueurs partent récupérer la balle.
        /// (0 : Aucune équipe, 1 : Alliés, 2 : Ennemis)
        /// </summary>
        public bool ReservedTeamID { get; set; }

        /// <summary>
        /// Indique quelle équipe porte la balle.
        /// Si à -1, aucune équipe ne porte la balle
        /// </summary>
        public int ActiveTeamID { get; set; }

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Tags des surfaces rendant la balle inactive")]
        private string[] _obstacleTags;

        [SerializeField]
        [Tooltip("Tag de la zone hors-terrain")]
        private string _outOfFieldTag;

        [SerializeField]
        [Tooltip("Tag des joueurs")]
        private string _playerTag;

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

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _t = transform;
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Appelée quand collision avec un autre objet
        /// </summary>
        /// <param name="collision">Infos sur la collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (ActiveTeamID == -1)
                return;

            GameObject go = collision.gameObject;

            for (int i = 0; i < _obstacleTags.Length; ++i)
            {
                if (go.CompareTag(_obstacleTags[i]))
                {
                    IsLive = false;
                }
            }

            if (go.CompareTag(_outOfFieldTag))
            {
                //TAF : Ramaner la balle en jeu par les receveurs
                IsLive = false;
            }

            if (go.CompareTag(_playerTag) && IsLive)
            {
                //TAF : Eliminer le joueur
            }

        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Réinitialise la balle pour la prochaine manche
        /// </summary>
        internal void ResetBall()
        {
            _rb.linearVelocity = _rb.angularVelocity = Vector3.zero;
        }

        #endregion
    }
}