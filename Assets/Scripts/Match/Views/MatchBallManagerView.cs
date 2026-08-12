using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des ballons
    /// </summary>
    [RequireComponent(typeof(MatchBallManagerViewModel))]
    public class MatchBallManagerView : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<BallView> Balls { get; private set; } = new();

        /// <summary>
        /// Les données de l'état des ballons
        /// </summary>
        internal ReadOnlyCollection<BallState> BallStates => _vm.BallStates.AsReadOnly();

        #endregion

        #region Inspecteur

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
        /// Le ViewModel
        /// </summary>
        private MatchBallManagerViewModel _vm;

        /// <summary>
        /// Le MatchManagerView
        /// </summary>
        private MatchManagerView _matchV;

        /// <summary>
        /// Le MatchSpawnerView
        /// </summary>
        private MatchSpawnerView _spawnerV;

        /// <summary>
        /// Le MatchCharacterManagerView
        /// </summary>
        private MatchCharacterManagerView _characterManagerV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchBallManagerViewModel>();
            _matchV = FindAnyObjectByType<MatchManagerView>();
            _spawnerV = FindAnyObjectByType<MatchSpawnerView>();
            _characterManagerV = FindAnyObjectByType<MatchCharacterManagerView>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent += OnNewSetStarted;
            _characterManagerV.OnBallPickedUpEvent += OnBallPickedUp;
            _characterManagerV.OnShootEvent += OnShoot;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _matchV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _matchV.OnNewSetStartedEvent -= OnNewSetStarted;
            _characterManagerV.OnBallPickedUpEvent -= OnBallPickedUp;
            _characterManagerV.OnShootEvent -= OnShoot;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_matchV.MatchIsOver)
                return;

            for (int i = 0; i < Balls.Count; ++i)
            {
                Balls[i].UpdateView(_vm.BallStates[i]);
            }
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Attache les callbacks aux entités de jeu
        /// </summary>
        internal void SubscribeEntities()
        {
            for (int i = 0; i < Balls.Count; ++i)
            {
                BallView ball = Balls[i];
                ball.OnCollisionEnterEvent += OnBallCollisionEnter;
            }
        }

        /// <summary>
        /// Détache les callbacks aux entités de jeu
        /// </summary>
        internal void UnsubscribeEntities()
        {
            for (int i = 0; i < Balls.Count; ++i)
            {
                BallView ball = Balls[i];
                ball.OnCollisionEnterEvent -= OnBallCollisionEnter;
            }
        }

        /// <summary>
        /// Assigne les persos et ballons
        /// </summary>
        /// <param name="ballsT">Transforms des ballons</param>
        internal void SetEntities(List<Transform> ballsT)
        {
            _vm.SetEntities(ballsT.Count);

            Balls.Clear();

            for (int i = 0; i < ballsT.Count; ++i)
            {
                Balls.Add(ballsT[i].GetComponent<BallView>());
            }
        }

        /// <summary>
        /// Réinitialise les données du gestionnaire pour une nouvelle manche
        /// </summary>
        internal void ResetManager()
        {
            _vm.ResetManager();

            for (int i = 0; i < Balls.Count; ++i)
            {
                Balls[i].ResetBall(_vm.BallStates[i]);
            }
        }

        /// <summary>
        /// Indique si le ballon renseigné est actif
        /// </summary>
        /// <param name="ball">Le ballon</param>
        /// <returns>true si le ballon est actif</returns>
        internal bool GetIsBallLive(BallView ball)
        {
            return _vm.GetIsBallLive(Balls.IndexOf(ball));
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelée quand une nouvelle partie commence
        /// </summary>
        private void OnNewMatchStarted(MatchSettingsData _)
        {
            // Détache les callbacks des anciennes instances
            UnsubscribeEntities();

            SetEntities(_spawnerV.BallsT);
            SubscribeEntities();
        }

        /// <summary>
        /// Appelée quand une nouvelle manche commence
        /// </summary>
        private void OnNewSetStarted()
        {
            ResetManager();
        }

        /// <summary>
        /// Appelé quand le ballon heurte un objet
        /// </summary>
        /// <param name="sender">Le ballon</param>
        private void OnBallCollisionEnter(object sender, Collision collision)
        {
            int ballIndex = Balls.IndexOf(sender as BallView);
            GameObject go = collision.gameObject;

            // Désactive la balle si elle touche le sol ou un mur 
            for (int i = 0; i < _obstacleTags.Length; ++i)
            {
                if (go.CompareTag(_obstacleTags[i]))
                {
                    _vm.SetBallAsDead(ballIndex);
                }
            }

            if (go.CompareTag(_outOfFieldTag))
            {
                //TAF : Ramener la balle en jeu par les receveurs
            }
        }

        /// <summary>
        /// Appelé quand le ballon est ramassé par un joueur
        /// </summary>
        /// <param name="sender">Le ballon</param>
        /// <param name="e">Les données de l'événement</param>
        private void OnBallPickedUp(object _, BallPickedUpEventArgs e)
        {
            _vm.SetBallAsPickedUp(e.BallIndex, e.CharacterIndex, e.CharacterIsAlly);
            Balls[e.BallIndex].SetAsPickedUp();
        }

        /// <summary>
        /// Appelée quand un ballon est lancé par un joueur
        /// </summary>
        /// <param name="characterIndex">L'ID du tireur</param>
        private void OnShoot(object _, ShootEventArgs e)
        {
            _vm.SetBallAsLive(e.BallIndex);
        }

        #endregion
    }
}