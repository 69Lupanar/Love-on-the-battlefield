using System;
using Assets.Scripts.Scenes;
using Assets.Scripts.Teams;
using Assets.Scripts.Utilities.Views;
using UnityEngine;

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
        [Tooltip("Préfab des labels glissables/déposables dans l'interface")]
        private GameObject _draggableLabelPrefab;

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
        [Tooltip("La scène de rotation des joueurs à la mi-temps")]
        private SceneReference _halfTimeSwapScene;

        #endregion

        #region Instance

        /// <summary>
        /// Le MatchHalfTimeSwapViewModel
        /// </summary>
        private MatchHalfTimeSwapViewModel _vm;

        /// <summary>
        /// Transform
        /// </summary>
        private Transform _t;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _t = transform;
            _vm = GetComponent<MatchHalfTimeSwapViewModel>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par le bouton de reprise du match
        /// </summary>
        public void OnResumeMatchBtnClick()
        {
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
                CreateDraggableLabel(allyTeamComposition.MainCharacters[i], _allyMainParent);
            }

            for (int i = 0; i < allyTeamComposition.Substitutes.Count; ++i)
            {
                CreateDraggableLabel(allyTeamComposition.Substitutes[i], _allySubstituteParent);
            }

            for (int i = 0; i < enemyTeamComposition.MainCharacters.Count; ++i)
            {
                CreateDraggableLabel(enemyTeamComposition.MainCharacters[i], _enemyMainParent);
            }

            for (int i = 0; i < enemyTeamComposition.Substitutes.Count; ++i)
            {
                CreateDraggableLabel(enemyTeamComposition.Substitutes[i], _enemySubstituteParent);
            }
        }

        #endregion

        #region Méthodes privées

        #region Callbacks

        /// <summary>
        /// Appelée quand on commence é glisser un label
        /// </summary>
        /// <param name="sender">L'objet</param>
        private void OnDragStarted(object sender, EventArgs _)
        {
            (sender as DraggableLabel).transform.SetParent(_draggedItemsRootParent);
        }

        /// <summary>
        /// Appelée quand on dépose un label
        /// </summary>
        /// <param name="sender">L'objet</param>
        private void OnDropped(object sender, EventArgs _)
        {
            DraggableLabel label = sender as DraggableLabel;

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
        private void CreateDraggableLabel(CharacterData character, RectTransform container)
        {
            DraggableLabel label;

            if (_t.childCount > 0)
            {
                Transform child = _t.GetChild(0);
                child.gameObject.SetActive(true);
                child.SetParent(container);
                label = child.GetComponent<DraggableLabel>();
            }
            else
            {
                label = Instantiate(_draggableLabelPrefab, container).GetComponent<DraggableLabel>();
                label.OnDragStartedEvent += OnDragStarted;
                label.OnDroppedEvent += OnDropped;
            }

            label.SetText(character.Name);
        }

        /// <summary>
        /// Fait tourner les membres de l'équipe ennemie.
        /// Le changement est fait aléatoirement par le jeu
        /// en s'adaptant à la formation du joueur
        /// ou en fonction de la progression dans l'histoire (à déterminer)
        /// </summary>
        private void SwapEnemies()
        {
            //TAF : Faire la rotation et afficher les changements dans l'UI

            _vm.SwapEnemies();
        }

        #endregion
    }
}