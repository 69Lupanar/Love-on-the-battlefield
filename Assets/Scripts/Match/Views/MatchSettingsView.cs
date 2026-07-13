using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère l'UI des paramètres du match
    /// </summary>
    public class MatchSettingsView : MonoBehaviour
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

        #endregion

        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchManagerViewModel _managerVM;

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
            _managerVM = FindAnyObjectByType<MatchManagerViewModel>();
            _managerV = FindAnyObjectByType<MatchManagerView>();
            _nbAlliesField.SetTextWithoutNotify(_managerVM.NbAllies.ToString());
            _nbEnemiesField.SetTextWithoutNotify(_managerVM.NbEnemies.ToString());
            _nbBallsField.SetTextWithoutNotify(_managerVM.NbBalls.ToString());
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbAlliesInputFieldEndEdit(string str)
        {
            _managerVM.NbAllies = math.max(1, int.Parse(str));
            _nbAlliesField.SetTextWithoutNotify(_managerVM.NbAllies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbEnemiesInputFieldEndEdit(string str)
        {
            _managerVM.NbEnemies = math.max(1, int.Parse(str));
            _nbEnemiesField.SetTextWithoutNotify(_managerVM.NbEnemies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbBallsInputFieldEndEdit(string str)
        {
            _managerVM.NbBalls = math.max(1, int.Parse(str));
            _nbBallsField.SetTextWithoutNotify(_managerVM.NbBalls.ToString());
        }

        /// <summary>
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
            _managerV.StartNewMatch();
        }

        #endregion
    }
}