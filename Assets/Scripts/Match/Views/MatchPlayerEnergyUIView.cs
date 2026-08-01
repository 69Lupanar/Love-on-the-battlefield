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
        private MatchPlayerControllerViewModel _playerControllerVM;

        /// <summary>
        /// Perso contrôlé par le joueur
        /// </summary>
        private MatchCharacterController _activeCharacter;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// init
        /// </summary>
        private void Awake()
        {
            _matchManagerV = FindAnyObjectByType<MatchManagerView>();
            _playerControllerVM = FindAnyObjectByType<MatchPlayerControllerViewModel>();
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _playerControllerVM.OnActivePlayerChanged += OnActivePlayerChanged;
            _playerEnegyBarParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            bool showUI = _activeCharacter != null && _activeCharacter.Energy < 1f;
            _playerEnegyBarParent.gameObject.SetActive(showUI);

            if (showUI)
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        private void OnDisable()
        {
            _playerControllerVM.OnActivePlayerChanged += OnActivePlayerChanged;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Obtient le perso contrôlé par le joueur
        /// </summary>
        public void GetActivePlayer()
        {
            _activeCharacter = _playerControllerVM.Allies[_playerControllerVM.ActivePlayerIndex];
            _playerEnegyBarParent.SetParent(_activeCharacter.transform);
            _playerEnegyBarParent.localPosition = Vector3.zero;
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
            _activeCharacter = null;
            _playerEnegyBarParent.SetParent(null);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Màj l'UI de la barre d'énergie
        /// </summary>
        private void UpdateUI()
        {
            _energyBarImg.fillAmount = _activeCharacter.Energy;
        }

        #endregion
    }
}