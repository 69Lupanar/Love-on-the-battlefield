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
        private MatchCharacterManagerView _playerManagerV;

        /// <summary>
        /// true si un perso est contrôlé par le joueur
        /// </summary>
        private int _activeCharacterIndex = -1;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// init
        /// </summary>
        private void Awake()
        {
            _matchManagerV = FindAnyObjectByType<MatchManagerView>();
            _playerManagerV = FindAnyObjectByType<MatchCharacterManagerView>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _matchManagerV.OnNewMatchStartedEvent += OnNewMatchStarted;
            _playerManagerV.OnActivePlayerChanged += OnActivePlayerChanged;
            _playerEnegyBarParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDisable()
        {
            _matchManagerV.OnNewMatchStartedEvent -= OnNewMatchStarted;
            _playerManagerV.OnActivePlayerChanged -= OnActivePlayerChanged;
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            bool showUI = _activeCharacterIndex > -1 && _playerManagerV.AllyStates[_activeCharacterIndex].Energy < 1f;
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
        private void OnNewMatchStarted(object _, NewMatchStartedEventArgs e)
        {
            _activeCharacterIndex = 0;
            GetActivePlayer(0);
        }

        /// <summary>
        /// Appelé quand le perso actif du joueur change
        /// </summary>
        /// <param name="activeCharacterIndex">L'ID du perso actif</param>
        private void OnActivePlayerChanged(object _, int activeCharacterIndex)
        {
            _activeCharacterIndex = activeCharacterIndex;
            GetActivePlayer(activeCharacterIndex);
        }

        /// <summary>
        /// Masque l'UI en vidant la variable du joueur
        /// </summary>
        public void HideUI()
        {
            _activeCharacterIndex = -1;
            _playerEnegyBarParent.SetParent(null);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Obtient le perso contrôlé par le joueur
        /// </summary>
        /// <param name="activePlayerIndex">L'ID du perso actif</param>
        private void GetActivePlayer(int activePlayerIndex)
        {
            _playerEnegyBarParent.SetParent(_playerManagerV.Allies[activePlayerIndex].transform);
            _playerEnegyBarParent.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Màj l'UI de la barre d'énergie
        /// </summary>
        private void UpdateUI()
        {
            _energyBarImg.fillAmount = _playerManagerV.AllyStates[_activeCharacterIndex].Energy;
        }

        #endregion
    }
}