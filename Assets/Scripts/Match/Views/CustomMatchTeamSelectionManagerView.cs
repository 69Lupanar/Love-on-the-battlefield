using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using Assets.Scripts.Teams;
using Assets.Scripts.Utilities.Views;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    [RequireComponent(typeof(CustomMatchTeamSelectionManagerViewModel))]
    public sealed class CustomMatchTeamSelectionManagerView : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Si true, les équipes doivent avoir un nombre exact de joueurs ppaux. Si false, elles peuvent en avoir moins (min. 1)")]
        private bool _enforceStrictCharacterLimit = false;

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
        [Tooltip("Parent des noms des joueurs en réserve")]
        private RectTransform _reserveParent;

        [SerializeField]
        [Tooltip("Label des messages d'erreur")]
        private TextMeshProUGUI _errorMsgLabel;

        #endregion

        #region Instance

        /// <summary>
        /// Le CustomMatchTeamSelectionManagerViewModel
        /// </summary>
        private CustomMatchTeamSelectionManagerViewModel _vm;

        /// <summary>
        /// Le MatchSettingsView
        /// </summary>
        private MatchSettingsView _settingsV;

        /// <summary>
        /// Transform
        /// </summary>
        private Transform _t;

        /// <summary>
        /// L'équipe alliée
        /// </summary>
        private TeamRosterSO _allyTeam;

        /// <summary>
        /// L'équipe ennemie
        /// </summary>
        private TeamRosterSO _enemyTeam;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _t = transform;
            _vm = GetComponent<CustomMatchTeamSelectionManagerViewModel>();
            _settingsV = FindAnyObjectByType<MatchSettingsView>();

            _settingsV.OnAllyTeamChangedEvent += OnAllyTeamChanged;
            _settingsV.OnEnemyTeamChangedEvent += OnEnemyTeamChanged;
            _settingsV.OnCustomMatchUnlockablesLoadedEvent += OnCustomMatchUnlockablesLoaded;
        }

        /// <summary>
        /// init
        /// </summary>
        private void Start()
        {
            _errorMsgLabel.SetText(string.Empty);
        }

        /// <summary>
        /// nettoyage
        /// </summary>
        private void OnDestroy()
        {
            _settingsV.OnAllyTeamChangedEvent -= OnAllyTeamChanged;
            _settingsV.OnEnemyTeamChangedEvent -= OnEnemyTeamChanged;
            _settingsV.OnCustomMatchUnlockablesLoadedEvent -= OnCustomMatchUnlockablesLoaded;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        internal void SetAllyTeam(TeamCompositionData allyTeamComposition)
        {
            _vm.SetAllyTeam(allyTeamComposition);

            Clear(_allyMainParent);
            Clear(_allySubstituteParent);

            for (int i = 0; i < allyTeamComposition.MainCharacters.Count; ++i)
            {
                CreateDraggableLabel(allyTeamComposition.MainCharacters[i], _allyMainParent);
            }

            for (int i = 0; i < allyTeamComposition.Substitutes.Count; ++i)
            {
                CreateDraggableLabel(allyTeamComposition.Substitutes[i], _allySubstituteParent);
            }
        }

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="enemyTeamComposition">Ennemis</param>
        internal void SetEnemyTeam(TeamCompositionData enemyTeamComposition)
        {
            _vm.SetEnemyTeam(enemyTeamComposition);

            Clear(_enemyMainParent);
            Clear(_enemySubstituteParent);

            for (int i = 0; i < enemyTeamComposition.MainCharacters.Count; ++i)
            {
                CreateDraggableLabel(enemyTeamComposition.MainCharacters[i], _enemyMainParent);
            }

            for (int i = 0; i < enemyTeamComposition.Substitutes.Count; ++i)
            {
                CreateDraggableLabel(enemyTeamComposition.Substitutes[i], _enemySubstituteParent);
            }
        }

        /// <summary>
        /// Assigne les personnages en réserve
        /// </summary>
        /// <param name="characters">Les personnages en réserve</param>
        internal void SetReserveCharacters(List<CharacterSO> characters)
        {
            _vm.SetReserveCharacters(characters.Select(character => character.Data));

            Clear(_reserveParent);

            foreach (CharacterData character in _vm.Reserve)
            {
                CreateDraggableLabel(character, _reserveParent);
            }
        }

        #endregion

        #region Méthodes privées

        #region Callbacks

        /// <summary>
        /// Appelée quand on change d'équipe alliée
        /// </summary>
        private void OnAllyTeamChanged(object _, TeamRosterSO teamComposition)
        {
            SetAllyTeam(teamComposition.CompositionData);
        }

        /// <summary>
        /// Appelée quand on change d'équipe ennemie
        /// </summary>
        private void OnEnemyTeamChanged(object _, TeamRosterSO teamComposition)
        {
            SetEnemyTeam(teamComposition.CompositionData);
        }

        /// <summary>
        /// Appelée quand les assets sont chargées
        /// </summary>
        private void OnCustomMatchUnlockablesLoaded(object sender, EventArgs e)
        {
            SetReserveCharacters(CustomMatchModeUnlockables.Characters);
        }

        /// <summary>
        /// Appelée quand on commence à glisser un label
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

            if (label.LastParent == _reserveParent)
            {
                // Si le label vient de la réserve
            }
            else
            {
                // Si le label vient d'une des équipes

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

        #endregion
    }
}