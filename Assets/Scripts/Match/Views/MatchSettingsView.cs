using Assets.Scripts.Scenes;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère l'UI des paramètres du match
    /// </summary>
    public sealed class MatchSettingsView : MonoBehaviour
    {
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
        [Tooltip("La scène des matchs")]
        private SceneReference _matchScene;

        [SerializeField]
        [Tooltip("Paramètres d'un match")]
        private MatchSettingsData _matchSettings;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            UpdateUI();
        }

#if UNITY_EDITOR

        /// <summary>
        /// Quand l'inspecteur change
        /// </summary>
        private void OnValidate()
        {
            if (Application.isPlaying)
                UpdateUI();
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
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
            SceneLoader.LoadSceneAsync(_matchScene, () =>
            {
                MatchManagerView matchManager = FindAnyObjectByType<MatchManagerView>();

                if (matchManager != null)
                {
                    matchManager.StartNewMatch(_matchSettings);
                    matchManager.StartNewSet();
                }
            });
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Màj l'ui
        /// </summary>
        private void UpdateUI()
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