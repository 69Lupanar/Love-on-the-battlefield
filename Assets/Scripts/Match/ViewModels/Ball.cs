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
        /// Une fois la balle récupérée, cette variable passe à -1 pour permettre à toutes les équipes de la ramasser.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        public int ReservedTeamID { get; private set; }

        /// <summary>
        /// Indique quelle équipe porte la balle.
        /// (-1 : Aucune équipe, 0 : Alliés, 1 : Ennemis)
        /// </summary>
        public int ActiveTeamID { get; set; }

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Vitesse de la balle en dessous de laquelle on affiche les halos")]
        private float _displayHaloSpeedThreshold = 1f;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par un allié")]
        private GameObject _haloAlly;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par aucun joueur")]
        private GameObject _haloNeutral;

        [SerializeField]
        [Tooltip("Halo de la balle si portée par un ennemi")]
        private GameObject _haloEnemy;

        [SerializeField]
        [Tooltip("Tags des surfaces rendant la balle inactive")]
        private string[] _obstacleTags;

        [SerializeField]
        [Tooltip("Tag de la zone hors-terrain")]
        private string _outOfFieldTag;

        [SerializeField]
        [Tooltip("Tag des persos")]
        private string _characterTag;

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
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_rb.linearVelocity.sqrMagnitude < _displayHaloSpeedThreshold * _displayHaloSpeedThreshold &&
                _lastLinearVelocity.sqrMagnitude > _displayHaloSpeedThreshold * _displayHaloSpeedThreshold)
            {
                // Si la balle ralentit assez, on affiche son halo
                DisplayHalo(true);
            }
            if (_rb.linearVelocity.sqrMagnitude > _displayHaloSpeedThreshold * _displayHaloSpeedThreshold &&
                _lastLinearVelocity.sqrMagnitude < _displayHaloSpeedThreshold * _displayHaloSpeedThreshold)
            {
                // Si la balle n'est plus statique, on masque son halo
                DisplayHalo(false);
            }

            _lastLinearVelocity = _rb.linearVelocity;
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

            // Désactive la balle si elle touche le sol ou un mur 
            for (int i = 0; i < _obstacleTags.Length; ++i)
            {
                if (go.CompareTag(_obstacleTags[i]))
                {
                    IsLive = false;
                    ActiveTeamID = -1;
                }
            }

            if (go.CompareTag(_outOfFieldTag))
            {
                //TAF : Ramaner la balle en jeu par les receveurs
                IsLive = false;
            }

            if (go.CompareTag(_characterTag) && IsLive)
            {
                if (IsLive)
                {
                    //TAF : Eliminer le joueur
                }
                else
                {
                    //TAF : Récupérer la balle (passe ou ramassage en fonction de ActiveTeamID)
                }
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Réinitialise la balle pour la prochaine manche
        /// </summary>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        internal void ResetBall(int index, int nbBalls)
        {
            IsLive = false;
            ActiveTeamID = -1;
            ReservedTeamID = GetBallTeamID(index, nbBalls);
            DisplayHalo(true);
            _rb.linearVelocity = _rb.angularVelocity = Vector3.zero;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Calcule l'ID d'équipe de la balle
        /// </summary>
        /// <param name="index">L'ordre d'instantiation de la balle sur le terrain. Permet de déterminer l'équipe à laquelle elle est réservée.</param>
        /// <param name="nbBalls">Nombre total de balles sur le terrain</param>
        private int GetBallTeamID(int index, int nbBalls)
        {
            // Selon les règles du dodgeball avec balles en tissu, il y a par défaut 5 balles ;
            // Les 2 balles les plus à gauche sont réservées à l'ennemi,
            // les 2 à droite sont aux alliés, celles au centre sont neutres.
            // Comme on peut changer le nombre de balles avant chaque match,
            // on essaye de calculer automatiquement le nb de balles à réserver à chaque équipe.

            if (nbBalls == 1)
                return -1;  // Neutre

            int nbReserved = Mathf.CeilToInt(nbBalls / 3f);

            if (index < nbReserved)
                return 1;   // Ennemi
            else if (index >= nbBalls - nbReserved)
                return 0;   // Allié
            else
                return -1; // Neutre
        }

        /// <summary>
        /// Affiche le halo de la balle en fonction de son équipe
        /// </summary>
        private void DisplayHalo(bool show)
        {
            _haloAlly.SetActive(show && ReservedTeamID == 0 && ActiveTeamID == -1 && !IsLive);
            _haloNeutral.SetActive(show && ReservedTeamID == -1 && ActiveTeamID == -1 && !IsLive);
            _haloEnemy.SetActive(show && ReservedTeamID == 1 && ActiveTeamID == -1 && !IsLive);
        }

        #endregion
    }
}