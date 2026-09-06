using Assets.Scripts.Scenes;
using Assets.Scripts.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    [RequireComponent(typeof(MatchHalfTimeSwapViewModel))]
    public sealed class MatchHalfTimeSwapView : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Préfab des boutons des persos dans l'interface")]
        private GameObject _swapCharacterBtnPrefab;

        [SerializeField]
        [Tooltip("Label du message d'erreur")]
        private TextMeshProUGUI _errorMsgLabel;

        [SerializeField]
        [Tooltip("Le canvas parent")]
        private RectTransform _draggedItemsRootParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs ppaux")]
        private RectTransform _allyMainParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs remplaçants")]
        private RectTransform _allySubstituteParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs ppaux")]
        private RectTransform _enemyMainParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs remplaçants")]
        private RectTransform _enemySubstituteParent;

        [SerializeField]
        [Tooltip("La liste des parents, regroupés pour la détection du rect du label à glisser")]
        private RectTransform[] _containersRectTransforms;

        [SerializeField]
        [Tooltip("La scène de rotation des joueurs à la mi-temps")]
        private SceneReference _halfTimeSwapScene;

        #endregion

        #region Instance

        /// <summary>
        /// Le MatchHalfTimeSwapViewModel
        /// </summary>
        private MatchHalfTimeSwapViewModel _vm;

        /// <summary>
        /// Le MatchManagerViewModel
        /// </summary>
        private MatchManagerViewModel _matchManagerVM;

        /// <summary>
        /// Transform
        /// </summary>
        private Transform _t;

        /// <summary>
        /// Les IDs des boutons sélectionnés
        /// </summary>
        private int _firstIndex = -1, _secondIndex = -1;

        /// <summary>
        /// Le RectTransform où placer le bouton
        /// </summary>
        private RectTransform _sourceRT;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _t = transform;
            _vm = GetComponent<MatchHalfTimeSwapViewModel>();
            _matchManagerVM = FindAnyObjectByType<MatchManagerViewModel>();
        }

        /// <summary>
        /// init
        /// </summary>
        private void Start()
        {
            _errorMsgLabel.enabled = false;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par le bouton de reprise du match
        /// </summary>
        public void OnResumeMatchBtnClick()
        {
            // Vérifie que les équipes sont valides avant de retourner en jeu
            int error = _vm.CheckTeamCompositions(_matchManagerVM.NbAllies, _matchManagerVM.NbEnemies);

            if (error == -1)
            {
                _errorMsgLabel.enabled = false;

                SwapEnemies();

                SceneLoader.UnloadSceneAsync(_halfTimeSwapScene, () =>
                {
                    // Une fois la scène déchargée, on réassigne les joueurs actifs
                    // et on démarre une nouvelle manche
                    MatchManagerView matchManager = FindAnyObjectByType<MatchManagerView>();

                    if (matchManager != null)
                    {
                        matchManager.ResumeMatchAfterHalfTime();
                    }
                });
            }
            else
            {
                string errMsg = MatchConstants.ERROR_MESSAGES[error];
                Debug.LogError(error);
                _errorMsgLabel.SetText(errMsg);
                _errorMsgLabel.enabled = true;
            }
        }

        /// <summary>
        /// Appelée par le bouton de rotation des ennemis
        /// </summary>
        public void OnSwapEnemiesBtn()
        {
            SwapEnemies();
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        /// <param name="enemyTeamComposition">Ennemis</param>
        /// <param name="enforceStrictCountLimit">true si les équipes doivent avoir un nombre exact de joueurs actifs</param>
        internal void SetTeams(TeamCompositionData allyTeamComposition, TeamCompositionData enemyTeamComposition, bool enforceStrictCountLimit = true)
        {
            _vm.SetTeams(allyTeamComposition, enemyTeamComposition, enforceStrictCountLimit);

            Clear(_allyMainParent);
            Clear(_allySubstituteParent);
            Clear(_enemyMainParent);
            Clear(_enemySubstituteParent);

            for (int i = 0; i < allyTeamComposition.MainCharacters.Count; ++i)
            {
                CreateSwapCharacterBtn(allyTeamComposition.MainCharacters[i], _allyMainParent);
            }

            for (int i = 0; i < allyTeamComposition.Substitutes.Count; ++i)
            {
                CreateSwapCharacterBtn(allyTeamComposition.Substitutes[i], _allySubstituteParent);
            }

            for (int i = 0; i < enemyTeamComposition.MainCharacters.Count; ++i)
            {
                CreateSwapCharacterBtn(enemyTeamComposition.MainCharacters[i], _enemyMainParent);
            }

            for (int i = 0; i < enemyTeamComposition.Substitutes.Count; ++i)
            {
                CreateSwapCharacterBtn(enemyTeamComposition.Substitutes[i], _enemySubstituteParent);
            }
        }

        #endregion

        #region Méthodes privées

        #region Callbacks

        /// <summary>
        /// Appelée quand on clique sur les boutons des persos
        /// </summary>
        /// <param name="sender">Le bouton sur lequel on clique</param>
        private void OnSwapCharacterBtnClick(Button sender)
        {
            if (_firstIndex == -1)
            {
                // Sélectionne le 1er perso
                _firstIndex = sender.transform.GetSiblingIndex();
                _sourceRT = sender.transform.parent as RectTransform;
                sender.OnSelect(null);
            }
            else
            {
                // Sélectionne le 2è perso et lance l'échange
                _secondIndex = sender.transform.GetSiblingIndex();
                RectTransform targetRT = sender.transform.parent as RectTransform;

                if (_sourceRT != targetRT || _firstIndex != _secondIndex)
                {
                    SwapCharacters(_sourceRT, targetRT, _firstIndex, _secondIndex);
                }

                _sourceRT.GetChild(_firstIndex).GetComponent<Button>().OnDeselect(null);
                _firstIndex = -1;
                _secondIndex = -1;
            }
        }

        #endregion

        /// <summary>
        /// Vide le conteneur de ses enfants
        /// </summary>
        /// <param name="container">Le conteneur</param>
        private void Clear(RectTransform container)
        {
            while (container.childCount > 0)
            {
                Transform child = container.GetChild(0);
                child.gameObject.SetActive(false);
                child.SetParent(_t);
            }
        }

        /// <summary>
        /// Crée un label pour le joueur renseigné
        /// </summary>
        /// <param name="character">Le perso</param>
        /// <param name="container">Conteneur parent</param>
        private void CreateSwapCharacterBtn(CharacterData character, RectTransform container)
        {
            Button btn;

            if (_t.childCount > 0)
            {
                Transform child = _t.GetChild(0);
                child.gameObject.SetActive(true);
                child.SetParent(container);
                btn = child.GetComponent<Button>();
            }
            else
            {
                btn = Instantiate(_swapCharacterBtnPrefab, container).GetComponent<Button>();
                btn.onClick.AddListener(() => OnSwapCharacterBtnClick(btn));
            }

            btn.GetComponentInChildren<TextMeshProUGUI>().SetText(character.Name);
        }

        /// <summary>
        /// Echange les boutons de place
        /// </summary>
        /// <param name="sourceRT">Le parent du label source</param>
        /// <param name="targetRT">Le parent du label cible</param>
        /// <param name="firstIndex">Le label déplacé par le joueur</param>
        /// <param name="secondIndex">Le label à échanger</param>
        private void SwapCharacters(RectTransform sourceRT, RectTransform targetRT, int firstIndex, int secondIndex)
        {
            int oldListIndex = sourceRT == _allyMainParent ? 0 :
                               sourceRT == _allySubstituteParent ? 1 :
                               sourceRT == _enemyMainParent ? 2 :
                               sourceRT == _enemySubstituteParent ? 3 :
                               -1;

            int newListIndex = targetRT == _allyMainParent ? 0 :
                               targetRT == _allySubstituteParent ? 1 :
                               targetRT == _enemyMainParent ? 2 :
                               targetRT == _enemySubstituteParent ? 3 :
                               -1;

            _vm.SwapCharacters(oldListIndex, newListIndex, firstIndex, secondIndex);

            Transform firstBtn = sourceRT.GetChild(firstIndex);
            Transform secondBtn = targetRT.GetChild(secondIndex);

            firstBtn.SetParent(targetRT);
            firstBtn.SetSiblingIndex(secondIndex);
            secondBtn.SetParent(sourceRT);
            secondBtn.SetSiblingIndex(firstIndex);
        }

        /// <summary>
        /// Fait tourner les membres de l'équipe ennemie.
        /// Le changement est fait aléatoirement par le jeu
        /// en s'adaptant à la formation du joueur
        /// ou en fonction de la progression dans l'histoire (à déterminer)
        /// </summary>
        private void SwapEnemies()
        {
            // On récupère uniquement les indices des persos à tourner.
            // Pour l'instant, la sélection est purement aléatoire,
            // mais on pourra à l'avenir choisir les persos
            // en fonction de la formation du joueur
            // ou de la progression dans l'histoire
            // pour choisir des joueurs spécifiques.

            _vm.SelectEnemiesToSwap(out int[] mainIndices, out int[] substituteIndices);

            for (int i = 0; i < mainIndices.Length; ++i)
            {
                SwapCharacters(_enemyMainParent, _enemySubstituteParent, mainIndices[i], substituteIndices[i]);
            }
        }

        #endregion
    }
}