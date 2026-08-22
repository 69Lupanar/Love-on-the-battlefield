using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using Assets.Scripts.Scenes;
using Assets.Scripts.Teams;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère l'UI des paramètres du match
    /// </summary>
    public sealed class MatchSettingsView : MonoBehaviour
    {
        #region Evénements

        /// <summary>
        /// Appelée quand on change d'équipe alliée
        /// </summary>
        public EventHandler<TeamRosterSO> OnAllyTeamChangedEvent { get; set; }

        /// <summary>
        /// Appelée quand on change d'équipe ennemie
        /// </summary>
        public EventHandler<TeamRosterSO> OnEnemyTeamChangedEvent { get; set; }

        /// <summary>
        /// Appelée quand on récupère les équipes et persos
        /// </summary>
        public EventHandler OnCustomMatchUnlockablesLoadedEvent { get; set; }

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("InputField du nb d'alliés")]
        private TMP_InputField _nbAlliesField;

        [SerializeField]
        [Tooltip("InputField du nb d'ennemis")]
        private TMP_InputField _nbEnemiesField;

        [SerializeField]
        [Tooltip("InputField du nb de ballons")]
        private TMP_InputField _nbBallsField;

        [SerializeField]
        [Tooltip("InputField de la durée d'une partie")]
        private TMP_InputField _halfDurationField;

        [SerializeField]
        [Tooltip("InputField de la durée d'une manche")]
        private TMP_InputField _setDurationField;

        [SerializeField]
        [Tooltip("Dropdown pour choisir l'équipe alliée")]
        private TMP_Dropdown _allyTeamField;

        [SerializeField]
        [Tooltip("Dropdown pour choisir l'équipe ennemie")]
        private TMP_Dropdown _enemyTeamField;

        [SerializeField]
        [Tooltip("La scène des matchs")]
        private SceneReference _matchScene;

        [SerializeField]
        [Tooltip("Paramètres d'un match")]
        private MatchSettingsData _matchSettings;

        #endregion

        #region Instance

        /// <summary>
        /// Equipe par défaut pour les alliés
        /// </summary>
        private TeamRosterSO _allyTeam;

        /// <summary>
        /// Equipe par défaut pour les ennemis
        /// </summary>
        private TeamRosterSO _enemyTeam;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            GetCustomMatchUnlockables();
            UpdateDropdowns();
            UpdateInputFields();
        }

#if UNITY_EDITOR

        /// <summary>
        /// Quand l'inspecteur change
        /// </summary>
        private void OnValidate()
        {
            if (Application.isPlaying)
                UpdateInputFields();
        }

#endif

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbAlliesInputFieldEndEdit(string str)
        {
            _matchSettings.NbAllies = math.max(1, int.Parse(str));
            _nbAlliesField.SetTextWithoutNotify(_matchSettings.NbAllies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbEnemiesInputFieldEndEdit(string str)
        {
            _matchSettings.NbEnemies = math.max(1, int.Parse(str));
            _nbEnemiesField.SetTextWithoutNotify(_matchSettings.NbEnemies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbBallsInputFieldEndEdit(string str)
        {
            _matchSettings.NbBalls = math.max(1, int.Parse(str));
            _nbBallsField.SetTextWithoutNotify(_matchSettings.NbBalls.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnHalfDurationInputFieldEndEdit(string str)
        {
            _matchSettings.HalfDuration = math.max(2, int.Parse(str));
            _halfDurationField.SetTextWithoutNotify(_matchSettings.HalfDuration.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnSetDurationInputFieldEndEdit(string str)
        {
            _matchSettings.SetDuration = math.max(2, int.Parse(str));
            _setDurationField.SetTextWithoutNotify(_matchSettings.SetDuration.ToString());
        }

        /// <summary>
        /// Appelée par le dropdown
        /// </summary>
        public void OnAllyTeamDropdownValueChanged(int index)
        {
            _allyTeam = CustomMatchModeUnlockables.Teams[index];
            OnAllyTeamChangedEvent?.Invoke(this, _allyTeam);
        }

        /// <summary>
        /// Appelée par le dropdown
        /// </summary>
        public void OnEnemyTeamDropdownValueChanged(int index)
        {
            _enemyTeam = CustomMatchModeUnlockables.Teams[index];
            OnEnemyTeamChangedEvent?.Invoke(this, _enemyTeam);
        }

        /// <summary>
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
            SceneLoader.LoadSceneAsync(_matchScene, LoadSceneMode.Single, () =>
            {
                MatchManagerView matchManager = FindAnyObjectByType<MatchManagerView>();

                if (matchManager != null)
                {
                    matchManager.StartNewMatch(_matchSettings, _allyTeam, _enemyTeam);
                    matchManager.StartNewSet();
                }
            });
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Obtient les joueurs et équipes débloqués pour le mode Custom Match
        /// </summary>
        private void GetCustomMatchUnlockables()
        {
            CustomMatchModeUnlockables.Teams.Clear();
            CustomMatchModeUnlockables.Characters.Clear();

            // Equipes débloquées

            CustomMatchModeUnlockables.Teams.AddRange(CustomMatchModeUnlockables.GetRostersUnlockedInCustomMatch());

            // Persos débloqués

            CustomMatchModeUnlockables.Characters.AddRange(CustomMatchModeUnlockables.GetCharactersUnlockedInCustomMatch());

            OnCustomMatchUnlockablesLoadedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Màj les dropdowns
        /// </summary>
        private void UpdateDropdowns()
        {
            List<TMP_Dropdown.OptionData> options = CustomMatchModeUnlockables.Teams.Select(roster => new TMP_Dropdown.OptionData(roster.Team.Data.Name, roster.Team.Data.LogoSprite, roster.Team.Data.Color)).ToList();
            _allyTeamField.AddOptions(options);
            _enemyTeamField.AddOptions(options);

            // Va automatiquement assigner les équipes pour nous
            _allyTeamField.value = 0;
            _enemyTeamField.value = 0;
        }

        /// <summary>
        /// Màj les InputFields
        /// </summary>
        private void UpdateInputFields()
        {
            _nbAlliesField.SetTextWithoutNotify(_matchSettings.NbAllies.ToString());
            _nbEnemiesField.SetTextWithoutNotify(_matchSettings.NbEnemies.ToString());
            _nbBallsField.SetTextWithoutNotify(_matchSettings.NbBalls.ToString());
            _halfDurationField.SetTextWithoutNotify(_matchSettings.HalfDuration.ToString());
            _setDurationField.SetTextWithoutNotify(_matchSettings.SetDuration.ToString());
        }

        #endregion
    }
}