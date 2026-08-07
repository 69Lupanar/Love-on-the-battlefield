using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Affiche la barre d'énergie du joueur
    /// </summary>
    public class MatchPlayerEnergyUIView : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("L'objet contenant l'UI de la barre d'énergie")]
        private Transform _playerEnegyBarParent;

        [SerializeField]
        [Tooltip("L'image de la barre d'énergie")]
        private Image _energyBarImg;

        #endregion

        #region Instance

        /// <summary>
        /// Le MatchManagerView
        /// </summary>
        private MatchManagerView _matchManagerV;

        /// <summary>
        /// Le MatchPlayerControllerViewModel
        /// </summary>
        private MatchCharacterManagerView _playerControllerV;

        /// <summary>
        /// Perso contrôlé par le joueur
        /// </summary>
        private MatchCharacterControllerState _activeCharacterState;

        /// <summary>
        /// true si un perso est contrôlé par le joueur
        /// </summary>
        private bool _hasActiveCharacter;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// init
        /// </summary>
        private void Awake()
        {
            _matchManagerV = FindAnyObjectByType<MatchManagerView>();
            _playerControllerV = FindAnyObjectByType<MatchCharacterManagerView>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchManagerV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _playerControllerV.OnActivePlayerChanged += OnActivePlayerChanged;
            _playerEnegyBarParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDisable()
        {
            _matchManagerV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _playerControllerV.OnActivePlayerChanged -= OnActivePlayerChanged;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            bool showUI = _hasActiveCharacter && _activeCharacterState.Energy < 1f;
            _playerEnegyBarParent.gameObject.SetActive(showUI);

            if (showUI)
            {
                UpdateUI();
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelé quand le perso actif du joueur change
        /// </summary>
        private void OnNewMatchStarted(MatchSettingsData _)
        {
            GetActivePlayer();
        }

        /// <summary>
        /// Appelé quand le perso actif du joueur change
        /// </summary>
        private void OnActivePlayerChanged(int _)
        {
            GetActivePlayer();
        }

        /// <summary>
        /// Masque l'UI en vidant la variable du joueur
        /// </summary>
        public void HideUI()
        {
            _hasActiveCharacter = false;
            _playerEnegyBarParent.SetParent(null);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Obtient le perso contrôlé par le joueur
        /// </summary>
        private void GetActivePlayer()
        {
            _activeCharacterState = _playerControllerV.AllyStates[_playerControllerV.ActivePlayerIndex];
            _playerEnegyBarParent.SetParent(_playerControllerV.Allies[_playerControllerV.ActivePlayerIndex].transform);
            _playerEnegyBarParent.localPosition = Vector3.zero;
            _hasActiveCharacter = true;
        }

        /// <summary>
        /// Màj l'UI de la barre d'énergie
        /// </summary>
        private void UpdateUI()
        {
            _energyBarImg.fillAmount = _activeCharacterState.Energy;
        }

        #endregion
    }
}