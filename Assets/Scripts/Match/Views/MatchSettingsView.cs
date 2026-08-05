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
        [Tooltip("Paramètres d'un match")]
        private MatchSettingsData _matchSettings;

        #endregion

        #region Instance

        /// <summary>
        /// Le spawner des joueurs et ballons
        /// </summary>
        private MatchManagerView _managerV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _managerV = FindAnyObjectByType<MatchManagerView>();
            _nbAlliesField.SetTextWithoutNotify(_matchSettings.NbAllies.ToString());
            _nbEnemiesField.SetTextWithoutNotify(_matchSettings.NbEnemies.ToString());
            _nbBallsField.SetTextWithoutNotify(_matchSettings.NbBalls.ToString());
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbAlliesInputFieldEndEdit(string str)
        {
            _matchSettings.NbAllies = math.max(1, uint.Parse(str));
            _nbAlliesField.SetTextWithoutNotify(_matchSettings.NbAllies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbEnemiesInputFieldEndEdit(string str)
        {
            _matchSettings.NbEnemies = math.max(1, uint.Parse(str));
            _nbEnemiesField.SetTextWithoutNotify(_matchSettings.NbEnemies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbBallsInputFieldEndEdit(string str)
        {
            _matchSettings.NbBalls = math.max(1, uint.Parse(str));
            _nbBallsField.SetTextWithoutNotify(_matchSettings.NbBalls.ToString());
        }

        /// <summary>
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
            _managerV.StartNewMatch(_matchSettings);
            _managerV.StartNewSet();
        }

        #endregion
    }
}